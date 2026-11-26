#!/usr/bin/env python3
"""Hero preview backdrop corners must be studio grey ~150/255 (±2)."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
AID = "staff-founder-idle"
PNG = REPO / "docs/art/previews" / f"{AID}.png"
TARGET = 150
TOL = 2


def main() -> int:
    if not PNG.is_file():
        print(f"FAIL {AID} missing preview")
        return 1
    img = Image.open(PNG).convert("RGB")
    w, h = img.size
    corners = [(3, 3), (w - 4, 3), (3, h - 4), (w - 4, h - 4)]
    bad = []
    for x, y in corners:
        r, g, b = img.getpixel((x, y))
        if not (TARGET - TOL <= r <= TARGET + TOL and TARGET - TOL <= g <= TARGET + TOL and TARGET - TOL <= b <= TARGET + TOL):
            bad.append((x, y, (r, g, b)))
    if bad:
        print(f"FAIL {AID} corner_rgb {bad}")
        return 1
    print(f"OK {AID} corner_grey_150")
    return 0


if __name__ == "__main__":
    sys.exit(main())