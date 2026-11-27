#!/usr/bin/env python3
"""Wisp face features must sit on the front hemisphere (not the sphere top cap)."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "tools/art"))
from preview_quality_lib import _object_mask_from_images, _pixel_sat  # noqa: E402

WISP_IDS = ["wisp-happy", "wisp-neutral", "wisp-worried", "wisp-angry", "wisp-glowing"]
PREVIEW = REPO / "docs/art/previews"
MAX_CAP_FLOAT_PIX = 680


def _feature_in_strip(
    rp, w: int, h: int, is_obj: list[bool], y0: float, y1: float
) -> tuple[float, float, int]:
    cx_sum = cy_sum = 0.0
    n = 0
    y_lo = int(h * y0)
    y_hi = int(h * y1)
    for y in range(y_lo, y_hi):
        for x in range(int(w * 0.25), int(w * 0.75)):
            i = y * w + x
            if not is_obj[i]:
                continue
            r, g, b, _a = rp[i]
            sat = _pixel_sat(r, g, b)
            dark = r < 70 and g < 70 and b < 80
            bright = r > 200 and g > 200 and b > 190
            if g > r + 18 and g > b + 8 and not (dark or bright):
                continue
            if not (dark or bright or sat > 0.18):
                continue
            cx_sum += x
            cy_sum += y
            n += 1
    if n < 12:
        return 0.0, 0.0, 0
    return cx_sum / n, cy_sum / n, n


def analyze(aid: str, preview_dir: Path | None = None) -> tuple[bool, str]:
    from PIL import Image

    base = preview_dir or PREVIEW
    rgb_path = base / f"{aid}.png"
    mask_path = base / f"{aid}.mask.png"
    if not rgb_path.is_file() or not mask_path.is_file():
        return False, "missing preview"
    rgb = Image.open(rgb_path).convert("RGBA")
    mask = Image.open(mask_path).convert("RGBA")
    w, h = rgb.size
    rp = list(rgb.getdata())
    mp = list(mask.getdata())
    is_obj = _object_mask_from_images(rp, mp, w, h)
    ys = [y for y in range(h) for x in range(w) if is_obj[y * w + x]]
    if len(ys) < 80:
        return False, "sparse_mask"
    y_top, y_bot = min(ys), max(ys)
    span = y_bot - y_top + 1
    cap_limit = y_top + int(span * 0.18)
    eye_x, eye_y, eye_n = _feature_in_strip(rp, w, h, is_obj, 0.30, 0.52)
    mouth_x, mouth_y, mouth_n = _feature_in_strip(rp, w, h, is_obj, 0.48, 0.68)
    x_left = min(x for y in range(h) for x in range(w) if is_obj[y * w + x])
    x_right = max(x for y in range(h) for x in range(w) if is_obj[y * w + x])
    cap_float = 0
    x0 = x_left + int((x_right - x_left) * 0.25)
    x1 = x_right - int((x_right - x_left) * 0.25)
    for y in range(y_top, cap_limit):
        for x in range(x0, x1):
            i = y * w + x
            if not is_obj[i]:
                continue
            r, g, b, _a = rp[i]
            if (r > 238 and g > 235 and b > 225) or (r < 55 and g < 55 and b < 65):
                cap_float += 1
    if eye_n < 12 or mouth_n < 8:
        return False, f"eye_n={eye_n} mouth_n={mouth_n}"
    if cap_float > MAX_CAP_FLOAT_PIX:
        return False, f"above_cap={cap_float} cap_limit={cap_limit}"
    if eye_y < cap_limit + 2:
        return False, f"above_cap eye_y={eye_y:.1f} cap_limit={cap_limit}"
    if not (h * 0.29 <= eye_y <= h * 0.54):
        return False, f"eye_off_band eye_y={eye_y:.1f}"
    if mouth_y <= eye_y + 4:
        return False, f"mouth_above_eyes mouth_y={mouth_y:.1f} eye_y={eye_y:.1f}"
    mid_x = (x_left + x_right) * 0.5
    if abs(eye_x - mid_x) > (x_right - x_left) * 0.22:
        return False, f"off_center_x={eye_x:.1f}"
    in_band = eye_n + mouth_n
    return True, f"in_band={in_band} above_cap=0 eye_y={eye_y:.1f}"


def main() -> int:
    rc = 0
    for aid in WISP_IDS:
        ok, msg = analyze(aid)
        if ok:
            print(f"OK {aid} {msg}")
        else:
            print(f"FAIL {aid} {msg}")
            rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())