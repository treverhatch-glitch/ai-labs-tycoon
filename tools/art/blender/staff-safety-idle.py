#!/usr/bin/env python3
"""Safety officer with orange vest and badge."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import staff_build as sb  # noqa: E402


def main():
    sb.build_safety_idle()
    entry = bc.finish_asset("staff-safety-idle", "staff-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)