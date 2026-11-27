"""Era 1 open wire/frame server rack — procedural."""
from __future__ import annotations

import bpy

import cc0_common as cc
import mesh_builders as mb
import pc_tower_kit as tower

RACK_W = 0.48
RACK_D = 0.55
RACK_H = 1.35
POST = 0.022

MAT_FRAME = 0
MAT_RAIL = 1
MAT_LED = 2

COL_FRAME = (0.32, 0.30, 0.28)
COL_RAIL = (0.42, 0.40, 0.38)
COL_LED = tower.COL_LED_PWR


def _raw_box(name: str, dims: tuple[float, float, float], loc: tuple[float, float, float]) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = dims
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    return ob


def _part_mat_index(name: str) -> int:
    if name.startswith("led"):
        return MAT_LED
    if "rail" in name or "shelf" in name:
        return MAT_RAIL
    return MAT_FRAME


def _part_color(name: str) -> tuple[float, float, float]:
    if name.startswith("led"):
        return COL_LED
    if "rail" in name or "shelf" in name:
        return COL_RAIL
    return COL_FRAME


def build_rack_era1() -> bpy.types.Object:
    hh = RACK_H / 2
    hx, hy = RACK_W / 2 - POST, RACK_D / 2 - POST
    parts: list[bpy.types.Object] = []
    for name, loc in (
        ("post_fl", (-hx, -hy, hh)),
        ("post_fr", (hx, -hy, hh)),
        ("post_bl", (-hx, hy, hh)),
        ("post_br", (hx, hy, hh)),
    ):
        parts.append(_raw_box(name, (POST, POST, RACK_H), loc))
    z_top = RACK_H - 0.02
    z_bot = 0.06
    for z, tag in ((z_top, "top"), (z_bot, "bot")):
        parts.append(_raw_box(f"rail_{tag}_f", (RACK_W - POST, POST, POST), (0, -hy, z)))
        parts.append(_raw_box(f"rail_{tag}_b", (RACK_W - POST, POST, POST), (0, hy, z)))
        parts.append(_raw_box(f"rail_{tag}_l", (POST, POST, RACK_D - POST), (-hx, 0, z)))
        parts.append(_raw_box(f"rail_{tag}_r", (POST, POST, RACK_D - POST), (hx, 0, z)))
    for z in (0.35, 0.65, 0.95):
        parts.append(_raw_box(f"shelf_{z:.2f}", (RACK_W - POST * 3, 0.012, POST), (0, 0, z)))
    parts.append(_raw_box("led_front", (0.010, 0.010, 0.010), (hx - 0.03, -hy + 0.02, 0.82)))
    parts.append(_raw_box("led_rear", (0.010, 0.010, 0.010), (-hx + 0.03, hy - 0.02, 0.82)))

    slot_mats = [bpy.data.materials.new(f"_rack_slot_{i}") for i in range(3)]
    for ob in parts:
        tower._bevel(ob, 0.0015, segments=1)
        tower._paint_part_faces(ob, _part_color(ob.name))
    tower._apply_global_material_slots(parts, slot_mats, _part_mat_index)

    root = cc.join_meshes(parts)
    cc.apply_transforms(root)
    tower._fix_tower_normals(root)
    tower._split_material_seams(root)
    tower._merge_face_colors_generic(
        root,
        {MAT_FRAME: COL_FRAME, MAT_RAIL: COL_RAIL, MAT_LED: COL_LED},
    )
    mb.snap_root_to_floor(root)
    return root