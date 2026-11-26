#!/usr/bin/env python3
"""Hero staff previews: head centroid top 15% bbox, feet in bottom 15%."""
from __future__ import annotations

import json
import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
HERO_IDS = ("staff-founder-idle", "staff-researcher-idle")
MASK = REPO / "docs" / "art" / "previews"


def analyze(png: Path) -> tuple[bool, str]:
    mask_path = png.with_name(png.stem + ".mask.png")
    img = Image.open(png).convert("RGBA")
    w, h = img.size
    mask = Image.open(mask_path).convert("L") if mask_path.is_file() else None

    def on_figure(x: int, y: int) -> bool:
        if mask is not None:
            return mask.getpixel((x, y)) > 48
        r, g, b, a = img.getpixel((x, y))
        if a <= 48:
            return False
        if 0.42 < r / 255 < 0.54 and 0.42 < g / 255 < 0.54:
            return False
        return True

    ys = [y for y in range(h) for x in range(w) if on_figure(x, y)]
    if not ys:
        return False, "empty"
    min_y, max_y = min(ys), max(ys)
    span = max(1, max_y - min_y)
    top_cut = max_y - span * 0.15
    bot_cut = min_y + span * 0.15
    head_pts = [y for y in ys if y >= top_cut]
    foot_pts = [y for y in ys if y <= bot_cut]
    if not head_pts:
        return False, "no_pixels_in_top_15pct"
    if not foot_pts:
        return False, "no_pixels_in_bottom_15pct"
    head_cy = sum(head_pts) / len(head_pts)
    foot_cy = sum(foot_pts) / len(foot_pts)
    if head_cy < top_cut:
        return False, f"head_cy={head_cy:.1f} not in top band {top_cut:.1f}"
    if foot_cy > bot_cut:
        return False, f"foot_cy={foot_cy:.1f} not in bottom band {bot_cut:.1f}"
    if head_cy <= foot_cy:
        return False, "inverted_head_below_feet"
    fill = span / h
    if fill < 0.58 or fill > 0.78:
        return False, f"fill={fill:.2f} want 0.58-0.78 span={span} h={h}"
    return True, f"head={head_cy:.0f} foot={foot_cy:.0f} span={span} fill={fill:.2f}"


def main() -> int:
    rc = 0
    for aid in HERO_IDS:
        png = MASK / f"{aid}.png"
        if not png.is_file():
            print(f"FAIL {aid} missing preview")
            rc = 1
            continue
        ok, msg = analyze(png)
        if ok:
            print(f"OK {aid} {msg}")
        else:
            print(f"FAIL {aid} {msg}")
            rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())