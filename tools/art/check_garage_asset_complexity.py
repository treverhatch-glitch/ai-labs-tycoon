#!/usr/bin/env python3
"""Hatch geometry gates: parts, tris, post-import Y-up orientation on garage GLBs."""
from __future__ import annotations

import json
import os
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = os.environ.get("BLENDER_BIN", "blender")

MIN_SMALL_PARTS = 3
MIN_FURNITURE_PARTS = 6
FURNITURE = {
    "garage-desk-crt",
    "garage-folding-chair",
    "garage-coffee-drip",
    "garage-crt-tower",
    "garage-server-closet",
    "garage-voodude-rack",
    "garage-roll-door",
    "garage-whiteboard-1995",
}

BLENDER_CHECK = r"""
import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

repo = Path(sys.argv[-1])
manifest = json.loads((repo / "unity/Assets/Art/EarlyGame/garage-manifest.json").read_text())
TEXTURE = {"garage-floor-tile"}
UPRIGHT = {
    "garage-land-for-sale-sign",
}
MIN_TRIS = 300
MIN_SMALL = 3
MIN_FURN = 6
FURNITURE = {
    "garage-desk-crt",
    "garage-folding-chair",
    "garage-coffee-drip",
    "garage-crt-tower",
    "garage-server-closet",
    "garage-voodude-rack",
    "garage-roll-door",
    "garage-whiteboard-1995",
}
rc = 0

for entry in manifest["assets"]:
    asset_id = entry["id"]
    glb = repo / "unity/Assets/Art/EarlyGame/garage-kit" / f"{asset_id}.glb"
    part_count = entry.get("partCount")
    tris = entry.get("triangleCount")
    if part_count is not None and asset_id not in TEXTURE:
        need = MIN_FURN if asset_id in FURNITURE else MIN_SMALL
        if part_count < need:
            print(f"FAIL {asset_id} partCount={part_count} < {need}")
            rc = 1
            continue
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if len(meshes) != 1:
        print(f"FAIL {asset_id} mesh_objects={len(meshes)}")
        rc = 1
        continue
    ob = meshes[0]
    if tris is None:
        tris = sum(len(p.vertices) for p in ob.data.polygons)
    if asset_id not in TEXTURE and tris < MIN_TRIS:
        print(f"FAIL {asset_id} tris={tris} < {MIN_TRIS}")
        rc = 1
        continue
    bb = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    xs = [p.x for p in bb]
    ys = [p.y for p in bb]
    zs = [p.z for p in bb]
    ext_x = max(xs) - min(xs)
    ext_y = max(ys) - min(ys)
    ext_z = max(zs) - min(zs)
    min_y = min(ys)
    ground_slack = 0.22 if asset_id == "garage-folding-chair" else 0.015
    if min_y > ground_slack:
        print(f"FAIL {asset_id} min_y={min_y:.4f}")
        rc = 1
        continue
    if asset_id in UPRIGHT:
        h, d = ext_y, max(ext_x, ext_z)
        if ext_y + 0.04 < max(ext_x, ext_z):
            print(
                f"FAIL {asset_id} y_not_tallest ext=({ext_x:.3f},{ext_y:.3f},{ext_z:.3f})"
            )
            rc = 1
            continue
        if h <= d * 0.92:
            print(f"FAIL {asset_id} upright height={h:.3f} depth={d:.3f}")
            rc = 1
            continue
        print(f"OK {asset_id} tris={tris} upright Y h/d={h:.2f}/{d:.2f}")
    else:
        print(f"OK {asset_id} tris={tris} partCount={part_count}")
sys.exit(rc)
"""


def main() -> int:
    proc = subprocess.run(
        [BLENDER, "-b", "--python-expr", BLENDER_CHECK, "--", str(REPO)],
        cwd=REPO,
        text=True,
        capture_output=True,
    )
    if proc.stdout:
        print(proc.stdout.strip())
    if proc.stderr and proc.returncode != 0:
        print(proc.stderr[-2500:], file=sys.stderr)
    return proc.returncode


if __name__ == "__main__":
    sys.exit(main())