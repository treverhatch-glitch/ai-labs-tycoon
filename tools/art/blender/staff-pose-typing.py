#!/usr/bin/env python3
"""Typing pose at desk with monitor."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import staff_build as sb  # noqa: E402


def main():
    sb.build_pose_typing()
    entry = bc.finish_asset("staff-pose-typing", "staff-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)