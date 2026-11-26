#!/usr/bin/env python3
"""Folding chair: seat, back, crossed frame, grounded."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Seat", (0.48, 0.04, 0.46), (0, 0.48, 0))
    bc.add_cube("Back", (0.46, 0.42, 0.04), (0, 0.72, -0.20))
    # leg pairs with frame rails (touching seat + ground)
    for sx, sz in ((-1, 1), (1, 1), (-1, -1), (1, -1)):
        x, z = 0.20 * sx, 0.18 * sz
        bc.add_cylinder(f"Leg_{sx}_{sz}", 0.012, 0.46, (x, 0.23, z), vertices=8)
    bc.add_cube("RailF", (0.44, 0.025, 0.025), (0, 0.36, 0.17))
    bc.add_cube("RailB", (0.44, 0.025, 0.025), (0, 0.36, -0.17))
    bc.add_cube("BraceL", (0.025, 0.025, 0.36), (-0.20, 0.34, 0))
    bc.add_cube("BraceR", (0.025, 0.025, 0.36), (0.20, 0.34, 0))
    entry = bc.finish_asset("garage-folding-chair", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)