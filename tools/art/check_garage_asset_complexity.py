#!/usr/bin/env python3
"""Hatch geometry gates: sub-shapes / tris + upright aspect on garage GLBs."""
from __future__ import annotations

import json
import os
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = os.environ.get("BLENDER_BIN", "blender")

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
    "garage-roll-door",
    "garage-whiteboard-1995",
    "garage-land-for-sale-sign",
    "garage-window-frame",
    "garage-wall-panel",
    "garage-exterior-wall",
}
MIN_TRIS = 300
rc = 0

for entry in manifest["assets"]:
    asset_id = entry["id"]
    glb = repo / "unity/Assets/Art/EarlyGame/garage-kit" / f"{asset_id}.glb"
    tris = entry.get("triangleCount")
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
    elif asset_id in UPRIGHT:
        bb = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
        ys = [p.y for p in bb]
        zs = [p.z for p in bb]
        h = max(ys) - min(ys)
        d = max(zs) - min(zs)
        if h <= d + 0.02:
            print(f"FAIL {asset_id} upright height={h:.3f} depth={d:.3f}")
            rc = 1
        else:
            print(f"OK {asset_id} tris={tris} upright h/d={h:.2f}/{d:.2f}")
    else:
        print(f"OK {asset_id} tris={tris}")
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