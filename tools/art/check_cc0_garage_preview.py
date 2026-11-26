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
FIXTURE_FB = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-fbb7bf7c.png"
FIXTURE_708 = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-7086ea82.png"
FIXTURE_8B4 = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-8b4f9dba.png"
FIXTURE_3C2 = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-3c2deb99.png"
MIN_FLOOR_LUMA = 140.0
MIN_STAIN_DELTA_LUMA = 25.0
CARD_HUE_LO = 25.0
CARD_HUE_HI = 45.0
CARD_LUMA_MIN = 80.0
EDGE_MARGIN_FRAC = 0.03
MIN_STAIN_BLOB_PX = 400
MIN_PROP_BLOB_PX = 400
MIN_PROP_BLOBS_LARGE = 6
FIXTURE_4E_GATES = REPO / "docs/art/fixtures/garage-interior-room-kit-scene-gates-owner-fail-4e22d8dd.json"
CORNER_RGB = (150, 150, 150)
MIN_FLOOR_CC_FRAC = 0.20
MIN_WALL_CC_FRAC = 0.25
MIN_PROP_BLOBS = 6
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


def backdrop_nine_point_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    inset = CORNER_INSET
    margin_y = max(inset + 2, int(h * 0.045))
    pts = [
        (inset, inset),
        (w // 2, inset),
        (w - 1 - inset, inset),
        (inset, margin_y),
        (w - 1 - inset, margin_y),
        (int(w * 0.12), inset),
        (int(w * 0.88), inset),
        (inset, int(h * 0.085)),
        (w - 1 - inset, int(h * 0.085)),
    ]
    errs: list[str] = []
    for p in pts:
        if im.getpixel(p) != CORNER_RGB:
            errs.append(f"FAIL: {path_name} backdrop sample {p} RGB {im.getpixel(p)} != {CORNER_RGB}")
    return errs


def wall_base_scanline_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    max_run = 0
    for scan_y in (int(h * 0.702), int(h * 0.706), int(h * 0.710)):
        run = 0
        for x in range(int(w * 0.06), int(w * 0.92)):
            if bg[scan_y][x]:
                run += 1
                max_run = max(max_run, run)
            else:
                run = 0
    if max_run > 45:
        errs = [f"FAIL: {path_name} wall-base backdrop gap {max_run}px along scanline"]
        return errs
    return []


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
    bg = _background_mask(im)
    x0, x1 = int(w * 0.22), int(w * 0.78)
    y0, y1 = int(h * 0.22), int(h * 0.78)
    bleed = 0
    total = 0
    for y in range(y0, y1):
        for x in range(x0, x1):
            if bg[y][x]:
                continue
            total += 1
            px = im.getpixel((x, y))
            if px == CORNER_RGB:
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


def full_backdrop_mask_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    for y in range(h):
        for x in range(w):
            if bg[y][x] and im.getpixel((x, y)) != CORNER_RGB:
                return [f"FAIL: {path_name} backdrop mask pixel ({x},{y}) != {CORNER_RGB}"]
    return []


def _rgb_hue_deg(rgb: tuple[int, int, int]) -> float:
    import colorsys

    r, g, b = (c / 255.0 for c in rgb)
    h, _s, _v = colorsys.rgb_to_hsv(r, g, b)
    return h * 360.0


def edge_margin_band_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bx = max(1, int(w * EDGE_MARGIN_FRAC))
    by = max(1, int(h * EDGE_MARGIN_FRAC))
    errs: list[str] = []
    for y in range(h):
        for x in range(w):
            if bx <= x < w - bx and by <= y < h - by:
                continue
            if im.getpixel((x, y)) != CORNER_RGB:
                return [
                    f"FAIL: {path_name} outer {EDGE_MARGIN_FRAC * 100:.0f}% margin "
                    f"pixel ({x},{y}) != {CORNER_RGB}"
                ]
    return errs


def fail8_visual_gates(im, path_name: str) -> list[str]:
    """Fail #8: bright concrete floor, stain contrast, cardboard hue, no black props."""
    errs = fail7_visual_gates(im, path_name)
    w, h = im.size
    bg = _background_mask(im)
    floor_px: list[tuple[int, int, int]] = []
    stain_px: list[tuple[int, int, int]] = []
    for y in range(int(h * 0.52), h):
        for x in range(int(w * 0.08), int(w * 0.92)):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            if saturation(p) > 0.28:
                continue
            lum = luma(p)
            if lum < 70 or lum > 230:
                continue
            floor_px.append(p)
            if lum < MIN_FLOOR_LUMA - 5:
                stain_px.append(p)
    if floor_px:
        mean_l = sum(luma(p) for p in floor_px) / len(floor_px)
        if mean_l < MIN_FLOOR_LUMA:
            errs.append(f"FAIL: {path_name} floor mean luma {mean_l:.1f} < {MIN_FLOOR_LUMA}")
        if stain_px:
            stain_l = sum(luma(p) for p in stain_px) / len(stain_px)
            if mean_l - stain_l < MIN_STAIN_DELTA_LUMA:
                errs.append(
                    f"FAIL: {path_name} stain contrast {mean_l - stain_l:.1f} < {MIN_STAIN_DELTA_LUMA}"
                )
    box_px: list[tuple[int, int, int]] = []
    for y in range(int(h * 0.30), int(h * 0.88)):
        for x in range(int(w * 0.08), int(w * 0.55)):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            if saturation(p) < 0.14:
                continue
            lum = luma(p)
            if lum < 60:
                continue
            if p[0] <= p[2] + 8:
                continue
            hue = _rgb_hue_deg(p)
            if not (CARD_HUE_LO - 12 <= hue <= CARD_HUE_HI + 25):
                continue
            box_px.append(p)
    if len(box_px) >= 120:
        mean_h = sum(_rgb_hue_deg(p) for p in box_px) / len(box_px)
        mean_l = sum(luma(p) for p in box_px) / len(box_px)
        if not (CARD_HUE_LO <= mean_h <= CARD_HUE_HI):
            errs.append(f"FAIL: {path_name} cardboard hue {mean_h:.1f} outside {CARD_HUE_LO}-{CARD_HUE_HI}")
        if mean_l < CARD_LUMA_MIN:
            errs.append(f"FAIL: {path_name} cardboard mean luma {mean_l:.1f} < {CARD_LUMA_MIN}")
    return errs


def fail7_visual_gates(im, path_name: str) -> list[str]:
    """Fail #7: floor grey neutrality, oil stain blob, no black prop regions."""
    w, h = im.size
    errs: list[str] = []
    bg = _background_mask(im)
    floor_px: list[tuple[int, int, int]] = []
    stain_mask = [[False] * w for _ in range(h)]
    for y in range(int(h * 0.55), h):
        for x in range(int(w * 0.08), int(w * 0.92)):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            if saturation(p) > 0.24:
                continue
            if luma(p) < 45 or luma(p) > 200:
                continue
            floor_px.append(p)
            if p[0] + p[1] + p[2] < 200 and luma(p) < 105:
                stain_mask[y][x] = True
    if len(floor_px) < 300:
        errs.append(f"FAIL: {path_name} too few floor pixels for neutral-grey gate")
    else:
        mean_r = sum(p[0] for p in floor_px) / len(floor_px)
        mean_g = sum(p[1] for p in floor_px) / len(floor_px)
        mean_b = sum(p[2] for p in floor_px) / len(floor_px)
        if abs(mean_r - mean_b) > 12:
            errs.append(
                f"FAIL: {path_name} floor |R-B| {abs(mean_r - mean_b):.1f} > 12 "
                f"(mean RGB {mean_r:.0f},{mean_g:.0f},{mean_b:.0f})"
            )
    if floor_px:
        floor_mean_l = sum(luma(p) for p in floor_px) / len(floor_px)
        for y in range(int(h * 0.55), h):
            for x in range(int(w * 0.08), int(w * 0.92)):
                if bg[y][x]:
                    continue
                p = im.getpixel((x, y))
                if luma(p) < floor_mean_l - MIN_STAIN_DELTA_LUMA:
                    stain_mask[y][x] = True
    _, stain_sizes = _label_cc(stain_mask)
    stain_large = sum(1 for a in stain_sizes.values() if a >= MIN_STAIN_BLOB_PX)
    if stain_large < 1:
        errs.append(f"FAIL: {path_name} floor stain blob >={MIN_STAIN_BLOB_PX}px missing")

    prop_bins: dict[tuple[int, int], list[tuple[int, int, int]]] = {}
    for y in range(int(h * 0.32), int(h * 0.92)):
        for x in range(int(w * 0.10), int(w * 0.90)):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            if _is_wall_pixel(p):
                continue
            if saturation(p) < 0.14:
                continue
            key = (x // 80, y // 80)
            prop_bins.setdefault(key, []).append(p)
    for key, px in prop_bins.items():
        if len(px) < 90:
            continue
        mean_l = sum(luma(c) for c in px) / len(px)
        if mean_l < 25:
            errs.append(f"FAIL: {path_name} prop region {key} mean luma {mean_l:.1f} < 25")
            break
    return errs


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
    errs: list[str] = []
    if bbox_frac < target_lo or bbox_frac > target_hi:
        errs.append(f"FAIL: {path.name} subject bbox fill {bbox_frac * 100:.1f}% (target ~{fill * 100:.0f}%)")
    margin = EDGE_MARGIN_FRAC
    if (
        min(xs) < w * margin - 2
        or min(ys) < h * margin - 2
        or max(xs) > w * (1.0 - margin) + 2
        or max(ys) > h * (1.0 - margin) + 2
    ):
        errs.append(f"FAIL: {path.name} subject cropped: need >={margin * 100:.0f}% margin on all sides")
    return errs


def _label_cc(mask: list[list[bool]]) -> tuple[list[list[int]], dict[int, int]]:
    from collections import deque

    h, w = len(mask), len(mask[0])
    labels = [[0] * w for _ in range(h)]
    sizes: dict[int, int] = {}
    lbl = 0
    for y0 in range(h):
        for x0 in range(w):
            if not mask[y0][x0] or labels[y0][x0]:
                continue
            lbl += 1
            q: deque[tuple[int, int]] = deque([(x0, y0)])
            labels[y0][x0] = lbl
            n = 0
            while q:
                x, y = q.popleft()
                n += 1
                for nx, ny in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
                    if 0 <= nx < w and 0 <= ny < h and mask[ny][nx] and not labels[ny][nx]:
                        labels[ny][nx] = lbl
                        q.append((nx, ny))
            sizes[lbl] = n
    return labels, sizes


def _convex_hull(points: list[tuple[int, int]]) -> list[tuple[int, int]]:
    pts = sorted(set(points))
    if len(pts) <= 2:
        return pts

    def cross(o, a, b):
        return (a[0] - o[0]) * (b[1] - o[1]) - (a[1] - o[1]) * (b[0] - o[0])

    lower: list[tuple[int, int]] = []
    for p in pts:
        while len(lower) >= 2 and cross(lower[-2], lower[-1], p) <= 0:
            lower.pop()
        lower.append(p)
    upper: list[tuple[int, int]] = []
    for p in reversed(pts):
        while len(upper) >= 2 and cross(upper[-2], upper[-1], p) <= 0:
            upper.pop()
        upper.append(p)
    return lower[:-1] + upper[:-1]


def _point_in_poly(px: int, py: int, poly: list[tuple[int, int]]) -> bool:
    inside = False
    n = len(poly)
    if n < 3:
        return False
    j = n - 1
    for i in range(n):
        xi, yi = poly[i]
        xj, yj = poly[j]
        if ((yi > py) != (yj > py)) and (
            px < (xj - xi) * (py - yi) / (yj - yi + 1e-9) + xi
        ):
            inside = not inside
        j = i
    return inside


def _is_wall_pixel(p: tuple[int, int, int]) -> bool:
    if is_studio_backdrop(p):
        return False
    return saturation(p) >= 0.22 and p[0] >= p[2] + 6 and luma(p) >= 88


def room_pixel_gate_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    frame = w * h
    errs: list[str] = []
    bg = _background_mask(im)

    floor_mask = [[False] * w for _ in range(h)]
    for y in range(int(h * 0.42), h):
        for x in range(w):
            if not bg[y][x]:
                floor_mask[y][x] = True

    _, floor_sizes = _label_cc(floor_mask)
    if not floor_sizes:
        errs.append(f"FAIL: {path_name} no floor pixels for CC gate")
        return errs
    floor_lbl = max(floor_sizes, key=lambda k: floor_sizes[k])
    floor_frac = floor_sizes[floor_lbl] / frame
    if floor_frac < MIN_FLOOR_CC_FRAC:
        errs.append(
            f"FAIL: {path_name} floor connected component {floor_frac * 100:.1f}% < {MIN_FLOOR_CC_FRAC * 100:.0f}%"
        )

    labels, _ = _label_cc(floor_mask)
    floor_pts = [
        (x, y)
        for y in range(int(h * 0.62), h)
        for x in range(w)
        if labels[y][x] == floor_lbl
    ]
    if len(floor_pts) < 200:
        floor_pts = [(x, y) for y in range(h) for x in range(w) if labels[y][x] == floor_lbl]
    step = max(1, len(floor_pts) // 5000)
    hull = _convex_hull(floor_pts[::step])
    if len(hull) >= 3:
        inside_backdrop = 0
        inside_floor = 0
        for y in range(h):
            for x in range(w):
                if not _point_in_poly(x, y, hull):
                    continue
                inside_floor += 1
                if bg[y][x]:
                    inside_backdrop += 1
        if inside_floor and inside_backdrop / inside_floor > 0.04:
            errs.append(
                f"FAIL: {path_name} backdrop inside floor hull {inside_backdrop / inside_floor * 100:.1f}% > 3%"
            )

    wall_fg = 0
    for y in range(h):
        for x in range(w):
            if bg[y][x]:
                continue
            in_left = x < int(w * 0.48)
            in_back = y < int(h * 0.54)
            if not (in_left or in_back):
                continue
            wall_fg += 1
    wall_frac = wall_fg / frame
    if wall_frac < MIN_WALL_CC_FRAC:
        errs.append(f"FAIL: {path_name} wall foreground {wall_frac * 100:.1f}% < {MIN_WALL_CC_FRAC * 100:.0f}%")

    prop_mask = [[False] * w for _ in range(h)]
    for y in range(int(h * 0.34), int(h * 0.92)):
        for x in range(int(w * 0.10), int(w * 0.90)):
            p = im.getpixel((x, y))
            if bg[y][x]:
                continue
            if _is_wall_pixel(p):
                continue
            if p[2] > p[0] + 8 and saturation(p) >= 0.14:
                continue
            if y > int(h * 0.44) and saturation(p) < 0.14:
                continue
            if saturation(p) < 0.12:
                continue
            prop_mask[y][x] = True
    _, prop_sizes = _label_cc(prop_mask)
    large_cc = sum(1 for area in prop_sizes.values() if area >= MIN_PROP_BLOB_PX)
    mat_bins: dict[tuple[int, int, int], int] = {}
    for y in range(int(h * 0.34), int(h * 0.92)):
        for x in range(int(w * 0.10), int(w * 0.90)):
            if not prop_mask[y][x]:
                continue
            p = im.getpixel((x, y))
            key = (p[0] // 40, p[1] // 40, p[2] // 40)
            mat_bins[key] = mat_bins.get(key, 0) + 1
    large_mat = sum(1 for area in mat_bins.values() if area >= MIN_PROP_BLOB_PX)
    large = max(large_cc, large_mat)
    if large < MIN_PROP_BLOBS_LARGE:
        errs.append(
            f"FAIL: {path_name} prop blobs >={MIN_PROP_BLOB_PX}px: {large} < {MIN_PROP_BLOBS_LARGE}"
        )
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
    errs.extend(backdrop_nine_point_errs(im, path.name))
    errs.extend(wall_base_scanline_errs(im, path.name))
    errs.extend(quality_gates(im, path.name))
    errs.extend(fill_margin_errs(path))
    errs.extend(edge_margin_band_errs(im, path.name))
    errs.extend(full_backdrop_mask_errs(im, path.name))
    errs.extend(fail8_visual_gates(im, path.name))
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
    def fixture_must_fail(label: str, fix: Path) -> None:
        if not fix.is_file():
            errs.append(f"FAIL: missing regression fixture {fix}")
            return
        im_f = Image.open(fix).convert("RGB")
        bad = (
            room_pixel_gate_errs(im_f, fix.name)
            + backdrop_nine_point_errs(im_f, fix.name)
            + wall_base_scanline_errs(im_f, fix.name)
            + full_backdrop_mask_errs(im_f, fix.name)
            + fail8_visual_gates(im_f, fix.name)
            + edge_margin_band_errs(im_f, fix.name)
        )
        if not bad:
            errs.append(f"FAIL: fixture {label} must fail tightened room gates")

    def fixture_must_fail_fail8(label: str, fix: Path) -> None:
        if not fix.is_file():
            errs.append(f"FAIL: missing regression fixture {fix}")
            return
        im_f = Image.open(fix).convert("RGB")
        bad = (
            fail8_visual_gates(im_f, fix.name)
            + full_backdrop_mask_errs(im_f, fix.name)
            + edge_margin_band_errs(im_f, fix.name)
            + fill_margin_errs(fix)
        )
        if not bad:
            errs.append(f"FAIL: fixture {label} must fail fail-8 tightened gates")

    for label, fix in (
        ("7086ea82", FIXTURE_708),
        ("fbb7bf7c", FIXTURE_FB),
        ("c0d66236", FIXTURE_C0),
        ("8b4f9dba", FIXTURE_8B4),
    ):
        fixture_must_fail(label, fix)
    fixture_must_fail_fail8("3c2deb99", FIXTURE_3C2)
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