#!/usr/bin/env python3
"""Preview gates: corners, exposure, saturation, grey bleed, prop materials."""
from __future__ import annotations

import json
import sys
from collections import Counter
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
HERO = REPO / "docs/art/previews/garage-interior-room-kit.png"
FIXTURE_620 = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-620a0e37.png"
FIXTURE_C78 = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-c78b9371.png"
FIXTURE_4E = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-4e22d8dd.png"
FIXTURE_C0 = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-c0d66236.png"
FIXTURE_4E_GATES = REPO / "docs/art/fixtures/garage-interior-room-kit-scene-gates-owner-fail-4e22d8dd.json"
CORNER_RGB = (150, 150, 150)
MIN_WALL_REGION_COVER = 0.25
MIN_FLOOR_REGION_COVER = 0.20
MIN_FLOOR_LUMA_DELTA = 0.12 * 255
BACKDROP_LUMA = 0.299 * CORNER_RGB[0] + 0.587 * CORNER_RGB[1] + 0.114 * CORNER_RGB[2]
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


def _region_non_bg_fraction(im, x0: int, x1: int, y0: int, y1: int) -> float:
    w, h = im.size
    x0, x1 = max(0, x0), min(w, x1)
    y0, y1 = max(0, y0), min(h, y1)
    if x1 <= x0 or y1 <= y0:
        return 0.0
    fg = 0
    total = 0
    for y in range(y0, y1):
        for x in range(x0, x1):
            total += 1
            if not is_studio_backdrop(im.getpixel((x, y))):
                fg += 1
    return fg / total if total else 0.0


def room_pixel_gate_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    errs: list[str] = []
    left_wall = _region_non_bg_fraction(im, 0, int(w * 0.42), int(h * 0.18), int(h * 0.78))
    back_wall = _region_non_bg_fraction(im, int(w * 0.22), int(w * 0.88), int(h * 0.12), int(h * 0.52))
    wall_cover = (left_wall + back_wall) / 2.0
    if wall_cover < MIN_WALL_REGION_COVER:
        errs.append(
            f"FAIL: {path_name} wall regions non-bg {wall_cover * 100:.1f}% "
            f"(left {left_wall * 100:.1f}%, back {back_wall * 100:.1f}%) < {MIN_WALL_REGION_COVER * 100:.0f}%"
        )
    floor_cover = _region_non_bg_fraction(im, int(w * 0.14), int(w * 0.86), int(h * 0.52), int(h * 0.92))
    if floor_cover < MIN_FLOOR_REGION_COVER:
        errs.append(
            f"FAIL: {path_name} floor region non-bg {floor_cover * 100:.1f}% < {MIN_FLOOR_REGION_COVER * 100:.0f}%"
        )
    floor_devs: list[float] = []
    for y in range(int(h * 0.58), int(h * 0.90)):
        for x in range(int(w * 0.22), int(w * 0.78)):
            p = im.getpixel((x, y))
            if is_studio_backdrop(p):
                continue
            L = luma(p)
            floor_devs.append(abs(L - BACKDROP_LUMA))
    if floor_devs:
        mean_sep = sum(floor_devs) / len(floor_devs)
        if mean_sep < MIN_FLOOR_LUMA_DELTA:
            errs.append(
                f"FAIL: {path_name} floor mean |luma−backdrop| {mean_sep:.1f} < {MIN_FLOOR_LUMA_DELTA:.0f}"
            )
    else:
        errs.append(f"FAIL: {path_name} no floor foreground pixels for luma gate")
    return errs


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
    errs.extend(room_pixel_gate_errs(im, path.name))
    pixels = list(im.getdata())
    if pixels and max(pixels, key=lambda p: sum(p)) == (0, 0, 0) and sum(luma(p) for p in pixels) / len(pixels) < 5:
        errs.append(f"FAIL: {path.name} appears fully black")
    return errs


def verify_regression_fixtures_fail() -> list[str]:
    try:
        from PIL import Image
    except ImportError:
        return ["FAIL: Pillow required for fixture regression"]
    errs: list[str] = []
    scene_gate_mod = Path(__file__).resolve().parent / "check_cc0_garage_scene.py"
    if scene_gate_mod.is_file():
        import importlib.util

        spec = importlib.util.spec_from_file_location("check_cc0_garage_scene", scene_gate_mod)
        assert spec and spec.loader
        sg = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(sg)
        placement_gate_errs = sg.placement_gate_errs
    else:
        placement_gate_errs = None
    for label, path in (("620a0e37", FIXTURE_620), ("c78b9371", FIXTURE_C78)):
        if not path.is_file():
            errs.append(f"FAIL: missing regression fixture {path}")
            continue
        im = Image.open(path).convert("RGB")
        q = quality_gates(im, path.name)
        if not q:
            errs.append(f"FAIL: fixture {label} must fail quality gates")
    if FIXTURE_C0.is_file():
        im_c0 = Image.open(FIXTURE_C0).convert("RGB")
        room_bad = room_pixel_gate_errs(im_c0, FIXTURE_C0.name)
        if not room_bad:
            errs.append("FAIL: fixture c0d66236 must fail room pixel gates")
    else:
        errs.append(f"FAIL: missing regression fixture {FIXTURE_C0}")
    if not FIXTURE_4E.is_file():
        errs.append(f"FAIL: missing regression fixture {FIXTURE_4E}")
    elif not FIXTURE_4E_GATES.is_file():
        errs.append(f"FAIL: missing placement fixture {FIXTURE_4E_GATES}")
    else:
        if placement_gate_errs is None:
            errs.append("FAIL: cannot load placement_gate_errs for fixture 4e22d8dd")
        else:
            bad = json.loads(FIXTURE_4E_GATES.read_text(encoding="utf-8"))
            if not placement_gate_errs(bad, label="4e22d8dd"):
                errs.append("FAIL: fixture 4e22d8dd must fail placement gate (a)")
    return errs


def main() -> int:
    all_errs: list[str] = []
    all_errs.extend(verify_regression_fixtures_fail())
    if not HERO.is_file():
        all_errs.append(f"FAIL: missing {HERO}")
    else:
        all_errs.extend(check_image(HERO))
    if all_errs:
        for e in all_errs:
            print(e)
        return 1
    print("OK: CC0 garage preview gates")
    return 0


if __name__ == "__main__":
    sys.exit(main())