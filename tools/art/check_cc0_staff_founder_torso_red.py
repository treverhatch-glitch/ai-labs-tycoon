#!/usr/bin/env python3
"""Founder preview: front torso band >=80% red-ish (flat shirt, no denim bleed)."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"
MIN_RED_FRAC = 0.80


def _is_redish(r: int, g: int, b: int) -> bool:
    return r > g + 10 and r > b + 10 and r >= 100


def analyze(png: Path) -> tuple[bool, str]:
    mask_path = png.with_name(png.stem + ".mask.png")
    if not png.is_file() or not mask_path.is_file():
        return False, "missing preview"
    img = Image.open(png).convert("RGB")
    mask = Image.open(mask_path).convert("L")
    w, h = img.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return False, "empty"
    min_y, max_y = min(ys), max(ys)
    span = max(1, max_y - min_y)
    y0, y1 = min_y + int(span * 0.36), min_y + int(span * 0.50)
    x0, x1 = int(w * 0.38), int(w * 0.62)
    red = total = 0
    for y in range(y0, y1):
        for x in range(x0, x1):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))
            total += 1
            if _is_redish(r, g, b):
                red += 1
    if total < 80:
        return False, f"torso_front_px={total}"
    frac = red / total
    if frac < MIN_RED_FRAC:
        return False, f"torso_red_frac={frac:.2f} want>={MIN_RED_FRAC}"
    return True, f"torso_red_frac={frac:.2f}"


def main() -> int:
    ok, msg = analyze(PNG)
    print(f"{'OK' if ok else 'FAIL'} staff-founder-idle {msg}")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())