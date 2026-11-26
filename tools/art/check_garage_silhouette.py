#!/usr/bin/env python3
"""Fail garage GLB with <5 mesh parts or a single cuboid silhouette blob."""
from __future__ import annotations

import argparse
import json
import os
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"
MIN_MESH_PARTS = 5
BLENDER = os.environ.get("BLENDER_BIN", "blender")

BLENDER_CHECK = r"""
import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

repo = Path(sys.argv[-1])
manifest = json.loads((repo / "unity/Assets/Art/EarlyGame/garage-manifest.json").read_text())
MIN_PARTS = 5
CUBOID_FILL = 0.90
rc = 0


def vertex_cuboid_fill(ob):
    if not ob.data.vertices:
        return 1.0
    verts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    mins = Vector((min(v.x for v in verts), min(v.y for v in verts), min(v.z for v in verts)))
    maxs = Vector((max(v.x for v in verts), max(v.y for v in verts), max(v.z for v in verts)))
    span = maxs - mins
    if min(span.x, span.y, span.z) < 1e-4:
        return 1.0
    margin = 0.10
    shell = 0
    for v in verts:
        t = Vector((
            (v.x - mins.x) / span.x if span.x > 0 else 0.5,
            (v.y - mins.y) / span.y if span.y > 0 else 0.5,
            (v.z - mins.z) / span.z if span.z > 0 else 0.5,
        ))
        if (
            t.x <= margin
            or t.x >= 1 - margin
            or t.y <= margin
            or t.y >= 1 - margin
            or t.z <= margin
            or t.z >= 1 - margin
        ):
            shell += 1
    return shell / len(verts)


for entry in manifest["assets"]:
    asset_id = entry["id"]
    if asset_id == "garage-floor-tile":
        continue
    glb = repo / "unity/Assets/Art/EarlyGame/garage-kit" / f"{asset_id}.glb"
    if not glb.exists():
        print(f"FAIL missing glb {asset_id}")
        rc = 1
        continue
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    cc0 = entry.get("source") == "cc0-polyhaven"
    if not cc0 and len(meshes) < MIN_PARTS:
        print(f"FAIL {asset_id} mesh_parts={len(meshes)} (need >={MIN_PARTS})")
        rc = 1
        continue
    if not cc0 and len(meshes) == 1:
        fill = vertex_cuboid_fill(meshes[0])
        if fill >= CUBOID_FILL:
            print(f"FAIL {asset_id} cuboid_blob fill={fill:.3f}")
            rc = 1
            continue
    span = Vector((0.0, 0.0, 0.0))
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in meshes:
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins.x = min(mins.x, w.x)
            mins.y = min(mins.y, w.y)
            mins.z = min(mins.z, w.z)
            maxs.x = max(maxs.x, w.x)
            maxs.y = max(maxs.y, w.y)
            maxs.z = max(maxs.z, w.z)
    span = maxs - mins
    print(f"OK {asset_id} mesh_parts={len(meshes)} bbox={[round(x, 3) for x in span]}")
sys.exit(rc)
"""


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--expect-fail", action="store_true")
    args = parser.parse_args()
    proc = subprocess.run(
        [BLENDER, "-b", "--python-expr", BLENDER_CHECK, "--", str(REPO)],
        cwd=REPO,
        text=True,
        capture_output=True,
    )
    if proc.stdout:
        print(proc.stdout.strip())
    if proc.stderr:
        print(proc.stderr.strip(), file=sys.stderr)
    rc = proc.returncode
    if args.expect_fail and rc != 0:
        print("OK proof: silhouette gate failed as expected on prior head GLBs")
        return 0
    if args.expect_fail and rc == 0:
        print("FAIL proof: expected silhouette failures but all passed")
        return 1
    return rc


if __name__ == "__main__":
    raise SystemExit(main())