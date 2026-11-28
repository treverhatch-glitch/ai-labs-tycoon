#!/usr/bin/env python3
"""Fail hero staff previews when >80% masked pixels share one quantized colour."""
from __future__ import annotations

import sys
from collections import Counter
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
HERO_IDS = ("staff-founder-idle",)
PREVIEW = REPO / "docs/art/previews"
MAX_DOMINANT = 0.80
BIN = 28


def _character_pixels(png: Path, mask: Path | None) -> list[tuple[int, int, int]]:
    img = Image.open(png).convert("RGBA")
    mask_img = Image.open(mask).convert("L") if mask and mask.is_file() else None
    out: list[tuple[int, int, int]] = []
    w, h = img.size
    for y in range(h):
        for x in range(w):
            r, g, b, a = img.getpixel((x, y))
            alpha = mask_img.getpixel((x, y)) if mask_img else a
            if alpha <= 48:
                continue
            if r > 235 and g > 235 and b > 235:
                continue
            if 0.42 < r / 255 < 0.54 and 0.42 < g / 255 < 0.54 and 0.44 < b / 255 < 0.56:
                continue
            out.append((r, g, b))
    return out


def analyze(png: Path) -> tuple[bool, str]:
    mask = png.with_name(png.stem + ".mask.png")
    pixels = _character_pixels(png, mask)
    if len(pixels) < 200:
        return False, f"too_few_pixels={len(pixels)}"
    buckets = Counter((p[0] // BIN, p[1] // BIN, p[2] // BIN) for p in pixels)
    key, count = buckets.most_common(1)[0]
    share = count / len(pixels)
    if share > MAX_DOMINANT:
        return False, f"dominant={share:.1%} bucket={key} n={len(pixels)}"
    return True, f"dominant={share:.1%} colors={len(buckets)} n={len(pixels)}"


def main() -> int:
    rc = 0
    for aid in HERO_IDS:
        png = PREVIEW / f"{aid}.png"
        if not png.is_file():
            print(f"FAIL {aid} missing preview")
            rc = 1
            continue
        ok, msg = analyze(png)
        if ok:
            print(f"OK {aid} {msg}")
        else:
            print(f"FAIL {aid} {msg}")
            rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())