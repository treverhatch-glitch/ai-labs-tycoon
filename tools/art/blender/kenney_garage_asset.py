#!/usr/bin/env python3
"""Pass13: Kenney CC0 garage heroes (multipart, grounded Y-up). Poly Haven only where documented."""
from __future__ import annotations

import json
import math
import os
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
MAP = REPO / "tools/art/cc0/kenney_garage_map.json"
PH_MAP = REPO / "tools/art/cc0/garage_cc0_map.json"
INCOMING_PH = REPO / "third_party/cc0/polyhaven"


def _import_kenney_furniture(glb: str, part_name: str) -> bpy.types.Object:
    import cc0_common as cc0  # type: ignore

    path = cc0.kenney_furniture_glb(glb)
    meshes = cc0.import_gltf(path, part_name)
    return cc0.merge_mesh_objects(meshes, part_name)


def _import_kenney_building(glb: str, part_name: str) -> bpy.types.Object:
    import cc0_common as cc0  # type: ignore

    path = cc0.kenney_glb(glb)
    meshes = cc0.import_gltf(path, part_name)
    return cc0.merge_mesh_objects(meshes, part_name)


def _parent_keep_world(child: bpy.types.Object, parent: bpy.types.Object) -> None:
    mw = child.matrix_world.copy()
    child.parent = parent
    child.matrix_world = mw


def _scale_extent_y(ob: bpy.types.Object, target_y: float) -> None:
    """Scale uniformly so world Y extent matches target (CRT height on desk)."""
    import cc0_common as cc0  # type: ignore

    bpy.context.view_layer.update()
    lo, hi = cc0.world_bbox(ob)
    h = max(hi.y - lo.y, 1e-6)
    factor = target_y / h
    ob.scale = (ob.scale.x * factor, ob.scale.y * factor, ob.scale.z * factor)
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(scale=True)
    bpy.context.view_layer.update()


def _scale_height(ob: bpy.types.Object, target_h: float) -> None:
    import cc0_common as cc0  # type: ignore

    bpy.context.view_layer.update()
    lo, hi = cc0.world_bbox(ob)
    ext = hi - lo
    h = max(ext.x, ext.y, ext.z, 1e-6)
    if h < 1e-6:
        return
    factor = target_h / h
    ob.scale = (ob.scale.x * factor, ob.scale.y * factor, ob.scale.z * factor)
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(scale=True)


def _snap_floor(ob: bpy.types.Object, floor: float = 0.0) -> None:
    import cc0_common as cc0  # type: ignore

    cc0.snap_to_floor(ob, floor)


def _place_on_top(upper: bpy.types.Object, lower: bpy.types.Object, overlap: float = 0.004) -> None:
    import cc0_common as cc0  # type: ignore

    for _ in range(24):
        bpy.context.view_layer.update()
        _, hi = cc0.world_bbox(lower)
        lo_u, _ = cc0.world_bbox(upper)
        gap = lo_u.y - hi.y
        if gap <= 0.003:
            if gap < -0.001:
                upper.location.y += -gap - overlap
            break
        upper.location.y -= gap - overlap
    bpy.context.view_layer.update()


def _center_xz(ob: bpy.types.Object) -> None:
    import cc0_common as cc0  # type: ignore

    lo, hi = cc0.world_bbox(ob)
    cx = (lo.x + hi.x) * 0.5
    cz = (lo.z + hi.z) * 0.5
    ob.location.x -= cx
    ob.location.z -= cz


def _orient_kenney_y_up(ob: bpy.types.Object) -> None:
    """Kenney furniture often imports with Z or X as height; garage build is Y-up."""
    import cc0_common as cc0  # type: ignore

    bpy.context.view_layer.update()
    lo, hi = cc0.world_bbox(ob)
    ext = hi - lo
    if ext.z > ext.y * 1.08 and ext.z >= ext.x * 0.95:
        ob.rotation_euler = (math.pi / 2.0, 0.0, 0.0)
    elif ext.x > ext.y * 1.08 and ext.x >= ext.z * 0.95:
        ob.rotation_euler = (0.0, 0.0, -math.pi / 2.0)
    else:
        return
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(rotation=True)
    bpy.context.view_layer.update()


KENNEY_GARAGE_RESTYLE: dict[str, list[str] | dict[str, list[str]]] = {
    "garage-folding-chair": ["wood", "metal", "wood"],
    "garage-desk-crt": {
        "desktop": ["wood", "metal", "wood"],
        "keyboardgrey": ["wood", "prop"],
    },
    "garage-crt-monitor": {
        "crthousing": ["prop", "prop", "prop"],
        "crtmonitor": ["prop", "screen", "prop"],
        "crtscreen": ["prop", "screen", "prop"],
    },
    "garage-coffee-drip": {
        "dripbase": ["wood", "metal"],
        "driptower": ["metal", "lamp_accent", "wood"],
    },
    "garage-crt-tower": {
        "towerframe": ["shelf", "wood", "metal"],
        "towercrt": ["prop", "screen", "prop"],
        "towerspeaker": ["speaker_dark", "metal", "prop"],
    },
    "garage-voodude-rack": {
        "voorack": ["shelf", "wood", "metal"],
        "voobooks": ["wood", "prop", "wood"],
        "voospeaker": ["speaker_dark", "metal", "prop"],
    },
}


def apply_kenney_garage_materials(mesh_obs: list[bpy.types.Object], asset_id: str) -> None:
    """Flat Kenney restyle (no colormap UV) before bake — matches interior room kit tints."""
    import cc0_common as cc0  # type: ignore
    from garage_canonical_materials import assign_part_materials  # type: ignore

    spec = KENNEY_GARAGE_RESTYLE.get(asset_id)
    if spec is None:
        assign_part_materials(mesh_obs, asset_id)
        return
    if isinstance(spec, list):
        for ob in mesh_obs:
            if ob.type == "MESH":
                cc0.apply_kenney_restyle(ob, spec)
        return
    import blender_common as bc  # type: ignore

    for ob in mesh_obs:
        if ob.type != "MESH":
            continue
        lname = ob.name.lower().split(".")[0]
        roles = ["wood", "metal", "wood"]
        for key, tint_roles in spec.items():
            if key in lname:
                roles = tint_roles
                break
        cc0.apply_kenney_restyle(ob, roles)
        if asset_id == "garage-crt-monitor" or (
            asset_id == "garage-desk-crt" and "crt" in lname
        ):
            bc.assign_crt_screen_material(
                ob, (0.12, 0.76, 0.50, 1.0), bezel=(0.18, 0.20, 0.24, 1.0), screen_mix=0.52
            )


def _poly_gltf(polyhaven_id: str) -> Path:
    pack = INCOMING_PH / polyhaven_id
    entry = pack / "entry.gltf"
    if entry.exists():
        return pack / entry.read_text(encoding="utf-8").strip()
    gltfs = sorted(pack.glob("*.gltf"))
    if not gltfs:
        raise FileNotFoundError(f"no polyhaven gltf for {polyhaven_id}")
    return gltfs[0]


def _import_polyhaven(asset_id: str) -> list[bpy.types.Object]:
    import blender_common as bc  # type: ignore

    row = next(r for r in json.loads(PH_MAP.read_text())["assets"] if r["id"] == asset_id)
    gltf = _poly_gltf(row["polyhavenId"])
    bpy.ops.import_scene.gltf(filepath=str(gltf))
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not mesh_obs:
        raise RuntimeError(f"{asset_id}: poly import empty")
    if asset_id == "garage-roll-door":
        pass
    elif len(mesh_obs) > 12:
        import cc0_common as cc0  # type: ignore

        mesh_obs = [cc0.merge_mesh_objects(mesh_obs, f"{asset_id}_mesh")]
    span = 0.0
    import cc0_common as cc0  # type: ignore

    for ob in mesh_obs:
        lo, hi = cc0.world_bbox(ob)
        span = max(span, max(hi - lo))
    target = float(row.get("targetMaxDim", 1.3))
    mul = float(row.get("scaleMul", 1.0))
    if span > 1e-6:
        factor = (target * mul) / span
        for ob in mesh_obs:
            ob.scale = (ob.scale.x * factor, ob.scale.y * factor, ob.scale.z * factor)
        bpy.context.view_layer.update()
        for ob in mesh_obs:
            bpy.ops.object.select_all(action="DESELECT")
            ob.select_set(True)
            bpy.context.view_layer.objects.active = ob
            bpy.ops.object.transform_apply(scale=True)
    bc.snap_parts_to_ground(mesh_obs)
    if asset_id == "garage-roll-door":
        for ob in mesh_obs:
            _roll_stand_wall_object(ob)
            _center_xz(ob)
            _snap_floor(ob)
    return mesh_obs


def build_folding_chair() -> list[bpy.types.Object]:
    import cc0_common as cc0  # type: ignore

    path = cc0.kenney_furniture_glb("chairDesk.glb")
    meshes = cc0.import_gltf(path, "ChairSeat")
    chair = cc0.merge_mesh_objects(meshes, "ChairSeat")
    _orient_kenney_y_up(chair)
    _scale_height(chair, 0.86)
    _center_xz(chair)
    _snap_floor(chair)
    return [chair]


def _verify_crt_on_desktop(desk: bpy.types.Object, monitor: bpy.types.Object) -> None:
    """Numeric gate: CRT base on desk top, centred on desktop X/Z (±1 cm)."""
    import cc0_common as cc0  # type: ignore

    bpy.context.view_layer.update()
    d_lo, d_hi = cc0.world_bbox(desk)
    m_lo, m_hi = cc0.world_bbox(monitor)
    gap_y = abs(m_lo.y - d_hi.y)
    print(
        f"PASS22_VERIFY desk_top_y={d_hi.y:.4f} crt_base_y={m_lo.y:.4f} gap_y={gap_y:.4f} "
        f"desk_x=[{d_lo.x:.4f},{d_hi.x:.4f}] crt_x=[{m_lo.x:.4f},{m_hi.x:.4f}] "
        f"desk_z=[{d_lo.z:.4f},{d_hi.z:.4f}] crt_z=[{m_lo.z:.4f},{m_hi.z:.4f}]"
    )
    if gap_y > 0.010:
        raise RuntimeError(f"desk-crt: CRT base not on desk top (gap_y={gap_y:.4f}m)")
    if m_lo.x < d_lo.x - 0.001 or m_hi.x > d_hi.x + 0.001:
        raise RuntimeError("desk-crt: CRT X extent outside desktop")
    m_ctr = (m_lo + m_hi) * 0.5
    d_ctr = (d_lo + d_hi) * 0.5
    if abs(m_ctr.z - d_ctr.z) > 0.025:
        raise RuntimeError(f"desk-crt: CRT not centred on desktop Z (delta={abs(m_ctr.z - d_ctr.z):.4f}m)")


def _procedural_box(
    name: str, size: tuple[float, float, float], location: tuple[float, float, float]
) -> bpy.types.Object:
    """Metre-sized box (add_cube scale is half-extent; pass 2x size for target dims)."""
    import blender_common as bc  # type: ignore

    return bc.add_cube(name, (size[0] * 2.0, size[1] * 2.0, size[2] * 2.0), location)


def build_crt_monitor() -> list[bpy.types.Object]:
    """Boxy CRT (depth > width), teal screen, origin at base centre (Y=0)."""
    import blender_common as bc  # type: ignore

    width = 0.36
    height = 0.40
    depth = 0.52
    monitor = _procedural_box("CrtMonitor", (width, height, depth), (0.0, height * 0.5, depth * 0.5))
    _center_xz(monitor)
    _snap_floor(monitor)
    bc.assign_crt_screen_material(
        monitor, (0.12, 0.78, 0.52, 1.0), bezel=(0.18, 0.20, 0.24, 1.0), screen_mix=0.62, emission_strength=6.5
    )
    return [monitor]


def _place_crt_on_desktop(screen: bpy.types.Object, desk: bpy.types.Object) -> None:
    """CRT centred on desktop top, base flush on surface, screen facing +Z."""
    import cc0_common as cc0  # type: ignore

    bpy.context.view_layer.update()
    d_lo, d_hi = cc0.world_bbox(desk)
    screen.location = desk.location.copy()
    _center_on_part(screen, desk, dx=0.0, dz=0.0)
    screen.rotation_euler = (0.0, 0.0, 0.0)
    bpy.context.view_layer.update()
    _place_on_top(screen, desk, overlap=0.002)
    bpy.context.view_layer.update()


def _center_on_part(upper: bpy.types.Object, lower: bpy.types.Object, *, dx: float = 0.0, dz: float = 0.0) -> None:
    import cc0_common as cc0  # type: ignore

    bpy.context.view_layer.update()
    lo, hi = cc0.world_bbox(lower)
    ctr = (lo + hi) * 0.5
    ulo, uhi = cc0.world_bbox(upper)
    uctr = (ulo + uhi) * 0.5
    upper.location.x += ctr.x - uctr.x + dx
    upper.location.z += ctr.z - uctr.z + dz
    bpy.context.view_layer.update()


def _apply_all_transforms(parts: list[bpy.types.Object]) -> None:
    bpy.context.view_layer.update()
    ordered = [p for p in parts if p.parent] + [p for p in parts if not p.parent]
    for ob in ordered:
        if ob.type != "MESH":
            continue
        bpy.ops.object.select_all(action="DESELECT")
        ob.select_set(True)
        bpy.context.view_layer.objects.active = ob
        bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)


def finalize_desk_crt_for_export(parts: list[bpy.types.Object]) -> None:
    """Parent CRT to desk; bake transforms per mesh after parenting (export-time Ctrl+A)."""
    from garage_assembly import bake_parts_local_transforms  # type: ignore

    desk = next((p for p in parts if p.name.lower().startswith("desktop")), None)
    screen = next((p for p in parts if "crt" in p.name.lower()), None)
    keyboard = next((p for p in parts if "keyboard" in p.name.lower()), None)
    if desk is not None and screen is not None:
        _parent_keep_world(screen, desk)
    bpy.context.view_layer.update()
    free_parts = [p for p in parts if p.type == "MESH" and p is not screen]
    if screen is not None:
        bake_parts_local_transforms([screen])
    bake_parts_local_transforms(free_parts)
    bpy.context.view_layer.update()


def _finalize_desk_crt_parts(parts: list[bpy.types.Object]) -> list[bpy.types.Object]:
    desk = next((p for p in parts if p.name.lower().startswith("desktop")), None)
    screen = next((p for p in parts if "crt" in p.name.lower()), None)
    if desk is not None and screen is not None:
        _parent_keep_world(screen, desk)
    return parts


def build_desk_base() -> list[bpy.types.Object]:
    """Pass16 Kenney desk + keyboard (monitor is garage-crt-monitor)."""
    desk = _import_kenney_furniture("desk.glb", "Desktop")
    _orient_kenney_y_up(desk)
    _scale_height(desk, 0.76)
    _center_xz(desk)
    _snap_floor(desk)
    keyboard = _import_kenney_furniture("computerKeyboard.glb", "KeyboardGrey")
    _scale_height(keyboard, 0.04)
    keyboard.location = desk.location.copy()
    _center_on_part(keyboard, desk, dx=-0.08, dz=0.10)
    _place_on_top(keyboard, desk, overlap=0.008)
    return [desk, keyboard]


def build_desk_crt() -> list[bpy.types.Object]:
    import cc0_common as cc0  # type: ignore

    desk, keyboard = build_desk_base()
    monitor = cc0.merge_mesh_objects(build_crt_monitor(), "CrtMonitor")
    monitor.location = desk.location.copy()
    _place_crt_on_desktop(monitor, desk)
    _verify_crt_on_desktop(desk, monitor)
    return [desk, monitor, keyboard]


def build_coffee_drip() -> list[bpy.types.Object]:
    table = _import_kenney_furniture("sideTable.glb", "DripBase")
    _scale_height(table, 0.55)
    _center_xz(table)
    _snap_floor(table)
    lamp = _import_kenney_furniture("lampRoundTable.glb", "DripTower")
    _scale_height(lamp, 0.38)
    lamp.location = table.location.copy()
    _place_on_top(lamp, table)
    return [table, lamp]


def build_crt_tower() -> list[bpy.types.Object]:
    rack = _import_kenney_furniture("bookcaseClosed.glb", "TowerFrame")
    _scale_height(rack, 1.55)
    _center_xz(rack)
    _snap_floor(rack)
    screen = _import_kenney_furniture("computerScreen.glb", "TowerCrt")
    _scale_height(screen, 0.38)
    screen.location = rack.location.copy()
    screen.location.z += 0.02
    _place_on_top(screen, rack)
    spk = _import_kenney_furniture("speaker.glb", "TowerSpeaker")
    _scale_height(spk, 0.32)
    spk.location = rack.location.copy()
    spk.location.x += 0.22
    spk.location.z += 0.08
    _place_on_top(spk, rack)
    return [rack, screen, spk]


def _roll_stand_wall_object(ob: bpy.types.Object) -> None:
    """Poly Haven rollershutter: stand Z-tall import vertical (Y-up wall, face toward +Z)."""
    import cc0_common as cc0  # type: ignore

    bpy.context.view_layer.update()
    lo, hi = cc0.world_bbox(ob)
    ext = hi - lo
    if ext.z > ext.y * 1.05 and ext.z >= ext.x * 0.85:
        ob.rotation_euler.x = -math.pi / 2.0
        bpy.ops.object.select_all(action="DESELECT")
        ob.select_set(True)
        bpy.context.view_layer.objects.active = ob
        bpy.ops.object.transform_apply(rotation=True)
        bpy.context.view_layer.update()


def build_roll_door() -> list[bpy.types.Object]:
    """Room-kit style roll door: side posts, top housing, solid horizontal-slat curtain (Y-up wall)."""
    import blender_common as bc  # type: ignore
    import cc0_common as cc0  # type: ignore

    width = 1.02
    height = 2.35
    depth = 0.12
    post_w = 0.10
    gap_in = width - post_w * 2.0
    cz = depth * 0.55
    parts: list[bpy.types.Object] = []
    for tag, x in (("L", -width * 0.5 + post_w * 0.5), ("R", width * 0.5 - post_w * 0.5)):
        parts.append(_procedural_box(f"RollPost{tag}", (post_w, height, depth), (x, height * 0.5, cz)))
    parts.append(_procedural_box("RollHousing", (width, 0.16, depth * 1.05), (0.0, height - 0.08, cz)))
    curtain_h = height * 0.66
    curtain_base = 0.10
    curtain_w = gap_in + post_w * 0.35
    parts.append(
        _procedural_box(
            "RollCurtain",
            (curtain_w, curtain_h, depth * 0.95),
            (0.0, curtain_base + curtain_h * 0.5, cz),
        )
    )
    bc.snap_parts_to_ground(parts)
    bpy.context.view_layer.update()
    lo = Vector((1e9, 1e9, 1e9))
    hi = Vector((-1e9, -1e9, -1e9))
    import cc0_common as cc0  # type: ignore

    for ob in parts:
        mn, mx = cc0.world_bbox(ob)
        lo.x, lo.y, lo.z = min(lo.x, mn.x), min(lo.y, mn.y), min(lo.z, mn.z)
        hi.x, hi.y, hi.z = max(hi.x, mx.x), max(hi.y, mx.y), max(hi.z, mx.z)
    cx, cz = (lo.x + hi.x) * 0.5, (lo.z + hi.z) * 0.5
    for ob in parts:
        ob.location.x -= cx
        ob.location.z -= cz
    bc.snap_parts_to_ground(parts)
    return parts


def build_server_closet() -> list[bpy.types.Object]:
    ob = _import_kenney_furniture("bookcaseClosed.glb", "ServerCabinet")
    _orient_kenney_y_up(ob)
    _scale_height(ob, 1.88)
    _center_xz(ob)
    _snap_floor(ob)
    return [ob]


def build_lot_curb() -> list[bpy.types.Object]:
    ob = _import_kenney_building("border-high.glb", "LotCurb")
    _scale_height(ob, 0.35)
    _center_xz(ob)
    _snap_floor(ob)
    return [ob]


def build_land_sign() -> list[bpy.types.Object]:
    table = _import_kenney_furniture("sideTable.glb", "SignPost")
    _scale_height(table, 1.25)
    _center_xz(table)
    _snap_floor(table)
    box = _import_kenney_furniture("cardboardBoxClosed.glb", "SignBoard")
    _scale_height(box, 0.42)
    box.location = table.location.copy()
    _place_on_top(box, table)
    return [table, box]


def build_whiteboard() -> list[bpy.types.Object]:
    return _import_polyhaven("garage-whiteboard-1995")


def build_window_frame() -> list[bpy.types.Object]:
    ob = _import_kenney_building("wall-window-square.glb", "WindowFrame")
    _scale_height(ob, 1.35)
    _center_xz(ob)
    _snap_floor(ob)
    return [ob]


def build_wall_panel() -> list[bpy.types.Object]:
    wall = _import_kenney_building("wall-half.glb", "PanelWall")
    _scale_height(wall, 1.55)
    _center_xz(wall)
    _snap_floor(wall)
    plate = _import_kenney_building("plating.glb", "PanelPlate")
    _scale_height(plate, 0.55)
    plate.location = wall.location.copy()
    plate.location.x += 0.02
    _place_on_top(plate, wall, overlap=0.0)
    return [wall, plate]


def build_exterior_wall() -> list[bpy.types.Object]:
    wall = _import_kenney_building("wall.glb", "ExtWall")
    _scale_height(wall, 2.45)
    _center_xz(wall)
    _snap_floor(wall)
    win = _import_kenney_building("wall-window-square.glb", "ExtWindow")
    _scale_height(win, 1.05)
    win.location = wall.location.copy()
    win.location.y += 1.05
    win.location.x += 0.04
    bpy.context.view_layer.update()
    import cc0_common as cc0  # type: ignore

    lo_w, hi_w = cc0.world_bbox(wall)
    lo_win, hi_win = cc0.world_bbox(win)
    win.location.y += (lo_w.y + hi_w.y) * 0.55 - (lo_win.y + hi_win.y) * 0.5
    return [wall, win]


def build_floor_tile() -> list[bpy.types.Object]:
    ob = _import_kenney_building("floor.glb", "FloorSlab")
    import cc0_common as cc0  # type: ignore

    _center_xz(ob)
    bpy.context.view_layer.update()
    lo, hi = cc0.world_bbox(ob)
    span_xz = max(hi.x - lo.x, hi.z - lo.z, 0.5)
    target = 1.0
    if span_xz > 1e-6:
        s = target / span_xz
        ob.scale = (ob.scale.x * s, ob.scale.y, ob.scale.z * s)
        bpy.ops.object.select_all(action="DESELECT")
        ob.select_set(True)
        bpy.context.view_layer.objects.active = ob
        bpy.ops.object.transform_apply(scale=True)
    thickness = max(hi.y - lo.y, 1e-4)
    slab = 0.08 / thickness
    ob.scale.y *= slab
    bpy.ops.object.transform_apply(scale=True)
    _snap_floor(ob)
    return [ob]


def build_voodude_rack() -> list[bpy.types.Object]:
    rack = _import_kenney_furniture("bookcaseClosed.glb", "VooRack")
    _scale_height(rack, 1.35)
    _center_xz(rack)
    _snap_floor(rack)
    books = _import_kenney_furniture("books.glb", "VooBooks")
    _scale_height(books, 0.22)
    books.location = rack.location.copy()
    books.location.y += 0.55
    books.location.z += 0.05
    bpy.context.view_layer.update()
    import cc0_common as cc0  # type: ignore

    _lo, hi = cc0.world_bbox(rack)
    lo_b, _ = cc0.world_bbox(books)
    _place_on_top(books, rack, overlap=0.010)
    spk = _import_kenney_furniture("speaker.glb", "VooSpeaker")
    _scale_height(spk, 0.28)
    spk.location = rack.location.copy()
    spk.location.x += 0.16
    spk.location.z += 0.04
    _place_on_top(spk, rack, overlap=0.010)
    return [rack, books, spk]


BUILDERS = {
    "garage-folding-chair": build_folding_chair,
    "garage-crt-monitor": build_crt_monitor,
    "garage-desk-crt": build_desk_crt,
    "garage-coffee-drip": build_coffee_drip,
    "garage-crt-tower": build_crt_tower,
    "garage-roll-door": build_roll_door,
    "garage-server-closet": build_server_closet,
    "garage-lot-curb": build_lot_curb,
    "garage-land-for-sale-sign": build_land_sign,
    "garage-whiteboard-1995": build_whiteboard,
    "garage-window-frame": build_window_frame,
    "garage-wall-panel": build_wall_panel,
    "garage-exterior-wall": build_exterior_wall,
    "garage-floor-tile": build_floor_tile,
    "garage-voodude-rack": build_voodude_rack,
}


def build_kenney_garage(asset_id: str) -> dict:
    import blender_common as bc  # type: ignore

    os.environ["GARAGE_KENNEY_IMPORT"] = "1"
    os.environ["GARAGE_CC0_IMPORT"] = "1"
    bc.reset_scene()
    if asset_id not in BUILDERS:
        raise SystemExit(f"no kenney builder for {asset_id}")
    parts = BUILDERS[asset_id]()
    if asset_id == "garage-desk-crt":
        finalize_desk_crt_for_export(parts)
    bc.flatten_assembly_parts(parts)
    bc.snap_parts_to_ground(parts)
    row = next(r for r in json.loads(MAP.read_text(encoding="utf-8"))["assets"] if r["id"] == asset_id)
    license_note = row.get("license", "CC0-1.0 (Kenney, restyled in-repo)")
    entry = bc.finish_asset(asset_id, "garage-kit", license_note=license_note)
    entry["source"] = "kenney-cc0" if "Kenney" in license_note else "cc0-polyhaven"
    entry["generator"] = "tools/art/blender/kenney_garage_asset.py"
    return entry


def main() -> None:
    import blender_common as bc  # type: ignore

    if len(sys.argv) < 2:
        print("usage: kenney_garage_asset.py <garage-id|ALL>", file=sys.stderr)
        raise SystemExit(2)
    arg = sys.argv[-1]
    ids = [a["id"] for a in json.loads(MAP.read_text(encoding="utf-8"))["assets"]]
    targets = ids if arg == "ALL" else [arg]
    entries: list[dict] = []
    for asset_id in targets:
        entries.append(build_kenney_garage(asset_id))
        print("OK", asset_id, "kenney")
    if arg == "ALL":
        manifest = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"
        payload = json.loads(manifest.read_text(encoding="utf-8"))
        by_id = {e["id"]: e for e in entries}
        for i, a in enumerate(payload["assets"]):
            if a["id"] in by_id:
                payload["assets"][i] = {**a, **by_id[a["id"]]}
        payload["fidelity"] = "kenney-import-restyle"
        manifest.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    import blender_common as bc  # noqa: E402

    def _job():
        main()
        return None

    bc.run_script(_job)