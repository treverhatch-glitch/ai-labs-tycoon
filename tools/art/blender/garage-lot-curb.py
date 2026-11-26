#!/usr/bin/env python3
"""garage-lot-curb — multi-part garage kit."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
from garage_kit_builders import BUILDERS  # noqa: E402


def main():
    BUILDERS["garage-lot-curb"]()
    entry = bc.finish_asset("garage-lot-curb", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris", "parts")
    return entry


if __name__ == "__main__":
    bc.run_script(main)
