"""Founder eyes as separate meshes parented to body (GLB-safe world matrices)."""
from __future__ import annotations

import bpy
import bmesh
from mathutils import Matrix, Vector
from mathutils.bvhtree import BVHTree

import blender_common as bc
from kenney_face_axis import face_shell_coord, toe_forward
from kenney_geometry_landmarks import compute_landmarks

FOUNDER_EYE_RGBA = (26 / 255, 26 / 255, 26 / 255, 1.0)

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


def _body_bvh_evaluated(body: bpy.types.Object) -> BVHTree:
    deps = bpy.context.evaluated_depsgraph_get()
    eval_ob = body.evaluated_get(deps)
    mesh = eval_ob.to_mesh()
    bm = bmesh.new()
    bm.from_mesh(mesh)
    bm.transform(eval_ob.matrix_world)
    eval_ob.to_mesh_clear()
    tree = BVHTree.FromBMesh(bm)
    bm.free()
    return tree


def _ray_hit_face(
    bvh: BVHTree, origin_w: Vector, direction_w: Vector
) -> tuple[Vector, Vector] | None:
    direction_w = direction_w.normalized()
    for dist in (2.5, 2.0, 3.0, 1.5):
        loc, normal, _idx, _d = bvh.ray_cast(origin_w, direction_w, dist)
        if loc is not None:
            return loc, normal.normalized()
    return None


def _eye_world_matrix(center_w: Vector, face_fwd: Vector, radius: float, proud: float) -> bpy.types.Object:
    bpy.ops.mesh.primitive_uv_sphere_add(segments=12, ring_count=8, location=(0.0, 0.0, 0.0))
    ob = bpy.context.active_object
    thick = radius * 0.5
    ob.scale = (radius, thick, radius)
    bpy.ops.object.transform_apply(scale=True)
    fwd = face_fwd.normalized()
    rot4 = fwd.to_track_quat("Y", "Z").to_matrix().to_4x4()
    ob.matrix_world = Matrix.Translation(center_w) @ rot4
    return ob


def _parent_keep_world(ob: bpy.types.Object, parent: bpy.types.Object) -> None:
    world = ob.matrix_world.copy()
    ob.parent = parent
    ob.matrix_parent_inverse = parent.matrix_world.inverted()
    ob.matrix_world = world


def spawn_founder_eye_meshes(body: bpy.types.Object) -> list[bpy.types.Object]:
    _purge_face_objects()
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    _, right = _fwd_basis(fwd)
    eye_z = lm.zmin + lm.span * lm.h_eye
    proud = head_w * 0.012
    sep = head_w * 0.17
    radius = head_w * 0.034
    if body.name.split(".")[0] == "KenneyResearcherStock":
        sep = head_w * 0.21
        proud = head_w * 0.055
        radius = head_w * 0.052
    cast_in = -fwd
    eye_mat = _make_solid_mat("FounderEyeMesh", FOUNDER_EYE_RGBA, 0.88)
    bvh = _body_bvh_evaluated(body)
    created: list[bpy.types.Object] = []
    eye_anchor = hc + Vector((0.0, 0.0, eye_z - hc.z))
    shoot_dist = max(head_w * 0.55, 0.14)
    for name, sign in zip(EYE_OBJECT_NAMES, (-1.0, 1.0), strict=True):
        lateral = right * (sep * sign)
        origin = eye_anchor + lateral + fwd * shoot_dist
        hit = _ray_hit_face(bvh, origin, cast_in)
        if hit is None:
            seed = eye_anchor + lateral + fwd * (head_w * 0.04)
            face_fwd = fwd
        else:
            loc, norm = hit
            seed = loc
            outward = norm if norm.dot(fwd) >= 0 else -norm
            face_fwd = outward
        nearest, norm_n, _idx, _dist = bvh.find_nearest(seed)
        if nearest is not None:
            outward = norm_n if norm_n.dot(fwd) >= 0 else -norm_n
            center = nearest + outward * proud
            face_fwd = outward
        else:
            center = seed + face_fwd * proud
        ob = _eye_world_matrix(center, face_fwd, radius, proud)
        ob.name = name
        ob.data.materials.clear()
        ob.data.materials.append(eye_mat)
        bc.smooth_shade(ob)
        _parent_keep_world(ob, body)
        created.append(ob)
    return created


def attach_founder_face_decals(ob: bpy.types.Object) -> list[bpy.types.Object]:
    return spawn_founder_eye_meshes(ob)


def founder_preview_meshes(body: bpy.types.Object) -> list[bpy.types.Object]:
    return [body, *spawn_founder_eye_meshes(body)]


def freeze_eyes_for_gltf_export(mesh_obs: list[bpy.types.Object]) -> None:
    """glTF export with apply_transform=False drops parent-child offsets; root the eyes at world."""
    for ob in mesh_obs:
        if ob.type != "MESH" or not ob.name.split(".")[0].startswith("FounderEye_"):
            continue
        world = ob.matrix_world.copy()
        ob.parent = None
        ob.matrix_world = world