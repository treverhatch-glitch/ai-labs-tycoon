#!/usr/bin/env python3
"""Founder preview: no bare-skin pixels in torso band (hoodie coverage gate)."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
AID = "staff-founder-idle"
PNG = REPO / "docs/art/previews" / f"{AID}.png"
MAX_SKIN_RATIO = 0.018


def _is_skin(r: int, g: int, b: int) -> bool:
    if max(r, g, b) - min(r, g, b) < 32:
        return False
    if r < 158 or g < 108 or b < 72:
        return False
    if r <= g + 10:
        return False
    if b > g * 0.9:
        return False
    return r - b >= 28 and g - b >= 8


def main() -> int:
    mask_path = PNG.with_name(PNG.stem + ".mask.png")
    if not PNG.is_file() or not mask_path.is_file():
        print(f"FAIL {AID} missing preview or mask")
        return 1
    img = Image.open(PNG).convert("RGBA")
    mask = Image.open(mask_path).convert("L")
    w, h = img.size
    ys: list[int] = []
    for y in range(h):
        for x in range(w):
            if mask.getpixel((x, y)) > 48:
                ys.append(y)
    if not ys:
        print(f"FAIL {AID} empty mask")
        return 1
    min_y, max_y = min(ys), max(ys)
    span = max(1, max_y - min_y)
    band_lo = min_y + int(span * 0.42)
    band_hi = min_y + int(span * 0.68)
    head_cut = max_y - int(span * 0.22)
    skin = 0
    band = 0
    for y in range(band_lo, min(band_hi, head_cut)):
        for x in range(w):
            if mask.getpixel((x, y)) <= 48:
                continue
            band += 1
            r, g, b, _ = img.getpixel((x, y))
            if 0.42 < r / 255 < 0.54 and 0.42 < g / 255 < 0.54:
                continue
            if _is_skin(r, g, b):
                skin += 1
    ratio = skin / max(1, band)
    if ratio > MAX_SKIN_RATIO:
        print(f"FAIL {AID} torso_skin ratio={ratio:.4f} skin_px={skin} band_px={band}")
        return 1
    print(f"OK {AID} torso_skin ratio={ratio:.4f} band_px={band}")
    return 0


if __name__ == "__main__":
    sys.exit(main())