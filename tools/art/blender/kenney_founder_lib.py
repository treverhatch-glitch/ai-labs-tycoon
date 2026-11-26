"""Kenney Animated Characters 3 (CC0) → stylized founder hero (hoodie + jeans, no procedural wrap)."""
from __future__ import annotations

import math
from pathlib import Path

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc

INCOMING = bc.REPO_ROOT / "tools/art/cc0/incoming"
KENNEY_FBX = INCOMING / "kenney/animated-characters-3/Model/characterMedium.fbx"
TARGET_HEIGHT = 1.72
FOUNDER_SKIN = (0.84, 0.68, 0.54, 1.0)
FOUNDER_HOODIE = (0.14, 0.56, 0.50, 1.0)  # teal hoodie
FOUNDER_JEANS = (0.13, 0.24, 0.50, 1.0)
FOUNDER_SHOE = (0.22, 0.24, 0.28, 1.0)


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


def _rotate_y_up_to_art() -> None:
    """Kenney Z-up FBX → staff lane height on Y."""
    for ob in bpy.context.scene.objects:
        if ob.type != "MESH":
            continue
        ob.rotation_euler = (-math.pi / 2, 0.0, 0.0)
        bpy.context.view_layer.objects.active = ob
        ob.select_set(True)
        bpy.ops.object.transform_apply(rotation=True)


def _height_span(ob: bpy.types.Object) -> tuple[float, float, float]:
    ys = [(ob.matrix_world @ v.co).y for v in ob.data.vertices]
    lo, hi = min(ys), max(ys)
    return lo, hi, max(1e-4, hi - lo)


def _scale_to_height() -> None:
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    lo, hi, span = 1e9, -1e9, 1.0
    for ob in meshes:
        a, b, s = _height_span(ob)
        lo, hi = min(lo, a), max(hi, b)
    span = max(1e-4, hi - lo)
    scale = TARGET_HEIGHT / span
    for ob in meshes:
        ob.scale = (scale, scale, scale)
    bpy.ops.object.select_all(action="DESELECT")
    for ob in meshes:
        ob.select_set(True)
    bpy.ops.object.transform_apply(scale=True)
    for ob in meshes:
        ob.scale.x *= 0.71
        ob.scale.z *= 0.69
    bpy.ops.object.select_all(action="DESELECT")
    for ob in meshes:
        ob.select_set(True)
    bpy.ops.object.transform_apply(scale=True)
    bc.ground_scene_meshes()


def _extract_part(
    src: bpy.types.Object,
    name: str,
    y_min: float,
    y_max: float,
    x_side: int | None = None,
) -> bpy.types.Object | None:
    """Copy vertices in [y_min,y_max]; optional x_side -1=left, +1=right."""
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
            if x_side < 0 and w.x > 0.012:
                drop.append(v)
            elif x_side > 0 and w.x < -0.012:
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


def _split_kenney_parts(src: bpy.types.Object) -> list[bpy.types.Object]:
    lo, hi, span = _height_span(src)
    parts: list[bpy.types.Object] = []

    def band(f0: float, f1: float) -> tuple[float, float]:
        return lo + span * f0, lo + span * f1

    specs: list[tuple[str, float, float, int | None, tuple[float, float, float, float], float]] = [
        ("Foot_-1", 0.00, 0.14, -1, FOUNDER_SHOE, 0.42),
        ("Foot_1", 0.00, 0.14, 1, FOUNDER_SHOE, 0.42),
        ("Leg_-1", 0.12, 0.50, -1, FOUNDER_JEANS, 0.56),
        ("Leg_1", 0.12, 0.50, 1, FOUNDER_JEANS, 0.56),
        ("Hips", 0.40, 0.56, None, FOUNDER_JEANS, 0.54),
        ("Torso", 0.52, 0.74, None, FOUNDER_HOODIE, 0.62),
        ("Neck", 0.72, 0.78, None, FOUNDER_HOODIE, 0.60),
        ("Head", 0.76, 1.00, None, FOUNDER_SKIN, 0.52),
        ("Hand_-1", 0.48, 0.72, -1, FOUNDER_SKIN, 0.50),
        ("Hand_1", 0.48, 0.72, 1, FOUNDER_SKIN, 0.50),
    ]
    for name, f0, f1, side, color, rough in specs:
        if name.startswith("Hand_"):
            y0, y1 = band(f0, f1)
            ob = _extract_hand(src, name, y0, y1, side)
        else:
            y0, y1 = band(f0, f1)
            ob = _extract_part(src, name, y0, y1, side)
        if not ob or len(ob.data.vertices) < 6:
            if ob:
                bpy.data.objects.remove(ob, do_unlink=True)
            continue
        bc.assign_solid(ob, color, rough)
        parts.append(ob)
    bpy.data.objects.remove(src, do_unlink=True)
    return parts


def _extract_hand(
    src: bpy.types.Object, name: str, y_min: float, y_max: float, x_side: int
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
        if w.y < y_min or w.y > y_max:
            drop.append(v)
            continue
        if x_side < 0 and w.x > -0.045:
            drop.append(v)
        elif x_side > 0 and w.x < 0.045:
            drop.append(v)
    if drop:
        bmesh.ops.delete(bm, geom=drop, context="VERTS")
    if not bm.verts:
        bm.free()
        bpy.data.objects.remove(ob, do_unlink=True)
        return None
    bm.to_mesh(me)
    me.update()
    bm.free()
    return ob


def _snap_chain(*pairs: tuple[str, str]) -> None:
    import staff_connectivity as sc

    for parent_name, child_name in pairs:
        parent = bpy.data.objects.get(parent_name)
        child = bpy.data.objects.get(child_name)
        if not parent or not child:
            continue
        for _ in range(4):
            gap = sc.bbox_gap(sc._world_bbox(parent), sc._world_bbox(child))
            if gap <= 0.019:
                break
            pc = sum((parent.matrix_world @ v.co for v in parent.data.vertices), Vector()) / len(
                parent.data.vertices
            )
            cc = sum((child.matrix_world @ v.co for v in child.data.vertices), Vector()) / len(child.data.vertices)
            child.location += (pc - cc) * 0.45
    bpy.context.view_layer.update()


def _add_founder_mug(span: float) -> bpy.types.Object | None:
    hand = bpy.data.objects.get("Hand_1")
    if not hand:
        return None
    pts = [hand.matrix_world @ v.co for v in hand.data.vertices]
    center = sum(pts, Vector()) / len(pts)
    mug = bc.add_cube(
        "FounderMug",
        (span * 0.08, span * 0.09, span * 0.07),
        (center.x + span * 0.05, center.y, center.z + span * 0.02),
    )
    bc.assign_solid(mug, (0.94, 0.52, 0.12, 1.0), 0.38)
    return mug


def build_founder_kenney() -> None:
    src = _import_kenney()
    _rotate_y_up_to_art()
    _scale_to_height()
    parts = _split_kenney_parts(src)
    if len(parts) < 8:
        raise RuntimeError(f"Kenney split too few parts: {len(parts)}")
    torso = bpy.data.objects.get("Torso")
    _lo, _hi, span = _height_span(torso) if torso else (0, TARGET_HEIGHT, 0.35)
    _add_founder_mug(span)
    _snap_chain(("Hips", "Leg_-1"), ("Hips", "Leg_1"), ("Torso", "Neck"), ("Neck", "Head"))
    for ob in bpy.context.scene.objects:
        if ob.type != "MESH":
            continue
        try:
            bc.bevel_object(ob, width=0.003, segments=1)
        except RuntimeError:
            pass