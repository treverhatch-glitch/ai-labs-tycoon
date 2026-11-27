#!/usr/bin/env python3
"""Founder PNG: red sleeves + skin forearms visible in wing bands (3/4 front)."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"


def _is_red(r: int, g: int, b: int) -> bool:
    return r > 120 and g < 95 and b < 95 and r > g + 25


def _is_skin(r: int, g: int, b: int) -> bool:
    return r > 148 and g > 110 and b < 178 and (r - b) > 18 and r >= g * 0.85


def analyze(png: Path) -> tuple[bool, str]:
    mask_path = png.with_name(png.stem + ".mask.png")
    if not png.is_file() or not mask_path.is_file():
        return False, "missing preview"
    img = Image.open(png).convert("RGB")
    mask = Image.open(mask_path).convert("L")
    w, h = mask.size
    xs_fig = [x for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    ys_fig = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not xs_fig:
        return False, "empty_mask"
    min_x, max_x = min(xs_fig), max(xs_fig)
    min_y, max_y = min(ys_fig), max(ys_fig)
    span = max(1, max_y - min_y)
    y0, y1 = min_y + int(span * 0.30), min_y + int(span * 0.72)
    fig_w = max(1, max_x - min_x)
    wing_w = max(12, int(fig_w * 0.20))
    left_hi = min_x + wing_w
    right_lo = max_x - wing_w
    left_red = left_skin = right_red = right_skin = 0
    for y in range(y0, y1):
        for x in range(min_x, left_hi):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))
            if _is_red(r, g, b):
                left_red += 1
            elif _is_skin(r, g, b):
                left_skin += 1
        for x in range(right_lo, max_x + 1):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))
            if _is_red(r, g, b):
                right_red += 1
            elif _is_skin(r, g, b):
                right_skin += 1
    min_red, min_skin = 70, 55
    if max(left_red, right_red) < min_red:
        return False, f"arm_sleeve_px L={left_red} R={right_red} need>={min_red}"
    if min(left_red, right_red) < 40 and max(left_red, right_red) < 120:
        return False, f"arm_sleeve_one_side L={left_red} R={right_red}"
    if max(left_skin, right_skin) < min_skin:
        return False, f"arm_skin_px L={left_skin} R={right_skin} need>={min_skin}"
    if min(left_skin, right_skin) < 35 and max(left_skin, right_skin) < 100:
        return False, f"arm_skin_one_side L={left_skin} R={right_skin}"
    return True, f"sleeves L={left_red} R={right_red} skin L={left_skin} R={right_skin}"


def main() -> int:
    ok, msg = analyze(PNG)
    print(f"{'OK' if ok else 'FAIL'} staff-founder-idle {msg}")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())