#!/usr/bin/env python3
"""Roll door — frame block + inset curtain."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Frame", (2.04, 2.20, 0.14), (0, 1.10, 0))
    bc.add_cube("Curtain", (1.56, 1.56, 0.06), (0, 1.02, 0))
    entry = bc.finish_asset("garage-roll-door", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)