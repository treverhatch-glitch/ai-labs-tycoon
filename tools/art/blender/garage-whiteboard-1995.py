#!/usr/bin/env python3
"""Whiteboard with orange frame, legs, marker tray, scribble strokes."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    for dx in (-0.55, 0.55):
        bc.add_cube(f"Leg_{dx}", (0.05, 0.55, 0.05), (dx, 0.275, 0))
    bc.add_cube("FrameBot", (1.35, 0.06, 0.06), (0, 0.58, 0))
    bc.add_cube("FrameTop", (1.35, 0.06, 0.06), (0, 1.62, 0))
    bc.add_cube("FrameL", (0.06, 1.08, 0.06), (-0.66, 1.10, 0))
    bc.add_cube("FrameR", (0.06, 1.08, 0.06), (0.66, 1.10, 0))
    bc.add_cube("Surface", (1.22, 0.96, 0.03), (0, 1.10, 0.02))
    bc.add_cube("Tray", (0.42, 0.05, 0.10), (0, 0.52, 0.06))
    for i, (px, py, sx) in enumerate(
        ((-0.35, 1.22, 0.28), (0.1, 1.05, 0.22), (0.28, 1.28, 0.26), (-0.15, 0.95, 0.30))
    ):
        bc.add_cube(f"Stroke{i}", (sx, 0.022, 0.022), (px, py, 0.055))
    entry = bc.finish_asset("garage-whiteboard-1995", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)