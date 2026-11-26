#!/usr/bin/env python3
"""Hero: 4×4 m Kenney building + furniture kit (Blender Z-up layout)."""
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
GAP_TOL = 0.02
GROUND_TOL = 0.01
WALL_TOL = 0.03

KIT_ROLES: dict[str, str] = {
    "kit_floor": "floor",
    "kit_wall_back": "wall",
    "kit_wall_left": "wall",
    "kit_wall_corner": "wall",
    "kit_roll_door": "trim",
    "prop_desk": "wood",
    "prop_chair": "chair",
    "prop_computer": "screen",
    "prop_bookcase": "shelf",
    "prop_lamp": "lamp_accent",
    "prop_box": "box",
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


def build_walls_and_door() -> tuple[list[bpy.types.Object], list[bpy.types.Object], bpy.types.Object, bpy.types.Object]:
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

    doorway = kenney_piece(
        "wall-doorway-wide-square.glb",
        "wall_doorway",
        (ROOM / 2, wall_thick / 2, 0.0),
        scale=(TILE_SCALE, TILE_SCALE, WALL_Z_SCALE),
        rot_z=-math.pi / 2,
    )
    snap_min_z(doorway, 0.0)
    back.append(doorway)

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

    roll = kenney_piece(
        "door-rotate-square-d.glb",
        "kit_roll_door",
        (ROOM / 2, wall_thick + 0.02, WALL_H * 0.48),
        scale=(TILE_SCALE, TILE_SCALE, WALL_Z_SCALE * 0.92),
        rot_z=-math.pi / 2,
    )
    snap_min_z(roll, FLOOR_TOP)
    return back, left, corner, roll


def assert_scene_layout(
    floor_tiles: list[bpy.types.Object],
    kit_objects: dict[str, bpy.types.Object],
    wall_parts: list[bpy.types.Object],
) -> None:
    """Fail the Blender run before render when layout is broken."""
    failures: list[str] = []

    for t in floor_tiles:
        parts = t.name.split("_")
        if len(parts) >= 3:
            ix, iy = int(parts[-2]), int(parts[-1])
        else:
            ix = iy = -1
        mn, mx = cc0.world_bbox(t)
        if abs(mn.z) > GAP_TOL:
            failures.append(f"floor tile {t.name} minZ={mn.z:.3f} != 0")
        if mx.z > FLOOR_TOP + GAP_TOL:
            failures.append(f"floor tile {t.name} topZ={mx.z:.3f} > {FLOOR_TOP}")
        if ix >= 0:
            if abs(mn.x - ix * TILE) > GAP_TOL or abs(mx.x - (ix + 1) * TILE) > GAP_TOL:
                failures.append(f"floor tile {t.name} X span [{mn.x:.2f},{mx.x:.2f}] != [{ix},{ix+1}]")
            if abs(mn.y - iy * TILE) > GAP_TOL or abs(mx.y - (iy + 1) * TILE) > GAP_TOL:
                failures.append(f"floor tile {t.name} Y span [{mn.y:.2f},{mx.y:.2f}] != [{iy},{iy+1}]")

    for ob in wall_parts + [kit_objects["kit_wall_corner"]]:
        mn, _ = cc0.world_bbox(ob)
        if mn.z > GAP_TOL:
            failures.append(f"{ob.name} bottom Z={mn.z:.3f} not on z=0")
    roll_mn, _ = cc0.world_bbox(kit_objects["kit_roll_door"])
    if abs(roll_mn.z - FLOOR_TOP) > GROUND_TOL * 2:
        failures.append(f"kit_roll_door minZ={roll_mn.z:.3f} not on floor {FLOOR_TOP}")

    for key, ob in kit_objects.items():
        if not key.startswith("prop_"):
            continue
        mn, mx = cc0.world_bbox(ob)
        if abs(mn.z - FLOOR_TOP) > GROUND_TOL:
            failures.append(f"{key} minZ={mn.z:.3f} not on floor top {FLOOR_TOP}")
        if mn.x < -GAP_TOL or mx.x > ROOM + GAP_TOL or mn.y < -GAP_TOL or mx.y > ROOM + GAP_TOL:
            failures.append(f"{key} outside 4×4 footprint")

    if failures:
        for line in failures:
            print("BUILD_FAIL:", line)
        raise SystemExit(1)


def build_kenney_room() -> tuple[dict[str, bpy.types.Object], list[bpy.types.Object], list[bpy.types.Object]]:
    floor_tiles = build_floor_tiles()
    back, left, corner, roll = build_walls_and_door()
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
    floor_tiles = kit.pop("_floor_tiles")
    back = kit.pop("_wall_back")
    left = kit.pop("_wall_left")
    kit["kit_floor"] = cc0.join_welded(floor_tiles, "kit_floor", "floor")
    kit["kit_wall_back"] = cc0.join_welded(back, "kit_wall_back", "wall")
    kit["kit_wall_left"] = cc0.join_welded(left, "kit_wall_left", "wall")


def furniture_prop(name: str, glb: str, xy: tuple[float, float], *, rot_z: float = 0.0) -> bpy.types.Object:
    ob = kenney_piece(
        glb,
        name,
        (xy[0], xy[1], 0.0),
        scale=(TILE_SCALE, TILE_SCALE, TILE_SCALE),
        rot_z=rot_z,
        path=cc0.kenney_furniture_glb(glb),
    )
    snap_min_z(ob, FLOOR_TOP)
    return ob


def prop_specs() -> list[tuple[str, str, tuple[float, float], float]]:
    return [
        ("prop_desk", "desk.glb", (2.85, 2.35), math.radians(205)),
        ("prop_chair", "chairDesk.glb", (2.05, 1.85), math.radians(25)),
        ("prop_computer", "computerScreen.glb", (2.72, 2.42), math.radians(205)),
        ("prop_bookcase", "bookcaseClosed.glb", (0.55, 3.05), math.pi / 2),
        ("prop_lamp", "lampRoundTable.glb", (1.05, 0.95), math.radians(-18)),
        ("prop_box", "cardboardBoxClosed.glb", (3.15, 1.25), math.radians(40)),
    ]


def flush_bookcase(ob: bpy.types.Object) -> None:
    bpy.context.view_layer.update()
    mins, _ = cc0.world_bbox(ob)
    ob.location.x += WALL_TOL - mins.x


def flush_roll_door(ob: bpy.types.Object) -> None:
    bpy.context.view_layer.update()
    mins, _ = cc0.world_bbox(ob)
    ob.location.y += WALL_TOL - mins.y


def object_placement_meta(ob: bpy.types.Object, key: str) -> dict:
    mins, maxs = cc0.world_bbox(ob)
    cx = (mins.x + maxs.x) * 0.5
    cy = (mins.y + maxs.y) * 0.5
    ground_clear = mins.z - FLOOR_TOP
    dist_back = mins.y
    dist_left = mins.x
    inside = (
        mins.x >= -GAP_TOL
        and maxs.x <= ROOM + GAP_TOL
        and mins.y >= -GAP_TOL
        and maxs.y <= ROOM + GAP_TOL
    )
    grounded = abs(ground_clear) <= GROUND_TOL if key.startswith("prop_") else mins.z <= GAP_TOL
    return {
        "objectName": key,
        "x": round(cx, 4),
        "y": round(cy, 4),
        "minZ": round(mins.z, 4),
        "groundClearanceM": round(ground_clear, 4),
        "distToBackWallM": round(dist_back, 4),
        "distToLeftWallM": round(dist_left, 4),
        "grounded": grounded,
        "insideFloorBounds": inside,
        "rotationZDeg": round(math.degrees(ob.rotation_euler.z), 2),
    }


def write_scene_gate_metadata(kit_objects: dict[str, bpy.types.Object], path: Path) -> None:
    prop_keys = [k for k in kit_objects if k.startswith("prop_")]
    prop_meta = [object_placement_meta(kit_objects[k], k) for k in prop_keys]
    prop_meta.append(object_placement_meta(kit_objects["kit_roll_door"], "kit_roll_door"))
    payload = {
        "assetId": ASSET_ID,
        "floorTopZ": FLOOR_TOP,
        "floorBounds": {"minX": 0.0, "maxX": ROOM, "minY": 0.0, "maxY": ROOM},
        "wallCornerGapM": 0.0,
        "propInstances": prop_meta,
        "minPropCount": 6,
        "floorTileCount": GRID * GRID,
        "groundToleranceM": GROUND_TOL,
        "wallMountToleranceM": WALL_TOL,
    }
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")


def apply_kit_materials(kit_objects: dict[str, bpy.types.Object]) -> None:
    for key, ob in kit_objects.items():
        cc0.apply_palette_material(ob, KIT_ROLES.get(key, "prop"))


def main() -> dict:
    cc0.reset_scene()
    kit_objects, floor_tiles, _walls = build_kenney_room()
    prop_keys: list[str] = []
    for key, glb, xy, rot_z in prop_specs():
        kit_objects[key] = furniture_prop(key, glb, xy, rot_z=rot_z)
        prop_keys.append(key)

    flush_bookcase(kit_objects["prop_bookcase"])
    flush_roll_door(kit_objects["kit_roll_door"])
    assert_scene_layout(floor_tiles, kit_objects, _walls)
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
        fill=0.70,
        margin=0.06,
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