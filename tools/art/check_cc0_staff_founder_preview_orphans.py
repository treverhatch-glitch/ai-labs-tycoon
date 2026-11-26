#!/usr/bin/env python3
"""Founder PNG: no separate bright blobs away from body (e.g. stray mug)."""
from __future__ import annotations

import sys
from collections import deque
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"
MIN_MAIN_FRAC = 0.92
MIN_ORPHAN_PX = 24


def _components(mask: Image.Image) -> list[int]:
    w, h = mask.size
    seen = [[False] * w for _ in range(h)]
    sizes: list[int] = []

    def on(x: int, y: int) -> bool:
        return mask.getpixel((x, y)) > 48

    for y in range(h):
        for x in range(w):
            if seen[y][x] or not on(x, y):
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            seen[y][x] = True
            n = 0
            while q:
                cx, cy = q.popleft()
                n += 1
                for dx, dy in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                    nx, ny = cx + dx, cy + dy
                    if nx < 0 or ny < 0 or nx >= w or ny >= h or seen[ny][nx]:
                        continue
                    if not on(nx, ny):
                        continue
                    seen[ny][nx] = True
                    q.append((nx, ny))
            sizes.append(n)
    return sizes


def main() -> int:
    mask_path = PNG.with_name(PNG.stem + ".mask.png")
    if not mask_path.is_file():
        print("FAIL staff-founder-idle missing mask")
        return 1
    mask = Image.open(mask_path).convert("L")
    sizes = sorted(_components(mask), reverse=True)
    if not sizes:
        print("FAIL staff-founder-idle empty_mask")
        return 1
    main_n = sizes[0]
    total = sum(sizes)
    orphans = [s for s in sizes[1:] if s >= MIN_ORPHAN_PX]
    if main_n / total < MIN_MAIN_FRAC:
        print(f"FAIL staff-founder-idle fragmented main={main_n}/{total}")
        return 1
    if orphans:
        print(f"FAIL staff-founder-idle orphan_blobs={orphans[:5]}")
        return 1
    print(f"OK staff-founder-idle mask_main={main_n} total={total}")
    return 0


if __name__ == "__main__":
    sys.exit(main())