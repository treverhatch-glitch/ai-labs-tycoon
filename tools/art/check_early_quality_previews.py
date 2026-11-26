#!/usr/bin/env python3
"""Shared Hatch preview gates for any lane (staff / HUD / garage helpers)."""
from __future__ import annotations

import argparse
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "tools/art"))
from preview_quality_lib import MIN_LUMA_VAR, MIN_SAT, analyze_preview, mask_gate_errors  # noqa: E402


def check_id(preview_dir: Path, asset_id: str) -> int:
    rgb = preview_dir / f"{asset_id}.png"
    mask = preview_dir / f"{asset_id}.mask.png"
    meta = preview_dir / f"{asset_id}.frame.json"
    rc = 0
    if not rgb.is_file() or not mask.is_file() or not meta.is_file():
        print(f"FAIL missing rgb/mask/frame for {asset_id}")
        return 1
    for err in mask_gate_errors(mask):
        print(f"FAIL {asset_id} {err}")
        rc = 1
    sat, lvar, extent, _fs, px = analyze_preview(rgb, mask)
    if asset_id.startswith("wisp-"):
        min_sat = 0.08
    elif asset_id.startswith("staff-"):
        min_sat = 0.06
    else:
        min_sat = MIN_SAT
    if sat < min_sat:
        print(f"FAIL {asset_id} object_sat={sat:.3f} < {min_sat} (px={px})")
        rc = 1
    min_lvar = 0.0004 if asset_id.startswith("wisp-") else MIN_LUMA_VAR
    if lvar < min_lvar:
        print(f"FAIL {asset_id} luma_var={lvar:.4f}")
        rc = 1
    if rc == 0:
        print(f"OK {asset_id} obj_sat={sat:.3f} alpha_extent={extent:.3f}")
    return rc


def main() -> int:
    try:
        from PIL import Image  # noqa: F401
    except ImportError:
        print("FAIL: Pillow required")
        return 1
    ap = argparse.ArgumentParser(description="Hatch alpha-mask preview gate")
    ap.add_argument(
        "--preview-dir",
        type=Path,
        default=REPO / "docs/art/previews",
        help="Directory containing {id}.png, .mask.png, .frame.json",
    )
    ap.add_argument("asset_ids", nargs="+", help="Preview stem names (no extension)")
    args = ap.parse_args()
    rc = 0
    for asset_id in args.asset_ids:
        rc |= check_id(args.preview_dir, asset_id)
    return rc


if __name__ == "__main__":
    sys.exit(main())
