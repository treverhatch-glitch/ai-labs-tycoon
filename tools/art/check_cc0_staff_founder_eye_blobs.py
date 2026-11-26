#!/usr/bin/env python3
"""Founder head upper half: two dark eye blobs (modeled Quaternius eyes)."""
from __future__ import annotations

import sys
from collections import deque
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"
MAX_EYE_LUM = 115


def _figure_span(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, 0, 0
    return min(ys), max(ys), max(1, max(ys) - min(ys))


def _blobs(mask: Image.Image, img: Image.Image, y0: int, y1: int) -> list[tuple[int, int, int, int]]:
    w, _ = img.size
    seen = [[False] * w for _ in range(y1 - y0)]
    blobs: list[tuple[int, int, int, int]] = []

    def dark(x: int, y: int) -> bool:
        if mask.getpixel((x, y)) <= 48:
            return False
        r, g, b = img.getpixel((x, y))[:3]
        return max(r, g, b) <= MAX_EYE_LUM

    for y in range(y0, y1):
        for x in range(w):
            yi = y - y0
            if seen[yi][x] or not dark(x, y):
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            seen[yi][x] = True
            minx = maxx = x
            miny = maxy = y
            n = 0
            while q:
                cx, cy = q.popleft()
                n += 1
                minx, maxx = min(minx, cx), max(maxx, cx)
                miny, maxy = min(miny, cy), max(maxy, cy)
                for dx, dy in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                    nx, ny = cx + dx, cy + dy
                    if ny < y0 or ny >= y1 or nx < 0 or nx >= w:
                        continue
                    nyi = ny - y0
                    if seen[nyi][nx] or not dark(nx, ny):
                        continue
                    seen[nyi][nx] = True
                    q.append((nx, ny))
            if n >= 6:
                blobs.append((minx, miny, maxx, maxy))
    return blobs


def analyze_eye_blobs(png: Path) -> tuple[bool, str]:
    mask_path = png.with_name(png.stem + ".mask.png")
    if not png.is_file() or not mask_path.is_file():
        return False, "missing preview"
    img = Image.open(png).convert("RGB")
    mask = Image.open(mask_path).convert("L")
    w, _ = img.size
    min_y, _, span = _figure_span(mask)
    head_hi = min_y + int(span * 0.30)
    head_mid = min_y + int(span * 0.14)
    blobs = _blobs(mask, img, head_mid, head_hi)
    left = [b for b in blobs if (b[0] + b[2]) / 2 < w * 0.48]
    right = [b for b in blobs if (b[0] + b[2]) / 2 > w * 0.52]
    if len(blobs) >= 2:
        return True, f"eye_blobs n={len(blobs)} L={len(left)} R={len(right)}"
    if len(blobs) == 1:
        b = blobs[0]
        if (b[2] - b[0]) >= w * 0.07:
            return True, "eye_blobs_merged_pair"
    return False, f"eye_blobs_missing L={len(left)} R={len(right)} total={len(blobs)}"


def main() -> int:
    ok, msg = analyze_eye_blobs(PNG)
    if ok:
        print(f"OK staff-founder-idle {msg}")
        return 0
    print(f"FAIL staff-founder-idle {msg}")
    return 1


if __name__ == "__main__":
    sys.exit(main())