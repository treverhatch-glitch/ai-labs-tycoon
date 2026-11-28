#!/usr/bin/env python3
"""garage-server-closet — multi-part garage kit."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
from garage_kit_builders import BUILDERS  # noqa: E402


def main():
    BUILDERS["garage-server-closet"]()
    entry = bc.finish_asset("garage-server-closet", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris", "parts")
    return entry


if __name__ == "__main__":
    bc.run_script(main)
