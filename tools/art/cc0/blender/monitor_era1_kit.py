"""Era 1 chunky beige CRT monitor — tapered body, integral bezel, rounded screen."""
from __future__ import annotations

import bpy

import cc0_common as cc
import mesh_builders as mb
import pc_tower_kit as tower

MON_W = 0.40
MON_D = 0.40
SCREEN_W = 0.30
SCREEN_H = 0.23
BODY_H = 0.30
BEZEL_T = 0.04
RECESS = 0.01
BULGE = 0.004

MESH_SPEC: list[tuple[str, tuple[float, float, float], tuple[float, float, float]]] = []

MAT_BODY = 0
MAT_SCREEN = 1
MAT_LED_GREEN = 2
MAT_POWER = 3

COL_CASE = tower.COL_CASE
COL_SCREEN = (0.10, 0.15, 0.13)
COL_POWER = tower.COL_POWER
COL_LED_PWR = tower.COL_LED_PWR


def _raw_box(name: str, dims: tuple[float, float, float], loc: tuple[float, float, float]) -> bpy.types.Object:
    MESH_SPEC.append((name, dims, loc))
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = dims
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    return ob


def _part_mat_index(name: str) -> int:
    if name == "screen":
        return MAT_SCREEN
    if name == "led_pwr":
        return MAT_LED_GREEN
    if name == "power_btn":
        return MAT_POWER
    return MAT_BODY


def _part_color(name: str) -> tuple[float, float, float]:
    if name == "screen":
        return COL_SCREEN
    if name == "led_pwr":
        return COL_LED_PWR
    if name == "power_btn":
        return COL_POWER
    return COL_CASE


def mesh_spec_report() -> str:
    lines = ["| Object | Wx×D×H (m) | center (x,y,z) |", "|--------|-----------|----------------|"]
    for name, dims, loc in MESH_SPEC:
        lines.append(f"| {name} | {dims[0]:.3f}×{dims[1]:.3f}×{dims[2]:.3f} | ({loc[0]:.3f}, {loc[1]:.3f}, {loc[2]:.3f}) |")
    lines.append("\n*Parent: single joined mesh `compute-monitor-era1` after snap z=0.*")
    return "\n".join(lines)


def build_monitor_era1() -> bpy.types.Object:
    """Front = −Y. Front 0.40×0.38 → rear hood ~0.26×0.24."""
    MESH_SPEC.clear()
    z_body = 0.22
    y_front = -MON_D / 2
    yc = y_front - BEZEL_T / 2
    frame_y = yc

    parts: list[bpy.types.Object] = []
    parts.append(_raw_box("body_front", (MON_W, 0.22, BODY_H), (0, y_front + 0.11, z_body)))
    parts.append(_raw_box("hood_rear", (0.26, 0.20, 0.26), (0, MON_D / 2 - 0.13, z_body)))

    top_h = (BODY_H - SCREEN_H) / 2
    parts.append(_raw_box("bezel_top", (MON_W, top_h, BEZEL_T), (0, frame_y, z_body + SCREEN_H / 2 + top_h / 2)))
    parts.append(_raw_box("bezel_bot", (MON_W, top_h, BEZEL_T), (0, frame_y, z_body - SCREEN_H / 2 - top_h / 2)))
    side_w = (MON_W - SCREEN_W) / 2
    parts.append(_raw_box("bezel_l", (side_w, SCREEN_H, BEZEL_T), (-(SCREEN_W + side_w) / 2, frame_y, z_body)))
    parts.append(_raw_box("bezel_r", (side_w, SCREEN_H, BEZEL_T), ((SCREEN_W + side_w) / 2, frame_y, z_body)))

    scr_y = y_front - RECESS - BULGE / 2
    parts.append(_raw_box("screen", (SCREEN_W, BULGE, SCREEN_H), (0, scr_y, z_body)))

    for i, yo in enumerate((0.05, 0.0, -0.05)):
        parts.append(_raw_box(f"vent_rear_{i}", (0.18, 0.008, 0.012), (0, MON_D / 2 - 0.006, z_body + 0.12 + yo)))

    parts.append(_raw_box("neck", (0.07, 0.07, 0.045), (0, 0, 0.052)))
    bpy.ops.mesh.primitive_cylinder_add(radius=0.11, depth=0.022, location=(0, 0, 0.011))
    foot = bpy.context.active_object
    foot.name = "base_foot"
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    parts.append(foot)

    br = MON_W * 0.32
    bz = z_body - SCREEN_H / 2 - 0.05
    parts.append(tower._raw_cylinder("power_btn", 0.011, 0.008, (br, y_front - 0.012, bz)))
    parts.append(_raw_box("led_pwr", (0.008, 0.006, 0.008), (br - 0.032, y_front - 0.012, bz)))

    slot_mats = [bpy.data.materials.new(f"_mon_slot_{i}") for i in range(4)]
    for ob in parts:
        bw = 0.028 if ob.name == "screen" else 0.004
        tower._bevel(ob, bw if ob.name == "screen" else 0.004, segments=2 if ob.name == "screen" else 1)
        tower._paint_part_faces(ob, _part_color(ob.name))
    tower._apply_global_material_slots(parts, slot_mats, _part_mat_index)

    root = cc.join_meshes(parts)
    cc.apply_transforms(root)
    tower._fix_tower_normals(root)
    tower._split_material_seams(root)
    tower._merge_face_colors_generic(
        root,
        {MAT_BODY: COL_CASE, MAT_SCREEN: COL_SCREEN, MAT_LED_GREEN: COL_LED_PWR, MAT_POWER: COL_POWER},
    )
    mb.snap_root_to_floor(root)
    return root