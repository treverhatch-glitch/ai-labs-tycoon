#!/usr/bin/env python3
"""Window ~1.2×1m — shell, stiles, rails, mullions, glass, sill, lintel."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402

W, H, D = 1.2, 1.0, 0.14


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.12"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    bc.add_cube("OuterShell", (W, H, D), (0, H * 0.5, 0))
    bc.add_cube("StileL", (0.10, H - 0.16, D * 0.75), (-W * 0.5 + 0.05, H * 0.5, D * 0.08))
    bc.add_cube("StileR", (0.10, H - 0.16, D * 0.75), (W * 0.5 - 0.05, H * 0.5, D * 0.08))
    bc.add_cube("RailTop", (W - 0.16, 0.10, D * 0.75), (0, H - 0.05, D * 0.08))
    bc.add_cube("RailBottom", (W - 0.16, 0.10, D * 0.75), (0, 0.05, D * 0.08))
    glass = bc.add_cube("GlassPane", (W - 0.30, H - 0.30, 0.06), (0, H * 0.5, D * 0.22))
    bc.assign_solid(glass, (0.24, 0.52, 0.78, 1.0), 0.10)
    bc.add_cube("MullionV", (0.08, H - 0.22, D * 0.70), (0, H * 0.5, D * 0.10))
    bc.add_cube("MullionH", (W - 0.28, 0.08, D * 0.70), (0, H * 0.5, D * 0.10))
    bc.add_cube("Sill", (W + 0.10, 0.12, D + 0.08), (0, 0.06, 0.02))
    bc.add_cube("Lintel", (W + 0.10, 0.10, D + 0.06), (0, H - 0.05, 0.02))
    bc.add_cube("Weld", (0.24, 0.24, D * 0.65), (0, H * 0.5, 0))
    entry = bc.finish_asset("garage-window-frame", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris", entry["subShapeCount"], "parts")
    return entry


if __name__ == "__main__":
    bc.run_script(main)