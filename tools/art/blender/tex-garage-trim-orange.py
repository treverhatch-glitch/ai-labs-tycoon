#!/usr/bin/env python3
"""Blender 1024 PBR tile: garage trim orange."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_texture_tile as bt  # noqa: E402

ASSET_ID = "tex-garage-trim-orange"


def _strengthen_horizontal_grain(albedo: Path) -> None:
    from PIL import Image
    import math

    im = Image.open(albedo).convert("RGB")
    w, h = im.size
    px = im.load()
    period = 64
    for y in range(h):
        lift = 14.0 * math.sin(y * 2.0 * math.pi / period)
        warm = 8.0 * math.cos(y * 2.0 * math.pi / (period * 2))
        for x in range(w):
            r, g, b = px[x, y]
            px[x, y] = (
                min(255, int(r + lift + warm * 0.4)),
                min(255, int(g + lift * 0.55 + warm * 0.2)),
                min(255, int(b + lift * 0.15)),
            )
    im.save(albedo)


def main() -> None:
    bt.bake_tileable(ASSET_ID, (188, 68, 18), (252, 128, 32), pattern="trim_horiz", noise_scale=22.0)
    albedo = bt.TEX_OUT / f"{ASSET_ID}.png"
    if albedo.is_file():
        _strengthen_horizontal_grain(albedo)
        baked = bt.TEX_OUT / f"{ASSET_ID}_albedo.png"
        if baked.is_file():
            _strengthen_horizontal_grain(baked)
        bt._write_flat_previews(ASSET_ID, albedo, (188, 68, 18), (252, 128, 32), "trim_horiz")


if __name__ == "__main__":
    main()