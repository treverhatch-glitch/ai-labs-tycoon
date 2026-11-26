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
FORBIDDEN_GLB_NAME_SUBSTRINGS = ("chest", "crate", "lid", "bin", "cardboard")


def _glb_json_chunk(glb_path: Path) -> dict:
    import struct

    raw = glb_path.read_bytes()
    if len(raw) < 20 or raw[:4] != b"glTF":
        raise ValueError("not a GLB")
    length = struct.unpack("<I", raw[8:12])[0]
    if length > len(raw):
        raise ValueError("truncated GLB")
    off = 12
    while off + 8 <= length:
        chunk_len, chunk_type = struct.unpack("<II", raw[off : off + 8])
        off += 8
        chunk = raw[off : off + chunk_len]
        off += chunk_len
        if chunk_type == 0x4E4F534A:
            return json.loads(chunk.decode("utf-8"))
    raise ValueError("GLB missing JSON chunk")


def glb_forbidden_name_errs(glb_path: Path) -> list[str]:
    errs: list[str] = []
    try:
        doc = _glb_json_chunk(glb_path)
    except (ValueError, json.JSONDecodeError) as exc:
        return [f"FAIL: cannot parse GLB JSON {glb_path}: {exc}"]
    names: set[str] = set()
    for node in doc.get("nodes") or []:
        if node.get("name"):
            names.add(str(node["name"]))
    for mesh in doc.get("meshes") or []:
        if mesh.get("name"):
            names.add(str(mesh["name"]))
    for low_name in names:
        low = low_name.lower()
        for sub in FORBIDDEN_GLB_NAME_SUBSTRINGS:
            if sub in low:
                errs.append(f"FAIL: GLB forbidden name {low_name!r} (matches {sub!r})")
                break
    return errs


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
    glb_errs = glb_forbidden_name_errs(GLB)
    if glb_errs:
        for e in glb_errs:
            print(e)
        return 1
    print(f"OK: CC0 hero {HERO_ID} ({tris} tris, {len(furnishings)} furnishings)")
    return 0


if __name__ == "__main__":
    sys.exit(main())