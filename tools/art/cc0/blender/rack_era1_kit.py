"""Era 1 open-frame server rack with mounted era1 units."""
from __future__ import annotations

import bpy

import cc0_common as cc
import mesh_builders as mb
import pc_tower_kit as tower

RACK_W = 0.50
RACK_D = 0.60
RACK_H = 1.80
POST = 0.028

MESH_SPEC: list[tuple[str, tuple[float, float, float], tuple[float, float, float]]] = []

MAT_FRAME = 0
MAT_RAIL = 1
MAT_UNIT = 2
MAT_UNIT_FACE = 3
MAT_LED_GREEN = 4
MAT_LED_AMBER = 5
MAT_CABLE = 6

COL_FRAME = (0.30, 0.28, 0.26)
COL_RAIL = (0.38, 0.36, 0.34)
COL_UNIT = tower.COL_CASE
COL_FACE = tower.COL_BEZEL
COL_CABLE = (0.22, 0.21, 0.20)
COL_LED_G = tower.COL_LED_PWR
COL_LED_A = tower.COL_LED_HDD


def _raw_box(name: str, dims: tuple[float, float, float], loc: tuple[float, float, float]) -> bpy.types.Object:
    MESH_SPEC.append((name, dims, loc))
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = dims
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    return ob


def _part_mat_index(name: str) -> int:
    if "led_g" in name:
        return MAT_LED_GREEN
    if "led_a" in name:
        return MAT_LED_AMBER
    if name.startswith("cable"):
        return MAT_CABLE
    if "unit" in name and "face" in name:
        return MAT_UNIT_FACE
    if "unit" in name:
        return MAT_UNIT
    if "rail" in name or "shelf" in name or "hole" in name:
        return MAT_RAIL
    return MAT_FRAME


def _part_color(name: str) -> tuple[float, float, float]:
    if "led_g" in name:
        return COL_LED_G
    if "led_a" in name:
        return COL_LED_A
    if name.startswith("cable"):
        return COL_CABLE
    if "unit" in name and "face" in name:
        return COL_FACE
    if "unit" in name:
        return COL_UNIT
    if "rail" in name or "hole" in name:
        return COL_RAIL
    return COL_FRAME


def mesh_spec_report() -> str:
    lines = ["| Object | Wx×D×H (m) | center (x,y,z) |", "|--------|-----------|----------------|"]
    for name, dims, loc in MESH_SPEC:
        lines.append(f"| {name} | {dims[0]:.3f}×{dims[1]:.3f}×{dims[2]:.3f} | ({loc[0]:.3f}, {loc[1]:.3f}, {loc[2]:.3f}) |")
    lines.append("\n*Parent: single joined mesh; floor z=0.*")
    return "\n".join(lines)


def build_rack_era1() -> bpy.types.Object:
    MESH_SPEC.clear()
    hh = RACK_H / 2
    hx = RACK_W / 2 - POST
    hy = RACK_D / 2 - POST
    y_front = -hy
    parts: list[bpy.types.Object] = []

    for tag, loc in (
        ("post_fl", (-hx, -hy, hh)),
        ("post_fr", (hx, -hy, hh)),
        ("post_bl", (-hx, hy, hh)),
        ("post_br", (hx, hy, hh)),
    ):
        parts.append(_raw_box(tag, (POST, POST, RACK_H), loc))

    for z in (0.04, RACK_H - 0.04):
        parts.append(_raw_box(f"rail_bot_{z:.2f}", (RACK_W, POST, POST), (0, -hy, z)))
        parts.append(_raw_box(f"rail_bot_b_{z:.2f}", (RACK_W, POST, POST), (0, hy, z)))
        parts.append(_raw_box(f"rail_side_l_{z:.2f}", (POST, POST, RACK_D), (-hx, 0, z)))
        parts.append(_raw_box(f"rail_side_r_{z:.2f}", (POST, POST, RACK_D), (hx, 0, z)))

    for i, z in enumerate([0.55, 0.85, 1.15, 1.45, 1.75]):
        parts.append(_raw_box(f"rail_mount_f_{i}", (POST, 0.012, RACK_W - POST * 2), (-hx + 0.01, y_front + 0.02, z)))
        if i % 2 == 0:
            parts.append(_raw_box(f"hole_{i}", (0.008, 0.008, 0.008), (-hx + 0.022, y_front + 0.03, z)))

    uw, ud = RACK_W - POST * 4, RACK_D - POST * 2.5
    parts.append(_raw_box("unit_4u_body", (uw, ud, 0.18), (0, 0, 1.52)))
    parts.append(_raw_box("unit_4u_face", (uw - 0.02, 0.008, 0.16), (0, y_front + 0.04, 1.52)))
    parts.append(_raw_box("unit_2u_body", (uw, ud, 0.09), (0, 0, 1.18)))
    parts.append(_raw_box("unit_2u_face", (uw - 0.02, 0.008, 0.07), (0, y_front + 0.04, 1.18)))
    parts.append(_raw_box("unit_1u_body", (uw, ud, 0.045), (0, 0, 0.88)))
    parts.append(_raw_box("unit_1u_face", (uw - 0.02, 0.008, 0.035), (0, y_front + 0.04, 0.88)))
    for j, xo in enumerate((-0.12, -0.04, 0.04, 0.12)):
        parts.append(_raw_box(f"led_g_{j}", (0.008, 0.006, 0.006), (xo, y_front + 0.045, 0.88)))
        if j < 3:
            parts.append(_raw_box(f"led_a_{j}", (0.006, 0.005, 0.005), (xo + 0.02, y_front + 0.045, 0.875)))

    for i, xo in enumerate((-0.08, 0.0, 0.08)):
        parts.append(_raw_box(f"cable_{i}", (0.035, 0.18, 0.025), (xo, hy - 0.12, 1.0 + i * 0.15)))

    slot_mats = [bpy.data.materials.new(f"_rack_slot_{i}") for i in range(7)]
    for ob in parts:
        tower._bevel(ob, 0.002 if "unit" in ob.name else 0.0015, segments=1)
        tower._paint_part_faces(ob, _part_color(ob.name))
    tower._apply_global_material_slots(parts, slot_mats, _part_mat_index)

    root = cc.join_meshes(parts)
    cc.apply_transforms(root)
    tower._fix_tower_normals(root)
    tower._split_material_seams(root)
    tower._merge_face_colors_generic(
        root,
        {
            MAT_FRAME: COL_FRAME,
            MAT_RAIL: COL_RAIL,
            MAT_UNIT: COL_UNIT,
            MAT_UNIT_FACE: COL_FACE,
            MAT_LED_GREEN: COL_LED_G,
            MAT_LED_AMBER: COL_LED_A,
            MAT_CABLE: COL_CABLE,
        },
    )
    mb.snap_root_to_floor(root)
    return root