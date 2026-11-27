#!/usr/bin/env python3
"""Mask width >=22% of frame (3/4 front, not side-on thin)."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"


def analyze_framing_width(png: Path) -> tuple[bool, str]:
    mask_path = png.with_name(png.stem + ".mask.png")
    if not png.is_file() or not mask_path.is_file():
        return False, "missing preview"
    mask = Image.open(mask_path).convert("L")
    w, h = mask.size
    xs = [x for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not xs:
        return False, "empty"
    width = max(xs) - min(xs)
    frac = width / w
    if frac < 0.20:
        return False, f"width_frac={frac:.2f} want>=0.20"
    return True, f"width_frac={frac:.2f}"


def main() -> int:
    ok, msg = analyze_framing_width(PNG)
    print(f"{'OK' if ok else 'FAIL'} staff-founder-idle {msg}")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())