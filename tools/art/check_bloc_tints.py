#!/usr/bin/env python3
"""Ensure world-bloc-tints.json blocs match CampusWorldMapTokens.KnownBlocIds."""
import json
import re
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
SRC = REPO / "docs/art/world-bloc-tints.json"
CS = REPO / "unity/Assets/Scripts/Hq/CampusWorldMapTokens.cs"


def ids_from_cs() -> list[str]:
    text = CS.read_text(encoding="utf-8")
    block = re.search(r"KnownBlocIds\s*=\s*\{([^}]+)\}", text, re.S)
    if not block:
        raise SystemExit("KnownBlocIds block not found")
    return re.findall(r'"([^"]+)"', block.group(1))


def main() -> int:
    blocs = sorted(json.loads(SRC.read_text(encoding="utf-8"))["blocs"].keys())
    known = sorted(ids_from_cs())
    if blocs != known:
        print("json blocs:", blocs)
        print("cs known:", known)
        return 1
    print(f"OK: {len(blocs)} bloc tints in sync")
    return 0


if __name__ == "__main__":
    sys.exit(main())