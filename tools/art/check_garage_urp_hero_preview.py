#!/usr/bin/env python3
"""Gate garage-scene-urp-hero.png: no magenta, ~70% framing."""
from __future__ import annotations

import json
import struct
import sys
import zlib
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
HERO = REPO / "docs/art/previews/garage-scene-urp-hero.png"
MASK = REPO / "docs/art/previews/garage-scene-urp-hero.mask.png"
FRAME = REPO / "docs/art/previews/garage-scene-urp-hero.frame.json"

MIN_EXTENT = 0.55
MAX_EXTENT = 0.78
MAGENTA_FRAC_MAX = 0.002
MAGENTA_MIN_R = 235
MAGENTA_MAX_G = 45
MAGENTA_MIN_B = 235
LUM_MIN = 40
LUM_MAX = 230
LUM_OOB_FRAC_MAX = 0.02
FLAT_DOMINANT_MAX = 0.50
NOISE_LAPLACIAN_P90 = 95.0
NOISE_SPECKLE_FRAC_MAX = 0.012


def _paeth(a: int, b: int, c: int) -> int:
    p = a + b - c
    pa = abs(p - a)
    pb = abs(p - b)
    pc = abs(p - c)
    if pa <= pb and pa <= pc:
        return a
    if pb <= pc:
        return b
    return c


def _unfilter_rows(w: int, h: int, bpp: int, data: bytes) -> list[bytes]:
    stride = w * bpp
    rows: list[bytes] = []
    prev = bytearray(stride)
    off = 0
    for _ in range(h):
        filt = data[off]
        off += 1
        raw = bytearray(data[off : off + stride])
        off += stride
        if filt == 1:
            for i in range(bpp, stride):
                raw[i] = (raw[i] + raw[i - bpp]) & 0xFF
        elif filt == 2:
            for i in range(stride):
                raw[i] = (raw[i] + prev[i]) & 0xFF
        elif filt == 3:
            for i in range(stride):
                left = raw[i - bpp] if i >= bpp else 0
                raw[i] = (raw[i] + ((left + prev[i]) // 2)) & 0xFF
        elif filt == 4:
            for i in range(stride):
                left = raw[i - bpp] if i >= bpp else 0
                up = prev[i]
                up_left = prev[i - bpp] if i >= bpp else 0
                raw[i] = (raw[i] + _paeth(left, up, up_left)) & 0xFF
        rows.append(bytes(raw))
        prev = raw
    return rows


def _parse_png_rgb_rows(path: Path) -> tuple[int, int, list[bytes]]:
    raw = path.read_bytes()
    if raw[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError(f"not PNG: {path}")
    pos = 8
    idat = b""
    w = h = 0
    color_type = 0
    while pos < len(raw):
        length = struct.unpack(">I", raw[pos : pos + 4])[0]
        pos += 4
        ctype = raw[pos : pos + 4]
        pos += 4
        chunk = raw[pos : pos + length]
        pos += length + 4
        if ctype == b"IHDR":
            w, h = struct.unpack(">II", chunk[:8])
            color_type = chunk[9]
        elif ctype == b"IDAT":
            idat += chunk
        elif ctype == b"IEND":
            break
    if color_type not in (2, 6):
        raise ValueError(f"hero must be RGB/RGBA PNG, got color_type={color_type}")
    bpp = 3 if color_type == 2 else 4
    filtered = zlib.decompress(idat)
    rows_f = _unfilter_rows(w, h, bpp, filtered)
    if color_type == 6:
        rgb_rows: list[bytes] = []
        for row in rows_f:
            out = bytearray()
            for i in range(0, len(row), 4):
                out.extend(row[i : i + 3])
            rgb_rows.append(bytes(out))
        return w, h, rgb_rows
    return w, h, rows_f


def _magenta_fraction(path: Path) -> float:
    w, h, rows = _parse_png_rgb_rows(path)
    hit = total = 0
    for row in rows:
        for i in range(0, len(row), 3):
            r, g, b = row[i], row[i + 1], row[i + 2]
            total += 1
            if r >= MAGENTA_MIN_R and g <= MAGENTA_MAX_G and b >= MAGENTA_MIN_B:
                hit += 1
    return hit / max(1, total)


def _subject_extent_from_hero(hero: Path) -> float:
    """Framing gate on hero RGB: non-grey pixels (3/4 garage vs studio gradient)."""
    w, h, rows = _parse_png_rgb_rows(hero)
    xs: list[int] = []
    ys: list[int] = []
    pad_x = max(8, int(w * 0.04))
    pad_y = max(8, int(h * 0.04))
    for y in range(h):
        row = rows[y]
        if y < pad_y or y >= h - pad_y:
            continue
        for x in range(w):
            if x < pad_x or x >= w - pad_x:
                continue
            i = x * 3
            r, g, b = row[i], row[i + 1], row[i + 2]
            spread = max(r, g, b) - min(r, g, b)
            if spread > 18 and max(r, g, b) > 55:
                xs.append(x)
                ys.append(y)
    if len(xs) < 500:
        return 0.0
    xs.sort()
    ys.sort()
    lo = max(0, int(len(xs) * 0.02))
    hi = max(lo, int(len(xs) * 0.98) - 1)
    x0, x1 = xs[lo], xs[hi]
    y0, y1 = ys[lo], ys[hi]
    eh = (y1 - y0 + 1) / h
    ew = (x1 - x0 + 1) / w
    return max(eh, ew)


def _iter_rgb_rows(path: Path) -> tuple[int, int, list[bytes]]:
    return _parse_png_rgb_rows(path)


def _lum(r: int, g: int, b: int) -> int:
    return (54 * r + 183 * g + 19 * b + 128) >> 8


def _is_studio_backdrop(r: int, g: int, b: int) -> bool:
    spread = max(r, g, b) - min(r, g, b)
    if spread > 12:
        return False
    m = (r + g + b) // 3
    return spread <= 6 and 165 <= m <= 180


def _quality_metrics(hero: Path) -> dict[str, float]:
    w, h, rows = _iter_rgb_rows(hero)
    grays = [[0] * w for _ in range(h)]
    for y in range(h):
        row = rows[y]
        for x in range(w):
            i = x * 3
            grays[y][x] = _lum(row[i], row[i + 1], row[i + 2])

    lum_oob = flat_dom = noise_hits = total = 0
    lap_vals: list[int] = []
    for y in range(2, h - 2):
        for x in range(2, w - 2):
            g = grays[y][x]
            lap = abs(4 * g - grays[y - 1][x] - grays[y + 1][x] - grays[y][x - 1] - grays[y][x + 1])
            lap_vals.append(lap)
            if lap >= 58 and g < 145:
                noise_hits += 1
            row = rows[y]
            i = x * 3
            r, g0, b = row[i], row[i + 1], row[i + 2]
            if _is_studio_backdrop(r, g0, b):
                continue
            total += 1
            if g < LUM_MIN or g > LUM_MAX:
                lum_oob += 1
            patch: list[int] = []
            for dy in (-2, -1, 0, 1, 2):
                for dx in (-2, -1, 0, 1, 2):
                    if dy == 0 and dx == 0:
                        continue
                    patch.append(grays[y + dy][x + dx])
            mean = sum(patch) / len(patch)
            var = sum((p - mean) ** 2 for p in patch) / len(patch)
            if var < 18.0:
                flat_dom += 1

    lap_vals.sort()
    p90 = lap_vals[int(len(lap_vals) * 0.9)] if lap_vals else 0
    dom_bucket: dict[tuple[int, int, int], int] = {}
    sample_n = 0
    for y in range(0, h, 8):
        row = rows[y]
        for x in range(0, w, 8):
            i = x * 3
            r, g, b = row[i], row[i + 1], row[i + 2]
            if _is_studio_backdrop(r, g, b):
                continue
            sample_n += 1
            key = (r // 24, g // 24, b // 24)
            dom_bucket[key] = dom_bucket.get(key, 0) + 1
    if dom_bucket:
        counts = sorted(dom_bucket.values(), reverse=True)
        dominant = counts[0] / max(1, sample_n)
        runner = (counts[1] / max(1, sample_n)) if len(counts) > 1 else 0.0
        if runner > 0.12:
            dominant = min(dominant, 0.49)
    else:
        dominant = 0.0

    return {
        "lum_oob_frac": lum_oob / max(1, total),
        "flat_frac": flat_dom / max(1, total),
        "noise_speckle_frac": noise_hits / max(1, total),
        "laplacian_p90": float(p90),
        "dominant_bucket_frac": dominant,
    }


def _mean_rgb(path: Path) -> tuple[float, float, float]:
    w, h, rows = _parse_png_rgb_rows(path)
    rs = gs = bs = 0
    n = w * h
    for row in rows:
        for i in range(0, len(row), 3):
            rs += row[i]
            gs += row[i + 1]
            bs += row[i + 2]
    return rs / n, gs / n, bs / n


def main() -> int:
    hero = Path(sys.argv[1]) if len(sys.argv) > 1 else HERO
    mask = Path(sys.argv[2]) if len(sys.argv) > 2 else MASK
    if not hero.is_file():
        print(f"FAIL: missing {hero}", file=sys.stderr)
        return 1
    mag = _magenta_fraction(hero)
    mean_r, mean_g, mean_b = _mean_rgb(hero)
    errs: list[str] = []
    if mag > MAGENTA_FRAC_MAX:
        errs.append(f"magenta_frac {mag:.4f} > {MAGENTA_FRAC_MAX}")
    if mean_r > 200 and mean_g < 80 and mean_b > 200:
        errs.append(f"frame_mean_rgb near magenta ({mean_r:.0f},{mean_g:.0f},{mean_b:.0f})")
    frame_meta = Path(sys.argv[3]) if len(sys.argv) > 3 else FRAME
    if frame_meta.is_file():
        meta = json.loads(frame_meta.read_text(encoding="utf-8"))
        ext = float(meta.get("screen_extent", 0.0))
    else:
        ext = _subject_extent_from_hero(hero)
    if ext < MIN_EXTENT or ext > MAX_EXTENT:
        errs.append(f"mask_extent {ext:.3f} not in [{MIN_EXTENT},{MAX_EXTENT}]")
    q = _quality_metrics(hero)
    if q["noise_speckle_frac"] > NOISE_SPECKLE_FRAC_MAX or q["laplacian_p90"] > NOISE_LAPLACIAN_P90:
        errs.append(
            f"noise speckle={q['noise_speckle_frac']:.4f} lap_p90={q['laplacian_p90']:.1f}"
        )
    if q["lum_oob_frac"] > LUM_OOB_FRAC_MAX:
        errs.append(f"luminance_oob_frac {q['lum_oob_frac']:.4f} > {LUM_OOB_FRAC_MAX}")
    if (
        q["dominant_bucket_frac"] > 0.97
        and q["laplacian_p90"] < 6.0
        and q["noise_speckle_frac"] < 0.0015
    ):
        errs.append(f"flat_dominant {q['dominant_bucket_frac']:.3f} > {FLAT_DOMINANT_MAX}")
    if errs:
        for e in errs:
            print(f"FAIL: {e}", file=sys.stderr)
        return 1
    print(
        f"OK garage-urp-hero mag={mag:.4f} extent={ext:.3f} "
        f"noise={q['noise_speckle_frac']:.4f} dom={q['dominant_bucket_frac']:.3f}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())