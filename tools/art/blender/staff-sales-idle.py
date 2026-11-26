#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/staff-sales-idle.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Torso", (0.4, 0.52, 0.24), (0, 0.92, 0))
    bc.add_cube("Head", (0.26, 0.26, 0.26), (0, 1.38, 0))
    bc.add_cube("Phone", (0.06, 0.14, 0.05), (0.28, 1.05, 0.1))
    entry = bc.finish_asset("staff-sales-idle", "staff-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

