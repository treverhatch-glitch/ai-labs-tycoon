#!/usr/bin/env python3
"""Print mesh + material inventory from exported GLB (trimesh + JSON). Gate with --gate."""
from __future__ import annotations

import argparse
import json
import struct
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[3]

MONITOR_GLB = REPO / "unity/Assets/Art/Cc0/compute-kit/compute-monitor-era1/compute-monitor-era1.glb"
RACK_GLB = REPO / "unity/Assets/Art/Cc0/compute-kit/compute-server-rack-era1/compute-server-rack-era1.glb"

REQUIRED_MONITOR_NODES = frozenset({"body_tapered", "screen", "power_led", "power_button"})


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


def _node_mesh_names(doc: dict) -> list[str]:
    return [str(n.get("name")) for n in doc.get("nodes") or [] if n.get("mesh") is not None]


def _mesh_table_names(doc: dict) -> list[str]:
    return [str(m.get("name", "")) for m in doc.get("meshes") or []]


def gate_rack(names: set[str]) -> list[str]:
    errs: list[str] = []
    if "rack_frame" not in names:
        errs.append("missing rack_frame")
    if not any("drive_bay" in n for n in names):
        errs.append("missing unit_4u_drive_bay_*")
    if not any("vent_" in n for n in names):
        errs.append("missing unit_2u_vent_*")
    if not any("port_led" in n for n in names):
        errs.append("missing port_led_*")
    if not any("side_rail" in n for n in names):
        errs.append("missing side_rail_*")
    for unit in ("unit_1u", "unit_2u_a", "unit_2u_b", "unit_4u"):
        if unit not in names:
            errs.append(f"missing {unit}")
    return errs


def gate_monitor(names: set[str], mats: list[dict]) -> list[str]:
    errs: list[str] = []
    missing = REQUIRED_MONITOR_NODES - names
    if missing:
        errs.append(f"missing nodes: {sorted(missing)}")
    led = next((m for m in mats if m.get("name") == "M_compute-monitor-era1_led"), None)
    if not led or not _emissive_factor(led):
        errs.append("M_compute-monitor-era1_led missing or non-emissive")
    return errs


def inspect_glb(glb_path: Path) -> tuple[set[str], list[dict]]:
    import trimesh

    print(f"\n=== {glb_path.relative_to(REPO)} ===")
    doc = _glb_json(glb_path)
    scene = trimesh.load(glb_path, force="scene")
    geom = getattr(scene, "geometry", {}) or {}
    print(f"trimesh geometry count: {len(geom)}")
    for name, mesh in sorted(geom.items()):
        print(f"  mesh: {name!r}  faces={len(mesh.faces)}  verts={len(mesh.vertices)}")

    mats = doc.get("materials") or []
    for i, mat in enumerate(mats):
        em = _emissive_factor(mat)
        print(f"  material[{i}]: name={mat.get('name')!r}  emissiveFactor={em}")

    node_names = _node_mesh_names(doc)
    mesh_names = _mesh_table_names(doc)
    print(f"  glTF mesh table names: {', '.join(mesh_names)}")
    print(f"  glTF mesh nodes: {', '.join(node_names)}")
    all_names = set(node_names) | set(mesh_names) | set(geom.keys())
    return all_names, mats


def main(argv: list[str]) -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--gate", action="store_true", help="exit 1 if required mesh names missing")
    parser.add_argument("glb", nargs="*", type=Path)
    args = parser.parse_args(argv[1:])
    paths = args.glb or [MONITOR_GLB, RACK_GLB]
    gate_errs: list[str] = []
    for p in paths:
        if not p.is_file():
            print(f"MISSING: {p}", file=sys.stderr)
            return 1
        names, mats = inspect_glb(p)
        if args.gate and "monitor" in p.name:
            gate_errs.extend(gate_monitor(names, mats))
        if args.gate and "rack" in p.name:
            gate_errs.extend(gate_rack(names))
            led = next((m for m in mats if m.get("name") == "M_compute-server-rack-era1_led"), None)
            if not led or not _emissive_factor(led):
                gate_errs.append("M_compute-server-rack-era1_led missing or non-emissive")
    if gate_errs:
        print("\nGATE FAIL:", file=sys.stderr)
        for e in gate_errs:
            print(f"  - {e}", file=sys.stderr)
        return 1
    if args.gate:
        print("\nGATE PASS: required export mesh names present")
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv))