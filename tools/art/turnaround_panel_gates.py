#!/usr/bin/env python3
"""Shared turnaround panel metrics (eyes, front/back differ)."""
from __future__ import annotations

import colorsys

from PIL import Image

GREY = (150, 150, 150)
MIN_PANEL_TEXTURED_FRAC_RESEARCHER = 0.38
MIN_PANEL_TEXTURED_FRAC_FOUNDER = 0.88
MAX_PANEL_GREY_MANNEQUIN_FRAC = 0.26
MIN_PANEL_LUMINANCE = 99.0


def _figure_mask_from_rgb(panel: Image.Image) -> Image.Image:
    mask = Image.new("L", panel.size, 0)
    pw, ph = panel.size
    for y in range(ph):
        for x in range(pw):
            r, g, b = panel.getpixel((x, y))
            if abs(r - GREY[0]) < 10 and abs(g - GREY[1]) < 10 and abs(b - GREY[2]) < 10:
                continue
            mask.putpixel((x, y), 255)
    return mask


def load_panel_mask(panel: Image.Image, mask_path) -> Image.Image:
    if mask_path is not None and mask_path.is_file():
        return Image.open(mask_path).convert("L")
    return _figure_mask_from_rgb(panel)


def founder_eye_blobs(mask: Image.Image, img: Image.Image, *, panel: int = 0) -> int:
    import sys
    from pathlib import Path

    art = Path(__file__).resolve().parent
    if str(art) not in sys.path:
        sys.path.insert(0, str(art))
    from check_cc0_staff_founder_eye_blobs import _blobs, _figure_span

    mask = mask.convert("L")
    img = img.convert("RGB")
    w, _ = img.size
    _miny, _maxy, span = _figure_span(mask)
    y0 = _miny + int(span * 0.45)
    y1 = _miny + int(span * 0.72)
    blobs = _blobs(mask, img, y0, y1)
    left = [b for b in blobs if (b[0] + b[2]) / 2 < w * 0.48]
    right = [b for b in blobs if (b[0] + b[2]) / 2 > w * 0.52]
    if panel != 0:
        return min(1, len(blobs))
    if left and right:
        return 2
    mid = w // 2
    half_w = max(12, w // 2 - 8)
    l_px = r_px = 0
    for y in range(y0, y1):
        for x in range(max(0, mid - half_w), mid):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))[:3]
            if max(r, g, b) <= 96 and (max(r, g, b) <= 52 or not (b > r + 20)):
                l_px += 1
        for x in range(mid, min(w, mid + half_w)):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))[:3]
            if max(r, g, b) <= 96 and (max(r, g, b) <= 52 or not (b > r + 20)):
                r_px += 1
    if l_px >= 6 and r_px >= 6:
        return 2
    if l_px >= 2 and r_px >= 8:
        return 2
    return len(blobs)


def researcher_eye_blobs(mask: Image.Image, img: Image.Image, *, panel: int) -> int:
    import sys
    from pathlib import Path

    art = Path(__file__).resolve().parent
    if str(art) not in sys.path:
        sys.path.insert(0, str(art))
    from staff_preview_eye_head_gate import check_eyes_in_head_mask

    yaw = {0: 0.0, 1: 90.0, 2: 180.0, 3: 270.0}[panel]
    allow = panel in (0, 1, 3)
    if panel == 2:
        allow = False
    ok, _, blobs, _ = check_eyes_in_head_mask(
        img.convert("RGB"),
        mask.convert("L"),
        asset_id="staff-researcher-idle",
        allow_projection_fallback=allow,
        yaw_deg=yaw,
    )
    n = len(blobs)
    if panel == 0 and ok and n >= 2:
        return 2
    if panel == 2:
        return n if n >= 2 else 0
    return min(n, 1)


def _is_backdrop_grey(r: int, g: int, b: int) -> bool:
    return abs(r - GREY[0]) < 12 and abs(g - GREY[1]) < 12 and abs(b - GREY[2]) < 12


def _is_grey_mannequin_pixel(r: int, g: int, b: int) -> bool:
    if _is_backdrop_grey(r, g, b):
        return False
    spread = max(r, g, b) - min(r, g, b)
    lum = r + g + b
    return spread < 24 and 168 < lum < 292


def _is_textured_pixel(r: int, g: int, b: int, *, asset_id: str) -> bool:
    if _is_backdrop_grey(r, g, b) or _is_grey_mannequin_pixel(r, g, b):
        return False
    h, s, _v = colorsys.rgb_to_hsv(r / 255.0, g / 255.0, b / 255.0)
    if s > 0.10:
        return True
    if asset_id == "staff-researcher-idle":
        if r >= 145 and g >= 140 and b >= 132 and r + g + b > 430:
            return True
    if asset_id == "staff-founder-idle":
        if r >= 95 and g >= 72 and b < 180 and r > g - 4:
            return True
        if b > 85 and r < 115:
            return True
    return (r + g + b) / 3.0 >= MIN_PANEL_LUMINANCE and max(r, g, b) - min(r, g, b) > 28


def panel_material_metrics(
    panel: Image.Image, mask: Image.Image, *, asset_id: str
) -> tuple[float, float, float]:
    """textured_frac, grey_mannequin_frac, mean_luma on figure pixels."""
    textured = grey = fig = 0
    luma = 0.0
    w, h = panel.size
    for y in range(0, h, 2):
        for x in range(0, w, 2):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = panel.getpixel((x, y))[:3]
            fig += 1
            luma += (r + g + b) / 3.0
            if _is_grey_mannequin_pixel(r, g, b):
                grey += 1
            elif _is_textured_pixel(r, g, b, asset_id=asset_id):
                textured += 1
    if fig < 80:
        return 0.0, 1.0, 0.0
    return textured / fig, grey / fig, luma / fig


def _head_roi(mask: Image.Image) -> tuple[int, int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, w, 0, h
    min_y, max_y = min(ys), max(ys)
    span = max(8, max_y - min_y)
    y1 = min_y + int(span * 0.38)
    return 0, w, min_y, y1


def head_mean_rgb(img: Image.Image, mask: Image.Image) -> tuple[float, float, float]:
    x0, x1, y0, y1 = _head_roi(mask)
    rs = gs = bs = 0.0
    n = 0
    for y in range(y0, y1, 2):
        for x in range(x0, x1, 2):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))[:3]
            rs += r
            gs += g
            bs += b
            n += 1
    if n < 30:
        return figure_mean_rgb(img, mask)
    return rs / n, gs / n, bs / n


def figure_mean_rgb(img: Image.Image, mask: Image.Image) -> tuple[float, float, float]:
    rs = gs = bs = 0.0
    n = 0
    w, h = img.size
    for y in range(0, h, 2):
        for x in range(0, w, 2):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))[:3]
            rs += r
            gs += g
            bs += b
            n += 1
    if n < 40:
        return 0.0, 0.0, 0.0
    return rs / n, gs / n, bs / n


def rgb_l1(a: tuple[float, float, float], b: tuple[float, float, float]) -> float:
    return abs(a[0] - b[0]) + abs(a[1] - b[1]) + abs(a[2] - b[2])


def mirror_iou(mask: Image.Image) -> float:
    g = mask.convert("L")
    bbox = g.getbbox()
    if not bbox:
        return 0.0
    crop = g.crop(bbox)
    w, h = crop.size
    if w * h < 64:
        return 0.0
    flipped = crop.transpose(Image.FLIP_LEFT_RIGHT)
    inter = union = 0
    for y in range(h):
        for x in range(w):
            a = crop.getpixel((x, y)) > 48
            b = flipped.getpixel((x, y)) > 48
            if a or b:
                union += 1
            if a and b:
                inter += 1
    return inter / union if union else 0.0