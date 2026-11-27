"""Era 1 chunky beige CRT monitor — tapered body, integral bezel, bulged screen."""
from __future__ import annotations

import math

import bpy
import cc0_common as cc
import era1_mesh_selfcheck as sc
import mesh_builders as mb
import pc_tower_kit as tower

MON_W = 0.40
MON_D = 0.40
SCREEN_W = 0.30
SCREEN_H = 0.23
BODY_H = 0.30
BEZEL_T = 0.04
RECESS = 0.01
SCREEN_BULGE = 0.01

MESH_SPEC: list[tuple[str, tuple[float, float, float], tuple[float, float, float]]] = []

MAT_BODY = 0
MAT_SCREEN = 1
MAT_LED_GREEN = 2
MAT_POWER = 3

COL_CASE = tower.COL_CASE
COL_BEZEL = tower.COL_BEZEL
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


def _bulged_screen(name: str, w: float, h: float, bulge: float, y_center: float, z_center: float) -> bpy.types.Object:
    """4:3 screen with ~1 cm center bulge toward front (−Y)."""
    MESH_SPEC.append((name, (w, bulge * 2, h), (0, y_center, z_center)))
    depth = bulge * 0.35
    bpy.ops.mesh.primitive_cube_add(size=1, location=(0, y_center, z_center))
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = (w, depth, h)
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)

    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.subdivide(number_cuts=5)
    bpy.ops.object.mode_set(mode="OBJECT")

    hw, hh = w * 0.5, h * 0.5
    mesh = ob.data
    for v in mesh.vertices:
        nx = v.co.x / hw if hw > 0 else 0
        nz = v.co.z / hh if hh > 0 else 0
        t = max(0.0, 1.0 - math.sqrt(nx * nx + nz * nz))
        v.co.y -= bulge * (t**1.6)

    mesh.update()
    return ob


def _weighted_com_xy(objects: list[bpy.types.Object]) -> tuple[float, float]:
    vol = 0.0
    cx = cy = 0.0
    for ob in objects:
        d = ob.dimensions
        v = max(d.x * d.y * d.z, 1e-9)
        loc = ob.matrix_world.translation
        cx += loc.x * v
        cy += loc.y * v
        vol += v
    return cx / vol, cy / vol


def _parent_to_body(parts: list[bpy.types.Object], root_name: str = "body_front") -> None:
    root = next(p for p in parts if p.name == root_name)
    for p in parts:
        if p != root:
            p.parent = root
    bpy.ops.object.select_all(action="DESELECT")
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = root
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)


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
    if name == "bezel_front":
        return COL_BEZEL
    return COL_CASE


def mesh_spec_report() -> str:
    lines = ["| Object | Wx×D×H (m) | center (x,y,z) |", "|--------|-----------|----------------|"]
    for name, dims, loc in MESH_SPEC:
        lines.append(f"| {name} | {dims[0]:.3f}×{dims[1]:.3f}×{dims[2]:.3f} | ({loc[0]:.3f}, {loc[1]:.3f}, {loc[2]:.3f}) |")
    lines.append("\n*Parent: `body_front` then join → `compute-monitor-era1`, floor z=0.*")
    return "\n".join(lines)


def build_monitor_era1() -> bpy.types.Object:
    """Front = −Y. Single bezel plate; bulged CRT screen recessed 1 cm."""
    MESH_SPEC.clear()
    z_body = 0.22
    y_front = -MON_D / 2

    parts: list[bpy.types.Object] = []
    body = _raw_box("body_front", (MON_W, 0.22, BODY_H), (0, y_front + 0.11, z_body))
    parts.append(body)
    parts.append(_raw_box("hood_rear", (0.26, 0.20, 0.26), (0, MON_D / 2 - 0.13, z_body)))

    # One thick bezel flush on front face (−Y).
    parts.append(_raw_box("bezel_front", (MON_W, BEZEL_T, BODY_H), (0, y_front - BEZEL_T / 2, z_body)))

    y_scr = y_front - RECESS - SCREEN_BULGE * 0.5
    parts.append(_bulged_screen("screen", SCREEN_W, SCREEN_H, SCREEN_BULGE, y_scr, z_body))

    y_hood = MON_D / 2 - 0.13
    for i, yo in enumerate((0.07, 0.02, -0.03)):
        parts.append(
            _raw_box(f"vent_groove_{i}", (0.18, 0.003, 0.012), (0, y_hood + 0.098, z_body + yo))
        )

    com_x, com_y = _weighted_com_xy([p for p in parts if p.name in ("body_front", "hood_rear", "bezel_front")])

    parts.append(_raw_box("neck", (0.07, 0.07, 0.045), (com_x, com_y, 0.052)))
    bpy.ops.mesh.primitive_cylinder_add(radius=0.11, depth=0.022, location=(com_x, com_y, 0.011))
    foot = bpy.context.active_object
    foot.name = "base_foot"
    MESH_SPEC.append(("base_foot", (0.22, 0.22, 0.022), (com_x, com_y, 0.011)))
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    parts.append(foot)

    br = MON_W * 0.36
    bz = z_body - SCREEN_H / 2 + 0.035
    y_face = y_front - BEZEL_T / 2 - 0.002
    parts.append(tower._raw_cylinder("power_btn", 0.010, 0.006, (br, y_face, bz)))
    parts.append(_raw_box("led_pwr", (0.008, 0.006, 0.006), (br - 0.028, y_face, bz)))

    sc.assert_monitor_parts(parts)
    _parent_to_body(parts)

    slot_mats = [bpy.data.materials.new(f"_mon_slot_{i}") for i in range(4)]
    for ob in parts:
        bw = 0.012 if ob.name == "screen" else 0.004
        segs = 2 if ob.name == "screen" else 1
        tower._bevel(ob, bw, segments=segs)
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