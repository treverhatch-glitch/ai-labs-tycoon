"""Founder hero: import canonical stock Kenney mesh; flat material slots only (no mesh edits)."""
from __future__ import annotations

import hashlib
import json
import shutil
from pathlib import Path

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc
import kenney_founder_lib as kf
from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward
from kenney_geometry_landmarks import FounderLandmarks, compute_landmarks, torso_half_width

STOCK_SOURCE = bc.REPO_ROOT / "tools" / "art" / "source" / "kenney"
STOCK_BLEND = STOCK_SOURCE / "staff-founder-kenney-stock.blend"
STOCK_KIT_GLB = STOCK_SOURCE / "staff-founder-kenney-stock-kit.glb"
STOCK_KIT_SHA = STOCK_SOURCE / "staff-founder-kenney-stock-kit.glb.sha256"
STOCK_GLB = bc.ART_ROOT / "staff-kit" / "staff-founder-kenney-stock.glb"
STOCK_META = STOCK_BLEND.with_suffix(".stock.json")
STOCK_OBJECT_NAME = "KenneyFounderStock"

FOUNDER_BROW = (0.20, 0.16, 0.14, 1.0)
FOUNDER_MOUTH = (0.18, 0.12, 0.12, 1.0)

SLOT_SOLE = 0
SLOT_SHOE = 1
SLOT_JEANS = 2
SLOT_SHIRT = 3
SLOT_SKIN = 4
SLOT_HAND = 5
SLOT_HAIR = 6
SLOT_EYE = 7
SLOT_BROW = 8
SLOT_MOUTH = 9

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


def _ensure_material(name: str, rgba: tuple[float, float, float, float], rough: float) -> bpy.types.Material:
    mat = bpy.data.materials.get(name)
    if mat:
        bpy.data.materials.remove(mat)
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = rgba
        bsdf.inputs["Roughness"].default_value = rough
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
            return SLOT_SHIRT
        return SLOT_SKIN

    if h > lm.h_neck:
        head_pts = [p for p in all_ws if (p.z - lo) / span > lm.h_neck]
        head_top = max((p.z - lo) / span for p in head_pts) if head_pts else 0.97
        h_hairline = lm.h_neck + 0.85 * (head_top - lm.h_neck)
        if h >= h_hairline:
            if y_face < 0.05 or h >= h_hairline + 0.012:
                return SLOT_HAIR
        elif y_face < -0.03 or (h > lm.h_neck + 0.08 and y_face < 0.04):
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


def collect_founder_stock_meshes() -> list[bpy.types.Object]:
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
    return [body, *eyes]


def apply_founder_materials(ob: bpy.types.Object) -> None:
    _strip_mesh_vertex_colors(ob.data)
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    lm = compute_landmarks(ob)
    fwd = toe_forward([ob])
    hc, y_face_fn = face_shell_coord(ob, fwd)
    head_w = _head_width_at(ws, lm.zmin, lm.span, lm.h_eye)

    mats = [
        _ensure_material("FounderSole", kf.FOUNDER_SHOE_SOLE, 0.72),
        _ensure_material("FounderShoe", kf.FOUNDER_SHOE, 0.38),
        _ensure_material("FounderJeans", kf.FOUNDER_JEANS, 0.56),
        _ensure_material("FounderTee", kf.FOUNDER_HOODIE_RED, 0.58),
        _ensure_material("FounderSkin", kf.FOUNDER_SKIN, 0.50),
        _ensure_material("FounderHand", kf.FOUNDER_SKIN, 0.48),
        _ensure_material("FounderHair", kf.FOUNDER_HAIR, 0.48),
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
    _apply_arm_vertex_groups(bm, slots, ob, lm, ws, hc, fwd, force_override=True)
    _grow_arm_from_shoulders(bm, slots, ob, lm, ws, hc, fwd)
    _grow_arm_regions(bm, slots, ob, lm, ws, hc, fwd, y_face_fn)
    _apply_hand_skin_vertex_groups(bm, slots, ob)
    for face, slot in zip(bm.faces, slots, strict=True):
        face.material_index = slot
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()
    bc.smooth_shade(ob)


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
        elif shirt >= 0.28 and shirt > skin * 0.85:
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
    hand_top = lm.h_waist + 0.25 * (lm.h_neck - lm.h_waist)
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
            if slots[face.index] == SLOT_HAND:
                slots[face.index] = SLOT_SHIRT
            continue
        skin_arm = hand + fore
        armish = skin_arm + shirt
        if armish < 0.10 and not force_override:
            continue
        if leg > armish and armish < 0.22:
            continue
        if fore >= 0.12 and lat > thw + span * 0.008:
            slots[face.index] = SLOT_HAND
            continue
        if hand >= 0.12 and lat > thw + span * 0.008:
            slots[face.index] = SLOT_HAND
            continue
        if shirt >= 0.14 and lat > thw + span * 0.008:
            slots[face.index] = SLOT_SHIRT
            continue
        if h < hand_top and hand + fore >= shirt * 0.35:
            slots[face.index] = SLOT_HAND
        elif shirt >= hand * 0.3:
            slots[face.index] = SLOT_SHIRT


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
    hand_top = lm.h_waist + 0.25 * (lm.h_neck - lm.h_waist)
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
                slots[oi] = SLOT_HAND if h < hand_top else SLOT_SHIRT
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
    hand_top = lm.h_waist + 0.25 * (lm.h_neck - lm.h_waist)
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
                slots[idx] = SLOT_HAND if h < hand_top else SLOT_SHIRT
                changed = True
            if slots[idx] != SLOT_JEANS or not arm:
                continue
            for e in face.edges:
                for other in e.link_faces:
                    oi = other.index
                    if slots[oi] in (SLOT_SHIRT, SLOT_HAND):
                        slots[idx] = SLOT_HAND if h < hand_top else SLOT_SHIRT
                        changed = True
                        break
        if not changed:
            break


def _height_span(ob: bpy.types.Object) -> tuple[float, float]:
    zs = [(ob.matrix_world @ v.co).z for v in ob.data.vertices]
    return min(zs), max(zs)


def load_stock_and_recolor() -> bpy.types.Object:
    if not STOCK_BLEND.is_file() or not STOCK_META.is_file():
        raise RuntimeError(f"FAIL staff-founder-idle missing stock blend {STOCK_BLEND}")
    meta = json.loads(STOCK_META.read_text(encoding="utf-8"))
    want_verts = int(meta["vertexCount"])
    want_tris = int(meta["triangleCount"])
    kf._clear_scene()
    bpy.ops.wm.open_mainfile(filepath=str(STOCK_BLEND))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if len(meshes) != 1:
        raise RuntimeError(f"FAIL staff-founder-idle stock_import parts={len(meshes)} want=1")
    ob = meshes[0]
    ob.name = STOCK_OBJECT_NAME
    if len(ob.data.vertices) != want_verts:
        raise RuntimeError(
            f"FAIL staff-founder-idle stock_verts={len(ob.data.vertices)} want={want_verts}"
        )
    tri_count = sum(len(p.vertices) for p in ob.data.polygons)
    if tri_count != want_tris:
        raise RuntimeError(
            f"FAIL staff-founder-idle stock_tris={tri_count} want={want_tris}"
        )
    apply_founder_materials(ob)
    from founder_face_decals import spawn_founder_eye_meshes

    spawn_founder_eye_meshes(ob)
    return ob