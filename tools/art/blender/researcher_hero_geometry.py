"""Researcher hero — lab coat, hair cap, glasses as separate CC0 Blender geometry."""
from __future__ import annotations

import math

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc
from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward
from kenney_geometry_landmarks import compute_landmarks, torso_half_width
from kenney_researcher_stock_recolor import (
    RESEARCHER_HAIR,
    _ensure_material,
    _head_width_at,
)

LAB_COAT_RGB = (228 / 255, 226 / 255, 220 / 255, 1.0)
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
_VG_COAT_SHELL = frozenset(
    {
        "Hips",
        "Spine",
        "Chest",
        "UpperChest",
        "LeftShoulder",
        "RightShoulder",
        "LeftArm",
        "RightArm",
        "LeftForeArm",
        "RightForeArm",
        "LeftUpLeg",
        "RightUpLeg",
    }
)
_VG_COAT_BLOCK = frozenset(
    {
        "Head",
        "Neck",
        "LeftLeg",
        "RightLeg",
        "LeftFoot",
        "RightFoot",
        "LeftToes",
        "RightToes",
    }
)
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
        handish = lat > thw * 0.68 and h < lm.h_waist - 0.07
        if handish and y_face > -0.08:
            doomed.append(face)
    if doomed:
        bmesh.ops.delete(bm, geom=doomed, context="FACES")
    bm.to_mesh(coat.data)
    coat.data.update()
    bm.free()


def _face_mean_vg(ob: bpy.types.Object, face: bmesh.types.BMFace, names: frozenset[str]) -> float:
    return sum(_vg_weight(ob, v.index, names) for v in face.verts) / max(1, len(face.verts))


def _face_in_coat_height_band(
    face: bmesh.types.BMFace,
    ob: bpy.types.Object,
    *,
    lo: float,
    span: float,
    h_lo: float,
    h_hi: float,
) -> bool:
    for v in face.verts:
        h = ((ob.matrix_world @ v.co).z - lo) / span
        if h < h_lo or h > h_hi:
            return False
    return True


def _coat_open_front(
    face: bmesh.types.BMFace,
    body: bpy.types.Object,
    *,
    lm,
    lo: float,
    span: float,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
    y_face_fn,
) -> bool:
    wc = _world_centroid(face, body)
    h = (wc.z - lo) / span
    lat = lateral_from_front(wc, hc, fwd)
    thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
    y_face = y_face_fn(wc)
    if y_face < 0.04:
        return False
    if h < lm.h_neck - 0.10 or h > lm.h_neck + 0.05:
        return False
    neck_w = _face_mean_vg(body, face, frozenset({"Neck"}))
    if neck_w < 0.06:
        return False
    return lat < thw * 0.26


def _duplicate_coat_shell_faces(
    body: bpy.types.Object,
    bm_src: bmesh.types.BMesh,
    *,
    lm,
    lo: float,
    span: float,
    coat_hem: float,
    coat_top: float,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
    y_face_fn,
) -> set[int]:
    pick: set[int] = set()
    for f in bm_src.faces:
        if not _face_in_coat_height_band(
            f, body, lo=lo, span=span, h_lo=coat_hem, h_hi=coat_top
        ):
            continue
        if _face_mean_vg(body, f, _VG_COAT_BLOCK) > 0.10:
            continue
        if _face_mean_vg(body, f, _VG_HANDS) > 0.16:
            continue
        if _face_mean_vg(body, f, _VG_COAT_SHELL) < 0.20:
            continue
        if _coat_open_front(
            f, body, lm=lm, lo=lo, span=span, hc=hc, fwd=fwd, ws=ws, y_face_fn=y_face_fn
        ):
            continue
        pick.add(f.index)

    changed = True
    while changed:
        changed = False
        for f in bm_src.faces:
            if f.index in pick:
                continue
            if not any(nb.index in pick for e in f.edges for nb in e.link_faces):
                continue
            if not _face_in_coat_height_band(
                f, body, lo=lo, span=span, h_lo=coat_hem, h_hi=coat_top
            ):
                continue
            if _face_mean_vg(body, f, _VG_COAT_BLOCK) > 0.12:
                continue
            if _face_mean_vg(body, f, _VG_HANDS) > 0.20:
                continue
            if _face_mean_vg(body, f, _VG_COAT_SHELL) < 0.06:
                continue
            if _coat_open_front(
                f, body, lm=lm, lo=lo, span=span, hc=hc, fwd=fwd, ws=ws, y_face_fn=y_face_fn
            ):
                continue
            pick.add(f.index)
            changed = True
    return pick


def _bisect_coat_below_hem(coat: bpy.types.Object, lm, coat_hem: float) -> None:
    lo, span = lm.zmin, lm.span
    z_cut = lo + span * coat_hem
    inv = coat.matrix_world.inverted()
    plane_co = inv @ Vector((0.0, 0.0, z_cut))
    plane_no = (inv.to_3x3() @ Vector((0.0, 0.0, 1.0))).normalized()
    bm = bmesh.new()
    bm.from_mesh(coat.data)
    geom = list(bm.verts) + list(bm.edges) + list(bm.faces)
    bmesh.ops.bisect_plane(
        bm,
        geom=geom,
        dist=0.0002,
        plane_co=plane_co,
        plane_no=plane_no,
        clear_inner=True,
        clear_outer=False,
    )
    loose = [v for v in bm.verts if not v.link_faces]
    if loose:
        bmesh.ops.delete(bm, geom=loose, context="VERTS")
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


def _prune_coat_micro_islands(coat: bpy.types.Object, *, min_faces: int = 10) -> None:
    """Drop floating solidify shards; keep main coat shell (+ sleeves)."""
    bm = bmesh.new()
    bm.from_mesh(coat.data)
    bm.faces.ensure_lookup_table()
    visited: set[int] = set()
    islands: list[list[bmesh.types.BMFace]] = []
    for seed in bm.faces:
        if seed.index in visited:
            continue
        stack = [seed]
        island: list[bmesh.types.BMFace] = []
        visited.add(seed.index)
        while stack:
            face = stack.pop()
            island.append(face)
            for e in face.edges:
                for nb in e.link_faces:
                    if nb.index not in visited:
                        visited.add(nb.index)
                        stack.append(nb)
        islands.append(island)
    if len(islands) <= 1:
        bm.free()
        return
    islands.sort(key=len, reverse=True)
    main_n = len(islands[0])
    doomed: list[bmesh.types.BMFace] = []
    for isl in islands[1:]:
        if len(isl) < min_faces or len(isl) < max(12, int(main_n * 0.06)):
            doomed.extend(isl)
    if doomed:
        bmesh.ops.delete(bm, geom=doomed, context="FACES")
    loose = [v for v in bm.verts if not v.link_faces]
    if loose:
        bmesh.ops.delete(bm, geom=loose, context="VERTS")
    bm.to_mesh(coat.data)
    coat.data.update()
    bm.free()


def _coat_body_basis(fwd: Vector) -> tuple[Vector, Vector, Vector]:
    from founder_face_decals import _fwd_basis

    up = Vector((0.0, 0.0, 1.0))
    _, right = _fwd_basis(fwd)
    return fwd.normalized(), right.normalized(), up


def _stitch_coat_sleeve_shoulder(
    bm: bmesh.types.BMesh,
    body: bpy.types.Object,
    *,
    side: str,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> None:
    """Bridge torso shell ↔ sleeve tube so coat reads as one piece in preview."""
    lo, span = lm.zmin, lm.span
    sh_h = lm.h_neck - 0.11
    thw = torso_half_width(ws, lo, span, sh_h, lm.cx, lm.cy)
    sign = -1.0 if side == "Left" else 1.0
    _, right, _ = _coat_body_basis(fwd)
    sh_z = lo + span * sh_h
    torso_anchor = hc + Vector((0.0, 0.0, sh_z - hc.z))
    sleeve_anchor = hc + right * (sign * thw * 1.08) + Vector((0.0, 0.0, sh_z - hc.z))
    torso = sorted(
        bm.verts,
        key=lambda v: (body.matrix_world @ v.co - torso_anchor).length,
    )[:6]
    sleeve = sorted(
        bm.verts,
        key=lambda v: (body.matrix_world @ v.co - sleeve_anchor).length,
    )[:6]
    torso = [v for v in torso if abs((body.matrix_world @ v.co).z - sh_z) < span * 0.04]
    sleeve = [v for v in sleeve if abs((body.matrix_world @ v.co).z - sh_z) < span * 0.05]
    if len(torso) < 2 or len(sleeve) < 2:
        return
    a0, a1 = torso[-2], torso[-1]
    b0, b1 = sleeve[0], sleeve[1]
    try:
        bm.faces.new((a0, a1, b1, b0))
    except ValueError:
        pass


def _prune_bmesh_islands(bm: bmesh.types.BMesh, *, min_faces: int) -> None:
    bm.faces.ensure_lookup_table()
    visited: set[int] = set()
    islands: list[list[bmesh.types.BMFace]] = []
    for seed in bm.faces:
        if seed.index in visited:
            continue
        stack = [seed]
        island: list[bmesh.types.BMFace] = []
        visited.add(seed.index)
        while stack:
            face = stack.pop()
            island.append(face)
            for e in face.edges:
                for nb in e.link_faces:
                    if nb.index not in visited:
                        visited.add(nb.index)
                        stack.append(nb)
        islands.append(island)
    doomed = [f for isl in islands if len(isl) < min_faces for f in isl]
    if doomed:
        bmesh.ops.delete(bm, geom=doomed, context="FACES")


def _build_procedural_coat_body(
    bm: bmesh.types.BMesh,
    body: bpy.types.Object,
    *,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> None:
    """Closed tapered shell shoulder → mid-thigh (no kit-face extraction)."""
    fwd_u, right, _up = _coat_body_basis(fwd)
    lo, span = lm.zmin, lm.span
    inv = body.matrix_world.inverted()
    hem_h = lm.h_waist - 0.20
    top_h = lm.h_neck + 0.02
    n_ring = 15
    n_seg = 36
    grid: list[list[bmesh.types.BMVert]] = []
    thw_top = torso_half_width(ws, lo, span, top_h, lm.cx, lm.cy)
    for ri in range(n_ring):
        t = ri / max(1, n_ring - 1)
        h_norm = hem_h + (top_h - hem_h) * t
        z = lo + span * h_norm
        thw = torso_half_width(ws, lo, span, h_norm, lm.cx, lm.cy)
        r_hem = thw * 1.24
        r_sh = thw_top * 1.14
        r = r_hem + (r_sh - r_hem) * t
        if h_norm >= lm.h_neck - 0.16:
            r = max(r, thw_top * 1.12)
        if t < 0.22:
            r *= 1.0 + 0.08 * (0.22 - t) / 0.22
        row: list[bmesh.types.BMVert] = []
        chest = Vector((hc.x, hc.y, z))
        for si in range(n_seg):
            ang = 2.0 * math.pi * si / n_seg
            seam_pinch = 0.94 if abs(math.cos(ang - math.pi * 0.5)) > 0.92 else 1.0
            rad = r * seam_pinch
            off = right * (math.cos(ang) * rad) + fwd_u * (math.sin(ang) * rad * 0.94)
            wco = chest + off
            radial = Vector((wco.x - chest.x, wco.y - chest.y, 0.0))
            if radial.length > 1e-5:
                wco += radial.normalized() * (span * 0.028)
            row.append(bm.verts.new(inv @ wco))
        grid.append(row)
    for ri in range(n_ring - 1):
        for si in range(n_seg):
            si1 = (si + 1) % n_seg
            try:
                bm.faces.new((grid[ri][si], grid[ri][si1], grid[ri + 1][si1], grid[ri + 1][si]))
            except ValueError:
                pass


def _build_procedural_sleeve_tube(
    bm: bmesh.types.BMesh,
    body: bpy.types.Object,
    *,
    side: str,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> None:
    """Separate sleeve tube on upper_arm + forearm chain."""
    fwd_u, right, up = _coat_body_basis(fwd)
    lo, span = lm.zmin, lm.span
    inv = body.matrix_world.inverted()
    sign = -1.0 if side == "Left" else 1.0
    sh_h = lm.h_neck - 0.10
    wr_h = lm.h_waist - 0.18
    thw_sh = torso_half_width(ws, lo, span, sh_h, lm.cx, lm.cy)
    sh_z = lo + span * sh_h
    wr_z = lo + span * wr_h
    sh = hc + right * (sign * thw_sh * 0.98) + Vector((0.0, 0.0, sh_z - hc.z))
    wr = hc + right * (sign * (thw_sh * 1.06)) + fwd_u * (span * 0.04) + Vector((0.0, 0.0, wr_z - hc.z))
    axis = (wr - sh).normalized()
    perp = axis.cross(up)
    if perp.length < 1e-4:
        perp = right
    perp.normalize()
    perp2 = axis.cross(perp).normalized()
    radius = max(span * 0.052, thw_sh * 0.17)
    n_len = 9
    n_seg = 14
    sleeve: list[list[bmesh.types.BMVert]] = []
    for li in range(n_len):
        t = li / max(1, n_len - 1)
        center = sh + (wr - sh) * t
        row: list[bmesh.types.BMVert] = []
        for si in range(n_seg):
            ang = 2.0 * math.pi * si / n_seg
            off = perp * (math.cos(ang) * radius) + perp2 * (math.sin(ang) * radius)
            row.append(bm.verts.new(inv @ (center + off)))
        sleeve.append(row)
    for li in range(n_len - 1):
        for si in range(n_seg):
            si1 = (si + 1) % n_seg
            try:
                bm.faces.new((sleeve[li][si], sleeve[li][si1], sleeve[li + 1][si1], sleeve[li + 1][si]))
            except ValueError:
                pass


def _bmesh_faces_from_indices(
    bm: bmesh.types.BMesh,
    bm_src: bmesh.types.BMesh,
    indices: set[int],
) -> dict[int, bmesh.types.BMVert]:
    """Duplicate body faces; map body vert index → new bmesh vert."""
    vmap: dict[int, bmesh.types.BMVert] = {}
    for fi in indices:
        sf = bm_src.faces[fi]
        new_vs = []
        for sv in sf.verts:
            si = sv.index
            if si not in vmap:
                vmap[si] = bm.verts.new(sv.co.copy())
            new_vs.append(vmap[si])
        try:
            bm.faces.new(new_vs)
        except ValueError:
            pass
    return vmap


def _collect_arm_sleeve_face_indices(
    body: bpy.types.Object,
    bm_src: bmesh.types.BMesh,
    *,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> set[int]:
    lo, span = lm.zmin, lm.span
    pick: set[int] = set()
    for f in bm_src.faces:
        if _arm_coat_face(f, body, lm=lm, hc=hc, fwd=fwd, ws=ws):
            pick.add(f.index)
        elif _sleeve_source_face(
            f, body, lm=lm, lo=lo, span=span, hc=hc, fwd=fwd, ws=ws
        ):
            pick.add(f.index)
    changed = True
    while changed:
        changed = False
        for f in bm_src.faces:
            if f.index in pick:
                continue
            if not any(nb.index in pick for e in f.edges for nb in e.link_faces):
                continue
            if _sleeve_grow_face(
                f, body, lm=lm, lo=lo, span=span, hc=hc, fwd=fwd, ws=ws
            ):
                pick.add(f.index)
                changed = True
    return pick


def _copy_body_vertex_groups(
    body: bpy.types.Object,
    coat: bpy.types.Object,
    src_for_coat_vert: list[int | None],
) -> None:
    """One source body vert index per coat vert (pre-solidify correspondence)."""
    coat.vertex_groups.clear()
    vg_dst: dict[str, bpy.types.VertexGroup] = {}
    for vg in body.vertex_groups:
        vg_dst[vg.name] = coat.vertex_groups.new(name=vg.name)
    for ci, si in enumerate(src_for_coat_vert):
        if si is None or si < 0 or si >= len(body.data.vertices):
            continue
        for g in body.data.vertices[si].groups:
            if g.group >= len(body.vertex_groups):
                continue
            name = body.vertex_groups[g.group].name
            vg_dst[name].add([ci], g.weight, "REPLACE")


_VG_BODY_COAT_INFLATE = frozenset(
    {
        "Hips",
        "Spine",
        "Chest",
        "UpperChest",
        "LeftShoulder",
        "RightShoulder",
        "LeftArm",
        "RightArm",
        "LeftForeArm",
        "RightForeArm",
        "LeftUpLeg",
        "RightUpLeg",
    }
)


def remove_legacy_lab_coat_objects() -> None:
    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH" and ob.name.split(".")[0] == "ResearcherLabCoat":
            bpy.data.objects.remove(ob, do_unlink=True)


def _world_centroid_face(face: bmesh.types.BMFace, ob: bpy.types.Object) -> Vector:
    wc = Vector()
    for v in face.verts:
        wc += ob.matrix_world @ v.co
    return wc / len(face.verts)


def apply_researcher_body_lab_coat_displace(body: bpy.types.Object) -> None:
    """Single skinned mesh: puff coat-slot verts along normals, same armature."""
    from kenney_researcher_stock_recolor import SLOT_LAB_COAT

    coat_vert: set[int] = set()
    for poly in body.data.polygons:
        if poly.material_index != SLOT_LAB_COAT:
            continue
        for vi in poly.vertices:
            coat_vert.add(vi)

    if len(coat_vert) < 80:
        raise RuntimeError(f"FAIL staff-researcher-idle body_coat_verts={len(coat_vert)} too_few")

    bm = bmesh.new()
    bm.from_mesh(body.data)
    bm.faces.ensure_lookup_table()
    bm.verts.ensure_lookup_table()

    push = max(body.dimensions.length * 0.012, 0.008)
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    _VG_NO_DISPLACE = frozenset({"Head", "Neck"})
    for vi in coat_vert:
        if _vg_weight(body, vi, _VG_NO_DISPLACE) > 0.18:
            continue
        v = bm.verts[vi]
        if v.normal.length > 1e-6:
            arm = _vg_weight(body, vi, _VG_SLEEVE)
            scale = 0.26 if arm > 0.32 else 0.56
            v.co += v.normal * push * scale

    bm.to_mesh(body.data)
    body.data.update()
    bm.free()
    body["researcher_lab_coat_on_body"] = True
    bc.smooth_shade(body)


def spawn_researcher_lab_coat(body: bpy.types.Object) -> bpy.types.Object | None:
    """Deprecated separate shell — coat is on the body mesh."""
    remove_legacy_lab_coat_objects()
    apply_researcher_body_lab_coat_displace(body)
    return None


def _spawn_researcher_lab_coat_legacy_shell(body: bpy.types.Object) -> bpy.types.Object:
    """Lab coat = duplicated torso + arm body faces, solidify, copied armature weights."""
    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH" and ob.name.split(".")[0] == "ResearcherLabCoat":
            bpy.data.objects.remove(ob, do_unlink=True)
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, y_face_fn = face_shell_coord(body, fwd)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lo, span = lm.zmin, lm.span
    coat_hem = lm.h_waist - 0.34
    coat_top = lm.h_neck + 0.04

    bm_src = bmesh.new()
    bm_src.from_mesh(body.data)
    bm_src.faces.ensure_lookup_table()

    torso_pick = _duplicate_coat_shell_faces(
        body,
        bm_src,
        lm=lm,
        lo=lo,
        span=span,
        coat_hem=coat_hem,
        coat_top=coat_top,
        hc=hc,
        fwd=fwd,
        ws=ws,
        y_face_fn=y_face_fn,
    )
    arm_pick = _collect_arm_sleeve_face_indices(
        body, bm_src, lm=lm, hc=hc, fwd=fwd, ws=ws
    )
    all_pick = torso_pick | arm_pick
    if len(all_pick) < 48:
        bm_src.free()
        raise RuntimeError(f"FAIL staff-researcher-idle lab_coat_faces={len(all_pick)} too_few")

    bm = bmesh.new()
    vmap = _bmesh_faces_from_indices(bm, bm_src, all_pick)
    bm_src.free()
    bm.verts.ensure_lookup_table()
    bm.faces.ensure_lookup_table()
    bmesh.ops.remove_doubles(bm, verts=bm.verts, dist=0.0004)
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    push = max(body.dimensions.length * 0.011, 0.008)
    for v in bm.verts:
        if v.normal.length > 1e-6:
            v.co += v.normal * push

    mesh = bpy.data.meshes.new("ResearcherLabCoatMesh")
    bm.to_mesh(mesh)
    bm.free()
    mesh.update()

    coat = bpy.data.objects.new("ResearcherLabCoat", mesh)
    bpy.context.collection.objects.link(coat)
    mat = _ensure_material("ResearcherLabCoatGeo", LAB_COAT_RGB, 0.42, specular=0.08)
    coat.data.materials.append(mat)
    world = body.matrix_world.copy()
    coat.matrix_world = world

    bpy.context.view_layer.objects.active = coat
    coat.select_set(True)
    bpy.ops.object.shade_smooth()
    thick = max(body.dimensions.length * 0.012, 0.006)
    solid = coat.modifiers.new("CoatSolidify", "SOLIDIFY")
    solid.thickness = thick
    solid.offset = 1.0
    solid.use_rim = True
    bpy.ops.object.modifier_apply(modifier=solid.name)
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.remove_doubles(threshold=0.00055)
    bpy.ops.mesh.normals_make_consistent(inside=False)
    bpy.ops.object.mode_set(mode="OBJECT")

    remap: list[int] = []
    ref_cos = {si: body.data.vertices[si].co.copy() for si in vmap}
    inv_coat = coat.matrix_world.inverted()
    for v in coat.data.vertices:
        best_i = min(ref_cos, key=lambda si: (inv_coat @ v.co - ref_cos[si]).length_squared)
        remap.append(best_i)
    _copy_body_vertex_groups(body, coat, remap)
    coat["coat_weights_from_body"] = True

    _trim_coat_above_chin(coat, body, lm, hc, fwd, ws, y_face_fn)
    _bisect_coat_below_hem(coat, lm, coat_hem)
    bm_hem = bmesh.new()
    bm_hem.from_mesh(coat.data)
    _append_coat_hem_quads(bm_hem, body, lm=lm, hc=hc, fwd=fwd, ws=ws)
    bm_hem.to_mesh(coat.data)
    coat.data.update()
    bm_hem.free()
    _trim_coat_hands_crotch(coat, body, lm, hc, fwd, ws, y_face_fn)
    _trim_coat_orphan_shards(coat, body, lm, hc, fwd, ws, y_face_fn)
    _prune_coat_micro_islands(coat, min_faces=8)
    _prune_coat_wrist_shards(coat, lm, hc, fwd, ws)

    if len(coat.data.polygons) < 40:
        raise RuntimeError(
            f"FAIL staff-researcher-idle lab_coat_faces_post={len(coat.data.polygons)}"
        )
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
        radius=head_w * 0.50,
        location=local,
    )
    cap = bpy.context.active_object
    cap.name = "ResearcherHairCap"
    cap.scale = (1.05, 0.96, 0.56)
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
    outer = eye_diameter * 1.3
    bar = max(full_sep * 0.090, head_w * 0.0088)
    half = outer * 0.5

    def _open_square_rim(center_w: Vector, lateral_sign: float, name: str) -> None:
        side_len = outer * 0.88
        temple = center_w + right * (lateral_sign * (half - bar * 0.48))
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=temple)
        side = bpy.context.active_object
        side.name = f"{name}.side"
        side.scale = (bar * 1.06, bar * 1.12, side_len)
        side.rotation_euler = rot
        bpy.ops.object.transform_apply(scale=True, rotation=True)
        side.data.materials.append(mat)
        parts.append(side)
        brow = center_w + up * (half - bar * 0.50)
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=brow)
        top = bpy.context.active_object
        top.name = f"{name}.top"
        top.scale = (outer * 0.92, bar * 1.10, bar * 1.06)
        top.rotation_euler = rot
        bpy.ops.object.transform_apply(scale=True, rotation=True)
        top.data.materials.append(mat)
        parts.append(top)

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
    _open_square_rim(anchors[0], -1.0, "ResearcherGlasses.L")
    _open_square_rim(anchors[1], 1.0, "ResearcherGlasses.R")
    mid = (anchors[0] + anchors[1]) * 0.5 - up * (head_w * 0.06)
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=mid)
    bridge = bpy.context.active_object
    bridge.name = "ResearcherGlasses.bridge"
    bridge.scale = (full_sep * 0.20, bar * 1.08, bar * 1.18)
    bridge.rotation_euler = rot
    bpy.ops.object.transform_apply(scale=True, rotation=True)
    bridge.data.materials.append(mat)
    parts.append(bridge)

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