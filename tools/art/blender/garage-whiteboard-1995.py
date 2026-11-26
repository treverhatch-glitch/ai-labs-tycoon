#!/usr/bin/env python3
"""Whiteboard — single board slab."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Board", (1.30, 1.08, 0.06), (0, 1.08, 0))
    entry = bc.finish_asset("garage-whiteboard-1995", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)