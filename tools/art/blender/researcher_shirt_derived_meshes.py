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


def _sanitize_coat_part(
    ob: bpy.types.Object,
    body: bpy.types.Object,
    *,
    allow: frozenset[str],
    z_knee: float,
    project_shell: bool = False,
    cull_below_knee: bool = True,
) -> None:
    if project_shell:
        _project_verts_onto_body_shell(ob, body)
    _transfer_coat_weights_nearest_face_interp(ob, body)
    _purge_vertex_groups(ob, allow=allow, deny=VG_DENY_HEAD)
    _normalize_vertex_groups(ob)
    _cull_spike_edges(ob, max_edge_m=MAX_COAT_EDGE_M)
    if cull_below_knee:
        _cull_verts_below_z(ob, z_knee - 0.008)
    _cull_verts_far_from_body(ob, body, max_gap_m=MAX_COAT_BODY_GAP_M)
    if len(ob.data.vertices) < 12:
        raise RuntimeError(f"FAIL staff-researcher-idle coat_cull_empty name={ob.name}")


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
    wrist_lo = lm.h_shoe + 0.02
    sleeve_hi = lm.h_neck + 0.08
    pick: set[int] = set()
    for f in bm_src.faces:
        if f.material_index not in (shirt_slot, hand_slot):
            continue
        wc = _face_world_centroid(body, f)
        h = (wc.z - lo) / span
        if h < wrist_lo - 0.05 or h > sleeve_hi:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat <= thw * 0.24:
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
) -> list[bpy.types.Object]:
    for name in (SKIRT_NAME, SLEEVES_NAME):
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
    sleeve_pick = pick_shirt_sleeve_faces(
        body,
        bm_src,
        shirt_slot=shirt_slot,
        hand_slot=5,
        lm=lm,
        hc=hc,
        fwd=fwd,
        ws=ws,
    )
    bm_src.free()
    parts: list[bpy.types.Object] = []
    if len(skirt_pick) < 12:
        raise RuntimeError(f"FAIL staff-researcher-idle skirt_faces={len(skirt_pick)} too_few")
    if len(sleeve_pick) < 16:
        raise RuntimeError(f"FAIL staff-researcher-idle sleeve_faces={len(sleeve_pick)} too_few")
    bm_src = bmesh.new()
    bm_src.from_mesh(body.data)
    bm_src.faces.ensure_lookup_table()
    skirt, _ = _mesh_from_face_pick(body, bm_src, skirt_pick, SKIRT_NAME)
    sleeves, _ = _mesh_from_face_pick(body, bm_src, sleeve_pick, SLEEVES_NAME)
    bm_src.free()
    lo, span, skirt_top_z, z_hip, z_mid_thigh, _zk = _skirt_z_band(body, lm)
    z_knee = knee_world_z(body, lm)
    arm = next((o for o in bpy.context.scene.objects if o.type == "ARMATURE"), None)
    z_neck = _neck_cut_world_z(arm, lm)
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
    sleeves.data.materials.clear()
    sleeves.data.materials.append(coat_mat)
    _sanitize_coat_part(
        sleeves,
        weight_body,
        allow=SLEEVE_ALLOWED_VG,
        z_knee=z_knee,
        project_shell=False,
        cull_below_knee=False,
    )
    parts.extend([skirt, sleeves])
    return parts