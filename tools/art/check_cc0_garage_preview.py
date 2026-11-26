#!/usr/bin/env python3
"""Preview gates: corners, exposure, saturation, grey bleed, prop materials."""
from __future__ import annotations

import sys
from collections import Counter
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
HERO = REPO / "docs/art/previews/garage-interior-room-kit.png"
SCENE = REPO / "docs/art/previews/garage-interior-room-kit-scene.png"
FIXTURE_620 = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-620a0e37.png"
FIXTURE_C78 = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-c78b9371.png"
CORNER_RGB = (150, 150, 150)
CORNER_INSET = 3
BLOWN_LUMA = 255 * 0.95


def luma(rgb: tuple[int, int, int]) -> float:
    r, g, b = rgb
    return 0.299 * r + 0.587 * g + 0.114 * b


def saturation(rgb: tuple[int, int, int]) -> float:
    r, g, b = (c / 255.0 for c in rgb)
    mx, mn = max(r, g, b), min(r, g, b)
    if mx <= 1e-6:
        return 0.0
    return (mx - mn) / mx


def is_studio_backdrop(rgb: tuple[int, int, int]) -> bool:
    if rgb == CORNER_RGB:
        return True
    L = luma(rgb)
    return saturation(rgb) < 0.10 and 125 <= L <= 156


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


def _background_mask(im):
    from collections import deque

    w, h = im.size
    bg = [[False] * w for _ in range(h)]
    q: deque[tuple[int, int]] = deque()
    for x in range(w):
        for y in (0, h - 1):
            if is_studio_backdrop(im.getpixel((x, y))):
                bg[y][x] = True
                q.append((x, y))
    for y in range(h):
        for x in (0, w - 1):
            if is_studio_backdrop(im.getpixel((x, y))):
                if not bg[y][x]:
                    bg[y][x] = True
                    q.append((x, y))
    while q:
        x, y = q.popleft()
        for nx, ny in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
            if 0 <= nx < w and 0 <= ny < h and not bg[ny][nx]:
                if is_studio_backdrop(im.getpixel((nx, ny))):
                    bg[ny][nx] = True
                    q.append((nx, ny))
    return bg


def blown_out_errs(im, path_name: str) -> list[str]:
    rgb = im.convert("RGB")
    pixels = list(rgb.getdata())
    if not pixels:
        return [f"FAIL: empty {path_name}"]
    blown = sum(1 for p in pixels if luma(p) >= BLOWN_LUMA)
    frac = blown / len(pixels)
    if frac >= 0.05:
        return [f"FAIL: {path_name} blown-out pixels {frac * 100:.2f}% >= 5%"]
    return []


def mean_saturation_fg(im, path_name: str) -> list[str]:
    w, h = im.size
    sats: list[float] = []
    for y in range(h):
        for x in range(w):
            p = im.getpixel((x, y))
            if is_studio_backdrop(p):
                continue
            if luma(p) < 88:
                continue
            s = saturation(p)
            if s < 0.05:
                continue
            sats.append(s)
    if not sats:
        return [f"FAIL: {path_name} no colored foreground for saturation"]
    mean_sat = sum(sats) / len(sats)
    if mean_sat <= 0.15:
        return [f"FAIL: {path_name} mean fg saturation {mean_sat:.3f} <= 0.15"]
    return []


def grey_bleed_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    x0, x1 = int(w * 0.22), int(w * 0.78)
    y0, y1 = int(h * 0.22), int(h * 0.78)
    bleed = 0
    total = 0
    for y in range(y0, y1):
        for x in range(x0, x1):
            total += 1
            px = im.getpixel((x, y))
            L = luma(px)
            if px == CORNER_RGB or (148 <= L <= 152 and saturation(px) < 0.08):
                bleed += 1
    if total and bleed / total > 0.02:
        return [f"FAIL: {path_name} grey bleed through floor/wall {bleed / total * 100:.2f}% > 2%"]
    return []


def prop_material_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    mask = _background_mask(im)
    buckets: set[tuple[int, int, int]] = set()
    for y in range(h):
        for x in range(w):
            if mask[y][x]:
                continue
            p = im.getpixel((x, y))
            if luma(p) >= 242:
                continue
            if saturation(p) < 0.18:
                continue
            buckets.add((p[0] // 36, p[1] // 36, p[2] // 36))
    if len(buckets) < 6:
        return [f"FAIL: {path_name} distinct prop materials {len(buckets)} < 6"]
    return []


def fill_margin_errs(path: Path, fill: float = 0.70) -> list[str]:
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
            if is_studio_backdrop(px):
                continue
            if luma(px) < 70:
                continue
            if luma(px) >= 242 and saturation(px) < 0.12:
                continue
            xs.append(x)
            ys.append(y)
    if not xs:
        return [f"FAIL: {path.name} no subject pixels for fill"]
    bbox_frac = ((max(xs) - min(xs) + 1) * (max(ys) - min(ys) + 1)) / (w * h)
    target_lo = fill - 0.16
    target_hi = fill + (0.28 if path.name.endswith("-scene.png") else 0.22)
    if bbox_frac < target_lo or bbox_frac > target_hi:
        return [f"FAIL: {path.name} subject bbox fill {bbox_frac * 100:.1f}% (target ~{fill * 100:.0f}%)"]
    return []


def quality_gates(im, path_name: str) -> list[str]:
    errs: list[str] = []
    errs.extend(blown_out_errs(im, path_name))
    errs.extend(mean_saturation_fg(im, path_name))
    errs.extend(grey_bleed_errs(im, path_name))
    errs.extend(prop_material_errs(im, path_name))
    return errs


def check_image(path: Path) -> list[str]:
    try:
        from PIL import Image
    except ImportError:
        return [f"FAIL: Pillow required to check {path}"]
    im = Image.open(path).convert("RGB")
    errs = corner_rgb_150_errs(im)
    errs.extend(quality_gates(im, path.name))
    errs.extend(fill_margin_errs(path))
    pixels = list(im.getdata())
    if pixels and max(pixels, key=lambda p: sum(p)) == (0, 0, 0) and sum(luma(p) for p in pixels) / len(pixels) < 5:
        errs.append(f"FAIL: {path.name} appears fully black")
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
    if diff < 4.0:
        return [f"FAIL: hero and scene previews too similar (mean luma diff {diff:.1f})"]
    return []


def verify_regression_fixtures_fail() -> list[str]:
    try:
        from PIL import Image
    except ImportError:
        return ["FAIL: Pillow required for fixture regression"]
    errs: list[str] = []
    for label, path in (("620a0e37", FIXTURE_620), ("c78b9371", FIXTURE_C78)):
        if not path.is_file():
            errs.append(f"FAIL: missing regression fixture {path}")
            continue
        im = Image.open(path).convert("RGB")
        q = quality_gates(im, path.name)
        if not q:
            errs.append(f"FAIL: fixture {label} must fail quality gates")
    return errs


def main() -> int:
    all_errs: list[str] = []
    all_errs.extend(verify_regression_fixtures_fail())
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