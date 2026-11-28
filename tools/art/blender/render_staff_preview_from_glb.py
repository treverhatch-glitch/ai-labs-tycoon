#!/usr/bin/env python3
"""Headless studio preview from an on-disk staff GLB (isolated Blender process)."""
from __future__ import annotations

import os
import sys
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main() -> int:
    if "--" not in sys.argv:
        print("FAIL preview_from_glb usage: glb out asset_id", file=sys.stderr)
        return 1
    idx = sys.argv.index("--") + 1
    glb_path = Path(sys.argv[idx])
    preview_path = Path(sys.argv[idx + 1])
    asset_id = sys.argv[idx + 2]
    os.environ.setdefault("EARLY_ART_PREVIEW_RIG", "1")
    os.environ.setdefault("STAFF_PREVIEW_MARGIN", "1.42")
    os.environ.setdefault("STAFF_PREVIEW_YAW_DEG", "30")
    os.environ.setdefault("STAFF_PREVIEW_ELEV_DEG", "10")
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bc.render_preview_from_glb(glb_path, preview_path, asset_id=asset_id)
    if not preview_path.is_file():
        print(f"FAIL preview_from_glb missing {preview_path}")
        return 1
    mask = preview_path.with_name(preview_path.stem + ".mask.png")
    if not mask.is_file():
        print(f"FAIL preview_from_glb missing {mask}")
        return 1
    print(f"OK preview_from_glb {preview_path.name}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())