"""Headless upright gate (invoked by check_staff_upright.py)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

repo = Path(sys.argv[-1])
manifest = json.loads((repo / "unity/Assets/Art/EarlyGame/staff-manifest.json").read_text())
rc = 0
for entry in manifest["assets"]:
    aid = entry["id"]
    glb = repo / "unity/Assets/Art/EarlyGame/staff-kit" / f"{aid}.glb"
    if not glb.exists():
        print("FAIL missing", aid)
        rc = 1
        continue
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if len(meshes) != 1:
        print(f"FAIL {aid} mesh_objects={len(meshes)}")
        rc = 1
        continue
    ob = meshes[0]
    corners = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    xs = [c.x for c in corners]
    ys = [c.y for c in corners]
    zs = [c.z for c in corners]
    sx, sy, sz = max(xs) - min(xs), max(ys) - min(ys), max(zs) - min(zs)
    min_y = min(ys)
    if sy < max(sx, sz) * 0.9:
        print(f"FAIL {aid} not upright sy={sy:.3f} sx={sx:.3f} sz={sz:.3f}")
        rc = 1
    elif min_y > 0.025:
        print(f"FAIL {aid} float min_y={min_y:.4f}")
        rc = 1
    elif sy < 0.4:
        print(f"FAIL {aid} height sy={sy:.3f}")
        rc = 1
    else:
        print(f"OK {aid} upright sy={sy:.3f} grounded y0={min_y:.4f}")
raise SystemExit(rc)