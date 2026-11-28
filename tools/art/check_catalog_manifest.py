#!/usr/bin/env python3
"""Ensure CampusPropCatalog.AllAssetIds matches manifest.json."""
import json
import re
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
MANIFEST = REPO / "unity/Assets/Art/Generated/manifest.json"
CATALOG = REPO / "unity/Assets/Scripts/Hq/CampusPropCatalog.cs"


def ids_from_catalog() -> list[str]:
    text = CATALOG.read_text(encoding="utf-8")
    block = re.search(r"AllAssetIds = new\[\]\s*\{([^}]+)\}", text, re.S)
    if not block:
        raise SystemExit("AllAssetIds block not found")
    return re.findall(r'"([^"]+)"', block.group(1))


def main() -> int:
    manifest_ids = sorted(a["id"] for a in json.loads(MANIFEST.read_text())["assets"])
    catalog_ids = sorted(ids_from_catalog())
    if manifest_ids != catalog_ids:
        only_m = set(manifest_ids) - set(catalog_ids)
        only_c = set(catalog_ids) - set(manifest_ids)
        print("manifest-only:", sorted(only_m))
        print("catalog-only:", sorted(only_c))
        return 1
    print(f"OK: {len(catalog_ids)} props in sync")
    return 0


if __name__ == "__main__":
    sys.exit(main())