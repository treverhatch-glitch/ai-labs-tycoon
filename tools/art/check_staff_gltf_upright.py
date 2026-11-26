#!/usr/bin/env python3
"""Run Blender glTF import upright gate on committed staff GLBs."""
from __future__ import annotations

import shutil
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = Path(__import__("os").environ.get("BLENDER_BIN", "blender"))
SCRIPT = REPO / "tools/art/blender/check_staff_gltf_blender.py"


def main() -> int:
    if not shutil.which(str(BLENDER)) and not BLENDER.is_file():
        print("FAIL blender not found", file=sys.stderr)
        return 1
    proc = subprocess.run(
        [str(BLENDER), "-b", "--python", str(SCRIPT), "--", str(REPO)],
        cwd=REPO,
        text=True,
        capture_output=True,
    )
    if proc.stdout:
        print(proc.stdout, end="")
    if proc.stderr:
        print(proc.stderr, end="", file=sys.stderr)
    return proc.returncode


if __name__ == "__main__":
    sys.exit(main())