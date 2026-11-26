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
    img = Image.open(png).convert("RGBA")
    w, h = img.size
    alpha = [img.getpixel((x, y))[3] for y in range(h) for x in range(w)]
    ys = [y for y in range(h) for x in range(w) if img.getpixel((x, y))[3] > 48]
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
    if fill < 0.62:
        return False, f"fill={fill:.2f} want>=0.62 span={span} h={h}"
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