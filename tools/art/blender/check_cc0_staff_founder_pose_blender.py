"""Founder GLB: standing idle silhouette (tall bbox, vertical spine)."""
from __future__ import annotations

import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(sys.argv[-1])
GLB = REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.glb"
MIN_HEIGHT_WIDTH = 1.35
MAX_SPINE_TILT_DEG = 18.0


def _centroid(ob: bpy.types.Object) -> Vector:
    pts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    return sum(pts, Vector()) / max(1, len(pts))


def main() -> None:
    if not GLB.is_file():
        print("FAIL staff-founder-idle missing glb")
        raise SystemExit(1)
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)
    bpy.ops.import_scene.gltf(filepath=str(GLB))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        print("FAIL staff-founder-idle no_meshes")
        raise SystemExit(1)
    by = {o.name.split(".")[0]: o for o in meshes}
    core_names = ("Head", "Neck", "Torso", "Hips", "Leg_-1", "Leg_1", "Foot_-1", "Foot_1")
    core = [by[n] for n in core_names if n in by]
    sample = core if core else meshes
    verts = [ob.matrix_world @ v.co for ob in sample for v in ob.data.vertices]
    xs = [p.x for p in verts]
    ys = [p.y for p in verts]
    zs = [p.z for p in verts]
    sx, sy, sz = max(xs) - min(xs), max(ys) - min(ys), max(zs) - min(zs)
    width = max(sx, sy)
    ratio = sz / max(1e-6, width)
    if ratio < MIN_HEIGHT_WIDTH:
        print(f"FAIL staff-founder-idle pose_bbox_h_w={ratio:.2f} want>{MIN_HEIGHT_WIDTH}")
        raise SystemExit(1)
    hips = by.get("Hips") or by.get("Torso")
    head = by.get("Head")
    if not hips or not head:
        print("FAIL staff-founder-idle pose_spine_parts")
        raise SystemExit(1)
    axis = _centroid(head) - _centroid(hips)
    horiz = math.sqrt(axis.x * axis.x + axis.y * axis.y)
    tilt = math.degrees(math.atan2(horiz, abs(axis.z)))
    if tilt > MAX_SPINE_TILT_DEG:
        print(f"FAIL staff-founder-idle spine_tilt_deg={tilt:.1f} max={MAX_SPINE_TILT_DEG}")
        raise SystemExit(1)
    print(f"OK staff-founder-idle pose_bbox_h_w={ratio:.2f} spine_tilt_deg={tilt:.1f}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()