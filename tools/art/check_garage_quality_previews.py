#!/usr/bin/env python3
"""Garage preview gate — full-frame saturation (garage lane)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"
MIN_SAT = 0.15
MIN_LUMA_VAR = 0.004

sys.path.insert(0, str(REPO / "tools" / "art"))
from preview_quality_lib import mean_saturation_full  # noqa: E402


def main() -> int:
    try:
        from PIL import Image  # noqa: F401
    except ImportError:
        print("FAIL: Pillow required for saturation gate")
        return 1

    ids = [a["id"] for a in json.loads(MANIFEST.read_text())["assets"]]
    rc = 0
    for asset_id in ids:
        path = PREVIEW / f"{asset_id}.png"
        if not path.exists():
            print("FAIL missing preview", asset_id)
            rc = 1
            continue
        sat, lvar = mean_saturation_full(path)
        if sat < MIN_SAT:
            print(f"FAIL {asset_id} mean_sat={sat:.3f} < {MIN_SAT}")
            rc = 1
        elif lvar < MIN_LUMA_VAR:
            print(f"FAIL {asset_id} flat frame luma_var={lvar:.4f}")
            rc = 1
        else:
            print(f"OK {asset_id} sat={sat:.3f} lvar={lvar:.4f}")
    return rc


if __name__ == "__main__":
    sys.exit(main())