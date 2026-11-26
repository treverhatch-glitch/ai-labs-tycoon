#!/usr/bin/env python3
"""Founder preview: face band in upper mask must not be dominated by hair/occlusion."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
AID = "staff-founder-idle"
PNG = REPO / "docs/art/previews" / f"{AID}.png"
MIN_FACE_BAND_PX = 120
MIN_BRIGHT_CENTER = 0.22


def main() -> int:
    mask_path = PNG.with_name(PNG.stem + ".mask.png")
    if not PNG.is_file() or not mask_path.is_file():
        print(f"FAIL {AID} missing preview or mask")
        return 1
    img = Image.open(PNG).convert("RGBA")
    mask = Image.open(mask_path).convert("L")
    w, h = img.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        print(f"FAIL {AID} empty mask")
        return 1
    min_y, max_y = min(ys), max(ys)
    span = max(1, max_y - min_y)
    top_lo = max_y - int(span * 0.22)
    x_lo, x_hi = int(w * 0.32), int(w * 0.68)
    band = 0
    bright = 0
    for y in range(top_lo, max_y):
        for x in range(x_lo, x_hi):
            if mask.getpixel((x, y)) <= 48:
                continue
            band += 1
            r, g, b, _ = img.getpixel((x, y))
            lum = (r + g + b) / (3.0 * 255.0)
            if lum >= MIN_BRIGHT_CENTER:
                bright += 1
    if band < MIN_FACE_BAND_PX:
        print(f"FAIL {AID} face_band_px={band}")
        return 1
    ratio = bright / max(1, band)
    if ratio < 0.35:
        print(f"FAIL {AID} face_occluded bright_ratio={ratio:.2f} band={band}")
        return 1
    print(f"OK {AID} face_visible bright_ratio={ratio:.2f} band={band}")
    return 0


if __name__ == "__main__":
    sys.exit(main())