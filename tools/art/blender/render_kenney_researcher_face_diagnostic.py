#!/usr/bin/env python3
"""0° face-on diagnostic (Kenney +Y front), researcher recolor."""
from __future__ import annotations

import os
import sys
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
from kenney_researcher_stock_recolor import (  # noqa: E402
    STOCK_META,
    STOCK_OBJECT_NAME,
    apply_researcher_materials,
    bake_researcher_stock_body,
    collect_researcher_stock_meshes,
    spawn_researcher_eye_meshes,
    spawn_researcher_glasses,
)


def main() -> None:
    if not STOCK_META.is_file():
        print("FAIL face_diagnostic missing stock meta")
        raise SystemExit(1)
    ob = bake_researcher_stock_body()
    ob.name = STOCK_OBJECT_NAME
    apply_researcher_materials(ob)
    spawn_researcher_eye_meshes(ob)
    spawn_researcher_glasses(ob)
    meshes = collect_researcher_stock_meshes()
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