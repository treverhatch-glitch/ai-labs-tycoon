#!/usr/bin/env python3
"""Four yaw views (0/90/180/270° about Z) for staff-founder shoe-forward axis."""
from __future__ import annotations

import os
import sys
from pathlib import Path

import bpy
from PIL import Image, ImageDraw

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
from kenney_face_axis import toe_forward  # noqa: E402
from kenney_founder_stock_recolor import STOCK_BLEND, apply_founder_materials  # noqa: E402

OUT = bc.PREVIEW_ROOT / "staff-founder-axis-sheet.png"
YAWS = (0, 90, 180, 270)


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
    tmp_dir = OUT.parent / "_axis_tmp"
    tmp_dir.mkdir(parents=True, exist_ok=True)
    for yaw in YAWS:
        os.environ["STAFF_PREVIEW_YAW_DEG"] = str(yaw)
        tile = tmp_dir / f"axis_{yaw}.png"
        bc.render_preview([ob], tile, asset_id="staff-founder-idle")
        tiles.append(Image.open(tile).convert("RGB"))
    tw, th = tiles[0].size
    sheet = Image.new("RGB", (tw * 2, th * 2), (150, 150, 150))
    for i, im in enumerate(tiles):
        sheet.paste(im, ((i % 2) * tw, (i // 2) * th))
    draw = ImageDraw.Draw(sheet)
    for i, yaw in enumerate(YAWS):
        x = (i % 2) * tw + 10
        y = (i // 2) * th + 10
        draw.text((x, y), f"{yaw}° Z", fill=(250, 250, 250))
    draw.text((10, th * 2 - 22), "front = mean(toe)-mean(heel) XY h<0.07; glTF -Y", fill=(230, 230, 230))
    sheet.save(OUT)
    print(f"OK axis_sheet wrote {OUT.relative_to(bc.REPO_ROOT)}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()