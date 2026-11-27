#!/usr/bin/env python3
"""Idle preview: lab coat at hip/thigh + dark glasses frames in eye band."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-researcher-idle.png"
MASK = PNG.with_name(PNG.stem + ".mask.png")


def _figure_span(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, 0, 0
    return min(ys), max(ys), max(1, max(ys) - min(ys))


def main() -> int:
    if not PNG.is_file() or not MASK.is_file():
        print("FAIL staff-researcher-idle idle_visual missing_png")
        return 1
    img = Image.open(PNG).convert("RGB")
    mask = Image.open(MASK).convert("L")
    w, h = img.size
    min_y, max_y, span = _figure_span(mask)
    lo = min_y + int(span * 0.38)
    hi = min_y + int(span * 0.62)
    coat = thigh = 0
    for y in range(lo, hi):
        for x in range(w):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))
            if r >= 175 and g >= 173 and b >= 168 and max(r, g, b) - min(r, g, b) < 22:
                coat += 1
            if y >= min_y + int(span * 0.48) and r >= 170:
                thigh += 1
    if coat < 120 or thigh < 80:
        print(
            f"FAIL staff-researcher-idle idle_visual coat_px={coat} thigh_offwhite={thigh} "
            f"want coat>=120 thigh>=80"
        )
        return 1
    fy0 = min_y + int(span * 0.10)
    fy1 = min_y + int(span * 0.24)
    dark = 0
    for y in range(fy0, fy1):
        for x in range(w):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))
            if max(r, g, b) <= 95 and (r + g + b) < 240:
                dark += 1
    if dark < 35:
        print(f"FAIL staff-researcher-idle idle_visual eye_band_dark_px={dark} want>=35")
        return 1
    print(f"OK staff-researcher-idle idle_visual coat={coat} eye_dark={dark}")
    return 0


if __name__ == "__main__":
    sys.exit(main())