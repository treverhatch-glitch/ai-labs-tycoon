#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/garage-land-for-sale-sign.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cylinder("Post", 0.05, 1.6, (0, 0.8, 0))
    bc.add_cube("Board", (1.2, 0.7, 0.08), (0, 1.45, 0))
    entry = bc.finish_asset("garage-land-for-sale-sign", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

