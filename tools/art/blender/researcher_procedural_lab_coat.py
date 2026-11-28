"""Procedural lab coat — tube torso + bone-aligned sleeve tubes (FAIL #30)."""
from __future__ import annotations

import math

import bmesh
import bpy
from mathutils import Matrix, Vector

_IDENTITY = Matrix.Identity(4)

from kenney_face_axis import face_shell_coord, lateral_from_front, toe_forward
from kenney_geometry_landmarks import compute_landmarks, torso_half_width
from researcher_shirt_derived_meshes import (
    MAX_COAT_BODY_GAP_M,
    SKIRT_NAME,
    SLEEVES_NAME,
    _attach_coat_part_to_body_or_arm,
    _body_bvh_world,
    _neck_cut_world_z,
    _skirt_z_band,
)

LEGACY_COAT_BASES = frozenset(
    {SKIRT_NAME, SLEEVES_NAME, "ResearcherLabCoat", "ResearcherCoat"}
)

# FAIL #34 option (a): shoulder caps only — no forearm tubes (bare forearms OK).
RESEARCHER_SHORT_SLEEVES = True
# FAIL #35: shoulder cap length along upper arm (not full upper-arm tube).
SHOULDER_CAP_UPPER_ARM_T = 0.24


def researcher_coat_short_sleeves(sleeves: bpy.types.Object) -> bool:
    if sleeves.get("researcher_short_sleeves"):
        return True
    # Heuristic when custom prop missing (older GLB / reimport).
    for nm in ("LeftForeArm", "RightForeArm"):
        vg = sleeves.vertex_groups.get(nm)
        if vg is None:
            continue
        for v in sleeves.data.vertices:
            for g in v.groups:
                if g.group == vg.index and g.weight > 0.45:
                    return False
    return RESEARCHER_SHORT_SLEEVES


def remove_legacy_coat_objects() -> int:
    removed = 0
    for ob in list(bpy.context.scene.objects):
        if ob.type != "MESH":
            continue
        if ob.name.split(".")[0] in LEGACY_COAT_BASES:
            me = ob.data
            bpy.data.objects.remove(ob, do_unlink=True)
            if me.users == 0:
                bpy.data.meshes.remove(me)
            removed += 1
    return removed


def pose_bone_world_segment(
    arm: bpy.types.Object | None,
    bone_name: str,
    *,
    rest_pose: bool = True,
) -> tuple[Vector, Vector] | None:
    """Bone head→tail in world space (rest bind or current pose)."""
    if arm is None:
        return None
    bpy.context.view_layer.update()
    mw = arm.matrix_world
    if not rest_pose:
        pb = arm.pose.bones.get(bone_name)
        if pb is not None:
            return mw @ pb.head, mw @ pb.tail
    bone = arm.data.bones.get(bone_name)
    if bone is None:
        return None
    return mw @ bone.head_local, mw @ bone.tail_local


_bone_world_segment = pose_bone_world_segment


def _bone_armature_local_segment(
    arm: bpy.types.Object, bone_name: str
) -> tuple[Vector, Vector] | None:
    bone = arm.data.bones.get(bone_name)
    if bone is None:
        return None
    return Vector(bone.head_local), Vector(bone.tail_local)


def _attach_sleeves_to_armature_object(
    sleeves: bpy.types.Object, arm: bpy.types.Object
) -> None:
    """Armature-local mesh + modifier (deforms with LeftArm/ForeArm VGs, not Spine parent)."""
    if sleeves.parent:
        sleeves.parent = None
    for mod in list(sleeves.modifiers):
        if mod.type == "ARMATURE":
            sleeves.modifiers.remove(mod)
    sleeves.parent = arm
    sleeves.parent_type = "OBJECT"
    sleeves.matrix_local = _IDENTITY.copy()
    sleeves.matrix_world = arm.matrix_world.copy()
    mod = sleeves.modifiers.new("CoatWalkArmature", "ARMATURE")
    mod.object = arm
    bpy.context.view_layer.update()


def _ortho_basis(axis: Vector) -> tuple[Vector, Vector]:
    ax = axis.normalized()
    ref = Vector((0.0, 0.0, 1.0)) if abs(ax.z) < 0.92 else Vector((0.0, 1.0, 0.0))
    u = ref.cross(ax).normalized()
    v = ax.cross(u).normalized()
    return u, v


def _bmesh_tube(
    bm: bmesh.types.BMesh,
    p0: Vector,
    p1: Vector,
    *,
    radius0: float,
    radius1: float,
    segments: int = 12,
    rings: int = 8,
) -> None:
    axis = p1 - p0
    length = axis.length
    if length < 1e-6:
        return
    u, v = _ortho_basis(axis)
    ring_verts: list[list[bmesh.types.BMVert]] = []
    for ri in range(rings):
        t = ri / max(1, rings - 1)
        center = p0.lerp(p1, t)
        rad = radius0 + (radius1 - radius0) * t
        ring: list[bmesh.types.BMVert] = []
        for si in range(segments):
            ang = (2.0 * math.pi * si) / segments
            offset = (u * math.cos(ang) + v * math.sin(ang)) * rad
            ring.append(bm.verts.new(center + offset))
        ring_verts.append(ring)
    for ri in range(rings - 1):
        for si in range(segments):
            sn = (si + 1) % segments
            try:
                bm.faces.new(
                    (
                        ring_verts[ri][si],
                        ring_verts[ri + 1][si],
                        ring_verts[ri + 1][sn],
                        ring_verts[ri][sn],
                    )
                )
            except ValueError:
                pass


def _bmesh_torso_frustum(
    bm: bmesh.types.BMesh,
    *,
    cx: float,
    cy: float,
    z_top: float,
    z_bot: float,
    r_top: float,
    r_bot: float,
    segments: int = 20,
    rings: int = 14,
) -> None:
    ring_verts: list[list[bmesh.types.BMVert]] = []
    for ri in range(rings):
        t = ri / max(1, rings - 1)
        z = z_top + (z_bot - z_top) * t
        rad = r_top + (r_bot - r_top) * t
        ring = []
        for si in range(segments):
            ang = (2.0 * math.pi * si) / segments
            ring.append(bm.verts.new((cx + math.cos(ang) * rad, cy + math.sin(ang) * rad, z)))
        ring_verts.append(ring)
    for ri in range(rings - 1):
        for si in range(segments):
            sn = (si + 1) % segments
            try:
                bm.faces.new(
                    (
                        ring_verts[ri][si],
                        ring_verts[ri + 1][si],
                        ring_verts[ri + 1][sn],
                        ring_verts[ri][sn],
                    )
                )
            except ValueError:
                pass


def _mesh_from_bmesh(bm: bmesh.types.BMesh, name: str) -> bpy.types.Object:
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    mesh = bpy.data.meshes.new(f"{name}Mesh")
    bm.to_mesh(mesh)
    bm.free()
    mesh.update()
    ob = bpy.data.objects.new(name, mesh)
    bpy.context.collection.objects.link(ob)
    return ob


def _bmesh_to_local(bm: bmesh.types.BMesh, world_matrix: Matrix) -> None:
    inv = world_matrix.inverted()
    for v in bm.verts:
        v.co = inv @ v.co


def _ensure_vg(ob: bpy.types.Object, name: str) -> bpy.types.VertexGroup:
    if name in ob.vertex_groups:
        return ob.vertex_groups[name]
    return ob.vertex_groups.new(name=name)


def _assign_torso_weights(ob: bpy.types.Object, lm) -> None:
    lo, span = lm.zmin, lm.span
    for g in ("Hips", "Spine", "Chest", "UpperChest"):
        _ensure_vg(ob, g)
    for vi in range(len(ob.data.vertices)):
        wc = ob.matrix_world @ ob.data.vertices[vi].co
        h = (wc.z - lo) / span
        if h < lm.h_waist - 0.04:
            weights = (("Hips", 0.65), ("Spine", 0.35))
        elif h < lm.h_waist + 0.12:
            weights = (("Spine", 0.7), ("Hips", 0.3))
        elif h < lm.h_neck - 0.06:
            weights = (("Spine", 0.45), ("Chest", 0.55))
        else:
            weights = (("Chest", 0.55), ("UpperChest", 0.45))
        for g in list(ob.data.vertices[vi].groups):
            ob.vertex_groups[g.group].remove([vi])
        for nm, w in weights:
            ob.vertex_groups[nm].add([vi], w, "REPLACE")


def _segment_distance(p: Vector, a: Vector, b: Vector) -> float:
    ab = b - a
    if ab.length_squared < 1e-12:
        return (p - a).length
    t = max(0.0, min(1.0, (p - a).dot(ab) / ab.length_squared))
    return (p - (a + ab * t)).length


def _assign_sleeve_weights_lateral(sleeves: bpy.types.Object, lm) -> None:
    lo, span = lm.zmin, lm.span
    for side in ("Left", "Right"):
        for nm in (f"{side}Arm", f"{side}ForeArm"):
            _ensure_vg(sleeves, nm)
    for vi in range(len(sleeves.data.vertices)):
        wc = sleeves.matrix_world @ sleeves.data.vertices[vi].co
        side = "Right" if wc.x >= lm.cx else "Left"
        h = (wc.z - lo) / span
        bone = f"{side}ForeArm" if h < lm.h_waist + 0.16 else f"{side}Arm"
        for g in list(sleeves.data.vertices[vi].groups):
            sleeves.vertex_groups[g.group].remove([vi])
        sleeves.vertex_groups[bone].add([vi], 1.0, "REPLACE")


def _assign_sleeve_weights_from_bones(
    sleeves: bpy.types.Object,
    arm: bpy.types.Object,
    *,
    short_sleeves: bool = False,
) -> None:
    """VG 1.0 on upper/lower arm bones only (not Spine / not Hand competition)."""
    bones = (
        ("LeftArm", "RightArm")
        if short_sleeves
        else ("LeftArm", "RightArm", "LeftForeArm", "RightForeArm")
    )
    for b in bones:
        _ensure_vg(sleeves, b)
    for vi in range(len(sleeves.data.vertices)):
        co = sleeves.data.vertices[vi].co
        best = None
        best_d = 1e9
        for bone in bones:
            seg = _bone_armature_local_segment(arm, bone)
            if seg is None:
                continue
            d = _segment_distance(co, seg[0], seg[1])
            if d < best_d:
                best_d = d
                best = bone
        if best is None:
            continue
        for g in list(sleeves.data.vertices[vi].groups):
            sleeves.vertex_groups[g.group].remove([vi])
        sleeves.vertex_groups[best].add([vi], 1.0, "REPLACE")


def _vg_weight(ob: bpy.types.Object, vi: int, group: str) -> float:
    vg = ob.vertex_groups.get(group)
    if vg is None:
        return 0.0
    for g in ob.data.vertices[vi].groups:
        if g.group == vg.index:
            return g.weight
    return 0.0


def _body_arm_axis_from_weights(
    body: bpy.types.Object, side: str
) -> tuple[Vector, Vector, float] | None:
    ua, fa, hand = f"{side}Arm", f"{side}ForeArm", f"{side}Hand"
    upper: list[Vector] = []
    fore: list[Vector] = []
    wrist: list[Vector] = []
    mw = body.matrix_world
    for vi in range(len(body.data.vertices)):
        wc = mw @ body.data.vertices[vi].co
        if _vg_weight(body, vi, ua) >= 0.2:
            upper.append(wc)
        if _vg_weight(body, vi, fa) >= 0.2:
            fore.append(wc)
        if _vg_weight(body, vi, hand) >= 0.15:
            wrist.append(wc)
    if len(upper) < 6:
        return None
    p0 = sum(upper, Vector()) / len(upper)
    if wrist:
        p1 = min(wrist, key=lambda w: w.z)
    elif fore:
        p1 = min(fore, key=lambda w: w.z)
    else:
        return None
    radii: list[float] = []
    axis = p1 - p0
    if axis.length_squared < 1e-10:
        return None
    axis_n = axis.normalized()
    for wc in upper + fore + wrist:
        along = (wc - p0).dot(axis_n)
        proj = p0 + axis_n * along
        radii.append((wc - proj).length)
    rad = max(0.032, sorted(radii)[int(len(radii) * 0.82)] * 1.1) if radii else 0.04
    return p0, p1, rad


def _cull_torso_armpit_sliver_faces(
    torso: bpy.types.Object,
    body: bpy.types.Object,
    lm,
    span: float,
    hc: Vector,
    fwd: Vector,
) -> int:
    """Remove lateral coat columns under shoulder that read as armpit slivers (FAIL #35)."""
    _, y_face_fn = face_shell_coord(body, fwd)
    bm = bmesh.new()
    bm.from_mesh(torso.data)
    mw = torso.matrix_world
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lo = lm.zmin
    to_del: list[bmesh.types.BMFace] = []
    for f in bm.faces:
        wc = sum((mw @ v.co for v in f.verts), Vector()) / len(f.verts)
        h = (wc.z - lo) / span
        if h < lm.h_neck - 0.06 or h > lm.h_waist - 0.04:
            continue
        if y_face_fn(wc) < -0.08:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if thw * 0.74 <= lat <= thw * 1.52:
            to_del.append(f)
    removed = len(to_del)
    if to_del:
        bmesh.ops.delete(bm, geom=to_del, context="FACES_ONLY")
        bmesh.ops.delete(
            bm, geom=[v for v in bm.verts if not v.link_faces], context="VERTS"
        )
        bm.to_mesh(torso.data)
        torso.data.update()
    bm.free()
    return removed


def _cull_short_sleeve_medial_armpit_faces(
    sleeves: bpy.types.Object,
    body: bpy.types.Object,
    lm,
    span: float,
    hc: Vector,
    fwd: Vector,
) -> int:
    """Drop inward-facing cap faces that read as vertical armpit slivers (FAIL #35)."""
    _, y_face_fn = face_shell_coord(body, fwd)
    bm = bmesh.new()
    bm.from_mesh(sleeves.data)
    bm.faces.ensure_lookup_table()
    mw = sleeves.matrix_world
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lo = lm.zmin
    removed = 0
    for f in list(bm.faces):
        wc = Vector((0.0, 0.0, 0.0))
        for v in f.verts:
            wc += mw @ v.co
        wc /= len(f.verts)
        h = (wc.z - lo) / span
        if h < lm.h_neck - 0.14 or h > lm.h_waist - 0.06:
            continue
        lat = lateral_from_front(wc, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat < thw * 0.42:
            continue
        n_w = (mw.to_3x3() @ f.normal).normalized()
        side = "Right" if wc.x >= lm.cx else "Left"
        medial = (side == "Right" and n_w.x < -0.25) or (side == "Left" and n_w.x > 0.25)
        torso_side = lat < thw * 1.12 and lat > thw * 0.32
        front_shell = y_face_fn(wc) > -0.06
        if medial and torso_side and front_shell:
            bm.faces.remove(f)
            removed += 1
    if removed:
        bmesh.ops.delete(bm, geom=[v for v in bm.verts if not v.link_faces], context="VERTS")
        bm.to_mesh(sleeves.data)
        sleeves.data.update()
    bm.free()
    return removed


def _nudge_short_cap_distal_to_torso(
    sleeves: bpy.types.Object,
    torso: bpy.types.Object,
    body: bpy.types.Object,
    lm,
    span: float,
    hc: Vector,
    fwd: Vector,
) -> None:
    """Close render gap: cap hem meets torso shell (FAIL #35 front armpit line)."""
    from researcher_shirt_derived_meshes import _body_bvh_world

    tree = _body_bvh_world(torso)
    if tree is None:
        tree = _body_bvh_world(body)
    if tree is None:
        return
    _, y_face_fn = face_shell_coord(body, fwd)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lo = lm.zmin
    inv = sleeves.matrix_world.inverted()
    cap = span * 0.024
    for vi in range(len(sleeves.data.vertices)):
        w = sleeves.matrix_world @ sleeves.data.vertices[vi].co
        h = (w.z - lo) / span
        if h < lm.h_neck - 0.10 or h > lm.h_waist + 0.04:
            continue
        if y_face_fn(w) < -0.10:
            continue
        lat = lateral_from_front(w, hc, fwd)
        thw = torso_half_width(ws, lo, span, h, lm.cx, lm.cy)
        if lat < thw * 0.48:
            continue
        loc, n, _idx, dist = tree.find_nearest(w)
        if loc is None or dist is None or dist > span * 0.14:
            continue
        if dist <= cap:
            continue
        outward = w - loc
        if outward.length_squared < 1e-12 and n.length_squared > 1e-12:
            outward = n.copy()
        if outward.length_squared < 1e-12:
            continue
        w = loc + outward.normalized() * cap
        sleeves.data.vertices[vi].co = inv @ w
    sleeves.data.update()


def _merge_armpit_overlap(
    torso: bpy.types.Object,
    sleeves: bpy.types.Object,
    arm: bpy.types.Object | None,
    body: bpy.types.Object,
    lm,
    span: float,
) -> None:
    z_band = span * 0.11
    for side in ("Left", "Right"):
        seg = pose_bone_world_segment(arm, f"{side}Arm") if arm else None
        if seg is None:
            axis = _body_arm_axis_from_weights(body, side)
            if axis is None:
                continue
            shoulder = axis[0]
        else:
            shoulder = seg[0]
        inv = torso.matrix_world.inverted()
        for vi in range(len(torso.data.vertices)):
            w = torso.matrix_world @ torso.data.vertices[vi].co
            if abs(w.z - shoulder.z) > z_band:
                continue
            if side == "Left" and w.x > lm.cx + span * 0.03:
                continue
            if side == "Right" and w.x < lm.cx - span * 0.03:
                continue
            delta = shoulder - w
            delta.z *= 0.35
            dist = delta.length
            if dist < 1e-6 or dist > span * 0.2:
                continue
            target = span * 0.016
            if dist > target:
                w = w + delta * ((dist - target) / dist)
                torso.data.vertices[vi].co = inv @ w
        torso.data.update()


def _arm_fallback_segment(
    body: bpy.types.Object, lm, hc: Vector, fwd: Vector, side: str, *, upper: bool
) -> tuple[Vector, Vector, float, float]:
    lo, span = lm.zmin, lm.span
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    h0 = lm.h_neck - 0.02 if upper else lm.h_waist + 0.14
    h1 = lm.h_waist + 0.08 if upper else lm.h_shoe + 0.18
    sign = 1.0 if side == "Right" else -1.0
    thw = torso_half_width(ws, lo, span, (h0 + h1) * 0.5, lm.cx, lm.cy)
    lat = hc + fwd * (0.05 if upper else 0.03)
    p0 = Vector((lm.cx, lm.cy, lo + span * h0)) + lat * (sign * thw * 1.02)
    p1 = Vector((lm.cx, lm.cy, lo + span * h1)) + lat * (sign * thw * 0.98)
    r0 = span * (0.042 if upper else 0.036)
    r1 = span * (0.038 if upper else 0.032)
    return p0, p1, r0, r1


def _nudge_idle_cuffs_to_body_shell(
    sleeves: bpy.types.Object, body: bpy.types.Object, lm, *, max_gap_m: float
) -> None:
    lo, span = lm.zmin, lm.span
    tree = _body_bvh_world(body)
    inv = sleeves.matrix_world.inverted()
    cap = max_gap_m * 0.97
    for vi in range(len(sleeves.data.vertices)):
        w = sleeves.matrix_world @ sleeves.data.vertices[vi].co
        h = (w.z - lo) / span
        if h < lm.h_shoe + 0.12 or h > lm.h_shoe + 0.34:
            continue
        loc, _n, _idx, dist = tree.find_nearest(w)
        if loc is None or dist is None or dist <= cap:
            continue
        outward = w - loc
        if outward.length_squared > 1e-12:
            w = loc + outward.normalized() * cap
        sleeves.data.vertices[vi].co = inv @ w
    sleeves.data.update()


def _assign_hand_weights_on_cuff_ring(
    sleeves: bpy.types.Object, arm: bpy.types.Object, span: float
) -> None:
    for side in ("Left", "Right"):
        hand = f"{side}Hand"
        seg = _bone_armature_local_segment(arm, hand)
        if seg is None:
            continue
        _ensure_vg(sleeves, hand)
        wrist = seg[0]
        hand_axis = seg[1] - seg[0]
        hand_len2 = hand_axis.length_squared
        for vi in range(len(sleeves.data.vertices)):
            co = sleeves.data.vertices[vi].co
            if (co - wrist).length > span * 0.12:
                continue
            if hand_len2 > 1e-12:
                t = (co - wrist).dot(hand_axis) / hand_len2
                if t > 0.22:
                    continue
            fa = _vg_weight(sleeves, vi, f"{side}ForeArm")
            ua = _vg_weight(sleeves, vi, f"{side}Arm")
            if fa + ua < 0.15:
                continue
            for g in list(sleeves.data.vertices[vi].groups):
                sleeves.vertex_groups[g.group].remove([vi])
            sleeves.vertex_groups[hand].add([vi], 1.0, "REPLACE")


def _nudge_cuff_verts_toward_wrist(
    sleeves: bpy.types.Object, arm: bpy.types.Object, *, max_pull_m: float
) -> None:
    inv = sleeves.matrix_world.inverted()
    for side in ("Left", "Right"):
        wrist = None
        seg = _bone_armature_local_segment(arm, f"{side}Hand")
        if seg is not None:
            wrist = sleeves.matrix_world @ seg[0]
        else:
            bone = arm.data.bones.get(f"{side}Hand") or arm.data.bones.get(
                f"{side}ForeArm"
            )
            if bone is not None:
                wrist = arm.matrix_world @ bone.head_local
        if wrist is None:
            continue
        fa = f"{side}ForeArm"
        for vi in range(len(sleeves.data.vertices)):
            fa_w = 0.0
            for g in sleeves.data.vertices[vi].groups:
                if g.group < len(sleeves.vertex_groups):
                    if sleeves.vertex_groups[g.group].name == fa:
                        fa_w += g.weight
            if fa_w < 0.45:
                continue
            w = sleeves.matrix_world @ sleeves.data.vertices[vi].co
            d = (w - wrist).length
            if d < 1e-6 or d > max_pull_m * 3.5:
                continue
            if d > max_pull_m:
                w = wrist + (w - wrist).normalized() * max_pull_m
            sleeves.data.vertices[vi].co = inv @ w
    sleeves.data.update()


def _clamp_verts_to_body_gap(
    coat: bpy.types.Object,
    body: bpy.types.Object,
    *,
    max_gap_m: float,
    z_min_world: float | None = None,
) -> None:
    """Shell offset cap — no shrinkwrap modifier or data transfer."""
    tree = _body_bvh_world(body)
    inv = coat.matrix_world.inverted()
    cap = max_gap_m * 0.97
    for vi in range(len(coat.data.vertices)):
        w = coat.matrix_world @ coat.data.vertices[vi].co
        if z_min_world is not None and w.z < z_min_world:
            continue
        loc, n, _idx, dist = tree.find_nearest(w)
        if loc is None or dist is None or dist <= cap:
            continue
        outward = w - loc
        if outward.length_squared > 1e-12:
            w = loc + outward.normalized() * cap
        elif n is not None and n.length_squared > 1e-12:
            w = loc + n.normalized() * cap
        coat.data.vertices[vi].co = inv @ w
    coat.data.update()


def _flatten_torso_hem(torso: bpy.types.Object, hem_z: float, span: float) -> None:
    inv = torso.matrix_world.inverted()
    for vi in range(len(torso.data.vertices)):
        w = torso.matrix_world @ torso.data.vertices[vi].co
        if w.z < hem_z + span * 0.05:
            w.z = hem_z
            torso.data.vertices[vi].co = inv @ w
    torso.data.update()


def spawn_procedural_lab_coat(
    body: bpy.types.Object,
    arm: bpy.types.Object | None,
    coat_mat: bpy.types.Material,
) -> list[bpy.types.Object]:
    remove_legacy_coat_objects()
    lm = compute_landmarks(body)
    fwd = toe_forward([body]).normalized()
    hc, _ = face_shell_coord(body, fwd)
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    lo, span = lm.zmin, lm.span
    _lo, _sp, _st, z_hip, z_mid, z_knee = _skirt_z_band(body, lm)
    z_top = lo + span * (lm.h_neck - 0.08)
    z_sh = _neck_cut_world_z(arm, lm)
    z_top = min(z_top, z_sh - span * 0.025)
    z_bot = max(z_mid - span * 0.008, z_knee - span * 0.021)
    thw_top = torso_half_width(ws, lo, span, lm.h_neck - 0.04, lm.cx, lm.cy)
    thw_bot = torso_half_width(ws, lo, span, lm.h_waist + 0.02, lm.cx, lm.cy)
    r_top = thw_top * 0.93
    r_bot = thw_bot * 1.08

    mw = body.matrix_world.copy()
    bm_torso = bmesh.new()
    _bmesh_torso_frustum(
        bm_torso,
        cx=lm.cx,
        cy=lm.cy,
        z_top=z_top,
        z_bot=z_bot,
        r_top=r_top,
        r_bot=r_bot,
        segments=22,
        rings=16,
    )
    bmesh.ops.remove_doubles(bm_torso, verts=bm_torso.verts, dist=0.0008)
    _bmesh_to_local(bm_torso, mw)
    torso = _mesh_from_bmesh(bm_torso, SKIRT_NAME)
    torso.data.materials.append(coat_mat)
    torso.matrix_world = mw.copy()

    bm_slv = bmesh.new()
    use_arm_local = arm is not None
    for side in ("Left", "Right"):
        ua, fa = f"{side}Arm", f"{side}ForeArm"
        if use_arm_local:
            seg_u = _bone_armature_local_segment(arm, ua)
            seg_f = _bone_armature_local_segment(arm, fa)
        else:
            seg_u = seg_f = None
        if seg_u is None or seg_f is None:
            axis = _body_arm_axis_from_weights(body, side)
            if axis is None:
                p0, p1, r0, r1 = _arm_fallback_segment(
                    body, lm, hc, fwd, side, upper=True
                )
                cap_p1 = (
                    p0.lerp(p1, SHOULDER_CAP_UPPER_ARM_T)
                    if RESEARCHER_SHORT_SLEEVES
                    else p1
                )
                _bmesh_tube(
                    bm_slv,
                    p0,
                    cap_p1,
                    radius0=r0,
                    radius1=r1 * (0.88 if RESEARCHER_SHORT_SLEEVES else 1.0),
                    segments=12,
                    rings=5 if RESEARCHER_SHORT_SLEEVES else 7,
                )
                if RESEARCHER_SHORT_SLEEVES:
                    continue
                overlap = (p1 - p0).length * 0.08
                _fe, ft, _fr0, fr1 = _arm_fallback_segment(
                    body, lm, hc, fwd, side, upper=False
                )
                elbow = p1 - (p1 - p0).normalized() * overlap
                _bmesh_tube(bm_slv, elbow, ft, radius0=r1, radius1=fr1, segments=12, rings=8)
            else:
                p0, p1, rad = axis
                if use_arm_local:
                    inv_a = arm.matrix_world.inverted()
                    p0, p1 = inv_a @ p0, inv_a @ p1
                elbow = p0.lerp(p1, 0.48)
                end = (
                    p0.lerp(p1, SHOULDER_CAP_UPPER_ARM_T)
                    if RESEARCHER_SHORT_SLEEVES
                    else p1
                )
                r1 = rad * 0.88 if RESEARCHER_SHORT_SLEEVES else rad * 0.98
                rings = 5 if RESEARCHER_SHORT_SLEEVES else 7
                _bmesh_tube(
                    bm_slv, p0, end, radius0=rad, radius1=r1, segments=12, rings=rings
                )
                if not RESEARCHER_SHORT_SLEEVES:
                    _bmesh_tube(
                        bm_slv, elbow, p1, radius0=rad * 0.96, radius1=rad * 0.86,
                        segments=12, rings=8,
                    )
            continue
        u0, u1 = seg_u
        f0, f1 = seg_f
        rad_u = max(span * 0.042, (u1 - u0).length * 0.098)
        if RESEARCHER_SHORT_SLEEVES and use_arm_local:
            cap_u1 = u0.lerp(u1, SHOULDER_CAP_UPPER_ARM_T)
            _bmesh_tube(
                bm_slv,
                u0,
                cap_u1,
                radius0=rad_u,
                radius1=rad_u * 0.88,
                segments=12,
                rings=5,
            )
            continue
        rad_f = max(span * 0.034, (f1 - f0).length * 0.088)
        _bmesh_tube(bm_slv, u0, u1, radius0=rad_u, radius1=rad_u * 0.96, segments=12, rings=7)
        overlap = (u1 - u0).length * 0.06
        elbow = u1 - (u1 - u0).normalized() * overlap
        _bmesh_tube(bm_slv, elbow, f1, radius0=rad_f * 0.98, radius1=rad_f * 0.86, segments=12, rings=8)
        if use_arm_local:
            wrist_seg = _bone_armature_local_segment(arm, f"{side}Hand")
        else:
            wrist_seg = None
        if wrist_seg is not None:
            wrist = wrist_seg[1]
            cuff_r = max(span * 0.026, rad_f * 0.68)
            _bmesh_tube(
                bm_slv,
                f1,
                wrist,
                radius0=rad_f * 0.78,
                radius1=cuff_r,
                segments=10,
                rings=5,
            )

    bmesh.ops.remove_doubles(bm_slv, verts=bm_slv.verts, dist=0.014)
    if RESEARCHER_SHORT_SLEEVES:
        cap_mw = arm.matrix_world.copy() if use_arm_local else mw
        ws_tmp = [body.matrix_world @ v.co for v in body.data.vertices]
        lo = lm.zmin
        for side in ("Left", "Right"):
            to_del: list[bmesh.types.BMFace] = []
            for f in bm_slv.faces:
                wc = sum((cap_mw @ v.co for v in f.verts), Vector()) / len(f.verts)
                h = (wc.z - lo) / span
                if h < lm.h_neck - 0.14 or h > lm.h_waist - 0.06:
                    continue
                lat = lateral_from_front(wc, hc, fwd)
                thw = torso_half_width(ws_tmp, lo, span, h, lm.cx, lm.cy)
                if lat < thw * 0.38:
                    continue
                ax = (f.verts[0].co - f.verts[1].co).cross(
                    f.verts[1].co - f.verts[2].co
                )
                if ax.length_squared < 1e-12:
                    continue
                n = ax.normalized()
                if side == "Right" and wc.x >= lm.cx:
                    if n.x < -0.2 and thw * 0.38 < lat < thw * 1.05:
                        to_del.append(f)
                elif side == "Left" and wc.x < lm.cx:
                    if n.x > 0.2 and thw * 0.38 < lat < thw * 1.05:
                        to_del.append(f)
            if to_del:
                bmesh.ops.delete(bm_slv, geom=to_del, context="FACES_ONLY")
    if not use_arm_local:
        _bmesh_to_local(bm_slv, mw)
    sleeves = _mesh_from_bmesh(bm_slv, SLEEVES_NAME)
    sleeves.data.materials.append(coat_mat)
    if RESEARCHER_SHORT_SLEEVES:
        sleeves["researcher_short_sleeves"] = True
    if use_arm_local:
        sleeves.matrix_world = arm.matrix_world.copy()
    else:
        sleeves.matrix_world = mw.copy()

    short_sl = RESEARCHER_SHORT_SLEEVES
    if arm is not None:
        _assign_torso_weights(torso, lm)
        _assign_sleeve_weights_from_bones(sleeves, arm, short_sleeves=short_sl)
    else:
        _assign_torso_weights(torso, lm)
        _assign_sleeve_weights_lateral(sleeves, lm)
        from researcher_scene_mesh_audit import WRIST_CUFF_WRIST_GAP_M

        _nudge_idle_cuffs_to_body_shell(
            sleeves, body, lm, max_gap_m=WRIST_CUFF_WRIST_GAP_M
        )

    _flatten_torso_hem(torso, z_bot, span)
    if RESEARCHER_SHORT_SLEEVES:
        _cull_torso_armpit_sliver_faces(torso, body, lm, span, hc, fwd)
        _cull_torso_armpit_sliver_faces(torso, body, lm, span, hc, fwd)
    z_floor = z_knee - span * 0.02
    gap_cap = MAX_COAT_BODY_GAP_M * 0.98
    _clamp_verts_to_body_gap(torso, body, max_gap_m=gap_cap, z_min_world=z_hip - span * 0.02)
    if arm is None:
        _clamp_verts_to_body_gap(sleeves, body, max_gap_m=gap_cap)
    inv_t = torso.matrix_world.inverted()
    for vi in range(len(torso.data.vertices)):
        w = torso.matrix_world @ torso.data.vertices[vi].co
        if w.z < z_floor:
            w.z = z_floor
            torso.data.vertices[vi].co = inv_t @ w
    torso.data.update()
    _merge_armpit_overlap(torso, sleeves, arm, body, lm, span)
    if RESEARCHER_SHORT_SLEEVES:
        _nudge_short_cap_distal_to_torso(sleeves, torso, body, lm, span, hc, fwd)
        _cull_short_sleeve_medial_armpit_faces(sleeves, body, lm, span, hc, fwd)
    if arm is not None:
        if not short_sl:
            _assign_hand_weights_on_cuff_ring(sleeves, arm, span)
            _nudge_cuff_verts_toward_wrist(sleeves, arm, max_pull_m=0.038)
            _assign_sleeve_weights_from_bones(sleeves, arm, short_sleeves=False)
            _assign_hand_weights_on_cuff_ring(sleeves, arm, span)
        else:
            _assign_sleeve_weights_from_bones(sleeves, arm, short_sleeves=True)
        from researcher_scene_mesh_audit import log_sleeve_bone_alignment_audit

        from researcher_scene_mesh_audit import format_sleeve_rest_pose_audit_block

        log_sleeve_bone_alignment_audit(
            sleeves, arm, header=f"spawn span={span:.3f} rest=1"
        )
        audit_path = (
            __import__("blender_common", fromlist=["PREVIEW_ROOT"]).PREVIEW_ROOT
            / "staff-researcher-sleeve-bone-align-audit.log"
        )
        block = format_sleeve_rest_pose_audit_block(sleeves, arm)
        with audit_path.open("a", encoding="utf-8") as fh:
            fh.write("\n---\n" + block + "\n")
    _attach_coat_part_to_body_or_arm(torso, body, arm)
    if arm is not None:
        _attach_sleeves_to_armature_object(sleeves, arm)
    else:
        _attach_coat_part_to_body_or_arm(sleeves, body, arm)
    return [torso, sleeves]