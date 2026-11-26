#!/usr/bin/env python3
"""GPU rack — shell + faceplate overlap."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("RackShell", (0.66, 1.94, 0.66), (0, 0.97, 0))
    face = bc.add_cube("FacePlate", (0.60, 1.50, 0.18), (0, 1.02, 0.16))
    bc.assign_solid(face, (0.12, 0.12, 0.14, 1.0), 0.38)
    entry = bc.finish_asset("garage-voodude-rack", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)