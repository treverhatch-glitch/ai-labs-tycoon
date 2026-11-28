#!/usr/bin/env python3
"""0° face-on diagnostic — Kenney researcher hero."""
from __future__ import annotations

import os
import sys
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import kenney_researcher_lib as kr  # noqa: E402


def main() -> None:
    os.environ["RESEARCHER_STOCK_RECOLOR"] = "1"
    os.environ["RESEARCHER_SKIP_LAB_COAT"] = "1"
    kr.build_researcher_kenney()
    meshes = kr.collect_researcher_scene_meshes()
    for ob in meshes:
        if ob.type != "MESH" or not ob.name.split(".")[0].startswith("FounderEye_"):
            continue
        mat = ob.active_material
        if mat and mat.use_nodes:
            bsdf = mat.node_tree.nodes.get("Principled BSDF")
            if bsdf:
                bsdf.inputs["Base Color"].default_value = (0.03, 0.03, 0.04, 1.0)
                bsdf.inputs["Roughness"].default_value = 1.0
    os.environ["EARLY_ART_PREVIEW_RIG"] = "1"
    os.environ["STAFF_PREVIEW_YAW_DEG"] = "0"
    os.environ["STAFF_PREVIEW_ELEV_DEG"] = "8"
    os.environ["STAFF_PREVIEW_MARGIN"] = "1.35"
    out = bc.PREVIEW_ROOT / "staff-researcher-face-diagnostic-0deg.png"
    os.environ["STAFF_PREVIEW_ASSET"] = "staff-researcher-idle"
    bc.render_preview(meshes, out, asset_id="staff-researcher-idle")
    print(f"OK face_diagnostic wrote {out}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()