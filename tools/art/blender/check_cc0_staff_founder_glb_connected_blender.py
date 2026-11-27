"""Founder GLB (Z-up): parts near torso; no stray floor fragments."""
from __future__ import annotations

import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(sys.argv[-1])
GLB = REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.glb"
FLOOR_Z_MAX = 0.20
TORSO_MAX_XY = 0.62
ARM_MAX_XY = 1.14
ARM_MIN_Z = 0.42
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
    stock = by.get("KenneyFounderStock")
    if stock and len(meshes) == 1:
        print("OK staff-founder-idle glb_connected parts=1 stock_mesh")
        raise SystemExit(0)
    if stock and len(meshes) == 3 and by.get("FounderEye_L") and by.get("FounderEye_R"):
        print("OK staff-founder-idle glb_connected parts=3 stock+eyes")
        raise SystemExit(0)
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
            hi.z < FLOOR_Z_MAX
            and base not in FLOOR_KEEP
            and base not in ("Hips", "Torso", "Neck")
            and c.z < 0.14
            and len(ob.data.vertices) < 90
        ):
            print(f"FAIL staff-founder-idle floor_fragment={base} z={hi.z:.3f}")
            raise SystemExit(1)
        if base in FLOOR_KEEP or base.startswith("Leg_"):
            continue
        dxy = math.hypot(c.x - tc.x, c.y - tc.y)
        allow = ARM_MAX_XY if base.startswith(("Hand_", "LowerArm_", "UpperArm_")) else TORSO_MAX_XY
        if dxy > allow and base not in ("Hair",):
            print(f"FAIL staff-founder-idle disconnected={base} dxy={dxy:.3f}")
            raise SystemExit(1)
    for ob in meshes:
        base = ob.name.split(".")[0]
        if not base.startswith(("Hand_", "LowerArm_", "UpperArm_")):
            continue
        lo, hi = _bbox(ob)
        if hi.z < ARM_MIN_Z:
            print(f"FAIL staff-founder-idle arm_floor_debris={base} z_hi={hi.z:.3f}")
            raise SystemExit(1)
    print(f"OK staff-founder-idle glb_connected parts={len(meshes)}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()