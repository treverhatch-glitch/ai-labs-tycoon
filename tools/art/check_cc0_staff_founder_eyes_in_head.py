#!/usr/bin/env python3
"""Eye mesh blobs must sit inside the head silhouette (idle + 0° diagnostic)."""
from __future__ import annotations

import sys
from collections import deque
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
MAX_EYE_LUM = 96
MIN_BLOB_PX = 6

PAIRS = (
    ("staff-founder-idle", True),
    ("staff-founder-face-diagnostic-0deg", False),
)


def _figure_span(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, 0, 0
    return min(ys), max(ys), max(1, max(ys) - min(ys))


def _eye_blobs(mask: Image.Image, img: Image.Image, y0: int, y1: int) -> list[tuple[float, float, int]]:
    w, _ = img.size
    seen = [[False] * w for _ in range(y1 - y0)]
    out: list[tuple[float, float, int]] = []

    def dark(x: int, y: int) -> bool:
        if mask.getpixel((x, y)) <= 48:
            pass  # eye meshes may sit just proud of body mask
        r, g, b = img.getpixel((x, y))[:3]
        return max(r, g, b) <= MAX_EYE_LUM and (r + g + b) < 140

    for y in range(y0, y1):
        for x in range(w):
            yi = y - y0
            if seen[yi][x] or not dark(x, y):
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            seen[yi][x] = True
            sx = sy = n = 0
            while q:
                cx, cy = q.popleft()
                sx += cx
                sy += cy
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
                out.append((sx / n, sy / n, n))
    return out


def _row_head_span(mask: Image.Image, y: int) -> tuple[int, int] | None:
    w, _ = mask.size
    xs = [x for x in range(w) if mask.getpixel((x, y)) > 48]
    if len(xs) < 8:
        return None
    return min(xs), max(xs)


def _inside_head_row(mask: Image.Image, cx: float, cy: float, pad: int = 6) -> bool:
    y = int(cy)
    span = _row_head_span(mask, y)
    if span is None:
        for dy in (-3, -2, -1, 1, 2, 3):
            span = _row_head_span(mask, y + dy)
            if span:
                break
    if span is None:
        return False
    x0, x1 = span
    return (x0 - pad) <= cx <= (x1 + pad)


def _check_asset(stem: str, use_silo: bool) -> tuple[bool, str]:
    png = REPO / "docs/art/previews" / f"{stem}.png"
    mask_path = png.with_name(stem + ".mask.png")
    if use_silo:
        silo = png.with_name(stem + ".silhouette.mask.png")
        if silo.is_file():
            mask_path = silo
    if not png.is_file() or not mask_path.is_file():
        return False, "missing_png"
    img = Image.open(png).convert("RGB")
    mask = Image.open(mask_path).convert("L")
    min_y, _max_y, span = _figure_span(mask)
    if span < 30:
        return False, "empty_mask"
    band_y0 = min_y + int(span * 0.45)
    band_y1 = min_y + int(span * 0.72)
    blobs = _eye_blobs(mask, img, band_y0, band_y1)
    if len(blobs) < 2:
        return False, f"blobs={len(blobs)}"
    inside_blobs = [b for b in blobs if _inside_head_row(mask, b[0], b[1])]
    if len(inside_blobs) < 2:
        return False, f"inside_head={len(inside_blobs)} blobs={len(blobs)}"
    best_pair: tuple[tuple[float, float, int], tuple[float, float, int]] | None = None
    best_sep = -1.0
    for i, a in enumerate(inside_blobs):
        for b in inside_blobs[i + 1 :]:
            sep = abs(a[0] - b[0])
            if sep > best_sep:
                best_sep = sep
                best_pair = (a, b)
    if not best_pair:
        return False, "no_pair"
    lx, rx = sorted(best_pair, key=lambda t: t[0])
    min_sep = span * (0.03 if stem.endswith("0deg") else 0.04)
    if (rx[0] - lx[0]) < min_sep:
        return False, f"eyes_too_close sep={rx[0]-lx[0]:.0f} min={min_sep:.0f}"
    return True, f"inside=2 L=({lx[0]:.0f},{lx[1]:.0f}) R=({rx[0]:.0f},{rx[1]:.0f})"


def main() -> int:
    rc = 0
    for stem, silo in PAIRS:
        ok, msg = _check_asset(stem, silo)
        if ok:
            print(f"OK staff-founder-idle eyes_in_head {stem} {msg}")
        else:
            print(f"FAIL staff-founder-idle eyes_in_head {stem} {msg}")
            rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())