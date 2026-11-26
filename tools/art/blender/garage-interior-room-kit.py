#!/usr/bin/env python3
"""Hero: 4×4 m Kenney building + furniture kit cut-away garage."""
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
WALL_H_SCALE = 2.7 / 2.0
TILE_SCALE = 0.5
GROUND_EPS = 0.01
WALL_MOUNT_EPS = 0.02

KIT_ROLES: dict[str, str] = {
    "kit_floor": "floor",
    "kit_wall_back": "wall",
    "kit_wall_left": "wall",
    "kit_wall_corner": "wall",
    "kit_doorway": "wall",
    "kit_roll_door": "trim",
    "prop_desk": "wood",
    "prop_chair": "chair",
    "prop_computer": "screen",
    "prop_keyboard": "metal",
    "prop_bookcase": "shelf",
    "prop_lamp": "lamp_accent",
    "prop_box_closed": "box",
    "prop_box_open": "box_open",
    "prop_plant": "plant",
}


def kenney_piece(
    glb_name: str,
    obj_name: str,
    center: tuple[float, float, float],
    *,
    scale: tuple[float, float, float] = (TILE_SCALE, TILE_SCALE, TILE_SCALE),
    rot_x: float = 0.0,
    rot_y: float = 0.0,
    rot_z: float = 0.0,
    path: Path | None = None,
) -> bpy.types.Object:
    glb = path or cc0.kenney_glb(glb_name)
    if not glb.is_file():
        raise RuntimeError(f"missing Kenney GLB: {glb}")
    meshes = cc0.import_gltf(glb, obj_name)
    ob = cc0.merge_mesh_objects(meshes, obj_name)
    ob.location = center
    ob.rotation_euler = (rot_x, rot_y, rot_z)
    ob.scale = scale
    bpy.context.view_layer.update()
    return ob


def snap_bottom_to_y(ob: bpy.types.Object, y: float) -> None:
    mins, _ = cc0.world_bbox(ob)
    ob.location.y += y - mins.y


def build_floor_tiles() -> list[bpy.types.Object]:
    tiles: list[bpy.types.Object] = []
    rot_x = math.pi / 2
    for ix in range(GRID):
        for iz in range(GRID):
            name = f"floor_{ix}_{iz}"
            cx = TILE * (ix + 0.5)
            cz = TILE * (iz + 0.5)
            ob = kenney_piece(
                "floor.glb",
                name,
                (cx, 0.0, cz),
                scale=(TILE_SCALE, TILE_SCALE, TILE_SCALE),
                rot_x=rot_x,
            )
            snap_bottom_to_y(ob, 0.0)
            tiles.append(ob)
    return tiles


def wall_scale() -> tuple[float, float, float]:
    return (TILE_SCALE, WALL_H_SCALE, TILE_SCALE)


def build_walls() -> tuple[list[bpy.types.Object], list[bpy.types.Object], bpy.types.Object, bpy.types.Object]:
    back: list[bpy.types.Object] = []
    left: list[bpy.types.Object] = []
    ws = wall_scale()
    for ix in range(GRID):
        cx = TILE * (ix + 0.5)
        if ix in (1, 2):
            continue
        ob = kenney_piece(
            "wall.glb",
            f"wall_back_{ix}",
            (cx, 0.0, TILE_SCALE * 0.1),
            scale=ws,
            rot_y=math.pi / 2,
        )
        snap_bottom_to_y(ob, FLOOR_TOP)
        back.append(ob)

    doorway = kenney_piece(
        "wall-doorway-wide-square.glb",
        "wall_back_doorway",
        (ROOM / 2, 0.0, TILE_SCALE * 0.1),
        scale=(TILE_SCALE, WALL_H_SCALE, TILE_SCALE),
        rot_y=math.pi / 2,
    )
    snap_bottom_to_y(doorway, FLOOR_TOP)
    back.append(doorway)

    for iz in range(GRID):
        cz = TILE * (iz + 0.5)
        ob = kenney_piece(
            "wall.glb",
            f"wall_left_{iz}",
            (TILE_SCALE * 0.1, 0.0, cz),
            scale=ws,
        )
        snap_bottom_to_y(ob, FLOOR_TOP)
        left.append(ob)

    corner = kenney_piece(
        "wall-corner.glb",
        "kit_wall_corner",
        (TILE_SCALE * 0.05, 0.0, TILE_SCALE * 0.05),
        scale=(TILE_SCALE, WALL_H_SCALE, TILE_SCALE),
    )
    snap_bottom_to_y(corner, FLOOR_TOP)

    roll = kenney_piece(
        "door-rotate-square-d.glb",
        "kit_roll_door",
        (ROOM / 2, FLOOR_TOP + 1.05, TILE_SCALE * 0.12),
        scale=(TILE_SCALE, WALL_H_SCALE * 0.92, TILE_SCALE),
        rot_y=math.pi / 2,
    )
    return back, left, corner, roll


def build_kenney_room() -> dict[str, bpy.types.Object]:
    floor_tiles = build_floor_tiles()
    kit_floor = cc0.join_welded(floor_tiles, "kit_floor", "floor")

    back, left, corner, roll = build_walls()
    kit_wall_back = cc0.join_welded(back, "kit_wall_back", "wall")
    kit_wall_left = cc0.join_welded(left, "kit_wall_left", "wall")

    return {
        "kit_floor": kit_floor,
        "kit_wall_back": kit_wall_back,
        "kit_wall_left": kit_wall_left,
        "kit_wall_corner": corner,
        "kit_roll_door": roll,
    }


def furniture_prop(name: str, glb: str, loc: tuple[float, float, float], *, rot_y: float = 0.0) -> bpy.types.Object:
    ob = kenney_piece(
        glb,
        name,
        (loc[0], FLOOR_TOP, loc[2]),
        scale=(TILE_SCALE, TILE_SCALE, TILE_SCALE),
        rot_y=rot_y,
        path=cc0.kenney_furniture_glb(glb),
    )
    snap_bottom_to_y(ob, FLOOR_TOP)
    return ob


def prop_specs() -> list[tuple[str, str, tuple[float, float, float], float]]:
    return [
        ("prop_desk", "desk.glb", (2.7, 0.0, 2.55), math.pi),
        ("prop_chair", "chairDesk.glb", (2.15, 0.0, 2.35), math.radians(200)),
        ("prop_computer", "computerScreen.glb", (2.55, 0.0, 2.62), math.pi),
        ("prop_keyboard", "computerKeyboard.glb", (2.45, 0.0, 2.42), math.radians(175)),
        ("prop_bookcase", "bookcaseClosed.glb", (0.55, 0.0, 2.1), math.pi / 2),
        ("prop_lamp", "lampRoundTable.glb", (1.15, 0.0, 1.05), 0.0),
        ("prop_box_closed", "cardboardBoxClosed.glb", (1.35, 0.0, 3.15), math.radians(22)),
        ("prop_box_open", "cardboardBoxOpen.glb", (1.75, 0.0, 2.85), math.radians(-15)),
        ("prop_plant", "plantSmall1.glb", (3.25, 0.0, 3.25), math.radians(35)),
    ]


def flush_bookcase_to_left(ob: bpy.types.Object) -> None:
    bpy.context.view_layer.update()
    mins, _ = cc0.world_bbox(ob)
    ob.location.x += WALL_MOUNT_EPS - mins.x


def flush_roll_door(ob: bpy.types.Object) -> None:
    snap_bottom_to_y(ob, FLOOR_TOP)
    bpy.context.view_layer.update()
    mins, _ = cc0.world_bbox(ob)
    ob.location.z += 0.04 - mins.z


def object_placement_meta(ob: bpy.types.Object, key: str) -> dict:
    mins, maxs = cc0.world_bbox(ob)
    cx = (mins.x + maxs.x) * 0.5
    cz = (mins.z + maxs.z) * 0.5
    ground_clear = mins.y - FLOOR_TOP
    dist_back = mins.z
    dist_left = mins.x
    inside = (
        mins.x >= -TILE_SCALE
        and maxs.x <= ROOM + GROUND_EPS
        and mins.z >= -GROUND_EPS
        and maxs.z <= ROOM + GROUND_EPS
    )
    if key == "kit_roll_door":
        grounded = abs(ground_clear) <= GROUND_EPS * 4 and dist_back <= 0.12
    else:
        grounded = abs(ground_clear) <= GROUND_EPS
    return {
        "objectName": key,
        "x": round(cx, 4),
        "z": round(cz, 4),
        "minY": round(mins.y, 4),
        "groundClearanceM": round(ground_clear, 4),
        "distToBackWallM": round(dist_back, 4),
        "distToLeftWallM": round(dist_left, 4),
        "grounded": grounded,
        "insideFloorBounds": inside,
        "rotationYDeg": round(math.degrees(ob.rotation_euler.y), 2),
        "rotationZDeg": round(math.degrees(ob.rotation_euler.z), 2),
    }


def write_scene_gate_metadata(kit_objects: dict[str, bpy.types.Object], path: Path) -> None:
    prop_keys = [k for k in kit_objects if k.startswith("prop_")]
    prop_meta = [object_placement_meta(kit_objects[k], k) for k in prop_keys]
    prop_meta.append(object_placement_meta(kit_objects["kit_roll_door"], "kit_roll_door"))
    wall_rots = {
        name: {
            "yDeg": round(math.degrees(kit_objects[name].rotation_euler.y)),
            "zDeg": round(math.degrees(kit_objects[name].rotation_euler.z)),
        }
        for name in ("kit_wall_back", "kit_wall_left")
    }
    payload = {
        "assetId": ASSET_ID,
        "floorTopY": FLOOR_TOP,
        "floorBounds": {"minX": 0.0, "maxX": ROOM, "minZ": 0.0, "maxZ": ROOM},
        "wallCornerGapM": 0.0,
        "wallRotationsDeg": wall_rots,
        "propInstances": prop_meta,
        "minPropCount": 6,
        "floorTileCount": GRID * GRID,
        "wallSegmentCountBack": GRID,
        "wallSegmentCountLeft": GRID,
        "groundToleranceM": GROUND_EPS,
        "wallMountToleranceM": WALL_MOUNT_EPS,
    }
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")


def apply_kit_materials(kit_objects: dict[str, bpy.types.Object]) -> None:
    for key, ob in kit_objects.items():
        role = KIT_ROLES.get(key, "prop")
        cc0.apply_palette_material(ob, role)


def main() -> dict:
    cc0.reset_scene()

    kit_objects = build_kenney_room()
    prop_keys: list[str] = []
    for key, glb, loc, rot_y in prop_specs():
        kit_objects[key] = furniture_prop(key, glb, loc, rot_y=rot_y)
        prop_keys.append(key)

    flush_bookcase_to_left(kit_objects["prop_bookcase"])
    flush_roll_door(kit_objects["kit_roll_door"])
    apply_kit_materials(kit_objects)

    export_list = list(kit_objects.values())
    gate_path = cc0.PREVIEW_ROOT / f"{ASSET_ID}-scene-gates.json"
    write_scene_gate_metadata(kit_objects, gate_path)

    import os

    preview_only = os.environ.get("CC0_PREVIEW_ONLY") == "1"
    maps_by_part: dict[str, dict[str, str]] = {}
    if not preview_only:
        out_dir = cc0.ART_ROOT / CATEGORY
        tex_dir = out_dir / "textures" / ASSET_ID
        out_dir.mkdir(parents=True, exist_ok=True)
        for key, ob in kit_objects.items():
            sub = tex_dir / key
            maps_by_part[key] = cc0.bake_object_maps(ob, sub, f"{ASSET_ID}_{key}")
        glb_path = out_dir / f"{ASSET_ID}.glb"
        cc0.export_gltf_collection(export_list, glb_path)

    preview_path = cc0.PREVIEW_ROOT / f"{ASSET_ID}.png"
    frame_kit = [kit_objects[k] for k in kit_objects if k.startswith("kit_")]
    cc0.render_kit_preview(
        export_list,
        preview_path,
        resolution=768,
        mode="hero",
        fill=0.70,
        margin=0.08,
        frame_objects=frame_kit,
        frame_span=ROOM,
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
        "textures": {
            part: {
                k: f"Assets/Art/EarlyGame/{CATEGORY}/textures/{ASSET_ID}/{part}/{ASSET_ID}_{part}_{k}.png"
                for k in maps
            }
            for part, maps in maps_by_part.items()
        },
        "license": "CC0-1.0 (Kenney building + furniture kits, ambientCG Concrete015)",
        "sources": ["kenney_building-kit", "kenney_furniture-kit", "ambientcg-Concrete015"],
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