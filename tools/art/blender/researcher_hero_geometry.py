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

_VG_COAT_BODY = frozenset(
    {"Hips", "Spine", "Chest", "UpperChest", "LeftShoulder", "RightShoulder"}
)
_VG_COAT_UPPER_ARM = frozenset({"LeftArm", "RightArm"})
_VG_COAT_THIGH = frozenset({"LeftUpLeg", "RightUpLeg"})
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
    if coat < 0.55:
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


def _face_thigh_drape(
    face: bmesh.types.BMFace,
    ob: bpy.types.Object,
    *,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
    y_face_fn,
) -> bool:
    leg = sum(_vg_weight(ob, v.index, _VG_COAT_THIGH) for v in face.verts)
    if leg < 0.65:
        return False
    if not _all_verts_under_chin(face, ob, lm, lm.h_neck - 0.14):
        return False
    lo, span = lm.zmin, lm.span
    wc = _world_centroid(face, ob)
    h = (wc.z - lo) / span
    if h < lm.h_waist - 0.05 or h > lm.h_waist + 0.46:
        return False
    y_face = y_face_fn(wc)
    if y_face < -0.55:
        return False
    lat = lateral_from_front(wc, hc, fwd)
    thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
    return lat < thw * 1.12


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
    if _face_thigh_drape(face, ob, lm=lm, hc=hc, fwd=fwd, ws=ws, y_face_fn=y_face_fn):
        return True
    if not _face_dominated_by_coat_bones(face, ob, exclude_head_neck=True):
        return False
    if not _all_verts_under_chin(face, ob, lm, lm.h_neck - 0.16):
        return False
    for v in face.verts:
        if _vg_weight(ob, v.index, _VG_COAT_EXCLUDE) > 0.05:
            return False
    lo, span = lm.zmin, lm.span
    wc = _world_centroid(face, ob)
    h = (wc.z - lo) / span
    if h < lm.h_waist - 0.06 or h > lm.h_waist + 0.12:
        return False
    if h > lm.h_neck - 0.16:
        return False
    y_face = y_face_fn(wc)
    lat = lateral_from_front(wc, hc, fwd)
    thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
    arm = lat > thw + span * 0.012
    sleeve_top = lm.h_waist + 0.10
    if arm and h > sleeve_top:
        return False
    if not arm and y_face < -0.12:
        return False
    if y_face > 0.06 and lat < thw * 0.22:
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
    chin_z = lo + span * (lm.h_neck - 0.18)
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


def _add_thigh_coat_panels(
    coat: bpy.types.Object,
    body: bpy.types.Object,
    *,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> None:
    from founder_face_decals import _fwd_basis

    lo, span = lm.zmin, lm.span
    head_w = _head_width_at(ws, lo, span, lm.h_eye)
    waist_z = lo + span * (lm.h_waist + 0.06)
    hem_z = lo + span * (lm.h_waist + 0.41)
    depth = head_w * 0.045
    _, right = _fwd_basis(fwd)
    panels: list[bpy.types.Object] = []
    for sign in (-1.0, 1.0):
        center = hc + right * (sign * head_w * 0.18) + fwd * (head_w * 0.14)
        center.z = waist_z + (hem_z - waist_z) * 0.58
        local = body.matrix_world.inverted() @ center
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=local)
        panel = bpy.context.active_object
        panel.scale = (head_w * 0.13, depth, (hem_z - waist_z) * 0.78)
        rot = fwd.to_track_quat("Y", "Z").to_euler()
        panel.rotation_euler = rot
        bpy.ops.object.transform_apply(scale=True, rotation=True)
        if coat.data.materials:
            panel.data.materials.append(coat.data.materials[0])
        panels.append(panel)
    bpy.ops.object.select_all(action="DESELECT")
    coat.select_set(True)
    for p in panels:
        p.select_set(True)
    bpy.context.view_layer.objects.active = coat
    bpy.ops.object.join()


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
    pick = [
        f
        for f in bm_src.faces
        if _is_coat_source_face(f, body, lm=lm, hc=hc, fwd=fwd, ws=ws, y_face_fn=y_face_fn)
    ]
    if len(pick) < 20:
        raise RuntimeError(f"FAIL staff-researcher-idle lab_coat_faces={len(pick)} too_few")

    bm = bmesh.new()
    vmap: dict[int, bmesh.types.BMVert] = {}
    for sf in pick:
        new_vs = []
        for sv in sf.verts:
            if sv.index not in vmap:
                vmap[sv.index] = bm.verts.new(sv.co.copy())
            new_vs.append(vmap[sv.index])
        bm.faces.new(new_vs)

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
    thick = max(body.dimensions.length * 0.011, 0.0045)
    solid = coat.modifiers.new("CoatSolidify", "SOLIDIFY")
    solid.thickness = thick
    solid.offset = 0.92
    bpy.context.view_layer.objects.active = coat
    coat.select_set(True)
    bpy.ops.object.modifier_apply(modifier=solid.name)
    bpy.ops.object.shade_smooth()
    _trim_coat_above_chin(coat, body, lm, hc, fwd, ws, y_face_fn)
    _add_thigh_coat_panels(coat, body, lm=lm, hc=hc, fwd=fwd, ws=ws)
    return coat


def spawn_researcher_hair_cap(body: bpy.types.Object) -> bpy.types.Object:
    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH" and ob.name.split(".")[0] == "ResearcherHairCap":
            bpy.data.objects.remove(ob, do_unlink=True)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    crown_z = lm.zmin + lm.span * (lm.h_neck + 0.20)
    local = body.matrix_world.inverted() @ Vector((hc.x, hc.y, crown_z))
    bpy.ops.mesh.primitive_uv_sphere_add(
        segments=14,
        ring_count=10,
        radius=head_w * 0.52,
        location=local,
    )
    cap = bpy.context.active_object
    cap.name = "ResearcherHairCap"
    cap.scale = (1.12, 1.02, 0.72)
    bpy.ops.object.transform_apply(scale=True)
    cap.parent = body
    cap.matrix_parent_inverse = body.matrix_world.inverted()
    mat = _ensure_material("ResearcherHairCapMat", RESEARCHER_HAIR, 0.52)
    cap.data.materials.append(mat)
    bc.smooth_shade(cap)
    return cap


def spawn_researcher_glasses(body: bpy.types.Object) -> bpy.types.Object:
    from founder_face_decals import _fwd_basis

    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH" and ob.name.split(".")[0].startswith("ResearcherGlasses"):
            bpy.data.objects.remove(ob, do_unlink=True)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    fwd_xy, right = _fwd_basis(fwd)
    eye_z = lm.zmin + lm.span * lm.h_eye
    sep = head_w * 0.178
    depth = head_w * 0.009
    lens_r = head_w * 0.065
    mat = _ensure_material("ResearcherGlassesFrame", GLASSES_RGB, 0.22, specular=0.18)
    parts: list[bpy.types.Object] = []

    def _ring(center: Vector, name: str) -> bpy.types.Object:
        bpy.ops.mesh.primitive_torus_add(
            major_radius=lens_r,
            minor_radius=depth,
            major_segments=16,
            minor_segments=8,
            location=center,
        )
        ob = bpy.context.active_object
        ob.name = name
        rot = fwd_xy.to_track_quat("Y", "Z").to_euler()
        ob.rotation_euler = rot
        ob.data.materials.append(mat)
        bc.smooth_shade(ob)
        parts.append(ob)
        return ob

    proud = head_w * 0.19
    anchors: list[Vector] = []
    for sign in (-1.0, 1.0):
        lat = right * (sep * sign)
        origin = hc + lat + Vector((0.0, 0.0, eye_z - hc.z)) + fwd * proud
        anchors.append(origin)
    _ring(anchors[0], "ResearcherGlasses.L")
    _ring(anchors[1], "ResearcherGlasses.R")
    mid = (anchors[0] + anchors[1]) * 0.5
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=mid)
    bridge = bpy.context.active_object
    bridge.name = "ResearcherGlasses.bridge"
    bridge.scale = (sep * 0.26, depth * 1.8, depth * 2.8)
    bridge.rotation_euler = fwd_xy.to_track_quat("Y", "Z").to_euler()
    bpy.ops.object.transform_apply(scale=True, rotation=True)
    bridge.data.materials.append(mat)
    parts.append(bridge)
    for i, anchor in enumerate(anchors):
        side = -1.0 if i == 0 else 1.0
        temple_end = anchor + right * (head_w * 0.15 * side) + fwd_xy * (head_w * -0.03)
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(anchor + temple_end) * 0.5)
        temple = bpy.context.active_object
        temple.name = f"ResearcherGlasses.temple{i}"
        temple.scale = (head_w * 0.075, depth * 1.2, depth * 1.4)
        temple.rotation_euler = fwd_xy.to_track_quat("Y", "Z").to_euler()
        bpy.ops.object.transform_apply(scale=True, rotation=True)
        temple.data.materials.append(mat)
        parts.append(temple)

    bpy.ops.object.select_all(action="DESELECT")
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = parts[0]
    bpy.ops.object.join()
    glasses = bpy.context.active_object
    glasses.name = "ResearcherGlasses"
    glasses.parent = body
    glasses.matrix_parent_inverse = body.matrix_world.inverted()
    return glasses