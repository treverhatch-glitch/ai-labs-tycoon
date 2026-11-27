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
from kenney_founder_stock_recolor import STOCK_BLEND, apply_founder_materials  # noqa: E402

OUT = bc.PREVIEW_ROOT / "staff-founder-axis-sheet.png"
YAWS = (0, 90, 180, 270)
LABELS = ("0° front", "90° Z", "180° Z", "270° Z")


def _eye_blobs(mask: Image.Image, img: Image.Image) -> int:
    mask = mask.convert("L")
    w, h = mask.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return 0
    miny, maxy = min(ys), max(ys)
    span = max(1, maxy - miny)
    y0, y1 = miny + int(span * 0.05), miny + int(span * 0.32)
    from collections import deque

    seen = [[False] * w for _ in range(max(1, y1 - y0))]
    blobs = 0

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
            q = deque([(x, y)])
            seen[yi][x] = True
            n = 0
            while q:
                cx, cy = q.popleft()
                n += 1
                for dx, dy in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                    nx, ny = cx + dx, cy + dy
                    if ny < y0 or ny >= y1 or nx < 0 or nx >= w:
                        continue
                    nyi = ny - y0
                    if seen[nyi][nx] or not dark(nx, ny):
                        continue
                    seen[nyi][nx] = True
                    q.append((nx, ny))
            if n >= 4:
                blobs += 1
    return blobs


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


def _is_founder_rgb(im: Image.Image) -> bool:
    w, h = im.size
    red = blue = skin = 0
    n = 0
    for y in range(0, h, 4):
        for x in range(0, w, 4):
            r, g, b = im.getpixel((x, y))
            if r + g + b < 60:
                continue
            n += 1
            if r > g + 12 and r > b + 8:
                red += 1
            if b > r + 6 and b > g + 4:
                blue += 1
            if r > 148 and g > 110 and b < 175:
                skin += 1
    if n < 50:
        return False
    return (red + blue + skin) / n > 0.35


def main() -> None:
    if not STOCK_BLEND.is_file():
        print("FAIL axis_sheet missing stock blend")
        raise SystemExit(1)
    bpy.ops.wm.open_mainfile(filepath=str(STOCK_BLEND))
    ob = next((o for o in bpy.context.scene.objects if o.type == "MESH"), None)
    if not ob:
        print("FAIL axis_sheet no_mesh")
        raise SystemExit(1)
    apply_founder_materials(ob)
    fwd = toe_forward([ob])
    print(f"OK axis_sheet front=({fwd.x:.3f},{fwd.y:.3f}) gltf_ref=-Y")

    os.environ["EARLY_ART_PREVIEW_RIG"] = "1"
    os.environ["STAFF_PREVIEW_ASSET"] = "staff-founder-idle"
    os.environ["STAFF_PREVIEW_ELEV_DEG"] = "8"
    os.environ["STAFF_PREVIEW_MARGIN"] = "1.32"
    tiles: list[Image.Image] = []
    ious: dict[int, float] = {}
    eyes: dict[int, int] = {}

    with tempfile.TemporaryDirectory(prefix="founder_axis_") as tdir:
        tmp = Path(tdir)
        for yaw in YAWS:
            os.environ["STAFF_PREVIEW_YAW_DEG"] = str(yaw)
            tile = tmp / f"axis_{yaw}.png"
            bc.render_preview([ob], tile, asset_id="staff-founder-idle")
            rgb = Image.open(tile).convert("RGB")
            if not _is_founder_rgb(rgb):
                print(f"FAIL axis_sheet yaw={yaw} not_recolored")
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