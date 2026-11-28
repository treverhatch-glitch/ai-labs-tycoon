#!/usr/bin/env python3
"""Hatch preview metrics: alpha-mask silhouette + object-only saturation."""
from __future__ import annotations

import json
from pathlib import Path

MIN_SAT = 0.15
MIN_SILHOUETTE_HEIGHT = 0.40
MAX_SILHOUETTE_HEIGHT = 0.75
MIN_LUMA_VAR = 0.0005
EDGE_EPSILON = 0.008  # normalized; alpha bbox must not touch frame edge
ALPHA_THRESHOLD = 200  # ignore EEVEE fringe; object-alpha only


def _pixel_sat(r: int, g: int, b: int) -> float:
    rf, gf, bf = r / 255.0, g / 255.0, b / 255.0
    mx = max(rf, gf, bf)
    mn = min(rf, gf, bf)
    if mx <= 1e-6:
        return 0.0
    return (mx - mn) / mx


def _object_mask_from_images(
    _rgb_pixels: list[tuple[int, int, int, int]] | None,
    mask_pixels: list[tuple[int, int, int, int]],
    _w: int,
    _h: int,
) -> list[bool]:
    """True where alpha-pass pixel belongs to the hero (no luminance / full-frame fallback)."""
    return [mc[3] >= ALPHA_THRESHOLD for mc in mask_pixels]


def analyze_alpha_mask(mask_path: Path) -> dict[str, float | bool | list[float]]:
    from PIL import Image

    im = Image.open(mask_path).convert("RGBA")
    w, h = im.size
    pixels = list(im.getdata())
    mask = _object_mask_from_images(None, pixels, w, h)
    xs: list[int] = []
    ys: list[int] = []
    for y in range(h):
        for x in range(w):
            if mask[y * w + x]:
                xs.append(x)
                ys.append(y)
    if not xs:
        return {
            "height_coverage": 0.0,
            "width_coverage": 0.0,
            "area_coverage": 0.0,
            "touches_edge": True,
            "bbox_norm": [0.0, 0.0, 0.0, 0.0],
            "object_pixels": 0,
        }
    x0, x1 = min(xs), max(xs)
    y0, y1 = min(ys), max(ys)
    height_cov = (y1 - y0 + 1) / h
    width_cov = (x1 - x0 + 1) / w
    area_cov = len(xs) / (w * h)
    extent_cov = max(height_cov, width_cov)
    u0, v0 = x0 / w, y0 / h
    u1, v1 = (x1 + 1) / w, (y1 + 1) / h
    touches = (
        u0 <= EDGE_EPSILON
        or v0 <= EDGE_EPSILON
        or u1 >= 1.0 - EDGE_EPSILON
        or v1 >= 1.0 - EDGE_EPSILON
    )
    return {
        "height_coverage": height_cov,
        "width_coverage": width_cov,
        "extent_coverage": extent_cov,
        "area_coverage": area_cov,
        "touches_edge": touches,
        "bbox_norm": [u0, v0, u1, v1],
        "object_pixels": len(xs),
    }


def analyze_preview(rgb_path: Path, mask_path: Path | None) -> tuple[float, float, float, float, int]:
    from PIL import Image

    im = Image.open(rgb_path).convert("RGBA")
    w, h = im.size
    rgb_pixels = list(im.getdata())
    if mask_path is None or not mask_path.exists():
        mask_path = rgb_path.with_name(rgb_path.stem + ".mask.png")
    if not mask_path.exists():
        return 0.0, 0.0, 0.0, 0.0, 0

    mask_im = Image.open(mask_path).convert("RGBA")
    if mask_im.size != (w, h):
        mask_im = mask_im.resize((w, h), Image.Resampling.BILINEAR)
    mask_pixels = list(mask_im.getdata())
    is_obj = _object_mask_from_images(rgb_pixels, mask_pixels, w, h)

    obj_sats: list[float] = []
    obj_lum: list[float] = []
    for i, on in enumerate(is_obj):
        if not on:
            continue
        r, g, b, _a = rgb_pixels[i]
        obj_sats.append(_pixel_sat(r, g, b))
        obj_lum.append(0.2126 * r / 255 + 0.7152 * g / 255 + 0.0722 * b / 255)

    alpha = analyze_alpha_mask(mask_path)
    height_cov = float(alpha["extent_coverage"])
    mean_sat = sum(obj_sats) / len(obj_sats) if obj_sats else 0.0
    if obj_lum:
        lbar = sum(obj_lum) / len(obj_lum)
        lvar = sum((x - lbar) ** 2 for x in obj_lum) / len(obj_lum)
    else:
        lvar = 0.0
    return mean_sat, lvar, height_cov, 0.0, int(alpha.get("object_pixels", 0))


def mask_gate_errors(mask_path: Path, *, asset_id: str = "") -> list[str]:
    if not mask_path.exists():
        return [f"missing mask {mask_path.name}"]
    alpha = analyze_alpha_mask(mask_path)
    errs: list[str] = []
    if alpha["touches_edge"] and asset_id != "garage-scene-kit":
        errs.append("alpha_bbox_touches_edge")
    ext = float(alpha["extent_coverage"])
    lo = 0.38 if asset_id == "garage-scene-kit" else MIN_SILHOUETTE_HEIGHT
    if ext < lo or ext > MAX_SILHOUETTE_HEIGHT:
        errs.append(f"alpha_extent={ext:.3f} need {MIN_SILHOUETTE_HEIGHT}-{MAX_SILHOUETTE_HEIGHT}")
    if int(alpha.get("object_pixels", 0)) < 64:
        errs.append("alpha_mask_too_empty")
    return errs