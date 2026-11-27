#!/usr/bin/env python3
"""Staff GLB must be Y-up: height dominant, grounded, head above feet."""
from __future__ import annotations

import json
import os
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = os.environ.get("BLENDER_BIN", "blender")

def main() -> int:
    script = REPO / "tools/art/blender/check_staff_upright_blender.py"
    proc = subprocess.run(
        [BLENDER, "-b", "--python-exit-code", "1", "--python", str(script), "--", str(REPO)],
        capture_output=True,
        text=True,
    )
    print(proc.stdout, end="")
    if proc.stderr:
        print(proc.stderr, file=sys.stderr, end="")
    return proc.returncode


if __name__ == "__main__":
    sys.exit(main())