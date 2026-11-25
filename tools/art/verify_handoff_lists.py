#!/usr/bin/env python3
"""Sanity-check list_* scripts match expected counts (stdout only)."""
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
EXPECTED = {"props": 30, "icons": 10, "silhouettes": 4}


def count(cmd: list[str]) -> int:
    r = subprocess.run(cmd, cwd=REPO, capture_output=True, text=True, check=True)
    return len([ln for ln in r.stdout.splitlines() if ln.strip()])


def main() -> int:
    checks = [
        ("props", [sys.executable, "tools/art/list_catalog_ids.py"], EXPECTED["props"]),
        ("icons", [sys.executable, "tools/art/list_icon_ids.py"], EXPECTED["icons"]),
        ("silhouettes", [sys.executable, "tools/art/list_silhouette_ids.py"], EXPECTED["silhouettes"]),
    ]
    ok = True
    for name, cmd, want in checks:
        got = count(cmd)
        if got != want:
            print(f"FAIL {name}: got {got} want {want}")
            ok = False
        else:
            print(f"OK: {name} list {got}")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())