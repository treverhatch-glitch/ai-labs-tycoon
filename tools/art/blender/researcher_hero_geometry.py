"""Researcher hero — lab coat, hair cap, glasses as separate CC0 Blender geometry."""
from __future__ import annotations

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc
from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward
from kenney_geometry_landmarks import compute_landmarks, torso_half_width
from kenney_researcher_stock_recolor import (
    RESEARCHER_HAIR,
    _ensure_material,
    _head_width_at,
)

LAB_COAT_RGB = (237 / 255, 237 / 255, 230 / 255, 1.0)
GLASSES_RGB = (34 / 255, 34 / 255, 34 / 255, 1.0)


def _researcher_head_top(ws: list[Vector], lm) -> float:
    head_pts = [p for p in ws if (p.z - lm.zmin) / lm.span > lm.h_neck + 0.02]
    if not head_pts:
        return 0.96
    zs = sorted((p.z - lm.zmin) / lm.span for p in head_pts)
    return zs[max(0, int(len(zs) * 0.88) - 1)]


def _researcher_chin_h(ws: list[Vector], lm) -> float:
    head_pts = [p for p in ws if (p.z - lm.zmin) / lm.span > lm.h_neck + 0.02]
    if not head_pts:
        return lm.h_neck + 0.08
    return min((p.z - lm.zmin) / lm.span for p in head_pts)


def _researcher_eye_h(ws: list[Vector], lm, head_top: float) -> float:
    """Mid-face (~58% of skull height above chin)."""
    chin_h = _researcher_chin_h(ws, lm)
    return chin_h + 0.45 * (head_top - chin_h)

_VG_COAT_BODY = frozenset(
    {"Hips", "Spine", "Chest", "UpperChest", "LeftShoulder", "RightShoulder"}
)
_VG_COAT_UPPER_ARM = frozenset({"LeftArm", "RightArm"})
_VG_COAT_FOREARM = frozenset({"LeftForeArm", "RightForeArm"})
_VG_COAT_SHOULDER = frozenset({"LeftShoulder", "RightShoulder"})
_VG_COAT_THIGH = frozenset({"LeftUpLeg", "RightUpLeg"})
_VG_HANDS = frozenset(
    {
        "LeftHand",
        "RightHand",
        "LeftHandIndex1",
        "RightHandIndex1",
        "LeftHandThumb1",
        "RightHandThumb1",
    }
)
_VG_COAT_EXCLUDE = frozenset({"Head", "Neck"})


def _vg_weight(ob: bpy.types.Object, vi: int, names: frozenset[str]) -> float:
    total = 0.0
    for g in ob.data.vertices[vi].groups:
        vg = ob.vertex_groups[g.group]
        if vg.name in names:
            total += g.weight
    return total


def _face_dominated_by_coat_bones(
    face: bmesh.types.BMFace,
    ob: bpy.types.Object,
    *,
    exclude_head_neck: bool,
) -> bool:
    coat = 0.0
    bad = 0.0
    for v in face.verts:
        coat += _vg_weight(ob, v.index, _VG_COAT_BODY)
        coat += _vg_weight(ob, v.index, _VG_COAT_UPPER_ARM) * 0.85
        bad += _vg_weight(ob, v.index, _VG_COAT_EXCLUDE)
    if coat < 0.38:
        return False
    if exclude_head_neck and bad > 0.12:
        return False
    if bad > coat * 0.35:
        return False
    return True


def _world_centroid(face: bmesh.types.BMFace, ob: bpy.types.Object) -> Vector:
    wc = Vector()
    for v in face.verts:
        wc += ob.matrix_world @ v.co
    return wc / len(face.verts)


def _all_verts_under_chin(
    face: bmesh.types.BMFace,
    ob: bpy.types.Object,
    lm,
    chin_h: float,
) -> bool:
    lo, span = lm.zmin, lm.span
    chin_z = lo + span * chin_h
    for v in face.verts:
        if (ob.matrix_world @ v.co).z > chin_z:
            return False
    return True


def _arm_coat_face(
    face: bmesh.types.BMFace,
    ob: bpy.types.Object,
    *,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> bool:
    arm = sum(
        _vg_weight(ob, v.index, _VG_COAT_UPPER_ARM)
        + _vg_weight(ob, v.index, _VG_COAT_FOREARM) * 0.9
        for v in face.verts
    )
    if arm < 0.85:
        return False
    if not _all_verts_under_chin(face, ob, lm, lm.h_neck - 0.14):
        return False
    for v in face.verts:
        if _vg_weight(ob, v.index, _VG_HANDS) > 0.14:
            return False
        if _vg_weight(ob, v.index, _VG_COAT_EXCLUDE) > 0.06:
            return False
    lo, span = lm.zmin, lm.span
    wc = _world_centroid(face, ob)
    h = (wc.z - lo) / span
    wrist = lm.h_waist - 0.06
    if h < wrist or h > lm.h_neck - 0.16:
        return False
    lat = lateral_from_front(wc, hc, fwd)
    thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
    return lat > thw + span * 0.008


def _append_coat_hem_quads(
    bm: bmesh.types.BMesh,
    body: bpy.types.Object,
    *,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> None:
    from founder_face_decals import _fwd_basis

    _, right = _fwd_basis(fwd)
    lo, span = lm.zmin, lm.span
    hw = _head_width_at(ws, lo, span, lm.h_eye)
    z0 = lo + span * (lm.h_waist + 0.01)
    z1 = lo + span * (lm.h_waist + 0.44)
    proud = hw * 0.11
    flare = hw * 0.15
    inv = body.matrix_world.inverted()

    def _lv(w: Vector) -> Vector:
        return inv @ w

    for side in (-1.0, 1.0):
        mid = hc + right * (side * hw * 0.22) + fwd * proud
        inner_lo = _lv(Vector((mid.x - right.x * hw * 0.05, mid.y - right.y * hw * 0.05, z0)))
        inner_hi = _lv(Vector((mid.x - right.x * hw * 0.05, mid.y - right.y * hw * 0.05, z1)))
        out = mid + right * (side * flare * 1.15)
        outer_lo = _lv(Vector((out.x, out.y, z0)))
        outer_hi = _lv(Vector((out.x, out.y, z1)))
        vs = [bm.verts.new(c) for c in (inner_lo, outer_lo, outer_hi, inner_hi)]
        bm.faces.new(tuple(vs))


def _is_coat_source_face(
    face: bmesh.types.BMFace,
    ob: bpy.types.Object,
    *,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
    y_face_fn,
) -> bool:
    if _arm_coat_face(face, ob, lm=lm, hc=hc, fwd=fwd, ws=ws):
        return True
    chest_w = sum(
        _vg_weight(ob, v.index, frozenset({"Chest", "UpperChest", "Spine"}))
        for v in face.verts
    )
    lo, span = lm.zmin, lm.span
    wc = _world_centroid(face, ob)
    h = (wc.z - lo) / span
    lat = lateral_from_front(wc, hc, fwd)
    thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
    if (
        chest_w >= 0.75
        and _all_verts_under_chin(face, ob, lm, lm.h_neck - 0.17)
        and lm.h_waist - 0.14 <= h <= lm.h_neck - 0.19
        and lat <= thw + span * 0.018
    ):
        y_face = y_face_fn(wc)
        if not (y_face > 0.07 and lat < thw * 0.11):
            for v in face.verts:
                if _vg_weight(ob, v.index, _VG_COAT_EXCLUDE) > 0.06:
                    break
            else:
                return True
    if not _face_dominated_by_coat_bones(face, ob, exclude_head_neck=True):
        return False
    coat_w = sum(
        _vg_weight(ob, v.index, _VG_COAT_BODY) + _vg_weight(ob, v.index, _VG_COAT_UPPER_ARM)
        for v in face.verts
    )
    lo, span = lm.zmin, lm.span
    wc = _world_centroid(face, ob)
    h = (wc.z - lo) / span
    if not _all_verts_under_chin(face, ob, lm, lm.h_neck - 0.17):
        return False
    for v in face.verts:
        if _vg_weight(ob, v.index, _VG_COAT_EXCLUDE) > 0.05:
            return False
    lo, span = lm.zmin, lm.span
    wc = _world_centroid(face, ob)
    h = (wc.z - lo) / span
    lat = lateral_from_front(wc, hc, fwd)
    thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
    hip_w = sum(_vg_weight(ob, v.index, frozenset({"Hips"})) for v in face.verts)
    if hip_w >= 0.85 and lm.h_waist - 0.12 <= h <= lm.h_waist + 0.06:
        y_face = y_face_fn(wc)
        if y_face > -0.22 and lat < thw * 1.08:
            return True
    if h < lm.h_waist - 0.14 or h > lm.h_neck - 0.20:
        return False
    y_face = y_face_fn(wc)
    if lat > thw + span * 0.016:
        return False
    if y_face < -0.36:
        return False
    if y_face > 0.05 and lat < thw * 0.26:
        return False
    return True


def _trim_coat_above_chin(
    coat: bpy.types.Object,
    body: bpy.types.Object,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
    y_face_fn,
) -> None:
    lo, span = lm.zmin, lm.span
    chin_z = lo + span * (lm.h_neck - 0.20)
    bm = bmesh.new()
    bm.from_mesh(coat.data)
    bm.faces.ensure_lookup_table()
    doomed: list[bmesh.types.BMFace] = []
    for face in bm.faces:
        wc = coat.matrix_world @ face.calc_center_median()
        if wc.z > chin_z:
            doomed.append(face)
            continue
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        y_face = y_face_fn(wc)
        if h > lm.h_neck - 0.24 and y_face > -0.05 and lat < thw * 0.95:
            doomed.append(face)
    if doomed:
        bmesh.ops.delete(bm, geom=doomed, context="FACES")
    bm.to_mesh(coat.data)
    coat.data.update()
    bm.free()


def _trim_coat_orphan_shards(
    coat: bpy.types.Object,
    body: bpy.types.Object,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
    y_face_fn,
) -> None:
    """Remove floating hip / forearm coat islands."""
    lo, span = lm.zmin, lm.span
    bm = bmesh.new()
    bm.from_mesh(coat.data)
    bm.faces.ensure_lookup_table()
    doomed: list[bmesh.types.BMFace] = []
    for face in bm.faces:
        if len(face.verts) < 3:
            doomed.append(face)
            continue
        wc = coat.matrix_world @ face.calc_center_median()
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if (
            lm.h_waist + 0.21 <= h <= lm.h_waist + 0.30
            and lat > thw * 1.08
            and y_face_fn(wc) < 0.05
        ):
            doomed.append(face)
        if h < lm.h_waist - 0.06 and lat > thw * 0.62:
            doomed.append(face)
    if doomed:
        bmesh.ops.delete(bm, geom=doomed, context="FACES")
    bm.to_mesh(coat.data)
    coat.data.update()
    bm.free()


def spawn_researcher_lab_coat(body: bpy.types.Object) -> bpy.types.Object:
    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH" and ob.name.split(".")[0] == "ResearcherLabCoat":
            bpy.data.objects.remove(ob, do_unlink=True)
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, y_face_fn = face_shell_coord(body, fwd)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    bm_src = bmesh.new()
    bm_src.from_mesh(body.data)
    bm_src.faces.ensure_lookup_table()
    lo, span = lm.zmin, lm.span
    coat_lo = lm.h_waist - 0.12
    coat_hi = lm.h_waist + 0.50
    pick: list[bmesh.types.BMFace] = []
    for f in bm_src.faces:
        wc = _world_centroid(f, body)
        h = (wc.z - lo) / span
        if h > lm.h_neck + 0.04 or h < coat_lo or h > coat_hi:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        y_face = y_face_fn(wc)
        open_chest = y_face > 0.07 and lat < thw * 0.26 and lm.h_neck <= h <= lm.h_waist + 0.08
        on_torso = lat <= thw * 1.22 and y_face > -0.38 and not open_chest
        on_arm = (
            lat > thw * 0.72
            and lm.h_neck - 0.06 <= h <= lm.h_waist + 0.12
            and y_face > -0.35
        )
        on_hip_hem = lm.h_waist - 0.04 <= h <= coat_hi and lat <= thw * 1.18 and y_face > -0.20
        if open_chest:
            continue
        if on_torso or on_arm or on_hip_hem or _is_coat_source_face(
            f, body, lm=lm, hc=hc, fwd=fwd, ws=ws, y_face_fn=y_face_fn
        ):
            pick.append(f)
    if len(pick) < 24:
        raise RuntimeError(f"FAIL staff-researcher-idle lab_coat_faces={len(pick)} too_few")

    bm = bmesh.new()
    vmap: dict[int, bmesh.types.BMVert] = {}
    for sf in pick:
        new_vs = []
        for sv in sf.verts:
            if sv.index not in vmap:
                vmap[sv.index] = bm.verts.new(sv.co.copy())
            new_vs.append(vmap[sv.index])
        try:
            bm.faces.new(new_vs)
        except ValueError:
            pass

    mesh = bpy.data.meshes.new("ResearcherLabCoatMesh")
    bm.to_mesh(mesh)
    bm.free()
    bm_src.free()

    coat = bpy.data.objects.new("ResearcherLabCoat", mesh)
    bpy.context.collection.objects.link(coat)
    mat = _ensure_material("ResearcherLabCoatGeo", LAB_COAT_RGB, 0.68)
    coat.data.materials.append(mat)
    coat.parent = body
    coat.matrix_parent_inverse = body.matrix_world.inverted()
    thick = max(body.dimensions.length * 0.022, 0.016)
    solid = coat.modifiers.new("CoatSolidify", "SOLIDIFY")
    solid.thickness = thick
    solid.offset = 1.0
    bpy.context.view_layer.objects.active = coat
    coat.select_set(True)
    bpy.ops.object.modifier_apply(modifier=solid.name)
    bpy.ops.object.shade_smooth()
    bpy.ops.object.select_all(action="DESELECT")
    coat.select_set(True)
    bpy.context.view_layer.objects.active = coat
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.remove_doubles(threshold=0.0008)
    bpy.ops.mesh.normals_make_consistent(inside=False)
    bpy.ops.object.mode_set(mode="OBJECT")
    if len(coat.data.polygons) < 8:
        raise RuntimeError(f"FAIL staff-researcher-idle lab_coat_faces_post={len(coat.data.polygons)}")
    return coat


def spawn_researcher_hair_cap(body: bpy.types.Object) -> bpy.types.Object:
    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH" and ob.name.split(".")[0] == "ResearcherHairCap":
            bpy.data.objects.remove(ob, do_unlink=True)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    head_top = _researcher_head_top(ws, lm)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    crown_z = lm.zmin + lm.span * (head_top - 0.04)
    back = hc - fwd * (head_w * 0.36)
    local = body.matrix_world.inverted() @ Vector((back.x, back.y, crown_z))
    bpy.ops.mesh.primitive_uv_sphere_add(
        segments=14,
        ring_count=10,
        radius=head_w * 0.40,
        location=local,
    )
    cap = bpy.context.active_object
    cap.name = "ResearcherHairCap"
    cap.scale = (0.92, 0.72, 0.48)
    bpy.ops.object.transform_apply(scale=True)
    cap.parent = body
    cap.matrix_parent_inverse = body.matrix_world.inverted()
    mat = _ensure_material("ResearcherHairCapMat", RESEARCHER_HAIR, 0.52)
    cap.data.materials.append(mat)
    bc.smooth_shade(cap)
    return cap


def spawn_researcher_glasses(body: bpy.types.Object) -> bpy.types.Object:
    from founder_face_decals import _fwd_basis, _parent_keep_world

    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH" and ob.name.split(".")[0].startswith("ResearcherGlasses"):
            bpy.data.objects.remove(ob, do_unlink=True)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    _, right = _fwd_basis(fwd)
    head_top = _researcher_head_top(ws, lm)
    eye_h = _researcher_eye_h(ws, lm, head_top)
    eye_z = lm.zmin + lm.span * eye_h
    sep = head_w * 0.178
    depth = head_w * 0.028
    lens_r = head_w * 0.078
    mat = _ensure_material("ResearcherGlassesFrame", GLASSES_RGB, 0.08, specular=0.05)
    proud = head_w * 0.19
    rot = fwd.to_track_quat("Y", "Z").to_euler()
    parts: list[bpy.types.Object] = []

    def _rim(center: Vector, name: str) -> bpy.types.Object:
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=center)
        ob = bpy.context.active_object
        ob.name = name
        ob.scale = (lens_r * 2.1, depth, lens_r * 1.08)
        ob.rotation_euler = rot
        bpy.ops.object.transform_apply(scale=True, rotation=True)
        ob.data.materials.append(mat)
        bc.smooth_shade(ob)
        parts.append(ob)
        return ob

    anchors: list[Vector] = []
    for sign in (-1.0, 1.0):
        lat = right * (sep * sign)
        origin = hc + lat + Vector((0.0, 0.0, eye_z - hc.z)) + fwd * proud
        anchors.append(origin)
    _rim(anchors[0], "ResearcherGlasses.L")
    _rim(anchors[1], "ResearcherGlasses.R")
    mid = (anchors[0] + anchors[1]) * 0.5
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=mid)
    bridge = bpy.context.active_object
    bridge.name = "ResearcherGlasses.bridge"
    bridge.scale = (sep * 0.32, depth * 1.15, depth * 2.4)
    bridge.rotation_euler = rot
    bpy.ops.object.transform_apply(scale=True, rotation=True)
    bridge.data.materials.append(mat)
    parts.append(bridge)

    bpy.ops.object.select_all(action="DESELECT")
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = parts[0]
    bpy.ops.object.join()
    glasses = bpy.context.active_object
    glasses.name = "ResearcherGlasses"
    bc.smooth_shade(glasses)
    bpy.context.view_layer.objects.active = glasses
    glasses.select_set(True)
    bpy.ops.object.origin_set(type="ORIGIN_GEOMETRY", center="MEDIAN")
    _parent_keep_world(glasses, body)
    return glasses