"""GLB / build gate: head parts above feet; feet grounded (Z-up export)."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(sys.argv[-1])
GLB = REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.glb"
MIN_HEAD_ABOVE_FEET_M = 1.17
MAX_FOOT_ABOVE_FLOOR_M = 0.13


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
    by = {o.name.split(".")[0]: o for o in meshes}
    head = by.get("Head")
    feet = [by[n] for n in ("Foot_-1", "Foot_1", "Leg_-1", "Leg_1") if n in by]
    if not head or len(feet) < 2:
        print("FAIL staff-founder-idle standing_parts_missing")
        raise SystemExit(1)
    head_z = max((head.matrix_world @ v.co).z for v in head.data.vertices)
    foot_centroids = [_centroid(f).z for f in feet]
    foot_floor = min(
        (f.matrix_world @ v.co).z for f in feet for v in f.data.vertices
    )
    gap = head_z - max(foot_centroids)
    floor_z = foot_floor
    if gap < MIN_HEAD_ABOVE_FEET_M:
        print(f"FAIL staff-founder-idle head_feet_z_gap={gap:.2f}m")
        raise SystemExit(1)
    if floor_z > MAX_FOOT_ABOVE_FLOOR_M:
        print(f"FAIL staff-founder-idle feet_float_z={floor_z:.3f}m")
        raise SystemExit(1)
    print(f"OK staff-founder-idle standing head_feet_z_gap={gap:.2f}m floor_z={floor_z:.3f}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()