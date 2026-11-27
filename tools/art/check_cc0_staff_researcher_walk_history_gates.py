#!/usr/bin/env python3
"""Walk-strip gates must FAIL on historical researcher SHAs (regression proof)."""
from __future__ import annotations

import subprocess
import sys
import tempfile
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
CHECK = REPO / "tools/art/check_cc0_staff_hero_walk_strip.py"
HISTORIC = ("8885108d", "4ba51dd5", "4a2b3261")


def main() -> int:
    failures = 0
    for sha in HISTORIC:
        png = REPO / "docs/art/previews/staff-researcher-walk-strip.png"
        with tempfile.NamedTemporaryFile(suffix=".png", delete=False) as tmp:
            tmp_path = Path(tmp.name)
        proc = subprocess.run(
            ["git", "show", f"{sha}:docs/art/previews/staff-researcher-walk-strip.png"],
            cwd=REPO,
            capture_output=True,
        )
        if proc.returncode != 0:
            print(f"FAIL history {sha} missing_strip_blob")
            failures += 1
            continue
        tmp_path.write_bytes(proc.stdout)
        chk = subprocess.run(
            [sys.executable, str(CHECK), "staff-researcher-idle", "--png", str(tmp_path)],
            cwd=REPO,
            capture_output=True,
            text=True,
        )
        line = (chk.stdout or chk.stderr or "").strip().splitlines()
        summary = line[-1] if line else f"exit={chk.returncode}"
        if chk.returncode == 0:
            print(f"FAIL history {sha} want_fail got_pass {summary}")
            failures += 1
        else:
            print(f"OK history {sha} gate_fail {summary}")
        tmp_path.unlink(missing_ok=True)
    return 1 if failures else 0


if __name__ == "__main__":
    raise SystemExit(main())