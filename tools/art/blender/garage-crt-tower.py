#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/garage-crt-tower.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Body", (0.24, 0.58, 0.44), (0, 0.29, 0))
    bc.add_cube("Floppy", (0.18, 0.06, 0.32), (0, 0.44, 0.2))
    bc.add_cube("Led", (0.04, 0.04, 0.02), (0.05, 0.52, 0.22))
    entry = bc.finish_asset("garage-crt-tower", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

