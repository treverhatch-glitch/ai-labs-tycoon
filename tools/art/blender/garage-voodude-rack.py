#!/usr/bin/env python3
"""Headless Blender: VooDude 1 GPU rack."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Frame", (0.62, 1.85, 0.78), (0, 0.925, 0))
    for slot in range(6):
        sy = 0.22 + slot * 0.27
        bc.add_cube(f"UBay{slot}", (0.54, 0.2, 0.58), (0, sy, 0.02))
        bc.add_cube(f"Card{slot}", (0.42, 0.06, 0.48), (0, sy + 0.05, 0.18))
        bc.add_cylinder(f"Fan{slot}", 0.08, 0.02, (0.12, sy + 0.08, 0.32))
    bc.add_cube("Badge", (0.38, 0.1, 0.02), (0, 1.72, 0.36))
    entry = bc.finish_asset("garage-voodude-rack", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)