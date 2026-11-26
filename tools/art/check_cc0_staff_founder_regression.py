#!/usr/bin/env python3
"""Regression: legacy bake + headstand PNG fixtures must fail upright gates."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = Path(__import__("os").environ.get("BLENDER_BIN", "blender"))
BAKE_SCRIPT = REPO / "tools/art/blender/check_cc0_staff_founder_bake_regression_blender.py"

FIXTURES = (
    REPO / "tools/art/fixtures/staff-founder-idle-headstand-e3b3c52d.png",
    REPO / "tools/art/fixtures/staff-founder-idle-headstand-e164f581.png",
)

sys.path.insert(0, str(REPO / "tools/art"))
from check_cc0_staff_founder_preview_silhouette import analyze as png_analyze  # noqa: E402


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
        return 1
    for png in FIXTURES:
        if not png.is_file():
            print(f"FAIL staff-founder-idle missing fixture {png.name}")
            return 1
        ok, msg = png_analyze(png)
        if ok:
            print(f"FAIL staff-founder-idle fixture_passed {png.name}: {msg}")
            return 1
        if "headstand" not in msg and "head" not in msg:
            print(f"FAIL staff-founder-idle fixture_unexpected {png.name}: {msg}")
            return 1
        print(f"OK staff-founder-idle fixture_rejected {png.name}: {msg}")
    return 0


if __name__ == "__main__":
    sys.exit(main())