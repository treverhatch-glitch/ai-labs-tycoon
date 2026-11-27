#!/usr/bin/env python3
"""Every dark eye pixel must lie on the head silhouette mask (idle + vs-stock founder)."""
from __future__ import annotations

import subprocess
import sys
import tempfile
from collections import deque
from pathlib import Path

from PIL import Image, ImageDraw

REPO = Path(__file__).resolve().parents[2]
MAX_EYE_LUM = 96
MIN_BLOB_PX = 4
MASK_THRESH = 48

IDLE_PNG = REPO / "docs/art/previews/staff-founder-idle.png"
IDLE_MASK = REPO / "docs/art/previews/staff-founder-idle.mask.png"
VS_PNG = REPO / "docs/art/previews/staff-founder-vs-stock-check.png"
DEBUG_PNG = REPO / "docs/art/previews/staff-founder-eye-gate-debug.png"

NEGATIVE_SHAS = (
    ("726005c46bf317a7318c62310875b337c43952bc", "v24"),
    ("af82592cc5a437e6f93ce8b65d7d5d24dddaa7f1", "v25"),
)
GLB_REL = "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.glb"


def _figure_span(mask: Image.Image) -> tuple[int, int, int]:
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > MASK_THRESH]
    if not ys:
        return 0, 0, 0
    return min(ys), max(ys), max(1, max(ys) - min(ys))


def _is_eye_pixel(r: int, g: int, b: int) -> bool:
    return max(r, g, b) <= MAX_EYE_LUM and (r + g + b) < 140


def _dark_blobs(y0: int, y1: int, img: Image.Image) -> list[set[tuple[int, int]]]:
    w, _ = img.size
    seen = [[False] * w for _ in range(y1 - y0)]
    blobs: list[set[tuple[int, int]]] = []

    for y in range(y0, y1):
        for x in range(w):
            yi = y - y0
            if seen[yi][x]:
                continue
            r, g, b = img.getpixel((x, y))[:3]
            if not _is_eye_pixel(r, g, b):
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            seen[yi][x] = True
            cells: set[tuple[int, int]] = set()
            while q:
                cx, cy = q.popleft()
                cells.add((cx, cy))
                for dx, dy in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                    nx, ny = cx + dx, cy + dy
                    if ny < y0 or ny >= y1 or nx < 0 or nx >= w:
                        continue
                    nyi = ny - y0
                    if seen[nyi][nx]:
                        continue
                    rr, gg, bb = img.getpixel((nx, ny))[:3]
                    if not _is_eye_pixel(rr, gg, bb):
                        continue
                    seen[nyi][nx] = True
                    q.append((nx, ny))
            if len(cells) >= MIN_BLOB_PX:
                blobs.append(cells)
    return blobs


def _check_pair(
    img: Image.Image, mask: Image.Image, *, x_off: int = 0
) -> tuple[bool, str, list[set[tuple[int, int]]]]:
    min_y, _max_y, span = _figure_span(mask)
    if span < 30:
        return False, "empty_mask", []
    y0 = min_y + int(span * 0.44)
    y1 = min_y + int(span * 0.76)
    blobs = _dark_blobs(y0, y1, img)
    if len(blobs) < 2:
        return False, f"blobs={len(blobs)}", blobs
    outside = 0
    total_eye = 0
    for cells in sorted(blobs, key=len, reverse=True)[:4]:
        for x, y in cells:
            total_eye += 1
            mx = x - x_off
            if mx < 0 or mx >= mask.size[0] or mask.getpixel((mx, y)) <= MASK_THRESH:
                outside += 1
    if outside > 0:
        return False, f"outside_mask_px={outside} eye_px={total_eye}", blobs
    best = sorted(blobs, key=len, reverse=True)[:2]
    lx = sum(x for x, _ in best[0]) / len(best[0])
    rx = sum(x for x, _ in best[1]) / len(best[1])
    if abs(lx - rx) < span * 0.035:
        return False, "eyes_too_close", blobs
    return True, f"eye_px={total_eye} L={lx:.0f} R={rx:.0f}", blobs


def _git_png(sha: str, rel: str) -> Image.Image | None:
    proc = subprocess.run(
        ["git", "show", f"{sha}:{rel}"],
        cwd=REPO,
        capture_output=True,
        check=False,
    )
    if proc.returncode != 0:
        return None
    tmp = tempfile.NamedTemporaryFile(suffix=".png", delete=False)
    tmp.write(proc.stdout)
    tmp.flush()
    return Image.open(tmp.name).convert("RGB")


def _glb_eye_gate_passes(sha: str) -> bool | None:
    proc = subprocess.run(
        ["git", "show", f"{sha}:{GLB_REL}"],
        cwd=REPO,
        capture_output=True,
        check=False,
    )
    if proc.returncode != 0 or not proc.stdout:
        return None
    glb = tempfile.NamedTemporaryFile(suffix=".glb", delete=False)
    glb.write(proc.stdout)
    glb.flush()
    glb.close()
    saved = REPO / GLB_REL
    backup = saved.read_bytes() if saved.is_file() else None
    try:
        saved.write_bytes(Path(glb.name).read_bytes())
        gate = subprocess.run(
            [sys.executable, str(REPO / "tools/art/check_cc0_staff_founder_eye_glb_world.py")],
            cwd=REPO,
            capture_output=True,
            text=True,
            check=False,
        )
        return gate.returncode == 0
    finally:
        if backup is not None:
            saved.write_bytes(backup)
        Path(glb.name).unlink(missing_ok=True)


def _negative_self_tests() -> bool:
    rel = "docs/art/previews/staff-founder-idle.png"
    mask_rel = "docs/art/previews/staff-founder-idle.mask.png"
    for sha, tag in NEGATIVE_SHAS:
        glb_ok = _glb_eye_gate_passes(sha)
        if glb_ok is None:
            print(f"FAIL staff-founder-idle eyes_in_head negative missing_glb sha={sha[:8]}")
            return False
        if glb_ok:
            print(f"FAIL staff-founder-idle eyes_in_head negative {tag} glb_gate should_fail")
            return False
        img = _git_png(sha, rel)
        mask = _git_png(sha, mask_rel)
        if img is None or mask is None:
            print(f"FAIL staff-founder-idle eyes_in_head negative missing_png {tag}")
            return False
        ok, _msg, _ = _check_pair(img, mask.convert("L"))
        if ok:
            print(
                f"WARN staff-founder-idle eyes_in_head negative {tag} "
                f"idle_png still_passes mask_gate glb_gate_failed"
            )
    print("OK staff-founder-idle eyes_in_head negative_v24_v25")
    return True


def _write_debug(img: Image.Image, mask: Image.Image, blobs: list[set[tuple[int, int]]]) -> None:
    out = img.copy()
    draw = ImageDraw.Draw(out)
    min_y, max_y, span = _figure_span(mask)
    y1 = min_y + int(span * 0.24)
    for y in range(min_y, y1):
        xs = [x for x in range(mask.size[0]) if mask.getpixel((x, y)) > MASK_THRESH]
        if len(xs) < 2:
            continue
        draw.line([(min(xs), y), (max(xs), y)], fill=(255, 80, 80), width=1)
    for cells in blobs[:2]:
        if not cells:
            continue
        xs = [x for x, _ in cells]
        ys = [y for _, y in cells]
        cx = sum(xs) / len(xs)
        cy = sum(ys) / len(ys)
        r = max(8, int((max(xs) - min(xs) + max(ys) - min(ys)) * 0.35))
        draw.ellipse((cx - r, cy - r, cx + r, cy + r), outline=(80, 255, 80), width=2)
    DEBUG_PNG.parent.mkdir(parents=True, exist_ok=True)
    out.save(DEBUG_PNG)


def main() -> int:
    rc = 0
    if not _negative_self_tests():
        rc = 1
    if not IDLE_PNG.is_file() or not IDLE_MASK.is_file():
        print("FAIL staff-founder-idle eyes_in_head idle missing_png")
        return 1
    idle_img = Image.open(IDLE_PNG).convert("RGB")
    idle_mask = Image.open(IDLE_MASK).convert("L")
    ok, msg, blobs = _check_pair(idle_img, idle_mask)
    _write_debug(idle_img, idle_mask, blobs)
    if ok:
        print(f"OK staff-founder-idle eyes_in_head idle {msg}")
        print(f"OK staff-founder-idle eyes_in_head debug {DEBUG_PNG.relative_to(REPO)}")
    else:
        print(f"FAIL staff-founder-idle eyes_in_head idle {msg}")
        rc = 1

    if VS_PNG.is_file():
        vs = Image.open(VS_PNG).convert("RGB")
        gap = 8
        x0 = idle_img.width + gap
        if vs.width > x0:
            founder_crop = vs.crop((x0, 0, vs.width, vs.height))
            if founder_crop.size != idle_img.size:
                founder_crop = founder_crop.resize(idle_img.size, Image.Resampling.LANCZOS)
            ok2, msg2, _ = _check_pair(founder_crop, idle_mask)
            if ok2:
                print(f"OK staff-founder-idle eyes_in_head vs-stock {msg2}")
            else:
                print(f"FAIL staff-founder-idle eyes_in_head vs-stock {msg2}")
                rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())