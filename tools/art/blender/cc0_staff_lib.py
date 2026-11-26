"""Kenney rigged humanoid → idle pose, Y-up staff parts (founder / researcher heroes)."""
from __future__ import annotations

import math

import bpy
import bmesh
from mathutils import Vector

import blender_common as bc

INCOMING = bc.REPO_ROOT / "tools/art/cc0/incoming"
KENNEY_MODEL = INCOMING / "kenney/animated-characters-3/Model/characterMedium.fbx"
KENNEY_IDLE = INCOMING / "kenney/animated-characters-3/Animations/idle.fbx"
TARGET_HEIGHT = 1.72
FOUNDER_SKIN = (0.82, 0.66, 0.52, 1.0)
RESEARCHER_SKIN = (0.58, 0.72, 0.86, 1.0)


def _bbox_y_span(ob: bpy.types.Object) -> tuple[float, float, float]:
    co = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    ys = [v.y for v in co]
    min_y, max_y = min(ys), max(ys)
    return min_y, max_y, max_y - min_y


def _trim_tpose_arms(mesh: bpy.types.Object, min_y: float, span: float) -> None:
    """Remove Kenney horizontal arm tubes; idle arms are rebuilt as parts."""
    y_lo = min_y + span * 0.48
    y_hi = min_y + span * 0.82
    half_w = span * 0.16
    mw = mesh.matrix_world
    bm = bmesh.new()
    bm.from_mesh(mesh.data)
    bm.verts.ensure_lookup_table()
    drop = [
        v.index
        for v in bm.verts
        if y_lo <= (mw @ v.co).y <= y_hi and abs((mw @ v.co).x) > half_w
    ]
    if drop:
        bmesh.ops.delete(bm, geom=[bm.verts[i] for i in drop], context="VERTS")
    bm.to_mesh(mesh.data)
    mesh.data.update()
    bm.free()


def _import_kenney_idle_mesh(skin: tuple[float, float, float, float]) -> bpy.types.Object:
    """Z-up FBX → Y-up standing (~1.72 m) torso/legs; idle arms added later."""
    bpy.ops.object.select_all(action="DESELECT")
    bpy.ops.import_scene.fbx(filepath=str(KENNEY_MODEL))
    arm = next((o for o in bpy.context.scene.objects if o.type == "ARMATURE"), None)
    if not arm:
        raise RuntimeError("Kenney model missing armature")
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    mesh = next(
        (o for o in meshes if o.name == "characterMedium"),
        max(meshes, key=lambda o: len(o.data.vertices), default=None),
    )
    if not mesh:
        raise RuntimeError("Kenney model missing skin mesh")
    for o in list(bpy.context.scene.objects):
        if o.type == "MESH" and o != mesh:
            bpy.data.objects.remove(o, do_unlink=True)

    bpy.ops.import_scene.fbx(filepath=str(KENNEY_IDLE))
    action = next(
        (a for a in bpy.data.actions if "Idle" in a.name and "Targeting" not in a.name),
        None,
    )
    if not action:
        raise RuntimeError("Kenney idle animation not found")
    _ = action

    arm_mod = next((m for m in mesh.modifiers if m.type == "ARMATURE"), None)
    if arm_mod is None:
        arm_mod = mesh.modifiers.new("Armature", type="ARMATURE")
        arm_mod.object = arm
    bpy.context.view_layer.objects.active = mesh
    mesh.select_set(True)
    bpy.ops.object.modifier_apply(modifier=arm_mod.name)
    for o in list(bpy.context.scene.objects):
        if o != mesh:
            bpy.data.objects.remove(o, do_unlink=True)

    mesh.rotation_euler = (-math.pi / 2, 0.0, 0.0)
    bpy.ops.object.transform_apply(rotation=True, scale=True)
    _min, _max, height = _bbox_y_span(mesh)
    if height < 1e-4:
        raise RuntimeError("degenerate Kenney mesh height")
    scale = TARGET_HEIGHT / height
    mesh.scale = (scale, scale, scale)
    bpy.ops.object.transform_apply(scale=True)
    bc.ground_scene_meshes()

    min_y, _, span = _bbox_y_span(mesh)
    _trim_tpose_arms(mesh, min_y, span)

    mesh.name = "Torso"
    bc.assign_solid(mesh, skin, 0.52)
    try:
        bc.bevel_object(mesh, width=0.006, segments=1)
    except RuntimeError:
        pass
    return mesh


def _torso_anchor(torso: bpy.types.Object) -> tuple[float, float, float]:
    co = [torso.matrix_world @ Vector(c) for c in torso.bound_box]
    return (
        sum(v.x for v in co) / len(co),
        sum(v.y for v in co) / len(co),
        sum(v.z for v in co) / len(co),
    )


def _add_idle_arms(
    min_y: float,
    span: float,
    cx: float,
    cz: float,
    skin: tuple[float, float, float, float],
) -> tuple[Vector, Vector]:
    shoulder_y = min_y + span * 0.68
    upper_len = span * 0.13
    lower_len = span * 0.11
    hand_l = hand_r = None
    for side in ("-1", "1"):
        sign = -1 if side == "-1" else 1
        ax = cx + sign * span * 0.06
        bc.add_cube(
            f"Shoulder_{side}",
            (span * 0.11, span * 0.11, span * 0.11),
            (ax, shoulder_y, cz + span * 0.03),
        )
        bc.assign_solid(bpy.data.objects[f"Shoulder_{side}"], skin, 0.52)
        upper_cy = shoulder_y - upper_len * 0.45
        bc.add_cube(
            f"UpperArm_{side}",
            (span * 0.09, upper_len, span * 0.09),
            (ax, upper_cy, cz + span * 0.04),
        )
        bc.assign_solid(bpy.data.objects[f"UpperArm_{side}"], skin, 0.52)
        elbow_y = upper_cy - upper_len * 0.29
        bc.add_cube(
            f"Elbow_{side}",
            (span * 0.08, span * 0.08, span * 0.08),
            (ax, elbow_y, cz + span * 0.04),
        )
        bc.assign_solid(bpy.data.objects[f"Elbow_{side}"], skin, 0.52)
        lower_cy = elbow_y - lower_len * 0.26
        bc.add_cube(
            f"LowerArm_{side}",
            (span * 0.085, lower_len, span * 0.085),
            (ax, lower_cy, cz + span * 0.04),
        )
        bc.assign_solid(bpy.data.objects[f"LowerArm_{side}"], skin, 0.52)
        hy = lower_cy - lower_len * 0.28
        bc.add_cube(
            f"Hand_{side}",
            (span * 0.085, span * 0.075, span * 0.075),
            (ax, hy, cz + span * 0.04),
        )
        bc.assign_solid(bpy.data.objects[f"Hand_{side}"], skin, 0.55)
        wrist = Vector((ax, hy, cz + span * 0.04))
        if sign < 0:
            hand_l = wrist
        else:
            hand_r = wrist
    assert hand_l and hand_r
    return hand_l, hand_r


def _embed_connectors(
    torso: bpy.types.Object,
    skin: tuple[float, float, float, float],
) -> tuple[float, float]:
    min_y, _, span = _bbox_y_span(torso)
    cx, _, cz = _torso_anchor(torso)
    box = span * 0.06
    head_y = min_y + span * 0.83
    neck_y = head_y - span * 0.05
    anchor_y = min_y + span * 0.42
    leg_box = span * 0.11
    hip_leg_y = anchor_y - leg_box * 0.25
    foot_y = hip_leg_y - leg_box * 0.42

    bc.add_cube("Neck", (box, span * 0.10, box), (cx, neck_y, cz))
    bc.assign_solid(bpy.data.objects["Neck"], skin, 0.52)
    head = bc.add_cube(
        "Head", (span * 0.21, span * 0.19, span * 0.15), (cx, head_y, cz + span * 0.06)
    )
    bc.assign_solid(head, skin, 0.55)
    bc.add_cube("Hips", (box * 1.4, box * 1.6, box * 1.4), (cx, anchor_y, cz))
    bc.assign_solid(bpy.data.objects["Hips"], skin, 0.52)

    for tag, side in (("_-1", -1.0), ("_1", 1.0)):
        bx = cx + side * span * 0.04
        for name, loc in (
            (f"Leg{tag}", (bx, hip_leg_y, cz)),
            (f"Foot{tag}", (bx, foot_y, cz)),
        ):
            ob = bc.add_cube(name, (leg_box, leg_box, leg_box), loc)
            bc.assign_solid(ob, skin, 0.52)
    return head_y, span


def _add_face_two_point(head_y: float, span: float) -> None:
    head = bpy.data.objects.get("Head")
    if not head:
        return
    co = [head.matrix_world @ Vector(c) for c in head.bound_box]
    cx = sum(v.x for v in co) / len(co)
    cy = sum(v.y for v in co) / len(co)
    fz = max(v.z for v in co) - span * 0.005
    r = max(0.035, span * 0.042)
    for sx in (-1, 1):
        ex = cx + r * 1.05 * sx
        eye = bc.add_cube(f"Eye_{sx}", (r * 1.0, r * 0.85, r * 0.22), (ex, cy + r * 0.28, fz))
        bc.assign_solid(eye, (0.97, 0.98, 1.0, 1.0), 0.28)
        pupil = bc.add_cube(f"Pupil_{sx}", (r * 0.5, r * 0.55, r * 0.12), (ex, cy + r * 0.35, fz + r * 0.15))
        bc.assign_solid(pupil, (0.12, 0.16, 0.28, 1.0), 0.18)
    mouth = bc.add_cube("Mouth", (r * 1.35, r * 0.35, r * 0.12), (cx, cy - r * 0.5, fz))
    bc.assign_solid(mouth, (0.72, 0.32, 0.38, 1.0), 0.42)


def build_founder_cc0() -> None:
    torso = _import_kenney_idle_mesh(FOUNDER_SKIN)
    min_y, _, span = _bbox_y_span(torso)
    cx, torso_y, cz = _torso_anchor(torso)
    head_y, _ = _embed_connectors(torso, FOUNDER_SKIN)
    _, hand_r = _add_idle_arms(min_y, span, cx, cz, FOUNDER_SKIN)
    _add_face_two_point(head_y, span)

    bc.add_cube(
        "Blazer",
        (span * 0.34, span * 0.46, span * 0.22),
        (cx, torso_y, cz + span * 0.08),
    )
    bc.assign_solid(bpy.data.objects["Blazer"], (0.36, 0.12, 0.58, 1.0), 0.52)
    bc.add_cube(
        "Hood",
        (span * 0.30, span * 0.18, span * 0.24),
        (cx, head_y + span * 0.05, cz + span * 0.04),
    )
    bc.assign_solid(bpy.data.objects["Hood"], (0.28, 0.10, 0.48, 1.0), 0.48)
    hair = bc.add_cube(
        "Hair",
        (span * 0.24, span * 0.10, span * 0.18),
        (cx, head_y + span * 0.06, cz + span * 0.02),
    )
    bc.assign_solid(hair, (0.18, 0.10, 0.08, 1.0), 0.38)

    bc.add_cube(
        "FounderMug",
        (span * 0.09, span * 0.11, span * 0.08),
        (hand_r.x + span * 0.05, hand_r.y, hand_r.z + span * 0.02),
    )
    bc.assign_solid(bpy.data.objects["FounderMug"], (0.94, 0.52, 0.12, 1.0), 0.38)


def build_researcher_cc0() -> None:
    torso = _import_kenney_idle_mesh(RESEARCHER_SKIN)
    min_y, _, span = _bbox_y_span(torso)
    cx, torso_y, cz = _torso_anchor(torso)
    head_y, _ = _embed_connectors(torso, RESEARCHER_SKIN)
    _, hand_r = _add_idle_arms(min_y, span, cx, cz, RESEARCHER_SKIN)
    _add_face_two_point(head_y, span)

    coat = bc.add_cube(
        "LabCoat",
        (span * 0.36, span * 0.50, span * 0.24),
        (cx, torso_y, cz + span * 0.10),
    )
    bc.assign_solid(coat, (0.95, 0.96, 0.98, 1.0), 0.42)

    head = bpy.data.objects.get("Head")
    hy, hz = head_y, span * 0.08
    if head:
        co = [head.matrix_world @ Vector(c) for c in head.bound_box]
        hy = sum(v.y for v in co) / len(co)
        hz = max(v.z for v in co)
    glasses = bc.add_cube(
        "Glasses",
        (span * 0.22, span * 0.07, span * 0.07),
        (cx, hy + span * 0.02, hz - span * 0.01),
    )
    bc.assign_solid(glasses, (0.12, 0.38, 0.88, 1.0), 0.28)

    tab = bc.add_cube(
        "Tablet",
        (span * 0.14, span * 0.18, span * 0.022),
        (hand_r.x + span * 0.04, hand_r.y, hand_r.z + span * 0.03),
    )
    bc.assign_solid(tab, (0.18, 0.50, 0.92, 1.0), 0.30)
    bc.assign_emission(tab, (0.35, 0.72, 0.98, 1.0), strength=1.2)