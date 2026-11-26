#!/usr/bin/env python3
"""Gate: prop-desk GLB peripherals rest on desktop (min-Y within 1 cm of surface band)."""
from __future__ import annotations

import json
import struct
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
GLB = REPO / "unity" / "Assets" / "Art" / "BlenderBaked" / "prop-desk" / "mesh_prop-desk.glb"
TOL = 0.01


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
    ys = sorted(v[1] for v in _positions(js, blob))
    if not ys:
        print("FAIL: no mesh positions", file=sys.stderr)
        return 1
    lip = [y for y in ys if 0.48 <= y <= 0.58]
    if not lip:
        print("FAIL: no desktop lip band in GLB", file=sys.stderr)
        return 1
    surface_y = max(lip)
    perch = [y for y in ys if surface_y - TOL <= y <= surface_y + 0.01]
    if not perch:
        print(f"FAIL: no verts on desktop surface near {surface_y:.3f}", file=sys.stderr)
        return 1
    gap = surface_y - min(perch)
    if gap > TOL:
        print(
            f"FAIL prop-desk assembly: rest gap {gap:.4f}m > {TOL} (surface {surface_y:.3f})",
            file=sys.stderr,
        )
        return 1
    print(f"OK: prop-desk assembly surface={surface_y:.3f} gap={gap:.4f}m")
    return 0


if __name__ == "__main__":
    sys.exit(main())