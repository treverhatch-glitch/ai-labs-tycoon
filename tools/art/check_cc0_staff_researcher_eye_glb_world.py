#!/usr/bin/env python3
"""Researcher GLB: eye meshes on head (reuses founder import gate)."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
SCRIPT = REPO / "tools/art/blender/check_cc0_staff_researcher_eye_glb_world_blender.py"
BLENDER = Path(__import__("os").environ.get("BLENDER_BIN", "blender"))


def main() -> int:
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