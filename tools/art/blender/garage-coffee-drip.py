#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/garage-coffee-drip.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Base", (0.28, 0.32, 0.22), (0, 0.16, 0))
    bc.add_cylinder("Pot", 0.1, 0.22, (0, 0.5, 0.05))
    entry = bc.finish_asset("garage-coffee-drip", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

