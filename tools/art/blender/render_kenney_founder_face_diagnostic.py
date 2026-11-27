#!/usr/bin/env python3
"""0° face-on diagnostic (Kenney +Y front), saved under docs/art/previews/."""
from __future__ import annotations

import os
import sys
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
from kenney_founder_stock_recolor import (  # noqa: E402
    STOCK_BLEND,
    apply_founder_materials,
    collect_founder_stock_meshes,
)
from founder_face_decals import spawn_founder_eye_meshes  # noqa: E402


def main() -> None:
    if not STOCK_BLEND.is_file():
        print("FAIL face_diagnostic missing stock blend (run stock_check first)")
        raise SystemExit(1)
    bpy.ops.wm.open_mainfile(filepath=str(STOCK_BLEND))
    ob = next((o for o in bpy.context.scene.objects if o.type == "MESH"), None)
    if not ob:
        print("FAIL face_diagnostic no_mesh")
        raise SystemExit(1)
    apply_founder_materials(ob)
    spawn_founder_eye_meshes(ob)
    meshes = collect_founder_stock_meshes()
    os.environ["EARLY_ART_PREVIEW_RIG"] = "1"
    os.environ["STAFF_PREVIEW_YAW_DEG"] = "0"
    os.environ["STAFF_PREVIEW_ELEV_DEG"] = "8"
    os.environ["STAFF_PREVIEW_MARGIN"] = "1.35"
    out = bc.PREVIEW_ROOT / "staff-founder-face-diagnostic-0deg.png"
    os.environ["STAFF_PREVIEW_ASSET"] = "staff-founder-idle"
    bc.render_preview(meshes, out, asset_id="staff-founder-idle")
    print(f"OK face_diagnostic wrote {out}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()