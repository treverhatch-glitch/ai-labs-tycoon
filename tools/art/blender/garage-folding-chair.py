#!/usr/bin/env python3
"""Folding chair — single mesh block (blue garage seat)."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Chair", (0.50, 0.94, 0.44), (0, 0.47, 0))
    entry = bc.finish_asset("garage-folding-chair", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)