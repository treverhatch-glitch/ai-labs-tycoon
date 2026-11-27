#!/usr/bin/env python3
"""Turnaround PNGs: layout, backdrop, per-panel eyes, front/back differ."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

from turnaround_panel_gates import (
    GREY,
    figure_mean_rgb,
    founder_eye_blobs,
    load_panel_mask,
    mirror_iou,
    researcher_eye_blobs,
    rgb_l1,
)

REPO = Path(__file__).resolve().parents[2]
GAP = 8
PANELS = 5
MIN_FRONT_BACK_RGB_L1 = 18.0
MAX_SIDE_FRONT_RGB_L1 = 14.0


def _panel_bounds(w: int, h: int, i: int) -> tuple[int, int, int, int]:
    panel_w = (w - GAP * (PANELS - 1)) // PANELS
    x0 = i * (panel_w + GAP)
    return x0, 0, x0 + panel_w, h


def _eye_count(panel: Image.Image, mask: Image.Image, panel_i: int, asset_id: str) -> int:
    if asset_id == "staff-founder-idle":
        return founder_eye_blobs(mask, panel, panel=panel_i)
    return researcher_eye_blobs(mask, panel, panel=panel_i)


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
    for i in range(4):
        x0, y0, x1, y1 = _panel_bounds(w, h, i)
        panel = img.crop((x0, y0, x1, y1))
        mask_path = path.parent / f"{path.stem}.panel{i}.mask.png"
        mask = load_panel_mask(panel, mask_path)
        masks.append(mask)
        eye_counts.append(_eye_count(panel, mask, i, asset_id))
        means.append(figure_mean_rgb(panel, mask))

    limits = {0: (2, 2), 1: (0, 1), 2: (0, 0), 3: (0, 1)}
    for i, (lo, hi) in limits.items():
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
    if fb < MIN_FRONT_BACK_RGB_L1:
        return False, f"front_back_flat rgb_l1={fb:.1f} want>={MIN_FRONT_BACK_RGB_L1}"
    if abs(means[0][0] - means[1][0]) > MAX_SIDE_FRONT_RGB_L1 and fb < MIN_FRONT_BACK_RGB_L1 + 4:
        pass
    side_delta = min(rgb_l1(means[0], means[1]), rgb_l1(means[0], means[3]))
    if side_delta < 6.0 and fb < MIN_FRONT_BACK_RGB_L1 + 2:
        return False, f"panels_too_similar side_l1={side_delta:.1f} front_back={fb:.1f}"
    iou_front = mirror_iou(masks[0])
    iou_back = mirror_iou(masks[2])
    if iou_front > 0.02 and abs(iou_front - iou_back) < 0.04 and fb < MIN_FRONT_BACK_RGB_L1:
        return False, f"mirror_iou_flat front={iou_front:.3f} back={iou_back:.3f}"
    return True, f"ok eyes={eye_counts} rgb_l1_front_back={fb:.1f}"


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
    raise SystemExit(main())