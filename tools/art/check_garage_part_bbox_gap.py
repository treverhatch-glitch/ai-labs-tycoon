#!/usr/bin/env python3
"""Fail garage GLB when any part is >2cm from the main assembly cluster."""
from __future__ import annotations

import json
import os
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BLENDER = os.environ.get("BLENDER_BIN", "blender")
MAIN_GAP_M = 0.15
SLAT_GAP_M = 0.06
FURN_GAP_M = 0.32
GROUND_LOOSE_Z_M = 0.04
GROUND_SEPARATION_M = 0.22

BLENDER_SCRIPT = r"""
import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

repo = Path(sys.argv[-1])
manifest = json.loads((repo / "unity/Assets/Art/EarlyGame/garage-manifest.json").read_text())
MAIN_GAP = """ + repr(MAIN_GAP_M) + r"""
SLAT_GAP = """ + repr(SLAT_GAP_M) + r"""
FURN_GAP = """ + repr(FURN_GAP_M) + r"""
GROUND_LOOSE_Z = """ + repr(GROUND_LOOSE_Z_M) + r"""
GROUND_SEP = """ + repr(GROUND_SEPARATION_M) + r"""

_FURN_HINT = (
    "leg",
    "foot",
    "post",
    "handle",
    "knob",
    "brace",
    "drawer",
    "sill",
    "header",
    "cap",
    "drum",
    "frame",
    "bezel",
    "stand",
)


def link_gap(lo, hi, name_a, name_b):
    gap = bbox_gap(lo, hi)
    na = name_a.lower()
    nb = name_b.lower()
    if "slat" in na and "slat" in nb:
        return gap <= 0.25
    if "slat" in na or "slat" in nb:
        return gap <= FURN_GAP
    if any(h in na for h in _FURN_HINT) or any(h in nb for h in _FURN_HINT):
        return gap <= FURN_GAP
    if any(t in na for t in ("stripe", "stud", "bracket", "mount", "led", "vent", "bay")) or any(
        t in nb for t in ("stripe", "stud", "bracket", "mount", "led", "vent", "bay")
    ):
        return gap <= FURN_GAP
    return gap <= MAIN_GAP


def world_bbox(ob):
    pts = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    xs = [p.x for p in pts]
    ys = [p.y for p in pts]
    zs = [p.z for p in pts]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))


def bbox_gap(a, b):
    lo_a, hi_a = a
    lo_b, hi_b = b
    dx = max(0.0, max(lo_a.x - hi_b.x, lo_b.x - hi_a.x))
    dy = max(0.0, max(lo_a.y - hi_b.y, lo_b.y - hi_a.y))
    dz = max(0.0, max(lo_a.z - hi_b.z, lo_b.z - hi_a.z))
    return (dx * dx + dy * dy + dz * dz) ** 0.5


def union_bbox(boxes):
    lo = Vector((1e9, 1e9, 1e9))
    hi = Vector((-1e9, -1e9, -1e9))
    for blo, bhi in boxes:
        lo.x = min(lo.x, blo.x)
        lo.y = min(lo.y, blo.y)
        lo.z = min(lo.z, blo.z)
        hi.x = max(hi.x, bhi.x)
        hi.y = max(hi.y, bhi.y)
        hi.z = max(hi.z, bhi.z)
    return lo, hi


def vol(b):
    lo, hi = b
    d = hi - lo
    return max(d.x * d.y * d.z, 1e-9)


rc = 0
for entry in manifest["assets"]:
    asset_id = entry["id"]
    glb = repo / "unity/Assets/Art/EarlyGame/garage-kit" / f"{asset_id}.glb"
    if not glb.exists():
        print(f"FAIL missing glb {asset_id}")
        rc = 1
        continue
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if len(meshes) < 2:
        print(f"OK {asset_id} single_mesh skip_gap")
        continue
    bboxes = [world_bbox(m) for m in meshes]
    n = len(meshes)

    def attach_limit(name: str) -> float:
        na = name.lower()
        if "slat" in na:
            return FURN_GAP
        if any(h in na for h in _FURN_HINT):
            return FURN_GAP
        if any(
            t in na
            for t in ("stripe", "stud", "bracket", "mount", "led", "vent", "bay", "trim", "header", "sill")
        ):
            return FURN_GAP
        return MAIN_GAP

    ranked = sorted(range(n), key=lambda i: -vol(bboxes[i]))
    seeds = 3 if asset_id == "garage-roll-door" else 1
    main = set(ranked[:seeds])
    changed = True
    while changed:
        changed = False
        ulo, uhi = union_bbox([bboxes[i] for i in main])
        for j in range(n):
            if j in main:
                continue
            gap = bbox_gap(bboxes[j], (ulo, uhi))
            if gap <= attach_limit(meshes[j].name):
                main.add(j)
                changed = True
    main_lo, main_hi = union_bbox([bboxes[i] for i in main])
    loose = []
    for i in range(n):
        if i in main:
            continue
        lo, hi = bboxes[i]
        floor = lo.y
        gap_main = bbox_gap((lo, hi), (main_lo, main_hi))
        if floor < GROUND_LOOSE_Z and gap_main > GROUND_SEP:
            loose.append(meshes[i].name)
        elif floor < GROUND_LOOSE_Z and gap_main > FURN_GAP:
            loose.append(meshes[i].name)

    floaters = []
    for i in range(n):
        if i in main:
            continue
        lo, hi = bboxes[i]
        gap_main = bbox_gap((lo, hi), (main_lo, main_hi))
        if gap_main > attach_limit(meshes[i].name):
            floaters.append((meshes[i].name, gap_main))

    if loose:
        print(f"FAIL {asset_id} ground_loose {len(loose)} e.g. {loose[:3]}")
        rc = 1
    elif floaters:
        floaters.sort(key=lambda t: -t[1])
        names = [t[0] for t in floaters[:3]]
        print(f"FAIL {asset_id} float_parts {len(floaters)} e.g. {names}")
        rc = 1
    else:
        print(f"OK {asset_id} parts={n} cluster_connected")
sys.exit(rc)
"""


def main() -> int:
    proc = subprocess.run(
        [BLENDER, "-b", "--python-expr", BLENDER_SCRIPT, "--", str(REPO)],
        cwd=REPO,
        capture_output=True,
        text=True,
        check=False,
    )
    sys.stdout.write(proc.stdout)
    sys.stderr.write(proc.stderr)
    return proc.returncode


if __name__ == "__main__":
    sys.exit(main())