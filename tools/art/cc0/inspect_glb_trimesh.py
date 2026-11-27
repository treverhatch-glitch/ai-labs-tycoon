#!/usr/bin/env python3
"""Print mesh + material inventory from exported GLB (trimesh + JSON)."""
from __future__ import annotations

import json
import struct
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[3]


def _glb_json(glb_path: Path) -> dict:
    raw = glb_path.read_bytes()
    off = 12
    while off + 8 <= len(raw):
        chunk_len, chunk_type = struct.unpack_from("<II", raw, off)
        off += 8
        if chunk_type == 0x4E4F534A:
            return json.loads(raw[off : off + chunk_len])
        off += chunk_len
    raise ValueError(f"no JSON chunk in {glb_path}")


def _emissive_factor(material: dict) -> list[float] | None:
    ext = material.get("extensions") or {}
    kh = ext.get("KHR_materials_emissive_strength")
    base = material.get("emissiveFactor")
    if not base:
        return None
    if kh and "emissiveStrength" in kh:
        s = float(kh["emissiveStrength"])
        return [float(c) * s for c in base]
    return [float(c) for c in base]


def inspect_glb(glb_path: Path) -> None:
    import trimesh

    print(f"\n=== {glb_path.relative_to(REPO)} ===")
    doc = _glb_json(glb_path)
    scene = trimesh.load(glb_path, force="scene")
    geom = getattr(scene, "geometry", {}) or {}
    print(f"trimesh geometry count: {len(geom)}")
    for name, mesh in sorted(geom.items()):
        print(f"  mesh: {name!r}  faces={len(mesh.faces)}  verts={len(mesh.vertices)}")

    for i, mat in enumerate(doc.get("materials") or []):
        em = _emissive_factor(mat)
        print(f"  material[{i}]: name={mat.get('name')!r}  emissiveFactor={em}")

    mesh_nodes = []
    for node in doc.get("nodes") or []:
        if node.get("mesh") is not None:
            mesh_nodes.append(node.get("name"))
    if mesh_nodes:
        print("  glTF mesh nodes:", ", ".join(mesh_nodes))


def main(argv: list[str]) -> int:
    paths = [Path(a) for a in argv[1:]] if len(argv) > 1 else [
        REPO / "unity/Assets/Art/Cc0/compute-kit/compute-monitor-era1/compute-monitor-era1.glb",
        REPO / "unity/Assets/Art/Cc0/compute-kit/compute-server-rack-era1/compute-server-rack-era1.glb",
    ]
    for p in paths:
        if not p.is_file():
            print(f"MISSING: {p}", file=sys.stderr)
            return 1
        inspect_glb(p)
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv))