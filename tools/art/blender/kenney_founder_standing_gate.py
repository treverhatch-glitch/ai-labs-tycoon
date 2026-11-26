"""Kenney standing checks on armature (pre-bake) and evaluated mesh (post-bake)."""
from __future__ import annotations

from mathutils import Vector

MIN_HEAD_ABOVE_FEET_M = 1.2
MAX_FOOT_ABOVE_FLOOR_M = 0.05
MIN_ARM_DROP_FRAC = 0.35

_HEAD_BONES = ("Head", "HeadCtrl")
_FOOT_BONES = (
    "LeftFoot",
    "RightFoot",
    "LeftToes",
    "RightToes",
    "LeftHeelRoll",
    "RightHeelRoll",
)


def _vertical_span(verts: list[Vector]) -> tuple[str, float, float, float]:
    ys = [v.y for v in verts]
    zs = [v.z for v in verts]
    span_y = max(ys) - min(ys)
    span_z = max(zs) - min(zs)
    if span_z >= span_y:
        return "z", min(zs), max(zs), span_z
    return "y", min(ys), max(ys), span_y


def rig_head_foot_world_z(arm) -> tuple[float, float] | None:
    head_z: float | None = None
    for name in _HEAD_BONES:
        w = _bone_world(arm, name)
        if w:
            head_z = w.z
            break
    foot_z: list[float] = []
    for name in _FOOT_BONES:
        w = _bone_world(arm, name)
        if w:
            foot_z.append(w.z)
    if head_z is None or not foot_z:
        return None
    return head_z, min(foot_z)


def _band_centroid_z(mesh_ob, frac: float, high: bool) -> float:
    verts = [mesh_ob.matrix_world @ v.co for v in mesh_ob.data.vertices]
    zs = [v.z for v in verts]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    cut = hi - span * frac if high else lo + span * frac
    band = [v.z for v in verts if (v.z >= cut if high else v.z <= cut)]
    if not band:
        raise ValueError("band_empty")
    return sum(band) / len(band)


def mesh_head_at_high_z(mesh_ob, head_ref_z: float, foot_ref_z: float) -> bool:
    """True when the high-Z mesh end is closer to the rig head than to feet."""
    top_c = _band_centroid_z(mesh_ob, 0.18, high=True)
    return abs(top_c - head_ref_z) < abs(top_c - foot_ref_z)


def verify_evaluated_mesh(
    mesh_ob, head_ref_z: float | None = None, foot_ref_z: float | None = None
) -> tuple[bool, str]:
    """Head cluster must be higher on Z than feet and align with rig landmarks when provided."""
    verts = [mesh_ob.matrix_world @ v.co for v in mesh_ob.data.vertices]
    if len(verts) < 16:
        return False, "mesh_too_small"
    axis, lo, hi, span = _vertical_span(verts)
    if axis != "z":
        return False, f"eval_axis={axis} want=z"
    top_lo = hi - span * 0.18
    bot_hi = lo + span * 0.18
    top = [v.z for v in verts if v.z >= top_lo]
    bot = [v.z for v in verts if v.z <= bot_hi]
    if not top or not bot:
        return False, "mesh_head_feet_bands_empty"
    head_c = sum(top) / len(top)
    foot_c = sum(bot) / len(bot)
    gap = head_c - foot_c
    floor = min(bot)
    if gap < MIN_HEAD_ABOVE_FEET_M * 0.85:
        return False, f"eval_head_feet_gap={gap:.2f}m axis={axis}"
    if floor > MAX_FOOT_ABOVE_FLOOR_M:
        return False, f"eval_feet_float={floor:.3f}m"
    if head_ref_z is not None and foot_ref_z is not None:
        if not mesh_head_at_high_z(mesh_ob, head_ref_z, foot_ref_z):
            return False, "eval_head_on_low_z_end"
    return True, f"eval_head_feet_gap={gap:.2f}m axis={axis} floor={floor:.3f}"


def _bone_world(arm, name: str) -> Vector | None:
    pb = arm.pose.bones.get(name)
    if not pb:
        return None
    return arm.matrix_world @ pb.head


def verify_kenney_arm_drop(arm: bpy.types.Object) -> tuple[bool, str]:
    hip = arm.pose.bones.get("Hips") or arm.pose.bones.get("Spine")
    if not hip:
        return False, "arm_pose_no_hip"
    hip_z = (arm.matrix_world @ hip.head).z
    drops: list[float] = []
    for side in ("Left", "Right"):
        sh = arm.pose.bones.get(f"{side}Shoulder")
        hand = arm.pose.bones.get(f"{side}Hand")
        if not sh or not hand:
            continue
        sh_z = (arm.matrix_world @ sh.head).z
        hand_z = (arm.matrix_world @ hand.head).z
        shoulder_h = sh_z - hip_z
        if shoulder_h < 1e-4:
            continue
        drops.append((sh_z - hand_z) / shoulder_h)
    if len(drops) < 2:
        return False, "arm_pose_bones_missing"
    worst = min(drops)
    if worst < MIN_ARM_DROP_FRAC:
        return False, f"arm_tpose_drop={worst:.2f} want>={MIN_ARM_DROP_FRAC}"
    return True, f"arm_drop={worst:.2f}"


def verify_standing_rig(arm: bpy.types.Object) -> tuple[bool, str]:
    head_z: float | None = None
    for name in _HEAD_BONES:
        w = _bone_world(arm, name)
        if w:
            head_z = w.z
            break
    foot_z: list[float] = []
    for name in _FOOT_BONES:
        w = _bone_world(arm, name)
        if w:
            foot_z.append(w.z)
    if head_z is None or len(foot_z) < 2:
        return False, "standing_bones_missing"
    floor_z = min(foot_z)
    max_foot = max(foot_z)
    gap = head_z - max_foot
    if gap < MIN_HEAD_ABOVE_FEET_M:
        return False, f"head_feet_z_gap={gap:.2f}m want>={MIN_HEAD_ABOVE_FEET_M}"
    if floor_z > MAX_FOOT_ABOVE_FLOOR_M:
        return False, f"feet_float_z={floor_z:.3f}m max={MAX_FOOT_ABOVE_FLOOR_M}"
    return True, f"head_feet_z_gap={gap:.2f}m floor_z={floor_z:.3f}"