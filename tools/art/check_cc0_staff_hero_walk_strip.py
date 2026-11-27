#!/usr/bin/env python3
"""Hero walk/run strip PNG exists and has four frames on grey backdrop."""
from __future__ import annotations

import argparse
import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
GREY = (150, 150, 150)


def check_strip(png: Path) -> tuple[bool, str]:
    if not png.is_file() or png.stat().st_size < 2000:
        return False, "missing_strip"
    img = Image.open(png).convert("RGB")
    w, h = img.size
    if w < 4 * 64 or h < 64:
        return False, f"strip_too_small {w}x{h}"
    tile_w = w // 4
    corners = [img.getpixel((c, h // 2)) for c in (4, tile_w + 4, 2 * tile_w + 4, 3 * tile_w + 4)]
    if not all(abs(r - GREY[0]) < 12 for r, _g, _b in corners):
        return False, f"backdrop corners={corners[:2]} want≈{GREY}"
    return True, f"strip {w}x{h} tiles=4"


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("asset_id", choices=("staff-founder-idle", "staff-researcher-idle"))
    args = parser.parse_args()
    stem = "staff-founder-walk-strip" if args.asset_id == "staff-founder-idle" else "staff-researcher-walk-strip"
    png = PREVIEW / f"{stem}.png"
    ok, msg = check_strip(png)
    if ok:
        print(f"OK {args.asset_id} walk_strip {msg}")
        return 0
    print(f"FAIL {args.asset_id} walk_strip {msg}")
    return 1


if __name__ == "__main__":
    sys.exit(main())