"""Founder Kenney lane: chunky clothes baked as mesh bands (no procedural shells)."""
from __future__ import annotations

import bpy
from mathutils import Vector

import blender_common as bc

PROCEDURAL_GARMENT = (
    "HoodieTorso",
    "HoodieSleeve",
    "HoodiePocket",
    "JeansHips",
    "JeansLeg",
    "ClothTop",
    "ClothBottom",
    "Hoodie",
    "Jeans",
    "Hood",
    "KenneyFounder",
)
CLOTHING_SHELLS = frozenset(
    {
        "Torso",
        "Leg_-1",
        "Leg_1",
        "Hips",
        "UpperArm_-1",
        "UpperArm_1",
        "Foot_-1",
        "Foot_1",
    }
)
SKIN_OK = frozenset(
    {
        "Head",
        "Hair",
        "Eye_-1",
        "Eye_1",
        "Mouth",
        "Eyebrows",
        "Hand_-1",
        "Hand_1",
        "LowerArm_-1",
        "LowerArm_1",
    }
)
MIN_HEAD_FRAC = 0.20
MAX_HEAD_FRAC = 0.31
MIN_CLOTHING_VERTS = 12


def _height_axis(mesh_obs: list[bpy.types.Object]) -> tuple[str, float, float]:
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in mesh_obs:
        for v in ob.data.vertices:
            w = ob.matrix_world @ v.co
            mins.x = min(mins.x, w.x)
            mins.y = min(mins.y, w.y)
            mins.z = min(mins.z, w.z)
            maxs.x = max(maxs.x, w.x)
            maxs.y = max(maxs.y, w.y)
            maxs.z = max(maxs.z, w.z)
    sy = maxs.y - mins.y
    sz = maxs.z - mins.z
    if sz >= sy:
        return "z", mins.z, maxs.z
    return "y", mins.y, maxs.y


def _part_span(ob: bpy.types.Object, axis: str) -> float:
    vals = [(ob.matrix_world @ v.co).z if axis == "z" else (ob.matrix_world @ v.co).y for v in ob.data.vertices]
    return max(vals) - min(vals)


def _is_skin_rgba(ob: bpy.types.Object, skin=(0.84, 0.68, 0.54)) -> bool:
    mat = ob.data.materials[0] if ob.data.materials else None
    if not mat or not mat.use_nodes:
        return False
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if not bsdf:
        return False
    r, g, b, _ = tuple(bsdf.inputs["Base Color"].default_value)
    return (
        abs(r - skin[0]) < 0.12
        and abs(g - skin[1]) < 0.12
        and abs(b - skin[2]) < 0.12
        and b < g < r
    )


def check_clothing_enclosure(mesh_obs: list[bpy.types.Object]) -> tuple[bool, str]:
    by = {o.name.split(".")[0]: o for o in mesh_obs if o.type == "MESH"}
    for name in by:
        for bad in PROCEDURAL_GARMENT:
            if name.startswith(bad):
                return False, f"procedural_garment={name}"
    missing = [k for k in ("Torso", "Head", "Leg_-1", "Leg_1") if k not in by]
    if missing:
        return False, f"missing_core={missing}"
    for shell in CLOTHING_SHELLS:
        ob = by.get(shell)
        if not ob:
            continue
        if len(ob.data.vertices) < MIN_CLOTHING_VERTS:
            return False, f"shell_empty={shell}"
        if bc.mesh_island_count(ob) > 1:
            return False, f"shell_fragmented={shell} islands={bc.mesh_island_count(ob)}"
    for name, ob in by.items():
        if name in SKIN_OK:
            continue
        if name in CLOTHING_SHELLS or name.startswith(("UpperArm_", "Foot_", "Sole_")):
            if _is_skin_rgba(ob):
                return False, f"skin_outside_clothing={name}"
    return True, "clothing_enclosed"


def check_head_height_fraction(mesh_obs: list[bpy.types.Object]) -> tuple[bool, str]:
    head = next((o for o in mesh_obs if o.name.split(".")[0] == "Head"), None)
    if not head or not head.data.vertices:
        return False, "head_missing"
    axis, lo, hi = _height_axis(mesh_obs)
    total = max(1e-4, hi - lo)
    hspan = _part_span(head, axis)
    frac = hspan / total
    if frac < MIN_HEAD_FRAC:
        return False, f"head_frac={frac:.3f} need>={MIN_HEAD_FRAC}"
    if frac > MAX_HEAD_FRAC:
        return False, f"head_frac={frac:.3f} need<={MAX_HEAD_FRAC}"
    return True, f"head_frac={frac:.3f}"


def check_founder_chunky_bake(mesh_obs: list[bpy.types.Object]) -> tuple[bool, str]:
    ok, msg = check_clothing_enclosure(mesh_obs)
    if not ok:
        return False, msg
    ok2, msg2 = check_head_height_fraction(mesh_obs)
    if not ok2:
        return False, msg2
    return True, f"{msg} {msg2}"