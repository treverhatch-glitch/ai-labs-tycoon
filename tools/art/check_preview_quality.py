#!/usr/bin/env python3
"""Fail near-uniform / low-chroma preview PNGs for BlenderBaked gate assets."""
from __future__ import annotations

import colorsys
import hashlib
import json
import math
import struct
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW_DIR = REPO / "docs" / "art" / "previews"
BLENDER_ROOT = REPO / "unity" / "Assets" / "Art" / "BlenderBaked"

GATED_ASSETS = (
    "prop-desk",
    "prop-server-rack",
)

MIN_STDDEV = 8.0
MIN_COVERAGE = 0.20
MIN_MEAN_LUM = 40.0
MIN_MEAN_SAT = 0.10
BORDER_PX = 4
BORDER_MIN_PIXELS = 18
BACKDROP_RGB = 150
BACKDROP_BAND_FRAC = 0.03
BACKDROP_GRID_FRACS = (0.02, 0.5, 0.98)
BACKDROP_CHANNEL_TOL = 1
MAX_BACKDROP_BAND_BAD_PX = 128


def _backdrop_channel_ok(r: int, g: int, b: int) -> bool:
    return all(abs(ch - BACKDROP_RGB) <= BACKDROP_CHANNEL_TOL for ch in (r, g, b))

FIXTURES_MUST_FAIL: tuple[tuple[str, Path, str], ...] = (
    (
        "prop-server-rack",
        REPO / "tools/art/fixtures/prop-server-rack-dfc65859-regression.png",
        "dfc65859",
    ),
    (
        "prop-server-rack",
        REPO / "tools/art/fixtures/prop-server-rack-0125cd4d-regression.png",
        "0125cd4d",
    ),
    (
        "prop-server-rack",
        REPO / "tools/art/fixtures/prop-server-rack-aa299d14-regression.png",
        "aa299d14",
    ),
)
MIN_RACK_SUBJECT_LUM_STDDEV = 0.038
MIN_RACK_LED_PIXELS = 48
MIN_RACK_FRONT_LED_PIXELS = 36
MIN_RACK_FRONT_NORMAL_DOT = 0.30
MIN_RACK_H_EDGE_FEATURES = 10
MIN_RACK_CONTACT_SHADOW_FRAC = 0.015
MAX_RACK_OBJECT_BROWN_FRAC = 0.22
MIN_SIL_FRAC = 0.35
MAX_SIL_FRAC = 0.75
MIN_SUBJECT_BBOX_FRAC = 0.60
MAX_SUBJECT_BBOX_FRAC = 0.90
MIN_FRAME_MARGIN_FRAC = 0.05
MAX_FLAT_PIXEL_FRAC = 0.50
MAX_GROUND_LUM = 165


def _read_png_rgb(path: Path) -> tuple[int, int, list[tuple[int, int, int]]]:
    try:
        from PIL import Image

        im = Image.open(path).convert("RGB")
        w, h = im.size
        return w, h, list(im.getdata())
    except ImportError:
        pass
    import zlib

    data = path.read_bytes()
    if data[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError(f"not PNG: {path}")
    pos = 8
    width = height = None
    color_type = None
    raw = b""
    while pos < len(data):
        length = struct.unpack(">I", data[pos : pos + 4])[0]
        pos += 4
        ctype = data[pos : pos + 4]
        pos += 4
        chunk = data[pos : pos + length]
        pos += length + 4
        if ctype == b"IHDR":
            width, height, bitdepth, color_type = struct.unpack(">IIBB", chunk[:10])[:4]
        elif ctype == b"IDAT":
            raw += chunk
        elif ctype == b"IEND":
            break
    if width is None or height is None:
        raise ValueError(f"no IHDR: {path}")
    bpp = {0: 1, 2: 3, 3: 1, 4: 2, 6: 4}.get(color_type or 2, 3)
    inflated = zlib.decompress(raw)

    def _paeth(a: int, b: int, c: int) -> int:
        p = a + b - c
        pa, pb, pc = abs(p - a), abs(p - b), abs(p - c)
        if pa <= pb and pa <= pc:
            return a
        if pb <= pc:
            return b
        return c

    row_bytes = width * bpp
    prior = bytearray(row_bytes)
    flat = bytearray()
    pos = 0
    for _y in range(height):
        ftype = inflated[pos]
        pos += 1
        chunk = bytearray(inflated[pos : pos + row_bytes])
        pos += row_bytes
        if ftype == 1:
            for i in range(row_bytes):
                chunk[i] = (chunk[i] + prior[i]) & 0xFF
        elif ftype == 2:
            for i in range(row_bytes):
                chunk[i] = (chunk[i] + flat[i]) & 0xFF
        elif ftype == 3:
            for i in range(row_bytes):
                left = prior[i] if i >= bpp else 0
                chunk[i] = (chunk[i] + (left // 2)) & 0xFF
        elif ftype == 4:
            for i in range(row_bytes):
                left = prior[i] if i >= bpp else 0
                up = flat[i]
                up_left = prior[i - bpp] if i >= bpp else 0
                chunk[i] = (chunk[i] + _paeth(left, up, up_left)) & 0xFF
        prior = chunk
        flat.extend(chunk)

    pixels: list[tuple[int, int, int]] = []
    for y in range(height):
        row = flat[y * row_bytes : (y + 1) * row_bytes]
        for x in range(width):
            i = x * bpp
            if bpp >= 3:
                pixels.append((row[i], row[i + 1], row[i + 2]))
            else:
                g = row[i]
                pixels.append((g, g, g))
    return width, height, pixels


def _bg_rgb(pixels: list[tuple[int, int, int]], width: int, height: int) -> tuple[int, int, int]:
    picks = [
        width // 2,
        width // 4,
        (3 * width) // 4,
        8,
        width - 9,
    ]
    samples = [pixels[x] for x in picks]
    return (
        sum(s[0] for s in samples) // len(samples),
        sum(s[1] for s in samples) // len(samples),
        sum(s[2] for s in samples) // len(samples),
    )


def _is_subject(r: int, g: int, b: int, bg: tuple[int, int, int]) -> bool:
    dist = abs(r - bg[0]) + abs(g - bg[1]) + abs(b - bg[2])
    if dist < 32:
        return False
    lum = (r * 299 + g * 587 + b * 114) // 1000
    mx, mn = max(r, g, b), min(r, g, b)
    sat = (mx - mn) / 255.0 if mx else 0.0
    if sat < 0.07 and 100 <= lum <= 150:
        return False
    if r > g + 3 and g >= b - 2 and r > 68 and lum > 24:
        return True
    if r > g + 8 and g > b + 6 and lum > 50:
        return True
    return sat > 0.09 and lum > 52 and lum < 245


def _object_pixel(r: int, g: int, b: int, bg: tuple[int, int, int]) -> bool:
    return abs(r - bg[0]) + abs(g - bg[1]) + abs(b - bg[2]) > 28


def _pixel_lum(r: int, g: int, b: int) -> float:
    return max(r, g, b) / 255.0


def _hue_deg(r: int, g: int, b: int) -> float:
    h, _l, s = colorsys.rgb_to_hls(r / 255.0, g / 255.0, b / 255.0)
    if max(r, g, b) < 20:
        return -1.0
    return h * 360.0 if s > 0.06 else -1.0


def _rack_detail_metrics(
    pixels: list[tuple[int, int, int]],
    width: int,
    height: int,
    bg: tuple[int, int, int],
) -> tuple[float, int, int]:
    """Reject flat brown-box previews: luminance spread, LED pixels, bay edges."""
    all_lums: list[float] = []
    led_px = 0
    xs: list[int] = []
    ys: list[int] = []
    x_lo = int(width * 0.26)
    x_hi = int(width * 0.80)
    y_lo = int(height * 0.14)
    y_hi = int(height * 0.90)
    for y in range(height):
        for x in range(width):
            r, g, b = pixels[y * width + x]
            if _object_pixel(r, g, b, bg):
                if r > g + 15 and g >= b - 8 and r > 105:
                    continue
                all_lums.append((r + g + b) / (3.0 * 255.0))
            if not (x_lo <= x <= x_hi and y_lo <= y <= y_hi):
                continue
            if not _object_pixel(r, g, b, bg):
                continue
            xs.append(x)
            ys.append(y)
            if g > r + 14 and g > 72 and b < g - 5:
                led_px += 1
            elif b > r + 10 and b > 72 and g < b - 4:
                led_px += 1
    if len(all_lums) < 80 or len(xs) < 40:
        return 0.0, led_px, 0
    mean = sum(all_lums) / len(all_lums)
    std = math.sqrt(sum((v - mean) ** 2 for v in all_lums) / len(all_lums))
    x0, x1 = min(xs), max(xs)
    y0, y1 = min(ys), max(ys)
    edge_rows = 0
    step = max(1, (y1 - y0) // 18)
    for y in range(y0, y1, step):
        prev: float | None = None
        jumps = 0
        for x in range(x0, x1):
            r, g, b = pixels[y * width + x]
            if not _object_pixel(r, g, b, bg):
                prev = None
                continue
            lum = (r + g + b) / (3.0 * 255.0)
            if prev is not None and abs(lum - prev) > 0.075:
                jumps += 1
            prev = lum
        if jumps >= 2:
            edge_rows += 1
    return std, led_px, edge_rows


def _rack_front_led_pixels(
    pixels: list[tuple[int, int, int]],
    width: int,
    height: int,
    bg: tuple[int, int, int],
) -> int:
    x0, x1 = int(width * 0.26), int(width * 0.80)
    y0, y1 = int(height * 0.14), int(height * 0.90)
    led_px = 0
    for y in range(y0, y1):
        for x in range(x0, x1):
            r, g, b = pixels[y * width + x]
            if not _object_pixel(r, g, b, bg):
                continue
            if g > r + 14 and g > 72 and b < g - 5:
                led_px += 1
            elif b > r + 10 and b > 72 and g < b - 4:
                led_px += 1
    return led_px


def _rack_brown_object_frac(
    pixels: list[tuple[int, int, int]],
    width: int,
    height: int,
    bg: tuple[int, int, int],
) -> float:
    brown = 0
    total = 0
    for r, g, b in pixels:
        if not _object_pixel(r, g, b, bg):
            continue
        total += 1
        if r > g + 12 and r > 88 and b < g:
            brown += 1
    return (brown / total) if total else 0.0


def _rack_contact_shadow_frac(
    pixels: list[tuple[int, int, int]],
    width: int,
    height: int,
    bg: tuple[int, int, int],
) -> float:
    y0 = int(height * 0.9)
    dark = 0
    total = 0
    for y in range(y0, height):
        for x in range(int(width * 0.25), int(width * 0.76)):
            r, g, b = pixels[y * width + x]
            lum = (r * 299 + g * 587 + b * 114) // 1000
            total += 1
            if 70 <= lum <= 142:
                dist = abs(r - bg[0]) + abs(g - bg[1]) + abs(b - bg[2])
                if dist > 8 or lum < BACKDROP_RGB - 4:
                    dark += 1
    return (dark / total) if total else 0.0


def _backdrop_metrics(
    pixels: list[tuple[int, int, int]],
    width: int,
    height: int,
    bg: tuple[int, int, int],
) -> tuple[int, float]:
    """3% edge band: backdrop pixels only, non-150 count. 9-point grid on backdrop samples."""
    band = max(1, int(min(width, height) * BACKDROP_BAND_FRAC))
    bad = 0
    for y in range(height):
        for x in range(width):
            if x >= band and x < width - band and y >= band and y < height - band:
                continue
            if y >= height - band:
                continue
            r, g, b = pixels[y * width + x]
            if _object_pixel(r, g, b, bg):
                continue
            if not _backdrop_channel_ok(r, g, b):
                bad += 1
    grid_max = 0.0
    for u in BACKDROP_GRID_FRACS:
        for v in BACKDROP_GRID_FRACS:
            if v >= 0.9 and 0.12 < u < 0.88:
                continue
            x = min(width - 1, int(u * (width - 1)))
            y = min(height - 1, int(v * (height - 1)))
            r, g, b = pixels[y * width + x]
            if _object_pixel(r, g, b, bg):
                continue
            delta = max(abs(ch - BACKDROP_RGB) for ch in (r, g, b))
            grid_max = max(grid_max, float(delta))
    return bad, grid_max


def _frame_pixel(
    r: int,
    g: int,
    b: int,
    bg: tuple[int, int, int],
    *,
    asset_id: str | None,
    y: int | None = None,
    height: int | None = None,
) -> bool:
    if asset_id == "prop-desk":
        return _is_subject(r, g, b, bg)
    if asset_id == "prop-server-rack":
        if not _object_pixel(r, g, b, bg):
            return False
        if y is not None and height is not None and y >= int(height * 0.91):
            lum = (r * 299 + g * 587 + b * 114) // 1000
            if lum >= 122 and lum <= 150:
                dist = abs(r - bg[0]) + abs(g - bg[1]) + abs(b - bg[2])
                if dist < 48:
                    return False
        return True
    return _object_pixel(r, g, b, bg)


def _framing_metrics(
    width: int,
    height: int,
    pixels: list[tuple[int, int, int]],
    *,
    asset_id: str | None = None,
) -> dict[str, float | bool]:
    bg = _bg_rgb(pixels, width, height)
    edge_hits = {"t": 0, "b": 0, "l": 0, "r": 0}
    row_hits = [0] * height
    for y in range(height):
        for x in range(width):
            r, g, b = pixels[y * width + x]
            if not _object_pixel(r, g, b, bg):
                continue
            row_hits[y] += 1
            subj = _is_subject(r, g, b, bg)
            if subj:
                if y < BORDER_PX:
                    edge_hits["t"] += 1
                if y >= height - BORDER_PX and y < int(height * 0.92):
                    edge_hits["b"] += 1
                if x < BORDER_PX:
                    edge_hits["l"] += 1
                if x >= width - BORDER_PX:
                    edge_hits["r"] += 1
    border_touch = any(v >= BORDER_MIN_PIXELS for v in edge_hits.values())
    subj_ys: list[int] = []
    for y in range(height):
        for x in range(width):
            r, g, b = pixels[y * width + x]
            if _frame_pixel(r, g, b, bg, asset_id=asset_id, y=y, height=height):
                subj_ys.append(y)
                break
    sil_frac = (max(subj_ys) - min(subj_ys) + 1) / height if subj_ys else 0.0
    xs: list[int] = []
    ys2: list[int] = []
    for y in range(height):
        for x in range(width):
            r, g, b = pixels[y * width + x]
            if _frame_pixel(r, g, b, bg, asset_id=asset_id, y=y, height=height):
                xs.append(x)
                ys2.append(y)
    if xs:
        bw = (max(xs) - min(xs) + 1) / width
        bh = (max(ys2) - min(ys2) + 1) / height
        subject_bbox = max(bw, bh)
        margin_l = min(xs) / width
        margin_r = (width - 1 - max(xs)) / width
        margin_t = min(ys2) / height
        margin_b = (height - 1 - max(ys2)) / height
        min_margin = min(margin_l, margin_r, margin_t, margin_b)
    else:
        subject_bbox = 0.0
        min_margin = 0.0
    return {
        "border_touch": border_touch,
        "silhouette_frac": sil_frac,
        "subject_bbox_frac": subject_bbox,
        "min_margin_frac": min_margin,
    }


def stats(path: Path, asset_id: str | None = None) -> dict[str, float]:
    width, height, pixels = _read_png_rgb(path)
    if not pixels:
        return {"stddev": 0.0, "coverage": 0.0, "mean_lum": 0.0, "mean_sat": 0.0}
    lums: list[int] = []
    sats: list[float] = []
    for r, g, b in pixels:
        lums.append((r * 299 + g * 587 + b * 114) // 1000)
        mx, mn = max(r, g, b), min(r, g, b)
        sats.append((mx - mn) / 255.0 if mx else 0.0)
    n = len(lums)
    mean = sum(lums) / n
    var = sum((x - mean) ** 2 for x in lums) / n
    std = var**0.5
    coverage = sum(1 for x in lums if x > 24) / n
    bg = _bg_rgb(pixels, width, height)
    subj_lum: list[int] = []
    subj_sat: list[float] = []
    for i, (r, g, b) in enumerate(pixels):
        if _is_subject(r, g, b, bg):
            subj_lum.append(lums[i])
            subj_sat.append(sats[i])
    cn = len(subj_lum) or 1
    frame = _framing_metrics(width, height, pixels, asset_id=asset_id)
    ground_lum: list[int] = []
    y0 = int(height * 0.88)
    for y in range(y0, height):
        for x in range(int(width * 0.25), int(width * 0.75)):
            r, g, b = pixels[y * width + x]
            ground_lum.append((r * 299 + g * 587 + b * 114) // 1000)
    gmean = sum(ground_lum) / len(ground_lum) if ground_lum else 255.0
    from collections import Counter

    near_bg = sum(
        1
        for r, g, b in pixels
        if abs(r - bg[0]) + abs(g - bg[1]) + abs(b - bg[2]) < 18
    )
    flat_frac = near_bg / n
    corner_samples: list[tuple[int, int, int]] = []
    margin = 3
    for cx, cy in (
        (margin, margin),
        (width - 1 - margin, margin),
        (margin, height - 1 - margin),
        (width - 1 - margin, height - 1 - margin),
    ):
        corner_samples.append(pixels[cy * width + cx])
    corner_lums = [
        (r * 299 + g * 587 + b * 114) // 1000 for r, g, b in corner_samples
    ]
    corner_mean = sum(corner_lums) / len(corner_lums) if corner_lums else 255.0
    corner_rgb_max_delta = 0.0
    for r, g, b in corner_samples:
        for ch in (r, g, b):
            corner_rgb_max_delta = max(corner_rgb_max_delta, abs(ch - BACKDROP_RGB))
    backdrop_band_bad, grid_nine_delta = _backdrop_metrics(pixels, width, height, bg)
    desk_extra: dict[str, float] = {}
    rack_extra: dict[str, float] = {}
    if asset_id == "prop-server-rack":
        r_std, led_n, edge_n = _rack_detail_metrics(pixels, width, height, bg)
        rack_extra = {
            "rack_subject_lum_stddev": r_std,
            "rack_led_pixel_count": float(led_n),
            "rack_h_edge_features": float(edge_n),
            "rack_front_led_pixel_count": float(
                _rack_front_led_pixels(pixels, width, height, bg)
            ),
            "rack_contact_shadow_frac": _rack_contact_shadow_frac(
                pixels, width, height, bg
            ),
            "rack_brown_object_frac": _rack_brown_object_frac(
                pixels, width, height, bg
            ),
        }
        is_live = path.resolve() == (PREVIEW_DIR / f"{asset_id}.png").resolve()
        meta_path = PREVIEW_DIR / "prop-server-rack.preview_meta.json"
        if is_live and meta_path.is_file():
            try:
                meta = json.loads(meta_path.read_text(encoding="utf-8"))
                rack_extra["rack_front_normal_dot"] = float(
                    meta.get("rack_front_normal_dot", 0.0)
                )
            except (json.JSONDecodeError, TypeError, ValueError):
                rack_extra["rack_front_normal_dot"] = 0.0
        elif asset_id == "prop-server-rack":
            rack_extra["rack_front_normal_dot"] = 0.0
    if asset_id == "prop-desk":
        subj_sats_sorted = sorted(subj_sat)
        med_sat = (
            subj_sats_sorted[len(subj_sats_sorted) // 2] if subj_sats_sorted else 0.0
        )
        y0, y1 = int(height * 0.42), int(height * 0.54)
        x0, x1 = int(width * 0.12), int(width * 0.88)
        desk_px = 0
        brown_px = 0
        for y in range(y0, y1):
            for x in range(x0, x1):
                r, g, b = pixels[y * width + x]
                if not _is_subject(r, g, b, bg):
                    continue
                desk_px += 1
                if r > g + 4 and g > b and r > 95:
                    brown_px += 1
        desk_brown_frac = brown_px / desk_px if desk_px else 0.0
        sy0, sy1 = int(height * 0.16), int(height * 0.50)
        sx0, sx1 = int(width * 0.36), int(width * 0.64)
        screen_lums: list[float] = []
        for y in range(sy0, sy1):
            for x in range(sx0, sx1):
                r, g, b = pixels[y * width + x]
                if not _is_subject(r, g, b, bg):
                    continue
                screen_lums.append(max(r, g, b) / 255.0)
        screen_peak = max(screen_lums) if screen_lums else 0.0
        outside_screen_sat: list[float] = []
        blow_n = 0
        outside_n = 0
        for y in range(height):
            for x in range(width):
                r, g, b = pixels[y * width + x]
                if not _is_subject(r, g, b, bg):
                    continue
                in_screen = sx0 <= x <= sx1 and sy0 <= y <= sy1
                mx, mn = max(r, g, b), min(r, g, b)
                if not in_screen:
                    outside_n += 1
                    outside_screen_sat.append((mx - mn) / 255.0)
                    if mx >= 255:
                        blow_n += 1
        blow_frac_outside = blow_n / outside_n if outside_n else 0.0
        outside_mean_sat = (
            sum(outside_screen_sat) / len(outside_screen_sat) if outside_screen_sat else 0.0
        )
        if outside_n < 80:
            outside_mean_sat = sum(subj_sat) / cn if subj_sat else 0.0
        desk_lums: list[float] = []
        desk_sats: list[float] = []
        kx0, kx1 = int(width * 0.20), int(width * 0.54)
        ky0, ky1 = int(height * 0.48), int(height * 0.54)
        for y in range(y0, y1):
            for x in range(x0, x1):
                r, g, b = pixels[y * width + x]
                if not _object_pixel(r, g, b, bg):
                    continue
                if sx0 <= x <= sx1 and sy0 <= y <= sy1:
                    continue
                if kx0 <= x <= kx1 and ky0 <= y <= ky1:
                    continue
                if not (r > g + 4 and g > b and r > 88):
                    continue
                mx, mn = max(r, g, b), min(r, g, b)
                desk_lums.append((r + g + b) / (3.0 * 255.0))
                desk_sats.append((mx - mn) / 255.0)
        desk_mean_lum = sum(desk_lums) / len(desk_lums) if desk_lums else 0.0
        desk_mean_sat = sum(desk_sats) / len(desk_sats) if desk_sats else 0.0
        dark_cells = 0
        cell_total = 0
        for gy in range(4):
            for gx in range(4):
                x0c = int(width * (0.08 + gx * 0.21))
                x1c = int(width * (0.08 + (gx + 1) * 0.21))
                y0c = int(height * (0.08 + gy * 0.21))
                y1c = int(height * (0.08 + (gy + 1) * 0.21))
                cell_lums: list[float] = []
                for y in range(y0c, y1c):
                    for x in range(x0c, x1c):
                        r, g, b = pixels[y * width + x]
                        if not _object_pixel(r, g, b, bg):
                            continue
                        if sx0 <= x <= sx1 and sy0 <= y <= sy1:
                            continue
                        cell_lums.append(max(r, g, b) / 255.0)
                if len(cell_lums) < 12:
                    continue
                cell_total += 1
                if sum(cell_lums) / len(cell_lums) < 0.08:
                    dark_cells += 1
        dark_cell_frac = dark_cells / cell_total if cell_total else 0.0
        bezel_orange_n = 0
        bezel_n = 0
        by0, by1 = int(height * 0.10), int(height * 0.36)
        bx0, bx1 = int(width * 0.38), int(width * 0.66)
        for y in range(by0, by1):
            for x in range(bx0, bx1):
                if sx0 <= x <= sx1 and sy0 <= y <= sy1:
                    continue
                r, g, b = pixels[y * width + x]
                if not _object_pixel(r, g, b, bg):
                    continue
                bezel_n += 1
                if r > g + 6 and g > b + 2 and r > 88:
                    bezel_orange_n += 1
        bezel_wood_frac = bezel_orange_n / bezel_n if bezel_n else 0.0
        obj_pix = 0
        dark_obj = 0
        for y in range(height):
            for x in range(width):
                r, g, b = pixels[y * width + x]
                if not _is_subject(r, g, b, bg):
                    continue
                obj_pix += 1
                if _pixel_lum(r, g, b) < 0.06:
                    dark_obj += 1
        dark_object_frac = dark_obj / obj_pix if obj_pix else 1.0
        screen_edge_rows: dict[int, list[int]] = {}
        mid_x = (sx0 + sx1) // 2
        for y in range(sy0, sy1):
            for x in range(mid_x - 10, mid_x + 10):
                if x < sx0 or x >= sx1:
                    continue
                r, g, b = pixels[y * width + x]
                if not _is_subject(r, g, b, bg):
                    continue
                if _pixel_lum(r, g, b) < 0.48:
                    continue
                screen_edge_rows.setdefault(y, []).append(x)
        monitor_skew_deg = 0.0
        if len(screen_edge_rows) >= 8:
            top_y = min(screen_edge_rows)
            bot_y = max(screen_edge_rows)
            top_mid = (min(screen_edge_rows[top_y]) + max(screen_edge_rows[top_y])) * 0.5
            bot_mid = (min(screen_edge_rows[bot_y]) + max(screen_edge_rows[bot_y])) * 0.5
            h_span = max(bot_y - top_y, 1)
            monitor_skew_deg = math.degrees(math.atan(abs(top_mid - bot_mid) / h_span))
        py0, py1 = int(height * 0.24), int(height * 0.30)
        px0, px1 = int(width * 0.38), int(width * 0.47)
        plant_n = 0
        plant_green = 0
        for y in range(py0, py1):
            for x in range(px0, px1):
                r, g, b = pixels[y * width + x]
                if not _object_pixel(r, g, b, bg):
                    continue
                plant_n += 1
                h_deg = _hue_deg(r, g, b)
                if 100.0 <= h_deg <= 130.0 and g > r + 8 and g > b + 6:
                    plant_green += 1
        plant_green_frac = plant_green / plant_n if plant_n else 0.0
        desk_extra = {
            "subject_median_sat": med_sat,
            "desk_top_brown_frac": desk_brown_frac,
            "screen_peak_lum": screen_peak,
            "corner_rgb_max_delta": corner_rgb_max_delta,
            "outside_screen_mean_sat": outside_mean_sat,
            "blow_frac_outside_screen": blow_frac_outside,
            "desk_mean_lum": desk_mean_lum,
            "desk_mean_sat": desk_mean_sat,
            "dark_subject_cell_frac": dark_cell_frac,
            "bezel_wood_frac": bezel_wood_frac,
            "dark_object_frac": dark_object_frac,
            "monitor_skew_deg": monitor_skew_deg,
            "plant_green_frac": plant_green_frac,
        }
    return {
        "stddev": std,
        "coverage": coverage,
        "mean_lum": sum(subj_lum) / cn,
        "mean_sat": sum(subj_sat) / cn,
        "border_touch": float(frame["border_touch"]),
        "silhouette_frac": float(frame["silhouette_frac"]),
        "subject_bbox_frac": float(frame["subject_bbox_frac"]),
        "min_margin_frac": float(frame["min_margin_frac"]),
        "flat_pixel_frac": flat_frac,
        "ground_lum": gmean,
        "corner_lum_mean": corner_mean,
        "corner_rgb_max_delta": corner_rgb_max_delta,
        "backdrop_band_bad_px": float(backdrop_band_bad),
        "backdrop_grid_nine_delta": grid_nine_delta,
        **desk_extra,
        **rack_extra,
    }


def gate_fail(asset_id: str, s: dict[str, float]) -> list[str]:
    reasons: list[str] = []
    if s["stddev"] < MIN_STDDEV or s["coverage"] < MIN_COVERAGE:
        reasons.append(
            f"uniform stddev={s['stddev']:.2f} coverage={s['coverage']:.1%}"
        )
    min_sat = 0.075 if asset_id in ("prop-desk", "prop-server-rack") else MIN_MEAN_SAT
    if s["mean_lum"] < MIN_MEAN_LUM or s["mean_sat"] < min_sat:
        reasons.append(
            f"chroma mean_lum={s['mean_lum']:.1f} mean_sat={s['mean_sat']:.3f}"
        )
    if s.get("border_touch", 0) >= 0.5:
        reasons.append("framing border_touch")
    sil = s.get("silhouette_frac", 0.0)
    sil_max = 0.90 if asset_id in ("prop-desk", "prop-server-rack") else MAX_SIL_FRAC
    if sil and (sil < MIN_SIL_FRAC or sil > sil_max):
        reasons.append(f"framing silhouette_frac={sil:.2%}")
    if asset_id in ("prop-desk", "prop-server-rack"):
        sb = s.get("subject_bbox_frac", 0.0)
        if sb < MIN_SUBJECT_BBOX_FRAC or sb > MAX_SUBJECT_BBOX_FRAC:
            reasons.append(f"subject_bbox_frac={sb:.2%}")
        mg = s.get("min_margin_frac", 0.0)
        if asset_id != "prop-server-rack" and mg < MIN_FRAME_MARGIN_FRAC:
            reasons.append(f"min_margin_frac={mg:.2%}")
        flat_max = 0.78 if asset_id == "prop-desk" else (0.82 if asset_id == "prop-server-rack" else MAX_FLAT_PIXEL_FRAC)
        if s.get("flat_pixel_frac", 1.0) > flat_max:
            reasons.append(f"flat_pixel_frac={s['flat_pixel_frac']:.2%}")
        ground_max = 178 if asset_id in ("prop-desk", "prop-server-rack") else MAX_GROUND_LUM
        if s.get("ground_lum", 255) > ground_max:
            reasons.append(f"ground_lum={s['ground_lum']:.1f}")
        if s.get("corner_rgb_max_delta", 99.0) > 0.0:
            reasons.append(
                f"corner_rgb_max_delta={s.get('corner_rgb_max_delta', 0):.1f} (want exactly 150)"
            )
        if asset_id == "prop-server-rack":
            if s.get("backdrop_band_bad_px", 0.0) > MAX_BACKDROP_BAND_BAD_PX:
                reasons.append(
                    f"backdrop_band_bad_px={int(s.get('backdrop_band_bad_px', 0))} (max {MAX_BACKDROP_BAND_BAD_PX})"
                )
            if s.get("backdrop_grid_nine_delta", 0.0) > BACKDROP_CHANNEL_TOL:
                reasons.append(
                    f"backdrop_grid_nine_delta={s.get('backdrop_grid_nine_delta', 0):.1f} (max {BACKDROP_CHANNEL_TOL})"
                )
    if asset_id == "prop-server-rack":
        if s.get("rack_subject_lum_stddev", 0.0) < MIN_RACK_SUBJECT_LUM_STDDEV:
            reasons.append(
                f"rack_subject_lum_stddev={s.get('rack_subject_lum_stddev', 0):.3f} (min {MIN_RACK_SUBJECT_LUM_STDDEV})"
            )
        if s.get("rack_led_pixel_count", 0.0) < MIN_RACK_LED_PIXELS:
            reasons.append(
                f"rack_led_pixel_count={int(s.get('rack_led_pixel_count', 0))} (min {MIN_RACK_LED_PIXELS})"
            )
        if s.get("rack_h_edge_features", 0.0) < MIN_RACK_H_EDGE_FEATURES:
            reasons.append(
                f"rack_h_edge_features={int(s.get('rack_h_edge_features', 0))} (min {MIN_RACK_H_EDGE_FEATURES})"
            )
        if s.get("rack_front_led_pixel_count", 0.0) < MIN_RACK_FRONT_LED_PIXELS:
            reasons.append(
                f"rack_front_led_pixel_count={int(s.get('rack_front_led_pixel_count', 0))} (min {MIN_RACK_FRONT_LED_PIXELS})"
            )
        if s.get("rack_front_normal_dot", 0.0) < MIN_RACK_FRONT_NORMAL_DOT:
            reasons.append(
                f"rack_front_normal_dot={s.get('rack_front_normal_dot', 0):.2f} (min {MIN_RACK_FRONT_NORMAL_DOT})"
            )
        if s.get("rack_contact_shadow_frac", 0.0) < MIN_RACK_CONTACT_SHADOW_FRAC:
            reasons.append(
                f"rack_contact_shadow_frac={s.get('rack_contact_shadow_frac', 0):.3%} (min {MIN_RACK_CONTACT_SHADOW_FRAC:.1%})"
            )
        if s.get("rack_brown_object_frac", 0.0) > MAX_RACK_OBJECT_BROWN_FRAC:
            reasons.append(
                f"rack_brown_object_frac={s.get('rack_brown_object_frac', 0):.2%} (max {MAX_RACK_OBJECT_BROWN_FRAC:.0%})"
            )
    if asset_id == "prop-desk":
        if s.get("subject_median_sat", 1.0) < 0.08:
            reasons.append(f"subject_median_sat={s.get('subject_median_sat', 0):.3f}")
        if s.get("desk_top_brown_frac", 0.0) < 0.12:
            reasons.append(f"desk_top_brown_frac={s.get('desk_top_brown_frac', 0):.2%}")
        if s.get("screen_peak_lum", 0.0) < 0.6:
            reasons.append(f"screen_peak_lum={s.get('screen_peak_lum', 0):.2f}")
        if s.get("outside_screen_mean_sat", 0.0) < 0.04:
            reasons.append(f"outside_screen_mean_sat={s.get('outside_screen_mean_sat', 0):.3f}")
        blow_max = 0.026 if asset_id == "prop-desk" else 0.02
        if s.get("blow_frac_outside_screen", 1.0) > blow_max:
            reasons.append(f"blow_frac_outside_screen={s.get('blow_frac_outside_screen', 0):.2%}")
        if s.get("bezel_wood_frac", 0.0) > 0.08:
            reasons.append(f"bezel_wood_frac={s.get('bezel_wood_frac', 0):.2%}")
        dml = s.get("desk_mean_lum", 0.0)
        if dml < 0.30 or dml > 0.62:
            reasons.append(f"desk_mean_lum={dml:.2f} (want 0.30-0.62)")
        if s.get("desk_mean_sat", 0.0) < 0.20:
            reasons.append(f"desk_mean_sat={s.get('desk_mean_sat', 0):.2f}")
        if s.get("dark_subject_cell_frac", 0.0) > 0.25:
            reasons.append(f"dark_subject_cell_frac={s.get('dark_subject_cell_frac', 0):.2%}")
        if s.get("dark_object_frac", 1.0) > 0.05:
            reasons.append(f"dark_object_frac={s.get('dark_object_frac', 0):.2%}")
        if s.get("monitor_skew_deg", 99.0) > 5.0:
            reasons.append(f"monitor_skew_deg={s.get('monitor_skew_deg', 0):.1f}")
        if s.get("plant_green_frac", 0.0) < 0.12:
            reasons.append(f"plant_green_frac={s.get('plant_green_frac', 0):.2%}")
        frame = PREVIEW_DIR / "prop-desk.frame.json"
        glb = BLENDER_ROOT / "prop-desk" / "mesh_prop-desk.glb"
        if glb.is_file():
            sha = hashlib.sha256(glb.read_bytes()).hexdigest()
            if frame.is_file():
                meta = json.loads(frame.read_text())
                if meta.get("glb_sha256") and meta.get("glb_sha256") != sha:
                    reasons.append("preview_frame_glb_sha_mismatch")
    return reasons


def glb_sha256(asset_id: str) -> str | None:
    glb = BLENDER_ROOT / asset_id / f"mesh_{asset_id}.glb"
    if not glb.is_file():
        return None
    return hashlib.sha256(glb.read_bytes()).hexdigest()


def write_frame_json(preview: Path, asset_id: str, s: dict[str, float]) -> None:
    sha = glb_sha256(asset_id)
    frame = PREVIEW_DIR / f"{asset_id}.frame.json"
    frame.write_text(
        json.dumps(
            {
                "assetId": asset_id,
                "preview": str(preview.relative_to(REPO)),
                "glb_sha256": sha,
                "resolution": 1024,
                "quality": s,
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )


def main() -> int:
    failures: list[str] = []
    for aid in GATED_ASSETS:
        png = PREVIEW_DIR / f"{aid}.png"
        if not png.is_file():
            failures.append(f"{aid}: missing preview")
            continue
        s = stats(png, aid)
        write_frame_json(png, aid, s)
        bad = gate_fail(aid, s)
        if bad:
            failures.append(f"{aid}: " + "; ".join(bad))
    for aid, fixture, head in FIXTURES_MUST_FAIL:
        if not fixture.is_file():
            failures.append(f"{head}: missing regression fixture {fixture}")
            continue
        s_fix = stats(fixture, aid)
        bad_fix = gate_fail(aid, s_fix)
        if not bad_fix:
            failures.append(
                f"{head}: fixture {fixture.name} must FAIL gates (dfc65859 regression)"
            )
        else:
            print(
                f"OK: fixture {fixture.name} fails gates as expected ({head}): "
                + "; ".join(bad_fix[:3])
            )
    if failures:
        for line in failures:
            print(f"FAIL preview quality: {line}", file=sys.stderr)
        return 1
    print(
        f"OK: {len(GATED_ASSETS)} gated preview(s) pass uniform/chroma/framing gates"
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())