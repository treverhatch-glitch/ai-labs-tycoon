#!/usr/bin/env python3
"""Headless Blender: garage-floor-tile. Run: blender -b --python tools/art/blender/garage-floor-tile.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    slab = bc.add_cube("Slab", (0.48, 0.05, 0.48), (0, 0.025, 0))
    pad = bc.add_cube("Pad", (0.40, 0.010, 0.40), (0, 0.048, 0))
    bc.add_cube("Grout", (0.03, 0.015, 0.40), (-0.2, 0.048, 0.0))
    bc.add_cube("Accent", (0.48, 0.024, 0.14), (0.0, 0.049, 0.16))
    bc.add_cube("Corner", (0.20, 0.024, 0.20), (-0.12, 0.049, -0.12))
    bc.solidify_object(slab, 0.005)
    entry = bc.finish_asset("garage-floor-tile", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)