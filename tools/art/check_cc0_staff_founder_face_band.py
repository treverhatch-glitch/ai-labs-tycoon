#!/usr/bin/env python3
"""Founder eye-height band: skin (R>G>B, sat 0.15–0.6), reject cyan 160–200°."""
from __future__ import annotations

import colorsys
import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"


def _figure_span(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, 0, 0
    return min(ys), max(ys), max(1, max(ys) - min(ys))


def analyze_face_cyan_mask(png: Path) -> tuple[bool, str]:
    """Lower-face band: rejects cyan mask wash (fb35ba42-class failures)."""
    mask_path = png.with_name(png.stem + ".mask.png")
    if not png.is_file() or not mask_path.is_file():
        return False, "missing preview"
    img = Image.open(png).convert("RGBA")
    mask = Image.open(mask_path).convert("L")
    w, h = img.size
    min_y, max_y, span = _figure_span(mask)
    if span < 10:
        return False, "empty_mask"
    band_lo = min_y + int(span * 0.16)
    band_hi = min_y + int(span * 0.26)
    px = 0
    cyan_n = 0
    for y in range(band_lo, band_hi):
        for x in range(w):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b, *_ = img.getpixel((x, y))
            px += 1
            h_deg, sat, _ = colorsys.rgb_to_hsv(r / 255, g / 255, b / 255)
            if 160.0 <= h_deg * 360.0 <= 200.0 and sat > 0.20:
                cyan_n += 1
    if px < 40:
        return True, f"face_cyan_clear n={px}"
    if cyan_n > px * 0.10:
        return False, f"face_cyan_mask n={cyan_n}/{px}"
    return True, f"face_cyan_clear n={cyan_n}/{px}"


def analyze_face_band(png: Path) -> tuple[bool, str]:
    mask_path = png.with_name(png.stem + ".mask.png")
    if not png.is_file() or not mask_path.is_file():
        return False, "missing preview"
    img = Image.open(png).convert("RGBA")
    mask = Image.open(mask_path).convert("L")
    img = Image.open(PNG).convert("RGBA")
    mask = Image.open(mask_path).convert("L")
    w, h = img.size
    min_y, max_y, span = _figure_span(mask)
    if span < 10:
        print("FAIL staff-founder-idle empty_mask")
        return 1
    band_lo = min_y + int(span * 0.08)
    band_hi = min_y + int(span * 0.18)
    px: list[tuple[int, int, int]] = []
    cyan_n = 0
    skin_n = 0
    for y in range(band_lo, band_hi):
        for x in range(w):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b, *_ = img.getpixel((x, y))
            px.append((r, g, b))
            lum = max(r, g, b) / 255.0
            if lum > 0.94:
                continue
            h, sat, _ = colorsys.rgb_to_hsv(r / 255, g / 255, b / 255)
            hue = h * 360.0
            if 160.0 <= hue <= 200.0 and sat > 0.18:
                cyan_n += 1
            if r > g > b and 0.12 <= sat <= 0.60:
                skin_n += 1
    if len(px) < 80:
        return False, f"face_band_empty n={len(px)}"
    if cyan_n > len(px) * 0.12:
        return False, f"face_cyan n={cyan_n}/{len(px)}"
    if skin_n < len(px) * 0.14:
        return False, f"face_not_skin skin={skin_n}/{len(px)}"
    return True, f"skin={skin_n} cyan={cyan_n} n={len(px)}"


def main() -> int:
    ok, msg = analyze_face_band(PNG)
    if ok:
        print(f"OK staff-founder-idle face_band {msg}")
        return 0
    print(f"FAIL staff-founder-idle {msg}")
    return 1


if __name__ == "__main__":
    sys.exit(main())