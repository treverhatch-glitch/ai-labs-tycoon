"""Textured hero walk strips — Kenney CC0 Run clip as walk cycle (see manifest)."""
from __future__ import annotations

import math
import os
from dataclasses import dataclass

import bpy
from mathutils import Quaternion

import kenney_founder_lib as kf

RUN_FBX = kf.INCOMING / "kenney/animated-characters-3/Animations/run.fbx"
WALK_FBX = kf.INCOMING / "kenney/animated-characters-3/Animations/walk.fbx"
KENNEY_RUN_LICENSE = "tools/art/cc0/manifest.json (Kenney Animated Characters 3)"
LOCOMOTION_WALK_GROUND_MAX_M = 0.11
COAT_SPINE_BONES = frozenset(
    {"Hips", "Spine", "Chest", "UpperChest", "Neck", "Head"},
)
COAT_TORSO_BONES = frozenset(
    {"Hips", "Spine", "Chest", "UpperChest", "LeftUpLeg", "RightUpLeg"}
)
COAT_ARM_BONES = frozenset(
    {
        "LeftShoulder",
        "RightShoulder",
        "LeftArm",
        "RightArm",
        "LeftForeArm",
        "RightForeArm",
    }
)
COAT_WEIGHT_DENY = frozenset(
    {
        "Head",
        "Neck",
        "LeftLeg",
        "RightLeg",
        "LeftFoot",
        "RightFoot",
        "LeftToes",
        "RightToes",
        "LeftHand",
        "RightHand",
        "LeftHandIndex1",
        "RightHandIndex1",
        "LeftHandThumb1",
        "RightHandThumb1",
    }
)
WALK_ARM_BONES = (
    "LeftShoulder",
    "LeftArm",
    "LeftForeArm",
    "RightShoulder",
    "RightArm",
    "RightForeArm",
)
ARM_BLEND_BONES = WALK_ARM_BONES


def _lead_arm_schedule(
    labels: tuple[str, ...] | list[str], frames: list[int]
) -> dict[int, str]:
    out: dict[int, str] = {}
    for label, fr in zip(labels, frames, strict=False):
        if label == "contact_L":
            out[int(fr)] = "Right"
        elif label == "contact_R":
            out[int(fr)] = "Left"
    return out


@dataclass
class WalkHeroScene:
    arm: bpy.types.Object
    body: bpy.types.Object
    render_meshes: list[bpy.types.Object]
    action_name: str
    walk_frames: list[int]
    labels: tuple[str, str, str, str]
    idle_arm_quats: dict[str, Quaternion]
    bind_arm_quats: dict[str, Quaternion]
    lead_arm_by_frame: dict[int, str]


def measure_walk_frame_pose(
    data: WalkHeroScene, frame: int, *, repose: bool = True
) -> dict[str, float]:
    """Bone metrics after pose_walk_for_render (same armature as walk PNG tiles)."""
    scene = bpy.context.scene
    if repose:
        pose_walk_for_render(scene, data, frame)
    else:
        bpy.context.view_layer.update()
    deps = bpy.context.evaluated_depsgraph_get()
    deps.update()
    arm = data.arm
    spread = _ankle_spread_x(arm)
    knee_l = 180.0
    knee_r = 180.0
    for side in ("Left", "Right"):
        up = _bone_world_simple(arm, f"{side}UpLeg")
        knee = _bone_world_simple(arm, f"{side}Leg")
        foot = _bone_world_simple(arm, f"{side}Foot")
        if up and knee and foot:
            u = (up - knee).normalized()
            v = (foot - knee).normalized()
            dot = max(-1.0, min(1.0, u.dot(v)))
            flex = 180.0 - math.degrees(math.acos(dot))
            if side == "Left":
                knee_l = flex
            else:
                knee_r = flex
    swings: dict[str, float] = {}
    shoulder_swings: dict[str, float] = {}
    for side in ("Left", "Right"):
        ua = arm.pose.bones.get(f"{side}Arm")
        idle_q = data.idle_arm_quats.get(f"{side}Arm")
        if ua and idle_q:
            dq = ua.rotation_quaternion @ idle_q.inverted()
            swings[side] = abs(math.degrees(dq.angle))
        else:
            swings[side] = 0.0
        sh = arm.pose.bones.get(f"{side}Shoulder")
        idle_sh = data.idle_arm_quats.get(f"{side}Shoulder")
        if sh and idle_sh:
            dq_sh = sh.rotation_quaternion @ idle_sh.inverted()
            shoulder_swings[side] = abs(math.degrees(dq_sh.angle))
        else:
            shoulder_swings[side] = 0.0
    return {
        "ankle_x": spread,
        "knee_l": knee_l,
        "knee_r": knee_r,
        "arm_swing_l": swings["Left"],
        "arm_swing_r": swings["Right"],
        "shoulder_swing_l": shoulder_swings["Left"],
        "shoulder_swing_r": shoulder_swings["Right"],
    }


def _purge_dup_armature() -> None:
    dup = bpy.data.objects.get("Root.001")
    if dup:
        bpy.data.objects.remove(dup, do_unlink=True)


def _pick_locomotion_action() -> bpy.types.Action:
    for act in bpy.data.actions:
        if act.name.endswith("Walk") or "|Walk" in act.name:
            return act
    for act in bpy.data.actions:
        if act.name.endswith("Run") or "|Run" in act.name:
            return act
    raise RuntimeError("walk_strip missing Kenney Walk/Run action")


def mute_locomotion_arm_fcurves(arm: bpy.types.Object) -> None:
    """Legs/spine from locomotion clip; arms driven by walk retarget (not Run T-pose)."""
    act = arm.animation_data.action if arm.animation_data else None
    if not act:
        return
    for fc in act.fcurves:
        for bone in WALK_ARM_BONES:
            if f'pose.bones["{bone}"' in fc.data_path:
                fc.mute = True
                break


def _foot_z(arm: bpy.types.Object, bone: str) -> float:
    pb = arm.pose.bones.get(bone)
    if not pb:
        return 1e9
    return (arm.matrix_world @ pb.head).z


def _stance_foot_min_z(arm: bpy.types.Object, side: str) -> float:
    zs = []
    for name in (f"{side}Foot", f"{side}Toes"):
        z = _foot_z(arm, name)
        if z < 1e8:
            zs.append(z)
    return min(zs) if zs else 1e9


def reground_walk_contact(
    arm: bpy.types.Object,
    mesh: bpy.types.Object,
    scene: bpy.types.Scene,
    frame: int,
    bind_arm_quats: dict[str, Quaternion],
    idle_arm_quats: dict[str, Quaternion],
) -> float:
    scene.frame_set(frame)
    apply_idle_arm_blend(arm, idle_arm_quats, bind_arm_quats)
    mn = min(_stance_foot_min_z(arm, "Left"), _stance_foot_min_z(arm, "Right"))
    if mn > 1e8:
        return 0.0
    dz = -mn
    if abs(dz) < 1e-6:
        return 0.0
    arm.location.z += dz
    mesh.location.z += dz
    bpy.context.view_layer.update()
    return dz


def ground_walk_cycle(
    arm: bpy.types.Object,
    mesh: bpy.types.Object,
    scene: bpy.types.Scene,
    start: int,
    end: int,
    bind_arm_quats: dict[str, Quaternion],
    idle_arm_quats: dict[str, Quaternion],
) -> float:
    """Shift hero so lowest foot (idle arms blended) over the clip rests on z=0."""
    mn = 1e9
    for fr in range(start, end + 1):
        scene.frame_set(fr)
        apply_idle_arm_blend(arm, idle_arm_quats, bind_arm_quats)
        for side in ("Left", "Right"):
            mn = min(mn, _stance_foot_min_z(arm, side))
    if mn > 1e8:
        return 0.0
    dz = -mn
    if abs(dz) < 1e-6:
        return 0.0
    arm.location.z += dz
    mesh.location.z += dz
    bpy.context.view_layer.update()
    return dz


def _sample_feet(
    arm: bpy.types.Object,
    scene: bpy.types.Scene,
    start: int,
    end: int,
    bind_arm_quats: dict[str, Quaternion] | None,
    idle_arm_quats: dict[str, Quaternion] | None,
) -> list[tuple[int, float, float]]:
    rows: list[tuple[int, float, float]] = []
    for fr in range(start, end + 1):
        scene.frame_set(fr)
        if idle_arm_quats and bind_arm_quats:
            apply_idle_arm_blend(arm, idle_arm_quats, bind_arm_quats)
        else:
            bpy.context.view_layer.update()
        lz = _stance_foot_min_z(arm, "Left")
        rz = _stance_foot_min_z(arm, "Right")
        rows.append((fr, lz, rz))
    return rows


def _pick_passing(
  rows: list[tuple[int, float, float]],
  exclude: set[int],
  *,
  stance_max_m: float,
) -> int:
    if not rows:
        return 0
    best_fr = rows[0][0]
    best_score = -1.0
    for fr, lz, rz in rows:
        if fr in exclude:
            continue
        low = min(lz, rz)
        if low > stance_max_m:
            continue
        score = max(lz, rz) - low
        if score > best_score:
            best_score = score
            best_fr = fr
    if best_score < 0:
        quarter = max(1, (rows[-1][0] - rows[0][0]) // 4)
        return rows[min(len(rows) // 2, len(rows) - 1)][0] if quarter else rows[0][0]
    return best_fr


def _ankle_spread_x(arm: bpy.types.Object) -> float:
    lf = _bone_world_simple(arm, "LeftFoot")
    rf = _bone_world_simple(arm, "RightFoot")
    if not lf or not rf:
        return 0.0
    return abs(lf.x - rf.x)


def _bone_world_simple(arm: bpy.types.Object, name: str):
    pb = arm.pose.bones.get(name)
    if not pb:
        return None
    return arm.matrix_world @ pb.head


def _knee_depth(arm: bpy.types.Object, side: str) -> float:
    up = _bone_world_simple(arm, f"{side}UpLeg")
    knee = _bone_world_simple(arm, f"{side}Leg")
    foot = _bone_world_simple(arm, f"{side}Foot")
    if not up or not knee or not foot:
        return 0.0
    return (knee.z - up.z) + (foot.z - knee.z) * 0.25


def _frame_gait_score(arm: bpy.types.Object) -> tuple[float, float, float]:
    spread = _ankle_spread_x(arm)
    knee = _knee_depth(arm, "Left") + _knee_depth(arm, "Right")
    swing = max(_stance_foot_min_z(arm, "Left"), _stance_foot_min_z(arm, "Right")) - min(
        _stance_foot_min_z(arm, "Left"), _stance_foot_min_z(arm, "Right")
    )
    return spread, knee, swing


def _spread_minmax_frames(
    arm: bpy.types.Object,
    scene: bpy.types.Scene,
    start: int,
    end: int,
    bind_arm_quats: dict[str, Quaternion] | None,
    idle_arm_quats: dict[str, Quaternion] | None,
    *,
    forbidden: set[int] | None = None,
    grounded_only: bool = False,
) -> tuple[int, int]:
    forbidden = forbidden or set()
    lo_fr, lo_sp = start, 1e9
    hi_fr, hi_sp = start, -1.0
    for fr in range(start, end + 1):
        if fr in forbidden:
            continue
        scene.frame_set(fr)
        if bind_arm_quats and idle_arm_quats:
            apply_idle_arm_blend(arm, idle_arm_quats, bind_arm_quats)
        if grounded_only:
            low = min(_stance_foot_min_z(arm, "Left"), _stance_foot_min_z(arm, "Right"))
            if low > LOCOMOTION_WALK_GROUND_MAX_M:
                continue
        sp = _ankle_spread_x(arm)
        if sp < lo_sp:
            lo_sp, lo_fr = sp, fr
        if sp > hi_sp:
            hi_sp, hi_fr = sp, fr
    return lo_fr, hi_fr


def pick_walk_contact_frames(
    arm: bpy.types.Object,
    scene: bpy.types.Scene,
    start: int,
    end: int,
    bind_arm_quats: dict[str, Quaternion] | None = None,
    idle_arm_quats: dict[str, Quaternion] | None = None,
) -> list[int]:
    """Contact L → down → contact R → up (Kenney Run slice used as walk)."""
    rows = _sample_feet(arm, scene, start, end, bind_arm_quats, idle_arm_quats)

    def _pose_fr(fr: int) -> None:
        scene.frame_set(fr)
        if bind_arm_quats and idle_arm_quats:
            apply_idle_arm_blend(arm, idle_arm_quats, bind_arm_quats)

    def _pick_contact(side: str, exclude: set[int]) -> int:
        col = 1 if side == "Left" else 2
        best_z = min(t[col] for t in rows)
        cands = [t for t in rows if t[col] <= best_z + 0.025 and t[0] not in exclude]
        if not cands:
            cands = sorted(rows, key=lambda t: t[col])[:4]
        best_fr = cands[0][0]
        best_spread = 1e9
        for fr, *_ in cands:
            scene.frame_set(fr)
            if bind_arm_quats and idle_arm_quats:
                apply_idle_arm_blend(arm, idle_arm_quats, bind_arm_quats)
            sp = _ankle_spread_x(arm)
            if sp < best_spread:
                best_spread = sp
                best_fr = fr
        return best_fr

    contact_l = _pick_contact("Left", set())
    contact_r = _pick_contact("Right", {contact_l})
    if contact_l == contact_r:
        quarter = max(1, (end - start) // 4)
        return [start + quarter, start + 2 * quarter, start + 3 * quarter, end]
    lo, hi = sorted((contact_l, contact_r))
    mid_rows = [fr for fr, _, _ in rows if lo <= fr <= hi]

    def _pick_down() -> int:
        best_fr = mid_rows[0] if mid_rows else contact_l
        best_score = -1e9
        for fr in range(start, end + 1):
            if fr in (contact_l, contact_r):
                continue
            _pose_fr(fr)
            spread, knee, swing = _frame_gait_score(arm)
            low_feet = min(_stance_foot_min_z(arm, "Left"), _stance_foot_min_z(arm, "Right"))
            if min(_stance_foot_min_z(arm, "Left"), _stance_foot_min_z(arm, "Right")) > 0.14:
                continue
            score = knee * 2.2 + swing + spread * 0.35
            if score > best_score:
                best_score = score
                best_fr = fr
        return best_fr

    def _pick_up(exclude: set[int]) -> int:
        best_fr = rows[0][0]
        best_score = -1.0
        for fr, lz, rz in rows:
            if fr in exclude:
                continue
            _pose_fr(fr)
            low_foot = min(_stance_foot_min_z(arm, "Left"), _stance_foot_min_z(arm, "Right"))
            if low_foot > LOCOMOTION_WALK_GROUND_MAX_M:
                continue
            spread, knee, swing = _frame_gait_score(arm)
            score = swing * 1.2 + spread * 0.55 + knee * 0.35
            if score > best_score:
                best_score = score
                best_fr = fr
        if best_score < 0:
            grounded = []
            for fr, _, _ in rows:
                if fr in exclude:
                    continue
                _pose_fr(fr)
                low = min(_stance_foot_min_z(arm, "Left"), _stance_foot_min_z(arm, "Right"))
                if low <= LOCOMOTION_WALK_GROUND_MAX_M:
                    grounded.append(fr)
            if grounded:
                return _pick_passing(
                    [r for r in rows if r[0] in grounded],
                    exclude=exclude,
                    stance_max_m=LOCOMOTION_WALK_GROUND_MAX_M,
                )
            return _pick_passing(rows, exclude=exclude, stance_max_m=0.35)
        return best_fr

    down = _pick_down()
    up = _pick_up({contact_l, contact_r, down})
    frames = [contact_l, down, contact_r, up]
    if len(set(frames)) < 4:
        quarter = max(1, (end - start) // 4)
        frames = [
            contact_l,
            lo + quarter,
            contact_r,
            min(end, hi + quarter),
        ]
    if max(frames) - min(frames) < 4:
        quarter = max(1, (end - start) // 4)
        frames = [start + quarter, start + 2 * quarter, start + 3 * quarter, end]
    return frames


def _capture_arm_bind(arm: bpy.types.Object) -> dict[str, Quaternion]:
    out: dict[str, Quaternion] = {}
    for name in WALK_ARM_BONES:
        pb = arm.pose.bones.get(name)
        if pb:
            out[name] = pb.rotation_quaternion.copy()
    return out


def _capture_arm_idle(arm: bpy.types.Object) -> dict[str, Quaternion]:
    out: dict[str, Quaternion] = {}
    for name in WALK_ARM_BONES:
        pb = arm.pose.bones.get(name)
        if pb:
            out[name] = pb.rotation_quaternion.copy()
    return out


def _lead_stride_side(arm: bpy.types.Object) -> str:
    """Foot farther forward (+X in walk strip space) drives opposite-arm swing."""
    lf = _bone_world_simple(arm, "LeftFoot")
    rf = _bone_world_simple(arm, "RightFoot")
    if not lf or not rf:
        return ""
    lead = lf.x - rf.x
    if lead > 0.02:
        return "Left"
    if lead < -0.02:
        return "Right"
    return ""


def _counter_swing_quat(arm: bpy.types.Object, side: str) -> Quaternion:
    lf = _bone_world_simple(arm, "LeftFoot")
    rf = _bone_world_simple(arm, "RightFoot")
    if not lf or not rf:
        return Quaternion((1.0, 0.0, 0.0, 0.0))
    foot_lead = lf.x - rf.x
    signed = -foot_lead if side == "Left" else foot_lead
    lead_stride = _lead_stride_side(arm)
    lead_arm = (lead_stride == "Left" and side == "Right") or (
        lead_stride == "Right" and side == "Left"
    )
    if lead_stride and not lead_arm:
        return Quaternion((1.0, 0.0, 0.0, 0.0))
    amp = max(-1.0, min(1.0, signed / 0.22)) * math.radians(22.0)
    axis = (1.0, 0.0, 0.0)
    return Quaternion(axis, amp)


def apply_walk_arm_retarget(
    arm: bpy.types.Object,
    bind: dict[str, Quaternion],
    idle: dict[str, Quaternion],
    *,
    lead_arm: str | None = None,
) -> None:
    """Idle hang + counter-swing on lead arm vs opposite stride (locomotion arms muted)."""
    _ = bind
    for side in ("Left", "Right"):
        ua = arm.pose.bones.get(f"{side}Arm")
        fa = arm.pose.bones.get(f"{side}ForeArm")
        sh = arm.pose.bones.get(f"{side}Shoulder")
        if not ua or not fa:
            continue
        for pb in (sh, ua, fa):
            if not pb:
                continue
            name = pb.name
            if name not in idle:
                continue
            pb.rotation_mode = "QUATERNION"
            pb.rotation_quaternion = idle[name].copy()
        if lead_arm and side != lead_arm:
            continue
        if lead_arm:
            swing = Quaternion((1.0, 0.0, 0.0), math.radians(22.0))
        else:
            lead_stride = _lead_stride_side(arm)
            lead_side = (
                "Right"
                if lead_stride == "Left"
                else "Left"
                if lead_stride == "Right"
                else side
            )
            if side != lead_side:
                continue
            swing = _counter_swing_quat(arm, side)
        if abs(swing.angle) < 1e-6:
            continue
        if sh:
            sh_swing = Quaternion(swing.axis, swing.angle * 0.78)
            sh.rotation_quaternion = (sh_swing @ sh.rotation_quaternion).normalized()
        ua.rotation_quaternion = (swing @ ua.rotation_quaternion).normalized()
        fa.rotation_quaternion = (swing @ fa.rotation_quaternion).normalized()
    bpy.context.view_layer.update()


def apply_idle_arm_blend(
    arm: bpy.types.Object,
    idle: dict[str, Quaternion] | None = None,
    bind: dict[str, Quaternion] | None = None,
    blend: float = 0.0,
    *,
    lead_arm: str | None = None,
) -> None:
    _ = blend
    if idle and bind:
        apply_walk_arm_retarget(arm, bind, idle, lead_arm=lead_arm)


def ensure_locomotion_action(arm: bpy.types.Object, action_name: str) -> bpy.types.Action:
    act = bpy.data.actions.get(action_name)
    if not act:
        for candidate in bpy.data.actions:
            if candidate.name == action_name or candidate.name.endswith(action_name.split("|")[-1]):
                act = candidate
                break
    if not act:
        act = _pick_locomotion_action()
    arm.animation_data_create()
    arm.animation_data.action = act
    return act


def pose_walk_for_render(
    scene: bpy.types.Scene,
    data: WalkHeroScene,
    frame: int,
) -> None:
    ensure_locomotion_action(data.arm, data.action_name)
    if hasattr(data.arm.data, "pose_position"):
        data.arm.data.pose_position = "POSE"
    scene.frame_set(int(frame))
    apply_idle_arm_blend(
        data.arm,
        data.idle_arm_quats,
        data.bind_arm_quats,
        lead_arm=data.lead_arm_by_frame.get(int(frame)),
    )
    bpy.context.view_layer.update()
    deps = bpy.context.evaluated_depsgraph_get()
    deps.update()


def _weight_mesh_to_head(hair: bpy.types.Object, arm: bpy.types.Object) -> None:
    if hair.type != "MESH":
        return
    if hair.parent:
        world = hair.matrix_world.copy()
        hair.parent = None
        hair.matrix_world = world
    for mod in list(hair.modifiers):
        if mod.type == "ARMATURE":
            hair.modifiers.remove(mod)
    mod = hair.modifiers.new("HairArmature", "ARMATURE")
    mod.object = arm
    hair.vertex_groups.clear()
    head = hair.vertex_groups.new(name="Head")
    head.add(list(range(len(hair.data.vertices))), 1.0, "REPLACE")
    bpy.context.view_layer.update()


def _bind_coat_researcher_armature(
    coat: bpy.types.Object,
    body: bpy.types.Object,
    arm: bpy.types.Object,
) -> None:
    """Lab coat: duplicated body weights, or nearest-body fallback if missing."""
    from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward
    from kenney_geometry_landmarks import compute_landmarks, torso_half_width
    if coat.type != "MESH" or not body.data.vertices:
        return
    if coat.get("coat_weights_from_body") and len(coat.vertex_groups) >= 4:
        for mod in list(coat.modifiers):
            if mod.type in {"ARMATURE", "MESH_DEFORM"}:
                coat.modifiers.remove(mod)
        mod = coat.modifiers.new("CoatWalkArmature", "ARMATURE")
        mod.object = arm
        bpy.context.view_layer.update()
        return
    if coat.parent:
        world = coat.matrix_world.copy()
        coat.parent = None
        coat.matrix_world = world
    for mod in list(coat.modifiers):
        if mod.type in {"ARMATURE", "MESH_DEFORM"}:
            coat.modifiers.remove(mod)
    mod = coat.modifiers.new("CoatWalkArmature", "ARMATURE")
    mod.object = arm
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _y_face_fn = face_shell_coord(body, fwd)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lo, span = lm.zmin, lm.span
    import mathutils.kdtree as kdtree

    kd = kdtree.KDTree(len(body.data.vertices))
    for i, v in enumerate(body.data.vertices):
        kd.insert(body.matrix_world @ v.co, i)
    kd.balance()
    bone_names = COAT_TORSO_BONES | COAT_ARM_BONES
    coat.vertex_groups.clear()
    for bone in bone_names:
        coat.vertex_groups.new(name=bone)

    def _sleeve_vert(co, raw: dict[str, float]) -> bool:
        h = (co.z - lo) / span
        lat = lateral_from_front(co, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        arm_sum = sum(raw.get(b, 0.0) for b in COAT_ARM_BONES)
        if arm_sum > 0.20:
            return True
        return (lm.h_neck - 0.28 <= h <= lm.h_waist + 0.02) and lat > thw * 0.38

    def _nearest_weights(world_co) -> dict[str, float]:
        _co, idx, dist = kd.find(world_co)
        raw: dict[str, float] = {}
        for g in body.data.vertices[idx].groups:
            if g.weight < 1e-5 or g.group >= len(body.vertex_groups):
                continue
            name = body.vertex_groups[g.group].name
            raw[name] = max(raw.get(name, 0.0), g.weight)
        for _co2, idx2, dist2 in kd.find_n(world_co, 4):
            w_dist = 1.0 / max(dist2, 1e-4)
            for g in body.data.vertices[idx2].groups:
                if g.weight < 1e-5 or g.group >= len(body.vertex_groups):
                    continue
                name = body.vertex_groups[g.group].name
                raw[name] = max(raw.get(name, 0.0), g.weight * w_dist * 0.35)
        return raw

    from researcher_shirt_derived_meshes import (
        SKIRT_ALLOWED_VG,
        SKIRT_NAME,
        SLEEVE_ALLOWED_VG,
        SLEEVES_NAME,
    )

    base = coat.name.split(".")[0]
    is_skirt = base == SKIRT_NAME
    is_sleeves = base == SLEEVES_NAME
    for v in coat.data.vertices:
        co = coat.matrix_world @ v.co
        raw = _nearest_weights(co)
        sleeve = _sleeve_vert(co, raw) if not is_skirt else False
        picked: dict[str, float] = {}
        if is_sleeves:
            for name, w in raw.items():
                if name in SLEEVE_ALLOWED_VG:
                    picked[name] = picked.get(name, 0.0) + w
            if sum(picked.values()) < 0.08:
                side = "Left" if lateral_from_front(co, hc, fwd) < 0 else "Right"
                picked[f"{side}Arm"] = 0.55
                picked[f"{side}ForeArm"] = 0.45
            total = sum(picked.values()) or 1.0
            for name, w in picked.items():
                coat.vertex_groups[name].add([v.index], w / total, "REPLACE")
            continue
        if is_skirt:
            for name, w in raw.items():
                if name in SKIRT_ALLOWED_VG:
                    picked[name] = picked.get(name, 0.0) + w
            if not picked:
                picked["Hips"] = 1.0
            total = sum(picked.values())
            for name, w in picked.items():
                coat.vertex_groups[name].add([v.index], w / total, "REPLACE")
            continue
        for name, w in raw.items():
            if name in COAT_WEIGHT_DENY:
                continue
            if sleeve:
                if name in COAT_ARM_BONES:
                    picked[name] = picked.get(name, 0.0) + w
            elif name in COAT_TORSO_BONES:
                picked[name] = picked.get(name, 0.0) + w
            elif name in COAT_ARM_BONES and name.endswith("Shoulder"):
                picked[name] = picked.get(name, 0.0) + w * 0.22
        if sleeve and sum(picked.values()) < 0.08:
            side = "Left" if lateral_from_front(co, hc, fwd) < 0 else "Right"
            picked[f"{side}Shoulder"] = 0.35
            picked[f"{side}Arm"] = 0.55
            picked[f"{side}ForeArm"] = 0.45
        if not picked:
            picked["Chest"] = 1.0
        total = sum(picked.values())
        for name, w in picked.items():
            coat.vertex_groups[name].add([v.index], w / total, "REPLACE")
    bpy.context.view_layer.objects.active = coat
    coat.select_set(True)
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=0.00055)
    bpy.ops.mesh.normals_make_consistent(inside=False)
    bpy.ops.object.mode_set(mode="OBJECT")
    bpy.context.view_layer.update()


def reinforce_founder_forearm_weights(body: bpy.types.Object) -> None:
    """Prefer arm chains over leg weights on sleeve verts (reduces hip forearm blobs)."""
    chains = (
        ("Left", ("LeftArm", "LeftForeArm", "LeftHand"), ("LeftUpLeg", "LeftLeg")),
        ("Right", ("RightArm", "RightForeArm", "RightHand"), ("RightUpLeg", "RightLeg")),
    )
    if not body.vertex_groups:
        return
    for _side, arm_bones, leg_bones in chains:
        for v in body.data.vertices:
            arm_w = leg_w = 0.0
            for g in v.groups:
                name = body.vertex_groups[g.group].name
                if name in arm_bones:
                    arm_w += g.weight
                elif name in leg_bones:
                    leg_w += g.weight
            if arm_w < 0.12 or leg_w <= arm_w:
                continue
            for name in arm_bones:
                body.vertex_groups[name].add([v.index], (arm_w / max(leg_w, 1e-4)) * 0.08, "ADD")
            for name in leg_bones:
                body.vertex_groups[name].add([v.index], -0.35, "SUBTRACT")


def _skin_mesh_to_armature(
    child: bpy.types.Object,
    body: bpy.types.Object,
    arm: bpy.types.Object,
) -> None:
    """Parented extras: nearest-body vertex weights (fallback when mesh deform is unavailable)."""
    if child.type != "MESH" or not body.data.vertices:
        return
    if child.parent:
        world = child.matrix_world.copy()
        child.parent = None
        child.matrix_world = world
    for mod in list(child.modifiers):
        if mod.type == "ARMATURE":
            child.modifiers.remove(mod)
    mod = child.modifiers.new("WalkArmature", "ARMATURE")
    mod.object = arm
    import mathutils.kdtree as kdtree

    kd = kdtree.KDTree(len(body.data.vertices))
    for i, v in enumerate(body.data.vertices):
        kd.insert(body.matrix_world @ v.co, i)
    kd.balance()
    vg_map: dict[str, bpy.types.VertexGroup] = {}
    for vg in body.vertex_groups:
        cg = child.vertex_groups.get(vg.name) or child.vertex_groups.new(name=vg.name)
        vg_map[vg.name] = cg
    for v in child.data.vertices:
        co = child.matrix_world @ v.co
        _co, idx, _dist = kd.find(co)
        bw: dict[str, float] = {}
        for g in body.data.vertices[idx].groups:
            if g.weight < 1e-5:
                continue
            if 0 <= g.group < len(body.vertex_groups):
                bw[body.vertex_groups[g.group].name] = g.weight
        for name, w in bw.items():
            vg_map[name].add([v.index], w, "REPLACE")
    bpy.context.view_layer.update()


def _mesh_needs_walk_pose_bake(ob: bpy.types.Object) -> bool:
    if ob.type != "MESH":
        return False
    base = ob.name.split(".")[0]
    if base in (
        "KenneyResearcherStock",
        "ResearcherCoatSkirt",
        "ResearcherCoatSleeves",
        "ResearcherHairCap",
    ):
        return True
    return any(mod.type == "ARMATURE" for mod in ob.modifiers)


def _duplicate_mesh_with_armature_applied(ob: bpy.types.Object) -> bpy.types.Object:
    view_layer = bpy.context.view_layer
    prev_active = view_layer.objects.active
    prev_sel = {o for o in bpy.context.selected_objects}
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    view_layer.objects.active = ob
    bpy.ops.object.duplicate()
    dup = view_layer.objects.active
    assert dup is not None
    dup.name = f"{ob.name.split('.')[0]}_WalkBake"
    for mod in list(dup.modifiers):
        if mod.type != "ARMATURE":
            dup.modifiers.remove(mod)
    arm_mods = [m for m in dup.modifiers if m.type == "ARMATURE"]
    for mod in arm_mods:
        bpy.ops.object.modifier_apply(modifier=mod.name)
    if dup.parent:
        world = dup.matrix_world.copy()
        dup.parent = None
        dup.matrix_world = world
    for o in bpy.context.selected_objects:
        o.select_set(False)
    for o in prev_sel:
        o.select_set(True)
    view_layer.objects.active = prev_active or ob
    return dup


def bake_evaluated_render_meshes(
    sources: list[bpy.types.Object],
) -> tuple[list[bpy.types.Object], list[bpy.types.Object]]:
    """Posed static meshes for walk PNGs (armature applied at current frame)."""
    bpy.context.view_layer.update()
    bpy.context.evaluated_depsgraph_get().update()
    baked: list[bpy.types.Object] = []
    cleanup: list[bpy.types.Object] = []
    for ob in sources:
        if ob.type != "MESH" or not _mesh_needs_walk_pose_bake(ob):
            baked.append(ob)
            continue
        dup = _duplicate_mesh_with_armature_applied(ob)
        baked.append(dup)
        cleanup.append(dup)
    return baked, cleanup


def dispose_baked_render_meshes(
    sources: list[bpy.types.Object],
    cleanup: list[bpy.types.Object],
) -> None:
    for ob in sources:
        if ob.type == "MESH":
            ob.hide_render = False
    for dup in cleanup:
        me = dup.data
        bpy.data.objects.remove(dup, do_unlink=True)
        if me.users == 0:
            bpy.data.meshes.remove(me)


def _purge_fbx_import_extras(keep: set[str]) -> None:
    _purge_dup_armature()
    for ob in list(bpy.data.objects):
        if ob.type not in {"MESH", "ARMATURE"}:
            continue
        base = ob.name.split(".")[0]
        if base in keep or ob.name in keep:
            continue
        bpy.data.objects.remove(ob, do_unlink=True)


def _assign_locomotion_action(
    arm: bpy.types.Object, *, keep_objects: set[str]
) -> tuple[bpy.types.Action, int, int]:
    clip = WALK_FBX if WALK_FBX.is_file() else RUN_FBX
    if not clip.is_file():
        raise RuntimeError(f"missing Kenney walk/run {clip}")
    kf._import_fbx(str(clip))
    keep_objects.add(arm.name.split(".")[0])
    _purge_fbx_import_extras(keep_objects)
    act = _pick_locomotion_action()
    arm.animation_data_create()
    arm.animation_data.action = act
    mute_locomotion_arm_fcurves(arm)
    start, end = int(act.frame_range[0]), int(act.frame_range[1])
    return act, start, end


def build_founder_walk_scene() -> WalkHeroScene:
    from founder_face_decals import spawn_founder_eye_meshes
    from kenney_founder_stock_recolor import apply_founder_materials

    kf._clear_scene()
    arm, mesh = kf._import_rigged()
    bind_arms = _capture_arm_bind(arm)
    kf._ground_rig(arm, mesh)
    kf._pose_kenney_idle_arms(arm)
    idle_arms = _capture_arm_idle(arm)
    bpy.context.view_layer.update()
    apply_founder_materials(mesh)
    reinforce_founder_forearm_weights(mesh)
    mesh.name = "KenneyFounderStock"
    eyes = spawn_founder_eye_meshes(mesh)
    keep = {
        arm.name.split(".")[0],
        mesh.name.split(".")[0],
        "KenneyFounderStock",
        "Root",
    }
    keep |= {o.name.split(".")[0] for o in eyes}
    act, start, end = _assign_locomotion_action(arm, keep_objects=keep)
    scene = bpy.context.scene
    scene.frame_start = start
    scene.frame_end = end
    ground_walk_cycle(arm, mesh, scene, start, end, bind_arms, idle_arms)
    walk_frames = pick_walk_contact_frames(arm, scene, start, end, bind_arms, idle_arms)
    ground_walk_cycle(arm, mesh, scene, start, end, bind_arms, idle_arms)
    labels = ("contact_L", "down", "contact_R", "up")
    return WalkHeroScene(
        arm=arm,
        body=mesh,
        render_meshes=[mesh, *eyes],
        action_name=act.name,
        walk_frames=walk_frames,
        labels=labels,
        idle_arm_quats=idle_arms,
        bind_arm_quats=bind_arms,
        lead_arm_by_frame=_lead_arm_schedule(labels, walk_frames),
    )


def apply_walk_materials_to_hero(
    meshes: list[bpy.types.Object], asset_id: str
) -> None:
    from kenney_founder_stock_recolor import apply_founder_materials
    from kenney_researcher_from_founder import (
        _tune_coat_mat,
        apply_researcher_founder_base_materials,
    )

    for ob in meshes:
        if ob.type != "MESH":
            continue
        base = ob.name.split(".")[0]
        if asset_id == "staff-founder-idle":
            if base == "KenneyFounderStock":
                apply_founder_materials(ob)
            elif base.startswith("FounderEye_"):
                founder_eye_preview_tweak([ob])
        elif asset_id == "staff-researcher-idle":
            if base == "KenneyResearcherStock":
                apply_researcher_founder_base_materials(ob)
            elif base in ("ResearcherHairCap", "ResearcherCoatSkirt", "ResearcherCoatSleeves"):
                _tune_coat_mat(ob)


def build_researcher_walk_scene() -> WalkHeroScene:
    from kenney_researcher_from_founder import (
        apply_researcher_founder_base_materials,
        spawn_researcher_shirt_derived_parts,
    )
    from kenney_researcher_stock_recolor import spawn_researcher_eye_meshes
    from researcher_hero_geometry import spawn_researcher_hair_cap

    kf._clear_scene()
    arm, mesh = kf._import_rigged()
    bind_arms = _capture_arm_bind(arm)
    kf._ground_rig(arm, mesh)
    kf._pose_kenney_idle_arms(arm)
    idle_arms = _capture_arm_idle(arm)
    bpy.context.view_layer.update()
    mesh.name = "KenneyResearcherStock"
    apply_researcher_founder_base_materials(mesh, preserve_armature_weights=True)
    reinforce_founder_forearm_weights(mesh)
    eyes = spawn_researcher_eye_meshes(mesh)
    hair = spawn_researcher_hair_cap(mesh)
    _weight_mesh_to_head(hair, arm)
    keep = {
        arm.name.split(".")[0],
        mesh.name.split(".")[0],
        "KenneyResearcherStock",
        "Root",
    }
    keep |= {o.name.split(".")[0] for o in (*eyes, hair)}
    act, start, end = _assign_locomotion_action(arm, keep_objects=keep)
    scene = bpy.context.scene
    scene.frame_start = start
    scene.frame_end = end
    ground_walk_cycle(arm, mesh, scene, start, end, bind_arms, idle_arms)
    walk_frames = pick_walk_contact_frames(arm, scene, start, end, bind_arms, idle_arms)
    ground_walk_cycle(arm, mesh, scene, start, end, bind_arms, idle_arms)
    scene.frame_set(start)
    bpy.context.view_layer.update()
    from researcher_shirt_derived_meshes import (
        snapshot_evaluated_body,
        sync_coat_parts_to_body,
    )

    snap = snapshot_evaluated_body(mesh)
    try:
        coat_parts = spawn_researcher_shirt_derived_parts(
            snap, arm, weight_body=mesh
        )
    finally:
        snap_me = snap.data
        bpy.data.objects.remove(snap, do_unlink=True)
        if snap_me.users == 0:
            bpy.data.meshes.remove(snap_me)
    sync_coat_parts_to_body(mesh, coat_parts, arm)
    from researcher_scene_mesh_audit import audit_researcher_render_meshes

    render_set = [mesh, *coat_parts, *eyes, hair]
    audit_researcher_render_meshes(mesh, render_set, arm=arm, prune=False)
    from researcher_shirt_derived_meshes import SLEEVES_NAME, finalize_coat_sleeve_islands
    from kenney_geometry_landmarks import compute_landmarks

    sleeves_ob = next(
        (o for o in coat_parts if o.name.split(".")[0] == SLEEVES_NAME),
        None,
    )
    if sleeves_ob is not None:
        lm_walk = compute_landmarks(mesh)
        from researcher_shirt_derived_meshes import (
            _boost_sleeve_forearm_weights,
            _prune_coat_loose_islands,
            _transfer_coat_weights_nearest_face_interp,
        )

        _prune_coat_loose_islands(sleeves_ob, mesh, min_verts=4, touch_m=0.16)
        _transfer_coat_weights_nearest_face_interp(sleeves_ob, mesh)
        from researcher_shirt_derived_meshes import _snap_sleeve_cuffs_to_body_wrist

        _snap_sleeve_cuffs_to_body_wrist(sleeves_ob, mesh, lm_walk)
        _boost_sleeve_forearm_weights(
            sleeves_ob, lm_walk, body_cx=lm_walk.cx, arm=arm
        )
        scene.frame_set(int(walk_frames[0]))
        bpy.context.view_layer.update()
        from researcher_shirt_derived_meshes import nudge_sleeve_cuffs_toward_wrists

        nudge_sleeve_cuffs_toward_wrists(sleeves_ob, arm, lm_walk)
    extras = [*coat_parts, *eyes, hair]
    keep |= {o.name.split(".")[0] for o in extras}
    labels = ("contact_L", "down", "contact_R", "up")
    return WalkHeroScene(
        arm=arm,
        body=mesh,
        render_meshes=[mesh, *coat_parts, *eyes, hair],
        action_name=act.name,
        walk_frames=walk_frames,
        labels=labels,
        idle_arm_quats=idle_arms,
        bind_arm_quats=bind_arms,
        lead_arm_by_frame=_lead_arm_schedule(labels, walk_frames),
    )


def configure_preview_env(asset_id: str) -> None:
    os.environ["EARLY_ART_PREVIEW_RIG"] = "1"
    os.environ["STAFF_PREVIEW_SKIP_MASK"] = "1"
    os.environ["STAFF_PREVIEW_LOCK_CAMERA"] = "1"
    os.environ["STAFF_PREVIEW_ASSET"] = asset_id
    os.environ["STAFF_PREVIEW_YAW_DEG"] = "30"
    os.environ["STAFF_PREVIEW_ELEV_DEG"] = "10"
    os.environ["STAFF_PREVIEW_MARGIN"] = "1.44"
    if asset_id == "staff-founder-idle":
        os.environ.setdefault("STAFF_PREVIEW_FOUNDER_LIGHT", "0.30")
    if asset_id == "staff-researcher-idle":
        os.environ.setdefault("RESEARCHER_STOCK_RECOLOR", "1")


def researcher_lab_coat_preview_tweak(meshes: list[bpy.types.Object]) -> None:
    """No render-time overlay: lab coat colour comes from mesh material slots only."""
    del meshes


def founder_eye_preview_tweak(meshes: list[bpy.types.Object]) -> None:
    for ob in meshes:
        if ob.type != "MESH" or not ob.name.split(".")[0].startswith("FounderEye_"):
            continue
        mat = ob.active_material
        if not mat or not mat.use_nodes:
            continue
        bsdf = mat.node_tree.nodes.get("Principled BSDF")
        if bsdf:
            bsdf.inputs["Base Color"].default_value = (0.05, 0.05, 0.05, 1.0)
            bsdf.inputs["Roughness"].default_value = 1.0
            spec = bsdf.inputs.get("Specular") or bsdf.inputs.get("Specular IOR Level")
            if spec is not None:
                spec.default_value = 0.0