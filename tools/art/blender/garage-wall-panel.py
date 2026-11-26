#!/usr/bin/env python3
"""Wall panel — block with wide inset stripe."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Wall", (0.48, 2.2, 0.10), (0, 1.1, 0))
    bc.add_cube("Stripe", (0.14, 1.8, 0.05), (0.14, 1.15, 0.03))
    entry = bc.finish_asset("garage-wall-panel", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)