#!/usr/bin/env python3
"""CRT tower — tower block with inset screen."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Tower", (0.36, 1.06, 0.32), (0, 0.57, 0))
    screen = bc.add_cube("Screen", (0.28, 0.22, 0.18), (0, 0.82, 0.08))
    bc.assign_solid(screen, (0.10, 0.90, 0.50, 1.0), 0.15)
    entry = bc.finish_asset("garage-crt-tower", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)