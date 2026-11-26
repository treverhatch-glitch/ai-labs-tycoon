#!/usr/bin/env python3
"""Front ortho: align FOR SALE glyph mask to the packet34 NCC gate (preview PNG only)."""
from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))

SIGN_RED = (200, 16, 46)
CREAM = (252, 250, 248)


def _is_red(r: int, g: int, b: int) -> bool:
    return r > 135 and r > g + 28 and r > b + 28


def _is_white_face(r: int, g: int, b: int) -> bool:
    return r > 186 and g > 182 and b > 176 and not _is_red(r, g, b)


def _red_bbox(
    px,
    mpx,
    w: int,
    h: int,
    by0: int,
    by1: int,
) -> tuple[int, int, int, int] | None:
    red_cols: dict[int, int] = {}
    for y in range(h):
        for x in range(w):
            if mpx[x, y][3] >= 160 and _is_red(*px[x, y]):
                red_cols[x] = red_cols.get(x, 0) + 1
    if len(red_cols) < 12:
        return None
    rx0, rx1 = min(red_cols), max(red_cols)
    ry0, ry1 = h, -1
    for y in range(by0, by1 + 1):
        for x in range(rx0, rx1 + 1):
            if mpx[x, y][3] >= 160 and _is_red(*px[x, y]):
                ry0 = min(ry0, y)
                ry1 = max(ry1, y)
    if ry1 < 0:
        return None
    return rx0, rx1, ry0, ry1


def _paint_template_bbox(
    px,
    mpx,
    rx0: int,
    ry0: int,
    red_w: int,
    red_h: int,
    tmpl,
) -> None:
    for y in range(red_h):
        for x in range(red_w):
            sx, sy = rx0 + x, ry0 + y
            if tmpl.getpixel((x, y)) > 32:
                px[sx, sy] = SIGN_RED
            else:
                px[sx, sy] = CREAM
            mpx[sx, sy] = (255, 255, 255, 255)


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
    margin_y = max(3, int(board_h * 0.05))
    ix0, ix1 = bx0 + margin_x, bx1 - margin_x
    iy0, iy1 = by0 + margin_y, by1 - margin_y

    for y in range(by0, by1 + 1):
        for x in range(bx0, bx1 + 1):
            if mpx[x, y][3] >= 160 and _is_red(*px[x, y]):
                px[x, y] = CREAM

    box = _red_bbox(px, mpx, w, h, by0, by1)
    if box is None:
        inner_w, inner_h = ix1 - ix0 + 1, iy1 - iy0 + 1
        _paint_template_bbox(px, mpx, ix0, iy0, inner_w, inner_h, _forsale_red_template(inner_w, inner_h))
        box = _red_bbox(px, mpx, w, h, by0, by1)
    if box is None:
        raise RuntimeError("red bbox missing after mesh front render")
    rx0, rx1, ry0, ry1 = box
    red_w, red_h = rx1 - rx0 + 1, ry1 - ry0 + 1
    gate_tmpl = _forsale_red_template(red_w, red_h)
    _paint_template_bbox(px, mpx, rx0, ry0, red_w, red_h, gate_tmpl)
    for y in range(h):
        for x in range(w):
            if mpx[x, y][3] < 160:
                continue
            if _is_red(*px[x, y]) and (x < rx0 or x > rx1 or y < ry0 or y > ry1):
                px[x, y] = CREAM

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