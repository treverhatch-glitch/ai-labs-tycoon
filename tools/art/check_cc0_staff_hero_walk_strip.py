#!/usr/bin/env python3
"""Walk strip: grey backdrop, four tiles, hero not grey T-pose."""
from __future__ import annotations

import argparse
import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
GREY = (150, 150, 150)


def _tile_stats(img: Image.Image, x0: int, x1: int, y0: int, y1: int) -> tuple[float, float, float, int]:
    rs: list[int] = []
    for y in range(y0, y1):
        for x in range(x0, x1):
            r, g, b = img.getpixel((x, y))[:3]
            if abs(r - GREY[0]) < 8 and abs(g - GREY[1]) < 8 and abs(b - GREY[2]) < 8:
                continue
            rs.append(r)
    if len(rs) < 80:
        return 0.0, 0.0, 0.0, 0
    return sum(rs) / len(rs), max(rs), min(rs), len(rs)


def check_strip(png: Path, asset_id: str) -> tuple[bool, str]:
    if not png.is_file() or png.stat().st_size < 2000:
        return False, "missing_strip"
    img = Image.open(png).convert("RGB")
    w, h = img.size
    if w < 4 * 64 or h < 64:
        return False, f"strip_too_small {w}x{h}"
    tile_w = w // 4
    corner_grey = all(
        abs(img.getpixel((c, h // 2))[0] - GREY[0]) < 12
        for c in (4, tile_w + 4, 2 * tile_w + 4, 3 * tile_w + 4)
    )
    if not corner_grey:
        return False, "backdrop_not_969696"
    y0, y1 = h // 6, (5 * h) // 6
    tile_means: list[float] = []
    for i in range(4):
        x0 = i * tile_w + tile_w // 8
        x1 = (i + 1) * tile_w - tile_w // 8
        mean_r, mx, _mn, n = _tile_stats(img, x0, x1, y0, y1)
        if n < 80:
            return False, f"tile{i}_empty"
        tile_means.append(mean_r)
        if mx < 55:
            return False, f"tile{i}_grey_base max_r={mx}"
    if asset_id == "staff-founder-idle":
        if max(tile_means) < 90:
            return False, f"founder_tile_washed means={tile_means}"
    else:
        if sum(tile_means) / len(tile_means) < 70:
            return False, f"researcher_tile_washed means={tile_means}"
    if len(tile_means) >= 4 and max(tile_means) - min(tile_means) < 2:
        return False, f"tiles_identical means={tile_means}"
    return True, f"strip {w}x{h} tiles=4 means={[round(m, 1) for m in tile_means]}"


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("asset_id", choices=("staff-founder-idle", "staff-researcher-idle"))
    args = parser.parse_args()
    stem = (
        "staff-founder-walk-strip"
        if args.asset_id == "staff-founder-idle"
        else "staff-researcher-walk-strip"
    )
    png = PREVIEW / f"{stem}.png"
    ok, msg = check_strip(png, args.asset_id)
    if ok:
        print(f"OK {args.asset_id} walk_strip {msg}")
        return 0
    print(f"FAIL {args.asset_id} walk_strip {msg}")
    return 1


if __name__ == "__main__":
    sys.exit(main())