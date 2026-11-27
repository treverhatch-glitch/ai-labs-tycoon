"""Kenney stock: toe-forward axis and eye decal landmarks."""
from __future__ import annotations

from mathutils import Vector

import bpy


def toe_forward(mesh_obs: list[bpy.types.Object]) -> Vector:
    """Toe → heel horizontal axis (camera sits on toe/forward side)."""
    ob = next(
        (o for o in mesh_obs if o.type == "MESH" and o.name.split(".")[0] == "KenneyFounderStock"),
        None,
    )
    if not ob:
        ob = next((o for o in mesh_obs if o.type == "MESH"), None)
    if not ob:
        return Vector((0.0, 1.0, 0.0))
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    zs = [p.z for p in ws]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    feet = [p for p in ws if (p.z - lo) / span < 0.12]
    if len(feet) < 6:
        feet = [p for p in ws if p.z < lo + span * 0.15]
    if not feet:
        return Vector((0.0, 1.0, 0.0))
    best_spread = -1.0
    best = Vector((0.0, 1.0, 0.0))
    for axis in (1, 0):
        toe = max(feet, key=lambda v: v[axis])
        heel = min(feet, key=lambda v: v[axis])
        spread = abs(toe[axis] - heel[axis])
        if spread > best_spread:
            best_spread = spread
            delta = toe - heel
            delta.z = 0.0
            if delta.length > 1e-5:
                best = delta.normalized()
            elif axis == 1:
                best = Vector((0.0, 1.0, 0.0))
            else:
                best = Vector((1.0, 0.0, 0.0))
    return best


def resolve_face_forward(mesh_obs: list[bpy.types.Object]) -> Vector:
    """Alias: preview + materials use toe-forward (+Y after stock bake)."""
    return toe_forward(mesh_obs)


def face_shell_coord(ob: bpy.types.Object, fwd: Vector):
    """Forward dot product: positive toward face (toe-forward side)."""
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    zs = [p.z for p in ws]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    head = [p for p in ws if (p.z - lo) / span > 0.74]
    hc = sum(head, Vector()) / len(head) if head else sum(ws, Vector()) / len(ws)
    fn = lambda p, _hc=hc, _f=fwd: (p - _hc).dot(_f)
    return hc.y, fn


def eye_landmarks_world(ob: bpy.types.Object) -> tuple[Vector, Vector, float]:
    """Eye decal centers from FounderEye material faces (after recolor)."""
    fwd = toe_forward([ob])
    _, y_face_fn = face_shell_coord(ob, fwd)
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
        wc /= len(poly.loop_indices)
        if y_face_fn(wc) < 0.02:
            continue
        pts.append(wc)
    if len(pts) >= 2:
        lateral = 1 if abs(fwd.x) >= max(abs(fwd.y), 0.5) else 0
        pts.sort(key=lambda p: p[lateral])
        return pts[0], pts[-1], max(p.z for p in pts)
    fwd = toe_forward([ob])
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    zs = [p.z for p in ws]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    cx = sum(p.x for p in ws) / len(ws)
    shell, _ = face_shell_coord(ob, fwd)
    if abs(fwd.y) > 0.6:
        face_y = shell
        eye_z = lo + span * 0.855
        sep = span * 0.074
        return Vector((cx - sep, face_y, eye_z)), Vector((cx + sep, face_y, eye_z)), lo + span * 0.89
    eye_z = lo + span * 0.855
    sep = span * 0.074
    return Vector((shell - sep, cx, eye_z)), Vector((shell + sep, cx, eye_z)), lo + span * 0.89