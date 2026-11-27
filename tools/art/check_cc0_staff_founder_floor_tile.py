#!/usr/bin/env python3
"""Reject bright white floor tile under feet; background stays grey ~150."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"


def main() -> int:
    if not PNG.is_file():
        print("FAIL staff-founder-idle missing preview")
        return 1
    img = Image.open(PNG).convert("RGB")
    w, h = img.size
    white = 0
    grey = 0
    for y in range(h - 48, h - 4):
        for x in range(w // 4, (3 * w) // 4):
            r, g, b = img.getpixel((x, y))
            lum = max(r, g, b)
            if lum >= 248 and max(r, g, b) - min(r, g, b) < 8:
                white += 1
            if 142 <= r <= 158 and 142 <= g <= 158 and 142 <= b <= 158:
                grey += 1
    if white > 120:
        print(f"FAIL staff-founder-idle white_floor_tile n={white}")
        return 1
    if grey < 40:
        print(f"FAIL staff-founder-idle floor_not_grey grey={grey}")
        return 1
    print(f"OK staff-founder-idle floor_grey_only white={white} grey={grey}")
    return 0


if __name__ == "__main__":
    sys.exit(main())