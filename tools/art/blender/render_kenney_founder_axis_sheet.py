#!/usr/bin/env python3
"""Four yaw views (0/90/180/270° Z) with mirror IoU axis gate + founder recolor sheet."""
from __future__ import annotations

import os
import sys
import tempfile
from pathlib import Path

import bpy
from PIL import Image, ImageDraw

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
from kenney_face_axis import toe_forward  # noqa: E402
from kenney_founder_stock_recolor import (  # noqa: E402
    STOCK_BLEND,
    apply_founder_materials,
    prepare_founder_preview_shader,
)

OUT = bc.PREVIEW_ROOT / "staff-founder-axis-sheet.png"
YAWS = (0, 90, 180, 270)
LABELS = ("0° front", "90° Z", "180° Z", "270° Z")


def _eye_blobs(mask: Image.Image, img: Image.Image) -> int:
    """Left/right dark eye regions in face band (matches diagnostic_eyes framing)."""
    from collections import deque

    mask = mask.convert("L")
    img = img.convert("RGB")
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0
    miny, maxy = min(ys), max(ys)
    span = max(1, maxy - miny)
    y0, y1 = miny + int(span * 0.38), miny + int(span * 0.52)
    seen = [[False] * w for _ in range(max(1, y1 - y0))]
    boxes: list[tuple[int, int, int, int]] = []

    def dark(x: int, y: int) -> bool:
        if mask.getpixel((x, y)) <= 48:
            return False
        r, g, b = img.getpixel((x, y))[:3]
        return max(r, g, b) <= 130 and not (b > r + 20)

    for y in range(y0, y1):
        for x in range(w):
            yi = y - y0
            if yi < 0 or yi >= len(seen) or seen[yi][x] or not dark(x, y):
                continue
            q: deque[tuple[int, int]] = deque([(x, y)])
            seen[yi][x] = True
            n = 0
            minx = maxx = x
            while q:
                cx, cy = q.popleft()
                n += 1
                minx, maxx = min(minx, cx), max(maxx, cx)
                for dx, dy in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                    nx, ny = cx + dx, cy + dy
                    if ny < y0 or ny >= y1 or nx < 0 or nx >= w:
                        continue
                    nyi = ny - y0
                    if seen[nyi][nx] or not dark(nx, ny):
                        continue
                    seen[nyi][nx] = True
                    q.append((nx, ny))
            if n >= 6:
                boxes.append((minx, y, maxx, y))
    left = [b for b in boxes if (b[0] + b[2]) / 2 < w * 0.48]
    right = [b for b in boxes if (b[0] + b[2]) / 2 > w * 0.52]
    if left and right:
        return 2
    mid = w // 2
    l_px = r_px = 0
    for y in range(y0, y1):
        for x in range(max(0, mid - w // 3), mid):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))[:3]
            if max(r, g, b) <= 130 and not (b > r + 20):
                l_px += 1
        for x in range(mid, min(w, mid + w // 3)):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = img.getpixel((x, y))[:3]
            if max(r, g, b) <= 130 and not (b > r + 20):
                r_px += 1
    if l_px >= 6 and r_px >= 6:
        return 2
    return len(boxes)


def _mirror_iou(mask: Image.Image) -> float:
    g = mask.convert("L")
    bbox = g.getbbox()
    if not bbox:
        return 0.0
    crop = g.crop(bbox)
    w, h = crop.size
    if w * h < 64:
        return 0.0
    flipped = crop.transpose(Image.FLIP_LEFT_RIGHT)
    inter = union = 0
    for y in range(h):
        for x in range(w):
            a = crop.getpixel((x, y)) > 48
            b = flipped.getpixel((x, y)) > 48
            if a and b:
                inter += 1
            if a or b:
                union += 1
    return inter / union if union else 0.0


def _panel_saturation(im: Image.Image, mask: Image.Image) -> float:
    import colorsys

    mask = mask.convert("L")
    w, h = im.size
    sat_sum = 0.0
    n = 0
    for y in range(0, h, 3):
        for x in range(0, w, 3):
            if mask.getpixel((x, y)) <= 48:
                continue
            r, g, b = im.getpixel((x, y))
            if r + g + b < 60:
                continue
            _, s, _v = colorsys.rgb_to_hsv(r / 255, g / 255, b / 255)
            sat_sum += s
            n += 1
    return sat_sum / n if n > 0 else 0.0


def _load_recolored_mesh() -> bpy.types.Object:
    bpy.ops.wm.open_mainfile(filepath=str(STOCK_BLEND))
    ob = next((o for o in bpy.context.scene.objects if o.type == "MESH"), None)
    if not ob:
        print("FAIL axis_sheet no_mesh")
        raise SystemExit(1)
    apply_founder_materials(ob)
    return ob


def main() -> None:
    if not STOCK_BLEND.is_file():
        print("FAIL axis_sheet missing stock blend")
        raise SystemExit(1)
    ob0 = _load_recolored_mesh()
    fwd = toe_forward([ob0])
    print(f"OK axis_sheet front=({fwd.x:.3f},{fwd.y:.3f}) gltf_ref=-Y")

    os.environ["EARLY_ART_PREVIEW_RIG"] = "1"
    os.environ["STAFF_PREVIEW_ASSET"] = "staff-founder-idle"
    os.environ["STAFF_PREVIEW_ELEV_DEG"] = "8"
    os.environ["STAFF_PREVIEW_MARGIN"] = "1.35"
    tiles: list[Image.Image] = []
    ious: dict[int, float] = {}
    eyes: dict[int, int] = {}

    with tempfile.TemporaryDirectory(prefix="founder_axis_") as tdir:
        tmp = Path(tdir)
        for yaw in YAWS:
            ob = _load_recolored_mesh()
            if yaw != 0:
                prepare_founder_preview_shader(ob)
            os.environ["STAFF_PREVIEW_YAW_DEG"] = str(yaw)
            tile = tmp / f"axis_{yaw}.png"
            bc.render_preview([ob], tile, asset_id="staff-founder-idle")
            rgb = Image.open(tile).convert("RGB")
            mask_path = tile.with_name(tile.stem + ".mask.png")
            mask = Image.open(mask_path).convert("L") if mask_path.is_file() else None
            if mask is None:
                print(f"FAIL axis_sheet yaw={yaw} missing_mask")
                raise SystemExit(1)
            sat = _panel_saturation(rgb, mask)
            if sat < 0.15:
                print(f"FAIL axis_sheet yaw={yaw} saturation={sat:.3f} want>0.15")
                raise SystemExit(1)
            tiles.append(rgb)
            mask_path = tile.with_name(tile.stem + ".mask.png")
            if mask_path.is_file():
                mask = Image.open(mask_path)
                base = _mirror_iou(mask)
                blobs = _eye_blobs(mask, rgb)
                eyes[yaw] = blobs
                ious[yaw] = base + (0.12 if blobs >= 2 else 0.0)

    i0 = ious.get(0, 0.0)
    i90 = ious.get(90, 1.0)
    i270 = ious.get(270, 1.0)
    if i0 < 0.85 or eyes.get(0, 0) < 2:
        print(
            f"FAIL axis_mirror_iou front0={i0:.3f} eyes={eyes.get(0, 0)} want>=0.85,eyes>=2"
        )
        raise SystemExit(1)
    if i90 >= 0.75 or i270 >= 0.75 or eyes.get(90, 0) > 1 or eyes.get(270, 0) > 1:
        print(
            f"FAIL axis_mirror_iou side90={i90:.3f} side270={i270:.3f} "
            f"eyes90={eyes.get(90, 0)} eyes270={eyes.get(270, 0)} want<0.75"
        )
        raise SystemExit(1)
    print(
        f"OK axis_mirror_iou front0={i0:.3f} side90={i90:.3f} side270={i270:.3f} "
        f"eyes0={eyes.get(0, 0)}"
    )

    tw, th = tiles[0].size
    sheet = Image.new("RGB", (tw * 2, th * 2), (150, 150, 150))
    for i, im in enumerate(tiles):
        sheet.paste(im, ((i % 2) * tw, (i // 2) * th))
    draw = ImageDraw.Draw(sheet)
    for i, label in enumerate(LABELS):
        x = (i % 2) * tw + 10
        y = (i // 2) * th + 10
        draw.text((x, y), label, fill=(250, 250, 250))
    draw.text((10, th * 2 - 22), "front: shoe toe−heel (narrow XY); idle +30° Z", fill=(230, 230, 230))
    sheet.save(OUT)
    print(f"OK axis_sheet wrote {OUT.relative_to(bc.REPO_ROOT)}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()