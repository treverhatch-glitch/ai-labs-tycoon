#!/usr/bin/env python3
"""Hero: 4×4 m cut-away garage (solid meshes + garage-kit props)."""
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
WALL_H = 2.2
WALL_T = 0.1

KIT_ROLES: dict[str, str] = {
    "kit_floor": "floor",
    "kit_wall_back": "wall",
    "kit_wall_left": "wall",
    "kit_trim_back": "trim",
    "kit_trim_left": "trim",
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
) -> bpy.types.Object:
    meshes = cc0.import_gltf(garage(asset_id), obj_name)
    ob = cc0.merge_mesh_objects(meshes, obj_name)
    ob.location = loc
    if rot_y_deg:
        ob.rotation_euler[1] = math.radians(rot_y_deg)
    if rot_z_deg:
        ob.rotation_euler[2] = math.radians(rot_z_deg)
    return ob


def build_solid_shell() -> dict[str, bpy.types.Object]:
    """Continuous floor + two fixed walls; open toward +X,+Z (camera side)."""
    floor = cc0.add_metric_box(
        "kit_floor",
        (ROOM, 0.045, ROOM),
        (ROOM / 2, 0.0225, ROOM / 2),
        "floor",
    )
    wall_back = cc0.add_metric_box(
        "kit_wall_back",
        (ROOM, WALL_H, WALL_T),
        (ROOM / 2, WALL_H / 2, WALL_T / 2),
        "wall",
    )
    wall_left = cc0.add_metric_box(
        "kit_wall_left",
        (WALL_T, WALL_H, ROOM),
        (WALL_T / 2, WALL_H / 2, ROOM / 2),
        "wall",
    )
    trim_back = cc0.add_metric_box(
        "kit_trim_back",
        (ROOM, 0.14, 0.12),
        (ROOM / 2, 0.07, 0.10),
        "trim",
    )
    trim_left = cc0.add_metric_box(
        "kit_trim_left",
        (0.12, 0.14, ROOM),
        (0.10, 0.07, ROOM / 2),
        "trim",
    )
    roll = place_prop("garage-roll-door", "kit_roll_door", (ROOM / 2, 0.0, 0.16))
    window = place_prop("garage-window-frame", "kit_window", (0.14, 1.05, ROOM * 0.55), rot_y_deg=90)
    return {
        "kit_floor": floor,
        "kit_wall_back": wall_back,
        "kit_wall_left": wall_left,
        "kit_trim_back": trim_back,
        "kit_trim_left": trim_left,
        "kit_roll_door": roll,
        "kit_window": window,
    }


def prop_positions() -> list[tuple[str, str, tuple[float, float, float], float, float]]:
    return [
        ("prop_desk_crt", "garage-desk-crt", (2.75, 0.0, 2.95), 0.0, -8.0),
        ("prop_folding_chair", "garage-folding-chair", (2.35, 0.0, 2.55), 0.0, 12.0),
        ("prop_crt_tower", "garage-crt-tower", (3.05, 0.0, 1.45), 0.0, 0.0),
        ("prop_server_closet", "garage-server-closet", (1.15, 0.0, 3.05), 0.0, 90.0),
        ("prop_whiteboard", "garage-whiteboard-1995", (2.1, 0.0, 0.35), 0.0, 0.0),
        ("prop_cable_spool", "garage-cable-spool", (1.05, 0.0, 1.75), 0.0, 0.0),
        ("prop_coffee_drip", "garage-coffee-drip", (3.25, 0.0, 1.05), 0.0, -18.0),
    ]


def apply_kit_materials(kit_objects: dict[str, bpy.types.Object]) -> None:
    for key, ob in kit_objects.items():
        cc0.apply_palette_material(ob, KIT_ROLES.get(key, "prop"))


def write_scene_gate_metadata(
    kit_objects: dict[str, bpy.types.Object],
    prop_meta: list[dict],
    path: Path,
) -> None:
    walls = {k: kit_objects[k] for k in ("kit_wall_back", "kit_wall_left")}
    wall_rots = {}
    for name, ob in walls.items():
        wall_rots[name] = {
            "yDeg": round(math.degrees(ob.rotation_euler.y)),
            "zDeg": round(math.degrees(ob.rotation_euler.z)),
        }
    payload = {
        "assetId": ASSET_ID,
        "floorBounds": {"minX": 0.0, "maxX": ROOM, "minZ": 0.0, "maxZ": ROOM},
        "wallRotationsDeg": wall_rots,
        "propInstances": prop_meta,
        "minPropCount": 6,
    }
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")


def collect_prop_meta(kit_objects: dict[str, bpy.types.Object], prop_keys: list[str]) -> list[dict]:
    out: list[dict] = []
    for key in prop_keys:
        ob = kit_objects[key]
        cx, cz = ob.location.x, ob.location.z
        inside = 0.0 <= cx <= ROOM and 0.0 <= cz <= ROOM
        out.append(
            {
                "objectName": key,
                "x": round(cx, 4),
                "z": round(cz, 4),
                "rotationYDeg": round(math.degrees(ob.rotation_euler.y), 2),
                "rotationZDeg": round(math.degrees(ob.rotation_euler.z), 2),
                "insideFloorBounds": inside,
            }
        )
    return out


def main() -> dict:
    cc0.reset_scene()

    kit_objects = build_solid_shell()
    prop_specs = prop_positions()
    for key, asset_id, loc, rot_z, rot_y in prop_specs:
        kit_objects[key] = place_prop(asset_id, key, loc, rot_z_deg=rot_z, rot_y_deg=rot_y)

    apply_kit_materials(kit_objects)

    export_list = list(kit_objects.values())
    prop_keys = [k for k, *_ in prop_specs]
    prop_meta = collect_prop_meta(kit_objects, prop_keys)
    gate_path = cc0.PREVIEW_ROOT / f"{ASSET_ID}-scene-gates.json"
    write_scene_gate_metadata(kit_objects, prop_meta, gate_path)

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

    frame_kit = [kit_objects[k] for k in kit_objects if k.startswith("kit_")]
    preview_path = cc0.PREVIEW_ROOT / f"{ASSET_ID}.png"
    scene_preview = cc0.PREVIEW_ROOT / f"{ASSET_ID}-scene.png"
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
    cc0.render_kit_preview(
        export_list,
        scene_preview,
        resolution=1024,
        mode="scene",
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
        "scenePreview": f"docs/art/previews/{ASSET_ID}-scene.png",
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