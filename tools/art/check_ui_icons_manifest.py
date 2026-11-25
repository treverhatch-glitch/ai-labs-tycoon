#!/usr/bin/env python3
"""Ensure CampusIconCatalog.AllIconIds matches ui-icons-manifest.json."""
import json
import re
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
MANIFEST = REPO / "unity/Assets/Art/Generated/ui-icons-manifest.json"
CS = REPO / "unity/Assets/Scripts/Hq/CampusIconCatalog.cs"


def ids_from_catalog() -> list[str]:
    text = CS.read_text(encoding="utf-8")
    block = re.search(r"AllIconIds\s*=\s*\{([^}]+)\}", text, re.S)
    if not block:
        raise SystemExit("AllIconIds block not found")
    return re.findall(r'"([^"]+)"', block.group(1))


def main() -> int:
    manifest_ids = sorted(i["id"] for i in json.loads(MANIFEST.read_text(encoding="utf-8"))["icons"])
    catalog_ids = sorted(ids_from_catalog())
    if manifest_ids != catalog_ids:
        print("manifest-only:", sorted(set(manifest_ids) - set(catalog_ids)))
        print("catalog-only:", sorted(set(catalog_ids) - set(manifest_ids)))
        return 1
    print(f"OK: {len(catalog_ids)} icons in sync")
    return 0


if __name__ == "__main__":
    sys.exit(main())