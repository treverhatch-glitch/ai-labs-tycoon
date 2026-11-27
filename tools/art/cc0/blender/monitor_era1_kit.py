"""Era 1 chunky beige CRT monitor — procedural single mesh."""
from __future__ import annotations

import bpy

import cc0_common as cc
import mesh_builders as mb
import pc_tower_kit as tower

MON_W = 0.40
MON_D = 0.40
MON_H = 0.38

MAT_BODY = 0
MAT_BEZEL = 1
MAT_SCREEN = 2
MAT_LED_GREEN = 3
MAT_LED_AMBER = 4
MAT_POWER = 5

COL_CASE = tower.COL_CASE
COL_BEZEL = tower.COL_BEZEL
COL_SCREEN = (0.12, 0.17, 0.15)
COL_POWER = tower.COL_POWER
COL_LED_PWR = tower.COL_LED_PWR
COL_LED_HDD = tower.COL_LED_HDD


def _raw_box(name: str, dims: tuple[float, float, float], loc: tuple[float, float, float]) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = dims
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    return ob


def _part_mat_index(name: str) -> int:
    if name == "screen":
        return MAT_SCREEN
    if name.startswith("bezel"):
        return MAT_BEZEL
    if name == "led_pwr":
        return MAT_LED_GREEN
    if name == "led_hdd":
        return MAT_LED_AMBER
    if name == "power_btn":
        return MAT_POWER
    return MAT_BODY


def _part_color(name: str) -> tuple[float, float, float]:
    if name == "screen":
        return COL_SCREEN
    if name.startswith("bezel"):
        return COL_BEZEL
    if name == "led_pwr":
        return COL_LED_PWR
    if name == "led_hdd":
        return COL_LED_HDD
    if name == "power_btn":
        return COL_POWER
    return COL_CASE


def build_monitor_era1() -> bpy.types.Object:
    hh = MON_H / 2
    yf = -MON_D / 2
    yc = yf - 0.006
    parts: list[bpy.types.Object] = []

    parts.append(_raw_box("chassis", (MON_W, MON_D, MON_H), (0, 0, hh + 0.04)))
    parts.append(_raw_box("bezel_front", (MON_W * 0.94, 0.028, MON_H * 0.88), (0, yc, hh + 0.05)))
    parts.append(_raw_box("bezel_side", (MON_W, 0.02, MON_H * 0.9), (0, yf + 0.04, hh + 0.05)))
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.19, location=(0, yf - 0.012, hh + 0.06))
    screen = bpy.context.active_object
    screen.name = "screen"
    screen.scale = (0.92, 0.22, 0.78)
    bpy.ops.object.transform_apply(scale=True)
    parts.append(screen)

    parts.append(_raw_box("neck", (0.10, 0.10, 0.06), (0, 0, 0.07)))
    parts.append(_raw_box("base", (0.28, 0.22, 0.03), (0, 0.04, 0.015)))
    sw = -MON_W * 0.32
    parts.append(tower._raw_cylinder("power_btn", 0.012, 0.010, (sw, yc, 0.12)))
    parts.append(_raw_box("led_pwr", (0.008, 0.006, 0.008), (sw + 0.04, yc, 0.12)))
    parts.append(_raw_box("led_hdd", (0.008, 0.006, 0.008), (sw + 0.06, yc, 0.12)))

    slot_mats = [bpy.data.materials.new(f"_mon_slot_{i}") for i in range(6)]
    for ob in parts:
        w = 0.001 if ob.name == "screen" else 0.004
        tower._bevel(ob, w, segments=1)
        tower._paint_part_faces(ob, _part_color(ob.name))
    tower._apply_global_material_slots(parts, slot_mats, _part_mat_index)

    root = cc.join_meshes(parts)
    cc.apply_transforms(root)
    tower._fix_tower_normals(root)
    tower._split_material_seams(root)
    tower._merge_face_colors_generic(
        root,
        {
            MAT_BODY: COL_CASE,
            MAT_BEZEL: COL_BEZEL,
            MAT_SCREEN: COL_SCREEN,
            MAT_LED_GREEN: COL_LED_PWR,
            MAT_LED_AMBER: COL_LED_HDD,
            MAT_POWER: COL_POWER,
        },
    )
    mb.snap_root_to_floor(root)
    return root