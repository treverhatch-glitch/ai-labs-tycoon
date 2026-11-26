#!/usr/bin/env python3
"""Exterior wall segment — corrugated panel + trim."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.24"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    bc.add_cube("Core", (0.52, 2.2, 0.22), (0, 1.1, 0))
    bc.add_cube("Trim", (0.54, 0.14, 0.22), (0, 2.11, 0.0))
    bc.add_cube("Sill", (0.54, 0.14, 0.22), (0, 0.09, 0.0))
    for i, y in enumerate((0.45, 1.05, 1.65)):
        bc.add_cube(f"Seam{i}", (0.48, 0.04, 0.20), (0, y, 0.0))
    for i in range(4):
        x = -0.15 + i * 0.10
        bc.add_cube(f"Rib{i}", (0.05, 1.9, 0.16), (x, 1.1, 0.03))
    entry = bc.finish_asset("garage-exterior-wall", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)