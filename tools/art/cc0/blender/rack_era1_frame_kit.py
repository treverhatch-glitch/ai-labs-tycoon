"""Era1 open-frame rack — full rebuild: welded single island, closed rail rectangle."""
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
RAIL_Z = (RAIL / 2, RACK_H - RAIL / 2)
OVERLAP = 0.004

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
    if "face" in name:
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
    lines.append("\n*4 corner posts + closed rails; units overlap frame 4mm.*")
    return "\n".join(lines)


def build_rack_era1() -> bpy.types.Object:
    MESH_SPEC.clear()
    hx = RACK_W / 2 - POST / 2
    hy = RACK_D / 2 - POST / 2
    y_front = -hy + POST / 2
    inner_w = RACK_W - POST * 2 + OVERLAP
    inner_d = RACK_D - POST * 2 + OVERLAP
    parts: list[bpy.types.Object] = []

    for tag, (x, y) in zip(
        POST_NAMES,
        ((-hx, -hy), (hx, -hy), (-hx, hy), (hx, hy)),
    ):
        parts.append(_box(tag, (POST, POST, RACK_H), (x, y, RACK_H / 2)))

    for z, tag in zip(RAIL_Z, ("bot", "top")):
        parts.append(_box(f"rail_{tag}_front", (RACK_W, RAIL, RAIL), (0, -hy, z)))
        parts.append(_box(f"rail_{tag}_back", (RACK_W, RAIL, RAIL), (0, hy, z)))
        parts.append(_box(f"rail_{tag}_left", (RAIL, RAIL, RACK_D), (-hx, 0, z)))
        parts.append(_box(f"rail_{tag}_right", (RAIL, RAIL, RACK_D), (hx, 0, z)))

    mount_z = (0.90, 1.18, 1.50)
    span = RACK_W - POST
    for mi, z in enumerate(mount_z):
        parts.append(_box(f"rail_mount_h_{mi}", (span, RAIL * 0.45, RAIL), (0, y_front, z)))
        for hi, xo in enumerate((-span * 0.38, 0.0, span * 0.38)):
            parts.append(_box(f"hole_{mi}_{hi}", (0.010, 0.010, 0.010), (xo, y_front + RAIL * 0.35, z)))

    uw, ud = inner_w, inner_d
    z4, z2, z1 = 1.50, 1.18, 0.90
    parts.append(_box("unit_4u", (uw, ud, 0.178), (0, 0, z4)))
    parts.append(_box("unit_4u_face", (uw - 0.02, 0.012, 0.158), (0, y_front + ud * 0.48, z4)))
    for bi, zoff in enumerate((0.05, 0.0, -0.05)):
        parts.append(_box(f"bay_{bi}", (0.10, 0.010, 0.032), (0.12 * (bi - 1), y_front + ud * 0.48 + 0.007, z4 + zoff)))

    parts.append(_box("unit_2u", (uw, ud, 0.088), (0, 0, z2)))
    parts.append(_box("unit_2u_face", (uw - 0.02, 0.012, 0.068), (0, y_front + ud * 0.48, z2)))

    parts.append(_box("unit_1u", (uw, ud, 0.042), (0, 0, z1)))
    parts.append(_box("unit_1u_face", (uw - 0.02, 0.012, 0.032), (0, y_front + ud * 0.48, z1)))
    for j, xo in enumerate((-0.14, -0.05, 0.04, 0.13)):
        parts.append(_box(f"led_g_{j}", (0.010, 0.008, 0.008), (xo, y_front + ud * 0.48 + 0.008, z1)))
        if j < 3:
            parts.append(_box(f"led_a_{j}", (0.008, 0.006, 0.006), (xo + 0.024, y_front + ud * 0.48 + 0.008, z1)))

    y_back = hy - RAIL
    z_cap = RACK_H - RAIL * 0.5
    for ci, xo in enumerate((-0.09, 0.0, 0.09)):
        for seg in range(5):
            t = seg / 4
            z = min(z_cap - ci * 0.03 - t * 0.42, z_cap - 0.02)
            y = y_back - 0.015 - t * (0.20 + ci * 0.06)
            parts.append(_box(f"cable_{ci}_{seg}", (0.026, 0.048, 0.018), (xo, y, max(z, 0.25))))

    sc.assert_rack_parts(parts, POST_NAMES, RAIL_Z)

    slot_mats = [bpy.data.materials.new(f"_rack_slot_{i}") for i in range(7)]
    for ob in parts:
        tower._bevel(ob, 0.002, segments=1)
        tower._paint_part_faces(ob, _part_color(ob.name))
    tower._apply_global_material_slots(parts, slot_mats, _part_mat_index)

    root = mb._union_parts(parts[:])
    cc.apply_transforms(root)
    tower._fix_tower_normals(root)
    root.name = "post_fl"
    _bb = cc._world_bbox([root])
    if _bb[1].z > RACK_H + 0.02 or _bb[0].z < -0.02:
        bm = bmesh.new()
        bm.from_mesh(root.data)
        for v in bm.verts:
            if v.co.z > RACK_H:
                v.co.z = RACK_H - 0.001
            if v.co.z < 0:
                v.co.z = 0.001
        bm.to_mesh(root.data)
        bm.free()
    tower.store_face_material_index(root)
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