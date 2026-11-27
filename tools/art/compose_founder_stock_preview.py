#!/usr/bin/env python3
"""Side-by-side stock check (grey) and founder hero preview."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image, ImageDraw

REPO = Path(__file__).resolve().parents[2]
STOCK = REPO / "docs/art/previews/staff-founder-kenney-stock-check.png"
FOUNDER = REPO / "docs/art/previews/staff-founder-idle.png"
OUT = REPO / "docs/art/previews/staff-founder-vs-stock-check.png"


def main() -> int:
    if not STOCK.is_file() or not FOUNDER.is_file():
        print("FAIL compose_founder_stock missing_png")
        return 1
    a = Image.open(STOCK).convert("RGB")
    b = Image.open(FOUNDER).convert("RGB")
    h = max(a.height, b.height)

    def pad(im: Image.Image) -> Image.Image:
        if im.height == h:
            return im
        canvas = Image.new("RGB", (im.width, h), (150, 150, 150))
        canvas.paste(im, (0, (h - im.height) // 2))
        return canvas

    a, b = pad(a), pad(b)
    gap = 8
    out = Image.new("RGB", (a.width + gap + b.width, h), (120, 120, 120))
    out.paste(a, (0, 0))
    out.paste(b, (a.width + gap, 0))
    draw = ImageDraw.Draw(out)
    draw.text((12, 10), "stock", fill=(240, 240, 240))
    draw.text((a.width + gap + 12, 10), "founder", fill=(240, 240, 240))
    out.save(OUT)
    print(f"OK compose_founder_stock wrote {OUT.relative_to(REPO)}")
    return 0


if __name__ == "__main__":
    sys.exit(main())