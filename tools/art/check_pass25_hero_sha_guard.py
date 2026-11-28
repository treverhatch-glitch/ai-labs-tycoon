#!/usr/bin/env python3
"""Pass25: refuse push if desk-crt / roll-door heroes still match pass23 bytes."""
from __future__ import annotations

import hashlib
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"

FORBIDDEN = {
    "garage-desk-crt.png": "fe18d20330c32f094153cabf28d3a693ba07fd70d1a6cef824bc2bb2ff0f9f6f",
    "garage-roll-door.png": "4fd0bbd3fdf6a89eb1a97b3b2770c85f7e572c30ab9791c3a14f0fa7aa6cf5ae",
}


def sha256(path: Path) -> str:
    h = hashlib.sha256()
    h.update(path.read_bytes())
    return h.hexdigest()


def main() -> int:
    rc = 0
    for name, bad in FORBIDDEN.items():
        path = PREVIEW / name
        if not path.is_file():
            print(f"FAIL missing {path}")
            rc = 1
            continue
        digest = sha256(path)
        if digest == bad:
            print(f"FAIL {name} still pass23 hero sha256 {bad[:16]}… — re-render before push")
            rc = 1
        else:
            print(f"OK {name} sha256={digest}")
    return rc


if __name__ == "__main__":
    sys.exit(main())