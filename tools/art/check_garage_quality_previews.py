#!/usr/bin/env python3
"""Reject garage previews with washed-out color (Hatch: full-frame mean saturation)."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"
MIN_SAT = 0.15
MIN_LUMA_VAR = 0.004


def mean_saturation_full(path: Path) -> tuple[float, float]:
    from PIL import Image

    im = Image.open(path).convert("RGB")
    pixels = list(im.getdata())
    sats: list[float] = []
    lum: list[float] = []
    for r, g, b in pixels:
        rf, gf, bf = r / 255.0, g / 255.0, b / 255.0
        mx = max(rf, gf, bf)
        mn = min(rf, gf, bf)
        lum.append(0.2126 * rf + 0.7152 * gf + 0.0722 * bf)
        if mx <= 1e-6:
            continue
        sats.append((mx - mn) / mx)
    if not sats:
        return 0.0, 0.0
    mean_sat = sum(sats) / len(sats)
    lbar = sum(lum) / len(lum)
    var = sum((x - lbar) ** 2 for x in lum) / len(lum)
    return mean_sat, var


def main() -> int:
    import json

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