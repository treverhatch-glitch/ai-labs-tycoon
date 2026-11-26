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
# glTF re-import bbox: width X, height Z (up), depth Y (thickness).
BBOX_SPEC = {
    "garage-exterior-wall": (4.0, 3.0, 0.20),
    "garage-window-frame": (1.2, 1.0, 0.22),
    "garage-land-for-sale-sign": (0.95, 1.25, 0.29),
    "garage-lot-curb": (1.0, 0.15, 0.30),
}
BBOX_TOL = 0.25
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
    need_meshes = 3 if asset_id == "garage-land-for-sale-sign" else 1
    if len(meshes) != need_meshes:
        print(f"FAIL {asset_id} mesh_objects={len(meshes)} (need {need_meshes})")
        rc = 1
        continue
    ob = meshes[0] if len(meshes) == 1 else None
    island_fail = False
    for mobj in meshes:
        bpy.context.view_layer.objects.active = mobj
        bpy.ops.object.mode_set(mode="EDIT")
        bpy.ops.mesh.select_all(action="SELECT")
        bpy.ops.mesh.remove_doubles(threshold=0.0005)
        bpy.ops.object.mode_set(mode="OBJECT")
        isl = mesh_island_count(mobj.data)
        max_islands = 48 if mobj.name == "letters" else 1
        if isl > max_islands:
            print(f"FAIL {asset_id} {mobj.name} islands={isl}")
            island_fail = True
    if island_fail:
        rc = 1
        continue
    corners = []
    for mobj in meshes:
        corners.extend(mobj.matrix_world @ Vector(c) for c in mobj.bound_box)
  # Re-imported glTF is Z-up in Blender (glTF +Y maps to Blender +Z).
    xs = [c.x for c in corners]
    ys = [c.y for c in corners]
    zs = [c.z for c in corners]
    min_z = min(zs)
    width_x = max(xs) - min(xs)
    thick_y = max(ys) - min(ys)
    height_z = max(zs) - min(zs)
    tris = int(entry.get("triangleCount", 0))
    if asset_id == "garage-lot-curb":
        upright_ok = height_z <= max(width_x, thick_y) * 0.55
    else:
        upright_ok = height_z > thick_y * 1.12
    bbox_ok = True
    if asset_id in BBOX_SPEC:
        sw, sh, sd = BBOX_SPEC[asset_id]
        def _within(actual, target):
            lo, hi = target * (1.0 - BBOX_TOL), target * (1.0 + BBOX_TOL)
            return lo <= actual <= hi
        bbox_ok = _within(width_x, sw) and _within(height_z, sh) and _within(thick_y, sd)
        if not bbox_ok:
            print(
                f"FAIL {asset_id} bbox "
                f"x={width_x:.3f} z={height_z:.3f} y={thick_y:.3f} "
                f"need {sw},{sh},{sd} +/-{int(BBOX_TOL*100)}%"
            )
    if min_z > 0.012:
        print(f"FAIL {asset_id} min_z={min_z:.4f}")
        rc = 1
    elif not upright_ok:
        print(f"FAIL {asset_id} not_upright h={height_z:.3f} thick={thick_y:.3f}")
        rc = 1
    elif not bbox_ok:
        rc = 1
    else:
        print(
            f"OK {asset_id} meshes={len(meshes)} min_z={min_z:.4f} "
            f"bbox={width_x:.2f}x{height_z:.2f}x{thick_y:.2f} tris={tris}"
        )
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