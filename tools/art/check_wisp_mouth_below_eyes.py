#!/usr/bin/env python3
"""Wisp preview faces: mouth pixels must sit below eye pixels (image Y down)."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "tools/art"))
from preview_quality_lib import _object_mask_from_images, _pixel_sat  # noqa: E402

WISP_IDS = ["wisp-happy", "wisp-neutral", "wisp-worried", "wisp-angry", "wisp-glowing"]
PREVIEW = REPO / "docs/art/previews"


def _band_centroids(rp, mp, w, h, is_obj, y0: float, y1: float) -> tuple[float, float, int]:
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
      if not (dark or bright or sat > 0.18):
        continue
      cx_sum += x
      cy_sum += y
      n += 1
  if n < 12:
    return 0.0, 0.0, 0
  return cx_sum / n, cy_sum / n, n


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
    _, eye_y, eye_n = _band_centroids(rp, mp, w, h, is_obj, 0.30, 0.52)
    _, mouth_y, mouth_n = _band_centroids(rp, mp, w, h, is_obj, 0.48, 0.68)
    if eye_n < 12 or mouth_n < 8:
      print(f"FAIL {aid} eye_n={eye_n} mouth_n={mouth_n}")
      rc = 1
    elif mouth_y <= eye_y + 4:
      print(f"FAIL {aid} mouth_y={mouth_y:.1f} eye_y={eye_y:.1f} (mouth not below eyes)")
      rc = 1
    else:
      print(f"OK {aid} mouth_y={mouth_y:.1f} eye_y={eye_y:.1f}")
  return rc


if __name__ == "__main__":
  sys.exit(main())