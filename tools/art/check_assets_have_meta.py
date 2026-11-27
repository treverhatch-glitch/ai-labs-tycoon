#!/usr/bin/env python3
"""Fail if any file under unity/Assets lacks a sibling .meta (folders use .meta inside folder)."""
from __future__ import annotations

import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2] / "unity" / "Assets"
SKIP = {".DS_Store", ".gitkeep"}


def meta_for(path: Path) -> Path:
    if path.is_dir():
        return path / ".meta"
    return path.with_suffix(path.suffix + ".meta")


def main() -> int:
    missing: list[str] = []
    for path in sorted(ROOT.rglob("*")):
        if path.name in SKIP or path.name.endswith(".meta"):
            continue
        if not meta_for(path).is_file():
            missing.append(path.relative_to(ROOT).as_posix())
    if missing:
        for m in missing[:40]:
            print(f"FAIL: missing meta for Assets/{m}")
        if len(missing) > 40:
            print(f"FAIL: ... and {len(missing) - 40} more")
        return 1
    print("OK: every Assets file/folder has .meta")
    return 0


if __name__ == "__main__":
    sys.exit(main())