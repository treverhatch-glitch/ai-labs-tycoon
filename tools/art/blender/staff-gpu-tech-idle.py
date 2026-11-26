#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/staff-gpu-tech-idle.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cylinder("Cap", 0.14, 0.06, (0, 1.48, 0))
    bc.add_cube("Torso", (0.38, 0.52, 0.24), (0, 0.92, 0))
    bc.add_cylinder("Tool", 0.02, 0.2, (0.3, 1.05, 0.05))
    entry = bc.finish_asset("staff-gpu-tech-idle", "staff-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

