#!/usr/bin/env python3
"""Blender 1024 PBR tile: garage trim orange."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_texture_tile as bt  # noqa: E402

ASSET_ID = "tex-garage-trim-orange"


def main() -> None:
    bt.bake_tileable(ASSET_ID, (188, 68, 18), (252, 128, 32), pattern="metal", noise_scale=18.0)


if __name__ == "__main__":
    main()