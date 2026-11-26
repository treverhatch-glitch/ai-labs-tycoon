#!/usr/bin/env python3
"""Run headless Blender bakes for listed campus-kit assets."""
from __future__ import annotations

import shutil
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = shutil.which("blender")
SCRIPTS = REPO / "tools" / "art" / "blender"


def run_asset(script_name: str) -> None:
    script = SCRIPTS / script_name
    if not script.is_file():
        raise FileNotFoundError(script)
    if not BLENDER:
        print("FAIL: blender not on PATH", file=sys.stderr)
        raise SystemExit(1)
    cmd = [BLENDER, "--background", "--python", str(script)]
    print("+", " ".join(cmd), flush=True)
    subprocess.run(cmd, cwd=REPO, check=True)


def main(argv: list[str]) -> None:
    if not argv:
        argv = [
            "prop_desk.py",
            "ui_rival_badge.py",
            "prop_whiteboard.py",
            "prop_server_rack.py",
        ]
    for name in argv:
        run_asset(name)


if __name__ == "__main__":
    main(sys.argv[1:])