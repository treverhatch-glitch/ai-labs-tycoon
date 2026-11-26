#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/staff-recruiter-idle.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Torso", (0.37, 0.52, 0.24), (0, 0.92, 0))
    bc.add_cube("Folder", (0.22, 0.28, 0.06), (-0.28, 0.96, 0.1))
    bc.add_cube("Tie", (0.06, 0.2, 0.03), (0, 1.05, 0.14))
    entry = bc.finish_asset("staff-recruiter-idle", "staff-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

