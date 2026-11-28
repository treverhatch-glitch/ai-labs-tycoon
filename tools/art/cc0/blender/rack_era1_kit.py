"""Era 1 open-frame server rack with mounted era1 units."""
from __future__ import annotations

import bpy

import cc0_common as cc
import era1_mesh_selfcheck as sc
import mesh_builders as mb
import pc_tower_kit as tower

RACK_W = 0.50
RACK_D = 0.60
RACK_H = 1.80
POST = 0.028
RAIL_Z = (0.04, RACK_H - 0.04)

POST_NAMES = ("post_fl", "post_fr", "post_bl", "post_br")

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
    if "unit" in name or "bay" in name:
        return MAT_UNIT
    if "rail" in name or "hole" in name:
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
    if "unit" in name or "bay" in name:
        return COL_UNIT
    if "rail" in name or "hole" in name:
        return COL_RAIL
    return COL_FRAME


def mesh_spec_report() -> str:
    lines = ["| Object | Wx×D×H (m) | center (x,y,z) |", "|--------|-----------|----------------|"]
    for name, dims, loc in MESH_SPEC:
        lines.append(f"| {name} | {dims[0]:.3f}×{dims[1]:.3f}×{dims[2]:.3f} | ({loc[0]:.3f}, {loc[1]:.3f}, {loc[2]:.3f}) |")
    lines.append("\n*Parent: `post_fl` then join → floor z=0; 4 posts only.*")
    return "\n".join(lines)


def _closed_frame_rails(parts: list, hx: float, hy: float, z: float, tag: str) -> None:
    parts.append(_raw_box(f"rail_{tag}_front", (RACK_W, POST, POST), (0, -hy, z)))
    parts.append(_raw_box(f"rail_{tag}_back", (RACK_W, POST, POST), (0, hy, z)))
    parts.append(_raw_box(f"rail_{tag}_left", (POST, POST, RACK_D), (-hx, 0, z)))
    parts.append(_raw_box(f"rail_{tag}_right", (POST, POST, RACK_D), (hx, 0, z)))


def _drooping_cables(parts: list, hy: float) -> None:
    y_back = hy - POST
    for i, xo in enumerate((-0.10, 0.0, 0.10)):
        z0 = 1.62 - i * 0.04
        segs = 5
        for s in range(segs):
            t = s / max(segs - 1, 1)
            z = z0 - t * (0.35 + i * 0.08)
            y = y_back - 0.02 - t * (0.22 + i * 0.05)
            parts.append(_raw_box(f"cable_{i}_{s}", (0.028, 0.055, 0.022), (xo, y, z)))


def build_rack_era1() -> bpy.types.Object:
    MESH_SPEC.clear()
    hh = RACK_H / 2
    hx = RACK_W / 2 - POST / 2
    hy = RACK_D / 2 - POST / 2
    y_front = -hy
    inner_w = RACK_W - POST * 3
    inner_d = RACK_D - POST * 3

    parts: list[bpy.types.Object] = []

    locs = (
        ("post_fl", (-hx, -hy, hh)),
        ("post_fr", (hx, -hy, hh)),
        ("post_bl", (-hx, hy, hh)),
        ("post_br", (hx, hy, hh)),
    )
    for tag, loc in locs:
        parts.append(_raw_box(tag, (POST, POST, RACK_H), loc))

    for z, tag in zip(RAIL_Z, ("bottom", "top")):
        _closed_frame_rails(parts, hx, hy, z, tag)

    mount_heights = (0.88, 1.18, 1.52)
    span = RACK_W - POST * 2
    for i, z in enumerate(mount_heights):
        parts.append(_raw_box(f"rail_mount_h_{i}", (span, 0.015, POST), (0, y_front + POST * 0.5, z)))
        for j, xo in enumerate((-span * 0.42, 0.0, span * 0.42)):
            parts.append(_raw_box(f"hole_{i}_{j}", (0.008, 0.008, 0.008), (xo, y_front + POST * 0.65, z)))

    uw, ud = inner_w, inner_d
    parts.append(_raw_box("unit_4u_body", (uw, ud, 0.18), (0, 0, 1.52)))
    parts.append(_raw_box("unit_4u_face", (uw - 0.02, 0.01, 0.16), (0, y_front + ud * 0.48, 1.52)))
    for bi, xo in enumerate((-0.12, 0.0, 0.12)):
        parts.append(_raw_box(f"bay_{bi}", (0.09, 0.008, 0.03), (xo, y_front + ud * 0.48 + 0.006, 1.54 + bi * 0.04)))
    parts.append(_raw_box("unit_2u_body", (uw, ud, 0.09), (0, 0, 1.18)))
    parts.append(_raw_box("unit_2u_face", (uw - 0.02, 0.01, 0.07), (0, y_front + ud * 0.48, 1.18)))

    parts.append(_raw_box("unit_1u_body", (uw, ud, 0.045), (0, 0, 0.88)))
    parts.append(_raw_box("unit_1u_face", (uw - 0.02, 0.01, 0.035), (0, y_front + ud * 0.48, 0.88)))
    for j, xo in enumerate((-0.14, -0.05, 0.04, 0.13)):
        parts.append(_raw_box(f"led_g_{j}", (0.008, 0.006, 0.006), (xo, y_front + ud * 0.48 + 0.006, 0.88)))
        if j < 3:
            parts.append(_raw_box(f"led_a_{j}", (0.006, 0.005, 0.005), (xo + 0.025, y_front + ud * 0.48 + 0.006, 0.877)))

    _drooping_cables(parts, hy)

    sc.assert_rack_parts(parts, POST_NAMES, RAIL_Z)

    root_post = next(p for p in parts if p.name == "post_fl")
    for p in parts:
        if p != root_post:
            p.parent = root_post
    bpy.ops.object.select_all(action="DESELECT")
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = root_post
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)

    slot_mats = [bpy.data.materials.new(f"_rack_slot_{i}") for i in range(7)]
    for ob in parts:
        tower._bevel(ob, 0.002 if "unit" in ob.name or "bay" in ob.name else 0.0015, segments=1)
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