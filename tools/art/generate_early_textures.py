#!/usr/bin/env python3
"""Tileable 512 procedural albedo + normal maps (no checker placeholders)."""
from __future__ import annotations

import json
import shutil
from pathlib import Path

from noise_lib import height_carpet, height_concrete, height_metal, height_wood, write_normal_map, write_png_rgb

REPO_ROOT = Path(__file__).resolve().parents[2]
OUT = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "textures"
STREAM = REPO_ROOT / "unity" / "Assets" / "StreamingAssets" / "EarlyGame" / "textures"
SIZE = 512

PALETTE = {
    "tex-garage-carpet": ((184, 168, 144), height_carpet),
    "tex-garage-concrete": ((200, 190, 175), height_concrete),
    "tex-garage-trim-orange": ((168, 88, 40), height_wood),
    "tex-garage-wall-beige": ((210, 200, 180), height_wood),
    "tex-garage-door-metal": ((138, 142, 150), height_metal),
    "tex-garage-server-paint": ((106, 110, 118), height_metal),
    "tex-hud-paper-warm": ((244, 240, 232), height_wood),
    "tex-crt-phosphor": ((60, 150, 90), height_carpet),
}


def albedo_from_height(base_rgb: tuple[int, int, int], height_fn):
    br, bg, bb = base_rgb

    def fn(_nx, _ny, x, y):
        h = height_fn(x / SIZE, y / SIZE)
        return br * h, bg * h, bb * h

    return fn


def main() -> None:
    entries = []
    OUT.mkdir(parents=True, exist_ok=True)
    STREAM.mkdir(parents=True, exist_ok=True)
    for asset_id, (rgb, hfn) in PALETTE.items():
        albedo_path = OUT / f"{asset_id}.png"
        normal_path = OUT / f"{asset_id}_normal.png"
        write_png_rgb(albedo_path, SIZE, SIZE, albedo_from_height(rgb, hfn))
        write_normal_map(normal_path, SIZE, SIZE, hfn)
        shutil.copy2(albedo_path, STREAM / albedo_path.name)
        shutil.copy2(normal_path, STREAM / normal_path.name)
        entries.append(
            {
                "id": asset_id,
                "path": f"Assets/Art/EarlyGame/textures/{asset_id}.png",
                "normalPath": f"Assets/Art/EarlyGame/textures/{asset_id}_normal.png",
                "license": "CC0-1.0 (procedural, in-repo)",
                "sizePx": SIZE,
                "tileable": True,
            }
        )
    manifest = REPO_ROOT / "unity/Assets/Art/EarlyGame" / "textures-manifest.json"
    manifest.write_text(
        json.dumps({"generator": "tools/art/generate_early_textures.py", "fidelity": "packet-2", "assets": entries}, indent=2)
        + "\n",
        encoding="utf-8",
    )
    print(f"Wrote {len(entries)} albedo+normal pairs at {SIZE}px")


if __name__ == "__main__":
    main()