#!/usr/bin/env python3
"""Blender 1024 PBR tile: garage wall beige."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_texture_tile as bt  # noqa: E402

ASSET_ID = "tex-garage-wall-beige"


def main() -> None:
    bt.bake_tileable(ASSET_ID, (128, 108, 88), (228, 198, 158), pattern="corrugated", noise_scale=8.0)


if __name__ == "__main__":
    main()