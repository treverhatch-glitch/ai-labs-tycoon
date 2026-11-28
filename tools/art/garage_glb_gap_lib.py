#!/usr/bin/env python3
"""Multipart GLB: one connected cluster; floaters within gap_frac of assembly span."""
from __future__ import annotations

import json
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
GAP_FRAC = 0.02

_BLENDER = rf"""
import json
import sys
from pathlib import Path
import bpy
from mathutils import Vector

glb = Path(sys.argv[-2])
gap_frac = float(sys.argv[-1])
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=str(glb))
meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
if len(meshes) < 2:
    print(json.dumps({{"ok": True, "msg": "single_mesh"}}))
    raise SystemExit(0)

def aabb(ob):
    pts = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    xs, ys, zs = [p.x for p in pts], [p.y for p in pts], [p.z for p in pts]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))

def gap(box_a, box_b):
    lo_a, hi_a = box_a
    lo_b, hi_b = box_b
    dx = max(0.0, max(lo_a.x - hi_b.x, lo_b.x - hi_a.x))
    dy = max(0.0, max(lo_a.y - hi_b.y, lo_b.y - hi_a.y))
    dz = max(0.0, max(lo_a.z - hi_b.z, lo_b.z - hi_a.z))
    return (dx*dx + dy*dy + dz*dz) ** 0.5

def vol(ob):
    lo, hi = aabb(ob)
    d = hi - lo
    return d.x * d.y * d.z

lo = Vector((1e9, 1e9, 1e9))
hi = Vector((-1e9, -1e9, -1e9))
for ob in meshes:
    blo, bhi = aabb(ob)
    lo.x = min(lo.x, blo.x); lo.y = min(lo.y, blo.y); lo.z = min(lo.z, blo.z)
    hi.x = max(hi.x, bhi.x); hi.y = max(hi.y, bhi.y); hi.z = max(hi.z, bhi.z)
span = max((hi - lo).length, 0.2)
touch_gap = max(span * gap_frac, 0.022)
struct_gap = max(span * 0.12, 0.22)

def edge_ok(i, j):
    na = meshes[i].name.lower()
    nb = meshes[j].name.lower()
    g = gap(aabb(meshes[i]), aabb(meshes[j]))
    small = min(vol(meshes[i]), vol(meshes[j])) < max(vol(meshes[i]), vol(meshes[j])) * 0.14
    if small:
        return g <= touch_gap
    if "slat" in na or "slat" in nb:
        return g <= struct_gap
    return g <= struct_gap

n = len(meshes)
seen = [False] * n
comps = []
for start in range(n):
    if seen[start]:
        continue
    stack = [start]
    seen[start] = True
    comp = [start]
    while stack:
        u = stack.pop()
        for v in range(n):
            if seen[v] or not edge_ok(u, v):
                continue
            seen[v] = True
            stack.append(v)
            comp.append(v)
    comps.append(comp)

if len(comps) == 1:
    print(json.dumps({{"ok": True, "parts": n, "touch_gap": touch_gap}}))
    raise SystemExit(0)
worst = meshes[comps[1][0]].name if len(comps) > 1 else ""
print(json.dumps({{"ok": False, "components": len(comps), "floater": worst, "touch_gap": touch_gap, "span": span}}))
"""


def check_glb_gap(asset_id: str, *, gap_frac: float = GAP_FRAC) -> tuple[bool, str]:
    glb = KIT / f"{asset_id}.glb"
    if not glb.is_file():
        return False, "missing_glb"
    proc = subprocess.run(
        ["blender", "-b", "--python-expr", _BLENDER, "--", str(glb), str(gap_frac)],
        cwd=REPO,
        capture_output=True,
        text=True,
        check=False,
    )
    if proc.returncode != 0:
        return False, (proc.stderr or proc.stdout or "blender_fail")[:240]
    for line in proc.stdout.splitlines():
        line = line.strip()
        if line.startswith("{"):
            data = json.loads(line)
            if data.get("ok"):
                return True, f"cluster_ok parts={data.get('parts', 0)}"
            return False, f"disconnected={data.get('components')} floater={data.get('floater')}"
    return False, "no_gap_result"