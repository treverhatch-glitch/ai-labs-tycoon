#!/usr/bin/env python3
"""Preview hero palette: #969696 studio, no saturated RGB primaries on object mask."""
from __future__ import annotations

import colorsys
import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"
STUDIO_RGB = (150, 150, 150)
CORNER_TOL = 12
MAX_PRIMARY_FRAC = 0.22


def is_primary_pixel(r: int, g: int, b: int) -> bool:
    """Literal RGB primaries only (reject Hatch vcol-style saturated primaries)."""
    h, s, v = colorsys.rgb_to_hsv(r / 255.0, g / 255.0, b / 255.0)
    if s < 0.82 or v < 0.5:
        return False
    deg = h * 360.0
    if deg < 18 or deg > 342:
        return True
    if 108 <= deg <= 138:
        return True
    if 208 <= deg <= 238:
        return True
    return False


def check_preview(asset_id: str) -> int:
    from PIL import Image
    import numpy as np

    path = PREVIEW / f"{asset_id}.png"
    mask_path = PREVIEW / f"{asset_id}.mask.png"
    if not path.is_file() or not mask_path.is_file():
        print(f"FAIL {asset_id} missing preview/mask")
        return 1
    im = Image.open(path).convert("RGB")
    w, h = im.size
    inset = 4
    corners = [
        im.getpixel((inset, inset)),
        im.getpixel((w - 1 - inset, inset)),
        im.getpixel((inset, h - 1 - inset)),
        im.getpixel((w - 1 - inset, h - 1 - inset)),
    ]
    for i, px in enumerate(corners):
        if any(abs(px[c] - STUDIO_RGB[c]) > CORNER_TOL for c in range(3)):
            print(f"FAIL {asset_id} corner{i}={px} need ~{STUDIO_RGB}")
            return 1
    mask = Image.open(mask_path).convert("RGBA")
    rgb = np.array(im)
    alpha = np.array(mask)[:, :, 3]
    sel = alpha >= 200
    if not sel.any():
        print(f"FAIL {asset_id} empty mask")
        return 1
    obj = rgb[sel]
    primary = sum(1 for row in obj if is_primary_pixel(int(row[0]), int(row[1]), int(row[2])))
    frac = primary / len(obj)
    if frac > MAX_PRIMARY_FRAC:
        print(f"FAIL {asset_id} primary_frac={frac:.3f} > {MAX_PRIMARY_FRAC}")
        return 1
    print(f"OK {asset_id} studio_corners primary_frac={frac:.3f}")
    return 0


def main() -> int:
    try:
        from PIL import Image  # noqa: F401
    except ImportError:
        print("FAIL: Pillow required")
        return 1
    rc = 0
    data = json.loads(MANIFEST.read_text(encoding="utf-8"))
    for entry in data["assets"]:
        rc |= check_preview(entry["id"])
    rc |= check_preview("garage-scene-kit")
    return rc


if __name__ == "__main__":
    sys.exit(main())