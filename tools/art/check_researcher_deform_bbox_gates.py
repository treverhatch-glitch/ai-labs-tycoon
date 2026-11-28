#!/usr/bin/env python3
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = os.environ.get("BLENDER_BIN", "blender")
SCRIPT = REPO / "tools/art/blender/check_researcher_deform_bbox_gates_blender.py"


def main() -> int:
    proc = subprocess.run(
        [BLENDER, "-b", "--python", str(SCRIPT)],
        cwd=REPO,
        text=True,
        capture_output=True,
    )
    for line in (proc.stdout or "").splitlines():
        if line.startswith(("OK ", "FAIL ")):
            print(line)
    if proc.returncode != 0 and proc.stderr:
        print(proc.stderr, file=sys.stderr)
    return proc.returncode


if __name__ == "__main__":
    raise SystemExit(main())