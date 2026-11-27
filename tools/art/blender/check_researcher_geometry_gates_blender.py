#!/usr/bin/env python3
"""FAIL#19 geometry gates: skirt Z band, sleeve arm VG, hair bbox vs head."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import kenney_researcher_lib as kr  # noqa: E402
from kenney_face_axis import toe_forward  # noqa: E402
from kenney_geometry_landmarks import compute_landmarks  # noqa: E402
from researcher_shirt_derived_meshes import SKIRT_NAME, SLEEVES_NAME, VG_DENY_HEAD  # noqa: E402

SKIRT_ALLOWED = frozenset(
    {"Hips", "Spine", "Chest", "UpperChest", "LeftUpLeg", "RightUpLeg", "LeftLeg", "RightLeg"}
)
SLEEVE_ALLOWED = frozenset(
    {"LeftShoulder", "LeftArm", "LeftForeArm", "RightShoulder", "RightArm", "RightForeArm"}
)


def _head_bbox_width(body: bpy.types.Object, lm) -> float:
    lo, span = lm.zmin, lm.span
    z0 = lo + span * (lm.h_neck + 0.04)
    xs: list[float] = []
    ys: list[float] = []
    for v in body.data.vertices:
        w = body.matrix_world @ v.co
        if w.z < z0:
            continue
        xs.append(w.x)
        ys.append(w.y)
    if len(xs) < 12:
        return 0.0
    return max(max(xs) - min(xs), max(ys) - min(ys))


def _bbox_width(ob: bpy.types.Object) -> float:
    mn = Vector((1e9, 1e9, 1e9))
    mx = Vector((-1e9, -1e9, -1e9))
    for v in ob.data.vertices:
        w = ob.matrix_world @ v.co
        mn.x = min(mn.x, w.x)
        mx.x = max(mx.x, w.x)
        mn.y = min(mn.y, w.y)
        mx.y = max(mx.y, w.y)
        mn.z = min(mn.z, w.z)
        mx.z = max(mx.z, w.z)
    return max(mx.x - mn.x, mx.y - mn.y)


def _bone_z(arm: bpy.types.Object | None, name: str) -> float | None:
    if not arm:
        return None
    b = arm.data.bones.get(name)
    if not b:
        return None
    return (arm.matrix_world @ b.head_local).z


def _vg_weight_sum(ob: bpy.types.Object, vi: int, allowed: frozenset[str]) -> float:
    s = 0.0
    for g in ob.data.vertices[vi].groups:
        if g.group >= len(ob.vertex_groups):
            continue
        if ob.vertex_groups[g.group].name in allowed:
            s += g.weight
    return s


def main() -> int:
    kr.build_researcher_kenney()
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    body = next(o for o in meshes if o.name.split(".")[0] == "KenneyResearcherStock")
    skirt = next((o for o in meshes if o.name.split(".")[0] == SKIRT_NAME), None)
    sleeves = next((o for o in meshes if o.name.split(".")[0] == SLEEVES_NAME), None)
    hair = next((o for o in meshes if o.name.split(".")[0] == "ResearcherHairCap"), None)
    arm = next((o for o in bpy.context.scene.objects if o.type == "ARMATURE"), None)
    lm = compute_landmarks(body)
    lo, span = lm.zmin, lm.span
    z_hip = lo + span * (lm.h_waist - 0.06)
    z_knee = lo + span * (lm.h_waist + 0.55)
    if arm:
        hk = _bone_z(arm, "Hips")
        lk = _bone_z(arm, "LeftLeg")
        if hk is not None:
            z_hip = hk - span * 0.02
        if lk is not None:
            z_knee = lk + span * 0.02
    if not skirt or not sleeves or not hair:
        print("FAIL staff-researcher-idle geometry missing_parts")
        return 1
    zmin = min((skirt.matrix_world @ v.co).z for v in skirt.data.vertices)
    zmax = max((skirt.matrix_world @ v.co).z for v in skirt.data.vertices)
    if zmin < z_hip - span * 0.04 or zmax > z_knee:
        print(
            f"FAIL staff-researcher-idle skirt_z_band zmin={zmin:.4f} zmax={zmax:.4f} "
            f"want_hip={z_hip:.4f} knee={z_knee:.4f}"
        )
        return 1
    for v in skirt.data.vertices:
        for g in v.groups:
            if g.group < len(skirt.vertex_groups):
                nm = skirt.vertex_groups[g.group].name
                if nm in VG_DENY_HEAD and g.weight > 0.02:
                    print(f"FAIL staff-researcher-idle skirt_head_weight bone={nm} w={g.weight}")
                    return 1
    cov_l = cov_r = 0
    for vi in range(len(sleeves.data.vertices)):
        arm_cov = _vg_weight_sum(sleeves, vi, SLEEVE_ALLOWED)
        if arm_cov < 0.35:
            continue
        co = sleeves.matrix_world @ sleeves.data.vertices[vi].co
        lat_sign = 1.0 if co.x >= 0 else -1.0
        if lat_sign > 0:
            cov_r += 1
        else:
            cov_l += 1
    if cov_l < 8 or cov_r < 8:
        print(f"FAIL staff-researcher-idle sleeve_coverage L={cov_l} R={cov_r}")
        return 1
    hw = _bbox_width(hair)
    bw = _head_bbox_width(body, lm)
    ratio = hw / bw
    if ratio > 1.05:
        print(f"FAIL staff-researcher-idle hair_bbox_ratio={ratio:.3f} want<=1.05 hw={hw:.4f} head={bw:.4f}")
        return 1
    print(
        f"OK staff-researcher-idle geometry skirt_z=({zmin:.3f},{zmax:.3f}) "
        f"sleeve_verts L={cov_l} R={cov_r} hair_ratio={ratio:.3f}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())