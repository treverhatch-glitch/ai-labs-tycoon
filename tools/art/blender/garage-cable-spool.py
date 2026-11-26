#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/garage-cable-spool.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cylinder("Hub", 0.1, 0.16, (0, 0.08, 0))
    bc.add_cylinder("Coil", 0.16, 0.24, (0, 0.2, 0))
    entry = bc.finish_asset("garage-cable-spool", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

