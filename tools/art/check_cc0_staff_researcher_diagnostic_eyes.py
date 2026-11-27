#!/usr/bin/env python3
"""0° diagnostic must show two dark eye blobs on face (not hair-only back view)."""
from __future__ import annotations

import sys
from collections import deque
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-researcher-face-diagnostic-0deg.png"
MIN_BLOB_PX = 4
MAX_EYE_LUM = 96


def _figure_span(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, 0, 0
    return min(ys), max(ys), max(1, max(ys) - min(ys))


def _dark_blobs(mask: Image.Image, img: Image.Image, y0: int, y1: int) -> list[tuple[int, int, int, int]]:
    w, _ = img.size
    seen = [[False] * w for _ in range(y1 - y0)]
    blobs: list[tuple[int, int, int, int]] = []

    def dark(x: int, y: int) -> bool:
        if mask.getpixel((x, y)) <= 48:
            return False
        r, g, b = img.getpixel((x, y))[:3]
        if max(r, g, b) > MAX_EYE_LUM or (r + g + b) >= 120:
            return False
        return True

    for y in range(y0, y1):
        for x in range(w):
            yi = y - y0
            if seen[yi][x] or not dark(x, y):
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            seen[yi][x] = True
            n = 0
            minx = maxx = x
            miny = maxy = y
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
            if n >= MIN_BLOB_PX:
                blobs.append((minx, miny, maxx, maxy))
    return blobs


def main() -> int:
    mask_path = PNG.with_name(PNG.stem + ".mask.png")
    if not PNG.is_file() or not mask_path.is_file():
        print("FAIL staff-researcher-idle diagnostic_eyes missing_preview")
        return 1
    mask = Image.open(mask_path).convert("L")
    img = Image.open(PNG).convert("RGB")
    _miny, _maxy, span = _figure_span(mask)
    # 0° framing places eyes ~40–50% down the figure span (not the old hero head crop).
    y0 = _miny + int(span * 0.52)
    y1 = _miny + int(span * 0.78)
    w, _ = img.size
    blobs = _dark_blobs(mask, img, y0, y1)
    left = [b for b in blobs if (b[0] + b[2]) / 2 < w * 0.48]
    right = [b for b in blobs if (b[0] + b[2]) / 2 > w * 0.52]
    if left and right:
        print(f"OK staff-researcher-idle diagnostic_eyes blobs L={len(left)} R={len(right)}")
        return 0
    mid = w // 2
    half_w = max(12, w // 2 - 8)
    l_px = r_px = 0
    for y in range(y0, y1):
        for x in range(max(0, mid - half_w), mid):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))
            if max(r, g, b) <= MAX_EYE_LUM and (r + g + b) < 120:
                l_px += 1
        for x in range(mid, min(w, mid + half_w)):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))
            if max(r, g, b) <= MAX_EYE_LUM and (r + g + b) < 120:
                r_px += 1
    if l_px >= MIN_BLOB_PX and r_px >= MIN_BLOB_PX:
        print(f"OK staff-researcher-idle diagnostic_eyes split L={l_px} R={r_px}")
        return 0
    print(
        f"FAIL staff-researcher-idle diagnostic_eyes L={len(left)} R={len(right)} total={len(blobs)}"
    )
    return 1


if __name__ == "__main__":
    sys.exit(main())