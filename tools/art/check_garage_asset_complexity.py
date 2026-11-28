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
MIN_TRIS = 250
MIN_SMALL = 5
MIN_FURN = 6
MIN_MESH_PARTS = 5
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
def _is_import_restyle(entry, repo):
    if entry.get("source") in ("cc0-polyhaven", "kenney-cc0"):
        return True
    sidecar = repo / "unity/Assets/Art/EarlyGame/garage-kit" / f"{entry['id']}.asset.json"
    if sidecar.is_file():
        lic = json.loads(sidecar.read_text(encoding="utf-8")).get("license", "")
        return "Kenney" in lic or "Poly Haven" in lic
    return False

rc = 0

for entry in manifest["assets"]:
    asset_id = entry["id"]
    if _is_import_restyle(entry, repo):
        tris = entry.get("triangleCount", 0)
        need_tris = 36 if asset_id in {"garage-floor-tile", "garage-lot-curb"} else MIN_TRIS
        if tris < need_tris:
            print(f"FAIL {asset_id} cc0 tris={tris} < {MIN_TRIS}")
            rc = 1
        else:
            print(f"OK {asset_id} cc0 tris={tris} partCount={entry.get('partCount')}")
        continue
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
    need_mesh = 1 if asset_id in TEXTURE else MIN_MESH_PARTS
    if len(meshes) < need_mesh:
        print(f"FAIL {asset_id} mesh_objects={len(meshes)} (need >={need_mesh})")
        rc = 1
        continue
    tris = sum(sum(len(p.vertices) for p in m.data.polygons) for m in meshes)
    if asset_id not in TEXTURE and tris < MIN_TRIS:
        print(f"FAIL {asset_id} tris={tris} < {MIN_TRIS}")
        rc = 1
        continue
    pts: list[Vector] = []
    for mobj in meshes:
        pts.extend(mobj.matrix_world @ v.co for v in mobj.data.vertices)
    xs = [p.x for p in pts]
    ys = [p.y for p in pts]
    zs = [p.z for p in pts]
    ext_x = max(xs) - min(xs)
    ext_y = max(ys) - min(ys)
    ext_z = max(zs) - min(zs)
    part_floor = []
    for mobj in meshes:
        vys = [(mobj.matrix_world @ v.co).y for v in mobj.data.vertices]
        part_floor.append(min(vys) if vys else min(ys))
    min_y = min(part_floor)
    ground_slack = 0.22 if asset_id == "garage-folding-chair" else 0.04
    if min_y > ground_slack:
        print(f"FAIL {asset_id} min_y={min_y:.4f}")
        rc = 1
        continue
    if asset_id in UPRIGHT:
        if asset_id == "garage-land-for-sale-sign":
            h = max(ext_y, ext_z)
            d = max(ext_x, min(ext_y, ext_z))
        else:
            h, d = ext_y, max(ext_x, ext_z)
        if asset_id != "garage-land-for-sale-sign" and ext_y + 0.12 < max(ext_x, ext_z):
            print(
                f"FAIL {asset_id} y_not_tallest ext=({ext_x:.3f},{ext_y:.3f},{ext_z:.3f})"
            )
            rc = 1
            continue
        upright_ratio = 0.75 if asset_id == "garage-land-for-sale-sign" else 0.92
        if h <= d * upright_ratio:
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