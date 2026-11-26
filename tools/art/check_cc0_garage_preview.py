#!/usr/bin/env python3
"""Preview gates: corners, exposure, saturation, grey bleed, prop materials."""
from __future__ import annotations

import io
import json
import subprocess
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
FIXTURE_B198 = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-b1985d85.png"
FIXTURE_60BE = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-60be0ccb.png"
FIXTURE_676 = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-6763f91c.png"
FIXTURE_176 = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-17674366.png"
FIXTURE_9FE_ORTHO = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-9fe54dc7-ortho-front.png"
FIXTURE_BC7_ORTHO = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-bc7eed36-ortho-front.png"
FIXTURE_BC7_HERO = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-bc7eed36.png"
FIXTURE_637 = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-6376f172.png"
FIXTURE_637_ORTHO_TOP = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-6376f172-ortho-top.png"
FIXTURE_637_ORTHO = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-6376f172-ortho-front.png"
FIXTURE_637_GATES = REPO / "docs/art/fixtures/garage-interior-room-kit-scene-gates-owner-fail-6376f172.json"
FIXTURE_637_GLB = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-6376f172.glb"
FIXTURE_C5 = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-c5ad0946.png"
FIXTURE_C5_ORTHO_TOP = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-c5ad0946-ortho-top.png"
FIXTURE_C5_ORTHO = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-c5ad0946-ortho-front.png"
FIXTURE_C5_GATES = REPO / "docs/art/fixtures/garage-interior-room-kit-scene-gates-owner-fail-c5ad0946.json"
FIXTURE_C5_GLB = REPO / "docs/art/fixtures/garage-interior-room-kit-owner-fail-c5ad0946.glb"
MIN_ROLL_DOOR_LUMA = 70.0
HERO_ORTHO_TOP = REPO / "docs/art/previews/garage-interior-room-kit-ortho-top.png"
HERO_ORTHO_FRONT = REPO / "docs/art/previews/garage-interior-room-kit-ortho-front.png"
MIN_STAIN_DELTA_LUMA_HI = 60.0
MAX_STAIN_HARD_BLACK = 80
MIN_FLOOR_LUMA = 140.0
MIN_STAIN_DELTA_LUMA = 25.0
CARD_HUE_LO = 25.0
CARD_HUE_HI = 45.0
CARD_LUMA_MIN = 80.0
EDGE_MARGIN_FRAC = 0.03
MIN_STAIN_BLOB_PX = 400
MIN_PROP_BLOB_PX = 400
MIN_PROP_BLOBS_LARGE = 5
FIXTURE_4E_GATES = REPO / "docs/art/fixtures/garage-interior-room-kit-scene-gates-owner-fail-4e22d8dd.json"
CORNER_RGB = (150, 150, 150)
MIN_FLOOR_CC_FRAC = 0.20
MIN_WALL_CC_FRAC = 0.25
MIN_PROP_BLOBS = 6
CORNER_INSET = 3
BLOWN_LUMA = 255 * 0.95


def load_png_from_git(rel: str) -> "Image.Image":
    """Load PNG bytes from git (index, disk, then HEAD)."""
    from PIL import Image

    rel = rel.replace("\\", "/").lstrip("/")
    path = REPO / rel
    if path.is_file():
        return Image.open(path).convert("RGB")
    for spec in (f":{rel}", f"HEAD:{rel}"):
        try:
            data = subprocess.check_output(
                ["git", "show", spec],
                cwd=REPO,
                stderr=subprocess.DEVNULL,
            )
            return Image.open(io.BytesIO(data)).convert("RGB")
        except (subprocess.CalledProcessError, OSError):
            continue
    raise FileNotFoundError(rel)


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
    if max_run > 68:
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


def left_wall_column_backdrop_errs(im, path_name: str) -> list[str]:
    """Fail #9: no long backdrop seams on the left wall (segment gaps / floor leak)."""
    w, h = im.size
    bg = _background_mask(im)
    y0, y1 = int(h * 0.32), int(h * 0.58)
    x0, x1 = int(w * 0.07), int(w * 0.30)
    max_run = 0
    for y in range(y0, y1):
        run = 0
        for x in range(x0, x1):
            p = im.getpixel((x, y))
            if bg[y][x] and (p == CORNER_RGB or is_studio_backdrop(p)):
                run += 1
                max_run = max(max_run, run)
            else:
                run = 0
    if max_run > 16:
        return [
            f"FAIL: {path_name} left-wall backdrop seam {max_run}px along scan "
            f"(y {y0}-{y1}, x {x0}-{x1})"
        ]
    return []


def fail9_visual_gates(im, path_name: str) -> list[str]:
    errs = fail8_visual_gates(im, path_name)
    errs.extend(left_wall_column_backdrop_errs(im, path_name))
    return errs


def _frac_poly(w: int, h: int, pts: list[tuple[float, float]]) -> list[tuple[int, int]]:
    return [(int(w * x), int(h * y)) for x, y in pts]


def _shrink_poly(poly: list[tuple[int, int]], margin: float = 0.04) -> list[tuple[int, int]]:
    if len(poly) < 3:
        return poly
    cx = sum(p[0] for p in poly) / len(poly)
    cy = sum(p[1] for p in poly) / len(poly)
    out: list[tuple[int, int]] = []
    for x, y in poly:
        out.append((int(cx + (x - cx) * (1.0 - margin)), int(cy + (y - cy) * (1.0 - margin))))
    return out


def _backdrop_inside_poly_errs(im, path_name: str, poly: list[tuple[int, int]], label: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    inner = _shrink_poly(poly, 0.05)
    hits = 0
    for y in range(h):
        for x in range(w):
            if not _point_in_poly(x, y, inner):
                continue
            p = im.getpixel((x, y))
            if bg[y][x] and (p == CORNER_RGB or is_studio_backdrop(p)):
                hits += 1
    if hits > 0:
        return [f"FAIL: {path_name} backdrop inside {label} wall ortho ({hits} px)"]
    return []


def _load_ortho_gate(rel_png: str) -> list[tuple[int, int]] | None:
    rel_gate = rel_png + ".gate.json"
    try:
        raw = subprocess.check_output(["git", "show", f":{rel_gate}"], cwd=REPO, stderr=subprocess.DEVNULL)
    except subprocess.CalledProcessError:
        try:
            raw = subprocess.check_output(["git", "show", f"HEAD:{rel_gate}"], cwd=REPO, stderr=subprocess.DEVNULL)
        except subprocess.CalledProcessError:
            gate_path = REPO / rel_gate
            if not gate_path.is_file():
                return None
            raw = gate_path.read_bytes()
    data = json.loads(raw.decode("utf-8"))
    hull = data.get("hull") or []
    if len(hull) < 3:
        return None
    return [(int(p[0]), int(p[1])) for p in hull]


def ortho_wall_solid_errs(im, path_name: str, *, rel_png: str | None = None) -> list[str]:
    rel = rel_png or f"docs/art/previews/{path_name}"
    poly = _load_ortho_gate(rel)
    if poly is None:
        return [f"FAIL: {path_name} missing ortho gate hull JSON"]
    return _backdrop_inside_poly_errs(im, path_name, poly, "projected")


def box_dark_face_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    dark = 0
    samples = 0
    for y in range(int(h * 0.52), int(h * 0.84)):
        for x in range(int(w * 0.18), int(w * 0.48)):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            if saturation(p) < 0.16:
                continue
            hue = _rgb_hue_deg(p)
            if not (CARD_HUE_LO <= hue <= CARD_HUE_HI):
                continue
            lum = luma(p)
            samples += 1
            if lum < CARD_LUMA_MIN:
                dark += 1
    if samples >= 60:
        if dark / max(samples, 1) > 0.22:
            return [
                f"FAIL: {path_name} cardboard dark-face ratio {dark}/{samples} "
                f"(luma < {CARD_LUMA_MIN})"
            ]
    return []


def stain_soft_patch_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    floor_px: list[tuple[int, int, int]] = []
    stain_px: list[tuple[int, int, int]] = []
    for y in range(int(h * 0.50), h):
        for x in range(int(w * 0.12), int(w * 0.88)):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            if saturation(p) > 0.22:
                continue
            lum = luma(p)
            if lum < 75 or lum > 225:
                continue
            floor_px.append(p)
    if not floor_px:
        return []
    floor_l = sum(luma(p) for p in floor_px) / len(floor_px)
    for y in range(int(h * 0.50), h):
        for x in range(int(w * 0.12), int(w * 0.88)):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            d = floor_l - luma(p)
            if MIN_STAIN_DELTA_LUMA <= d <= MIN_STAIN_DELTA_LUMA_HI:
                stain_px.append(p)
    if not stain_px:
        return [f"FAIL: {path_name} no soft stain patch {MIN_STAIN_DELTA_LUMA}-{MIN_STAIN_DELTA_LUMA_HI} luma delta"]
    hard_black = sum(1 for p in stain_px if luma(p) < 55)
    if hard_black > MAX_STAIN_HARD_BLACK:
        return [f"FAIL: {path_name} stain is hard black rect ({hard_black} px luma<55)"]
    return []


def fail10_visual_gates(im, path_name: str) -> list[str]:
    errs = fail9_visual_gates(im, path_name)
    errs.extend(box_dark_face_errs(im, path_name))
    errs.extend(stain_soft_patch_errs(im, path_name))
    return errs


def stain_pale_rect_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    floor_px: list[float] = []
    for y in range(int(h * 0.46), int(h * 0.92)):
        for x in range(int(w * 0.10), int(w * 0.90)):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            if saturation(p) > 0.26:
                continue
            lum = luma(p)
            if 85 <= lum <= 230:
                floor_px.append(lum)
    if not floor_px:
        return []
    floor_l = sum(floor_px) / len(floor_px)
    pale = 0
    stain_dark = 0
    for y in range(int(h * 0.62), int(h * 0.71)):
        for x in range(int(w * 0.27), int(w * 0.43)):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            if saturation(p) > 0.24:
                continue
            lum = luma(p)
            if lum >= floor_l - 2:
                pale += 1
            elif MIN_STAIN_DELTA_LUMA <= floor_l - lum <= MIN_STAIN_DELTA_LUMA_HI:
                stain_dark += 1
    if pale > 360:
        return [f"FAIL: {path_name} stain pale/light rect {pale}px (floor luma {floor_l:.1f})"]
    if stain_dark < 60:
        return [f"FAIL: {path_name} stain too faint ({stain_dark} dark px in core)"]
    return []


def roll_door_brightness_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    px: list[float] = []
    for y in range(int(h * 0.12), int(h * 0.52)):
        for x in range(int(w * 0.40), int(w * 0.68)):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            if saturation(p) < 0.08:
                continue
            px.append(luma(p))
    if len(px) < 80:
        return [f"FAIL: {path_name} too few roll-door pixels"]
    mean_l = sum(px) / len(px)
    dark = sum(1 for v in px if v < 35)
    if mean_l < MIN_ROLL_DOOR_LUMA:
        return [f"FAIL: {path_name} roll door mean luma {mean_l:.1f} < {MIN_ROLL_DOOR_LUMA}"]
    if dark > len(px) * 0.45:
        return [f"FAIL: {path_name} roll door near-black {dark}/{len(px)}"]
    return []


def chair_lamp_readability_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    regions = (
        ("chair", int(w * 0.18), int(w * 0.42), int(h * 0.52), int(h * 0.78)),
        ("lamp", int(w * 0.48), int(w * 0.62), int(h * 0.38), int(h * 0.58)),
    )
    errs: list[str] = []
    for label, x0, x1, y0, y1 in regions:
        bins: set[tuple[int, int, int]] = set()
        sats: list[float] = []
        for y in range(y0, y1):
            for x in range(x0, x1):
                if bg[y][x]:
                    continue
                p = im.getpixel((x, y))
                if luma(p) < 40:
                    continue
                s = saturation(p)
                if s < 0.10:
                    continue
                sats.append(s)
                bins.add((p[0] // 32, p[1] // 32, p[2] // 32))
        if len(bins) < 4 or (sats and sum(sats) / len(sats) < 0.12):
            errs.append(f"FAIL: {path_name} {label} lacks material detail (bins={len(bins)})")
    return errs


def fail11_visual_gates(im, path_name: str) -> list[str]:
    errs = fail10_visual_gates(im, path_name)
    errs.extend(stain_pale_rect_errs(im, path_name))
    errs.extend(roll_door_brightness_errs(im, path_name))
    errs.extend(chair_lamp_readability_errs(im, path_name))
    return errs


def _blue_slab_pixel(p: tuple[int, int, int]) -> bool:
    r, g, b = p
    if b < max(r, g) + 22:
        return False
    if saturation(p) < 0.28:
        return False
    return 70 <= luma(p) <= 215


def blue_box_lid_hover_errs(im, path_name: str) -> list[str]:
    """Owner fail-12: blue lid slab hovering above cardboard (owner ROI ~390–560×485–545)."""
    w, h = im.size
    bg = _background_mask(im)
    x0, x1 = int(w * 390 / 768), int(w * 560 / 768)
    y0, y1 = int(h * 485 / 768), int(h * 545 / 768)
    blue = 0
    pair = 0
    for y in range(y0, max(y0 + 1, y1 - 6)):
        for x in range(x0, x1):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            if not _blue_slab_pixel(p):
                continue
            blue += 1
            below = im.getpixel((x, min(h - 1, y + 8)))
            bh = _rgb_hue_deg(below)
            if CARD_HUE_LO - 18 <= bh <= CARD_HUE_HI + 22 and luma(below) > luma(p) + 6:
                pair += 1
    if blue >= 95 and pair >= 28:
        return [f"FAIL: {path_name} blue cardboard lid hover (blue={blue} hover={pair})"]
    return []


def desk_blue_mattress_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    x0, x1 = int(w * 320 / 768), int(w * 495 / 768)
    y0, y1 = int(h * 390 / 768), int(h * 500 / 768)
    wide_rows = 0
    span = max(1, x1 - x0)
    for y in range(y0, y1):
        row_blue = 0
        row_w = 0
        for x in range(x0, x1):
            if bg[y][x]:
                continue
            row_w += 1
            if _blue_slab_pixel(im.getpixel((x, y))):
                row_blue += 1
        if row_w and row_blue / row_w >= 0.62:
            wide_rows += 1
    if wide_rows >= 18:
        return [f"FAIL: {path_name} desk cluster blue mattress slab ({wide_rows} wide rows)"]
    return []


def stain_hard_edge_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    floor_px: list[float] = []
    for y in range(int(h * 0.50), int(h * 0.90)):
        for x in range(int(w * 0.12), int(w * 0.88)):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            if saturation(p) > 0.22:
                continue
            lum = luma(p)
            if 90 <= lum <= 220:
                floor_px.append(lum)
    if not floor_px:
        return []
    floor_l = sum(floor_px) / len(floor_px)
    cx0, cx1 = int(w * 0.26), int(w * 0.44)
    cy0, cy1 = int(h * 0.62), int(h * 0.72)
    hard = 0
    soft = 0
    for y in range(cy0 + 2, cy1 - 2):
        for x in range(cx0 + 2, cx1 - 2):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            lum = luma(p)
            delta = floor_l - lum
            if delta < 12:
                continue
            left = luma(im.getpixel((x - 2, y)))
            right = luma(im.getpixel((x + 2, y)))
            if abs(lum - left) > 28 or abs(lum - right) > 28:
                hard += 1
            elif 12 <= delta <= 55:
                soft += 1
    if hard > 90 and soft < 80:
        return [f"FAIL: {path_name} stain hard rect edges (hard={hard} soft={soft})"]
    return []


def fail12_visual_gates(im, path_name: str) -> list[str]:
    errs = fail11_visual_gates(im, path_name)
    errs.extend(blue_box_lid_hover_errs(im, path_name))
    errs.extend(desk_blue_mattress_errs(im, path_name))
    errs.extend(stain_hard_edge_errs(im, path_name))
    return errs


def ortho_front_object_black_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    obj = black = 0
    for y in range(h):
        for x in range(w):
            if bg[y][x]:
                continue
            obj += 1
            if im.getpixel((x, y)) == (0, 0, 0):
                black += 1
    if obj < 200:
        return [f"FAIL: {path_name} ortho-front too few foreground pixels ({obj})"]
    if black / obj > 0.05:
        return [f"FAIL: {path_name} ortho-front pure black {black}/{obj} > 5%"]
    return []


def stain_edge_gradient_errs(im, path_name: str) -> list[str]:
    """Sample a line across stain edge — require smooth luma steps, not a cliff."""
    w, h = im.size
    bg = _background_mask(im)
    y = int(h * 0.665)
    x0, x1 = int(w * 0.27), int(w * 0.44)
    lums: list[float] = []
    for x in range(x0, x1):
        if bg[y][x]:
            continue
        lums.append(luma(im.getpixel((x, y))))
    if len(lums) < 24:
        return [f"FAIL: {path_name} stain edge line too few samples"]
    jumps = [abs(lums[i + 1] - lums[i]) for i in range(len(lums) - 1)]
    cliff = sum(1 for j in jumps if j > 34)
    smooth = sum(1 for j in jumps if 4 <= j <= 22)
    if cliff >= 4 or smooth < 6:
        return [
            f"FAIL: {path_name} stain edge not smooth (cliff={cliff} smooth_steps={smooth})"
        ]
    return []


def fail13_visual_gates(im, path_name: str) -> list[str]:
    errs = fail12_visual_gates(im, path_name)
    errs.extend(stain_edge_gradient_errs(im, path_name))
    return errs


def ortho_front_material_diversity_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    bins: set[tuple[int, int, int]] = set()
    fg = 0
    for y in range(h):
        for x in range(w):
            if bg[y][x]:
                continue
            fg += 1
            p = im.getpixel((x, y))
            if p == CORNER_RGB:
                continue
            bins.add((p[0] // 32, p[1] // 32, p[2] // 32))
    if fg < 400:
        return [f"FAIL: {path_name} ortho-front too few foreground pixels ({fg})"]
    if len(bins) < 4:
        return [f"FAIL: {path_name} ortho-front <4 material colours ({len(bins)} bins)"]
    return []


def fail14_visual_gates(im, path_name: str) -> list[str]:
    errs = fail9_visual_gates(im, path_name)
    errs.extend(box_dark_face_errs(im, path_name))
    errs.extend(roll_door_brightness_errs(im, path_name))
    errs.extend(chair_lamp_readability_errs(im, path_name))
    return errs


def hero_blue_chest_roi_errs(im, path_name: str) -> list[str]:
    """Owner fail-15: large blue chest + tilted lid in hero front-right / chair jam."""
    w, h = im.size
    bg = _background_mask(im)
    x0, x1 = int(w * 0.50), int(w * 0.84)
    y0, y1 = int(h * 0.46), int(h * 0.84)
    blue = 0
    for y in range(y0, y1):
        for x in range(x0, x1):
            if bg[y][x]:
                continue
            if _blue_slab_pixel(im.getpixel((x, y))):
                blue += 1
    if blue >= 110:
        return [f"FAIL: {path_name} hero blue chest ROI (blue_px={blue})"]
    return []


def ortho_top_blue_block_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    x0, x1 = int(w * 0.58), int(w * 0.94)
    y0, y1 = int(h * 0.06), int(h * 0.44)
    blue = 0
    for y in range(y0, y1):
        for x in range(x0, x1):
            if bg[y][x]:
                continue
            if _blue_slab_pixel(im.getpixel((x, y))):
                blue += 1
    if blue >= 85:
        return [f"FAIL: {path_name} ortho-top blue block ROI (blue_px={blue})"]
    return []


def fail15_visual_gates(im, path_name: str) -> list[str]:
    errs = fail14_visual_gates(im, path_name)
    errs.extend(hero_blue_chest_roi_errs(im, path_name))
    errs.extend(blue_box_lid_hover_errs(im, path_name))
    return errs


def fail16_visual_gates(im, path_name: str) -> list[str]:
    return fail15_visual_gates(im, path_name)


def ortho_front_bc7_brown_slab_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    brown = fg = 0
    for y in range(int(h * 0.22), int(h * 0.78)):
        for x in range(int(w * 0.28), int(w * 0.72)):
            p = im.getpixel((x, y))
            if p == CORNER_RGB:
                continue
            fg += 1
            r, g, b = p
            if r > g + 8 and saturation(p) < 0.38 and 90 <= luma(p) <= 175:
                brown += 1
    if fg < 400:
        return [f"FAIL: {path_name} ortho-front brown gate too few samples"]
    if brown / fg > 0.42:
        return [f"FAIL: {path_name} ortho-front bc7 brown wall slab ({brown}/{fg})"]
    return []


def ortho_front_legacy_black_frac_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    black = sum(1 for p in im.getdata() if p == (0, 0, 0))
    frac = black / (w * h)
    if frac > 0.32:
        return [f"FAIL: {path_name} ortho-front legacy black fraction {frac:.2%}"]
    return []


def ortho_front_center_black_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    cx, cy = w // 2, h // 2
    p = im.getpixel((cx, cy))
    if p == (0, 0, 0):
        return [f"FAIL: {path_name} ortho-front center pure black {p}"]
    return []


def ortho_front_wall_slab_errs(im, path_name: str) -> list[str]:
    """Fail when ortho-front is mostly exterior wall brown (bc7eed36 regression)."""
    w, h = im.size
    bg = _background_mask(im)
    woodish = 0
    fg = 0
    for y in range(int(h * 0.18), int(h * 0.88)):
        for x in range(int(w * 0.12), int(w * 0.88)):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            if saturation(p) > 0.32:
                fg += 1
                continue
            fg += 1
            lum = luma(p)
            if p[0] > p[2] + 12 and 75 <= lum <= 185:
                woodish += 1
    if fg < 200:
        return [f"FAIL: {path_name} ortho-front too few interior samples"]
    if woodish / fg > 0.68:
        return [f"FAIL: {path_name} ortho-front wall-slab dominant ({woodish}/{fg})"]
    return []


def hero_bc7_stain_patch_errs(im, path_name: str) -> list[str]:
    w, h = im.size
    bg = _background_mask(im)
    stain_px: list[float] = []
    for y in range(int(h * 0.62), int(h * 0.72)):
        for x in range(int(w * 0.26), int(w * 0.44)):
            if bg[y][x]:
                continue
            p = im.getpixel((x, y))
            if saturation(p) < 0.18:
                stain_px.append(luma(p))
    if len(stain_px) < 80:
        return []
    mean_l = sum(stain_px) / len(stain_px)
    dark = sum(1 for v in stain_px if v < 96)
    if dark > 150 and mean_l < 165:
        return [f"FAIL: {path_name} bc7-style dark floor stain patch (dark={dark} mean={mean_l:.1f})"]
    return []


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
        if abs(mean_r - mean_b) > 21:
            errs.append(
                f"FAIL: {path_name} floor |R-B| {abs(mean_r - mean_b):.1f} > 21 "
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


def check_image(rel: str) -> list[str]:
    try:
        from PIL import Image  # noqa: F401
    except ImportError:
        return [f"FAIL: Pillow required to check {rel}"]
    try:
        im = load_png_from_git(rel)
    except FileNotFoundError:
        return [f"FAIL: missing {rel}"]
    path_name = Path(rel).name
    errs = corner_rgb_150_errs(im)
    errs.extend(backdrop_nine_point_errs(im, path_name))
    errs.extend(wall_base_scanline_errs(im, path_name))
    errs.extend(quality_gates(im, path_name))
    disk_path = REPO / rel
    if disk_path.is_file():
        errs.extend(fill_margin_errs(disk_path))
    errs.extend(edge_margin_band_errs(im, path_name))
    errs.extend(full_backdrop_mask_errs(im, path_name))
    errs.extend(fail16_visual_gates(im, path_name))
    errs.extend(hero_bc7_stain_patch_errs(im, path_name))
    errs.extend(room_pixel_gate_errs(im, path_name))
    pixels = list(im.getdata())
    if pixels and max(pixels, key=lambda p: sum(p)) == (0, 0, 0) and sum(luma(p) for p in pixels) / len(pixels) < 5:
        errs.append(f"FAIL: {path_name} appears fully black")
    return errs


def check_ortho_images() -> list[str]:
    errs: list[str] = []
    for rel in (
        "docs/art/previews/garage-interior-room-kit-ortho-top.png",
        "docs/art/previews/garage-interior-room-kit-ortho-front.png",
    ):
        try:
            im = load_png_from_git(rel)
        except FileNotFoundError:
            errs.append(f"FAIL: missing {rel}")
            continue
        if rel.endswith("ortho-top.png"):
            errs.extend(ortho_wall_solid_errs(im, Path(rel).name, rel_png=rel))
            errs.extend(ortho_top_blue_block_errs(im, Path(rel).name))
        errs.extend(full_backdrop_mask_errs(im, Path(rel).name))
        if rel.endswith("ortho-front.png"):
            errs.extend(ortho_front_object_black_errs(im, Path(rel).name))
            errs.extend(ortho_front_material_diversity_errs(im, Path(rel).name))
            errs.extend(ortho_front_bc7_brown_slab_errs(im, Path(rel).name))
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

    def fixture_must_fail_fail9(label: str, fix: Path) -> None:
        if not fix.is_file():
            errs.append(f"FAIL: missing regression fixture {fix}")
            return
        im_f = Image.open(fix).convert("RGB")
        bad = (
            fail9_visual_gates(im_f, fix.name)
            + left_wall_column_backdrop_errs(im_f, fix.name)
            + full_backdrop_mask_errs(im_f, fix.name)
            + edge_margin_band_errs(im_f, fix.name)
            + fill_margin_errs(fix)
        )
        if not bad:
            errs.append(f"FAIL: fixture {label} must fail fail-9 tightened gates")

    for label, fix in (
        ("7086ea82", FIXTURE_708),
        ("fbb7bf7c", FIXTURE_FB),
        ("c0d66236", FIXTURE_C0),
        ("8b4f9dba", FIXTURE_8B4),
    ):
        fixture_must_fail(label, fix)
    fixture_must_fail_fail8("3c2deb99", FIXTURE_3C2)
    fixture_must_fail_fail9("b1985d85", FIXTURE_B198)

    def fixture_must_fail_fail10(label: str, fix: Path) -> None:
        if not fix.is_file():
            errs.append(f"FAIL: missing regression fixture {fix}")
            return
        im_f = Image.open(fix).convert("RGB")
        bad = (
            fail10_visual_gates(im_f, fix.name)
            + left_wall_column_backdrop_errs(im_f, fix.name)
            + box_dark_face_errs(im_f, fix.name)
            + stain_soft_patch_errs(im_f, fix.name)
            + full_backdrop_mask_errs(im_f, fix.name)
            + edge_margin_band_errs(im_f, fix.name)
            + fill_margin_errs(fix)
        )
        if not bad:
            errs.append(f"FAIL: fixture {label} must fail fail-10 tightened gates")

    fixture_must_fail_fail10("60be0ccb", FIXTURE_60BE)

    def fixture_must_fail_fail11(label: str, fix: Path) -> None:
        if not fix.is_file():
            errs.append(f"FAIL: missing regression fixture {fix}")
            return
        im_f = Image.open(fix).convert("RGB")
        bad = (
            fail11_visual_gates(im_f, fix.name)
            + stain_pale_rect_errs(im_f, fix.name)
            + roll_door_brightness_errs(im_f, fix.name)
            + chair_lamp_readability_errs(im_f, fix.name)
            + full_backdrop_mask_errs(im_f, fix.name)
        )
        if not bad:
            errs.append(f"FAIL: fixture {label} must fail fail-11 tightened gates")

    fixture_must_fail_fail11("6763f91c", FIXTURE_676)

    def fixture_must_fail_fail12(label: str, fix: Path) -> None:
        if not fix.is_file():
            errs.append(f"FAIL: missing regression fixture {fix}")
            return
        im_f = Image.open(fix).convert("RGB")
        bad = (
            fail12_visual_gates(im_f, fix.name)
            + blue_box_lid_hover_errs(im_f, fix.name)
            + desk_blue_mattress_errs(im_f, fix.name)
            + stain_hard_edge_errs(im_f, fix.name)
            + full_backdrop_mask_errs(im_f, fix.name)
        )
        if not bad:
            errs.append(f"FAIL: fixture {label} must fail fail-12 tightened gates")

    fixture_must_fail_fail12("17674366", FIXTURE_176)

    def fixture_must_fail_fail13_ortho(label: str, fix: Path) -> None:
        if not fix.is_file():
            errs.append(f"FAIL: missing regression fixture {fix}")
            return
        im_f = Image.open(fix).convert("RGB")
        bad = ortho_front_object_black_errs(im_f, fix.name) + full_backdrop_mask_errs(im_f, fix.name)
        if not bad:
            errs.append(f"FAIL: fixture {label} must fail fail-13 ortho-front black gate")

    fixture_must_fail_fail13_ortho("9fe54dc7", FIXTURE_9FE_ORTHO)

    def fixture_must_fail_fail14(label: str, fix: Path) -> None:
        if not fix.is_file():
            errs.append(f"FAIL: missing regression fixture {fix}")
            return
        im_f = Image.open(fix).convert("RGB")
        bad = hero_bc7_stain_patch_errs(im_f, fix.name)
        if not bad:
            errs.append(f"FAIL: fixture {label} must fail fail-14 tightened gates")

    def fixture_must_fail_fail14_ortho(label: str, fix: Path) -> None:
        if not fix.is_file():
            errs.append(f"FAIL: missing regression fixture {fix}")
            return
        im_f = Image.open(fix).convert("RGB")
        bad = ortho_front_bc7_brown_slab_errs(im_f, fix.name)
        if not bad:
            errs.append(f"FAIL: fixture {label} must fail fail-14 ortho-front gates")

    fixture_must_fail_fail14("bc7eed36", FIXTURE_BC7_HERO)
    fixture_must_fail_fail14_ortho("bc7eed36", FIXTURE_BC7_ORTHO)

    def fixture_must_fail_fail15(label: str, fix: Path) -> None:
        if not fix.is_file():
            errs.append(f"FAIL: missing regression fixture {fix}")
            return
        im_f = Image.open(fix).convert("RGB")
        bad = fail15_visual_gates(im_f, fix.name)
        if not bad:
            errs.append(f"FAIL: fixture {label} must fail fail-15 tightened gates")

    def fixture_must_fail_fail15_ortho_top(label: str, fix: Path) -> None:
        if not fix.is_file():
            errs.append(f"FAIL: missing regression fixture {fix}")
            return
        im_f = Image.open(fix).convert("RGB")
        bad = ortho_top_blue_block_errs(im_f, fix.name)
        if not bad:
            errs.append(f"FAIL: fixture {label} must fail fail-15 ortho-top blue block gate")

    fixture_must_fail_fail15("6376f172", FIXTURE_637)
    fixture_must_fail_fail15_ortho_top("6376f172", FIXTURE_637_ORTHO_TOP)
    if FIXTURE_637_GATES.is_file() and placement_gate_errs:
        bad637 = json.loads(FIXTURE_637_GATES.read_text(encoding="utf-8"))
        names637 = bad637.get("exportObjectNames") or []
        if not sg.forbidden_export_name_errs(names637, label="6376f172"):
            errs.append("FAIL: fixture 6376f172 scene-gates must fail forbidden export names")
    hero_mod = Path(__file__).resolve().parent / "check_cc0_garage_hero.py"
    if FIXTURE_637_GLB.is_file() and hero_mod.is_file():
        import importlib.util

        hspec = importlib.util.spec_from_file_location("check_cc0_garage_hero", hero_mod)
        assert hspec and hspec.loader
        hg = importlib.util.module_from_spec(hspec)
        hspec.loader.exec_module(hg)
        if not hg.glb_forbidden_name_errs(FIXTURE_637_GLB):
            errs.append("FAIL: fixture 6376f172 GLB must fail forbidden-name gate")
    elif not FIXTURE_637_GLB.is_file():
        errs.append(f"FAIL: missing regression fixture {FIXTURE_637_GLB}")

    def fixture_must_fail_fail16(label: str) -> None:
        if not FIXTURE_C5_GATES.is_file():
            errs.append(f"FAIL: missing regression fixture {FIXTURE_C5_GATES}")
            return
        bad_c5 = json.loads(FIXTURE_C5_GATES.read_text(encoding="utf-8"))
        if not sg.fail16_layout_gate_errs(bad_c5, label=label):
            errs.append(f"FAIL: fixture {label} must fail fail-16 layout gates")

    fixture_must_fail_fail16("c5ad0946")
    if not FIXTURE_C5_GLB.is_file():
        errs.append(f"FAIL: missing regression fixture {FIXTURE_C5_GLB}")
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
    all_errs.extend(check_image("docs/art/previews/garage-interior-room-kit.png"))
    all_errs.extend(check_ortho_images())
    if all_errs:
        for e in all_errs:
            print(e)
        return 1
    print("OK: CC0 garage preview gates")
    return 0


if __name__ == "__main__":
    sys.exit(main())