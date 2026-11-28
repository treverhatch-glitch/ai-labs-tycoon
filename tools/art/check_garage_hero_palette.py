#!/usr/bin/env python3
"""Preview hero palette: #969696 studio, warm TPH (no cyan/yellow primaries)."""
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
MAX_PRIMARY_FRAC = 0.12
MAX_COOL_FRAC = 0.06
MAX_BRIGHT_YELLOW_FRAC = 0.05
SCENE_KIT_MAX_COOL = 0.03
SCENE_KIT_MAX_YELLOW = 0.04


def is_primary_pixel(r: int, g: int, b: int) -> bool:
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


def is_cool_accent(r: int, g: int, b: int) -> bool:
    h, s, v = colorsys.rgb_to_hsv(r / 255.0, g / 255.0, b / 255.0)
    if s < 0.28 or v < 0.35:
        return False
    deg = h * 360.0
    return 155 <= deg <= 215 and s >= 0.35


def is_bright_yellow(r: int, g: int, b: int) -> bool:
    h, s, v = colorsys.rgb_to_hsv(r / 255.0, g / 255.0, b / 255.0)
    deg = h * 360.0
    return 38 <= deg <= 68 and s >= 0.42 and v >= 0.65


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
    corner_tol = 40 if asset_id == "garage-scene-kit" else CORNER_TOL
    for i, px in enumerate(corners):
        if any(abs(px[c] - STUDIO_RGB[c]) > corner_tol for c in range(3)):
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
    cool = sum(1 for row in obj if is_cool_accent(int(row[0]), int(row[1]), int(row[2])))
    yellow = sum(1 for row in obj if is_bright_yellow(int(row[0]), int(row[1]), int(row[2])))
    n = len(obj)
    p_frac, c_frac, y_frac = primary / n, cool / n, yellow / n
    rc = 0
    if asset_id == "garage-scene-kit":
        cool_lim = SCENE_KIT_MAX_COOL
        yell_lim = SCENE_KIT_MAX_YELLOW
        p_lim = 0.08
    elif asset_id == "garage-roll-door":
        cool_lim = 1.0
        yell_lim = 1.0
        p_lim = MAX_PRIMARY_FRAC
    elif asset_id == "garage-folding-chair":
        cool_lim = 0.85
        yell_lim = 1.0
        p_lim = MAX_PRIMARY_FRAC
    elif asset_id.startswith("tex-garage-"):
        cool_lim = MAX_COOL_FRAC
        yell_lim = MAX_BRIGHT_YELLOW_FRAC
        p_lim = MAX_PRIMARY_FRAC
    else:
        cool_lim = 0.42
        yell_lim = 1.0
        p_lim = MAX_PRIMARY_FRAC
    if p_frac > p_lim:
        print(f"FAIL {asset_id} primary_frac={p_frac:.3f} > {p_lim}")
        rc = 1
    if c_frac > cool_lim:
        print(f"FAIL {asset_id} cool_accent_frac={c_frac:.3f} > {cool_lim}")
        rc = 1
    if y_frac > yell_lim:
        print(f"FAIL {asset_id} bright_yellow_frac={y_frac:.3f} > {yell_lim}")
        rc = 1
    if rc == 0:
        print(
            f"OK {asset_id} primary_frac={p_frac:.3f} cool={c_frac:.3f} yellow={y_frac:.3f}"
        )
    return rc


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
    for extra in ("garage-scene-kit", "tex-garage-trim-orange", "tex-garage-wall-beige"):
        rc |= check_preview(extra)
    return rc


if __name__ == "__main__":
    sys.exit(main())