#!/usr/bin/env python3
"""FAIL#19 geometry gates: skirt Z band, sleeve arm VG, hair bbox vs head."""
from __future__ import annotations

import os
import sys

os.environ.setdefault("RESEARCHER_STOCK_RECOLOR", "1")
from pathlib import Path

import bpy
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import kenney_researcher_lib as kr  # noqa: E402
from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward  # noqa: E402
from kenney_geometry_landmarks import compute_landmarks  # noqa: E402
from researcher_scene_mesh_audit import (  # noqa: E402
    RENDER_ISLAND_BODY_BBOX_GAP_M,
    audit_researcher_render_meshes,
    verify_sleeve_cuff_wrist_gap,
)
from researcher_shirt_derived_meshes import (  # noqa: E402
    MAX_COAT_BODY_GAP_M,
    SKIRT_NAME,
    SLEEVES_NAME,
    VG_DENY_HEAD,
    _body_bvh_world,
    _count_mesh_islands,
    _neck_cut_world_z,
    _skirt_z_band,
    _stretch_skirt_hem_to_mid_thigh,
)

SKIRT_ALLOWED = frozenset({"Hips", "Spine", "Chest", "UpperChest", "LeftUpLeg", "RightUpLeg"})
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
    lo, span, skirt_top_z, z_hip, z_mid_thigh, z_knee = _skirt_z_band(body, lm)
    if not skirt or not sleeves or not hair:
        print("FAIL staff-researcher-idle geometry missing_parts")
        return 1
    shell_gap = 0.0
    tree = _body_bvh_world(body)
    for coat in (skirt, sleeves):
        for v in coat.data.vertices:
            co = coat.matrix_world @ v.co
            _loc, _n, _idx, dist = tree.find_nearest(co)
            if dist is not None:
                shell_gap = max(shell_gap, dist)
    if shell_gap > MAX_COAT_BODY_GAP_M + 1e-4:
        print(
            f"FAIL staff-researcher-idle coat_body_gap={shell_gap:.4f} "
            f"max={MAX_COAT_BODY_GAP_M}"
        )
        return 1
    for coat in (skirt, sleeves):
        isl = _count_mesh_islands(coat)
        if isl != 1:
            print(
                f"FAIL staff-researcher-idle coat_islands={isl} "
                f"mesh={coat.name.split('.')[0]} want=1"
            )
            return 1
    z_neck = _neck_cut_world_z(arm, lm)
    above_neck = sum(
        1 for v in skirt.data.vertices if (skirt.matrix_world @ v.co).z > z_neck + 0.006
    )
    if above_neck > 0:
        print(f"FAIL staff-researcher-idle skirt_verts_above_neck={above_neck} neck_z={z_neck:.4f}")
        return 1
    _stretch_skirt_hem_to_mid_thigh(skirt, z_hip, z_mid_thigh, skirt_top_z=skirt_top_z)
    fwd = toe_forward([body]).normalized()
    hc, y_face_fn = face_shell_coord(body, fwd)
    coat_front = coat_back = 0
    for v in skirt.data.vertices:
        wc = skirt.matrix_world @ v.co
        if wc.z > z_neck + 0.01 or wc.z < z_hip - span * 0.08:
            continue
        yf = y_face_fn(wc)
        if yf >= -0.02:
            coat_front += 1
        if yf <= 0.03:
            coat_back += 1
    if coat_front < 20 or coat_back < 20:
        print(
            f"FAIL staff-researcher-idle coat_shell front={coat_front} back={coat_back} want>=24"
        )
        return 1
    zmin = min((skirt.matrix_world @ v.co).z for v in skirt.data.vertices)
    zmax = max((skirt.matrix_world @ v.co).z for v in skirt.data.vertices)
    if (
        zmin > z_mid_thigh + span * 0.04
        or zmax < z_mid_thigh - span * 0.06
        or zmin < z_knee - span * 0.02
    ):
        print(
            f"FAIL staff-researcher-idle skirt_z_band zmin={zmin:.4f} zmax={zmax:.4f} "
            f"want_hip={z_hip:.4f} mid_thigh={z_mid_thigh:.4f} knee={z_knee:.4f}"
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
    ok_cuff, cuff_msg, cuff_gap = verify_sleeve_cuff_wrist_gap(sleeves, body, arm, lm)
    if not ok_cuff:
        print(f"FAIL staff-researcher-idle {cuff_msg}")
        return 1
    ok_isl, isl_msg = audit_researcher_render_meshes(
        body, meshes, prune=False
    )
    if not ok_isl:
        print(f"FAIL staff-researcher-idle {isl_msg} max={RENDER_ISLAND_BODY_BBOX_GAP_M}")
        return 1
    hw = _bbox_width(hair)
    bw = _head_bbox_width(body, lm)
    ratio = hw / bw
    if ratio > 1.05 or ratio < 0.58:
        print(
            f"FAIL staff-researcher-idle hair_bbox_ratio={ratio:.3f} "
            f"want=[0.58,1.05] hw={hw:.4f} head={bw:.4f}"
        )
        return 1
    hair_v = 0.0
    for slot in hair.material_slots:
        mat = slot.material
        if mat and mat.use_nodes:
            bsdf = mat.node_tree.nodes.get("Principled BSDF")
            if bsdf:
                c = bsdf.inputs["Base Color"].default_value
                hair_v = max(hair_v, 0.299 * c[0] + 0.587 * c[1] + 0.114 * c[2])
    skin_v = 0.62
    for slot in body.material_slots:
        mat = slot.material
        if mat and mat.name.startswith("ResearcherSkin"):
            if mat.use_nodes:
                bsdf = mat.node_tree.nodes.get("Principled BSDF")
                if bsdf:
                    c = bsdf.inputs["Base Color"].default_value
                    skin_v = 0.299 * c[0] + 0.587 * c[1] + 0.114 * c[2]
                    break
    if hair_v > skin_v * 0.72:
        print(
            f"FAIL staff-researcher-idle hair_too_light hair_v={hair_v:.3f} "
            f"skin_v={skin_v:.3f}"
        )
        return 1
    print(
        f"OK staff-researcher-idle geometry skirt_z=({zmin:.3f},{zmax:.3f}) "
        f"mid_thigh={z_mid_thigh:.3f} skirt_above_neck={above_neck} "
        f"coat_gap={shell_gap:.4f} coat_islands=1 sleeve_verts L={cov_l} R={cov_r} "
        f"{cuff_msg} {isl_msg} "
        f"hair_ratio={ratio:.3f} hair_v={hair_v:.3f} skin_v={skin_v:.3f}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())