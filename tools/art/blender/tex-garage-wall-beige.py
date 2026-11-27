#!/usr/bin/env python3
"""Tileable garage wall — warm beige cinderblock (running bond, no corrugated noise)."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_texture_tile as bt  # noqa: E402

ASSET_ID = "tex-garage-wall-beige"


def main() -> None:
    base = (146, 124, 100)
    mortar = (108, 96, 84)
    bt.bake_tileable(ASSET_ID, base, mortar, pattern="cinderblock", noise_scale=6.0)


if __name__ == "__main__":
    main()