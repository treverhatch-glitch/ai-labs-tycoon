#!/usr/bin/env python3
"""Preview gates: luminance, flat color, ~70% subject + 5% margin, distinct hero/scene."""
from __future__ import annotations

import sys
from collections import Counter
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
HERO = REPO / "docs/art/previews/garage-interior-room-kit.png"
SCENE = REPO / "docs/art/previews/garage-interior-room-kit-scene.png"


def luma(rgb: tuple[int, int, int]) -> float:
    r, g, b = rgb
    return 0.299 * r + 0.587 * g + 0.114 * b


def saturation(rgb: tuple[int, int, int]) -> float:
    r, g, b = (c / 255.0 for c in rgb)
    mx, mn = max(r, g, b), min(r, g, b)
    if mx <= 1e-6:
        return 0.0
    return (mx - mn) / mx


def is_background(rgb: tuple[int, int, int]) -> bool:
    """Mid-grey studio backdrop + blown HDRI spill only."""
    L = luma(rgb)
    sat = saturation(rgb)
    if sat < 0.07 and 142 <= L <= 168:
        return True
    if sat < 0.05 and L >= 228:
        return True
    return False


def _background_mask(im):
    from collections import deque

    w, h = im.size
    bg = [[False] * w for _ in range(h)]
    q: deque[tuple[int, int]] = deque()
    for x in range(w):
        for y in (0, h - 1):
            if is_background(im.getpixel((x, y))):
                bg[y][x] = True
                q.append((x, y))
    for y in range(h):
        for x in (0, w - 1):
            if is_background(im.getpixel((x, y))):
                if not bg[y][x]:
                    bg[y][x] = True
                    q.append((x, y))
    while q:
        x, y = q.popleft()
        for nx, ny in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
            if 0 <= nx < w and 0 <= ny < h and not bg[ny][nx]:
                if is_background(im.getpixel((nx, ny))):
                    bg[ny][nx] = True
                    q.append((nx, ny))
    return bg


def fill_margin_errs(path: Path, fill: float = 0.70, margin: float = 0.05) -> list[str]:
    try:
        from PIL import Image
    except ImportError:
        return [f"FAIL: Pillow required for {path}"]
    im = Image.open(path).convert("RGB")
    w, h = im.size
    mask = _background_mask(im)
    fg_count = sum(1 for y in range(h) for x in range(w) if not mask[y][x])
    fg_frac = fg_count / (w * h)
    target_lo = fill - 0.22
    target_hi = fill + 0.28
    errs: list[str] = []
    if fg_frac < target_lo or fg_frac > target_hi:
        errs.append(f"FAIL: {path.name} subject fill {fg_frac * 100:.1f}% (target ~{fill * 100:.0f}%)")
    strip = max(4, int(margin * min(w, h)))
    border_bg = 0
    border_total = 0
    for x in range(w):
        for y in list(range(strip)) + list(range(h - strip, h)):
            border_total += 1
            if mask[y][x]:
                border_bg += 1
    for y in range(strip, h - strip):
        for x in list(range(strip)) + list(range(w - strip, w)):
            border_total += 1
            if mask[y][x]:
                border_bg += 1
    if border_total and border_bg / border_total < 0.10:
        errs.append(f"FAIL: {path.name} margin backdrop {border_bg / border_total * 100:.1f}% < 10%")
    return errs


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
    mask = _background_mask(im)
    w, h = im.size
    fg = [im.getpixel((x, y)) for y in range(h) for x in range(w) if not mask[y][x]]
    if not fg:
        return [f"FAIL: {path.name} no foreground pixels"]
    buckets = Counter((p[0] // 16, p[1] // 16, p[2] // 16) for p in fg)
    top_frac = buckets.most_common(1)[0][1] / len(fg)
    if top_frac > 0.50:
        errs.append(f"FAIL: {path.name} flat bucket {top_frac * 100:.1f}% > 50%")
    if max(pixels, key=lambda p: sum(p)) == (0, 0, 0) and mean_l < 5:
        errs.append(f"FAIL: {path.name} appears fully black")
    errs.extend(fill_margin_errs(path))
    return errs


def previews_differ() -> list[str]:
    try:
        from PIL import Image
    except ImportError:
        return []
    if not HERO.is_file() or not SCENE.is_file():
        return ["FAIL: missing hero or scene preview"]
    a = Image.open(HERO).convert("RGB").resize((64, 64))
    b = Image.open(SCENE).convert("RGB").resize((64, 64))
    diff = sum(abs(luma(pa) - luma(pb)) for pa, pb in zip(a.getdata(), b.getdata())) / (64 * 64)
    if diff < 4.5:
        return [f"FAIL: hero and scene previews too similar (mean luma diff {diff:.1f})"]
    return []


def main() -> int:
    all_errs: list[str] = []
    for path in (HERO, SCENE):
        if not path.is_file():
            all_errs.append(f"FAIL: missing {path}")
            continue
        all_errs.extend(check_image(path))
    all_errs.extend(previews_differ())
    if all_errs:
        for e in all_errs:
            print(e)
        return 1
    print("OK: CC0 garage preview gates")
    return 0


if __name__ == "__main__":
    sys.exit(main())