"""Shared garage material slots (2–4 colours per prop survive join/export)."""
from __future__ import annotations

import bpy

_SLOT_SPECS: dict[str, tuple[tuple[float, float, float, float], float]] = {
    "steel": ((0.40, 0.44, 0.50, 1.0), 0.32),
    "wood": ((0.52, 0.36, 0.22, 1.0), 0.55),
    "plastic_beige": ((0.88, 0.82, 0.70, 1.0), 0.38),
    "plastic_dark": ((0.22, 0.22, 0.24, 1.0), 0.55),
    "plastic_orange": ((0.86, 0.42, 0.14, 1.0), 0.35),
    "concrete": ((0.48, 0.46, 0.44, 1.0), 0.82),
    "dark_metal": ((0.18, 0.20, 0.24, 1.0), 0.35),
    "corrugated": ((0.72, 0.74, 0.76, 1.0), 0.28),
    "crt_bezel": ((0.88, 0.82, 0.72, 1.0), 0.35),
    "glass_dark": ((0.06, 0.08, 0.10, 1.0), 0.08),
    "cable_black": ((0.05, 0.05, 0.06, 1.0), 0.75),
    "coil_copper": ((0.72, 0.48, 0.18, 1.0), 0.42),
    "whiteboard": ((0.94, 0.93, 0.88, 1.0), 0.32),
    "frame_wood": ((0.82, 0.56, 0.22, 1.0), 0.42),
    "sign_yellow": ((0.98, 0.88, 0.10, 1.0), 0.34),
    "grass": ((0.18, 0.58, 0.24, 1.0), 0.65),
    "curb_grey": ((0.58, 0.54, 0.50, 1.0), 0.75),
    "marker_red": ((0.78, 0.16, 0.12, 1.0), 0.38),
    "marker_blue": ((0.14, 0.38, 0.72, 1.0), 0.38),
    "marker_green": ((0.16, 0.52, 0.22, 1.0), 0.38),
    "keyboard_grey": ((0.78, 0.78, 0.80, 1.0), 0.35),
    "desk_wood": ((0.18, 0.12, 0.08, 1.0), 0.78),
}

_CACHE: dict[str, bpy.types.Material] = {}


def get_canonical_material(slot: str) -> bpy.types.Material:
    key = slot
    cached = _CACHE.get(key)
    if cached is not None and cached.name in bpy.data.materials:
        return cached
    rgba, rough = _SLOT_SPECS[key]
    name = f"Garage_{slot}"
    mat = bpy.data.materials.get(name)
    if mat is None:
        mat = bpy.data.materials.new(name)
        mat.use_nodes = True
        bsdf = mat.node_tree.nodes["Principled BSDF"]
        bsdf.inputs["Base Color"].default_value = rgba
        bsdf.inputs["Roughness"].default_value = rough
    _CACHE[key] = mat
    return mat


def assign_slot(ob: bpy.types.Object, slot: str) -> None:
    ob.data.materials.clear()
    ob.data.materials.append(get_canonical_material(slot))