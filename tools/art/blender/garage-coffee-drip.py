#!/usr/bin/env python3
"""Mr Coffee style drip — red/orange body, grounded single mesh."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Base", (0.32, 0.06, 0.26), (0, 0.03, 0))
    bc.add_cube("Body", (0.30, 0.38, 0.22), (0, 0.25, 0))
    bc.add_cube("Warmer", (0.28, 0.04, 0.20), (0, 0.46, 0))
    pot = bc.add_cylinder("Pot", 0.09, 0.20, (0, 0.54, 0.02), vertices=20)
    bc.add_cylinder("Handle", 0.02, 0.14, (0.14, 0.58, 0.02), vertices=8)
    bc.add_cube("Spout", (0.06, 0.04, 0.08), (-0.12, 0.64, 0.02))
    bc.add_cube("Cord", (0.02, 0.01, 0.18), (0.14, 0.01, -0.08))
    entry = bc.finish_asset("garage-coffee-drip", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)