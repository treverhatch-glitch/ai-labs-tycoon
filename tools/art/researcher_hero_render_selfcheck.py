#!/usr/bin/env python3
"""Front-view render self-check: lab coat coverage in chest bbox, single coat island."""
from __future__ import annotations

import sys
from collections import deque
from pathlib import Path

from PIL import Image

HATCH_MIN_COVERAGE = 0.70
HATCH_COAT_ISLANDS = 1
MIN_ISLAND_PX = 48


def _offwhite_coat(r: int, g: int, b: int) -> bool:
    chroma = max(r, g, b) - min(r, g, b)
    if r >= 198 and g >= 196 and b >= 188 and chroma < 32:
        return True
    if r >= 168 and g >= 166 and b >= 160 and chroma < 44:
        return r >= g - 8 and r >= b - 10
    if r >= 215 and g >= 212 and b >= 205 and chroma < 20:
        return True
    # Shaded lab-coat shell (v16 solidify + preview exposure)
    return r >= 118 and g >= 115 and b >= 108 and chroma < 58 and r >= g - 8


def _figure_span(mask: Image.Image) -> tuple[int, int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, h - 1, 0, w - 1
    xs = [x for y in ys for x in range(w) if mask.getpixel((x, y)) > 48]
    return min(ys), max(ys), min(xs), max(xs)


def _count_islands(
    coat_mask: list[list[bool]],
    y0: int,
    y1: int,
    x_lo: int,
    x_hi: int,
    w: int,
    *,
    min_pixels: int = MIN_ISLAND_PX,
) -> int:
    visited = [[False] * w for _ in range(y1 + 1)]
    islands = 0
    for y in range(y0, y1 + 1):
        for x in range(x_lo, min(w, x_hi + 1)):
            if not coat_mask[y][x] or visited[y][x]:
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            visited[y][x] = True
            size = 0
            while q:
                cx0, cy0 = q.popleft()
                size += 1
                for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                    nx, ny = cx0 + dx, cy0 + dy
                    if (
                        y0 <= ny <= y1
                        and x_lo <= nx < min(w, x_hi + 1)
                        and coat_mask[ny][nx]
                        and not visited[ny][nx]
                    ):
                        visited[ny][nx] = True
                        q.append((nx, ny))
            if size >= min_pixels:
                islands += 1
    return islands


def analyze_front_coat(png: Path, mask: Path | None = None) -> dict[str, float]:
    if mask is None:
        mask = png.with_name(png.stem + ".mask.png")
    img = Image.open(png).convert("RGB")
    m = Image.open(mask).convert("L") if mask.is_file() else None
    w, h = img.size
    min_y, max_y, min_x, max_x = _figure_span(m) if m else (0, h - 1, 0, w - 1)
    span = max(1, max_y - min_y)
    cx = (min_x + max_x) // 2
    band_w = max(12, (max_x - min_x) * 2 // 5)
    x_lo, x_hi = cx - band_w, cx + band_w
    chest_y0 = max_y - int(span * 0.56)
    chest_y1 = max_y - int(span * 0.285)
    open_half = max(4, (max_x - min_x) // 18)
    chest_total = 0
    coat_px = 0
    coat_mask = [[False] * w for _ in range(h)]
    for y in range(chest_y0, chest_y1 + 1):
        for x in range(max(0, x_lo), min(w, x_hi + 1)):
            if m and m.getpixel((x, y)) <= 48:
                continue
            if abs(x - cx) < open_half and y > chest_y0 + (chest_y1 - chest_y0) // 5:
                continue
            chest_total += 1
            r, g, b = img.getpixel((x, y))
            if _offwhite_coat(r, g, b):
                coat_px += 1
                coat_mask[y][x] = True
    coverage = coat_px / max(1, chest_total)
    coat_islands = _count_islands(coat_mask, chest_y0, chest_y1, max(0, x_lo), x_hi, w)
    return {
        "chest_total": float(chest_total),
        "coat_px": float(coat_px),
        "coverage": coverage,
        "coat_islands": float(coat_islands),
        "chest_y0": float(chest_y0),
        "chest_y1": float(chest_y1),
    }


def assert_front_coat_render(
    png: Path,
    *,
    min_coverage: float = HATCH_MIN_COVERAGE,
    max_coat_islands: int = HATCH_COAT_ISLANDS,
) -> None:
    m = analyze_front_coat(png)
    if m["coverage"] < min_coverage:
        raise RuntimeError(
            f"FAIL researcher_hero_render_selfcheck coat_coverage={m['coverage']:.3f} "
            f"want>={min_coverage} coat_px={m['coat_px']:.0f} chest_total={m['chest_total']:.0f}"
        )
    if m["coat_islands"] > max_coat_islands:
        raise RuntimeError(
            f"FAIL researcher_hero_render_selfcheck coat_islands={int(m['coat_islands'])} "
            f"want<={max_coat_islands} (disconnected floaters)"
        )
    print(
        "OK researcher_hero_render_selfcheck "
        f"coverage={m['coverage']:.3f} coat_px={m['coat_px']:.0f} "
        f"chest_total={m['chest_total']:.0f} coat_islands={int(m['coat_islands'])}"
    )


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: researcher_hero_render_selfcheck.py <front.png>", file=sys.stderr)
        return 1
    assert_front_coat_render(Path(sys.argv[1]))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())