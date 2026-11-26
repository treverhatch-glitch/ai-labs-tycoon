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
    if asset_id == "garage-lot-curb" and hero_h > 0.72:
        return [f"curb_too_tall h={hero_h:.2f} need<0.72"]
    if asset_id == "garage-lot-curb":
        for area, height_cov, _ in islands[1:]:
            if height_cov > 0.22 and area > 0.02 * w * h:
                return [f"curb_extra_tall_island h={height_cov:.2f}"]
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
    if asset_id == "garage-lot-curb" and tall_warm_cols >= 3:
        return [f"curb_vertical_backdrop_cols={tall_warm_cols}"]
    if tall_warm_cols >= 8:
        return [f"vertical_backdrop_slab_cols={tall_warm_cols}"]

    return []


def check_sign_for_sale_contrast(rgb_path: Path, mask_path: Path) -> list[str]:
    """Board region needs dark glyphs on bright field (FOR SALE readable)."""
    rgb = _load_rgba(rgb_path)
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())
    xs, ys = [], []
    for y in range(h):
        for x in range(w):
            if mpx[y * w + x][3] >= 160:
                xs.append(x)
                ys.append(y)
    if not xs:
        return ["sign_mask_empty"]
    x0, x1, y0, y1 = min(xs), max(xs), min(ys), max(ys)
    cx0 = x0 + (x1 - x0) // 4
    cx1 = x1 - (x1 - x0) // 4
    cy0 = y0 + (y1 - y0) // 5
    cy1 = y1 - (y1 - y0) // 4
    light_lum: list[float] = []
    dark_lum: list[float] = []
    red_glyphs = 0
    for y in range(cy0, cy1 + 1):
        for x in range(cx0, cx1 + 1):
            if mpx[y * w + x][3] < 160:
                continue
            r, g, b, _a = rpx[y * w + x]
            lum = 0.2126 * r + 0.7152 * g + 0.0722 * b
            if lum > 185:
                light_lum.append(lum)
            elif lum < 95:
                dark_lum.append(lum)
            if r > 150 and r > g + 35 and r > b + 35:
                red_glyphs += 1
    if len(light_lum) < 48:
        return [f"sign_board_not_bright px={len(light_lum)}"]
    if len(dark_lum) < 24 and red_glyphs < 80:
        return [f"sign_no_dark_glyphs px={len(dark_lum)} red={red_glyphs}"]
    if red_glyphs >= 80:
        contrast = (sum(light_lum) / len(light_lum)) - 82.0
    else:
        contrast = (sum(light_lum) / len(light_lum)) - (sum(dark_lum) / len(dark_lum))
    if contrast < 70:
        return [f"sign_text_contrast={contrast:.1f}"]
    return []


def check_curb_preview_concrete(rgb_path: Path, mask_path: Path) -> list[str]:
    """Fail white/translucent slab; require grey concrete variation on hero mask."""
    rgb = _load_rgba(rgb_path)
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())
    samples: list[tuple[int, int, int]] = []
    for i, (r, g, b, a) in enumerate(rpx):
        if mpx[i][3] >= 160:
            samples.append((r, g, b))
    if len(samples) < 32:
        return ["curb_mask_empty"]
    mean_r = sum(s[0] for s in samples) / len(samples)
    mean_g = sum(s[1] for s in samples) / len(samples)
    mean_b = sum(s[2] for s in samples) / len(samples)
    if mean_r > 200 and mean_g > 198 and mean_b > 196:
        return [f"curb_preview_white mean=({mean_r:.0f},{mean_g:.0f},{mean_b:.0f})"]
    if mean_r < 45 and mean_g < 45 and mean_b < 45:
        return [f"curb_preview_gloss_black mean=({mean_r:.0f},{mean_g:.0f},{mean_b:.0f})"]
    std = (sum((s[0] - mean_r) ** 2 + (s[1] - mean_g) ** 2 + (s[2] - mean_b) ** 2 for s in samples) / len(samples)) ** 0.5
    if std < 8.0:
        return [f"curb_preview_flat std={std:.1f}"]
    return []


def check_sign_albedo_file(tex_dir: Path) -> list[str]:
    path = tex_dir / "garage-land-for-sale-sign_albedo.png"
    if not path.is_file():
        return ["sign_albedo_missing"]
    try:
        from PIL import Image
    except ImportError:
        return []
    if path.stat().st_size < 12_000:
        return [f"sign_albedo_too_small bytes={path.stat().st_size}"]
    im = Image.open(path).convert("RGB")
    uniq = len({px for px in im.getdata()})
    if uniq < 120:
        return [f"sign_albedo_flat unique={uniq}"]
    return []


def check_wall_cinderblock_visible(rgb_path: Path, mask_path: Path, repo_textures: Path) -> list[str]:
    """Fail when wall preview centre reads as blank white, not cinderblock."""
    import math

    from PIL import Image, ImageStat

    ref_path = repo_textures / "tex-garage-wall-beige.png"
    preview_ref = repo_textures.parents[2] / "docs/art/previews/tex-garage-wall-beige.png"
    if ref_path.is_file():
        if ImageStat.Stat(Image.open(ref_path).convert("RGB")).stddev[0] < 6.0 and preview_ref.is_file():
            ref_path = preview_ref
    elif preview_ref.is_file():
        ref_path = preview_ref
    else:
        return []
    rgb = _load_rgba(rgb_path)
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    xs, ys = [], []
    for y in range(h):
        for x in range(w):
            if mpx[y * w + x][3] >= 160:
                xs.append(x)
                ys.append(y)
    if not xs:
        return ["wall_mask_empty"]
    x0, x1 = min(xs), max(xs)
    y0, y1 = min(ys), max(ys)
    cx0 = x0 + (x1 - x0) // 5
    cx1 = x1 - (x1 - x0) // 5
    cy0 = y0 + (y1 - y0) // 5
    cy1 = y1 - (y1 - y0) // 5
    samples: list[tuple[int, int, int]] = []
    rpx = list(rgb.getdata())
    for y in range(cy0, cy1 + 1):
        for x in range(cx0, cx1 + 1):
            if mpx[y * w + x][3] >= 160:
                r, g, b, _a = rpx[y * w + x]
                samples.append((r, g, b))
    if len(samples) < 64:
        return ["wall_center_too_small"]
    mean_r = sum(s[0] for s in samples) / len(samples)
    mean_g = sum(s[1] for s in samples) / len(samples)
    mean_b = sum(s[2] for s in samples) / len(samples)
    if mean_r > 228 and mean_g > 225 and mean_b > 220:
        return [f"wall_center_white ({mean_r:.0f},{mean_g:.0f},{mean_b:.0f})"]
    sats = [_pixel_sat(s[0], s[1], s[2]) for s in samples]
    mean_sat = sum(sats) / len(sats)
    if mean_sat < 0.07:
        return [f"wall_center_desat={mean_sat:.3f}"]
    ref = Image.open(ref_path).convert("RGB")
    ref_px = list(ref.getdata())
    ref_sat = sum(_pixel_sat(r, g, b) for r, g, b in ref_px[::17]) / max(1, len(ref_px[::17]))
    if mean_sat < ref_sat * 0.22:
        return [f"wall_center_not_cinderblock sat={mean_sat:.3f} ref={ref_sat:.3f}"]
    ref_mean = (
        sum(p[0] for p in ref_px) / len(ref_px),
        sum(p[1] for p in ref_px) / len(ref_px),
        sum(p[2] for p in ref_px) / len(ref_px),
    )
    dist = math.sqrt((mean_r - ref_mean[0]) ** 2 + (mean_g - ref_mean[1]) ** 2 + (mean_b - ref_mean[2]) ** 2)
    if dist > 128:
        return [f"wall_center_color_far_from_swatch dist={dist:.1f}"]
    return []


def check_wall_uv_blocks_per_metre(frame_path: Path) -> list[str]:
    """Wall preview must map ~10 blocks / 4 m → 2–3 blocks/m (swatch has 4 blocks/tile)."""
    if not frame_path.is_file():
        return ["wall_frame_missing"]
    meta = json.loads(frame_path.read_text(encoding="utf-8"))
    bpm = meta.get("wall_blocks_per_m_h")
    if bpm is None:
        sx = float(meta.get("wall_uv_mapping_scale_x", 10.0))
        tiles = float(meta.get("wall_swatch_blocks_per_tile_x", 4))
        wall_w = float(meta.get("wall_world_w_m", 4.0))
        bpm = sx * tiles / wall_w
    if bpm < 2.0 or bpm > 3.0:
        return [f"wall_blocks_per_m_h={bpm:.2f} need 2.0-3.0"]
    return []


def check_sign_albedo_layout(tex_dir: Path) -> list[str]:
    """No inner grey board frame; red FOR SALE fully inside 5% cream margin."""
    path = tex_dir / "garage-land-for-sale-sign_albedo.png"
    if not path.is_file():
        return ["sign_albedo_missing"]
    im = _load_rgba(path).convert("RGB")
    w, h = im.size
    margin = int(min(w, h) * 0.05)
    px = list(im.getdata())

    def _is_red(r: int, g: int, b: int) -> bool:
        return r > 150 and r > g + 35 and r > b + 35

    outer_cream = sum(1 for r, g, b in px if (r, g, b) == (245, 238, 220))
    inner_panel = sum(1 for r, g, b in px if (r, g, b) == (252, 245, 228))
    if outer_cream > 0.12 * w * h and inner_panel > 0.22 * w * h:
        return [f"sign_inner_board_layers outer={outer_cream} inner={inner_panel}"]
    inner_cream = 0
    for y in range(margin, h - margin):
        for x in range(margin, w - margin):
            r, g, b = px[y * w + x]
            if r > 235 and g > 228 and b > 210 and not _is_red(r, g, b):
                inner_cream += 1
    red_x, red_y = [], []
    for y in range(h):
        for x in range(w):
            r, g, b = px[y * w + x]
            if _is_red(r, g, b):
                red_x.append(x)
                red_y.append(y)
    if len(red_x) < 400:
        return [f"sign_red_glyphs_too_few px={len(red_x)}"]
    rx0, rx1, ry0, ry1 = min(red_x), max(red_x), min(red_y), max(red_y)
    if rx0 < margin or ry0 < margin or rx1 > w - margin or ry1 > h - margin:
        return [
            f"sign_text_outside_board red_bbox=({rx0},{ry0})-({rx1},{ry1}) margin={margin}"
        ]
    if inner_cream < 800:
        return [f"sign_board_field_small cream_px={inner_cream}"]
    return []


def check_sign_front_red_inside_board(front_rgb: Path, front_mask: Path) -> list[str]:
    """Front-on hero: red glyph bbox inside cream board bbox from mask."""
    if not front_rgb.is_file() or not front_mask.is_file():
        return ["sign_front_preview_missing"]
    rgb = _load_rgba(front_rgb)
    mask = _load_rgba(front_mask)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())

    def _is_red(r: int, g: int, b: int) -> bool:
        return r > 140 and r > g + 30 and r > b + 30

    hero_x, hero_y = [], []
    red_x, red_y = [], []
    for y in range(h):
        for x in range(w):
            r, g, b, _a = rpx[y * w + x]
            if mpx[y * w + x][3] >= 160:
                hero_x.append(x)
                hero_y.append(y)
            if mpx[y * w + x][3] >= 160 and _is_red(r, g, b):
                red_x.append(x)
                red_y.append(y)
    if not hero_x or not red_x:
        return [f"sign_front_board_or_red_missing hero={len(hero_x)} red={len(red_x)}"]
    hx0, hx1, hy0, hy1 = min(hero_x), max(hero_x), min(hero_y), max(hero_y)
    face_h = hy1 - hy0
    by0 = hy0 + int(face_h * 0.22)
    by1 = hy1
    bx0, bx1 = hx0, hx1
    board_x, board_y = hero_x, [y for y in hero_y if y >= by0]
    if not board_y:
        return ["sign_front_board_face_empty"]
    by0, by1 = min(board_y), max(board_y)
    rx0, rx1, ry0, ry1 = min(red_x), max(red_x), min(red_y), max(red_y)
    pad_x = max(2, int((bx1 - bx0) * 0.02))
    pad_y = max(2, int((by1 - by0) * 0.03))
    if rx1 > bx1 or ry1 > by1 or rx0 < bx0 or ry0 < by0:
        return [
            f"sign_front_text_outside_board red=({rx0},{ry0})-({rx1},{ry1}) "
            f"board=({bx0},{by0})-({bx1},{by1})"
        ]
    if rx0 < bx0 + pad_x or ry0 < by0 + pad_y:
        return [
            f"sign_front_text_margin red=({rx0},{ry0})-({rx1},{ry1}) "
            f"board=({bx0},{by0})-({bx1},{by1}) pad=({pad_x},{pad_y})"
        ]
    return []


def check_wall_mortar_horizontal(rgb_path: Path, mask_path: Path) -> list[str]:
    """Centre region must show stronger horizontal than vertical mortar edge energy."""
    try:
        import numpy as np
    except ImportError:
        return []
    rgb = _load_rgba(rgb_path).convert("L")
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    xs, ys = [], []
    for y in range(h):
        for x in range(w):
            if mpx[y * w + x][3] >= 160:
                xs.append(x)
                ys.append(y)
    if not xs:
        return ["wall_mask_empty"]
    x0, x1, y0, y1 = min(xs), max(xs), min(ys), max(ys)
    cx0 = x0 + (x1 - x0) // 5
    cx1 = x1 - (x1 - x0) // 5
    cy0 = y0 + (y1 - y0) // 5
    cy1 = y1 - (y1 - y0) // 5
    arr = np.array(rgb, dtype=np.float32)
    m = np.zeros((h, w), dtype=bool)
    for y in range(h):
        for x in range(w):
            m[y, x] = mpx[y * w + x][3] >= 160
    crop = arr[cy0 : cy1 + 1, cx0 : cx1 + 1]
    mc = m[cy0 : cy1 + 1, cx0 : cx1 + 1]
    if crop.shape[0] < 8 or crop.shape[1] < 8:
        return ["wall_center_too_small"]
    def _band_energy(sig: np.ndarray) -> float:
        s = sig.astype(np.float64)
        s = s - s.mean()
        if len(s) < 16:
            return 0.0
        spec = np.abs(np.fft.rfft(s)[1 : min(24, len(s) // 2)])
        return float(spec.max()) if spec.size else 0.0

    row_prof = np.array([crop[y, :].mean() for y in range(crop.shape[0])])
    col_prof = np.array([crop[:, x].mean() for x in range(crop.shape[1])])
    h_energy = _band_energy(row_prof)
    v_energy = _band_energy(col_prof)
    if h_energy + 1e-6 < v_energy:
        return [f"wall_mortar_vertical_dominant h={h_energy:.2f} v={v_energy:.2f}"]
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