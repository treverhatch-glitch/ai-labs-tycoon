"""
CC0 lane: import incoming packs, restyle (compute role palette), bake 1024, export glTF, HDRI preview.
Run inside Blender 4.x only.
"""
from __future__ import annotations

import json
import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[4]
INCOMING = REPO / "tools/art/cc0/incoming"
OUT_ROOT = REPO / "unity/Assets/Art/Cc0/compute-kit"
PREVIEW_DIR = REPO / "docs/art/previews"
HDR_PATH = INCOMING / "polyhaven/studio_small_09_1k.hdr"
TEX_SIZE = 1024
COMPUTE_BASE = (0.55, 0.40, 0.95)
GREY150 = 150.0 / 255.0
GREY150_PREVIEW = 149.35 / 255.0
PREVIEW_FILL = 0.70


def reset_scene() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)


def import_glb(path: Path) -> list[bpy.types.Object]:
    if not path.is_file():
        raise FileNotFoundError(path)
    before = set(bpy.context.scene.objects)
    bpy.ops.import_scene.gltf(filepath=str(path))
    return [o for o in bpy.context.scene.objects if o not in before and o.type == "MESH"]


def import_obj(path: Path) -> list[bpy.types.Object]:
    if not path.is_file():
        raise FileNotFoundError(path)
    before = set(bpy.context.scene.objects)
    try:
        bpy.ops.wm.obj_import(filepath=str(path))
    except AttributeError:
        bpy.ops.import_scene.obj(filepath=str(path))
    return [o for o in bpy.context.scene.objects if o not in before and o.type == "MESH"]


def join_meshes(objects: list[bpy.types.Object]) -> bpy.types.Object:
    if not objects:
        raise RuntimeError("no meshes to join")
    bpy.ops.object.select_all(action="DESELECT")
    for ob in objects:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = objects[0]
    if len(objects) > 1:
        bpy.ops.object.join()
    return bpy.context.view_layer.objects.active


def apply_transforms(ob: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)


def bevel_object(ob: bpy.types.Object, width: float = 0.012, segments: int = 2) -> None:
    mod = ob.modifiers.new("Bevel", type="BEVEL")
    mod.width = width
    mod.segments = segments
    mod.limit_method = "ANGLE"
    mod.angle_limit = math.radians(60)
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.modifier_apply(modifier=mod.name)


def smart_uv(ob: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.uv.smart_project(angle_limit=math.radians(66), island_margin=0.02)
    bpy.ops.object.mode_set(mode="OBJECT")


def part_palette(asset_id: str) -> list[tuple[float, float, float]]:
    r, g, b = COMPUTE_BASE
    palettes = {
        "compute-server-rack": [
            (0.28, 0.30, 0.34),
            (0.38, 0.40, 0.44),
            (r * 0.65, g * 0.65, min(1.0, b * 0.7)),
            (0.18, 0.20, 0.24),
            (0.12, 0.78, 0.38),
            (0.18, 0.45, 0.92),
            (0.62, 0.64, 0.68),
        ],
        "compute-pc-tower-era1": [
            (0.84, 0.80, 0.72),
            (0.58, 0.54, 0.48),
            (0.95, 0.48, 0.12),
            (0.22, 0.78, 0.28),
            (0.76, 0.72, 0.64),
            (0.32, 0.30, 0.28),
        ],
        "compute-monitor-set": [
            (0.20, 0.22, 0.26),
            (0.88, 0.90, 0.92),
            (r * 0.75, g * 0.75, b * 0.85),
            (0.35, 0.38, 0.42),
            (0.55, 0.58, 0.62),
        ],
    }
    return palettes.get(asset_id, [COMPUTE_BASE, (r * 0.8, g * 0.85, b * 0.9)])


def paint_vertex_colors(ob: bpy.types.Object, colors: list[tuple[float, float, float]]) -> None:
    mesh = ob.data
    while mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[0])
    parts = _split_loose_parts(ob)
    for i, p in enumerate(parts):
        c = colors[i % len(colors)]
        is_led = "server-rack" in ob.name and (i % 7) in (5, 6)
        if is_led:
            c = (0.12, 0.82, 0.38) if (i % 2) == 0 else (0.15, 0.45, 0.92)
        if mesh.color_attributes.get("BakeColor") is None:
            mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
        vcol = p.data.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
        col = (c[0], c[1], c[2], 1.0)
        for corner_i in range(len(p.data.loops)):
            vcol.data[corner_i].color = col
    if len(parts) > 1:
        ob = join_meshes(parts)
    return ob


def _split_loose_parts(obj: bpy.types.Object) -> list[bpy.types.Object]:
    bpy.ops.object.select_all(action="DESELECT")
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.separate(type="LOOSE")
    bpy.ops.object.mode_set(mode="OBJECT")
    parts = [o for o in bpy.context.selected_objects if o.type == "MESH"]
    return parts or [obj]


def bake_material(ob: bpy.types.Object, asset_id: str) -> bpy.types.Material:
    mat = bpy.data.materials.new(f"M_bake_{asset_id}")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    attr = nodes.new("ShaderNodeAttribute")
    attr.attribute_name = "BakeColor"
    attr.attribute_type = "GEOMETRY"
    emit = nodes.new("ShaderNodeEmission")
    emit.inputs["Strength"].default_value = 1.0
    links.new(attr.outputs["Color"], emit.inputs["Color"])
    links.new(emit.outputs["Emission"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)
    return mat


def rasterize_vertex_colors(ob: bpy.types.Object, tex_path: Path, asset_id: str = "") -> None:
    mesh = ob.data
    attr = mesh.color_attributes.get("BakeColor")
    uv = mesh.uv_layers.active
    if attr is None or uv is None:
        raise RuntimeError("missing BakeColor or UV")
    tex_path.parent.mkdir(parents=True, exist_ok=True)
    img = bpy.data.images.new(f"T_raster_{ob.name}", TEX_SIZE, TEX_SIZE, alpha=False)
    if "pc-tower-era1" in asset_id:
        fill = (200 / 255.0, 190 / 255.0, 165 / 255.0, 1.0)
    elif "pc-tower" in asset_id:
        fill = (0.26, 0.40, 0.42, 1.0)
    else:
        fill = (0.45, 0.45, 0.48, 1.0)
    px = list(fill) * (TEX_SIZE * TEX_SIZE)
    face_domain = attr.domain == "FACE"
    for poly in mesh.polygons:
        face_c = attr.data[poly.index].color if face_domain else None
        for li in poly.loop_indices:
            u, v = uv.data[li].uv
            x = min(TEX_SIZE - 1, max(0, int(u * TEX_SIZE)))
            y = min(TEX_SIZE - 1, max(0, int((1.0 - v) * TEX_SIZE)))
            c = face_c if face_domain else attr.data[li].color
            o = (y * TEX_SIZE + x) * 4
            px[o : o + 4] = [c[0], c[1], c[2], 1.0]
    img.pixels = px
    img.filepath_raw = str(tex_path)
    img.file_format = "PNG"
    img.save()


def bake_ao(ob: bpy.types.Object, mat: bpy.types.Material, ao_path: Path) -> None:
    ao_img = bpy.data.images.new(f"T_ao_{ob.name}", TEX_SIZE, TEX_SIZE, alpha=False)
    ao_node = mat.node_tree.nodes.new("ShaderNodeTexImage")
    ao_node.image = ao_img
    mat.node_tree.nodes.active = ao_node
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.context.scene.render.engine = "CYCLES"
    bpy.context.scene.cycles.device = "CPU"
    bpy.context.scene.cycles.samples = 24
    bpy.ops.object.bake(type="AO", margin=8, use_clear=True)
    ao_img.filepath_raw = str(ao_path)
    ao_img.file_format = "PNG"
    ao_img.save()


def _era1_solid_mat(
    asset_id: str, suffix: str, rgb: tuple[float, float, float], rough: float, emission: float = 0.0
) -> bpy.types.Material:
    mat = bpy.data.materials.new(f"M_{asset_id}_{suffix}")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    col = (rgb[0], rgb[1], rgb[2], 1.0)
    bsdf.inputs["Base Color"].default_value = col
    bsdf.inputs["Roughness"].default_value = rough
    bsdf.inputs["Metallic"].default_value = 0.05
    if emission > 0 and "Emission Color" in bsdf.inputs:
        bsdf.inputs["Emission Color"].default_value = col
        bsdf.inputs["Emission Strength"].default_value = emission
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    mat.blend_method = "OPAQUE"
    mat.shadow_method = "OPAQUE"
    return mat


def urp_material_era1_tower(ob: bpy.types.Object, asset_id: str, albedo_path: Path) -> None:
    """Distinct solid slot colours (survive glTF); LEDs emissive strength 5+."""
    import pc_tower_kit as kit

    case = kit.COL_CASE
    bezel = kit.COL_BEZEL
    power = kit.COL_POWER
    led_g = kit.COL_LED_PWR
    led_a = kit.COL_LED_HDD
    ob.data.materials.clear()
    ob.data.materials.append(_era1_solid_mat(asset_id, "body", case, 0.40))
    ob.data.materials.append(_era1_solid_mat(asset_id, "bezel", bezel, 0.44))
    ob.data.materials.append(_era1_solid_mat(asset_id, "led_pwr", led_g, 0.25, emission=5.5))
    ob.data.materials.append(_era1_solid_mat(asset_id, "led_hdd", led_a, 0.25, emission=5.5))
    ob.data.materials.append(_era1_solid_mat(asset_id, "power", power, 0.46))
    kit.restore_face_material_index(ob)


def urp_material_era1_monitor(ob: bpy.types.Object, asset_id: str, albedo_path: Path) -> None:
    apply_era1_monitor_export_materials(asset_id, [ob])


def apply_era1_monitor_export_materials(asset_id: str, objects: list[bpy.types.Object]) -> None:
    import monitor_era1_kit as kit
    import pc_tower_kit as tower

    body = _era1_solid_mat(asset_id, "body", kit.COL_CASE, 0.40)
    screen = _era1_solid_mat(asset_id, "screen", kit.COL_SCREEN, 0.78, emission=1.6)
    led = _era1_solid_mat(asset_id, "led", kit.COL_LED_PWR, 0.25, emission=5.5)
    power = _era1_solid_mat(asset_id, "power", kit.COL_POWER, 0.46)
    for ob in objects:
        ob.data.materials.clear()
        if ob.name == "screen":
            ob.data.materials.append(screen)
        elif ob.name == "power_led":
            ob.data.materials.append(led)
        elif ob.name == "power_button":
            ob.data.materials.append(power)
        else:
            ob.data.materials.append(body)
    shell = next((o for o in objects if o.name == "body_tapered"), None)
    if shell and shell.data.polygons and shell.data.polygons[0].material_index > 0:
        tower.restore_face_material_index(shell)


def urp_material_era1_rack(ob: bpy.types.Object, asset_id: str, albedo_path: Path) -> None:
    apply_era1_rack_export_materials(asset_id, [ob])


def apply_era1_rack_export_materials(asset_id: str, objects: list[bpy.types.Object]) -> None:
    import rack_era1_frame_kit as kit
    import pc_tower_kit as tower

    frame_mats = [
        _era1_solid_mat(asset_id, "frame", kit.COL_FRAME, 0.48),
        _era1_solid_mat(asset_id, "rail", kit.COL_RAIL, 0.42),
        _era1_solid_mat(asset_id, "unit", kit.COL_UNIT, 0.40),
        _era1_solid_mat(asset_id, "unit_face", kit.COL_FACE, 0.44),
        _era1_solid_mat(asset_id, "led", kit.COL_LED_G, 0.25, emission=5.5),
        _era1_solid_mat(asset_id, "led_amber", kit.COL_LED_A, 0.25, emission=5.5),
        _era1_solid_mat(asset_id, "cable", kit.COL_CABLE, 0.65),
    ]
    led = frame_mats[4]
    face = frame_mats[3]
    unit = frame_mats[2]
    import rack_era1_frame_kit as rk

    bay_mat = _era1_solid_mat(asset_id, "bay", rk.COL_BAY, 0.55)
    for ob in objects:
        ob.data.materials.clear()
        if ob.name == "rack_frame":
            for m in frame_mats:
                ob.data.materials.append(m)
            tower.restore_face_material_index(ob)
        elif "led" in ob.name or "port_led" in ob.name:
            ob.data.materials.append(led)
        elif "drive_bay" in ob.name:
            ob.data.materials.append(bay_mat)
        elif "face" in ob.name or "vent" in ob.name or "slot" in ob.name:
            ob.data.materials.append(face)
        elif "side_rail" in ob.name:
            ob.data.materials.append(frame_mats[1])
        elif ob.name.startswith("unit_"):
            ob.data.materials.append(unit)
        else:
            ob.data.materials.append(frame_mats[0])


def _snap_objects_to_floor(objects: list[bpy.types.Object]) -> None:
    bpy.context.view_layer.update()
    min_z = min((ob.matrix_world @ v.co).z for ob in objects for v in ob.data.vertices)
    if abs(min_z) < 1e-6:
        return
    for ob in objects:
        ob.location.z -= min_z
    bpy.context.view_layer.update()


def _join_for_bake(objects: list[bpy.types.Object], name: str) -> bpy.types.Object:
    copies: list[bpy.types.Object] = []
    for ob in objects:
        dup = ob.copy()
        dup.data = ob.data.copy()
        bpy.context.collection.objects.link(dup)
        copies.append(dup)
    root = join_meshes(copies)
    root.name = name
    return root


def urp_material(ob: bpy.types.Object, asset_id: str, albedo_path: Path) -> None:
    mat = bpy.data.materials.new(f"M_{asset_id}")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    tex = nodes.new("ShaderNodeTexImage")
    if str(albedo_path) not in bpy.data.images:
        img = bpy.data.images.load(str(albedo_path))
    else:
        img = bpy.data.images[str(albedo_path)]
    img.colorspace_settings.name = "sRGB"
    tex.image = img
    rough = 0.38 if "pc-tower" in asset_id else 0.52
    bsdf.inputs["Roughness"].default_value = rough
    bsdf.inputs["Metallic"].default_value = 0.08 if "pc-tower" in asset_id else 0.12
    if "Alpha" in bsdf.inputs:
        bsdf.inputs["Alpha"].default_value = 1.0
    mat.blend_method = "OPAQUE"
    mat.shadow_method = "OPAQUE"
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def export_gltf_y_up(ob: bpy.types.Object, path: Path) -> None:
    export_gltf_y_up_multi([ob], path)


def _apply_all_modifiers(ob: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    for mod in list(ob.modifiers):
        try:
            bpy.ops.object.modifier_apply(modifier=mod.name)
        except RuntimeError:
            ob.modifiers.remove(mod)
    ob.data.name = ob.name


def _purge_staging_meshes(keep: set[bpy.types.Object]) -> None:
    keep_ids = {id(o) for o in keep}
    for ob in list(bpy.data.objects):
        if id(ob) in keep_ids or ob.type != "MESH":
            continue
        if ob.name.startswith("mesh_compute") or ob.name.startswith("_bake_staging"):
            bpy.data.objects.remove(ob, do_unlink=True)


def export_gltf_y_up_multi(
    objects: list[bpy.types.Object], path: Path, *, lean_topology: bool = False
) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    _purge_staging_meshes(set(objects))
    leaves: list[bpy.types.Object] = []
    for ob in objects:
        ob.parent = None
        _apply_all_modifiers(ob)
        ob.data.name = ob.name
        leaves.append(ob)
    bpy.ops.object.select_all(action="DESELECT")
    for ob in leaves:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = leaves[0]
    for img in list(bpy.data.images):
        if img.filepath and not img.packed_file:
            try:
                img.pack()
            except RuntimeError:
                bpy.data.images.remove(img)
    bpy.ops.export_scene.gltf(
        filepath=str(path),
        export_format="GLB",
        use_selection=True,
        export_apply=False,
        export_texcoords=not lean_topology,
        export_normals=not lean_topology,
        export_materials="EXPORT",
        export_image_format="AUTO",
        export_yup=True,
    )


def _world_bbox(objs: list[bpy.types.Object]) -> tuple[Vector, Vector]:
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in objs:
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins.x = min(mins.x, w.x)
            mins.y = min(mins.y, w.y)
            mins.z = min(mins.z, w.z)
            maxs.x = max(maxs.x, w.x)
            maxs.y = max(maxs.y, w.y)
            maxs.z = max(maxs.z, w.z)
    return mins, maxs


def _setup_hdri_grey_world(hdri_strength: float = 0.42) -> None:
    world = bpy.context.scene.world or bpy.data.worlds.new("World")
    bpy.context.scene.world = world
    world.use_nodes = True
    nodes = world.node_tree.nodes
    links = world.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputWorld")
    bg = nodes.new("ShaderNodeBackground")
    bg.inputs["Color"].default_value = (GREY150, GREY150, GREY150, 1.0)
    bg.inputs["Strength"].default_value = 1.0
    if HDR_PATH.is_file():
        env = nodes.new("ShaderNodeTexEnvironment")
        env.image = bpy.data.images.load(str(HDR_PATH), check_existing=True)
        mix = nodes.new("ShaderNodeMixShader")
        light = nodes.new("ShaderNodeBackground")
        light.inputs["Color"].default_value = (1, 1, 1, 1)
        light.inputs["Strength"].default_value = 0.38
        links.new(env.outputs["Color"], light.inputs["Color"])
        links.new(bg.outputs["Background"], mix.inputs[1])
        links.new(light.outputs["Background"], mix.inputs[2])
        mix.inputs["Fac"].default_value = hdri_strength
        links.new(mix.outputs["Shader"], out.inputs["Surface"])
    else:
        links.new(bg.outputs["Background"], out.inputs["Surface"])


def _setup_preview_garage_style_world(*, lit_strength: float = 0.16) -> None:
    """Match passed garage #96 hero: camera sees grey-150; HDRI lights mesh only."""
    world = bpy.context.scene.world or bpy.data.worlds.new("World")
    bpy.context.scene.world = world
    world.use_nodes = True
    nodes = world.node_tree.nodes
    links = world.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputWorld")
    g = GREY150_PREVIEW
    bg_grey = nodes.new("ShaderNodeBackground")
    bg_grey.inputs["Color"].default_value = (g, g, g, 1.0)
    bg_grey.inputs["Strength"].default_value = 1.0
    if HDR_PATH.is_file():
        env = nodes.new("ShaderNodeTexEnvironment")
        env.image = bpy.data.images.load(str(HDR_PATH), check_existing=True)
        env.image.colorspace_settings.name = "Linear Rec.709"
        bg_lit = nodes.new("ShaderNodeBackground")
        bg_lit.inputs["Strength"].default_value = lit_strength
        links.new(env.outputs["Color"], bg_lit.inputs["Color"])
        path = nodes.new("ShaderNodeLightPath")
        mix = nodes.new("ShaderNodeMixShader")
        links.new(path.outputs["Is Camera Ray"], mix.inputs["Fac"])
        links.new(bg_lit.outputs["Background"], mix.inputs[1])
        links.new(bg_grey.outputs["Background"], mix.inputs[2])
        links.new(mix.outputs["Shader"], out.inputs["Surface"])
    else:
        links.new(bg_grey.outputs["Background"], out.inputs["Surface"])


def _era1_preview_lights(center: Vector, span: float, mins: Vector) -> None:
    """Garage #96 rig @ 9ea57244 + optional camera-left fill at 12% of key."""
    bpy.ops.object.light_add(type="SUN", location=center + Vector((span * 2, -span * 2, span * 3)))
    key = bpy.context.active_object
    key_energy = 2.86
    key.data.energy = key_energy
    key.data.angle = math.radians(4)
    key.rotation_euler = (math.radians(52), 0, math.radians(35))

    bpy.ops.object.light_add(type="AREA", location=center + Vector((-span, span * 1.2, span * 1.5)))
    fill = bpy.context.active_object
    fill.data.energy = 84.0
    fill.data.size = span * 0.9
    fill.rotation_euler = (math.radians(60), 0, math.radians(25))

    bpy.ops.object.light_add(type="AREA", location=center + Vector((-span * 1.5, -span * 0.55, center.z + span * 0.4)))
    side = bpy.context.active_object
    side.data.energy = key_energy * 0.12
    side.data.size = span * 0.85
    side.rotation_euler = (math.radians(72), 0, math.radians(-12))

    g = GREY150_PREVIEW
    bpy.ops.mesh.primitive_plane_add(size=span * 1.05, location=(center.x, center.y, mins.z - 0.002))
    ground = bpy.context.active_object
    gmat = bpy.data.materials.new("Ground")
    gmat.use_nodes = True
    gmat.node_tree.nodes["Principled BSDF"].inputs["Base Color"].default_value = (g, g, g, 1)
    gmat.node_tree.nodes["Principled BSDF"].inputs["Roughness"].default_value = 0.92
    ground.data.materials.append(gmat)


def _is_era1_hero(asset_id: str) -> bool:
    return asset_id.endswith("-era1")


def _boost_era1_emissive_mats() -> None:
    for mat in bpy.data.materials:
        low = mat.name.lower()
        if not mat.use_nodes:
            continue
        bsdf = mat.node_tree.nodes.get("Principled BSDF")
        if not bsdf:
            continue
        if "led_pwr" in low:
            bsdf.inputs["Base Color"].default_value = (0.08, 0.88, 0.28, 1.0)
            if "Emission Color" in bsdf.inputs:
                bsdf.inputs["Emission Color"].default_value = (0.06, 0.92, 0.30, 1.0)
            es = bsdf.inputs.get("Emission Strength")
            if es is not None:
                es.default_value = 5.5
        elif "led_hdd" in low or ("led" in low and "pwr" not in low and "screen" not in low):
            bsdf.inputs["Base Color"].default_value = (1.0, 0.58, 0.06, 1.0)
            if "Emission Color" in bsdf.inputs:
                bsdf.inputs["Emission Color"].default_value = (0.98, 0.52, 0.05, 1.0)
            es = bsdf.inputs.get("Emission Strength")
            if es is not None:
                es.default_value = 5.5


def render_preview_from_glb(glb_path: Path, asset_id: str) -> Path:
    """3/4 ortho preview on grey ground using committed GLB."""
    import hashlib

    PREVIEW_DIR.mkdir(parents=True, exist_ok=True)
    preview = PREVIEW_DIR / f"{asset_id}.png"
    reset_scene()
    era1 = _is_era1_hero(asset_id)
    if era1:
        _setup_preview_garage_style_world(lit_strength=0.16)
        bpy.context.scene.view_settings.view_transform = "Standard"
        bpy.context.scene.view_settings.exposure = -0.35
    else:
        _setup_hdri_grey_world(0.42)
    bpy.ops.import_scene.gltf(filepath=str(glb_path))
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    for ob in mesh_obs:
        for mat in ob.data.materials:
            if mat is None:
                continue
            mat.blend_method = "OPAQUE"
            mat.shadow_method = "OPAQUE"
            if mat.use_nodes:
                bsdf = mat.node_tree.nodes.get("Principled BSDF")
                if bsdf and "Alpha" in bsdf.inputs:
                    bsdf.inputs["Alpha"].default_value = 1.0
    if era1:
        _boost_era1_emissive_mats()
    if not mesh_obs:
        raise RuntimeError(f"no mesh in {glb_path}")
    mins, maxs = _world_bbox(mesh_obs)
    center = (mins + maxs) / 2
    span = max(maxs.x - mins.x, maxs.y - mins.y, maxs.z - mins.z, 0.01)

    if era1:
        _era1_preview_lights(center, span, mins)
    else:
        floor_z = mins.z
        bpy.ops.mesh.primitive_plane_add(size=max(span * 24, 40), location=(center.x, center.y, floor_z - 0.001))
        ground = bpy.context.active_object
        gmat = bpy.data.materials.new("Ground")
        gmat.use_nodes = True
        bsdf_g = gmat.node_tree.nodes["Principled BSDF"]
        bsdf_g.inputs["Base Color"].default_value = (GREY150 * 0.92, GREY150 * 0.92, GREY150 * 0.94, 1)
        bsdf_g.inputs["Roughness"].default_value = 0.9
        ground.data.materials.append(gmat)
        ground.cycles.is_shadow_catcher = True
        bpy.ops.object.light_add(type="SUN", location=center + Vector((span * 2, -span * 2, span * 3)))
        key = bpy.context.active_object
        key.data.energy = 2.2
        key.data.angle = math.radians(4)
        bpy.ops.object.light_add(type="AREA", location=(center.x, center.y, floor_z - span * 0.05))
        fill = bpy.context.active_object
        fill.data.energy = 60
        fill.data.size = span * 3
        fill.rotation_euler = (math.radians(90), 0, 0)

    obj_h = max(maxs.z - mins.z, 0.01)
    obj_w = max(maxs.x - mins.x, maxs.y - mins.y, 0.01)
    fill_target = PREVIEW_FILL
    ortho = max(obj_h / fill_target, (obj_w * 1.12) / fill_target)

    cam_data = bpy.data.cameras.new("PreviewCam")
    cam = bpy.data.objects.new("PreviewCam", cam_data)
    bpy.context.collection.objects.link(cam)
    cam.data.type = "ORTHO"
    cam.data.ortho_scale = ortho
    dist = span * 4.0
    elev = math.radians(30)
    # Tower front = −Y; camera at (−X,−Y) shows front + right (+X) panel.
    az = math.radians(-135) if era1 else math.radians(45)
    cam.location = center + Vector(
        (
            dist * math.cos(elev) * math.cos(az),
            dist * math.cos(elev) * math.sin(az),
            dist * math.sin(elev) * 0.35 + obj_h * 0.1,
        )
    )
    direction = center - cam.location
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    bpy.context.scene.camera = cam

    bpy.context.scene.render.engine = "BLENDER_EEVEE"
    if hasattr(bpy.context.scene, "eevee"):
        ee = bpy.context.scene.eevee
        if hasattr(ee, "use_shadows"):
            ee.use_shadows = True
        if hasattr(ee, "shadow_cube_size"):
            ee.shadow_cube_size = "4096"
        if era1:
            if hasattr(ee, "use_soft_shadows"):
                ee.use_soft_shadows = True
            if hasattr(ee, "use_bloom"):
                ee.use_bloom = True
            if hasattr(ee, "bloom_intensity"):
                ee.bloom_intensity = 0.05
            if hasattr(ee, "use_gtao"):
                ee.use_gtao = True
    bpy.context.scene.render.film_transparent = False
    bpy.context.scene.render.resolution_x = 1024
    bpy.context.scene.render.resolution_y = 1024
    bpy.context.scene.render.filepath = str(preview)
    bpy.context.scene.render.image_settings.file_format = "PNG"
    bpy.ops.render.render(write_still=True)

    sha = hashlib.sha256(glb_path.read_bytes()).hexdigest()
    frame = PREVIEW_DIR / f"{asset_id}.frame.json"
    frame.write_text(
        json.dumps(
            {
                "assetId": asset_id,
                "preview": str(preview.relative_to(REPO)),
                "glb": str(glb_path.relative_to(REPO)),
                "glb_sha256": sha,
                "hdri": str(HDR_PATH.relative_to(REPO)) if HDR_PATH.is_file() else None,
                "resolution": 1024,
                "silhouetteFill": PREVIEW_FILL,
                "backdropGrey": 150,
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )
    return preview


def render_preview_front_from_glb(glb_path: Path, out_name: str) -> Path:
    """Straight-on front ortho (−Y front); grey-150 ~70% fill."""
    preview = PREVIEW_DIR / f"{out_name}.png"
    reset_scene()
    _setup_preview_garage_style_world(lit_strength=0.16)
    bpy.context.scene.view_settings.view_transform = "Standard"
    bpy.context.scene.view_settings.exposure = -0.35
    bpy.ops.import_scene.gltf(filepath=str(glb_path))
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    _boost_era1_emissive_mats()
    if not mesh_obs:
        raise RuntimeError(f"no mesh in {glb_path}")
    mins, maxs = _world_bbox(mesh_obs)
    center = (mins + maxs) / 2
    span = max(maxs.x - mins.x, maxs.y - mins.y, maxs.z - mins.z, 0.01)
    _era1_preview_lights(center, span, mins)

    obj_h = max(maxs.z - mins.z, 0.01)
    obj_w = max(maxs.x - mins.x, 0.01)
    ortho = max(obj_h / PREVIEW_FILL, obj_w / PREVIEW_FILL)

    cam_data = bpy.data.cameras.new("FrontCam")
    cam = bpy.data.objects.new("FrontCam", cam_data)
    bpy.context.collection.objects.link(cam)
    cam.data.type = "ORTHO"
    cam.data.ortho_scale = ortho
    dist = span * 4.0
    cam.location = center + Vector((0.0, -dist, center.z * 0.0 + obj_h * 0.02))
    direction = center - cam.location
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    bpy.context.scene.camera = cam

    bpy.context.scene.render.engine = "BLENDER_EEVEE"
    if hasattr(bpy.context.scene, "eevee"):
        ee = bpy.context.scene.eevee
        if hasattr(ee, "use_bloom"):
            ee.use_bloom = True
        if hasattr(ee, "use_gtao"):
            ee.use_gtao = True
    bpy.context.scene.render.resolution_x = 1024
    bpy.context.scene.render.resolution_y = 1024
    bpy.context.scene.render.filepath = str(preview)
    bpy.context.scene.render.image_settings.file_format = "PNG"
    bpy.ops.render.render(write_still=True)
    return preview


def finish_cc0_asset(
    asset_id: str,
    root: bpy.types.Object,
    sources: list[str],
    export_objects: list[bpy.types.Object] | None = None,
) -> dict:
    export_objs = list(export_objects) if export_objects else [root]
    era1 = _is_era1_hero(asset_id)
    if not era1:
        bevel_w = 0.014 if "pc-tower" in asset_id else 0.016 if asset_id.startswith("compute-") else 0.012
        bevel_object(root, width=bevel_w, segments=2)
    if era1 or "pc-tower" in asset_id:
        if export_objects:
            _snap_objects_to_floor(export_objs)
        else:
            import mesh_builders as mb

            mb.snap_root_to_floor(root)

    bake_root = _join_for_bake(export_objs, f"_bake_staging_{asset_id}") if export_objects else root
    root = bake_root
    root.name = f"_bake_staging_{asset_id}" if export_objects else f"mesh_{asset_id}"
    mesh = root.data
    has_bake = mesh.color_attributes.get("BakeColor") is not None
    if not has_bake:
        colors = part_palette(asset_id)
        parts = _split_loose_parts(root)
        for i, p in enumerate(parts):
            c = colors[i % len(colors)]
            is_led = "server-rack" in asset_id and (i % 7) in (5, 6)
            if is_led:
                c = (0.12, 0.82, 0.38) if (i % 2) == 0 else (0.15, 0.45, 0.92)
            pm = p.data
            if pm.color_attributes.get("BakeColor") is None:
                pm.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
            vcol = pm.color_attributes["BakeColor"]
            col = (c[0], c[1], c[2], 1.0)
            for corner_i in range(len(pm.loops)):
                vcol.data[corner_i].color = col
        root = join_meshes(parts) if len(parts) > 1 else parts[0]
    root.name = f"mesh_{asset_id}"
    if era1 and not export_objects:
        import pc_tower_kit as kit

        kit.store_face_material_index(root)
    bake_mat = bake_material(root, asset_id)
    smart_uv(root)

    out_dir = OUT_ROOT / asset_id
    tex_dir = out_dir / "textures"
    albedo = tex_dir / f"{asset_id}_albedo.png"
    ao = tex_dir / f"{asset_id}_ao.png"
    rasterize_vertex_colors(root, albedo, asset_id)
    bake_ao(root, bake_mat, ao)
    if asset_id == "compute-pc-tower-era1":
        urp_material_era1_tower(root, asset_id, albedo)
    elif asset_id == "compute-monitor-era1":
        if export_objects:
            apply_era1_monitor_export_materials(asset_id, export_objs)
        else:
            urp_material_era1_monitor(root, asset_id, albedo)
    elif asset_id == "compute-server-rack-era1":
        if export_objects:
            apply_era1_rack_export_materials(asset_id, export_objs)
        else:
            urp_material_era1_rack(root, asset_id, albedo)
    else:
        urp_material(root, asset_id, albedo)

    glb = out_dir / f"{asset_id}.glb"
    if export_objects:
        _purge_staging_meshes(set(export_objs))
        export_gltf_y_up_multi(
            export_objs,
            glb,
            lean_topology=(asset_id == "compute-monitor-era1"),
        )
    else:
        export_gltf_y_up(root, glb)
    if era1 and asset_id in ("compute-monitor-era1", "compute-server-rack-era1"):
        import era1_glb_selfcheck as glb_sc

        audit = glb_sc.assert_glb_audit(glb, asset_id)
        glb_sc.write_audit_json(glb, asset_id, audit)
    preview = render_preview_from_glb(glb, asset_id)
    preview_front: str | None = None
    if era1:
        pf = render_preview_front_from_glb(glb, f"{asset_id}-front")
        preview_front = str(pf.relative_to(REPO))

    result = {
        "id": asset_id,
        "path": f"Assets/Art/Cc0/compute-kit/{asset_id}/{asset_id}.glb",
        "textures": {
            "albedo": f"Assets/Art/Cc0/compute-kit/{asset_id}/textures/{asset_id}_albedo.png",
            "ao": f"Assets/Art/Cc0/compute-kit/{asset_id}/textures/{asset_id}_ao.png",
        },
        "preview": str(preview.relative_to(REPO)),
        "license": "CC0-1.0 (derivative; see docs/art/LICENSES.md)",
        "sources": sources,
        "generator": f"tools/art/cc0/blender/{asset_id.replace('-', '_')}.py",
    }
    if preview_front:
        result["previewFront"] = preview_front
    return result


def write_compute_manifest(entries: list[dict]) -> None:
    path = OUT_ROOT / "compute-manifest.json"
    OUT_ROOT.mkdir(parents=True, exist_ok=True)
    payload = {
        "lane": "compute-hardware-cc0",
        "fidelity": "cc0-restyle-bake",
        "blenderVersion": f"{bpy.app.version[0]}.{bpy.app.version[1]}.{bpy.app.version[2]}",
        "rolePalette": "RoomPurpose.Compute",
        "assets": sorted(entries, key=lambda e: e["id"]),
    }
    path.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")


def factory_glb(name: str) -> Path:
    return INCOMING / "kenney-factory-kit/Models/GLB format" / f"{name}.glb"


def furniture_obj(name: str) -> Path:
    return INCOMING / "kenney-furniture-kit/Models" / f"{name}.obj"


def quaternius_obj(name: str) -> Path:
    return (
        INCOMING
        / "quaternius-scifi-megakit/Modular SciFi MegaKit[Standard]/OBJ/Props"
        / f"{name}.obj"
    )