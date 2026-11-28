#!/usr/bin/env python3
"""CI gate: garage-desk-crt GLB seats CRT on desktop (hero camera projection)."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy

REPO = Path(__file__).resolve().parents[3]
GLB = REPO / "unity/Assets/Art/EarlyGame/garage-kit/garage-desk-crt.glb"


def main() -> int:
    if not GLB.is_file():
        print(f"FAIL missing {GLB}")
        return 1
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    import blender_common as bc  # type: ignore

    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=str(GLB))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    meshes = bc.flatten_glb_preview_meshes(meshes)
    try:
        bc.assert_desk_crt_on_desktop_for_preview(meshes)
    except RuntimeError as exc:
        print(f"FAIL garage-desk-crt {exc}")
        return 1
    print("OK garage-desk-crt preview_placement (world + hero camera)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())