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
    if "led" in name or "port_led" in name:
        return MAT_LED_GREEN
    if "led_a" in name:
        return MAT_LED_AMBER
    if name.startswith("cable"):
        return MAT_CABLE
    if "face" in name:
        return MAT_UNIT_FACE
    if "drive_bay" in name or "vent" in name or "slot" in name:
        return MAT_UNIT_FACE
    if "unit" in name:
        return MAT_UNIT
    if "side_rail" in name or "rail" in name or "depth" in name:
        return MAT_RAIL
    return MAT_FRAME


def _part_color(name: str) -> tuple[float, float, float]:
    if "led" in name or "port_led" in name:
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


def build_rack_era1() -> list[bpy.types.Object]:
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

    frame_parts = parts[:]
    sc.assert_rack_parts(frame_parts, POST_NAMES, RAIL_Z)

    y_ff = -0.245
    y_bk = 0.20
    ud_srv = y_bk - y_ff
    uc_y = (y_ff + y_bk) / 2
    uw_srv = RACK_W - POST * 2 + OVERLAP
    u = 0.04445
    z1 = 0.9 + u / 2
    z2a = 0.9 + u + u / 2 + u
    z2b = z2a + u + u / 2 + u
    z4 = 1.6 - u - 0.18 / 2

    server_parts: list[bpy.types.Object] = []

    def _side_rails(unit: str, z_c: float, h: float) -> None:
        z_shelf = z_c - h / 2 - u * 0.6
        y_mid = (y_ff + y_bk) / 2
        for side, sx in (("l", -hx + POST * 0.5), ("r", hx - POST * 0.5)):
            server_parts.append(
                _box(f"{unit}_side_rail_{side}", (POST + 0.028, ud_srv * 0.95, RAIL * 1.2), (sx, y_mid, z_shelf))
            )
            server_parts.append(
                _box(f"{unit}_side_rail_{side}_leg", (RAIL * 1.1, 0.026, h * 0.48), (sx, y_ff + 0.014, z_c - h * 0.22))
            )

    def _closed_unit(name: str, h: float, z: float, *, vents: int = 0, ports: int = 0, leds: int = 0) -> None:
        yf = y_ff + 0.004
        server_parts.append(_box(name, (uw_srv, ud_srv, h), (0, uc_y, z)))
        server_parts.append(_box(f"{name}_face", (uw_srv - 0.008, 0.010, h - 0.008), (0, yf, z)))
        if name == "unit_4u":
            for row in range(4):
                for col in range(3):
                    xo = (col - 1) * 0.092
                    zo = z + (row - 1.5) * 0.036
                    server_parts.append(
                        _box(
                            f"{name}_drive_bay_{row}_{col}",
                            (0.082, 0.012, 0.028),
                            (xo, yf + 0.006, zo),
                        )
                    )
            for li, xo in enumerate((0.10, 0.14)):
                server_parts.append(_box(f"{name}_led_{li}", (0.009, 0.008, 0.008), (xo, yf + 0.007, z - h * 0.32)))
        for vi in range(vents):
            server_parts.append(
                _box(f"{name}_vent_{vi}", (uw_srv - 0.10, 0.010, 0.016), (0, yf + 0.006, z + 0.024 - vi * 0.028))
            )
        if ports:
            for pi in range(8):
                xo = -0.14 + pi * 0.04
                server_parts.append(
                    _box(f"{name}_port_led_{pi}", (0.010, 0.008, 0.010), (xo, yf + 0.007, z))
                )
        for li in range(leds):
            server_parts.append(
                _box(
                    f"{name}_led_{li}",
                    (0.008, 0.006, 0.006),
                    (uw_srv * 0.32 - li * 0.065, yf + 0.004, z - h * 0.26),
                )
            )
        _side_rails(name, z, h)

    _closed_unit("unit_1u", 0.045, z1, ports=8)
    _closed_unit("unit_2u_a", 0.090, z2a, vents=3, leds=2)
    _closed_unit("unit_2u_b", 0.090, z2b, vents=3, leds=2)
    _closed_unit("unit_4u", 0.180, z4)

    slot_mats = [bpy.data.materials.new(f"_rack_slot_{i}") for i in range(7)]
    for ob in frame_parts:
        tower._bevel(ob, 0.0015, segments=1)
        tower._paint_part_faces(ob, _part_color(ob.name))
    tower._apply_global_material_slots(frame_parts, slot_mats, _part_mat_index)

    frame = mb._union_parts(frame_parts)
    cc.apply_transforms(frame)
    _weld_mesh(frame, 0.002)
    _weld_mesh(frame, WELD)
    tower._fix_tower_normals(frame)
    frame.name = "rack_frame"
    bm = bmesh.new()
    bm.from_mesh(frame.data)
    for v in bm.verts:
        if v.co.z > RACK_H:
            v.co.z = RACK_H - 0.001
        if v.co.z < 0:
            v.co.z = 0.0
    bm.to_mesh(frame.data)
    bm.free()
    tower.store_face_material_index(frame)
    tower._merge_face_colors_generic(
        frame,
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

    for ob in server_parts:
        tower._bevel(ob, 0.0015, segments=1)
        tower._paint_part_faces(ob, _part_color(ob.name))
    tower._apply_global_material_slots(server_parts, slot_mats, _part_mat_index)
    for ob in server_parts:
        tower._fix_tower_normals(ob)

    return [frame] + server_parts