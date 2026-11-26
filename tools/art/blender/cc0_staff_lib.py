"""Quaternius modular rigged humanoid → idle staff heroes (founder / researcher)."""
from __future__ import annotations

import math
from collections import defaultdict

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
    bpy.context.view_layer.objects.active = arm
    bpy.ops.object.mode_set(mode="POSE")
    for side, sign in (("l", 1.0), ("r", -1.0)):
        ua = arm.pose.bones.get(f"upperarm_{side}")
        la = arm.pose.bones.get(f"lowerarm_{side}")
        if ua:
            ua.rotation_mode = "ZYX"
            ua.rotation_euler = (0.0, sign * math.radians(8), sign * math.radians(-78))
        if la:
            la.rotation_mode = "ZYX"
            la.rotation_euler = (0.0, 0.0, sign * math.radians(-18))
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
            bc.assign_solid(ob, (0.95, 0.96, 0.98, 1.0), 0.32)
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


def _style_founder(
    objects: dict[str, bpy.types.Object],
    skin: tuple[float, float, float, float],
    span: float,
    cx: float,
    cz: float,
    min_y: float,
) -> None:
    top = objects.get("ClothTop")
    bottom = objects.get("ClothBottom")
    hair = objects.get("Hair_Long")
    eyebrows = objects.get("Eyebrows")
    if eyebrows:
        _rename_ob(eyebrows, "Eyebrows")
        bc.assign_solid(eyebrows, (0.14, 0.09, 0.07, 1.0), 0.45)
    if top:
        _rename_ob(top, "Hoodie")
        bc.assign_solid(top, (0.28, 0.22, 0.48, 1.0), 0.58)
    head = bpy.data.objects.get("Head")
    if head:
        import staff_connectivity as sc

        lo, hi = sc._world_bbox(head)
        center = (lo + hi) * 0.5
        ext = hi - lo
        hood = bc.add_cube(
            "Hood",
            (ext.x * 1.22, ext.y * 1.18, ext.z * 1.08),
            (center.x, center.y + ext.y * 0.06, center.z),
        )
        bc.assign_solid(hood, (0.24, 0.18, 0.42, 1.0), 0.62)
    if bottom:
        _rename_ob(bottom, "Trousers")
        bc.assign_solid(bottom, (0.16, 0.22, 0.48, 1.0), 0.55)
    if hair:
        _rename_ob(hair, "Hair")
        bc.assign_solid(hair, (0.14, 0.10, 0.08, 1.0), 0.42)
    shoes = _add_shoes(span)
    for ob in shoes:
        bc.assign_solid(ob, (0.92, 0.94, 0.96, 1.0), 0.38)
    hand_r = bpy.data.objects.get("Hand_1")
    if hand_r:
        hr = _mesh_centroid(hand_r)
        mug = bc.add_cube(
            "FounderMug",
            (span * 0.09, span * 0.10, span * 0.08),
            (hr.x + span * 0.04, hr.y + span * 0.01, hr.z + span * 0.03),
        )
        bc.assign_solid(mug, (0.94, 0.52, 0.12, 1.0), 0.38)


def _style_researcher(
    objects: dict[str, bpy.types.Object],
    skin: tuple[float, float, float, float],
    span: float,
    cx: float,
    cz: float,
    min_y: float,
) -> None:
    top = objects.get("ClothTop")
    bottom = objects.get("ClothBottom")
    hair = objects.get("Hair_Long")
    eyebrows = objects.get("Eyebrows")
    if top:
        shirt = _rename_ob(top, "Shirt")
        bc.assign_solid(shirt, (0.22, 0.38, 0.72, 1.0), 0.48)
        coat = _dup_ob(shirt, "LabCoat")
        coat.scale = (1.08, 1.18, 1.10)
        coat.location.y += span * 0.02
        bc.assign_solid(coat, (0.94, 0.95, 0.97, 1.0), 0.40)
    if bottom:
        _rename_ob(bottom, "Trousers")
        bc.assign_solid(bottom, (0.20, 0.24, 0.34, 1.0), 0.58)
    if hair:
        _rename_ob(hair, "Hair")
        hair.scale = (0.92, 0.75, 0.95)
        bc.assign_solid(hair, (0.22, 0.18, 0.14, 1.0), 0.48)
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
    if torso:
        for name in ("Hoodie", "Hood", "Shirt", "LabCoat", "Trousers"):
            ob = bpy.data.objects.get(name)
            if ob:
                nudge(ob, torso)
    if head:
        for name in ("Hair", "Hood", "Eyebrows", "Glasses"):
            ob = bpy.data.objects.get(name)
            if ob:
                nudge(ob, head)
    for tag in ("-1", "1"):
        foot = bpy.data.objects.get(f"Foot_{tag}")
        shoe = bpy.data.objects.get(f"Shoe_{tag}")
        if foot and shoe:
            nudge(shoe, foot)
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
    for ob in bpy.context.scene.objects:
        if ob.type != "MESH":
            continue
        try:
            bc.bevel_object(ob, width=0.004, segments=1)
        except RuntimeError:
            pass


def build_founder_cc0() -> None:
    _build_role("founder", FOUNDER_SKIN, _style_founder)


def build_researcher_cc0() -> None:
    _build_role("researcher", RESEARCHER_SKIN, _style_researcher)