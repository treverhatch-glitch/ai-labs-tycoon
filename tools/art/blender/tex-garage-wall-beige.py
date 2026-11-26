#!/usr/bin/env python3
"""Blender 1024 PBR tile: garage wall beige."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_texture_tile as bt  # noqa: E402

ASSET_ID = "tex-garage-wall-beige"


def main() -> None:
    bt.bake_tileable(ASSET_ID, (178, 138, 88), (228, 188, 128), noise_scale=12.0)


if __name__ == "__main__":
    main()