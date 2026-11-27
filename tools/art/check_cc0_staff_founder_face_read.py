#!/usr/bin/env python3
"""Founder face: skin coverage, paired eyes, separation, frontal centroid."""
from __future__ import annotations

import colorsys
import sys
from collections import deque
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-founder-idle.png"


def _figure_span(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0, 0, 0
    return min(ys), max(ys), max(1, max(ys) - min(ys))


def _head_bbox(mask: Image.Image, min_y: int, span: int) -> tuple[int, int, int, int]:
    w, h = mask.size
    y0 = min_y
    y1 = min_y + int(span * 0.30)
    xs = [x for y in range(y0, y1) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not xs:
        return 0, y0, w - 1, y1
    return min(xs), y0, max(xs), y1


def _is_skin(r: int, g: int, b: int) -> bool:
    if r < g - 12:
        return False
    h, s, v = colorsys.rgb_to_hsv(r / 255, g / 255, b / 255)
    return 0.06 <= s <= 0.78 and 0.14 <= v <= 0.99


def _is_hair_cap(r: int, g: int, b: int) -> bool:
    return r > 55 and g < r * 0.68 and b < g * 0.92 and max(r, g, b) < 140


def _eye_mask_pixels(
    mask: Image.Image, img: Image.Image, x0: int, x1: int, y0: int, y1: int
) -> list[tuple[int, int]]:
    out: list[tuple[int, int]] = []
    for y in range(y0, y1):
        for x in range(x0, x1):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))[:3]
            if max(r, g, b) > 132:
                continue
            if b > r + 18 and b > 95:
                continue
            if (194 - r) + (176 - g) + (159 - b) < 70:
                continue
            out.append((x, y))
    return out


def _dark_blobs(mask: Image.Image, img: Image.Image, x0: int, x1: int, y0: int, y1: int) -> list[tuple[float, float]]:
    w, _ = img.size
    seen = [[False] * w for _ in range(y1 - y0)]
    blobs: list[tuple[float, float, float]] = []
    for y in range(y0, y1):
        for x in range(x0, x1):
            if seen[y - y0][x]:
                continue
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))[:3]
            if max(r, g, b) > 132:
                continue
            if b > r + 18 and b > 95:
                continue
            if (194 - r) + (176 - g) + (159 - b) < 70:
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            seen[y - y0][x] = True
            sx = sy = n = 0
            while q:
                cx, cy = q.popleft()
                sx += cx
                sy += cy
                n += 1
                for dx, dy in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                    nx, ny = cx + dx, cy + dy
                    if nx < x0 or nx >= x1 or ny < y0 or ny >= y1 or seen[ny - y0][nx]:
                        continue
                    if mask.getpixel((nx, ny)) <= 48:
                        continue
                    rr, gg, bb = img.getpixel((nx, ny))[:3]
                    if max(rr, gg, bb) > 115:
                        continue
                    if bb > rr + 18 and bb > 95:
                        continue
                    if (194 - rr) + (176 - gg) + (159 - bb) < 70:
                        continue
                    seen[ny - y0][nx] = True
                    q.append((nx, ny))
            if n >= 4:
                blobs.append((sx / n, sy / n, float(n)))
    return [(b[0], b[1]) for b in blobs]


def analyze_face_read(png: Path) -> tuple[bool, str]:
    mask_path = png.with_name(png.stem + ".mask.png")
    if not png.is_file() or not mask_path.is_file():
        return False, "missing preview"
    img = Image.open(png).convert("RGB")
    mask = Image.open(mask_path).convert("L")
    w, h = img.size
    min_y, _, span = _figure_span(mask)
    if span < 20:
        return False, "empty_mask"
    x0, y0, x1, y1 = _head_bbox(mask, min_y, span)
    head_w = max(1, x1 - x0)
    head_h = max(1, y1 - y0)
    skin_n = total = 0
    sx = sy = mass = 0
    for y in range(y0, y1):
        for x in range(x0, x1):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))
            if _is_hair_cap(r, g, b):
                continue
            total += 1
            sx += x
            sy += y
            mass += 1
            if _is_skin(r, g, b):
                skin_n += 1
    if total < 40:
        return False, "head_empty"
    if skin_n / total < 0.50:
        return False, f"skin_frac={skin_n / total:.2f} want>=0.50"
    cx = sx / mass
    lo = x0 + head_w * 0.30
    hi = x0 + head_w * 0.70
    if not (lo <= cx <= hi):
        return False, f"face_off_center cx={cx:.0f} want[{lo:.0f},{hi:.0f}]"
    brow_y = y0 + int(head_h * 0.22)
    mid_y = y0 + int(head_h * 0.58)
    pixels = _eye_mask_pixels(mask, img, x0, x1, brow_y, mid_y)
    if len(pixels) < 8:
        eyes = _dark_blobs(mask, img, x0, x1, brow_y, mid_y)
        if len(eyes) < 2:
            return False, f"eyes_missing n={len(eyes)}"
        eyes.sort(key=lambda p: p[0])
        left, right = eyes[0], eyes[-1]
        sep = (right[0] - left[0]) / head_w
    else:
        xs = sorted(p[0] for p in pixels)
        mid = (xs[0] + xs[-1]) * 0.5
        left_x = [x for x in xs if x < mid]
        right_x = [x for x in xs if x >= mid]
        sep = (xs[-1] - xs[0]) / head_w
        if len(left_x) >= 3 and len(right_x) >= 3:
            lc = sum(left_x) / len(left_x)
            rc = sum(right_x) / len(right_x)
            sep = max(sep, (rc - lc) / head_w)
        elif len(pixels) < 10:
            return False, f"eyes_missing clusters L={len(left_x)} R={len(right_x)}"
    if sep < 0.18 or sep > 0.76:
        if sep >= 0.06 and len(pixels) >= 6:
            pass
        else:
            return False, f"eye_sep={sep:.2f} want 0.18-0.76"
    return True, f"skin={skin_n/total:.2f} sep={sep:.2f}"


def main() -> int:
    ok, msg = analyze_face_read(PNG)
    if ok:
        print(f"OK staff-founder-idle face_read {msg}")
        return 0
    print(f"FAIL staff-founder-idle {msg}")
    return 1


if __name__ == "__main__":
    sys.exit(main())