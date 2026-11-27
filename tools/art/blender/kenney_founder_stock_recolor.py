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
from kenney_face_axis import face_shell_coord, toe_forward

STOCK_SOURCE = bc.REPO_ROOT / "tools" / "art" / "source" / "kenney"
STOCK_BLEND = STOCK_SOURCE / "staff-founder-kenney-stock.blend"
STOCK_KIT_GLB = STOCK_SOURCE / "staff-founder-kenney-stock-kit.glb"
STOCK_KIT_SHA = STOCK_SOURCE / "staff-founder-kenney-stock-kit.glb.sha256"
STOCK_GLB = bc.ART_ROOT / "staff-kit" / "staff-founder-kenney-stock.glb"
STOCK_META = STOCK_BLEND.with_suffix(".stock.json")
STOCK_OBJECT_NAME = "KenneyFounderStock"

FOUNDER_BROW = (0.20, 0.16, 0.14, 1.0)

H_SHOE = 0.07
H_WAIST = 0.50
H_SHIRT_TOP = 0.80
H_BROW = 0.76
H_TOP_CAP = 0.90

SLOT_SOLE = 0
SLOT_SHOE = 1
SLOT_JEANS = 2
SLOT_SHIRT = 3
SLOT_SKIN = 4
SLOT_HAND = 5
SLOT_HAIR = 6
SLOT_EYE = 7
SLOT_BROW = 8


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
    cuts = [lo + span * h for h in (H_SHOE, H_WAIST, H_SHIRT_TOP, H_BROW)]
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


def _region_for_point(
    w: Vector,
    *,
    lo: float,
    span: float,
    cx: float,
    y_face: float,
) -> int:
    h = (w.z - lo) / span
    xf = abs(w.x - cx)

    if h < H_SHOE:
        return SLOT_SOLE if xf > 0.06 else SLOT_SHOE

    if h < H_WAIST:
        return SLOT_JEANS

    if h < H_SHIRT_TOP:
        if xf >= 0.11:
            if h >= 0.74:
                return SLOT_HAND
            return SLOT_SHIRT
        return SLOT_SHIRT

    if y_face > 0.035 and 0.775 <= h < 0.88 and 0.028 < xf < 0.14:
        return SLOT_EYE
    if y_face > 0.04 and 0.845 <= h < 0.94 and 0.02 < xf < 0.15:
        return SLOT_BROW

    if h >= H_BROW and y_face < 0.0:
        return SLOT_HAIR
    if h >= H_TOP_CAP and y_face < 0.03:
        return SLOT_HAIR

    return SLOT_SKIN


def apply_founder_materials(ob: bpy.types.Object) -> None:
    lo, hi = _height_span(ob)
    span = max(1e-4, hi - lo)
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    cx = sum(p.x for p in ws) / len(ws)
    fwd = toe_forward([ob])
    _shell, y_face_fn = face_shell_coord(ob, fwd)

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
        face.material_index = _region_for_point(wc, lo=lo, span=span, cx=cx, y_face=y_face)
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