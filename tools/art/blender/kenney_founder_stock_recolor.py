"""Founder hero: import canonical stock Kenney mesh; flat material slots only (no mesh edits)."""
from __future__ import annotations

import json

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc
import kenney_founder_lib as kf
from kenney_face_axis import resolve_face_forward

STOCK_SOURCE = bc.REPO_ROOT / "tools" / "art" / "source" / "kenney"
STOCK_BLEND = STOCK_SOURCE / "staff-founder-kenney-stock.blend"
STOCK_GLB = bc.ART_ROOT / "staff-kit" / "staff-founder-kenney-stock.glb"
STOCK_META = STOCK_BLEND.with_suffix(".stock.json")
STOCK_OBJECT_NAME = "KenneyFounderStock"

FOUNDER_BROW = (0.20, 0.16, 0.14, 1.0)

SLOT_SOLE = 0
SLOT_SHOE = 1
SLOT_JEANS = 2
SLOT_SHIRT = 3
SLOT_SKIN = 4
SLOT_HAND = 5
SLOT_HAIR = 6
SLOT_EYE = 7
SLOT_BROW = 8

# horizontal band landmarks (fraction of height span)
BAND_SHOE_TOP = 0.13
BAND_ANKLE = 0.20
BAND_WAIST = 0.46
BAND_NECK = 0.54
BAND_BROW = 0.70
BAND_HAIRLINE = 0.735


def export_stock_glb(baked: bpy.types.Object) -> None:
    STOCK_SOURCE.mkdir(parents=True, exist_ok=True)
    STOCK_GLB.parent.mkdir(parents=True, exist_ok=True)
    baked.name = STOCK_OBJECT_NAME
    bc.assign_solid(baked, (0.58, 0.58, 0.58, 1.0), 0.62)
    bc.smooth_shade(baked)
    for other in list(bpy.context.scene.objects):
        if other != baked:
            bpy.data.objects.remove(other, do_unlink=True)
    bpy.ops.wm.save_as_mainfile(filepath=str(STOCK_BLEND))
    bc.export_gltf(baked, STOCK_GLB)
    meta = {
        "vertexCount": len(baked.data.vertices),
        "triangleCount": sum(len(p.vertices) for p in baked.data.polygons),
        "objectName": STOCK_OBJECT_NAME,
    }
    STOCK_META.write_text(json.dumps(meta, indent=2) + "\n", encoding="utf-8")


def _landmark_z(lo: float, span: float) -> dict[str, float]:
    return {
        "shoe_top": lo + span * BAND_SHOE_TOP,
        "waist": lo + span * BAND_WAIST,
        "neck": lo + span * BAND_NECK,
        "brow": lo + span * BAND_BROW,
        "hairline": lo + span * BAND_HAIRLINE,
    }


def _mesh_frame(ob: bpy.types.Object) -> tuple[float, float, float, Vector, float]:
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    zs = [p.z for p in ws]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    cx = sum(p.x for p in ws) / len(ws)
    fwd = resolve_face_forward([ob])
    head_pts = [p for p in ws if p.z > hi - span * 0.28]
    hc = sum(head_pts, Vector()) / len(head_pts) if head_pts else sum(ws, Vector()) / len(ws)
    face_depth = span * 0.06
    if abs(fwd.y) > 0.6:
        face_shell_y = hc.y + face_depth * (1.0 if fwd.y > 0 else -1.0)
        y_face_coord = lambda p: (p.y - face_shell_y) * (1.0 if fwd.y > 0 else -1.0)
    elif fwd.x > 0:
        face_shell_y = hc.x + face_depth
        y_face_coord = lambda p: p.x - face_shell_y
    else:
        face_shell_y = hc.x - face_depth
        y_face_coord = lambda p: face_shell_y - p.x
    return lo, span, cx, fwd, y_face_coord


def bisect_stock_landmarks(ob: bpy.types.Object) -> None:
    """Cut mesh at horizontal garment planes (stock + founder share topology)."""
    zs = [(ob.matrix_world @ v.co).z for v in ob.data.vertices]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    _bisect_horizontal_bands(ob, lo, span)


def _bisect_horizontal_bands(ob: bpy.types.Object, lo: float, span: float) -> None:
    """Split faces at garment landmark planes (horizontal cuts only)."""
    cuts = [
        lo + span * BAND_SHOE_TOP,
        lo + span * BAND_WAIST,
        lo + span * BAND_NECK,
        lo + span * BAND_HAIRLINE,
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


def _region_for_point(
    w: Vector,
    *,
    lo: float,
    span: float,
    cx: float,
    z: dict[str, float],
    y_face: float,
) -> int:
    zf = (w.z - lo) / span
    xf = abs(w.x - cx)

    if zf >= BAND_HAIRLINE and xf < 0.15:
        return SLOT_HAIR

    if y_face > 0.012 and xf < 0.13:
        if 0.665 <= zf < 0.708 and 0.045 < xf < 0.12:
            return SLOT_EYE
        if 0.700 <= zf < BAND_HAIRLINE:
            return SLOT_BROW

    if zf >= BAND_NECK and zf < BAND_HAIRLINE and xf < 0.14:
        return SLOT_SKIN

    if zf >= BAND_WAIST and zf < BAND_NECK:
        if xf >= 0.14 and zf >= BAND_NECK - 0.06:
            return SLOT_SKIN
        return SLOT_SHIRT

    if zf >= BAND_NECK - 0.02 and zf < BAND_WAIST and xf >= 0.10:
        return SLOT_SHIRT

    if zf >= 0.56 and xf >= 0.13:
        return SLOT_HAND
    if BAND_NECK <= zf < 0.62 and xf >= 0.11:
        return SLOT_SKIN

    if zf < BAND_WAIST:
        if zf < 0.06 and xf > 0.07:
            return SLOT_SOLE
        if zf < BAND_SHOE_TOP and xf > 0.06:
            return SLOT_SHOE
        return SLOT_JEANS

    return SLOT_SHIRT


def apply_founder_materials(ob: bpy.types.Object) -> None:
    """Horizontal garment bands + front eye/brow decals (mesh already landmark-bisected in stock)."""
    lo, span, cx, _fwd, y_face_fn = _mesh_frame(ob)
    z = _landmark_z(lo, span)

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
        face.material_index = _region_for_point(
            wc, lo=lo, span=span, cx=cx, z=z, y_face=y_face
        )
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()
    bc.smooth_shade(ob)


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