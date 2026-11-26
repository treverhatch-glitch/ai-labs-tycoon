#!/usr/bin/env python3
"""Desk + CRT workstation."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Desk", (1.18, 0.76, 0.70), (0, 0.38, 0))
    bc.add_cube("CRTBlock", (0.52, 0.54, 0.44), (0.14, 0.62, 0.18))
    entry = bc.finish_asset("garage-desk-crt", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)