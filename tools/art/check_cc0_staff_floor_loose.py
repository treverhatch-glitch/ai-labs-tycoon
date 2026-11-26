#!/usr/bin/env python3
"""Reject hero staff GLBs with mesh islands sitting on the floor away from feet."""
from __future__ import annotations

import json
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
HERO_IDS = ("staff-founder-idle", "staff-researcher-idle")
FLOOR_Y = 0.09
MAX_FLOOR_H = 0.14
MIN_FOOT_AWAY = 0.18


def _parse_bbox(ob: dict) -> tuple[tuple[float, float, float], tuple[float, float, float]]:
    mi = ob.get("min", [0, 0, 0])
    ma = ob.get("max", [0, 0, 0])
    return (tuple(mi), tuple(ma))


def main() -> int:
    script = REPO / "tools/art/blender/check_staff_floor_loose_blender.py"
    rc = 0
    for aid in HERO_IDS:
        glb = REPO / "unity/Assets/Art/EarlyGame/staff-kit" / f"{aid}.glb"
        if not glb.is_file():
            print(f"FAIL {aid} missing_glb")
            rc = 1
            continue
        out = subprocess.run(
            ["blender", "-b", "--python", str(script), "--", str(glb), aid],
            capture_output=True,
            text=True,
            check=False,
        )
        line = ""
        for raw in (out.stdout + out.stderr).splitlines():
            if raw.startswith(f"OK {aid}") or raw.startswith(f"FAIL {aid}"):
                line = raw
        print(line or f"FAIL {aid} no_checker_output")
        if line.startswith("FAIL"):
            rc = 1
    return rc


if __name__ == "__main__":
    raise SystemExit(main())