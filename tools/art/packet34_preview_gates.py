#!/usr/bin/env python3
"""Hatch packet34 hard gates (PR #90 rework)."""
from __future__ import annotations

import json
import math
from pathlib import Path

MIN_ICON_SAT = 0.25
MIN_ICON_GLYPH_COVERAGE = 0.50
MAX_ICON_GLYPH_COVERAGE = 0.85
MIN_PANEL_COVERAGE = 0.60
MAX_PANEL_COVERAGE = 0.88
NEAR_DUP_SAT_THRESHOLD = 0.92  # mean sat correlation proxy via histogram
MID_GREY_RGB = (128, 128, 133)

ICON_IDS = (
    "icon-cash-era0",
    "icon-date-era0",
    "icon-era-bar",
    "icon-build-panel",
    "icon-hire-panel",
    "icon-train-panel",
    "icon-gpu-hire",
    "icon-week-advance",
)
PANEL_IDS = ("panel-build-bg", "panel-hire-bg", "panel-train-bg", "hud-topbar-beige")
ALL_HUD_SPRITE_IDS = ICON_IDS + PANEL_IDS


def _load_rgba(path: Path):
    from PIL import Image

    return Image.open(path).convert("RGBA")


def _pixel_sat(r: int, g: int, b: int) -> float:
    rf, gf, bf = r / 255.0, g / 255.0, b / 255.0
    mx = max(rf, gf, bf)
    mn = min(rf, gf, bf)
    if mx <= 1e-6:
        return 0.0
    return (mx - mn) / mx


def object_mask_sat_and_extent(rgb_path: Path, mask_path: Path) -> tuple[float, float, int]:
    from preview_quality_lib import ALPHA_THRESHOLD, analyze_alpha_mask  # noqa: WPS433

    im = _load_rgba(rgb_path)
    mask_im = _load_rgba(mask_path)
    if mask_im.size != im.size:
        mask_im = mask_im.resize(im.size)
    px = list(im.getdata())
    mp = list(mask_im.getdata())
    sats: list[float] = []
    for (r, g, b, _a), (_r, _g, _b, ma) in zip(px, mp):
        if ma < ALPHA_THRESHOLD:
            continue
        sats.append(_pixel_sat(r, g, b))
    alpha = analyze_alpha_mask(mask_path)
    extent = float(alpha["extent_coverage"])
    mean_sat = sum(sats) / len(sats) if sats else 0.0
    return mean_sat, extent, int(alpha.get("object_pixels", 0))


def check_icon_ortho(frame_path: Path) -> list[str]:
    if not frame_path.is_file():
        return [f"missing {frame_path.name}"]
    meta = json.loads(frame_path.read_text(encoding="utf-8"))
    if not meta.get("orthographic"):
        return ["not_orthographic"]
    if not meta.get("transparent_bg"):
        return ["bg_not_transparent"]
    return []


def check_icon_sat_coverage(asset_id: str, preview_dir: Path) -> list[str]:
    rgb = preview_dir / f"{asset_id}.png"
    mask = preview_dir / f"{asset_id}.mask.png"
    errs: list[str] = []
    if not rgb.is_file() or not mask.is_file():
        return [f"missing rgb/mask for {asset_id}"]
    sat, extent, px = object_mask_sat_and_extent(rgb, mask)
    if sat < MIN_ICON_SAT:
        errs.append(f"object_sat={sat:.3f} < {MIN_ICON_SAT} (px={px})")
    if asset_id == "hud-topbar-beige":
        lo, hi = 0.55, 0.92
    elif asset_id in PANEL_IDS:
        lo, hi = MIN_PANEL_COVERAGE, MAX_PANEL_COVERAGE
    else:
        lo, hi = MIN_ICON_GLYPH_COVERAGE, MAX_ICON_GLYPH_COVERAGE
    if extent < lo or extent > hi:
        errs.append(f"glyph_coverage={extent:.3f} need {lo}-{hi}")
    return errs


def _color_hist_signature(path: Path, bins: int = 16) -> list[float]:
    im = _load_rgba(path)
    w, h = im.size
    sig = [0.0] * (bins * bins)
    for r, g, b, a in im.getdata():
        if a < 128:
            continue
        ri, gi = min(bins - 1, r * bins // 256), min(bins - 1, g * bins // 256)
        sig[ri * bins + gi] += 1.0
    total = sum(sig) or 1.0
    return [x / total for x in sig]


def _hist_similarity(a: list[float], b: list[float]) -> float:
    dot = sum(x * y for x, y in zip(a, b))
    na = math.sqrt(sum(x * x for x in a)) or 1.0
    nb = math.sqrt(sum(x * x for x in b)) or 1.0
    return dot / (na * nb)


def check_near_duplicates(preview_dir: Path, asset_ids: tuple[str, ...], *, min_sim: float = 0.94) -> list[str]:
    sigs = {aid: _color_hist_signature(preview_dir / f"{aid}.png") for aid in asset_ids if (preview_dir / f"{aid}.png").is_file()}
    errs: list[str] = []
    ids = list(sigs)
    for i, a in enumerate(ids):
        for b in ids[i + 1 :]:
            sim = _hist_similarity(sigs[a], sigs[b])
            if sim >= min_sim:
                errs.append(f"near_duplicate {a} ~ {b} sim={sim:.3f}")
    return errs


def check_env_preview_backdrop(asset_id: str, rgb_path: Path, mask_path: Path) -> list[str]:
    """Fail vertical backdrop slabs / multi-island masks (all packet34 env previews)."""
    from preview_quality_lib import analyze_alpha_mask

    errs = check_vertical_backdrop_plane(rgb_path, mask_path, asset_id=asset_id)
    if asset_id != "garage-lot-curb":
        errs = [e for e in errs if not e.startswith("curb_too_tall")]
    if asset_id == "garage-exterior-wall":
        alpha = analyze_alpha_mask(mask_path)
        wc, hc = float(alpha["width_coverage"]), float(alpha["height_coverage"])
        if wc < hc * 0.86:
            errs.append(f"wall_vertical_slab_silhouette w={wc:.2f} h={hc:.2f}")
    if asset_id == "garage-window-frame":
        alpha = analyze_alpha_mask(mask_path)
        if float(alpha["height_coverage"]) > 0.62 and float(alpha["width_coverage"]) < 0.38:
            errs.append("window_tall_backdrop_slab")
    return errs


def check_vertical_backdrop_plane(
    rgb_path: Path, mask_path: Path, *, asset_id: str = "garage-lot-curb"
) -> list[str]:
    """Fail when preview has a second tall neutral slab in-frame (old grass wall)."""
    from PIL import Image

    mask_im = _load_rgba(mask_path)
    w, h = mask_im.size
    px = mask_im.getdata()
    grid: list[list[bool]] = [[px[y * w + x][3] >= 160 for x in range(w)] for y in range(h)]

    def flood(y: int, x: int, seen: set[tuple[int, int]]) -> tuple[int, int, int, int]:
        stack = [(y, x)]
        ys: list[int] = []
        xs: list[int] = []
        while stack:
            cy, cx = stack.pop()
            if (cy, cx) in seen or cy < 0 or cy >= h or cx < 0 or cx >= w or not grid[cy][cx]:
                continue
            seen.add((cy, cx))
            ys.append(cy)
            xs.append(cx)
            stack.extend([(cy + 1, cx), (cy - 1, cx), (cy, cx + 1), (cy, cx - 1)])
        if not ys:
            return 0, 0, 0, 0
        return min(xs), max(xs), min(ys), max(ys), len(xs)

    seen: set[tuple[int, int]] = set()
    islands: list[tuple[int, int, float]] = []
    for y in range(h):
        for x in range(w):
            if grid[y][x] and (y, x) not in seen:
                x0, x1, y0, y1, area = flood(y, x, seen)
                height_cov = (y1 - y0 + 1) / h
                islands.append((area, height_cov, height_cov))
    if not islands:
        return ["mask_empty"]
    islands.sort(reverse=True)
    hero_h = islands[0][1]
    for area, height_cov, _ in islands[1:]:
        min_area = 0.035 * w * h if asset_id == "garage-land-for-sale-sign" else 0.05 * w * h
        if height_cov > 0.32 and area > min_area:
            return [f"extra_vertical_mask_island h={height_cov:.2f}"]
    if asset_id == "garage-lot-curb" and hero_h > 0.40:
        return [f"curb_too_tall h={hero_h:.2f} need<0.40"]
    im = _load_rgba(rgb_path)
    if mask_im.size != im.size:
        mask_im = mask_im.resize(im.size)
    mask_px = list(mask_im.getdata())
    rgb_px = list(im.getdata())
    xs = [x for y in range(h) for x in range(w) if mask_px[y * w + x][3] >= 160]
    if not xs:
        return []
    x0, x1 = min(xs), max(xs)
    tall_warm_cols = 0
    for x in range(x0, x1 + 1):
        warm_run = 0
        for y in range(h):
            if mask_px[y * w + x][3] >= 160:
                warm_run = 0
                continue
            r, g, b, _a = rgb_px[y * w + x]
            if g > r + 10 and g > b + 6 and r > 95:
                warm_run += 1
                if warm_run > h * 0.38:
                    tall_warm_cols += 1
                    break
    if tall_warm_cols >= 8:
        return [f"vertical_backdrop_slab_cols={tall_warm_cols}"]

    return []


def check_backdrop_mid_grey(rgb_path: Path, *, sample_margin: float = 0.04) -> list[str]:
    """Env previews: corners neutral grey (lit), not warm beige gradient."""
    im = _load_rgba(rgb_path)
    w, h = im.size
    pts = [
        (int(w * sample_margin), int(h * sample_margin)),
        (int(w * (1 - sample_margin)), int(h * sample_margin)),
        (int(w * sample_margin), int(h * (1 - sample_margin))),
        (int(w * (1 - sample_margin)), int(h * (1 - sample_margin))),
    ]
    for x, y in pts:
        r, g, b, _a = im.getpixel((x, y))
        neutral = abs(r - g) < 24 and abs(g - b) < 24 and 120 <= r <= 215
        warm_beige = r > b + 28 and g > b + 18 and r > 185
        if warm_beige or not neutral:
            return [f"backdrop_not_mid_grey corner=({r},{g},{b})"]
    return []


def check_tile2x2_seam(preview_dir: Path, asset_id: str, *, edge_tol: float = 22.0) -> list[str]:
    """Left/right and top/bottom seams of the swatch should match when tiled."""
    from PIL import ImageChops, ImageStat

    rgb = preview_dir / f"{asset_id}.png"
    if not rgb.is_file():
        return []
    im = _load_rgba(rgb).convert("RGB")
    w, h = im.size
    left = im.crop((0, 0, 2, h))
    right = im.crop((w - 2, 0, w, h))
    top = im.crop((0, 0, w, 2))
    bottom = im.crop((0, h - 2, w, h))
    errs: list[str] = []
    for label, a, b in (("lr", left, right), ("tb", top, bottom)):
        diff = ImageStat.Stat(ImageChops.difference(a, b)).mean
        if max(diff) > edge_tol:
            errs.append(f"tile_seam_{label} mean_delta={max(diff):.1f} > {edge_tol}")
    return errs


def check_texture_swatch(preview_dir: Path, asset_id: str) -> list[str]:
    rgb = preview_dir / f"{asset_id}.png"
    tile = preview_dir / f"{asset_id}.tile2x2.png"
    errs: list[str] = []
    if not rgb.is_file():
        errs.append(f"missing swatch {asset_id}.png")
    if not tile.is_file():
        errs.append(f"missing tile2x2 proof {asset_id}.tile2x2.png")
    if rgb.is_file():
        im = _load_rgba(rgb)
        sats = [_pixel_sat(r, g, b) for r, g, b, a in im.getdata() if a > 200]
        sat = sum(sats) / len(sats) if sats else 0.0
        if sat < MIN_ICON_SAT:
            errs.append(f"swatch_sat={sat:.3f} < {MIN_ICON_SAT}")
        if im.size[0] < 480 or im.size[1] < 480:
            errs.append(f"swatch_not_full_frame size={im.size}")
    return errs


def check_icon_sheet(preview_dir: Path) -> list[str]:
    sheet = preview_dir / "icon-sheet-64.png"
    if not sheet.is_file():
        return ["missing icon-sheet-64.png"]
    im = _load_rgba(sheet)
    if im.height != 64:
        return [f"icon_sheet_height={im.height} need 64"]
    if im.width < 64 * len(ICON_IDS):
        return [f"icon_sheet_width={im.width} too narrow"]
    return []