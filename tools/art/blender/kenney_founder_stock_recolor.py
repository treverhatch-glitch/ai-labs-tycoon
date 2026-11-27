"""Founder hero: import canonical stock Kenney GLB; materials only (no mesh edits)."""
from __future__ import annotations

import json
from pathlib import Path

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc
import kenney_founder_lib as kf

STOCK_BLEND = bc.ART_ROOT / "staff-kit" / "staff-founder-kenney-stock.blend"
STOCK_GLB = bc.ART_ROOT / "staff-kit" / "staff-founder-kenney-stock.glb"
STOCK_META = STOCK_BLEND.with_suffix(".stock.json")
STOCK_OBJECT_NAME = "KenneyFounderStock"


def export_stock_glb(baked: bpy.types.Object) -> None:
    """Write grey stock mesh used by check frame and founder import."""
    STOCK_BLEND.parent.mkdir(parents=True, exist_ok=True)
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


def _height_band(ob: bpy.types.Object) -> tuple[float, float, float]:
    zs = [(ob.matrix_world @ v.co).z for v in ob.data.vertices]
    lo, hi = min(zs), max(zs)
    return lo, hi, max(1e-4, hi - lo)


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
) -> int:
    """Material slot index for world point w (Z-up, front +Y)."""
    zf = (w.z - lo) / span
    side = -1 if w.x < cx - 0.02 else (1 if w.x > cx + 0.02 else 0)

    def band(f0: float, f1: float) -> bool:
        return f0 - 1e-5 <= zf <= f1 + 1e-5

    if band(0.00, 0.05) and side != 0:
        return 0  # sole dark
    if band(0.04, 0.13) and side != 0:
        return 1  # shoe
    if band(0.00, 0.48) and side != 0:
        return 2  # jeans (no white leg stripe)
    if band(0.36, 0.54) and abs(w.x - cx) < 0.14:
        return 2  # hips jeans
    if band(0.42, 0.62) and abs(w.x - cx) < 0.14:
        return 3  # torso red
    if band(0.42, 0.58) and side != 0:
        return 3  # upper arm red
    if band(0.50, 0.68) and side != 0:
        return 4  # lower arm skin
    if band(0.58, 0.72) and side != 0:
        return 5  # hand skin
    if band(0.56, 0.64) and abs(w.x - cx) < 0.12:
        return 4  # neck skin
    if band(0.60, 0.73) and 0.055 < abs(w.x - cx) < 0.13:
        return 7  # eyes (face materials only)
    if band(0.58, 0.77) and abs(w.x - cx) < 0.14:
        return 4  # head skin
    if band(0.74, 0.84) and abs(w.x - cx) < 0.12:
        return 6  # hair
    if band(0.42, 0.62) and side != 0:
        return 3
  # default fill gaps with nearest garment
    if zf < 0.5:
        return 2
    if zf < 0.72:
        return 3 if abs(w.x - cx) > 0.1 else 4
    return 4


def apply_founder_materials(ob: bpy.types.Object) -> None:
    """Assign per-face materials by height/side; vertices unchanged."""
    mats = [
        _ensure_material("FounderSole", kf.FOUNDER_SHOE_SOLE, 0.72),
        _ensure_material("FounderShoe", kf.FOUNDER_SHOE, 0.38),
        _ensure_material("FounderJeans", kf.FOUNDER_JEANS, 0.56),
        _ensure_material("FounderTee", kf.FOUNDER_HOODIE_RED, 0.58),
        _ensure_material("FounderSkin", kf.FOUNDER_SKIN, 0.50),
        _ensure_material("FounderHand", kf.FOUNDER_SKIN, 0.48),
        _ensure_material("FounderHair", kf.FOUNDER_HAIR, 0.48),
        _ensure_material("FounderEye", kf.FOUNDER_EYE, 0.32),
    ]
    ob.data.materials.clear()
    for m in mats:
        ob.data.materials.append(m)

    lo, _hi, span = _height_band(ob)
    cx = sum((ob.matrix_world @ v.co).x for v in ob.data.vertices) / len(ob.data.vertices)

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        face.material_index = _region_for_point(wc, lo=lo, span=span, cx=cx)
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