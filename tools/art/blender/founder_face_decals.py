"""Separate eye/brow/mouth mesh islands on founder stock (no painted face polys)."""
from __future__ import annotations

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc
import kenney_founder_lib as kf
from kenney_face_axis import face_shell_coord, toe_forward
from kenney_geometry_landmarks import compute_landmarks

FOUNDER_BROW_RGBA = (0.20, 0.16, 0.14, 1.0)
FOUNDER_MOUTH_RGBA = (0.18, 0.12, 0.12, 1.0)


def _head_width_at(ws: list[Vector], lo: float, span: float, h: float) -> float:
    pts = [p for p in ws if abs((p.z - lo) / span - h) < 0.015]
    if len(pts) < 4:
        return span * 0.18
    xs = [p.x for p in pts]
    ys = [p.y for p in pts]
    return max(max(xs) - min(xs), max(ys) - min(ys))


def _fwd_basis(fwd: Vector) -> tuple[Vector, Vector]:
    f_xy = Vector((fwd.x, fwd.y, 0.0))
    if f_xy.length < 1e-5:
        f_xy = Vector((0.0, -1.0, 0.0))
    else:
        f_xy.normalize()
    right = Vector((-f_xy.y, f_xy.x, 0.0))
    return f_xy, right


def _add_cube_faces(
    bm: bmesh.types.BMesh,
    ob: bpy.types.Object,
    center_world: Vector,
    size: Vector,
    mat_index: int,
) -> None:
    inv = ob.matrix_world.inverted()
    center = inv @ center_world
    half = size * 0.5
    bmesh.ops.create_cube(bm, size=1.0)
    new_faces = bm.faces[-6:]
    new_verts = {v for f in new_faces for v in f.verts}
    for v in new_verts:
        v.co = Vector(
            (
                (v.co.x * 2 - 1) * half.x,
                (v.co.y * 2 - 1) * half.y,
                (v.co.z * 2 - 1) * half.z,
            )
        )
        v.co = center + v.co
    for f in new_faces:
        f.material_index = mat_index


def attach_founder_face_decals(ob: bpy.types.Object) -> None:
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    lm = compute_landmarks(ob)
    fwd = toe_forward([ob])
    hc, _ = face_shell_coord(ob, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    f_xy, right = _fwd_basis(fwd)

    shell = head_w * 0.11
    eye_z = lm.zmin + lm.span * lm.h_eye
    face_push = f_xy * shell
    eye_half = Vector((head_w * 0.04, head_w * 0.035, head_w * 0.032))
    sep = head_w * 0.20

    def mat_index(name: str, rgba: tuple[float, float, float, float], rough: float) -> int:
        for i, m in enumerate(ob.data.materials):
            if m and m.name == name:
                return i
        mat = bpy.data.materials.new(name=name)
        mat.use_nodes = True
        bsdf = mat.node_tree.nodes.get("Principled BSDF")
        if bsdf:
            bsdf.inputs["Base Color"].default_value = rgba
            bsdf.inputs["Roughness"].default_value = rough
        ob.data.materials.append(mat)
        return len(ob.data.materials) - 1

    eye_i = mat_index("FounderEye", kf.FOUNDER_EYE, 0.32)
    brow_i = mat_index("FounderBrow", FOUNDER_BROW_RGBA, 0.36)
    mouth_i = mat_index("FounderMouth", FOUNDER_MOUTH_RGBA, 0.40)

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()

    for sign in (-1.0, 1.0):
        center = hc + face_push + right * (sep * sign)
        center.z = eye_z
        _add_cube_faces(bm, ob, center, eye_half * 2, eye_i)

    brow_z = eye_z + lm.span * 0.042
    brow_half = Vector((head_w * 0.055, head_w * 0.028, head_w * 0.025))
    for sign in (-1.0, 1.0):
        center = hc + face_push * 1.05 + right * (sep * sign)
        center.z = brow_z
        _add_cube_faces(bm, ob, center, brow_half * 2, brow_i)

    mouth_z = eye_z - lm.span * 0.085
    mouth_half = Vector((head_w * 0.05, head_w * 0.028, head_w * 0.022))
    mouth_c = hc + face_push * 0.92
    mouth_c.z = mouth_z
    _add_cube_faces(bm, ob, mouth_c, mouth_half * 2, mouth_i)

    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()
    bc.smooth_shade(ob)