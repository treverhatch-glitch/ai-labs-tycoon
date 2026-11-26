#!/usr/bin/env python3
"""Contact sheet: 15 garage props on mid-grey floor, 3/4 view — no shell/backdrop."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
PREVIEW = REPO / "docs/art/previews"
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"

FLOOR_Y = 0.0
COLS = 5
SPACING_X = 1.35
SPACING_Z = 1.25


def mesh_min_y(ob: bpy.types.Object) -> float:
    return min((ob.matrix_world @ v.co).y for v in ob.data.vertices)


def snap_prop_to_floor(ob: bpy.types.Object, floor_y: float = FLOOR_Y) -> None:
    bpy.context.view_layer.update()
    low = mesh_min_y(ob)
    if abs(low - floor_y) > 1e-4:
        ob.location.y += floor_y - low
    bpy.context.view_layer.update()


def import_prop(asset_id: str, loc: tuple[float, float, float], rot_y_deg: float) -> list[bpy.types.Object]:
    glb = KIT / f"{asset_id}.glb"
    if not glb.exists():
        print(f"WARN missing {glb}")
        return []
    bpy.ops.import_scene.gltf(filepath=str(glb))
    imported = [o for o in bpy.context.selected_objects if o.type == "MESH"]
    if not imported:
        return []
    rot_rad = rot_y_deg * 3.14159265 / 180.0
    import blender_common as common  # type: ignore

    for ob in imported:
        ob.location = Vector(loc)
        ob.rotation_euler[1] = rot_rad
        bpy.ops.object.select_all(action="DESELECT")
        ob.select_set(True)
        bpy.context.view_layer.objects.active = ob
        bpy.ops.object.transform_apply(rotation=True)
        snap_prop_to_floor(ob)
        common._boost_garage_part_preview_material(ob)
        common._boost_garage_part_preview_material(ob)
    return imported


def layout_from_manifest() -> list[tuple[str, tuple[float, float, float], float]]:
    data = json.loads(MANIFEST.read_text(encoding="utf-8"))
    ids = [a["id"] for a in data["assets"]]
    rows: list[tuple[str, tuple[float, float, float], float]] = []
    ox = -((COLS - 1) * SPACING_X) * 0.5
    for i, asset_id in enumerate(ids):
        col = i % COLS
        row = i // COLS
        x = ox + col * SPACING_X
        z = row * SPACING_Z
        rot = 22.0 if col % 2 == 0 else -18.0
        rows.append((asset_id, (x, FLOOR_Y, z), rot))
    return rows


def main() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    props: list[bpy.types.Object] = []
    for asset_id, loc, rot_y in layout_from_manifest():
        props.extend(import_prop(asset_id, loc, rot_y))

    import blender_preview_rig as rig  # type: ignore

    out = PREVIEW / "garage-scene-kit.png"
    PREVIEW.mkdir(parents=True, exist_ok=True)
    rig.render_preview_studio(
        props,
        out,
        resolution=768,
        up_axis="Y",
        lens_mm=42.0,
        azimuth_deg=42.0,
        elevation_deg=30.0,
        framing_margin=1.12,
        mask_meshes=props,
    )
    print("OK garage-scene-kit", out, "props", len(props))


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()