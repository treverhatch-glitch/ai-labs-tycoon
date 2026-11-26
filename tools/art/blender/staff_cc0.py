"""CC0 kit staff hero pipeline (Kenney rigged platformer base)."""
from __future__ import annotations

from pathlib import Path

import blender_common as bc

REPO = Path(__file__).resolve().parents[3]
KENNEY_CHARACTER = (
    REPO / "third_party/cc0/kenney/starter-kit-3d-platformer/character.glb"
)
KENNEY_COLORMAP = (
    REPO / "third_party/cc0/kenney/starter-kit-3d-platformer/Textures/colormap.png"
)

ENGINEER_SHIRT = (0.35, 0.78, 0.32, 1.0)
ENGINEER_TROUSERS = (0.22, 0.28, 0.32, 1.0)
SKIN = (0.76, 0.58, 0.44, 1.0)
HARDHAT = (0.95, 0.78, 0.12, 1.0)


def _world_bbox(ob):
    from mathutils import Vector

    corners = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    xs = [c.x for c in corners]
    ys = [c.y for c in corners]
    zs = [c.z for c in corners]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))


def _import_character() -> list:
    import bpy

    if not KENNEY_CHARACTER.is_file():
        raise FileNotFoundError(f"CC0 base missing: {KENNEY_CHARACTER}")
    bpy.ops.import_scene.gltf(filepath=str(KENNEY_CHARACTER))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    RENAME = {
        "leg-left": "Leg_-1",
        "leg-right": "Leg_1",
        "torso": "Torso",
        "arm-left": "Arm_-1",
        "arm-right": "Arm_1",
        "antenna": "KenneyAntenna",
    }
    out: list = []
    for ob in meshes:
        base = ob.name.split(".")[0]
        if base in RENAME:
            ob.name = RENAME[base]
        out.append(ob)
    _apply_object_transforms(out)
    return out


def _apply_object_transforms(mesh_obs: list) -> None:
    import bpy

    if not mesh_obs:
        return
    bpy.ops.object.select_all(action="DESELECT")
    for ob in mesh_obs:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = mesh_obs[0]
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)


def _apply_torso_colormap(torso) -> None:
    import bpy

    if not KENNEY_COLORMAP.is_file():
        bc.assign_solid(torso, SKIN, 0.55)
        return
    mat = bpy.data.materials.new("KenneyTorsoFace")
    mat.use_nodes = True
    nt = mat.node_tree
    nodes = nt.nodes
    links = nt.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    tex = nodes.new("ShaderNodeTexImage")
    tex.image = bpy.data.images.load(str(KENNEY_COLORMAP), check_existing=True)
    tex.image.colorspace_settings.name = "sRGB"
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = 0.55
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    torso.data.materials.clear()
    torso.data.materials.append(mat)


def _recolor_engineer(mesh_obs: list) -> None:
    for ob in mesh_obs:
        base = ob.name.split(".")[0]
        if base == "Torso":
            continue
        if base in ("Leg_-1", "Leg_1"):
            bc.assign_solid(ob, ENGINEER_TROUSERS, 0.58)
        elif base in ("Arm_-1", "Arm_1"):
            bc.assign_solid(ob, ENGINEER_SHIRT, 0.48)
        elif base == "KenneyAntenna":
            ob.hide_render = True
            ob.hide_viewport = True


def _remove_kenney_antenna(mesh_obs: list) -> None:
    import bpy

    for ob in list(mesh_obs):
        if ob.name.split(".")[0] != "KenneyAntenna":
            continue
        mesh_obs.remove(ob)
        bpy.data.objects.remove(ob, do_unlink=True)
        break


def _add_engineer_hard_hat(mesh_obs: list) -> None:
    import bpy

    names = {o.name.split(".")[0]: o for o in mesh_obs}
    torso = names.get("Torso")
    if not torso:
        raise RuntimeError("CC0 engineer: missing Torso for hard hat")
    hat = bc.add_cube("HardHatEng", (0.38, 0.10, 0.36), (0.0, 0.0, 0.0))
    bpy.ops.object.select_all(action="DESELECT")
    hat.select_set(True)
    torso.select_set(True)
    bpy.context.view_layer.objects.active = torso
    bpy.ops.object.parent_set(type="OBJECT", keep_transform=False)
    hat.location = (0.0, 0.22, 0.78)
    bpy.context.view_layer.update()
    bc.assign_solid(hat, HARDHAT, 0.42)
    mesh_obs.append(hat)


def _add_engineer_props(mesh_obs: list) -> list:
    """Standing pose: laptop at chest (front +Y)."""
    from mathutils import Vector

    names = {o.name.split(".")[0]: o for o in mesh_obs}
    torso = names.get("Torso")
    arm_r = names.get("Arm_1")
    if not torso:
        raise RuntimeError("CC0 engineer: missing Torso")

    import bpy

    laptop = bc.add_cube("Laptop", (0.18, 0.10, 0.03), (0.0, 0.0, 0.0))
    bpy.ops.object.select_all(action="DESELECT")
    laptop.select_set(True)
    torso.select_set(True)
    bpy.context.view_layer.objects.active = torso
    bpy.ops.object.parent_set(type="OBJECT", keep_transform=False)
    laptop.location = (0.08, 0.22, 0.38)
    bpy.context.view_layer.update()
    bc.assign_solid(laptop, (0.35, 0.38, 0.42, 1.0), 0.32)
    lap_w = laptop.matrix_world.translation
    screen = bc.add_cube(
        "LaptopScreenEmit", (0.16, 0.10, 0.008), tuple(lap_w + Vector((0, 0.07, 0.01)))
    )
    bc.assign_emission(screen, (0.18, 0.72, 0.38, 1.0), strength=2.5)
    new_parts = [laptop, screen]
    for ob in new_parts:
        mesh_obs.append(ob)
    return new_parts


def validate_cc0_upright(mesh_obs: list, asset_id: str) -> None:
    """Tallest axis Z; head/hat centroid above torso (standing)."""
    from mathutils import Vector

    skip = {"Laptop", "LaptopScreenEmit", "KenneyAntenna"}
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in mesh_obs:
        if ob.type != "MESH" or ob.name.split(".")[0] in skip:
            continue
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins.x, mins.y, mins.z = min(mins.x, w.x), min(mins.y, w.y), min(mins.z, w.z)
            maxs.x, maxs.y, maxs.z = max(maxs.x, w.x), max(maxs.y, w.y), max(maxs.z, w.z)
    span = maxs - mins
    if span.z < span.y * 0.98:
        raise RuntimeError(
            f"FAIL {asset_id}: cc0_upright lying span_z={span.z:.3f} span_y={span.y:.3f}"
        )
    if span.z < 0.35:
        raise RuntimeError(f"FAIL {asset_id}: cc0_upright too_short span_z={span.z:.3f}")
    by_name = {o.name.split(".")[0]: o for o in mesh_obs if o.type == "MESH"}
    torso = by_name.get("Torso")
    head = by_name.get("HardHatEng")
    if torso and head:
        torso_z = sum((torso.matrix_world @ Vector(c)).z for c in torso.bound_box) / 8
        head_z = max((head.matrix_world @ Vector(c)).z for c in head.bound_box)
        if head_z < torso_z + 0.04:
            raise RuntimeError(f"FAIL {asset_id}: cc0_upright head_z={head_z:.3f} torso_z={torso_z:.3f}")


def orient_staff_cc0_assembly(mesh_obs: list, asset_id: str):
    """Uniform scale only: Z-up Kenney rig grounded at z=0 (~1.72 m tall)."""
    import bpy
    import staff_connectivity as staff_conn

    validate_cc0_upright(mesh_obs, asset_id)
    root = staff_conn.parent_staff_cc0_hierarchy(mesh_obs)
    bpy.context.view_layer.update()
    min_z, max_z, sz = bc._mesh_scene_span_z(mesh_obs)  # noqa: SLF001
    if sz < 0.35:
        raise RuntimeError(f"FAIL {asset_id}: cc0 height too small z={sz:.3f}")
    target_h = 1.72
    scale = target_h / sz
    if abs(scale - 1.0) > 0.01:
        root.scale = (scale, scale, scale)
        bpy.ops.object.select_all(action="DESELECT")
        root.select_set(True)
        bpy.context.view_layer.objects.active = root
        bpy.ops.object.transform_apply(scale=True)
    bpy.context.view_layer.update()
    def scene_min_z_verts() -> float:
        from mathutils import Vector

        zmin = 1e9
        for ob in mesh_obs:
            if ob.type != "MESH":
                continue
            for v in ob.data.vertices:
                zmin = min(zmin, (ob.matrix_world @ v.co).z)
        return zmin

    for _ in range(3):
        bpy.context.view_layer.update()
        min_z = scene_min_z_verts()
        if min_z <= 0.002:
            break
        root.location.z -= min_z
    bpy.context.view_layer.update()
    min_z, max_z, sz = bc._mesh_scene_span_z(mesh_obs)  # noqa: SLF001
    _min, _max, sz = bc._mesh_scene_span_z(mesh_obs)  # noqa: SLF001
    if sz < 1.55 or sz > 2.05:
        raise RuntimeError(f"FAIL {asset_id}: cc0 height sz={sz:.3f} want 1.55-2.05")
    if abs(min_z) > 0.025:
        raise RuntimeError(f"FAIL {asset_id}: feet min_z={min_z:.4f} want ~0")
    validate_cc0_upright(mesh_obs, asset_id)
    return root


def bake_cc0_world_transforms(mesh_obs: list, root) -> None:
    """Drop export parents so glTF re-import keeps Kenney part spacing."""
    import bpy

    bpy.context.view_layer.update()
    all_obs = [o for o in mesh_obs if o.type == "MESH"]
    if root and root not in all_obs:
        all_obs.append(root)
    for ob in all_obs:
        bpy.ops.object.select_all(action="DESELECT")
        ob.select_set(True)
        bpy.context.view_layer.objects.active = ob
        if ob.parent:
            bpy.ops.object.parent_clear(type="CLEAR_KEEP_TRANSFORM")
    bpy.ops.object.select_all(action="DESELECT")
    for ob in mesh_obs:
        if ob.type != "MESH":
            continue
        ob.select_set(True)
    if mesh_obs:
        bpy.context.view_layer.objects.active = mesh_obs[0]
        bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    bpy.context.view_layer.update()


def build_engineer_cc0() -> None:
    mesh_obs = _import_character()
    torso = next((o for o in mesh_obs if o.name.startswith("Torso")), None)
    if torso:
        _apply_torso_colormap(torso)
    _recolor_engineer(mesh_obs)
    _add_engineer_hard_hat(mesh_obs)
    _remove_kenney_antenna(mesh_obs)
    _add_engineer_props(mesh_obs)
    for ob in mesh_obs:
        base = ob.name.split(".")[0]
        if base == "KenneyAntenna":
            continue
        bc.smooth_shade(ob)
        bc.bevel_object(ob, width=0.004, segments=1)