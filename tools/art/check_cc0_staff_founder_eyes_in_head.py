#!/usr/bin/env python3
"""Every dark eye pixel must lie on the head silhouette mask (idle + vs-stock founder)."""
from __future__ import annotations

import sys
from collections import deque
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
MAX_EYE_LUM = 96
MIN_BLOB_PX = 4
MASK_THRESH = 48

IDLE_PNG = REPO / "docs/art/previews/staff-founder-idle.png"
IDLE_MASK = REPO / "docs/art/previews/staff-founder-idle.mask.png"
VS_PNG = REPO / "docs/art/previews/staff-founder-vs-stock-check.png"
IDLE_ONLY = REPO / "docs/art/previews/staff-founder-idle.png"


def _figure_span(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > MASK_THRESH]
    if not ys:
        return 0, 0, 0
    return min(ys), max(ys), max(1, max(ys) - min(ys))


def _is_eye_pixel(r: int, g: int, b: int) -> bool:
    return max(r, g, b) <= MAX_EYE_LUM and (r + g + b) < 140


def _dark_blobs(y0: int, y1: int, img: Image.Image, mask: Image.Image) -> list[set[tuple[int, int]]]:
    w, _ = img.size
    seen = [[False] * w for _ in range(y1 - y0)]
    blobs: list[set[tuple[int, int]]] = []

    for y in range(y0, y1):
        for x in range(w):
            yi = y - y0
            if seen[yi][x]:
                continue
            r, g, b = img.getpixel((x, y))[:3]
            if not _is_eye_pixel(r, g, b):
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            seen[yi][x] = True
            cells: set[tuple[int, int]] = set()
            while q:
                cx, cy = q.popleft()
                cells.add((cx, cy))
                for dx, dy in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                    nx, ny = cx + dx, cy + dy
                    if ny < y0 or ny >= y1 or nx < 0 or nx >= w:
                        continue
                    nyi = ny - y0
                    if seen[nyi][nx]:
                        continue
                    rr, gg, bb = img.getpixel((nx, ny))[:3]
                    if not _is_eye_pixel(rr, gg, bb):
                        continue
                    seen[nyi][nx] = True
                    q.append((nx, ny))
            if len(cells) >= MIN_BLOB_PX:
                blobs.append(cells)
    return blobs


def _check_pair(label: str, img: Image.Image, mask: Image.Image, x_off: int = 0) -> tuple[bool, str]:
    min_y, _max_y, span = _figure_span(mask)
    if span < 30:
        return False, "empty_mask"
    y0 = min_y + int(span * 0.44)
    y1 = min_y + int(span * 0.76)
    blobs = _dark_blobs(y0, y1, img, mask)
    if len(blobs) < 2:
        return False, f"blobs={len(blobs)}"
    outside = 0
    total_eye = 0
    for cells in sorted(blobs, key=len, reverse=True)[:3]:
        for x, y in cells:
            total_eye += 1
            mx = x - x_off
            if mx < 0 or mx >= mask.size[0]:
                outside += 1
                continue
            if mask.getpixel((mx, y)) <= MASK_THRESH:
                outside += 1
    if outside > 0:
        return False, f"outside_mask_px={outside} eye_px={total_eye} blobs={len(blobs)}"
    if len(blobs) < 2:
        return False, "need_two_blobs"
    big = sorted(blobs, key=len, reverse=True)[:2]
    lx = sum(x for x, _ in big[0]) / len(big[0])
    rx = sum(x for x, _ in big[1]) / len(big[1])
    if abs(lx - rx) < span * 0.035:
        return False, "eyes_too_close"
    return True, f"eye_px={total_eye} L={lx:.0f} R={rx:.0f}"


def main() -> int:
    rc = 0
    if not IDLE_PNG.is_file() or not IDLE_MASK.is_file():
        print("FAIL staff-founder-idle eyes_in_head idle missing_png")
        return 1
    idle_img = Image.open(IDLE_PNG).convert("RGB")
    idle_mask = Image.open(IDLE_MASK).convert("L")
    ok, msg = _check_pair("idle", idle_img, idle_mask)
    if ok:
        print(f"OK staff-founder-idle eyes_in_head idle {msg}")
    else:
        print(f"FAIL staff-founder-idle eyes_in_head idle {msg}")
        rc = 1

    if VS_PNG.is_file() and IDLE_PNG.is_file() and IDLE_MASK.is_file():
        vs = Image.open(VS_PNG).convert("RGB")
        ref = Image.open(IDLE_PNG)
        gap = 8
        x0 = ref.width + gap
        if vs.width <= x0:
            print("FAIL staff-founder-idle eyes_in_head vs-stock bad_layout")
            rc = 1
        else:
            founder_crop = vs.crop((x0, 0, vs.width, vs.height))
            if founder_crop.size != ref.size:
                founder_crop = founder_crop.resize(ref.size, Image.Resampling.LANCZOS)
            ok2, msg2 = _check_pair("vs-stock", founder_crop, idle_mask)
            if ok2:
                print(f"OK staff-founder-idle eyes_in_head vs-stock {msg2}")
            else:
                print(f"FAIL staff-founder-idle eyes_in_head vs-stock {msg2}")
                rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())