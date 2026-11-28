#!/usr/bin/env python3
"""Contact sheet: 15 garage props — warm albedo, single studio frame."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
TEX = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures"
PREVIEW = REPO / "docs/art/previews"
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"

FLOOR_Y = 0.0
COLS = 5
SPACING_X = 1.05
SPACING_Z = 0.95


def mesh_min_y(ob: bpy.types.Object) -> float:
    return min((ob.matrix_world @ v.co).y for v in ob.data.vertices)


def _group_bounds_y(meshes: list[bpy.types.Object]) -> tuple[float, float]:
    ymin, ymax = 1e9, -1e9
    for ob in meshes:
        for v in ob.data.vertices:
            wy = (ob.matrix_world @ v.co).y
            ymin = min(ymin, wy)
            ymax = max(ymax, wy)
    return ymin, ymax


def import_prop(asset_id: str, loc: tuple[float, float, float], rot_y_deg: float) -> bpy.types.Object | None:
    glb = KIT / f"{asset_id}.glb"
    if not glb.exists():
        print(f"WARN missing {glb}")
        return None
    before = set(bpy.context.scene.objects)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    imported = [o for o in bpy.context.scene.objects if o not in before and o.type == "MESH"]
    if not imported:
        return None
    pivot = bpy.data.objects.new(f"prop_{asset_id}", None)
    bpy.context.collection.objects.link(pivot)
    pivot.location = Vector(loc)
    pivot.rotation_euler[1] = rot_y_deg * 3.14159265 / 180.0
    for ob in imported:
        ob.parent = pivot
        ob.matrix_parent_inverse = pivot.matrix_world.inverted()
    bpy.context.view_layer.update()
    ymin, _ = _group_bounds_y(imported)
    if abs(ymin - FLOOR_Y) > 1e-4:
        pivot.location.y += FLOOR_Y - ymin
    bpy.context.view_layer.update()
    import blender_common as common  # type: ignore

    tex_dir = TEX / asset_id
    joined = common.duplicate_join_meshes(imported, f"{asset_id}_scene_join")
    joined.parent = pivot
    joined.matrix_parent_inverse = pivot.matrix_world.inverted()
    common.apply_preview_material(joined, tex_dir, asset_id)
    return pivot


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
        rot = 18.0 if col % 2 == 0 else -14.0
        rows.append((asset_id, (x, FLOOR_Y, z), rot))
    return rows


def main() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    hero_meshes: list[bpy.types.Object] = []
    for asset_id, loc, rot_y in layout_from_manifest():
        pivot = import_prop(asset_id, loc, rot_y)
        if pivot:
            join = bpy.data.objects.get(f"{asset_id}_scene_join")
            if join:
                hero_meshes.append(join)

    import os

    import blender_preview_rig as rig  # type: ignore

    os.environ["EARLY_ART_PREVIEW_KIND"] = "garage"
    os.environ["GARAGE_PREVIEW_FLOOR_SCALE"] = "1.04"
    out = PREVIEW / "garage-scene-kit.png"
    PREVIEW.mkdir(parents=True, exist_ok=True)
    rig.render_preview_studio(
        hero_meshes,
        out,
        resolution=768,
        up_axis="Y",
        lens_mm=42.0,
        azimuth_deg=36.0,
        elevation_deg=28.0,
        framing_margin=1.10,
        mask_meshes=hero_meshes,
    )
    print("OK garage-scene-kit", out, "props", len(hero_meshes))


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()