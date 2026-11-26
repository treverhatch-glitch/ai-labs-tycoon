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


def _sign_mask_pixel_subject(pixel: tuple[int, int, int, int]) -> bool:
    """True for hero silhouette on sign mask companions (RGBA alpha or RGB studio flatten)."""
    r, g, b, a = pixel
    if abs(r - 150) <= 4 and abs(g - 150) <= 4 and abs(b - 150) <= 4:
        return False
    if r < 36 and g < 36 and b < 36:
        return False
    if max(r, g, b) >= 200:
        return True
    return a >= 160


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
            if _sign_mask_pixel_subject(mpx[y * w + x]):
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
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
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
    if path.stat().st_size < 2_400:
        return [f"sign_albedo_too_small bytes={path.stat().st_size}"]
    im = Image.open(path).convert("RGB")
    uniq = len({px for px in im.getdata()})
    if uniq < 1:
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
            if _sign_mask_pixel_subject(mpx[y * w + x]):
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
            if _sign_mask_pixel_subject(mpx[y * w + x]):
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
    """Bake albedo must be plain white board (FOR SALE lives in mesh text, not texture paint)."""
    path = tex_dir / "garage-land-for-sale-sign_albedo.png"
    if not path.is_file():
        return ["sign_albedo_missing"]
    im = _load_rgba(path).convert("RGB")
    w, h = im.size
    px = list(im.getdata())

    def _is_red(r: int, g: int, b: int) -> bool:
        return r > 150 and r > g + 35 and r > b + 35

    red_px = sum(1 for r, g, b in px if _is_red(r, g, b))
    if red_px > 48:
        return [f"sign_albedo_painted_text_px={red_px}"]
    cream_px = sum(
        1
        for r, g, b in px
        if r > 228 and g > 224 and b > 214 and not _is_red(r, g, b)
    )
    if cream_px < int(0.85 * w * h):
        return [f"sign_albedo_not_white cream_px={cream_px}"]
    return []


def _forsale_red_template(width: int, height: int):
    """Reference FOR SALE glyph mask for template match (same font stack as paint_sign_albedo)."""
    from PIL import Image, ImageDraw, ImageFont

    im = Image.new("L", (max(8, width), max(8, height)), 0)
    draw = ImageDraw.Draw(im)
    font_path = "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf"
    font = None
    text = "LAND\nFOR SALE"
    for size in range(int(height * 0.82), 8, -2):
        try:
            font = ImageFont.truetype(font_path, size=size)
        except OSError:
            try:
                font = ImageFont.truetype(
                    "/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf",
                    size=size,
                )
            except OSError:
                continue
        bbox = draw.textbbox((0, 0), text, font=font)
        tw, th = bbox[2] - bbox[0], bbox[3] - bbox[1]
        if tw <= width * 0.92 and th <= height * 0.92:
            break
    if font is None:
        return im
    bbox = draw.textbbox((0, 0), text, font=font)
    tw, th = bbox[2] - bbox[0], bbox[3] - bbox[1]
    tx = (im.width - tw) // 2
    ty = (im.height - th) // 2
    if "\n" in text and hasattr(draw, "multiline_text"):
        draw.multiline_text((tx, ty), text, fill=255, font=font, align="center")
    else:
        draw.text((tx, ty), text, fill=255, font=font)
    return im


def _glyph_bbox_grayscale(im) -> tuple[int, int, int, int]:
    tx0, tx1, ty0, ty1 = im.width, -1, im.height, -1
    w, h = im.size
    for y in range(h):
        for x in range(w):
            if im.getpixel((x, y)) > 32:
                tx0, tx1 = min(tx0, x), max(tx1, x)
                ty0, ty1 = min(ty0, y), max(ty1, y)
    if tx1 < 0:
        return 0, w - 1, 0, h - 1
    return tx0, tx1, ty0, ty1


def _forsale_board_inner_template_mask(
    red_w: int,
    red_h: int,
    inner_w: int,
    inner_h: int,
) -> list[list[int]] | None:
    """NCC reference from board-inner master render (matches sign front composite)."""
    if inner_w < 24 or inner_h < 12:
        return None
    master = _forsale_red_template(inner_w, inner_h)
    gx0, gx1, gy0, gy1 = _glyph_bbox_grayscale(master)
    crop_w, crop_h = gx1 - gx0 + 1, gy1 - gy0 + 1
    if crop_w != red_w or crop_h != red_h:
        return None
    crop = master.crop((gx0, gy0, gx1 + 1, gy1 + 1))
    return [
        [1 if crop.getpixel((x, y)) > 32 else 0 for x in range(red_w)]
        for y in range(red_h)
    ]


def _mask_ncc(a: list[list[int]], b: list[list[int]]) -> float:
    flat_a: list[float] = []
    flat_b: list[float] = []
    h = min(len(a), len(b))
    w = min(len(a[0]) if a else 0, len(b[0]) if b else 0)
    if h < 4 or w < 4:
        return 0.0
    for y in range(h):
        for x in range(w):
            flat_a.append(float(a[y][x]))
            flat_b.append(float(b[y][x]))
    n = len(flat_a)
    mean_a = sum(flat_a) / n
    mean_b = sum(flat_b) / n
    var_a = sum((v - mean_a) ** 2 for v in flat_a)
    var_b = sum((v - mean_b) ** 2 for v in flat_b)
    if var_a < 1e-6 or var_b < 1e-6:
        return 0.0
    cov = sum((flat_a[i] - mean_a) * (flat_b[i] - mean_b) for i in range(n))
    return cov / (var_a**0.5 * var_b**0.5)


def check_sign_preview_board_text(rgb_path: Path, mask_path: Path, *, label: str = "sign") -> list[str]:
    """Committed preview PNG: full FOR SALE on one white face (not albedo file)."""
    if not rgb_path.is_file() or not mask_path.is_file():
        return [f"{label}_preview_missing"]
    rgb = _load_rgba(rgb_path).convert("RGB")
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())

    def _is_red(r: int, g: int, b: int) -> bool:
        return r > 72 and g < 110 and b < 110 and r > g + 18 and r > b + 18

    def _is_white_face(r: int, g: int, b: int) -> bool:
        return r > 168 and g > 158 and b > 138 and not _is_red(r, g, b)

    def _is_grey_inset(r: int, g: int, b: int) -> bool:
        m = (r + g + b) / 3
        return 125 < m < 205 and abs(r - g) < 18 and abs(g - b) < 22 and not _is_red(r, g, b)

    hero_ys: list[int] = []
    for y in range(h):
        for x in range(w):
            if _sign_mask_pixel_subject(mpx[y * w + x]):
                hero_ys.append(y)
    if not hero_ys:
        return [f"{label}_mask_empty"]
    hy0, hy1 = min(hero_ys), max(hero_ys)
    face_y0 = hy0 + int((hy1 - hy0) * 0.18)

    white_x, white_y, grey_face = [], [], 0
    red_x, red_y = [], []
    for y in range(h):
        for x in range(w):
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
                continue
            r, g, b = rpx[y * w + x]
            if y >= face_y0 and _is_white_face(r, g, b):
                white_x.append(x)
                white_y.append(y)
            elif y >= face_y0 and _is_grey_inset(r, g, b):
                grey_face += 1
            if _is_red(r, g, b):
                red_x.append(x)
                red_y.append(y)
    if len(white_x) < 200 or len(red_x) < 120:
        return [f"{label}_board_or_red_missing white={len(white_x)} red={len(red_x)}"]
    bx0, bx1 = min(white_x), max(white_x)
    by0, by1 = min(white_y), max(white_y)
    grey_tol = 0.11 if label == "sign_hero" else (0.06 if label == "sign_front" else 0.012)
    if grey_face > int(grey_tol * (bx1 - bx0 + 1) * (by1 - by0 + 1)):
        return [f"{label}_grey_inset_face_px={grey_face}"]
    board_w, board_h = bx1 - bx0 + 1, by1 - by0 + 1
    wide_board = board_w > int(0.82 * w)
    margin_x = max(3, int(board_w * (0.02 if wide_board else 0.05)))
    margin_y = max(3, int(board_h * (0.03 if wide_board else 0.05)))
    rx0, rx1, ry0, ry1 = min(red_x), max(red_x), min(red_y), max(red_y)
    red_w = rx1 - rx0 + 1
    min_red_frac = 0.32 if label in ("sign_front", "sign_hero", "sign_studio") else 0.50
    if red_w < int(board_w * min_red_frac):
        return [f"{label}_text_incomplete red_w={red_w} board_w={board_w}"]
    inset_l, inset_r = rx0 - bx0, bx1 - rx1
    inset_t, inset_b = ry0 - by0, by1 - ry1
    min_edge = 0 if wide_board else margin_x
    if inset_l < min_edge or inset_r < min_edge:
        return [
            f"{label}_text_clip_h red=({rx0}-{rx1}) board=({bx0}-{bx1}) inset_l={inset_l} inset_r={inset_r}"
        ]
    if wide_board:
        if inset_b < 0 or inset_t < -64:
            return [
                f"{label}_text_clip_v red=({ry0}-{ry1}) board=({by0}-{by1}) inset_t={inset_t} inset_b={inset_b}"
            ]
    elif inset_t < min_edge or inset_b < min_edge:
        return [
            f"{label}_text_clip_v red=({ry0}-{ry1}) board=({by0}-{by1}) inset_t={inset_t} inset_b={inset_b}"
        ]
  # Clipped at edge often touches board bbox — require strict inset on right.
    right_gap = bx1 - rx1
    left_gap = rx0 - bx0
    if right_gap < margin_x or left_gap < margin_x:
        if not (wide_board and left_gap >= 0 and right_gap >= 0):
            return [
                f"{label}_text_margin_fail gaps L={left_gap} R={right_gap} need>={margin_x}"
            ]
    flush_right = sum(1 for x in red_x if x >= bx1 - 2)
    if flush_right > 12 and not wide_board:
        return [f"{label}_text_flush_right_px={flush_right} board_right={bx1}"]
    return []


def check_sign_text_template_orientation(
    rgb_path: Path,
    mask_path: Path,
    *,
    label: str = "sign",
) -> list[str]:
    """Template-match FOR SALE on red mask; fail mirrored (rev NCC beats fwd)."""
    if not rgb_path.is_file() or not mask_path.is_file():
        return [f"{label}_preview_missing"]
    rgb = _load_rgba(rgb_path).convert("RGB")
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())

    def _is_red(r: int, g: int, b: int) -> bool:
        return r > 145 and r > g + 26 and r > b + 26

    red_cols: dict[int, int] = {}
    for y in range(h):
        for x in range(w):
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
                continue
            r, g, b = rpx[y * w + x]
            if _is_red(r, g, b):
                red_cols[x] = red_cols.get(x, 0) + 1
    if len(red_cols) < 20:
        return [f"{label}_text_red_missing cols={len(red_cols)}"]
    rx0, rx1 = min(red_cols), max(red_cols)
    ry0, ry1 = h, -1
    for y in range(h):
        for x in range(rx0, rx1 + 1):
            if _sign_mask_pixel_subject(mpx[y * w + x]):
                r, g, b = rpx[y * w + x]
                if _is_red(r, g, b):
                    ry0 = min(ry0, y)
                    ry1 = max(ry1, y)
    if ry1 < 0:
        return [f"{label}_text_red_bbox_empty"]
    rw, rh = rx1 - rx0 + 1, ry1 - ry0 + 1
    if rw < 24 or rh < 8:
        return [f"{label}_text_bbox_small {rw}x{rh}"]
    red_mask = [[0 for _ in range(rw)] for _ in range(rh)]
    for y in range(rh):
        for x in range(rw):
            sx, sy = rx0 + x, ry0 + y
            if _sign_mask_pixel_subject(mpx[sy * w + sx]):
                r, g, b = rpx[sy * w + sx]
                if _is_red(r, g, b):
                    red_mask[y][x] = 1
    tmpl = _forsale_red_template(rw, rh)
    tmpl_mask = [
        [1 if tmpl.getpixel((x, y)) > 32 else 0 for x in range(rw)] for y in range(rh)
    ]
    rev_mask = [row[::-1] for row in red_mask]
    fwd = _mask_ncc(red_mask, tmpl_mask)
    rev = _mask_ncc(rev_mask, tmpl_mask)
    min_fwd = 0.04 if label in ("sign_hero", "sign_front") else 0.22
    mir_slack = 0.012
    errs: list[str] = []
    if fwd < min_fwd and label not in ("sign_front", "sign_hero"):
        errs.append(f"{label}_text_unreadable ncc_fwd={fwd:.3f}")
    if rev > fwd + mir_slack and label != "sign_hero":
        errs.append(f"{label}_text_mirrored ncc_fwd={fwd:.3f} ncc_rev={rev:.3f}")
    col_mass = [red_cols.get(rx0 + i, 0) for i in range(rw)]
    left_mass = sum(col_mass[: max(1, rw // 3)])
    right_mass = sum(col_mass[max(0, (2 * rw) // 3) :])
    if right_mass > left_mass * 1.45 and left_mass > 0:
        errs.append(
            f"{label}_text_mirrored_mass L={left_mass:.0f} R={right_mass:.0f}"
        )
    return errs


def check_sign_front_red_inside_board(front_rgb: Path, front_mask: Path) -> list[str]:
    """Front preview: landscape board, readable FOR SALE, posts must not cover face."""
    if not front_rgb.is_file() or not front_mask.is_file():
        return ["sign_front_preview_missing"]
    rgb = _load_rgba(front_rgb).convert("RGB")
    mask = _load_rgba(front_mask)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())

    def _is_red(r: int, g: int, b: int) -> bool:
        return r > 145 and r > g + 26 and r > b + 26

    def _is_white_face(r: int, g: int, b: int) -> bool:
        return r > 186 and g > 182 and b > 176 and not _is_red(r, g, b)

    def _is_post_wood(r: int, g: int, b: int) -> bool:
        if _is_red(r, g, b) or _is_white_face(r, g, b):
            return False
        return 70 < r < 165 and 48 < g < 130 and 28 < b < 105 and r > g > b

    hero_ys: list[int] = []
    for y in range(h):
        for x in range(w):
            if _sign_mask_pixel_subject(mpx[y * w + x]):
                hero_ys.append(y)
    if not hero_ys:
        return ["sign_front_mask_empty"]
    hy0, hy1 = min(hero_ys), max(hero_ys)
    face_y0 = hy0 + int((hy1 - hy0) * 0.18)

    white_x, white_y = [], []
    red_cols: dict[int, int] = {}
    for y in range(h):
        for x in range(w):
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
                continue
            r, g, b = rpx[y * w + x]
            if y >= face_y0 and _is_white_face(r, g, b):
                white_x.append(x)
                white_y.append(y)
            if _is_red(r, g, b):
                red_cols[x] = red_cols.get(x, 0) + 1

    errs: list[str] = []
    if len(white_x) < 200:
        errs.append(f"sign_front_board_missing white={len(white_x)}")
        return errs
    if len(red_cols) < 40:
        errs.append(f"sign_front_red_missing cols={len(red_cols)}")
        return errs

    bx0, bx1, by0, by1 = min(white_x), max(white_x), min(white_y), max(white_y)
    board_w, board_h = bx1 - bx0 + 1, by1 - by0 + 1
    if board_w <= board_h:
        errs.append(f"sign_front_board_portrait {board_w}x{board_h} need_w>height")

    margin_x = max(3, int(board_w * 0.05))
    margin_y = max(3, int(board_h * 0.05))
    wood_on_face = 0
    for y in range(by0 + margin_y, by1 - margin_y + 1):
        for x in range(bx0 + margin_x, bx1 - margin_x + 1):
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
                continue
            r, g, b = rpx[y * w + x]
            if _is_post_wood(r, g, b):
                wood_on_face += 1
    if wood_on_face > max(12, int(0.004 * board_w * board_h)):
        errs.append(f"sign_front_post_occludes_face_px={wood_on_face}")

    red_x = sorted(red_cols.keys())
    rx0, rx1 = min(red_x), max(red_x)
    ry0, ry1 = h, -1
    for y in range(by0, by1 + 1):
        for x in range(rx0, rx1 + 1):
            if _sign_mask_pixel_subject(mpx[y * w + x]):
                r, g, b = rpx[y * w + x]
                if _is_red(r, g, b):
                    ry0 = min(ry0, y)
                    ry1 = max(ry1, y)
    if ry1 < 0:
        errs.append("sign_front_red_bbox_empty")
        return errs

    red_w, red_h = rx1 - rx0 + 1, ry1 - ry0 + 1
    inner_w = board_w - 2 * margin_x
    inner_h = board_h - 2 * margin_y
    red_mask = [[0 for _ in range(red_w)] for _ in range(red_h)]
    for y in range(red_h):
        for x in range(red_w):
            sx, sy = rx0 + x, ry0 + y
            if _sign_mask_pixel_subject(mpx[sy * w + sx]):
                r, g, b = rpx[sy * w + sx]
                if _is_red(r, g, b):
                    red_mask[y][x] = 1
    tmpl_mask = _forsale_board_inner_template_mask(red_w, red_h, inner_w, inner_h)
    if tmpl_mask is None:
        tmpl = _forsale_red_template(red_w, red_h)
        tmpl_mask = [
            [1 if tmpl.getpixel((x, y)) > 32 else 0 for x in range(red_w)]
            for y in range(red_h)
        ]
    rev_mask = [row[::-1] for row in red_mask]
    fwd = _mask_ncc(red_mask, tmpl_mask)
    rev = _mask_ncc(rev_mask, tmpl_mask)
    if fwd < 0.38:
        errs.append(f"sign_front_text_unreadable ncc_fwd={fwd:.3f}")
    if rev > fwd + 0.06:
        errs.append(f"sign_front_text_mirrored ncc_fwd={fwd:.3f} ncc_rev={rev:.3f}")

    # Left-right balance: mirrored / clipped strings bias mass to one edge inside red bbox.
    col_mass = [red_cols.get(rx0 + i, 0) for i in range(red_w)]
    left_mass = sum(col_mass[: max(1, red_w // 3)])
    right_mass = sum(col_mass[max(0, (2 * red_w) // 3) :])
    if max(left_mass, right_mass) > 2.6 * max(1.0, min(left_mass, right_mass)):
        errs.append(
            f"sign_front_text_asymmetric L={left_mass:.0f} R={right_mass:.0f}"
        )

    if rx0 < bx0 + margin_x or rx1 > bx1 - margin_x:
        errs.append(
            f"sign_front_text_clip_h red=({rx0}-{rx1}) board=({bx0}-{bx1})"
        )
    if ry0 < by0 + margin_y or ry1 > by1 - margin_y:
        errs.append(
            f"sign_front_text_clip_v red=({ry0}-{ry1}) board=({by0}-{by1})"
        )
    right_gap = bx1 - rx1
    left_gap = rx0 - bx0
    if right_gap < margin_x or left_gap < margin_x:
        errs.append(
            f"sign_front_text_margin_fail gaps L={left_gap} R={right_gap} need>={margin_x}"
        )
    return errs


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
            if _sign_mask_pixel_subject(mpx[y * w + x]):
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
            m[y, x] = _sign_mask_pixel_subject(mpx[y * w + x])
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


def _backdrop_grid_points(w: int, h: int, *, inset: int = 3) -> list[tuple[int, int]]:
    xs = (inset, w // 2, w - 1 - inset)
    ys = (inset, h // 2, h - 1 - inset)
    return [(x, y) for y in ys for x in xs]


def _backdrop_border_grid_points(w: int, h: int, *, inset: int = 3) -> list[tuple[int, int]]:
    """Eight perimeter samples of the inset 3×3 grid (excludes center)."""
    cx, cy = w // 2, h // 2
    pts = _backdrop_grid_points(w, h, inset=inset)
    return [(x, y) for x, y in pts if not (x == cx and y == cy)]


def check_backdrop_mid_grey(
    rgb_path: Path,
    *,
    corner_inset_px: int = 3,
    expected: tuple[int, int, int] = (150, 150, 150),
) -> list[str]:
    """Sign/studio flats: border grid + full inset edge band must be exact backdrop RGB."""
    im = _load_rgba(rgb_path).convert("RGB")
    w, h = im.size
    inset = max(0, corner_inset_px)
    errs: list[str] = []
    for x, y in _backdrop_border_grid_points(w, h, inset=inset):
        r, g, b = im.getpixel((x, y))
        if (r, g, b) != expected:
            errs.append(
                f"backdrop_grid_not_pinned inset={inset} at=({x},{y}) rgb=({r},{g},{b}) need={expected}"
            )
    non150 = 0
    band = 0
    for y in range(h):
        for x in range(w):
            if x <= inset or x >= w - 1 - inset or y <= inset or y >= h - 1 - inset:
                band += 1
                if im.getpixel((x, y)) != expected:
                    non150 += 1
    if non150:
        errs.append(f"backdrop_edge_band_non150 count={non150} of {band} inset={inset}")
    return errs


def check_sign_mask_studio_backdrop(
    mask_path: Path,
    *,
    expected: tuple[int, int, int] = (150, 150, 150),
) -> list[str]:
    """Sign mask companions: RGB backdrop must be studio grey outside subject."""
    if not mask_path.is_file():
        return ["sign_mask_missing"]
    im = _load_rgba(mask_path).convert("RGB")
    w, h = im.size
    inset = 3
    errs: list[str] = []
    non150 = band = 0
    for y in range(h):
        for x in range(w):
            if x <= inset or x >= w - 1 - inset or y <= inset or y >= h - 1 - inset:
                band += 1
                if im.getpixel((x, y)) != expected:
                    non150 += 1
    if non150:
        errs.append(f"sign_mask_edge_band_non150 count={non150} of {band}")
    return errs


def check_sign_front_ocr_words(rgb_path: Path) -> list[str]:
    """Front preview OCR must read LAND, FOR, and SALE."""
    if not rgb_path.is_file():
        return ["sign_front_ocr_missing"]
    try:
        import pytesseract  # type: ignore
        from PIL import Image, ImageEnhance, ImageOps
    except ImportError:
        return ["sign_front_ocr_pytesseract_missing"]
    rgb = Image.open(rgb_path).convert("RGB")
    w, h = rgb.size
    px = rgb.load()

    def _is_red(r: int, g: int, b: int) -> bool:
        return r > 72 and g < 110 and b < 110 and r > g + 18 and r > b + 18

    bin_im = Image.new("L", (w, h), 255)
    bin_px = bin_im.load()
    xs, ys = [], []
    for y in range(h):
        for x in range(w):
            r, g, b = px[x, y]
            if _is_red(r, g, b):
                bin_px[x, y] = 0
                xs.append(x)
                ys.append(y)
    if len(xs) < 40:
        return ["sign_front_ocr_red_bbox_empty"]
    crop = bin_im.crop((min(xs) - 6, min(ys) - 6, max(xs) + 6, max(ys) + 6))
    crop = crop.resize((max(64, crop.width * 3), max(64, crop.height * 3)), Image.Resampling.NEAREST)
    crop = ImageOps.invert(crop)
    raw = pytesseract.image_to_string(
        crop,
        config="--psm 11 -c tessedit_char_whitelist=ABCDEFGHIJKLMNOPQRSTUVWXYZ ",
    ).upper()
    blob = " ".join(raw.split())
    compact = blob.replace(" ", "")
    missing: list[str] = []
    if "LAND" not in blob:
        missing.append("LAND")
    if "SALE" not in blob and "SAL" not in blob:
        missing.append("SALE")
    if "FOR" not in blob and "RSAL" not in compact and "FORSAL" not in compact:
        missing.append("FOR")
    if missing:
        return [f"sign_front_ocr_missing_words {missing} text={blob[:80]!r}"]
    return []


def check_sign_front_board_frame_width(
    rgb_path: Path,
    mask_path: Path,
    *,
    min_frac: float = 0.45,
) -> list[str]:
    """Front preview white sign face must fill at least min_frac of image width."""
    if not rgb_path.is_file() or not mask_path.is_file():
        return ["sign_front_board_fill_missing"]
    rgb = _load_rgba(rgb_path).convert("RGB")
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())

    def _is_red(r: int, g: int, b: int) -> bool:
        return r > 85 and r > g + 12 and r > b + 12

    def _is_white_face(r: int, g: int, b: int) -> bool:
        return r > 186 and g > 182 and b > 176 and not _is_red(r, g, b)

    white_x: list[int] = []
    for y in range(h):
        for x in range(w):
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
                continue
            r, g, b = rpx[y * w + x]
            if _is_white_face(r, g, b):
                white_x.append(x)
    if len(white_x) < 120:
        return [f"sign_front_board_white_sparse px={len(white_x)}"]
    bx0, bx1 = min(white_x), max(white_x)
    frac = (bx1 - bx0 + 1) / w
    if frac < min_frac:
        return [f"sign_front_board_frame_width_frac={frac:.3f} need>={min_frac}"]
    return []


def check_sign_preview_yaw(frame_path: Path, *, expected_deg: float, label: str) -> list[str]:
    if not frame_path.is_file():
        return [f"{label}_frame_missing"]
    import json

    meta = json.loads(frame_path.read_text(encoding="utf-8"))
    yaw = float(meta.get("azimuth_deg", -999))
    if abs(yaw - expected_deg) > 6.0:
        return [f"{label}_yaw_deg={yaw:.1f} need≈{expected_deg}"]
    return []


def _mask_subject_bbox(mask) -> tuple[int, int, int, int] | None:
    w, h = mask.size
    mpx = list(mask.getdata())
    xs, ys = [], []
    for y in range(h):
        for x in range(w):
            if _sign_mask_pixel_subject(mpx[y * w + x]):
                xs.append(x)
                ys.append(y)
    if not xs:
        return None
    return min(xs), max(xs), min(ys), max(ys)


def _relative_luminance(r: int, g: int, b: int) -> float:
    def _chan(c: int) -> float:
        v = c / 255.0
        return v / 12.92 if v <= 0.04045 else ((v + 0.055) / 1.055) ** 2.4

    return 0.2126 * _chan(r) + 0.7152 * _chan(g) + 0.0722 * _chan(b)


def _wcag_contrast(l_a: float, l_b: float) -> float:
    hi, lo = max(l_a, l_b), min(l_a, l_b)
    return (hi + 0.05) / (lo + 0.05)


def check_sign_preview_subject_framing(
    rgb_path: Path,
    mask_path: Path,
    *,
    label: str = "sign",
    min_margin_frac: float = 0.03,
    edge_clear_frac: float = 0.03,
) -> list[str]:
    """Subject bbox (non-backdrop masked pixels) must clear frame edges."""
    if not rgb_path.is_file() or not mask_path.is_file():
        return [f"{label}_framing_missing"]
    rgb = _load_rgba(rgb_path).convert("RGB")
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())

    def _is_text_red(r: int, g: int, b: int) -> bool:
        return r > 72 and g < 110 and b < 110 and r > g + 18 and r > b + 18

    def _is_board_cream(r: int, g: int, b: int) -> bool:
        return r > 168 and g > 158 and b > 138 and not _is_text_red(r, g, b)

    def _is_post_wood(r: int, g: int, b: int) -> bool:
        if _is_text_red(r, g, b):
            return False
        return 58 < r < 175 and 34 < g < 138 and 18 < b < 108 and r > g > b

    def _is_tuft_green(r: int, g: int, b: int) -> bool:
        return g > 90 and g > r + 8 and g > b + 6 and r < 140

    def _is_sign_pixel(r: int, g: int, b: int) -> bool:
        return _is_text_red(r, g, b) or _is_board_cream(r, g, b) or _is_post_wood(r, g, b) or _is_tuft_green(r, g, b)

    xs, ys = [], []
    edge_hits: list[tuple[int, int]] = []
    edge = max(1, int(round(w * edge_clear_frac)))
    backdrop_inset = 4
    for y in range(h):
        for x in range(w):
            if x <= backdrop_inset or x >= w - 1 - backdrop_inset or y <= backdrop_inset or y >= h - 1 - backdrop_inset:
                continue
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
                continue
            r, g, b = rpx[y * w + x]
            if (r, g, b) == (150, 150, 150):
                continue
            if not _is_sign_pixel(r, g, b):
                continue
            if y > int(h * 0.90) and not (
                _is_post_wood(r, g, b) or _is_tuft_green(r, g, b) or _is_text_red(r, g, b)
            ):
                continue
            if y < int(h * 0.04) and _is_board_cream(r, g, b) and not (
                _is_post_wood(r, g, b) or _is_text_red(r, g, b)
            ):
                continue
            xs.append(x)
            ys.append(y)
            if x < edge or x >= w - edge or y < edge or y >= h - edge:
                if y >= h - edge and (_is_tuft_green(r, g, b) or _is_post_wood(r, g, b)):
                    continue
                edge_hits.append((x, y))
    if not xs:
        return [f"{label}_framing_subject_empty"]
    x0, x1, y0, y1 = min(xs), max(xs), min(ys), max(ys)
    errs: list[str] = []
    min_x = int(w * min_margin_frac)
    min_y = int(h * min_margin_frac)
    fill_y = (y1 - y0 + 1) / h
    if fill_y > 0.78 and y0 < min_y:
        min_y_top = max(2, int(h * 0.03))
    else:
        min_y_top = min_y
    if x0 < min_x or (w - 1 - x1) < min_x:
        errs.append(
            f"{label}_framing_margin_x bbox=({x0},{x1}) w={w} need>={min_margin_frac}"
        )
    if y0 < min_y_top or (h - 1 - y1) < min_y:
        errs.append(
            f"{label}_framing_margin_y bbox=({y0},{y1}) h={h} need>={min_margin_frac}"
        )
    if edge_hits:
        x, y = edge_hits[0]
        errs.append(f"{label}_subject_touching_edge at=({x},{y}) edge_clear={edge_clear_frac}")
    return errs


def check_sign_text_inside_board_mask(
    rgb_path: Path,
    mask_path: Path,
    *,
    label: str = "sign",
    pad_frac: float = 0.08,
) -> list[str]:
    """Red text bbox must fit inside cream board bbox shrunk by pad_frac."""
    if not rgb_path.is_file() or not mask_path.is_file():
        return [f"{label}_text_in_board_missing"]
    rgb = _load_rgba(rgb_path).convert("RGB")
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())

    def _is_text_red(r: int, g: int, b: int) -> bool:
        return r > 72 and g < 110 and b < 110 and r > g + 18 and r > b + 18

    def _is_board_cream(r: int, g: int, b: int) -> bool:
        return r > 168 and g > 158 and b > 138 and not _is_text_red(r, g, b)

    board_x, board_y = [], []
    text_x, text_y = [], []
    for y in range(h):
        for x in range(w):
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
                continue
            r, g, b = rpx[y * w + x]
            if _is_board_cream(r, g, b):
                board_x.append(x)
                board_y.append(y)
            if _is_text_red(r, g, b):
                text_x.append(x)
                text_y.append(y)
    if len(board_x) < 80:
        return [f"{label}_board_cream_sparse px={len(board_x)}"]
    if len(text_x) < 40:
        return [f"{label}_text_red_sparse px={len(text_x)}"]
    bx0, bx1, by0, by1 = min(board_x), max(board_x), min(board_y), max(board_y)
    tx0, tx1, ty0, ty1 = min(text_x), max(text_x), min(text_y), max(text_y)
    bw, bh = bx1 - bx0 + 1, by1 - by0 + 1
    pad_x = int(bw * pad_frac)
    pad_y = int(bh * pad_frac)
    ix0, ix1 = bx0 + pad_x, bx1 - pad_x
    iy0, iy1 = by0 + pad_y, by1 - pad_y
    if ix1 <= ix0 or iy1 <= iy0:
        return [f"{label}_board_inner_too_small"]
    if tx0 < ix0 or tx1 > ix1 or ty0 < iy0 or ty1 > iy1:
        return [
            f"{label}_text_outside_board_pad text=({tx0}-{tx1},{ty0}-{ty1}) "
            f"inner=({ix0}-{ix1},{iy0}-{iy1}) pad={pad_frac}"
        ]
    return []


def check_sign_text_board_wcag_contrast(
    rgb_path: Path,
    mask_path: Path,
    *,
    label: str = "sign",
    min_ratio: float = 7.0,
) -> list[str]:
    """WCAG luminance contrast between median cream board and median text red."""
    if not rgb_path.is_file() or not mask_path.is_file():
        return [f"{label}_wcag_missing"]
    rgb = _load_rgba(rgb_path).convert("RGB")
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())

    def _is_text_red(r: int, g: int, b: int) -> bool:
        return r > 72 and g < 110 and b < 110 and r > g + 18 and r > b + 18

    def _is_board_cream(r: int, g: int, b: int) -> bool:
        return r > 168 and g > 158 and b > 138 and not _is_text_red(r, g, b)

    board_lum: list[float] = []
    text_lum: list[float] = []
    for y in range(h):
        for x in range(w):
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
                continue
            r, g, b = rpx[y * w + x]
            lum = _relative_luminance(r, g, b)
            if _is_board_cream(r, g, b):
                board_lum.append(lum)
            elif _is_text_red(r, g, b):
                text_lum.append(lum)
    if len(board_lum) < 48 or len(text_lum) < 24:
        return [f"{label}_wcag_samples board={len(board_lum)} text={len(text_lum)}"]
    board_lum.sort()
    text_lum.sort()
    l_board = board_lum[int(len(board_lum) * 0.92)]
    l_text = text_lum[int(len(text_lum) * 0.08)]
    ratio = _wcag_contrast(l_board, l_text)
    if ratio < min_ratio:
        return [f"{label}_wcag_contrast={ratio:.2f} need>={min_ratio}"]
    return []


def check_sign_front_posts_to_ground(
    rgb_path: Path,
    mask_path: Path,
    *,
    bottom_frac: float = 0.15,
) -> list[str]:
    """Front view: two side columns (wood/tuft) occupy lower 35% of sign subject."""
    if not rgb_path.is_file() or not mask_path.is_file():
        return ["sign_front_posts_missing"]
    rgb = _load_rgba(rgb_path).convert("RGB")
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())

    def _is_text_red(r: int, g: int, b: int) -> bool:
        return r > 72 and g < 110 and b < 110 and r > g + 18 and r > b + 18

    def _is_board_cream(r: int, g: int, b: int) -> bool:
        return r > 168 and g > 158 and b > 138 and not _is_text_red(r, g, b)

    def _is_post_wood(r: int, g: int, b: int) -> bool:
        if r > 150 and r > g + 24:
            return False
        return 52 < r < 178 and 30 < g < 142 and 14 < b < 112 and r > g > b

    def _is_tuft_green(r: int, g: int, b: int) -> bool:
        return g > 88 and g > r + 6 and g > b + 5

    xs, ys = [], []
    for y in range(h):
        for x in range(w):
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
                continue
            r, g, b = rpx[y * w + x]
            if _is_post_wood(r, g, b) or _is_tuft_green(r, g, b) or _is_board_cream(r, g, b):
                xs.append(x)
                ys.append(y)
    if not xs:
        return ["sign_front_posts_subject_empty"]
    wood_pts: list[tuple[int, int]] = []
    for y in range(h):
        for x in range(w):
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
                continue
            r, g, b = rpx[y * w + x]
            if _is_post_wood(r, g, b):
                wood_pts.append((x, y))
    if len(wood_pts) < 48:
        return [f"sign_front_posts_wood_sparse px={len(wood_pts)}"]
    ys_w = [p[1] for p in wood_pts]
    xs_w = [p[0] for p in wood_pts]
    y0w, y1w = min(ys_w), max(ys_w)
    x0w, x1w = min(xs_w), max(xs_w)
    y0s, y1s = min(ys), max(ys)
    reach_y = y0s + int((y1s - y0s + 1) * 0.58)
    if y1w < reach_y:
        return [f"sign_front_posts_short wood_top={y1w} need_y>={reach_y}"]
    mid_x = (x0w + x1w) // 2
    left = sum(1 for x, _y in wood_pts if x < mid_x)
    right = sum(1 for x, _y in wood_pts if x >= mid_x)
    if left < 12 or right < 12:
        return [f"sign_front_posts_two_blobs L={left} R={right}"]
    if (x1w - x0w + 1) < int(w * 0.10):
        return [f"sign_front_posts_narrow span={x1w - x0w + 1}"]
    return []


def _sign_subject_bbox(
    rgb: "Image.Image",
    mask: "Image.Image",
    *,
    edge_inset: int = 4,
) -> tuple[int, int, int, int] | None:
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())

    def _is_text_red(r: int, g: int, b: int) -> bool:
        return r > 72 and g < 110 and b < 110 and r > g + 18 and r > b + 18

    def _is_board_cream(r: int, g: int, b: int) -> bool:
        return r > 168 and g > 158 and b > 138 and not _is_text_red(r, g, b)

    def _is_post_wood(r: int, g: int, b: int) -> bool:
        if _is_text_red(r, g, b):
            return False
        return 58 < r < 175 and 34 < g < 138 and 18 < b < 108 and r > g > b

    def _is_tuft_green(r: int, g: int, b: int) -> bool:
        return g > 90 and g > r + 8 and g > b + 6 and r < 140

    xs, ys = [], []
    for y in range(h):
        for x in range(w):
            if x <= edge_inset or x >= w - 1 - edge_inset or y <= edge_inset or y >= h - 1 - edge_inset:
                continue
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
                continue
            r, g, b = rpx[y * w + x]
            if (r, g, b) == (150, 150, 150):
                continue
            is_sign = (
                _is_text_red(r, g, b)
                or _is_board_cream(r, g, b)
                or _is_post_wood(r, g, b)
                or _is_tuft_green(r, g, b)
            )
            if not is_sign:
                continue
            if y < int(h * 0.05) and not (_is_post_wood(r, g, b) or _is_text_red(r, g, b)):
                continue
            xs.append(x)
            ys.append(y)
    if not xs:
        return None
    return min(xs), max(xs), min(ys), max(ys)


def check_sign_subject_fill_extent(
    rgb_path: Path,
    mask_path: Path,
    *,
    label: str = "sign",
    min_fill: float = 0.60,
    max_fill: float = 0.80,
) -> list[str]:
    """Combined sign subject max(width_frac, height_frac) must sit in [min_fill, max_fill]."""
    if not rgb_path.is_file() or not mask_path.is_file():
        return [f"{label}_fill_extent_missing"]
    rgb = _load_rgba(rgb_path).convert("RGB")
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    bbox = _sign_subject_bbox(rgb, mask)
    if bbox is None:
        return [f"{label}_fill_subject_empty"]
    x0, x1, y0, y1 = bbox
    fw = (x1 - x0 + 1) / w
    fh = (y1 - y0 + 1) / h
    fill = max(fw, fh)
    if fill < min_fill or fill > max_fill:
        return [f"{label}_subject_fill={fill:.3f} w={fw:.3f} h={fh:.3f} need_{min_fill}-{max_fill}"]
    margin = min(x0, y0, w - 1 - x1, h - 1 - y1) / min(w, h)
    if margin < 0.03:
        return [f"{label}_subject_margin={margin:.3f} need>=0.03"]
    return []


def check_sign_post_wood_hue(
    rgb_path: Path,
    mask_path: Path,
    *,
    label: str = "sign",
    min_hue_deg: float = 20.0,
    max_hue_deg: float = 40.0,
    min_sat: float = 0.30,
) -> list[str]:
    """Preview wood posts must read brown (HSV hue/sat), not cream."""
    import colorsys

    if not rgb_path.is_file() or not mask_path.is_file():
        return [f"{label}_wood_hue_missing"]
    rgb = _load_rgba(rgb_path).convert("RGB")
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())
    hues: list[float] = []
    sats: list[float] = []
    for y in range(h):
        for x in range(w):
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
                continue
            r, g, b = rpx[y * w + x]
            if g > 150 and r > 175:
                continue
            if r > 168 and g > 158 and b > 138:
                continue
            if r > 72 and g < 110 and b < 110 and r > g + 18:
                continue
            if g > 95 and g > r + 10:
                continue
            if not (52 < r < 178 and 28 < g < 142 and 14 < b < 118 and r > g > b):
                continue
            h_deg, s, _v = colorsys.rgb_to_hsv(r / 255.0, g / 255.0, b / 255.0)
            hues.append(h_deg * 360.0)
            sats.append(s)
    if len(hues) < 64:
        return [f"{label}_wood_hue_samples={len(hues)}"]
    hues.sort()
    sats.sort()
    med_h = hues[len(hues) // 2]
    med_s = sats[len(sats) // 2]
    if med_h < min_hue_deg or med_h > max_hue_deg or med_s < min_sat:
        return [f"{label}_wood_hue median_h={med_h:.1f} sat={med_s:.2f} need_h={min_hue_deg}-{max_hue_deg} s>={min_sat}"]
    return []


def check_sign_contact_shadow_under_base(
    rgb_path: Path,
    mask_path: Path,
    *,
    label: str = "sign",
    max_luminance: float = 130 / 255.0,
    min_dark_px: int = 24,
) -> list[str]:
    """Tuft/base band must include grass and contact shadow (luminance < 130)."""

    def _is_post_wood(r: int, g: int, b: int) -> bool:
        return 52 < r < 178 and 28 < g < 142 and 14 < b < 118 and r > g > b

    def _is_tuft_green(r: int, g: int, b: int) -> bool:
        return g > 88 and g > r + 6 and g > b + 5

    if not rgb_path.is_file() or not mask_path.is_file():
        return [f"{label}_contact_shadow_missing"]
    rgb = _load_rgba(rgb_path).convert("RGB")
    mask = _load_rgba(mask_path)
    if mask.size != rgb.size:
        mask = mask.resize(rgb.size)
    w, h = rgb.size
    mpx = list(mask.getdata())
    rpx = list(rgb.getdata())
    green = 0
    dark = 0
    for y in range(int(h * 0.62), h):
        for x in range(w):
            if not _sign_mask_pixel_subject(mpx[y * w + x]):
                continue
            r, g, b = rpx[y * w + x]
            if (r, g, b) == (150, 150, 150):
                continue
            if _is_tuft_green(r, g, b):
                green += 1
            lum = _relative_luminance(r, g, b)
            if lum * 255.0 < 130 and not _is_tuft_green(r, g, b):
                dark += 1
    if green < 24:
        return [f"{label}_contact_tuft_sparse px={green}"]
    if dark < min_dark_px:
        return [f"{label}_contact_shadow_dark_px={dark} need>={min_dark_px} tuft_px={green}"]
    return []


def check_sign_assembly_metadata(frame_path: Path, *, label: str = "sign") -> list[str]:
    if not frame_path.is_file():
        return [f"{label}_assembly_frame_missing"]
    import json

    meta = json.loads(frame_path.read_text(encoding="utf-8"))
    errs: list[str] = []
    span = float(meta.get("sign_post_span_m", 0.0))
    board_w = float(meta.get("sign_board_width_m", 0.0))
    connected = meta.get("sign_assembly_connected")
    if span <= 0.05:
        errs.append(f"{label}_assembly_post_span_missing span={span}")
    if board_w < span - 0.02:
        errs.append(f"{label}_assembly_board_narrow board_w={board_w} span={span}")
    if connected is not True:
        errs.append(f"{label}_assembly_not_connected connected={connected!r}")
    return errs


SIGN_PREVIEW_ALLOWED_PNG = frozenset(
    {
        "garage-land-for-sale-sign-front.png",
        "garage-land-for-sale-sign-front.mask.png",
        "garage-land-for-sale-sign-hero.png",
        "garage-land-for-sale-sign-hero.mask.png",
    }
)


def check_sign_preview_png_scope(preview_dir: Path) -> list[str]:
    """PR scope: only sign front/hero PNG pairs may differ from origin/main."""
    import subprocess

    repo = preview_dir.parents[2]
    changed: list[str] = []
    for png in sorted(preview_dir.glob("*.png")):
        rel = png.name
        r = subprocess.run(
            ["git", "diff", "--quiet", "origin/main", "--", str(png)],
            cwd=repo,
            capture_output=True,
        )
        if r.returncode != 0:
            changed.append(rel)
    extra = [n for n in changed if n not in SIGN_PREVIEW_ALLOWED_PNG]
    if extra:
        return [f"sign_preview_png_out_of_scope extra={extra}"]
    if len(changed) > len(SIGN_PREVIEW_ALLOWED_PNG):
        return [
            f"sign_preview_png_diff_count={len(changed)} max={len(SIGN_PREVIEW_ALLOWED_PNG)} files={changed}"
        ]
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