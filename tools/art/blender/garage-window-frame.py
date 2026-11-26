#!/usr/bin/env python3
"""Window frame — ~1.2m x 1.0m with mullions."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402

W, H, D = 1.2, 1.0, 0.14


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.22"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    bc.add_cube("Frame", (W, H, D), (0, H * 0.5, 0))
    glass = bc.add_cube("Glass", (W - 0.28, H - 0.28, 0.06), (0, H * 0.5, D * 0.22))
    bc.assign_solid(glass, (0.22, 0.48, 0.72, 1.0), 0.12)
    bc.add_cube("MullionV", (0.08, H - 0.20, D * 0.70), (0, H * 0.5, D * 0.10))
    bc.add_cube("MullionH", (W - 0.24, 0.08, D * 0.70), (0, H * 0.5, D * 0.10))
    bc.add_cube("Sill", (W + 0.10, 0.12, D + 0.08), (0, 0.06, 0.02))
    bc.add_cube("Lintel", (W + 0.10, 0.10, D + 0.06), (0, H - 0.05, 0.02))
    entry = bc.finish_asset("garage-window-frame", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)