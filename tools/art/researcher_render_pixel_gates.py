#!/usr/bin/env python3
"""Researcher preview pixels: coat torso box, back-of-head hair, charcoal legs."""
from __future__ import annotations

from PIL import Image

CHARCOAL_RGB = (58, 58, 58)
MIN_TORSO_COAT_WHITE_FRAC = 0.90
MAX_BACK_HEAD_SKIN_FRAC = 0.0
MAX_LEG_MEDIAN_CHARCOAL_DIST = 95.0


def _is_backdrop(r: int, g: int, b: int, grey: tuple[int, int, int] = (150, 150, 150)) -> bool:
    return abs(r - grey[0]) < 12 and abs(g - grey[1]) < 12 and abs(b - grey[2]) < 12


def _is_coat_white(r: int, g: int, b: int) -> bool:
    if _is_backdrop(r, g, b):
        return False
    spread = max(r, g, b) - min(r, g, b)
    lum = r + g + b
    if spread > 55:
        return False
    if r > g + 12:
        return False
    if spread < 28 and lum >= 205 and r >= 85:
        return True
    return lum >= 248 and r >= 98 and g >= 94 and abs(r - g) < 18 and spread < 44


def _is_skin_tan(r: int, g: int, b: int) -> bool:
    if _is_backdrop(r, g, b) or _is_coat_white(r, g, b):
        return False
    spread = max(r, g, b) - min(r, g, b)
    # Lit brown hair reads tan at preview exposure; keep back-head gate on true skin.
    if r < 148 and b < 112 and spread < 56:
        return False
    return 88 <= r <= 205 and 62 <= g <= 175 and 48 <= b <= 165 and r >= g - 24 and spread < 78


def _figure_span_y(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, h, max(1, h)
    min_y, max_y = min(ys), max(ys)
    return min_y, max_y, max(8, max_y - min_y)


def torso_box_coat_white_frac(panel: Image.Image, mask: Image.Image) -> tuple[float, int]:
    """Shoulder→hip center column (excludes most arm/hand pixels)."""
    w, h = panel.size
    min_y, _max_y, span = _figure_span_y(mask)
    y0 = min_y + int(span * 0.24)
    y1 = min_y + int(span * 0.66)
    x0, x1 = w // 2 - w // 5, w // 2 + w // 5
    coat = fig = 0
    for y in range(y0, y1, 2):
        for x in range(x0, x1, 2):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = panel.getpixel((x, y))[:3]
            if _is_backdrop(r, g, b):
                continue
            fig += 1
            if _is_coat_white(r, g, b):
                coat += 1
    if fig < 40:
        return 0.0, fig
    return coat / fig, fig


def back_of_head_skin_frac(panel: Image.Image, mask: Image.Image, panel_i: int) -> float:
    if panel_i not in (1, 2, 3):
        return 0.0
    w, h = panel.size
    min_y, _max_y, span = _figure_span_y(mask)
    if panel_i == 2:
        y0 = min_y + int(span * 0.05)
        y1 = min_y + int(span * 0.22)
        x0, x1 = w // 5, (4 * w) // 5
    else:
        # 90°/270°: crown + rear hair only (exclude cheek/neck profile skin).
        y0 = min_y + int(span * 0.02)
        y1 = min_y + int(span * 0.13)
        if panel_i == 1:
            x0, x1 = (9 * w) // 20, (17 * w) // 20
        else:
            x0, x1 = (3 * w) // 20, (11 * w) // 20
    skin = n = 0
    for y in range(y0, y1, 2):
        for x in range(x0, x1, 2):
            if mask.getpixel((x, y)) <= 48:
                continue
            n += 1
            if _is_skin_tan(*panel.getpixel((x, y))[:3]):
                skin += 1
    return skin / n if n > 30 else 0.0


def leg_roi_median_rgb(panel: Image.Image, mask: Image.Image) -> tuple[float, float, float]:
    w, h = panel.size
    min_y, _max_y, span = _figure_span_y(mask)
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


def check_researcher_panel_pixels(
    panel: Image.Image, mask: Image.Image, *, panel_i: int = 0
) -> tuple[bool, str]:
    coat, cn = torso_box_coat_white_frac(panel, mask)
    if cn >= 40 and coat < MIN_TORSO_COAT_WHITE_FRAC:
        return False, f"torso_coat_white={coat:.3f} want>={MIN_TORSO_COAT_WHITE_FRAC}"
    back_skin = back_of_head_skin_frac(panel, mask, panel_i)
    if back_skin > MAX_BACK_HEAD_SKIN_FRAC:
        return False, f"back_head_skin={back_skin:.3f} want<={MAX_BACK_HEAD_SKIN_FRAC}"
    if panel_i in (0, 3):
        _mr, _mg, dist = leg_roi_median_rgb(panel, mask)
        if _mr > 0 and dist > MAX_LEG_MEDIAN_CHARCOAL_DIST:
            return False, f"leg_median_dist={dist:.1f} want<={MAX_LEG_MEDIAN_CHARCOAL_DIST}"
    else:
        dist = 0.0
    return True, f"ok torso_coat={coat:.2f} back_head_skin={back_skin:.3f} leg_dist={dist:.1f}"