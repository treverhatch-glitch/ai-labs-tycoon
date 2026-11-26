#!/usr/bin/env python3
"""Exterior wall segment — ~4m x 3m x 0.2m corrugated panel."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402

W, H, D = 4.0, 3.0, 0.20


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.24"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    bc.add_cube("Core", (W, H, D), (0, H * 0.5, 0))
    bc.add_cube("Trim", (W + 0.08, 0.18, D + 0.04), (0, H - 0.06, 0))
    bc.add_cube("Base", (W + 0.06, 0.16, D + 0.04), (0, 0.08, 0))
    for i, y in enumerate((0.65, 1.45, 2.25)):
        bc.add_cube(f"Course{i}", (W - 0.12, 0.06, D - 0.02), (0, y, 0))
    for i in range(9):
        x = -W * 0.4 + i * (W * 0.8 / 8)
        bc.add_cube(f"Rib{i}", (0.07, H - 0.30, D * 0.82), (x, H * 0.5, 0.0))
    bc.add_cube("Spine", (W * 0.55, H * 0.92, D * 0.55), (0, H * 0.5, 0))
    entry = bc.finish_asset("garage-exterior-wall", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)