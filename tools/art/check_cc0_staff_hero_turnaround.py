#!/usr/bin/env python3
"""Turnaround PNGs: layout, backdrop, per-panel eyes, materials, front/back differ."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

from turnaround_panel_gates import (
    GREY,
    MAX_PANEL_GREY_MANNEQUIN_FRAC,
    MIN_PANEL_TEXTURED_FRAC_FOUNDER,
    MIN_PANEL_TEXTURED_FRAC_RESEARCHER,
    MIN_PANEL_LUMINANCE,
    head_mean_rgb,
    founder_eye_blobs,
    load_panel_mask,
    figure_column_span,
    MAX_SIDE_SPAN_VS_FRONT,
    MIN_FRONT_BACK_SILHOUETTE_XOR,
    mirror_iou,
    panel_material_metrics,
    researcher_eye_blobs,
    rgb_l1,
    silhouette_xor_frac,
)

REPO = Path(__file__).resolve().parents[2]
GAP = 8
PANELS = 5
MIN_FRONT_BACK_HEAD_L1 = 7.0


def _panel_bounds(w: int, h: int, i: int) -> tuple[int, int, int, int]:
    panel_w = (w - GAP * (PANELS - 1)) // PANELS
    x0 = i * (panel_w + GAP)
    return x0, 0, x0 + panel_w, h


def _eye_count(panel: Image.Image, mask: Image.Image, panel_i: int, asset_id: str) -> int:
    if asset_id == "staff-founder-idle":
        return founder_eye_blobs(mask, panel, panel=panel_i if panel_i < 4 else 1)
    return researcher_eye_blobs(mask, panel, panel=min(panel_i, 3))


def _check(path: Path, name: str, asset_id: str) -> tuple[bool, str]:
    if not path.is_file() or path.stat().st_size < 4000:
        return False, f"missing {name}"
    img = Image.open(path).convert("RGB")
    w, h = img.size
    if w < h * 3:
        return False, f"layout {w}x{h}"
    panel_w = (w - GAP * (PANELS - 1)) // PANELS
    if panel_w < 32:
        return False, f"panel_w={panel_w}"
    samples = [4]
    for i in range(PANELS - 1):
        samples.append(i * (panel_w + GAP) + panel_w + GAP // 2)
    for x in samples:
        r, g, b = img.getpixel((min(x, w - 1), h // 2))
        if abs(r - GREY[0]) > 12 or abs(g - GREY[1]) > 12 or abs(b - GREY[2]) > 12:
            return False, f"backdrop_grey at_x={x} rgb=({r},{g},{b})"

    eye_counts: list[int] = []
    means: list[tuple[float, float, float]] = []
    masks: list[Image.Image] = []
    textures: list[float] = []
    greys: list[float] = []

    for i in range(PANELS):
        x0, y0, x1, y1 = _panel_bounds(w, h, i)
        panel = img.crop((x0, y0, x1, y1))
        mask_path = path.parent / f"{path.stem}.panel{i}.mask.png"
        mask = load_panel_mask(panel, mask_path)
        masks.append(mask)
        tex, grey, luma = panel_material_metrics(panel, mask, asset_id=asset_id)
        textures.append(tex)
        greys.append(grey)
        min_tex = (
            MIN_PANEL_TEXTURED_FRAC_FOUNDER
            if asset_id == "staff-founder-idle"
            else MIN_PANEL_TEXTURED_FRAC_RESEARCHER
        )
        if tex < min_tex:
            return False, f"panel{i}_textured={tex:.3f} want>={min_tex}"
        if grey > MAX_PANEL_GREY_MANNEQUIN_FRAC:
            return False, f"panel{i}_grey_mannequin={grey:.3f} want<={MAX_PANEL_GREY_MANNEQUIN_FRAC}"
        if luma < MIN_PANEL_LUMINANCE:
            return False, f"panel{i}_dim_luma={luma:.1f} want>={MIN_PANEL_LUMINANCE}"
        if asset_id == "staff-researcher-idle":
            from researcher_render_pixel_gates import check_researcher_panel_pixels

            ok_px, msg_px = check_researcher_panel_pixels(panel, mask)
            if not ok_px:
                return False, f"panel{i}_pixel_gate {msg_px}"
        if i < 4:
            eye_counts.append(_eye_count(panel, mask, i, asset_id))
            means.append(head_mean_rgb(panel, mask))

    limits = {0: (2, 2), 1: (0, 1), 2: (0, 0), 3: (0, 1)}
    for i, (_lo, hi) in limits.items():
        c = eye_counts[i]
        if i == 0:
            if c != 2:
                return False, f"panel{i}_eyes={c} want=2"
        elif i == 2:
            if c != 0:
                return False, f"panel{i}_eyes={c} want=0"
        elif c > hi:
            return False, f"panel{i}_eyes={c} want<={hi}"

    fb = rgb_l1(means[0], means[2])
    if fb < MIN_FRONT_BACK_HEAD_L1:
        return False, f"front_back_flat head_l1={fb:.1f} want>={MIN_FRONT_BACK_HEAD_L1}"
    sil_xor = silhouette_xor_frac(masks[0], masks[2])
    if sil_xor < MIN_FRONT_BACK_SILHOUETTE_XOR:
        return False, f"front_back_silhouette_xor={sil_xor:.3f} want>={MIN_FRONT_BACK_SILHOUETTE_XOR}"
    front_span = figure_column_span(masks[0])
    side_span = min(figure_column_span(masks[1]), figure_column_span(masks[3]))
    if front_span >= 48 and side_span / front_span > MAX_SIDE_SPAN_VS_FRONT:
        return False, (
            f"side_not_narrower span_side={side_span} span_front={front_span} "
            f"ratio={side_span / front_span:.3f} want<={MAX_SIDE_SPAN_VS_FRONT}"
        )
    return True, (
        f"ok eyes={eye_counts} textures={[round(t, 2) for t in textures]} "
        f"grey={[round(g, 2) for g in greys]} head_l1={fb:.1f} sil_xor={sil_xor:.3f}"
    )


def main() -> int:
    sys.path.insert(0, str(REPO / "tools" / "art"))
    rc = 0
    for stem, asset in (
        ("staff-founder-turnaround", "staff-founder-idle"),
        ("staff-researcher-turnaround", "staff-researcher-idle"),
    ):
        ok, msg = _check(REPO / "docs/art/previews" / f"{stem}.png", stem, asset)
        tag = "OK" if ok else "FAIL"
        print(f"{tag} {asset} turnaround {msg}")
        if not ok:
            rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())