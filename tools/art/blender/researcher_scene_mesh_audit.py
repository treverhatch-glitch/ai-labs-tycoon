"""FAIL #27: prune/verify floating mesh islands vs hero body AABB."""
from __future__ import annotations

import bmesh
import bpy
from mathutils import Vector

RENDER_ISLAND_BODY_BBOX_GAP_M = 0.05
WRIST_CUFF_WRIST_GAP_M = 0.04


def _world_aabb(ob: bpy.types.Object) -> tuple[Vector, Vector]:
    mn = Vector((1e9, 1e9, 1e9))
    mx = Vector((-1e9, -1e9, -1e9))
    if not ob.data.vertices:
        return mn, mx
    for v in ob.data.vertices:
        w = ob.matrix_world @ v.co
        mn.x = min(mn.x, w.x)
        mn.y = min(mn.y, w.y)
        mn.z = min(mn.z, w.z)
        mx.x = max(mx.x, w.x)
        mx.y = max(mx.y, w.y)
        mx.z = max(mx.z, w.z)
    return mn, mx


def _aabb_separation_gap(a_mn: Vector, a_mx: Vector, b_mn: Vector, b_mx: Vector) -> float:
    """Shortest distance between two axis-aligned boxes (0 when overlapping)."""
    gap = 0.0
    for i in range(3):
        if a_mx[i] < b_mn[i]:
            gap = max(gap, b_mn[i] - a_mx[i])
        elif b_mx[i] < a_mn[i]:
            gap = max(gap, a_mn[i] - b_mx[i])
    return gap


def _mesh_island_sets(
    ob: bpy.types.Object,
) -> list[tuple[set[int], set[int]]]:
    """Each item is (face_indices, vert_indices)."""
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    visited: set[int] = set()
    islands: list[tuple[set[int], set[int]]] = []
    for seed in bm.faces:
        if seed.index in visited:
            continue
        stack = [seed]
        vert_ids: set[int] = set()
        face_ids: set[int] = set()
        while stack:
            face = stack.pop()
            if face.index in visited:
                continue
            visited.add(face.index)
            face_ids.add(face.index)
            for v in face.verts:
                vert_ids.add(v.index)
            for e in face.edges:
                for nb in e.link_faces:
                    if nb.index not in visited:
                        stack.append(nb)
        islands.append((face_ids, vert_ids))
    bm.free()
    return islands


def _vertset_world_aabb(ob: bpy.types.Object, vert_ids: set[int]) -> tuple[Vector, Vector]:
    mn = Vector((1e9, 1e9, 1e9))
    mx = Vector((-1e9, -1e9, -1e9))
    mw = ob.matrix_world
    me = ob.data
    for vi in vert_ids:
        w = mw @ me.vertices[vi].co
        mn.x = min(mn.x, w.x)
        mn.y = min(mn.y, w.y)
        mn.z = min(mn.z, w.z)
        mx.x = max(mx.x, w.x)
        mx.y = max(mx.y, w.y)
        mx.z = max(mx.z, w.z)
    return mn, mx


def prune_scene_floating_islands(
    body: bpy.types.Object,
    meshes: list[bpy.types.Object],
    *,
    max_gap_m: float = RENDER_ISLAND_BODY_BBOX_GAP_M,
) -> int:
    """Drop mesh islands whose AABB sits > max_gap_m from the body AABB."""
    body_mn, body_mx = _world_aabb(body)
    removed_faces = 0
    for ob in meshes:
        if ob.type != "MESH" or len(ob.data.vertices) < 1:
            continue
        islands = _mesh_island_sets(ob)
        if len(islands) <= 1:
            continue
        doomed_face_ids: set[int] = set()
        sized = [(faces, verts, len(verts)) for faces, verts in islands]
        sized.sort(key=lambda row: row[2], reverse=True)
        for idx, (face_ids, vert_ids, _n) in enumerate(sized):
            mn, mx = _vertset_world_aabb(ob, vert_ids)
            gap = _aabb_separation_gap(mn, mx, body_mn, body_mx)
            if idx == 0 and gap <= max_gap_m * 3.0:
                continue
            if gap > max_gap_m:
                doomed_face_ids |= face_ids
        if not doomed_face_ids:
            continue
        bm = bmesh.new()
        bm.from_mesh(ob.data)
        bm.faces.ensure_lookup_table()
        by_idx = {f.index: f for f in bm.faces}
        kill_faces = [by_idx[i] for i in doomed_face_ids if i in by_idx]
        if not kill_faces:
            continue
        bmesh.ops.delete(bm, geom=kill_faces, context="FACES")
        loose = [v for v in bm.verts if not v.link_faces]
        if loose:
            bmesh.ops.delete(bm, geom=loose, context="VERTS")
        bm.to_mesh(ob.data)
        ob.data.update()
        bm.free()
        removed_faces += len(kill_faces)
    return removed_faces


def verify_render_mesh_islands(
    body: bpy.types.Object,
    meshes: list[bpy.types.Object],
    *,
    max_gap_m: float = RENDER_ISLAND_BODY_BBOX_GAP_M,
) -> tuple[bool, str]:
    body_mn, body_mx = _world_aabb(body)
    worst = 0.0
    culprit = ""
    for ob in meshes:
        if ob.type != "MESH":
            continue
        for _faces, vert_ids in _mesh_island_sets(ob):
            mn, mx = _vertset_world_aabb(ob, vert_ids)
            gap = _aabb_separation_gap(mn, mx, body_mn, body_mx)
            if gap > worst:
                worst = gap
                culprit = f"{ob.name.split('.')[0]} verts={len(vert_ids)} gap={gap:.4f}"
    if worst > max_gap_m + 1e-4:
        return False, f"{culprit} max_gap={worst:.4f} want<={max_gap_m}"
    return True, f"render_islands max_gap={worst:.4f}"


def _wrist_head_world(arm: bpy.types.Object | None, side: str) -> Vector | None:
    if arm is None:
        return None
    pb = arm.pose.bones.get(f"{side}Hand") or arm.pose.bones.get(f"{side}ForeArm")
    if pb is None:
        return None
    return (arm.matrix_world @ pb.matrix).translation


def _estimate_wrist_from_body(body: bpy.types.Object, lm, side: str) -> Vector:
    lo, span = lm.zmin, lm.span
    z0 = lo + span * (lm.h_shoe + 0.18)
    z1 = lo + span * (lm.h_shoe + 0.26)
    sign = -1.0 if side == "Left" else 1.0
    pts: list[Vector] = []
    for v in body.data.vertices:
        w = body.matrix_world @ v.co
        if w.z < z0 or w.z > z1:
            continue
        if sign * (w.x - lm.cx) < span * 0.10:
            continue
        pts.append(w)
    if pts:
        return sum(pts, Vector()) / len(pts)
    z = lo + span * (lm.h_shoe + 0.22)
    return Vector((lm.cx + sign * span * 0.20, lm.cy, z))


def _cuff_band_samples(
    sleeves: bpy.types.Object, lm, side: str
) -> list[Vector]:
    from researcher_shirt_derived_meshes import SLEEVE_ALLOWED_VG

    lo, span = lm.zmin, lm.span
    wrist_h = lm.h_shoe + 0.22
    out: list[Vector] = []
    for v in sleeves.data.vertices:
        wc = sleeves.matrix_world @ v.co
        h = (wc.z - lo) / span
        if h < wrist_h - 0.06 or h > wrist_h + 0.05:
            continue
        if side == "Left":
            if wc.x > lm.cx - span * 0.08:
                continue
        elif wc.x < lm.cx + span * 0.08:
            continue
        arm_cov = 0.0
        for g in v.groups:
            if g.group >= len(sleeves.vertex_groups):
                continue
            if sleeves.vertex_groups[g.group].name in SLEEVE_ALLOWED_VG:
                arm_cov += g.weight
        if arm_cov < 0.25:
            continue
        out.append(wc)
    return out


def verify_sleeve_cuff_wrist_gap(
    sleeves: bpy.types.Object,
    body: bpy.types.Object,
    arm: bpy.types.Object | None,
    lm,
) -> tuple[bool, str, float]:
    worst = 0.0
    from researcher_shirt_derived_meshes import _body_bvh_world

    tree = _body_bvh_world(body)
    for side in ("Left", "Right"):
        cuff_dists: list[float] = []
        for wc in _cuff_band_samples(sleeves, lm, side):
            _loc, _n, _idx, shell = tree.find_nearest(wc)
            cuff_dists.append(shell if shell is not None else 1.0)
        if len(cuff_dists) < 4:
            return False, f"cuff_sample_{side} n={len(cuff_dists)} want>=4", worst
        side_worst = max(cuff_dists)
        worst = max(worst, side_worst)
        if side_worst > WRIST_CUFF_WRIST_GAP_M + 1e-4:
            return (
                False,
                f"cuff_{side} gap={side_worst:.4f} max={WRIST_CUFF_WRIST_GAP_M}",
                worst,
            )
    bone_note = ""
    if arm is not None and all(_wrist_head_world(arm, s) is not None for s in ("Left", "Right")):
        deps = bpy.context.evaluated_depsgraph_get()
        deps.update()
        ev = sleeves.evaluated_get(deps)
        bone_worst = 0.0
        from researcher_shirt_derived_meshes import SLEEVE_ALLOWED_VG

        for side in ("Left", "Right"):
            wrist = _wrist_head_world(arm, side)
            assert wrist is not None
            ranked: list[float] = []
            for v in ev.data.vertices:
                wc = ev.matrix_world @ v.co
                if side == "Left" and wc.x > lm.cx - lm.span * 0.04:
                    continue
                if side == "Right" and wc.x < lm.cx + lm.span * 0.04:
                    continue
                arm_cov = sum(
                    g.weight
                    for g in sleeves.data.vertices[v.index].groups
                    if g.group < len(sleeves.vertex_groups)
                    and sleeves.vertex_groups[g.group].name in SLEEVE_ALLOWED_VG
                )
                if arm_cov < 0.20:
                    continue
                ranked.append((wc - wrist).length)
            ranked.sort()
            if ranked:
                bone_worst = max(bone_worst, ranked[0])
        if bone_worst > 0.0:
            bone_note = f" bone_cuff_nearest={bone_worst:.4f}"
    return True, f"cuff_wrist_gap={worst:.4f}{bone_note}", worst


def audit_researcher_render_meshes(
    body: bpy.types.Object,
    meshes: list[bpy.types.Object],
    *,
    prune: bool = True,
) -> tuple[bool, str]:
    if prune:
        prune_scene_floating_islands(body, meshes)
    ok, msg = verify_render_mesh_islands(body, meshes)
    if not ok:
        return False, msg
    return True, msg