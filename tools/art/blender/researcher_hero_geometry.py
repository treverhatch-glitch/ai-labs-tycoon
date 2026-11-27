"""Researcher hero — lab coat, hair cap, glasses as separate CC0 Blender geometry."""
from __future__ import annotations

import math

import bpy
import bmesh
from mathutils import Matrix, Vector

import blender_common as bc
from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward
from kenney_geometry_landmarks import compute_landmarks, torso_half_width
from kenney_researcher_stock_recolor import (
    RESEARCHER_HAIR,
    _ensure_material,
    _head_width_at,
)

LAB_COAT_RGB = (237 / 255, 237 / 255, 230 / 255, 1.0)
GLASSES_RGB = (32 / 255, 32 / 255, 32 / 255, 1.0)


def _researcher_head_top(ws: list[Vector], lm) -> float:
    head_pts = [p for p in ws if (p.z - lm.zmin) / lm.span > lm.h_neck + 0.02]
    if not head_pts:
        return 0.96
    zs = sorted((p.z - lm.zmin) / lm.span for p in head_pts)
    return zs[max(0, int(len(zs) * 0.88) - 1)]


def _researcher_chin_h(ws: list[Vector], lm) -> float:
    head_pts = [p for p in ws if (p.z - lm.zmin) / lm.span > lm.h_neck + 0.02]
    if not head_pts:
        return lm.h_neck + 0.08
    return min((p.z - lm.zmin) / lm.span for p in head_pts)


def _researcher_eye_h(ws: list[Vector], lm, head_top: float) -> float:
    """Mid-face (~58% of skull height above chin)."""
    chin_h = _researcher_chin_h(ws, lm)
    return chin_h + 0.45 * (head_top - chin_h)

_VG_COAT_BODY = frozenset(
    {"Hips", "Spine", "Chest", "UpperChest", "LeftShoulder", "RightShoulder"}
)
_VG_COAT_UPPER_ARM = frozenset({"LeftArm", "RightArm"})
_VG_COAT_FOREARM = frozenset({"LeftForeArm", "RightForeArm"})
_VG_COAT_SHOULDER = frozenset({"LeftShoulder", "RightShoulder"})
_VG_COAT_THIGH = frozenset({"LeftUpLeg", "RightUpLeg"})
_VG_HANDS = frozenset(
    {
        "LeftHand",
        "RightHand",
        "LeftHandIndex1",
        "RightHandIndex1",
        "LeftHandThumb1",
        "RightHandThumb1",
    }
)
_VG_COAT_EXCLUDE = frozenset({"Head", "Neck"})
_VG_SLEEVE = frozenset(
    {
        "LeftShoulder",
        "RightShoulder",
        "LeftArm",
        "RightArm",
        "LeftForeArm",
        "RightForeArm",
    }
)


def _coat_open_v(
    lat: float,
    thw: float,
    h: float,
    lm,
    y_face: float,
) -> bool:
    if y_face < 0.02:
        return False
    v_lo = lm.h_neck - 0.18
    v_hi = lm.h_waist - 0.02
    if h < v_lo or h > v_hi:
        return False
    t = (h - v_lo) / max(1e-4, v_hi - v_lo)
    return lat < thw * (0.36 - t * 0.16)


def _sleeve_source_face(
    face: bmesh.types.BMFace,
    ob: bpy.types.Object,
    *,
    lm,
    lo: float,
    span: float,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> bool:
    wc = _world_centroid(face, ob)
    h = (wc.z - lo) / span
    if h < lm.h_neck - 0.22 or h > lm.h_waist - 0.05:
        return False
    hand = sum(_vg_weight(ob, v.index, _VG_HANDS) for v in face.verts) / max(1, len(face.verts))
    if hand > 0.14:
        return False
    arm = sum(_vg_weight(ob, v.index, _VG_SLEEVE) for v in face.verts) / max(1, len(face.verts))
    if arm < 0.08:
        return False
    lat = lateral_from_front(wc, hc, fwd)
    thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
    return lat > thw * 0.54


def _sleeve_grow_face(
    face: bmesh.types.BMFace,
    ob: bpy.types.Object,
    *,
    lm,
    lo: float,
    span: float,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> bool:
    if _sleeve_source_face(
        face, ob, lm=lm, lo=lo, span=span, hc=hc, fwd=fwd, ws=ws
    ):
        return True
    wc = _world_centroid(face, ob)
    h = (wc.z - lo) / span
    if h < lm.h_neck - 0.24 or h > lm.h_waist - 0.04:
        return False
    hand = sum(_vg_weight(ob, v.index, _VG_HANDS) for v in face.verts) / max(1, len(face.verts))
    if hand > 0.18:
        return False
    arm = sum(_vg_weight(ob, v.index, _VG_SLEEVE) for v in face.verts) / max(1, len(face.verts))
    if arm < 0.06:
        return False
    lat = lateral_from_front(wc, hc, fwd)
    thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
    return lat > thw * 0.50


def _vg_weight(ob: bpy.types.Object, vi: int, names: frozenset[str]) -> float:
    total = 0.0
    for g in ob.data.vertices[vi].groups:
        vg = ob.vertex_groups[g.group]
        if vg.name in names:
            total += g.weight
    return total


def _face_dominated_by_coat_bones(
    face: bmesh.types.BMFace,
    ob: bpy.types.Object,
    *,
    exclude_head_neck: bool,
) -> bool:
    coat = 0.0
    bad = 0.0
    for v in face.verts:
        coat += _vg_weight(ob, v.index, _VG_COAT_BODY)
        coat += _vg_weight(ob, v.index, _VG_COAT_UPPER_ARM) * 0.85
        bad += _vg_weight(ob, v.index, _VG_COAT_EXCLUDE)
    if coat < 0.38:
        return False
    if exclude_head_neck and bad > 0.12:
        return False
    if bad > coat * 0.35:
        return False
    return True


def _world_centroid(face: bmesh.types.BMFace, ob: bpy.types.Object) -> Vector:
    wc = Vector()
    for v in face.verts:
        wc += ob.matrix_world @ v.co
    return wc / len(face.verts)


def _all_verts_under_chin(
    face: bmesh.types.BMFace,
    ob: bpy.types.Object,
    lm,
    chin_h: float,
) -> bool:
    lo, span = lm.zmin, lm.span
    chin_z = lo + span * chin_h
    for v in face.verts:
        if (ob.matrix_world @ v.co).z > chin_z:
            return False
    return True


def _arm_coat_face(
    face: bmesh.types.BMFace,
    ob: bpy.types.Object,
    *,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> bool:
    arm = sum(
        _vg_weight(ob, v.index, _VG_COAT_UPPER_ARM)
        + _vg_weight(ob, v.index, _VG_COAT_FOREARM) * 0.9
        for v in face.verts
    )
    if arm < 0.85:
        return False
    if not _all_verts_under_chin(face, ob, lm, lm.h_neck - 0.14):
        return False
    for v in face.verts:
        if _vg_weight(ob, v.index, _VG_HANDS) > 0.14:
            return False
        if _vg_weight(ob, v.index, _VG_COAT_EXCLUDE) > 0.06:
            return False
    lo, span = lm.zmin, lm.span
    wc = _world_centroid(face, ob)
    h = (wc.z - lo) / span
    wrist = lm.h_waist - 0.06
    if h < wrist or h > lm.h_neck - 0.16:
        return False
    lat = lateral_from_front(wc, hc, fwd)
    thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
    return lat > thw + span * 0.008


def _append_coat_hem_quads(
    bm: bmesh.types.BMesh,
    body: bpy.types.Object,
    *,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> None:
    from founder_face_decals import _fwd_basis

    _, right = _fwd_basis(fwd)
    lo, span = lm.zmin, lm.span
    hw = _head_width_at(ws, lo, span, lm.h_eye)
    z0 = lo + span * (lm.h_waist + 0.01)
    z1 = lo + span * (lm.h_waist + 0.44)
    proud = hw * 0.11
    flare = hw * 0.15
    inv = body.matrix_world.inverted()

    def _lv(w: Vector) -> Vector:
        return inv @ w

    for side in (-1.0, 1.0):
        mid = hc + right * (side * hw * 0.22) + fwd * proud
        inner_lo = _lv(Vector((mid.x - right.x * hw * 0.05, mid.y - right.y * hw * 0.05, z0)))
        inner_hi = _lv(Vector((mid.x - right.x * hw * 0.05, mid.y - right.y * hw * 0.05, z1)))
        out = mid + right * (side * flare * 1.15)
        outer_lo = _lv(Vector((out.x, out.y, z0)))
        outer_hi = _lv(Vector((out.x, out.y, z1)))
        vs = [bm.verts.new(c) for c in (inner_lo, outer_lo, outer_hi, inner_hi)]
        bm.faces.new(tuple(vs))


def _is_coat_source_face(
    face: bmesh.types.BMFace,
    ob: bpy.types.Object,
    *,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
    y_face_fn,
) -> bool:
    if _arm_coat_face(face, ob, lm=lm, hc=hc, fwd=fwd, ws=ws):
        return True
    chest_w = sum(
        _vg_weight(ob, v.index, frozenset({"Chest", "UpperChest", "Spine"}))
        for v in face.verts
    )
    lo, span = lm.zmin, lm.span
    wc = _world_centroid(face, ob)
    h = (wc.z - lo) / span
    lat = lateral_from_front(wc, hc, fwd)
    thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
    if (
        chest_w >= 0.75
        and _all_verts_under_chin(face, ob, lm, lm.h_neck - 0.17)
        and lm.h_waist - 0.14 <= h <= lm.h_neck - 0.19
        and lat <= thw + span * 0.018
    ):
        y_face = y_face_fn(wc)
        if not (y_face > 0.07 and lat < thw * 0.11):
            for v in face.verts:
                if _vg_weight(ob, v.index, _VG_COAT_EXCLUDE) > 0.06:
                    break
            else:
                return True
    if not _face_dominated_by_coat_bones(face, ob, exclude_head_neck=True):
        return False
    coat_w = sum(
        _vg_weight(ob, v.index, _VG_COAT_BODY) + _vg_weight(ob, v.index, _VG_COAT_UPPER_ARM)
        for v in face.verts
    )
    lo, span = lm.zmin, lm.span
    wc = _world_centroid(face, ob)
    h = (wc.z - lo) / span
    if not _all_verts_under_chin(face, ob, lm, lm.h_neck - 0.17):
        return False
    for v in face.verts:
        if _vg_weight(ob, v.index, _VG_COAT_EXCLUDE) > 0.05:
            return False
    lo, span = lm.zmin, lm.span
    wc = _world_centroid(face, ob)
    h = (wc.z - lo) / span
    lat = lateral_from_front(wc, hc, fwd)
    thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
    hip_w = sum(_vg_weight(ob, v.index, frozenset({"Hips"})) for v in face.verts)
    if hip_w >= 0.85 and lm.h_waist - 0.12 <= h <= lm.h_waist + 0.06:
        y_face = y_face_fn(wc)
        if y_face > -0.22 and lat < thw * 1.08:
            return True
    if h < lm.h_waist - 0.14 or h > lm.h_neck - 0.20:
        return False
    y_face = y_face_fn(wc)
    if lat > thw + span * 0.016:
        return False
    if y_face < -0.36:
        return False
    if y_face > 0.05 and lat < thw * 0.26:
        return False
    return True


def _trim_coat_above_chin(
    coat: bpy.types.Object,
    body: bpy.types.Object,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
    y_face_fn,
) -> None:
    lo, span = lm.zmin, lm.span
    chin_z = lo + span * (lm.h_neck - 0.20)
    bm = bmesh.new()
    bm.from_mesh(coat.data)
    bm.faces.ensure_lookup_table()
    doomed: list[bmesh.types.BMFace] = []
    for face in bm.faces:
        wc = coat.matrix_world @ face.calc_center_median()
        if wc.z > chin_z:
            doomed.append(face)
            continue
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        y_face = y_face_fn(wc)
        if h > lm.h_neck - 0.24 and y_face > -0.05 and lat < thw * 0.95:
            doomed.append(face)
    if doomed:
        bmesh.ops.delete(bm, geom=doomed, context="FACES")
    bm.to_mesh(coat.data)
    coat.data.update()
    bm.free()


def _trim_coat_orphan_shards(
    coat: bpy.types.Object,
    body: bpy.types.Object,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
    y_face_fn,
) -> None:
    """Remove floating hip / forearm coat islands."""
    lo, span = lm.zmin, lm.span
    bm = bmesh.new()
    bm.from_mesh(coat.data)
    bm.faces.ensure_lookup_table()
    doomed: list[bmesh.types.BMFace] = []
    for face in bm.faces:
        if len(face.verts) < 3:
            doomed.append(face)
            continue
        wc = coat.matrix_world @ face.calc_center_median()
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if (
            lm.h_waist + 0.21 <= h <= lm.h_waist + 0.30
            and lat > thw * 1.08
            and y_face_fn(wc) < 0.05
        ):
            doomed.append(face)
    if doomed:
        bmesh.ops.delete(bm, geom=doomed, context="FACES")
    bm.to_mesh(coat.data)
    coat.data.update()
    bm.free()


def _trim_coat_hands_crotch(
    coat: bpy.types.Object,
    body: bpy.types.Object,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
    y_face_fn,
) -> None:
    lo, span = lm.zmin, lm.span
    bm = bmesh.new()
    bm.from_mesh(coat.data)
    bm.faces.ensure_lookup_table()
    doomed: list[bmesh.types.BMFace] = []
    for face in bm.faces:
        wc = _world_centroid(face, coat)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        y_face = y_face_fn(wc)
        if h < lm.h_waist - 0.11 and lat > thw * 0.52:
            handish = lat > thw * 0.72 and h < lm.h_waist - 0.09
            if handish:
                doomed.append(face)
                continue
        if h < lm.h_waist - 0.16 and lat < thw * 0.52 and y_face > -0.20:
            doomed.append(face)
            continue
        if h < lm.h_waist + 0.02 and lat < thw * 0.45 and y_face > -0.12:
            doomed.append(face)
    if doomed:
        bmesh.ops.delete(bm, geom=doomed, context="FACES")
    bm.to_mesh(coat.data)
    coat.data.update()
    bm.free()


def _prune_coat_wrist_shards(
    coat: bpy.types.Object,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> None:
    lo, span = lm.zmin, lm.span
    bm = bmesh.new()
    bm.from_mesh(coat.data)
    bm.faces.ensure_lookup_table()
    visited: set[int] = set()
    doomed: list[bmesh.types.BMFace] = []
    for seed in bm.faces:
        if seed.index in visited:
            continue
        stack = [seed]
        island: list[bmesh.types.BMFace] = []
        while stack:
            face = stack.pop()
            if face.index in visited:
                continue
            visited.add(face.index)
            island.append(face)
            for e in face.edges:
                for nb in e.link_faces:
                    if nb.index not in visited:
                        stack.append(nb)
        if len(island) > 20:
            continue
        wc = Vector()
        for f in island:
            wc += _world_centroid(f, coat)
        wc /= len(island)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if h < lm.h_waist - 0.08 and lat > thw * 0.58:
            doomed.extend(island)
    if doomed:
        bmesh.ops.delete(bm, geom=doomed, context="FACES")
    bm.to_mesh(coat.data)
    coat.data.update()
    bm.free()


def _fwd_right_flat(fwd: Vector) -> tuple[Vector, Vector]:
    f_xy = Vector((fwd.x, fwd.y, 0.0))
    if f_xy.length < 1e-6:
        f_xy = Vector((0.0, -1.0, 0.0))
    else:
        f_xy.normalize()
    right = Vector((-f_xy.y, f_xy.x, 0.0))
    return f_xy, right


def _append_bmesh(target: bmesh.types.BMesh, src: bmesh.types.BMesh) -> None:
    vmap: dict[bmesh.types.BMVert, bmesh.types.BMVert] = {}
    for v in src.verts:
        vmap[v] = target.verts.new(v.co.copy())
    for f in src.faces:
        try:
            target.faces.new([vmap[v] for v in f.verts])
        except ValueError:
            pass


def _vg_weighted_centroid(
    ob: bpy.types.Object,
    names: frozenset[str],
    *,
    lo: float,
    span: float,
    h_lo: float,
    h_hi: float,
    min_w: float = 0.14,
) -> Vector | None:
    acc = Vector()
    n = 0
    for v in ob.data.vertices:
        w = _vg_weight(ob, v.index, names)
        if w < min_w:
            continue
        wc = ob.matrix_world @ v.co
        h = (wc.z - lo) / span
        if h < h_lo or h > h_hi:
            continue
        acc += wc * w
        n += w
    if n < 1e-6:
        return None
    return acc / n


def _sleeve_axis_world(
    body: bpy.types.Object,
    side: str,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> tuple[Vector, Vector, float, float]:
    lo, span = lm.zmin, lm.span
    lat_sign = 1.0 if side == "Right" else -1.0
    _, right = _fwd_right_flat(fwd)
    h_sh = lm.h_neck - 0.22
    h_wr = lm.h_waist - 0.14
    thw_sh = torso_half_width(ws, lo, span, h_sh, lm.cx, lm.cy)
    thw_wr = torso_half_width(ws, lo, span, h_wr, lm.cx, lm.cy)
    z_sh = lo + span * h_sh
    z_wr = lo + span * h_wr
    sh = hc + right * (lat_sign * thw_sh * 0.80) + Vector((0.0, 0.0, z_sh - hc.z))
    wr = hc + right * (lat_sign * thw_wr * 0.74) + Vector((0.0, 0.0, z_wr - hc.z))
    xy_sh = _vg_weighted_centroid(
        body,
        frozenset({f"{side}Shoulder", f"{side}Arm"}),
        lo=lo,
        span=span,
        h_lo=h_sh - 0.04,
        h_hi=h_sh + 0.04,
    )
    xy_wr = _vg_weighted_centroid(
        body,
        frozenset({f"{side}ForeArm"}),
        lo=lo,
        span=span,
        h_lo=h_wr - 0.05,
        h_hi=h_wr + 0.02,
        min_w=0.08,
    )
    if xy_sh is not None:
        sh.x, sh.y = xy_sh.x, xy_sh.y
    if xy_wr is not None:
        wr.x, wr.y = xy_wr.x, xy_wr.y
    if wr.z >= sh.z - span * 0.02:
        wr.z = z_wr
        sh.z = z_sh
    r_top = max(span * 0.030, thw_sh * 0.17)
    r_bot = max(span * 0.026, r_top * 0.76)
    return sh, wr, r_top, r_bot


def _build_tapered_sleeve_bmesh(
    body: bpy.types.Object,
    p_top: Vector,
    p_bot: Vector,
    r_top: float,
    r_bot: float,
    *,
    segments: int = 10,
    rings: int = 12,
    cuff_scale: float = 1.12,
) -> bmesh.types.BMesh:
    inv = body.matrix_world.inverted()
    axis = p_bot - p_top
    length = axis.length
    bm = bmesh.new()
    if length < 1e-6:
        return bm
    axis_n = axis / length
    aux = Vector((0.0, 0.0, 1.0))
    if abs(axis_n.dot(aux)) > 0.9:
        aux = Vector((0.0, 1.0, 0.0))
    right = axis_n.cross(aux).normalized()
    up = right.cross(axis_n).normalized()
    vgrid: list[list[bmesh.types.BMVert]] = []
    for ri in range(rings):
        t = ri / max(1, rings - 1)
        rad = r_top * (1.0 - t) + r_bot * t
        if t > 0.93:
            rad = r_bot * cuff_scale
        center = p_top + axis * t
        ring: list[bmesh.types.BMVert] = []
        for si in range(segments):
            ang = 2.0 * math.pi * si / segments
            offset = right * math.cos(ang) * rad + up * math.sin(ang) * rad
            ring.append(bm.verts.new(inv @ (center + offset)))
        vgrid.append(ring)
    for ri in range(rings - 1):
        for si in range(segments):
            s2 = (si + 1) % segments
            try:
                bm.faces.new((vgrid[ri][si], vgrid[ri][s2], vgrid[ri + 1][s2], vgrid[ri + 1][si]))
            except ValueError:
                pass
    return bm


def _coat_front_open(dir_xy: Vector, fwd_xy: Vector) -> bool:
    return dir_xy.dot(-fwd_xy) > math.cos(math.radians(38.0))


def _coat_width_at(ws, lo, span, h_norm, lm, flare: float) -> float:
    return torso_half_width(ws, lo, span, h_norm, lm.cx, lm.cy) * 1.08 * flare + span * 0.010


def _build_open_coat_panels_bmesh(
    body: bpy.types.Object,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> bmesh.types.BMesh:
    lo, span = lm.zmin, lm.span
    z_top = lo + span * (lm.h_neck - 0.06)
    z_back_top = lo + span * (lm.h_neck + 0.02)
    z_hem = lo + span * (lm.h_waist + 0.42)
    fwd_xy, right_xy = _fwd_right_flat(fwd)
    inv = body.matrix_world.inverted()
    bm = bmesh.new()
    open_front = 0.44
    coat_proud = span * 0.028

    def _corner(z: float, lat_scale: float, proud: float, flare: float) -> bmesh.types.BMVert:
        h = (z - lo) / span
        half_w = _coat_width_at(ws, lo, span, h, lm, flare)
        center = Vector((hc.x, hc.y, z))
        return bm.verts.new(inv @ (center + right_xy * (lat_scale * half_w) - fwd_xy * proud))

    flare_top, flare_hem = 1.04, 1.20
    back_p = coat_proud * 2.72
    blt = _corner(z_back_top, -1.0, back_p, flare_top)
    brt = _corner(z_back_top, 1.0, back_p, flare_top)
    blb = _corner(z_hem, -1.0, back_p, flare_hem)
    brb = _corner(z_hem, 1.0, back_p, flare_hem)
    try:
        bm.faces.new((blt, brt, brb, blb))
    except ValueError:
        pass

    for sign in (-1.0, 1.0):
        outer_t = _corner(z_top, sign * 1.04, coat_proud, flare_top)
        inner_t = _corner(z_top, sign * open_front, coat_proud, flare_top)
        inner_b = _corner(z_hem, sign * open_front, coat_proud, flare_hem)
        outer_b = _corner(z_hem, sign * 1.08, coat_proud * 1.35, flare_hem)
        try:
            if sign < 0:
                bm.faces.new((outer_t, inner_t, inner_b, outer_b))
            else:
                bm.faces.new((inner_t, outer_t, outer_b, inner_b))
        except ValueError:
            pass

    for sign in (-1.0, 1.0):
        h_pad = lm.h_waist + 0.38
        hw = _coat_width_at(ws, lo, span, h_pad, lm, 1.14)
        pad_c = hc + right_xy * (sign * hw * 1.08) - fwd_xy * (span * 0.058)
        pad_c.z = lo + span * h_pad
        pad_m = Matrix.Translation(inv @ pad_c) @ Matrix.Diagonal(
            (span * 0.19, span * 0.10, span * 0.26, 1.0)
        )
        bmesh.ops.create_cube(bm, size=1.0, matrix=pad_m)

    return bm


def spawn_researcher_lab_coat(body: bpy.types.Object) -> bpy.types.Object:
    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH" and ob.name.split(".")[0] == "ResearcherLabCoat":
            bpy.data.objects.remove(ob, do_unlink=True)
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _y_face_fn = face_shell_coord(body, fwd)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    bm = _build_open_coat_panels_bmesh(body, lm, hc, fwd, ws)
    for side in ("Left", "Right"):
        p0, p1, rt, rb = _sleeve_axis_world(body, side, lm, hc, fwd, ws)
        slv = _build_tapered_sleeve_bmesh(body, p0, p1, rt, rb, segments=10, rings=12)
        _append_bmesh(bm, slv)
        slv.free()
    if len(bm.faces) < 24:
        raise RuntimeError(f"FAIL staff-researcher-idle lab_coat_faces={len(bm.faces)} too_few")
    mesh = bpy.data.meshes.new("ResearcherLabCoatMesh")
    bm.to_mesh(mesh)
    bm.free()
    coat = bpy.data.objects.new("ResearcherLabCoat", mesh)
    bpy.context.collection.objects.link(coat)
    mat = _ensure_material("ResearcherLabCoatGeo", LAB_COAT_RGB, 0.68)
    coat.data.materials.append(mat)
    coat.parent = body
    coat.matrix_parent_inverse = body.matrix_world.inverted()
    bc.smooth_shade(coat)
    return coat


def spawn_researcher_hair_cap(body: bpy.types.Object) -> bpy.types.Object:
    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH" and ob.name.split(".")[0] == "ResearcherHairCap":
            bpy.data.objects.remove(ob, do_unlink=True)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    head_top = _researcher_head_top(ws, lm)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    crown_z = lm.zmin + lm.span * (head_top - 0.03)
    crown = hc - fwd * (head_w * 0.10) + Vector((0.0, 0.0, crown_z - hc.z))
    local = body.matrix_world.inverted() @ crown
    bpy.ops.mesh.primitive_uv_sphere_add(
        segments=18,
        ring_count=12,
        radius=head_w * 0.52,
        location=local,
    )
    cap = bpy.context.active_object
    cap.name = "ResearcherHairCap"
    cap.scale = (1.18, 0.95, 0.58)
    bpy.ops.object.transform_apply(scale=True)
    cap.parent = body
    cap.matrix_parent_inverse = body.matrix_world.inverted()
    mat = _ensure_material("ResearcherHairCapMat", RESEARCHER_HAIR, 0.52)
    cap.data.materials.append(mat)
    bc.smooth_shade(cap)
    return cap


def spawn_researcher_glasses(body: bpy.types.Object) -> bpy.types.Object:
    from founder_face_decals import _body_bvh_evaluated, _fwd_basis, _parent_keep_world, _ray_hit_face

    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH" and ob.name.split(".")[0].startswith("ResearcherGlasses"):
            bpy.data.objects.remove(ob, do_unlink=True)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    _, right = _fwd_basis(fwd)
    head_top = _researcher_head_top(ws, lm)
    eye_h = _researcher_eye_h(ws, lm, head_top)
    eye_z = lm.zmin + lm.span * eye_h
    eye_radius = head_w * 0.036
    eye_diameter = eye_radius * 2.0
    half_sep = head_w * 0.158
    full_sep = half_sep * 2.0
    proud = max(lm.span * 0.014, head_w * 0.048, 0.012)
    mat = _ensure_material("ResearcherGlassesFrame", GLASSES_RGB, 0.04, specular=0.02)
    rot = fwd.to_track_quat("Y", "Z").to_euler()
    up = Vector((0.0, 0.0, 1.0))
    bvh = _body_bvh_evaluated(body)
    cast_in = -fwd
    parts: list[bpy.types.Object] = []
    outer = eye_diameter * 1.30
    bar = max(full_sep * 0.088, head_w * 0.0086)
    half = outer * 0.5

    def _rect_frame(center_w: Vector, lateral_sign: float) -> bpy.types.Object:
        local_parts: list[bpy.types.Object] = []
        side_len = outer * 0.92
        temple = center_w + right * (lateral_sign * (half - bar * 0.44))
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=temple)
        side = bpy.context.active_object
        side.scale = (bar * 1.10, bar * 1.14, side_len)
        side.rotation_euler = rot
        bpy.ops.object.transform_apply(scale=True, rotation=True)
        side.data.materials.append(mat)
        local_parts.append(side)
        brow = center_w + up * (half - bar * 0.46)
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=brow)
        top = bpy.context.active_object
        top.scale = (outer * 0.88, bar * 1.06, bar * 1.02)
        top.rotation_euler = rot
        bpy.ops.object.transform_apply(scale=True, rotation=True)
        top.data.materials.append(mat)
        local_parts.append(top)
        cheek = center_w - up * (half - bar * 0.46)
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=cheek)
        bottom = bpy.context.active_object
        bottom.scale = (outer * 0.88, bar * 1.06, bar * 1.02)
        bottom.rotation_euler = rot
        bpy.ops.object.transform_apply(scale=True, rotation=True)
        bottom.data.materials.append(mat)
        local_parts.append(bottom)
        bpy.ops.object.select_all(action="DESELECT")
        for p in local_parts:
            p.select_set(True)
        bpy.context.view_layer.objects.active = local_parts[0]
        bpy.ops.object.join()
        return bpy.context.active_object

    anchors: list[Vector] = []
    for sign in (-1.0, 1.0):
        lat = right * (half_sep * sign)
        seed = hc + lat + Vector((0.0, 0.0, eye_z - hc.z))
        hit = _ray_hit_face(bvh, seed + fwd * 0.25, cast_in)
        if hit is not None:
            loc, norm = hit
            outward = norm if norm.dot(fwd) >= 0 else -norm
            center = loc + outward * proud
        else:
            center = seed + fwd * proud
        anchors.append(center)
    parts.append(_rect_frame(anchors[0], -1.0))
    parts.append(_rect_frame(anchors[1], 1.0))
    mid = (anchors[0] + anchors[1]) * 0.5
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=mid + up * (half - bar * 0.40))
    bridge = bpy.context.active_object
    bridge.scale = (half_sep * 0.14, bar * 0.96, bar * 0.78)
    bridge.rotation_euler = rot
    bpy.ops.object.transform_apply(scale=True, rotation=True)
    bridge.data.materials.append(mat)
    parts.append(bridge)
    temple_l = anchors[0] + right * (-half - bar * 0.95)
    temple_r = anchors[1] + right * (half + bar * 0.95)
    for loc in (temple_l, temple_r):
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=loc)
        arm = bpy.context.active_object
        arm.scale = (bar * 2.6, bar * 1.04, bar * 0.88)
        arm.rotation_euler = rot
        bpy.ops.object.transform_apply(scale=True, rotation=True)
        arm.data.materials.append(mat)
        parts.append(arm)
    bpy.ops.object.select_all(action="DESELECT")
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = parts[0]
    bpy.ops.object.join()
    glasses = bpy.context.active_object
    glasses.name = "ResearcherGlasses"
    bc.smooth_shade(glasses)
    bpy.context.view_layer.objects.active = glasses
    glasses.select_set(True)
    bpy.ops.object.origin_set(type="ORIGIN_GEOMETRY", center="MEDIAN")
    _parent_keep_world(glasses, body)
    return glasses