"""Era 1 PC tower — joined solid block, front-face-only detail paint."""
from __future__ import annotations

import bpy

import cc0_common as cc
import mesh_builders as mb

TOWER_W = 0.20
TOWER_D = 0.45
TOWER_H = 0.45

COL_CASE = (0.86, 0.82, 0.72)
COL_BEZEL = (0.66, 0.62, 0.54)
COL_BAY = (0.48, 0.44, 0.40)
COL_SLOT = (0.22, 0.20, 0.18)
COL_POWER = (0.92, 0.50, 0.14)
COL_LED_PWR = (0.20, 0.88, 0.32)
COL_LED_HDD = (0.96, 0.52, 0.12)
COL_BADGE = (0.28, 0.72, 0.68)


def _raw_box(name: str, dims: tuple[float, float, float], loc: tuple[float, float, float]) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = dims
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    return ob


def _paint_era1_solid(ob: bpy.types.Object) -> None:
    mesh = ob.data
    while mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[0])
    vcol = mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    z0, z1 = 0.0, TOWER_H
    for poly in mesh.polygons:
        ny = poly.normal.y
        front = ny < -0.55
        for li in poly.loop_indices:
            co = mesh.vertices[mesh.loops[li].vertex_index].co
            x, y, z = co.x, co.y, co.z
            c = COL_CASE
            if z < 0.012:
                c = (0.42, 0.40, 0.36)
            elif front:
                c = COL_BEZEL
                if z > 0.34:
                    c = COL_BAY
                if 0.24 < z < 0.36:
                    c = COL_BAY
                if 0.38 < z < 0.46 and -0.04 < x < 0.06:
                    c = COL_SLOT
                if 0.20 < z < 0.28 and -0.04 < x < 0.06:
                    c = COL_SLOT
                if 0.05 < z < 0.14:
                    c = COL_SLOT
                if 0.18 < z < 0.26 and x < -0.045:
                    c = COL_POWER
                if 0.18 < z < 0.26 and -0.02 < x < 0.01:
                    c = COL_LED_PWR
                if 0.18 < z < 0.26 and 0.01 < x < 0.04:
                    c = COL_LED_HDD
                if z > 0.38 and x > 0.05:
                    c = COL_BADGE
            vcol.data[li].color = (*c, 1.0)


def build_pc_tower_cc0() -> bpy.types.Object:
    hh = TOWER_H / 2
    y_front = -TOWER_D / 2 + 0.010
    d_face = 0.048
    parts: list[bpy.types.Object] = []

    parts.append(_raw_box("chassis", (TOWER_W, TOWER_D, TOWER_H), (0, 0, hh)))
    parts.append(_raw_box("front_bezel", (TOWER_W - 0.006, d_face, TOWER_H - 0.03), (0, y_front, hh)))
    parts.append(
        _raw_box("bay_cd_a", (TOWER_W - 0.024, d_face + 0.006, 0.112), (0, y_front - 0.002, hh + 0.10)),
    )
    parts.append(
        _raw_box("bay_cd_b", (TOWER_W - 0.024, d_face + 0.006, 0.098), (0, y_front - 0.002, hh - 0.02)),
    )
    parts.append(
        _raw_box("floppy", (TOWER_W - 0.044, d_face + 0.005, 0.050), (0, y_front - 0.002, hh - 0.15)),
    )
    parts.append(_raw_box("cd_slot", (TOWER_W - 0.08, d_face + 0.008, 0.014), (0.015, y_front - 0.003, hh + 0.10)))
    parts.append(_raw_box("eject", (0.030, d_face + 0.008, 0.026), (TOWER_W * 0.28, y_front - 0.003, hh + 0.10)))
    parts.append(_raw_box("power", (0.050, d_face + 0.008, 0.050), (-TOWER_W * 0.26, y_front - 0.003, hh - 0.26)))
    for xo in (-0.042, 0.0, 0.042):
        parts.append(_raw_box("vent", (0.034, d_face + 0.006, 0.018), (xo, y_front - 0.002, 0.068)))
    parts.append(_raw_box("badge", (0.036, d_face + 0.004, 0.046), (TOWER_W * 0.22, y_front - 0.001, hh + 0.14)))
    parts.append(_raw_box("led_pwr", (0.020, d_face + 0.006, 0.020), (-0.018, y_front - 0.003, hh - 0.28)))
    parts.append(_raw_box("led_hdd", (0.020, d_face + 0.006, 0.020), (0.020, y_front - 0.003, hh - 0.28)))

    root = cc.join_meshes(parts)
    cc.apply_transforms(root)
    _paint_era1_solid(root)
    mb.snap_root_to_floor(root)
    return root