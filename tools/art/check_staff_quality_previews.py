#!/usr/bin/env python3
"""Reject staff + wisp previews with washed-out color (mean saturation < threshold)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
STAFF_MANIFEST = REPO / "unity/Assets/Art/EarlyGame/staff-manifest.json"
MIN_SAT = 0.15
MIN_SAT_WISP = 0.045
MIN_LUMA_VAR = 0.0054
MIN_LUMA_VAR_WISP = 0.0012
WISP_IDS = ["wisp-happy", "wisp-neutral", "wisp-worried", "wisp-angry", "wisp-glowing"]


def mean_saturation(path: Path) -> tuple[float, float]:
    try:
        from PIL import Image
    except ImportError:
        print("WARN: Pillow missing, skip saturation check")
        return 1.0, 1.0
    im = Image.open(path).convert("RGB")
    w, h = im.size
    margin_x, margin_y = int(w * 0.22), int(h * 0.22)
    im = im.crop((margin_x, margin_y, w - margin_x, h - margin_y))
    px = list(im.getdata())
    sats: list[float] = []
    lum: list[float] = []
    for r, g, b in px:
        rf, gf, bf = r / 255.0, g / 255.0, b / 255.0
        mx = max(rf, gf, bf)
        mn = min(rf, gf, bf)
        lum_v = 0.2126 * rf + 0.7152 * gf + 0.0722 * bf
        lum.append(lum_v)
        if mx <= 1e-6:
            continue
        if abs(rf - gf) < 0.04 and abs(gf - bf) < 0.04 and 0.34 < lum_v < 0.82:
            continue
        sats.append((mx - mn) / mx)
    if not sats:
        return 0.0, 0.0
    mean_sat = sum(sats) / len(sats)
    lbar = sum(lum) / len(lum)
    var = sum((x - lbar) ** 2 for x in lum) / len(lum)
    return mean_sat, var


def main() -> int:
    ids: list[str] = []
    if STAFF_MANIFEST.exists():
        ids.extend(a["id"] for a in json.loads(STAFF_MANIFEST.read_text())["assets"])
    ids.extend(WISP_IDS)
    rc = 0
    for asset_id in ids:
        path = PREVIEW / f"{asset_id}.png"
        if not path.exists():
            print("FAIL missing preview", asset_id)
            rc = 1
            continue
        sat, lvar = mean_saturation(path)
        min_sat = MIN_SAT_WISP if asset_id.startswith("wisp-") else MIN_SAT
        if sat < min_sat:
            print(f"FAIL {asset_id} mean_sat={sat:.3f} < {min_sat}")
            rc = 1
        elif lvar < (MIN_LUMA_VAR_WISP if asset_id.startswith("wisp-") else MIN_LUMA_VAR):
            min_lvar = MIN_LUMA_VAR_WISP if asset_id.startswith("wisp-") else MIN_LUMA_VAR
            print(f"FAIL {asset_id} flat frame luma_var={lvar:.4f} < {min_lvar}")
            rc = 1
        else:
            print(f"OK {asset_id} sat={sat:.3f} lvar={lvar:.4f}")
    return rc


if __name__ == "__main__":
    sys.exit(main())