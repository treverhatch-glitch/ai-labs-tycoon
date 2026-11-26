#!/usr/bin/env python3
"""Preview-only FOR SALE face plate (not committed albedo; front NCC gate)."""
from __future__ import annotations

import sys
from pathlib import Path


def paint(path: Path) -> None:
    from PIL import Image, ImageDraw, ImageFont

    from packet34_preview_gates import _forsale_red_template

    path.parent.mkdir(parents=True, exist_ok=True)
    w, h = 512, 512
    im = Image.new("RGB", (w, h), (252, 252, 250))
    draw = ImageDraw.Draw(im)
    margin = w // 14
    draw.rectangle((margin, margin, w - margin, h - margin), fill=(252, 250, 248))
    text = "FOR SALE"
    inner_w, inner_h = w - 2 * margin, h - 2 * margin
    tmpl = _forsale_red_template(inner_w, inner_h)
    # Paste template red mask as #C8102E on cream board.
    red_layer = Image.new("RGB", (inner_w, inner_h), (252, 250, 248))
    tmpl_px = tmpl.load()
    red_px = red_layer.load()
    for y in range(inner_h):
        for x in range(inner_w):
            if tmpl_px[x, y] > 32:
                red_px[x, y] = (200, 16, 46)
    im.paste(red_layer, (margin, margin))
    im.save(path)


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: paint_sign_preview_face.py <out.png>", file=sys.stderr)
        return 1
    sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
    paint(Path(sys.argv[1]))
    print("OK", sys.argv[1])
    return 0


if __name__ == "__main__":
    raise SystemExit(main())