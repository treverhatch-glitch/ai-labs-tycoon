"""Kenney Animated Characters 3 (CC0) — founder hero via rig idle rest pose bake."""
from __future__ import annotations

import math

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc
from kenney_founder_standing_gate import verify_standing_rig

INCOMING = bc.REPO_ROOT / "tools/art/cc0/incoming"
KENNEY_FBX = INCOMING / "kenney/animated-characters-3/Model/characterMedium.fbx"
KENNEY_IDLE_FBX = INCOMING / "kenney/animated-characters-3/Animations/idle.fbx"
KENNEY_IDLE_REST_ACTION = "0.Targeting Pose"
KENNEY_IDLE_REST_FRAME = 0
TARGET_HEIGHT = 1.72
FOUNDER_SKIN = (0.84, 0.68, 0.54, 1.0)
FOUNDER_HOODIE = (0.10, 0.62, 0.55, 1.0)
FOUNDER_JEANS = (0.12, 0.28, 0.58, 1.0)
FOUNDER_SHOE = (0.20, 0.22, 0.26, 1.0)
FBX_KW = {
    "use_manual_orientation": True,
    "axis_forward": "-Z",
    "axis_up": "Y",
    "use_anim": True,
}


def _clear_scene() -> None:
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)


def _import_fbx(path) -> None:
    bpy.ops.import_scene.fbx(filepath=str(path), **FBX_KW)


def _apply_transforms(*objs: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    for o in objs:
        o.select_set(True)
    if objs:
        bpy.context.view_layer.objects.active = objs[0]
        bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)


def _import_rigged() -> tuple[bpy.types.Object, bpy.types.Object]:
    if not KENNEY_FBX.is_file():
        raise RuntimeError(f"missing Kenney FBX {KENNEY_FBX}")
    if not KENNEY_IDLE_FBX.is_file():
        raise RuntimeError(f"missing Kenney idle {KENNEY_IDLE_FBX}")
    _clear_scene()
    _import_fbx(KENNEY_FBX)
    arm = bpy.data.objects.get("Root")
    mesh = bpy.data.objects.get("characterMedium")
    if not arm or arm.type != "ARMATURE" or not mesh or mesh.type != "MESH":
        raise RuntimeError("Kenney rig import failed")
    _apply_transforms(arm, mesh)
    _import_fbx(KENNEY_IDLE_FBX)
    for extra in [o for o in bpy.context.scene.objects if o.type == "ARMATURE" and o != arm]:
        bpy.data.objects.remove(extra, do_unlink=True)
    return arm, mesh


def _ground_rig(arm: bpy.types.Object, mesh: bpy.types.Object) -> None:
    zs: list[float] = []
    for name in (
        "LeftFoot",
        "RightFoot",
        "LeftToes",
        "RightToes",
        "LeftHeelRoll",
        "RightHeelRoll",
    ):
        pb = arm.pose.bones.get(name)
        if pb:
            zs.append((arm.matrix_world @ pb.head).z)
            zs.append((arm.matrix_world @ pb.tail).z)
    if not zs:
        return
    dz = -min(zs)
    if abs(dz) < 1e-6:
        return
    arm.location.z += dz
    mesh.location.z += dz
    bpy.context.view_layer.update()


def _apply_idle_rest_pose(arm: bpy.types.Object) -> None:
    action = next(
        (a for a in bpy.data.actions if KENNEY_IDLE_REST_ACTION in a.name),
        None,
    )
    if not action:
        raise RuntimeError(f"missing Kenney idle rest action ({KENNEY_IDLE_REST_ACTION})")
    arm.animation_data_create()
    arm.animation_data.action = action
    bpy.context.scene.frame_set(KENNEY_IDLE_REST_FRAME)
    bpy.context.view_layer.update()


def _bake_mesh(mesh: bpy.types.Object, arm: bpy.types.Object) -> bpy.types.Object:
    deps = bpy.context.evaluated_depsgraph_get()
    mesh_eval = mesh.evaluated_get(deps)
    baked = bpy.data.meshes.new_from_object(mesh_eval)
    bpy.data.objects.remove(mesh, do_unlink=True)
    bpy.data.objects.remove(arm, do_unlink=True)
    ob = bpy.data.objects.new("KenneyFounder", baked)
    bpy.context.collection.objects.link(ob)
    return ob


def _apply_object_xform(ob: bpy.types.Object) -> None:
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.transform_apply(rotation=True, scale=True, location=True)


def _ensure_mesh_upright(ob: bpy.types.Object) -> None:
    """Z-up Kenney bake → Y-up art space with head at max Y."""
    verts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    max_z, min_z = max(v.z for v in verts), min(v.z for v in verts)
    max_y, min_y = max(v.y for v in verts), min(v.y for v in verts)
    if max_z - min_z >= max_y - min_y:
        if max_z < min_z + 1e-4:
            ob.rotation_euler = (math.pi, 0.0, 0.0)
            _apply_object_xform(ob)
        else:
            ob.rotation_euler = (math.pi / 2, 0.0, 0.0)
            _apply_object_xform(ob)
    verts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    if max(v.y for v in verts) < min(v.y for v in verts) + 1e-4:
        ob.rotation_euler = (math.pi, 0.0, 0.0)
        _apply_object_xform(ob)


def _face_positive_y(ob: bpy.types.Object) -> None:
    verts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    max_y, min_y = max(v.y for v in verts), min(v.y for v in verts)
    span = max(1e-4, max_y - min_y)
    top = [v for v in verts if v.y > max_y - span * 0.2]
    bot = [v for v in verts if v.y < min_y + span * 0.2]
    if not top or not bot:
        return
    head = Vector(
        (
            sum(v.x for v in top) / len(top),
            sum(v.y for v in top) / len(top),
            sum(v.z for v in top) / len(top),
        )
    )
    hip = Vector(
        (
            sum(v.x for v in bot) / len(bot),
            sum(v.y for v in bot) / len(bot),
            sum(v.z for v in bot) / len(bot),
        )
    )
    fwd = head - hip
    fwd.y = 0.0
    if fwd.length < 1e-4:
        return
    yaw = math.atan2(fwd.x, fwd.y)
    ob.rotation_euler = (0.0, -yaw, 0.0)
    _apply_object_xform(ob)
    if abs(fwd.normalized().y) < 0.5:
        ob.rotation_euler = (0.0, -math.pi / 2, 0.0)
        _apply_object_xform(ob)


def _height_span(ob: bpy.types.Object) -> tuple[float, float, float]:
    ys = [(ob.matrix_world @ v.co).y for v in ob.data.vertices]
    lo, hi = min(ys), max(ys)
    return lo, hi, max(1e-4, hi - lo)


def _scale_character(src: bpy.types.Object) -> float:
    lo, hi, span = _height_span(src)
    scale = TARGET_HEIGHT / span
    src.scale = (scale * 0.74, scale, scale * 0.72)
    _apply_object_xform(src)
    bc.ground_scene_meshes()
    return _height_span(src)[2]


def _extract_part(
    src: bpy.types.Object,
    name: str,
    y_min: float,
    y_max: float,
    x_side: int | None = None,
    x_abs_max: float | None = None,
) -> bpy.types.Object | None:
    me = src.data.copy()
    ob = bpy.data.objects.new(name, me)
    ob.matrix_world = src.matrix_world.copy()
    bpy.context.collection.objects.link(ob)
    bm = bmesh.new()
    bm.from_mesh(me)
    bm.verts.ensure_lookup_table()
    drop = []
    for v in bm.verts:
        w = ob.matrix_world @ v.co
        if w.y < y_min - 1e-5 or w.y > y_max + 1e-5:
            drop.append(v)
            continue
        if x_side is not None:
            if x_side < 0 and w.x > 0.008:
                drop.append(v)
            elif x_side > 0 and w.x < -0.008:
                drop.append(v)
        if x_abs_max is not None and abs(w.x) > x_abs_max:
            drop.append(v)
    if drop:
        bmesh.ops.delete(bm, geom=drop, context="VERTS")
    if not bm.verts:
        bm.free()
        bpy.data.objects.remove(ob, do_unlink=True)
        return None
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    bm.to_mesh(me)
    me.update()
    bm.free()
    ob.name = name
    return ob


def _split_parts(src: bpy.types.Object) -> list[bpy.types.Object]:
    lo, hi, span = _height_span(src)

    def band(f0: float, f1: float) -> tuple[float, float]:
        return lo + span * f0, lo + span * f1

    specs: list[tuple[str, float, float, int | None, tuple[float, float, float, float], float]] = [
        ("Foot_-1", 0.00, 0.15, -1, FOUNDER_SHOE, 0.42),
        ("Foot_1", 0.00, 0.15, 1, FOUNDER_SHOE, 0.42),
        ("Leg_-1", 0.11, 0.48, -1, FOUNDER_JEANS, 0.56),
        ("Leg_1", 0.11, 0.48, 1, FOUNDER_JEANS, 0.56),
        ("Hips", 0.39, 0.55, None, FOUNDER_JEANS, 0.54),
        ("Torso", 0.48, 0.78, None, FOUNDER_HOODIE, 0.62),
        ("Neck", 0.70, 0.83, None, FOUNDER_HOODIE, 0.60),
        ("Head", 0.73, 1.00, None, FOUNDER_SKIN, 0.52),
    ]
    parts: list[bpy.types.Object] = []
    for name, f0, f1, side, color, rough in specs:
        y0, y1 = band(f0, f1)
        ob = _extract_part(src, name, y0, y1, side)
        if not ob or len(ob.data.vertices) < 8:
            if ob:
                bpy.data.objects.remove(ob, do_unlink=True)
            continue
        bc.assign_solid(ob, color, rough)
        parts.append(ob)
    bpy.data.objects.remove(src, do_unlink=True)
    return parts


def _add_mug() -> None:
    torso = bpy.data.objects.get("Torso")
    if not torso:
        return
    xs = [(torso.matrix_world @ v.co).x for v in torso.data.vertices]
    ys = [(torso.matrix_world @ v.co).y for v in torso.data.vertices]
    y = sum(ys) / len(ys)
    x = max(xs) * 0.92
    z = sum((torso.matrix_world @ v.co).z for v in torso.data.vertices) / len(torso.data.vertices)
    mug = bc.add_cube("FounderMug", (0.09, 0.09, 0.08), (x, y, z))
    bc.assign_solid(mug, (0.94, 0.52, 0.12, 1.0), 0.38)


def build_founder_kenney() -> None:
    arm, mesh = _import_rigged()
    _apply_idle_rest_pose(arm)
    _ground_rig(arm, mesh)
    ok, msg = verify_standing_rig(arm)
    if not ok:
        raise RuntimeError(f"FAIL staff-founder-idle: {msg}")
    baked = _bake_mesh(mesh, arm)
    _ensure_mesh_upright(baked)
    _face_positive_y(baked)
    _scale_character(baked)
    parts = _split_parts(baked)
    if len(parts) < 8:
        raise RuntimeError(f"Kenney split failed parts={len(parts)}")
    _add_mug()
    for ob in bpy.context.scene.objects:
        if ob.type != "MESH":
            continue
        try:
            bc.bevel_object(ob, width=0.0025, segments=1)
        except RuntimeError:
            pass