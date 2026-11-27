#!/usr/bin/env python3
"""Walk strip: grey backdrop, textured hero all tiles, torso colour vs frame 0."""
from __future__ import annotations

import argparse
import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
GREY = (150, 150, 150)
TORSO_RGB_TOL = 32.0
MIN_TILE_SPREAD = 0.45
MIN_HAND_BELOW_SHOULDER_PX = 28
MAX_SHOULDER_HAND_Y_RATIO = 1.08
LEG_FOREIGN_RGB_TOL = 36.0
MAX_GREY_NEUTRAL = 22.0
MAX_CAMERA_CENTROID_DRIFT_PX = 22


def _tile_stats(img: Image.Image, x0: int, x1: int, y0: int, y1: int) -> tuple[float, float, float, int]:
    rs: list[int] = []
    gs: list[int] = []
    bs: list[int] = []
    for y in range(y0, y1):
        for x in range(x0, x1):
            r, g, b = img.getpixel((x, y))[:3]
            if abs(r - GREY[0]) < 8 and abs(g - GREY[1]) < 8 and abs(b - GREY[2]) < 8:
                continue
            rs.append(r)
            gs.append(g)
            bs.append(b)
    if len(rs) < 80:
        return 0.0, 0.0, 0.0, 0
    return (
        sum(rs) / len(rs),
        sum(gs) / len(gs),
        sum(bs) / len(bs),
        len(rs),
    )


def _leg_window(tile_w: int, h: int, i: int) -> tuple[int, int, int, int]:
    x0 = i * tile_w + tile_w // 6
    x1 = (i + 1) * tile_w - tile_w // 6
    y0 = (3 * h) // 5
    y1 = h - h // 10
    return x0, x1, y0, y1


def _arm_windows(tile_w: int, h: int, i: int) -> tuple[tuple[int, int, int, int], tuple[int, int, int, int]]:
    x0 = i * tile_w + tile_w // 8
    x1 = (i + 1) * tile_w - tile_w // 8
    shoulder = (x0, x1, h // 5, h // 3)
    hand = (x0, x1, h // 2, int(h * 0.72))
    return shoulder, hand


def _mean_non_grey_y(img: Image.Image, x0: int, x1: int, y0: int, y1: int) -> float | None:
    ys: list[int] = []
    for y in range(y0, y1, 2):
        for x in range(x0, x1, 2):
            r, g, b = img.getpixel((x, y))[:3]
            if abs(r - GREY[0]) < 10 and abs(g - GREY[1]) < 10 and abs(b - GREY[2]) < 10:
                continue
            ys.append(y)
    if len(ys) < 40:
        return None
    return sum(ys) / len(ys)


def _leg_foreign_pixels(img: Image.Image, x0: int, x1: int, y0: int, y1: int, asset_id: str) -> int:
    """Skin/tee bleed inside leg silhouette (mesh tear heuristic)."""
    foreign = 0
    total = 0
    for y in range(y0, y1, 2):
        for x in range(x0, x1, 2):
            r, g, b = img.getpixel((x, y))
            if abs(r - GREY[0]) < 10 and abs(g - GREY[1]) < 10 and abs(b - GREY[2]) < 10:
                continue
            total += 1
            if asset_id == "staff-founder-idle":
                if r > 145 and g > 105 and b > 85 and r > b + 8:
                    foreign += 1
            else:
                if r > 152 and g > 118 and b < 105 and r > g + 18:
                    foreign += 1
    return foreign if total > 60 else 0


def _shirt_roi(tile_w: int, h: int, i: int) -> tuple[int, int, int, int]:
    x0 = i * tile_w + tile_w // 4
    x1 = (i + 1) * tile_w - tile_w // 4
    y0 = h // 4
    y1 = h // 2
    return x0, x1, y0, y1


def _is_neutral_grey(r: int, g: int, b: int) -> bool:
    return abs(r - GREY[0]) < 14 and abs(g - GREY[1]) < 14 and abs(b - GREY[2]) < 14


def _char_centroid_x(img: Image.Image, tile_w: int, i: int, h: int) -> float | None:
    xs: list[int] = []
    x0 = i * tile_w
    x1 = (i + 1) * tile_w
    for y in range(h // 6, (5 * h) // 6, 3):
        for x in range(x0 + 8, x1 - 8, 3):
            r, g, b = img.getpixel((x, y))[:3]
            if _is_neutral_grey(r, g, b):
                continue
            xs.append(x)
    if len(xs) < 60:
        return None
    return sum(xs) / len(xs)


def _torso_window(tile_w: int, h: int, i: int) -> tuple[int, int, int, int]:
    x0 = i * tile_w + tile_w // 5
    x1 = (i + 1) * tile_w - tile_w // 5
    y0 = h // 4
    y1 = (3 * h) // 5
    return x0, x1, y0, y1


def _rgb_dist(a: tuple[float, float, float], b: tuple[float, float, float]) -> float:
    return sum((a[i] - b[i]) ** 2 for i in range(3)) ** 0.5


def check_strip(png: Path, asset_id: str) -> tuple[bool, str]:
    if not png.is_file() or png.stat().st_size < 2000:
        return False, "missing_strip"
    img = Image.open(png).convert("RGB")
    w, h = img.size
    if w < 4 * 64 or h < 64:
        return False, f"strip_too_small {w}x{h}"
    tile_w = w // 4
    corner_grey = all(
        abs(img.getpixel((c, h // 2))[0] - GREY[0]) < 12
        for c in (4, tile_w + 4, 2 * tile_w + 4, 3 * tile_w + 4)
    )
    if not corner_grey:
        return False, "backdrop_not_969696"
    tile_means: list[float] = []
    torso_rgb: list[tuple[float, float, float]] = []
    centroids: list[float] = []
    for i in range(4):
        cx = _char_centroid_x(img, tile_w, i, h)
        if cx is None:
            return False, f"tile{i}_silhouette_empty"
        centroids.append(cx - (i * tile_w + tile_w * 0.5))
        sx0, sx1, sy0, sy1 = _shirt_roi(tile_w, h, i)
        sm_r, sm_g, sm_b, sn = _tile_stats(img, sx0, sx1, sy0, sy1)
        if sn < 40:
            return False, f"tile{i}_shirt_roi_empty"
        if asset_id == "staff-founder-idle":
            if _rgb_dist((sm_r, sm_g, sm_b), GREY) < MAX_GREY_NEUTRAL:
                return False, f"tile{i}_grey_shirt mean={(round(sm_r,1), round(sm_g,1), round(sm_b,1))}"
            if sm_r < 85:
                return False, f"tile{i}_founder_shirt_washed r={sm_r:.1f}"
        if asset_id == "staff-researcher-idle":
            skin_y0, skin_y1 = h // 5, h // 3
            sk_r, sk_g, sk_b, sk_n = _tile_stats(img, sx0, sx1, skin_y0, skin_y1)
            if sk_n > 30 and _rgb_dist((sk_r, sk_g, sk_b), GREY) < MAX_GREY_NEUTRAL:
                return False, f"tile{i}_grey_skin mean={(round(sk_r,1), round(sk_g,1), round(sk_b,1))}"
            legx0, legx1, legy0, legy1 = _leg_window(tile_w, h, i)
            lr, lg, lb, ln = _tile_stats(img, legx0, legx1, legy0, legy1)
            if ln > 40 and _rgb_dist((lr, lg, lb), GREY) < MAX_GREY_NEUTRAL:
                return False, f"tile{i}_grey_trousers mean={(round(lr,1), round(lg,1), round(lb,1))}"
        sh_win, hand_win = _arm_windows(tile_w, h, i)
        sh_y = _mean_non_grey_y(img, *sh_win)
        hand_y = _mean_non_grey_y(img, *hand_win)
        if sh_y is None or hand_y is None:
            return False, f"tile{i}_arm_band_empty"
        if hand_y - sh_y < MIN_HAND_BELOW_SHOULDER_PX:
            return False, f"tile{i}_arm_tpose hand_y={hand_y:.1f} sh_y={sh_y:.1f}"
        lx0, lx1, ly0, ly1 = _leg_window(tile_w, h, i)
        foreign = _leg_foreign_pixels(img, lx0, lx1, ly0, ly1, asset_id)
        if foreign > 24:
            return False, f"tile{i}_leg_mesh_tear foreign_px={foreign}"
        x0, x1, y0, y1 = _torso_window(tile_w, h, i)
        mean_r, mean_g, mean_b, n = _tile_stats(img, x0, x1, y0, y1)
        if n < 80:
            return False, f"tile{i}_empty"
        torso_rgb.append((mean_r, mean_g, mean_b))
        _, mx, _mn, _ = _tile_stats(img, x0, x1, y0, y1)
        mx_r = max(
            img.getpixel((x, y))[0]
            for y in range(y0, y1, 4)
            for x in range(x0, x1, 4)
            if abs(img.getpixel((x, y))[0] - GREY[0]) > 8
        ) if n else 0
        tile_means.append(mean_r)
        if mx_r < 55:
            return False, f"tile{i}_grey_base max_r={mx_r}"
    ref = torso_rgb[0]
    for i in range(1, 4):
        d = _rgb_dist(ref, torso_rgb[i])
        if d > TORSO_RGB_TOL:
            return False, f"tile{i}_torso_drift dist={d:.1f} ref={ref} cur={torso_rgb[i]}"
    if asset_id == "staff-founder-idle":
        if max(tile_means) < 90:
            return False, f"founder_tile_washed means={tile_means}"
    else:
        if sum(tile_means) / len(tile_means) < 70:
            return False, f"researcher_tile_washed means={tile_means}"
    if max(tile_means) - min(tile_means) < MIN_TILE_SPREAD:
        return False, f"tiles_identical means={tile_means}"
    if max(centroids) - min(centroids) > MAX_CAMERA_CENTROID_DRIFT_PX:
        return False, f"camera_drift centroid_x={[round(c, 1) for c in centroids]}"
    leg_means = []
    for i in range(4):
        lx0, lx1, ly0, ly1 = _leg_window(tile_w, h, i)
        lr, _, _, ln = _tile_stats(img, lx0, lx1, ly0, ly1)
        if ln > 30:
            leg_means.append(lr)
    if len(leg_means) == 4 and max(leg_means) - min(leg_means) < 3.5:
        return False, f"gait_legs_static leg_r={[round(x,1) for x in leg_means]}"
    return True, (
        f"strip {w}x{h} tiles=4 torso0={tuple(round(c,1) for c in ref)} "
        f"means={[round(m, 1) for m in tile_means]}"
    )


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("asset_id", choices=("staff-founder-idle", "staff-researcher-idle"))
    args = parser.parse_args()
    stem = (
        "staff-founder-walk-strip"
        if args.asset_id == "staff-founder-idle"
        else "staff-researcher-walk-strip"
    )
    png = PREVIEW / f"{stem}.png"
    ok, msg = check_strip(png, args.asset_id)
    if ok:
        print(f"OK {args.asset_id} walk_strip {msg}")
        return 0
    print(f"FAIL {args.asset_id} walk_strip {msg}")
    return 1


if __name__ == "__main__":
    sys.exit(main())