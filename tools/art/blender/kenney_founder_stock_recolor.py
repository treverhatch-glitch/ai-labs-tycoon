"""Founder hero: import canonical stock Kenney mesh; materials only (no mesh edits)."""
from __future__ import annotations

import json
from pathlib import Path

import bpy
from mathutils import Vector

import blender_common as bc
import kenney_founder_lib as kf

STOCK_SOURCE = bc.REPO_ROOT / "tools" / "art" / "source" / "kenney"
STOCK_BLEND = STOCK_SOURCE / "staff-founder-kenney-stock.blend"
STOCK_GLB = bc.ART_ROOT / "staff-kit" / "staff-founder-kenney-stock.glb"
STOCK_META = STOCK_BLEND.with_suffix(".stock.json")
STOCK_OBJECT_NAME = "KenneyFounderStock"
VCOL_NAME = "BakeCol"


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


def _height_band(ob: bpy.types.Object) -> tuple[float, float, float, float, float]:
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    zs = [p.z for p in ws]
    ys = [p.y for p in ws]
    lo, hi = min(zs), max(zs)
    cx = sum(p.x for p in ws) / len(ws)
    cy = sum(ys) / len(ys)
    return lo, hi, max(1e-4, hi - lo), cx, cy


def _face_sign_from_verts(ob: bpy.types.Object) -> float:
    """+1 when face toward +Y, else -1 (Kenney Z-up import)."""
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    zs = [p.z for p in ws]
    z_hi = max(zs)
    span = max(1e-4, z_hi - min(zs))
    top = [p for p in ws if p.z > z_hi - span * 0.22]
    if len(top) < 8:
        return 1.0
    hc = sum(top, Vector()) / len(top)
    nose_fwd = max(top, key=lambda v: v.y)
    nose_back = min(top, key=lambda v: v.y)
    df = abs(nose_fwd.y - hc.y)
    db = abs(nose_back.y - hc.y)
    return 1.0 if df >= db else -1.0


def _smoothstep(edge0: float, edge1: float, x: float) -> float:
    if edge1 <= edge0:
        return 1.0 if x >= edge1 else 0.0
    t = max(0.0, min(1.0, (x - edge0) / (edge1 - edge0)))
    return t * t * (3.0 - 2.0 * t)


def _mix_rgba(
    a: tuple[float, float, float, float],
    b: tuple[float, float, float, float],
    t: float,
) -> tuple[float, float, float, float]:
    t = max(0.0, min(1.0, t))
    return tuple(a[i] + (b[i] - a[i]) * t for i in range(4))


def _brow_rgba() -> tuple[float, float, float, float]:
    s = kf.FOUNDER_SKIN
    return (s[0] * 0.62, s[1] * 0.58, s[2] * 0.52, 1.0)


def _mouth_rgba() -> tuple[float, float, float, float]:
    s = kf.FOUNDER_SKIN
    return (s[0] * 0.55, s[1] * 0.48, s[2] * 0.46, 1.0)


def _founder_rgba_at(
    w: Vector,
    *,
    lo: float,
    span: float,
    cx: float,
    cy: float,
    face_sign: float,
) -> tuple[float, float, float, float]:
    zf = (w.z - lo) / span
    xf = abs(w.x - cx)
    y_face = (w.y - cy) * face_sign
    side = max(0.0, _smoothstep(0.08, 0.20, xf))

    jeans = kf.FOUNDER_JEANS
    shirt = kf.FOUNDER_HOODIE_RED
    skin = kf.FOUNDER_SKIN
    hair = kf.FOUNDER_HAIR
    shoe = kf.FOUNDER_SHOE
    sole = kf.FOUNDER_SHOE_SOLE
    eye = kf.FOUNDER_EYE

    waist = _smoothstep(0.44, 0.50, zf)
    col = _mix_rgba(jeans, shirt, _smoothstep(0.42, 0.48, zf))

    arm_shirt = side * _smoothstep(0.52, 0.60, zf) * _smoothstep(0.11, 0.20, xf)
    col = _mix_rgba(col, shirt, arm_shirt)

    arm_skin = side * _smoothstep(0.54, 0.62, zf) * _smoothstep(0.14, 0.24, xf)
    col = _mix_rgba(col, skin, arm_skin)

    hand = side * _smoothstep(0.58, 0.66, zf)
    col = _mix_rgba(col, skin, hand)

    neck = _smoothstep(0.55, 0.61, zf) * (1.0 - _smoothstep(0.10, 0.16, xf))
    col = _mix_rgba(col, skin, neck)

    foot = (1.0 - side) * _smoothstep(0.02, 0.06, zf)
    col = _mix_rgba(col, sole, foot * 0.9)
    foot_top = side * _smoothstep(0.05, 0.11, zf) * (1.0 - _smoothstep(0.12, 0.22, zf))
    col = _mix_rgba(col, shoe, foot_top)

    leg_jean = (1.0 - waist) * side
    col = _mix_rgba(col, jeans, leg_jean * 0.85)

    head_shell = _smoothstep(0.58, 0.64, zf) * (1.0 - _smoothstep(0.77, 0.84, zf))
    head_front = _smoothstep(-0.02, 0.06, y_face)
    col = _mix_rgba(col, skin, head_shell * head_front)

    eye_l = (
        _smoothstep(0.665, 0.695, zf)
        * (1.0 - _smoothstep(0.705, 0.725, zf))
        * _smoothstep(0.045, 0.075, xf)
        * (1.0 - _smoothstep(0.10, 0.12, xf))
        * _smoothstep(0.025, 0.05, y_face)
    )
    eye_r = eye_l
    col = _mix_rgba(col, eye, max(eye_l, eye_r))

    brow = (
        _smoothstep(0.705, 0.735, zf)
        * (1.0 - _smoothstep(0.735, 0.76, zf))
        * (1.0 - _smoothstep(0.11, 0.13, xf))
        * _smoothstep(0.02, 0.055, y_face)
    )
    col = _mix_rgba(col, _brow_rgba(), brow)

    mouth = (
        _smoothstep(0.615, 0.635, zf)
        * (1.0 - _smoothstep(0.635, 0.655, zf))
        * (1.0 - _smoothstep(0.035, 0.05, xf))
        * _smoothstep(0.04, 0.07, y_face)
    )
    col = _mix_rgba(col, _mouth_rgba(), mouth * 0.55)

    hair_back = _smoothstep(0.78, 0.85, zf) * (1.0 - _smoothstep(0.01, 0.05, y_face))
    hair_top = _smoothstep(0.82, 0.90, zf) * (1.0 - _smoothstep(0.06, 0.12, y_face))
    col = _mix_rgba(col, hair, max(hair_back, hair_top))

    return col


def _paint_founder_vertex_colors(ob: bpy.types.Object) -> None:
    lo, _hi, span, cx, cy = _height_band(ob)
    face_sign = _face_sign_from_verts(ob)
    mesh = ob.data
    while mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[0])
    vcol = mesh.color_attributes.new(name=VCOL_NAME, type="BYTE_COLOR", domain="CORNER")
    mesh.color_attributes.active_color = vcol
    for poly in mesh.polygons:
        for loop_idx in poly.loop_indices:
            vi = mesh.loops[loop_idx].vertex_index
            w = ob.matrix_world @ mesh.vertices[vi].co
            rgba = _founder_rgba_at(w, lo=lo, span=span, cx=cx, cy=cy, face_sign=face_sign)
            vcol.data[loop_idx].color = rgba


def apply_founder_materials(ob: bpy.types.Object) -> None:
    """Smooth vertex-color paint + single shader material; vertices unchanged."""
    _paint_founder_vertex_colors(ob)
    bc.unify_vertex_color_material(ob, "staff-founder-idle")
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