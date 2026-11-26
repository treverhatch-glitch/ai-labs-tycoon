#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/garage-roll-door.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Frame", (2.0, 2.2, 0.12), (0, 1.1, 0))
    for i in range(10):
        bc.add_cube(f"Slat{i}", (1.88, 0.12, 0.04), (0, 0.12 + i * 0.2, 0.04))
    entry = bc.finish_asset("garage-roll-door", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

