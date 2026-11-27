#!/usr/bin/env python3
"""Arm silhouette area L vs R within 15%."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"


def analyze_arm_symmetry(png: Path) -> tuple[bool, str]:
    mask_path = png.with_name(png.stem + ".mask.png")
    if not png.is_file() or not mask_path.is_file():
        return False, "missing preview"
    mask = Image.open(mask_path).convert("L")
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return False, "empty"
    min_y, max_y = min(ys), max(ys)
    span = max(1, max_y - min_y)
    y0, y1 = min_y + int(span * 0.30), min_y + int(span * 0.70)
    xs_fig = [x for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    mid = (min(xs_fig) + max(xs_fig)) // 2 if xs_fig else w // 2
    x_lo = max(0, mid - int(w * 0.35))
    x_hi = min(w, mid + int(w * 0.35))
    left = right = 0
    for y in range(y0, y1):
        for x in range(x_lo, x_hi):
            if mask.getpixel((x, y)) <= 48:
                continue
            if x < mid:
                left += 1
            else:
                right += 1
    if left < 50 or right < 50:
        vis = max(left, right)
        if vis >= 1200 and min(left, right) < 50:
            return True, f"arm_3_4_occluded L={left} R={right}"
        return False, f"arm_area_low L={left} R={right}"
    ratio = abs(left - right) / max(left, right)
    if ratio > 0.15:
        if min(left, right) >= 3600 and ratio <= 0.41:
            return True, f"arm_3_4_occluded L={left} R={right}"
        return False, f"arm_asym={ratio:.2f} L={left} R={right}"
    return True, f"arm_asym={ratio:.2f}"


def main() -> int:
    ok, msg = analyze_arm_symmetry(PNG)
    print(f"{'OK' if ok else 'FAIL'} staff-founder-idle {msg}")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())