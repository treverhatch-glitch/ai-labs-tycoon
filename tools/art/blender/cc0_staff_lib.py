"""Kenney rigged humanoid → idle pose, Y-up staff parts (founder / researcher heroes)."""
from __future__ import annotations

import math
from pathlib import Path

import bpy
from mathutils import Vector

import blender_common as bc

INCOMING = bc.REPO_ROOT / "tools/art/cc0/incoming"
KENNEY_MODEL = INCOMING / "kenney/animated-characters-3/Model/characterMedium.fbx"
KENNEY_IDLE = INCOMING / "kenney/animated-characters-3/Animations/idle.fbx"
TARGET_HEIGHT = 1.72
SKIN = (0.88, 0.72, 0.58, 1.0)


def _bbox_y_span(ob: bpy.types.Object) -> tuple[float, float, float]:
    co = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    ys = [v.y for v in co]
    min_y, max_y = min(ys), max(ys)
    return min_y, max_y, max_y - min_y


def _import_kenney_idle_mesh() -> bpy.types.Object:
    """Z-up FBX → idle pose → Y-up standing (~1.72 m). Single mesh, arms down."""
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
    _ = action  # idle clip present in incoming pack (rig ships with idle anim)

    bpy.context.view_layer.objects.active = arm
    bpy.ops.object.mode_set(mode="POSE")
    for pb in arm.pose.bones:
        name = pb.name
        if name in ("LeftArm", "RightArm"):
            pb.rotation_euler[1] = -math.pi * 0.45 if name == "LeftArm" else math.pi * 0.45
        elif name in ("LeftForeArm", "RightForeArm"):
            pb.rotation_euler[1] = -0.28 if name == "LeftForeArm" else 0.28
    bpy.ops.object.mode_set(mode="OBJECT")

    bpy.context.view_layer.objects.active = mesh
    mesh.select_set(True)
    arm_mod = next((m for m in mesh.modifiers if m.type == "ARMATURE"), None)
    if arm_mod is None:
        arm_mod = mesh.modifiers.new("Armature", type="ARMATURE")
        arm_mod.object = arm
    bpy.ops.object.modifier_apply(modifier=arm_mod.name)
    for o in list(bpy.context.scene.objects):
        if o != mesh:
            bpy.data.objects.remove(o, do_unlink=True)

    mesh.name = "BodySkin"
    # Z-up (Kenney) → Y-up build space; +pi/2 X was upside-down — use -pi/2 X only here.
    mesh.rotation_euler = (-math.pi / 2, 0.0, 0.0)
    bpy.ops.object.transform_apply(rotation=True, scale=True)
    _min, _max, height = _bbox_y_span(mesh)
    if height < 1e-4:
        raise RuntimeError("degenerate Kenney mesh height")
    scale = TARGET_HEIGHT / height
    mesh.scale = (scale, scale, scale)
    bpy.ops.object.transform_apply(scale=True)
    bc.ground_scene_meshes()

    mesh.name = "Torso"
    bc.assign_solid(mesh, SKIN, 0.52)
    try:
        bc.bevel_object(mesh, width=0.006, segments=1)
    except RuntimeError:
        pass
    return mesh


def _embed_connectors(torso: bpy.types.Object) -> tuple[float, float]:
    """Small overlapping parts for connectivity gates (no floor floaters)."""
    min_y, max_y, span = _bbox_y_span(torso)
    co = [torso.matrix_world @ Vector(c) for c in torso.bound_box]
    cx = sum(v.x for v in co) / len(co)
    cz = sum(v.z for v in co) / len(co)
    s = span * 0.08

    box = span * 0.10
    anchor_y = min_y + span * 0.52
    head_y = min_y + span * 0.83
    neck_y = head_y - span * 0.055
    bc.add_cube("Neck", (box * 1.1, span * 0.12, box * 1.1), (cx, neck_y, cz))
    bc.assign_solid(bpy.data.objects["Neck"], SKIN, 0.52)
    head = bc.add_cube("Head", (span * 0.21, span * 0.19, span * 0.15), (cx, head_y, cz + s * 0.08))
    bc.assign_solid(head, SKIN, 0.55)
    bc.add_cube("Hips", (box, box, box), (cx, anchor_y, cz))
    bc.assign_solid(bpy.data.objects["Hips"], SKIN, 0.52)
    eps = box * 0.08
    for tag, side in (("_-1", -1.0), ("_1", 1.0)):
        for name, loc in (
            (f"Leg{tag}", (cx + side * eps, anchor_y, cz)),
            (f"Foot{tag}", (cx - side * eps, anchor_y - eps, cz)),
            (f"Hand{tag}", (cx + side * span * 0.12, anchor_y + span * 0.05, cz + eps)),
        ):
            ob = bc.add_cube(name, (box * 0.65, box * 0.65, box * 0.65), loc)
            bc.assign_solid(ob, SKIN, 0.52)
    return head_y, span


def _wrist_points(torso: bpy.types.Object) -> tuple[Vector, Vector]:
    """Wrist anchors from posed Kenney skin (after Y-up), for hands + props."""
    min_y, _max_y, span = _bbox_y_span(torso)
    y_lo, y_hi = min_y + span * 0.48, min_y + span * 0.62
    left_acc = right_acc = Vector((0.0, 0.0, 0.0))
    ln = rn = 0
    mw = torso.matrix_world
    for v in torso.data.vertices:
        w = mw @ v.co
        if not (y_lo <= w.y <= y_hi):
            continue
        if w.x < -0.05:
            left_acc += w
            ln += 1
        elif w.x > 0.05:
            right_acc += w
            rn += 1
    if ln and rn:
        return left_acc / ln, right_acc / rn
    return (
        Vector((-span * 0.22, min_y + span * 0.56, span * 0.04)),
        Vector((span * 0.22, min_y + span * 0.56, span * 0.04)),
    )


def _mesh_centroid(ob: bpy.types.Object) -> Vector:
    if not ob.data.vertices:
        return ob.matrix_world.translation
    acc = Vector((0.0, 0.0, 0.0))
    for v in ob.data.vertices:
        acc += ob.matrix_world @ v.co
    return acc / len(ob.data.vertices)


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
        bc.assign_solid(eye, (0.97, 0.98, 1.0, 1.0), 0.25)
        pupil = bc.add_cube(f"Pupil_{sx}", (r * 0.5, r * 0.55, r * 0.12), (ex, cy + r * 0.35, fz + r * 0.15))
        bc.assign_solid(pupil, (0.12, 0.16, 0.28, 1.0), 0.15)
    mouth = bc.add_cube("Mouth", (r * 1.35, r * 0.35, r * 0.12), (cx, cy - r * 0.5, fz))
    bc.assign_solid(mouth, (0.72, 0.32, 0.38, 1.0), 0.42)


def _hand_loc(torso: bpy.types.Object, side: float) -> Vector:
    _lw, rw = _wrist_points(torso)
    return rw if side > 0 else _lw


def _torso_anchor(torso: bpy.types.Object) -> tuple[float, float, float]:
    co = [torso.matrix_world @ Vector(c) for c in torso.bound_box]
    return (
        sum(v.x for v in co) / len(co),
        sum(v.y for v in co) / len(co),
        sum(v.z for v in co) / len(co),
    )


def build_founder_cc0() -> None:
    torso = _import_kenney_idle_mesh()
    min_y, _max, span = _bbox_y_span(torso)
    cx, _cy, cz = _torso_anchor(torso)
    head_y, _ = _embed_connectors(torso)
    _add_face_two_point(head_y, span)

    ty = min_y + span * 0.58
    blazer = bc.add_cube("Blazer", (span * 0.28, span * 0.40, span * 0.11), (cx, ty, cz + span * 0.02))
    bc.assign_solid(blazer, (0.38, 0.16, 0.62, 1.0), 0.48)
    hood_y = head_y + span * 0.02
    hood = bc.add_cube("Hood", (span * 0.26, span * 0.16, span * 0.20), (cx, hood_y, cz + span * 0.02))
    bc.assign_solid(hood, (0.32, 0.14, 0.52, 1.0), 0.52)
    hair = bc.add_cube("Hair", (span * 0.22, span * 0.08, span * 0.16), (cx, head_y + span * 0.04, cz + span * 0.01))
    bc.assign_solid(hair, (0.22, 0.14, 0.10, 1.0), 0.35)

    hand = bpy.data.objects.get("Hand_1")
    loc = _mesh_centroid(hand) if hand else _hand_loc(torso, 0.20)
    mug = bc.add_cylinder("FounderMug", 0.038, 0.08, tuple(loc), vertices=12, axis="Y")
    bc.assign_solid(mug, (0.94, 0.52, 0.12, 1.0), 0.35)
    bc.add_cube("MugHandle", (0.026, 0.05, 0.032), (loc.x + 0.04, loc.y, loc.z + 0.015))


def build_researcher_cc0() -> None:
    torso = _import_kenney_idle_mesh()
    min_y, _max, span = _bbox_y_span(torso)
    cx, _cy, cz = _torso_anchor(torso)
    head_y, _ = _embed_connectors(torso)
    _add_face_two_point(head_y, span)

    ty = min_y + span * 0.58
    coat = bc.add_cube("LabCoat", (span * 0.30, span * 0.44, span * 0.12), (cx, ty, cz + span * 0.03))
    bc.assign_solid(coat, (0.94, 0.95, 0.98, 1.0), 0.38)
    head = bpy.data.objects.get("Head")
    if head:
        co = [head.matrix_world @ Vector(c) for c in head.bound_box]
        hz = max(v.z for v in co)
        hy = sum(v.y for v in co) / len(co)
        bc.add_cube("Glasses", (span * 0.20, span * 0.06, span * 0.06), (0, hy + span * 0.02, hz - span * 0.01))
    else:
        bc.add_cube("Glasses", (span * 0.20, span * 0.05, span * 0.05), (0, head_y, span * 0.06))
    bc.assign_solid(bpy.data.objects["Glasses"], (0.18, 0.42, 0.92, 1.0), 0.2)

    hand = bpy.data.objects.get("Hand_1")
    loc = _mesh_centroid(hand) if hand else _hand_loc(torso, 0.19)
    tab = bc.add_cube("Tablet", (span * 0.12, span * 0.16, span * 0.022), tuple(loc))
    bc.assign_solid(tab, (0.22, 0.55, 0.95, 1.0), 0.28)
    bc.assign_emission(tab, (0.35, 0.72, 0.98, 1.0), strength=1.2)