#!/usr/bin/env python3
"""Re-render staff preview + mask from committed multi-mesh GLB (no rebake)."""
from __future__ import annotations

import os
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main() -> None:
    asset_id = os.environ.get("STAFF_PREVIEW_ASSET", "staff-engineer-work")
    os.environ["EARLY_ART_STAFF_CC0"] = "1"
    os.environ["EARLY_ART_CC0_HDRI"] = "1"
    os.environ["EARLY_ART_PREVIEW_KIND"] = "staff"
    glb = bc.ART_ROOT / "staff-kit" / f"{asset_id}.glb"
    if not glb.is_file():
        raise FileNotFoundError(glb)
    bpy = __import__("bpy")
    bpy.ops.import_scene.gltf(filepath=str(glb))
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    import staff_cc0 as scc0  # noqa: WPS433

    scc0.validate_cc0_upright(mesh_obs, asset_id)
    preview_path = bc.PREVIEW_ROOT / f"{asset_id}.png"
    bc.render_preview(mesh_obs, preview_path, asset_id=asset_id)
    print("OK", preview_path)


if __name__ == "__main__":
    bc.run_script(main)