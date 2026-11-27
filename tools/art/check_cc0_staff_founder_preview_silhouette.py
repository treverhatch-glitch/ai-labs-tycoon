#!/usr/bin/env python3
"""Founder PNG: head (skin) on top, shoes at bottom, shoulders wider than head."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
AID = "staff-founder-idle"
PNG = REPO / "docs/art/previews" / f"{AID}.png"


def _mask_rows(img: Image.Image, mask: Image.Image) -> tuple[list[int], int, int, int]:
    w, h = img.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        raise ValueError("empty_mask")
    min_y, max_y = min(ys), max(ys)
    return ys, w, h, max_y - min_y


def _is_skin(r: int, g: int, b: int) -> bool:
    return r > 148 and g > 110 and b < 178 and (r - b) > 18 and r >= g * 0.85


def _is_hair(r: int, g: int, b: int) -> bool:
    return b > 165 and r < 195 and g < 215


def _band_stats(img: Image.Image, mask: Image.Image, w: int, y0: int, y1: int):
    px = []
    xs = []
    for y in range(y0, y1):
        for x in range(w):
            if mask.getpixel((x, y)) <= 48:
                continue
            xs.append(x)
            r, g, b, *_ = img.convert("RGBA").getpixel((x, y))
            px.append((r, g, b))
    if not px:
        return None
    mean = tuple(sum(c[i] for c in px) / len(px) for i in range(3))
    width = max(xs) - min(xs)
    skin = sum(1 for r, g, b in px if _is_skin(r, g, b))
    hair = sum(1 for r, g, b in px if _is_hair(r, g, b))
    rgba = img.convert("RGBA")
    head_xs: list[int] = []
    for y in range(y0, y1):
        for x in range(w):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b, *_ = rgba.getpixel((x, y))
            if _is_skin(r, g, b) or _is_hair(r, g, b):
                head_xs.append(x)
    shoeish = sum(1 for r, g, b in px if r < 140 and g < 145 and b < 165 and (r + g + b) < 380)
    washed = sum(1 for r, g, b in px if r > 248 and g > 248 and b > 245)
    n = len(px)
    head_w = max(head_xs) - min(head_xs) if len(head_xs) >= 4 else width
    return {
        "mean": mean,
        "width": width,
        "head_width": head_w,
        "n": n,
        "skin_ratio": skin / n,
        "hair_ratio": hair / n,
        "head_ratio": (skin + hair) / n,
        "shoe_ratio": shoeish / n,
        "washed_ratio": washed / n,
    }


def analyze(png: Path) -> tuple[bool, str]:
    mask_path = png.with_name(png.stem + ".mask.png")
    img = Image.open(png)
    mask = Image.open(mask_path).convert("L") if mask_path.is_file() else None
    if mask is None:
        return False, "no_mask"
    ys, w, h, span = _mask_rows(img, mask)
    min_y, max_y = min(ys), max(ys)
    top_hi = min_y + int(span * 0.20)
    bot_lo = max_y - int(span * 0.10)
    shoulder_lo = min_y + int(span * 0.40)
    shoulder_hi = min_y + int(span * 0.52)
    top = _band_stats(img, mask, w, min_y, top_hi)
    bot = _band_stats(img, mask, w, bot_lo, max_y + 1)
    shoulders = _band_stats(img, mask, w, shoulder_lo, shoulder_hi)
    if not top or not bot or not shoulders:
        return False, "band_empty"
    if top["head_ratio"] < 0.012:
        return False, f"headstand_top_no_head head={top['head_ratio']:.3f}"
    if top["washed_ratio"] > 0.92 and top["head_ratio"] < 0.04:
        return False, f"headstand_top_washed washed={top['washed_ratio']:.2f}"
    if bot["shoe_ratio"] < 0.03 and sum(bot["mean"]) / 3 > 160:
        return False, f"headstand_bottom_not_shoes shoe={bot['shoe_ratio']:.3f}"
    top_narrow = top["head_width"] if top["head_ratio"] >= 0.02 else top["width"]
    wide_limit = 1.75 if top["head_ratio"] >= 0.28 else 1.22
    if top_narrow >= shoulders["width"] * wide_limit:
        return False, (
            f"headstand_top_wide top_w={top_narrow} shoulder_w={shoulders['width']}"
        )
    return True, (
        f"top_head={top['head_ratio']:.2f} bot_shoe={bot['shoe_ratio']:.2f} "
        f"top_w={top_narrow} shoulder_w={shoulders['width']}"
    )


def main() -> int:
    if not PNG.is_file():
        print(f"FAIL {AID} missing preview")
        return 1
    ok, msg = analyze(PNG)
    if ok:
        print(f"OK {AID} {msg}")
        return 0
    print(f"FAIL {AID} {msg}")
    return 1


if __name__ == "__main__":
    sys.exit(main())