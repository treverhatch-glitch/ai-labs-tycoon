"""Duplicate founder shirt-slot mesh for coat skirt + sleeves (proven skinning)."""
from __future__ import annotations

import bpy
import bmesh
from mathutils import Vector
from mathutils.bvhtree import BVHTree

import blender_common as bc
from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward
from kenney_geometry_landmarks import compute_landmarks, torso_half_width
from researcher_hero_geometry import _bmesh_faces_from_indices, _copy_body_vertex_groups

SKIRT_NAME = "ResearcherCoatSkirt"
SLEEVES_NAME = "ResearcherCoatSleeves"

SKIRT_ALLOWED_VG = frozenset(
    {"Hips", "Spine", "Chest", "UpperChest", "LeftUpLeg", "RightUpLeg"}
)
SLEEVE_ALLOWED_VG = frozenset(
    {
        "LeftShoulder",
        "LeftArm",
        "LeftForeArm",
        "RightShoulder",
        "RightArm",
        "RightForeArm",
    }
)
VG_DENY_HEAD = frozenset({"Head", "Neck"})
MAX_COAT_BODY_GAP_M = 0.08
MAX_COAT_EDGE_M = 0.15
MIN_COAT_ISLAND_VERTS = 50
COAT_ISLAND_BODY_TOUCH_M = 0.035


def _face_world_centroid(body: bpy.types.Object, face: bmesh.types.BMFace) -> Vector:
    wc = Vector((0.0, 0.0, 0.0))
    for v in face.verts:
        wc += body.matrix_world @ v.co
    return wc / len(face.verts)


def _body_bvh_world(body: bpy.types.Object) -> BVHTree:
    bm = bmesh.new()
    bm.from_mesh(body.data)
    bm.transform(body.matrix_world)
    bm.faces.ensure_lookup_table()
    tree = BVHTree.FromBMesh(bm)
    bm.free()
    return tree


def _transfer_coat_weights_nearest_face_interp(
    coat: bpy.types.Object, body: bpy.types.Object
) -> None:
    """Data Transfer: nearest-face interpolated vertex groups from rigged body."""
    if coat.type != "MESH" or body.type != "MESH":
        return
    for vg in body.vertex_groups:
        if vg.name not in coat.vertex_groups:
            coat.vertex_groups.new(name=vg.name)
    mod = coat.modifiers.new("CoatWeightXfer", "DATA_TRANSFER")
    mod.object = body
    mod.use_vert_data = True
    mod.data_types_verts = {"VGROUP_WEIGHTS"}
    mod.vert_mapping = "POLYINTERP_NEAREST"
    mod.mix_mode = "REPLACE"
    mod.mix_factor = 1.0
    view = bpy.context.view_layer
    prev_active = view.objects.active
    prev_mode = coat.mode
    prev_sel = [o for o in view.objects if o.select_get()]
    try:
        if coat.mode != "OBJECT":
            bpy.ops.object.mode_set(mode="OBJECT")
        bpy.ops.object.select_all(action="DESELECT")
        coat.select_set(True)
        view.objects.active = coat
        bpy.ops.object.modifier_apply(modifier=mod.name)
    except RuntimeError as exc:
        if mod.name in coat.modifiers:
            coat.modifiers.remove(mod)
        raise RuntimeError(f"FAIL staff-researcher-idle coat_weight_transfer {exc}") from exc
    finally:
        for o in view.objects:
            o.select_set(False)
        for o in prev_sel:
            o.select_set(True)
        if prev_active:
            view.objects.active = prev_active
        if prev_mode != "OBJECT" and prev_active == coat:
            bpy.ops.object.mode_set(mode=prev_mode)


def _cull_verts_far_from_body(
    ob: bpy.types.Object, body: bpy.types.Object, *, max_gap_m: float
) -> int:
    tree = _body_bvh_world(body)
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.verts.ensure_lookup_table()
    kill = []
    for v in bm.verts:
        co_w = ob.matrix_world @ v.co
        _loc, _n, _idx, dist = tree.find_nearest(co_w)
        if dist is None or dist > max_gap_m:
            kill.append(v)
    if not kill:
        bm.free()
        return 0
    bmesh.ops.delete(bm, geom=kill, context="VERTS")
    bmesh.ops.remove_doubles(bm, verts=bm.verts, dist=0.0003)
    bm.to_mesh(ob.data)
    ob.data.update()
    n = len(kill)
    bm.free()
    return n


def _cull_verts_below_z(ob: bpy.types.Object, z_cut: float) -> int:
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    kill = [v for v in bm.verts if (ob.matrix_world @ v.co).z < z_cut]
    if not kill:
        bm.free()
        return 0
    bmesh.ops.delete(bm, geom=kill, context="VERTS")
    bm.to_mesh(ob.data)
    ob.data.update()
    n = len(kill)
    bm.free()
    return n


def _cull_spike_edges(ob: bpy.types.Object, *, max_edge_m: float) -> int:
    mw = ob.matrix_world
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.verts.ensure_lookup_table()
    bm.edges.ensure_lookup_table()
    kill: set[bmesh.types.BMVert] = set()
    for e in bm.edges:
        a = mw @ e.verts[0].co
        b = mw @ e.verts[1].co
        if (a - b).length > max_edge_m:
            kill.add(e.verts[0])
            kill.add(e.verts[1])
    if not kill:
        bm.free()
        return 0
    bmesh.ops.delete(bm, geom=list(kill), context="VERTS")
    bm.to_mesh(ob.data)
    ob.data.update()
    n = len(kill)
    bm.free()
    return n


def _project_verts_onto_body_shell(ob: bpy.types.Object, body: bpy.types.Object) -> None:
    tree = _body_bvh_world(body)
    inv = ob.matrix_world.inverted()
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    for v in bm.verts:
        co_w = ob.matrix_world @ v.co
        loc, _n, _idx, _dist = tree.find_nearest(co_w)
        if loc is not None:
            v.co = inv @ loc
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()


def _keep_largest_coat_island(ob: bpy.types.Object) -> int:
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    visited: set[int] = set()
    catalog: list[tuple[list[bmesh.types.BMFace], int]] = []
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
        catalog.append((island, len(island)))
    if len(catalog) <= 1:
        bm.free()
        return len(catalog)
    catalog.sort(key=lambda row: row[1], reverse=True)
    doomed = [f for isl, _ in catalog[1:] for f in isl]
    bmesh.ops.delete(bm, geom=doomed, context="FACES")
    bm.to_mesh(ob.data)
    ob.data.update()
    left = len(catalog[0][0])
    bm.free()
    return left


def _count_mesh_islands(ob: bpy.types.Object) -> int:
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    visited: set[int] = set()
    n = 0
    for seed in bm.faces:
        if seed.index in visited:
            continue
        n += 1
        stack = [seed]
        visited.add(seed.index)
        while stack:
            face = stack.pop()
            for e in face.edges:
                for nb in e.link_faces:
                    if nb.index not in visited:
                        visited.add(nb.index)
                        stack.append(nb)
    bm.free()
    return n


def _prune_coat_loose_islands(
    ob: bpy.types.Object,
    body: bpy.types.Object,
    *,
    min_verts: int = MIN_COAT_ISLAND_VERTS,
    touch_m: float = COAT_ISLAND_BODY_TOUCH_M,
) -> int:
    """Drop shoulder shards: islands < min_verts or not touching body surface."""
    tree = _body_bvh_world(body)
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    bm.verts.ensure_lookup_table()
    visited: set[int] = set()
    catalog: list[tuple[list[bmesh.types.BMFace], int, bool]] = []
    for seed in bm.faces:
        if seed.index in visited:
            continue
        stack = [seed]
        island: list[bmesh.types.BMFace] = []
        island_verts: set[int] = set()
        visited.add(seed.index)
        while stack:
            face = stack.pop()
            island.append(face)
            for v in face.verts:
                island_verts.add(v.index)
            for e in face.edges:
                for nb in e.link_faces:
                    if nb.index not in visited:
                        visited.add(nb.index)
                        stack.append(nb)
        touches = False
        for vi in island_verts:
            co_w = ob.matrix_world @ bm.verts[vi].co
            _loc, _n, _idx, dist = tree.find_nearest(co_w)
            if dist is not None and dist <= touch_m:
                touches = True
                break
        catalog.append((island, len(island_verts), touches))
    catalog.sort(key=lambda row: row[1], reverse=True)
    doomed: list[bmesh.types.BMFace] = []
    removed = 0
    for idx, (island, n_verts, touches) in enumerate(catalog):
        keep_main = idx == 0 and touches and n_verts >= 12
        if keep_main:
            continue
        if n_verts < min_verts or not touches:
            doomed.extend(island)
            removed += n_verts
    if doomed:
        bmesh.ops.delete(bm, geom=doomed, context="FACES")
    bmesh.ops.remove_doubles(bm, verts=bm.verts, dist=0.0012)
    loose = [v for v in bm.verts if not v.link_faces]
    if loose:
        bmesh.ops.delete(bm, geom=loose, context="VERTS")
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()
    return removed


def _sanitize_coat_part(
    ob: bpy.types.Object,
    body: bpy.types.Object,
    *,
    allow: frozenset[str],
    z_knee: float,
    project_shell: bool = False,
    cull_below_knee: bool = True,
    island_touch_m: float = COAT_ISLAND_BODY_TOUCH_M,
    max_body_gap_m: float = MAX_COAT_BODY_GAP_M,
    require_single_island: bool = True,
) -> None:
    if project_shell:
        _project_verts_onto_body_shell(ob, body)
    _transfer_coat_weights_nearest_face_interp(ob, body)
    _purge_vertex_groups(ob, allow=allow, deny=VG_DENY_HEAD)
    _normalize_vertex_groups(ob)
    _prune_coat_loose_islands(ob, body, touch_m=island_touch_m)
    _cull_spike_edges(ob, max_edge_m=MAX_COAT_EDGE_M)
    if cull_below_knee:
        _cull_verts_below_z(ob, z_knee - 0.008)
    _cull_verts_far_from_body(ob, body, max_gap_m=max_body_gap_m)
    if len(ob.data.vertices) < 12:
        raise RuntimeError(f"FAIL staff-researcher-idle coat_cull_empty name={ob.name}")
    islands = _count_mesh_islands(ob)
    if require_single_island and islands != 1:
        _keep_largest_coat_island(ob)
        islands = _count_mesh_islands(ob)
    if require_single_island and islands != 1:
        raise RuntimeError(
            f"FAIL staff-researcher-idle coat_islands={islands} name={ob.name} want=1"
        )


def lock_sleeve_cuff_weights_to_forearm(
    sleeves: bpy.types.Object,
    arm: bpy.types.Object,
    lm,
    *,
    max_wrist_dist_m: float = 0.12,
) -> int:
    from researcher_scene_mesh_audit import _wrist_head_world

    deps = bpy.context.evaluated_depsgraph_get()
    deps.update()
    mesh_ob = sleeves.evaluated_get(deps)
    locked = 0
    for vi in range(len(sleeves.data.vertices)):
        wc = mesh_ob.matrix_world @ mesh_ob.data.vertices[vi].co
        best_side = None
        best_d = 1e9
        for side in ("Left", "Right"):
            wrist = _wrist_head_world(arm, side)
            if wrist is None:
                continue
            d = (wc - wrist).length
            if d < best_d:
                best_d = d
                best_side = side
        if best_side is None or best_d > max(lm.span * 0.35, max_wrist_dist_m * 2.5):
            continue
        if best_d > max_wrist_dist_m:
            continue
        fa = f"{best_side}ForeArm"
        hd = f"{best_side}Hand"
        for g in list(sleeves.data.vertices[vi].groups):
            sleeves.vertex_groups[g.group].remove([vi])
        if fa not in sleeves.vertex_groups:
            sleeves.vertex_groups.new(name=fa)
        if hd not in sleeves.vertex_groups:
            sleeves.vertex_groups.new(name=hd)
        w_fa = min(1.0, max(0.65, 1.0 - best_d / max_wrist_dist_m * 0.35))
        sleeves.vertex_groups[fa].add([vi], w_fa, "REPLACE")
        sleeves.vertex_groups[hd].add([vi], 1.0 - w_fa, "REPLACE")
        locked += 1
    return locked


def nudge_sleeve_cuffs_toward_wrists(
    sleeves: bpy.types.Object,
    arm: bpy.types.Object,
    lm,
    *,
    iterations: int = 24,
    target_gap_m: float = 0.034,
) -> float:
    """Heuristic: nudge rest cuffs so evaluated geometry sits near posed wrist bones."""
    from researcher_scene_mesh_audit import _infer_vert_arm_side, _wrist_head_world

    inv_obj = sleeves.matrix_world.inverted()
    lo, span = lm.zmin, lm.span
    wrist_h = lm.h_shoe + 0.22
    worst = 1.0
    for _ in range(iterations):
        bpy.context.view_layer.update()
        deps = bpy.context.evaluated_depsgraph_get()
        deps.update()
        mesh_ob = sleeves.evaluated_get(deps)
        worst = 0.0
        moved = False
        for side in ("Left", "Right"):
            wrist = _wrist_head_world(arm, side)
            if wrist is None:
                continue
            ranked: list[tuple[float, int, Vector]] = []
            for vi in range(len(sleeves.data.vertices)):
                if _infer_vert_arm_side(sleeves, vi) != side:
                    continue
                wc = mesh_ob.matrix_world @ mesh_ob.data.vertices[vi].co
                h = (wc.z - lo) / span
                if h < wrist_h - 0.20:
                    continue
                dist = (wc - wrist).length
                worst = max(worst, dist)
                ranked.append((dist, vi, wc))
            ranked.sort(key=lambda row: row[0])
            for dist, vi, wc in ranked[:16]:
                if dist <= target_gap_m:
                    continue
                radial = wc - wrist
                if radial.length < 1e-5:
                    radial = Vector((0.0, target_gap_m, 0.0))
                target_w = wrist + radial.normalized() * min(target_gap_m, radial.length * 0.65)
                delta_w = target_w - wc
                sleeves.data.vertices[vi].co += inv_obj @ delta_w
                moved = True
        if not moved:
            break
    sleeves.data.update()
    return worst


def bind_cuff_verts_to_hand_bone(
    sleeves: bpy.types.Object,
    arm: bpy.types.Object,
    lm,
    *,
    max_wrist_dist_m: float | None = None,
    cuff_radius_m: float = 0.034,
) -> int:
    """Single-bone bind: rest verts placed so posed Hand wrist keeps cuffs within radius."""
    from researcher_scene_mesh_audit import _wrist_head_world

    lo, span = lm.zmin, lm.span
    wrist_h = lm.h_shoe + 0.22
    pick_radius = max_wrist_dist_m if max_wrist_dist_m is not None else span * 0.55
    deps = bpy.context.evaluated_depsgraph_get()
    deps.update()
    mesh_ob = sleeves.evaluated_get(deps)
    inv_obj = sleeves.matrix_world.inverted()
    touched = 0
    for vi in range(len(sleeves.data.vertices)):
        wc = mesh_ob.matrix_world @ mesh_ob.data.vertices[vi].co
        h = (wc.z - lo) / span
        if h < wrist_h - 0.24:
            continue
        best_side = None
        best_d = 1e9
        for side in ("Left", "Right"):
            wrist = _wrist_head_world(arm, side)
            if wrist is None:
                continue
            d = (wc - wrist).length
            if d < best_d:
                best_d = d
                best_side = side
        if best_side is None or best_d > pick_radius:
            continue
        pb = arm.pose.bones.get(f"{best_side}Hand")
        if pb is None:
            pb = arm.pose.bones.get(f"{best_side}ForeArm")
        if pb is None:
            continue
        wrist = _wrist_head_world(arm, best_side)
        assert wrist is not None
        radial = wc - wrist
        if radial.length < 1e-5:
            radial = Vector((0.0, cuff_radius_m, 0.0))
        target_w = wrist + radial.normalized() * min(cuff_radius_m, radial.length + 0.01)
        bone_mw = arm.matrix_world @ pb.matrix
        co_rest = inv_obj @ bone_mw.inverted() @ target_w
        sleeves.data.vertices[vi].co = co_rest
        hd = f"{best_side}Hand"
        fa = f"{best_side}ForeArm"
        for g in list(sleeves.data.vertices[vi].groups):
            sleeves.vertex_groups[g.group].remove([vi])
        if hd not in sleeves.vertex_groups:
            sleeves.vertex_groups.new(name=hd)
        sleeves.vertex_groups[hd].add([vi], 0.92, "REPLACE")
        if fa in sleeves.vertex_groups or True:
            if fa not in sleeves.vertex_groups:
                sleeves.vertex_groups.new(name=fa)
            sleeves.vertex_groups[fa].add([vi], 0.08, "REPLACE")
        touched += 1
    sleeves.data.update()
    return touched


def snap_sleeve_cuffs_to_arm(
    sleeves: bpy.types.Object,
    arm: bpy.types.Object,
    lm,
    *,
    max_radius_m: float = 0.038,
) -> int:
    """Pull cuff-band verts toward posed wrist bones (walk / rigged scenes)."""
    from researcher_scene_mesh_audit import _wrist_head_world

    deps = bpy.context.evaluated_depsgraph_get()
    deps.update()
    mesh_ob = sleeves.evaluated_get(deps)
    lo, span = lm.zmin, lm.span
    wrist_h = lm.h_shoe + 0.22
    inv = sleeves.matrix_world.inverted()
    bm = bmesh.new()
    bm.from_mesh(sleeves.data)
    bm.verts.ensure_lookup_table()
    touched = 0
    for v in bm.verts:
        vi = v.index
        if vi >= len(mesh_ob.data.vertices):
            continue
        wc = mesh_ob.matrix_world @ mesh_ob.data.vertices[vi].co
        h = (wc.z - lo) / span
        if h < wrist_h - 0.18 or h > wrist_h + 0.14:
            continue
        best_side = None
        best_dist = 1e9
        for side in ("Left", "Right"):
            wrist = _wrist_head_world(arm, side)
            if wrist is None:
                continue
            d = (wc - wrist).length
            if d < best_dist:
                best_dist = d
                best_side = side
        if best_side is None or best_dist > span * 0.45:
            continue
        wrist = _wrist_head_world(arm, best_side)
        assert wrist is not None
        radial = wc - wrist
        if radial.length < 1e-6:
            radial = Vector((0.0, 0.02, 0.0))
        target = wrist + radial.normalized() * min(max_radius_m, max(radial.length, 0.02))
        v.co = inv @ target
        touched += 1
    if touched:
        bm.to_mesh(sleeves.data)
        sleeves.data.update()
    bm.free()
    return touched


def _snap_sleeve_cuffs_to_body_wrist(
    sleeves: bpy.types.Object, body: bpy.types.Object, lm
) -> None:
    """Cuff band: snap onto body shell near wrist height for forearm skinning."""
    lo, span = lm.zmin, lm.span
    wrist_h = lm.h_shoe + 0.22
    wrist_z = lo + span * wrist_h
    z_lo = wrist_z - span * 0.05
    z_hi = wrist_z + span * 0.04
    tree = _body_bvh_world(body)
    inv = sleeves.matrix_world.inverted()
    bm = bmesh.new()
    bm.from_mesh(sleeves.data)
    bm.verts.ensure_lookup_table()
    touched = 0
    for v in bm.verts:
        wc = sleeves.matrix_world @ v.co
        if wc.z < z_lo or wc.z > z_hi:
            continue
        if abs(wc.x - lm.cx) < span * 0.08:
            continue
        probe = Vector((wc.x, wc.y, wrist_z))
        loc, _n, _idx, _dist = tree.find_nearest(probe)
        if loc is None:
            loc = probe
        v.co = inv @ loc
        touched += 1
    if touched:
        bm.to_mesh(sleeves.data)
        sleeves.data.update()
    bm.free()


def _boost_sleeve_forearm_weights(
    sleeves: bpy.types.Object,
    lm,
    *,
    body_cx: float | None = None,
    arm: bpy.types.Object | None = None,
) -> None:
    """Cuff verts: skin mostly to forearm bones for walk swing."""
    from researcher_scene_mesh_audit import _wrist_head_world

    lo, span = lm.zmin, lm.span
    wrist_h = lm.h_shoe + 0.22
    cx = body_cx if body_cx is not None else lm.cx
    for vi in range(len(sleeves.data.vertices)):
        wc = sleeves.matrix_world @ sleeves.data.vertices[vi].co
        h = (wc.z - lo) / span
        if h < wrist_h - 0.32:
            continue
        side = None
        if arm is not None:
            d_l = (
                (wc - _wrist_head_world(arm, "Left")).length
                if _wrist_head_world(arm, "Left")
                else 1e9
            )
            d_r = (
                (wc - _wrist_head_world(arm, "Right")).length
                if _wrist_head_world(arm, "Right")
                else 1e9
            )
            if min(d_l, d_r) > span * 0.55:
                continue
            side = "Left" if d_l < d_r else "Right"
        else:
            if wc.x < cx - span * 0.02:
                side = "Left"
            elif wc.x > cx + span * 0.02:
                side = "Right"
        if side is None:
            continue
        fa = f"{side}ForeArm"
        ua = f"{side}Arm"
        sh = f"{side}Shoulder"
        if fa not in sleeves.vertex_groups:
            sleeves.vertex_groups.new(name=fa)
        if ua not in sleeves.vertex_groups:
            sleeves.vertex_groups.new(name=ua)
        for g in list(sleeves.data.vertices[vi].groups):
            sleeves.vertex_groups[g.group].remove([vi])
        band = max(0.0, min(1.0, (h - (wrist_h - 0.12)) / 0.14))
        fw = 0.55 + 0.40 * band
        aw = 1.0 - fw
        sleeves.vertex_groups[fa].add([vi], fw, "REPLACE")
        if sh in sleeves.vertex_groups:
            sleeves.vertex_groups[ua].add([vi], aw, "REPLACE")


def _repair_weights_from_nearest_body(
    ob: bpy.types.Object, body: bpy.types.Object, *, allow: frozenset[str]
) -> None:
    import mathutils.kdtree as kdtree

    kd = kdtree.KDTree(len(body.data.vertices))
    for i, v in enumerate(body.data.vertices):
        kd.insert(body.matrix_world @ v.co, i)
    kd.balance()
    for vi, v in enumerate(ob.data.vertices):
        total = sum(g.weight for g in v.groups)
        if total >= 0.92:
            continue
        co = ob.matrix_world @ v.co
        _idx, bidx, _dist = kd.find(co)
        raw: dict[str, float] = {}
        for g in body.data.vertices[bidx].groups:
            if g.group >= len(body.vertex_groups):
                continue
            name = body.vertex_groups[g.group].name
            if name in allow:
                raw[name] = raw.get(name, 0.0) + g.weight
        if not raw:
            raw["Chest"] = 1.0
        tot = sum(raw.values())
        for g in list(v.groups):
            ob.vertex_groups[g.group].remove([vi])
        for name, w in raw.items():
            if name not in ob.vertex_groups:
                ob.vertex_groups.new(name=name)
            ob.vertex_groups[name].add([vi], w / tot, "REPLACE")


def _normalize_vertex_groups(ob: bpy.types.Object) -> None:
    for vi in range(len(ob.data.vertices)):
        v = ob.data.vertices[vi]
        gs = [g for g in v.groups if g.weight > 1e-5]
        if not gs:
            continue
        total = sum(g.weight for g in gs)
        if total < 1e-6:
            continue
        scale = 1.0 / total
        if abs(scale - 1.0) < 1e-4:
            continue
        for g in gs:
            ob.vertex_groups[g.group].add([vi], g.weight * scale, "REPLACE")


def _purge_vertex_groups(
    ob: bpy.types.Object, *, allow: frozenset[str], deny: frozenset[str] | None = None
) -> None:
    deny = deny or frozenset()
    for v in ob.data.vertices:
        for g in list(v.groups):
            name = ob.vertex_groups[g.group].name
            if name in deny or (allow and name not in allow):
                ob.vertex_groups[g.group].remove([v.index])
    for name in list(ob.vertex_groups.keys()):
        if name in deny or (allow and name not in allow):
            ob.vertex_groups.remove(ob.vertex_groups[name])


def _mesh_from_face_pick(
    body: bpy.types.Object, bm_src: bmesh.types.BMesh, pick: set[int], name: str
) -> tuple[bpy.types.Object, list[int | None]]:
    bm = bmesh.new()
    vmap = _bmesh_faces_from_indices(bm, bm_src, pick)
    inv_map: dict[int, int] = {}
    for si, bv in vmap.items():
        inv_map[bv.index] = si
    src_for: list[int | None] = []
    for v in bm.verts:
        src_for.append(inv_map.get(v.index))
    mesh = bpy.data.meshes.new(f"{name}Mesh")
    bm.to_mesh(mesh)
    bm.free()
    ob = bpy.data.objects.new(name, mesh)
    bpy.context.collection.objects.link(ob)
    _copy_body_vertex_groups(body, ob, src_for)
    bc.smooth_shade(ob)
    return ob, src_for


def knee_world_z(body: bpy.types.Object, lm) -> float:
    return lm.zmin + lm.span * (lm.h_shoe + 0.36)


def _skirt_z_band(
    body: bpy.types.Object, lm
) -> tuple[float, float, float, float, float, float]:
    lo, span = lm.zmin, lm.span
    skirt_top_z = lo + span * (lm.h_neck - 0.12)
    z_hip = lo + span * (lm.h_waist - 0.06)
    z_knee = lo + span * (lm.h_shoe + 0.38)
    hip_pts: list[float] = []
    knee_pts: list[float] = []
    for v in body.data.vertices:
        w = body.matrix_world @ v.co
        h = (w.z - lo) / span
        if lm.h_waist - 0.08 < h < lm.h_waist + 0.06:
            hip_pts.append(w.z)
        if lm.h_shoe + 0.22 < h < lm.h_waist + 0.14:
            knee_pts.append(w.z)
    if hip_pts:
        z_hip = min(hip_pts)
    if knee_pts:
        z_knee = sum(knee_pts) / len(knee_pts)
    arm = next((o for o in bpy.context.scene.objects if o.type == "ARMATURE"), None)
    if arm:
        hip_b = arm.data.bones.get("Hips")
        if hip_b:
            z_hip = (arm.matrix_world @ hip_b.head_local).z - span * 0.02
    z_mid_thigh = (z_hip + z_knee) * 0.5
    return lo, span, skirt_top_z, z_hip, z_mid_thigh, z_knee


def pick_shirt_skirt_faces(
    body: bpy.types.Object,
    bm_src: bmesh.types.BMesh,
    *,
    shirt_slot: int,
    jeans_slot: int,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> set[int]:
    lo, span, skirt_top_z, z_hip, z_mid_thigh, _z_knee = _skirt_z_band(body, lm)
    pick: set[int] = set()
    for f in bm_src.faces:
        if f.material_index not in (shirt_slot, jeans_slot):
            continue
        wc = _face_world_centroid(body, f)
        if wc.z < z_hip - span * 0.03 or wc.z > z_mid_thigh + span * 0.04:
            continue
        if wc.z > skirt_top_z:
            continue
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if wc.z >= z_hip + (z_mid_thigh - z_hip) * 0.18:
            lat_cap = thw * 1.95
        else:
            lat_cap = thw * 1.42
        if lat > lat_cap:
            continue
        pick.add(f.index)
    hem_seed = max((z_hip - span * 0.02, 0.98))
    for f in bm_src.faces:
        if f.index in pick or f.material_index != jeans_slot:
            continue
        wc = _face_world_centroid(body, f)
        if wc.z < hem_seed or wc.z > z_mid_thigh + span * 0.04:
            continue
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat > thw * 1.85:
            continue
        pick.add(f.index)
    torso_lo = z_hip - span * 0.02
    for f in bm_src.faces:
        if f.index in pick or f.material_index not in (shirt_slot, jeans_slot):
            continue
        wc = _face_world_centroid(body, f)
        if wc.z < torso_lo or wc.z > skirt_top_z + span * 0.03:
            continue
        h = (wc.z - lo) / span
        if h < lm.h_neck - 0.14:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat > thw * 1.92:
            continue
        pick.add(f.index)
    z_top = skirt_top_z + span * 0.05
    z_lo = z_hip - span * 0.04
    changed = True
    while changed:
        changed = False
        for f in bm_src.faces:
            if f.index in pick:
                continue
            if f.material_index not in (shirt_slot, jeans_slot):
                continue
            if not any(nb.index in pick for e in f.edges for nb in e.link_faces):
                continue
            wc = _face_world_centroid(body, f)
            if wc.z < z_lo or wc.z > z_top:
                continue
            pick.add(f.index)
            changed = True
    return pick


def _neck_cut_world_z(arm: bpy.types.Object | None, lm) -> float:
    lo, span = lm.zmin, lm.span
    if arm and arm.type == "ARMATURE":
        neck = arm.data.bones.get("Neck")
        if neck:
            return (arm.matrix_world @ neck.head_local).z
    return lo + span * (lm.h_neck - 0.10)


def _stretch_skirt_hem_to_mid_thigh(
    skirt: bpy.types.Object, z_hip: float, z_mid: float, *, skirt_top_z: float
) -> None:
    if not skirt.data.vertices:
        return
    mw = skirt.matrix_world.copy()
    inv = mw.inverted()
    bm = bmesh.new()
    bm.from_mesh(skirt.data)
    bm.verts.ensure_lookup_table()
    zs = [(mw @ v.co).z for v in bm.verts]
    z_hi = max(zs)
    if z_hi >= z_mid - 0.02:
        bm.free()
        return
    scale = (z_mid - z_hip) / max(1e-6, z_hi - z_hip)
    for v in bm.verts:
        w = mw @ v.co
        if w.z > skirt_top_z or w.z < z_hip:
            continue
        w.z = z_hip + (w.z - z_hip) * scale
        v.co = inv @ w
    bm.to_mesh(skirt.data)
    skirt.data.update()
    bm.free()


def _prune_sleeve_torso_bridge(ob: bpy.types.Object, lm) -> int:
    """Drop shirt faces bridging both arms across the chest (splits one island into L/R)."""
    lo, span = lm.zmin, lm.span
    cx = lm.cx
    z_lo = lo + span * (lm.h_waist - 0.02)
    z_hi = lo + span * (lm.h_neck - 0.06)
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    doomed: list[bmesh.types.BMFace] = []
    for f in bm.faces:
        wc = ob.matrix_world @ f.calc_center_median()
        if wc.z < z_lo or wc.z > z_hi:
            continue
        if abs(wc.x - cx) > span * 0.11:
            continue
        doomed.append(f)
    if doomed:
        bmesh.ops.delete(bm, geom=doomed, context="FACES")
        bm.to_mesh(ob.data)
        ob.data.update()
    removed = len(doomed)
    bm.free()
    return removed


def finalize_coat_sleeve_islands(
    sleeves: bpy.types.Object,
    body: bpy.types.Object,
    lm,
) -> None:
    from researcher_scene_mesh_audit import _delete_island_faces, _mesh_island_sets

    _prune_sleeve_torso_bridge(sleeves, lm)
    _prune_coat_loose_islands(sleeves, body, min_verts=8, touch_m=0.12)

    while len(_mesh_island_sets(sleeves)) > 2:
        face_ids, _ = min(_mesh_island_sets(sleeves), key=lambda row: len(row[1]))
        _delete_island_faces(sleeves, face_ids)
    isl = _count_mesh_islands(sleeves)
    if isl < 1 or isl > 2:
        raise RuntimeError(
            f"FAIL staff-researcher-idle coat_islands={isl} mesh={SLEEVES_NAME} want<=2"
        )


def _prune_verts_above_z(ob: bpy.types.Object, z_cut: float, *, margin_m: float = 0.006) -> int:
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.verts.ensure_lookup_table()
    kill = [
        v.index
        for v in bm.verts
        if (ob.matrix_world @ v.co).z > z_cut + margin_m
    ]
    if not kill:
        bm.free()
        return 0
    bmesh.ops.delete(bm, geom=[bm.verts[i] for i in kill], context="VERTS")
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()
    return len(kill)


def pick_shirt_sleeve_faces(
    body: bpy.types.Object,
    bm_src: bmesh.types.BMesh,
    *,
    shirt_slot: int,
    hand_slot: int,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> set[int]:
    lo, span = lm.zmin, lm.span
    wrist_lo = lm.h_shoe + 0.17
    sleeve_hi = lm.h_neck + 0.12
    pick: set[int] = set()
    for f in bm_src.faces:
        mat_ok = f.material_index in (shirt_slot, hand_slot, 2)
        if not mat_ok:
            continue
        wc = _face_world_centroid(body, f)
        h = (wc.z - lo) / span
        if h < wrist_lo - 0.02 or h > sleeve_hi:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        arm_band = lat > thw * 0.52
        if not arm_band:
            continue
        if f.material_index == 2 and h > lm.h_waist + 0.08:
            continue
        if f.material_index == hand_slot and lat <= thw * 0.68:
            continue
        pick.add(f.index)
    changed = True
    while changed:
        changed = False
        for f in bm_src.faces:
            if f.index in pick:
                continue
            if f.material_index not in (shirt_slot, 5):
                continue
            if not any(nb.index in pick for e in f.edges for nb in e.link_faces):
                continue
            wc = _face_world_centroid(body, f)
            h = (wc.z - lo) / span
            if wrist_lo - 0.05 <= h <= sleeve_hi:
                pick.add(f.index)
                changed = True
    return pick


def snapshot_evaluated_body(body: bpy.types.Object) -> bpy.types.Object:
    deps = bpy.context.evaluated_depsgraph_get()
    deps.update()
    me = bpy.data.meshes.new_from_object(body, depsgraph=deps)
    snap = bpy.data.objects.new("_BodyPoseSnap", me)
    snap.matrix_world = body.matrix_world.copy()
    bpy.context.collection.objects.link(snap)
    return snap


def sync_coat_parts_to_body(
    body: bpy.types.Object, parts: list[bpy.types.Object], arm: bpy.types.Object | None
) -> None:
    for ob in parts:
        if ob.name.split(".")[0] == SKIRT_NAME:
            for mod in list(ob.modifiers):
                if mod.type == "ARMATURE":
                    ob.modifiers.remove(mod)
            ob.parent = body
            ob.matrix_parent_inverse = body.matrix_world.inverted()
            continue
        if arm is not None and any(m.type == "ARMATURE" for m in ob.modifiers):
            ob.matrix_world = body.matrix_world.copy()
        elif arm is None:
            if ob.parent != body:
                ob.parent = body
                ob.matrix_parent_inverse = body.matrix_world.inverted()


def _attach_coat_part_to_body_or_arm(
    ob: bpy.types.Object,
    body: bpy.types.Object,
    arm: bpy.types.Object | None,
) -> None:
    if ob.name.split(".")[0] == SKIRT_NAME:
        ob.parent = body
        ob.matrix_parent_inverse = body.matrix_world.inverted()
        return
    if arm is not None:
        if ob.parent:
            ob.parent = None
        ob.matrix_world = body.matrix_world.copy()
        for mod in list(ob.modifiers):
            if mod.type == "ARMATURE":
                ob.modifiers.remove(mod)
        mod = ob.modifiers.new("CoatWalkArmature", "ARMATURE")
        mod.object = arm
        bpy.context.view_layer.update()
        return
    ob.parent = body
    ob.matrix_parent_inverse = body.matrix_world.inverted()


def spawn_shirt_derived_coat_parts(
    body: bpy.types.Object,
    *,
    shirt_slot: int,
    coat_mat,
    weight_body: bpy.types.Object | None = None,
    spawn_parts: tuple[str, ...] = (SKIRT_NAME, SLEEVES_NAME),
) -> list[bpy.types.Object]:
    for name in spawn_parts:
        for ob in list(bpy.context.scene.objects):
            if ob.type == "MESH" and ob.name.split(".")[0] == name:
                bpy.data.objects.remove(ob, do_unlink=True)
    weight_body = weight_body or body
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    bm_src = bmesh.new()
    bm_src.from_mesh(body.data)
    bm_src.faces.ensure_lookup_table()
    skirt_pick = pick_shirt_skirt_faces(
        body,
        bm_src,
        shirt_slot=shirt_slot,
        jeans_slot=2,
        lm=lm,
        hc=hc,
        fwd=fwd,
        ws=ws,
    )
    if len(skirt_pick) < 12:
        lo, span = lm.zmin, lm.span
        for f in bm_src.faces:
            if f.material_index not in (shirt_slot, 2):
                continue
            wc = _face_world_centroid(body, f)
            h = (wc.z - lo) / span
            if lm.h_waist - 0.06 <= h <= lm.h_waist + 0.40 and h < lm.h_neck - 0.14:
                skirt_pick.add(f.index)
    want_skirt = SKIRT_NAME in spawn_parts
    want_sleeves = SLEEVES_NAME in spawn_parts
    sleeve_pick: set[int] = set()
    if want_sleeves:
        sleeve_src = weight_body if weight_body != body else body
        bm_sleeves = bmesh.new()
        bm_sleeves.from_mesh(sleeve_src.data)
        bm_sleeves.faces.ensure_lookup_table()
        lm_s = compute_landmarks(sleeve_src)
        ws_s = [sleeve_src.matrix_world @ v.co for v in sleeve_src.data.vertices]
        hc_s, _ = face_shell_coord(sleeve_src, fwd)
        sleeve_pick = pick_shirt_sleeve_faces(
            sleeve_src,
            bm_sleeves,
            shirt_slot=shirt_slot,
            hand_slot=5,
            lm=lm_s,
            hc=hc_s,
            fwd=fwd,
            ws=ws_s,
        )
    bm_src.free()
    parts: list[bpy.types.Object] = []
    if want_skirt and len(skirt_pick) < 12:
        raise RuntimeError(f"FAIL staff-researcher-idle skirt_faces={len(skirt_pick)} too_few")
    if want_sleeves and len(sleeve_pick) < 16:
        raise RuntimeError(f"FAIL staff-researcher-idle sleeve_faces={len(sleeve_pick)} too_few")
    skirt = sleeves = None
    if want_skirt:
        bm_sk = bmesh.new()
        bm_sk.from_mesh(body.data)
        bm_sk.faces.ensure_lookup_table()
        skirt, _ = _mesh_from_face_pick(body, bm_sk, skirt_pick, SKIRT_NAME)
        bm_sk.free()
    if want_sleeves:
        sleeve_src = weight_body if weight_body != body else body
        bm_sl = bmesh.new()
        bm_sl.from_mesh(sleeve_src.data)
        bm_sl.faces.ensure_lookup_table()
        sleeves, _ = _mesh_from_face_pick(sleeve_src, bm_sl, sleeve_pick, SLEEVES_NAME)
        bm_sl.free()
    lo, span, skirt_top_z, z_hip, z_mid_thigh, _zk = _skirt_z_band(body, lm)
    z_knee = knee_world_z(body, lm)
    arm = next((o for o in bpy.context.scene.objects if o.type == "ARMATURE"), None)
    z_neck = _neck_cut_world_z(arm, lm)
    if skirt is None:
        assert not want_skirt
        assert sleeves is not None
        # sleeves-only path
        sleeves.data.materials.clear()
        sleeves.data.materials.append(coat_mat)
        _sanitize_coat_part(
            sleeves,
            weight_body,
            allow=SLEEVE_ALLOWED_VG,
            z_knee=z_knee,
            project_shell=False,
            cull_below_knee=False,
            island_touch_m=0.10,
            max_body_gap_m=0.15,
            require_single_island=False,
        )
        from researcher_scene_mesh_audit import head_top_world

        removed = _prune_verts_above_z(sleeves, head_top_world(body) - 0.015)
        if removed and len(sleeves.data.vertices) < 20:
            raise RuntimeError(f"FAIL staff-researcher-idle sleeves_prune_head removed={removed}")
        shell_body = weight_body
        _snap_sleeve_cuffs_to_body_wrist(sleeves, shell_body, lm)
        _transfer_coat_weights_nearest_face_interp(sleeves, weight_body)
        _purge_vertex_groups(sleeves, allow=SLEEVE_ALLOWED_VG, deny=VG_DENY_HEAD)
        _normalize_vertex_groups(sleeves)
        _boost_sleeve_forearm_weights(sleeves, lm, body_cx=lm.cx)
        _cull_spike_edges(sleeves, max_edge_m=MAX_COAT_EDGE_M)
        if shell_body == body:
            _cull_verts_far_from_body(sleeves, shell_body, max_gap_m=MAX_COAT_BODY_GAP_M)
        finalize_coat_sleeve_islands(sleeves, shell_body, lm)
        parts.append(sleeves)
        return parts
    assert skirt is not None
    _stretch_skirt_hem_to_mid_thigh(skirt, z_hip, z_mid_thigh, skirt_top_z=skirt_top_z)
    z_after = max((skirt.matrix_world @ v.co).z for v in skirt.data.vertices)
    if z_after < z_mid_thigh - span * 0.06:
        _stretch_skirt_hem_to_mid_thigh(skirt, z_hip, z_mid_thigh, skirt_top_z=skirt_top_z)
    removed = _prune_verts_above_z(skirt, z_neck)
    if removed > 0 and len(skirt.data.vertices) < 12:
        raise RuntimeError(f"FAIL staff-researcher-idle skirt_prune_neck removed={removed}")
    skirt.data.materials.clear()
    skirt.data.materials.append(coat_mat)
    _sanitize_coat_part(
        skirt,
        weight_body,
        allow=SKIRT_ALLOWED_VG,
        z_knee=z_knee,
        project_shell=True,
    )
    _stretch_skirt_hem_to_mid_thigh(skirt, z_hip, z_mid_thigh, skirt_top_z=skirt_top_z)
    _project_verts_onto_body_shell(skirt, body)
    _transfer_coat_weights_nearest_face_interp(skirt, weight_body)
    _purge_vertex_groups(skirt, allow=SKIRT_ALLOWED_VG, deny=VG_DENY_HEAD)
    _normalize_vertex_groups(skirt)
    _cull_spike_edges(skirt, max_edge_m=MAX_COAT_EDGE_M)
    _cull_verts_far_from_body(skirt, body, max_gap_m=MAX_COAT_BODY_GAP_M)
    if len(skirt.data.vertices) < 12:
        raise RuntimeError("FAIL staff-researcher-idle skirt_post_hem_empty")
    if not want_sleeves:
        parts.append(skirt)
        return parts
    assert sleeves is not None
    sleeves.data.materials.clear()
    sleeves.data.materials.append(coat_mat)
    _sanitize_coat_part(
        sleeves,
        weight_body,
        allow=SLEEVE_ALLOWED_VG,
        z_knee=z_knee,
        project_shell=False,
        cull_below_knee=False,
        island_touch_m=0.10,
        max_body_gap_m=0.15,
        require_single_island=False,
    )
    from researcher_scene_mesh_audit import head_top_world

    removed = _prune_verts_above_z(sleeves, head_top_world(body) - 0.015)
    if removed and len(sleeves.data.vertices) < 20:
        raise RuntimeError(f"FAIL staff-researcher-idle sleeves_prune_head removed={removed}")
    shell_body = weight_body
    _snap_sleeve_cuffs_to_body_wrist(sleeves, shell_body, lm)
    _transfer_coat_weights_nearest_face_interp(sleeves, weight_body)
    _purge_vertex_groups(sleeves, allow=SLEEVE_ALLOWED_VG, deny=VG_DENY_HEAD)
    _normalize_vertex_groups(sleeves)
    _boost_sleeve_forearm_weights(sleeves, lm, body_cx=lm.cx)
    _cull_spike_edges(sleeves, max_edge_m=MAX_COAT_EDGE_M)
    if shell_body == body:
        _cull_verts_far_from_body(sleeves, shell_body, max_gap_m=MAX_COAT_BODY_GAP_M)
    finalize_coat_sleeve_islands(sleeves, shell_body, lm)
    parts.extend([skirt, sleeves])
    return parts