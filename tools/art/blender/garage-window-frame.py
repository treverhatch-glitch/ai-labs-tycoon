#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/garage-window-frame.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Frame", (0.9, 0.7, 0.1), (0, 1.5, 0))
    bc.add_cube("Header", (0.92, 0.08, 0.11), (0, 1.86, 0.0))
    bc.add_cube("Pane", (0.7, 0.5, 0.04), (0, 1.5, 0.03))
    bc.add_cube("Mullion", (0.05, 0.46, 0.045), (0, 1.5, 0.035))
    bc.add_cube("Sill", (0.94, 0.10, 0.14), (0, 1.10, 0.02))
    entry = bc.finish_asset("garage-window-frame", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

