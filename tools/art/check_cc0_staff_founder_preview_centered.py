#!/usr/bin/env python3
"""Founder preview: figure mask centroid near horizontal center."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"


def main() -> int:
    mask_path = PNG.with_name(PNG.stem + ".mask.png")
    if not mask_path.is_file():
        print("FAIL staff-founder-idle missing mask")
        return 1
    mask = Image.open(mask_path).convert("L")
    w, h = mask.size
    xs: list[int] = []
    for y in range(h):
        for x in range(w):
            if mask.getpixel((x, y)) > 48:
                xs.append(x)
    if not xs:
        print("FAIL staff-founder-idle empty_mask")
        return 1
    cx = sum(xs) / len(xs)
    off = abs(cx - w * 0.5) / w
    if off > 0.08:
        print(f"FAIL staff-founder-idle mask_off_center off={off:.3f}")
        return 1
    print(f"OK staff-founder-idle mask_center_off={off:.3f}")
    return 0


if __name__ == "__main__":
    sys.exit(main())