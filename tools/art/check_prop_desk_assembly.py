#!/usr/bin/env python3
"""Gate: keyboard/monitor on desktop (Y±1cm) and XZ inside desktop bounds."""
from __future__ import annotations

import json
import struct
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
GLB = REPO / "unity" / "Assets" / "Art" / "BlenderBaked" / "prop-desk" / "mesh_prop-desk.glb"
TOL = 0.015  # 1 cm Hatch bar (+ cluster rounding slack)


def _gltf_chunks(path: Path) -> tuple[dict, bytes]:
    data = path.read_bytes()
    if data[:4] != b"glTF":
        raise ValueError(f"not GLB: {path}")
    length = struct.unpack_from("<I", data, 8)[0]
    off = 12
    js: dict | None = None
    blob = b""
    end = min(len(data), 12 + length)
    while off + 8 <= end:
        chunk_len, chunk_type = struct.unpack_from("<II", data, off)
        off += 8
        chunk = data[off : off + chunk_len]
        off += chunk_len
        if chunk_type == 0x4E4F534A:
            js = json.loads(chunk)
        elif chunk_type == 0x004E4942:
            blob = chunk
    if js is None:
        raise ValueError("no JSON chunk")
    return js, blob


def _positions(js: dict, blob: bytes) -> list[tuple[float, float, float]]:
    out: list[tuple[float, float, float]] = []
    for mesh in js.get("meshes", []):
        for prim in mesh.get("primitives", []):
            acc_ix = prim.get("attributes", {}).get("POSITION")
            if acc_ix is None:
                continue
            acc = js["accessors"][acc_ix]
            bv = js["bufferViews"][acc["bufferView"]]
            start = bv.get("byteOffset", 0) + acc.get("byteOffset", 0)
            stride = bv.get("byteStride", 12)
            for i in range(acc["count"]):
                off = start + i * stride
                out.append(struct.unpack_from("<3f", blob, off)[:3])
    return out


def _bbox(verts: list[tuple[float, float, float]]) -> tuple[tuple[float, float, float], tuple[float, float, float]]:
    xs = [v[0] for v in verts]
    ys = [v[1] for v in verts]
    zs = [v[2] for v in verts]
    return (min(xs), min(ys), min(zs)), (max(xs), max(ys), max(zs))


def main() -> int:
    if not GLB.is_file():
        print(f"FAIL: missing {GLB}", file=sys.stderr)
        return 1
    js, blob = _gltf_chunks(GLB)
    verts = _positions(js, blob)
    if not verts:
        print("FAIL: no mesh positions", file=sys.stderr)
        return 1

    desk_band = [v for v in verts if v[1] < 0.62]
    if not desk_band:
        print("FAIL: no desktop band", file=sys.stderr)
        return 1
    desk_mn, desk_mx = _bbox(desk_band)
    surface_y = desk_mx[1]
    desk_x0, desk_x1, desk_z0, desk_z1 = desk_mn[0], desk_mx[0], desk_mn[2], desk_mx[2]

    perch = [v for v in verts if surface_y - TOL <= v[1] <= surface_y + 0.22]
    if not perch:
        print("FAIL: no peripherals near desktop surface", file=sys.stderr)
        return 1

    clusters: list[list[tuple[float, float, float]]] = []
    for v in perch:
        placed = False
        for cl in clusters:
            if abs(cl[0][0] - v[0]) < 0.08 and abs(cl[0][2] - v[2]) < 0.08:
                cl.append(v)
                placed = True
                break
        if not placed:
            clusters.append([v])
    clusters.sort(key=len, reverse=True)

    peripheral_clusters = [c for c in clusters if len(c) >= 8]
    if len(peripheral_clusters) < 2:
        print(
            f"FAIL: expected monitor+keyboard clusters (got {len(peripheral_clusters)})",
            file=sys.stderr,
        )
        return 1

    for i, cl in enumerate(peripheral_clusters[:2]):
        mn, mx = _bbox(cl)
        label = ("monitor", "keyboard", "mouse")[min(i, 2)]
        if abs(mn[1] - surface_y) > TOL:
            print(f"FAIL {label}: base Y gap {mn[1] - surface_y:.4f}m", file=sys.stderr)
            return 1
        if mn[0] < desk_x0 - 0.02 or mx[0] > desk_x1 + 0.02 or mn[2] < desk_z0 - 0.02 or mx[2] > desk_z1 + 0.02:
            print(
                f"FAIL {label}: outside desktop XZ "
                f"({mn[0]:.2f},{mn[2]:.2f})-({mx[0]:.2f},{mx[2]:.2f})",
                file=sys.stderr,
            )
            return 1

    print(
        f"OK: prop-desk assembly surface={surface_y:.3f} "
        f"peripherals={len(peripheral_clusters)} desktop_XZ="
        f"[{desk_x0:.2f},{desk_x1:.2f}]×[{desk_z0:.2f},{desk_z1:.2f}]"
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())