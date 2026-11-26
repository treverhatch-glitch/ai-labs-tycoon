#!/usr/bin/env python3
"""Corrugated roll door — frame + interlocked slats (connected)."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("FrameL", (0.08, 2.15, 0.10), (-0.98, 1.075, 0))
    bc.add_cube("FrameR", (0.08, 2.15, 0.10), (0.98, 1.075, 0))
    bc.add_cube("FrameTop", (2.0, 0.10, 0.10), (0, 2.15, 0))
    bc.add_cube("Sill", (2.0, 0.08, 0.14), (0, 0.04, 0))
    for i in range(12):
        y = 0.12 + i * 0.16
        bc.add_cube(f"Slat{i}", (1.84, 0.10, 0.05), (0, y, 0.02))
        bc.add_cube(f"Rib{i}", (1.84, 0.02, 0.06), (0, y + 0.04, 0.04))
    entry = bc.finish_asset("garage-roll-door", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)