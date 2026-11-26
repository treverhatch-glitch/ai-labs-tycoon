#!/usr/bin/env python3
"""Packet 3–4 env: same GLB island/ground gate as garage (#88), scoped ids."""
from __future__ import annotations

import json
import os
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = os.environ.get("BLENDER_BIN", "blender")
ENV_IDS = (
    "garage-exterior-wall",
    "garage-land-for-sale-sign",
    "garage-lot-curb",
    "garage-window-frame",
)

BLENDER_CHECK = """
import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

repo = Path(sys.argv[-1])
only = json.loads(sys.argv[-2])
manifest = json.loads((repo / "unity/Assets/Art/EarlyGame/garage-manifest.json").read_text())
rc = 0


def mesh_island_count(mesh) -> int:
    import bmesh

    bm = bmesh.new()
    bm.from_mesh(mesh)
    bm.faces.ensure_lookup_table()
    visited: set[int] = set()
    islands = 0
    for face in bm.faces:
        if face.index in visited:
            continue
        islands += 1
        stack = [face]
        while stack:
            f = stack.pop()
            if f.index in visited:
                continue
            visited.add(f.index)
            for e in f.edges:
                for linked in e.link_faces:
                    if linked.index not in visited:
                        stack.append(linked)
    bm.free()
    return islands


for entry in manifest["assets"]:
    asset_id = entry["id"]
    if asset_id not in only:
        continue
    glb = repo / "unity/Assets/Art/EarlyGame/garage-kit" / f"{asset_id}.glb"
    if not glb.exists():
        print(f"FAIL missing glb {asset_id}")
        rc = 1
        continue
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if len(meshes) != 1:
        print(f"FAIL {asset_id} mesh_objects={len(meshes)} (need 1)")
        rc = 1
        continue
    ob = meshes[0]
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=0.0005)
    bpy.ops.object.mode_set(mode="OBJECT")
    islands = mesh_island_count(ob.data)
    corners = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    min_y = min(c.y for c in corners)
    xs = [c.x for c in corners]
    ys = [c.y for c in corners]
    zs = [c.z for c in corners]
    height = max(ys) - min(ys)
    depth = max(max(xs) - min(xs), max(zs) - min(zs), 1e-6)
    tris = int(entry.get("triangleCount", 0))
    shapes = int(entry.get("subShapeCount", 1))
    parts_ok = tris >= 300 or shapes >= 3
    upright_ok = height > depth * 1.02
    if islands > 1:
        print(f"FAIL {asset_id} islands={islands}")
        rc = 1
    elif min_y > 0.012:
        print(f"FAIL {asset_id} min_y={min_y:.4f}")
        rc = 1
    elif not upright_ok:
        print(f"FAIL {asset_id} not_upright height={height:.3f} depth={depth:.3f}")
        rc = 1
    elif not parts_ok:
        print(f"FAIL {asset_id} tris={tris} shapes={shapes} (need 300+ tris or 3+ shapes)")
        rc = 1
    else:
        print(f"OK {asset_id} islands=1 min_y={min_y:.4f} upright tris={tris}")
sys.exit(rc)
"""


def main() -> int:
    proc = subprocess.run(
        [
            BLENDER,
            "-b",
            "--python-expr",
            BLENDER_CHECK,
            "--",
            json.dumps(list(ENV_IDS)),
            str(REPO),
        ],
        cwd=REPO,
        text=True,
        capture_output=True,
    )
    if proc.stdout:
        print(proc.stdout.strip())
    if proc.returncode != 0 and proc.stderr:
        print(proc.stderr[-3000:], file=sys.stderr)
    return proc.returncode


if __name__ == "__main__":
    sys.exit(main())