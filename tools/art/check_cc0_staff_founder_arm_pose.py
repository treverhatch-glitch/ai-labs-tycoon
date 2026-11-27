#!/usr/bin/env python3
"""Founder GLB arm-idle pose gate."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = Path(__import__("os").environ.get("BLENDER_BIN", "blender"))
SCRIPT = REPO / "tools/art/blender/check_cc0_staff_founder_arm_pose_blender.py"


def main() -> int:
    preview_py = REPO / "tools/art/check_cc0_staff_founder_arm_preview.py"
    prev = subprocess.run([sys.executable, str(preview_py)], cwd=REPO, text=True, capture_output=True)
    if prev.stdout:
        for line in prev.stdout.splitlines():
            if line.startswith("OK ") or line.startswith("FAIL "):
                print(line)
    if prev.returncode != 0:
        return prev.returncode
    proc = subprocess.run(
        [str(BLENDER), "-b", "--python", str(SCRIPT), "--", str(REPO)],
        cwd=REPO,
        text=True,
        capture_output=True,
    )
    if proc.stdout:
        for line in proc.stdout.splitlines():
            if line.startswith("OK ") or line.startswith("FAIL "):
                print(line)
    return proc.returncode


if __name__ == "__main__":
    sys.exit(main())