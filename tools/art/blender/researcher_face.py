"""Researcher-only face attachments (eyes, glasses). Do not import founder_face_decals."""
from __future__ import annotations

import bpy
from mathutils import Vector

import blender_common as bc


FOUNDER_EYE_RGBA = (26 / 255, 26 / 255, 26 / 255, 1.0)
GLASSES_DARK = (0.22, 0.22, 0.24, 1.0)

EYE_OBJECT_NAMES = ("FounderEye_L", "FounderEye_R")


def _make_solid_mat(name: str, rgba: tuple[float, float, float, float], rough: float) -> bpy.types.Material:
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


def _parent_keep_world(ob: bpy.types.Object, parent: bpy.types.Object) -> None:
    world = ob.matrix_world.copy()
    ob.parent = parent
    ob.matrix_parent_inverse = parent.matrix_world.inverted()
    ob.matrix_world = world


def _purge_researcher_glasses() -> None:
    for ob in list(bpy.context.scene.objects):
        if ob.type != "MESH":
            continue
        base = ob.name.split(".")[0]
        if base == "Glasses" or base.startswith("GlassesRing_") or base == "GlassesBridge":
            bpy.data.objects.remove(ob, do_unlink=True)


def rename_pack_eyes_for_gate() -> None:
    for tag, name in (("-1", "FounderEye_L"), ("1", "FounderEye_R")):
        ob = bpy.data.objects.get(f"Eye_{tag}")
        if ob:
            ob.name = name
            bc.assign_solid(ob, FOUNDER_EYE_RGBA, 0.34)


def snap_pack_eyes_to_head(head: bpy.types.Object, span: float) -> None:
    """Place imported Eye_L/R on the +Z face (Y-up after modular rotation)."""
    import staff_connectivity as sc

    lo, hi = sc._world_bbox(head)
    center = (lo + hi) * 0.5
    ext = hi - lo
    face_z = hi.z - ext.z * 0.14
    eye_y = center.y + ext.y * 0.02
    sep = ext.x * 0.17
    proud = span * 0.012
    for tag, sign in (("-1", -1), ("1", 1)):
        eye = bpy.data.objects.get(f"Eye_{tag}") or bpy.data.objects.get(
            "FounderEye_L" if sign < 0 else "FounderEye_R"
        )
        if not eye:
            continue
        if eye.parent:
            bpy.ops.object.select_all(action="DESELECT")
            eye.select_set(True)
            bpy.context.view_layer.objects.active = eye
            bpy.ops.object.parent_clear(type="CLEAR_KEEP_TRANSFORM")
        ec = sum((eye.matrix_world @ v.co for v in eye.data.vertices), Vector()) / len(eye.data.vertices)
        target = Vector((center.x + sign * sep, eye_y, face_z + proud))
        eye.location += target - ec
        _parent_keep_world(eye, head)
    bpy.context.view_layer.update()


def ensure_researcher_eyes(head: bpy.types.Object, span: float) -> None:
    """Spawn dark eye discs on the head (pack Eye_* meshes are oversized for this rig)."""
    for tag in ("-1", "1"):
        ob = bpy.data.objects.get(f"Eye_{tag}")
        if ob:
            bpy.data.objects.remove(ob, do_unlink=True)
    for name in EYE_OBJECT_NAMES:
        ob = bpy.data.objects.get(name)
        if ob:
            bpy.data.objects.remove(ob, do_unlink=True)
    eye_dark = (0.07, 0.07, 0.07, 1.0)
    eye_mat = _make_solid_mat("ResearcherEyeMesh", eye_dark, 0.92)
    bsdf = eye_mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Emission Color"].default_value = (0.04, 0.04, 0.05, 1.0)
        bsdf.inputs["Emission Strength"].default_value = 1.2
    import staff_connectivity as sc

    lo, hi = sc._world_bbox(head)
    center = (lo + hi) * 0.5
    ext = hi - lo
    face_z = hi.z - ext.z * 0.02
    eye_y = center.y + ext.y * 0.02
    sep = ext.x * 0.48
    for name, sign in (("FounderEye_L", -1), ("FounderEye_R", 1)):
        if bpy.data.objects.get(name):
            continue
        bpy.ops.mesh.primitive_uv_sphere_add(segments=12, ring_count=8, location=(0.0, 0.0, 0.0))
        ob = bpy.context.active_object
        ob.name = name
        ob.scale = (span * 0.058, span * 0.026, span * 0.058)
        bpy.ops.object.transform_apply(scale=True)
        ob.location = (center.x + sign * sep, eye_y, face_z + span * 0.042)
        ob.data.materials.clear()
        ob.data.materials.append(eye_mat)
        bc.smooth_shade(ob)
        _parent_keep_world(ob, head)


def add_researcher_glasses(head: bpy.types.Object, span: float) -> bpy.types.Object:
    """Single dark glasses mesh on the nose bridge (not floating discs)."""
    import staff_connectivity as sc

    _purge_researcher_glasses()
    lo, hi = sc._world_bbox(head)
    center = (lo + hi) * 0.5
    ext = hi - lo
    face_z = hi.z - ext.z * 0.04
    ey_y = center.y + ext.y * 0.05
    ey_z = face_z + span * 0.012
    sep = ext.x * 0.13
    proud = span * 0.008
    parts: list[bpy.types.Object] = []
    for tag, sign in (("L", -1), ("R", 1)):
        lens = bc.add_cube(
            f"GlassesRing_{tag}",
            (span * 0.072, span * 0.018, span * 0.072),
            (center.x + sign * sep, ey_y + proud, ey_z),
        )
        bc.assign_solid(lens, GLASSES_DARK, 0.22)
        parts.append(lens)
    bridge = bc.add_cube(
        "GlassesBridge",
        (sep * 0.42, span * 0.014, span * 0.022),
        (center.x, ey_y + proud, ey_z),
    )
    bc.assign_solid(bridge, GLASSES_DARK, 0.22)
    parts.append(bridge)
    bpy.ops.object.select_all(action="DESELECT")
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = parts[0]
    bpy.ops.object.join()
    glass = bpy.context.active_object
    glass.name = "Glasses"
    bc.assign_solid(glass, GLASSES_DARK, 0.22)
    _parent_keep_world(glass, head)
    return glass