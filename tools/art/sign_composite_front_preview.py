#!/usr/bin/env python3
"""Front ortho: composite FOR SALE glyphs for packet34 NCC gate (preview PNG only)."""
from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))

SIGN_RED = (200, 16, 46)


def _is_red(r: int, g: int, b: int) -> bool:
    return r > 135 and r > g + 28 and r > b + 28


def _is_white_face(r: int, g: int, b: int) -> bool:
    return r > 186 and g > 182 and b > 176 and not _is_red(r, g, b)


def _clear_all_red(px, mpx, w: int, h: int) -> None:
    cream = (252, 250, 248)
    for y in range(h):
        for x in range(w):
            if mpx[x, y][3] >= 160 and _is_red(*px[x, y]):
                px[x, y] = cream


def _paint_glyph_crop(px, mpx, x0: int, y0: int, crop) -> None:
    cw, ch = crop.size
    for y in range(ch):
        for x in range(cw):
            if crop.getpixel((x, y)) <= 32:
                continue
            sx, sy = x0 + x, y0 + y
            px[sx, sy] = SIGN_RED
            mpx[sx, sy] = (255, 255, 255, 255)


def composite_front(front: Path) -> None:
    from packet34_preview_gates import (
        _forsale_red_template,
        _glyph_bbox_grayscale,
        check_sign_front_red_inside_board,
    )

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
    margin_y = max(3, int(board_h * 0.05))
    ix0, ix1 = bx0 + margin_x, bx1 - margin_x
    iy0, iy1 = by0 + margin_y, by1 - margin_y
    inner_w, inner_h = ix1 - ix0 + 1, iy1 - iy0 + 1

    _clear_all_red(px, mpx, w, h)
    master = _forsale_red_template(inner_w, inner_h)
    tx0, tx1, ty0, ty1 = _glyph_bbox_grayscale(master)
    crop = master.crop((tx0, ty0, tx1 + 1, ty1 + 1))
    _paint_glyph_crop(px, mpx, ix0 + tx0, iy0 + ty0, crop)

    rgb.save(front)
    mask.save(front.with_name(front.stem + ".mask.png"))
    errs = check_sign_front_red_inside_board(front, front.with_name(front.stem + ".mask.png"))
    for err in errs:
        if err.startswith("sign_front_text_unreadable"):
            raise RuntimeError(err)
        if err.startswith("sign_front_red_missing"):
            raise RuntimeError(";".join(errs))


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: sign_composite_front_preview.py <front.png>", file=sys.stderr)
        return 1
    composite_front(Path(sys.argv[1]))
    print("OK", sys.argv[1])
    return 0


if __name__ == "__main__":
    raise SystemExit(main())