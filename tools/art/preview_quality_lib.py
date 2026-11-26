"""Shared early-art preview metrics (garage, staff, packet 3–4).

Object-mask pixels via neutral-backdrop rules; alpha-equivalent bbox framing;
saturation and distinctness helpers.
"""
from __future__ import annotations

import math
from pathlib import Path

MIN_RGB_DISTANCE = 14.0
NEUTRAL_LUMA_BAND = (0.26, 0.54)
NEUTRAL_SAT_CEIL = 0.12
MID_GREY_RGB = (128, 128, 133)
EDGE_PAD_FRAC = 0.015


def _pixel_sat_lum(r: int, g: int, b: int) -> tuple[float, float]:
    rf, gf, bf = r / 255.0, g / 255.0, b / 255.0
    mx = max(rf, gf, bf)
    mn = min(rf, gf, bf)
    lum = 0.2126 * rf + 0.7152 * gf + 0.0722 * bf
    if mx <= 1e-6:
        return 0.0, lum
    return (mx - mn) / mx, lum


def _dist_rgb(a: tuple[int, int, int], b: tuple[int, int, int]) -> float:
    return math.sqrt(sum((x - y) ** 2 for x, y in zip(a, b)))


def _backdrop_rgb(im) -> tuple[int, int, int]:
    w, h = im.size
    samples = [
        im.getpixel((2, 2)),
        im.getpixel((w - 3, 2)),
        im.getpixel((2, h - 3)),
        im.getpixel((w - 3, h - 3)),
        im.getpixel((w // 2, 2)),
        im.getpixel((w // 2, h - 3)),
    ]
    r = sum(p[0] for p in samples) // len(samples)
    g = sum(p[1] for p in samples) // len(samples)
    b = sum(p[2] for p in samples) // len(samples)
    return (r, g, b)


def is_object_pixel(sat: float, lum: float) -> bool:
    if sat >= 0.04:
        return True
    if lum < NEUTRAL_LUMA_BAND[0] or lum > NEUTRAL_LUMA_BAND[1]:
        return True
    if sat > NEUTRAL_SAT_CEIL and lum < 0.72:
        return True
    return False


def is_object_pixel_hud(
    rgb: tuple[int, int, int], bg: tuple[int, int, int], sat: float, lum: float
) -> bool:
    return _dist_rgb(rgb, bg) > 22


def alpha_object_mask(im, hud_mid_grey: bool = False) -> tuple[list[int], list[int], list[float]]:
    w, h = im.size
    has_alpha = im.mode in ("RGBA", "LA") or (im.mode == "P" and "transparency" in im.info)
    if im.mode != "RGBA":
        im = im.convert("RGBA")
    bg = _backdrop_rgb(im)
    xs: list[int] = []
    ys: list[int] = []
    sats: list[float] = []
    floor_y = int(h * 0.86) if not hud_mid_grey else h
    for y in range(h):
        if y >= floor_y:
            continue
        for x in range(w):
            r, g, b, a = im.getpixel((x, y))
            if has_alpha and a < 28:
                continue
            if not has_alpha:
                sat, lum = _pixel_sat_lum(r, g, b)
                if hud_mid_grey:
                    ok = is_object_pixel_hud((r, g, b), bg, sat, lum)
                else:
                    ok = _dist_rgb((r, g, b), bg) > 20 and (
                        sat > 0.075 or lum < 0.24 or lum > 0.62
                    )
                if not ok:
                    continue
            sat, _ = _pixel_sat_lum(r, g, b)
            xs.append(x)
            ys.append(y)
            sats.append(sat)
    return xs, ys, sats


def alpha_bbox_metrics(path: Path, hud_mid_grey: bool = False) -> dict:
    from PIL import Image

    im = Image.open(path).convert("RGB")
    w, h = im.size
    xs, ys, sats = alpha_object_mask(im, hud_mid_grey=hud_mid_grey)
    if not xs:
        return {
            "sat": 0.0,
            "fill_w": 0.0,
            "fill_h": 0.0,
            "edge_touch": True,
        }
    pad = max(2, int(min(w, h) * EDGE_PAD_FRAC))
    min_x, max_x = min(xs), max(xs)
    min_y, max_y = min(ys), max(ys)
    fill_w = (max_x - min_x + 1) / w
    fill_h = (max_y - min_y + 1) / h
    vivid = [s for s in sats if s > 0.045] or sats
    sat = sum(vivid) / len(vivid)
    edge_touch = (
        min_x <= pad or max_x >= w - 1 - pad or min_y <= pad or max_y >= h - 1 - pad
    )
    return {"sat": sat, "fill_w": fill_w, "fill_h": fill_h, "edge_touch": edge_touch}


def crop_center(im):
    w, h = im.size
    margin_x, margin_y = int(w * 0.12), int(h * 0.08)
    return im.crop((margin_x, margin_y, w - margin_x, h - margin_y))


def mean_saturation_object_mask(path: Path) -> tuple[float, float, float]:
    from PIL import Image

    im = crop_center(Image.open(path).convert("RGB"))
    px = list(im.getdata())
    sats: list[float] = []
    lum_all: list[float] = []
    for r, g, b in px:
        sat, lum = _pixel_sat_lum(r, g, b)
        lum_all.append(lum)
        if not is_object_pixel(sat, lum):
            continue
        sats.append(sat)
    if not sats:
        return 0.0, 0.0, 0.0
    sats_sorted = sorted(sats)
    accent_idx = max(0, int(len(sats_sorted) * 0.82) - 1)
    accent = sats_sorted[accent_idx]
    mean_sat = sum(sats) / len(sats)
    lbar = sum(lum_all) / len(lum_all)
    var = sum((x - lbar) ** 2 for x in lum_all) / len(lum_all)
    return mean_sat, var, accent


def mean_saturation_full(path: Path) -> tuple[float, float]:
    from PIL import Image

    im = Image.open(path).convert("RGB")
    px = list(im.getdata())
    sats: list[float] = []
    lum: list[float] = []
    for r, g, b in px:
        sat, lum_v = _pixel_sat_lum(r, g, b)
        lum.append(lum_v)
        if sat < 0.02:
            continue
        sats.append(sat)
    if not sats:
        return 0.0, 0.0
    mean_sat = sum(sats) / len(sats)
    lbar = sum(lum) / len(lum)
    var = sum((x - lbar) ** 2 for x in lum) / len(lum)
    return mean_sat, var


def downscale_rgb_crop(path: Path, size: int = 32) -> list[tuple[int, int, int]]:
    from PIL import Image

    im = crop_center(Image.open(path).convert("RGB"))
    return list(im.resize((size, size)).getdata())


def rgb_distance(a: list[tuple[int, int, int]], b: list[tuple[int, int, int]]) -> float:
    if len(a) != len(b):
        return 0.0
    total = 0.0
    used = 0
    for (r1, g1, b1), (r2, g2, b2) in zip(a, b):
        s1, _ = _pixel_sat_lum(r1, g1, b1)
        s2, _ = _pixel_sat_lum(r2, g2, b2)
        if s1 < 0.045 and s2 < 0.045:
            continue
        used += 1
        total += abs(r1 - r2) + abs(g1 - g2) + abs(b1 - b2)
    if used == 0:
        return 0.0
    return total / (used * 3.0)


def correlation_thumb(a: list[float], b: list[float]) -> float:
    dot = sum(x * y for x, y in zip(a, b))
    na = sum(x * x for x in a) ** 0.5
    nb = sum(y * y for y in b) ** 0.5
    if na < 1e-9 or nb < 1e-9:
        return 0.0
    return dot / (na * nb)


def thumb_vector(path: Path) -> list[float]:
    from PIL import Image

    im = Image.open(path).convert("L")
    w, h = im.size
    im = im.crop((int(w * 0.12), int(h * 0.12), int(w * 0.88), int(h * 0.88)))
    im = im.resize((48, 48))
    data = [v / 255.0 for v in im.get_flattened_data()]
    avg = sum(data) / len(data)
    return [(v - avg) for v in data]