#!/usr/bin/env python3
"""EarlyGameGarageCatalog ids ↔ garage-manifest.json."""
import json
import re
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"
CATALOG = REPO / "unity/Assets/Scripts/Hq/EarlyGameGarageCatalog.cs"
STAFF_MANIFEST = REPO / "unity/Assets/Art/EarlyGame/staff-manifest.json"
STAFF_CATALOG = REPO / "unity/Assets/Scripts/Hq/EarlyGameStaffCatalog.cs"


def ids_from_catalog(path: Path) -> list[str]:
    text = path.read_text(encoding="utf-8")
    block = re.search(r"AllAssetIds = new\[\]\s*\{([^}]+)\}", text, re.S)
    if not block:
        raise SystemExit(f"AllAssetIds block not found in {path}")
    return re.findall(r'"([^"]+)"', block.group(1))


def sync(manifest_path: Path, catalog_path: Path, label: str, kit_subdir: str) -> int:
    manifest = json.loads(manifest_path.read_text())["assets"]
    manifest_ids = sorted(a["id"] for a in manifest)
    catalog_ids = sorted(ids_from_catalog(catalog_path))
    if manifest_ids != catalog_ids:
        print(f"{label} manifest-only:", sorted(set(manifest_ids) - set(catalog_ids)))
        print(f"{label} catalog-only:", sorted(set(catalog_ids) - set(manifest_ids)))
        return 1
    art_root = REPO / "unity/Assets/Art/EarlyGame" / kit_subdir
    missing = [a["id"] for a in manifest if not (art_root / f"{a['id']}.glb").exists()]
    if missing:
        print(f"{label} missing .glb:", missing)
        return 1
    print(f"OK: {len(catalog_ids)} {label} assets in sync")
    return 0


def main() -> int:
    rc = 0
    rc |= sync(MANIFEST, CATALOG, "garage", "garage-kit")
    rc |= sync(STAFF_MANIFEST, STAFF_CATALOG, "staff", "staff-kit")
    return rc


if __name__ == "__main__":
    sys.exit(main())