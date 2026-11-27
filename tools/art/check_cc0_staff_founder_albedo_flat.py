#!/usr/bin/env python3
"""Flat material colours: low hue spread within shirt/jeans/skin clusters in preview."""
from __future__ import annotations

import colorsys
import math
import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"
MAX_HUE_STDDEV = 25.0
MIN_CLUSTER_PX = 200


def _hue_stddev(pixels: list[tuple[int, int, int]]) -> float:
    hues: list[float] = []
    for r, g, b in pixels:
        h, s, _v = colorsys.rgb_to_hsv(r / 255, g / 255, b / 255)
        if s < 0.06:
            continue
        hues.append(h * 360.0)
    if len(hues) < 40:
        return 0.0
    mean = sum(hues) / len(hues)
    var = sum((h - mean) ** 2 for h in hues) / len(hues)
    return math.sqrt(var)


def _masked_pixels(img: Image.Image, mask: Image.Image) -> list[tuple[int, int, int]]:
    w, h = img.size
    out: list[tuple[int, int, int]] = []
    for y in range(h):
        for x in range(w):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))
            if max(r, g, b) < 20:
                continue
            out.append((r, g, b))
    return out


def main() -> int:
    mask_path = PNG.with_name(PNG.stem + ".mask.png")
    if not PNG.is_file() or not mask_path.is_file():
        print("FAIL staff-founder-idle missing_preview")
        return 1
    img = Image.open(PNG).convert("RGB")
    mask = Image.open(mask_path).convert("L")
    px = _masked_pixels(img, mask)
    if len(px) < 800:
        print(f"FAIL staff-founder-idle flat_colors sparse={len(px)}")
        return 1

    shirts = [p for p in px if p[0] > p[1] + 8 and p[0] > p[2] + 8]
    jeans = [p for p in px if p[2] >= p[0] + 4 and p[2] > p[1] + 2]
    skins = [p for p in px if p[0] > p[1] and p[0] > p[2] and p[0] - p[1] < 35]

    checks = (
        ("shirt", shirts),
        ("jeans", jeans),
        ("skin", skins),
    )
    worst = 0.0
    worst_name = ""
    for name, cluster in checks:
        if len(cluster) < MIN_CLUSTER_PX:
            print(f"FAIL staff-founder-idle flat_{name}_cluster={len(cluster)}")
            return 1
        sd = _hue_stddev(cluster)
        if sd > worst:
            worst = sd
            worst_name = name
        if sd > MAX_HUE_STDDEV:
            print(f"FAIL staff-founder-idle flat_{name}_hue_std={sd:.1f} max={MAX_HUE_STDDEV}")
            return 1
    print(f"OK staff-founder-idle albedo_flat worst={worst_name} hue_std={worst:.1f}")
    return 0


if __name__ == "__main__":
    sys.exit(main())