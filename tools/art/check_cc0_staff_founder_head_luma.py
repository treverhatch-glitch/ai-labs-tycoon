#!/usr/bin/env python3
"""Founder PNG head band: not blown-out white; some color saturation."""
from __future__ import annotations

import colorsys
import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"


def main() -> int:
    mask_path = PNG.with_name(PNG.stem + ".mask.png")
    if not PNG.is_file() or not mask_path.is_file():
        print("FAIL staff-founder-idle missing preview")
        return 1
    img = Image.open(PNG).convert("RGBA")
    mask = Image.open(mask_path).convert("L")
    w, h = img.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        print("FAIL staff-founder-idle empty_mask")
        return 1
    min_y, max_y = min(ys), max(ys)
    span = max(1, max_y - min_y)
    top_hi = min_y + int(span * 0.20)
    px = []
    for y in range(min_y, top_hi):
        for x in range(w):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b, *_ = img.getpixel((x, y))
            px.append((r / 255.0, g / 255.0, b / 255.0))
    if len(px) < 40:
        print("FAIL staff-founder-idle head_band_empty")
        return 1
    def _luma(c: tuple[float, float, float]) -> float:
        return 0.2126 * c[0] + 0.7152 * c[1] + 0.0722 * c[2]

    colored = [c for c in px if colorsys.rgb_to_hsv(*c)[1] > 0.10]
    if len(colored) < 32:
        print(f"FAIL staff-founder-idle head_low_sat n={len(colored)}")
        return 1
    lums = sorted(_luma(c) for c in colored)
    lum = lums[len(lums) // 2]
    sat = sum(colorsys.rgb_to_hsv(*c)[1] for c in colored) / len(colored)
    if lum >= 0.85:
        print(f"FAIL staff-founder-idle head_washed lum={lum:.2f}")
        return 1
    if sat <= 0.10:
        print(f"FAIL staff-founder-idle head_grey sat={sat:.2f}")
        return 1
    print(f"OK staff-founder-idle head_lum={lum:.2f} sat={sat:.2f}")
    return 0


if __name__ == "__main__":
    sys.exit(main())