#!/usr/bin/env python3
"""Force backdrop flood mask to exact RGB 150 (matches check_cc0_garage_preview)."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEWS = [
    REPO / "docs/art/previews/garage-interior-room-kit.png",
    REPO / "docs/art/previews/garage-interior-room-kit-ortho-top.png",
    REPO / "docs/art/previews/garage-interior-room-kit-ortho-front.png",
]
EDGE_FRAC = 0.03
CORNER_RGB = (150, 150, 150)


def main() -> int:
    try:
        from PIL import Image
    except ImportError:
        print("FAIL: Pillow required", file=sys.stderr)
        return 1
    import importlib.util

    spec = importlib.util.spec_from_file_location(
        "check_cc0_garage_preview",
        REPO / "tools/art/check_cc0_garage_preview.py",
    )
    assert spec and spec.loader
    chk = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(chk)
    bg_mask = chk._background_mask
    for path in PREVIEWS:
        if not path.is_file():
            print(f"FAIL: missing {path}", file=sys.stderr)
            return 1
        im = Image.open(path).convert("RGB")
        w, h = im.size
        bg = bg_mask(im)
        for y in range(h):
            for x in range(w):
                if bg[y][x]:
                    im.putpixel((x, y), CORNER_RGB)
        bx = max(1, int(w * EDGE_FRAC))
        by = max(1, int(h * EDGE_FRAC))
        for y in range(h):
            for x in range(w):
                if x < bx or x >= w - bx or y < by or y >= h - by:
                    im.putpixel((x, y), CORNER_RGB)
        im.save(path)
        print("OK pin backdrop", path.name)
    return 0


if __name__ == "__main__":
    sys.exit(main())