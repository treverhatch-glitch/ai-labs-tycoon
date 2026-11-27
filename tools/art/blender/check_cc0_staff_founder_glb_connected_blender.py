"""Founder GLB: every body part centroid near torso; no loose floor fragments."""
from __future__ import annotations

import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(sys.argv[-1])
GLB = REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.glb"
FLOOR_Y_MAX = 0.18
TORSO_MAX_XZ = 0.62
ARM_MAX_XZ = 1.14
FLOOR_KEEP = frozenset(
    {
        "Sole_-1",
        "Sole_1",
        "Foot_-1",
        "Foot_1",
        "Leg_-1",
        "Leg_1",
    }
)


def _centroid(ob: bpy.types.Object) -> Vector:
    pts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    return sum(pts, Vector()) / max(1, len(pts))


def _bbox(ob: bpy.types.Object) -> tuple[Vector, Vector]:
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for v in ob.data.vertices:
        w = ob.matrix_world @ v.co
        mins.x = min(mins.x, w.x)
        mins.y = min(mins.y, w.y)
        mins.z = min(mins.z, w.z)
        maxs.x = max(maxs.x, w.x)
        maxs.y = max(maxs.y, w.y)
        maxs.z = max(maxs.z, w.z)
    return mins, maxs


def main() -> None:
    if not GLB.is_file():
        print("FAIL staff-founder-idle missing glb")
        raise SystemExit(1)
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)
    bpy.ops.import_scene.gltf(filepath=str(GLB))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    by = {o.name.split(".")[0]: o for o in meshes}
    torso = by.get("Torso")
    if not torso:
        print("FAIL staff-founder-idle connected torso_missing")
        raise SystemExit(1)
    tc = _centroid(torso)
    for ob in meshes:
        base = ob.name.split(".")[0]
        c = _centroid(ob)
        lo, hi = _bbox(ob)
        if base.startswith(("Eye_", "Mouth", "Eyebrows", "Hair")):
            continue
        if (
            hi.y < FLOOR_Y_MAX
            and base not in FLOOR_KEEP
            and base not in ("Hips", "Torso", "Neck")
            and c.y < 0.14
            and len(ob.data.vertices) < 90
        ):
            print(f"FAIL staff-founder-idle floor_fragment={base} y={hi.y:.3f}")
            raise SystemExit(1)
        if base in FLOOR_KEEP or base.startswith("Leg_"):
            continue
        dxz = math.hypot(c.x - tc.x, c.z - tc.z)
        allow = ARM_MAX_XZ if base.startswith(("Hand_", "LowerArm_", "UpperArm_")) else TORSO_MAX_XZ
        if dxz > allow and base not in ("Hair",):
            print(f"FAIL staff-founder-idle disconnected={base} dxz={dxz:.3f}")
            raise SystemExit(1)
    for ob in meshes:
        base = ob.name.split(".")[0]
        if not base.startswith(("Hand_", "LowerArm_", "UpperArm_")):
            continue
        c = _centroid(ob)
        lo, hi = _bbox(ob)
        if lo.y < 0.12 and hi.y < 0.35:
            print(f"FAIL staff-founder-idle arm_floor_debris={base} y={hi.y:.3f}")
            raise SystemExit(1)
    print(f"OK staff-founder-idle glb_connected parts={len(meshes)}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()