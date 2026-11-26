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