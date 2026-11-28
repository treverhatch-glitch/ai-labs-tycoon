#!/usr/bin/env python3
"""garage-crt-monitor — boxy CRT hero prop (separate from desk base)."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
from kenney_garage_asset import BUILDERS  # noqa: E402


def main():
    BUILDERS["garage-crt-monitor"]()
    entry = bc.finish_asset("garage-crt-monitor", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris", "parts")
    return entry


if __name__ == "__main__":
    bc.run_script(main)