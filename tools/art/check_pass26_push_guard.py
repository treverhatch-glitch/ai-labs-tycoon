#!/usr/bin/env python3
"""Pass26: pass25 hero-sha guard + pixel gates (run before push)."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]


def main() -> int:
    rc = 0
    for script in (
        "check_pass25_hero_sha_guard.py",
        "check_pass26_garage_pixel_gates.py",
    ):
        p = subprocess.run([sys.executable, str(REPO / "tools/art" / script)], cwd=REPO)
        rc |= p.returncode
    return rc


if __name__ == "__main__":
    sys.exit(main())