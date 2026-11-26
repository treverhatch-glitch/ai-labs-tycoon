#!/usr/bin/env python3
"""Curb + grass — vertical curb segment."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.22"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    bc.add_cube("Curb", (0.18, 0.72, 0.22), (0, 0.36, 0))
    bc.add_cube("Top", (0.42, 0.08, 0.30), (0, 0.72, 0.02))
    bc.add_cube("Grass", (0.40, 0.12, 0.26), (0, 0.08, 0.10))
    entry = bc.finish_asset("garage-lot-curb", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)