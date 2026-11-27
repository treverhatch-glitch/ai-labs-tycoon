#!/usr/bin/env python3
"""Staff researcher previews: avoid blown highlights on character (<5% pure white)."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PREVIEWS = (
    REPO / "docs/art/previews/staff-researcher-idle.png",
    REPO / "docs/art/previews/staff-researcher-vs-stock-check.png",
)
MAX_CLIP_FRAC = 0.05
MASK_THRESH = 48


def _clip_frac(png: Path) -> tuple[float, int, int]:
    img = Image.open(png).convert("RGB")
    mask_path = png.with_name(png.stem + ".mask.png")
    if not mask_path.is_file():
        return 1.0, 0, 0
    mask = Image.open(mask_path).convert("L")
    w, h = img.size
    on = clip = 0
    for y in range(h):
        for x in range(w):
            if mask.getpixel((x, y)) <= MASK_THRESH:
                continue
            on += 1
            r, g, b = img.getpixel((x, y))
            if r >= 255 and g >= 255 and b >= 255:
                clip += 1
    frac = (clip / on) if on else 0.0
    return frac, clip, on


def main() -> int:
    rc = 0
    for png in PREVIEWS:
        if not png.is_file():
            print(f"FAIL staff-researcher-idle preview_clip missing {png.name}")
            rc = 1
            continue
        frac, clip, on = _clip_frac(png)
        if frac >= MAX_CLIP_FRAC:
            print(f"FAIL staff-researcher-idle preview_clip {png.name} white_frac={frac:.3f} px={clip}/{on}")
            rc = 1
        else:
            print(f"OK staff-researcher-idle preview_clip {png.name} white_frac={frac:.3f}")
    return rc


if __name__ == "__main__":
    sys.exit(main())