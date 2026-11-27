#!/usr/bin/env python3
"""Walk strip gates: backdrop, materials, arms, camera, gait.

Arm T-pose heuristic (PNG): upper-body horizontal span must stay below MAX_ARM_SPAN_FRAC
of tile width (complements Blender gate hand Z <= shoulder_Z - 0.30*arm_length).

Researcher lab-coat sleeve flare: white coat pixels must not extend laterally beyond
torso shoulder silhouette width + COAT_FLARE_SHOULDER_PAD (15%).
"""
from __future__ import annotations

import argparse
import sys
from collections import deque
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
MAX_ARM_SPAN_FRAC = 0.58
COAT_FLARE_SHOULDER_PAD = 0.15
MIN_COAT_FLARE_FAIL_PX = 14
MAX_HEAD_FACE_BLUE_PX = 10
MAX_HAND_BLUE_PX = 16
MAX_SLEEVE_BLUE_PX = 12
MIN_COAT_BODY_PX = 80
MIN_COAT_MAJOR_COMPONENT_PX = 20
MAX_COAT_ENCLOSED_DARK_FRAC = 0.02


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


def _leg_foreign_pixels(
    img: Image.Image, x0: int, x1: int, y0: int, y1: int, asset_id: str
) -> tuple[int, int]:
    """Skin/tee bleed inside lower leg (mesh tear); ignore upper band (hands overlap in walk)."""
    y_cut = y0 + (y1 - y0) // 3
    foreign = 0
    total = 0
    for y in range(y_cut, y1, 2):
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
    if total < 60:
        return 0, 0
    return foreign, total


def _shirt_roi(tile_w: int, h: int, i: int) -> tuple[int, int, int, int]:
    x0 = i * tile_w + tile_w // 4
    x1 = (i + 1) * tile_w - tile_w // 4
    y0 = h // 4
    y1 = h // 2
    return x0, x1, y0, y1


def _is_neutral_grey(r: int, g: int, b: int) -> bool:
    return abs(r - GREY[0]) < 14 and abs(g - GREY[1]) < 14 and abs(b - GREY[2]) < 14


def _is_lab_coat_white(r: int, g: int, b: int) -> bool:
    return (
        r >= 188
        and g >= 183
        and b >= 176
        and (r - b) < 36
        and abs(r - g) < 26
        and max(r, g, b) - min(r, g, b) < 42
    )


def _is_researcher_shirt_blue(r: int, g: int, b: int) -> bool:
    """Tee / teal shirt slots — not lit skin, white collar, or lab coat."""
    if _is_lab_coat_white(r, g, b):
        return False
    tee = g > 158 and b > 168 and r < 138 and g > r + 28
    teal = r < 92 and g > 122 and 108 < b < 152 and g >= b - 2
    bleed = b > r + 20 and g > r + 8 and b >= 148 and r < 168
    return tee or teal or bleed


def _is_trouser_blue(r: int, g: int, b: int) -> bool:
    return b > 92 and r < 95 and g < 102 and b > g + 6


def _head_face_window(tile_w: int, h: int, i: int) -> tuple[int, int, int, int]:
    """Full head + jaw (not neck-band only)."""
    x0 = i * tile_w + tile_w // 6
    x1 = (i + 1) * tile_w - tile_w // 6
    y0 = h // 10
    y1 = h // 3
    return x0, x1, y0, y1


def _researcher_head_face_blue_px(img: Image.Image, tile_w: int, h: int, i: int) -> int:
    x0, x1, y0, y1 = _head_face_window(tile_w, h, i)
    blue = 0
    for y in range(y0, y1, 2):
        for x in range(x0, x1, 2):
            r, g, b = img.getpixel((x, y))[:3]
            if _is_neutral_grey(r, g, b):
                continue
            if _is_researcher_shirt_blue(r, g, b):
                blue += 1
    return blue


def _hand_blue_window(tile_w: int, h: int, i: int) -> tuple[int, int, int, int]:
    x0 = i * tile_w + tile_w // 8
    x1 = (i + 1) * tile_w - tile_w // 8
    y0 = int(h * 0.58)
    y1 = int(h * 0.71)
    return x0, x1, y0, y1


def _researcher_hand_blue_px(img: Image.Image, tile_w: int, h: int, i: int) -> int:
    x0, x1, y0, y1 = _hand_blue_window(tile_w, h, i)
    blue = 0
    for y in range(y0, y1, 2):
        for x in range(x0, x1, 2):
            r, g, b = img.getpixel((x, y))[:3]
            if _is_neutral_grey(r, g, b):
                continue
            if _is_researcher_shirt_blue(r, g, b):
                blue += 1
    return blue


def _sleeve_blue_window(tile_w: int, h: int, i: int) -> tuple[int, int, int, int]:
    x0 = i * tile_w + tile_w // 10
    x1 = (i + 1) * tile_w - tile_w // 10
    y0 = h // 6
    y1 = h // 2
    return x0, x1, y0, y1


def _researcher_sleeve_blue_px(img: Image.Image, tile_w: int, h: int, i: int) -> int:
    """Shirt-blue on upper arms / shoulders (outside narrow torso band)."""
    x0, x1, y0, y1 = _sleeve_blue_window(tile_w, h, i)
    cx = (x0 + x1) * 0.5
    half_torso = (x1 - x0) * 0.17
    blue = 0
    for y in range(y0, y1, 2):
        for x in range(x0, x1, 2):
            if abs(x - cx) < half_torso:
                continue
            r, g, b = img.getpixel((x, y))[:3]
            if _is_neutral_grey(r, g, b):
                continue
            if _is_researcher_shirt_blue(r, g, b):
                blue += 1
    return blue


def _tile_stats_excluding_coat(
    img: Image.Image, x0: int, x1: int, y0: int, y1: int
) -> tuple[float, float, float, int]:
    rs: list[int] = []
    gs: list[int] = []
    bs: list[int] = []
    for y in range(y0, y1):
        for x in range(x0, x1):
            r, g, b = img.getpixel((x, y))[:3]
            if abs(r - GREY[0]) < 8 and abs(g - GREY[1]) < 8 and abs(b - GREY[2]) < 8:
                continue
            if _is_lab_coat_white(r, g, b):
                continue
            rs.append(r)
            gs.append(g)
            bs.append(b)
    if len(rs) < 40:
        return 0.0, 0.0, 0.0, 0
    return sum(rs) / len(rs), sum(gs) / len(gs), sum(bs) / len(bs), len(rs)


def _coat_body_window(tile_w: int, h: int, i: int) -> tuple[int, int, int, int]:
    x0 = i * tile_w + tile_w // 10
    x1 = (i + 1) * tile_w - tile_w // 10
    y0 = h // 3
    y1 = int(h * 0.62)
    return x0, x1, y0, y1


def _is_enclosed_dark(r: int, g: int, b: int) -> bool:
    if _is_neutral_grey(r, g, b):
        return False
    if _is_lab_coat_white(r, g, b):
        return False
    if _is_trouser_blue(r, g, b):
        return True
    return r < 118 and g < 128 and b < 135


def _researcher_coat_shell_gate(
    img: Image.Image, tile_w: int, h: int, i: int
) -> tuple[int, int, float, int]:
    """Chest→hem in coat bbox: one coat component; enclosed dark < 2% of coat area."""
    x0, x1, y0, y1 = _coat_body_window(tile_w, h, i)
    step = 2
    w = max(4, (x1 - x0) // step)
    hh = max(4, (y1 - y0) // step)
    coat = [[False] * w for _ in range(hh)]
    for yi in range(hh):
        for xi in range(w):
            r, g, b = img.getpixel((x0 + xi * step, y0 + yi * step))[:3]
            if _is_lab_coat_white(r, g, b):
                coat[yi][xi] = True
    total_coat = sum(sum(row) for row in coat)
    seen = [[False] * w for _ in range(hh)]
    sizes: list[int] = []
    for yi in range(hh):
        for xi in range(w):
            if not coat[yi][xi] or seen[yi][xi]:
                continue
            q: deque[tuple[int, int]] = deque([(xi, yi)])
            seen[yi][xi] = True
            n = 0
            while q:
                cx, cy = q.popleft()
                n += 1
                for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                    nx, ny = cx + dx, cy + dy
                    if 0 <= nx < w and 0 <= ny < hh and coat[ny][nx] and not seen[ny][nx]:
                        seen[ny][nx] = True
                        q.append((nx, ny))
            sizes.append(n)
    major_sizes = [s for s in sizes if s >= MIN_COAT_MAJOR_COMPONENT_PX]
    comps = len(major_sizes)
    # flood exterior (non-coat reachable from bbox border)
    outside = [[False] * w for _ in range(hh)]
    edge_q: deque[tuple[int, int]] = deque()
    for xi in range(w):
        for yi in (0, hh - 1):
            if not coat[yi][xi]:
                outside[yi][xi] = True
                edge_q.append((xi, yi))
    for yi in range(hh):
        for xi in (0, w - 1):
            if not coat[yi][xi] and not outside[yi][xi]:
                outside[yi][xi] = True
                edge_q.append((xi, yi))
    while edge_q:
        cx, cy = edge_q.popleft()
        for dx, dy in ((1, 0), (-1, 0), (0, 1), (0, -1)):
            nx, ny = cx + dx, cy + dy
            if 0 <= nx < w and 0 <= ny < hh and not coat[ny][nx] and not outside[ny][nx]:
                outside[ny][nx] = True
                edge_q.append((nx, ny))
    enclosed_dark = 0
    for yi in range(hh):
        for xi in range(w):
            if coat[yi][xi] or outside[yi][xi]:
                continue
            r, g, b = img.getpixel((x0 + xi * step, y0 + yi * step))[:3]
            if _is_enclosed_dark(r, g, b):
                enclosed_dark += 1
    hole_frac = enclosed_dark / total_coat if total_coat else 1.0
    return comps, total_coat, hole_frac, enclosed_dark


def _researcher_coat_sleeve_flare_px(
    img: Image.Image, tile_w: int, h: int, i: int
) -> tuple[int, float]:
    """Count lab-coat white pixels outside shoulder silhouette + 15% margin."""
    x0, x1 = i * tile_w + tile_w // 12, (i + 1) * tile_w - tile_w // 12
    y_sh0, y_sh1 = h // 5, h // 3
    xs_sh: list[int] = []
    for y in range(y_sh0, y_sh1, 2):
        for x in range(x0, x1, 2):
            r, g, b = img.getpixel((x, y))[:3]
            if _is_neutral_grey(r, g, b):
                continue
            xs_sh.append(x)
    if len(xs_sh) < 36:
        return 0, 0.0
    shoulder_w = float(max(xs_sh) - min(xs_sh))
    center = (max(xs_sh) + min(xs_sh)) * 0.5
    half_allow = shoulder_w * 0.5 * (1.0 + COAT_FLARE_SHOULDER_PAD)
    y0, y1 = h // 7, int(h * 0.56)
    flare = 0
    for y in range(y0, y1, 2):
        for x in range(x0, x1, 2):
            r, g, b = img.getpixel((x, y))[:3]
            if not _is_lab_coat_white(r, g, b):
                continue
            if abs(x - center) > half_allow:
                flare += 1
    return flare, shoulder_w


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
            head_blue = _researcher_head_face_blue_px(img, tile_w, h, i)
            if head_blue > MAX_HEAD_FACE_BLUE_PX:
                return False, f"tile{i}_head_blue_px={head_blue}"
            hand_blue = _researcher_hand_blue_px(img, tile_w, h, i)
            if hand_blue > MAX_HAND_BLUE_PX:
                return False, f"tile{i}_hand_blue_px={hand_blue}"
            sleeve_blue = _researcher_sleeve_blue_px(img, tile_w, h, i)
            if sleeve_blue > MAX_SLEEVE_BLUE_PX:
                return False, f"tile{i}_sleeve_blue_px={sleeve_blue}"
            coat_comps, coat_px, hole_frac, enclosed_dark = _researcher_coat_shell_gate(
                img, tile_w, h, i
            )
            if coat_px < MIN_COAT_BODY_PX:
                return False, f"tile{i}_coat_sparse px={coat_px}"
            if coat_comps != 1:
                return False, f"tile{i}_coat_shred components={coat_comps} want=1"
            if hole_frac >= MAX_COAT_ENCLOSED_DARK_FRAC:
                return (
                    False,
                    f"tile{i}_coat_hole frac={hole_frac:.3f} enclosed={enclosed_dark}",
                )
            flare_px, sh_w = _researcher_coat_sleeve_flare_px(img, tile_w, h, i)
            if flare_px >= MIN_COAT_FLARE_FAIL_PX:
                return (
                    False,
                    f"tile{i}_coat_sleeve_tpose flare_px={flare_px} shoulder_w={sh_w:.1f}",
                )
        sh_win, hand_win = _arm_windows(tile_w, h, i)
        sh_y = _mean_non_grey_y(img, *sh_win)
        hand_y = _mean_non_grey_y(img, *hand_win)
        if sh_y is None or hand_y is None:
            return False, f"tile{i}_arm_band_empty"
        if hand_y - sh_y < MIN_HAND_BELOW_SHOULDER_PX:
            return False, f"tile{i}_arm_tpose hand_y={hand_y:.1f} sh_y={sh_y:.1f}"
        x0, x1 = i * tile_w + tile_w // 8, (i + 1) * tile_w - tile_w // 8
        y0, y1 = h // 4, h // 2
        xs = [
            x
            for y in range(y0, y1, 2)
            for x in range(x0, x1, 2)
            if not _is_neutral_grey(*img.getpixel((x, y))[:3])
        ]
        if xs and (max(xs) - min(xs)) > tile_w * MAX_ARM_SPAN_FRAC:
            return False, f"tile{i}_arm_span_tpose span_frac={(max(xs)-min(xs))/tile_w:.2f}"
        lx0, lx1, ly0, ly1 = _leg_window(tile_w, h, i)
        foreign, leg_n = _leg_foreign_pixels(img, lx0, lx1, ly0, ly1, asset_id)
        if leg_n > 0 and foreign > 48 and foreign / leg_n > 0.22:
            return False, f"tile{i}_leg_mesh_tear foreign_px={foreign} ratio={foreign/leg_n:.2f}"
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
    torso_spread = max(tile_means) - min(tile_means)
    min_torso_spread = (
        0.12 if asset_id == "staff-researcher-idle" else MIN_TILE_SPREAD
    )
    if torso_spread < min_torso_spread:
        return False, f"tiles_identical means={tile_means}"
    if max(centroids) - min(centroids) > MAX_CAMERA_CENTROID_DRIFT_PX:
        return False, f"camera_drift centroid_x={[round(c, 1) for c in centroids]}"
    leg_means = []
    for i in range(4):
        lx0, lx1, ly0, ly1 = _leg_window(tile_w, h, i)
        lr, _, _, ln = _tile_stats_excluding_coat(img, lx0, lx1, ly0, ly1)
        if ln > 30:
            leg_means.append(lr)
    min_leg_spread = 0.72 if asset_id == "staff-researcher-idle" else 1.35
    if len(leg_means) == 4 and max(leg_means) - min(leg_means) < min_leg_spread:
        return False, f"gait_legs_static leg_r={[round(x,1) for x in leg_means]}"
    return True, (
        f"strip {w}x{h} tiles=4 torso0={tuple(round(c,1) for c in ref)} "
        f"means={[round(m, 1) for m in tile_means]}"
    )


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("asset_id", choices=("staff-founder-idle", "staff-researcher-idle"))
    parser.add_argument(
        "--png",
        type=Path,
        default=None,
        help="Optional walk-strip PNG (default: docs/art/previews/<stem>.png)",
    )
    args = parser.parse_args()
    stem = (
        "staff-founder-walk-strip"
        if args.asset_id == "staff-founder-idle"
        else "staff-researcher-walk-strip"
    )
    png = args.png if args.png is not None else PREVIEW / f"{stem}.png"
    ok, msg = check_strip(png, args.asset_id)
    if ok:
        print(f"OK {args.asset_id} walk_strip {msg}")
        return 0
    print(f"FAIL {args.asset_id} walk_strip {msg}")
    return 1


if __name__ == "__main__":
    sys.exit(main())