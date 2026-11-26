#!/usr/bin/env python3
"""Exterior wall + inset orange trim."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.28"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    bc.add_cube("Core", (0.48, 2.4, 0.20), (0, 1.2, 0))
    bc.add_cube("Trim", (0.40, 0.16, 0.10), (0, 2.18, 0.04))
    bc.add_cube("Stud", (0.12, 0.12, 0.12), (0, 0.65, 0))
    entry = bc.finish_asset("garage-exterior-wall", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)