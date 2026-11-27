#!/usr/bin/env python3
"""Researcher preview pixels: head skin+hair only, white torso column, charcoal legs."""
from __future__ import annotations

from PIL import Image

CHARCOAL_RGB = (58, 58, 58)
MAX_HEAD_COAT_WHITE_FRAC = 0.12
MAX_HEAD_FOREIGN_FRAC = 0.10
MAX_TORSO_NONWHITE_FRAC = 0.20
MAX_LEG_MEDIAN_CHARCOAL_DIST = 95.0


def _is_backdrop(r: int, g: int, b: int, grey: tuple[int, int, int] = (150, 150, 150)) -> bool:
    return abs(r - grey[0]) < 12 and abs(g - grey[1]) < 12 and abs(b - grey[2]) < 12


def _is_coat_white(r: int, g: int, b: int) -> bool:
    if _is_backdrop(r, g, b):
        return False
    spread = max(r, g, b) - min(r, g, b)
    lum = r + g + b
    spread = max(r, g, b) - min(r, g, b)
    if spread > 55:
        return False
    if r > g + 10:
        return False
    return lum >= 248 and r >= 98 and g >= 94 and abs(r - g) < 18 and spread < 44


def _is_skin_pixel(r: int, g: int, b: int) -> bool:
    if _is_backdrop(r, g, b) or _is_coat_white(r, g, b):
        return False
    spread = max(r, g, b) - min(r, g, b)
    return 88 <= r <= 200 and 62 <= g <= 172 and 48 <= b <= 158 and r >= g - 22 and spread < 72


def _is_hair_pixel(r: int, g: int, b: int) -> bool:
    if _is_backdrop(r, g, b) or _is_coat_white(r, g, b):
        return False
    spread = max(r, g, b) - min(r, g, b)
    lum = r + g + b
    if lum < 200 and r < 115 and b < 125:
        return True
    return r < 125 and g < 118 and b < 132 and spread < 65 and 55 < lum < 300


def head_coat_white_frac(panel: Image.Image, mask: Image.Image) -> float:
    w, h = panel.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 1.0
    min_y, max_y = min(ys), max(ys)
    span = max_y - min_y
    y0 = min_y + int(span * 0.06)
    y1 = min_y + int(span * 0.19)
    x0, x1 = w // 2 - w // 5, w // 2 + w // 5
    coat = n = 0
    for y in range(y0, y1, 2):
        for x in range(x0, x1, 2):
            if mask.getpixel((x, y)) <= 48:
                continue
            n += 1
            if _is_coat_white(*panel.getpixel((x, y))[:3]):
                coat += 1
    return coat / n if n > 40 else 1.0


def head_foreign_frac(panel: Image.Image, mask: Image.Image) -> tuple[float, int]:
    w, h = panel.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 1.0, 0
    min_y, max_y = min(ys), max(ys)
    span = max_y - min_y
    y0 = min_y + int(span * 0.06)
    y1 = min_y + int(span * 0.19)
    x0, x1 = w // 2 - w // 5, w // 2 + w // 5
    foreign = n = 0
    for y in range(y0, y1, 2):
        for x in range(x0, x1, 2):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = panel.getpixel((x, y))[:3]
            n += 1
            if _is_skin_pixel(r, g, b) or _is_hair_pixel(r, g, b):
                continue
            if max(r, g, b) < 52:
                continue
            foreign += 1
    if n < 40:
        return 1.0, n
    return foreign / n, n


def torso_column_nonwhite_frac(
    panel: Image.Image, mask: Image.Image, *, grey: tuple[int, int, int] = (150, 150, 150)
) -> float:
    w, h = panel.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 1.0
    min_y, max_y = min(ys), max(ys)
    span = max_y - min_y
    y0 = min_y + int(span * 0.24)
    y1 = min_y + int(span * 0.72)
    x0, x1 = w // 2 - w // 12, w // 2 + w // 12
    non_white = fig = 0
    for y in range(y0, y1, 2):
        in_neck = y < min_y + int(span * 0.32)
        for x in range(x0, x1, 2):
            if mask.getpixel((x, y)) <= 48:
                continue
            if _is_backdrop(*panel.getpixel((x, y))[:3], grey=grey):
                continue
            fig += 1
            r, g, b = panel.getpixel((x, y))[:3]
            if _is_coat_white(r, g, b):
                continue
            spread = max(r, g, b) - min(r, g, b)
            lum = r + g + b
            if spread < 28 and lum >= 205 and r >= 85:
                continue
            if in_neck:
                continue
            non_white += 1
    if fig < 30:
        return 1.0
    return non_white / fig


def leg_roi_median_rgb(panel: Image.Image, mask: Image.Image) -> tuple[float, float, float]:
    w, h = panel.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0.0, 0.0, 0.0
    min_y, max_y = min(ys), max(ys)
    span = max_y - min_y
    y0 = min_y + int(span * 0.58)
    y1 = min_y + int(span * 0.92)
    x0, x1 = w // 4, (3 * w) // 4
    rs: list[int] = []
    for y in range(y0, y1, 2):
        for x in range(x0, x1, 2):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = panel.getpixel((x, y))[:3]
            if _is_backdrop(r, g, b) or _is_coat_white(r, g, b):
                continue
            rs.append((r + g + b) // 3)
    if len(rs) < 20:
        return 0.0, 0.0, 0.0
    rs.sort()
    med = rs[len(rs) // 2]
    dist = ((med - CHARCOAL_RGB[0]) ** 2 * 3) ** 0.5
    return float(med), float(med), dist


def check_researcher_panel_pixels(panel: Image.Image, mask: Image.Image) -> tuple[bool, str]:
    head_coat = head_coat_white_frac(panel, mask)
    if head_coat > MAX_HEAD_COAT_WHITE_FRAC:
        return False, f"head_coat_white={head_coat:.3f} want<={MAX_HEAD_COAT_WHITE_FRAC}"
    foreign, hn = head_foreign_frac(panel, mask)
    if hn >= 40 and foreign > MAX_HEAD_FOREIGN_FRAC:
        return False, f"head_foreign={foreign:.3f} want<={MAX_HEAD_FOREIGN_FRAC}"
    torso_bad = torso_column_nonwhite_frac(panel, mask)
    if torso_bad > MAX_TORSO_NONWHITE_FRAC:
        return False, f"torso_nonwhite={torso_bad:.3f} want<={MAX_TORSO_NONWHITE_FRAC}"
    _mr, _mg, dist = leg_roi_median_rgb(panel, mask)
    if _mr > 0 and dist > MAX_LEG_MEDIAN_CHARCOAL_DIST:
        return False, f"leg_median_dist={dist:.1f} want<={MAX_LEG_MEDIAN_CHARCOAL_DIST}"
    return True, f"ok head_foreign={foreign:.3f} torso_nw={torso_bad:.3f} leg_dist={dist:.1f}"