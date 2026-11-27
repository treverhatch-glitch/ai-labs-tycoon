#!/usr/bin/env python3
"""Front-view render self-check: lab coat coverage in chest bbox, no floaters."""
from __future__ import annotations

import sys
from collections import deque
from pathlib import Path

from PIL import Image


def _offwhite_coat(r: int, g: int, b: int) -> bool:
    if r >= 198 and g >= 196 and b >= 188 and max(r, g, b) - min(r, g, b) < 32:
        return True
    if r >= 172 and g >= 170 and b >= 164 and max(r, g, b) - min(r, g, b) < 42:
        return r >= g - 6 and r >= b - 8
    return r >= 215 and g >= 212 and b >= 205 and max(r, g, b) - min(r, g, b) < 20


def _figure_span(mask: Image.Image) -> tuple[int, int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, h - 1, 0, w - 1
    xs = [x for y in ys for x in range(w) if mask.getpixel((x, y)) > 48]
    return min(ys), max(ys), min(xs), max(xs)


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
    chest_y1 = max_y - int(span * 0.20)
    chest_y0 = max_y - int(span * 0.56)
    chest_y0 += max(4, int((chest_y1 - chest_y0) * 0.10))
    open_half = max(5, (max_x - min_x) // 18)
    chest_total = 0
    coat_px = 0
    coat_mask = [[False] * w for _ in range(h)]
    for y in range(chest_y0, chest_y1 + 1):
        for x in range(max(0, x_lo), min(w, x_hi + 1)):
            if m and m.getpixel((x, y)) <= 48:
                continue
            if abs(x - cx) < open_half and y > chest_y0 + (chest_y1 - chest_y0) // 4:
                continue
            chest_total += 1
            r, g, b = img.getpixel((x, y))
            if _offwhite_coat(r, g, b):
                coat_px += 1
                coat_mask[y][x] = True
    coverage = coat_px / max(1, chest_total)
    # Largest 4-connected coat component vs all coat in chest
    visited = [[False] * w for _ in range(h)]
    best = 0
    total_coat = 0
    for y in range(chest_y0, chest_y1 + 1):
        for x in range(max(0, x_lo), min(w, x_hi + 1)):
            if not coat_mask[y][x] or visited[y][x]:
                continue
            total_coat += 1
            q: deque[tuple[int, int]] = deque([(x, y)])
            visited[y][x] = True
            size = 0
            while q:
                cx0, cy0 = q.popleft()
                size += 1
                for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                    nx, ny = cx0 + dx, cy0 + dy
                    if (
                        chest_y0 <= ny <= chest_y1
                        and max(0, x_lo) <= nx < min(w, x_hi + 1)
                        and coat_mask[ny][nx]
                        and not visited[ny][nx]
                    ):
                        visited[ny][nx] = True
                        q.append((nx, ny))
            best = max(best, size)
    main_frac = best / max(1, coat_px)
    return {
        "chest_total": float(chest_total),
        "coat_px": float(coat_px),
        "coverage": coverage,
        "main_component_frac": main_frac,
        "chest_y0": float(chest_y0),
        "chest_y1": float(chest_y1),
    }


def assert_front_coat_render(png: Path, *, min_coverage: float = 0.38, min_main_frac: float = 0.50) -> None:
    m = analyze_front_coat(png)
    if m["coverage"] < min_coverage:
        raise RuntimeError(
            f"FAIL researcher_hero_render_selfcheck coat_coverage={m['coverage']:.3f} "
            f"want>={min_coverage} coat_px={m['coat_px']:.0f} chest_total={m['chest_total']:.0f}"
        )
    if m["main_component_frac"] < min_main_frac:
        raise RuntimeError(
            f"FAIL researcher_hero_render_selfcheck coat_floaters "
            f"main_frac={m['main_component_frac']:.3f} want>={min_main_frac}"
        )
    print(
        "OK researcher_hero_render_selfcheck "
        f"coverage={m['coverage']:.3f} coat_px={m['coat_px']:.0f} "
        f"chest_total={m['chest_total']:.0f} main_frac={m['main_component_frac']:.3f}"
    )


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: researcher_hero_render_selfcheck.py <front.png>", file=sys.stderr)
        return 1
    assert_front_coat_render(Path(sys.argv[1]))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())