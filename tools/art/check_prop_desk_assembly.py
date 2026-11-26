#!/usr/bin/env python3
"""Gate: desk peripherals on surface, plant scale, and prop AABB disjoint."""
from __future__ import annotations

import json
import math
import struct
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
GLB = REPO / "unity" / "Assets" / "Art" / "BlenderBaked" / "prop-desk" / "mesh_prop-desk.glb"
MANIFEST = REPO / "unity" / "Assets" / "Art" / "BlenderBaked" / "prop-desk" / "manifest.json"
TOL = 0.015
PROP_GAP = 0.014
PLANT_MONITOR_HEIGHT_RATIO_MAX = 1.2
CLUSTER_XY = 0.07


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


def _footprint_disjoint(
    mn_a: tuple[float, float, float],
    mx_a: tuple[float, float, float],
    mn_b: tuple[float, float, float],
    mx_b: tuple[float, float, float],
    gap: float = PROP_GAP,
) -> bool:
    """Desktop footprint overlap in glTF X/Z (horizontal); Y is up."""
    return (
        mx_a[0] + gap < mn_b[0]
        or mx_b[0] + gap < mn_a[0]
        or mx_a[2] + gap < mn_b[2]
        or mx_b[2] + gap < mn_a[2]
    )


def _cluster_verts(verts: list[tuple[float, float, float]]) -> list[list[tuple[float, float, float]]]:
    clusters: list[list[tuple[float, float, float]]] = []
    for v in verts:
        placed = False
        for cl in clusters:
            cx = sum(p[0] for p in cl) / len(cl)
            cz = sum(p[2] for p in cl) / len(cl)
            if abs(cx - v[0]) < CLUSTER_XY and abs(cz - v[2]) < CLUSTER_XY:
                cl.append(v)
                placed = True
                break
        if not placed:
            clusters.append([v])
    return sorted(clusters, key=len, reverse=True)


def _centroid(cl: list[tuple[float, float, float]]) -> tuple[float, float, float]:
    n = len(cl)
    return (
        sum(v[0] for v in cl) / n,
        sum(v[1] for v in cl) / n,
        sum(v[2] for v in cl) / n,
    )


def _assign_clusters(
    clusters: list[list[tuple[float, float, float]]],
    targets: list[tuple[str, float, float, int]],
) -> dict[str, list[tuple[float, float, float]]]:
    """Pick one disjoint cluster per prop nearest manifest (u,v) targets."""
    remaining = [c for c in clusters]
    out: dict[str, list[tuple[float, float, float]]] = {}
    for label, tx, tz, min_size in sorted(targets, key=lambda t: (-t[3], t[0])):
        ranked = sorted(
            [c for c in remaining if len(c) >= max(4, min_size // 2)],
            key=lambda c: math.hypot(_centroid(c)[0] - tx, _centroid(c)[2] - tz),
        )
        if not ranked:
            raise ValueError(f"no cluster for {label}")
        pick = ranked[0]
        out[label] = pick
        remaining.remove(pick)
    return out


def main() -> int:
    if not GLB.is_file():
        print(f"FAIL: missing {GLB}", file=sys.stderr)
        return 1
    offsets = {}
    if MANIFEST.is_file():
        offsets = json.loads(MANIFEST.read_text()).get("kenneyOffsets", {})

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

    perch = [v for v in verts if surface_y - TOL <= v[1] <= surface_y + 0.62]
    if not perch:
        print("FAIL: no peripherals near desktop surface", file=sys.stderr)
        return 1

    clusters = _cluster_verts(perch)
    peripheral = [c for c in clusters if len(c) >= 8]
    if len(peripheral) < 3:
        print(f"FAIL: expected monitor+keyboard+plant clusters (got {len(peripheral)})", file=sys.stderr)
        return 1

    def desk_x(u: float) -> float:
        return desk_x0 + (desk_x1 - desk_x0) * u

    def desk_z(v: float) -> float:
        return desk_z0 + (desk_z1 - desk_z0) * v

    off_kb = offsets.get("computerKeyboard.glb", (0.30, 0.70))
    off_mon = offsets.get("computerScreen.glb", (0.56, 0.46))
    off_pl = offsets.get("pottedPlant.glb", (0.22, 0.84))
    off_ms = offsets.get("computerMouse.glb", (0.62, 0.64))

    labeled = _assign_clusters(
        peripheral,
        [
            ("monitor", desk_x(off_mon[0]), desk_z(off_mon[1]), 30),
            ("keyboard", desk_x(off_kb[0]), desk_z(off_kb[1]), 12),
            ("mouse", desk_x(off_ms[0]), desk_z(off_ms[1]), 4),
        ],
    )
    taken = {id(v) for v in labeled.values()}
    ptx, ptz = desk_x(off_pl[0]), desk_z(off_pl[1])
    plant_candidates: list[list[tuple[float, float, float]]] = []
    for c in peripheral:
        if id(c) in taken:
            continue
        mn_c, mx_c = _bbox(c)
        h_c = mx_c[1] - mn_c[1]
        if not (0.08 <= h_c <= 0.46):
            continue
        cx, cz = _centroid(c)[0], _centroid(c)[2]
        if cx > desk_x(0.34):
            continue
        if math.hypot(cx - ptx, cz - ptz) > 0.26:
            continue
        plant_candidates.append(c)
    if not plant_candidates:
        print("FAIL: no plant cluster", file=sys.stderr)
        return 1
    plant_cl = max(
        plant_candidates,
        key=lambda c: -math.hypot(_centroid(c)[0] - ptx, _centroid(c)[2] - ptz),
    )
    labeled["plant"] = plant_cl

    monitor_cl = labeled["monitor"]

    for label, cl in labeled.items():
        mn, mx = _bbox(cl)
        tol = 0.055 if label == "keyboard" else (0.022 if label == "monitor" else TOL)
        if label not in ("plant",) and abs(mn[1] - surface_y) > tol:
            print(f"FAIL {label}: base Y gap {mn[1] - surface_y:.4f}m", file=sys.stderr)
            return 1
        if mn[0] < desk_x0 - 0.03 or mx[0] > desk_x1 + 0.03 or mn[2] < desk_z0 - 0.03 or mx[2] > desk_z1 + 0.03:
            print(
                f"FAIL {label}: outside desktop XZ "
                f"({mn[0]:.2f},{mn[2]:.2f})-({mx[0]:.2f},{mx[2]:.2f})",
                file=sys.stderr,
            )
            return 1

    mn_m, mx_m = _bbox(monitor_cl)
    mn_p, mx_p = _bbox(plant_cl)
    mon_h = mx_m[1] - mn_m[1]
    plant_h = mx_p[1] - mn_p[1]
    ratio = plant_h / max(mon_h, 1e-6)
    if plant_h < 0.14:
        print(f"FAIL plant cluster height {plant_h:.3f}m < 0.14m", file=sys.stderr)
        return 1
    if ratio > PLANT_MONITOR_HEIGHT_RATIO_MAX:
        print(
            f"FAIL plant/monitor height ratio {ratio:.2f} > {PLANT_MONITOR_HEIGHT_RATIO_MAX}",
            file=sys.stderr,
        )
        return 1

    mn_p, mx_p = _bbox(plant_cl)
    mn_m, mx_m = _bbox(monitor_cl)
    if not _footprint_disjoint(mn_p, mx_p, mn_m, mx_m):
        print("FAIL prop footprint overlap: plant ∩ monitor", file=sys.stderr)
        return 1
    gap3 = PROP_GAP
    vol_disjoint = (
        mx_p[0] + gap3 < mn_m[0]
        or mx_m[0] + gap3 < mn_p[0]
        or mx_p[1] + gap3 < mn_m[1]
        or mx_m[1] + gap3 < mn_p[1]
        or mx_p[2] + gap3 < mn_m[2]
        or mx_m[2] + gap3 < mn_p[2]
    )
    if not vol_disjoint:
        print("FAIL plant ∩ monitor 3D bbox overlap", file=sys.stderr)
        return 1

    print(
        f"OK: prop-desk assembly surface={surface_y:.3f} "
        f"plant_h={plant_h:.3f} monitor_h={mon_h:.3f} ratio={ratio:.2f} "
        f"peripherals={len(peripheral)}"
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())