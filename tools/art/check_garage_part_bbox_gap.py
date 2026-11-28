#!/usr/bin/env python3
"""Fail garage GLB when any mesh island is >2cm from the main cluster (strict)."""
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = os.environ.get("BLENDER_BIN", "blender")
MAX_GAP_M = 0.02


def main() -> int:
    script = r"""
import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

repo = Path(sys.argv[-1])
manifest = json.loads((repo / "unity/Assets/Art/EarlyGame/garage-manifest.json").read_text())
MAX_GAP = """ + repr(
        MAX_GAP_M
    ) + r"""


def world_bbox(ob):
    pts = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    xs, ys, zs = [p.x for p in pts], [p.y for p in pts], [p.z for p in pts]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))


def bbox_gap(a, b):
    lo_a, hi_a = a
    lo_b, hi_b = b
    dx = max(0.0, max(lo_a.x - hi_b.x, lo_b.x - hi_a.x))
    dy = max(0.0, max(lo_a.y - hi_b.y, lo_b.y - hi_a.y))
    dz = max(0.0, max(lo_a.z - hi_b.z, lo_b.z - hi_a.z))
    return (dx * dx + dy * dy + dz * dz) ** 0.5


def vol(b):
    lo, hi = b
    d = hi - lo
    return max(d.x * d.y * d.z, 1e-9)


def union_bbox(boxes):
    lo = Vector((1e9, 1e9, 1e9))
    hi = Vector((-1e9, -1e9, -1e9))
    for blo, bhi in boxes:
        lo.x = min(lo.x, blo.x)
        lo.y = min(lo.y, blo.y)
        lo.z = min(lo.z, blo.z)
        hi.x = max(hi.x, bhi.x)
        hi.y = max(hi.y, bhi.y)
        hi.z = max(hi.z, bhi.z)
    return lo, hi


def main_cluster(n, bboxes):
    ranked = sorted(range(n), key=lambda i: -vol(bboxes[i]))
    cluster = {ranked[0]}
    changed = True
    while changed:
        changed = False
        ulo, uhi = union_bbox([bboxes[i] for i in cluster])
        for j in range(n):
            if j in cluster:
                continue
            if bbox_gap(bboxes[j], (ulo, uhi)) <= MAX_GAP:
                cluster.add(j)
                changed = True
    return cluster


rc = 0
for entry in manifest["assets"]:
    asset_id = entry["id"]
    glb = repo / "unity/Assets/Art/EarlyGame/garage-kit" / f"{asset_id}.glb"
    if not glb.exists():
        print(f"FAIL missing glb {asset_id}")
        rc = 1
        continue
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if len(meshes) < 2:
        print(f"OK {asset_id} parts={len(meshes)} islands=1 max_gap=0.000")
        continue
    bboxes = [world_bbox(m) for m in meshes]
    n = len(meshes)
    cluster = main_cluster(n, bboxes)
    ulo, uhi = union_bbox([bboxes[i] for i in cluster])
    orphans = []
    max_orphan_gap = 0.0
    for i in range(n):
        if i in cluster:
            continue
        g = bbox_gap(bboxes[i], (ulo, uhi))
        max_orphan_gap = max(max_orphan_gap, g)
        orphans.append((meshes[i].name, g))
    islands = 1 if len(cluster) == n else 1 + len(orphans)
    if orphans:
        orphans.sort(key=lambda t: -t[1])
        worst = orphans[0]
        print(
            f"FAIL {asset_id} islands={islands} orphan={worst[0]} gap={worst[1]:.4f}>{MAX_GAP} parts={n}"
        )
        rc = 1
    else:
        print(f"OK {asset_id} parts={n} islands=1 max_gap={max_orphan_gap:.4f}")
sys.exit(rc)
"""
    proc = subprocess.run(
        [BLENDER, "-b", "--python-expr", script, "--", str(REPO)],
        cwd=REPO,
        capture_output=True,
        text=True,
        check=False,
    )
    sys.stdout.write(proc.stdout)
    sys.stderr.write(proc.stderr)
    return proc.returncode


if __name__ == "__main__":
    sys.exit(main())