#!/usr/bin/env python3
"""Fail if git tracks bytecode under tools/art."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]


def main() -> int:
    out = subprocess.check_output(
        ["git", "ls-files", "tools/art"],
        cwd=REPO,
        text=True,
    )
    bad = [ln for ln in out.splitlines() if ln.endswith(".pyc") or "/__pycache__/" in ln]
    if bad:
        print("FAIL: tracked bytecode under tools/art:")
        for ln in bad:
            print(" ", ln)
        return 1
    print("OK: zero tracked .pyc under tools/art")
    return 0


if __name__ == "__main__":
    sys.exit(main())