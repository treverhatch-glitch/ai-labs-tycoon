#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/staff-founder-idle.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Torso", (0.38, 0.52, 0.24), (0, 0.92, 0))
    bc.add_cube("Mug", (0.1, 0.12, 0.1), (0.26, 0.98, 0.05))
    entry = bc.finish_asset("staff-founder-idle", "staff-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

