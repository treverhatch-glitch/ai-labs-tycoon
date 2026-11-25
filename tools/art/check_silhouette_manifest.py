#!/usr/bin/env python3
"""Ensure CampusSilhouetteCatalog.Supported matches rival-silhouettes-manifest.json."""
import json
import re
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
MANIFEST = REPO / "unity/Assets/Art/Generated/rival-silhouettes-manifest.json"
CS = REPO / "unity/Assets/Scripts/Hq/CampusSilhouetteCatalog.cs"


def ids_from_cs() -> list[str]:
    text = CS.read_text(encoding="utf-8")
    block = re.search(r"Supported\s*=\s*\{([^}]+)\}", text, re.S)
    if not block:
        raise SystemExit("Supported block not found")
    return re.findall(r'"([^"]+)"', block.group(1))


def main() -> int:
    manifest_ids = sorted(s["id"] for s in json.loads(MANIFEST.read_text(encoding="utf-8"))["silhouettes"])
    supported = sorted(ids_from_cs())
    if manifest_ids != supported:
        print("manifest-only:", sorted(set(manifest_ids) - set(supported)))
        print("cs-only:", sorted(set(supported) - set(manifest_ids)))
        return 1
    print(f"OK: {len(supported)} silhouettes in sync")
    return 0


if __name__ == "__main__":
    sys.exit(main())