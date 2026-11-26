#!/usr/bin/env python3
"""Founder PNG: teal torso band, denim leg band (rendered pixels, not GLB metadata)."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"


def _band_pixels(img: Image.Image, mask: Image.Image, y0: float, y1: float) -> list[tuple[int, int, int]]:
    w, h = img.size
    lo, hi = int(h * y0), int(h * y1)
    out: list[tuple[int, int, int]] = []
    for y in range(lo, hi):
        for x in range(w):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b, _ = img.getpixel((x, y))
            if max(r, g, b) - min(r, g, b) < 12:
                continue
            out.append((r, g, b))
    return out


def main() -> int:
    mask_path = PNG.with_name(PNG.stem + ".mask.png")
    if not PNG.is_file() or not mask_path.is_file():
        print("FAIL staff-founder-idle missing preview")
        return 1
    img = Image.open(PNG).convert("RGBA")
    mask = Image.open(mask_path).convert("L")
    torso = _band_pixels(img, mask, 0.38, 0.58)
    legs = _band_pixels(img, mask, 0.58, 0.82)
    if len(torso) < 200:
        print(f"FAIL staff-founder-idle torso_px={len(torso)}")
        return 1
    if len(legs) < 200:
        print(f"FAIL staff-founder-idle leg_px={len(legs)}")
        return 1
    t_mean = tuple(sum(c[i] for c in torso) / len(torso) for i in range(3))
    l_mean = tuple(sum(c[i] for c in legs) / len(legs) for i in range(3))
    # Teal: G and B dominate over R
    if not (t_mean[1] > t_mean[0] + 8 and t_mean[2] > t_mean[0] + 8):
        print(f"FAIL staff-founder-idle torso_not_teal rgb={tuple(round(c) for c in t_mean)}")
        return 1
    # Denim: B highest, G mid
    if not (l_mean[2] > l_mean[1] > l_mean[0]):
        print(f"FAIL staff-founder-idle legs_not_denim rgb={tuple(round(c) for c in l_mean)}")
        return 1
    print(f"OK staff-founder-idle torso_rgb={tuple(round(c) for c in t_mean)} leg_rgb={tuple(round(c) for c in l_mean)}")
    return 0


if __name__ == "__main__":
    sys.exit(main())