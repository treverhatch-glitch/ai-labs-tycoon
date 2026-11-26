#!/usr/bin/env python3
"""For-sale sign — post + board."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.28"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    bc.add_cube("Post", (0.14, 1.85, 0.14), (0, 0.925, 0))
    bc.add_cube("Sign", (1.25, 1.65, 0.12), (0, 1.45, 0.02))
    bc.add_cube("Stud", (0.10, 0.10, 0.10), (0, 0.55, 0))
    entry = bc.finish_asset("garage-land-for-sale-sign", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)