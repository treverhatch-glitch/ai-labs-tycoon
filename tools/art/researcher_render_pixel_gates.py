#!/usr/bin/env python3
"""Researcher preview pixels: torso coat box (absolute ROI), back-of-head hair, legs."""
from __future__ import annotations

from dataclasses import dataclass

from PIL import Image

CHARCOAL_RGB = (58, 58, 58)
MIN_TORSO_COAT_WHITE_FRAC = 0.90
MAX_TORSO_SKIN_FRAC = 0.08
MAX_BACK_HEAD_SKIN_FRAC = 0.002
MAX_LEG_MEDIAN_CHARCOAL_DIST = 95.0


@dataclass(frozen=True)
class TorsoGateBox:
    panel_i: int
    x0: int
    y0: int
    x1: int
    y1: int
    coat_white_frac: float
    skin_tan_frac: float
    sample_n: int

    def as_dict(self) -> dict:
        return {
            "panel": self.panel_i,
            "x0": self.x0,
            "y0": self.y0,
            "x1": self.x1,
            "y1": self.y1,
            "coat_white_pct": round(100.0 * self.coat_white_frac, 1),
            "skin_tan_pct": round(100.0 * self.skin_tan_frac, 1),
            "n": self.sample_n,
        }


def _is_backdrop(r: int, g: int, b: int, grey: tuple[int, int, int] = (150, 150, 150)) -> bool:
    return abs(r - grey[0]) < 12 and abs(g - grey[1]) < 12 and abs(b - grey[2]) < 12


def _is_coat_white(r: int, g: int, b: int) -> bool:
    if _is_backdrop(r, g, b):
        return False
    spread = max(r, g, b) - min(r, g, b)
    lum = r + g + b
    if (
        r >= 132
        and g >= 128
        and b >= 120
        and lum >= 400
        and spread < 52
    ):
        return True
    if spread > 55:
        return False
    if r > g + 12:
        return False
    if spread < 28 and lum >= 205 and r >= 85:
        return True
    if (
        96 <= r <= 148
        and g >= r - 14
        and b >= r - 24
        and lum >= 298
        and spread < 26
    ):
        return True
    return lum >= 248 and r >= 98 and g >= 94 and abs(r - g) < 18 and spread < 44


def _is_skin_tan(r: int, g: int, b: int) -> bool:
    if _is_backdrop(r, g, b) or _is_coat_white(r, g, b):
        return False
    spread = max(r, g, b) - min(r, g, b)
    if r < 148 and b < 112 and spread < 56:
        return False
    return 88 <= r <= 205 and 62 <= g <= 175 and 48 <= b <= 165 and r >= g - 24 and spread < 78


def _torso_roi_x(panel: Image.Image, panel_i: int, mask: Image.Image | None) -> tuple[int, int]:
    w, _h = panel.size
    if mask is not None and panel_i in (1, 3):
        xs: list[int] = []
        mw, mh = mask.size
        for y in range(mh):
            for x in range(mw):
                if mask.getpixel((x, y)) > 48:
                    xs.append(x)
        if len(xs) > 80:
            min_x, max_x = min(xs), max(xs)
            span = max(8, max_x - min_x)
            return int(min_x + span * 0.30), int(min_x + span * 0.70)
    return w // 10, w - w // 10


def torso_gate_box(
    panel: Image.Image, panel_i: int, mask: Image.Image | None = None
) -> TorsoGateBox:
    """Shoulder→hip ROI; side panels use mask-centered torso column."""
    w, h = panel.size
    x0, x1 = _torso_roi_x(panel, panel_i, mask)
    y0, y1 = int(h * 0.19), int(h * 0.48)
    coat = skin = fig = 0
    for y in range(y0, y1, 2):
        for x in range(x0, x1, 2):
            r, g, b = panel.getpixel((x, y))[:3]
            if _is_backdrop(r, g, b):
                continue
            fig += 1
            if _is_coat_white(r, g, b):
                coat += 1
            elif _is_skin_tan(r, g, b):
                skin += 1
    coat_f = coat / fig if fig >= 40 else 0.0
    skin_f = skin / fig if fig >= 40 else 1.0
    return TorsoGateBox(panel_i, x0, y0, x1, y1, coat_f, skin_f, fig)


def torso_gate_boxes_report(panels: list[Image.Image]) -> list[dict]:
    rows: list[dict] = []
    gate_panels = (0, 1, 2, 3) if len(panels) >= 4 else tuple(range(len(panels)))
    for i in gate_panels:
        rows.append(torso_gate_box(panels[i], i).as_dict())
    return rows


def _figure_span_y(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, h, max(1, h)
    min_y, max_y = min(ys), max(ys)
    return min_y, max_y, max(8, max_y - min_y)


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
    panel: Image.Image,
    mask: Image.Image,
    *,
    panel_i: int = 0,
    walk_tile: bool = False,
) -> tuple[bool, str]:
    if panel_i in (0, 1, 2, 3):
        box = torso_gate_box(panel, panel_i, mask)
        if box.sample_n >= 40 and box.coat_white_frac < MIN_TORSO_COAT_WHITE_FRAC:
            return (
                False,
                f"panel{panel_i}_torso_coat_white={box.coat_white_frac:.3f} want>={MIN_TORSO_COAT_WHITE_FRAC}",
            )
        if box.sample_n >= 40 and box.skin_tan_frac > MAX_TORSO_SKIN_FRAC:
            return (
                False,
                f"panel{panel_i}_torso_skin_tan={box.skin_tan_frac:.3f} want<={MAX_TORSO_SKIN_FRAC}",
            )
        coat = box.coat_white_frac
    else:
        coat = 0.0
    if not walk_tile:
        back_skin = back_of_head_skin_frac(panel, mask, panel_i)
        if back_skin > MAX_BACK_HEAD_SKIN_FRAC:
            return False, f"back_head_skin={back_skin:.3f} want<={MAX_BACK_HEAD_SKIN_FRAC}"
    else:
        back_skin = 0.0
    if not walk_tile and panel_i in (0, 3):
        _mr, _mg, dist = leg_roi_median_rgb(panel, mask)
        if _mr > 0 and dist > MAX_LEG_MEDIAN_CHARCOAL_DIST:
            return False, f"leg_median_dist={dist:.1f} want<={MAX_LEG_MEDIAN_CHARCOAL_DIST}"
    else:
        dist = 0.0
    return True, f"ok torso_coat={coat:.2f} back_head_skin={back_skin:.3f} leg_dist={dist:.1f}"