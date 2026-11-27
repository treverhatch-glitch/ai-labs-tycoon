"""Walk cycle: materials, limb integrity, floor contact, arm drop, leg alternation."""
from __future__ import annotations

import bpy

MIN_LIMB_SPAN_M = 0.12
LIMB_SPAN_DRIFT_M = 0.45
FOOT_FLOOR_MAX_M = 0.16
FOOT_PASSING_MAX_M = 0.42
ARM_ABOVE_SHOULDER_MAX_M = 0.12
KNEE_ALT_MIN_M = 0.04

LIMB_SPANS = (
    ("LeftShoulder", "LeftHand"),
    ("RightShoulder", "RightHand"),
    ("LeftUpLeg", "LeftFoot"),
    ("RightUpLeg", "RightFoot"),
)

FOUNDER_MATS = frozenset({"FounderTee", "FounderJeans", "FounderSkin"})
RESEARCHER_MATS = frozenset({"ResearcherTee", "ResearcherLabCoat", "ResearcherTrousers"})


def _bone_world(arm, name: str):
    pb = arm.pose.bones.get(name)
    if not pb:
        return None
    return arm.matrix_world @ pb.head


def _bone_span(arm, a: str, b: str) -> float | None:
    pa = arm.pose.bones.get(a)
    pb = arm.pose.bones.get(b)
    if not pa or not pb:
        return None
    wa = arm.matrix_world @ pa.head
    wb = arm.matrix_world @ pb.head
    return (wa - wb).length


def _frame_update(scene, frame: int) -> None:
    scene.frame_set(frame)
    bpy.context.view_layer.update()


def _min_foot_z(arm) -> float:
    zs = []
    for name in ("LeftFoot", "RightFoot", "LeftToes", "RightToes"):
        w = _bone_world(arm, name)
        if w:
            zs.append(w.z)
    return min(zs) if zs else 1e9


def _stance_foot_z(arm, side: str) -> float:
    zs = []
    for name in (f"{side}Foot", f"{side}Toes"):
        w = _bone_world(arm, name)
        if w:
            zs.append(w.z)
    return min(zs) if zs else 1e9


def _knee_height(arm, side: str) -> float | None:
    w = _bone_world(arm, f"{side}Leg")
    return w.z if w else None


def _arms_below_horizontal(arm) -> tuple[bool, str]:
    for side in ("Left", "Right"):
        sh = _bone_world(arm, f"{side}Shoulder")
        hand = _bone_world(arm, f"{side}Hand")
        if not sh or not hand:
            return False, f"arm_bones_missing_{side}"
        if hand.z > sh.z + ARM_ABOVE_SHOULDER_MAX_M:
            return False, f"arm_high_{side} dz={hand.z - sh.z:.3f}m"
    return True, "arms_down"


def _hero_materials_ok(body, asset_id: str) -> tuple[bool, str]:
    if not body or body.type != "MESH":
        return False, "walk_body_missing"
    names = {m.name.split(".")[0] for m in body.data.materials if m}
    if asset_id == "staff-founder-idle":
        if len(names) < 6 or not (names & FOUNDER_MATS):
            return False, f"walk_materials_founder={sorted(names)}"
    elif asset_id == "staff-researcher-idle":
        if len(names) < 6 or not (names & RESEARCHER_MATS):
            return False, f"walk_materials_researcher={sorted(names)}"
    grey = sum(1 for n in names if n in ("Material", "Kenney", "characterMedium"))
    if grey >= len(names) - 1:
        return False, "walk_materials_grey_base"
    return True, f"mats={len(names)}"


def _pose_walk_frame(scene, arm, frame: int, idle_arm_quats) -> None:
    scene.frame_set(frame)
    if idle_arm_quats:
        from kenney_hero_walk_strip import apply_idle_arm_blend

        apply_idle_arm_blend(arm, idle_arm_quats)
    else:
        bpy.context.view_layer.update()


def verify_walk_cycle(
    arm,
    mesh,
    *,
    frame_a: int,
    frame_b: int,
    scene,
    quarter_frames: list[int] | None = None,
    asset_id: str = "",
    render_meshes: list | None = None,
    idle_arm_quats=None,
) -> tuple[bool, str]:
    _ = render_meshes
    samples = quarter_frames or [int(frame_a), int(frame_b)]
    ok_m, msg_m = _hero_materials_ok(mesh, asset_id or "staff-founder-idle")
    if not ok_m:
        return False, msg_m
    span_lists: list[list[float]] = []
    foot_heights: list[float] = []
    knee_lr: list[tuple[float, float]] = []
    cycle_min = 1e9
    for fr in range(int(min(samples)), int(max(samples)) + 1):
        _pose_walk_frame(scene, arm, fr, idle_arm_quats)
        cycle_min = min(cycle_min, _min_foot_z(arm))
    for idx, fr in enumerate(samples):
        _pose_walk_frame(scene, arm, fr, idle_arm_quats)
        ok_m2, msg_m2 = _hero_materials_ok(mesh, asset_id or "staff-founder-idle")
        if not ok_m2:
            return False, f"frame={fr} {msg_m2}"
        ok_arm, msg_arm = _arms_below_horizontal(arm)
        if not ok_arm:
            return False, f"frame={fr} walk_{msg_arm}"
        passing = idx in (1, 3)
        if passing:
            low = _min_foot_z(arm)
            high = max(_stance_foot_z(arm, "Left"), _stance_foot_z(arm, "Right"))
            if low > cycle_min + FOOT_FLOOR_MAX_M:
                return False, f"walk_floating passing frame={fr} stance_z={low:.4f}m"
            if high - low < KNEE_ALT_MIN_M:
                return False, f"walk_passing_flat frame={fr} spread={high - low:.4f}m"
            if high > FOOT_PASSING_MAX_M:
                return False, f"walk_passing_too_high frame={fr} swing_z={high:.4f}m"
            fz = low
        else:
            stance = "Left" if idx == 0 else "Right"
            fz = _stance_foot_z(arm, stance)
            if fz > cycle_min + FOOT_FLOOR_MAX_M:
                return False, f"walk_floating contact_{stance} frame={fr} foot_z={fz:.4f}m"
        foot_heights.append(fz)
        spans = [_bone_span(arm, a, b) for a, b in LIMB_SPANS]
        if any(s is None for s in spans):
            return False, f"walk_limb_bones_missing frame={fr}"
        good = [s for s in spans if s is not None]
        if min(good) < MIN_LIMB_SPAN_M:
            return False, f"walk_limb_detach frame={fr}"
        span_lists.append(good)
        lk = _knee_height(arm, "Left")
        rk = _knee_height(arm, "Right")
        if lk is None or rk is None:
            return False, f"walk_knee_missing frame={fr}"
        knee_lr.append((lk, rk))
    ref = span_lists[0]
    worst_drift = 0.0
    for spans in span_lists[1:]:
        for a, b in zip(ref, spans, strict=False):
            worst_drift = max(worst_drift, abs(a - b))
    if worst_drift > LIMB_SPAN_DRIFT_M:
        return False, f"walk_limb_tear drift={worst_drift:.4f}m"
    if len(knee_lr) >= 2:
        d0 = abs(knee_lr[0][0] - knee_lr[0][1])
        d2 = abs(knee_lr[min(2, len(knee_lr) - 1)][0] - knee_lr[min(2, len(knee_lr) - 1)][1])
        if abs(d0 - d2) < KNEE_ALT_MIN_M:
            return False, f"walk_leg_symmetry knee_alt={d0:.3f}/{d2:.3f}"
    return True, (
        f"walk_ok limb_drift={worst_drift:.4f}m foot_z={min(foot_heights):.3f}m "
        f"{msg_m} samples={samples}"
    )


def pick_run_action() -> object | None:
    for act in bpy.data.actions:
        if act.name.endswith("Run") or "|Run" in act.name:
            return act
    return None