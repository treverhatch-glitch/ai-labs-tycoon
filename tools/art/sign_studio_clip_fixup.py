#!/usr/bin/env python3
"""Clamp studio sign red glyphs to the white board face box (preview only)."""
from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))


def _is_red(r: int, g: int, b: int) -> bool:
    return r > 135 and r > g + 28 and r > b + 28


def _is_white_face(r: int, g: int, b: int) -> bool:
    return r > 186 and g > 182 and b > 176 and not _is_red(r, g, b)


def fix_studio(path: Path) -> None:
    from PIL import Image

    rgb = Image.open(path).convert("RGB")
    mask = Image.open(path.with_name(path.stem + ".mask.png")).convert("RGBA")
    w, h = rgb.size
    mpx = mask.load()
    px = rgb.load()
    hero_ys: list[int] = []
    for y in range(h):
        for x in range(w):
            if mpx[x, y][3] >= 160:
                hero_ys.append(y)
    hy0, hy1 = min(hero_ys), max(hero_ys)
    face_y0 = hy0 + int((hy1 - hy0) * 0.18)
    white_x, white_y = [], []
    for y in range(h):
        for x in range(w):
            if mpx[x, y][3] < 160:
                continue
            if y >= face_y0 and _is_white_face(*px[x, y]):
                white_x.append(x)
                white_y.append(y)
    if len(white_x) < 100:
        return
    bx0, bx1, by0, by1 = min(white_x), max(white_x), min(white_y), max(white_y)
    margin_x = max(3, int((bx1 - bx0 + 1) * 0.05))
    margin_y = max(3, int((by1 - by0 + 1) * 0.05))
    x0, x1 = bx0 + margin_x, bx1 - margin_x
    y0, y1 = by0 + margin_y, by1 - margin_y
    cream = (252, 250, 248)
    for y in range(h):
        for x in range(w):
            if mpx[x, y][3] < 160:
                continue
            if not _is_red(*px[x, y]):
                continue
            if x < x0 or x > x1 or y < y0 or y > y1:
                px[x, y] = cream
    rgb.save(path)


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: sign_studio_clip_fixup.py <studio.png>", file=sys.stderr)
        return 1
    fix_studio(Path(sys.argv[1]))
    print("OK", sys.argv[1])
    return 0


if __name__ == "__main__":
    raise SystemExit(main())