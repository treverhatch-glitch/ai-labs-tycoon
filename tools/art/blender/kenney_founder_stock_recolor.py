"""Founder hero: import canonical stock Kenney mesh; flat material slots only (no mesh edits)."""
from __future__ import annotations

import hashlib
import json
import shutil
from pathlib import Path

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc
import kenney_founder_lib as kf
from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward
from kenney_geometry_landmarks import FounderLandmarks, compute_landmarks, torso_half_width

STOCK_SOURCE = bc.REPO_ROOT / "tools" / "art" / "source" / "kenney"
STOCK_BLEND = STOCK_SOURCE / "staff-founder-kenney-stock.blend"
STOCK_KIT_GLB = STOCK_SOURCE / "staff-founder-kenney-stock-kit.glb"
STOCK_KIT_SHA = STOCK_SOURCE / "staff-founder-kenney-stock-kit.glb.sha256"
STOCK_GLB = bc.ART_ROOT / "staff-kit" / "staff-founder-kenney-stock.glb"
STOCK_META = STOCK_BLEND.with_suffix(".stock.json")
STOCK_OBJECT_NAME = "KenneyFounderStock"

FOUNDER_BROW = (0.20, 0.16, 0.14, 1.0)
FOUNDER_MOUTH = (0.18, 0.12, 0.12, 1.0)

SLOT_SOLE = 0
SLOT_SHOE = 1
SLOT_JEANS = 2
SLOT_SHIRT = 3
SLOT_SKIN = 4
SLOT_HAND = 5
SLOT_HAIR = 6
SLOT_EYE = 7
SLOT_BROW = 8
SLOT_MOUTH = 9


def kit_glb_sha256() -> str:
    if STOCK_KIT_SHA.is_file():
        first = STOCK_KIT_SHA.read_text(encoding="utf-8").strip().split()[0]
        if len(first) == 64:
            return first
    if STOCK_KIT_GLB.is_file():
        return hashlib.sha256(STOCK_KIT_GLB.read_bytes()).hexdigest()
    raise RuntimeError("missing staff-founder-kenney-stock-kit.glb")


def install_stock_kit_glb() -> None:
    """Unity staff-kit GLB must match pinned Kenney kit bytes (not re-exported bake)."""
    want = kit_glb_sha256()
    STOCK_GLB.parent.mkdir(parents=True, exist_ok=True)
    if not STOCK_KIT_GLB.is_file():
        raise RuntimeError(f"FAIL missing kit glb {STOCK_KIT_GLB}")
    got = hashlib.sha256(STOCK_KIT_GLB.read_bytes()).hexdigest()
    if got != want:
        raise RuntimeError(f"FAIL kit glb sha256 got={got} want={want}")
    shutil.copyfile(STOCK_KIT_GLB, STOCK_GLB)
    out = hashlib.sha256(STOCK_GLB.read_bytes()).hexdigest()
    if out != want:
        raise RuntimeError("FAIL stock glb install mismatch")


def export_stock_glb(baked: bpy.types.Object) -> None:
    STOCK_SOURCE.mkdir(parents=True, exist_ok=True)
    baked.name = STOCK_OBJECT_NAME
    bc.assign_solid(baked, (0.58, 0.58, 0.58, 1.0), 0.62)
    bc.smooth_shade(baked)
    for other in list(bpy.context.scene.objects):
        if other != baked:
            bpy.data.objects.remove(other, do_unlink=True)
    bpy.ops.wm.save_as_mainfile(filepath=str(STOCK_BLEND))
    meta = {
        "vertexCount": len(baked.data.vertices),
        "triangleCount": sum(len(p.vertices) for p in baked.data.polygons),
        "objectName": STOCK_OBJECT_NAME,
        "kitGlbSha256": kit_glb_sha256(),
    }
    STOCK_META.write_text(json.dumps(meta, indent=2) + "\n", encoding="utf-8")
    install_stock_kit_glb()
    print(f"OK stock_kit_glb sha256={meta['kitGlbSha256']}")


def bisect_stock_landmarks(ob: bpy.types.Object) -> None:
    zs = [(ob.matrix_world @ v.co).z for v in ob.data.vertices]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    lm = compute_landmarks(ob)
    cuts = [
        lo + span * lm.h_shoe,
        lo + span * lm.h_waist,
        lo + span * lm.h_neck,
        lo + span * (lm.h_eye + 0.04),
    ]
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    geom = list(bm.verts) + list(bm.edges) + list(bm.faces)
    for z in cuts:
        bmesh.ops.bisect_plane(
            bm,
            geom=geom,
            plane_co=(0.0, 0.0, z),
            plane_no=(0.0, 0.0, 1.0),
            dist=1e-5,
            clear_inner=False,
            clear_outer=False,
        )
        geom = list(bm.verts) + list(bm.edges) + list(bm.faces)
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()


def _ensure_material(name: str, rgba: tuple[float, float, float, float], rough: float) -> bpy.types.Material:
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


def _head_width_at(ws: list[Vector], lo: float, span: float, h: float) -> float:
    pts = [p for p in ws if abs((p.z - lo) / span - h) < 0.015]
    return _xy_extent_static(pts, span)


def _xy_extent_static(pts: list[Vector], span: float) -> float:
    if len(pts) < 4:
        return span * 0.18
    xs = [p.x for p in pts]
    ys = [p.y for p in pts]
    return max(max(xs) - min(xs), max(ys) - min(ys))


def _region_for_point(
    w: Vector,
    *,
    lm: FounderLandmarks,
    ws: list[Vector],
    y_face: float,
    lat: float,
    torso_hw: float,
    head_w: float,
) -> int:
    lo, span = lm.zmin, lm.span
    h = (w.z - lo) / span
    lat_n = lat / span
    arm = lat > torso_hw + span * 0.012
    arm_hand_top = lm.h_waist + 0.25 * (lm.h_neck - lm.h_waist)

    if h < lm.h_shoe:
        return SLOT_SOLE if lat_n > 0.05 else SLOT_SHOE

    if h < lm.h_waist:
        return SLOT_JEANS

    if h > lm.h_neck:
        eye_lo = lm.h_eye - 0.032
        eye_hi = lm.h_eye + 0.028
        brow_lo = lm.h_eye + 0.034
        eye_in = head_w * 0.05
        eye_out = head_w * 0.44
        if y_face > 0.12 and eye_lo <= h < eye_hi and eye_in < lat < eye_out:
            return SLOT_EYE
        if y_face > 0.12 and brow_lo <= h < eye_hi + 0.055 and lat < eye_out * 1.1:
            return SLOT_BROW
        if y_face > 0.10 and eye_lo - 0.045 <= h < eye_lo and lat < head_w * 0.08:
            return SLOT_MOUTH
        if y_face < -0.03 or (h > lm.h_neck + 0.12 and y_face < 0.05):
            return SLOT_HAIR
        return SLOT_SKIN

    if arm:
        if h < arm_hand_top:
            return SLOT_HAND
        return SLOT_SHIRT

    return SLOT_SHIRT


def apply_founder_materials(ob: bpy.types.Object) -> None:
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    lm = compute_landmarks(ob)
    fwd = toe_forward([ob])
    hc, y_face_fn = face_shell_coord(ob, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)

    mats = [
        _ensure_material("FounderSole", kf.FOUNDER_SHOE_SOLE, 0.72),
        _ensure_material("FounderShoe", kf.FOUNDER_SHOE, 0.38),
        _ensure_material("FounderJeans", kf.FOUNDER_JEANS, 0.56),
        _ensure_material("FounderTee", kf.FOUNDER_HOODIE_RED, 0.58),
        _ensure_material("FounderSkin", kf.FOUNDER_SKIN, 0.50),
        _ensure_material("FounderHand", kf.FOUNDER_SKIN, 0.48),
        _ensure_material("FounderHair", kf.FOUNDER_HAIR, 0.48),
        _ensure_material("FounderEye", kf.FOUNDER_EYE, 0.32),
        _ensure_material("FounderBrow", FOUNDER_BROW, 0.36),
        _ensure_material("FounderMouth", FOUNDER_MOUTH, 0.40),
    ]
    ob.data.materials.clear()
    for m in mats:
        ob.data.materials.append(m)

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        y_face = y_face_fn(wc)
        lat = lateral_from_front(wc, hc, fwd)
        h = (wc.z - lm.zmin) / lm.span
        thw = torso_half_width(ws, lm.zmin, lm.span, h, lm.cx, lm.cy)
        face.material_index = _region_for_point(
            wc,
            lm=lm,
            ws=ws,
            y_face=y_face,
            lat=lat,
            torso_hw=thw,
            head_w=head_w,
        )
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()
    bc.smooth_shade(ob)


def _height_span(ob: bpy.types.Object) -> tuple[float, float]:
    zs = [(ob.matrix_world @ v.co).z for v in ob.data.vertices]
    return min(zs), max(zs)


def load_stock_and_recolor() -> bpy.types.Object:
    if not STOCK_BLEND.is_file() or not STOCK_META.is_file():
        raise RuntimeError(f"FAIL staff-founder-idle missing stock blend {STOCK_BLEND}")
    meta = json.loads(STOCK_META.read_text(encoding="utf-8"))
    want_verts = int(meta["vertexCount"])
    want_tris = int(meta["triangleCount"])
    kf._clear_scene()
    bpy.ops.wm.open_mainfile(filepath=str(STOCK_BLEND))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if len(meshes) != 1:
        raise RuntimeError(f"FAIL staff-founder-idle stock_import parts={len(meshes)} want=1")
    ob = meshes[0]
    ob.name = STOCK_OBJECT_NAME
    if len(ob.data.vertices) != want_verts:
        raise RuntimeError(
            f"FAIL staff-founder-idle stock_verts={len(ob.data.vertices)} want={want_verts}"
        )
    tri_count = sum(len(p.vertices) for p in ob.data.polygons)
    if tri_count != want_tris:
        raise RuntimeError(
            f"FAIL staff-founder-idle stock_tris={tri_count} want={want_tris}"
        )
    apply_founder_materials(ob)
    return ob