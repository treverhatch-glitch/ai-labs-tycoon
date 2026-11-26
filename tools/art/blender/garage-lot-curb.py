#!/usr/bin/env python3
"""Curb + grass strip — overlapping weld."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.16"
    bc.add_cube("Curb", (0.46, 0.16, 0.24), (0, 0.08, 0))
    bc.add_cube("Grass", (0.46, 0.10, 0.28), (0, 0.14, 0.06))
    entry = bc.finish_asset("garage-lot-curb", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)