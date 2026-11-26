#!/usr/bin/env python3
"""Hero: welded 4×4 m cut-away garage + grounded garage-kit props."""
from __future__ import annotations

import json
import math
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import bpy  # noqa: E402
import cc0_common as cc0  # noqa: E402
from mathutils import Vector  # noqa: E402


ASSET_ID = "garage-interior-room-kit"
CATEGORY = "cc0-garage-kit"
ROOM = 4.0
FLOOR_T = 0.05
WALL_H = 2.7
WALL_T = 0.1
FLOOR_TOP = FLOOR_T
GROUND_EPS = 0.01
WALL_MOUNT_EPS = 0.02


KIT_ROLES: dict[str, str] = {
    "kit_floor": "floor",
    "kit_wall_back": "wall",
    "kit_wall_left": "wall",
    "kit_roll_door": "metal",
    "kit_window": "glass",
    "prop_desk_crt": "wood",
    "prop_folding_chair": "prop",
    "prop_crt_tower": "metal",
    "prop_server_closet": "metal",
    "prop_whiteboard": "board",
    "prop_cable_spool": "wood",
    "prop_coffee_drip": "prop",
}

WALL_MOUNTED = {"kit_window"}


def garage(name: str) -> Path:
    path = cc0.garage_glb(name)
    if not path.is_file():
        raise RuntimeError(f"missing garage-kit GLB: {path}")
    return path


def place_prop(
    asset_id: str,
    obj_name: str,
    loc: tuple[float, float, float],
    *,
    rot_y_deg: float = 0,
    rot_z_deg: float = 0,
    scale: float = 1.0,
) -> bpy.types.Object:
    meshes = cc0.import_gltf(garage(asset_id), obj_name)
    ob = cc0.merge_mesh_objects(meshes, obj_name)
    ob.location = loc
    if rot_y_deg:
        ob.rotation_euler[1] = math.radians(rot_y_deg)
    if rot_z_deg:
        ob.rotation_euler[2] = math.radians(rot_z_deg)
    if scale != 1.0:
        ob.scale = (scale, scale, scale)
    return ob


def build_welded_room() -> dict[str, bpy.types.Object]:
    """Single concrete slab; back/left walls welded on slab (open +X,+Z)."""
    floor = cc0.add_metric_box(
        "kit_floor",
        (ROOM, FLOOR_T, ROOM),
        (ROOM / 2, FLOOR_T / 2, ROOM / 2),
        "floor",
    )
    y0 = FLOOR_TOP
    wall_h = WALL_H
    back_core = cc0.add_metric_box(
        "wall_back_core",
        (ROOM, wall_h, WALL_T),
        (ROOM / 2, y0 + wall_h / 2, WALL_T / 2),
        None,
    )
    back_trim = cc0.add_metric_box(
        "wall_back_trim",
        (ROOM, 0.14, 0.08),
        (ROOM / 2, y0 + 0.07, 0.09),
        None,
    )
    left_core = cc0.add_metric_box(
        "wall_left_core",
        (WALL_T, wall_h, ROOM),
        (WALL_T / 2, y0 + wall_h / 2, ROOM / 2),
        None,
    )
    left_trim = cc0.add_metric_box(
        "wall_left_trim",
        (0.08, 0.14, ROOM),
        (0.09, y0 + 0.07, ROOM / 2),
        None,
    )
    corner_back = cc0.add_metric_box(
        "corner_back",
        (WALL_T, wall_h, WALL_T),
        (WALL_T / 2, y0 + wall_h / 2, WALL_T / 2),
        None,
    )
    corner_left = cc0.add_metric_box(
        "corner_left",
        (WALL_T, wall_h, WALL_T),
        (WALL_T / 2, y0 + wall_h / 2, WALL_T / 2),
        None,
    )
    kit_wall_back = cc0.join_welded([back_core, back_trim, corner_back], "kit_wall_back", "wall")
    kit_wall_left = cc0.join_welded([left_core, left_trim, corner_left], "kit_wall_left", "wall")

    roll = place_prop("garage-roll-door", "kit_roll_door", (ROOM / 2, y0 + 1.1, WALL_T + 0.02))
    window = place_prop(
        "garage-window-frame",
        "kit_window",
        (WALL_T + 0.06, y0 + 1.12, ROOM * 0.52),
        rot_y_deg=90,
    )
    return {
        "kit_floor": floor,
        "kit_wall_back": kit_wall_back,
        "kit_wall_left": kit_wall_left,
        "kit_roll_door": roll,
        "kit_window": window,
    }


def prop_positions() -> list[tuple[str, str, tuple[float, float, float], float, float]]:
    return [
        ("prop_desk_crt", "garage-desk-crt", (2.65, 0.0, 2.85), 0.0, -12.0),
        ("prop_folding_chair", "garage-folding-chair", (2.25, 0.0, 2.45), 0.0, 18.0),
        ("prop_crt_tower", "garage-crt-tower", (3.05, 0.0, 1.65), 0.0, 0.0),
        ("prop_server_closet", "garage-server-closet", (3.25, 0.0, 3.05), 0.0, 90.0),
        ("prop_whiteboard", "garage-whiteboard-1995", (2.0, 0.0, 0.42), 0.0, 0.0),
        ("prop_cable_spool", "garage-cable-spool", (1.2, 0.0, 1.85), 0.0, 0.0),
        ("prop_coffee_drip", "garage-coffee-drip", (1.05, 0.0, 1.05), 0.0, 0.0),
    ]


def flush_to_wall(ob: bpy.types.Object, *, axis: str, pos: float) -> None:
    bpy.context.view_layer.update()
    mins, _ = cc0.world_bbox(ob)
    if axis == "z":
        ob.location.z += pos - mins.z
    elif axis == "x":
        ob.location.x += pos - mins.x


def mount_wall_items(kit_objects: dict[str, bpy.types.Object]) -> None:
    wb = kit_objects["prop_whiteboard"]
    cc0.snap_to_floor(wb, FLOOR_TOP)
    flush_to_wall(wb, axis="z", pos=0.03)

    door = kit_objects["kit_roll_door"]
    flush_to_wall(door, axis="z", pos=0.03)
    mins, _ = cc0.world_bbox(door)
    door.location.y += FLOOR_TOP - mins.y

    flush_to_wall(kit_objects["kit_window"], axis="x", pos=WALL_T + 0.02)


def finalize_props(kit_objects: dict[str, bpy.types.Object], prop_keys: list[str]) -> None:
    for key in prop_keys:
        if key in WALL_MOUNTED:
            continue
        cc0.snap_to_floor(kit_objects[key], FLOOR_TOP)


def object_placement_meta(ob: bpy.types.Object, key: str) -> dict:
    mins, maxs = cc0.world_bbox(ob)
    cx = (mins.x + maxs.x) * 0.5
    cz = (mins.z + maxs.z) * 0.5
    ground_clear = mins.y - FLOOR_TOP
    dist_back = mins.z
    dist_left = mins.x
    inside = (
        mins.x >= -WALL_T
        and maxs.x <= ROOM + GROUND_EPS
        and mins.z >= -GROUND_EPS
        and maxs.z <= ROOM + GROUND_EPS
    )
    if key == "kit_window":
        grounded = dist_left <= WALL_MOUNT_EPS and mins.x <= ROOM + GROUND_EPS
    elif key == "kit_roll_door":
        grounded = abs(ground_clear) <= GROUND_EPS and dist_back <= 0.06
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
    for kit_key in ("kit_roll_door", "kit_window"):
        prop_meta.append(object_placement_meta(kit_objects[kit_key], kit_key))
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
        "groundToleranceM": GROUND_EPS,
        "wallMountToleranceM": WALL_MOUNT_EPS,
    }
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")


def apply_kit_materials(kit_objects: dict[str, bpy.types.Object]) -> None:
    for key, ob in kit_objects.items():
        cc0.apply_palette_material(ob, KIT_ROLES.get(key, "prop"))


def main() -> dict:
    cc0.reset_scene()

    kit_objects = build_welded_room()
    prop_specs = prop_positions()
    prop_keys = [k for k, *_ in prop_specs]
    for key, asset_id, loc, rot_z, rot_y in prop_specs:
        kit_objects[key] = place_prop(asset_id, key, (loc[0], FLOOR_TOP, loc[2]), rot_z_deg=rot_z, rot_y_deg=rot_y)

    finalize_props(kit_objects, prop_keys)
    mount_wall_items(kit_objects)
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
        margin=0.05,
        frame_objects=frame_kit,
        frame_span=ROOM,
    )

    if preview_only:
        print("OK preview-only", ASSET_ID)
        return {"id": ASSET_ID, "previewOnly": True}

    total_tris = sum(len(ob.data.polygons) for ob in export_list if ob.data)
    furnish_ids = prop_keys + ["kit_roll_door", "kit_window"]
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
        "license": "CC0-1.0 (garage-kit + CC0 textures)",
        "sources": ["garage-kit", "ambientcg-Concrete015"],
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