#!/usr/bin/env python3
"""Window frame — single block with blue glass tint."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    ob = bc.add_cube("Frame", (0.92, 0.72, 0.12), (0, 1.48, 0))
    bc.assign_solid(ob, (1.0, 0.42, 0.02, 1.0), 0.32)
    entry = bc.finish_asset("garage-window-frame", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)