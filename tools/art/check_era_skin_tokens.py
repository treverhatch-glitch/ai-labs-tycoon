#!/usr/bin/env python3
"""Ensure era-skin-tokens.json eras match CampusEraSkinTokens.DocumentedEraIds."""
import json
import re
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
SRC = REPO / "docs/art/era-skin-tokens.json"
CS = REPO / "unity/Assets/Scripts/Hq/CampusEraSkinTokens.cs"


def ids_from_cs() -> list[str]:
    text = CS.read_text(encoding="utf-8")
    block = re.search(r"DocumentedEraIds\s*=\s*\{([^}]+)\}", text, re.S)
    if not block:
        raise SystemExit("DocumentedEraIds block not found")
    return re.findall(r'"([^"]+)"', block.group(1))


def main() -> int:
    eras = sorted(json.loads(SRC.read_text(encoding="utf-8"))["eras"].keys())
    documented = sorted(ids_from_cs())
    if eras != documented:
        print("json eras:", eras)
        print("cs documented:", documented)
        return 1
    print(f"OK: {len(eras)} era skins in sync")
    return 0


if __name__ == "__main__":
    sys.exit(main())