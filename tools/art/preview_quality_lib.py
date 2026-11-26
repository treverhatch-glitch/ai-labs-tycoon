"""Shared early-art preview metrics (garage + staff lanes).

- Object-mask saturation: pixels with sat >= 0.04 or outside neutral backdrop band.
- Distinctness: 32×32 mean |ΔRGB| on center crop (MIN_RGB_DISTANCE).
"""
from __future__ import annotations

from pathlib import Path

MIN_RGB_DISTANCE = 14.0
NEUTRAL_LUMA_BAND = (0.26, 0.54)
NEUTRAL_SAT_CEIL = 0.12


def _pixel_sat_lum(r: int, g: int, b: int) -> tuple[float, float]:
    rf, gf, bf = r / 255.0, g / 255.0, b / 255.0
    mx = max(rf, gf, bf)
    mn = min(rf, gf, bf)
    lum = 0.2126 * rf + 0.7152 * gf + 0.0722 * bf
    if mx <= 1e-6:
        return 0.0, lum
    return (mx - mn) / mx, lum


def crop_center(im):
    w, h = im.size
    margin_x, margin_y = int(w * 0.12), int(h * 0.08)
    return im.crop((margin_x, margin_y, w - margin_x, h - margin_y))


def is_object_pixel(sat: float, lum: float) -> bool:
    if sat >= 0.04:
        return True
    if lum < NEUTRAL_LUMA_BAND[0] or lum > NEUTRAL_LUMA_BAND[1]:
        return True
    if sat > NEUTRAL_SAT_CEIL and lum < 0.72:
        return True
    return False


def wisp_mood_accent_sat(path: Path) -> float:
    """Peak saturation on vivid object pixels (face / emissive), not pastel body."""
    from PIL import Image

    im = crop_center(Image.open(path).convert("RGB"))
    vivid: list[float] = []
    for r, g, b in im.getdata():
        sat, lum = _pixel_sat_lum(r, g, b)
        if not is_object_pixel(sat, lum):
            continue
        if sat >= 0.08:
            vivid.append(sat)
    if not vivid:
        return 0.0
    return max(vivid)


def mean_saturation_object_mask(path: Path) -> tuple[float, float, float]:
    """Return (object_mean_sat, full luma_var, accent_sat on object pixels)."""
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
    """Garage lane: full-frame mean saturation (all pixels)."""
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


def downscale_rgb_face_band(path: Path, size: int = 32) -> list[tuple[int, int, int]]:
    """Upper band crop for wisp mood markers / faces (distinctness)."""
    from PIL import Image

    im = crop_center(Image.open(path).convert("RGB"))
    w, h = im.size
    im = im.crop((0, 0, w, max(8, int(h * 0.58))))
    return list(im.resize((size, size)).getdata())


def rgb_distance_full(a: list[tuple[int, int, int]], b: list[tuple[int, int, int]]) -> float:
    if len(a) != len(b):
        return 0.0
    total = 0.0
    for (r1, g1, b1), (r2, g2, b2) in zip(a, b):
        total += abs(r1 - r2) + abs(g1 - g2) + abs(b1 - b2)
    return total / (len(a) * 3.0)


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