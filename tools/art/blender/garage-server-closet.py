#!/usr/bin/env python3
"""Server closet — single cabinet block."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Shell", (1.10, 2.0, 0.90), (0, 1.0, 0))
    entry = bc.finish_asset("garage-server-closet", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)