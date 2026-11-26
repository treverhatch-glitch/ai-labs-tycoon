#!/usr/bin/env python3
"""Rebuild garage/staff manifests and sync EarlyGameGarageCatalog ids from .glb files."""
from __future__ import annotations

import json
import re
from pathlib import Path

REPO = Path(__file__).resolve().parents[3]
GARAGE = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
STAFF = REPO / "unity/Assets/Art/EarlyGame/staff-kit"
CATALOG_G = REPO / "unity/Assets/Scripts/Hq/EarlyGameGarageCatalog.cs"
CATALOG_S = REPO / "unity/Assets/Scripts/Hq/EarlyGameStaffCatalog.cs"


def _prior_entries(kit_dir: Path) -> dict[str, dict]:
    name = "garage-manifest.json" if kit_dir.name == "garage-kit" else "staff-manifest.json"
    path = REPO / "unity/Assets/Art/EarlyGame" / name
    if not path.exists():
        return {}
    return {a["id"]: a for a in json.loads(path.read_text()).get("assets", [])}


def collect(kit_dir: Path) -> list[dict]:
    prior = _prior_entries(kit_dir)
    entries = []
    for glb in sorted(kit_dir.glob("*.glb")):
        aid = glb.stem
        tex_base = kit_dir / "textures" / aid
        textures = {}
        if tex_base.is_dir():
            for kind in ("albedo", "normal", "roughness"):
                p = tex_base / f"{aid}_{kind}.png"
                if p.exists():
                    textures[kind] = f"Assets/Art/EarlyGame/{kit_dir.name}/textures/{aid}/{aid}_{kind}.png"
        entry = {
            "id": aid,
            "path": f"Assets/Art/EarlyGame/{kit_dir.name}/{aid}.glb",
            "textures": textures,
            "preview": f"docs/art/previews/{aid}.png",
            "license": "CC0-1.0 (Blender-baked, in-repo)",
        }
        old = prior.get(aid, {})
        for key in (
            "vertexCount",
            "triangleCount",
            "subShapeCount",
            "meshIslands",
            "grounded",
            "blenderVersion",
            "generator",
        ):
            if key in old:
                entry[key] = old[key]
        entries.append(entry)
    return entries


def patch_catalog(path: Path, ids: list[str], root_suffix: str) -> None:
    text = path.read_text(encoding="utf-8")
    block = ",\n            ".join(f'"{i}"' for i in ids)
    text = re.sub(
        r'AllAssetIds = new\[\]\s*\{[^}]+\}',
        f"AllAssetIds = new[]\n        {{\n            {block},\n        }}",
        text,
        count=1,
        flags=re.S,
    )
    text = re.sub(
        r'const string Root = "[^"]+";',
        f'const string Root = "Assets/Art/EarlyGame/{root_suffix}/";',
        text,
        count=1,
    )
    text = re.sub(
        r"\.obj",
        ".glb",
        text,
    )
    path.write_text(text, encoding="utf-8")


def main() -> int:
    garage = collect(GARAGE)
    staff = collect(STAFF)
    (REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json").write_text(
        json.dumps({"fidelity": "blender-baked", "assets": garage}, indent=2) + "\n", encoding="utf-8"
    )
    (REPO / "unity/Assets/Art/EarlyGame/staff-manifest.json").write_text(
        json.dumps({"fidelity": "blender-baked", "assets": staff}, indent=2) + "\n", encoding="utf-8"
    )
    if garage:
        patch_catalog(CATALOG_G, [e["id"] for e in garage], "garage-kit")
    if staff:
        patch_catalog(CATALOG_S, [e["id"] for e in staff], "staff-kit")
    print(f"OK manifests garage={len(garage)} staff={len(staff)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())