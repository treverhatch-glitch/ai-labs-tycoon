#!/usr/bin/env python3
"""CC0 hero: manifest ↔ GLB ↔ previews."""
import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-cc0-manifest.json"
HERO_ID = "garage-interior-room-kit"
GLB = REPO / "unity/Assets/Art/EarlyGame/cc0-garage-kit" / f"{HERO_ID}.glb"
PREVIEW = REPO / "docs/art/previews" / f"{HERO_ID}.png"
LICENSES = REPO / "docs/art/LICENSES.md"


def main() -> int:
    if not MANIFEST.is_file():
        print("FAIL: missing manifest", MANIFEST)
        return 1
    assets = json.loads(MANIFEST.read_text(encoding="utf-8")).get("assets", [])
    hero = next((a for a in assets if a.get("id") == HERO_ID), None)
    if not hero or not hero.get("hero"):
        print("FAIL: hero entry missing in manifest")
        return 1
    if not GLB.is_file() or GLB.stat().st_size < 4096:
        print("FAIL: missing or tiny GLB", GLB)
        return 1
    if not PREVIEW.is_file() or PREVIEW.stat().st_size < 1024:
        print("FAIL: missing preview", PREVIEW)
        return 1
    if not LICENSES.is_file():
        print("FAIL: missing LICENSES", LICENSES)
        return 1
    tris = hero.get("triangleCount", 0)
    if tris < 200:
        print(f"FAIL: triangleCount {tris} < 200 (not Kenney-derived?)")
        return 1
    sources = hero.get("sources") or []
    allowed = {"garage-kit", "kenney_building-kit", "kenney_furniture-kit"}
    if not allowed.intersection(sources):
        print("FAIL: manifest missing Kenney CC0 kit source", sources)
        return 1
    furnishings = hero.get("furnishings") or []
    if len(furnishings) < 6:
        print(f"FAIL: furnishings {len(furnishings)} < 6", furnishings)
        return 1
    print(f"OK: CC0 hero {HERO_ID} ({tris} tris, {len(furnishings)} furnishings)")
    return 0


if __name__ == "__main__":
    sys.exit(main())