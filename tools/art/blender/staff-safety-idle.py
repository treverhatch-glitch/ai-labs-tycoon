#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/staff-safety-idle.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Vest", (0.4, 0.45, 0.26), (0, 0.98, 0))
    bc.add_cube("Head", (0.26, 0.26, 0.26), (0, 1.38, 0))
    bc.add_cube("Clip", (0.14, 0.2, 0.06), (0.24, 0.92, 0.12))
    entry = bc.finish_asset("staff-safety-idle", "staff-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

