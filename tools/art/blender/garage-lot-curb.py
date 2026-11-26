#!/usr/bin/env python3
"""Street curb — long low beveled concrete (no grass backdrop)."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.18"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    curb = bc.add_cube("Curb", (4.0, 0.18, 0.32), (0, 0.09, 0))
    bc.assign_solid(curb, (0.58, 0.56, 0.54, 1.0), 0.82)
    bc.bevel_object(curb, 0.025, 3)
    entry = bc.finish_asset("garage-lot-curb", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)