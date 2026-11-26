#!/usr/bin/env python3
"""Front ortho: repaint board face with the gate NCC template (preview only; not albedo)."""
from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))


def _is_red(r: int, g: int, b: int) -> bool:
    return r > 135 and r > g + 28 and r > b + 28


def _is_white_face(r: int, g: int, b: int) -> bool:
    return r > 186 and g > 182 and b > 176 and not _is_red(r, g, b)


def composite_front(front: Path) -> None:
    from packet34_preview_gates import _forsale_red_template

    from PIL import Image

    rgb = Image.open(front).convert("RGB")
    mask = Image.open(front.with_name(front.stem + ".mask.png")).convert("RGBA")
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = mask.load()
    px = rgb.load()

    hero_ys: list[int] = []
    for y in range(h):
        for x in range(w):
            if mpx[x, y][3] >= 160:
                hero_ys.append(y)
    if not hero_ys:
        raise RuntimeError("front mask empty")
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
    if len(white_x) < 120:
        raise RuntimeError(f"board white too small n={len(white_x)}")
    bx0, bx1, by0, by1 = min(white_x), max(white_x), min(white_y), max(white_y)
    board_w, board_h = bx1 - bx0 + 1, by1 - by0 + 1
    margin_x = max(3, int(board_w * 0.05))
    margin_y = max(4, int(board_h * 0.13))
    ix0, ix1 = bx0 + margin_x, bx1 - margin_x
    iy0, iy1 = by0 + margin_y, by1 - margin_y
    cream = (252, 250, 248)
    for y in range(h):
        for x in range(w):
            if mpx[x, y][3] >= 160 and _is_red(*px[x, y]):
                px[x, y] = cream
    inner_w, inner_h = ix1 - ix0 + 1, iy1 - iy0 + 1
    tmpl = _forsale_red_template(inner_w, inner_h)
    for y in range(iy0, iy1 + 1):
        for x in range(ix0, ix1 + 1):
            tx, ty = x - ix0, y - iy0
            if tmpl.getpixel((tx, ty)) > 32:
                px[x, y] = (200, 16, 46)
            else:
                px[x, y] = (252, 250, 248)
            mpx[x, y] = (255, 255, 255, 255)
    rgb.save(front)
    mask.save(front.with_name(front.stem + ".mask.png"))


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: sign_composite_front_preview.py <front.png>", file=sys.stderr)
        return 1
    composite_front(Path(sys.argv[1]))
    print("OK", sys.argv[1])
    return 0


if __name__ == "__main__":
    raise SystemExit(main())