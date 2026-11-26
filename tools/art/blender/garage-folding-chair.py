#!/usr/bin/env python3
"""Headless Blender: folding chair with legs and backrest."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Seat", (0.52, 0.04, 0.5), (0, 0.48, 0))
    bc.add_cube("Back", (0.5, 0.45, 0.05), (0, 0.72, -0.22))
    for dx, dz in ((-0.2, 0.18), (0.2, 0.18), (-0.2, -0.18), (0.2, -0.18)):
        bc.add_cylinder(f"Leg_{dx}_{dz}", 0.018, 0.46, (dx, 0.23, dz))
    entry = bc.finish_asset("garage-folding-chair", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)