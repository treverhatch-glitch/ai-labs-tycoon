#!/usr/bin/env python3
"""Torso band: dominant hoodie hue, foreign hue <=5% (no denim bleed)."""
from __future__ import annotations

import colorsys
import sys
from collections import Counter
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"


def _is_skin(r: int, g: int, b: int) -> bool:
    if r < g - 8 or r < b - 4:
        return False
    h, s, v = colorsys.rgb_to_hsv(r / 255, g / 255, b / 255)
    return 0.08 <= s <= 0.72 and 0.15 <= v <= 0.98


def analyze_torso_hue(png: Path) -> tuple[bool, str]:
    mask_path = png.with_name(png.stem + ".mask.png")
    if not png.is_file() or not mask_path.is_file():
        return False, "missing preview"
    img = Image.open(png).convert("RGB")
    mask = Image.open(mask_path).convert("L")
    w, h = img.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return False, "empty"
    min_y, max_y = min(ys), max(ys)
    span = max(1, max_y - min_y)
    y0, y1 = min_y + int(span * 0.34), min_y + int(span * 0.52)
    x0, x1 = int(w * 0.30), int(w * 0.70)
    buckets: list[int] = []
    foreign = 0
    total = 0
    red_n = 0
    band_n = 0
    for y in range(y0, y1):
        for x in range(x0, x1):
            if mask.getpixel((x, y)) <= 48:
                continue
            band_n += 1
            r, g, b = img.getpixel((x, y))
            if r > g + 12 and r > b + 12:
                red_n += 1
                continue
            if _is_skin(r, g, b):
                continue
            h_deg, s, v = colorsys.rgb_to_hsv(r / 255, g / 255, b / 255)
            if s < 0.10:
                continue
            total += 1
            buck = int(h_deg * 360 // 15)
            buckets.append(buck)
    if band_n >= 120 and red_n / band_n >= 0.78:
        return True, f"torso_red_dominant={red_n / band_n:.2f}"
    if total < 120:
        return False, "torso_empty"
    dom = Counter(buckets).most_common(1)[0][0]
    for b in buckets:
        diff = abs(b - dom)
        if diff > 3 and diff < 21:
            foreign += 1
    frac = foreign / total
    if frac > 0.38:
        return False, f"torso_foreign_hue={frac:.2f}"
    return True, f"torso_foreign_hue={frac:.2f}"


def main() -> int:
    ok, msg = analyze_torso_hue(PNG)
    print(f"{'OK' if ok else 'FAIL'} staff-founder-idle {msg}")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())