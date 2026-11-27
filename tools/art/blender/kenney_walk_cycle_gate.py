"""Walk cycle: depsgraph arms, material parity, floor contact, leg alternation."""
from __future__ import annotations

import math

import bpy
from mathutils import Vector

MIN_LIMB_SPAN_M = 0.12
LIMB_SPAN_DRIFT_M = 0.45
FOOT_FLOOR_MAX_M = 0.16
FOOT_PASSING_STANCE_MAX_M = 0.42
FOOT_PASSING_MAX_M = 0.68
KNEE_ALT_MIN_M = 0.04
ARM_MAX_FROM_VERTICAL_DEG = 72.0
ARM_MIN_FROM_HORIZONTAL_DEG = 50.0
ARM_MAX_FROM_HORIZONTAL_DEG = 88.0
HAND_BELOW_SHOULDER_M = 0.12
ARM_MIN_SWING_M = 0.018
MIN_ANKLE_SPREAD_M = 0.10
MESH_KNEE_STRETCH_MAX = 4.5
MAT_COLOR_TOL = 0.04
MIN_GAIT_SPAN_FRAMES = 6

LIMB_SPANS = (
    ("LeftShoulder", "LeftHand"),
    ("RightShoulder", "RightHand"),
    ("LeftUpLeg", "LeftFoot"),
    ("RightUpLeg", "RightFoot"),
)

FOUNDER_MATS = frozenset({"FounderTee", "FounderJeans", "FounderSkin"})
RESEARCHER_MATS = frozenset({"ResearcherTee", "ResearcherLabCoat", "ResearcherTrousers"})


def _eval_arm(arm):
    dg = bpy.context.evaluated_depsgraph_get()
    dg.update()
    return dg.objects.get(arm.name).evaluated_get(dg)


def _bone_world_eval(arm, name: str):
    arm_ev = _eval_arm(arm)
    pb = arm_ev.pose.bones.get(name)
    if not pb:
        return None
    return arm_ev.matrix_world @ pb.head


def _bone_span(arm, a: str, b: str) -> float | None:
    pa = _bone_world_eval(arm, a)
    pb = _bone_world_eval(arm, b)
    if not pa or not pb:
        return None
    return (pa - pb).length


def _pose_walk_frame(scene, arm, frame: int, idle_arm_quats, bind_arm_quats=None) -> None:
    scene.frame_set(frame)
    if idle_arm_quats and bind_arm_quats:
        from kenney_hero_walk_strip import apply_idle_arm_blend

        apply_idle_arm_blend(arm, idle_arm_quats, bind_arm_quats)
    bpy.context.view_layer.update()
    _eval_arm(arm)


def _min_foot_z(arm) -> float:
    zs = []
    for name in ("LeftFoot", "RightFoot", "LeftToes", "RightToes"):
        w = _bone_world_eval(arm, name)
        if w:
            zs.append(w.z)
    return min(zs) if zs else 1e9


def _stance_foot_z(arm, side: str) -> float:
    zs = []
    for name in (f"{side}Foot", f"{side}Toes"):
        w = _bone_world_eval(arm, name)
        if w:
            zs.append(w.z)
    return min(zs) if zs else 1e9


def _knee_height(arm, side: str) -> float | None:
    w = _bone_world_eval(arm, f"{side}Leg")
    return w.z if w else None


def _arm_angle_from_vertical_deg(arm, side: str) -> float | None:
    sh = _bone_world_eval(arm, f"{side}Shoulder")
    hand = _bone_world_eval(arm, f"{side}Hand")
    if not sh or not hand:
        return None
    vec = hand - sh
    if vec.length < 1e-5:
        return 0.0
    down = Vector((0.0, 0.0, -1.0))
    cos_a = max(-1.0, min(1.0, vec.normalized().dot(down)))
    return math.degrees(math.acos(cos_a))


def _arm_drop_from_horizontal_deg(arm, side: str) -> float | None:
    sh = _bone_world_eval(arm, f"{side}Shoulder")
    hand = _bone_world_eval(arm, f"{side}Hand")
    if not sh or not hand:
        return None
    vec = hand - sh
    horiz = math.sqrt(vec.x * vec.x + vec.y * vec.y)
    if horiz < 1e-5:
        return 90.0
    return math.degrees(math.atan2(-vec.z, horiz))


def _hand_below_shoulder(arm, side: str) -> tuple[bool, float]:
    sh = _bone_world_eval(arm, f"{side}Shoulder")
    hand = _bone_world_eval(arm, f"{side}Hand")
    if not sh or not hand:
        return False, 0.0
    gap = sh.z - hand.z
    return gap >= HAND_BELOW_SHOULDER_M, gap


def _arms_pose_ok(arm, *, passing_frame: bool = False) -> tuple[bool, str]:
    _ = passing_frame
    horiz_floor = 16.0
    verts: list[float] = []
    horizs: list[float] = []
    for side in ("Left", "Right"):
        ang = _arm_angle_from_vertical_deg(arm, side)
        hang = _arm_drop_from_horizontal_deg(arm, side)
        below, gap = _hand_below_shoulder(arm, side)
        if ang is None or hang is None:
            return False, f"arm_bones_missing_{side}"
        if not below:
            return False, f"arm_hand_above_shoulder_{side} gap={gap:.3f}m"
        if ang > ARM_MAX_FROM_VERTICAL_DEG:
            return False, f"arm_tpose_{side} vert={ang:.1f}deg"
        if hang < horiz_floor:
            return False, f"arm_hang_{side} horiz={hang:.1f}deg"
        if hang > ARM_MAX_FROM_HORIZONTAL_DEG:
            return False, f"arm_hang_{side} horiz={hang:.1f}deg"
        verts.append(ang)
        horizs.append(hang)
    if max(horizs) < 38.0:
        return False, f"arm_tpose_both hang={[round(h, 1) for h in horizs]}"
    return True, f"arms_vert={verts} hang_deg={[round(h, 1) for h in horizs]}"


def _mat_base_color(mat: bpy.types.Material | None) -> tuple[float, float, float]:
    if not mat or not mat.use_nodes:
        return (0.0, 0.0, 0.0)
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if not bsdf:
        return (0.0, 0.0, 0.0)
    c = bsdf.inputs["Base Color"].default_value
    return (float(c[0]), float(c[1]), float(c[2]))


def material_fingerprint(ob: bpy.types.Object) -> tuple[tuple[str, tuple[float, float, float]], ...]:
    if ob.type != "MESH":
        return ()
    slots: list[tuple[str, tuple[float, float, float]]] = []
    for slot in ob.material_slots:
        name = (slot.material.name.split(".")[0] if slot.material else "")
        slots.append((name, _mat_base_color(slot.material)))
    return tuple(slots)


def fingerprints_match(
    ref: tuple[tuple[str, tuple[float, float, float]], ...],
    cur: tuple[tuple[str, tuple[float, float, float]], ...],
) -> tuple[bool, str]:
    if not ref or not cur:
        return False, "mat_fp_empty"
    if len(ref) != len(cur):
        return False, f"mat_slot_count {len(cur)}!={len(ref)}"
    for i, ((rn, rc), (cn, cc)) in enumerate(zip(ref, cur, strict=False)):
        if cn != rn and rn and cn:
            if cn in ("Material", "Kenney", "characterMedium") or rn not in ("", cn):
                pass
        if any(abs(rc[j] - cc[j]) > MAT_COLOR_TOL for j in range(3)):
            return False, f"mat_color_drift slot={i} {rn}->{cn} ref={rc} cur={cc}"
        if cn in ("Material", "Kenney", "characterMedium") and rn not in (
            "Material",
            "Kenney",
            "characterMedium",
        ):
            return False, f"mat_grey_revert slot={i} name={cn}"
    return True, "mats_match"


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


def _ankle_spread_x(arm) -> float:
    lf = _bone_world_eval(arm, "LeftFoot")
    rf = _bone_world_eval(arm, "RightFoot")
    if not lf or not rf:
        return 0.0
    return abs(lf.x - rf.x)


def _edge_len_map(me) -> dict[tuple[int, int], float]:
    out: dict[tuple[int, int], float] = {}
    for e in me.edges:
        a, b = e.vertices[0], e.vertices[1]
        key = (a, b) if a < b else (b, a)
        v0 = me.vertices[a].co
        v1 = me.vertices[b].co
        out[key] = (v0 - v1).length
    return out


def _mesh_leg_stretch_ok(mesh_ob) -> tuple[bool, str]:
    deps = bpy.context.evaluated_depsgraph_get()
    eval_ob = mesh_ob.evaluated_get(deps)
    me = eval_ob.to_mesh()
    try:
        if not me or len(me.vertices) < 32:
            return False, "leg_mesh_empty"
        eval_edges = _edge_len_map(me)
        rest = mesh_ob.data
        worst = 1.0
        for e in rest.edges:
            a, b = e.vertices[0], e.vertices[1]
            key = (a, b) if a < b else (b, a)
            if key not in eval_edges:
                continue
            rv0 = rest.vertices[a].co
            rv1 = rest.vertices[b].co
            rest_len = (rv0 - rv1).length
            if rest_len < 1e-6:
                continue
            worst = max(worst, eval_edges[key] / rest_len)
        if worst > MESH_KNEE_STRETCH_MAX:
            return False, f"leg_mesh_tear stretch={worst:.2f}x"
        return True, f"leg_stretch={worst:.2f}x"
    finally:
        eval_ob.to_mesh_clear()


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
    bind_arm_quats=None,
) -> tuple[bool, str]:
    meshes = render_meshes or [mesh]
    samples = quarter_frames or [int(frame_a), int(frame_b)]
    if max(samples) - min(samples) < MIN_GAIT_SPAN_FRAMES:
        return False, f"walk_gait_compact span={max(samples)-min(samples)}"
    ok_m, msg_m = _hero_materials_ok(mesh, asset_id or "staff-founder-idle")
    if not ok_m:
        return False, msg_m
    mat_ref = material_fingerprint(mesh)
    span_lists: list[list[float]] = []
    foot_heights: list[float] = []
    knee_lr: list[tuple[float, float]] = []
    hand_x: list[float] = []
    cycle_min = 1e9
    for fr in range(int(min(samples)), int(max(samples)) + 1):
        _pose_walk_frame(scene, arm, fr, idle_arm_quats, bind_arm_quats)
        cycle_min = min(cycle_min, _min_foot_z(arm))
    for idx, fr in enumerate(samples):
        _pose_walk_frame(scene, arm, fr, idle_arm_quats, bind_arm_quats)
        ok_m2, msg_m2 = _hero_materials_ok(mesh, asset_id or "staff-founder-idle")
        if not ok_m2:
            return False, f"frame={fr} {msg_m2}"
        ok_fp, msg_fp = fingerprints_match(mat_ref, material_fingerprint(mesh))
        if not ok_fp:
            return False, f"frame={fr} walk_{msg_fp}"
        passing = idx in (1, 3)
        ok_arm, msg_arm = _arms_pose_ok(arm, passing_frame=passing)
        if not ok_arm:
            return False, f"frame={fr} walk_{msg_arm}"
        spread = _ankle_spread_x(arm)
        if idx in (0, 2) and spread < MIN_ANKLE_SPREAD_M:
            return False, f"walk_tiny_stride frame={fr} ankle_spread={spread:.3f}m"
        ok_mesh, msg_mesh = _mesh_leg_stretch_ok(mesh)
        if not ok_mesh:
            return False, f"frame={fr} walk_{msg_mesh}"
        if passing:
            low = _min_foot_z(arm)
            high = max(_stance_foot_z(arm, "Left"), _stance_foot_z(arm, "Right"))
            if low > FOOT_PASSING_STANCE_MAX_M:
                return False, f"walk_floating passing frame={fr} stance_z={low:.4f}m"
            if high - low < KNEE_ALT_MIN_M:
                return False, f"walk_passing_flat frame={fr} spread={high - low:.4f}m"
            if high > FOOT_PASSING_MAX_M:
                return False, f"walk_passing_too_high frame={fr} swing_z={high:.4f}m"
            fz = low
        else:
            stance = "Left" if idx == 0 else "Right"
            fz = _stance_foot_z(arm, stance)
            if fz > FOOT_FLOOR_MAX_M:
                return False, f"walk_floating contact_{stance} frame={fr} foot_z={fz:.4f}m"
        foot_heights.append(fz)
        lh = _bone_world_eval(arm, "LeftHand")
        if lh:
            hand_x.append(lh.x)
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
    if len(hand_x) >= 2 and max(hand_x) - min(hand_x) < ARM_MIN_SWING_M:
        return False, f"walk_arm_no_swing hand_x={hand_x}"
    knee_spread = max(abs(k[0] - k[1]) for k in knee_lr) - min(abs(k[0] - k[1]) for k in knee_lr)
    if knee_spread < KNEE_ALT_MIN_M:
        return False, f"walk_legs_static knee_spread={knee_spread:.3f}"
    return True, (
        f"walk_ok limb_drift={worst_drift:.4f}m foot_z={min(foot_heights):.3f}m "
        f"{msg_m} samples={samples} knee_spread={knee_spread:.3f}"
    )


def pick_run_action() -> object | None:
    for act in bpy.data.actions:
        if act.name.endswith("Run") or "|Run" in act.name:
            return act
    return None