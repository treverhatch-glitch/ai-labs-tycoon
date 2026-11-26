#!/usr/bin/env python3
"""Headless Blender: garage-floor-tile. Run: blender -b --python tools/art/blender/garage-floor-tile.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    slab = bc.add_cube("Slab", (0.45, 0.04, 0.45), (0, 0.02, 0))
    pad = bc.add_cube("Pad", (0.38, 0.008, 0.38), (0, 0.046, 0))
    bc.solidify_object(slab, 0.005)
    entry = bc.finish_asset("garage-floor-tile", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")


if __name__ == "__main__":
    bc.run_script(main)