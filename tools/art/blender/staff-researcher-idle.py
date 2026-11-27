#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/staff-researcher-idle.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Torso", (0.36, 0.52, 0.24), (0, 0.92, 0))
    bc.add_cube("Head", (0.26, 0.26, 0.26), (0, 1.38, 0))
    bc.add_cube("Ponytail", (0.08, 0.22, 0.12), (-0.18, 1.42, 0))
    bc.add_cube("Clipboard", (0.18, 0.24, 0.04), (0.22, 0.95, 0.1))
    entry = bc.finish_asset("staff-researcher-idle", "staff-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

