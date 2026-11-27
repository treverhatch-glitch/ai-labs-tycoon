#!/usr/bin/env python3
"""Hero preview backdrop corners must be studio grey ~150/255 (±2)."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
HERO_IDS = ("staff-founder-idle", "staff-researcher-idle")
TARGET = 150
TOL = 2


def _check(aid: str) -> tuple[bool, str]:
    png = REPO / "docs/art/previews" / f"{aid}.png"
    if not png.is_file():
        return False, f"missing preview"
    img = Image.open(png).convert("RGB")
    w, h = img.size
    corners = [(3, 3), (w - 4, 3), (3, h - 4), (w - 4, h - 4)]
    bad = []
    for x, y in corners:
        r, g, b = img.getpixel((x, y))
        if not (
            TARGET - TOL <= r <= TARGET + TOL
            and TARGET - TOL <= g <= TARGET + TOL
            and TARGET - TOL <= b <= TARGET + TOL
        ):
            bad.append((x, y, (r, g, b)))
    if bad:
        return False, f"corner_rgb {bad}"
    return True, "corner_grey_150"


def main() -> int:
    rc = 0
    for aid in HERO_IDS:
        ok, msg = _check(aid)
        if ok:
            print(f"OK {aid} {msg}")
        else:
            print(f"FAIL {aid} {msg}")
            rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())