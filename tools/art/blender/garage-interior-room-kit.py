#!/usr/bin/env python3
"""Hero CC0 asset: cut-away garage interior + 6 CC0 furnishings."""
from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import bpy  # noqa: E402
import cc0_common as cc0  # noqa: E402


ASSET_ID = "garage-interior-room-kit"
CATEGORY = "cc0-garage-kit"
CELL = 2.0

ROLE = {
    "kit_floor": "floor",
    "kit_wall_back": "wall",
    "kit_wall_left": "wall",
    "kit_roll_door": "metal",
    "kit_window": "glass",
    "prop_workbench": "prop",
    "prop_shelving": "prop",
    "prop_desk_pc": "prop",
    "prop_cables": "metal",
    "prop_whiteboard": "board",
}


def place(path: Path, name: str, loc: tuple[float, float, float], rot_z_deg: float = 0) -> bpy.types.Object:
    meshes = cc0.import_gltf(path, name)
    ob = cc0.merge_mesh_objects(meshes, name)
    ob.location = loc
    if rot_z_deg:
        ob.rotation_euler[2] = cc0.math.radians(rot_z_deg)
    return ob


def main() -> dict:
    cc0.reset_scene()
    kit_root = cc0.INCOMING / "kenney_building-kit/Models/GLB format"
    if not kit_root.is_dir():
        raise RuntimeError(f"run tools/art/cc0/download_cc0_packs.sh first; missing {kit_root}")

    # Floor (3×3 tiles)
    floor_parts: list[bpy.types.Object] = []
    for ix in range(-1, 2):
        for iz in range(-1, 2):
            m = cc0.import_gltf(cc0.kenney_glb("floor.glb"), f"floor_{ix}_{iz}")
            ob = cc0.merge_mesh_objects(m, f"floor_{ix}_{iz}")
            ob.location = (ix * CELL, iz * CELL, 0)
            floor_parts.append(ob)
    kit_floor = cc0.merge_mesh_objects(floor_parts, "kit_floor")

    # Cut-away shell: back + left wall only (open to camera corner)
    kit_wall_back = place(cc0.kenney_glb("wall.glb"), "kit_wall_back", (0, -CELL, 0))
    kit_wall_left = place(cc0.kenney_glb("wall.glb"), "kit_wall_left", (-CELL, 0, 0), rot_z_deg=90)

    kit_window = place(cc0.kenney_glb("wall-window-square.glb"), "kit_window", (-CELL + 0.02, -0.5, 1.1), rot_z_deg=90)

    doorway = place(cc0.kenney_glb("wall-doorway-wide-square.glb"), "door_frame", (0, CELL, 0), rot_z_deg=180)
    door_leaf = place(cc0.kenney_glb("door-rotate-square-d.glb"), "door_leaf", (0.6, CELL - 0.12, 0.85), rot_z_deg=160)
    door_slat = place(cc0.kenney_glb("plating-wide.glb"), "door_slat", (0, CELL - 0.05, 1.45), rot_z_deg=180)
    door_slat.scale = (1.15, 0.4, 0.1)
    kit_roll_door = cc0.merge_mesh_objects([doorway, door_leaf, door_slat], "kit_roll_door")

    # ≥6 CC0 furnishings (Kenney Building Kit)
    prop_workbench = place(cc0.kenney_glb("plating-detailed.glb"), "prop_workbench", (0.6, -0.4, 0.05))
    prop_workbench.scale = (1.1, 0.85, 1.0)
    leg = place(cc0.kenney_glb("column-thin.glb"), "wb_leg", (0.2, -0.8, 0.0))
    prop_workbench = cc0.merge_mesh_objects([prop_workbench, leg], "prop_workbench")

    shelf_a = place(cc0.kenney_glb("plating.glb"), "shelf_a", (-1.2, -1.2, 0.6))
    shelf_b = place(cc0.kenney_glb("plating.glb"), "shelf_b", (-1.2, -1.2, 1.2))
    post = place(cc0.kenney_glb("column-thin.glb"), "shelf_post", (-1.45, -1.35, 0.0))
    prop_shelving = cc0.merge_mesh_objects([shelf_a, shelf_b, post], "prop_shelving")

    desk = place(cc0.kenney_glb("door-rotate-square-b.glb"), "desk_top", (-0.3, 0.35, 0.75), rot_z_deg=90)
    desk.scale = (0.9, 0.55, 0.45)
    pc = place(cc0.kenney_glb("detail-pipe.glb"), "pc_tower", (-0.55, 0.55, 0.75))
    prop_desk_pc = cc0.merge_mesh_objects([desk, pc], "prop_desk_pc")

    cable_a = place(cc0.kenney_glb("detail-pipe.glb"), "cable_a", (0.9, 0.3, 0.05), rot_z_deg=25)
    cable_b = place(cc0.kenney_glb("detail-pipe.glb"), "cable_b", (1.1, -0.1, 0.05), rot_z_deg=-15)
    prop_cables = cc0.merge_mesh_objects([cable_a, cable_b], "prop_cables")

    prop_whiteboard = place(cc0.kenney_glb("wall-half.glb"), "prop_whiteboard", (0.2, -CELL + 0.08, 1.35))
    prop_whiteboard.scale = (1.6, 0.15, 1.2)

    kit_objects: dict[str, bpy.types.Object] = {
        "kit_floor": kit_floor,
        "kit_wall_back": kit_wall_back,
        "kit_wall_left": kit_wall_left,
        "kit_roll_door": kit_roll_door,
        "kit_window": kit_window,
        "prop_workbench": prop_workbench,
        "prop_shelving": prop_shelving,
        "prop_desk_pc": prop_desk_pc,
        "prop_cables": prop_cables,
        "prop_whiteboard": prop_whiteboard,
    }

    for key, ob in kit_objects.items():
        cc0.bevel_object(ob, width=0.012, segments=2)
        bpy.ops.object.select_all(action="DESELECT")
        ob.select_set(True)
        bpy.context.view_layer.objects.active = ob
        bpy.ops.object.mode_set(mode="EDIT")
        bpy.ops.mesh.select_all(action="SELECT")
        bpy.ops.uv.smart_project(angle_limit=66, island_margin=0.02)
        bpy.ops.object.mode_set(mode="OBJECT")
        cc0.apply_kenney_material(ob, ROLE[key])

    export_list = list(kit_objects.values())
    out_dir = cc0.ART_ROOT / CATEGORY
    tex_dir = out_dir / "textures" / ASSET_ID
    out_dir.mkdir(parents=True, exist_ok=True)
    maps_by_part: dict[str, dict[str, str]] = {}
    for key, ob in kit_objects.items():
        sub = tex_dir / key
        maps_by_part[key] = cc0.bake_object_maps(ob, sub, f"{ASSET_ID}_{key}")

    glb_path = out_dir / f"{ASSET_ID}.glb"
    cc0.export_gltf_collection(export_list, glb_path)

    preview_path = cc0.PREVIEW_ROOT / f"{ASSET_ID}.png"
    scene_preview = cc0.PREVIEW_ROOT / f"{ASSET_ID}-scene.png"
    cc0.render_kit_preview(export_list, preview_path, resolution=768)
    cc0.render_kit_preview(export_list, scene_preview, resolution=1024)

    total_tris = sum(sum(len(p.vertices) for p in ob.data.polygons) for ob in export_list)
    furnish_ids = ["kit_roll_door"] + [k for k in kit_objects if k.startswith("prop_")]
    entry = {
        "id": ASSET_ID,
        "hero": True,
        "path": f"Assets/Art/EarlyGame/{CATEGORY}/{ASSET_ID}.glb",
        "preview": f"docs/art/previews/{ASSET_ID}.png",
        "scenePreview": f"docs/art/previews/{ASSET_ID}-scene.png",
        "textures": {
            part: {k: f"Assets/Art/EarlyGame/{CATEGORY}/textures/{ASSET_ID}/{part}/{ASSET_ID}_{part}_{k}.png"
                   for k in maps}
            for part, maps in maps_by_part.items()
        },
        "license": "CC0-1.0 (Kenney Building Kit + baked derivatives)",
        "sources": ["kenney_building-kit", "ambientcg-Concrete015", "polyhaven/studio_small_08_1k.hdr"],
        "furnishings": furnish_ids,
        "triangleCount": total_tris,
        "blenderVersion": cc0.BLENDER_VERSION,
        "generator": "tools/art/blender/garage-interior-room-kit.py",
        "parts": list(kit_objects.keys()),
    }
    cc0.write_cc0_manifest(entry)
    print("OK", ASSET_ID, total_tris, "tris", len(furnish_ids), "props")
    return entry


if __name__ == "__main__":
    import os

    os.chdir(cc0.REPO_ROOT)
    main()