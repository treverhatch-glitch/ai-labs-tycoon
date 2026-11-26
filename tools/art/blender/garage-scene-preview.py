#!/usr/bin/env python3
"""Combined garage scene preview — all 15 props, game-style 3/4 camera."""
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

# Approximate garage lot layout (meters, Y-up like garage scripts)
SLOTS: list[tuple[str, tuple[float, float, float]]] = [
    ("garage-floor-tile", (0.0, 0.0, 0.0)),
    ("garage-wall-panel", (-1.8, 1.1, -2.2)),
    ("garage-exterior-wall", (2.0, 1.2, -2.4)),
    ("garage-roll-door", (0.0, 1.1, -2.6)),
    ("garage-window-frame", (1.2, 1.5, -2.3)),
    ("garage-land-for-sale-sign", (-2.4, 0.8, 0.6)),
    ("garage-lot-curb", (2.6, 0.08, 1.0)),
    ("garage-folding-chair", (-0.8, 0.47, 0.5)),
    ("garage-desk-crt", (1.0, 0.38, 0.2)),
    ("garage-coffee-drip", (0.3, 0.29, 1.2)),
    ("garage-crt-tower", (1.8, 0.57, 0.3)),
    ("garage-server-closet", (-1.5, 1.0, 1.8)),
    ("garage-voodude-rack", (0.6, 0.97, -0.6)),
    ("garage-cable-spool", (-0.3, 0.2, 1.6)),
    ("garage-whiteboard-1995", (2.2, 1.08, -0.8)),
]


def main() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    heroes: list[bpy.types.Object] = []
    ids = [a["id"] for a in json.loads(MANIFEST.read_text())["assets"]]
    for asset_id in ids:
        glb = KIT / f"{asset_id}.glb"
        if not glb.exists():
            print(f"WARN missing {glb}")
            continue
        bpy.ops.import_scene.gltf(filepath=str(glb))
        imported = [o for o in bpy.context.selected_objects if o.type == "MESH"]
        if not imported:
            continue
        if len(imported) > 1:
            bpy.ops.object.select_all(action="DESELECT")
            for o in imported:
                o.select_set(True)
            bpy.context.view_layer.objects.active = imported[0]
            bpy.ops.object.join()
            root = bpy.context.view_layer.objects.active
        else:
            root = imported[0]
        loc = next((p for aid, p in SLOTS if aid == asset_id), (0.0, 0.0, 0.0))
        root.location = Vector(loc)
        root.name = asset_id
        heroes.append(root)

    import blender_preview_rig as rig  # type: ignore

    out = PREVIEW / "garage-scene-kit.png"
    PREVIEW.mkdir(parents=True, exist_ok=True)
    rig.render_preview_studio(heroes, out, resolution=768, up_axis="Y")
    print("OK garage-scene-kit", out)


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()