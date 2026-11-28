#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/garage-exterior-wall.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Core", (0.45, 2.4, 0.18), (0, 1.2, 0))
    bc.add_cube("Trim", (0.48, 0.08, 0.2), (0, 2.35, 0.01))
    entry = bc.finish_asset("garage-exterior-wall", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

