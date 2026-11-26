#!/usr/bin/env python3
"""EarlyGameHudCatalog ids ↔ hud-manifest.json + wisp-manifest.json."""
import json
import re
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
HUD_MANIFEST = REPO / "unity/Assets/Art/EarlyGame/hud-manifest.json"
WISP_MANIFEST = REPO / "unity/Assets/Art/EarlyGame/wisp-manifest.json"
HUD_CATALOG = REPO / "unity/Assets/Scripts/Hq/EarlyGameHudCatalog.cs"
WISP_CATALOG = REPO / "unity/Assets/Scripts/Hq/EarlyGameWispCatalog.cs"


def ids_from_catalog(path: Path, array_name: str) -> list[str]:
    text = path.read_text(encoding="utf-8")
    block = re.search(rf"{array_name} = new\[\]\s*\{{([^}}]+)\}}", text, re.S)
    if not block:
        raise SystemExit(f"{array_name} block not found in {path}")
    return re.findall(r'"([^"]+)"', block.group(1))


def main() -> int:
    hud_ids = sorted(a["id"] for a in json.loads(HUD_MANIFEST.read_text())["assets"])
    cat_hud = sorted(ids_from_catalog(HUD_CATALOG, "HudIconIds"))
    if hud_ids != cat_hud:
        print("hud manifest-only:", set(hud_ids) - set(cat_hud))
        print("hud catalog-only:", set(cat_hud) - set(hud_ids))
        return 1
    wisp_ids = sorted(a["id"] for a in json.loads(WISP_MANIFEST.read_text())["assets"])
    cat_wisp = sorted(ids_from_catalog(WISP_CATALOG, "WispMoodIds"))
    if wisp_ids != cat_wisp:
        print("wisp manifest-only:", set(wisp_ids) - set(cat_wisp))
        print("wisp catalog-only:", set(cat_wisp) - set(wisp_ids))
        return 1
    print(f"OK: {len(cat_hud)} hud icons, {len(cat_wisp)} wisp moods in sync")
    return 0


if __name__ == "__main__":
    sys.exit(main())