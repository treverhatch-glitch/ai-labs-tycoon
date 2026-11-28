"""Researcher hero from founder stock mesh (same bake/rig); no slot-patch pipeline."""
from __future__ import annotations

import json

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc
from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward
from kenney_founder_stock_recolor import (
    ARM_VG_BONES,
    SLOT_HAIR,
    SLOT_HAND,
    SLOT_JEANS,
    SLOT_SHIRT,
    SLOT_SKIN,
    STOCK_BLEND,
    STOCK_META,
    STOCK_OBJECT_NAME as FOUNDER_STOCK_NAME,
    _head_width_at,
    _apply_arm_vertex_groups,
    _apply_hand_skin_vertex_groups,
    _ensure_material,
    _grow_arm_from_shoulders,
    _grow_arm_regions,
    _region_for_point,
    _strip_mesh_vertex_colors,
)
from kenney_geometry_landmarks import compute_landmarks, torso_half_width
from kenney_researcher_stock_recolor import (
    RESEARCHER_HAIR,
    RESEARCHER_SOLE,
    RESEARCHER_TROUSERS,
    _vg_mean_on_face,
    spawn_researcher_eye_meshes,
)
from researcher_hero_geometry import spawn_researcher_hair_cap
from researcher_shirt_derived_meshes import (
    SKIRT_NAME,
    SLEEVES_NAME,
    _skirt_z_band,
)

STOCK_OBJECT_NAME = "KenneyResearcherStock"


def swap_body_mesh_from_founder_stock(mesh: bpy.types.Object) -> None:
    """Replace Kenney FBX body with founder stock topology (matches bake GLB)."""
    if not STOCK_BLEND.is_file():
        raise RuntimeError(f"FAIL staff-researcher-idle missing stock blend {STOCK_BLEND}")
    with bpy.data.libraries.load(str(STOCK_BLEND), link=False) as (data_from, data_to):
        if FOUNDER_STOCK_NAME not in data_from.objects:
            raise RuntimeError("FAIL staff-researcher-idle stock_blend missing KenneyFounderStock")
        data_to.meshes = list(data_from.meshes)
        data_to.objects = [FOUNDER_STOCK_NAME]
    stock_ob = bpy.data.objects.get(FOUNDER_STOCK_NAME)
    if stock_ob is None:
        stock_ob = next(
            (o for o in bpy.data.objects if o.name.split(".")[0] == FOUNDER_STOCK_NAME),
            None,
        )
    if stock_ob is None:
        raise RuntimeError("FAIL staff-researcher-idle stock_blend import failed")
    mesh.data = stock_ob.data
    mesh.data.name = f"{STOCK_OBJECT_NAME}Mesh"
    bpy.data.objects.remove(stock_ob, do_unlink=True)
    mesh.name = STOCK_OBJECT_NAME

# Match founder slot indices; slot 3 is lab coat (was shirt).
SLOT_SHOE = 1
RESEARCHER_LAB_COAT_RGBA = (0.99, 0.97, 0.94, 1.0)
RESEARCHER_SKIN = (0.62, 0.52, 0.44, 1.0)


def _finalize_researcher_coat_slots(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Full white torso + long sleeves; open lapel (no chin tube); hands stay skin."""
    lo, span = lm.zmin, lm.span
    wrist_h = lm.h_shoe + 0.22
    coat_top = lm.h_neck - 0.10
    _, span, _, z_hip, z_mid_thigh, _zk = _skirt_z_band(ob, lm)
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        y_face = y_face_fn(wc)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)

        if coat_top <= h <= lm.h_waist + 0.06 and lat <= thw * 1.28:
            throat = (
                y_face > 0.10
                and lat < thw * 0.20
                and lm.h_neck - 0.03 <= h <= lm.h_neck + 0.06
            )
            if not throat:
                slots[face.index] = SLOT_SHIRT

        if z_hip - span * 0.04 <= wc.z <= z_mid_thigh + span * 0.05 and lat <= thw * 1.58:
            if slots[face.index] in (SLOT_JEANS, SLOT_SKIN):
                slots[face.index] = SLOT_SHIRT

        if coat_top <= h <= lm.h_waist + 0.12 and lat <= thw * 1.35:
            throat = y_face > 0.10 and lat < thw * 0.20 and lm.h_neck - 0.03 <= h <= lm.h_neck + 0.05
            if not throat and slots[face.index] in (SLOT_JEANS, SLOT_SKIN):
                slots[face.index] = SLOT_SHIRT

        if (
            lm.h_waist - 0.04 <= h <= lm.h_waist + 0.20
            and y_face > 0.02
            and lat <= thw * 1.48
        ):
            if slots[face.index] in (SLOT_JEANS, SLOT_SKIN):
                slots[face.index] = SLOT_SHIRT

        if lat > thw * 0.76 and coat_top <= h <= wrist_h + 0.05:
            if h >= wrist_h - 0.04 and slots[face.index] == SLOT_HAND:
                continue
            if slots[face.index] in (SLOT_SKIN, SLOT_HAND, SLOT_JEANS):
                slots[face.index] = SLOT_SHIRT
            elif lat > thw * 0.82:
                slots[face.index] = SLOT_SHIRT

        if h > lm.h_neck + 0.06 and lat < thw * 0.95 and abs(y_face) < 0.12:
            if slots[face.index] == SLOT_SHIRT:
                slots[face.index] = SLOT_SKIN

        lapel = (
            coat_top <= h <= lm.h_neck + 0.05
            and thw * 0.24 <= lat <= thw * 0.44
            and y_face > 0.08
        )
        if lapel:
            slots[face.index] = SLOT_SHIRT

        if h > lm.h_neck - 0.02 and lat < thw * 0.62 and y_face > -0.02:
            if slots[face.index] == SLOT_SHIRT:
                slots[face.index] = SLOT_HAIR if h > lm.h_neck + 0.10 else SLOT_SKIN


def _demote_body_collar_shirt_faces(body: bpy.types.Object, lm) -> int:
    """FAIL #28: white collar shards — shirt slot above neck → skin."""
    lo, span = lm.zmin, lm.span
    chin_h = lo + span * (lm.h_neck - 0.06)
    bm = bmesh.new()
    bm.from_mesh(body.data)
    bm.faces.ensure_lookup_table()
    changed = 0
    for face in bm.faces:
        if face.material_index != SLOT_SHIRT:
            continue
        wc = Vector()
        for v in face.verts:
            wc += body.matrix_world @ v.co
        wc /= len(face.verts)
        if wc.z > chin_h:
            face.material_index = SLOT_SKIN
            changed += 1
    if changed:
        bm.to_mesh(body.data)
        body.data.update()
    bm.free()
    return changed


def _strip_body_arm_shirt_to_skin(body: bpy.types.Object, lm) -> int:
    """No kit sleeve faces on body — procedural coat covers arms."""
    lo, span = lm.zmin, lm.span
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    bm = bmesh.new()
    bm.from_mesh(body.data)
    bm.faces.ensure_lookup_table()
    changed = 0
    for face in bm.faces:
        if face.material_index != SLOT_SHIRT:
            continue
        wc = Vector()
        for v in face.verts:
            wc += body.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat > thw * 0.72 and lm.h_waist - 0.06 <= h <= lm.h_neck + 0.06:
            face.material_index = SLOT_SKIN
            changed += 1
    if changed:
        bm.to_mesh(body.data)
        body.data.update()
    bm.free()
    return changed


def _tune_coat_mat(ob: bpy.types.Object) -> None:
    for slot in ob.material_slots:
        mat = slot.material
        if not mat or not mat.name.startswith("ResearcherLabCoat"):
            continue
        if not mat.use_nodes:
            continue
        bsdf = mat.node_tree.nodes.get("Principled BSDF")
        if not bsdf:
            continue
        bsdf.inputs["Base Color"].default_value = RESEARCHER_LAB_COAT_RGBA
        bsdf.inputs["Roughness"].default_value = 0.34
        em = bsdf.inputs.get("Emission") or bsdf.inputs.get("Emission Color")
        stren = bsdf.inputs.get("Emission Strength")
        if em is not None:
            em.default_value = (0.94, 0.93, 0.90, 1.0)
        if stren is not None:
            stren.default_value = 0.42


def apply_researcher_founder_base_materials(
    ob: bpy.types.Object,
    *,
    world_verts: list | None = None,
    preserve_armature_weights: bool = False,
) -> None:
    """Founder topology + landmark slots; shirt→white coat, jeans→charcoal."""
    _strip_mesh_vertex_colors(ob.data)
    if world_verts is not None:
        ws = world_verts
    else:
        deps = bpy.context.evaluated_depsgraph_get()
        ev = ob.evaluated_get(deps)
        ws = [ev.matrix_world @ v.co for v in ev.data.vertices]
    lm = compute_landmarks(ob)
    fwd = toe_forward([ob])
    hc, y_face_fn = face_shell_coord(ob, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)

    mats = [
        _ensure_material("ResearcherSole", RESEARCHER_SOLE, 0.72),
        _ensure_material("ResearcherShoe", RESEARCHER_SOLE, 0.42),
        _ensure_material("ResearcherTrousers", RESEARCHER_TROUSERS, 0.58),
        _ensure_material("ResearcherLabCoat", RESEARCHER_LAB_COAT_RGBA, 0.38),
        _ensure_material("ResearcherSkin", RESEARCHER_SKIN, 0.50),
        _ensure_material("ResearcherHand", RESEARCHER_SKIN, 0.48),
        _ensure_material("ResearcherHair", RESEARCHER_HAIR, 0.48),
    ]
    ob.data.materials.clear()
    for m in mats:
        ob.data.materials.append(m)

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    slots: list[int] = []
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        y_face = y_face_fn(wc)
        lat = lateral_from_front(wc, hc, fwd)
        h = (wc.z - lm.zmin) / lm.span
        thw = torso_half_width(ws, lm.zmin, lm.span, h, lm.cx, lm.cy)
        slots.append(
            _region_for_point(
                wc,
                lm=lm,
                all_ws=ws,
                y_face=y_face,
                lat=lat,
                torso_hw=thw,
                head_w=head_w,
            )
        )
    if not preserve_armature_weights:
        _apply_arm_vertex_groups(bm, slots, ob, lm, ws, hc, fwd, force_override=True)
        _grow_arm_from_shoulders(bm, slots, ob, lm, ws, hc, fwd)
        _grow_arm_regions(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
        _apply_hand_skin_vertex_groups(bm, slots, ob)
    _finalize_researcher_coat_slots(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    for face, slot in zip(bm.faces, slots, strict=True):
        face.material_index = slot
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()
    _tune_coat_mat(ob)
    bc.smooth_shade(ob)


def spawn_researcher_coat_skirt(body: bpy.types.Object, arm: bpy.types.Object | None = None) -> bpy.types.Object:
    coat_mat = _ensure_material("ResearcherCoatSkirtMat", RESEARCHER_LAB_COAT_RGBA, 0.36)
    from researcher_procedural_lab_coat import spawn_procedural_lab_coat

    return spawn_procedural_lab_coat(body, arm, coat_mat)[0]


def spawn_researcher_shirt_derived_parts(
    body: bpy.types.Object,
    arm: bpy.types.Object | None = None,
    *,
    weight_body: bpy.types.Object | None = None,
) -> list[bpy.types.Object]:
    del weight_body
    coat_mat = _ensure_material("ResearcherCoatSkirtMat", RESEARCHER_LAB_COAT_RGBA, 0.36)
    from researcher_procedural_lab_coat import spawn_procedural_lab_coat

    return spawn_procedural_lab_coat(body, arm, coat_mat)


def collect_researcher_founder_meshes() -> list[bpy.types.Object]:
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    body = next(
        (o for o in meshes if o.name.split(".")[0] == STOCK_OBJECT_NAME),
        None,
    )
    if not body:
        body = meshes[0]
    eyes = sorted(
        (o for o in meshes if o.name.split(".")[0].startswith("FounderEye_")),
        key=lambda o: o.name,
    )
    props = sorted(
        (
            o
            for o in meshes
            if o.name.split(".")[0] in (SKIRT_NAME, SLEEVES_NAME, "ResearcherHairCap")
        ),
        key=lambda o: o.name,
    )
    return [body, *eyes, *props]


TROUSER_GREY_NAME = "trouser_grey"
TROUSER_GREY_RGBA = (100 / 255, 100 / 255, 102 / 255, 1.0)
_THIGH_SHIN_VG = frozenset({"LeftUpLeg", "RightUpLeg", "LeftLeg", "RightLeg"})
_FOOT_VG = frozenset({"LeftFoot", "RightFoot", "LeftToes", "RightToes"})
_LEG_DOMINANT_VG = _THIGH_SHIN_VG | _FOOT_VG
_ARM_VG = frozenset(ARM_VG_BONES)


def _ensure_trouser_grey_material(roughness: float = 0.58) -> bpy.types.Material:
    mat = bpy.data.materials.get(TROUSER_GREY_NAME)
    if mat is None:
        mat = bpy.data.materials.new(TROUSER_GREY_NAME)
    mat.use_nodes = True
    nt = mat.node_tree
    for n in list(nt.nodes):
        nt.nodes.remove(n)
    out = nt.nodes.new("ShaderNodeOutputMaterial")
    bsdf = nt.nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Base Color"].default_value = TROUSER_GREY_RGBA
    bsdf.inputs["Roughness"].default_value = roughness
    spec = bsdf.inputs.get("Specular") or bsdf.inputs.get("Specular IOR Level")
    if spec is not None:
        spec.default_value = 0.35
    nt.links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    return mat


def _coat_hem_normalized(
    body: bpy.types.Object, meshes: list[bpy.types.Object], lm
) -> float:
    skirt = next(
        (o for o in meshes if o.name.split(".")[0] == "ResearcherCoatSkirt"),
        None,
    )
    if skirt is not None:
        zs = [(skirt.matrix_world @ v.co).z for v in skirt.data.vertices]
        if zs:
            hem_z = min(zs)
            return (hem_z - lm.zmin) / max(1e-6, lm.span)
    hem = lm.h_waist + 0.50
    if hem < lm.h_shoe + 0.12:
        hem = max(lm.h_waist - 0.15, lm.h_shoe + 0.12)
    return hem


def _face_vg_weight_map(
    ob: bpy.types.Object, face: bmesh.types.BMFace
) -> dict[str, float]:
    vg_names = {g.index: g.name for g in ob.vertex_groups}
    acc: dict[str, float] = {}
    n = max(1, len(face.verts))
    for v in face.verts:
        for g in ob.data.vertices[v.index].groups:
            name = vg_names.get(g.group, "")
            if name:
                acc[name] = acc.get(name, 0.0) + g.weight / n
    return acc


def _face_dominant_leg_below_hem(
    ob: bpy.types.Object, face: bmesh.types.BMFace, *, arm_cap: float = 0.14
) -> bool:
    weights = _face_vg_weight_map(ob, face)
    if not weights:
        return False
    arm_w = max((weights.get(b, 0.0) for b in _ARM_VG), default=0.0)
    hand_w = _vg_mean_on_face(ob, face, _ARM_VG)
    if arm_w > arm_cap or hand_w > arm_cap:
        return False
    dominant = max(weights, key=weights.get)
    if dominant in _ARM_VG:
        return False
    if dominant in _LEG_DOMINANT_VG and weights[dominant] >= 0.12:
        return True
    leg_best = max((weights.get(b, 0.0) for b in _LEG_DOMINANT_VG), default=0.0)
    return leg_best >= 0.12 and leg_best > arm_w + 0.02


def apply_trouser_grey_leg_faces() -> int:
    """Below coat hem: trouser_grey on faces whose dominant VG is leg (not arm/hand)."""
    meshes = collect_researcher_founder_meshes()
    body = next(
        o
        for o in meshes
        if o.type == "MESH" and o.name.split(".")[0] == STOCK_OBJECT_NAME
    )
    trouser_mat = _ensure_trouser_grey_material()
    lm = compute_landmarks(body)
    lo, span = lm.zmin, lm.span
    hem_h = _coat_hem_normalized(body, meshes, lm)
    h_lo = lm.h_shoe - 0.058
    slot_index = None
    for i, slot in enumerate(body.data.materials):
        if slot and slot.name == TROUSER_GREY_NAME:
            slot_index = i
            break
    if slot_index is None:
        slot_index = len(body.data.materials)
        body.data.materials.append(trouser_mat)

    bm = bmesh.new()
    bm.from_mesh(body.data)
    bm.faces.ensure_lookup_table()
    changed = 0
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += body.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h >= hem_h or h < h_lo:
            continue
        if not _face_dominant_leg_below_hem(body, face):
            continue
        face.material_index = slot_index
        changed += 1
    bm.to_mesh(body.data)
    bm.free()
    body.data.update()
    return changed


def load_researcher_from_founder_stock() -> bpy.types.Object:
    if not STOCK_BLEND.is_file() or not STOCK_META.is_file():
        raise RuntimeError(f"FAIL staff-researcher-idle missing founder stock {STOCK_BLEND}")
    meta = json.loads(STOCK_META.read_text(encoding="utf-8"))
    want_verts = int(meta["vertexCount"])
    want_tris = int(meta["triangleCount"])
    bpy.ops.wm.open_mainfile(filepath=str(STOCK_BLEND))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if len(meshes) != 1:
        raise RuntimeError(f"FAIL staff-researcher-idle founder_stock parts={len(meshes)} want=1")
    ob = meshes[0]
    if ob.name.split(".")[0] == FOUNDER_STOCK_NAME:
        ob.name = STOCK_OBJECT_NAME
    if len(ob.data.vertices) != want_verts:
        raise RuntimeError(
            f"FAIL staff-researcher-idle stock_verts={len(ob.data.vertices)} want={want_verts}"
        )
    tri_count = sum(len(p.vertices) for p in ob.data.polygons)
    if tri_count != want_tris:
        raise RuntimeError(
            f"FAIL staff-researcher-idle stock_tris={tri_count} want={want_tris}"
        )
    apply_researcher_founder_base_materials(ob)
    lm = compute_landmarks(ob)
    _demote_body_collar_shirt_faces(ob, lm)
    _strip_body_arm_shirt_to_skin(ob, lm)
    arm = next((o for o in bpy.context.scene.objects if o.type == "ARMATURE"), None)
    spawn_researcher_shirt_derived_parts(ob, arm=arm)
    spawn_researcher_eye_meshes(ob)
    spawn_researcher_hair_cap(ob)
    from researcher_scene_mesh_audit import (
        audit_researcher_render_meshes,
        reset_render_island_audit_log,
    )

    reset_render_island_audit_log()
    meshes = collect_researcher_founder_meshes()
    audit_researcher_render_meshes(ob, meshes, prune=False)
    n_trouser = apply_trouser_grey_leg_faces()
    print(f"OK researcher trouser_grey faces={n_trouser}")
    return ob