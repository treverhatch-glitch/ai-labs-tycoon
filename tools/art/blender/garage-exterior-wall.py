#!/usr/bin/env python3
"""Exterior wall — 4×3×0.2m beige block + orange trim."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402

W, H, D = 4.0, 3.0, 0.20


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.16"
    os.environ["PREVIEW_AZIMUTH_DEG"] = "52"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    bc.add_cube("Core", (W, H, D), (0, H * 0.5, 0))
    bc.add_cube("Trim", (W + 0.08, 0.18, D + 0.04), (0, H - 0.06, 0))
    bc.add_cube("Base", (W + 0.06, 0.16, D + 0.04), (0, 0.08, 0))
    # Interior part markers only (no surface ribs — texture carries mortar).
    for i in range(9):
        y = 0.35 + i * (2.2 / 9.0)
        x = -1.5 + (i % 3) * 1.5
        bc.add_cube(f"Marker{i}", (0.08, 0.08, 0.08), (x, y, 0))
    entry = bc.finish_asset("garage-exterior-wall", "garage-kit")
    bc.rerender_garage_preview("garage-exterior-wall", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris", entry["subShapeCount"], "parts")
    return entry


if __name__ == "__main__":
    bc.run_script(main)