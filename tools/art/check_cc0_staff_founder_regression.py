#!/usr/bin/env python3
"""Regression: legacy Kenney bake orientation must fail screen-up."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = Path(__import__("os").environ.get("BLENDER_BIN", "blender"))
BAKE_SCRIPT = REPO / "tools/art/blender/check_cc0_staff_founder_bake_regression_blender.py"


def main() -> int:
    proc = subprocess.run(
        [str(BLENDER), "-b", "--python", str(BAKE_SCRIPT), "--", str(REPO)],
        cwd=REPO,
        text=True,
        capture_output=True,
    )
    if proc.stdout:
        for line in proc.stdout.splitlines():
            if line.startswith("OK ") or line.startswith("FAIL "):
                print(line)
    if proc.returncode != 0:
        if proc.stderr:
            print(proc.stderr, file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())