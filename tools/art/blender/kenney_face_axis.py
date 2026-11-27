"""Kenney stock mesh: resolve face forward (+Y after canonical stock bake)."""
from __future__ import annotations

from mathutils import Vector

import bpy


def _head_samples(ob: bpy.types.Object) -> list[Vector]:
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    zs = [p.z for p in ws]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    band = [p for p in ws if lo + span * 0.55 <= p.z <= lo + span * 0.78]
    return band if len(band) >= 12 else ws


def resolve_face_forward(mesh_obs: list[bpy.types.Object]) -> Vector:
    """Unit vector from head center toward nose (Z-up Kenney stock)."""
    stock = next(
        (o for o in mesh_obs if o.type == "MESH" and o.name.split(".")[0] == "KenneyFounderStock"),
        None,
    )
    head = next((o for o in mesh_obs if o.type == "MESH" and o.name.split(".")[0] == "Head"), None)
    ob = stock or head
    if not ob:
        return Vector((0.0, 1.0, 0.0))
    pts = _head_samples(ob)
    hc = sum(pts, Vector()) / len(pts)
    best_spread = -1.0
    best_vec = Vector((0.0, 1.0, 0.0))
    for idx in (1, 0):
        vals = [v[idx] for v in pts]
        spread = max(vals) - min(vals)
        if spread < 1e-4:
            continue
        fwd_v = max(pts, key=lambda v: v[idx])
        df = abs(fwd_v[idx] - hc[idx])
        db = abs(min(pts, key=lambda v: v[idx])[idx] - hc[idx])
        if max(df, db) < spread * 0.06:
            continue
        if spread > best_spread:
            best_spread = spread
            if idx == 1:
                best_vec = Vector((0.0, 1.0 if df >= db else -1.0, 0.0))
            else:
                best_vec = Vector((1.0 if fwd_v.x > hc.x else -1.0, 0.0, 0.0))
    return best_vec.normalized()


def eye_landmarks_world(ob: bpy.types.Object) -> tuple[Vector, Vector, float]:
    """Eye decal centers from FounderEye material faces (after recolor)."""
    fwd = resolve_face_forward([ob])
    slots = list(ob.data.materials)
    eye_idx = next((i for i, m in enumerate(slots) if m and "FounderEye" in m.name), 7)
    pts: list[Vector] = []
    mesh = ob.data
    for poly in mesh.polygons:
        if poly.material_index != eye_idx:
            continue
        wc = Vector()
        for li in poly.loop_indices:
            vi = mesh.loops[li].vertex_index
            wc += ob.matrix_world @ mesh.vertices[vi].co
        pts.append(wc / len(poly.loop_indices))
    if len(pts) >= 2:
        pts.sort(key=lambda p: p.x)
        return pts[0], pts[-1], max(p.z for p in pts)
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    zs = [p.z for p in ws]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    cx = sum(p.x for p in ws) / len(ws)
    head = [p for p in ws if p.z > hi - span * 0.28]
    if fwd.y >= 0.5:
        face_y = max(p.y for p in head) - span * 0.025
    elif fwd.y <= -0.5:
        face_y = min(p.y for p in head) + span * 0.025
    elif fwd.x > 0:
        face_y = max(p.x for p in head) - span * 0.025
    else:
        face_y = min(p.x for p in head) + span * 0.025
    eye_z = lo + span * 0.685
    sep = span * 0.072
    if fwd.y >= 0.5 or fwd.y <= -0.5:
        left = Vector((cx - sep, face_y, eye_z))
        right = Vector((cx + sep, face_y, eye_z))
    else:
        left = Vector((face_y - sep, cx - sep * 0.2, eye_z))
        right = Vector((face_y + sep, cx + sep * 0.2, eye_z))
    return left, right, lo + span * 0.735