#!/usr/bin/env python3
"""Garage preview gates: alpha-mask silhouette, object-only sat, frame meta."""
from __future__ import annotations

import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"

sys.path.insert(0, str(REPO / "tools/art"))
from preview_quality_lib import (  # noqa: E402
    MAX_SILHOUETTE_HEIGHT,
    MIN_LUMA_VAR,
    MIN_SAT,
    MIN_SILHOUETTE_HEIGHT,
    analyze_alpha_mask,
    analyze_preview,
    mask_gate_errors,
)


def check_asset(asset_id: str, *, source: str | None = None) -> int:
    path = PREVIEW / f"{asset_id}.png"
    mask = PREVIEW / f"{asset_id}.mask.png"
    meta = PREVIEW / f"{asset_id}.frame.json"
    if not path.exists():
        print(f"FAIL missing preview {asset_id}")
        return 1
    if not mask.exists():
        print(f"FAIL missing mask {asset_id}.mask.png")
        return 1
    if not meta.exists():
        print(f"FAIL missing frame meta {asset_id}.frame.json")
        return 1

    rc = 0
    for err in mask_gate_errors(mask):
        print(f"FAIL {asset_id} {err}")
        rc = 1

    sat, lvar, height_cov, _floor_sat, obj_px = analyze_preview(path, mask)
    alpha = analyze_alpha_mask(mask)
    if rc == 0 and (height_cov < MIN_SILHOUETTE_HEIGHT or height_cov > MAX_SILHOUETTE_HEIGHT):
        print(
            f"FAIL {asset_id} alpha_extent={height_cov:.3f} "
            f"(need {MIN_SILHOUETTE_HEIGHT}-{MAX_SILHOUETTE_HEIGHT})"
        )
        rc = 1
    cc0 = source == "cc0-polyhaven"
    if cc0 and rc == 0:
        print(
            f"OK {asset_id} cc0 preview sat={sat:.3f} alpha_extent={height_cov:.3f} "
            f"area={float(alpha['area_coverage']):.3f} (Hatch visual)"
        )
        return 0
    if sat < MIN_SAT:
        print(f"FAIL {asset_id} object_sat={sat:.3f} < {MIN_SAT} (mask pixels={obj_px})")
        rc = 1
    if lvar < MIN_LUMA_VAR:
        print(f"FAIL {asset_id} flat object luma_var={lvar:.4f}")
        rc = 1
    if rc == 0:
        print(
            f"OK {asset_id} obj_sat={sat:.3f} alpha_extent={height_cov:.3f} "
            f"area={float(alpha['area_coverage']):.3f} lvar={lvar:.4f}"
        )
    return rc


def main() -> int:
    try:
        from PIL import Image  # noqa: F401
    except ImportError:
        print("FAIL: Pillow required")
        return 1

    assets = json.loads(MANIFEST.read_text())["assets"]
    rc = 0
    for entry in assets:
        rc |= check_asset(entry["id"], source=entry.get("source"))

    scene_id = "garage-scene-kit"
    scene = PREVIEW / f"{scene_id}.png"
    scene_mask = PREVIEW / f"{scene_id}.mask.png"
    scene_meta = PREVIEW / f"{scene_id}.frame.json"
    if scene.exists() and scene_meta.exists() and scene_mask.exists():
        for err in mask_gate_errors(scene_mask, asset_id=scene_id):
            print(f"FAIL {scene_id} {err}")
            rc = 1
        sat, _lvar, extent, _fs, px = analyze_preview(scene, scene_mask)
        alpha = analyze_alpha_mask(scene_mask)
        area = float(alpha["area_coverage"])
        scene_min_sat = 0.08
        if sat < scene_min_sat:
            print(f"FAIL {scene_id} object_sat={sat:.3f} (mask pixels={px})")
            rc = 1
        else:
            print(
                f"OK {scene_id} obj_sat={sat:.3f} alpha_extent={extent:.3f} "
                f"area={area:.3f}"
            )
    else:
        print(f"FAIL missing {scene_id} preview, mask, or frame meta")
        rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())