"""Researcher hero: canonical Kenney stock mesh; lab-coat material slots (no mesh edits)."""
from __future__ import annotations

import json
import math
import os
from pathlib import Path

import bpy
import bmesh
from mathutils import Quaternion, Vector

import blender_common as bc
import kenney_founder_lib as kf
from kenney_founder_standing_gate import (
    rig_head_foot_world_z,
    verify_kenney_arm_drop,
    verify_standing_rig,
)
from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward
from kenney_geometry_landmarks import FounderLandmarks, compute_landmarks, torso_half_width

STOCK_SOURCE = bc.REPO_ROOT / "tools" / "art" / "source" / "kenney"
STOCK_BLEND = STOCK_SOURCE / "staff-founder-kenney-stock.blend"
STOCK_KIT_GLB = STOCK_SOURCE / "staff-founder-kenney-stock-kit.glb"
STOCK_KIT_SHA = STOCK_SOURCE / "staff-founder-kenney-stock-kit.glb.sha256"
STOCK_GLB = bc.ART_ROOT / "staff-kit" / "staff-founder-kenney-stock.glb"
STOCK_META = STOCK_BLEND.with_suffix(".stock.json")
STOCK_OBJECT_NAME = "KenneyResearcherStock"

RESEARCHER_SOLE = (0.28, 0.30, 0.34, 1.0)
RESEARCHER_SHOE = (0.28, 0.30, 0.34, 1.0)
RESEARCHER_TROUSERS = (58 / 255, 58 / 255, 58 / 255, 1.0)
RESEARCHER_TEE = (0.48, 0.68, 0.92, 1.0)
RESEARCHER_LAB_COAT = (0.50, 0.52, 0.50, 1.0)
RESEARCHER_SHIRT = (0.08, 0.68, 0.54, 1.0)
RESEARCHER_COLLAR = (228 / 255, 226 / 255, 220 / 255, 1.0)
RESEARCHER_LAPEL = (0.36, 0.38, 0.37, 1.0)
RESEARCHER_SKIN = (0.51, 0.41, 0.34, 1.0)
RESEARCHER_HAIR = (0.18, 0.12, 0.09, 1.0)
RESEARCHER_GLASSES = (34 / 255, 34 / 255, 34 / 255, 1.0)

FOUNDER_BROW = (0.20, 0.16, 0.14, 1.0)
FOUNDER_MOUTH = (0.18, 0.12, 0.12, 1.0)

SLOT_SOLE = 0
SLOT_SHOE = 1
SLOT_JEANS = 2
SLOT_SHIRT = 3
SLOT_SKIN = 4
SLOT_HAND = 5
SLOT_HAIR = 6
SLOT_COLLAR = 7
SLOT_EYE = 8
SLOT_BROW = 9
SLOT_MOUTH = 10
SLOT_LAPEL = 11
SLOT_LAB_COAT = 12

_VG_SHIRT_ARMS = frozenset(
    {
        "LeftShoulder",
        "RightShoulder",
        "LeftArm",
        "RightArm",
    }
)
_VG_FOREARM_SKIN = frozenset({"LeftForeArm", "RightForeArm"})
_VG_HANDS = frozenset(
    {
        "LeftHand",
        "RightHand",
        "LeftHandIndex1",
        "LeftHandIndex2",
        "LeftHandIndex3",
        "LeftHandThumb1",
        "LeftHandThumb2",
        "RightHandIndex1",
        "RightHandIndex2",
        "RightHandIndex3",
        "RightHandThumb1",
        "RightHandThumb2",
    }
)
_VG_SHOULDERS = frozenset({"LeftShoulder", "RightShoulder"})


def kit_glb_sha256() -> str:
    if STOCK_KIT_SHA.is_file():
        first = STOCK_KIT_SHA.read_text(encoding="utf-8").strip().split()[0]
        if len(first) == 64:
            return first
    if STOCK_KIT_GLB.is_file():
        return hashlib.sha256(STOCK_KIT_GLB.read_bytes()).hexdigest()
    raise RuntimeError("missing staff-founder-kenney-stock-kit.glb")


def install_stock_kit_glb() -> None:
    """Unity staff-kit GLB must match pinned Kenney kit bytes (not re-exported bake)."""
    want = kit_glb_sha256()
    STOCK_GLB.parent.mkdir(parents=True, exist_ok=True)
    if not STOCK_KIT_GLB.is_file():
        raise RuntimeError(f"FAIL missing kit glb {STOCK_KIT_GLB}")
    got = hashlib.sha256(STOCK_KIT_GLB.read_bytes()).hexdigest()
    if got != want:
        raise RuntimeError(f"FAIL kit glb sha256 got={got} want={want}")
    shutil.copyfile(STOCK_KIT_GLB, STOCK_GLB)
    out = hashlib.sha256(STOCK_GLB.read_bytes()).hexdigest()
    if out != want:
        raise RuntimeError("FAIL stock glb install mismatch")


def export_stock_glb(baked: bpy.types.Object) -> None:
    STOCK_SOURCE.mkdir(parents=True, exist_ok=True)
    baked.name = STOCK_OBJECT_NAME
    bc.assign_solid(baked, (0.58, 0.58, 0.58, 1.0), 0.62)
    bc.smooth_shade(baked)
    for other in list(bpy.context.scene.objects):
        if other != baked:
            bpy.data.objects.remove(other, do_unlink=True)
    bpy.ops.wm.save_as_mainfile(filepath=str(STOCK_BLEND))
    meta = {
        "vertexCount": len(baked.data.vertices),
        "triangleCount": sum(len(p.vertices) for p in baked.data.polygons),
        "objectName": STOCK_OBJECT_NAME,
        "kitGlbSha256": kit_glb_sha256(),
    }
    STOCK_META.write_text(json.dumps(meta, indent=2) + "\n", encoding="utf-8")
    install_stock_kit_glb()
    print(f"OK stock_kit_glb sha256={meta['kitGlbSha256']}")


def bisect_stock_landmarks(ob: bpy.types.Object) -> None:
    zs = [(ob.matrix_world @ v.co).z for v in ob.data.vertices]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    lm = compute_landmarks(ob)
    cuts = [
        lo + span * lm.h_shoe,
        lo + span * lm.h_waist,
        lo + span * lm.h_neck,
        lo + span * (lm.h_eye + 0.04),
    ]
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    geom = list(bm.verts) + list(bm.edges) + list(bm.faces)
    for z in cuts:
        bmesh.ops.bisect_plane(
            bm,
            geom=geom,
            plane_co=(0.0, 0.0, z),
            plane_no=(0.0, 0.0, 1.0),
            dist=1e-5,
            clear_inner=False,
            clear_outer=False,
        )
        geom = list(bm.verts) + list(bm.edges) + list(bm.faces)
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()


def _ensure_material(
    name: str,
    rgba: tuple[float, float, float, float],
    rough: float,
    *,
    specular: float = 0.12,
) -> bpy.types.Material:
    mat = bpy.data.materials.get(name)
    if mat:
        bpy.data.materials.remove(mat)
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = rgba
        bsdf.inputs["Roughness"].default_value = rough
        if "Specular IOR Level" in bsdf.inputs:
            bsdf.inputs["Specular IOR Level"].default_value = specular
        elif "Specular" in bsdf.inputs:
            bsdf.inputs["Specular"].default_value = specular
    return mat


def _head_width_at(ws: list[Vector], lo: float, span: float, h: float) -> float:
    pts = [p for p in ws if abs((p.z - lo) / span - h) < 0.015]
    return _xy_extent_static(pts, span)


def _xy_extent_static(pts: list[Vector], span: float) -> float:
    if len(pts) < 4:
        return span * 0.18
    xs = [p.x for p in pts]
    ys = [p.y for p in pts]
    return max(max(xs) - min(xs), max(ys) - min(ys))


def _region_for_point(
    w: Vector,
    *,
    lm: FounderLandmarks,
    all_ws: list[Vector],
    y_face: float,
    lat: float,
    torso_hw: float,
    head_w: float,
) -> int:
    lo, span = lm.zmin, lm.span
    h = (w.z - lo) / span
    lat_n = lat / span
    arm = lat > torso_hw + span * 0.012
    arm_hand_top = lm.h_waist + 0.25 * (lm.h_neck - lm.h_waist)

    if h < lm.h_shoe:
        return SLOT_SOLE if lat_n > 0.05 else SLOT_SHOE

    if h < lm.h_waist:
        if arm:
            return SLOT_HAND if h < lm.h_shoe + 0.12 else SLOT_SHIRT
        return SLOT_JEANS

    chin_lo = lm.h_neck - 0.08
    chin_hi = lm.h_neck + 0.03
    if chin_lo <= h <= chin_hi:
        collar = (
            y_face > 0.16
            and lat < torso_hw * 0.42
            and lm.h_neck - 0.005 <= h <= lm.h_neck + 0.018
        )
        if collar:
            return SLOT_COLLAR
        return SLOT_SKIN

    if h > lm.h_neck:
        if lm.h_eye - 0.02 <= h <= lm.h_eye + 0.06 and y_face > -0.08:
            return SLOT_SKIN
        head_pts = [p for p in all_ws if (p.z - lo) / span > lm.h_neck]
        head_top = max((p.z - lo) / span for p in head_pts) if head_pts else 0.97
        h_hairline = lm.h_neck + 0.88 * (head_top - lm.h_neck)
        if h >= h_hairline and y_face <= 0.06:
            return SLOT_HAIR
        if h > lm.h_neck + 0.12 and y_face < -0.12:
            return SLOT_HAIR
        return SLOT_SKIN

    if arm:
        if h < arm_hand_top:
            return SLOT_HAND
        return SLOT_SHIRT

    return SLOT_SHIRT


def check_founder_decal_islands(ob: bpy.types.Object) -> tuple[bool, str]:
    """One body island; face decals may be separate islands (2 eyes + brow/mouth)."""
    decal_only = {SLOT_EYE, SLOT_BROW, SLOT_MOUTH}
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    visited: set[int] = set()
    islands: list[set[int]] = []
    for face in bm.faces:
        if face.index in visited:
            continue
        slots: set[int] = set()
        stack = [face]
        while stack:
            f = stack.pop()
            if f.index in visited:
                continue
            visited.add(f.index)
            slots.add(f.material_index)
            for e in f.edges:
                for linked in e.link_faces:
                    if linked.index not in visited:
                        stack.append(linked)
        islands.append(slots)
    bm.free()
    if len(islands) == 1:
        return True, "islands=1"
    body = eye = 0
    for slots in islands:
        if slots <= {SLOT_EYE}:
            eye += 1
        elif slots <= decal_only:
            continue
        else:
            body += 1
    if body != 1:
        return False, f"body_islands={body} total={len(islands)}"
    if eye != 2:
        return False, f"eye_islands={eye} want=2"
    return True, f"islands={len(islands)} body=1 eyes=2"


def _strip_mesh_vertex_colors(mesh: bpy.types.Mesh) -> None:
    while mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[0])


def collect_researcher_stock_meshes() -> list[bpy.types.Object]:
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
            if o.name.split(".")[0]
            in ("ResearcherGlasses", "ResearcherHairCap")
        ),
        key=lambda o: o.name,
    )
    return [body, *eyes, *props]


def apply_researcher_materials(
    ob: bpy.types.Object, *, world_verts: list | None = None
) -> None:
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
        _ensure_material("ResearcherShoe", RESEARCHER_SHOE, 0.42),
        _ensure_material("ResearcherTrousers", RESEARCHER_TROUSERS, 0.58),
        _ensure_material("ResearcherTee", RESEARCHER_COLLAR, 0.42, specular=0.05),
        _ensure_material("ResearcherSkin", RESEARCHER_SKIN, 0.54, specular=0.05),
        _ensure_material("ResearcherHand", RESEARCHER_SKIN, 0.52, specular=0.06),
        _ensure_material("ResearcherHair", RESEARCHER_HAIR, 0.48),
        _ensure_material("ResearcherUndershirt", RESEARCHER_SKIN, 0.48, specular=0.05),
        _ensure_material("ResearcherDecalEye", RESEARCHER_SKIN, 0.5),
        _ensure_material("ResearcherDecalBrow", RESEARCHER_SKIN, 0.5),
        _ensure_material("ResearcherDecalMouth", RESEARCHER_SKIN, 0.5),
        _ensure_material("ResearcherLapel", RESEARCHER_COLLAR, 0.52),
        _ensure_material("ResearcherLabCoat", (0.98, 0.96, 0.92, 1.0), 0.38, specular=0.06),
    ]
    coat_mat = ob.data.materials[-1]
    if coat_mat:
        coat_mat.use_backface_culling = False
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
    _apply_arm_vertex_groups(bm, slots, ob, lm, ws, hc, fwd, force_override=True)
    _grow_arm_from_shoulders(bm, slots, ob, lm, ws, hc, fwd)
    _grow_arm_regions(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _apply_researcher_hand_skin_only(bm, slots, ob, lm)
    _stamp_researcher_tie(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _stamp_lab_coat_lapels(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _stamp_lab_coat_open_front(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _stamp_coat_knee_front(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _stamp_tee_short_sleeve(bm, slots, ob, lm, ws, hc, fwd)
    _fix_researcher_collar_cuff_bleed(bm, slots, ob, lm, ws, hc, fwd)
    _fix_researcher_chin_neck_bleed(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _fix_researcher_hands_skin_no_blue(bm, slots, ob, lm)
    _audit_researcher_slots_no_blue(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _force_researcher_arm_skin_slots(bm, slots, ob, lm, ws, hc, fwd)
    _stamp_lab_coat_vgroups(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _grow_lab_coat_mesh_slots(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _finalize_researcher_body_coat(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _enforce_trousers_below_coat_hem(bm, slots, ob, lm, ws, hc, fwd)
    _strip_coat_from_head_neck_hair(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _clamp_coat_hem_leg_trousers(bm, slots, ob, lm, ws, hc, fwd)
    _force_leg_trousers_below_hem(bm, slots, ob, lm)
    _widen_outer_leg_jeans(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _grow_jeans_front_leg_skin_shell(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _ban_torso_skin_shirt_band(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _force_head_vgroup_slots(bm, slots, ob, lm, y_face_fn)
    _purge_coat_slots_above_neck(bm, slots, ob, lm, y_face_fn)
    _force_upper_head_slots_by_height(bm, slots, ob, lm, y_face_fn)
    _ensure_minimum_coat_torso(bm, slots, ob, lm)
    _convert_torso_skin_shirt_to_coat(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _seal_torso_lab_coat_shell(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _seal_head_hair_not_skin_back(bm, slots, ob, lm, y_face_fn)
    _strip_body_armpit_coat_slivers(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _grow_jeans_front_leg_skin_shell(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _paint_front_leg_shell_jeans(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _shrink_front_leg_skin_under_trousers(bm, ob, lm, ws, hc, fwd, y_face_fn)
    _strip_lab_coat_from_front_leg_shell(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    lo, span = lm.zmin, lm.span
    for face in bm.faces:
        fi = face.index
        if slots[fi] != SLOT_LAB_COAT:
            continue
        hw = _vg_mean_on_face(ob, face, frozenset({"Head"}))
        if hw > 0.05:
            wc = Vector()
            for v in face.verts:
                wc += ob.matrix_world @ v.co
            wc /= len(face.verts)
            h = (wc.z - lo) / span
            y_face = y_face_fn(wc)
            if y_face > 0.12 and h < lm.h_neck + 0.10:
                slots[fi] = SLOT_SKIN
            else:
                slots[fi] = SLOT_HAIR
    for face, slot in zip(bm.faces, slots, strict=True):
        face.material_index = slot
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()
    _tune_researcher_material_albedos(ob)
    bc.smooth_shade(ob)


def _tune_researcher_material_albedos(ob: bpy.types.Object) -> None:
    """Slot albedos (not per-render overlay) tuned for staff preview exposure."""
    targets = {
        "ResearcherLabCoat": (1.0, 0.995, 0.99, 1.0),
        "ResearcherTrousers": (112 / 255, 112 / 255, 112 / 255, 1.0),
        "ResearcherHair": RESEARCHER_HAIR,
        "ResearcherHairCapMat": RESEARCHER_HAIR,
        "ResearcherSkin": RESEARCHER_SKIN,
        "ResearcherHand": RESEARCHER_SKIN,
        "ResearcherTee": RESEARCHER_COLLAR,
    }
    for slot in ob.material_slots:
        mat = slot.material
        if not mat:
            continue
        base = mat.name.split(".")[0]
        rgba = targets.get(base)
        if not rgba or not mat.use_nodes:
            continue
        bsdf = mat.node_tree.nodes.get("Principled BSDF")
        if bsdf:
            bsdf.inputs["Base Color"].default_value = rgba
            if base == "ResearcherLabCoat":
                bsdf.inputs["Roughness"].default_value = 0.32
                em = bsdf.inputs.get("Emission") or bsdf.inputs.get("Emission Color")
                if em is not None:
                    em.default_value = (0.95, 0.94, 0.92, 1.0)
                stren = bsdf.inputs.get("Emission Strength")
                if stren is not None:
                    stren.default_value = 0.42


_VG_LEGS = frozenset(
    {"LeftUpLeg", "RightUpLeg", "LeftLeg", "RightLeg", "LeftFoot", "RightFoot", "LeftToes", "RightToes"}
)

ARM_VG_BONES = (
    "LeftShoulder",
    "RightShoulder",
    "LeftArm",
    "RightArm",
    "LeftForeArm",
    "RightForeArm",
    "LeftHand",
    "RightHand",
    "LeftHandIndex1",
    "LeftHandIndex2",
    "LeftHandIndex3",
    "LeftHandThumb1",
    "LeftHandThumb2",
    "RightHandIndex1",
    "RightHandIndex2",
    "RightHandIndex3",
    "RightHandThumb1",
    "RightHandThumb2",
)


def _stamp_researcher_skin_under_coat(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Torso under lab-coat shell is skin; open front keeps teal V + lapels only."""
    lo, span = lm.zmin, lm.span
    for face in bm.faces:
        if slots[face.index] not in (SLOT_SHIRT, SLOT_COLLAR):
            continue
        if slots[face.index] == SLOT_COLLAR:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if h < lm.h_waist + 0.45 and lat < thw * 1.02:
            slots[face.index] = SLOT_HAND


def _stamp_researcher_shirt_torso_arms(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Light-blue shirt on chest/torso and upper arms (visible in open coat V)."""
    lo, span = lm.zmin, lm.span
    sleeve_hi = lm.h_waist + 0.06
    for face in bm.faces:
        if slots[face.index] in (SLOT_JEANS, SLOT_SHOE, SLOT_SOLE, SLOT_HAIR):
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < lm.h_neck + 0.02 or h > lm.h_waist + 0.26:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        on_torso = lat <= thw * 1.10 and y_face_fn(wc) > -0.18
        on_arm = lat > thw * 0.68 and lm.h_neck - 0.12 <= h <= lm.h_waist + 0.10
        if on_torso or on_arm:
            slots[face.index] = SLOT_SHIRT


def _stamp_tee_short_sleeve(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
) -> None:
    """Wrists/hands only; forearms stay shirt white under lab-coat sleeve."""
    lo, span = lm.zmin, lm.span
    if not ob.vertex_groups:
        return
    vg_names = {g.index: g.name for g in ob.vertex_groups}
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat <= thw + span * 0.014:
            continue
        if h <= lm.h_waist - 0.11:
            continue
        hand = 0.0
        for v in face.verts:
            for g in ob.data.vertices[v.index].groups:
                if vg_names.get(g.group, "") in ("LeftHand", "RightHand"):
                    hand += g.weight
        hand /= max(1, len(face.verts))
        if hand < 0.22:
            continue
        if slots[face.index] == SLOT_SHIRT:
            slots[face.index] = SLOT_HAND


def _stamp_researcher_chest_v_shirt(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Light-blue shirt in open lab-coat front V (collar through chest)."""
    lo, span = lm.zmin, lm.span
    v_lo = lm.h_neck + 0.01
    v_hi = lm.h_waist - 0.02
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        y_face = y_face_fn(wc)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if y_face < 0.03 or h < v_lo or h > v_hi:
            continue
        t = (h - v_lo) / max(1e-4, v_hi - v_lo)
        if lat < thw * (0.36 - t * 0.16):
            slots[face.index] = SLOT_LAB_COAT


_VG_BODY_COAT = frozenset(
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
    }
)
_VG_TORSO_COAT = frozenset(
    {
        "Spine",
        "Chest",
        "UpperChest",
        "LeftShoulder",
        "RightShoulder",
    }
)


def _ban_torso_skin_shirt_band(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Torso column shoulder→mid-thigh: lab coat white only (narrow neck V may stay)."""
    lo, span = lm.zmin, lm.span
    for face in bm.faces:
        fi = face.index
        if slots[fi] not in (SLOT_SHIRT, SLOT_SKIN, SLOT_COLLAR, SLOT_LAPEL):
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < lm.h_neck + 0.06 or h > lm.h_waist + 0.46:
            continue
        y_face = y_face_fn(wc)
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if (
            slots[fi] == SLOT_COLLAR
            and y_face > 0.12
            and lat < thw * 0.12
            and lm.h_neck + 0.04 <= h <= lm.h_neck + 0.16
        ):
            continue
        if lat <= thw * 0.92:
            slots[fi] = SLOT_LAB_COAT


def _seal_torso_lab_coat_shell(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Front/back/sides: lab coat neck→mid-thigh; skin only face, neck V, hands."""
    lo, span = lm.zmin, lm.span
    skip = {SLOT_EYE, SLOT_BROW, SLOT_MOUTH, SLOT_SOLE, SLOT_SHOE}
    for face in bm.faces:
        fi = face.index
        if slots[fi] in skip:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < lm.h_neck + 0.08 or h > lm.h_waist + 0.55:
            continue
        if _vg_mean_on_face(ob, face, frozenset({"Head"})) > 0.08:
            continue
        hand_w = _vg_mean_on_face(ob, face, _VG_HANDS)
        if hand_w > 0.32:
            slots[fi] = SLOT_HAND
            continue
        y_face = y_face_fn(wc)
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if (
            y_face > 0.16
            and lat < thw * 0.055
            and lm.h_neck + 0.07 <= h <= lm.h_neck + 0.16
        ):
            slots[fi] = SLOT_SKIN
            continue
        if (
            y_face > -0.10
            and lm.h_neck - 0.05 <= h <= lm.h_waist - 0.05
            and thw * 0.76 <= lat <= thw * 1.48
        ):
            if slots[fi] == SLOT_LAB_COAT:
                slots[fi] = SLOT_SKIN
            continue
        if slots[fi] in (SLOT_SHIRT, SLOT_COLLAR, SLOT_LAPEL) and h > lm.h_neck + 0.14:
            slots[fi] = SLOT_LAB_COAT
            continue
        slots[fi] = SLOT_LAB_COAT


def _seal_head_hair_not_skin_back(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    y_face_fn,
) -> None:
    """Crown/back/sides of head = hair only; skin limited to front face."""
    lo, span = lm.zmin, lm.span
    head_vg = frozenset({"Head"})
    for face in bm.faces:
        fi = face.index
        if slots[fi] in {SLOT_EYE, SLOT_BROW, SLOT_MOUTH}:
            continue
        if _vg_mean_on_face(ob, face, head_vg) < 0.03:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < lm.h_neck - 0.02:
            continue
        y_face = y_face_fn(wc)
        if y_face > 0.16 and h < lm.h_neck + 0.11:
            slots[fi] = SLOT_SKIN
        else:
            slots[fi] = SLOT_HAIR


def _convert_torso_skin_shirt_to_coat(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    lo, span = lm.zmin, lm.span
    for face in bm.faces:
        fi = face.index
        if slots[fi] not in (SLOT_SKIN, SLOT_SHIRT, SLOT_COLLAR, SLOT_LAPEL):
            continue
        if _vg_mean_on_face(ob, face, frozenset({"Head", "Neck"})) > 0.15:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        y_face = y_face_fn(wc)
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lm.h_neck + 0.08 <= h <= lm.h_waist + 0.48 and lat <= thw * 0.88:
            if (
                slots[fi] == SLOT_COLLAR
                and y_face > 0.10
                and lat < thw * 0.10
                and lm.h_neck + 0.04 <= h <= lm.h_neck + 0.17
            ):
                continue
            slots[fi] = SLOT_LAB_COAT


def _ensure_minimum_coat_torso(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
) -> None:
    lo, span = lm.zmin, lm.span
    torso_vg = frozenset({"Spine", "Chest", "UpperChest", "Hips"})
    for face in bm.faces:
        fi = face.index
        if _vg_mean_on_face(ob, face, torso_vg) < 0.18:
            continue
        if _vg_mean_on_face(ob, face, frozenset({"Head"})) > 0.1:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if lm.h_neck + 0.04 <= h <= lm.h_waist + 0.50:
            slots[fi] = SLOT_LAB_COAT


def _force_upper_head_slots_by_height(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    y_face_fn,
) -> None:
    """Strip coat/shirt from above shoulders; hair/side-back handled in _seal_head."""
    lo, span = lm.zmin, lm.span
    decals = {SLOT_EYE, SLOT_BROW, SLOT_MOUTH}
    bad = {SLOT_LAB_COAT, SLOT_SHIRT, SLOT_COLLAR, SLOT_LAPEL, SLOT_HAND}
    head_vg = frozenset({"Head"})
    for face in bm.faces:
        fi = face.index
        if slots[fi] in decals:
            continue
        if _vg_mean_on_face(ob, face, head_vg) < 0.05:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < lm.h_neck + 0.03:
            continue
        if slots[fi] in bad:
            y_face = y_face_fn(wc)
            slots[fi] = (
                SLOT_SKIN if y_face > 0.14 and h < lm.h_neck + 0.10 else SLOT_HAIR
            )


def _purge_coat_slots_above_neck(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    y_face_fn,
) -> None:
    """Remove lab-coat material only from head / upper-neck faces."""
    lo, span = lm.zmin, lm.span
    head_vg = frozenset({"Head"})
    for face in bm.faces:
        fi = face.index
        if slots[fi] != SLOT_LAB_COAT:
            continue
        head_w = _vg_mean_on_face(ob, face, head_vg)
        if head_w < 0.12:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < lm.h_neck - 0.02:
            continue
        y_face = y_face_fn(wc)
        if y_face > 0.14 and h < lm.h_neck + 0.11:
            slots[fi] = SLOT_SKIN
        else:
            slots[fi] = SLOT_HAIR


def _force_head_vgroup_slots(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    y_face_fn,
) -> None:
    """Head vgroup: hair on back/sides; skin on forward face only."""
    lo, span = lm.zmin, lm.span
    head_vg = frozenset({"Head"})
    for face in bm.faces:
        fi = face.index
        if _vg_mean_on_face(ob, face, head_vg) < 0.035:
            continue
        if slots[fi] in {SLOT_EYE, SLOT_BROW, SLOT_MOUTH}:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        y_face = y_face_fn(wc)
        if y_face > 0.16 and h < lm.h_neck + 0.12:
            slots[fi] = SLOT_SKIN
        else:
            slots[fi] = SLOT_HAIR


def _vg_mean_on_face(ob: bpy.types.Object, face: bmesh.types.BMFace, names: frozenset[str]) -> float:
    vg_names = {g.index: g.name for g in ob.vertex_groups}
    total = 0.0
    for v in face.verts:
        for g in ob.data.vertices[v.index].groups:
            if vg_names.get(g.group, "") in names:
                total += g.weight
    return total / max(1, len(face.verts))


def _stamp_lab_coat_vgroups(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Single-mesh lab coat: white slot wherever rig weight > 0.5 on coat bones."""
    lo, span = lm.zmin, lm.span
    coat_hi = lm.h_waist + 0.58
    for face in bm.faces:
        if _vg_mean_on_face(ob, face, _VG_BODY_COAT) < 0.28:
            continue
        if _vg_mean_on_face(ob, face, _VG_HANDS) > 0.22:
            continue
        if _vg_mean_on_face(ob, face, frozenset({"Head", "Neck"})) > 0.18:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < lm.h_neck + 0.02 or h > coat_hi:
            continue
        if _vg_mean_on_face(ob, face, frozenset({"Head"})) > 0.10:
            continue
        y_face = y_face_fn(wc)
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if y_face > 0.06 and lat < thw * 0.28 and lm.h_neck <= h <= lm.h_waist - 0.05:
            continue
        if (
            y_face > -0.12
            and lm.h_neck - 0.04 <= h <= lm.h_waist - 0.06
            and thw * 0.82 <= lat <= thw * 1.38
        ):
            continue
        slots[face.index] = SLOT_LAB_COAT


def _finalize_researcher_body_coat(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Continuous lab coat: torso neck base → mid-thigh, front+back; narrow open V only."""
    lo, span = lm.zmin, lm.span
    coat_hem = lm.h_waist + 0.50
    coat_top = lm.h_neck + 0.03
    block = {SLOT_HAIR, SLOT_EYE, SLOT_BROW, SLOT_MOUTH, SLOT_SOLE, SLOT_SHOE}
    for face in bm.faces:
        fi = face.index
        if slots[fi] in block:
            continue
        if _vg_mean_on_face(ob, face, frozenset({"Head"})) > 0.10:
            continue
        if _vg_mean_on_face(ob, face, frozenset({"Neck"})) > 0.38:
            continue
        if _vg_mean_on_face(ob, face, _VG_HANDS) > 0.20:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < coat_top or h > coat_hem:
            continue
        y_face = y_face_fn(wc)
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        arm_v = _vg_mean_on_face(
            ob,
            face,
            frozenset(
                {
                    "LeftArm",
                    "RightArm",
                    "LeftForeArm",
                    "RightForeArm",
                    "LeftShoulder",
                    "RightShoulder",
                }
            ),
        )
        if (
            y_face > -0.12
            and lm.h_neck - 0.04 <= h <= lm.h_waist - 0.06
            and thw * 0.82 <= lat <= thw * 1.38
        ):
            continue
        if lat > thw * 1.24:
            if arm_v < 0.14 or h > lm.h_waist + 0.04:
                continue
        slots[fi] = SLOT_LAB_COAT


def _enforce_trousers_below_coat_hem(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
) -> None:
    """Deprecated no-op: converting jeans→coat caused white legs / torn hem."""
    del bm, slots, ob, lm, ws, hc, fwd


def _strip_coat_from_head_neck_hair(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Never paint lab coat on head / hair; keep small neck skin band."""
    lo, span = lm.zmin, lm.span
    for face in bm.faces:
        fi = face.index
        if slots[fi] != SLOT_LAB_COAT:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        head_w = _vg_mean_on_face(ob, face, frozenset({"Head"}))
        neck_w = _vg_mean_on_face(ob, face, frozenset({"Neck"}))
        if head_w > 0.18:
            y_face = y_face_fn(wc)
            if y_face > 0.12 and h < lm.h_neck + 0.10:
                slots[fi] = SLOT_SKIN
            else:
                slots[fi] = SLOT_HAIR
            continue
        if neck_w > 0.42 and h > lm.h_neck + 0.01:
            slots[fi] = SLOT_SKIN


def _leg_shell_lateral_cap(thw: float, span: float, leg_vg_mean: float) -> float:
    """Leg silhouette extends past torso_half_width — widen cap for strong leg VGs."""
    base = thw + max(span * 0.062, thw * 0.34)
    if leg_vg_mean >= 0.22:
        base = max(base, thw + span * 0.088)
    return base


def _widen_outer_leg_jeans(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Front leg shell: jeans over skin strips on inner/outer edges (FAIL #34/#35)."""
    lo, span = lm.zmin, lm.span
    leg_vg = frozenset({"LeftUpLeg", "RightUpLeg", "LeftLeg", "RightLeg"})
    for face in bm.faces:
        fi = face.index
        if slots[fi] in (SLOT_LAB_COAT, SLOT_HAIR, SLOT_EYE, SLOT_HAND, SLOT_SHOE, SLOT_SOLE):
            continue
        vg_leg = _vg_mean_on_face(ob, face, leg_vg)
        if vg_leg < 0.08:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < lm.h_shoe - 0.02 or h > lm.h_waist + 0.04:
            continue
        y_face = y_face_fn(wc)
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if y_face < -0.10:
            continue
        if lat > _leg_shell_lateral_cap(thw, span, vg_leg):
            continue
        if slots[fi] not in (SLOT_SKIN, SLOT_SHIRT):
            continue
        if vg_leg >= 0.20 and y_face > -0.06:
            slots[fi] = SLOT_JEANS
            continue
        outer = lat > thw * 0.08
        inner_front = y_face > -0.02 and lat > thw * 0.01
        if outer or inner_front:
            slots[fi] = SLOT_JEANS


def _paint_front_leg_shell_jeans(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Last pass: front hemisphere leg VGs → charcoal trousers (FAIL #35)."""
    lo, span = lm.zmin, lm.span
    leg_vg = frozenset({"LeftUpLeg", "RightUpLeg", "LeftLeg", "RightLeg"})
    for face in bm.faces:
        fi = face.index
        if slots[fi] in (SLOT_LAB_COAT, SLOT_HAIR, SLOT_HAND, SLOT_SHOE, SLOT_SOLE):
            continue
        vg_leg = _vg_mean_on_face(ob, face, leg_vg)
        if vg_leg < 0.06:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < lm.h_shoe - 0.02 or h > lm.h_waist + 0.02:
            continue
        y_face = y_face_fn(wc)
        if y_face < -0.08:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat > _leg_shell_lateral_cap(thw, span, vg_leg):
            continue
        if slots[fi] in (SLOT_SKIN, SLOT_SHIRT):
            slots[fi] = SLOT_JEANS
        elif vg_leg >= 0.35 and y_face > -0.04 and slots[fi] == SLOT_SKIN:
            slots[fi] = SLOT_JEANS


def _strip_lab_coat_from_front_leg_shell(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """No white coat on leg VGs on front/side shell (FAIL #35 silhouette)."""
    lo, span = lm.zmin, lm.span
    leg_vg = frozenset({"LeftUpLeg", "RightUpLeg", "LeftLeg", "RightLeg"})
    for face in bm.faces:
        fi = face.index
        if slots[fi] != SLOT_LAB_COAT:
            continue
        vg_leg = _vg_mean_on_face(ob, face, leg_vg)
        if vg_leg < 0.14:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < lm.h_shoe - 0.02 or h > lm.h_waist + 0.04:
            continue
        if y_face_fn(wc) < -0.12:
            continue
        slots[fi] = SLOT_JEANS


def _shrink_front_leg_skin_under_trousers(
    bm: bmesh.types.BMesh,
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Pull front leg shell verts slightly medial so skin does not silhouette past jeans."""
    lo, span = lm.zmin, lm.span
    leg_vg = frozenset({"LeftUpLeg", "RightUpLeg", "LeftLeg", "RightLeg"})
    inv = ob.matrix_world.inverted()
    fwd_xy = Vector((fwd.x, fwd.y, 0.0))
    if fwd_xy.length > 1e-5:
        fwd_xy.normalize()
    vg_names = {g.index: g.name for g in ob.vertex_groups}
    for v in bm.verts:
        w = ob.matrix_world @ v.co
        h = (w.z - lo) / span
        if h < lm.h_shoe - 0.01 or h > lm.h_waist + 0.03:
            continue
        leg_w = 0.0
        for g in ob.data.vertices[v.index].groups:
            if vg_names.get(g.group, "") in leg_vg:
                leg_w = max(leg_w, g.weight)
        if leg_w < 0.28:
            continue
        if y_face_fn(w) < -0.04:
            continue
        lat = lateral_from_front(w, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat < thw * 0.06:
            continue
        rel = Vector((w.x - hc.x, w.y - hc.y, 0.0))
        along = fwd_xy * rel.dot(fwd_xy)
        lateral = rel - along
        if lateral.length < 1e-6:
            continue
        cap = _leg_shell_lateral_cap(thw, span, leg_w)
        if lat > cap * 0.98:
            shrink = (cap * 0.96) / max(lat, 1e-6)
            shrink = min(1.0, max(0.82, shrink))
        else:
            shrink = 0.88 if lat > thw * 0.20 else 0.94
        w2 = Vector((hc.x, hc.y, w.z)) + along + lateral * shrink
        v.co = inv @ w2


def _strip_body_armpit_coat_slivers(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Lab coat on body mesh under shoulder → shirt/skin, not white slivers (FAIL #35)."""
    lo, span = lm.zmin, lm.span
    for face in bm.faces:
        fi = face.index
        if slots[fi] != SLOT_LAB_COAT:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < lm.h_neck - 0.05 or h > lm.h_waist - 0.05:
            continue
        if y_face_fn(wc) < -0.14:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if thw * 0.76 <= lat <= thw * 1.48:
            slots[fi] = SLOT_SKIN


def _grow_jeans_front_leg_skin_shell(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """BFS jeans over leg skin strips on front/side shell (FAIL #35)."""
    from collections import deque

    lo, span = lm.zmin, lm.span
    leg_vg = frozenset({"LeftUpLeg", "RightUpLeg", "LeftLeg", "RightLeg"})
    adj: list[set[int]] = [set() for _ in slots]
    for f in bm.faces:
        fi = f.index
        for e in f.edges:
            for nb in e.link_faces:
                if nb.index != fi:
                    adj[fi].add(nb.index)
    q: deque[int] = deque(
        fi for fi, sl in enumerate(slots) if sl == SLOT_JEANS
    )
    seen = set(q)
    for _ in range(14):
        n = len(q)
        for _ in range(n):
            fi = q.popleft()
            for ni in adj[fi]:
                if ni in seen:
                    continue
                if slots[ni] not in (SLOT_SKIN, SLOT_SHIRT):
                    continue
                face = bm.faces[ni]
                vg_leg = _vg_mean_on_face(ob, face, leg_vg)
                if vg_leg < 0.05:
                    continue
                wc = Vector()
                for v in face.verts:
                    wc += ob.matrix_world @ v.co
                wc /= len(face.verts)
                h = (wc.z - lo) / span
                if h < lm.h_shoe - 0.02 or h > lm.h_waist + 0.03:
                    continue
                if y_face_fn(wc) < -0.12:
                    continue
                lat = lateral_from_front(wc, hc, fwd)
                thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
                if lat > _leg_shell_lateral_cap(thw, span, vg_leg):
                    continue
                slots[ni] = SLOT_JEANS
                seen.add(ni)
                q.append(ni)


def _force_leg_trousers_below_hem(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
) -> None:
    lo, span = lm.zmin, lm.span
    hem = lm.h_waist + 0.34
    leg_vg = frozenset({"LeftUpLeg", "RightUpLeg", "LeftLeg", "RightLeg"})
    for face in bm.faces:
        fi = face.index
        if _vg_mean_on_face(ob, face, leg_vg) < 0.12:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h >= hem:
            slots[fi] = SLOT_JEANS
        elif slots[fi] == SLOT_LAB_COAT and h >= hem - 0.06:
            slots[fi] = SLOT_JEANS


def _clamp_coat_hem_leg_trousers(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
) -> None:
    """Below coat hem: charcoal trousers, not white coat on legs."""
    lo, span = lm.zmin, lm.span
    hem = lm.h_waist + 0.48
    leg_vg = frozenset({"LeftUpLeg", "RightUpLeg", "LeftLeg", "RightLeg"})
    for face in bm.faces:
        fi = face.index
        if slots[fi] != SLOT_LAB_COAT:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h <= hem:
            continue
        if _vg_mean_on_face(ob, face, leg_vg) > 0.12:
            slots[fi] = SLOT_JEANS


def _grow_lab_coat_mesh_slots(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    lo, span = lm.zmin, lm.span
    coat_hi = lm.h_waist + 0.58
    block = frozenset(
        {
            SLOT_HAIR,
            SLOT_EYE,
            SLOT_BROW,
            SLOT_MOUTH,
            SLOT_HAND,
            SLOT_SOLE,
            SLOT_SHOE,
        }
    )
    for _ in range(6):
        grown = False
        for face in bm.faces:
            fi = face.index
            if slots[fi] == SLOT_LAB_COAT or slots[fi] in block:
                continue
            if _vg_mean_on_face(ob, face, _VG_BODY_COAT) < 0.18:
                continue
            if not any(slots[nb.index] == SLOT_LAB_COAT for e in face.edges for nb in e.link_faces):
                continue
            wc = Vector()
            for v in face.verts:
                wc += ob.matrix_world @ v.co
            wc /= len(face.verts)
            h = (wc.z - lo) / span
            if h < lm.h_neck - 0.10 or h > coat_hi:
                continue
            y_face = y_face_fn(wc)
            lat = lateral_from_front(wc, hc, fwd)
            thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
            if y_face > 0.06 and lat < thw * 0.28 and lm.h_neck <= h <= lm.h_waist - 0.05:
                continue
            slots[fi] = SLOT_LAB_COAT
            grown = True
        if not grown:
            break


def _stamp_forearm_shirt_under_coat(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
) -> None:
    """Sleeves: white lab coat except wrists (skin hands)."""
    if not ob.vertex_groups:
        return
    lo, span = lm.zmin, lm.span
    vg_names = {g.index: g.name for g in ob.vertex_groups}
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < lm.h_neck - 0.14 or h > lm.h_waist - 0.05:
            continue
        hand = 0.0
        arm = 0.0
        for v in face.verts:
            for g in ob.data.vertices[v.index].groups:
                name = vg_names.get(g.group, "")
                if name in ARM_VG_BONES:
                    arm += g.weight
                if name in ("LeftHand", "RightHand"):
                    hand += g.weight
        arm /= max(1, len(face.verts))
        hand /= max(1, len(face.verts))
        if arm < 0.35:
            continue
        slots[face.index] = SLOT_HAND if hand > 0.12 else SLOT_LAB_COAT


def _stamp_researcher_tie(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Dark tie strip in the shirt V (lapel material)."""
    lo, span = lm.zmin, lm.span
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        y_face = y_face_fn(wc)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if not (lm.h_neck + 0.01 <= h <= lm.h_waist - 0.14):
            continue
        if y_face < 0.12 or lat > thw * 0.11:
            continue
        slots[face.index] = SLOT_LAPEL


def _stamp_coat_knee_front(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Lab coat hem to knee on front/side panels (over trousers)."""
    lo, span = lm.zmin, lm.span
    coat_hi = lm.h_waist + 0.36
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        y_face = y_face_fn(wc)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if h < lm.h_waist - 0.02 or h > coat_hi or y_face < -0.10:
            continue
        if slots[face.index] != SLOT_JEANS:
            continue
        if y_face > -0.02 and lat < thw * 1.05:
            slots[face.index] = SLOT_LAB_COAT


def _stamp_lab_coat_lapels(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Open-front lapel strips (darker grey) beside teal V-neck."""
    lo, span = lm.zmin, lm.span
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        y_face = y_face_fn(wc)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if not (lm.h_neck <= h <= lm.h_waist - 0.06):
            continue
        if y_face < 0.10:
            continue
        lapel = thw * 0.20 <= lat <= thw * 0.38
        if lapel:
            slots[face.index] = SLOT_LAB_COAT


def _grow_researcher_hair_cap(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """TPH-style puff: grow hair slot across crown sides."""
    from collections import deque

    lo, span = lm.zmin, lm.span
    seeds = [
        f.index
        for f in bm.faces
        if slots[f.index] == SLOT_HAIR
    ]
    if not seeds:
        return
    seen = set(seeds)
    q: deque[int] = deque(seeds)
    grown = 0
    while q and grown < 280:
        fi = q.popleft()
        face = bm.faces[fi]
        for edge in face.edges:
            for other in edge.link_faces:
                oi = other.index
                if oi in seen:
                    continue
                wc = Vector()
                for v in other.verts:
                    wc += ob.matrix_world @ v.co
                wc /= len(other.verts)
                h = (wc.z - lo) / span
                if h < lm.h_neck + 0.02 or h > lm.h_neck + 0.22:
                    continue
                y_face = y_face_fn(wc)
                if slots[oi] in (SLOT_EYE, SLOT_BROW, SLOT_MOUTH) or slots[oi] == SLOT_SKIN and y_face > 0.22:
                    continue
                if slots[oi] == SLOT_SKIN and y_face > 0.18 and h < lm.h_neck + 0.12:
                    continue
                seen.add(oi)
                slots[oi] = SLOT_HAIR
                q.append(oi)
                grown += 1


def _stamp_lab_coat_open_front(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Lab coat torso front+back to mid-thigh (narrow front V left for finalize)."""
    lo, span = lm.zmin, lm.span
    coat_hi = lm.h_waist + 0.55
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        y_face = y_face_fn(wc)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if h < lm.h_neck - 0.06 or h > coat_hi:
            continue
        if slots[face.index] in (SLOT_HAND, SLOT_HAIR, SLOT_EYE, SLOT_BROW, SLOT_MOUTH):
            continue
        if y_face > 0.08 and lat < thw * 0.11:
            continue
        if lat > thw * 1.22:
            continue
        slots[face.index] = SLOT_LAB_COAT


def _apply_researcher_hand_skin_only(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
) -> None:
    """Hands only (forearms stay shirt under coat sleeve)."""
    if not ob.vertex_groups:
        return
    vg_names = {g.index: g.name for g in ob.vertex_groups}
    lo, span = lm.zmin, lm.span

    def hand_w(vi: int) -> float:
        w = 0.0
        for g in ob.data.vertices[vi].groups:
            if vg_names.get(g.group, "") in _VG_HANDS:
                w += g.weight
        return w

    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h > lm.h_waist - 0.11:
            continue
        if sum(hand_w(v.index) for v in face.verts) / len(face.verts) < 0.24:
            continue
        slots[face.index] = SLOT_HAND


def _apply_hand_skin_vertex_groups(
    bm: bmesh.types.BMesh, slots: list[int], ob: bpy.types.Object
) -> None:
    if not ob.vertex_groups:
        return
    vg_names = {g.index: g.name for g in ob.vertex_groups}

    def skin_w(vi: int) -> float:
        w = 0.0
        for g in ob.data.vertices[vi].groups:
            name = vg_names.get(g.group, "")
            if name in _VG_HANDS or name in _VG_FOREARM_SKIN:
                w += g.weight
        return w

    def shirt_w(vi: int) -> float:
        w = 0.0
        for g in ob.data.vertices[vi].groups:
            if vg_names.get(g.group, "") in _VG_SHIRT_ARMS:
                w += g.weight
        return w

    for face in bm.faces:
        skin = shirt = 0.0
        for v in face.verts:
            skin += skin_w(v.index)
            shirt += shirt_w(v.index)
        if skin >= 0.22 and skin >= shirt * 0.55:
            slots[face.index] = SLOT_HAND
        elif shirt >= 0.42 and shirt > skin * 0.95 and skin < 0.18:
            slots[face.index] = SLOT_SHIRT


def _apply_arm_vertex_groups(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    *,
    force_override: bool = False,
) -> None:
    """Sleeves/hands from Kenney armature vertex groups (no height/lateral bands)."""
    if not ob.vertex_groups:
        return
    vg_names = {g.index: g.name for g in ob.vertex_groups}
    wrist_h = lm.h_waist - 0.10
    lo, span = lm.zmin, lm.span

    def _vert_weights(vi: int) -> tuple[float, float, float, float]:
        hand = shirt = leg = fore = 0.0
        for g in ob.data.vertices[vi].groups:
            name = vg_names.get(g.group, "")
            if name in _VG_HANDS:
                hand += g.weight
            elif name in _VG_FOREARM_SKIN:
                fore += g.weight
            elif name in _VG_SHIRT_ARMS:
                shirt += g.weight
            elif name in _VG_LEGS:
                leg += g.weight
        return hand, shirt, leg, fore

    for face in bm.faces:
        hand = shirt = leg = fore = 0.0
        wc = Vector()
        for v in face.verts:
            hwt, swt, lwt, frt = _vert_weights(v.index)
            hand += hwt
            shirt += swt
            leg += lwt
            fore += frt
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h < lm.h_shoe + 0.02 or h > lm.h_neck + 0.06:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat <= thw + span * 0.01:
            continue
        skin_arm = hand + fore
        armish = skin_arm + shirt
        if armish < 0.10 and not force_override:
            continue
        if leg > armish and armish < 0.22:
            continue
        if fore >= 0.12 and lat > thw + span * 0.008:
            slots[face.index] = SLOT_SHIRT if hand < 0.22 else SLOT_HAND
            continue
        if hand >= 0.12 and lat > thw + span * 0.008:
            slots[face.index] = SLOT_HAND
            continue
        if shirt >= 0.14 and lat > thw + span * 0.008:
            slots[face.index] = SLOT_SHIRT
            continue
        if h < wrist_h and hand >= 0.14:
            slots[face.index] = SLOT_HAND
        elif h < wrist_h and fore + shirt >= 0.12:
            slots[face.index] = SLOT_SHIRT
        elif shirt >= hand * 0.3:
            slots[face.index] = SLOT_SHIRT


def _force_researcher_arm_skin_slots(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
) -> None:
    """Arm chain under procedural coat: skin/hand only (never tee shirt)."""
    if not ob.vertex_groups:
        return
    lo, span = lm.zmin, lm.span
    vg_names = {g.index: g.name for g in ob.vertex_groups}
    arm_bones = frozenset(ARM_VG_BONES)

    def arm_hand(vi: int) -> tuple[float, float]:
        arm = hand = 0.0
        for g in ob.data.vertices[vi].groups:
            name = vg_names.get(g.group, "")
            if name in arm_bones:
                arm += g.weight
            if name in _VG_HANDS:
                hand += g.weight
        return arm, hand

    for face in bm.faces:
        idx = face.index
        if slots[idx] == SLOT_LAB_COAT:
            continue
        if slots[idx] not in (SLOT_SHIRT, SLOT_JEANS):
            continue
        wc = Vector()
        arm = hand = 0.0
        for v in face.verts:
            a, hwt = arm_hand(v.index)
            arm += a
            hand += hwt
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        arm /= len(face.verts)
        hand /= len(face.verts)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if arm < 0.12 or lat <= thw + span * 0.008:
            continue
        if h < lm.h_neck - 0.26 or h > lm.h_waist - 0.02:
            continue
        slots[idx] = SLOT_HAND if hand >= 0.10 else SLOT_SKIN


def _audit_researcher_slots_no_blue(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """No teal shirt slot on face, jaw, or hands (visible under procedural coat)."""
    lo, span = lm.zmin, lm.span
    vg_names = {g.index: g.name for g in ob.vertex_groups} if ob.vertex_groups else {}

    def hand_w(vi: int) -> float:
        return sum(
            g.weight
            for g in ob.data.vertices[vi].groups
            if vg_names.get(g.group, "") in _VG_HANDS
        )

    keep_face = {SLOT_EYE, SLOT_BROW, SLOT_MOUTH, SLOT_HAIR, SLOT_COLLAR}
    for face in bm.faces:
        idx = face.index
        if slots[idx] != SLOT_SHIRT:
            continue
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        y_face = y_face_fn(wc)
        hw = hand_w(sum(v.index for v in face.verts) // len(face.verts))
        if h >= lm.h_neck - 0.20 and y_face > 0.02:
            if slots[idx] not in keep_face:
                slots[idx] = SLOT_SKIN
            continue
        if lm.h_neck - 0.12 <= h <= lm.h_neck + 0.07 and lat > thw * 0.30:
            if slots[idx] in (SLOT_SHIRT, SLOT_JEANS):
                slots[idx] = SLOT_SKIN
            continue
        if h < lm.h_waist + 0.02 and (hw >= 0.08 or lat > thw * 0.72):
            slots[idx] = SLOT_HAND if hw >= 0.08 else SLOT_SKIN


def _fix_researcher_hands_skin_no_blue(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
) -> None:
    if not ob.vertex_groups:
        return
    vg_names = {g.index: g.name for g in ob.vertex_groups}
    lo, span = lm.zmin, lm.span
    wrist_h = lm.h_waist - 0.14

    def hand_w(vi: int) -> float:
        return sum(
            g.weight
            for g in ob.data.vertices[vi].groups
            if vg_names.get(g.group, "") in _VG_HANDS
        )

    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        if h > wrist_h + 0.05:
            continue
        if sum(hand_w(v.index) for v in face.verts) / len(face.verts) < 0.14:
            continue
        if slots[face.index] in (SLOT_JEANS, SLOT_SHIRT):
            slots[face.index] = SLOT_HAND


def _fix_researcher_chin_neck_bleed(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    """Under-chin band: skin (or coat collar), not shirt blue or trousers."""
    lo, span = lm.zmin, lm.span
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        idx = face.index
        if h < lm.h_neck - 0.10 or h > lm.h_neck + 0.07:
            continue
        if lat > thw * 0.62:
            continue
        y_face = y_face_fn(wc)
        if y_face < 0.12:
            continue
        if y_face > 0.10 and lm.h_neck - 0.03 <= h <= lm.h_neck + 0.045:
            if lat < thw * 0.48:
                slots[idx] = SLOT_COLLAR if (y_face > 0.18 and lat < thw * 0.34) else SLOT_SKIN
                continue
        if slots[idx] in (SLOT_JEANS, SLOT_SHIRT):
            slots[idx] = SLOT_SKIN


def _fix_researcher_collar_cuff_bleed(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
) -> None:
    """Neck ring and wrists: skin/shirt, not trouser blue from lateral bleed."""
    lo, span = lm.zmin, lm.span
    wrist_h = lm.h_waist - 0.10
    for face in bm.faces:
        wc = Vector()
        for v in face.verts:
            wc += ob.matrix_world @ v.co
        wc /= len(face.verts)
        h = (wc.z - lo) / span
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        idx = face.index
        if slots[idx] != SLOT_JEANS:
            continue
        if lm.h_neck - 0.06 <= h <= lm.h_neck + 0.10 and lat < thw * 0.62:
            slots[idx] = SLOT_SKIN
        elif h < wrist_h + 0.06 and lat > thw * 0.5:
            slots[idx] = SLOT_HAND


def _grow_arm_from_shoulders(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
) -> None:
    from collections import deque

    if not ob.vertex_groups:
        return
    vg_names = {g.index: g.name for g in ob.vertex_groups}
    wrist_h = lm.h_waist - 0.10
    lo, span = lm.zmin, lm.span

    def _shoulder_face(face: bmesh.types.BMFace) -> bool:
        for v in face.verts:
            for g in ob.data.vertices[v.index].groups:
                if vg_names.get(g.group) in _VG_SHOULDERS and g.weight > 0.32:
                    return True
        return False

    seeds = {
        f.index
        for f in bm.faces
        if _shoulder_face(f) and slots[f.index] in (SLOT_SHIRT, SLOT_HAND)
    }
    if not seeds:
        return
    seen = set(seeds)
    q: deque[int] = deque(seeds)
    while q:
        fi = q.popleft()
        face = bm.faces[fi]
        for edge in face.edges:
            for other in edge.link_faces:
                oi = other.index
                if oi in seen:
                    continue
                wc = Vector()
                for v in other.verts:
                    wc += ob.matrix_world @ v.co
                wc /= len(other.verts)
                h = (wc.z - lo) / span
                if h < lm.h_waist - 0.03 or h > lm.h_neck + 0.02:
                    continue
                lat = lateral_from_front(wc, hc, fwd)
                thw = torso_half_width(ws, lm.zmin, lm.span, h, lm.cx, lm.cy)
                if lat <= thw + lm.span * 0.006:
                    continue
                seen.add(oi)
                slots[oi] = SLOT_HAND if h < wrist_h else SLOT_SHIRT
                q.append(oi)


def _grow_arm_regions(
    bm: bmesh.types.BMesh,
    slots: list[int],
    ob: bpy.types.Object,
    lm: FounderLandmarks,
    ws: list[Vector],
    hc: Vector,
    fwd: Vector,
    y_face_fn,
) -> None:
    wrist_h = lm.h_waist - 0.10
    for _ in range(12):
        changed = False
        for face in bm.faces:
            wc = Vector()
            for v in face.verts:
                wc += ob.matrix_world @ v.co
            wc /= len(face.verts)
            h = (wc.z - lm.zmin) / lm.span
            lat = lateral_from_front(wc, hc, fwd)
            thw = torso_half_width(ws, lm.zmin, lm.span, h, lm.cx, lm.cy)
            arm = lat > thw + lm.span * 0.01
            idx = face.index
            if slots[idx] == SLOT_JEANS and arm and h < lm.h_neck:
                slots[idx] = SLOT_HAND if h < wrist_h else SLOT_SHIRT
                changed = True
            if slots[idx] != SLOT_JEANS or not arm:
                continue
            for e in face.edges:
                for other in e.link_faces:
                    oi = other.index
                    if slots[oi] in (SLOT_SHIRT, SLOT_HAND):
                        slots[idx] = SLOT_HAND if h < wrist_h else SLOT_SHIRT
                        changed = True
                        break
        if not changed:
            break


def _height_span(ob: bpy.types.Object) -> tuple[float, float]:
    zs = [(ob.matrix_world @ v.co).z for v in ob.data.vertices]
    return min(zs), max(zs)


def spawn_researcher_glasses(body: bpy.types.Object) -> bpy.types.Object:
    """Simple lab glasses (readability prop, parented to body)."""
    from founder_face_decals import _fwd_basis
    from kenney_geometry_landmarks import compute_landmarks

    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH" and ob.name.split(".")[0].startswith("ResearcherGlasses"):
            bpy.data.objects.remove(ob, do_unlink=True)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    _, right = _fwd_basis(fwd)
    eye_z = lm.zmin + lm.span * lm.h_eye
    sep = head_w * 0.192
    bar_y = head_w * 0.0055
    lens_r = head_w * 0.068
    mat = _ensure_material("ResearcherGlassesFrame", RESEARCHER_GLASSES, 0.32, specular=0.18)

    def _lens(center: Vector) -> bpy.types.Object:
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=center)
        ob = bpy.context.active_object
        ob.scale = (lens_r, bar_y, lens_r * 0.52)
        bpy.ops.object.transform_apply(scale=True)
        ob.data.materials.append(mat)
        bc.smooth_shade(ob)
        return ob

    anchors: list[Vector] = []
    for sign in (-1.0, 1.0):
        lateral = right * (sep * sign)
        origin = hc + lateral + Vector((0.0, 0.0, eye_z - hc.z)) + fwd * head_w * 0.19
        anchors.append(origin)
    left = _lens(anchors[0])
    right_lens = _lens(anchors[1])
    left.name = "ResearcherGlasses.L"
    right_lens.name = "ResearcherGlasses.R"
    mid = (anchors[0] + anchors[1]) * 0.5
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=mid)
    bridge = bpy.context.active_object
    bridge.name = "ResearcherGlasses"
    bridge.scale = (sep * 0.22, bar_y * 1.1, bar_y * 1.2)
    bridge.rotation_euler = fwd.to_track_quat("Y", "Z").to_euler()
    bpy.ops.object.transform_apply(scale=True, rotation=True)
    bridge.data.materials.append(mat)
    bpy.ops.object.select_all(action="DESELECT")
    bridge.select_set(True)
    left.select_set(True)
    right_lens.select_set(True)
    bpy.context.view_layer.objects.active = bridge
    bpy.ops.object.join()
    bridge = bpy.context.active_object
    bridge.name = "ResearcherGlasses"
    bridge.parent = body
    bridge.matrix_parent_inverse = body.matrix_world.inverted()
    bc.smooth_shade(bridge)
    return bridge


def _pose_researcher_idle_arms(arm: bpy.types.Object) -> None:
    """Arms along torso; hands mid-thigh (not groin). Researcher-only — does not touch shared blend."""
    if arm.animation_data and arm.animation_data.action:
        arm.animation_data.action = None
    bpy.context.view_layer.objects.active = arm
    arm.select_set(True)
    bpy.ops.object.mode_set(mode="POSE")
    for side in ("Left", "Right"):
        ua = arm.pose.bones.get(f"{side}Arm")
        fa = arm.pose.bones.get(f"{side}ForeArm")
        if not ua or not fa:
            continue
        ua.rotation_mode = "QUATERNION"
        fa.rotation_mode = "QUATERNION"
        if side == "Left":
            ua.rotation_quaternion = Quaternion((0.0, 0.0, 1.0), math.radians(-58.0))
            fa.rotation_quaternion = Quaternion((0.0, 0.0, 1.0), math.radians(-12.0))
        else:
            ua.rotation_quaternion = Quaternion((-1.0, 0.0, 0.0), math.radians(58.0))
            fa.rotation_quaternion = Quaternion((-1.0, 0.0, 0.0), math.radians(12.0))
    bpy.ops.object.mode_set(mode="OBJECT")
    bpy.context.view_layer.update()


def bake_researcher_stock_body() -> bpy.types.Object:
    """Kenney rig bake for researcher pose; does not write founder stock blend."""
    if not STOCK_META.is_file():
        raise RuntimeError(f"FAIL staff-researcher-idle missing stock meta {STOCK_META}")
    meta = json.loads(STOCK_META.read_text(encoding="utf-8"))
    want_verts = int(meta["vertexCount"])
    want_tris = int(meta["triangleCount"])
    kf._clear_scene()
    arm, mesh = kf._import_rigged()
    _pose_researcher_idle_arms(arm)
    kf._ground_rig(arm, mesh)
    ok, msg = verify_standing_rig(arm)
    if not ok:
        raise RuntimeError(f"FAIL staff-researcher-idle: {msg}")
    ok, msg = verify_kenney_arm_drop(arm)
    if not ok:
        raise RuntimeError(f"FAIL staff-researcher-idle: {msg}")
    refs = rig_head_foot_world_z(arm)
    if not refs:
        raise RuntimeError("FAIL staff-researcher-idle rig_head_foot_missing")
    head_ref, foot_ref = refs
    baked = kf._bake_mesh(mesh, arm, head_ref, foot_ref)
    kf._ensure_upright_z(baked, head_ref, foot_ref)
    kf._face_forward_z(baked)
    kf._scale_character(baked)
    from kenney_founder_stock_recolor import bisect_stock_landmarks

    bisect_stock_landmarks(baked)
    if len(baked.data.vertices) != want_verts:
        raise RuntimeError(
            f"FAIL staff-researcher-idle stock_verts={len(baked.data.vertices)} want={want_verts}"
        )
    tri_count = sum(len(p.vertices) for p in baked.data.polygons)
    if tri_count != want_tris:
        raise RuntimeError(
            f"FAIL staff-researcher-idle stock_tris={tri_count} want={want_tris}"
        )
    return baked


def spawn_researcher_eye_meshes(body: bpy.types.Object) -> list[bpy.types.Object]:
    """Larger, darker eye discs than founder default (researcher-only)."""
    from founder_face_decals import (
        EYE_OBJECT_NAMES,
        _body_bvh_evaluated,
        _eye_world_matrix,
        _fwd_basis,
        _head_width_at,
        _make_solid_mat,
        _parent_keep_world,
        _purge_face_objects,
        _ray_hit_face,
    )

    _purge_face_objects()
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)
    _, right = _fwd_basis(fwd)
    from researcher_hero_geometry import _researcher_eye_h, _researcher_head_top

    head_top = _researcher_head_top(ws, lm)
    eye_h = _researcher_eye_h(ws, lm, head_top)
    eye_z = lm.zmin + lm.span * eye_h
    proud = head_w * 0.034
    sep = head_w * 0.158
    radius = head_w * 0.036
    cast_in = -fwd
    eye_mat = _make_solid_mat("ResearcherEyeMesh", (0.02, 0.02, 0.025, 1.0), 0.98)
    bvh = _body_bvh_evaluated(body)
    created: list[bpy.types.Object] = []
    eye_anchor = hc + Vector((0.0, 0.0, eye_z - hc.z))
    shoot_dist = max(head_w * 0.55, 0.14)
    for name, sign in zip(EYE_OBJECT_NAMES, (-1.0, 1.0), strict=True):
        lateral = right * (sep * sign)
        origin = eye_anchor + lateral + fwd * shoot_dist
        hit = _ray_hit_face(bvh, origin, cast_in)
        seed = eye_anchor + lateral + fwd * (head_w * 0.055)
        face_fwd = fwd
        if hit is not None:
            loc, norm = hit
            seed = loc
            outward = norm if norm.dot(fwd) >= 0 else -norm
            face_fwd = outward
        nearest, norm_n, _idx, _dist = bvh.find_nearest(seed)
        if nearest is not None:
            outward = norm_n if norm_n.dot(fwd) >= 0 else -norm_n
            center = nearest + outward * proud
            face_fwd = outward
        else:
            center = seed + face_fwd * proud
        center.z = eye_z
        ob = _eye_world_matrix(center, face_fwd, radius, proud)
        ob.name = name
        ob.data.materials.clear()
        ob.data.materials.append(eye_mat)
        bc.smooth_shade(ob)
        _parent_keep_world(ob, body)
        created.append(ob)
    return created


def load_stock_and_recolor() -> bpy.types.Object:
    ob = bake_researcher_stock_body()
    ob.name = STOCK_OBJECT_NAME
    apply_researcher_materials(ob)
    from researcher_hero_geometry import (  # noqa: WPS433
        apply_researcher_body_lab_coat_displace,
        remove_legacy_lab_coat_objects,
        spawn_researcher_hair_cap,
    )

    if os.environ.get("RESEARCHER_SKIP_LAB_COAT", "").strip() != "1":
        remove_legacy_lab_coat_objects()
        apply_researcher_body_lab_coat_displace(ob)
    spawn_researcher_eye_meshes(ob)
    spawn_researcher_hair_cap(ob)
    return ob