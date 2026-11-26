#!/usr/bin/env python3
"""Preview gates: corner RGB 150 (3px inset), ~70% fill, distinct hero/scene."""
from __future__ import annotations

import sys
from collections import Counter
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
HERO = REPO / "docs/art/previews/garage-interior-room-kit.png"
SCENE = REPO / "docs/art/previews/garage-interior-room-kit-scene.png"
FIXTURE_FAIL = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-620a0e37.png"
CORNER_RGB = (150, 150, 150)
CORNER_INSET = 3


def luma(rgb: tuple[int, int, int]) -> float:
    r, g, b = rgb
    return 0.299 * r + 0.587 * g + 0.114 * b


def corner_rgb_150_errs(im) -> list[str]:
    w, h = im.size
    inset = CORNER_INSET
    corners = [
        (inset, inset),
        (w - 1 - inset, inset),
        (inset, h - 1 - inset),
        (w - 1 - inset, h - 1 - inset),
    ]
    errs: list[str] = []
    for c in corners:
        p = im.getpixel(c)
        if p != CORNER_RGB:
            errs.append(f"FAIL: corner {c} RGB {p} != {CORNER_RGB}")
    return errs


def saturation(rgb: tuple[int, int, int]) -> float:
    r, g, b = (c / 255.0 for c in rgb)
    mx, mn = max(r, g, b), min(r, g, b)
    if mx <= 1e-6:
        return 0.0
    return (mx - mn) / mx


def is_background(rgb: tuple[int, int, int]) -> bool:
    if rgb == CORNER_RGB:
        return True
    L = luma(rgb)
    sat = saturation(rgb)
    return sat < 0.08 and 142 <= L <= 158


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
    xs: list[int] = []
    ys: list[int] = []
    for y in range(h):
        for x in range(w):
            px = im.getpixel((x, y))
            if px == CORNER_RGB:
                continue
            L = luma(px)
            sat = saturation(px)
            if 135 <= L <= 175 and sat < 0.16:
                continue
            xs.append(x)
            ys.append(y)
    errs: list[str] = []
    if not xs:
        errs.append(f"FAIL: {path.name} no subject pixels for fill")
        return errs
    bbox_frac = ((max(xs) - min(xs) + 1) * (max(ys) - min(ys) + 1)) / (w * h)
    target_lo = fill - 0.14
    target_hi = fill + (0.26 if path.name.endswith("-scene.png") else 0.20)
    if bbox_frac < target_lo or bbox_frac > target_hi:
        errs.append(f"FAIL: {path.name} subject bbox fill {bbox_frac * 100:.1f}% (target ~{fill * 100:.0f}%)")
    return errs


def check_image(path: Path) -> list[str]:
    try:
        from PIL import Image
    except ImportError:
        return [f"FAIL: Pillow required to check {path}"]
    errs: list[str] = []
    im = Image.open(path).convert("RGB")
    errs.extend(corner_rgb_150_errs(im))
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
        errs.append(f"FAIL: {path.name} no foreground pixels")
    else:
        buckets = Counter((p[0] // 16, p[1] // 16, p[2] // 16) for p in fg)
        top_frac = buckets.most_common(1)[0][1] / len(fg)
    if top_frac > 0.92 and path.name.endswith("-scene.png"):
        errs.append(f"FAIL: {path.name} flat bucket {top_frac * 100:.1f}% > 92%")
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


def verify_owner_fixture_fails() -> list[str]:
    if not FIXTURE_FAIL.is_file():
        return [f"FAIL: missing owner fail fixture {FIXTURE_FAIL}"]
    try:
        from PIL import Image
    except ImportError:
        return [f"FAIL: Pillow required for fixture check"]
    im = Image.open(FIXTURE_FAIL).convert("RGB")
    corner_errs = corner_rgb_150_errs(im)
    if not corner_errs:
        return ["FAIL: owner fixture 620a0e37 must fail corner RGB 150 gate"]
    return []


def main() -> int:
    all_errs: list[str] = []
    all_errs.extend(verify_owner_fixture_fails())
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