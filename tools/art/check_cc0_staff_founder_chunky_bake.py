#!/usr/bin/env python3
"""Founder GLB: chunky Kenney bake gates (clothing enclosure + head proportion)."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = Path(__import__("os").environ.get("BLENDER_BIN", "blender"))
GLB = REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.glb"
SCRIPT = REPO / "tools/art/blender/check_cc0_staff_founder_chunky_bake_blender.py"


def main() -> int:
    if not GLB.is_file():
        print("FAIL staff-founder-idle missing glb")
        return 1
    proc = subprocess.run(
        [str(BLENDER), "-b", "--python", str(SCRIPT), "--", str(GLB)],
        cwd=REPO,
        text=True,
        capture_output=True,
    )
    for line in (proc.stdout or "").splitlines():
        if line.startswith("OK ") or line.startswith("FAIL "):
            print(line)
    return proc.returncode if proc.returncode else 0


if __name__ == "__main__":
    sys.exit(main())