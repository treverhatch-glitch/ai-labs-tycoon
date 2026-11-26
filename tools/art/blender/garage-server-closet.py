#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/garage-server-closet.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Shell", (1.1, 2.0, 0.9), (0, 1.0, 0))
    bc.add_cube("Door", (0.88, 1.72, 0.06), (0, 0.95, 0.42))
    bc.add_cube("Handle", (0.12, 0.04, 0.08), (0.32, 1.0, 0.48))
    entry = bc.finish_asset("garage-server-closet", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

