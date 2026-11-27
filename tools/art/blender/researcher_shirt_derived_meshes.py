"""Duplicate founder shirt-slot mesh for coat skirt + sleeves (proven skinning)."""
from __future__ import annotations

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc
from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward
from kenney_geometry_landmarks import compute_landmarks, torso_half_width
from researcher_hero_geometry import _bmesh_faces_from_indices, _copy_body_vertex_groups

SKIRT_NAME = "ResearcherCoatSkirt"
SLEEVES_NAME = "ResearcherCoatSleeves"

SKIRT_ALLOWED_VG = frozenset(
    {
        "Hips",
        "Spine",
        "Chest",
        "UpperChest",
        "LeftUpLeg",
        "RightUpLeg",
        "LeftLeg",
        "RightLeg",
    }
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


def _face_world_centroid(body: bpy.types.Object, face: bmesh.types.BMFace) -> Vector:
    wc = Vector((0.0, 0.0, 0.0))
    for v in face.verts:
        wc += body.matrix_world @ v.co
    return wc / len(face.verts)


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


def pick_shirt_skirt_faces(
    body: bpy.types.Object,
    bm_src: bmesh.types.BMesh,
    *,
    shirt_slot: int,
    lm,
    hc: Vector,
    fwd: Vector,
    ws: list[Vector],
) -> set[int]:
    lo, span = lm.zmin, lm.span
    pick: set[int] = set()
    for f in bm_src.faces:
        if f.material_index != shirt_slot:
            continue
        wc = _face_world_centroid(body, f)
        h = (wc.z - lo) / span
        if h < lm.h_waist - 0.20 or h > lm.h_waist + 0.52:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat > thw * 1.42:
            continue
        pick.add(f.index)
    return pick


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
    pick: set[int] = set()
    for f in bm_src.faces:
        if f.material_index not in (shirt_slot, hand_slot):
            continue
        wc = _face_world_centroid(body, f)
        h = (wc.z - lo) / span
        if h < lm.h_neck - 0.22 or h > lm.h_waist + 0.14:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat <= thw * 0.30:
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
            if lm.h_neck - 0.22 <= h <= lm.h_waist + 0.14:
                pick.add(f.index)
                changed = True
    return pick


def _attach_coat_part_to_body_or_arm(
    ob: bpy.types.Object,
    body: bpy.types.Object,
    arm: bpy.types.Object | None,
) -> None:
    if arm is not None:
        if ob.parent:
            world = ob.matrix_world.copy()
            ob.parent = None
            ob.matrix_world = world
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
) -> list[bpy.types.Object]:
    for name in (SKIRT_NAME, SLEEVES_NAME):
        for ob in list(bpy.context.scene.objects):
            if ob.type == "MESH" and ob.name.split(".")[0] == name:
                bpy.data.objects.remove(ob, do_unlink=True)
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    bm_src = bmesh.new()
    bm_src.from_mesh(body.data)
    bm_src.faces.ensure_lookup_table()
    skirt_pick = pick_shirt_skirt_faces(
        body, bm_src, shirt_slot=shirt_slot, lm=lm, hc=hc, fwd=fwd, ws=ws
    )
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
    for ob in (skirt, sleeves):
        ob.data.materials.clear()
        ob.data.materials.append(coat_mat)
        allow = SKIRT_ALLOWED_VG if ob.name.startswith(SKIRT_NAME) else SLEEVE_ALLOWED_VG
        _purge_vertex_groups(ob, allow=allow, deny=VG_DENY_HEAD)
        _repair_weights_from_nearest_body(ob, body, allow=allow)
        _normalize_vertex_groups(ob)
        parts.append(ob)
    return parts