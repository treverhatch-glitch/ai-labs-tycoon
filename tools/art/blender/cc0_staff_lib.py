"""Import Kenney / Quaternius rigged humanoids and split into staff-kit parts."""
from __future__ import annotations

from pathlib import Path

import bmesh
import bpy
from mathutils import Vector

import blender_common as bc

INCOMING = bc.REPO_ROOT / "tools/art/cc0/incoming"
TARGET_HEIGHT = 1.72
SKIN = (0.88, 0.72, 0.58, 1.0)


def _import_mesh(path: Path) -> bpy.types.Object:
    bpy.ops.object.select_all(action="DESELECT")
    if path.suffix.lower() == ".fbx":
        bpy.ops.import_scene.fbx(filepath=str(path))
    else:
        bpy.ops.import_scene.gltf(filepath=str(path))
    meshes = [o for o in bpy.context.selected_objects if o.type == "MESH"]
    for o in list(bpy.context.scene.objects):
        if o.type == "ARMATURE":
            bpy.data.objects.remove(o, do_unlink=True)
    if not meshes:
        meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        raise RuntimeError(f"no mesh in {path}")
    if len(meshes) > 1:
        bpy.ops.object.select_all(action="DESELECT")
        for ob in meshes:
            ob.select_set(True)
        bpy.context.view_layer.objects.active = meshes[0]
        bpy.ops.object.join()
        meshes = [bpy.context.view_layer.objects.active]
    ob = meshes[0]
    ob.rotation_euler = (1.5708, 0.0, 0.0)
    bpy.ops.object.transform_apply(rotation=True, scale=True)
    co = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    height = max(v.y for v in co) - min(v.y for v in co)
    if height < 1e-4:
        raise RuntimeError(f"degenerate height {path}")
    scale = TARGET_HEIGHT / height
    ob.scale = (scale, scale, scale)
    bpy.ops.object.transform_apply(scale=True)
    bc.ground_scene_meshes()
    ob.name = "SourceBody"
    return ob


def _world_y(ob: bpy.types.Object, vert_index: int) -> float:
    return (ob.matrix_world @ ob.data.vertices[vert_index].co).y


def _duplicate_band(
    source: bpy.types.Object,
    name: str,
    y_min: float,
    y_max: float,
    x_side: int | None = None,
) -> bpy.types.Object | None:
    if bpy.context.object and bpy.context.object.mode != "OBJECT":
        bpy.ops.object.mode_set(mode="OBJECT")
    def in_band_vert(v_co: Vector) -> bool:
        wy = (source.matrix_world @ v_co).y
        if wy < y_min - 0.02 or wy > y_max + 0.02:
            return False
        if x_side is None:
            return True
        wx = (source.matrix_world @ v_co).x
        if x_side < 0:
            return wx <= 0.04
        return wx >= -0.04

    bm_src = bmesh.new()
    bm_src.from_mesh(source.data)
    bm_src.verts.ensure_lookup_table()
    bm_src.faces.ensure_lookup_table()
    keep = {i for i, v in enumerate(bm_src.verts) if in_band_vert(v.co)}
    if len(keep) < 6:
        bm_src.free()
        return None
    faces = [f for f in bm_src.faces if any(v.index in keep for v in f.verts)]
    if not faces:
        bm_src.free()
        return None
    vert_map = sorted({v.index for f in faces for v in f.verts})
    bm_out = bmesh.new()
    v_lookup: dict[int, bmesh.types.BMVert] = {}
    for vi in vert_map:
        v_lookup[vi] = bm_out.verts.new(bm_src.verts[vi].co)
    bm_out.verts.ensure_lookup_table()
    bm_out.faces.ensure_lookup_table()
    for f in faces:
        try:
            bm_out.faces.new([v_lookup[v.index] for v in f.verts])
        except ValueError:
            pass
    bm_src.free()
    if not bm_out.faces:
        bm_out.free()
        return None
    mesh = bpy.data.meshes.new(name)
    bm_out.to_mesh(mesh)
    bm_out.free()
    mesh.update()
    ob = bpy.data.objects.new(name, mesh)
    ob.matrix_world = source.matrix_world.copy()
    bpy.context.collection.objects.link(ob)
    bc.assign_solid(ob, SKIN, 0.52)
    return ob


def _add_procedural_arms(min_y: float, span: float) -> None:
    shoulder_y = min_y + span * 0.72
    upper_len = span * 0.22
    lower_len = span * 0.18
    sw = 0.16
    for sx in (-1, 1):
        tag = f"_{sx}"
        bc.add_cube(f"Shoulder{tag}", (0.12, 0.12, 0.14), (sw * sx * 0.92, shoulder_y + 0.02, 0))
        upper_cy = shoulder_y - upper_len * 0.5 + 0.02
        bc.add_cylinder(
            f"UpperArm{tag}", 0.048, upper_len * 1.04, (sw * sx * 0.98, upper_cy, 0.02), vertices=10, axis="Y"
        )
        elbow_y = shoulder_y - upper_len + 0.01
        bc.add_cube(f"Elbow{tag}", (0.08, 0.08, 0.08), (sw * sx * 0.98, elbow_y, 0.02))
        lower_cy = elbow_y - lower_len * 0.5 + 0.01
        bc.add_cylinder(
            f"LowerArm{tag}", 0.044, lower_len * 1.03, (sw * sx * 1.02, lower_cy, 0.03), vertices=10, axis="Y"
        )
        hand_y = elbow_y - lower_len - 0.02
        hand = bc.add_cube(f"Hand{tag}", (0.085, 0.075, 0.075), (sw * sx * 1.04, hand_y + 0.03, 0.045))
        bc.assign_solid(hand, SKIN, 0.55)


def _mesh_centroid(ob: bpy.types.Object) -> Vector:
    if not ob.data.vertices:
        return ob.matrix_world.translation
    acc = Vector((0.0, 0.0, 0.0))
    for v in ob.data.vertices:
        acc += ob.matrix_world @ v.co
    return acc / len(ob.data.vertices)


def _head_metrics(source: bpy.types.Object) -> tuple[float, float]:
    co = [source.matrix_world @ Vector(c) for c in source.bound_box]
    max_y, min_y = max(v.y for v in co), min(v.y for v in co)
    span = max_y - min_y
    return max_y - span * 0.06, span


def _add_face_on_head(head_ob: bpy.types.Object) -> None:
    corners = [head_ob.matrix_world @ Vector(c) for c in head_ob.bound_box]
    cx = sum(v.x for v in corners) / len(corners)
    cy = sum(v.y for v in corners) / len(corners)
    max_z = max(v.z for v in corners)
    r = max(0.03, (max(v.y for v in corners) - min(v.y for v in corners)) * 0.12)
    eye_z = max_z - 0.01
    for sx in (-1, 1):
        ex = cx + 0.045 * sx
        eye = bc.add_cube(f"Eye_{sx}", (r, r * 0.85, r * 0.5), (ex, cy + r * 0.15, eye_z))
        bc.assign_solid(eye, (0.95, 0.95, 0.98, 1.0), 0.3)
        pupil = bc.add_cube(f"Pupil_{sx}", (r * 0.55, r * 0.55, r * 0.3), (ex, cy + r * 0.15, eye_z + 0.008))
        bc.assign_solid(pupil, (0.08, 0.1, 0.18, 1.0), 0.15)
    mouth = bc.add_cube("Mouth", (r * 1.2, r * 0.45, r * 0.35), (cx, cy - r * 0.55, eye_z - 0.005))
    bc.assign_solid(mouth, (0.78, 0.38, 0.42, 1.0), 0.45)


def _build_from_source(path: Path) -> tuple[float, float]:
    src = _import_mesh(path)
    co = [src.matrix_world @ Vector(c) for c in src.bound_box]
    min_y, max_y = min(v.y for v in co), max(v.y for v in co)
    span = max_y - min_y
    bands = [
        ("Foot_-1", min_y, min_y + span * 0.07, None),
        ("Foot_1", min_y, min_y + span * 0.07, None),
        ("Leg_-1", min_y + span * 0.05, min_y + span * 0.48, None),
        ("Leg_1", min_y + span * 0.05, min_y + span * 0.48, None),
        ("Hips", min_y + span * 0.42, min_y + span * 0.58, None),
        ("Torso", min_y + span * 0.52, min_y + span * 0.78, None),
        ("Neck", min_y + span * 0.74, min_y + span * 0.86, None),
        ("Head", min_y + span * 0.82, max_y + 0.02, None),
    ]
    parts: list[bpy.types.Object] = []
    for name, y0, y1, side in bands:
        part = _duplicate_band(src, name, y0, y1, side)
        if part:
            parts.append(part)
    bpy.data.objects.remove(src, do_unlink=True)
    for part in parts:
        try:
            bc.bevel_object(part, width=0.004, segments=1)
        except RuntimeError:
            pass
    made = len(parts)
    if made < 8:
        raise RuntimeError(f"CC0 split produced only {made} parts from {path.name}")
    _add_procedural_arms(min_y, span)
    head_ob = bpy.data.objects.get("Head")
    if not head_ob:
        raise RuntimeError("missing Head part after CC0 split")
    co = [head_ob.matrix_world @ Vector(c) for c in head_ob.bound_box]
    head_y = sum(v.y for v in co) / len(co)
    _add_face_on_head(head_ob)
    return head_y, span


def build_founder_cc0() -> None:
    kenney = INCOMING / "kenney/animated-characters-3/Model/characterMedium.fbx"
    head_y, span = _build_from_source(kenney)
    hood_y = head_y + span * 0.08
    bc.add_cube("Hood", (span * 0.22, span * 0.12, span * 0.16), (0, hood_y, 0.02))
    bc.add_cube("Hair", (span * 0.15, span * 0.04, span * 0.11), (0, hood_y + span * 0.02, 0.02))
    hand = bpy.data.objects.get("Hand_1") or bpy.data.objects.get("Torso")
    if hand:
        loc = _mesh_centroid(hand)
        mug = bc.add_cylinder(
            "FounderMug", 0.045, 0.09, (loc.x + 0.04, loc.y, loc.z + 0.05), vertices=12, axis="Y"
        )
        bc.assign_solid(mug, (0.92, 0.48, 0.12, 1.0), 0.35)
        bc.add_cube("MugHandle", (0.03, 0.06, 0.04), (loc.x + 0.075, loc.y, loc.z + 0.05))


def build_researcher_cc0() -> None:
    # Quaternius animated_human GLB is incoming CC0 reference; Kenney rig drives this hero bake
    # (Quaternius mesh failed Z-up gate in headless import — documented in manifest).
    kenney = INCOMING / "kenney/animated-characters-3/Model/characterMedium.fbx"
    head_y, span = _build_from_source(kenney)
    torso = bpy.data.objects.get("Torso")
    ty = bc.mesh_world_min_y(torso) + span * 0.08 if torso else head_y * 0.5
    bc.add_cube("LabCoat", (span * 0.2, span * 0.18, span * 0.13), (0, ty, 0.02))
    bc.add_cube("Glasses", (span * 0.12, span * 0.025, span * 0.04), (0, head_y + span * 0.01, span * 0.05))
    hand = bpy.data.objects.get("Hand_1") or bpy.data.objects.get("Torso")
    if hand:
        loc = _mesh_centroid(hand)
        clip = bc.add_cube("Clipboard", (span * 0.1, span * 0.14, span * 0.02), (loc.x + 0.03, loc.y + 0.02, loc.z + 0.04))
        bc.assign_solid(clip, (0.35, 0.38, 0.42, 1.0), 0.32)
        bc.add_cube("ClipMetal", (span * 0.035, span * 0.06, span * 0.015), (loc.x + 0.03, loc.y + span * 0.07, loc.z + 0.045))