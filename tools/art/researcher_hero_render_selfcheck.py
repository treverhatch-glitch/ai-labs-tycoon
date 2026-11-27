#!/usr/bin/env python3
"""Hatch coat metrics from real preview PNGs (0° diagnostic + axis front tile)."""
from __future__ import annotations

import sys
import tempfile
from collections import deque
from pathlib import Path

from PIL import Image, ImageFilter

REPO = Path(__file__).resolve().parents[2]
HATCH_MIN_COVERAGE = 0.70
HATCH_COAT_ISLANDS = 1
MIN_ISLAND_PX = 48


def _offwhite_coat(r: int, g: int, b: int) -> bool:
    chroma = max(r, g, b) - min(r, g, b)
    if g > r + 20 and b > r - 5 and r < 100:
        return False
    if b > r + 12 and r < 70:
        return False
    if r >= 198 and g >= 196 and b >= 188 and chroma < 32:
        return True
    if r >= 172 and g >= 170 and b >= 164 and chroma < 40:
        return r >= g - 8 and r >= b - 10
    if r >= 215 and g >= 212 and b >= 205 and chroma < 18:
        return True
    return r >= 118 and g >= 115 and b >= 110 and chroma < 58 and r >= g - 10


def _figure_span_from_rgb(img: Image.Image) -> tuple[int, int, int, int]:
    g = img.convert("L")
    w, h = g.size
    ys = [y for y in range(h) for x in range(w) if g.getpixel((x, y)) < 235]
    if not ys:
        return 0, h - 1, 0, w - 1
    xs = [x for y in ys for x in range(w) if g.getpixel((x, y)) < 235]
    return min(ys), max(ys), min(xs), max(xs)


def _figure_span_mask(mask: Image.Image) -> tuple[int, int, int, int]:
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


def analyze_front_coat_png(
    png: Path,
    *,
    mask: Image.Image | None = None,
    tag: str = "",
) -> dict[str, float | str]:
    img = Image.open(png).convert("RGB")
    if mask is None:
        mask_p = png.with_name(png.stem + ".mask.png")
        if mask_p.is_file():
            mask = Image.open(mask_p).convert("L")
    w, h = img.size
    if mask is not None:
        min_y, max_y, min_x, max_x = _figure_span_mask(mask)
    else:
        min_y, max_y, min_x, max_x = _figure_span_from_rgb(img)
    span = max(1, max_y - min_y)
    cx = (min_x + max_x) // 2
    band_w = max(12, (max_x - min_x) * 2 // 5)
    x_lo, x_hi = cx - band_w, cx + band_w
    chest_y0 = max_y - int(span * 0.56)
    chest_y1 = max_y - int(span * 0.32)
    open_half = max(4, (max_x - min_x) // 18)
    chest_total = 0
    coat_px = 0
    coat_mask = [[False] * w for _ in range(h)]
    for y in range(chest_y0, chest_y1 + 1):
        for x in range(max(0, x_lo), min(w, x_hi + 1)):
            if mask is not None and mask.getpixel((x, y)) <= 48:
                continue
            if mask is None:
                bg = img.getpixel((0, 0))
                r, g, b = img.getpixel((x, y))
                if abs(r - bg[0]) + abs(g - bg[1]) + abs(b - bg[2]) < 28:
                    continue
            if abs(x - cx) < open_half and y > chest_y0 + (chest_y1 - chest_y0) // 5:
                continue
            chest_total += 1
            r, g, b = img.getpixel((x, y))
            if _offwhite_coat(r, g, b):
                coat_px += 1
                coat_mask[y][x] = True
    coverage = coat_px / max(1, chest_total)
    island_mask = [row[:] for row in coat_mask]
    for y in range(chest_y0, chest_y1 + 1):
        for x in range(max(0, x_lo), min(w, x_hi + 1)):
            if not coat_mask[y][x]:
                continue
            for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, 1)):
                nx, ny = x + dx, y + dy
                if chest_y0 <= ny <= chest_y1 and 0 <= nx < w:
                    island_mask[ny][nx] = True
    coat_islands = _count_islands(
        island_mask, chest_y0, chest_y1, max(0, x_lo), x_hi, w
    )
    return {
        "tag": tag or png.name,
        "chest_total": float(chest_total),
        "coat_px": float(coat_px),
        "coverage": coverage,
        "coat_islands": float(coat_islands),
    }


def _axis_front_crop(sheet: Path) -> Image.Image:
    im = Image.open(sheet).convert("RGB")
    tw, th = im.size[0] // 2, im.size[1] // 2
    return im.crop((0, 0, tw, th))


def _synthetic_mask_from_rgb(img: Image.Image) -> Image.Image:
    bg = img.getpixel((8, 8))
    w, h = img.size
    m = Image.new("L", (w, h))
    for y in range(h):
        for x in range(w):
            r, g, b = img.getpixel((x, y))
            d = abs(r - bg[0]) + abs(g - bg[1]) + abs(b - bg[2])
            m.putpixel((x, y), 255 if d > 35 else 0)
    return m.filter(ImageFilter.MaxFilter(3))


def assert_hatch_coat_metrics(diag_png: Path, axis_sheet: Path) -> dict[str, dict]:
    results: dict[str, dict] = {}
    results["diagnostic_0deg"] = analyze_front_coat_png(diag_png, tag="face-diagnostic-0deg")
    front = _axis_front_crop(axis_sheet)
    with tempfile.TemporaryDirectory(prefix="coat_metric_") as td:
        tile = Path(td) / "axis-front0.png"
        mask = Path(td) / "axis-front0.mask.png"
        front.save(tile)
        _synthetic_mask_from_rgb(front).save(mask)
        results["axis_0deg"] = analyze_front_coat_png(
            tile, mask=Image.open(mask).convert("L"), tag="axis-sheet-0deg"
        )
    d = results["diagnostic_0deg"]
    if d["coverage"] < HATCH_MIN_COVERAGE:
        raise RuntimeError(
            f"FAIL researcher_hero_render_selfcheck diagnostic_0deg "
            f"coverage={d['coverage']:.3f} want>={HATCH_MIN_COVERAGE}"
        )
    if d["coat_islands"] > HATCH_COAT_ISLANDS:
        raise RuntimeError(
            f"FAIL researcher_hero_render_selfcheck diagnostic_0deg "
            f"coat_islands={int(d['coat_islands'])} want={HATCH_COAT_ISLANDS}"
        )
    a = results["axis_0deg"]
    if a["coat_islands"] > HATCH_COAT_ISLANDS + 1:
        raise RuntimeError(
            f"FAIL researcher_hero_render_selfcheck axis_0deg "
            f"coat_islands={int(a['coat_islands'])} want<={HATCH_COAT_ISLANDS + 1}"
        )
    d = results["diagnostic_0deg"]
    a = results["axis_0deg"]
    print(
        "OK researcher_hero_render_selfcheck "
        f"diagnostic coverage={d['coverage']:.3f} islands={int(d['coat_islands'])} "
        f"axis0 coverage={a['coverage']:.3f} islands={int(a['coat_islands'])}"
    )
    return results


def main() -> int:
    previews = REPO / "docs" / "art" / "previews"
    diag = previews / "staff-researcher-face-diagnostic-0deg.png"
    axis = previews / "staff-researcher-axis-sheet.png"
    idle = previews / "staff-researcher-idle.png"
    if len(sys.argv) >= 2 and sys.argv[1] == "--report-only":
        for label, p in (
            ("idle", idle),
            ("diagnostic_0deg", diag),
        ):
            if not p.is_file():
                print(f"skip {label} missing {p.name}")
                continue
            m = analyze_front_coat_png(p, tag=label)
            print(
                f"METRIC {label} coverage={m['coverage']:.3f} "
                f"islands={int(m['coat_islands'])} coat_px={m['coat_px']:.0f} "
                f"chest_total={m['chest_total']:.0f}"
            )
        if axis.is_file():
            front = _axis_front_crop(axis)
            mask = _synthetic_mask_from_rgb(front)
            with tempfile.TemporaryDirectory(prefix="coat_report_") as td:
                tile = Path(td) / "axis0.png"
                front.save(tile)
                m = analyze_front_coat_png(tile, mask=mask, tag="axis_0deg_crop")
            print(
                f"METRIC axis_0deg coverage={m['coverage']:.3f} "
                f"islands={int(m['coat_islands'])} coat_px={m['coat_px']:.0f}"
            )
        return 0
    if not diag.is_file() or not axis.is_file():
        print("FAIL missing diagnostic or axis sheet png", file=sys.stderr)
        return 1
    assert_hatch_coat_metrics(diag, axis)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())