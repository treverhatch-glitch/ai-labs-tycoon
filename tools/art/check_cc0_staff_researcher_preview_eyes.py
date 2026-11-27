#!/usr/bin/env python3
"""Founder: eyes project into preview camera frustum."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = Path(__import__("os").environ.get("BLENDER_BIN", "blender"))
SCRIPT = REPO / "tools/art/blender/check_cc0_staff_researcher_preview_eyes_blender.py"


def main() -> int:
    proc = subprocess.run(
        [str(BLENDER), "-b", "--python", str(SCRIPT), "--", str(REPO)],
        cwd=REPO,
        text=True,
        capture_output=True,
    )
    for line in (proc.stdout or "").splitlines():
        if line.startswith("OK ") or line.startswith("FAIL "):
            print(line)
    return proc.returncode if proc.returncode else (0 if "OK " in (proc.stdout or "") else 1)


if __name__ == "__main__":
    sys.exit(main())