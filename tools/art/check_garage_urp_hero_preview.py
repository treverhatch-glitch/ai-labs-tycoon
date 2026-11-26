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


def _parse_png(path: Path) -> tuple[int, int, int, bytes]:
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
    return w, h, color_type, zlib.decompress(idat)


def _magenta_fraction(path: Path) -> float:
    w, h, color_type, data = _parse_png(path)
    if color_type != 2:
        raise ValueError("hero must be RGB PNG")
    bpp = 3
    hit = total = 0
    off = 0
    for _ in range(h):
        off += 1
        row = data[off : off + w * bpp]
        off += w * bpp
        for i in range(0, len(row), 3):
            r, g, b = row[i], row[i + 1], row[i + 2]
            total += 1
            if r >= MAGENTA_MIN_R and g <= MAGENTA_MAX_G and b >= MAGENTA_MIN_B:
                hit += 1
    return hit / max(1, total)


def _subject_extent_from_hero(hero: Path) -> float:
    """Framing gate on hero RGB: non-grey pixels (3/4 garage vs studio gradient)."""
    w, h, color_type, data = _parse_png(hero)
    if color_type != 2:
        raise ValueError("hero must be RGB PNG")
    xs: list[int] = []
    ys: list[int] = []
    pad_x = max(8, int(w * 0.04))
    pad_y = max(8, int(h * 0.04))
    off = 0
    for y in range(h):
        off += 1
        row = data[off : off + w * 3]
        off += w * 3
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


def _mean_rgb(path: Path) -> tuple[float, float, float]:
    w, h, color_type, data = _parse_png(path)
    if color_type != 2:
        raise ValueError("hero must be RGB PNG")
    rs = gs = bs = 0
    n = w * h
    off = 0
    for _ in range(h):
        off += 1
        row = data[off : off + w * 3]
        off += w * 3
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
    if errs:
        for e in errs:
            print(f"FAIL: {e}", file=sys.stderr)
        return 1
    print(f"OK garage-urp-hero magenta_frac={mag:.4f}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())