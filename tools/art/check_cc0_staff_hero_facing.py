#!/usr/bin/env python3
"""Run Blender facing gate for founder + researcher."""
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = os.environ.get("BLENDER_BIN", "blender")
SCRIPT = REPO / "tools/art/blender/check_cc0_staff_hero_facing_blender.py"


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
    if proc.returncode != 0:
        if proc.stderr:
            print(proc.stderr, file=sys.stderr)
        return 1
    json_path = REPO / "docs/art/previews/staff-hero-facing-metrics.json"
    if not json_path.is_file():
        print("FAIL facing missing_metrics_json")
        return 1
    return 0


if __name__ == "__main__":
    raise SystemExit(main())