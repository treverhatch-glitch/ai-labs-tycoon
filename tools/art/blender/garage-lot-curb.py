#!/usr/bin/env python3
"""Long concrete curb — deck, gutter lip, grime stains."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.14"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    curb = bc.add_cube("CurbDeck", (4.0, 0.22, 0.36), (0, 0.11, 0))
    bc.assign_solid(curb, (0.58, 0.56, 0.54, 1.0), 0.82)
    bc.add_cube("GutterLip", (3.92, 0.05, 0.10), (0, 0.205, 0.12))
    bc.add_cube("StainA", (1.1, 0.05, 0.18), (-1.2, 0.11, 0.02))
    bc.add_cube("StainB", (0.9, 0.05, 0.16), (0.85, 0.105, 0.03))
    bc.bevel_object(curb, 0.028, 3)
    entry = bc.finish_asset("garage-lot-curb", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris", entry["subShapeCount"], "parts")
    return entry


if __name__ == "__main__":
    bc.run_script(main)