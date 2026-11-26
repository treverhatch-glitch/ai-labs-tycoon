#!/usr/bin/env python3
"""Street curb — low concrete + grass wedge."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.20"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    curb = bc.add_cube("Curb", (0.72, 0.14, 0.26), (0, 0.07, 0))
    grass = bc.add_cube("Grass", (0.72, 0.08, 0.30), (0, 0.11, 0.08))
    bc.assign_solid(grass, (0.28, 0.48, 0.22, 1.0), 0.72)
    bc.add_cube("Weld", (0.68, 0.10, 0.28), (0, 0.10, 0.04))
    entry = bc.finish_asset("garage-lot-curb", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)