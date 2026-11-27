"""Era1 open-frame rack — corner posts, closed rails, depth tie rods, welded island."""
from __future__ import annotations

import bmesh
import bpy

import cc0_common as cc
import era1_mesh_selfcheck as sc
import mesh_builders as mb
import pc_tower_kit as tower

RACK_W = 0.50
RACK_D = 0.60
RACK_H = 1.80
POST = 0.040
RAIL = 0.036
WELD = 0.0005
OVERLAP = 0.006

POST_NAMES = ("post_fl", "post_fr", "post_bl", "post_br")

MESH_SPEC: list[tuple[str, tuple[float, float, float], tuple[float, float, float]]] = []


def _hx() -> float:
    return 0.23


def _hy() -> float:
    return RACK_D / 2 - RAIL / 2


RAIL_Z = (RAIL / 2, RACK_H - RAIL / 2)

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


def _box(name: str, dims: tuple[float, float, float], loc: tuple[float, float, float]) -> bpy.types.Object:
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
    if "face" in name:
        return MAT_UNIT_FACE
    if "unit" in name or "bay" in name:
        return MAT_UNIT
    if "rail" in name or "depth" in name or "frame_weld" in name:
        return MAT_RAIL
    return MAT_FRAME


def _part_color(name: str) -> tuple[float, float, float]:
    if "led_g" in name:
        return COL_LED_G
    if "led_a" in name:
        return COL_LED_A
    if name.startswith("cable"):
        return COL_CABLE
    if "face" in name:
        return COL_FACE
    if "unit" in name or "bay" in name:
        return COL_UNIT
    if "rail" in name or "depth" in name:
        return COL_RAIL
    return COL_FRAME


def mesh_spec_report() -> str:
    lines = ["| Object | Wx×D×H (m) | center (x,y,z) |", "|--------|-----------|----------------|"]
    for name, dims, loc in MESH_SPEC:
        lines.append(f"| {name} | {dims[0]:.3f}×{dims[1]:.3f}×{dims[2]:.3f} | ({loc[0]:.3f}, {loc[1]:.3f}, {loc[2]:.3f}) |")
    return "\n".join(lines)


def _weld_mesh(ob: bpy.types.Object, dist: float = WELD) -> None:
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=dist)
    bpy.ops.object.mode_set(mode="OBJECT")


def build_rack_era1() -> bpy.types.Object:
    MESH_SPEC.clear()
    hx = _hx()
    hy = _hy()
    y_front = -hy + RAIL / 2
    y_back = hy - RAIL / 2
    ud = RACK_D - POST - RAIL
    uw = RACK_W - POST * 2 + OVERLAP
    parts: list[bpy.types.Object] = []

    for tag, (x, y) in zip(
        POST_NAMES,
        ((-hx, y_front), (hx, y_front), (-hx, y_back), (hx, y_back)),
    ):
        parts.append(_box(tag, (POST, POST, RACK_H), (x, y, RACK_H / 2)))

    for z, tag in zip(RAIL_Z, ("bot", "top")):
        parts.append(_box(f"rail_{tag}_front", (RACK_W, RAIL, RAIL), (0, y_front, z)))
        parts.append(_box(f"rail_{tag}_back", (RACK_W, RAIL, RAIL), (0, y_back, z)))
        parts.append(_box(f"rail_{tag}_left", (RAIL, RACK_D - RAIL, RAIL), (-hx, 0, z)))
        parts.append(_box(f"rail_{tag}_right", (RAIL, RACK_D - RAIL, RAIL), (hx, 0, z)))

    for z, tag in zip(RAIL_Z, ("bot", "top")):
        parts.append(_box(f"depth_tl_{tag}", (RAIL, RACK_D - RAIL, RAIL), (-hx, 0, z)))
        parts.append(_box(f"depth_tr_{tag}", (RAIL, RACK_D - RAIL, RAIL), (hx, 0, z)))

    uc_y = y_front + ud / 2 - OVERLAP
    z4, z2, z1 = 1.48, 1.18, 0.88

    parts.append(_box("unit_4u", (uw, ud, 0.180), (0, uc_y, z4)))
    parts.append(_box("unit_4u_face", (uw - 0.015, 0.012, 0.158), (0, y_front + 0.008, z4)))
    for bi, xo in enumerate((-0.11, 0.0, 0.11)):
        parts.append(_box(f"bay_{bi}", (0.088, 0.010, 0.034), (xo, y_front + 0.009, z4 + (bi - 1) * 0.038)))
    for lj, xo in enumerate((0.08, 0.12)):
        parts.append(_box(f"led_g_{lj}", (0.010, 0.008, 0.008), (xo, y_front + 0.010, z4 - 0.06)))

    parts.append(_box("unit_2u", (uw, ud, 0.090), (0, uc_y, z2)))
    parts.append(_box("unit_2u_face", (uw - 0.015, 0.012, 0.068), (0, y_front + 0.008, z2)))

    parts.append(_box("unit_1u", (uw, ud, 0.045), (0, uc_y, z1)))
    parts.append(_box("unit_1u_face", (uw - 0.015, 0.012, 0.032), (0, y_front + 0.008, z1)))
    for j, xo in enumerate((-0.12, -0.04, 0.04, 0.12)):
        parts.append(_box(f"led_g_{j}", (0.009, 0.008, 0.007), (xo, y_front + 0.010, z1)))

    parts.append(_box("cable_bundle", (RACK_W * 0.4, ud * 0.62, 0.55), (0, y_back - ud * 0.28, 1.25)))

    sc.assert_rack_parts(parts, POST_NAMES, RAIL_Z)

    slot_mats = [bpy.data.materials.new(f"_rack_slot_{i}") for i in range(7)]
    for ob in parts:
        tower._bevel(ob, 0.0015, segments=1)
        tower._paint_part_faces(ob, _part_color(ob.name))
    tower._apply_global_material_slots(parts, slot_mats, _part_mat_index)

    root = mb._union_parts(parts[:])
    cc.apply_transforms(root)
    _weld_mesh(root, WELD)
    tower._fix_tower_normals(root)
    root.name = "post_fl"
    bm = bmesh.new()
    bm.from_mesh(root.data)
    for v in bm.verts:
        if v.co.z > RACK_H:
            v.co.z = RACK_H - 0.001
        if v.co.z < 0:
            v.co.z = 0.0
    bm.to_mesh(root.data)
    bm.free()
    tower.store_face_material_index(root)
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
    _weld_mesh(root, WELD)
    return root