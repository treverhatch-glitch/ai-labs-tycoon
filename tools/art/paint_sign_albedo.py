#!/usr/bin/env python3
"""Write cream sign albedo with dark FOR SALE (invoked from Blender or CI)."""
from __future__ import annotations

import sys
from pathlib import Path


def paint(path: Path, *, min_unique: int = 120, min_bytes: int = 12_000) -> None:
    from PIL import Image, ImageDraw, ImageFont

    path.parent.mkdir(parents=True, exist_ok=True)
    w = h = 1024
    im = Image.new("RGB", (w, h), (245, 238, 220))
    draw = ImageDraw.Draw(im)
    margin = w // 14
    draw.rectangle((margin, margin, w - margin, h - margin), fill=(252, 245, 228))
    font = None
    for ft in (
        "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
        "/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf",
    ):
        try:
            font = ImageFont.truetype(ft, size=96)
            break
        except OSError:
            continue
    if font is None:
        raise SystemExit("no bold font for FOR SALE albedo")
    text = "FOR SALE"
    bbox = draw.textbbox((0, 0), text, font=font)
    tw, th = bbox[2] - bbox[0], bbox[3] - bbox[1]
    tx, ty = (w - tw) // 2, (h - th) // 2
    draw.text((tx, ty), text, fill=(22, 14, 10), font=font)
    pad = 3
    draw.rectangle((tx - pad, ty - pad, tx + tw + pad, ty + th + pad), outline=(12, 8, 6), width=4)
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