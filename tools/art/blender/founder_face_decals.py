"""Founder eyes/brows as separate mesh objects (visible in GLB + previews)."""
from __future__ import annotations

import bpy
from mathutils import Vector

import blender_common as bc
from kenney_face_axis import face_shell_coord, toe_forward
from kenney_geometry_landmarks import compute_landmarks

FOUNDER_EYE_RGBA = (26 / 255, 26 / 255, 26 / 255, 1.0)
FOUNDER_BROW_RGBA = (0.20, 0.16, 0.14, 1.0)
FOUNDER_MOUTH_RGBA = (0.18, 0.12, 0.12, 1.0)

EYE_OBJECT_NAMES = ("FounderEye_L", "FounderEye_R")


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
        if ob.type != "MESH":
            continue
        base = ob.name.split(".")[0]
        if base.startswith(("FounderEye_", "FounderBrow_", "FounderMouth")):
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


def _ray_hit_body(
    body: bpy.types.Object, origin: Vector, direction: Vector
) -> tuple[Vector, Vector] | None:
    direction = direction.normalized()
    deps = bpy.context.evaluated_depsgraph_get()
    for dist_mult in (1.0, 0.85, 1.1, 0.7):
        start = origin
        end = origin + direction * (body.dimensions.length * dist_mult + 0.5)
        cast = (end - start).normalized()
        hit, loc, norm, _fi, obj, _mat = bpy.context.scene.ray_cast(deps, start, cast)
        if hit and obj == body:
            return loc, norm
    return None


def _parent_eye_disc(
    name: str,
    center_w: Vector,
    face_fwd: Vector,
    radius: float,
    proud: float,
    mat: bpy.types.Material,
    parent: bpy.types.Object,
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_uv_sphere_add(segments=12, ring_count=8, location=(0.0, 0.0, 0.0))
    ob = bpy.context.active_object
    ob.name = name
    thick = radius * 0.55
    ob.scale = (radius, thick, radius)
    bpy.ops.object.transform_apply(scale=True)
    ob.parent = parent
    f_local = (parent.matrix_world.inverted().to_3x3() @ face_fwd).normalized()
    ob.rotation_mode = "QUATERNION"
    ob.rotation_quaternion = f_local.to_track_quat("Y", "Z")
    center_local = parent.matrix_world.inverted() @ center_w
    proud_local = ob.rotation_quaternion @ Vector((0.0, proud, 0.0))
    ob.location = center_local + proud_local
    ob.matrix_parent_inverse = parent.matrix_world.inverted()
    ob.data.materials.clear()
    ob.data.materials.append(mat)
    bc.smooth_shade(ob)
    return ob


def spawn_founder_eye_meshes(body: bpy.types.Object) -> list[bpy.types.Object]:
    """Two #1a1a1a discs on face shell via raycast (same forward as 0° diagnostic)."""
    _purge_face_objects()
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    f_xy, right = _fwd_basis(fwd)
    eye_z = lm.zmin + lm.span * lm.h_eye
    proud = head_w * 0.015
    sep = head_w * 0.20
    radius = head_w * 0.04
    cast_in = -f_xy
    eye_mat = _make_solid_mat("FounderEyeMesh", FOUNDER_EYE_RGBA, 0.88)
    created: list[bpy.types.Object] = []
    eye_anchor = hc + Vector((0.0, 0.0, eye_z - hc.z))
    shoot_dist = max(head_w * 0.55, 0.12)
    for name, sign in zip(EYE_OBJECT_NAMES, (-1.0, 1.0), strict=True):
        lateral = right * (sep * sign)
        origin = eye_anchor + lateral + f_xy * shoot_dist
        hit = _ray_hit_body(body, origin, cast_in)
        if hit is None:
            center = eye_anchor + lateral + f_xy * (head_w * 0.11 + proud)
        else:
            loc, _norm = hit
            center = loc - f_xy * proud
            center.z = eye_z
        created.append(
            _parent_eye_disc(name, center, f_xy, radius, proud, eye_mat, body)
        )
    return created


def attach_founder_face_decals(ob: bpy.types.Object) -> list[bpy.types.Object]:
    """Legacy entry: only separate eye meshes (no painted polys on head)."""
    return spawn_founder_eye_meshes(ob)


def founder_preview_meshes(body: bpy.types.Object) -> list[bpy.types.Object]:
    eyes = spawn_founder_eye_meshes(body)
    return [body, *eyes]