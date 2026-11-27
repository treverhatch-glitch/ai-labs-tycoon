#!/usr/bin/env python3
"""0° face-on diagnostic — Quaternius researcher hero."""
from __future__ import annotations

import os
import sys
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import cc0_staff_lib as c0  # noqa: E402


def main() -> None:
    if not c0.MODULAR_GLB.is_file():
        print("FAIL face_diagnostic missing quaternius modular glb")
        raise SystemExit(1)
    c0.build_researcher_cc0()
    meshes = c0.collect_researcher_scene_meshes()
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