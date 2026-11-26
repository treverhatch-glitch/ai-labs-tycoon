#!/usr/bin/env python3
"""Garage interior scene — shell + 15 props at game scale and camera."""
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

# Interior ~5×4 m; props at readable game scale (Y-up)
PROP_SLOTS: list[tuple[str, tuple[float, float, float], tuple[float, float, float]]] = [
    ("garage-desk-crt", (1.35, 0.0, 0.35), (1.0, 1.0, 1.0)),
    ("garage-folding-chair", (-0.55, 0.0, 0.55), (1.0, 1.0, 1.0)),
    ("garage-coffee-drip", (0.15, 0.0, 1.05), (1.0, 1.0, 1.0)),
    ("garage-crt-tower", (1.65, 0.0, 0.85), (1.0, 1.0, 1.0)),
    ("garage-cable-spool", (-0.85, 0.0, 1.15), (1.0, 1.0, 1.0)),
    ("garage-server-closet", (-1.55, 0.0, 1.35), (1.0, 1.0, 1.0)),
    ("garage-voodude-rack", (0.55, 0.0, -0.25), (1.0, 1.0, 1.0)),
    ("garage-whiteboard-1995", (1.85, 0.0, -0.55), (1.0, 1.0, 1.0)),
    ("garage-window-frame", (1.0, 0.0, -1.65), (1.0, 1.0, 1.0)),
    ("garage-land-for-sale-sign", (-1.95, 0.0, 0.25), (1.0, 1.0, 1.0)),
    ("garage-lot-curb", (2.15, 0.0, 1.45), (1.0, 1.0, 1.0)),
]


def build_garage_shell() -> list[bpy.types.Object]:
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    import blender_common as bc  # noqa: E402

    floor = bc.add_cube("GarageFloor", (5.2, 0.08, 4.2), (0.0, 0.04, 0.0))
    back = bc.add_cube("GarageBackWall", (5.0, 2.65, 0.14), (0.0, 1.32, -2.0))
    left = bc.add_cube("GarageLeftWall", (0.14, 2.65, 4.0), (-2.45, 1.32, 0.0))
    right = bc.add_cube("GarageRightWall", (0.14, 2.65, 4.0), (2.45, 1.32, 0.0))
    header = bc.add_cube("GarageHeader", (3.2, 0.22, 0.14), (0.0, 2.45, -1.98))
    for ob in (floor, back, left, right, header):
        bc.assign_solid(ob, (0.55, 0.53, 0.50, 1.0), 0.62)
    return [floor, back, left, right, header]


def import_prop(asset_id: str, loc: tuple[float, float, float], scale: tuple[float, float, float]) -> bpy.types.Object | None:
    glb = KIT / f"{asset_id}.glb"
    if not glb.exists():
        print(f"WARN missing {glb}")
        return None
    bpy.ops.import_scene.gltf(filepath=str(glb))
    imported = [o for o in bpy.context.selected_objects if o.type == "MESH"]
    if not imported:
        return None
    bpy.ops.object.select_all(action="DESELECT")
    for o in imported:
        o.select_set(True)
    bpy.context.view_layer.objects.active = imported[0]
    if len(imported) > 1:
        bpy.ops.object.join()
    root = bpy.context.view_layer.objects.active
    root.name = asset_id
    root.location = Vector(loc)
    root.scale = Vector(scale)
    bpy.ops.object.transform_apply(scale=True)
    bpy.context.view_layer.update()
    return root


def main() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    shell = build_garage_shell()
    props: list[bpy.types.Object] = []
    roll = import_prop("garage-roll-door", (0.0, 0.0, -1.92), (1.0, 1.0, 1.0))
    if roll:
        props.append(roll)
    panel = import_prop("garage-wall-panel", (-2.38, 0.0, -0.4), (1.0, 1.0, 1.0))
    if panel:
        props.append(panel)
    for asset_id, loc, scale in PROP_SLOTS:
        ob = import_prop(asset_id, loc, scale)
        if ob:
            props.append(ob)

    import blender_preview_rig as rig  # type: ignore

    all_meshes = shell + props
    out = PREVIEW / "garage-scene-kit.png"
    PREVIEW.mkdir(parents=True, exist_ok=True)
    rig.render_preview_studio(
        all_meshes,
        out,
        resolution=768,
        up_axis="Y",
        lens_mm=38.0,
        azimuth_deg=34.0,
        elevation_deg=23.0,
        framing_margin=1.06,
        mask_meshes=props,
    )
    print("OK garage-scene-kit", out, "meshes", len(all_meshes))


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()