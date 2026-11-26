#!/usr/bin/env python3
"""Hatch composites: staff lineup (8) + wisp mood row — mask-aware fill on mid-grey."""
from __future__ import annotations

import json
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
OUT = REPO / "docs/art/previews"
STAFF_MANIFEST = REPO / "unity/Assets/Art/EarlyGame/staff-manifest.json"
WISP_IDS = ["wisp-happy", "wisp-neutral", "wisp-worried", "wisp-angry", "wisp-glowing"]
BG = (122, 122, 128)
FLOOR = (102, 102, 108)


def _mask_bbox(mask_path: Path) -> tuple[int, int, int, int] | None:
    from PIL import Image

    im = Image.open(mask_path).convert("RGBA")
    w, h = im.size
    px = list(im.getdata())
    xs, ys = [], []
    for y in range(h):
        for x in range(w):
            if px[y * w + x][3] >= 20:
                xs.append(x)
                ys.append(y)
    if not xs:
        return None
    return min(xs), min(ys), max(xs), max(ys)


def _tile_filled(asset_id: str, cell: int, fill_frac: float) -> "Image.Image":
    from PIL import Image

    rgb = Image.open(PREVIEW / f"{asset_id}.png").convert("RGB")
    mask = PREVIEW / f"{asset_id}.mask.png"
    box = _mask_bbox(mask) if mask.is_file() else None
    if box:
        crop = rgb.crop(box)
    else:
        crop = rgb
    target = int(cell * fill_frac)
    cw, ch = crop.size
    scale = min(target / max(1, cw), target / max(1, ch))
    nw, nh = max(1, int(cw * scale)), max(1, int(ch * scale))
    tile = crop.resize((nw, nh), Image.Resampling.LANCZOS)
    canvas = Image.new("RGB", (cell, cell), BG)
    px = (cell - nw) // 2
    py = int(cell * 0.08 + (cell * 0.84 - nh) * 0.35)
    canvas.paste(tile, (px, py))
    if asset_id.startswith("wisp-"):
        floor_h = max(6, cell // 14)
        for x in range(cell):
            for y in range(cell - floor_h, cell):
                canvas.putpixel((x, y), FLOOR)
    return canvas


def _row(ids: list[str], out_name: str, cell: int = 512, fill: float = 0.68) -> None:
    from PIL import Image

    tiles = [_tile_filled(aid, cell, fill) for aid in ids]
    w, h = cell * len(tiles), cell
    canvas = Image.new("RGB", (w, h), BG)
    for i, t in enumerate(tiles):
        canvas.paste(t, (i * cell, 0))
    path = OUT / out_name
    canvas.save(path)
    print("OK composite", out_name, path)


def main() -> None:
    staff = [a["id"] for a in json.loads(STAFF_MANIFEST.read_text())["assets"]]
    _row(staff, "staff-lineup-packet2.png", cell=384, fill=0.72)
    _row(WISP_IDS, "wisp-moods-row.png", cell=320, fill=0.62)


if __name__ == "__main__":
    main()