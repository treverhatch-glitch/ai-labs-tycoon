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
ROLL_DOOR_TARGET_W = 3.0
GAP_TOL = 0.02
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
    "prop_rug": (0.004, 0.10),
    "prop_plant": (0.18, 0.62),
    "prop_speaker": (0.22, 0.55),
}

KIT_ROLES: dict[str, str] = {
    "kit_floor": "floor",
    "kit_wall_back": "wall",
    "kit_wall_left": "wall",
    "kit_wall_corner": "wall",
    "kit_roll_door": "metal",
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
    "prop_box_b": "box_open",
    "prop_rug": "prop",
    "prop_books": "wood",
    "prop_plant": "plant",
    "prop_speaker": "metal",
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


def build_walls() -> tuple[list[bpy.types.Object], list[bpy.types.Object], bpy.types.Object]:
    ws = _wall_scale()
    wall_thick = 0.05
    back: list[bpy.types.Object] = []
    left: list[bpy.types.Object] = []
    for ix in range(GRID):
        cx = TILE * (ix + 0.5)
        if ix in (1, 2):
            continue
        ob = kenney_piece(
            "wall.glb",
            f"wall_back_{ix}",
            (cx, wall_thick / 2, 0.0),
            scale=ws,
            rot_z=-math.pi / 2,
        )
        snap_min_z(ob, 0.0)
        back.append(ob)

    for iy in range(GRID):
        cy = TILE * (iy + 0.5)
        ob = kenney_piece("wall.glb", f"wall_left_{iy}", (wall_thick / 2, cy, 0.0), scale=ws)
        snap_min_z(ob, 0.0)
        left.append(ob)

    corner = kenney_piece(
        "wall-corner.glb",
        "kit_wall_corner",
        (wall_thick / 2, wall_thick / 2, 0.0),
        scale=(TILE_SCALE, TILE_SCALE, WALL_Z_SCALE),
    )
    snap_min_z(corner, 0.0)
    return back, left, corner


def build_roll_door() -> bpy.types.Object:
    """Ribbed metal roll-up (~3 m wide) on back wall — object name kit_roll_door."""
    wall_thick = 0.05
    ribs: list[bpy.types.Object] = []
    rib_xs = (0.65, 1.55, 2.45, 3.35)
    rib_scale = (0.46, 0.5, WALL_Z_SCALE * 0.96)
    for i, rx in enumerate(rib_xs):
        ribs.append(
            kenney_piece(
                "plating-wide.glb",
                f"roll_plate_{i}",
                (rx, wall_thick / 2 + 0.01, WALL_H * 0.5),
                scale=rib_scale,
                rot_z=-math.pi / 2,
            )
        )
    shutter = kenney_piece(
        "door-rotate-square-d.glb",
        "roll_shutter",
        (ROOM / 2, wall_thick / 2 + 0.015, WALL_H * 0.48),
        scale=(1.55, 0.45, WALL_Z_SCALE * 0.88),
        rot_z=-math.pi / 2,
    )
    snap_min_z(shutter, FLOOR_TOP)
    sill = kenney_piece(
        "plating-wide.glb",
        "roll_sill",
        (ROOM / 2, wall_thick / 2 + 0.014, 0.32),
        scale=(1.75, 0.62, 0.38),
        rot_z=-math.pi / 2,
    )
    snap_min_z(sill, 0.0)
    ob = cc0.merge_mesh_objects(ribs + [shutter, sill], "kit_roll_door")
    ob.name = "kit_roll_door"
    bpy.context.view_layer.update()
    mins, maxs = cc0.world_bbox(ob)
    span_x = maxs.x - mins.x
    if span_x < ROLL_DOOR_TARGET_W - 0.15:
        factor = ROLL_DOOR_TARGET_W / max(span_x, 0.01)
        ob.scale.x *= factor
        bpy.context.view_layer.update()
    mins, _ = cc0.world_bbox(ob)
    ob.location.y += WALL_TOL - mins.y
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

    prop_keys = [k for k in kit_objects if k.startswith("prop_")]
    if len(prop_keys) < 8:
        failures.append(f"prop count {len(prop_keys)} < 8")

    for key in prop_keys:
        ob = kit_objects[key]
        mn, mx = cc0.world_bbox(ob)
        if abs(mn.z - FLOOR_TOP) > GROUND_TOL:
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
    kit["kit_wall_back"] = cc0.join_welded(kit.pop("_wall_back"), "kit_wall_back", "wall")
    kit["kit_wall_left"] = cc0.join_welded(kit.pop("_wall_left"), "kit_wall_left", "wall")


def place_founder_cluster(kit_objects: dict[str, bpy.types.Object]) -> list[str]:
    """Founder corner: desk cluster + shop/readability props."""
    keys: list[str] = []
    cx, cy = 2.35, 2.15
    specs: list[tuple[str, str, tuple[float, float], float, float | None]] = [
        ("prop_desk", "desk.glb", (cx, cy), math.radians(200), FURN_SCALE),
        ("prop_chair", "chairDesk.glb", (cx - 0.55, cy - 0.45), math.radians(30), FURN_SCALE),
        ("prop_laptop", "laptop.glb", (cx + 0.05, cy + 0.12), math.radians(200), FURN_SCALE * 0.85),
        ("prop_keyboard", "computerKeyboard.glb", (cx - 0.08, cy + 0.05), math.radians(195), FURN_SCALE * 0.9),
        ("prop_lamp", "lampRoundTable.glb", (cx + 0.35, cy - 0.18), math.radians(-10), FURN_SCALE),
        ("prop_books", "books.glb", (cx - 0.22, cy + 0.22), math.radians(15), FURN_SCALE * 0.8),
        ("prop_bookcase", "bookcaseClosed.glb", (0.55, 3.35), math.pi / 2, FURN_SCALE * 0.95),
        ("prop_box_a", "cardboardBoxClosed.glb", (0.95, 3.05), math.radians(18), FURN_SCALE * 0.85),
        ("prop_box_b", "cardboardBoxOpen.glb", (1.2, 2.75), math.radians(-12), FURN_SCALE * 0.85),
        ("prop_workbench", "sideTable.glb", (1.15, 0.95), math.radians(8), FURN_SCALE),
        ("prop_plant", "plantSmall1.glb", (3.25, 1.05), math.radians(-25), FURN_SCALE * 1.05),
        ("prop_speaker", "speaker.glb", (0.92, 1.25), math.radians(12), FURN_SCALE * 0.92),
        ("prop_rug", "rugRectangle.glb", (cx - 0.05, cy - 0.15), math.radians(78), FURN_SCALE * 0.38),
    ]
    for key, glb, xy, rot, sc in specs:
        kit_objects[key] = furniture_prop(key, glb, xy, rot_z=rot, scale=sc if sc else FURN_SCALE)
        keys.append(key)

    kit_objects["prop_whiteboard"] = garage_piece(
        "garage-whiteboard-1995",
        "prop_whiteboard",
        (0.32, 1.55, 0.0),
        rot_z=math.pi / 2,
        scale=1.45,
    )
    snap_min_z(kit_objects["prop_whiteboard"], FLOOR_TOP)
    keys.append("prop_whiteboard")

    kit_objects["prop_server"] = garage_piece(
        "garage-server-closet",
        "prop_server",
        (3.05, 2.15, 0.0),
        rot_z=math.radians(-90),
        scale=1.1,
    )
    snap_min_z(kit_objects["prop_server"], FLOOR_TOP)
    keys.append("prop_server")

    bpy.context.view_layer.update()
    mins, _ = cc0.world_bbox(kit_objects["prop_bookcase"])
    kit_objects["prop_bookcase"].location.x += WALL_TOL - mins.x
    wb = kit_objects["prop_whiteboard"]
    mn, mx = cc0.world_bbox(wb)
    wb.location.x += WALL_TOL - mn.x
    bpy.context.view_layer.update()
    mn, mx = cc0.world_bbox(wb)
    if mx.y > ROOM - GAP_TOL:
        wb.location.y -= mx.y - (ROOM - GAP_TOL)
    if mn.y < GAP_TOL:
        wb.location.y += GAP_TOL - mn.y
    return keys


def write_scene_gate_metadata(kit_objects: dict[str, bpy.types.Object], path: Path) -> None:
    prop_keys = [k for k in kit_objects if k.startswith("prop_")]
    roll_mn, roll_mx = cc0.world_bbox(kit_objects["kit_roll_door"])
    props = []
    for key in prop_keys:
        mn, mx = cc0.world_bbox(kit_objects[key])
        hm = prop_height_m(kit_objects[key], key)
        props.append(
            {
                "objectName": key,
                "x": round((mn.x + mx.x) * 0.5, 4),
                "y": round((mn.y + mx.y) * 0.5, 4),
                "minZ": round(mn.z, 4),
                "heightM": round(hm, 4),
                "heightToWallRatio": round(hm / WALL_H, 4),
                "grounded": abs(mn.z - FLOOR_TOP) <= GROUND_TOL,
                "insideFloorBounds": True,
            }
        )
    payload = {
        "assetId": ASSET_ID,
        "floorTopZ": FLOOR_TOP,
        "wallHeightM": WALL_H,
        "floorBounds": {"minX": 0.0, "maxX": ROOM, "minY": 0.0, "maxY": ROOM},
        "wallCornerGapM": 0.0,
        "rollDoorObjectName": kit_objects["kit_roll_door"].name,
        "rollDoorWidthM": round(roll_mx.x - roll_mn.x, 4),
        "minPropCount": 8,
        "propInstances": props,
        "floorTileCount": GRID * GRID,
        "groundToleranceM": GROUND_TOL,
    }
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")


def apply_kit_materials(kit_objects: dict[str, bpy.types.Object]) -> None:
    for key, ob in kit_objects.items():
        cc0.apply_palette_material(ob, KIT_ROLES.get(key, "prop"))


def main() -> dict:
    cc0.reset_scene()
    kit_objects, floor_tiles, wall_parts = build_kenney_room()
    prop_keys = place_founder_cluster(kit_objects)
    assert_scene_layout(floor_tiles, kit_objects, wall_parts)
    weld_room_meshes(kit_objects)
    apply_kit_materials(kit_objects)

    gate_path = cc0.PREVIEW_ROOT / f"{ASSET_ID}-scene-gates.json"
    write_scene_gate_metadata(kit_objects, gate_path)

    export_list = list(kit_objects.values())
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
        fill=0.74,
        margin=0.02,
        frame_span=ROOM,
        up_axis="Z",
    )

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