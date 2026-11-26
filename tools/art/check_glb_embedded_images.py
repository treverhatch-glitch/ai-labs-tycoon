#!/usr/bin/env python3
"""Gate: BlenderBaked GLBs embed at least one baseColor image (buffer or uri)."""
from __future__ import annotations

import json
import struct
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
GLB = REPO / "unity" / "Assets" / "Art" / "BlenderBaked" / "prop-desk" / "mesh_prop-desk.glb"


def _gltf_json(path: Path) -> dict:
    data = path.read_bytes()
    if data[:4] != b"glTF":
        raise ValueError(f"not GLB: {path}")
    off = 12
    while off + 8 <= len(data):
        chunk_len, chunk_type = struct.unpack_from("<II", data, off)
        off += 8
        chunk = data[off : off + chunk_len]
        off += chunk_len
        if chunk_type == 0x4E4F534A:
            return json.loads(chunk)
    raise ValueError("no JSON chunk")


def main() -> int:
    if not GLB.is_file():
        print(f"FAIL: missing {GLB}", file=sys.stderr)
        return 1
    js = _gltf_json(GLB)
    images = js.get("images", [])
    if not images:
        print("FAIL: prop-desk GLB has no embedded images", file=sys.stderr)
        return 1
    embedded = 0
    for img in images:
        if img.get("bufferView") is not None or img.get("uri"):
            embedded += 1
    if embedded < 1:
        print("FAIL: no bufferView/uri images in prop-desk GLB", file=sys.stderr)
        return 1
    mats = js.get("materials", [])
    textured = any(
        "baseColorTexture" in (m.get("pbrMetallicRoughness") or {})
        for m in mats
    )
    if not textured:
        print("FAIL: prop-desk GLB materials missing baseColorTexture", file=sys.stderr)
        return 1
    print(f"OK: prop-desk GLB embeds {embedded} image(s), {len(mats)} material(s)")
    return 0


if __name__ == "__main__":
    sys.exit(main())