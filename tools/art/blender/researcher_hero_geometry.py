"""Researcher hero — lab coat, hair cap, glasses as separate CC0 Blender geometry."""
from __future__ import annotations

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc
from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward
from kenney_geometry_landmarks import compute_landmarks, torso_half_width
from kenney_researcher_stock_recolor import (
    RESEARCHER_GLASSES,
    RESEARCHER_HAIR,
    _ensure_material,
    _head_width_at,
)

LAB_COAT_RGB = (228 / 255, 227 / 255, 221 / 255, 1.0)


def _world_centroid(face: bmesh.types.BMFace, ob: bpy.types.Object) -> Vector:
    wc = Vector()
    for v in face.verts:
        wc += ob.matrix_world @ v.co
    return wc / len(face.verts)


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
    lo, span = lm.zmin, lm.span
    wc = _world_centroid(face, ob)
    h = (wc.z - lo) / span
    if h < lm.h_neck - 0.05 or h > lm.h_waist + 0.38:
        return False
    y_face = y_face_fn(wc)
    lat = lateral_from_front(wc, hc, fwd)
    thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
    arm = lat > thw + span * 0.012
    if not arm and y_face < -0.12:
        return False
    if y_face > 0.10 and lat < thw * 0.20:
        return False
    return True


def spawn_researcher_lab_coat(body: bpy.types.Object) -> bpy.types.Object:
    """Extruded shell over torso + upper arms (open V front), parented to body."""
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
    thick = max(body.dimensions.length * 0.016, 0.007)
    solid = coat.modifiers.new("CoatSolidify", "SOLIDIFY")
    solid.thickness = thick
    solid.offset = 1.0
    bpy.context.view_layer.objects.active = coat
    coat.select_set(True)
    bpy.ops.object.modifier_apply(modifier=solid.name)
    bpy.ops.object.shade_smooth()
    return coat


def spawn_researcher_hair_cap(body: bpy.types.Object) -> bpy.types.Object:
    """TPH-style volume cap; does not alter head skin albedo."""
    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH" and ob.name.split(".")[0] == "ResearcherHairCap":
            bpy.data.objects.remove(ob, do_unlink=True)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    crown_z = lm.zmin + lm.span * (lm.h_neck + 0.14)
    local = body.matrix_world.inverted() @ Vector((hc.x, hc.y, crown_z))
    bpy.ops.mesh.primitive_uv_sphere_add(
        segments=12,
        ring_count=8,
        radius=head_w * 0.46,
        location=local,
    )
    cap = bpy.context.active_object
    cap.name = "ResearcherHairCap"
    cap.scale = (1.05, 0.95, 0.62)
    bpy.ops.object.transform_apply(scale=True)
    cap.parent = body
    cap.matrix_parent_inverse = body.matrix_world.inverted()
    mat = _ensure_material("ResearcherHairCapMat", RESEARCHER_HAIR, 0.55)
    cap.data.materials.append(mat)
    bc.smooth_shade(cap)
    return cap


def spawn_researcher_glasses(body: bpy.types.Object) -> bpy.types.Object:
    """Thin frames, bridge, temples — in front of eyes for 3/4 idle."""
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
    sep = head_w * 0.175
    depth = head_w * 0.006
    lens_r = head_w * 0.062
    mat = _ensure_material("ResearcherGlassesFrame", RESEARCHER_GLASSES, 0.25, specular=0.25)
    parts: list[bpy.types.Object] = []

    def _ring(center: Vector, name: str) -> bpy.types.Object:
        bpy.ops.mesh.primitive_torus_add(
            major_radius=lens_r,
            minor_radius=depth,
            major_segments=14,
            minor_segments=6,
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

    proud = head_w * 0.11
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
    bridge.scale = (sep * 0.20, depth * 1.4, depth * 2.2)
    bridge.rotation_euler = fwd_xy.to_track_quat("Y", "Z").to_euler()
    bpy.ops.object.transform_apply(scale=True, rotation=True)
    bridge.data.materials.append(mat)
    parts.append(bridge)
    for i, anchor in enumerate(anchors):
        side = -1.0 if i == 0 else 1.0
        temple_end = anchor + right * (head_w * 0.14 * side) + fwd_xy * (head_w * -0.04)
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(anchor + temple_end) * 0.5)
        temple = bpy.context.active_object
        temple.name = f"ResearcherGlasses.temple{i}"
        temple.scale = (head_w * 0.07, depth, depth * 1.1)
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