#!/usr/bin/env python3
"""Rasterize HUD SVG glyphs to transparent PNG previews (orthographic 2D, not 3D bpy)."""
from __future__ import annotations

import json
import shutil
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
SVG_DIR = REPO / "unity/Assets/Art/EarlyGame/hud-icons"
PREVIEW = REPO / "docs/art/previews"
STREAM = REPO / "unity/Assets/StreamingAssets/EarlyGame/hud-icons"

ICON_SIZE = 512
ICON_IDS = (
    "icon-cash-era0",
    "icon-date-era0",
    "icon-era-bar",
    "icon-build-panel",
    "icon-hire-panel",
    "icon-train-panel",
    "icon-gpu-hire",
    "icon-week-advance",
)
PANEL_IDS = ("panel-build-bg", "panel-hire-bg", "panel-train-bg")
BAR_ID = "hud-topbar-beige"
ALL_IDS = ICON_IDS + PANEL_IDS + (BAR_ID,)


def _rasterize(svg: Path, out_png: Path, width: int, height: int | None = None) -> None:
    import cairosvg

    h = height or width
    cairosvg.svg2png(url=str(svg), write_to=str(out_png), output_width=width, output_height=h)


def _write_mask_from_alpha(png_path: Path, mask_path: Path) -> None:
    from PIL import Image

    im = Image.open(png_path).convert("RGBA")
    alpha = im.split()[3]
    mask = Image.merge("RGBA", (alpha, alpha, alpha, alpha))
    mask.save(mask_path)


def _write_frame(asset_id: str, frame_path: Path, *, width: int, height: int) -> None:
    frame_path.write_text(
        json.dumps(
            {
                "orthographic": True,
                "transparent_bg": True,
                "pixel_size": [width, height],
                "lane": "hud-sprite-2d",
                "asset_id": asset_id,
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )


def rasterize_all() -> None:
    PREVIEW.mkdir(parents=True, exist_ok=True)
    STREAM.mkdir(parents=True, exist_ok=True)
    for asset_id in ALL_IDS:
        svg = SVG_DIR / f"{asset_id}.svg"
        if not svg.is_file():
            raise FileNotFoundError(svg)
        out_preview = PREVIEW / f"{asset_id}.png"
        if asset_id == BAR_ID:
            _rasterize(svg, out_preview, width=512, height=128)
            _rasterize(svg, SVG_DIR / f"{asset_id}.png", width=512, height=128)
        elif asset_id in PANEL_IDS:
            _rasterize(svg, out_preview, width=ICON_SIZE, height=ICON_SIZE)
            _rasterize(svg, SVG_DIR / f"{asset_id}.png", width=256, height=256)
        else:
            _rasterize(svg, out_preview, width=ICON_SIZE, height=ICON_SIZE)
            _rasterize(svg, SVG_DIR / f"{asset_id}.png", width=64, height=64)
        mask = PREVIEW / f"{asset_id}.mask.png"
        _write_mask_from_alpha(out_preview, mask)
        h = 128 if asset_id == BAR_ID else (512 if asset_id in PANEL_IDS else ICON_SIZE)
        w = 512 if asset_id == BAR_ID else ICON_SIZE
        _write_frame(asset_id, PREVIEW / f"{asset_id}.frame.json", width=w, height=h)
        shutil.copy2(SVG_DIR / f"{asset_id}.png", STREAM / f"{asset_id}.png")
    _build_icon_sheet()
    print(f"OK rasterized {len(ALL_IDS)} HUD sprites + icon-sheet-64")


def _build_icon_sheet() -> None:
    from PIL import Image

    tiles = []
    for asset_id in ICON_IDS:
        p = SVG_DIR / f"{asset_id}.png"
        tiles.append(Image.open(p).convert("RGBA"))
    sheet = Image.new("RGBA", (64 * len(tiles), 64), (0, 0, 0, 0))
    for i, tile in enumerate(tiles):
        sheet.paste(tile, (i * 64, 0), tile)
    out = PREVIEW / "icon-sheet-64.png"
    sheet.save(out)
    sheet.save(REPO / "unity/Assets/Art/EarlyGame/hud-icons/icon-sheet-64.png")


def main() -> int:
    sys.path.insert(0, str(REPO / "tools/art"))
    subprocess.run([sys.executable, str(REPO / "tools/art/generate_early_hud_icons.py"), "--packet34"], check=True)
    rasterize_all()
    return 0


if __name__ == "__main__":
    raise SystemExit(main())