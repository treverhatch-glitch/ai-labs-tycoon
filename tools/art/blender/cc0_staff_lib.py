"""Quaternius modular rigged humanoid → idle staff heroes (founder / researcher)."""
from __future__ import annotations

import math

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc

INCOMING = bc.REPO_ROOT / "tools/art/cc0/incoming"
MODULAR_GLB = INCOMING / "quaternius/character.glb"
TARGET_HEIGHT = 1.72
FOUNDER_SKIN = (0.82, 0.66, 0.52, 1.0)
RESEARCHER_SKIN = (0.76, 0.70, 0.66, 1.0)

SKIN_PARTS: dict[str, frozenset[str]] = {
    "Head": frozenset({"Head"}),
    "Neck": frozenset({"neck_01"}),
    "Torso": frozenset({"spine_01", "spine_02", "spine_03"}),
    "Hips": frozenset({"pelvis", "root"}),
    "Leg_-1": frozenset({"thigh_l", "calf_l"}),
    "Foot_-1": frozenset({"foot_l", "ball_l"}),
    "Leg_1": frozenset({"thigh_r", "calf_r"}),
    "Foot_1": frozenset({"foot_r", "ball_r"}),
    "Shoulder_-1": frozenset({"clavicle_l"}),
    "UpperArm_-1": frozenset({"upperarm_l"}),
    "LowerArm_-1": frozenset({"lowerarm_l"}),
    "Hand_-1": frozenset(
        {
            "hand_l",
            "index_01_l",
            "index_02_l",
            "index_03_l",
            "middle_01_l",
            "middle_02_l",
            "middle_03_l",
            "pinky_01_l",
            "pinky_02_l",
            "pinky_03_l",
            "ring_01_l",
            "ring_02_l",
            "ring_03_l",
            "thumb_01_l",
            "thumb_02_l",
            "thumb_03_l",
        }
    ),
    "Shoulder_1": frozenset({"clavicle_r"}),
    "UpperArm_1": frozenset({"upperarm_r"}),
    "LowerArm_1": frozenset({"lowerarm_r"}),
    "Hand_1": frozenset(
        {
            "hand_r",
            "index_01_r",
            "index_02_r",
            "index_03_r",
            "middle_01_r",
            "middle_02_r",
            "middle_03_r",
            "pinky_01_r",
            "pinky_02_r",
            "pinky_03_r",
            "ring_01_r",
            "ring_02_r",
            "ring_03_r",
            "thumb_01_r",
            "thumb_02_r",
            "thumb_03_r",
        }
    ),
}


def _mesh_centroid(ob: bpy.types.Object) -> Vector:
    if not ob.data.vertices:
        return ob.matrix_world.translation.copy()
    pts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    return sum(pts, Vector()) / len(pts)


def _bbox_y_span(ob: bpy.types.Object) -> tuple[float, float, float]:
    co = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    ys = [v.y for v in co]
    return min(ys), max(ys), max(ys) - min(ys)


def _clear_scene() -> None:
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)


def _pose_idle_arms(arm: bpy.types.Object) -> None:
    """Relaxed symmetric A-pose — arms down, not hip-hands."""
    bpy.context.view_layer.objects.active = arm
    bpy.ops.object.mode_set(mode="POSE")
    for side, sign in (("l", 1.0), ("r", -1.0)):
        ua = arm.pose.bones.get(f"upperarm_{side}")
        la = arm.pose.bones.get(f"lowerarm_{side}")
        if ua:
            ua.rotation_mode = "ZYX"
            ua.rotation_euler = (0.0, sign * math.radians(6), sign * math.radians(-82))
        if la:
            la.rotation_mode = "ZYX"
            la.rotation_euler = (0.0, 0.0, sign * math.radians(-14))
    bpy.ops.object.mode_set(mode="OBJECT")


def _apply_armature_modifiers(arm: bpy.types.Object, meshes: list[bpy.types.Object]) -> None:
    for ob in meshes:
        mods = [m for m in ob.modifiers if m.type == "ARMATURE"]
        for mod in mods:
            bpy.context.view_layer.objects.active = ob
            ob.select_set(True)
            bpy.ops.object.modifier_apply(modifier=mod.name)
    bpy.data.objects.remove(arm, do_unlink=True)


def _rotate_scene_z_to_y() -> None:
    """GlTF Z-up → art-script Y-up standing convention."""
    for ob in list(bpy.context.scene.objects):
        if ob.type != "MESH":
            continue
        ob.rotation_euler = (-math.pi / 2, 0.0, 0.0)
        bpy.context.view_layer.objects.active = ob
        ob.select_set(True)
        bpy.ops.object.transform_apply(rotation=True, scale=True)


def _scale_scene_to_height() -> None:
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        return
    min_y, max_y = 1e9, -1e9
    for ob in meshes:
        lo, hi, _ = _bbox_y_span(ob)
        min_y = min(min_y, lo)
        max_y = max(max_y, hi)
    span = max(1e-4, max_y - min_y)
    scale = TARGET_HEIGHT / span
    for ob in meshes:
        ob.scale = (scale, scale, scale)
    bpy.ops.object.select_all(action="DESELECT")
    for ob in meshes:
        ob.select_set(True)
    bpy.ops.object.transform_apply(scale=True)
    bc.ground_scene_meshes()


def _vert_bone_owner(ob: bpy.types.Object, mesh: bpy.types.Mesh, vert_idx: int) -> tuple[str, float]:
    best = ("", 0.0)
    if vert_idx >= len(mesh.vertices):
        return best
    for g in mesh.vertices[vert_idx].groups:
        if g.weight <= best[1]:
            continue
        if 0 <= g.group < len(ob.vertex_groups):
            best = (ob.vertex_groups[g.group].name, g.weight)
    return best


def _split_mesh_by_skin_map(
    src: bpy.types.Object, part_name: str, allowed: frozenset[str], min_weight: float = 0.08
) -> bpy.types.Object | None:
    if not src.data.vertices:
        return None
    mesh = src.data.copy()
    dup = bpy.data.objects.new(part_name, mesh)
    dup.matrix_world = src.matrix_world.copy()
    for vg in src.vertex_groups:
        dup.vertex_groups.new(name=vg.name)
    for v in src.data.vertices:
        for g in v.groups:
            if g.weight <= 1e-6:
                continue
            name = src.vertex_groups[g.group].name
            grp = dup.vertex_groups.get(name)
            if grp:
                grp.add([v.index], g.weight, "REPLACE")
    bpy.context.collection.objects.link(dup)
    me = dup.data
    bm = bmesh.new()
    bm.from_mesh(me)
    bm.verts.ensure_lookup_table()
    drop = []
    for v in bm.verts:
        bone, w = _vert_bone_owner(dup, me, v.index)
        if w < min_weight or bone not in allowed:
            drop.append(v)
    if not drop:
        bm.free()
        bpy.data.objects.remove(dup, do_unlink=True)
        return None
    bmesh.ops.delete(bm, geom=drop, context="VERTS")
    if not bm.verts:
        bm.free()
        bpy.data.objects.remove(dup, do_unlink=True)
        return None
    bm.to_mesh(me)
    me.update()
    bm.free()
    dup.name = part_name
    return dup


def _split_skin_rig(body: bpy.types.Object, skin: tuple[float, float, float, float]) -> list[bpy.types.Object]:
    parts: list[bpy.types.Object] = []
    for part_name, groups in SKIN_PARTS.items():
        ob = _split_mesh_by_skin_map(body, part_name, groups)
        if ob and len(ob.data.vertices) >= 8:
            bc.assign_solid(ob, skin, 0.52)
            parts.append(ob)
    bpy.data.objects.remove(body, do_unlink=True)
    return parts


def _split_eyes(eyes: bpy.types.Object) -> list[bpy.types.Object]:
    cx = 0.0
    if eyes.data.vertices:
        ws = [eyes.matrix_world @ v.co for v in eyes.data.vertices]
        cx = sum(v.x for v in ws) / len(ws)
    out: list[bpy.types.Object] = []
    for side, tag in ((-1, "-1"), (1, "1")):
        me = eyes.data.copy()
        ob = bpy.data.objects.new(f"Eye_{tag}", me)
        ob.matrix_world = eyes.matrix_world.copy()
        bpy.context.collection.objects.link(ob)
        bm = bmesh.new()
        bm.from_mesh(me)
        if side < 0:
            drop = [v for v in bm.verts if (ob.matrix_world @ v.co).x > cx + 1e-4]
        else:
            drop = [v for v in bm.verts if (ob.matrix_world @ v.co).x < cx - 1e-4]
        if drop:
            bmesh.ops.delete(bm, geom=drop, context="VERTS")
        bm.to_mesh(me)
        me.update()
        bm.free()
        if len(ob.data.vertices) >= 4:
            bc.assign_solid(ob, (0.10, 0.08, 0.11, 1.0), 0.32)
            out.append(ob)
        else:
            bpy.data.objects.remove(ob, do_unlink=True)
    bpy.data.objects.remove(eyes, do_unlink=True)
    return out


def _import_modular_base() -> list[bpy.types.Object]:
    if not MODULAR_GLB.is_file():
        raise RuntimeError(f"missing modular GLB {MODULAR_GLB}")
    _clear_scene()
    bpy.ops.import_scene.gltf(filepath=str(MODULAR_GLB))
    arm = next((o for o in bpy.context.scene.objects if o.type == "ARMATURE"), None)
    if not arm:
        raise RuntimeError("modular character missing armature")
    for junk in list(bpy.context.scene.objects):
        if junk.type == "MESH" and junk.name in ("Icosphere",):
            bpy.data.objects.remove(junk, do_unlink=True)
    skinned = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    _pose_idle_arms(arm)
    _apply_armature_modifiers(arm, skinned)
    return [o for o in bpy.context.scene.objects if o.type == "MESH"]


def _rename_ob(ob: bpy.types.Object, name: str) -> bpy.types.Object:
    ob.name = name
    return ob


def _dup_ob(ob: bpy.types.Object, name: str) -> bpy.types.Object:
    dup = ob.copy()
    dup.data = ob.data.copy()
    dup.name = name
    bpy.context.collection.objects.link(dup)
    return dup


def _add_glasses(head: bpy.types.Object, span: float) -> bpy.types.Object:
    import staff_connectivity as sc

    lo, hi = sc._world_bbox(head)
    center = (lo + hi) * 0.5
    glass = bc.add_cube(
        "Glasses",
        (span * 0.20, span * 0.05, span * 0.045),
        (center.x, center.y + (hi.y - lo.y) * 0.08, center.z + (hi.z - lo.z) * 0.42),
    )
    bc.assign_solid(glass, (0.14, 0.16, 0.18, 1.0), 0.25)
    return glass


def _drop_modular_cloth() -> None:
    for key in ("ClothTop", "ClothBottom"):
        ob = bpy.data.objects.get(key)
        if ob:
            bpy.data.objects.remove(ob, do_unlink=True)


def _union_bbox(objects: list[bpy.types.Object]) -> tuple[Vector, Vector]:
    import staff_connectivity as sc

    lo = Vector((1e9, 1e9, 1e9))
    hi = Vector((-1e9, -1e9, -1e9))
    for ob in objects:
        a, b = sc._world_bbox(ob)
        lo.x = min(lo.x, a.x)
        lo.y = min(lo.y, a.y)
        lo.z = min(lo.z, a.z)
        hi.x = max(hi.x, b.x)
        hi.y = max(hi.y, b.y)
        hi.z = max(hi.z, b.z)
    return lo, hi


def _apply_scale_to_parts(scales: dict[str, tuple[float, float, float]]) -> None:
    for name, fac in scales.items():
        ob = bpy.data.objects.get(name)
        if not ob:
            continue
        ob.scale = (
            ob.scale.x * fac[0],
            ob.scale.y * fac[1],
            ob.scale.z * fac[2],
        )
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        return
    bpy.ops.object.select_all(action="DESELECT")
    for ob in meshes:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = meshes[0]
    bpy.ops.object.transform_apply(scale=True)


def _reseal_skin_chain_gaps() -> None:
    import staff_connectivity as sc

    for child_name, parent_name in (("Head", "Neck"), ("Neck", "Torso")):
        child = bpy.data.objects.get(child_name)
        parent = bpy.data.objects.get(parent_name)
        if not child or not parent:
            continue
        for _ in range(6):
            gap = sc.bbox_gap(sc._world_bbox(parent), sc._world_bbox(child))
            if gap <= 0.018:
                break
            pc = _mesh_centroid(parent)
            cc = _mesh_centroid(child)
            delta = pc - cc
            if delta.length < 1e-5:
                break
            child.location += delta * min(0.28, gap / delta.length)
        bpy.context.view_layer.update()


def _apply_founder_chibi_proportions() -> None:
    """Two Point–style: oversized head/hands, slight torso squash."""
    _apply_scale_to_parts(
        {
            "Head": (1.15, 1.12, 1.15),
            "Hand_-1": (1.10, 1.08, 1.08),
            "Hand_1": (1.10, 1.08, 1.08),
            "Torso": (1.03, 0.96, 1.04),
        }
    )
    _reseal_skin_chain_gaps()


def _reset_head_neck_pitch() -> None:
    for tag in ("Head", "Neck"):
        ob = bpy.data.objects.get(tag)
        if not ob:
            continue
        ob.rotation_euler = (0.0, 0.0, 0.0)
        bpy.context.view_layer.objects.active = ob
        ob.select_set(True)
        bpy.ops.object.transform_apply(rotation=True)
        ob.select_set(False)


FOUNDER_HOODIE_NAVY = (0.08, 0.10, 0.32, 1.0)
FOUNDER_HOODIE_RED = (0.62, 0.14, 0.12, 1.0)
FOUNDER_JEANS = (0.04, 0.20, 0.72, 1.0)
FOUNDER_HAIR = (0.36, 0.22, 0.14, 1.0)


def _build_founder_hoodie_jeans(
    span: float,
    torso: bpy.types.Object | None,
    neck: bpy.types.Object | None,
    head: bpy.types.Object | None,
    ua_l: bpy.types.Object | None,
    ua_r: bpy.types.Object | None,
    la_l: bpy.types.Object | None,
    la_r: bpy.types.Object | None,
    hips: bpy.types.Object | None,
    leg_l: bpy.types.Object | None,
    leg_r: bpy.types.Object | None,
) -> list[bpy.types.Object]:
    import staff_connectivity as sc

    built: list[bpy.types.Object] = []
    if torso:
        bc.assign_solid(torso, FOUNDER_HOODIE_RED, 0.58)
    for leg in (leg_l, leg_r):
        if leg:
            bc.assign_solid(leg, FOUNDER_JEANS, 0.56)
    if neck:
        bc.assign_solid(neck, FOUNDER_HOODIE_RED, 0.55)

    body = _fit_box_garment(
        "HoodieTorso",
        [o for o in (torso, neck) if o],
        (1.58, 1.48, 1.22),
        FOUNDER_HOODIE_RED,
        0.58,
        y_shift=span * 0.04,
    )
    if body:
        built.append(body)
    for tag, arms in (("-1", [ua_l, la_l]), ("1", [ua_r, la_r])):
        slv = _fit_box_garment(
            f"HoodieSleeve_{tag}",
            [o for o in arms if o],
            (1.22, 1.08, 1.12),
            FOUNDER_HOODIE_NAVY,
            0.62,
        )
        if slv:
            built.append(slv)
    if head:
        lo, hi = sc._world_bbox(head)
        center = (lo + hi) * 0.5
        ext = hi - lo
        hood = bc.add_cube(
            "Hood",
            (ext.x * 1.02, ext.y * 0.55, ext.z * 0.72),
            (center.x, center.y - ext.y * 0.12, center.z + ext.z * 0.18),
        )
        bc.assign_solid(hood, FOUNDER_HOODIE_NAVY, 0.58)
        built.append(hood)
    if torso:
        tc = _mesh_centroid(torso)
        pocket = bc.add_cube(
            "HoodiePocket",
            (span * 0.14, span * 0.04, span * 0.11),
            (tc.x + span * 0.05, tc.y + span * 0.22, tc.z - span * 0.06),
        )
        bc.assign_solid(pocket, FOUNDER_HOODIE_RED, 0.58)
        built.append(pocket)
    hip_box = _fit_box_garment(
        "JeansHips",
        [o for o in (hips, torso) if o],
        (1.12, 0.42, 1.06),
        FOUNDER_JEANS,
        0.54,
        y_shift=-span * 0.08,
    )
    if hip_box:
        built.append(hip_box)
    for tag, leg in (("-1", leg_l), ("1", leg_r)):
        if not leg:
            continue
        jeans = _fit_box_garment(
            f"JeansLeg_{tag}",
            [leg],
            (1.06, 1.02, 1.06),
            FOUNDER_JEANS,
            0.56,
        )
        if jeans:
            built.append(jeans)
    return built


def _fit_box_garment(
    name: str,
    anchors: list[bpy.types.Object],
    scale: tuple[float, float, float],
    color: tuple[float, float, float, float],
    roughness: float,
    y_shift: float = 0.0,
) -> bpy.types.Object | None:
    anchors = [o for o in anchors if o]
    if not anchors:
        return None
    lo, hi = _union_bbox(anchors)
    center = (lo + hi) * 0.5
    ext = hi - lo
    center.y += y_shift
    ob = bc.add_cube(
        name,
        (max(0.05, ext.x * scale[0]), max(0.05, ext.y * scale[1]), max(0.05, ext.z * scale[2])),
        (center.x, center.y, center.z),
    )
    bc.assign_solid(ob, color, roughness)
    return ob


def _add_shoes(span: float) -> list[bpy.types.Object]:
    import staff_connectivity as sc

    shoes = []
    for tag in ("-1", "1"):
        foot = bpy.data.objects.get(f"Foot_{tag}")
        if not foot:
            continue
        lo, hi = sc._world_bbox(foot)
        center = (lo + hi) * 0.5
        ext = hi - lo
        ob = bc.add_cube(
            f"Shoe_{tag}",
            (ext.x * 1.35, max(span * 0.06, ext.y * 0.95), ext.z * 1.35),
            (center.x, center.y, center.z),
        )
        shoes.append(ob)
    return shoes


def _add_founder_mouth(head: bpy.types.Object, span: float) -> None:
    import staff_connectivity as sc

    lo, hi = sc._world_bbox(head)
    center = (lo + hi) * 0.5
    ext = hi - lo
    mouth = bc.add_cube(
        "Mouth",
        (span * 0.052, span * 0.016, span * 0.011),
        (center.x, center.y + ext.y * 0.08, center.z - ext.z * 0.10),
    )
    bc.assign_solid(mouth, (0.52, 0.30, 0.26, 1.0), 0.52)


def _snap_eyes_to_head(head: bpy.types.Object, span: float) -> None:
    import staff_connectivity as sc

    lo, hi = sc._world_bbox(head)
    center = (lo + hi) * 0.5
    ext = hi - lo
    eye_y = center.y + ext.y * 0.10
    eye_z = center.z + ext.z * 0.06
    sep = ext.x * 0.12
    for tag, sign in (("-1", -1), ("1", 1)):
        eye = bpy.data.objects.get(f"Eye_{tag}")
        if not eye:
            continue
        ec = _mesh_centroid(eye)
        target = Vector((center.x + sign * sep, eye_y, eye_z))
        eye.location += target - ec
    bpy.context.view_layer.update()


def _add_founder_hair_cap(head: bpy.types.Object, span: float) -> bpy.types.Object | None:
    import staff_connectivity as sc

    lo, hi = sc._world_bbox(head)
    center = (lo + hi) * 0.5
    ext = hi - lo
    cap = bc.add_cube(
        "Hair",
        (ext.x * 0.72, ext.y * 0.16, ext.z * 0.30),
        (center.x, center.y - ext.y * 0.16, center.z + ext.z * 0.12),
    )
    bc.assign_solid(cap, FOUNDER_HAIR, 0.55)
    bc.smooth_shade(cap)
    return cap


def _build_founder_clean_garments(span: float) -> list[bpy.types.Object]:
    """Single solid hoodie + jeans (no modular cloth ring/skirt)."""
    import staff_connectivity as sc

    for stale in ("Hoodie", "Jeans", "Hood", "HoodieTorso", "JeansHips", "HoodiePocket"):
        ob = bpy.data.objects.get(stale)
        if ob:
            bpy.data.objects.remove(ob, do_unlink=True)
    torso = bpy.data.objects.get("Torso")
    neck = bpy.data.objects.get("Neck")
    ua_l = bpy.data.objects.get("UpperArm_-1")
    ua_r = bpy.data.objects.get("UpperArm_1")
    hips = bpy.data.objects.get("Hips")
    leg_l = bpy.data.objects.get("Leg_-1")
    leg_r = bpy.data.objects.get("Leg_1")
    built: list[bpy.types.Object] = []
    hoodie = _fit_box_garment(
        "Hoodie",
        [o for o in (torso, neck, ua_l, ua_r) if o],
        (1.24, 1.14, 1.08),
        FOUNDER_HOODIE_RED,
        0.62,
        y_shift=span * 0.018,
    )
    if hoodie:
        built.append(hoodie)
    jeans = _fit_box_garment(
        "Jeans",
        [o for o in (hips, leg_l, leg_r) if o],
        (1.12, 1.06, 1.08),
        FOUNDER_JEANS,
        0.60,
        y_shift=-span * 0.015,
    )
    if jeans:
        built.append(jeans)
    head = bpy.data.objects.get("Head")
    if head:
        lo, hi = sc._world_bbox(head)
        center = (lo + hi) * 0.5
        ext = hi - lo
        hood = bc.add_cube(
            "Hood",
            (ext.x * 0.82, ext.y * 0.18, ext.z * 0.40),
            (center.x, center.y - ext.y * 0.22, center.z + ext.z * 0.10),
        )
        bc.assign_solid(hood, FOUNDER_HOODIE_NAVY, 0.62)
        built.append(hood)
    anchor = torso or hips
    for cloth in (hoodie, jeans):
        if cloth and anchor:
            aa, ab = sc._world_bbox(anchor)
            ba, bb = sc._world_bbox(cloth)
            cloth.location += ((aa + ab) - (ba + bb)) * 0.5
    bpy.context.view_layer.update()
    return built


def _snap_modular_cloth_to_body() -> None:
    """Quaternius cloth imports offset from skin after height scale — re-center on torso/hips."""
    import staff_connectivity as sc

    torso = bpy.data.objects.get("Torso")
    hips = bpy.data.objects.get("Hips")
    for cloth_key, anchor_key in (("ClothTop", "Torso"), ("ClothBottom", "Hips"), ("Hoodie", "Torso"), ("Jeans", "Hips")):
        cloth = bpy.data.objects.get(cloth_key)
        anchor = bpy.data.objects.get(anchor_key)
        if not cloth or not anchor:
            continue
        aa, ab = sc._world_bbox(anchor)
        ba, bb = sc._world_bbox(cloth)
        cloth.location += ((aa + ab) - (ba + bb)) * 0.5
    bpy.context.view_layer.update()


def _style_founder(
    objects: dict[str, bpy.types.Object],
    skin: tuple[float, float, float, float],
    span: float,
    cx: float,
    cz: float,
    min_y: float,
) -> None:
    hair_long = objects.get("Hair_Long") or bpy.data.objects.get("Hair_Long")
    eyebrows = objects.get("Eyebrows") or bpy.data.objects.get("Eyebrows")
    head = bpy.data.objects.get("Head")
    if hair_long:
        bpy.data.objects.remove(hair_long, do_unlink=True)
    if eyebrows:
        _rename_ob(eyebrows, "Eyebrows")
        bc.assign_solid(eyebrows, (0.14, 0.09, 0.07, 1.0), 0.48)
    _build_founder_clean_garments(span)
    if head:
        _add_founder_mouth(head, span)
        _add_founder_hair_cap(head, span)
        _snap_eyes_to_head(head, span)
        bc.assign_solid(head, skin, 0.88)
        bc.smooth_shade(head)
    shoes = _add_shoes(span)
    for ob in shoes:
        bc.assign_solid(ob, (0.92, 0.94, 0.96, 1.0), 0.38)


def _style_researcher(
    objects: dict[str, bpy.types.Object],
    skin: tuple[float, float, float, float],
    span: float,
    cx: float,
    cz: float,
    min_y: float,
) -> None:
    hair = objects.get("Hair_Long")
    eyebrows = objects.get("Eyebrows")
    torso = bpy.data.objects.get("Torso")
    neck = bpy.data.objects.get("Neck")
    ua_l = bpy.data.objects.get("UpperArm_-1")
    ua_r = bpy.data.objects.get("UpperArm_1")
    hips = bpy.data.objects.get("Hips")
    leg_l = bpy.data.objects.get("Leg_-1")
    leg_r = bpy.data.objects.get("Leg_1")
    _fit_box_garment(
        "Shirt",
        [o for o in (torso, ua_l, ua_r) if o],
        (1.02, 0.98, 0.88),
        (0.20, 0.36, 0.68, 1.0),
        0.48,
    )
    _fit_box_garment(
        "LabCoat",
        [o for o in (torso, ua_l, ua_r, neck) if o],
        (1.18, 1.14, 1.08),
        (0.94, 0.95, 0.97, 1.0),
        0.40,
        y_shift=span * 0.01,
    )
    _fit_box_garment(
        "Trousers",
        [o for o in (hips, leg_l, leg_r) if o],
        (1.06, 1.0, 0.95),
        (0.14, 0.16, 0.24, 1.0),
        0.58,
        y_shift=-span * 0.02,
    )
    if hair:
        _rename_ob(hair, "Hair")
        hair.scale = (0.88, 0.62, 0.9)
        bc.assign_solid(hair, (0.28, 0.22, 0.18, 1.0), 0.48)
    if eyebrows:
        _rename_ob(eyebrows, "Eyebrows")
        bc.assign_solid(eyebrows, (0.18, 0.12, 0.10, 1.0), 0.45)
    shoes = _add_shoes(span)
    for ob in shoes:
        bc.assign_solid(ob, (0.28, 0.30, 0.34, 1.0), 0.42)
    head = bpy.data.objects.get("Head")
    if head:
        _add_glasses(head, span)
    hand_r = bpy.data.objects.get("Hand_1")
    if hand_r:
        hr = _mesh_centroid(hand_r)
        tab = bc.add_cube(
            "Tablet",
            (span * 0.13, span * 0.17, span * 0.022),
            (hr.x + span * 0.03, hr.y + span * 0.01, hr.z + span * 0.02),
        )
        bc.assign_solid(tab, (0.18, 0.50, 0.92, 1.0), 0.30)
        bc.assign_emission(tab, (0.35, 0.72, 0.98, 1.0), strength=1.2)


def _scene_mesh_map() -> dict[str, bpy.types.Object]:
    return {o.name.split(".")[0]: o for o in bpy.context.scene.objects if o.type == "MESH"}


def _tighten_skin_to_torso() -> None:
    torso = bpy.data.objects.get("Torso")
    if not torso:
        return
    tc = _mesh_centroid(torso)
    for tag in (
        "Neck",
        "Head",
        "Shoulder_-1",
        "Shoulder_1",
        "UpperArm_-1",
        "UpperArm_1",
        "LowerArm_-1",
        "LowerArm_1",
        "Hand_-1",
        "Hand_1",
        "Hips",
        "Leg_-1",
        "Leg_1",
    ):
        ob = bpy.data.objects.get(tag)
        if not ob:
            continue
        c = _mesh_centroid(ob)
        ob.location += (tc - c) * 0.045


def _snap_outfits_to_torso() -> None:
    import staff_connectivity as sc

    def nudge(ob: bpy.types.Object, anchor: bpy.types.Object) -> None:
        aa = sc._world_bbox(anchor)
        ba = sc._world_bbox(ob)
        if sc.bbox_gap(aa, ba) <= 0.02:
            return
        ac = (aa[0] + aa[1]) * 0.5
        bc = (ba[0] + ba[1]) * 0.5
        ob.location += ac - bc

    torso = bpy.data.objects.get("Torso")
    head = bpy.data.objects.get("Head")
    garment_names = (
        "HoodieTorso",
        "HoodieSleeve_-1",
        "HoodieSleeve_1",
        "Hood",
        "HoodiePocket",
        "JeansHips",
        "JeansLeg_-1",
        "JeansLeg_1",
        "Hoodie",
        "Shirt",
        "LabCoat",
        "Trousers",
    )
    if torso:
        for name in garment_names:
            ob = bpy.data.objects.get(name)
            if ob:
                nudge(ob, torso)
    if head:
        for name in ("Hair", "Eyebrows", "Glasses"):
            ob = bpy.data.objects.get(name)
            if ob:
                nudge(ob, head)
    for tag in ("-1", "1"):
        foot = bpy.data.objects.get(f"Foot_{tag}")
        shoe = bpy.data.objects.get(f"Shoe_{tag}")
        if foot and shoe:
            nudge(shoe, foot)
        leg = bpy.data.objects.get(f"Leg_{tag}")
        jeans = bpy.data.objects.get(f"JeansLeg_{tag}")
        if leg and jeans:
            nudge(jeans, leg)
    hips = bpy.data.objects.get("Hips")
    jeans_hips = bpy.data.objects.get("JeansHips")
    if hips and jeans_hips:
        nudge(jeans_hips, hips)
    jeans = bpy.data.objects.get("Jeans")
    if hips and jeans:
        for _ in range(6):
            nudge(jeans, hips)
    hoodie = bpy.data.objects.get("Hoodie")
    if torso and hoodie:
        for _ in range(6):
            nudge(hoodie, torso)
    bpy.context.view_layer.update()


def _build_role(role: str, skin: tuple[float, float, float, float], styler) -> None:
    meshes = _import_modular_base()
    by_name = {o.name: o for o in meshes}
    body = by_name.pop("Superhero_Female", None)
    if not body:
        raise RuntimeError("Superhero_Female body missing from modular pack")
    eyes = by_name.pop("Eyes", None)
    _split_skin_rig(body, skin)
    if eyes:
        _split_eyes(eyes)
    _rotate_scene_z_to_y()
    _scale_scene_to_height()
    _drop_modular_cloth()
    if role == "founder":
        _apply_founder_chibi_proportions()
        _reset_head_neck_pitch()
    parts = _scene_mesh_map()
    torso = parts.get("Torso") or parts.get("Hips")
    if not torso:
        raise RuntimeError("no torso after skin split")
    _min, _max, span = _bbox_y_span(torso)
    co = [torso.matrix_world @ Vector(c) for c in torso.bound_box]
    cx = sum(v.x for v in co) / len(co)
    cz = sum(v.z for v in co) / len(co)
    styler(parts, skin, span, cx, cz, _min)
    _snap_outfits_to_torso()
    _tighten_skin_to_torso()
    _snap_outfits_to_torso()
    for ob in bpy.context.scene.objects:
        if ob.type != "MESH":
            continue
        base = ob.name.split(".")[0]
        if base in ("Hoodie", "Jeans", "Hood", "Mouth", "Hair") or base.startswith("Shoe_"):
            bc.smooth_shade(ob)
            continue
        width = 0.002 if base == "Head" else 0.004
        try:
            bc.bevel_object(ob, width=width, segments=1)
        except RuntimeError:
            pass
        if base == "Head" or base.startswith("Hand_"):
            bc.smooth_shade(ob)


_SPINE_SKIP_PREFIXES = (
    "Jeans",
    "Hoodie",
    "Shoe",
    "Hood",
    "FounderMug",
    "HoodiePocket",
    "Foot_",
    "Leg_",
)


def check_spine_clearance(
    mesh_obs: list[bpy.types.Object],
    max_radius: float = 0.6,
    only_parts: frozenset[str] | None = None,
) -> tuple[bool, str]:
    all_obs = [o for o in mesh_obs if o.type == "MESH"]
    check_obs = [
        o
        for o in all_obs
        if only_parts is None or o.name.split(".")[0] in only_parts
    ]
    by_name = {o.name.split(".")[0]: o for o in all_obs}
    hips = by_name.get("Hips") or by_name.get("Torso")
    head = by_name.get("Head")
    if not hips or not head:
        return False, "spine_parts_missing"
    p0 = _mesh_centroid(hips)
    p1 = _mesh_centroid(head)
    axis = p1 - p0
    if axis.length < 0.2:
        return False, "spine_axis_short"
    axis_n = axis.normalized()
    worst = ("", 0.0)
    for ob in check_obs:
        if ob.type != "MESH":
            continue
        base = ob.name.split(".")[0]
        if base.startswith(_SPINE_SKIP_PREFIXES):
            continue
        for v in ob.data.vertices:
            rel = ob.matrix_world @ v.co - p0
            perp = rel - axis_n * rel.dot(axis_n)
            r = perp.length
            if r > worst[1]:
                worst = (base, r)
    if worst[1] > max_radius:
        return False, f"spine_clearance {worst[0]} r={worst[1]:.3f}m"
    return True, f"spine_ok r={worst[1]:.3f}m"


def build_founder_cc0() -> None:
    _build_role("founder", FOUNDER_SKIN, _style_founder)


def build_researcher_cc0() -> None:
    _build_role("researcher", RESEARCHER_SKIN, _style_researcher)