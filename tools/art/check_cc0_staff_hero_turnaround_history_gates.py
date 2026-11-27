#!/usr/bin/env python3
"""Turnaround gates must FAIL on known-bad SHAs."""
from __future__ import annotations

import subprocess
import sys
import tempfile
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
HISTORIC = (
    ("c06d0a57", "staff-founder-turnaround"),
    ("c06d0a57", "staff-researcher-turnaround"),
)


def main() -> int:
    failures = 0
    for sha, stem in HISTORIC:
        with tempfile.NamedTemporaryFile(suffix=".png", delete=False) as tmp:
            tmp_path = Path(tmp.name)
        proc = subprocess.run(
            ["git", "show", f"{sha}:docs/art/previews/{stem}.png"],
            cwd=REPO,
            capture_output=True,
        )
        if proc.returncode != 0:
            print(f"FAIL turnaround_history {sha} {stem} missing_blob")
            failures += 1
            continue
        tmp_path.write_bytes(proc.stdout)
        sys.path.insert(0, str(REPO / "tools" / "art"))
        from check_cc0_staff_hero_turnaround import _check

        asset = "staff-founder-idle" if "founder" in stem else "staff-researcher-idle"
        ok, msg = _check(tmp_path, stem, asset)
        tmp_path.unlink(missing_ok=True)
        if ok:
            print(f"FAIL turnaround_history {sha} {stem} want_fail got_pass {msg}")
            failures += 1
        else:
            print(f"OK turnaround_history {sha} {stem} gate_fail {msg}")
    return 1 if failures else 0


if __name__ == "__main__":
    raise SystemExit(main())