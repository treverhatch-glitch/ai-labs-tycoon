#!/usr/bin/env python3
"""Hero CC0 asset: cut-away garage corner + 6 CC0 furnishings."""
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


def wall_strip(axis: str) -> bpy.types.Object:
    """Two Kenney wall modules (4 m) on y=0 and x=0; open corner at (+X,+Y)."""
    if axis == "back":
        a = place(cc0.kenney_glb("wall.glb"), "wall_b_a", (CELL * 0.5, 0.0, 0.0))
        b = place(cc0.kenney_glb("wall.glb"), "wall_b_b", (CELL * 1.5, 0.0, 0.0))
        return cc0.merge_mesh_objects([a, b], "kit_wall_back")
    a = place(cc0.kenney_glb("wall.glb"), "wall_l_a", (0.0, CELL * 0.5, 0.0), rot_z_deg=90)
    b = place(cc0.kenney_glb("wall.glb"), "wall_l_b", (0.0, CELL * 1.5, 0.0), rot_z_deg=90)
    return cc0.merge_mesh_objects([a, b], "kit_wall_left")


def main() -> dict:
    cc0.reset_scene()
    kit_root = cc0.INCOMING / "kenney_building-kit/Models/GLB format"
    if not kit_root.is_dir():
        raise RuntimeError(f"run tools/art/cc0/download_cc0_packs.sh first; missing {kit_root}")

    floor_parts: list[bpy.types.Object] = []
    for ix in (0, 1):
        for iz in (0, 1):
            m = cc0.import_gltf(cc0.kenney_glb("floor.glb"), f"floor_{ix}_{iz}")
            ob = cc0.merge_mesh_objects(m, f"floor_{ix}_{iz}")
            ob.location = (ix * CELL, iz * CELL, 0)
            floor_parts.append(ob)
    kit_floor = cc0.merge_mesh_objects(floor_parts, "kit_floor")

    kit_wall_back = wall_strip("back")
    kit_wall_left = wall_strip("left")

    trim_back = place(cc0.kenney_glb("border-high.glb"), "trim_back", (CELL, 0.04, 2.05))
    trim_left = place(cc0.kenney_glb("border-high.glb"), "trim_left", (0.04, CELL, 2.05), rot_z_deg=90)

    kit_window = place(cc0.kenney_glb("wall-window-square.glb"), "kit_window", (0.05, 1.25, 1.12), rot_z_deg=90)

    doorway = place(cc0.kenney_glb("wall-doorway-wide-square.glb"), "door_frame", (1.35, 0.05, 0.0))
    door_leaf = place(cc0.kenney_glb("door-rotate-square-d.glb"), "door_leaf", (1.65, 0.14, 0.85), rot_z_deg=175)
    door_slat = place(cc0.kenney_glb("plating-wide.glb"), "door_slat", (1.25, 0.08, 1.5))
    door_slat.scale = (1.1, 0.35, 0.1)
    kit_roll_door = cc0.merge_mesh_objects([doorway, door_leaf, door_slat], "kit_roll_door")

    prop_workbench = place(cc0.kenney_glb("plating-detailed.glb"), "wb_top", (2.35, 2.15, 0.75))
    prop_workbench.scale = (1.05, 0.82, 0.22)
    leg_a = place(cc0.kenney_glb("column-thin.glb"), "wb_leg_a", (2.0, 1.85, 0.0))
    leg_b = place(cc0.kenney_glb("column-thin.glb"), "wb_leg_b", (2.75, 2.55, 0.0))
    prop_workbench = cc0.merge_mesh_objects([prop_workbench, leg_a, leg_b], "prop_workbench")

    shelf_a = place(cc0.kenney_glb("plating.glb"), "shelf_a", (0.35, 2.35, 0.65))
    shelf_b = place(cc0.kenney_glb("plating.glb"), "shelf_b", (0.35, 2.35, 1.2))
    shelf_post = place(cc0.kenney_glb("column-thin.glb"), "shelf_post", (0.22, 2.2, 0.0))
    prop_shelving = cc0.merge_mesh_objects([shelf_a, shelf_b, shelf_post], "prop_shelving")

    desk = place(cc0.kenney_glb("door-rotate-square-b.glb"), "desk_top", (1.55, 2.85, 0.72))
    desk.scale = (0.55, 0.95, 0.35)
    pc = place(cc0.kenney_glb("detail-pipe.glb"), "pc_tower", (1.35, 3.05, 0.72))
    prop_desk_pc = cc0.merge_mesh_objects([desk, pc], "prop_desk_pc")

    cable_a = place(cc0.kenney_glb("detail-pipe.glb"), "cable_a", (2.55, 2.35, 0.04), rot_z_deg=18)
    cable_b = place(cc0.kenney_glb("detail-pipe.glb"), "cable_b", (2.85, 1.85, 0.04), rot_z_deg=-12)
    prop_cables = cc0.merge_mesh_objects([cable_a, cable_b], "prop_cables")

    prop_whiteboard = place(cc0.kenney_glb("wall-half.glb"), "prop_whiteboard", (1.1, 0.1, 1.35))
    prop_whiteboard.scale = (1.45, 0.12, 1.05)

    bpy.data.objects.remove(trim_back, do_unlink=True)
    bpy.data.objects.remove(trim_left, do_unlink=True)

    export_list = [
        kit_floor,
        kit_wall_back,
        kit_wall_left,
        kit_roll_door,
        kit_window,
        prop_workbench,
        prop_shelving,
        prop_desk_pc,
        prop_cables,
        prop_whiteboard,
    ]
    center, _ = cc0._bounds_center(export_list)
    sx, sy = 2.0 - center.x, 2.0 - center.y
    for ob in export_list:
        ob.location.x += sx
        ob.location.y += sy

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
        cc0.apply_palette_material(ob, ROLE[key])

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
    furnishings = [kit_objects[k] for k in kit_objects if k.startswith("prop_") or k == "kit_roll_door"]
    cc0.render_kit_preview(
        export_list,
        preview_path,
        resolution=768,
        mode="hero",
        fill=0.70,
        margin=0.05,
    )
    cc0.render_kit_preview(export_list, scene_preview, resolution=1024, mode="scene", fill=0.70, margin=0.05)

    total_tris = sum(len(ob.data.polygons) for ob in export_list)
    furnish_ids = ["kit_roll_door"] + [k for k in kit_objects if k.startswith("prop_")]
    entry = {
        "id": ASSET_ID,
        "hero": True,
        "path": f"Assets/Art/EarlyGame/{CATEGORY}/{ASSET_ID}.glb",
        "preview": f"docs/art/previews/{ASSET_ID}.png",
        "scenePreview": f"docs/art/previews/{ASSET_ID}-scene.png",
        "textures": {
            part: {
                k: f"Assets/Art/EarlyGame/{CATEGORY}/textures/{ASSET_ID}/{part}/{ASSET_ID}_{part}_{k}.png"
                for k in maps
            }
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