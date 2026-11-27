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
        if h < lm.h_waist - 0.08 or h > lm.h_waist + 0.40:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat > thw * 1.25:
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
        if h < lm.h_neck - 0.18 or h > lm.h_waist + 0.06:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat <= thw * 0.36:
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
            if lm.h_neck - 0.18 <= h <= lm.h_waist + 0.06:
                pick.add(f.index)
                changed = True
    return pick


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
        _purge_vertex_groups(ob, allow=SKIRT_ALLOWED_VG if ob.name.startswith(SKIRT_NAME) else SLEEVE_ALLOWED_VG, deny=VG_DENY_HEAD)
        ob.parent = body
        ob.matrix_parent_inverse = body.matrix_world.inverted()
        parts.append(ob)
    return parts