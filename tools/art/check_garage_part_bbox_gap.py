#!/usr/bin/env python3
"""Fail garage GLB if multipart layout collapses (stacked centroids / origin pile)."""
from __future__ import annotations

import json
import os
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = os.environ.get("BLENDER_BIN", "blender")
MAX_STACK_CENTROID_R = 0.14
MIN_UNIQUE_CENTROID_FRAC = 0.55

BLENDER_SCRIPT = r"""
import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

repo = Path(sys.argv[-1])
manifest = json.loads((repo / "unity/Assets/Art/EarlyGame/garage-manifest.json").read_text())
MAX_STACK = """ + repr(MAX_STACK_CENTROID_R) + r"""
MIN_UNIQUE_FRAC = """ + repr(MIN_UNIQUE_CENTROID_FRAC) + r"""


def world_bbox(ob):
    pts = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    xs = [p.x for p in pts]
    ys = [p.y for p in pts]
    zs = [p.z for p in pts]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))


def bbox_gap(a, b):
    lo_a, hi_a = a
    lo_b, hi_b = b
    dx = max(0.0, max(lo_a.x - hi_b.x, lo_b.x - hi_a.x))
    dy = max(0.0, max(lo_a.y - hi_b.y, lo_b.y - hi_a.y))
    dz = max(0.0, max(lo_a.z - hi_b.z, lo_b.z - hi_a.z))
    return (dx * dx + dy * dy + dz * dz) ** 0.5


def centroid(ob):
    pts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    if not pts:
        lo, hi = world_bbox(ob)
        return (lo + hi) * 0.5
    return sum(pts, Vector()) / len(pts)


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
        print(f"OK {asset_id} single_mesh skip_gap")
        continue
    bboxes = [world_bbox(m) for m in meshes]
    cents = [centroid(m) for m in meshes]
    mean_c = sum(cents, Vector()) / len(cents)
    spread = max((c - mean_c).length for c in cents)
    lo = Vector((min(b[0].x for b in bboxes), min(b[0].y for b in bboxes), min(b[0].z for b in bboxes)))
    hi = Vector((max(b[1].x for b in bboxes), max(b[1].y for b in bboxes), max(b[1].z for b in bboxes)))
    extent = (hi - lo).length
    if spread < MAX_STACK and extent > 0.55:
        print(f"FAIL {asset_id} stacked_centroids spread={spread:.3f} extent={extent:.3f}")
        rc = 1
        continue
    rounded = {(round(c.x, 2), round(c.y, 2), round(c.z, 2)) for c in cents}
    frac = len(rounded) / len(meshes)
    if frac < MIN_UNIQUE_FRAC and len(meshes) >= 5:
        print(f"FAIL {asset_id} collapsed_centroids unique_frac={frac:.2f}")
        rc = 1
        continue
    print(f"OK {asset_id} parts={len(meshes)} spread={spread:.3f}")
sys.exit(rc)
"""


def main() -> int:
    proc = subprocess.run(
        [BLENDER, "-b", "--python-expr", BLENDER_SCRIPT, "--", str(REPO)],
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