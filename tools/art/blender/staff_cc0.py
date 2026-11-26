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
        "antenna": "HardHatEng",
    }
    out: list = []
    for ob in meshes:
        base = ob.name.split(".")[0]
        if base in RENAME:
            ob.name = RENAME[base]
        out.append(ob)
    return out


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
        elif base == "HardHatEng":
            bc.assign_solid(ob, HARDHAT, 0.42)


def _add_engineer_props(mesh_obs: list) -> list:
    """Laptop + screen glow in right hand; wrench in left."""
    import bpy
    from mathutils import Vector

    names = {o.name.split(".")[0]: o for o in mesh_obs}
    torso = names.get("Torso")
    if not torso:
        raise RuntimeError("CC0 engineer: missing Torso")

    def arm_center(ob) -> Vector:
        corners = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
        return sum(corners, Vector()) / 8

    # Kenney rig: Arm_1 is on −X, Arm_-1 on +X (front toward +Y).
    r_loc = arm_center(arm_r) if arm_r else torso.matrix_world.translation + Vector((-0.30, 0, 0.35))
    l_loc = arm_center(arm_l) if arm_l else torso.matrix_world.translation + Vector((0.30, 0, 0.35))

    mid = torso.matrix_world.translation + Vector((0.0, 0.06, 0.08))
    laptop = bc.add_cube("Laptop", (0.26, 0.18, 0.04), tuple(mid + Vector((-0.10, 0, 0))))
    bc.assign_solid(laptop, (0.35, 0.38, 0.42, 1.0), 0.32)
    lap_loc = mid + Vector((-0.10, 0, 0))
    screen = bc.add_cube("LaptopScreenEmit", (0.18, 0.12, 0.008), tuple(lap_loc + Vector((0, 0.06, 0.012))))
    bc.assign_emission(screen, (0.18, 0.72, 0.38, 1.0), strength=2.5)
    wrench = bc.add_cube("Wrench", (0.06, 0.20, 0.04), tuple(mid + Vector((0.10, 0, 0))))
    bc.assign_solid(wrench, (0.72, 0.72, 0.75, 1.0), 0.28)
    belt = bc.add_cube("ToolBelt", (0.34, 0.06, 0.22), tuple(torso.matrix_world.translation + Vector((0, -0.05, 0.02))))
    bc.assign_solid(belt, (0.48, 0.32, 0.18, 1.0), 0.55)

    new_parts = [laptop, screen, wrench, belt]
    for ob in new_parts:
        mesh_obs.append(ob)
    return new_parts


def orient_staff_cc0_assembly(mesh_obs: list, asset_id: str):
    """Scale imported Z-up Kenney rig to ~1.72 m, ground feet; keep Blender Z (glTF export → +Y)."""
    import bpy
    import staff_connectivity as staff_conn

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
    min_z, max_z, sz = bc._mesh_scene_span_z(mesh_obs)  # noqa: SLF001
    if abs(min_z) > 1e-4:
        root.location.z -= min_z
    bpy.context.view_layer.update()
    _min, _max, sz = bc._mesh_scene_span_z(mesh_obs)  # noqa: SLF001
    if sz < 1.55 or sz > 2.05:
        raise RuntimeError(f"FAIL {asset_id}: cc0 height sz={sz:.3f} want 1.55-2.05")
    return root


def build_engineer_cc0() -> None:
    mesh_obs = _import_character()
    torso = next((o for o in mesh_obs if o.name.startswith("Torso")), None)
    if torso:
        _apply_torso_colormap(torso)
    _recolor_engineer(mesh_obs)
    _add_engineer_props(mesh_obs)
    for ob in mesh_obs:
        bc.smooth_shade(ob)
        bc.bevel_object(ob, width=0.004, segments=1)