"""Era 1 PC tower — ONE case body + front-face extrusions (no separate bezel slab)."""
from __future__ import annotations

import bpy

import cc0_common as cc
import mesh_builders as mb

TOWER_W = 0.20
TOWER_D = 0.45
TOWER_H = 0.45
PROTRUDE = 0.008  # 8 mm front extrusion for shading

# Hatch v5 mesh spec (all merged into one object; no feet / side glass / fans).
MESH_SPEC: list[tuple[str, tuple[float, float, float], tuple[float, float, float]]] = []

COL_CASE = (0.86, 0.82, 0.72)
COL_BEZEL = (0.66, 0.62, 0.54)
COL_BAY = (0.48, 0.44, 0.40)
COL_SLOT = (0.22, 0.20, 0.18)
COL_POWER = (0.92, 0.50, 0.14)
COL_LED_PWR = (0.20, 0.88, 0.32)
COL_LED_HDD = (0.96, 0.52, 0.12)
COL_BADGE = (0.28, 0.72, 0.68)


def _raw_box(name: str, dims: tuple[float, float, float], loc: tuple[float, float, float]) -> bpy.types.Object:
    MESH_SPEC.append((name, dims, loc))
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = dims
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    return ob


def mesh_spec_report() -> str:
    lines = ["| Mesh | WxDxH (m) | center (x,y,z) |", "|------|-----------|----------------|"]
    for name, dims, loc in MESH_SPEC:
        lines.append(f"| {name} | {dims[0]:.3f}×{dims[1]:.3f}×{dims[2]:.3f} | ({loc[0]:.3f}, {loc[1]:.3f}, {loc[2]:.3f}) |")
    return "\n".join(lines)


def _paint_era1_solid(ob: bpy.types.Object) -> None:
    mesh = ob.data
    while mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[0])
    vcol = mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    y_front = -TOWER_D / 2
    for poly in mesh.polygons:
        front = poly.normal.y < -0.7
        for li in poly.loop_indices:
            co = mesh.vertices[mesh.loops[li].vertex_index].co
            x, z = co.x, co.z
            c = COL_CASE
            if front:
                c = COL_CASE
                if 0.36 < z < 0.42 and abs(x) < 0.095:
                    c = COL_BAY
                if 0.32 < z < 0.36 and abs(x) < 0.095:
                    c = COL_BAY
                if 0.38 < z < 0.44 and abs(x) < 0.04:
                    c = COL_SLOT
                if 0.296 < z < 0.308 and abs(x) < 0.055:
                    c = COL_SLOT
                if 0.07 < z < 0.11 and abs(x) < 0.065:
                    c = COL_SLOT
                if 0.16 < z < 0.22 and x < -0.02:
                    c = COL_POWER
                if 0.16 < z < 0.20 and -0.02 < x < 0.008:
                    c = COL_LED_PWR
                if 0.16 < z < 0.20 and 0.008 < x < 0.028:
                    c = COL_LED_HDD
                if z > 0.36 and x > 0.04:
                    c = COL_BADGE
            vcol.data[li].color = (*c, 1.0)


def build_pc_tower_cc0() -> bpy.types.Object:
    """Local axes: width X, depth Y, height Z. **Front = −Y** (faces 3/4 camera after az −45°)."""
    MESH_SPEC.clear()
    hh = TOWER_H / 2
    y0 = -TOWER_D / 2
    yc = y0 - PROTRUDE / 2

    parts: list[bpy.types.Object] = []
    parts.append(_raw_box("case_body", (TOWER_W, TOWER_D, TOWER_H), (0.0, 0.0, hh)))

    parts.append(_raw_box("bay_upper", (0.18, PROTRUDE, 0.040), (0.0, yc, 0.392)))
    parts.append(_raw_box("bay_lower", (0.18, PROTRUDE, 0.040), (0.0, yc, 0.342)))
    parts.append(_raw_box("floppy", (0.10, PROTRUDE, 0.010), (0.0, yc, 0.302)))
    parts.append(_raw_box("cd_slot", (0.12, PROTRUDE, 0.012), (0.02, yc, 0.385)))
    parts.append(_raw_box("eject_btn", (0.028, PROTRUDE, 0.028), (0.07, yc, 0.385)))
    parts.append(_raw_box("power_btn", (0.040, PROTRUDE, 0.040), (-0.06, yc, 0.190)))
    parts.append(_raw_box("led_pwr", (0.012, PROTRUDE, 0.012), (-0.02, yc, 0.190)))
    parts.append(_raw_box("led_hdd", (0.012, PROTRUDE, 0.012), (0.01, yc, 0.190)))
    parts.append(_raw_box("vent_bar", (0.12, PROTRUDE, 0.008), (0.0, yc, 0.088)))
    parts.append(_raw_box("badge", (0.036, PROTRUDE, 0.046), (0.06, yc, 0.400)))

    root = cc.join_meshes(parts)
    cc.apply_transforms(root)
    _paint_era1_solid(root)
    mb.snap_root_to_floor(root)
    return root