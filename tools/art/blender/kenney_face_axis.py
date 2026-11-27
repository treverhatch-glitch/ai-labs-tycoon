"""Kenney stock: shoe-derived front axis, face shell, eye landmarks."""
from __future__ import annotations

import statistics
from mathutils import Vector

import bpy

H_SHOE = 0.07
H_NECK = 0.80
H_HEAD_TOP = 0.97
# Eyes at ~55% of head height above the neck band (brows above).
H_EYE_CENTER = H_NECK + 0.55 * (H_HEAD_TOP - H_NECK)


def _pick_mesh(mesh_obs: list[bpy.types.Object]) -> bpy.types.Object | None:
    ob = next(
        (o for o in mesh_obs if o.type == "MESH" and o.name.split(".")[0] == "KenneyFounderStock"),
        None,
    )
    if not ob:
        ob = next((o for o in mesh_obs if o.type == "MESH"), None)
    return ob


def toe_forward(mesh_obs: list[bpy.types.Object]) -> Vector:
    """front = normalize(XY of mean(toe) − mean(heel)), shoe band h < 0.07."""
    ob = _pick_mesh(mesh_obs)
    if not ob:
        return Vector((0.0, -1.0, 0.0))
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    zs = [p.z for p in ws]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    shoes = [p for p in ws if (p.z - lo) / span < H_SHOE]
    if len(shoes) < 4:
        shoes = [p for p in ws if p.z < lo + span * H_SHOE * 1.2]
    if len(shoes) < 4:
        return Vector((0.0, -1.0, 0.0))

    best_ax = 0
    best_spread = -1.0
    for ax in (0, 1):
        spread = max(p[ax] for p in shoes) - min(p[ax] for p in shoes)
        if spread > best_spread:
            best_spread = spread
            best_ax = ax

    med = statistics.median(p[best_ax] for p in shoes)
    toe_pts = [p for p in shoes if p[best_ax] >= med]
    heel_pts = [p for p in shoes if p[best_ax] < med]
    if not toe_pts or not heel_pts:
        toe_pts, heel_pts = shoes[: len(shoes) // 2], shoes[len(shoes) // 2 :]
    mean_toe = sum(toe_pts, Vector()) / len(toe_pts)
    mean_heel = sum(heel_pts, Vector()) / len(heel_pts)
    delta = Vector((mean_toe.x - mean_heel.x, mean_toe.y - mean_heel.y, 0.0))
    if delta.length < 1e-5:
        return Vector((0.0, -1.0, 0.0))
    return delta.normalized()


def resolve_face_forward(mesh_obs: list[bpy.types.Object]) -> Vector:
    return toe_forward(mesh_obs)


def _head_center(ws: list[Vector], lo: float, span: float) -> Vector:
    head = [p for p in ws if (p.z - lo) / span > 0.74]
    return sum(head, Vector()) / len(head) if head else sum(ws, Vector()) / len(ws)


def face_shell_coord(ob: bpy.types.Object, fwd: Vector):
    """Forward dot: positive on toe-forward face."""
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    zs = [p.z for p in ws]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    hc = _head_center(ws, lo, span)
    fn = lambda p, _hc=hc, _f=fwd: (p - _hc).dot(_f)
    return hc, fn


def lateral_from_front(w: Vector, hc: Vector, fwd: Vector) -> float:
    d = Vector((w.x - hc.x, w.y - hc.y, 0.0))
    fwd_xy = Vector((fwd.x, fwd.y, 0.0))
    if fwd_xy.length < 1e-5:
        fwd_xy = Vector((0.0, -1.0, 0.0))
    else:
        fwd_xy.normalize()
    return (d - fwd_xy * d.dot(fwd_xy)).length


def eye_landmarks_world(ob: bpy.types.Object) -> tuple[Vector, Vector, float]:
    """Eye centers from forward-facing FounderEye faces."""
    fwd = toe_forward([ob])
    hc, y_face_fn = face_shell_coord(ob, fwd)
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
        if y_face_fn(wc) < 0.04:
            continue
        pts.append(wc)
    if len(pts) >= 2:
        lat_ax = 1 if abs(fwd.x) >= max(abs(fwd.y), 0.45) else 0
        pts.sort(key=lambda p: p[lat_ax])
        return pts[0], pts[-1], max(p.z for p in pts)

    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    zs = [p.z for p in ws]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    eye_z = lo + span * H_EYE_CENTER
    lat = span * 0.052
    f_xy = Vector((fwd.x, fwd.y, 0.0))
    if f_xy.length < 1e-5:
        f_xy = Vector((0.0, -1.0, 0.0))
    else:
        f_xy.normalize()
    right = Vector((-f_xy.y, f_xy.x, 0.0))
    center = hc + f_xy * (span * 0.08)
    return center - right * lat, center + right * lat, lo + span * (H_EYE_CENTER + 0.05)