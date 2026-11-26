#!/usr/bin/env python3
"""Floor tile — single slab (orange inset tint region)."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Slab", (0.48, 0.06, 0.48), (0, 0.03, 0))
    bc.add_cube("AccentCore", (0.34, 0.045, 0.34), (0, 0.03, 0))
    entry = bc.finish_asset("garage-floor-tile", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)