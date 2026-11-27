"""Founder hero: import canonical stock Kenney mesh; flat material slots only (no mesh edits)."""
from __future__ import annotations

import json

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc
import kenney_founder_lib as kf

STOCK_SOURCE = bc.REPO_ROOT / "tools" / "art" / "source" / "kenney"
STOCK_BLEND = STOCK_SOURCE / "staff-founder-kenney-stock.blend"
STOCK_GLB = bc.ART_ROOT / "staff-kit" / "staff-founder-kenney-stock.glb"
STOCK_META = STOCK_BLEND.with_suffix(".stock.json")
STOCK_OBJECT_NAME = "KenneyFounderStock"

FOUNDER_BROW = (0.20, 0.16, 0.14, 1.0)

# material slot indices
SLOT_SOLE = 0
SLOT_SHOE = 1
SLOT_JEANS = 2
SLOT_SHIRT = 3
SLOT_SKIN = 4
SLOT_HAND = 5
SLOT_HAIR = 6
SLOT_EYE = 7
SLOT_BROW = 8


def export_stock_glb(baked: bpy.types.Object) -> None:
    """Write grey stock mesh used by check frame and founder import."""
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


def _mesh_frame(ob: bpy.types.Object) -> tuple[float, float, float, float, float, float]:
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    zs = [p.z for p in ws]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    cx = sum(p.x for p in ws) / len(ws)
    top = [p for p in ws if p.z > hi - span * 0.22]
    cy = sum(p.y for p in top) / len(top) if top else sum(p.y for p in ws) / len(ws)
    face_sign = _face_sign_from_verts(ws, span, lo, hi)
    return lo, span, cx, cy, face_sign, hi


def _face_sign_from_verts(ws: list[Vector], span: float, lo: float, hi: float) -> float:
    top = [p for p in ws if p.z > hi - span * 0.22]
    if len(top) < 8:
        return 1.0
    hc = sum(top, Vector()) / len(top)
    nose_fwd = max(top, key=lambda v: v.y)
    nose_back = min(top, key=lambda v: v.y)
    df = abs(nose_fwd.y - hc.y)
    db = abs(nose_back.y - hc.y)
    return 1.0 if df >= db else -1.0


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
    cy: float,
    face_sign: float,
) -> int:
    """Flat material region by height / side / facing (Z-up Kenney import)."""
    zf = (w.z - lo) / span
    xf = abs(w.x - cx)
    y_face = (w.y - cy) * face_sign

    if zf < 0.06 and xf > 0.07:
        return SLOT_SOLE
    if zf < 0.13 and xf > 0.06:
        return SLOT_SHOE

    if 0.78 <= zf <= 0.88 and y_face < -0.012 and xf < 0.11:
        return SLOT_HAIR
    if zf >= 0.84 and y_face < 0.06 and xf < 0.10:
        return SLOT_HAIR

    if 0.655 <= zf <= 0.702 and 0.048 < xf < 0.118 and y_face > 0.028:
        return SLOT_EYE
    if 0.698 <= zf <= 0.738 and xf < 0.12 and y_face > 0.022:
        return SLOT_BROW

    if 0.58 <= zf <= 0.77 and xf < 0.13 and y_face > -0.008:
        return SLOT_SKIN
    if 0.54 <= zf <= 0.62 and xf < 0.11:
        return SLOT_SKIN

    if 0.40 <= zf <= 0.58 and xf >= 0.11:
        return SLOT_SHIRT
    if 0.40 <= zf <= 0.58 and xf < 0.16:
        return SLOT_SHIRT

    if 0.52 <= zf <= 0.68 and xf >= 0.12:
        return SLOT_SKIN
    if 0.58 <= zf <= 0.72 and xf >= 0.11:
        return SLOT_HAND

    if 0.32 <= zf <= 0.46 and xf < 0.14:
        return SLOT_JEANS
    if zf < 0.48 and xf >= 0.06:
        return SLOT_JEANS

    if zf < 0.42:
        return SLOT_JEANS
    if zf < 0.56:
        return SLOT_SHIRT if xf > 0.09 else SLOT_JEANS
    return SLOT_SKIN


def apply_founder_materials(ob: bpy.types.Object) -> None:
    """Assign per-face flat materials; vertices unchanged."""
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

    lo, span, cx, cy, face_sign, _hi = _mesh_frame(ob)

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        face.material_index = _region_for_point(wc, lo=lo, span=span, cx=cx, cy=cy, face_sign=face_sign)
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