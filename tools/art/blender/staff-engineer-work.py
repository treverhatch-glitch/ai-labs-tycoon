#!/usr/bin/env python3
"""Headless Blender: engineer with hard hat and laptop."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Hat", (0.36, 0.08, 0.36), (0, 1.52, 0))
    bc.add_cube("Head", (0.26, 0.26, 0.26), (0, 1.38, 0))
    bc.add_cube("Torso", (0.42, 0.52, 0.24), (0, 0.92, 0))
    bc.add_cube("Laptop", (0.38, 0.04, 0.28), (0, 1.0, 0.18))
    bc.add_cube("Screen", (0.32, 0.22, 0.03), (0, 1.08, 0.22))
    entry = bc.finish_asset("staff-engineer-work", "staff-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)