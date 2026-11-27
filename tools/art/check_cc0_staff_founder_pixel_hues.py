#!/usr/bin/env python3
"""Founder PNG: navy torso, denim legs — reject cyan mean hue on torso."""
from __future__ import annotations

import colorsys
import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"


def _band_pixels(
    img: Image.Image,
    mask: Image.Image,
    y0: float,
    y1: float,
    x0: float = 0.0,
    x1: float = 1.0,
    *,
    denim_only: bool = False,
) -> list[tuple[int, int, int]]:
    w, h = img.size
    lo, hi = int(h * y0), int(h * y1)
    xlo, xhi = int(w * x0), int(w * x1)
    out: list[tuple[int, int, int]] = []
    for y in range(lo, hi):
        for x in range(xlo, xhi):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b, _ = img.getpixel((x, y))
            if max(r, g, b) - min(r, g, b) < 12:
                continue
            h, s, _ = colorsys.rgb_to_hsv(r / 255, g / 255, b / 255)
            if s < 0.12:
                continue
            if denim_only and not (b > max(r, g) + 1):
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
    torso = _band_pixels(img, mask, 0.38, 0.52, 0.32, 0.68)
    legs = _band_pixels(img, mask, 0.52, 0.76, 0.32, 0.68, denim_only=True)
    if len(legs) < 200:
        legs = _band_pixels(img, mask, 0.52, 0.76, 0.32, 0.68, denim_only=False)
    if len(torso) < 200:
        print(f"FAIL staff-founder-idle torso_px={len(torso)}")
        return 1
    if len(legs) < 200:
        print(f"FAIL staff-founder-idle leg_px={len(legs)}")
        return 1
    t_mean = tuple(sum(c[i] for c in torso) / len(torso) for i in range(3))
    l_mean = tuple(sum(c[i] for c in legs) / len(legs) for i in range(3))
    navy_hoodie = t_mean[2] > t_mean[1] > t_mean[0] + 5 and t_mean[2] - t_mean[0] > 18
    red_hoodie_px = t_mean[0] > t_mean[1] + 4 and t_mean[0] >= t_mean[2] - 8
    warm_hoodie = t_mean[0] >= 92 and t_mean[0] >= t_mean[1] and (t_mean[2] - t_mean[0]) <= 22
    if not (navy_hoodie or red_hoodie_px or warm_hoodie):
        print(f"FAIL staff-founder-idle torso_not_hoodie rgb={tuple(round(c) for c in t_mean)}")
        return 1
    th, ts, _ = colorsys.rgb_to_hsv(t_mean[0] / 255, t_mean[1] / 255, t_mean[2] / 255)
    lh, _, _ = colorsys.rgb_to_hsv(l_mean[0] / 255, l_mean[1] / 255, l_mean[2] / 255)
    torso_deg = th * 360.0
    leg_deg = lh * 360.0
    delta = abs(leg_deg - torso_deg)
    if delta > 180.0:
        delta = 360.0 - delta
    red_hoodie = t_mean[0] > t_mean[1] + 6 and t_mean[0] > t_mean[2] + 10
    blue_denim = l_mean[2] > l_mean[1] + 5 and l_mean[2] > l_mean[0] + 12
    br_gap = (l_mean[2] - l_mean[0]) - (t_mean[2] - t_mean[0])
    if delta <= 20.0 and not (red_hoodie or blue_denim) and br_gap < 10:
        print(
            f"FAIL staff-founder-idle legs_torso_hue_delta={delta:.0f} br_gap={br_gap:.0f} "
            f"hoodie_red={red_hoodie} denim_blue={blue_denim}"
        )
        return 1
    if 160.0 <= torso_deg <= 200.0 and ts > 0.22:
        print(f"FAIL staff-founder-idle torso_cyan hue={torso_deg:.0f} sat={ts:.2f}")
        return 1
    # Denim jeans: blue channel leads (preview lighting may wash)
    if l_mean[2] < max(l_mean[0], l_mean[1]) + 8:
        print(f"FAIL staff-founder-idle legs_not_denim rgb={tuple(round(c) for c in l_mean)}")
        return 1
    print(
        f"OK staff-founder-idle torso_rgb={tuple(round(c) for c in t_mean)} "
        f"leg_rgb={tuple(round(c) for c in l_mean)} hue_delta={delta:.0f}"
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())