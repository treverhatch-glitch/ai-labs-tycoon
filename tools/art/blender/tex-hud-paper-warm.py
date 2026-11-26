#!/usr/bin/env python3
"""Blender 1024 PBR tile: HUD paper warm."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_texture_tile as bt  # noqa: E402

ASSET_ID = "tex-hud-paper-warm"


def main() -> None:
    bt.bake_tileable(ASSET_ID, (152, 98, 58), (255, 210, 138), noise_scale=3.2, fiber_spots=True)


if __name__ == "__main__":
    main()