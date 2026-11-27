#!/usr/bin/env python3
"""v13: founder mask overlays stock check (same pose, one head, two hands, no gaps)."""
from __future__ import annotations

import sys
from collections import deque
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
AID = "staff-founder-idle"
FOUNDER = REPO / "docs/art/previews" / f"{AID}.png"
STOCK = REPO / "docs/art/previews/staff-founder-kenney-stock-check.png"
MIN_IOU = 0.90
MIN_HAND_BLOBS = 2
MAX_HAND_BLOBS = 2
MIN_HEAD_BLOBS = 1
MAX_HEAD_BLOBS = 2
MAX_ORPHAN_PX = 75


def _mask_blobs(mask: Image.Image, y0: int, y1: int, min_px: int = 40) -> list[tuple[int, int, int, int, int]]:
    w, _ = mask.size
    seen = [[False] * w for _ in range(y1 - y0)]
    out: list[tuple[int, int, int, int, int]] = []

    def on(x: int, y: int) -> bool:
        return mask.getpixel((x, y)) > 48

    for y in range(y0, y1):
        for x in range(w):
            if seen[y - y0][x] or not on(x, y):
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            seen[y - y0][x] = True
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
                    if nx < 0 or ny < y0 or ny >= y1 or nx >= w or seen[ny - y0][nx]:
                        continue
                    if not on(nx, ny):
                        continue
                    seen[ny - y0][nx] = True
                    q.append((nx, ny))
            if n >= min_px:
                out.append((n, minx, miny, maxx, maxy))
    return out


def _iou(m1: Image.Image, m2: Image.Image, *, y0: int = 0, y1: int | None = None) -> float:
    if m1.size != m2.size:
        m2 = m2.resize(m1.size, Image.Resampling.NEAREST)
    w, h = m1.size
    y1 = h if y1 is None else y1
    inter = union = 0
    for y in range(y0, y1):
        for x in range(w):
            a = m1.getpixel((x, y)) > 48
            b = m2.getpixel((x, y)) > 48
            if a or b:
                union += 1
            if a and b:
                inter += 1
    return inter / union if union else 0.0


def main() -> int:
    fm = FOUNDER.with_name(FOUNDER.stem + ".mask.png")
    sm = STOCK.with_name(STOCK.stem + ".mask.png")
    if not fm.is_file() or not sm.is_file():
        print(f"FAIL {AID} silhouette_v13 missing_mask")
        return 1
    founder_m = Image.open(fm).convert("L")
    stock_m = Image.open(sm).convert("L")
    w, h = founder_m.size
    ys = [y for y in range(h) for x in range(w) if founder_m.getpixel((x, y)) > 48]
    if not ys:
        print(f"FAIL {AID} silhouette_v13 empty_founder")
        return 1
    min_y, max_y = min(ys), max(ys)
    span = max(1, max_y - min_y)
    iou = _iou(founder_m, stock_m)
    if iou < MIN_IOU:
        print(f"FAIL {AID} silhouette_v13 iou={iou:.3f} min={MIN_IOU}")
        return 1
    head_lo = min_y
    head_hi = min_y + int(span * 0.22)
    hand_lo = min_y + int(span * 0.42)
    hand_hi = min_y + int(span * 0.68)
    head_blobs = _mask_blobs(founder_m, head_lo, head_hi, min_px=120)
    hand_blobs = _mask_blobs(founder_m, hand_lo, hand_hi, min_px=80)
    if not (MIN_HEAD_BLOBS <= len(head_blobs) <= MAX_HEAD_BLOBS):
        print(f"FAIL {AID} silhouette_v13 head_blobs={len(head_blobs)}")
        return 1
    cx = w // 2
    left_hand = right_hand = 0
    for y in range(hand_lo, hand_hi):
        for x in range(w):
            if founder_m.getpixel((x, y)) <= 48:
                continue
            if x < cx - 6:
                left_hand += 1
            elif x > cx + 6:
                right_hand += 1
    if left_hand < 120 or right_hand < 120:
        print(
            f"FAIL {AID} silhouette_v13 hands L={left_hand} R={right_hand} "
            f"blobs={len(hand_blobs)}"
        )
        return 1
    # body orphans (exclude floor)
    floor_cut = int(h * 0.74)
    comps = _mask_blobs(founder_m, 0, floor_cut, min_px=MAX_ORPHAN_PX)
    comps.sort(reverse=True, key=lambda c: c[0])
    orphans = [c for c in comps[1:] if c[0] >= MAX_ORPHAN_PX and c[2] < floor_cut]
    if orphans:
        print(f"FAIL {AID} silhouette_v13 orphan_blobs={[c[0] for c in orphans[:5]]}")
        return 1
    print(
        f"OK {AID} silhouette_v13 iou={iou:.3f} head_blobs={len(head_blobs)} "
        f"hand_blobs={len(hand_blobs)}"
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())