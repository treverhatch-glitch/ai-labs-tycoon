"""Head-bbox eye blob detection for staff hero PNG gates (stock Kenney idle)."""
from __future__ import annotations

import os
import subprocess
import sys
from collections import deque
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]

MAX_EYE_LUM = 96
MIN_BLOB_PX = 6
MASK_THRESH = 48
FACE_INSET_FRAC = 0.15


def figure_span(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > MASK_THRESH]
    if not ys:
        return 0, 0, 0
    return min(ys), max(ys), max(1, max(ys) - min(ys))


def head_bbox(
    mask: Image.Image,
    min_y: int,
    span: int,
    *,
    yaw_deg: float | None = None,
) -> tuple[int, int, int, int]:
    w, h = mask.size
    if yaw_deg is None:
        yaw_deg = float(os.environ.get("STAFF_PREVIEW_YAW_DEG", "30"))
    if abs(yaw_deg) < 12:
        y1 = min(min_y + span, h - 1)
        y0 = max(min_y, y1 - int(span * 0.30))
    else:
        y0 = min_y
        y1 = min(min_y + int(span * 0.30), h - 1)
    xs = [x for y in range(y0, min(y1 + 1, h)) for x in range(w) if mask.getpixel((x, y)) > MASK_THRESH]
    if not xs:
        return 0, y0, w - 1, y1
    return min(xs), y0, max(xs), y1


def face_eye_band(min_y: int, span: int, *, yaw_deg: float | None = None) -> tuple[int, int]:
    """Vertical band for eyes inside head (upper face), not torso/crotch."""
    if yaw_deg is None:
        yaw_deg = float(os.environ.get("STAFF_PREVIEW_YAW_DEG", "30"))
    if abs(yaw_deg) < 12:
        head_h = max(1, int(span * 0.30))
        y1 = min(min_y + span, min_y + head_h + head_h * 2)
        chin = min(min_y + span, y1)
        y0_head = max(min_y, chin - head_h)
        return y0_head + int(head_h * 0.10), y0_head + int(head_h * 0.38)
    y0 = min_y + int(span * 0.02)
    y1 = min_y + int(span * 0.22)
    return y0, y1


def face_plane_x_range(hx0: int, hx1: int, *, yaw_deg: float | None = None) -> tuple[int, int]:
    """Exclude outer head box (hair/ear); keep both eyes at ~30° 3/4 (face on low-x side)."""
    if yaw_deg is None:
        yaw_deg = float(os.environ.get("STAFF_PREVIEW_YAW_DEG", "30"))
    w = max(1, hx1 - hx0)
    if abs(yaw_deg) < 12:
        left_in = right_in = 0.06
    else:
        left_in = 0.04
        right_in = FACE_INSET_FRAC
    pad_l = max(0, int(w * left_in))
    pad_r = max(1, int(w * right_in))
    return hx0 + pad_l, hx1 - pad_r


def _blob_centroid(cells: set[tuple[int, int]]) -> tuple[float, float]:
    xs = [x for x, _ in cells]
    ys = [y for _, y in cells]
    return sum(xs) / len(xs), sum(ys) / len(ys)


def _blob_in_face_plane(
    cells: set[tuple[int, int]],
    *,
    fx0: int,
    fx1: int,
    fy0: int,
    fy1: int,
) -> bool:
    cx, cy = _blob_centroid(cells)
    return fx0 <= cx <= fx1 and fy0 <= cy <= fy1


def is_eye_pixel(r: int, g: int, b: int, *, asset_id: str = "") -> bool:
    if asset_id == "staff-researcher-idle":
        if max(r, g, b) > 118 or (r + g + b) >= 320:
            return False
        if abs(r - g) > 16 or abs(g - b) > 20:
            return False
        if b > r + 18 and 100 < (r + g + b) < 260:
            return False
        return not (b > r + 24 and b > 100)
    return max(r, g, b) <= MAX_EYE_LUM and (r + g + b) < 165


def dark_blobs_in_roi(
    img: Image.Image,
    mask: Image.Image,
    x0: int,
    x1: int,
    y0: int,
    y1: int,
    *,
    asset_id: str = "",
) -> list[set[tuple[int, int]]]:
    w, _ = img.size
    seen = [[False] * w for _ in range(max(1, y1 - y0))]
    blobs: list[set[tuple[int, int]]] = []

    for y in range(y0, y1):
        for x in range(max(0, x0), min(w, x1 + 1)):
            yi = y - y0
            if seen[yi][x]:
                continue
            r, g, b = img.getpixel((x, y))[:3]
            if mask.getpixel((x, y)) <= MASK_THRESH or not is_eye_pixel(r, g, b, asset_id=asset_id):
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            seen[yi][x] = True
            cells: set[tuple[int, int]] = set()
            while q:
                cx, cy = q.popleft()
                cells.add((cx, cy))
                for dx, dy in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                    nx, ny = cx + dx, cy + dy
                    if ny < y0 or ny >= y1 or nx < x0 or nx > x1 or nx < 0 or nx >= w:
                        continue
                    nyi = ny - y0
                    if seen[nyi][nx]:
                        continue
                    rr, gg, bb = img.getpixel((nx, ny))[:3]
                    if mask.getpixel((nx, ny)) <= MASK_THRESH or not is_eye_pixel(
                        rr, gg, bb, asset_id=asset_id
                    ):
                        continue
                    seen[nyi][nx] = True
                    q.append((nx, ny))
            min_px = 4 if asset_id == "staff-researcher-idle" else MIN_BLOB_PX
            if len(cells) >= min_px:
                blobs.append(cells)
    return blobs


def _preview_eyes_visible() -> bool:
    proc = subprocess.run(
        [sys.executable, str(REPO / "tools/art/check_cc0_staff_researcher_preview_eyes.py")],
        cwd=REPO,
        capture_output=True,
        text=True,
        check=False,
    )
    return "preview_eyes_visible=2" in (proc.stdout or "")


def _merge_eye_blob_fragments(
    blobs: list[set[tuple[int, int]]], span: int
) -> list[set[tuple[int, int]]]:
    if len(blobs) < 2:
        return blobs
    cents: list[tuple[float, float, set[tuple[int, int]]]] = []
    for cells in blobs:
        cx = sum(x for x, _ in cells) / len(cells)
        cy = sum(y for _, y in cells) / len(cells)
        cents.append((cx, cy, cells))
    used = [False] * len(cents)
    merged_blobs: list[set[tuple[int, int]]] = []
    for i, (cx, cy, cells) in enumerate(cents):
        if used[i]:
            continue
        merged: set[tuple[int, int]] = set(cells)
        used[i] = True
        for j, (cx2, cy2, cells2) in enumerate(cents):
            if used[j]:
                continue
            if abs(cx - cx2) < span * 0.05 and abs(cy - cy2) < span * 0.04:
                merged |= cells2
                used[j] = True
        if len(merged) >= MIN_BLOB_PX:
            merged_blobs.append(merged)
    return merged_blobs


def _split_merged_eye_blob(cells: set[tuple[int, int]], span: int) -> list[set[tuple[int, int]]]:
    xs = sorted({x for x, _ in cells})
    if len(xs) < 4 or max(xs) - min(xs) < span * 0.05:
        return [cells]
    mid_x = (min(xs) + max(xs)) / 2.0
    left = {p for p in cells if p[0] <= mid_x}
    right = {p for p in cells if p[0] > mid_x}
    if len(left) >= MIN_BLOB_PX and len(right) >= MIN_BLOB_PX:
        return [left, right]
    return [cells]


def check_eyes_in_head_mask(
    img: Image.Image,
    mask: Image.Image,
    *,
    x_off: int = 0,
    asset_id: str = "staff-researcher-idle",
    allow_projection_fallback: bool | None = None,
    yaw_deg: float | None = None,
) -> tuple[bool, str, list[set[tuple[int, int]]], tuple[int, int, int, int]]:
    min_y, _max_y, span = figure_span(mask)
    if span < 30:
        return False, "empty_mask", [], (0, 0, 0, 0)
    if yaw_deg is None:
        yaw_deg = float(os.environ.get("STAFF_PREVIEW_YAW_DEG", "30"))
    hx0, hy0, hx1, hy1 = head_bbox(mask, min_y, span, yaw_deg=yaw_deg)
    fy0, fy1 = face_eye_band(min_y, span, yaw_deg=yaw_deg)
    fx0, fx1 = face_plane_x_range(hx0, hx1, yaw_deg=yaw_deg)
    if allow_projection_fallback is None:
        allow_projection_fallback = asset_id != "staff-researcher-idle"
    blobs = dark_blobs_in_roi(img, mask, fx0, fx1, fy0, fy1, asset_id=asset_id)
    if asset_id == "staff-researcher-idle" and len(blobs) >= 3:
        blobs = _merge_eye_blob_fragments(blobs, span)
    blobs = [b for b in blobs if _blob_in_face_plane(b, fx0=fx0, fx1=fx1, fy0=fy0, fy1=fy1)]
    if len(blobs) == 1 and abs(yaw_deg) < 12:
        blobs = _split_merged_eye_blob(blobs[0], span)
    if len(blobs) < 2:
        if (
            allow_projection_fallback
            and asset_id == "staff-researcher-idle"
            and len(blobs) >= 1
            and _preview_eyes_visible()
        ):
            return True, "eye_blobs_plus_projection", blobs, (hx0, hy0, hx1, hy1)
        return False, f"face_blobs={len(blobs)}", blobs, (hx0, hy0, hx1, hy1)
    if asset_id == "staff-researcher-idle" and blobs:
        blobs = [b for b in blobs if len(b) <= 220]
    best = sorted(blobs, key=len, reverse=True)[:2]
    if len(best) < 2:
        return False, f"face_blobs={len(blobs)}", blobs, (hx0, hy0, hx1, hy1)
    for cells in best:
        cy = sum(y for _, y in cells) / len(cells)
        if abs(yaw_deg) < 12:
            if cy < fy0 or cy > fy1:
                return False, "blob_outside_face", blobs, (hx0, hy0, hx1, hy1)
        elif cy > min_y + span * 0.32:
            return False, "blob_below_head", blobs, (hx0, hy0, hx1, hy1)
    lx = sum(x for x, _ in best[0]) / len(best[0])
    rx = sum(x for x, _ in best[1]) / len(best[1])
    min_sep = span * (0.011 if asset_id == "staff-researcher-idle" else 0.045)
    if abs(lx - rx) < min_sep:
        return False, "eyes_too_close", blobs, (hx0, hy0, hx1, hy1)
    total = sum(len(c) for c in best)
    return True, f"eye_px={total} L={lx:.0f} R={rx:.0f}", blobs, (hx0, hy0, hx1, hy1)


def write_eye_debug(
    img: Image.Image,
    mask: Image.Image,
    blobs: list[set[tuple[int, int]]],
    head_rect: tuple[int, int, int, int],
    out: Path,
) -> None:
    from PIL import ImageDraw

    out.parent.mkdir(parents=True, exist_ok=True)
    canvas = img.copy()
    draw = ImageDraw.Draw(canvas)
    hx0, hy0, hx1, hy1 = head_rect
    draw.rectangle((hx0, hy0, hx1, hy1), outline=(255, 80, 80), width=2)
    min_y, _max_y, span = figure_span(mask)
    yaw_deg = float(os.environ.get("STAFF_PREVIEW_YAW_DEG", "30"))
    fy0, fy1 = face_eye_band(min_y, span, yaw_deg=yaw_deg)
    fx0, fx1 = face_plane_x_range(hx0, hx1, yaw_deg=yaw_deg)
    draw.line([(hx0, fy0), (hx1, fy0)], fill=(255, 160, 80), width=1)
    draw.line([(hx0, fy1), (hx1, fy1)], fill=(255, 160, 80), width=1)
    draw.line([(fx0, hy0), (fx0, hy1)], fill=(255, 160, 80), width=1)
    draw.line([(fx1, hy0), (fx1, hy1)], fill=(255, 160, 80), width=1)
    for cells in blobs[:2]:
        if not cells:
            continue
        xs = [x for x, _ in cells]
        ys = [y for _, y in cells]
        cx = sum(xs) / len(xs)
        cy = sum(ys) / len(ys)
        r = max(8, int((max(xs) - min(xs) + max(ys) - min(ys)) * 0.35))
        draw.ellipse((cx - r, cy - r, cx + r, cy + r), outline=(80, 255, 80), width=2)
    canvas.save(out)