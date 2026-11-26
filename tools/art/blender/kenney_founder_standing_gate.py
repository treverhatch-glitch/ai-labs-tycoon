"""Kenney rig standing checks on evaluated armature before mesh bake."""
from __future__ import annotations

import bpy
from mathutils import Vector

MIN_HEAD_ABOVE_FEET_M = 1.2
MAX_FOOT_ABOVE_FLOOR_M = 0.05

_HEAD_BONES = ("Head", "HeadCtrl")
_FOOT_BONES = (
    "LeftFoot",
    "RightFoot",
    "LeftToes",
    "RightToes",
    "LeftHeelRoll",
    "RightHeelRoll",
)


def _bone_world(arm: bpy.types.Object, name: str) -> Vector | None:
    pb = arm.pose.bones.get(name)
    if not pb:
        return None
    return arm.matrix_world @ pb.head


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