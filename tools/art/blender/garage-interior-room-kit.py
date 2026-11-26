#!/usr/bin/env python3
"""Hero asset: 1995 cut-away garage (4×4 m) from in-repo garage-kit GLBs."""
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
TILE = 0.45
PANEL = TILE
N_ALONG = round(ROOM / TILE)  # 9 → 4.05 m, scaled to 4.0 below


def garage(name: str) -> Path:
    path = cc0.garage_glb(name)
    if not path.is_file():
        raise RuntimeError(f"missing garage-kit GLB: {path}")
    return path


def place_garage(
    asset_id: str,
    obj_name: str,
    loc: tuple[float, float, float],
    *,
    rot_y_deg: float = 0,
    rot_z_deg: float = 0,
    scale: tuple[float, float, float] | None = None,
) -> bpy.types.Object:
    meshes = cc0.import_gltf(garage(asset_id), obj_name)
    ob = cc0.merge_mesh_objects(meshes, obj_name)
    ob.location = loc
    if rot_y_deg:
        ob.rotation_euler[1] = math.radians(rot_y_deg)
    if rot_z_deg:
        ob.rotation_euler[2] = math.radians(rot_z_deg)
    if scale:
        ob.scale = scale
    return ob


def _room_scale_horizontal(objects: list[bpy.types.Object]) -> float:
    raw = N_ALONG * TILE
    scale = ROOM / raw
    for ob in objects:
        ob.location.x *= scale
        ob.location.z *= scale
        ob.scale[0] *= scale
        ob.scale[2] *= scale
    return scale


def _align_room_to_origin(objects: list[bpy.types.Object]) -> None:
    mins = Vector((1e9, 1e9, 1e9))
    for ob in objects:
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins.x = min(mins.x, w.x)
            mins.y = min(mins.y, w.y)
            mins.z = min(mins.z, w.z)
    for ob in objects:
        ob.location.x -= mins.x
        ob.location.z -= mins.z
        ob.location.y -= mins.y


def build_floor() -> bpy.types.Object:
    parts: list[bpy.types.Object] = []
    for ix in range(N_ALONG):
        for iy in range(N_ALONG):
            name = f"floor_{ix}_{iy}"
            ob = place_garage("garage-floor-tile", name, (ix * PANEL, 0.0, iy * PANEL))
            parts.append(ob)
    return cc0.merge_mesh_objects(parts, "kit_floor")


def build_wall_strip(axis: str) -> bpy.types.Object:
    """Flush 90° walls on back (z=0) and left (x=0); open corner at (+X,+Z)."""
    parts: list[bpy.types.Object] = []
    if axis == "back":
        for i in range(N_ALONG):
            parts.append(
                place_garage(
                    "garage-wall-panel",
                    f"wall_b_{i}",
                    (i * PANEL + PANEL * 0.5, 0.0, 0.0),
                )
            )
        return cc0.merge_mesh_objects(parts, "kit_wall_back")
    for i in range(N_ALONG):
        parts.append(
            place_garage(
                "garage-wall-panel",
                f"wall_l_{i}",
                (0.0, 0.0, i * PANEL + PANEL * 0.5),
                rot_y_deg=90,
            )
        )
    return cc0.merge_mesh_objects(parts, "kit_wall_left")


def prop_positions() -> list[tuple[str, str, tuple[float, float, float], float, float]]:
    """≥6 garage-kit props inside the 4×4 m floor (dial-up era layout)."""
    return [
        ("prop_desk_crt", "garage-desk-crt", (2.85, 0.0, 3.05), 0.0, 0.0),
        ("prop_folding_chair", "garage-folding-chair", (2.45, 0.0, 2.65), 15.0, 0.0),
        ("prop_crt_tower", "garage-crt-tower", (3.15, 0.0, 1.35), 0.0, 0.0),
        ("prop_server_closet", "garage-server-closet", (1.05, 0.0, 3.15), 0.0, 90.0),
        ("prop_whiteboard", "garage-whiteboard-1995", (2.0, 0.0, 0.22), 0.0, 0.0),
        ("prop_cable_spool", "garage-cable-spool", (0.95, 0.0, 1.65), 0.0, 0.0),
        ("prop_coffee_drip", "garage-coffee-drip", (3.35, 0.0, 0.95), 0.0, -12.0),
    ]


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


def collect_prop_meta(
    kit_objects: dict[str, bpy.types.Object],
    prop_keys: list[str],
) -> list[dict]:
    out: list[dict] = []
    bounds = (0.0, ROOM, 0.0, ROOM)
    for key in prop_keys:
        ob = kit_objects[key]
        cx = ob.location.x
        cz = ob.location.z
        inside = bounds[0] <= cx <= bounds[1] and bounds[2] <= cz <= bounds[3]
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

    kit_floor = build_floor()
    kit_wall_back = build_wall_strip("back")
    kit_wall_left = build_wall_strip("left")

    extent = N_ALONG * TILE
    kit_roll_door = place_garage("garage-roll-door", "kit_roll_door", (extent * 0.5, 0.0, 0.12))
    kit_window = place_garage(
        "garage-window-frame",
        "kit_window",
        (0.12, 1.05, extent * 0.52),
        rot_y_deg=90,
    )

    structure = [kit_floor, kit_wall_back, kit_wall_left, kit_roll_door, kit_window]
    sx = _room_scale_horizontal(structure)

    kit_objects: dict[str, bpy.types.Object] = {
        "kit_floor": kit_floor,
        "kit_wall_back": kit_wall_back,
        "kit_wall_left": kit_wall_left,
        "kit_roll_door": kit_roll_door,
        "kit_window": kit_window,
    }

    prop_specs = prop_positions()
    for key, asset_id, loc, rot_z, rot_y in prop_specs:
        scaled = (loc[0] * sx, loc[1], loc[2] * sx)
        ob = place_garage(asset_id, key, scaled, rot_y_deg=rot_y, rot_z_deg=rot_z)
        kit_objects[key] = ob

    export_list = list(kit_objects.values())
    _align_room_to_origin(export_list)

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

    preview_path = cc0.PREVIEW_ROOT / f"{ASSET_ID}.png"
    scene_preview = cc0.PREVIEW_ROOT / f"{ASSET_ID}-scene.png"
    frame_kit = [
        kit_objects[k]
        for k in (
            "kit_floor",
            "kit_wall_back",
            "kit_wall_left",
            "kit_roll_door",
            "kit_window",
        )
    ]
    cc0.render_kit_preview(
        export_list,
        preview_path,
        resolution=768,
        mode="hero",
        fill=0.70,
        margin=0.05,
        frame_objects=frame_kit,
    )
    cc0.render_kit_preview(
        export_list,
        scene_preview,
        resolution=1024,
        mode="scene",
        fill=0.70,
        margin=0.05,
        frame_objects=frame_kit,
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
        "license": "CC0-1.0 (garage-kit derivatives)",
        "sources": ["garage-kit"],
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