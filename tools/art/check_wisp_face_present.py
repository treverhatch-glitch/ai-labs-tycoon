#!/usr/bin/env python3
"""Wisp previews must show face features (dark eyes / mouth) in upper-center mask."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "tools/art"))
from preview_quality_lib import _object_mask_from_images, _pixel_sat  # noqa: E402

WISP_IDS = ["wisp-happy", "wisp-neutral", "wisp-worried", "wisp-angry", "wisp-glowing"]
PREVIEW = REPO / "docs/art/previews"
MIN_FACE_PIXELS = 48
MIN_FACE_SAT = 0.12


def main() -> int:
    from PIL import Image

    rc = 0
    for aid in WISP_IDS:
        rgb_path = PREVIEW / f"{aid}.png"
        mask_path = PREVIEW / f"{aid}.mask.png"
        if not rgb_path.is_file() or not mask_path.is_file():
            print(f"FAIL missing preview {aid}")
            rc = 1
            continue
        rgb = Image.open(rgb_path).convert("RGBA")
        mask = Image.open(mask_path).convert("RGBA")
        w, h = rgb.size
        rp = list(rgb.getdata())
        mp = list(mask.getdata())
        is_obj = _object_mask_from_images(rp, mp, w, h)
        face_pixels = 0
        sat_sum = 0.0
        for y in range(int(h * 0.32), int(h * 0.62)):
            for x in range(int(w * 0.28), int(w * 0.72)):
                i = y * w + x
                if not is_obj[i]:
                    continue
                r, g, b, _a = rp[i]
                s = _pixel_sat(r, g, b)
                if s >= MIN_FACE_SAT or (r < 80 and g < 80):
                    face_pixels += 1
                    sat_sum += s
        if face_pixels < MIN_FACE_PIXELS:
            print(f"FAIL {aid} face_pixels={face_pixels}")
            rc = 1
        else:
            mean_sat = sat_sum / face_pixels
            print(f"OK {aid} face_pixels={face_pixels} sat={mean_sat:.3f}")
    return rc


if __name__ == "__main__":
    sys.exit(main())