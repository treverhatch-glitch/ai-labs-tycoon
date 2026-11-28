#!/usr/bin/env python3
"""Render walk strip (Blender) then validate PNG."""
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = os.environ.get("BLENDER_BIN", "blender")
RENDER = REPO / "tools/art/blender/render_kenney_staff_walk_strip.py"
CHECK = REPO / "tools/art/check_cc0_staff_hero_walk_strip.py"


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: check_cc0_staff_hero_walk_cycle.py <staff-founder-idle|staff-researcher-idle>")
        return 1
    asset = sys.argv[1]
    env = os.environ.copy()
    env.setdefault("STAFF_PREVIEW_YAW_DEG", "30")
    env.setdefault("STAFF_PREVIEW_ELEV_DEG", "10")
    proc = subprocess.run(
        [BLENDER, "-b", "--python", str(RENDER), "--", asset],
        cwd=REPO,
        text=True,
        capture_output=True,
        env=env,
    )
    for line in (proc.stdout or "").splitlines():
        if line.startswith(("OK ", "FAIL ")):
            print(line)
    if proc.returncode != 0:
        if proc.stderr:
            print(proc.stderr, file=sys.stderr)
        return 1
    return subprocess.run([sys.executable, str(CHECK), asset], cwd=REPO).returncode


if __name__ == "__main__":
    raise SystemExit(main())