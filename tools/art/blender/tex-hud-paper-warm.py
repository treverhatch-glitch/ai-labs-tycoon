#!/usr/bin/env python3
"""Blender 1024 PBR tile: HUD paper warm."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_texture_tile as bt  # noqa: E402

ASSET_ID = "tex-hud-paper-warm"


def main() -> None:
    bt.bake_tileable(ASSET_ID, (218, 148, 68), (252, 208, 138), pattern="fibre", noise_scale=11.0)


if __name__ == "__main__":
    main()