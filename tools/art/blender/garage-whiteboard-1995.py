#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/garage-whiteboard-1995.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Board", (1.4, 0.9, 0.05), (0, 1.1, 0))
    bc.add_cube("Tray", (0.5, 0.04, 0.12), (0, 0.62, 0.04))
    entry = bc.finish_asset("garage-whiteboard-1995", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

