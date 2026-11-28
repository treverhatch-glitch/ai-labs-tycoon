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
MIN_ORPHAN_PX = 80


def _components(mask: Image.Image) -> list[tuple[int, int, int, int, int]]:
    w, h = mask.size
    seen = [[False] * w for _ in range(h)]
    sizes: list[tuple[int, int, int, int, int]] = []

    def on(x: int, y: int) -> bool:
        return mask.getpixel((x, y)) > 48

    for y in range(h):
        for x in range(w):
            if seen[y][x] or not on(x, y):
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            seen[y][x] = True
            n = 0
            minx = maxx = x
            miny = maxy = y
            while q:
                cx, cy = q.popleft()
                n += 1
                minx = min(minx, cx)
                maxx = max(maxx, cx)
                miny = min(miny, cy)
                maxy = max(maxy, cy)
                for dx, dy in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                    nx, ny = cx + dx, cy + dy
                    if nx < 0 or ny < 0 or nx >= w or ny >= h or seen[ny][nx]:
                        continue
                    if not on(nx, ny):
                        continue
                    seen[ny][nx] = True
                    q.append((nx, ny))
            sizes.append((n, minx, miny, maxx, maxy))
    return sizes


def main() -> int:
    mask_path = PNG.with_name(PNG.stem + ".mask.png")
    if not mask_path.is_file():
        print("FAIL staff-founder-idle missing mask")
        return 1
    mask = Image.open(mask_path).convert("L")
    comps = sorted(_components(mask), reverse=True, key=lambda c: c[0])
    if not comps:
        print("FAIL staff-founder-idle empty_mask")
        return 1
    h = mask.size[1]
    floor_cut = int(h * 0.74)
    body = [c for c in comps if c[2] < floor_cut]
    if not body:
        body = comps
    main_n = body[0][0]
    total = sum(c[0] for c in comps)
    body_total = sum(c[0] for c in body)
    ys = [y for y in range(mask.size[1]) for x in range(mask.size[0]) if mask.getpixel((x, y)) > 48]
    fig_top, fig_bot = min(ys), max(ys)
    head_y1 = fig_top + int(max(1, fig_bot - fig_top) * 0.55)
    orphans = []
    for c in body[1:]:
        if c[0] < MIN_ORPHAN_PX or c[2] >= floor_cut:
            continue
        if c[0] < 220 and c[2] <= head_y1:
            continue  # FounderEye_L/R separate meshes on face
        orphans.append(c)
    if main_n / body_total < MIN_MAIN_FRAC:
        print(f"FAIL staff-founder-idle fragmented main={main_n}/{body_total}")
        return 1
    if orphans:
        print(f"FAIL staff-founder-idle orphan_blobs={[c[0] for c in orphans[:5]]}")
        return 1
    print(f"OK staff-founder-idle mask_main={main_n} total={total}")
    return 0


if __name__ == "__main__":
    sys.exit(main())