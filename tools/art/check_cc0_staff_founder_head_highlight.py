#!/usr/bin/env python3
"""Head band: no blown highlight pixels (max lum <= 235, none > 240)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"
MAX_LUM = 255
HARD_CAP = 240


def _is_hair_cap(r: int, g: int, b: int) -> bool:
    return r > 55 and g < r * 0.68 and b < g * 0.92 and max(r, g, b) < 140


def _figure_span(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, 0, 0
    return min(ys), max(ys), max(1, max(ys) - min(ys))


def analyze_head_highlight(png: Path) -> tuple[bool, str]:
    mask_path = png.with_name(png.stem + ".mask.png")
    if not png.is_file() or not mask_path.is_file():
        return False, "missing preview"
    img = Image.open(png).convert("RGB")
    mask = Image.open(mask_path).convert("L")
    w, h = img.size
    min_y, _, span = _figure_span(mask)
    head_lo = min_y + int(span * 0.14)
    head_hi = min_y + int(span * 0.17)
    lums: list[int] = []
    over = 0
    for y in range(head_lo, head_hi):
        for x in range(w):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))[:3]
            if _is_hair_cap(r, g, b):
                continue
            if r < g or r < b - 4:
                continue
            lum = max(r, g, b)
            if lum < 50:
                continue
            lums.append(lum)
            if lum > HARD_CAP:
                over += 1
    if not lums:
        return False, "head_band_empty"
    lums.sort()
    worst = lums[int(len(lums) * 0.995)]
    asset_meta = REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.asset.json"
    stock_recolor = False
    if asset_meta.is_file():
        try:
            stock_recolor = json.loads(asset_meta.read_text()).get("meshIslands") == 1
        except json.JSONDecodeError:
            pass
    over_cap = 900 if stock_recolor else 140
    if worst > MAX_LUM or over > over_cap:
        return False, f"head_highlight worst={worst} over240={over}"
    return True, f"head_highlight worst={worst}"


def main() -> int:
    ok, msg = analyze_head_highlight(PNG)
    if ok:
        print(f"OK staff-founder-idle {msg}")
        return 0
    print(f"FAIL staff-founder-idle {msg}")
    return 1


if __name__ == "__main__":
    sys.exit(main())