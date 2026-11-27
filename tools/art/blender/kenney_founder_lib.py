"""Kenney Animated Characters 3 (CC0) — founder hero, Z-up mesh through export."""
from __future__ import annotations

import math

import bpy
import bmesh
from mathutils import Quaternion, Vector

import blender_common as bc
from kenney_founder_standing_gate import (
    rig_head_foot_world_z,
    verify_evaluated_mesh,
    verify_kenney_arm_drop,
    verify_standing_rig,
)

INCOMING = bc.REPO_ROOT / "tools/art/cc0/incoming"
KENNEY_FBX = INCOMING / "kenney/animated-characters-3/Model/characterMedium.fbx"
KENNEY_IDLE_FBX = INCOMING / "kenney/animated-characters-3/Animations/idle.fbx"
KENNEY_IDLE_REST_ACTION = "Idle"
KENNEY_IDLE_REST_FRAME = 1
TARGET_HEIGHT = 1.72
FOUNDER_SKIN = (0.84, 0.68, 0.54, 1.0)
FOUNDER_HAIR = (0.28, 0.16, 0.10, 1.0)
FOUNDER_HOODIE = (0.08, 0.10, 0.32, 1.0)
FOUNDER_HOODIE_RED = (0.62, 0.16, 0.14, 1.0)
FOUNDER_JEANS = (0.05, 0.10, 0.32, 1.0)
FOUNDER_SHOE = (0.88, 0.90, 0.92, 1.0)
FOUNDER_SHOE_SOLE = (0.32, 0.35, 0.40, 1.0)
FOUNDER_EYE = (0.12, 0.10, 0.14, 1.0)
FBX_KW = {
    "use_manual_orientation": True,
    "axis_forward": "-Z",
    "axis_up": "Y",
    "use_anim": True,
}


def _clear_scene() -> None:
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)


def _import_fbx(path) -> None:
    bpy.ops.import_scene.fbx(filepath=str(path), **FBX_KW)


def _apply_transforms(*objs: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    for o in objs:
        o.select_set(True)
    if objs:
        bpy.context.view_layer.objects.active = objs[0]
        bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)


def _import_rigged() -> tuple[bpy.types.Object, bpy.types.Object]:
    if not KENNEY_FBX.is_file():
        raise RuntimeError(f"missing Kenney FBX {KENNEY_FBX}")
    if not KENNEY_IDLE_FBX.is_file():
        raise RuntimeError(f"missing Kenney idle {KENNEY_IDLE_FBX}")
    _clear_scene()
    _import_fbx(KENNEY_FBX)
    arm = bpy.data.objects.get("Root")
    mesh = bpy.data.objects.get("characterMedium")
    if not arm or arm.type != "ARMATURE" or not mesh or mesh.type != "MESH":
        raise RuntimeError("Kenney rig import failed")
    _apply_transforms(arm, mesh)
    _import_fbx(KENNEY_IDLE_FBX)
    for extra in [o for o in bpy.context.scene.objects if o.type == "ARMATURE" and o != arm]:
        bpy.data.objects.remove(extra, do_unlink=True)
    return arm, mesh


def _ground_rig(arm: bpy.types.Object, mesh: bpy.types.Object) -> None:
    zs: list[float] = []
    for name in (
        "LeftFoot",
        "RightFoot",
        "LeftToes",
        "RightToes",
        "LeftHeelRoll",
        "RightHeelRoll",
    ):
        pb = arm.pose.bones.get(name)
        if pb:
            zs.append((arm.matrix_world @ pb.head).z)
            zs.append((arm.matrix_world @ pb.tail).z)
    if not zs:
        return
    dz = -min(zs)
    if abs(dz) < 1e-6:
        return
    arm.location.z += dz
    mesh.location.z += dz
    bpy.context.view_layer.update()


def _pose_kenney_idle_arms(arm: bpy.types.Object) -> None:
    """Arms ~10–15° from torso (not T); baked mesh must pass arm_drop gate."""
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
            ua.rotation_quaternion = Quaternion((0.0, 0.0, 1.0), math.radians(-72.0))
            fa.rotation_quaternion = Quaternion((0.0, 0.0, 1.0), math.radians(-12.0))
        else:
            ua.rotation_quaternion = Quaternion((-1.0, 0.0, 0.0), math.radians(82.0))
            fa.rotation_quaternion = Quaternion((-1.0, 0.0, 0.0), math.radians(15.0))
    bpy.ops.object.mode_set(mode="OBJECT")
    bpy.context.view_layer.update()


def _apply_idle_rest_pose(arm: bpy.types.Object) -> None:
    action = next(
        (a for a in bpy.data.actions if KENNEY_IDLE_REST_ACTION in a.name),
        None,
    )
    if not action:
        raise RuntimeError(f"missing Kenney idle rest action ({KENNEY_IDLE_REST_ACTION})")
    arm.animation_data_create()
    arm.animation_data.action = action
    bpy.context.scene.frame_set(KENNEY_IDLE_REST_FRAME)
    bpy.context.view_layer.update()


def _bake_mesh(
    mesh: bpy.types.Object, arm: bpy.types.Object, head_ref: float, foot_ref: float
) -> bpy.types.Object:
    deps = bpy.context.evaluated_depsgraph_get()
    mesh_eval = mesh.evaluated_get(deps)
    ok, msg = verify_evaluated_mesh(mesh_eval, head_ref, foot_ref)
    if not ok:
        raise RuntimeError(f"FAIL staff-founder-idle pre_bake_{msg}")
    baked = bpy.data.meshes.new_from_object(mesh_eval)
    world = mesh_eval.matrix_world.copy()
    bpy.data.objects.remove(mesh, do_unlink=True)
    bpy.data.objects.remove(arm, do_unlink=True)
    ob = bpy.data.objects.new("KenneyFounder", baked)
    ob.matrix_world = world
    bpy.context.collection.objects.link(ob)
    bpy.context.view_layer.update()
    return ob


def _apply_object_xform(ob: bpy.types.Object) -> None:
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.transform_apply(rotation=True, scale=True, location=True)


def _ensure_upright_z(ob: bpy.types.Object, head_ref: float, foot_ref: float) -> None:
    ok, _ = verify_evaluated_mesh(ob, head_ref, foot_ref)
    if ok:
        return
    ob.rotation_euler = (math.pi, 0.0, 0.0)
    _apply_object_xform(ob)
    ok, msg = verify_evaluated_mesh(ob, head_ref, foot_ref)
    if not ok:
        raise RuntimeError(f"FAIL staff-founder-idle mesh_inverted: {msg}")


def _face_forward_z(ob: bpy.types.Object) -> None:
    verts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    max_z, min_z = max(v.z for v in verts), min(v.z for v in verts)
    span = max(1e-4, max_z - min_z)
    top = [v for v in verts if v.z > max_z - span * 0.2]
    bot = [v for v in verts if v.z < min_z + span * 0.2]
    if not top or not bot:
        return
    hc = Vector(
        (sum(v.x for v in top) / len(top), sum(v.y for v in top) / len(top), sum(v.z for v in top) / len(top))
    )
    nose = min(top, key=lambda v: v.y)
    fwd = Vector((nose.x - hc.x, nose.y - hc.y, 0.0))
    if fwd.length < 1e-4:
        return
    yaw = math.atan2(fwd.x, fwd.y)
    ob.rotation_euler = (0.0, 0.0, -yaw)
    _apply_object_xform(ob)


def _height_span_z(ob: bpy.types.Object) -> tuple[float, float, float]:
    zs = [(ob.matrix_world @ v.co).z for v in ob.data.vertices]
    lo, hi = min(zs), max(zs)
    return lo, hi, max(1e-4, hi - lo)


def _scale_character(src: bpy.types.Object) -> float:
    lo, hi, span = _height_span_z(src)
    scale = TARGET_HEIGHT / span
    src.scale = (scale * 0.92, scale * 0.90, scale)
    _apply_object_xform(src)
    zs = [(src.matrix_world @ v.co).z for v in src.data.vertices]
    min_z = min(zs)
    if min_z < -1e-5:
        src.location.z -= min_z
        _apply_object_xform(src)
    ok, msg = verify_evaluated_mesh(src, None, None)
    if not ok:
        raise RuntimeError(f"FAIL staff-founder-idle post_scale_{msg}")
    return _height_span_z(src)[2]


def _extract_part(
    src: bpy.types.Object,
    name: str,
    z_min: float,
    z_max: float,
    x_side: int | None = None,
    x_center: float | None = None,
) -> bpy.types.Object | None:
    me = src.data.copy()
    ob = bpy.data.objects.new(name, me)
    ob.matrix_world = src.matrix_world.copy()
    bpy.context.collection.objects.link(ob)
    bm = bmesh.new()
    bm.from_mesh(me)
    drop = []
    for v in bm.verts:
        w = ob.matrix_world @ v.co
        if w.z < z_min - 1e-5 or w.z > z_max + 1e-5:
            drop.append(v)
            continue
        if x_side is not None:
            if x_side < 0 and w.x > 0.02:
                drop.append(v)
            elif x_side > 0 and w.x < -0.02:
                drop.append(v)
        if x_center is not None and abs(w.x - x_center) > 0.14:
            drop.append(v)
    if drop:
        bmesh.ops.delete(bm, geom=drop, context="VERTS")
    if not bm.verts:
        bm.free()
        bpy.data.objects.remove(ob, do_unlink=True)
        return None
    island_faces: list[list[bmesh.types.BMFace]] = []
    visited: set[int] = set()
    for face in bm.faces:
        if face.index in visited:
            continue
        stack = [face]
        group: list[bmesh.types.BMFace] = []
        while stack:
            f = stack.pop()
            if f.index in visited:
                continue
            visited.add(f.index)
            group.append(f)
            for e in f.edges:
                for linked in e.link_faces:
                    if linked.index not in visited:
                        stack.append(linked)
        island_faces.append(group)
    if len(island_faces) > 1:
        best = max(island_faces, key=len)
        drop_faces = [f for grp in island_faces if grp is not best for f in grp]
        if drop_faces:
            bmesh.ops.delete(bm, geom=drop_faces, context="FACES")
    bmesh.ops.recalc_face_normals(bm, faces=bm.faces)
    bm.to_mesh(me)
    me.update()
    bm.free()
    ob.name = name
    return ob


def _drop_tiny_islands(ob: bpy.types.Object, *, min_verts: int) -> None:
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    islands: list[list[bmesh.types.BMVert]] = []
    seen: set[int] = set()
    for v in bm.verts:
        if v.index in seen:
            continue
        stack = [v]
        group: list[bmesh.types.BMVert] = []
        while stack:
            cur = stack.pop()
            if cur.index in seen:
                continue
            seen.add(cur.index)
            group.append(cur)
            for e in cur.link_edges:
                stack.extend(e.verts)
        islands.append(group)
    if len(islands) <= 1:
        bm.free()
        return
    keep = max(islands, key=len)
    drop = [v for grp in islands if grp is not keep for v in grp]
    if drop:
        bmesh.ops.delete(bm, geom=drop, context="VERTS")
        bm.to_mesh(ob.data)
        ob.data.update()
    bm.free()


def _split_parts(src: bpy.types.Object) -> list[bpy.types.Object]:
    lo, hi, span = _height_span_z(src)

    def band(f0: float, f1: float) -> tuple[float, float]:
        return lo + span * f0, lo + span * f1

    specs: list[tuple[str, float, float, int | None, tuple[float, float, float, float], float]] = [
        ("Sole_-1", 0.00, 0.05, -1, FOUNDER_SHOE_SOLE, 0.72),
        ("Sole_1", 0.00, 0.05, 1, FOUNDER_SHOE_SOLE, 0.72),
        ("Foot_-1", 0.04, 0.13, -1, FOUNDER_SHOE, 0.38),
        ("Foot_1", 0.04, 0.13, 1, FOUNDER_SHOE, 0.38),
        ("Leg_-1", 0.00, 0.48, -1, FOUNDER_JEANS, 0.56),
        ("Leg_1", 0.00, 0.48, 1, FOUNDER_JEANS, 0.56),
        ("Hips", 0.34, 0.52, None, FOUNDER_JEANS, 0.54),
        ("Torso", 0.38, 0.76, None, FOUNDER_HOODIE_RED, 0.58),
        ("UpperArm_-1", 0.42, 0.58, -1, FOUNDER_HOODIE_RED, 0.56),
        ("UpperArm_1", 0.42, 0.58, 1, FOUNDER_HOODIE_RED, 0.56),
        ("LowerArm_-1", 0.50, 0.68, -1, FOUNDER_SKIN, 0.50),
        ("LowerArm_1", 0.50, 0.68, 1, FOUNDER_SKIN, 0.50),
        ("Hand_-1", 0.58, 0.72, -1, FOUNDER_SKIN, 0.48),
        ("Hand_1", 0.58, 0.72, 1, FOUNDER_SKIN, 0.48),
        ("Head", 0.59, 0.86, None, FOUNDER_SKIN, 0.50),
        ("Hair", 0.80, 0.96, None, FOUNDER_HAIR, 0.48),
    ]
    parts: list[bpy.types.Object] = []
    for name, f0, f1, side, color, rough in specs:
        z0, z1 = band(f0, f1)
        ob = _extract_part(src, name, z0, z1, x_side=side)
        if not ob or len(ob.data.vertices) < 8:
            if ob:
                bpy.data.objects.remove(ob, do_unlink=True)
            continue
        bc.assign_solid(ob, color, rough)
        if name.startswith("Leg_") or name == "Hips":
            ob.scale.x *= 1.12
            bpy.context.view_layer.objects.active = ob
            ob.select_set(True)
            bpy.ops.object.transform_apply(scale=True)
        elif name == "Torso":
            ob.scale.x *= 1.18
            bpy.context.view_layer.objects.active = ob
            ob.select_set(True)
            bpy.ops.object.transform_apply(scale=True)
            _drop_tiny_islands(ob, min_verts=28)
        elif name == "Head":
            ob.scale = (0.86, 0.88, 0.93)
            bpy.context.view_layer.objects.active = ob
            ob.select_set(True)
            bpy.ops.object.transform_apply(scale=True)
            _drop_tiny_islands(ob, min_verts=24)
        elif name == "Hair":
            ob.scale = (0.84, 0.90, 0.86)
            bpy.context.view_layer.objects.active = ob
            ob.select_set(True)
            bpy.ops.object.transform_apply(scale=True)
        elif name.startswith(("Foot_", "Leg_", "Sole_")):
            _drop_tiny_islands(ob, min_verts=20)
        parts.append(ob)
    bpy.data.objects.remove(src, do_unlink=True)
    return parts


def _purge_floor_stray_meshes() -> None:
    import staff_connectivity as sc

    torso = bpy.data.objects.get("Torso")
    tcx = 0.0
    if torso:
        lo, hi = sc._world_bbox(torso)
        tcx = (lo.x + hi.x) * 0.5
    keep = frozenset(
        {
            "Sole_-1",
            "Sole_1",
            "Foot_-1",
            "Foot_1",
            "Leg_-1",
            "Leg_1",
        }
    )
    for ob in list(bpy.context.scene.objects):
        if ob.type != "MESH":
            continue
        base = ob.name.split(".")[0]
        if base in keep:
            continue
        lo, hi = sc._world_bbox(ob)
        c = (lo + hi) * 0.5
        if hi.z > 0.18:
            continue
        if c.z < 0.10 and (abs(c.x - tcx) > 0.12 or len(ob.data.vertices) < 120):
            bpy.data.objects.remove(ob, do_unlink=True)


def _snap_soles_to_feet() -> None:
    import staff_connectivity as sc

    for sx in ("-1", "1"):
        foot = bpy.data.objects.get(f"Foot_{sx}")
        sole = bpy.data.objects.get(f"Sole_{sx}")
        if not foot or not sole:
            continue
        f_lo, f_hi = sc._world_bbox(foot)
        s_lo, s_hi = sc._world_bbox(sole)
        sole.location.x += (f_lo.x + f_hi.x) * 0.5 - (s_lo.x + s_hi.x) * 0.5
        sole.location.y += (f_lo.y + f_hi.y) * 0.5 - (s_lo.y + s_hi.y) * 0.5
        sole.location.z += f_lo.z - s_hi.z - 0.002
        bpy.context.view_layer.update()


def _flip_character_yaw() -> None:
    """Face +Y in preview (Kenney bake forward is −Y until mirrored)."""
    import staff_connectivity as sc

    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        return
    lo = Vector((1e9, 1e9, 1e9))
    hi = Vector((-1e9, -1e9, -1e9))
    for ob in meshes:
        a, b = sc._world_bbox(ob)
        lo.x = min(lo.x, a.x)
        lo.y = min(lo.y, a.y)
        lo.z = min(lo.z, a.z)
        hi.x = max(hi.x, b.x)
        hi.y = max(hi.y, b.y)
        hi.z = max(hi.z, b.z)
    pivot = bpy.data.objects.new("FounderYawPivot", None)
    pivot.location = (lo + hi) * 0.5
    bpy.context.collection.objects.link(pivot)
    bpy.ops.object.select_all(action="DESELECT")
    for ob in meshes:
        ob.select_set(True)
    pivot.select_set(True)
    bpy.context.view_layer.objects.active = pivot
    bpy.ops.object.parent_set(type="OBJECT", keep_transform=True)
    pivot.rotation_euler = (0.0, 0.0, math.pi)
    bpy.context.view_layer.update()
    bpy.ops.object.select_all(action="DESELECT")
    for ob in meshes:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = meshes[0]
    bpy.ops.object.parent_clear(type="CLEAR_KEEP_TRANSFORM")
    bpy.data.objects.remove(pivot, do_unlink=True)
    bc.ground_scene_meshes()




def _trim_torso_chest_flap() -> None:
    torso = bpy.data.objects.get("Torso")
    if not torso:
        return
    import staff_connectivity as sc

    tlo, thi = sc._world_bbox(torso)
    mid_z = (tlo.z + thi.z) * 0.5
    front_y = thi.y - (thi.y - tlo.y) * 0.12
    bm = bmesh.new()
    bm.from_mesh(torso.data)
    drop = []
    for v in bm.verts:
        w = torso.matrix_world @ v.co
        if w.z > mid_z + 0.03 and abs(w.x) < 0.065 and w.y > front_y - 0.05:
            drop.append(v)
    if drop:
        bmesh.ops.delete(bm, geom=drop, context="VERTS")
        bm.to_mesh(torso.data)
        torso.data.update()
    bm.free()
    _drop_tiny_islands(torso, min_verts=24)


def _flatten_torso_front() -> None:
    torso = bpy.data.objects.get("Torso")
    if not torso:
        return
    import staff_connectivity as sc

    tlo, thi = sc._world_bbox(torso)
    span_x = thi.x - tlo.x
    span_z = thi.z - tlo.z
    cx = (tlo.x + thi.x) * 0.5
    cz = tlo.z + span_z * 0.55
    front_y = thi.y - (thi.y - tlo.y) * 0.08
    fill = bc.add_cube("TorsoFill", (span_x * 0.42, 0.055, span_z * 0.38), (cx, front_y, cz))
    bc.assign_solid(fill, FOUNDER_HOODIE_RED, 0.56)
    bpy.ops.object.select_all(action="DESELECT")
    torso.select_set(True)
    fill.select_set(True)
    bpy.context.view_layer.objects.active = torso
    bpy.ops.object.join()
    joined = bpy.context.view_layer.objects.active
    joined.name = "Torso"
    bc.assign_solid(joined, FOUNDER_HOODIE_RED, 0.58)
    _drop_tiny_islands(joined, min_verts=24)


def _baked_arm_chunky(sx: str) -> bool:
    import staff_connectivity as sc

    upper = bpy.data.objects.get(f"UpperArm_{sx}")
    lower = bpy.data.objects.get(f"LowerArm_{sx}")
    hand = bpy.data.objects.get(f"Hand_{sx}")
    torso = bpy.data.objects.get("Torso")
    if not upper or not lower or not hand or not torso:
        return False
    if min(len(upper.data.vertices), len(lower.data.vertices), len(hand.data.vertices)) < 18:
        return False
    tlo, thi = sc._world_bbox(torso)
    tcx = (tlo.x + thi.x) * 0.5
    tcy = (tlo.y + thi.y) * 0.5
    hc = sum((hand.matrix_world @ v.co for v in hand.data.vertices), Vector()) / len(hand.data.vertices)
    spread = math.hypot(hc.x - tcx, hc.y - tcy)
    if spread < 0.24:
        return False
    shoulder_z = thi.z - (thi.z - tlo.z) * 0.05
    if hc.z > shoulder_z - 0.22:
        return False
    return True


def _ensure_side_arms() -> None:
    import staff_connectivity as sc

    torso = bpy.data.objects.get("Torso")
    if not torso:
        return
    tlo, thi = sc._world_bbox(torso)
    tcx = (tlo.x + thi.x) * 0.5
    span_x = thi.x - tlo.x
    arm_y = thi.y - (thi.y - tlo.y) * 0.10
    shoulder_z = thi.z - (thi.z - tlo.z) * 0.08
    for sx, sign in (("-1", -1.0), ("1", 1.0)):
        for tag in (f"UpperArm_{sx}", f"LowerArm_{sx}", f"Hand_{sx}"):
            old = bpy.data.objects.get(tag)
            if old:
                bpy.data.objects.remove(old, do_unlink=True)
        cx = tcx + sign * span_x * 0.52
        specs = (
            (f"UpperArm_{sx}", FOUNDER_HOODIE_RED, (0.16, 0.11, 0.14), shoulder_z - 0.07),
            (f"LowerArm_{sx}", FOUNDER_SKIN, (0.11, 0.10, 0.16), shoulder_z - 0.20),
            (f"Hand_{sx}", FOUNDER_SKIN, (0.11, 0.09, 0.08), shoulder_z - 0.32),
        )
        for name, color, size, cz in specs:
            ob = bc.add_cube(name, size, (cx, arm_y, cz))
            bc.assign_solid(ob, color, 0.48)


def _add_face_features() -> None:
    for tag in ("Eye_-1", "Eye_1", "Mouth", "Eyebrows"):
        old = bpy.data.objects.get(tag)
        if old:
            bpy.data.objects.remove(old, do_unlink=True)
    head = bpy.data.objects.get("Head")
    if not head or not head.data.vertices:
        return
    verts = [head.matrix_world @ v.co for v in head.data.vertices]
    hc = sum(verts, Vector()) / len(verts)
    ys = [v.y for v in verts]
    zs = [v.z for v in verts]
    ext_y = max(ys) - min(ys)
    ext_z = max(zs) - min(zs)
    face_y = max(ys) - ext_y * 0.06
    eye_z = hc.z + ext_z * 0.12
    eye_sep = 0.17
    for name, x_sign in (("Eye_-1", -1.0), ("Eye_1", 1.0)):
        pos = Vector((hc.x + eye_sep * 0.5 * x_sign, face_y, eye_z))
        eye = bc.add_cube(name, (0.082, 0.068, 0.072), (pos.x, pos.y, eye_z))
        bc.assign_solid(eye, FOUNDER_EYE, 0.35)
    mouth_c = Vector((hc.x, face_y + ext_y * 0.01, hc.z - ext_z * 0.14))
    mouth = bc.add_cube("Mouth", (0.09, 0.05, 0.028), (mouth_c.x, mouth_c.y, mouth_c.z))
    bc.assign_solid(mouth, (0.55, 0.28, 0.30, 1.0), 0.42)


def build_founder_kenney() -> None:
    arm, mesh = _import_rigged()
    _pose_kenney_idle_arms(arm)
    _ground_rig(arm, mesh)
    ok, msg = verify_standing_rig(arm)
    if not ok:
        raise RuntimeError(f"FAIL staff-founder-idle: {msg}")
    ok, msg = verify_kenney_arm_drop(arm)
    if not ok:
        raise RuntimeError(f"FAIL staff-founder-idle: {msg}")
    refs = rig_head_foot_world_z(arm)
    if not refs:
        raise RuntimeError("FAIL staff-founder-idle rig_head_foot_missing")
    head_ref, foot_ref = refs
    baked = _bake_mesh(mesh, arm, head_ref, foot_ref)
    _ensure_upright_z(baked, head_ref, foot_ref)
    _face_forward_z(baked)
    _scale_character(baked)
    _drop_tiny_islands(baked, min_verts=40)
    parts = _split_parts(baked)
    if len(parts) < 11:
        raise RuntimeError(f"Kenney split failed parts={len(parts)}")
    bc.ground_scene_meshes()
    _flip_character_yaw()
    _snap_soles_to_feet()
    _purge_floor_stray_meshes()
    bc.ground_scene_meshes()
    _trim_torso_chest_flap()
    _flatten_torso_front()
    _ensure_side_arms()
    _add_face_features()
    for tag in ("UpperArm_-1", "UpperArm_1", "Torso", "Head"):
        ob = bpy.data.objects.get(tag)
        if ob:
            bc.smooth_shade(ob)
    for ob in bpy.context.scene.objects:
        if ob.type != "MESH":
            continue
        try:
            bc.bevel_object(ob, width=0.0025, segments=1)
        except RuntimeError:
            pass
    bc.ground_scene_meshes()
    for ob in list(bpy.context.scene.objects):
        if ob.type == "MESH" and ob.name.split(".")[0] == "Cube":
            bpy.data.objects.remove(ob, do_unlink=True)