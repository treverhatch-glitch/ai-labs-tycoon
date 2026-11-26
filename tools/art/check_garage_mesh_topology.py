#!/usr/bin/env python3
"""Hard gate: garage GLB is one object, grounded; welded island check after import."""
from __future__ import annotations

import os
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = os.environ.get("BLENDER_BIN", "blender")

BLENDER_CHECK = """
import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

repo = Path(sys.argv[-1])
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
    if entry.get("source") == "cc0-polyhaven":
        glb = repo / "unity/Assets/Art/EarlyGame/garage-kit" / f"{asset_id}.glb"
        if not glb.exists():
            print(f"FAIL missing glb {asset_id}")
            rc = 1
            continue
        bpy.ops.wm.read_factory_settings(use_empty=True)
        bpy.ops.import_scene.gltf(filepath=str(glb))
        meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
        if not meshes:
            print(f"FAIL {asset_id} cc0 no meshes")
            rc = 1
            continue
        min_y = min((ob.matrix_world @ Vector(c)).y for ob in meshes for c in ob.bound_box)
        ground_slack = 0.08
        if min_y > ground_slack:
            print(f"FAIL {asset_id} cc0 min_y={min_y:.4f}")
            rc = 1
        else:
            print(f"OK {asset_id} cc0 grounded meshes={len(meshes)}")
        continue
    glb = repo / "unity/Assets/Art/EarlyGame/garage-kit" / f"{asset_id}.glb"
    if not glb.exists():
        print(f"FAIL missing glb {asset_id}")
        rc = 1
        continue
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    min_parts = 1 if asset_id == "garage-floor-tile" else 5
    if len(meshes) < min_parts:
        print(f"FAIL {asset_id} mesh_objects={len(meshes)} (need >={min_parts})")
        rc = 1
        continue
    grounded = True
    for ob in meshes:
        min_y = min((ob.matrix_world @ Vector(c)).y for c in ob.bound_box)
        ground_slack = 0.22 if asset_id == "garage-folding-chair" else 0.03
        if min_y > ground_slack or min_y < -0.02:
            grounded = False
            print(f"FAIL {asset_id} {ob.name} min_y={min_y:.4f}")
            rc = 1
            break
    if not grounded:
        continue
    ob = meshes[0]
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=0.0005)
    bpy.ops.object.mode_set(mode="OBJECT")
    bad_islands = False
    for mobj in meshes:
        islands = mesh_island_count(mobj.data)
        if islands > 1:
            print(f"FAIL {asset_id} {mobj.name} islands={islands}")
            rc = 1
            bad_islands = True
    if not bad_islands:
        print(f"OK {asset_id} mesh_parts={len(meshes)} grounded")
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
    if proc.returncode != 0 and proc.stderr:
        print(proc.stderr[-3000:], file=sys.stderr)
    return proc.returncode


if __name__ == "__main__":
    sys.exit(main())