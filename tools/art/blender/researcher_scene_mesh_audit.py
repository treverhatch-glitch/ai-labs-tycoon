"""FAIL #27–28: evaluated render island audit, prune, cuff wrist bone gate."""
from __future__ import annotations

from dataclasses import dataclass
from pathlib import Path

import bmesh
import bpy
from mathutils import Vector

import blender_common as bc
from kenney_geometry_landmarks import compute_landmarks

RENDER_ISLAND_BODY_BBOX_GAP_M = 0.05
WRIST_CUFF_WRIST_GAP_M = 0.04
RENDER_ISLAND_AUDIT_LOG = (
    bc.PREVIEW_ROOT / "staff-researcher-render-island-audit.log"
)
RENDER_SCENE_HYGIENE_LOG = (
    bc.PREVIEW_ROOT / "staff-researcher-render-scene-hygiene.log"
)
RESEARCHER_RENDER_MESH_ALLOW = frozenset(
    {
        "KenneyResearcherStock",
        "ResearcherCoatSkirt",
        "ResearcherCoatSleeves",
        "ResearcherHairCap",
        "FounderEye_L",
        "FounderEye_R",
        "PreviewShadowFloor",
        "PreviewCamera",
    }
)


@dataclass
class IslandAuditRow:
    frame: int
    panel: str
    mesh: str
    island: int
    vert_count: int
    bbox_min: tuple[float, float, float]
    bbox_max: tuple[float, float, float]
    body_gap_m: float
    above_head_m: float
    fail: bool


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


def _expanded_bbox(
    mn: Vector, mx: Vector, pad: float = RENDER_ISLAND_BODY_BBOX_GAP_M
) -> tuple[Vector, Vector]:
    return mn - Vector((pad, pad, pad)), mx + Vector((pad, pad, pad))


def head_top_world(body: bpy.types.Object, *, margin_m: float = 0.002) -> float:
    _mn, mx = _world_aabb(body)
    return mx.z + margin_m


def _aabb_separation_gap(a_mn: Vector, a_mx: Vector, b_mn: Vector, b_mx: Vector) -> float:
    gap = 0.0
    for i in range(3):
        if a_mx[i] < b_mn[i]:
            gap = max(gap, b_mn[i] - a_mx[i])
        elif b_mx[i] < a_mn[i]:
            gap = max(gap, a_mn[i] - b_mx[i])
    return gap


def _mesh_island_sets(ob: bpy.types.Object) -> list[tuple[set[int], set[int]]]:
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


def collect_evaluated_island_rows(
    body: bpy.types.Object,
    meshes: list[bpy.types.Object],
    *,
    frame: int,
    panel: str,
    pad_m: float = RENDER_ISLAND_BODY_BBOX_GAP_M,
) -> list[IslandAuditRow]:
    deps = bpy.context.evaluated_depsgraph_get()
    deps.update()
    body_ev = body.evaluated_get(deps)
    ev_mn, ev_mx = _world_aabb(body_ev)
    rest_mn, rest_mx = _world_aabb(body)
    body_mn = Vector(
        (min(ev_mn.x, rest_mn.x), min(ev_mn.y, rest_mn.y), min(ev_mn.z, rest_mn.z))
    )
    body_mx = Vector(
        (max(ev_mx.x, rest_mx.x), max(ev_mx.y, rest_mx.y), max(ev_mx.z, rest_mx.z))
    )
    exp_mn, exp_mx = _expanded_bbox(body_mn, body_mx, pad_m)
    head_z = max(head_top_world(body_ev), head_top_world(body))
    rows: list[IslandAuditRow] = []
    for ob in meshes:
        if ob.type != "MESH":
            continue
        base = ob.name.split(".")[0]
        if base == "ResearcherCoatSkirt" and not any(
            m.type == "ARMATURE" for m in ob.modifiers
        ):
            ev = ob
        else:
            ev = ob.evaluated_get(deps)
        for isl_i, (_faces, vert_ids) in enumerate(_mesh_island_sets(ev)):
            if not vert_ids:
                continue
            mn, mx = _vertset_world_aabb(ev, vert_ids)
            gap = _aabb_separation_gap(mn, mx, exp_mn, exp_mx)
            above = max(0.0, mn.z - head_z)
            fail = gap > pad_m + 1e-4 or above > 1e-4
            rows.append(
                IslandAuditRow(
                    frame=frame,
                    panel=panel,
                    mesh=base,
                    island=isl_i,
                    vert_count=len(vert_ids),
                    bbox_min=(mn.x, mn.y, mn.z),
                    bbox_max=(mx.x, mx.y, mx.z),
                    body_gap_m=gap,
                    above_head_m=above,
                    fail=fail,
                )
            )
    return rows


def append_island_audit_log(
    log_path: Path,
    rows: list[IslandAuditRow],
    *,
    header: str = "",
) -> None:
    log_path.parent.mkdir(parents=True, exist_ok=True)
    with log_path.open("a", encoding="utf-8") as fh:
        if header:
            fh.write(f"# {header}\n")
        for row in rows:
            flag = "FAIL" if row.fail else "ok"
            fh.write(
                f"{flag} frame={row.frame} panel={row.panel} "
                f"mesh={row.mesh} island={row.island} verts={row.vert_count} "
                f"gap={row.body_gap_m:.4f} above_head={row.above_head_m:.4f} "
                f"bbox_min=({row.bbox_min[0]:.3f},{row.bbox_min[1]:.3f},{row.bbox_min[2]:.3f}) "
                f"bbox_max=({row.bbox_max[0]:.3f},{row.bbox_max[1]:.3f},{row.bbox_max[2]:.3f})\n"
            )


def island_audit_failures(rows: list[IslandAuditRow]) -> list[IslandAuditRow]:
    return [r for r in rows if r.fail]


def _visible_scene_meshes() -> list[bpy.types.Object]:
    out: list[bpy.types.Object] = []
    for ob in bpy.context.scene.objects:
        if ob.type != "MESH" or ob.hide_render:
            continue
        if not ob.data or len(ob.data.vertices) < 1:
            continue
        out.append(ob)
    return out


def prepare_researcher_render_scene(
    hero_meshes: list[bpy.types.Object],
    *,
    hygiene_log: Path | None = None,
) -> list[str]:
    """Hide/remove non-character meshes before beauty render (mask pass already does)."""
    hero_ids = {id(o) for o in hero_meshes}
    hero_bases = {o.name.split(".")[0] for o in hero_meshes}
    actions: list[str] = []
    for ob in list(bpy.context.scene.objects):
        if ob.type == "ARMATURE":
            ob.hide_render = True
            actions.append(f"hide_render armature {ob.name}")
            continue
        if ob.type == "MESH":
            base = ob.name.split(".")[0]
            if base == "PreviewShadowFloor":
                continue
            if id(ob) in hero_ids:
                ob.hide_render = False
                continue
            if base.endswith("_WalkBake"):
                bpy.data.objects.remove(ob, do_unlink=True)
                actions.append(f"removed WalkBake {ob.name}")
                continue
            if base not in RESEARCHER_RENDER_MESH_ALLOW and base not in hero_bases:
                me = ob.data
                bpy.data.objects.remove(ob, do_unlink=True)
                if me.users == 0:
                    bpy.data.meshes.remove(me)
                actions.append(f"removed stray mesh {ob.name}")
                continue
            ob.hide_render = True
            actions.append(f"hide_render mesh {ob.name}")
            continue
        if ob.type in {"EMPTY", "CURVE", "SURFACE", "FONT", "META"}:
            ob.hide_render = True
            actions.append(f"hide_render {ob.type} {ob.name}")
    path = hygiene_log or RENDER_SCENE_HYGIENE_LOG
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("a", encoding="utf-8") as fh:
        fh.write("# render scene hygiene\n")
        for line in actions:
            fh.write(f"{line}\n")
    return actions


def assert_render_island_audit(
    body: bpy.types.Object,
    meshes: list[bpy.types.Object],
    *,
    frame: int,
    panel: str,
    log_path: Path | None = None,
) -> tuple[bool, str]:
    prepare_researcher_render_scene(meshes)
    audit_targets = [
        o
        for o in _visible_scene_meshes()
        if o.name.split(".")[0] != "PreviewShadowFloor"
    ]
    if not audit_targets:
        audit_targets = [o for o in meshes if o.type == "MESH"]
    rows = collect_evaluated_island_rows(
        body, audit_targets, frame=frame, panel=panel
    )
    bad = island_audit_failures(rows)
    path = log_path or RENDER_ISLAND_AUDIT_LOG
    append_island_audit_log(path, rows, header=f"frame={frame} panel={panel}")
    if bad:
        row = bad[0]
        return (
            False,
            f"render_island mesh={row.mesh} island={row.island} "
            f"gap={row.body_gap_m:.4f} above_head={row.above_head_m:.4f} "
            f"frame={frame} panel={panel}",
        )
    worst = max((r.body_gap_m for r in rows), default=0.0)
    return True, f"render_islands max_gap={worst:.4f} frame={frame} panel={panel}"


def _delete_island_faces(ob: bpy.types.Object, face_ids: set[int]) -> None:
    if not face_ids:
        return
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    by_idx = {f.index: f for f in bm.faces}
    kill = [by_idx[i] for i in face_ids if i in by_idx]
    if not kill:
        bm.free()
        return
    bmesh.ops.delete(bm, geom=kill, context="FACES")
    loose = [v for v in bm.verts if not v.link_faces]
    if loose:
        bmesh.ops.delete(bm, geom=loose, context="VERTS")
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()


def prune_islands_above_head(
    meshes: list[bpy.types.Object], head_z: float
) -> int:
    from researcher_shirt_derived_meshes import SLEEVES_NAME, _prune_verts_above_z

    removed = 0
    for ob in meshes:
        if ob.type != "MESH":
            continue
        if ob.name.split(".")[0] in ("FounderEye_L", "FounderEye_R", "ResearcherHairCap"):
            continue
        if ob.name.split(".")[0] == SLEEVES_NAME:
            removed += _prune_verts_above_z(ob, head_z - 0.02, margin_m=0.004)
            continue
        doomed: set[int] = set()
        for face_ids, vert_ids in _mesh_island_sets(ob):
            mn, _mx = _vertset_world_aabb(ob, vert_ids)
            if mn.z > head_z - 0.008:
                doomed |= face_ids
        if doomed:
            _delete_island_faces(ob, doomed)
            removed += len(doomed)
    return removed


def prune_scene_floating_islands(
    body: bpy.types.Object,
    meshes: list[bpy.types.Object],
    *,
    max_gap_m: float = RENDER_ISLAND_BODY_BBOX_GAP_M,
) -> int:
    body_mn, body_mx = _world_aabb(body)
    removed_faces = 0
    for ob in meshes:
        if ob.type != "MESH" or len(ob.data.vertices) < 1:
            continue
        if ob.name.split(".")[0] in ("FounderEye_L", "FounderEye_R", "ResearcherHairCap"):
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
            if gap > max_gap_m:
                doomed_face_ids |= face_ids
            elif idx > 0 and gap > max_gap_m * 0.55:
                doomed_face_ids |= face_ids
        if not doomed_face_ids:
            continue
        _delete_island_faces(ob, doomed_face_ids)
        removed_faces += len(doomed_face_ids)
    return removed_faces


def verify_render_mesh_islands(
    body: bpy.types.Object,
    meshes: list[bpy.types.Object],
    *,
    max_gap_m: float = RENDER_ISLAND_BODY_BBOX_GAP_M,
) -> tuple[bool, str]:
    body_mn, body_mx = _world_aabb(body)
    exp_mn, exp_mx = _expanded_bbox(body_mn, body_mx, max_gap_m)
    head_z = head_top_world(body)
    worst = 0.0
    culprit = ""
    for ob in meshes:
        if ob.type != "MESH":
            continue
        for _faces, vert_ids in _mesh_island_sets(ob):
            mn, mx = _vertset_world_aabb(ob, vert_ids)
            gap = _aabb_separation_gap(mn, mx, exp_mn, exp_mx)
            above = max(0.0, mn.z - head_z)
            if gap > worst:
                worst = gap
                culprit = f"{ob.name.split('.')[0]} verts={len(vert_ids)} gap={gap:.4f}"
            if above > 0.01:
                return (
                    False,
                    f"{ob.name.split('.')[0]} above_head={above:.4f} want=0",
                )
    if worst > max_gap_m + 1e-4:
        return False, f"{culprit} max_gap={worst:.4f} want<={max_gap_m}"
    return True, f"render_islands max_gap={worst:.4f}"


def purge_coat_head_weights(meshes: list[bpy.types.Object]) -> None:
    from researcher_shirt_derived_meshes import VG_DENY_HEAD

    for ob in meshes:
        if ob.type != "MESH":
            continue
        for vi in range(len(ob.data.vertices)):
            for g in list(ob.data.vertices[vi].groups):
                if g.group >= len(ob.vertex_groups):
                    continue
                if ob.vertex_groups[g.group].name in VG_DENY_HEAD:
                    ob.vertex_groups[g.group].remove([vi])


def prune_researcher_visual_artifacts(
    body: bpy.types.Object,
    meshes: list[bpy.types.Object],
    *,
    arm: bpy.types.Object | None = None,
) -> None:
    from researcher_shirt_derived_meshes import (
        SLEEVES_NAME,
        SKIRT_NAME,
        _neck_cut_world_z,
        _prune_verts_above_z,
    )

    lm = compute_landmarks(body)
    head_z = head_top_world(body)
    z_neck = _neck_cut_world_z(arm, lm)
    for ob in meshes:
        base = ob.name.split(".")[0]
        if base in (SKIRT_NAME, SLEEVES_NAME):
            _prune_verts_above_z(ob, z_neck + 0.008)
    purge_coat_head_weights(meshes)
    prune_islands_above_head(meshes, head_z)
    prune_scene_floating_islands(body, meshes)
    from researcher_shirt_derived_meshes import SKIRT_NAME, _cull_verts_far_from_body

    from researcher_shirt_derived_meshes import (
        SKIRT_NAME,
        _skirt_z_band,
        flatten_skirt_hem_mid_thigh,
    )

    _lo, _span, _top, _hip, z_mid, _knee = _skirt_z_band(body, lm)
    for ob in meshes:
        if ob.type == "MESH" and ob.name.split(".")[0] == SKIRT_NAME:
            flatten_skirt_hem_mid_thigh(ob, z_mid)
            _cull_verts_far_from_body(
                ob, body, max_gap_m=RENDER_ISLAND_BODY_BBOX_GAP_M
            )


def _infer_vert_arm_side(
    sleeves: bpy.types.Object,
    vi: int,
) -> str | None:
    best_side = None
    best_w = 0.0
    for side in ("Left", "Right"):
        side_w = 0.0
        for nm in (f"{side}Hand", f"{side}ForeArm", f"{side}Arm"):
            vg = sleeves.vertex_groups.get(nm)
            if vg is None:
                continue
            for g in sleeves.data.vertices[vi].groups:
                if g.group == vg.index:
                    side_w += g.weight
        if side_w > best_w:
            best_w = side_w
            best_side = side
    return best_side if best_w >= 0.2 else None


def _same_arm_side(
    wc: Vector,
    lm,
    arm: bpy.types.Object | None,
    side: str,
) -> bool:
    """True when world position lies on the same lateral half as that arm's wrist."""
    wrist = _wrist_head_world(arm, side) if arm is not None else None
    if wrist is not None:
        return (wc.x - lm.cx) * (wrist.x - lm.cx) > -lm.span * 0.03
    if side == "Left":
        return wc.x < lm.cx - lm.span * 0.08
    return wc.x > lm.cx + lm.span * 0.08


def _wrist_head_world(arm: bpy.types.Object | None, side: str) -> Vector | None:
    if arm is None:
        return None
    pb = arm.pose.bones.get(f"{side}Hand")
    if pb is None:
        pb = arm.pose.bones.get(f"{side}ForeArm")
    if pb is None:
        return None
    return (arm.matrix_world @ pb.matrix).translation


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
        if h < wrist_h - 0.10 or h > wrist_h + 0.08:
            continue
        if not _same_arm_side(wc, lm, None, side):
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
    *,
    use_evaluated: bool = False,
) -> tuple[bool, str, float]:
    worst = 0.0
    if arm is not None and all(_wrist_head_world(arm, s) is not None for s in ("Left", "Right")):
        from researcher_shirt_derived_meshes import SLEEVE_ALLOWED_VG

        deps = bpy.context.evaluated_depsgraph_get()
        deps.update()
        mesh_ob = sleeves.evaluated_get(deps) if use_evaluated else sleeves
        for side in ("Left", "Right"):
            wrist = _wrist_head_world(arm, side)
            assert wrist is not None
            ranked: list[float] = []
            vert_src = sleeves if mesh_ob is sleeves else sleeves
            for vi in range(len(mesh_ob.data.vertices)):
                wc = mesh_ob.matrix_world @ mesh_ob.data.vertices[vi].co
                side_tag = _infer_vert_arm_side(sleeves, vi)
                if side_tag != side:
                    if side_tag is None and not _same_arm_side(wc, lm, arm, side):
                        continue
                    if side_tag is not None:
                        continue
                src_i = vi if mesh_ob is sleeves else vi
                if src_i >= len(sleeves.data.vertices):
                    continue
                arm_cov = sum(
                    g.weight
                    for g in sleeves.data.vertices[src_i].groups
                    if g.group < len(sleeves.vertex_groups)
                    and sleeves.vertex_groups[g.group].name in SLEEVE_ALLOWED_VG
                )
                fa_w = 0.0
                for g in sleeves.data.vertices[src_i].groups:
                    if g.group >= len(sleeves.vertex_groups):
                        continue
                    nm = sleeves.vertex_groups[g.group].name
                    if nm in (f"{side}ForeArm", f"{side}Arm", f"{side}Hand"):
                        fa_w += g.weight
                if fa_w < 0.05:
                    continue
                ranked.append((wc - wrist).length)
            if len(ranked) < 4:
                wrist = _wrist_head_world(arm, side)
                assert wrist is not None
                for vi in range(len(mesh_ob.data.vertices)):
                    wc = mesh_ob.matrix_world @ mesh_ob.data.vertices[vi].co
                    if not _same_arm_side(wc, lm, arm, side):
                        continue
                    if (wc - wrist).length > lm.span * 0.28:
                        continue
                    ranked.append((wc - wrist).length)
                ranked.sort()
            if len(ranked) < 4:
                return False, f"cuff_sample_{side} n={len(ranked)} want>=4", worst
            ranked.sort()
            cuff_dists = ranked[: min(12, len(ranked))]
            side_worst = max(cuff_dists[:4])
            worst = max(worst, side_worst)
            if side_worst > WRIST_CUFF_WRIST_GAP_M + 1e-4:
                return (
                    False,
                    f"cuff_{side}_bone gap={side_worst:.4f} max={WRIST_CUFF_WRIST_GAP_M}",
                    worst,
                )
        return True, f"cuff_wrist_bone_gap={worst:.4f}", worst

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
                f"cuff_{side} shell gap={side_worst:.4f} max={WRIST_CUFF_WRIST_GAP_M}",
                worst,
            )
    return True, f"cuff_wrist_gap={worst:.4f}", worst


def audit_researcher_render_meshes(
    body: bpy.types.Object,
    meshes: list[bpy.types.Object],
    *,
    prune: bool = True,
    arm: bpy.types.Object | None = None,
) -> tuple[bool, str]:
    if prune:
        prune_researcher_visual_artifacts(body, meshes, arm=arm)
    ok, msg = verify_render_mesh_islands(body, meshes)
    if not ok:
        return False, msg
    return True, msg


def reset_render_island_audit_log() -> Path:
    path = RENDER_ISLAND_AUDIT_LOG
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text("# staff-researcher-idle evaluated mesh island audit\n", encoding="utf-8")
    RENDER_SCENE_HYGIENE_LOG.write_text(
        "# staff-researcher-idle render scene hygiene\n", encoding="utf-8"
    )
    return path