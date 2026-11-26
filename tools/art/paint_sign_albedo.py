#!/usr/bin/env python3
"""White sign face with red FOR SALE (≥5% inset on all sides)."""
from __future__ import annotations

import sys
from pathlib import Path


def paint(path: Path, *, min_unique: int = 80, min_bytes: int = 8_000) -> None:
    from PIL import Image, ImageDraw, ImageFont

    path.parent.mkdir(parents=True, exist_ok=True)
    w = h = 1024
    white = (252, 252, 250)
    im = Image.new("RGB", (w, h), white)
    draw = ImageDraw.Draw(im)
    margin = max(52, int(min(w, h) * 0.05))
    inner_w, inner_h = w - 2 * margin, h - 2 * margin
    font_path = "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf"
    text = "FOR SALE"
    font = None
    for size in range(200, 44, -2):
        try:
            font = ImageFont.truetype(font_path, size=size)
        except OSError:
            try:
                font = ImageFont.truetype(
                    "/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf",
                    size=size,
                )
            except OSError:
                continue
        bbox = draw.textbbox((0, 0), text, font=font)
        tw, th = bbox[2] - bbox[0], bbox[3] - bbox[1]
        if tw <= inner_w and th <= inner_h:
            break
    if font is None:
        raise SystemExit("no bold font for FOR SALE albedo")
    tx = (w - tw) // 2
    ty = (h - th) // 2
    draw.text((tx, ty), text, fill=(196, 34, 28), font=font)
    uniq = len({px for px in im.getdata()})
    if uniq < min_unique:
        raise SystemExit(f"sign albedo flat unique={uniq}")
    im.save(path)
    if path.stat().st_size < min_bytes:
        raise SystemExit(f"sign albedo too small bytes={path.stat().st_size}")


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: paint_sign_albedo.py <albedo.png>", file=sys.stderr)
        return 1
    paint(Path(sys.argv[1]))
    print("OK", sys.argv[1])
    return 0


if __name__ == "__main__":
    raise SystemExit(main())