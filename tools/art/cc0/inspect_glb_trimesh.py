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

KEY_MONITOR = ("power_led", "power_button", "body_tapered", "screen")
KEY_RACK = (
    "unit_4u_face",
    "unit_4u_drive_bay_0_0",
    "unit_2u_a_face",
    "unit_2u_a_vent_0",
    "unit_1u_face",
    "unit_1u_port_led_0",
    "unit_4u_side_rail_l",
)


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


def _topology_mesh(mesh):
    """Merged-vertex topology for watertight / boundary checks."""
    m = mesh.copy()
    m.merge_vertices()
    m.update_faces(m.nondegenerate_faces())
    m.remove_unreferenced_vertices()
    return m


def _boundary_edge_indices(mesh) -> list[int]:
    from trimesh import grouping

    m = _topology_mesh(mesh)
    return list(grouping.group_rows(m.edges_sorted, require_count=1))


def _bounds_line(name: str, mesh) -> str:
    b = mesh.bounds
    c = mesh.centroid
    return (
        f"  bounds {name}: "
        f"x[{b[0][0]:.4f},{b[1][0]:.4f}] "
        f"y[{b[0][1]:.4f},{b[1][1]:.4f}] "
        f"z[{b[0][2]:.4f},{b[1][2]:.4f}] "
        f"centroid=({c[0]:.4f},{c[1]:.4f},{c[2]:.4f})"
    )


def gate_rack_names(names: set[str]) -> list[str]:
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


def gate_rack_depth(geom: dict) -> list[str]:
    errs: list[str] = []
    for unit in ("unit_4u", "unit_2u_a", "unit_2u_b", "unit_1u"):
        face = f"{unit}_face"
        if face not in geom:
            continue
        fz_max = float(geom[face].bounds[1][2])
        for name, mesh in geom.items():
            if not name.startswith(f"{unit}_"):
                continue
            if name in (unit, face) or "side_rail" in name:
                continue
            z_max = float(mesh.bounds[1][2])
            if z_max <= fz_max + 0.0005:
                errs.append(f"{name} z_max {z_max:.4f} not proud of {face} z_max {fz_max:.4f}")
    return errs


def gate_monitor(names: set[str], mats: list[dict], geom: dict) -> list[str]:
    errs: list[str] = []
    missing = REQUIRED_MONITOR_NODES - names
    if missing:
        errs.append(f"missing nodes: {sorted(missing)}")
    led_mat = next((m for m in mats if m.get("name") == "M_compute-monitor-era1_led"), None)
    if not led_mat or not _emissive_factor(led_mat):
        errs.append("M_compute-monitor-era1_led missing or non-emissive")
    if "power_led" in geom:
        c = geom["power_led"].centroid
        if not (0.10 < c[0] < 0.16 and 0.04 < c[1] < 0.09):
            errs.append(f"power_led centroid ({c[0]:.4f},{c[1]:.4f}) outside lower bezel band")
        z0, z1 = geom["power_led"].bounds[:, 2]
        if z1 < 0.239 or z0 > 0.236:
            errs.append(f"power_led z range [{z0:.4f},{z1:.4f}] not on bezel face ~[0.235,0.240]")
    if "power_button" in geom:
        c = geom["power_button"].centroid
        if not (0.13 < c[0] < 0.19 and 0.04 < c[1] < 0.09):
            errs.append(f"power_button centroid ({c[0]:.4f},{c[1]:.4f}) outside lower bezel band")
    if "screen" in geom:
        scr = _topology_mesh(geom["screen"])
        bcount = len(_boundary_edge_indices(scr))
        if not scr.is_watertight or bcount != 0:
            errs.append(f"screen watertight={scr.is_watertight} boundary_edges={bcount}")
        sb = scr.bounds
        if abs(sb[1][0] - 0.15) > 0.002 or abs(sb[0][0] + 0.15) > 0.002:
            errs.append(f"screen x bounds {sb[0][0]:.4f}..{sb[1][0]:.4f} not ~±0.15")
        if abs(sb[0][1] - 0.105) > 0.002 or abs(sb[1][1] - 0.335) > 0.002:
            errs.append(f"screen y bounds {sb[0][1]:.4f}..{sb[1][1]:.4f} not 0.105–0.335")
        if sb[0][2] < 0.212 or sb[1][2] > 0.219:
            errs.append(f"screen z bounds {sb[0][2]:.4f}..{sb[1][2]:.4f} not ~0.213–0.218")
    if "body_tapered" in geom:
        body = _topology_mesh(geom["body_tapered"])
        verts = body.vertices
        front_boundary = 0
        for ei in _boundary_edge_indices(body):
            e = body.edges_sorted[ei]
            zmid = float((verts[e[0], 2] + verts[e[1], 2]) / 2)
            if zmid > 0.225:
                front_boundary += 1
        if front_boundary != 0:
            errs.append(f"body_tapered {front_boundary} boundary edges on bezel front (z_mid>0.225)")
    if "body_tapered" in geom and "screen" in geom:
        body, screen = geom["body_tapered"], geom["screen"]
        sb = screen.bounds
        inset = 0.003
        x0 = float(sb[0][0]) + inset
        x1 = float(sb[1][0]) - inset
        y0 = float(sb[0][1]) + inset
        y1 = float(sb[1][1]) - inset
        for i, face in enumerate(body.faces):
            v = body.vertices[face]
            if float(v[:, 2].max()) <= 0.205 + 1e-3:
                continue
            if (
                float(v[:, 0].min()) >= x0 - 1e-3
                and float(v[:, 0].max()) <= x1 + 1e-3
                and float(v[:, 1].min()) >= y0 - 1e-3
                and float(v[:, 1].max()) <= y1 + 1e-3
            ):
                errs.append(
                    f"body_tapered face {i} inside screen opening at z_max={float(v[:, 2].max()):.4f} (>0.205)"
                )
        verts = body.vertices
        rear = verts[verts[:, 2] < 0.215]
        if len(rear):
            if abs(rear[:, 0]).max() > 0.185 + 1e-3:
                errs.append(f"rear shell |x| max {abs(rear[:, 0]).max():.4f} > 0.185 for z<0.215")
            if rear[:, 1].max() > 0.395 + 1e-3:
                errs.append(f"rear shell y max {rear[:, 1].max():.4f} > 0.395 for z<0.215")
    return errs


def inspect_glb(glb_path: Path, *, sample_keys: tuple[str, ...]) -> tuple[set[str], list[dict], dict]:
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

    print("  sample bounds (glTF Y-up):")
    for k in sample_keys:
        if k in geom:
            print(_bounds_line(k, geom[k]))

    all_names = set(node_names) | set(mesh_names) | set(geom.keys())
    return all_names, mats, geom


def main(argv: list[str]) -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--gate", action="store_true", help="exit 1 if required checks fail")
    parser.add_argument("glb", nargs="*", type=Path)
    args = parser.parse_args(argv[1:])
    paths = args.glb or [MONITOR_GLB, RACK_GLB]
    gate_errs: list[str] = []
    for p in paths:
        if not p.is_file():
            print(f"MISSING: {p}", file=sys.stderr)
            return 1
        keys = KEY_MONITOR if "monitor" in p.name else KEY_RACK
        names, mats, geom = inspect_glb(p, sample_keys=keys)
        if args.gate and "monitor" in p.name:
            gate_errs.extend(gate_monitor(names, mats, geom))
        if args.gate and "rack" in p.name:
            gate_errs.extend(gate_rack_names(names))
            gate_errs.extend(gate_rack_depth(geom))
            led = next((m for m in mats if m.get("name") == "M_compute-server-rack-era1_led"), None)
            if not led or not _emissive_factor(led):
                gate_errs.append("M_compute-server-rack-era1_led missing or non-emissive")
    if gate_errs:
        print("\nGATE FAIL:", file=sys.stderr)
        for e in gate_errs:
            print(f"  - {e}", file=sys.stderr)
        return 1
    if args.gate:
        print("\nGATE PASS: names, depth, and monitor rear checks OK")
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv))