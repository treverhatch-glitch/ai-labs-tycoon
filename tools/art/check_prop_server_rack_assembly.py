#!/usr/bin/env python3
"""Gate: prop-server-rack GLB footprint, height, and floor contact."""
from __future__ import annotations

import json
import struct
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
GLB = REPO / "unity" / "Assets" / "Art" / "BlenderBaked" / "prop-server-rack" / "mesh_prop-server-rack.glb"
TARGET_W, TARGET_D, TARGET_H = 0.60, 0.85, 2.00
BASE_TOL = 0.025


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


def main() -> int:
    if not GLB.is_file():
        print(f"FAIL: missing {GLB}", file=sys.stderr)
        return 1
    js, blob = _gltf_chunks(GLB)
    verts = _positions(js, blob)
    if not verts:
        print("FAIL: no vertices", file=sys.stderr)
        return 1
    xs = [v[0] for v in verts]
    ys = [v[1] for v in verts]
    zs = [v[2] for v in verts]
    w, d, h = max(xs) - min(xs), max(zs) - min(zs), max(ys) - min(ys)
    base = min(ys)
    if base > BASE_TOL:
        print(f"FAIL: rack base floats at min_y={base:.4f}", file=sys.stderr)
        return 1
    if h < TARGET_H * 0.85 or h > TARGET_H * 1.15:
        print(f"FAIL: rack height {h:.3f}m", file=sys.stderr)
        return 1
    if w > TARGET_W * 1.12 or d > TARGET_D * 1.15:
        print(f"FAIL: rack footprint {w:.2f}×{d:.2f}m", file=sys.stderr)
        return 1
    print(f"OK prop-server-rack assembly {w:.2f}×{d:.2f}×{h:.2f}m base_y={base:.4f}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())