#!/usr/bin/env python3
"""Mr Coffee — machine block with inset carafe."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Machine", (0.38, 0.58, 0.32), (0, 0.29, 0))
    bc.add_cube("Carafe", (0.14, 0.22, 0.14), (0, 0.46, 0.04))
    entry = bc.finish_asset("garage-coffee-drip", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)