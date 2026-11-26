"""Kenney Animated Characters 3 (CC0) — stylized founder, idle facing camera."""
from __future__ import annotations

import math

import bpy
import bmesh
from mathutils import Matrix, Vector

import blender_common as bc

INCOMING = bc.REPO_ROOT / "tools/art/cc0/incoming"
KENNEY_FBX = INCOMING / "kenney/animated-characters-3/Model/characterMedium.fbx"
TARGET_HEIGHT = 1.72
FOUNDER_SKIN = (0.84, 0.68, 0.54, 1.0)
FOUNDER_HOODIE = (0.10, 0.62, 0.55, 1.0)
FOUNDER_JEANS = (0.12, 0.28, 0.58, 1.0)
FOUNDER_SHOE = (0.20, 0.22, 0.26, 1.0)
FACE_FORWARD = Vector((0.0, 1.0, 0.0))


def _clear_scene() -> None:
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)


def _import_kenney() -> bpy.types.Object:
    if not KENNEY_FBX.is_file():
        raise RuntimeError(f"missing Kenney FBX {KENNEY_FBX}")
    _clear_scene()
    bpy.ops.import_scene.fbx(filepath=str(KENNEY_FBX))
    ob = bpy.data.objects.get("characterMedium")
    if not ob or ob.type != "MESH":
        raise RuntimeError("Kenney characterMedium mesh missing")
    return ob


def _apply_object_xform(ob: bpy.types.Object) -> None:
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.transform_apply(rotation=True, scale=True, location=True)


def _orient_standing_face_y(src: bpy.types.Object) -> None:
    """Z-up Kenney → Y-up height, face +Y (toward studio camera)."""
    src.rotation_euler = (-math.pi / 2, 0.0, 0.0)
    _apply_object_xform(src)
    src.rotation_euler = (0.0, 0.0, -math.pi / 2)
    _apply_object_xform(src)


def _pose_idle_arms_down(src: bpy.types.Object) -> None:
    lo, hi, span = _height_span(src)
    shoulder_y = lo + span * 0.68
    me = src.data
    bm = bmesh.new()
    bm.from_mesh(me)
    bm.verts.ensure_lookup_table()
    arm_angle = math.radians(76.0)
    for v in bm.verts:
        w = src.matrix_world @ v.co
        if w.y < lo + span * 0.38 or w.y > lo + span * 0.82:
            continue
        if abs(w.x) < 0.06:
            continue
        side = 1.0 if w.x > 0 else -1.0
        pivot = Vector((side * 0.20, shoulder_y, w.z * 0.35))
        rel = w - pivot
        mat = Matrix.Rotation(-side * arm_angle, 3, Vector((0.0, 1.0, 0.0)))
        new_w = pivot + mat @ rel
        v.co = src.matrix_world.inverted() @ new_w
    bm.to_mesh(me)
    me.update()
    bm.free()


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
    inv = ob.matrix_world.inverted()
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
    src = _import_kenney()
    _orient_standing_face_y(src)
    _pose_idle_arms_down(src)
    _scale_character(src)
    parts = _split_parts(src)
    if len(parts) < 7:
        raise RuntimeError(f"Kenney split failed parts={len(parts)}")
    _add_mug()
    for ob in bpy.context.scene.objects:
        if ob.type != "MESH":
            continue
        try:
            bc.bevel_object(ob, width=0.0025, segments=1)
        except RuntimeError:
            pass