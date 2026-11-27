"""Proof import: print world bbox and head/foot Z for one staff GLB (PR evidence)."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy
from mathutils import Vector

glb = Path(sys.argv[-1])
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=str(glb))
meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
if not meshes:
    raise SystemExit("no mesh")
ob = meshes[0]
corners = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
xs = [c.x for c in corners]
ys = [c.y for c in corners]
zs = [c.z for c in corners]
print(f"OBJECT {ob.name} glb={glb.name}")
print(f"world_bbox min=({min(xs):.4f},{min(ys):.4f},{min(zs):.4f}) max=({max(xs):.4f},{max(ys):.4f},{max(zs):.4f})")
print(f"spans sx={max(xs)-min(xs):.4f} sy={max(ys)-min(ys):.4f} sz={max(zs)-min(zs):.4f}")
verts = [ob.matrix_world @ v.co for v in ob.data.vertices]
vzs = sorted(p.z for p in verts)
n = max(1, len(vzs) // 20)
foot_z = sum(vzs[:n]) / n
head_z = sum(vzs[-n:]) / n
print(f"foot_band_mean_Z={foot_z:.4f} head_band_mean_Z={head_z:.4f}")
print(f"head_above_feet={head_z > foot_z}")
for name in ("Head", "Foot_L", "Foot_R"):
    child = bpy.data.objects.get(name)
    if child:
        cz = (child.matrix_world @ Vector((0, 0, 0))).z
        print(f"{name} origin_Z={cz:.4f}")