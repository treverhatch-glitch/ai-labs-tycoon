"""Era 1 desktop PC tower — beige 90s rect case, flat per-part vertex colors."""
from __future__ import annotations

import bpy
from mathutils import Vector

import cc0_common as cc
import mesh_builders as mb

TOWER_W = 0.24
TOWER_D = 0.40
TOWER_H = 0.52
MIN_TRIS = 900

# Era 1: beige / taupe chassis, saturated accent only on power LED.
COL_CHASSIS = (0.84, 0.80, 0.72)
COL_TRIM = (0.76, 0.72, 0.64)
COL_BAY = (0.58, 0.54, 0.48)
COL_VENT = (0.32, 0.30, 0.28)
COL_PANEL = (0.70, 0.66, 0.58)
COL_FAN_HOLE = (0.42, 0.40, 0.38)
COL_FAN_GRILLE = (0.64, 0.60, 0.54)
COL_POWER = (0.95, 0.48, 0.12)
COL_LED = (0.22, 0.78, 0.28)
COL_FOOT = (0.38, 0.36, 0.34)


def _flat_color(ob: bpy.types.Object, rgb: tuple[float, float, float]) -> None:
    mesh = ob.data
    while mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[0])
    vcol = mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    col = (*rgb, 1.0)
    for i in range(len(mesh.loops)):
        vcol.data[i].color = col


def _box(
    name: str,
    dims: tuple[float, float, float],
    loc: tuple[float, float, float],
    color: tuple[float, float, float],
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = dims
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    _flat_color(ob, color)
    return ob


def _torus_ring(
    name: str,
    major_r: float,
    minor_r: float,
    loc: tuple[float, float, float],
    color: tuple[float, float, float],
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_torus_add(
        major_radius=major_r,
        minor_radius=minor_r,
        location=loc,
        rotation=(0.0, 1.5708, 0.0),
    )
    ob = bpy.context.active_object
    ob.name = name
    _flat_color(ob, color)
    return ob


def _cylinder(
    name: str,
    radius: float,
    depth: float,
    loc: tuple[float, float, float],
    color: tuple[float, float, float],
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cylinder_add(radius=radius, depth=depth, location=loc, rotation=(0.0, 1.5708, 0.0))
    ob = bpy.context.active_object
    ob.name = name
    _flat_color(ob, color)
    return ob


def _tri_count(ob: bpy.types.Object) -> int:
    return sum(len(p.vertices) for p in ob.data.polygons)


def build_pc_tower_cc0() -> bpy.types.Object:
    hh = TOWER_H / 2 + 0.012
    foot_h = 0.014
    parts: list[bpy.types.Object] = []
    body_h = TOWER_H - foot_h

    parts.append(_box("chassis", (TOWER_W, TOWER_D, body_h), (0, 0, foot_h + body_h / 2), COL_CHASSIS))
    parts.append(_box("top_trim", (TOWER_W - 0.01, TOWER_D - 0.01, 0.022), (0, 0, foot_h + body_h - 0.011), COL_TRIM))
    parts.append(_box("base_trim", (TOWER_W + 0.004, TOWER_D + 0.004, 0.016), (0, 0, foot_h + 0.008), COL_TRIM))

    front_y = -TOWER_D / 2 + 0.002
    bezel_d = 0.036
    parts.append(
        _box("front_bezel", (TOWER_W - 0.012, bezel_d, body_h - 0.04), (0, front_y, foot_h + body_h / 2), COL_TRIM)
    )
    parts.append(
        _box("bay_525", (TOWER_W - 0.05, bezel_d + 0.005, 0.12), (0, front_y - 0.001, foot_h + body_h - 0.20), COL_BAY)
    )
    parts.append(
        _box("bay_floppy", (TOWER_W - 0.06, bezel_d + 0.005, 0.05), (0, front_y - 0.001, foot_h + body_h - 0.33), COL_BAY)
    )
    parts.append(
        _box("power_btn", (0.038, bezel_d + 0.006, 0.038), (-TOWER_W * 0.22, front_y - 0.002, foot_h + body_h - 0.44), COL_POWER)
    )
    parts.append(
        _box("hdd_led", (0.018, bezel_d + 0.004, 0.012), (-TOWER_W * 0.08, front_y - 0.002, foot_h + body_h - 0.44), COL_LED)
    )
    for z in (foot_h + 0.14, foot_h + 0.22, foot_h + 0.30):
        parts.append(_box("vent", (TOWER_W - 0.10, 0.009, 0.014), (0, front_y - 0.003, z), COL_VENT))

    side_x = TOWER_W / 2 - 0.005
    mid_z = foot_h + body_h * 0.46
    parts.append(_box("side_panel", (0.016, TOWER_D * 0.46, body_h * 0.36), (side_x, 0.0, mid_z), COL_PANEL))
    for z_off, name in ((0.08, "fan_a"), (-0.12, "fan_b")):
        zc = mid_z + z_off
        parts.append(_cylinder(name, 0.044, 0.020, (side_x + 0.006, 0.0, zc), COL_FAN_HOLE))
        parts.append(_torus_ring(f"{name}_ring", 0.058, 0.010, (side_x + 0.008, 0.0, zc), COL_FAN_GRILLE))

    for sx in (-1, 1):
        for sy in (-1, 1):
            parts.append(
                _box("foot", (0.036, 0.036, foot_h), (sx * (TOWER_W / 2 - 0.055), sy * (TOWER_D / 2 - 0.055), foot_h / 2), COL_FOOT)
            )

    root = cc.join_meshes(parts)
    cc.apply_transforms(root)
    mb.snap_root_to_floor(root)
    tris = _tri_count(root)
    if tris < MIN_TRIS:
        raise RuntimeError(f"tower: {tris} tris (need >={MIN_TRIS})")
    return root