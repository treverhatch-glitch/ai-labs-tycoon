#!/usr/bin/env python3
"""Fail if hero preview PNG is black/flat (Hatch gate)."""
from __future__ import annotations

import sys
from collections import Counter
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEWS = [
    REPO / "docs/art/previews/garage-interior-room-kit.png",
    REPO / "docs/art/previews/garage-interior-room-kit-scene.png",
]


def luma(rgb: tuple[int, int, int]) -> float:
    r, g, b = rgb
    return 0.299 * r + 0.587 * g + 0.114 * b


def check_image(path: Path) -> list[str]:
    try:
        from PIL import Image
    except ImportError:
        return [f"FAIL: Pillow required to check {path}"]
    errs: list[str] = []
    im = Image.open(path).convert("RGB")
    pixels = list(im.getdata())
    if not pixels:
        return [f"FAIL: empty image {path}"]
    lumas = [luma(p) for p in pixels]
    mean_l = sum(lumas) / len(lumas)
    if mean_l < 40 or mean_l > 230:
        errs.append(f"FAIL: {path.name} mean luminance {mean_l:.1f} (need 40–230)")
    # quantize to 8-ish levels per channel to detect flat fields
    buckets = Counter((p[0] // 16, p[1] // 16, p[2] // 16) for p in pixels)
    top_frac = buckets.most_common(1)[0][1] / len(pixels)
    if top_frac > 0.50:
        errs.append(f"FAIL: {path.name} flat bucket {top_frac * 100:.1f}% > 50%")
    if max(pixels, key=lambda p: sum(p)) == (0, 0, 0) and mean_l < 5:
        errs.append(f"FAIL: {path.name} appears fully black")
    return errs


def main() -> int:
    all_errs: list[str] = []
    for path in PREVIEWS:
        if not path.is_file():
            all_errs.append(f"FAIL: missing {path}")
            continue
        all_errs.extend(check_image(path))
    if all_errs:
        for e in all_errs:
            print(e)
        return 1
    print("OK: CC0 garage preview luminance gate")
    return 0


if __name__ == "__main__":
    sys.exit(main())