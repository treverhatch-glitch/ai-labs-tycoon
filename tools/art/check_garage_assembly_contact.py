#!/usr/bin/env python3
"""Pass13: real multipart garage assembly gate from committed GLBs (Y-up world)."""
from __future__ import annotations

import json
import math
import os
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"
CONTACT_TOL_M = 0.003
GROUND_TOL_M = 0.002

# Total assembly height (max_y - min_y) in metres, Y-up after glTF import.
HEIGHT_BOUNDS: dict[str, tuple[float, float]] = {
    "garage-folding-chair": (0.78, 0.95),
    "garage-desk-crt": (0.68, 0.88),
    "garage-coffee-drip": (0.55, 1.05),
    "garage-crt-tower": (0.95, 2.05),
    "garage-roll-door": (2.15, 3.05),
    "garage-server-closet": (1.55, 2.05),
    "garage-lot-curb": (0.12, 0.55),
    "garage-land-for-sale-sign": (1.15, 2.25),
    "garage-whiteboard-1995": (1.15, 1.85),
    "garage-window-frame": (1.05, 1.65),
    "garage-wall-panel": (1.05, 1.75),
    "garage-exterior-wall": (1.85, 3.05),
    "garage-floor-tile": (0.04, 0.22),
    "garage-voodude-rack": (0.95, 1.85),
    "garage-cable-spool": (0.35, 0.75),
}

LEG_PAIR_IDS = frozenset({"garage-folding-chair", "garage-desk-crt"})


def _blender_analyze() -> str:
    script = r'''
import json, sys, math
from pathlib import Path
import bpy
from mathutils import Vector

repo = Path(sys.argv[-1])
CONTACT = ''' + repr(CONTACT_TOL_M) + r'''
manifest = json.loads((repo / "unity/Assets/Art/EarlyGame/garage-manifest.json").read_text())
rows = []


def world_aabb(ob):
    verts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    if verts:
        xs = [p.x for p in verts]
        ys = [p.y for p in verts]
        zs = [p.z for p in verts]
        return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))
    pts = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    xs = [p.x for p in pts]
    ys = [p.y for p in pts]
    zs = [p.z for p in pts]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))


def bbox_gap(lo_a, hi_a, lo_b, hi_b):
    dx = max(0.0, max(lo_a.x - hi_b.x, lo_b.x - hi_a.x))
    dy = max(0.0, max(lo_a.y - hi_b.y, lo_b.y - hi_a.y))
    dz = max(0.0, max(lo_a.z - hi_b.z, lo_b.z - hi_a.z))
    return math.sqrt(dx * dx + dy * dy + dz * dz)


for entry in manifest["assets"]:
    aid = entry["id"]
    glb = repo / "unity/Assets/Art/EarlyGame/garage-kit" / f"{aid}.glb"
    if not glb.is_file():
        rows.append({"id": aid, "error": "missing_glb"})
        continue
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        rows.append({"id": aid, "error": "no_meshes"})
        continue
    boxes = [world_aabb(m) for m in meshes]
    n = len(meshes)
    adj = [[] for _ in range(n)]
    for i in range(n):
        for j in range(i + 1, n):
            g = bbox_gap(boxes[i][0], boxes[i][1], boxes[j][0], boxes[j][1])
            if g <= CONTACT:
                adj[i].append(j)
                adj[j].append(i)
    seen = [False] * n
    components = 0
    for i in range(n):
        if seen[i]:
            continue
        components += 1
        stack = [i]
        seen[i] = True
        while stack:
            u = stack.pop()
            for v in adj[u]:
                if not seen[v]:
                    seen[v] = True
                    stack.append(v)
    lo = Vector((1e9, 1e9, 1e9))
    hi = Vector((-1e9, -1e9, -1e9))
    for blo, bhi in boxes:
        lo.x = min(lo.x, blo.x)
        lo.y = min(lo.y, blo.y)
        lo.z = min(lo.z, blo.z)
        hi.x = max(hi.x, bhi.x)
        hi.y = max(hi.y, bhi.y)
        hi.z = max(hi.z, bhi.z)
    ext_x = hi.x - lo.x
    ext_y = hi.y - lo.y
    ext_z = hi.z - lo.z
    assembly_height = max(ext_x, ext_y, ext_z)
    part_rows = []
    for ob, (blo, bhi) in zip(meshes, boxes):
        part_rows.append({
            "name": ob.name,
            "min_y": float(blo.y),
            "max_y": float(bhi.y),
            "cy": float((blo.y + bhi.y) * 0.5),
        })
    rows.append({
        "id": aid,
        "parts": n,
        "components": components,
        "min_y": float(lo.y),
        "max_y": float(hi.y),
        "height": float(assembly_height),
        "part_layout": part_rows,
    })
print("CONTACT_REPORT", json.dumps(rows))
'''
    proc = subprocess.run(
        [os.environ.get("BLENDER_BIN", "blender"), "-b", "--python-expr", script, "--", str(REPO)],
        cwd=REPO,
        capture_output=True,
        text=True,
        check=False,
    )
    if proc.returncode != 0:
        raise RuntimeError(proc.stderr or proc.stdout)
    for line in proc.stdout.splitlines():
        if line.startswith("CONTACT_REPORT"):
            return line.split("CONTACT_REPORT", 1)[1].strip()
    raise RuntimeError("no CONTACT_REPORT in blender output")


def _leg_reach_check(asset_id: str, parts: list[dict]) -> tuple[bool, str]:
    if asset_id not in LEG_PAIR_IDS:
        return True, "leg_reach_na"
    if len(parts) < 2:
        return True, "leg_reach_single_mesh"
    if not any("leg" in p["name"].lower() for p in parts):
        return True, "leg_reach_kenney_mesh"
    by_min_y = sorted(parts, key=lambda p: p["min_y"])
    floorish = [p for p in parts if p["min_y"] <= 0.004]
    if len(floorish) < 2:
        floorish = by_min_y[: max(4, min(4, len(by_min_y)))]
    upper = sorted(parts, key=lambda p: -p["cy"])
    seat_like = upper[0]
    leg_tops = max(p["max_y"] for p in floorish)
    seat_bottom = seat_like["min_y"]
    gap = seat_bottom - leg_tops
    if gap > CONTACT_TOL_M * 2:
        return False, f"leg_seat_gap={gap:.4f}m seat={seat_like['name']}"
    if gap < -0.01:
        return False, f"legs_through_seat gap={gap:.4f}"
    return True, f"leg_seat_gap={gap:.4f}m"


def check_row(row: dict) -> tuple[int, str]:
    aid = row["id"]
    if row.get("error"):
        return 1, f"FAIL {aid} {row['error']}"
    parts_n = row["parts"]
    comps = row["components"]
    min_y = row["min_y"]
    height = row["height"]
    if aid == "garage-floor-tile" and row.get("part_layout"):
        ys = [p["max_y"] - p["min_y"] for p in row["part_layout"]]
        height = min(ys) if ys else height
    msgs: list[str] = []
    rc = 0
    if parts_n < 1:
        print(f"FAIL {aid} parts=0")
        return 1, ""
    if comps != 1:
        print(f"FAIL {aid} contact_components={comps} (need 1) parts={parts_n}")
        rc = 1
    else:
        msgs.append(f"components=1")
    if abs(min_y) > GROUND_TOL_M:
        print(f"FAIL {aid} min_y={min_y:.4f} (tol {GROUND_TOL_M})")
        rc = 1
    else:
        msgs.append(f"min_y={min_y:.4f}")
    bounds = HEIGHT_BOUNDS.get(aid)
    if bounds:
        lo_b, hi_b = bounds
        if height < lo_b or height > hi_b:
            print(f"FAIL {aid} height={height:.3f} need [{lo_b},{hi_b}]")
            rc = 1
        else:
            msgs.append(f"height={height:.3f}")
    ok_leg, leg_msg = _leg_reach_check(aid, row.get("part_layout", []))
    if not ok_leg:
        print(f"FAIL {aid} {leg_msg}")
        rc = 1
    else:
        msgs.append(leg_msg)
    if rc == 0:
        print(f"OK {aid} contact_graph {' '.join(msgs)} parts={parts_n}")
    return rc, f"{aid}: components={comps} min_y={min_y:.4f} height={height:.3f} {leg_msg}"


def main() -> int:
    raw = _blender_analyze()
    rows = json.loads(raw)
    rc = 0
    summaries: list[str] = []
    for row in rows:
        r, summary = check_row(row)
        rc |= r
        if summary:
            summaries.append(summary)
    if os.environ.get("CONTACT_GATE_WRITE_REPORT"):
        out = Path(os.environ["CONTACT_GATE_WRITE_REPORT"])
        out.write_text("\n".join(summaries) + "\n", encoding="utf-8")
    return rc


if __name__ == "__main__":
    sys.exit(main())