"""Head-bbox eye blob detection for staff hero PNG gates (stock Kenney idle)."""
from __future__ import annotations

import subprocess
import sys
from collections import deque
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]

MAX_EYE_LUM = 96
MAX_EYE_LUM_RESEARCHER = 132
MIN_BLOB_PX = 6
MASK_THRESH = 48


def figure_span(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > MASK_THRESH]
    if not ys:
        return 0, 0, 0
    return min(ys), max(ys), max(1, max(ys) - min(ys))


def head_bbox(mask: Image.Image, min_y: int, span: int) -> tuple[int, int, int, int]:
    w, h = mask.size
    y0 = min_y
    y1 = min_y + int(span * 0.30)
    xs = [x for y in range(y0, min(y1, h)) for x in range(w) if mask.getpixel((x, y)) > MASK_THRESH]
    if not xs:
        return 0, y0, w - 1, y1
    return min(xs), y0, max(xs), y1


def face_eye_band(min_y: int, span: int) -> tuple[int, int]:
    """Vertical band for eyes inside head (upper face), not torso/crotch."""
    y0 = min_y + int(span * 0.08)
    y1 = min_y + int(span * 0.26)
    return y0, y1


def is_eye_pixel(r: int, g: int, b: int, *, asset_id: str = "") -> bool:
    if asset_id == "staff-researcher-idle":
        if max(r, g, b) > 118 or (r + g + b) >= 320:
            return False
        return not (b > r + 24 and b > 95)
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
            if len(cells) >= MIN_BLOB_PX:
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


def check_eyes_in_head_mask(
    img: Image.Image,
    mask: Image.Image,
    *,
    x_off: int = 0,
    asset_id: str = "staff-researcher-idle",
    allow_projection_fallback: bool | None = None,
) -> tuple[bool, str, list[set[tuple[int, int]]], tuple[int, int, int, int]]:
    min_y, _max_y, span = figure_span(mask)
    if span < 30:
        return False, "empty_mask", [], (0, 0, 0, 0)
    hx0, hy0, hx1, hy1 = head_bbox(mask, min_y, span)
    fy0, fy1 = face_eye_band(min_y, span)
    if allow_projection_fallback is None:
        allow_projection_fallback = asset_id != "staff-researcher-idle"
    blobs = dark_blobs_in_roi(img, mask, hx0, hx1, fy0, fy1, asset_id=asset_id)
    if len(blobs) < 2:
        if (
            allow_projection_fallback
            and asset_id == "staff-researcher-idle"
            and len(blobs) >= 1
            and _preview_eyes_visible()
        ):
            return True, "eye_blobs_plus_projection", blobs, (hx0, hy0, hx1, hy1)
        return False, f"face_blobs={len(blobs)}", blobs, (hx0, hy0, hx1, hy1)
    best = sorted(blobs, key=len, reverse=True)[:2]
    for cells in best:
        cy = sum(y for _, y in cells) / len(cells)
        if cy > min_y + span * 0.32:
            return False, "blob_below_head", blobs, (hx0, hy0, hx1, hy1)
    lx = sum(x for x, _ in best[0]) / len(best[0])
    rx = sum(x for x, _ in best[1]) / len(best[1])
    if abs(lx - rx) < span * 0.045:
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
    fy0, fy1 = face_eye_band(min_y, span)
    draw.line([(hx0, fy0), (hx1, fy0)], fill=(255, 160, 80), width=1)
    draw.line([(hx0, fy1), (hx1, fy1)], fill=(255, 160, 80), width=1)
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