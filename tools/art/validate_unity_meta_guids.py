#!/usr/bin/env python3
"""Fail if any Unity .meta guid is not exactly 32 lowercase hex chars."""
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2] / "unity" / "Assets"
PATTERN = re.compile(r"^guid:\s*([0-9a-f]+)\s*$", re.MULTILINE)


def main() -> int:
    bad: list[str] = []
    for meta in ROOT.rglob("*.meta"):
        text = meta.read_text(encoding="utf-8", errors="replace")
        m = PATTERN.search(text)
        if not m:
            bad.append(f"{meta}: no guid")
            continue
        g = m.group(1)
        if len(g) != 32:
            bad.append(f"{meta}: guid length {len(g)} ({g})")
    if bad:
        for line in bad:
            print(line)
        return 1
    print(f"OK: {sum(1 for _ in ROOT.rglob('*.meta'))} meta files")
    return 0


if __name__ == "__main__":
    sys.exit(main())