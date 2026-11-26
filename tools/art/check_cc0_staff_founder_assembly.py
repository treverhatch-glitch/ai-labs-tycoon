#!/usr/bin/env python3
"""Founder GLB: one grounded assembly, no stray islands on floor (except shadow prop)."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = Path(__import__("os").environ.get("BLENDER_BIN", "blender"))
GLB = REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.glb"

SCRIPT = """
import bpy, sys
from mathutils import Vector
glb = sys.argv[-1]
bpy.ops.object.select_all(action='SELECT'); bpy.ops.object.delete(use_global=False)
bpy.ops.import_scene.gltf(filepath=glb)
meshes = [o for o in bpy.context.scene.objects if o.type == 'MESH']
if not meshes:
    print('FAIL staff-founder-idle no_meshes'); raise SystemExit(1)
min_z = 1e9
max_y = -1e9
stray = []
for ob in meshes:
    zs = [(ob.matrix_world @ v.co).z for v in ob.data.vertices]
    ys = [(ob.matrix_world @ v.co).y for v in ob.data.vertices]
    mn, mx = min(zs), max(zs)
    min_z = min(min_z, mn)
    lo, hi = min(ys), max(ys)
    if mn < 0.08 and ob.name.split('.')[0] not in ('Foot_-1','Foot_1') and mn < min(zs) + 0.01:
        pass
    if mn < 0.04 and hi < 0.15 and len(ob.data.vertices) < 80:
        stray.append((ob.name, mn, hi))
    for z in zs:
        if z < 0.02:
            max_y = max(max_y, max(ys))
stray = [s for s in stray if 'Mug' not in s[0]]
if stray:
    print('FAIL staff-founder-idle floor_stray', stray)
    raise SystemExit(1)
# single cluster in Y (depth) for body parts excluding mug
centers = []
for ob in meshes:
    base = ob.name.split('.')[0]
    if base in ('FounderMug',):
        continue
    ys = [(ob.matrix_world @ v.co).y for v in ob.data.vertices]
    centers.append(sum(ys)/len(ys))
spread = max(centers) - min(centers)
if spread > 0.95:
    print(f'FAIL staff-founder-idle assembly_spread_y={spread:.2f}')
    raise SystemExit(1)
print(f'OK staff-founder-idle assembly_spread_y={spread:.2f} min_z={min_z:.3f}')
"""


def main() -> int:
    if not GLB.is_file():
        print("FAIL staff-founder-idle missing glb")
        return 1
    proc = subprocess.run(
        [str(BLENDER), "-b", "--python-expr", SCRIPT, "--", str(GLB)],
        cwd=REPO,
        text=True,
        capture_output=True,
    )
    for line in (proc.stdout or "").splitlines():
        if line.startswith("OK ") or line.startswith("FAIL "):
            print(line)
    if proc.returncode != 0:
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())