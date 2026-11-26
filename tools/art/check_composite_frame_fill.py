#!/usr/bin/env python3
"""Composites must fill 35–75% frame height (not tiny on white)."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
BG = (122, 122, 128)
MIN_FILL = 0.35
MAX_FILL = 0.88


def measure(path: Path) -> tuple[float, float]:
    from PIL import Image

    im = Image.open(path).convert("RGB")
    w, h = im.size
    col_w = max(8, w // 8)
    fills: list[float] = []
    for col in range(w // col_w):
        x0 = col * col_w
        crop = im.crop((x0, 0, x0 + col_w, h))
        px = list(crop.getdata())
        ys = [y for y in range(h) for x in range(col_w) if px[y * col_w + x] != BG]
        if not ys:
            fills.append(0.0)
            continue
        fills.append((max(ys) - min(ys) + 1) / h)
    return (min(fills) if fills else 0.0, sum(fills) / len(fills) if fills else 0.0)


def main() -> int:
    rc = 0
    for name, lo in (
        ("staff-lineup-packet2.png", MIN_FILL),
        ("wisp-moods-row.png", MIN_FILL),
    ):
        p = PREVIEW / name
        if not p.is_file():
            print(f"FAIL missing {name}")
            rc = 1
            continue
        min_f, avg = measure(p)
        if min_f < lo or avg > MAX_FILL:
            print(f"FAIL {name} min_fill={min_f:.2f} avg={avg:.2f}")
            rc = 1
        else:
            print(f"OK {name} min_fill={min_f:.2f} avg={avg:.2f}")
    return rc


if __name__ == "__main__":
    sys.exit(main())