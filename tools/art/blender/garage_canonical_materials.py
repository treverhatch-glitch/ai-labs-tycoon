"""Shared garage material slots (2–4 colours per prop survive join/export)."""
from __future__ import annotations

import bpy

_SLOT_SPECS: dict[str, tuple[tuple[float, float, float, float], float]] = {
    "steel": ((0.40, 0.44, 0.50, 1.0), 0.32),
    "wood": ((0.52, 0.36, 0.22, 1.0), 0.55),
    "plastic_beige": ((0.78, 0.76, 0.72, 1.0), 0.38),
    "plastic_cool": ((0.74, 0.76, 0.78, 1.0), 0.22),
    "plastic_dark": ((0.22, 0.22, 0.24, 1.0), 0.55),
    "plastic_orange": ((0.78, 0.48, 0.22, 1.0), 0.42),
    "trim_orange": ((0.82, 0.52, 0.24, 1.0), 0.40),
    "concrete": ((0.48, 0.46, 0.44, 1.0), 0.82),
    "dark_metal": ((0.18, 0.20, 0.24, 1.0), 0.35),
    "corrugated": ((0.72, 0.74, 0.76, 1.0), 0.28),
    "crt_bezel": ((0.88, 0.82, 0.72, 1.0), 0.35),
    "glass_dark": ((0.06, 0.08, 0.10, 1.0), 0.08),
    "cable_black": ((0.05, 0.05, 0.06, 1.0), 0.75),
    "coil_copper": ((0.62, 0.42, 0.26, 1.0), 0.48),
    "whiteboard": ((0.94, 0.93, 0.88, 1.0), 0.32),
    "frame_wood": ((0.82, 0.56, 0.22, 1.0), 0.42),
    "sign_cream": ((0.88, 0.80, 0.70, 1.0), 0.38),
    "grass": ((0.18, 0.58, 0.24, 1.0), 0.65),
    "curb_grey": ((0.58, 0.54, 0.50, 1.0), 0.75),
    "marker_warm": ((0.52, 0.36, 0.30, 1.0), 0.42),
    "keyboard_grey": ((0.78, 0.78, 0.80, 1.0), 0.35),
    "desk_wood": ((0.18, 0.12, 0.08, 1.0), 0.78),
}

_CACHE: dict[str, bpy.types.Material] = {}


def clear_cache() -> None:
    _CACHE.clear()


def get_canonical_material(slot: str) -> bpy.types.Material:
    key = slot
    cached = _CACHE.get(key)
    if cached is not None:
        try:
            name = cached.name
        except ReferenceError:
            _CACHE.pop(key, None)
            cached = None
        if cached is not None and name in bpy.data.materials:
            return bpy.data.materials[name]
        _CACHE.pop(key, None)
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


def _slot_for_part(ob: bpy.types.Object, asset_id: str) -> str:
    """Map kit part names to warm CC0 garage slots (no saturated primaries)."""
    name = ob.name.lower().split(".")[0]
    if "screen" in name or name == "chart":
        return "glass_dark"
    if "led" in name:
        return "dark_metal"
    if "pane" in name:
        return "plastic_cool"
    if "glass" in name:
        return "glass_dark"
    if "cord" in name or "cable" in name:
        return "cable_black"
    if "keyboard" in name or "key" in name:
        return "keyboard_grey"
    if "stroke" in name or name.startswith("pad"):
        return "marker_warm"
    if "letter" in name:
        return "plastic_dark"
    if "grass" in name or "tuft" in name:
        return "grass"
    if "curb" in name or "asphalt" in name or "gutter" in name:
        return "curb_grey"
    if asset_id == "garage-cable-spool":
        if "hub" in name:
            return "wood"
        if "flange" in name:
            return "dark_metal"
        if "coil" in name:
            return "coil_copper"
        if "cable" in name:
            return "cable_black"
    if "coil" in name or "cablestrand" in name:
        return "coil_copper"
    if "hub" in name or "flange" in name:
        return "wood"
    if "slat" in name or "curtain" in name or "corrugated" in name:
        return "corrugated"
    if asset_id == "garage-land-for-sale-sign" and "sign" in name:
        return "sign_cream"
    if asset_id == "garage-land-for-sale-sign" and "letter" in name:
        return "plastic_dark"
    if "whiteboard" in name or name == "surface":
        return "whiteboard"
    if "frame" in name or "trim" in name or "mullion" in name or "header" in name or "sill" in name:
        return "frame_wood"
    if "leg" in name or "brace" in name or "rail" in name or "post" in name and "collar" not in name:
        return "steel"
    if "bezel" in name or "crt" in name or "chassis" in name:
        return "crt_bezel"
    if "desktop" in name or "drawer" in name or "seat" in name or "backrest" in name:
        return "desk_wood"
    if "body" in name or "warmer" in name or "pot" in name or "carafe" in name or "spout" in name:
        return "plastic_orange"
    if asset_id == "garage-coffee-drip" and "base" in name:
        return "plastic_dark"
    if "shell" in name or "door" in name or "rack" in name or "closet" in name:
        return "dark_metal"
    if "slab" in name or "tile" in name or "wall" in name or "core" in name:
        return "concrete"
    if asset_id == "garage-floor-tile":
        if "grout" in name:
            return "plastic_dark"
        if "accent" in name or "corner" in name:
            return "trim_orange"
        if "pad" in name:
            return "wood"
    if "stripe" in name:
        return "trim_orange"
    if "handle" in name:
        return "plastic_orange"
    return "plastic_beige"


def _add_emission(ob: bpy.types.Object, rgba: tuple[float, float, float, float], strength: float) -> None:
    mat = ob.data.materials[0] if ob.data.materials else None
    if not mat or not mat.use_nodes:
        return
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if not bsdf:
        return
    bsdf.inputs["Emission Color"].default_value = rgba
    bsdf.inputs["Emission Strength"].default_value = strength


def assign_part_materials(mesh_obs: list[bpy.types.Object], asset_id: str) -> None:
    """Restyle multipart garage kit with canonical Garage_* slots before bake/export."""
    for ob in mesh_obs:
        if ob.name == "ConnectSpine":
            continue
        slot = _slot_for_part(ob, asset_id)
        assign_slot(ob, slot)
        lname = ob.name.lower()
        if "screen" in lname:
            _add_emission(ob, (0.10, 0.48, 0.32, 1.0), 2.2)
        elif "led" in lname:
            _add_emission(ob, (0.95, 0.35, 0.15, 1.0), 1.8)