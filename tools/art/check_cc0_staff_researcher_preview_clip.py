#!/usr/bin/env python3
"""Staff researcher previews: avoid blown highlights on character (<5% pure white)."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "tools" / "art"))
from staff_preview_eye_head_gate import figure_span  # noqa: E402
PREVIEWS = (
    REPO / "docs/art/previews/staff-researcher-idle.png",
    REPO / "docs/art/previews/staff-researcher-vs-stock-check.png",
)
MAX_CLIP_FRAC = 0.05
MAX_CLIP_FRAC_HARD = 0.0001  # idle must show visible shading, not flat clip
MASK_THRESH = 48


def _clip_frac(png: Path) -> tuple[float, int, int]:
    img = Image.open(png).convert("RGB")
    mask_path = png.with_name(png.stem + ".mask.png")
    if not mask_path.is_file():
        return 1.0, 0, 0
    mask = Image.open(mask_path).convert("L")
    w, h = img.size
    min_y, max_y, span = figure_span(mask)
    y_hi = max_y - max(2, int(span * 0.04)) if span > 20 else h
    on = clip = 0
    for y in range(min_y if span > 20 else 0, min(h, y_hi)):
        for x in range(w):
            if mask.getpixel((x, y)) <= MASK_THRESH:
                continue
            on += 1
            r, g, b = img.getpixel((x, y))
            if r >= 255 and g >= 255 and b >= 255:
                clip += 1
    frac = (clip / on) if on else 0.0
    return frac, clip, on


def _clip_frac_panel(png: Path, *, x0: int, x1: int) -> tuple[float, int, int]:
    img = Image.open(png).convert("RGB")
    mask_path = png.with_name(png.stem + ".mask.png")
    if not mask_path.is_file():
        return 1.0, 0, 0
    mask = Image.open(mask_path).convert("L")
    w, h = img.size
    on = clip = 0
    x0 = max(0, min(x0, w))
    x1 = max(x0 + 1, min(x1, w))
    for y in range(h):
        for x in range(x0, x1):
            if mask.getpixel((x, y)) <= MASK_THRESH:
                continue
            on += 1
            r, g, b = img.getpixel((x, y))
            if r >= 255 and g >= 255 and b >= 255:
                clip += 1
    frac = (clip / on) if on else 1.0
    return frac, clip, on


def main() -> int:
    rc = 0
    idle = PREVIEWS[0]
    if not idle.is_file():
        print(f"FAIL staff-researcher-idle preview_clip missing {idle.name}")
        return 1
    frac, clip, on = _clip_frac(idle)
    cap = MAX_CLIP_FRAC
    if frac >= cap:
        print(f"FAIL staff-researcher-idle preview_clip {idle.name} white_frac={frac:.3f} px={clip}/{on}")
        rc = 1
    else:
        print(f"OK staff-researcher-idle preview_clip {idle.name} white_frac={frac:.3f}")

    vs = PREVIEWS[1]
    if vs.is_file():
        w = Image.open(vs).size[0]
        gap = 8
        frac2, clip2, on2 = _clip_frac_panel(vs, x0=w // 2 + gap, x1=w)
        if on2 == 0:
            frac2, clip2, on2 = _clip_frac(vs)
        if frac2 >= MAX_CLIP_FRAC:
            print(f"FAIL staff-researcher-idle preview_clip {vs.name} white_frac={frac2:.3f} px={clip2}/{on2}")
            rc = 1
        else:
            print(f"OK staff-researcher-idle preview_clip {vs.name} white_frac={frac2:.3f}")
    else:
        print(f"FAIL staff-researcher-idle preview_clip missing {vs.name}")
        rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())