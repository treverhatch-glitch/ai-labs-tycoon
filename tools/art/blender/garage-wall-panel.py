#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/garage-wall-panel.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Wall", (0.45, 2.2, 0.08), (0, 1.1, 0))
    bc.add_cube("Baseboard", (0.45, 0.12, 0.1), (0, 0.06, 0.02))
    bc.add_cube("Stripe0", (0.08, 1.8, 0.025), (-0.14, 1.25, 0.048))
    bc.add_cube("Stripe1", (0.08, 1.8, 0.025), (0.14, 0.85, 0.048))
    bc.add_cube("Stripe2", (0.35, 0.08, 0.025), (0.0, 1.55, 0.048))
    entry = bc.finish_asset("garage-wall-panel", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry

if __name__ == "__main__":
    bc.run_script(main)

