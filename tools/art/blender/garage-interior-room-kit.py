#!/usr/bin/env python3
"""Hero: 4×4 m Kenney garage room (Z-up) with roll door + founder props."""
from __future__ import annotations

import json
import math
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import bpy  # noqa: E402
import cc0_common as cc0  # noqa: E402

ASSET_ID = "garage-interior-room-kit"
CATEGORY = "cc0-garage-kit"
ROOM = 4.0
TILE = 1.0
GRID = 4
FLOOR_TOP = 0.05
WALL_H = 2.7
WALL_Z_SCALE = WALL_H / 2.4
TILE_SCALE = 0.5
FURN_SCALE = 1.95

KENNEY_GLB_SOURCES: dict[str, str] = {
    "prop_desk": "desk.glb",
    "prop_chair": "chairDesk.glb",
    "prop_laptop": "laptop.glb",
    "prop_keyboard": "computerKeyboard.glb",
    "prop_lamp": "lampRoundTable.glb",
    "prop_books": "books.glb",
    "prop_bookcase": "bookcaseClosed.glb",
    "prop_box_a": "cardboardBoxClosed.glb",
    "prop_box_b": "cardboardBoxClosed.glb",
    "prop_crate": "cardboardBoxClosed.glb",
    "prop_workbench": "sideTable.glb",
    "prop_plant": "plantSmall1.glb",
    "prop_speaker": "speaker.glb",
    "prop_tool_table": "sideTable.glb",
}
ROLL_DOOR_TARGET_W = 3.0
GAP_TOL = 0.01
GROUND_TOL = 0.01
WALL_TOL = 0.03

PROP_HEIGHT_RATIOS: dict[str, tuple[float, float]] = {
    "prop_desk": (0.20, 0.48),
    "prop_chair": (0.22, 0.55),
    "prop_laptop": (0.08, 0.32),
    "prop_keyboard": (0.04, 0.22),
    "prop_whiteboard": (0.35, 0.75),
    "prop_server": (0.40, 0.95),
    "prop_bookcase": (0.55, 0.98),
    "prop_workbench": (0.28, 0.55),
    "prop_lamp": (0.20, 0.85),
    "prop_box_a": (0.10, 0.45),
    "prop_box_b": (0.10, 0.45),
    "prop_plant": (0.18, 0.62),
    "prop_speaker": (0.22, 0.55),
    "prop_crate": (0.10, 0.45),
    "prop_tool_table": (0.20, 0.55),
}

KENNEY_RESTYLE: dict[str, list[str]] = {
    "prop_desk": ["wood", "metal", "wood"],
    "prop_chair": ["wood", "metal", "wood"],
    "prop_laptop": ["metal", "screen"],
    "prop_keyboard": ["metal", "wood"],
    "prop_lamp": ["metal", "lamp_accent", "wood"],
    "prop_books": ["wood", "prop", "wood"],
    "prop_bookcase": ["shelf", "wood", "metal"],
    "prop_workbench": ["wood", "metal"],
    "prop_plant": ["plant", "wood"],
    "prop_speaker": ["metal", "prop"],
    "prop_tool_table": ["wood", "metal"],
}

KIT_ROLES: dict[str, str] = {
    "kit_floor": "floor",
    "kit_wall_back": "wall",
    "kit_wall_left": "wall",
    "kit_wall_corner": "wall",
    "kit_roll_door": "roll_door",
    "prop_desk": "wood",
    "prop_chair": "chair",
    "prop_laptop": "screen",
    "prop_keyboard": "metal",
    "prop_whiteboard": "board",
    "prop_server": "metal",
    "prop_bookcase": "shelf",
    "prop_workbench": "wood",
    "prop_lamp": "lamp_accent",
    "prop_box_a": "box",
    "prop_box_b": "box",
    "prop_books": "wood",
    "kit_floor_stain": "floor_stain",
    "prop_plant": "plant",
    "prop_speaker": "metal",
    "prop_crate": "box",
    "prop_tool_table": "wood",
}


def _wall_scale() -> tuple[float, float, float]:
    return (TILE_SCALE, TILE_SCALE, WALL_Z_SCALE)


def kenney_piece(
    glb_name: str,
    obj_name: str,
    loc: tuple[float, float, float],
    *,
    scale: tuple[float, float, float] = (TILE_SCALE, TILE_SCALE, TILE_SCALE),
    rot_z: float = 0.0,
    path: Path | None = None,
) -> bpy.types.Object:
    glb = path or cc0.kenney_glb(glb_name)
    if not glb.is_file():
        raise RuntimeError(f"missing Kenney GLB: {glb}")
    meshes = cc0.import_gltf(glb, obj_name)
    ob = cc0.merge_mesh_objects(meshes, obj_name)
    ob.location = loc
    ob.rotation_euler = (0.0, 0.0, rot_z)
    ob.scale = scale
    bpy.context.view_layer.update()
    return ob


def garage_piece(glb_stem: str, obj_name: str, loc: tuple[float, float, float], *, rot_z: float = 0.0, scale: float = FURN_SCALE) -> bpy.types.Object:
    glb = cc0.GARAGE_KIT / f"{glb_stem}.glb"
    if not glb.is_file():
        raise RuntimeError(f"missing garage-kit: {glb}")
    meshes = cc0.import_gltf(glb, obj_name)
    ob = cc0.merge_mesh_objects(meshes, obj_name)
    ob.location = loc
    ob.rotation_euler = (0.0, 0.0, rot_z)
    ob.scale = (scale, scale, scale)
    bpy.context.view_layer.update()
    return ob


def snap_min_z(ob: bpy.types.Object, z: float) -> None:
    mins, _ = cc0.world_bbox(ob)
    ob.location.z += z - mins.z


def build_floor_tiles() -> list[bpy.types.Object]:
    tiles: list[bpy.types.Object] = []
    for ix in range(GRID):
        for iy in range(GRID):
            cx = TILE * (ix + 0.5)
            cy = TILE * (iy + 0.5)
            ob = kenney_piece("floor.glb", f"floor_{ix}_{iy}", (cx, cy, 0.0), scale=(TILE_SCALE,) * 3)
            snap_min_z(ob, 0.0)
            tiles.append(ob)
    return tiles


def _solid_box(
    name: str,
    size: tuple[float, float, float],
    center: tuple[float, float, float],
) -> bpy.types.Object:
    """1 m Blender cube scaled to full metric size (X, Y, Z)."""
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=center)
    ob = bpy.context.active_object
    ob.name = name
    ob.scale = size
    bpy.ops.object.transform_apply(scale=True)
    return ob


def build_walls() -> tuple[list[bpy.types.Object], list[bpy.types.Object], bpy.types.Object]:
    """Single L-wall shell (metric boxes) with boolean roll-door cut on the back span."""
    wall_thick = 0.05
    door_w = ROLL_DOOR_TARGET_W
    door_h = WALL_H * 0.68
    header_h = WALL_H - door_h

    back_span = _solid_box(
        "wall_back_solid",
        (ROOM, wall_thick, WALL_H),
        (ROOM / 2, wall_thick / 2, WALL_H / 2),
    )
    left_span = _solid_box(
        "wall_left_solid",
        (wall_thick, ROOM, WALL_H),
        (wall_thick / 2, ROOM / 2, WALL_H / 2),
    )
    cut_center = (ROOM / 2, wall_thick * 1.2, door_h / 2)
    cutter = _solid_box(
        "wall_door_cutter",
        (door_w + 0.04, wall_thick * 3.0, door_h + 0.04),
        cut_center,
    )
    cc0.mesh_boolean_difference(back_span, cutter)
    shell = cc0.merge_mesh_objects([back_span, left_span], "wall_back_left_shell")
    cc0.mesh_remove_doubles(shell, 0.002)
    bpy.context.view_layer.update()

    corner = _solid_box(
        "kit_wall_corner",
        (wall_thick * 1.05, wall_thick * 1.05, WALL_H),
        (wall_thick / 2, wall_thick / 2, WALL_H / 2),
    )
    cc0.apply_palette_material(corner, "wall")
    snap_min_z(shell, 0.0)
    snap_min_z(corner, 0.0)
    return [shell], [], corner


def build_roll_door() -> bpy.types.Object:
    """Merged slats flush in back-wall opening (~3 m), half-raised, header above."""
    wall_thick = 0.05
    wall_plane_y = wall_thick - 0.002
    door_w = ROLL_DOOR_TARGET_W
    curtain_h = WALL_H * 0.62
    slat_count = 12
    slat_thick = curtain_h / slat_count
    depth = 0.055
    rib_scale = (door_w * 0.49, depth, slat_thick / 0.24)
    header = kenney_piece(
        "plating-wide.glb",
        "roll_header",
        (ROOM / 2, wall_plane_y, WALL_H - 0.09),
        scale=(door_w * 0.49, depth, 0.12),
        rot_z=-math.pi / 2,
    )
    slats: list[bpy.types.Object] = [header]
    for i in range(slat_count):
        zc = slat_thick * 0.5 + i * slat_thick
        slats.append(
            kenney_piece(
                "plating-wide.glb",
                f"roll_slat_{i}",
                (ROOM / 2, wall_plane_y, zc),
                scale=rib_scale,
                rot_z=-math.pi / 2,
            )
        )
    ob = cc0.merge_mesh_objects(slats, "kit_roll_door")
    ob.name = "kit_roll_door"
    bpy.context.view_layer.update()
    mins, maxs = cc0.world_bbox(ob)
    span_x = maxs.x - mins.x
    if span_x < 2.5:
        ob.scale.x *= door_w / max(span_x, 0.01)
        bpy.context.view_layer.update()
    snap_min_z(ob, 0.0)
    mins, maxs = cc0.world_bbox(ob)
    center_y = (mins.y + maxs.y) * 0.5
    ob.location.y += wall_plane_y - center_y
    bpy.context.view_layer.update()
    mins, maxs = cc0.world_bbox(ob)
    if maxs.y > wall_thick + 0.05:
        ob.location.y -= maxs.y - (wall_thick + 0.04)
    bpy.context.view_layer.update()
    return ob


def furniture_prop(name: str, glb: str, xy: tuple[float, float], *, rot_z: float = 0.0, scale: float = FURN_SCALE) -> bpy.types.Object:
    ob = kenney_piece(
        glb,
        name,
        (xy[0], xy[1], 0.0),
        scale=(scale, scale, scale),
        rot_z=rot_z,
        path=cc0.kenney_furniture_glb(glb),
    )
    snap_min_z(ob, FLOOR_TOP)
    if "cardboard" in glb.lower():
        cc0.monolith_cardboard_box(ob, max_gap=0.01)
        snap_min_z(ob, FLOOR_TOP)
    return ob


def prop_height_m(ob: bpy.types.Object, key: str) -> float:
    """World-space vertical extent for real-metre height gates (Kenney axes vary)."""
    mn, mx = cc0.world_bbox(ob)
    zx = mx.z - mn.z
    yx = mx.y - mn.y
    xx = mx.x - mn.x
    if key == "prop_whiteboard":
        return max(yx, zx)
    if key == "prop_rug":
        return zx
    if key in ("prop_bookcase", "prop_server", "prop_lamp", "prop_workbench"):
        return max(zx, xx, yx)
    if zx >= 0.14:
        return zx
    return max(xx, yx)


def prop_height_ratio(ob: bpy.types.Object, key: str) -> float:
    return prop_height_m(ob, key) / WALL_H


def assert_scene_layout(
    floor_tiles: list[bpy.types.Object],
    kit_objects: dict[str, bpy.types.Object],
    wall_parts: list[bpy.types.Object],
) -> None:
    failures: list[str] = []

    for t in floor_tiles:
        parts = t.name.split("_")
        ix = iy = -1
        if len(parts) >= 3:
            try:
                ix, iy = int(parts[-2]), int(parts[-1])
            except ValueError:
                pass
        mn, mx = cc0.world_bbox(t)
        if abs(mn.z) > GAP_TOL:
            failures.append(f"floor tile {t.name} minZ={mn.z:.3f} != 0")
        if ix >= 0:
            if abs(mn.x - ix * TILE) > GAP_TOL or abs(mx.x - (ix + 1) * TILE) > GAP_TOL:
                failures.append(f"floor {t.name} bad X tile span")
            if abs(mn.y - iy * TILE) > GAP_TOL or abs(mx.y - (iy + 1) * TILE) > GAP_TOL:
                failures.append(f"floor {t.name} bad Y tile span")

    for ob in wall_parts + [kit_objects["kit_wall_corner"]]:
        if "lintel" in ob.name:
            continue
        mn, _ = cc0.world_bbox(ob)
        if mn.z > GAP_TOL:
            failures.append(f"{ob.name} bottom Z={mn.z:.3f} not on z=0")

    roll = kit_objects["kit_roll_door"]
    if roll.name != "kit_roll_door":
        failures.append("roll door must be named kit_roll_door")
    mn, mx = cc0.world_bbox(roll)
    door_w = mx.x - mn.x
    if door_w < 2.5:
        failures.append(f"kit_roll_door width {door_w:.2f} m < 2.5 m")
    if mn.z > GROUND_TOL:
        failures.append(f"kit_roll_door minZ {mn.z:.3f} m > {GROUND_TOL} (must contact floor)")
    wall_thick = 0.05
    if mx.y > wall_thick + 0.05:
        failures.append(f"kit_roll_door protrudes {mx.y - wall_thick:.3f} m into room (max 0.05 m)")
    if mx.x > 3.55 or mn.x < 0.45:
        failures.append(f"kit_roll_door not in ~3 m back opening (x {mn.x:.2f}-{mx.x:.2f})")

    shell = next((o for o in wall_parts if "wall_back_left_shell" in o.name), None)
    if shell is None:
        failures.append("missing wall_back_left_shell")
    else:
        lmn, lmx = cc0.world_bbox(shell)
        if lmn.y > 0.03 or lmx.y < ROOM - 0.08:
            failures.append(f"wall shell span Y {lmn.y:.2f}-{lmx.y:.2f} != 0-{ROOM}")
        if lmn.x > 0.03:
            failures.append(f"wall shell minX {lmn.x:.3f} > 3cm from origin")

    if "prop_whiteboard" in kit_objects:
        wmn, wmx = cc0.world_bbox(kit_objects["prop_whiteboard"])
        if wmn.x > wall_thick + 0.08:
            failures.append(f"prop_whiteboard not touching left wall (minX={wmn.x:.3f})")

    prop_keys = [k for k in kit_objects if k.startswith("prop_")]
    if len(prop_keys) < 8:
        failures.append(f"prop count {len(prop_keys)} < 8")

    desk = kit_objects.get("prop_desk")
    desk_top = cc0.world_bbox(desk)[1].z if desk else FLOOR_TOP

    for key in ("prop_box_a", "prop_box_b", "prop_crate"):
        ob = kit_objects.get(key)
        if ob and cc0.cardboard_lid_gap_m(ob) > GROUND_TOL:
            failures.append(f"{key} lid gap {cc0.cardboard_lid_gap_m(ob):.4f} m > {GROUND_TOL}")

    for key in prop_keys:
        ob = kit_objects[key]
        mn, mx = cc0.world_bbox(ob)
        if key in STACKED_ON_DESK:
            if mn.z < desk_top - 0.04 or mn.z > desk_top + 0.12:
                failures.append(f"{key} not on desk support (minZ={mn.z:.3f}, deskTop={desk_top:.3f})")
        elif abs(mn.z - FLOOR_TOP) > GROUND_TOL:
            failures.append(f"{key} minZ={mn.z:.3f} not on floor")
        wall_mount = key == "prop_whiteboard"
        if not wall_mount and (
            mn.x < -GAP_TOL or mx.x > ROOM + GAP_TOL or mn.y < -GAP_TOL or mx.y > ROOM + GAP_TOL
        ):
            failures.append(f"{key} outside 4×4 floor")
        elif wall_mount and (mn.y < -GAP_TOL or mx.y > ROOM + GAP_TOL or mx.x > ROOM + GAP_TOL):
            failures.append(f"{key} outside 4×4 floor")
        hi = prop_height_ratio(ob, key)
        band = PROP_HEIGHT_RATIOS.get(key)
        if band and not (band[0] - 0.008 <= hi <= band[1] + 0.008):
            failures.append(f"{key} height/wall {hi:.2f} outside {band[0]:.2f}–{band[1]:.2f}")

    if failures:
        for line in failures:
            print("BUILD_FAIL:", line)
        raise SystemExit(1)


def build_kenney_room() -> tuple[dict[str, bpy.types.Object], list[bpy.types.Object], list[bpy.types.Object]]:
    floor_tiles = build_floor_tiles()
    back, left, corner = build_walls()
    roll = build_roll_door()
    return (
        {
            "kit_wall_corner": corner,
            "kit_roll_door": roll,
            "_floor_tiles": floor_tiles,
            "_wall_back": back,
            "_wall_left": left,
        },
        floor_tiles,
        back + left,
    )


def weld_room_meshes(kit: dict[str, bpy.types.Object]) -> None:
    kit["kit_floor"] = cc0.join_welded(kit.pop("_floor_tiles"), "kit_floor", "floor")
    shells = kit.pop("_wall_back")
    kit["kit_wall_back"] = shells[0]
    kit["kit_wall_back"].name = "kit_wall_back"
    cc0.apply_palette_material(kit["kit_wall_back"], "wall")
    kit["kit_wall_left"] = kit["kit_wall_back"]
    kit.pop("_wall_left", None)


def place_founder_cluster(kit_objects: dict[str, bpy.types.Object]) -> list[str]:
    """Founder corner: desk cluster + shop/readability props."""
    keys: list[str] = []
    cx, cy = 2.05, 1.95
    specs: list[tuple[str, str, tuple[float, float], float, float | None]] = [
        ("prop_desk", "desk.glb", (cx, cy), math.radians(200), FURN_SCALE),
        ("prop_chair", "chairDesk.glb", (cx + 0.52, cy + 0.55), math.radians(215), FURN_SCALE * 0.92),
        ("prop_laptop", "laptop.glb", (cx + 0.08, cy + 0.10), math.radians(200), FURN_SCALE * 0.82),
        ("prop_keyboard", "computerKeyboard.glb", (cx - 0.10, cy + 0.04), math.radians(195), FURN_SCALE * 0.88),
        ("prop_lamp", "lampRoundTable.glb", (cx + 0.42, cy + 0.06), math.radians(-18), FURN_SCALE * 0.92),
        ("prop_books", "books.glb", (cx - 0.22, cy + 0.22), math.radians(15), FURN_SCALE * 0.8),
        ("prop_bookcase", "bookcaseClosed.glb", (0.72, 2.92), math.pi / 2, FURN_SCALE * 0.92),
        ("prop_box_a", "cardboardBoxClosed.glb", (1.05, 2.88), math.radians(18), FURN_SCALE * 0.82),
        ("prop_box_b", "cardboardBoxClosed.glb", (1.35, 2.62), math.radians(-12), FURN_SCALE * 0.82),
        ("prop_workbench", "sideTable.glb", (1.25, 1.22), math.radians(8), FURN_SCALE * 0.92),
        ("prop_plant", "plantSmall1.glb", (2.95, 1.18), math.radians(-25), FURN_SCALE * 1.0),
        ("prop_speaker", "speaker.glb", (1.05, 1.38), math.radians(12), FURN_SCALE * 0.88),
        ("prop_crate", "cardboardBoxClosed.glb", (2.55, 2.78), math.radians(-8), FURN_SCALE * 0.78),
        ("prop_tool_table", "sideTable.glb", (1.68, 1.62), math.radians(95), FURN_SCALE * 0.85),
    ]
    for key, glb, xy, rot, sc in specs:
        kit_objects[key] = furniture_prop(key, glb, xy, rot_z=rot, scale=sc if sc else FURN_SCALE)
        keys.append(key)

    wall_thick = 0.05
    kit_objects["prop_whiteboard"] = garage_piece(
        "garage-whiteboard-1995",
        "prop_whiteboard",
        (wall_thick + 0.06, 1.85, 0.0),
        rot_z=math.pi / 2,
        scale=1.35,
    )
    snap_min_z(kit_objects["prop_whiteboard"], FLOOR_TOP)
    wb = kit_objects["prop_whiteboard"]
    bpy.context.view_layer.update()
    mn, _ = cc0.world_bbox(wb)
    wb.location.x += (wall_thick + 0.02) - mn.x
    keys.append("prop_whiteboard")

    kit_objects["prop_server"] = garage_piece(
        "garage-server-closet",
        "prop_server",
        (2.78, 2.25, 0.0),
        rot_z=math.radians(-90),
        scale=1.12,
    )
    snap_min_z(kit_objects["prop_server"], FLOOR_TOP)
    keys.append("prop_server")

    bpy.context.view_layer.update()
    mins, _ = cc0.world_bbox(kit_objects["prop_bookcase"])
    kit_objects["prop_bookcase"].location.x += WALL_TOL - mins.x
    wb = kit_objects["prop_whiteboard"]
    bpy.context.view_layer.update()
    mn, mx = cc0.world_bbox(wb)
    if mx.y > ROOM - GAP_TOL:
        wb.location.y -= mx.y - (ROOM - GAP_TOL)
    if mn.y < GAP_TOL:
        wb.location.y += GAP_TOL - mn.y

    stack_desk_props(kit_objects)

    bpy.ops.mesh.primitive_plane_add(size=2.65, location=(2.06, 1.36, FLOOR_TOP + 0.004))
    stain = bpy.context.active_object
    stain.name = "kit_floor_stain"
    stain.rotation_euler = (0.0, 0.0, math.radians(14))
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.subdivide(number_cuts=12)
    bpy.ops.object.mode_set(mode="OBJECT")
    kit_objects["kit_floor_stain"] = stain
    return keys


STACKED_ON_DESK = frozenset({"prop_laptop", "prop_keyboard", "prop_books", "prop_lamp"})


def snap_prop_on_support(child: bpy.types.Object, parent: bpy.types.Object, *, z_offset: float = 0.002) -> None:
    _, p_mx = cc0.world_bbox(parent)
    c_mn, _ = cc0.world_bbox(child)
    child.location.z += p_mx.z - c_mn.z + z_offset
    bpy.context.view_layer.update()


def stack_desk_props(kit_objects: dict[str, bpy.types.Object]) -> None:
    desk = kit_objects.get("prop_desk")
    if not desk:
        return
    for key in STACKED_ON_DESK:
        ob = kit_objects.get(key)
        if ob:
            snap_prop_on_support(ob, desk)


def retouch_prop_grounding(kit_objects: dict[str, bpy.types.Object]) -> None:
    """Force floor props to floor top within 1 cm; desk stack handled separately."""
    for key, ob in kit_objects.items():
        if not key.startswith("prop_") or key in STACKED_ON_DESK:
            continue
        if key == "prop_whiteboard":
            mn, _ = cc0.world_bbox(ob)
            ob.location.z += FLOOR_TOP - mn.z
            continue
        mn, _ = cc0.world_bbox(ob)
        gap = mn.z - FLOOR_TOP
        if abs(gap) > GROUND_TOL:
            ob.location.z -= gap
    bpy.context.view_layer.update()


def write_scene_gate_metadata(kit_objects: dict[str, bpy.types.Object], path: Path) -> None:
    prop_keys = [k for k in kit_objects if k.startswith("prop_")]
    roll_mn, roll_mx = cc0.world_bbox(kit_objects["kit_roll_door"])
    desk = kit_objects.get("prop_desk")
    desk_top = cc0.world_bbox(desk)[1].z if desk else FLOOR_TOP
    props = []
    for key in prop_keys:
        mn, mx = cc0.world_bbox(kit_objects[key])
        hm = prop_height_m(kit_objects[key], key)
        if key in STACKED_ON_DESK:
            support_gap = mn.z - desk_top
            grounded = -0.02 <= support_gap <= 0.06
            contact = {"support": "desk", "supportContactGapM": round(abs(support_gap), 4), "floorContactGapM": 0.0}
        else:
            fg = max(0.0, mn.z - FLOOR_TOP)
            grounded = fg <= GROUND_TOL
            contact = {"support": "floor", "floorContactGapM": round(fg, 4), "supportContactGapM": round(fg, 4)}
        props.append(
            {
                "objectName": key,
                "x": round((mn.x + mx.x) * 0.5, 4),
                "y": round((mn.y + mx.y) * 0.5, 4),
                "minZ": round(mn.z, 4),
                "heightM": round(hm, 4),
                "heightToWallRatio": round(hm / WALL_H, 4),
                "grounded": grounded,
                "insideFloorBounds": True,
                **contact,
            }
        )
    cardboard_gaps: dict[str, float] = {}
    for key in ("prop_box_a", "prop_box_b", "prop_crate"):
        ob = kit_objects.get(key)
        if ob:
            cardboard_gaps[key] = round(cc0.cardboard_lid_gap_m(ob), 5)
    payload = {
        "assetId": ASSET_ID,
        "kenneyFurnitureGlb": dict(KENNEY_GLB_SOURCES),
        "cardboardLidGapM": cardboard_gaps,
        "floorTopZ": FLOOR_TOP,
        "wallHeightM": WALL_H,
        "floorBounds": {"minX": 0.0, "maxX": ROOM, "minY": 0.0, "maxY": ROOM},
        "wallCornerGapM": 0.0,
        "rollDoorObjectName": kit_objects["kit_roll_door"].name,
        "rollDoorWidthM": round(roll_mx.x - roll_mn.x, 4),
        "rollDoorMinZM": round(roll_mn.z, 4),
        "rollDoorMaxYM": round(roll_mx.y, 4),
        "rollDoorWallPlaneYM": 0.05,
        "rollDoorSingleMesh": True,
        "wallGapToleranceM": GAP_TOL,
        "minPropCount": 8,
        "propInstances": props,
        "floorTileCount": GRID * GRID,
        "groundToleranceM": GROUND_TOL,
    }
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")


def apply_kit_materials(kit_objects: dict[str, bpy.types.Object]) -> None:
    for key, ob in kit_objects.items():
        if key.startswith("_") or key == "kit_wall_left":
            continue
        role = KIT_ROLES.get(key, "prop")
        if key == "kit_floor_stain":
            ob.data.materials.clear()
            ob.data.materials.append(cc0.build_stain_decal_material("kit_floor_stain"))
        elif key in ("prop_box_a", "prop_box_b", "prop_crate"):
            cc0.apply_palette_material(ob, "box")
        elif key in KENNEY_RESTYLE:
            cc0.apply_kenney_restyle(ob, KENNEY_RESTYLE[key])
        else:
            cc0.apply_palette_material(ob, role)


def main() -> dict:
    cc0.reset_scene()
    kit_objects, floor_tiles, wall_parts = build_kenney_room()
    prop_keys = place_founder_cluster(kit_objects)
    retouch_prop_grounding(kit_objects)
    assert_scene_layout(floor_tiles, kit_objects, wall_parts)
    weld_room_meshes(kit_objects)
    apply_kit_materials(kit_objects)

    gate_path = cc0.PREVIEW_ROOT / f"{ASSET_ID}-scene-gates.json"
    write_scene_gate_metadata(kit_objects, gate_path)

    export_list = list({id(ob): ob for ob in kit_objects.values() if ob is not None}.values())
    import os

    preview_only = os.environ.get("CC0_PREVIEW_ONLY") == "1"
    if not preview_only:
        out_dir = cc0.ART_ROOT / CATEGORY
        tex_dir = out_dir / "textures" / ASSET_ID
        out_dir.mkdir(parents=True, exist_ok=True)
        for key, ob in kit_objects.items():
            sub = tex_dir / key
            cc0.bake_object_maps(ob, sub, f"{ASSET_ID}_{key}")
        cc0.export_gltf_collection(export_list, out_dir / f"{ASSET_ID}.glb")

    preview_path = cc0.PREVIEW_ROOT / f"{ASSET_ID}.png"
    cc0.render_kit_preview(
        export_list,
        preview_path,
        resolution=768,
        mode="hero",
        fill=0.70,
        margin=0.03,
        frame_span=None,
        up_axis="Z",
    )
    wall_thick = 0.05
    header_h = WALL_H - WALL_H * 0.68
    ortho_qa = [
        _solid_box("ortho_qa_left", (wall_thick, ROOM, WALL_H), (wall_thick / 2, ROOM / 2, WALL_H / 2)),
        _solid_box("ortho_qa_back_l", (0.5, wall_thick, WALL_H), (0.25, wall_thick / 2, WALL_H / 2)),
        _solid_box("ortho_qa_back_r", (0.5, wall_thick, WALL_H), (3.75, wall_thick / 2, WALL_H / 2)),
        _solid_box(
            "ortho_qa_header",
            (ROLL_DOOR_TARGET_W, wall_thick, header_h),
            (ROOM / 2, wall_thick / 2, WALL_H - header_h / 2),
        ),
    ]
    for ob in ortho_qa:
        cc0.apply_palette_material(ob, "wall")
    cc0.render_ortho_wall_check(
        ortho_qa,
        cc0.PREVIEW_ROOT / f"{ASSET_ID}-ortho-top.png",
        view="top",
        room=ROOM,
        wall_h=WALL_H,
    )
    ortho_front_hide = frozenset({"kit_wall_back"})
    cc0.render_ortho_wall_check(
        export_list,
        cc0.PREVIEW_ROOT / f"{ASSET_ID}-ortho-front.png",
        view="front_left",
        room=ROOM,
        wall_h=WALL_H,
        hide_render_names=ortho_front_hide,
        interior_slice=True,
    )
    for ob in ortho_qa:
        bpy.data.objects.remove(ob, do_unlink=True)

    if preview_only:
        print("OK preview-only", ASSET_ID)
        return {"id": ASSET_ID, "previewOnly": True}

    total_tris = sum(len(ob.data.polygons) for ob in export_list if ob.data)
    furnish_ids = prop_keys + ["kit_roll_door"]
    entry = {
        "id": ASSET_ID,
        "hero": True,
        "path": f"Assets/Art/EarlyGame/{CATEGORY}/{ASSET_ID}.glb",
        "preview": f"docs/art/previews/{ASSET_ID}.png",
        "sceneGates": f"docs/art/previews/{ASSET_ID}-scene-gates.json",
        "license": "CC0-1.0 (Kenney building + furniture kits, ambientCG Concrete015)",
        "sources": ["kenney_building-kit", "kenney_furniture-kit", "ambientcg-Concrete015", "garage-kit"],
        "furnishings": furnish_ids,
        "triangleCount": total_tris,
        "blenderVersion": cc0.BLENDER_VERSION,
        "generator": "tools/art/blender/garage-interior-room-kit.py",
        "parts": list(kit_objects.keys()),
    }
    cc0.write_cc0_manifest(entry)
    print("OK", ASSET_ID, total_tris, "tris", len(prop_keys), "props")
    return entry


if __name__ == "__main__":
    import os

    os.chdir(cc0.REPO_ROOT)
    main()