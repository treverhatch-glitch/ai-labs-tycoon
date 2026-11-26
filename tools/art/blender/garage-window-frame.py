#!/usr/bin/env python3
"""Window frame — upright frame + glass."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.24"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    bc.add_cube("Frame", (0.52, 1.05, 0.14), (0, 0.525, 0))
    glass = bc.add_cube("Glass", (0.36, 0.72, 0.06), (0, 0.52, 0.06))
    bc.assign_solid(glass, (0.22, 0.42, 0.68, 1.0), 0.18)
    bc.add_cube("Stud", (0.10, 0.10, 0.10), (0, 0.48, 0.02))
    entry = bc.finish_asset("garage-window-frame", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)