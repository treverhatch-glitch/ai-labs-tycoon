#!/usr/bin/env python3
"""Window frame — frame, glass, sill."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.22"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    bc.add_cube("Frame", (0.88, 1.15, 0.14), (0, 0.58, 0))
    glass = bc.add_cube("Glass", (0.58, 0.78, 0.08), (0, 0.58, 0.04))
    bc.assign_solid(glass, (0.22, 0.42, 0.68, 1.0), 0.18)
    bc.add_cube("Sill", (0.96, 0.10, 0.20), (0, 0.05, 0.04))
    bc.add_cube("Lintel", (0.96, 0.08, 0.18), (0, 1.12, 0.04))
    bc.add_cube("Spine", (0.20, 0.20, 0.12), (0, 0.58, 0.0))
    entry = bc.finish_asset("garage-window-frame", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)