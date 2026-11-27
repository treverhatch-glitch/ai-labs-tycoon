#!/usr/bin/env python3
"""0° diagnostic must show two dark eye blobs on face (not hair-only back view)."""
from __future__ import annotations

import sys
from collections import deque
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-face-diagnostic-0deg.png"
MIN_BLOBS = 2
MIN_BLOB_PX = 4


def _figure_span(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, 0, 0
    return min(ys), max(ys), max(1, max(ys) - min(ys))


def _dark_blobs(mask: Image.Image, img: Image.Image, y0: int, y1: int) -> int:
    w, _ = img.size
    seen = [[False] * w for _ in range(y1 - y0)]
    blobs = 0

    def dark(x: int, y: int) -> bool:
        if mask.getpixel((x, y)) <= 48:
            return False
        r, g, b = img.getpixel((x, y))[:3]
        if max(r, g, b) > 130:
            return False
        if b > r + 20:
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
            while q:
                cx, cy = q.popleft()
                n += 1
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
                blobs += 1
    return blobs


def main() -> int:
    mask_path = PNG.with_name(PNG.stem + ".mask.png")
    if not PNG.is_file() or not mask_path.is_file():
        print("FAIL staff-founder-idle diagnostic_eyes missing_preview")
        return 1
    mask = Image.open(mask_path).convert("L")
    img = Image.open(PNG).convert("RGB")
    _miny, _maxy, span = _figure_span(mask)
    y0 = _miny + int(span * 0.05)
    y1 = _miny + int(span * 0.32)
    blobs = _dark_blobs(mask, img, y0, y1)
    if blobs < MIN_BLOBS:
        print(f"FAIL staff-founder-idle diagnostic_eyes blobs={blobs} want>={MIN_BLOBS}")
        return 1
    print(f"OK staff-founder-idle diagnostic_eyes blobs={blobs}")
    return 0


if __name__ == "__main__":
    sys.exit(main())