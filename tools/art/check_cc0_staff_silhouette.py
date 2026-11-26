#!/usr/bin/env python3
"""Hero staff preview silhouette: head in frame, front-ish width, ~70% fill, role colors."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
HERO_IDS = ("staff-founder-idle", "staff-researcher-idle")
PREVIEW = REPO / "docs/art/previews"
MIN_FILL = 0.58
MAX_FILL = 0.78
MIN_FRONT_WIDTH_RATIO = 0.24


def _masked_pixels(img: Image.Image, mask: Image.Image | None) -> list[tuple[int, int, int]]:
    w, h = img.size
    out: list[tuple[int, int, int]] = []
    for y in range(h):
        for x in range(w):
            r, g, b, a = img.getpixel((x, y))
            alpha = mask.getpixel((x, y)) if mask else a
            if alpha <= 48:
                continue
            if 0.42 < r / 255 < 0.54 and 0.42 < g / 255 < 0.54:
                continue
            out.append((x, y, (r, g, b)))
    return out


def analyze(aid: str, png: Path) -> tuple[bool, str]:
    mask_path = png.with_name(png.stem + ".mask.png")
    img = Image.open(png).convert("RGBA")
    mask = Image.open(mask_path).convert("L") if mask_path.is_file() else None
    pts = _masked_pixels(img, mask)
    if len(pts) < 400:
        return False, f"too_few_pixels={len(pts)}"
    h = img.size[1]
    ys = [p[1] for p in pts]
    min_y, max_y = min(ys), max(ys)
    span = max(1, max_y - min_y)
    fill = span / h
    if fill < MIN_FILL or fill > MAX_FILL:
        return False, f"fill={fill:.2f} want {MIN_FILL}-{MAX_FILL}"
    top_cut = max_y - span * 0.18
    head_pts = [p for p in pts if p[1] >= top_cut]
    if len(head_pts) < span * 0.35:
        return False, f"head_pixels={len(head_pts)} cropped"
    mid_lo = min_y + span * 0.35
    mid_hi = min_y + span * 0.65
    mid = [p for p in pts if mid_lo <= p[1] <= mid_hi]
    if not mid:
        return False, "no_torso_band"
    xs = [p[0] for p in mid]
    width = max(xs) - min(xs)
    if width / span < MIN_FRONT_WIDTH_RATIO:
        return False, f"side_silhouette width/span={width / span:.2f}"
    torso_rgb = [p[2] for p in mid]
    mean = tuple(sum(c[i] for c in torso_rgb) / len(torso_rgb) for i in range(3))
    brightness = sum(mean) / 3.0
    if aid == "staff-founder-idle" and brightness > 205:
        return False, f"founder_torso_too_bright rgb={tuple(int(c) for c in mean)}"
    if aid == "staff-researcher-idle" and brightness < 115:
        return False, f"researcher_torso_too_dark rgb={tuple(int(c) for c in mean)}"
    return True, f"fill={fill:.2f} width/span={width / span:.2f} torso_rgb={tuple(int(c) for c in mean)}"


def main() -> int:
    rc = 0
    for aid in HERO_IDS:
        png = PREVIEW / f"{aid}.png"
        if not png.is_file():
            print(f"FAIL {aid} missing preview")
            rc = 1
            continue
        ok, msg = analyze(aid, png)
        if ok:
            print(f"OK {aid} {msg}")
        else:
            print(f"FAIL {aid} {msg}")
            rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())