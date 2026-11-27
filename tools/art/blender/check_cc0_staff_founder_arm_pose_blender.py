"""GLB gate: wrist band below shoulder (not T-pose)."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy
from mathutils import Vector

_args = sys.argv[sys.argv.index("--") + 1 :] if "--" in sys.argv else sys.argv[-1:]
REPO = Path(_args[0])
GLB = Path(_args[1]) if len(_args) > 1 else REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.glb"
MIN_DROP = 0.35
MIN_HAND_SPREAD = 0.24


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
    torso = by.get("Torso")
    anchor = by.get("Neck") or torso or by.get("Head")
    arm_l = by.get("Hand_-1") or by.get("LowerArm_-1") or by.get("Arm_-1")
    arm_r = by.get("Hand_1") or by.get("LowerArm_1") or by.get("Arm_1")
    hips = by.get("Hips")
    if not anchor or not hips or (not arm_l and not arm_r):
        print("FAIL staff-founder-idle arm_pose_parts")
        raise SystemExit(1)
    hip_z = _centroid(hips).z
    neck_lo = max((anchor.matrix_world @ v.co).z for v in anchor.data.vertices)
    drops: list[float] = []
    for arm in (arm_l, arm_r):
        if not arm:
            continue
        verts = [arm.matrix_world @ v.co for v in arm.data.vertices]
        wrist_z = min(v.z for v in verts)
        shoulder_h = neck_lo - hip_z - 0.05
        if shoulder_h < 0.05:
            continue
        drops.append((neck_lo - wrist_z) / shoulder_h)
    if len(drops) < 2:
        print("FAIL staff-founder-idle arm_pose_no_arms")
        raise SystemExit(1)
    spreads: list[float] = []
    if torso:
        tc = _centroid(torso)
        for arm in (arm_l, arm_r):
            if not arm:
                continue
            c = _centroid(arm)
            spreads.append(((c.x - tc.x) ** 2 + (c.y - tc.y) ** 2) ** 0.5)
    if len(spreads) < 2 or min(spreads) < MIN_HAND_SPREAD:
        print(
            f"FAIL staff-founder-idle arm_side_spread={min(spreads) if spreads else 0:.2f} "
            f"want>={MIN_HAND_SPREAD}"
        )
        raise SystemExit(1)
    worst = min(drops)
    if worst < MIN_DROP:
        print(f"FAIL staff-founder-idle arm_tpose_drop={worst:.2f} want>={MIN_DROP}")
        raise SystemExit(1)
    print(f"OK staff-founder-idle arm_drop={worst:.2f}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()