#!/usr/bin/env python3
"""Fail if any .pyc or __pycache__ path is tracked in git."""
from __future__ import annotations

import subprocess
import sys


def main() -> int:
    out = subprocess.check_output(["git", "ls-files", "-z"], text=True, errors="replace")
    paths = [p for p in out.split("\0") if p]
    bad = [p for p in paths if "__pycache__" in p or p.endswith(".pyc")]
    if bad:
        for p in bad:
            print(f"FAIL: tracked bytecode {p}")
        print("Fix: git rm --cached <path> and ensure .gitignore has __pycache__/ and *.pyc")
        return 1
    print("OK: no tracked .pyc under git")
    return 0


if __name__ == "__main__":
    sys.exit(main())