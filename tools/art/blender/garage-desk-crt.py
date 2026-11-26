#!/usr/bin/env python3
"""Headless Blender: CRT desk hero prop."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Desktop", (1.15, 0.06, 0.68), (0, 0.74, 0))
    for i, ox in enumerate([-0.36, 0.0, 0.36]):
        bc.add_cube(f"Drawer{i}", (0.32, 0.22, 0.48), (ox, 0.0, 0.05))
    for dx, dz in ((-0.46, -0.28), (0.46, -0.28), (-0.46, 0.28), (0.46, 0.28)):
        bc.add_cylinder(f"Leg_{dx}", 0.035, 0.72, (dx, 0.36, dz))
    bc.add_cube("CRT", (0.48, 0.42, 0.45), (0.14, 0.88, -0.02))
    bc.add_cube("Screen", (0.4, 0.32, 0.04), (0.14, 0.9, 0.15))
    bc.add_cube("Keyboard", (0.44, 0.04, 0.2), (0, 0.76, 0.14))
    for row in range(2):
        for col in range(8):
            bc.add_cube(f"Key{row}_{col}", (0.032, 0.012, 0.032), (-0.14 + col * 0.038, 0.79, 0.08 + row * 0.035))
    entry = bc.finish_asset("garage-desk-crt", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)