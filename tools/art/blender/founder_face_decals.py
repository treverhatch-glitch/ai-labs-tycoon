"""Founder eyes/brows as separate mesh objects (visible in GLB + previews)."""
from __future__ import annotations

import bpy
import bmesh
from mathutils import Vector
from mathutils.bvhtree import BVHTree

import blender_common as bc
from kenney_face_axis import face_shell_coord, toe_forward
from kenney_geometry_landmarks import compute_landmarks

FOUNDER_EYE_RGBA = (26 / 255, 26 / 255, 26 / 255, 1.0)
FOUNDER_BROW_RGBA = (0.20, 0.16, 0.14, 1.0)
FOUNDER_MOUTH_RGBA = (0.18, 0.12, 0.12, 1.0)

EYE_OBJECT_NAMES = ("FounderEye_L", "FounderEye_R")
HEAD_ANCHOR_NAME = "FounderHeadAnchor"


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


def _purge_face_objects() -> None:
    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH":
            base = ob.name.split(".")[0]
            if base.startswith(("FounderEye_", "FounderBrow_", "FounderMouth")):
                bpy.data.objects.remove(ob, do_unlink=True)
        elif ob.type == "EMPTY" and ob.name.split(".")[0] == HEAD_ANCHOR_NAME:
            bpy.data.objects.remove(ob, do_unlink=True)


def _make_solid_mat(name: str, rgba: tuple[float, float, float, float], rough: float) -> bpy.types.Material:
    mat = bpy.data.materials.get(name)
    if mat:
        bpy.data.materials.remove(mat)
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = rgba
        bsdf.inputs["Roughness"].default_value = rough
    return mat


def _body_bvh_local(body: bpy.types.Object) -> BVHTree:
    bm = bmesh.new()
    bm.from_mesh(body.data)
    bm.transform(body.matrix_world)
    tree = BVHTree.FromBMesh(bm)
    bm.free()
    return tree


def _ray_hit_face(
    body: bpy.types.Object, bvh: BVHTree, origin_w: Vector, direction_w: Vector
) -> tuple[Vector, Vector] | None:
    direction_w = direction_w.normalized()
    for scale in (2.5, 2.0, 1.5, 3.0):
        loc, normal, _idx, _dist = bvh.ray_cast(origin_w, direction_w, scale)
        if loc is not None:
            return loc, normal.normalized()
    return None


def _ensure_head_anchor(body: bpy.types.Object, head_center_w: Vector) -> bpy.types.Object:
    old = bpy.data.objects.get(HEAD_ANCHOR_NAME)
    if old:
        bpy.data.objects.remove(old, do_unlink=True)
    empty = bpy.data.objects.new(HEAD_ANCHOR_NAME, None)
    bpy.context.scene.collection.objects.link(empty)
    empty.parent = body
    empty.empty_display_size = 0.05
    empty.location = body.matrix_world.inverted() @ head_center_w
    empty.matrix_parent_inverse = body.matrix_world.inverted()
    return empty


def _parent_eye_disc(
    name: str,
    center_w: Vector,
    face_fwd: Vector,
    radius: float,
    proud: float,
    mat: bpy.types.Material,
    anchor: bpy.types.Object,
) -> bpy.types.Object:
    body = anchor.parent
    if not body:
        raise RuntimeError("FounderHeadAnchor missing body parent")
    bpy.ops.mesh.primitive_uv_sphere_add(segments=12, ring_count=8, location=(0.0, 0.0, 0.0))
    ob = bpy.context.active_object
    ob.name = name
    thick = radius * 0.5
    ob.scale = (radius, thick, radius)
    bpy.ops.object.transform_apply(scale=True)
    ob.parent = anchor
    f_world = face_fwd.normalized()
    f_local = (anchor.matrix_world.inverted().to_3x3() @ f_world).normalized()
    ob.rotation_mode = "QUATERNION"
    ob.rotation_quaternion = f_local.to_track_quat("Y", "Z")
    center_local = anchor.matrix_world.inverted() @ center_w
    proud_local = ob.rotation_quaternion @ Vector((0.0, proud, 0.0))
    ob.location = center_local + proud_local
    ob.matrix_parent_inverse = anchor.matrix_world.inverted()
    ob.data.materials.clear()
    ob.data.materials.append(mat)
    bc.smooth_shade(ob)
    return ob


def spawn_founder_eye_meshes(body: bpy.types.Object) -> list[bpy.types.Object]:
    """Two #1a1a1a discs on head shell (BVH raycast, proud along surface normal)."""
    _purge_face_objects()
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    f_xy, right = _fwd_basis(fwd)
    eye_z = lm.zmin + lm.span * lm.h_eye
    proud = head_w * 0.012
    sep = head_w * 0.17
    radius = head_w * 0.034
    cast_in = -fwd
    eye_mat = _make_solid_mat("FounderEyeMesh", FOUNDER_EYE_RGBA, 0.88)
    anchor = _ensure_head_anchor(body, hc)
    bvh = _body_bvh_local(body)
    created: list[bpy.types.Object] = []
    eye_anchor = hc + Vector((0.0, 0.0, eye_z - hc.z))
    shoot_dist = max(head_w * 0.65, 0.15)
    for name, sign in zip(EYE_OBJECT_NAMES, (-1.0, 1.0), strict=True):
        lateral = right * (sep * sign)
        origin = eye_anchor + lateral + fwd * shoot_dist
        hit = _ray_hit_face(body, bvh, origin, cast_in)
        if hit is None:
            center = eye_anchor + lateral + fwd * (head_w * 0.09)
        else:
            loc, norm = hit
            outward = norm if norm.dot(fwd) >= 0 else -norm
            center = loc + outward * proud
        created.append(
            _parent_eye_disc(name, center, fwd, radius, proud, eye_mat, anchor)
        )
    return created


def attach_founder_face_decals(ob: bpy.types.Object) -> list[bpy.types.Object]:
    """Legacy entry: only separate eye meshes (no painted polys on head)."""
    return spawn_founder_eye_meshes(ob)


def founder_preview_meshes(body: bpy.types.Object) -> list[bpy.types.Object]:
    eyes = spawn_founder_eye_meshes(ob)
    anchor = bpy.data.objects.get(HEAD_ANCHOR_NAME)
    if anchor:
        return [body, anchor, *eyes]
    return [body, *eyes]