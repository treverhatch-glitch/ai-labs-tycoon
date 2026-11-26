#!/usr/bin/env python3
"""Typing pose — same build as staff-engineer-work, separate asset id."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Hat", (0.36, 0.08, 0.36), (0, 1.52, 0))
    bc.add_cube("Head", (0.26, 0.26, 0.26), (0, 1.38, 0))
    bc.add_cube("Torso", (0.42, 0.52, 0.24), (0, 0.92, 0))
    bc.add_cube("Laptop", (0.42, 0.04, 0.3), (0, 1.0, 0.2))
    for c in range(10):
        bc.add_cube(f"Key{c}", (0.03, 0.01, 0.03), (-0.18 + c * 0.04, 0.79, 0.12))
    entry = bc.finish_asset("staff-pose-typing", "staff-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)