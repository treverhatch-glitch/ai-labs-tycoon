"""
CC0 import → restyle → bake → export helpers (headless Blender).
"""
from __future__ import annotations

import json
import math
from pathlib import Path

import bpy
from mathutils import Vector

REPO_ROOT = Path(__file__).resolve().parents[3]
INCOMING = REPO_ROOT / "tools/art/cc0/incoming"
ART_ROOT = REPO_ROOT / "unity/Assets/Art/EarlyGame"
PREVIEW_ROOT = REPO_ROOT / "docs/art/previews"
HDRI_PATH = INCOMING / "polyhaven" / "studio_small_08_1k.hdr"
COLORMAP = INCOMING / "kenney_building-kit/Models/GLB format/Textures/colormap.png"
BLENDER_VERSION = f"{bpy.app.version[0]}.{bpy.app.version[1]}.{bpy.app.version[2]}"

PALETTE = {
    "floor": (0.50, 0.48, 0.45, 1.0),
    "wall": (0.92, 0.58, 0.28, 1.0),
    "trim": (0.88, 0.48, 0.18, 1.0),
    "metal": (0.48, 0.62, 0.88, 1.0),
    "glass": (0.55, 0.75, 0.88, 1.0),
    "wood": (0.72, 0.44, 0.20, 1.0),
    "prop": (0.78, 0.58, 0.38, 1.0),
    "board": (0.92, 0.90, 0.86, 1.0),
    "screen": (0.22, 0.48, 0.88, 1.0),
    "lamp_accent": (0.95, 0.58, 0.12, 1.0),
    "box": (0.78, 0.62, 0.38, 1.0),
    "box_open": (0.72, 0.56, 0.34, 1.0),
    "chair": (0.32, 0.52, 0.78, 1.0),
    "shelf": (0.78, 0.62, 0.38, 1.0),
    "plant": (0.22, 0.72, 0.32, 1.0),
    "floor_stain": (0.06, 0.055, 0.05, 1.0),
}

GARAGE_KIT = ART_ROOT / "garage-kit"
CORNER_RGB = (150, 150, 150)


def _srgb_byte_to_linear(byte: int) -> float:
    c = byte / 255.0
    if c <= 0.04045:
        return c / 12.92
    return ((c + 0.055) / 1.055) ** 2.4


BACKDROP_GREY = _srgb_byte_to_linear(150)


def garage_glb(name: str) -> Path:
    return GARAGE_KIT / f"{name}.glb"


def reset_scene() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.context.scene.render.engine = "CYCLES"
    bpy.context.scene.cycles.device = "CPU"
    bpy.context.scene.cycles.samples = 64
    bpy.context.scene.unit_settings.system = "METRIC"
    bpy.context.scene.unit_settings.scale_length = 1.0


def kenney_glb(name: str) -> Path:
    return INCOMING / "kenney_building-kit/Models/GLB format" / name


def kenney_furniture_glb(name: str) -> Path:
    return INCOMING / "kenney_furniture-kit/Models/GLB format" / name


def import_gltf(path: Path, name: str) -> list[bpy.types.Object]:
    before = set(bpy.context.scene.objects)
    bpy.ops.import_scene.gltf(filepath=str(path))
    imported = [o for o in bpy.context.scene.objects if o not in before and o.type == "MESH"]
    for ob in imported:
        ob.name = name if len(imported) == 1 else f"{name}_{ob.name}"
    return imported


def add_metric_box(
    name: str,
    size: tuple[float, float, float],
    location: tuple[float, float, float],
    role: str | None = None,
) -> bpy.types.Object:
    """Axis-aligned box with applied scale (Blender Y-up)."""
    bpy.ops.mesh.primitive_cube_add(size=1, location=location)
    ob = bpy.context.active_object
    ob.name = name
    ob.scale = (size[0] / 2, size[1] / 2, size[2] / 2)
    bpy.ops.object.transform_apply(scale=True)
    if role:
        apply_palette_material(ob, role)
    return ob


def world_bbox(ob: bpy.types.Object) -> tuple[Vector, Vector]:
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for corner in ob.bound_box:
        w = ob.matrix_world @ Vector(corner)
        mins.x = min(mins.x, w.x)
        mins.y = min(mins.y, w.y)
        mins.z = min(mins.z, w.z)
        maxs.x = max(maxs.x, w.x)
        maxs.y = max(maxs.y, w.y)
        maxs.z = max(maxs.z, w.z)
    return mins, maxs


def snap_to_floor(ob: bpy.types.Object, floor_top: float = 0.0) -> float:
    bpy.context.view_layer.update()
    mins, _ = world_bbox(ob)
    delta = floor_top - mins.y
    ob.location.y += delta
    return delta


def weld_mesh(ob: bpy.types.Object, threshold: float = 0.0008) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=threshold)
    bpy.ops.object.mode_set(mode="OBJECT")


def join_welded(
    objects: list[bpy.types.Object],
    name: str,
    role: str | None = None,
) -> bpy.types.Object:
    ob = merge_mesh_objects(objects, name)
    weld_mesh(ob)
    if role:
        apply_palette_material(ob, role)
    return ob


def merge_mesh_objects(objects: list[bpy.types.Object], name: str) -> bpy.types.Object:
    if not objects:
        raise RuntimeError(f"no meshes for {name}")
    bpy.ops.object.select_all(action="DESELECT")
    for ob in objects:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = objects[0]
    if len(objects) > 1:
        bpy.ops.object.join()
    root = bpy.context.view_layer.objects.active
    root.name = name
    return root


def bevel_object(ob: bpy.types.Object, width: float = 0.018, segments: int = 2) -> None:
    mod = ob.modifiers.new("Bevel", type="BEVEL")
    mod.width = width
    mod.segments = segments
    mod.limit_method = "ANGLE"
    mod.angle_limit = math.radians(50)
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.modifier_apply(modifier=mod.name)


def load_image(path: Path) -> bpy.types.Image | None:
    if not path.is_file():
        return None
    return bpy.data.images.load(str(path), check_existing=True)


def apply_palette_material(ob: bpy.types.Object, role: str) -> None:
    """Solid role tints (+ optional concrete floor) so exports are readable, not palette noise."""
    tint = PALETTE.get(role, PALETTE["prop"])
    mat = bpy.data.materials.new(name=f"m_{ob.name}_{role}")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    concrete = INCOMING / "ambientcg-Concrete015/Concrete015_2K-JPG_Color.jpg"
    if role == "floor" and concrete.is_file():
        tex = nodes.new("ShaderNodeTexImage")
        tex.image = load_image(concrete)
        mapping = nodes.new("ShaderNodeMapping")
        mapping.inputs["Scale"].default_value = (2.8, 2.8, 2.8)
        coord = nodes.new("ShaderNodeTexCoord")
        links.new(coord.outputs["Object"], mapping.inputs["Vector"])
        links.new(mapping.outputs["Vector"], tex.inputs["Vector"])
        warm = nodes.new("ShaderNodeMixRGB")
        warm.blend_type = "OVERLAY"
        bright = nodes.new("ShaderNodeMixRGB")
        bright.blend_type = "MIX"
        bright.inputs["Fac"].default_value = 0.55
        bright.inputs["Color2"].default_value = (0.88, 0.86, 0.84, 1.0)
        warm = nodes.new("ShaderNodeMixRGB")
        warm.blend_type = "OVERLAY"
        warm.inputs["Fac"].default_value = 0.22
        warm.inputs["Color2"].default_value = (0.72, 0.52, 0.34, 1.0)
        stain = nodes.new("ShaderNodeTexNoise")
        stain.inputs["Scale"].default_value = 4.0
        stain.inputs["Detail"].default_value = 6.0
        stain_map = nodes.new("ShaderNodeMapRange")
        stain_map.inputs["From Min"].default_value = 0.35
        stain_map.inputs["From Max"].default_value = 0.7
        stain_map.inputs["To Min"].default_value = 0.88
        stain_map.inputs["To Max"].default_value = 1.0
        links.new(stain.outputs["Fac"], stain_map.inputs["Value"])
        stain_mix = nodes.new("ShaderNodeMixRGB")
        stain_mix.blend_type = "MULTIPLY"
        stain_mix.inputs["Color2"].default_value = (0.55, 0.52, 0.50, 1.0)
        links.new(tex.outputs["Color"], bright.inputs["Color1"])
        links.new(bright.outputs["Color"], warm.inputs["Color1"])
        links.new(warm.outputs["Color"], stain_mix.inputs["Color1"])
        stain_fac = nodes.new("ShaderNodeMath")
        stain_fac.operation = "MULTIPLY"
        stain_fac.inputs[1].default_value = 0.28
        links.new(stain_map.outputs["Result"], stain_fac.inputs[0])
        links.new(stain_fac.outputs["Value"], stain_mix.inputs["Fac"])
        links.new(stain_mix.outputs["Color"], bsdf.inputs["Base Color"])
        bsdf.inputs["Roughness"].default_value = 0.84
    elif role == "floor_stain":
        bsdf.inputs["Base Color"].default_value = (0.04, 0.038, 0.035, 1.0)
        bsdf.inputs["Roughness"].default_value = 0.97
    elif role in ("box", "box_open"):
        bsdf.inputs["Base Color"].default_value = (0.88, 0.68, 0.42, 1.0)
        bsdf.inputs["Roughness"].default_value = 0.78
        wr = nodes.new("ShaderNodeTexNoise")
        wr.inputs["Scale"].default_value = 18.0
        wr.inputs["Detail"].default_value = 4.0
        mix = nodes.new("ShaderNodeMixRGB")
        mix.blend_type = "OVERLAY"
        mix.inputs["Fac"].default_value = 0.35
        mix.inputs["Color1"].default_value = (0.88, 0.68, 0.42, 1.0)
        mix.inputs["Color2"].default_value = (0.72, 0.54, 0.32, 1.0)
        links.new(wr.outputs["Fac"], mix.inputs["Fac"])
        links.new(mix.outputs["Color"], bsdf.inputs["Base Color"])
    elif role == "chair":
        bsdf.inputs["Base Color"].default_value = (0.28, 0.48, 0.82, 1.0)
        bsdf.inputs["Roughness"].default_value = 0.55
        wr = nodes.new("ShaderNodeTexNoise")
        wr.inputs["Scale"].default_value = 24.0
        mix = nodes.new("ShaderNodeMixRGB")
        mix.blend_type = "OVERLAY"
        mix.inputs["Fac"].default_value = 0.22
        mix.inputs["Color2"].default_value = (0.18, 0.32, 0.62, 1.0)
        links.new(wr.outputs["Fac"], mix.inputs["Fac"])
        links.new(mix.outputs["Color"], bsdf.inputs["Base Color"])
    elif role == "lamp_accent":
        bsdf.inputs["Base Color"].default_value = (0.94, 0.82, 0.55, 1.0)
        bsdf.inputs["Roughness"].default_value = 0.42
        emit = nodes.new("ShaderNodeEmission")
        emit.inputs["Color"].default_value = (1.0, 0.78, 0.35, 1.0)
        emit.inputs["Strength"].default_value = 0.28
        add = nodes.new("ShaderNodeAddShader")
        links.new(bsdf.outputs["BSDF"], add.inputs[0])
        links.new(emit.outputs["Emission"], add.inputs[1])
        links.new(add.outputs["Shader"], out.inputs["Surface"])
    else:
        bsdf.inputs["Base Color"].default_value = tint
        bsdf.inputs["Roughness"].default_value = 0.62 if role not in ("metal", "glass", "wood") else 0.48
    if role in ("metal",):
        bsdf.inputs["Metallic"].default_value = 0.55
    if role == "wood":
        bsdf.inputs["Roughness"].default_value = 0.68
    if role == "glass":
        bsdf.inputs["Roughness"].default_value = 0.08
        if "Transmission Weight" in bsdf.inputs:
            bsdf.inputs["Transmission Weight"].default_value = 0.35
        elif "Transmission" in bsdf.inputs:
            bsdf.inputs["Transmission"].default_value = 0.35
    if role == "screen":
        emit = nodes.new("ShaderNodeEmission")
        emit.inputs["Color"].default_value = tint
        emit.inputs["Strength"].default_value = 0.12
        add = nodes.new("ShaderNodeAddShader")
        links.new(bsdf.outputs["BSDF"], add.inputs[0])
        links.new(emit.outputs["Emission"], add.inputs[1])
        links.new(add.outputs["Shader"], out.inputs["Surface"])
    elif role != "lamp_accent":
        links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def bake_object_maps(ob: bpy.types.Object, tex_dir: Path, asset_id: str, size: int = 1024) -> dict[str, str]:
    tex_dir.mkdir(parents=True, exist_ok=True)
    kinds = ["albedo", "ao", "normal"]
    out_paths: dict[str, str] = {}
    for kind in kinds:
        img = bpy.data.images.new(f"{asset_id}_{kind}", width=size, height=size)
        img.file_format = "PNG"
        path = tex_dir / f"{asset_id}_{kind}.png"
        img.filepath_raw = str(path)
        mat = ob.data.materials[0] if ob.data.materials else None
        if mat and mat.use_nodes:
            bake_node = mat.node_tree.nodes.new("ShaderNodeTexImage")
            bake_node.image = img
            mat.node_tree.nodes.active = bake_node
        bpy.ops.object.select_all(action="DESELECT")
        ob.select_set(True)
        bpy.context.view_layer.objects.active = ob
        bpy.context.scene.render.bake.margin = 12
        if kind == "albedo":
            bpy.ops.object.bake(type="DIFFUSE", pass_filter={"COLOR"})
        elif kind == "ao":
            bpy.ops.object.bake(type="AO")
        else:
            bpy.ops.object.bake(type="NORMAL")
        img.save()
        out_paths[kind] = str(path)
    return out_paths


def export_gltf_collection(objects: list[bpy.types.Object], path: Path) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    for ob in objects:
        ob.select_set(True)
    bpy.ops.export_scene.gltf(
        filepath=str(path),
        use_selection=True,
        export_apply=True,
        export_format="GLB",
        export_yup=True,
        export_image_format="AUTO",
    )


def setup_hdri_world(strength: float = 0.95) -> None:
    setup_preview_world_grey_hdri(hdri_strength=strength)


def setup_preview_world_grey_hdri(*, hdri_strength: float = 1.05) -> None:
    """Camera sees flat RGB 150; HDRI lights non-camera rays only."""
    world = bpy.data.worlds.new("cc0_preview_world")
    bpy.context.scene.world = world
    world.use_nodes = True
    nodes = world.node_tree.nodes
    links = world.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputWorld")
    grey_bg = nodes.new("ShaderNodeBackground")
    grey_bg.inputs["Color"].default_value = (BACKDROP_GREY, BACKDROP_GREY, BACKDROP_GREY, 1)
    grey_bg.inputs["Strength"].default_value = 1.0
    light_path = nodes.new("ShaderNodeLightPath")
    mix = nodes.new("ShaderNodeMixShader")
    links.new(light_path.outputs["Is Camera Ray"], mix.inputs["Fac"])
    if HDRI_PATH.is_file():
        env = nodes.new("ShaderNodeTexEnvironment")
        env.image = load_image(HDRI_PATH)
        mapping = nodes.new("ShaderNodeMapping")
        texcoord = nodes.new("ShaderNodeTexCoord")
        mapping.inputs["Rotation"].default_value[2] = math.radians(215)
        links.new(texcoord.outputs["Generated"], mapping.inputs["Vector"])
        links.new(mapping.outputs["Vector"], env.inputs["Vector"])
        hdri_bg = nodes.new("ShaderNodeBackground")
        links.new(env.outputs["Color"], hdri_bg.inputs["Color"])
        hdri_bg.inputs["Strength"].default_value = hdri_strength
        links.new(hdri_bg.outputs["Background"], mix.inputs[1])
    else:
        amb = nodes.new("ShaderNodeBackground")
        amb.inputs["Color"].default_value = (0.72, 0.70, 0.68, 1)
        amb.inputs["Strength"].default_value = 0.6
        links.new(amb.outputs["Background"], mix.inputs[1])
    # Fac=1 (camera rays) → grey; Fac=0 → HDRI lights the scene
    links.new(grey_bg.outputs["Background"], mix.inputs[2])
    links.new(mix.outputs["Shader"], out.inputs["Surface"])


def add_preview_warm_lights_z(room_center: Vector) -> list[bpy.types.Object]:
    lights: list[bpy.types.Object] = []
    bpy.ops.object.light_add(type="POINT", location=(room_center.x, room_center.y, room_center.z + 2.0))
    key = bpy.context.active_object
    key.name = "preview_warm_key"
    key.data.color = (1.0, 0.82, 0.66)
    key.data.energy = 120.0
    key.data.shadow_soft_size = 0.38
    lights.append(key)
    bpy.ops.object.light_add(type="AREA", location=(room_center.x + 1.0, room_center.y - 0.6, room_center.z + 1.8))
    fill = bpy.context.active_object
    fill.name = "preview_warm_fill"
    fill.data.color = (1.0, 0.90, 0.78)
    fill.data.energy = 55.0
    fill.data.size = 1.6
    lights.append(fill)
    return lights


def add_preview_warm_lights(room_center: Vector) -> list[bpy.types.Object]:
    lights: list[bpy.types.Object] = []
    bpy.ops.object.light_add(type="POINT", location=(room_center.x, room_center.y + 1.6, room_center.z))
    key = bpy.context.active_object
    key.name = "preview_warm_key"
    key.data.color = (1.0, 0.82, 0.66)
    key.data.energy = 95.0
    key.data.shadow_soft_size = 0.42
    lights.append(key)
    bpy.ops.object.light_add(type="AREA", location=(room_center.x + 1.2, 1.8, room_center.z + 0.8))
    fill = bpy.context.active_object
    fill.name = "preview_warm_fill"
    fill.data.color = (1.0, 0.90, 0.78)
    fill.data.energy = 40.0
    fill.data.size = 1.4
    lights.append(fill)
    return lights


def _bounds_center(objects: list[bpy.types.Object]) -> tuple[Vector, float]:
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in objects:
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins.x = min(mins.x, w.x)
            mins.y = min(mins.y, w.y)
            mins.z = min(mins.z, w.z)
            maxs.x = max(maxs.x, w.x)
            maxs.y = max(maxs.y, w.y)
            maxs.z = max(maxs.z, w.z)
    center = (mins + maxs) * 0.5
    span_h = max(maxs.x - mins.x, maxs.z - mins.z, 0.1)
    span_v = max(maxs.y - mins.y, 0.1)
    span = max(span_h, span_v * 0.55)
    return center, span


def _camera_distance_for_fill(span: float, lens_mm: float, fill: float, margin: float) -> float:
    """Perspective distance so subject span fills ~fill of frame with ~margin border."""
    sensor = 36.0
    fov = 2.0 * math.atan(sensor / (2.0 * lens_mm))
    effective_fill = max(0.45, min(0.85, fill * (1.0 - 2.0 * margin)))
    half = max(span * 0.48, 0.75)
    return half / (math.tan(fov * 0.5) * effective_fill)


def render_kit_preview(
    objects: list[bpy.types.Object],
    path: Path,
    *,
    resolution: int = 768,
    mode: str = "scene",
    fill: float = 0.70,
    margin: float = 0.05,
    frame_span: float | None = None,
    frame_objects: list[bpy.types.Object] | None = None,
    up_axis: str = "Y",
) -> None:
    """Warm point + HDRI (hidden from camera); world corners RGB 150; ~70% fill."""
    PREVIEW_ROOT.mkdir(parents=True, exist_ok=True)
    setup_preview_world_grey_hdri(hdri_strength=0.72)
    bpy.context.scene.render.engine = "CYCLES"
    bpy.context.scene.cycles.samples = 80 if mode == "hero" else 96
    bpy.context.scene.cycles.use_denoising = False
    bpy.context.scene.render.film_transparent = False
    bpy.context.scene.view_settings.view_transform = "Standard"
    bpy.context.scene.view_settings.look = "None"
    bpy.context.scene.view_settings.exposure = 0.0
    if hasattr(bpy.context.scene, "cycles"):
        bpy.context.scene.cycles.film_exposure = 1.0

    frame_src = frame_objects or objects
    center, span3 = _bounds_center(frame_src)
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in frame_src:
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins.x = min(mins.x, w.x)
            mins.y = min(mins.y, w.y)
            mins.z = min(mins.z, w.z)
            maxs.x = max(maxs.x, w.x)
            maxs.y = max(maxs.y, w.y)
            maxs.z = max(maxs.z, w.z)
    if up_axis == "Z":
        span_xy = max(maxs.x - mins.x, maxs.y - mins.y, 0.1)
        span = max(span_xy, 3.6)
    else:
        span_xz = max(maxs.x - mins.x, maxs.z - mins.z, 0.1)
        span = max(span_xz, 3.6)
    if frame_span is not None:
        span = frame_span

    if up_axis == "Z":
        wall_top = maxs.z
    else:
        wall_top = maxs.y
    room = span if span <= 5.5 else max(span, 4.0)
    floor_h = 0.05
    lens = 48.0
    pitch = math.radians(34.0)
    dist_scale = 0.77 if mode == "hero" and up_axis == "Z" else (0.88 if mode == "hero" else 0.62)
    dist = max(_camera_distance_for_fill(room + 0.55, lens, fill, margin) * dist_scale, room * 0.85)
    horiz = dist * math.cos(pitch)
    elev = dist * math.sin(pitch)

    shadow: bpy.types.Object | None = None
    if up_axis == "Z":
        target = Vector((mins.x + span * 0.48, mins.y + span * 0.48, floor_h + wall_top * 0.42))
        lights = add_preview_warm_lights_z(target)
        cam_loc = Vector(
            (
                maxs.x + horiz * 0.58,
                maxs.y + horiz * 0.58,
                floor_h + wall_top * 0.28 + elev * 0.68,
            )
        )
        bpy.context.scene.cycles.use_shadow_catcher = False
    else:
        target = Vector((room * 0.48, floor_h + 1.05, room * 0.48))
        lights = add_preview_warm_lights(target)
        cam_loc = Vector((room + horiz * 0.58, target.y + elev * 0.55, room + horiz * 0.58))
        bpy.ops.mesh.primitive_plane_add(size=room * 1.05, location=(room / 2, -0.002, room / 2))
        shadow = bpy.context.active_object
        shadow.name = "preview_shadow_catcher"
        smat = bpy.data.materials.new("shadow_grey")
        smat.use_nodes = True
        sg = smat.node_tree.nodes.get("Principled BSDF")
        sg.inputs["Base Color"].default_value = (BACKDROP_GREY, BACKDROP_GREY, BACKDROP_GREY, 1)
        sg.inputs["Roughness"].default_value = 0.96
        shadow.data.materials.append(smat)
        shadow.is_shadow_catcher = True
        bpy.context.scene.cycles.use_shadow_catcher = True
    bpy.ops.object.camera_add(location=cam_loc)
    cam = bpy.context.active_object
    cam.data.lens = lens
    cam.data.clip_end = dist * 6.0
    direction = target - cam.location
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    bpy.context.scene.camera = cam

    bpy.context.scene.render.resolution_x = resolution
    bpy.context.scene.render.resolution_y = resolution
    bpy.context.scene.render.image_settings.file_format = "PNG"
    bpy.context.scene.render.filepath = str(path)
    bpy.ops.render.render(write_still=True)

    if shadow and bpy.data.objects.get(shadow.name):
        bpy.data.objects.remove(shadow, do_unlink=True)
    for ob in lights:
        bpy.data.objects.remove(ob, do_unlink=True)
    bpy.data.objects.remove(cam, do_unlink=True)
    _quantize_backdrop_rgb150(path)


def _quantize_backdrop_rgb150(path: Path) -> None:
    """Force every studio-backdrop pixel to exact RGB 150 (no edge vignette)."""
    try:
        from PIL import Image
    except ImportError:
        return
    from collections import deque

    def sat(rgb):
        r, g, b = (c / 255.0 for c in rgb)
        mx, mn = max(r, g, b), min(r, g, b)
        return 0.0 if mx <= 1e-6 else (mx - mn) / mx

    def is_backdrop_pixel(rgb: tuple[int, int, int]) -> bool:
        if rgb == CORNER_RGB:
            return True
        L = 0.299 * rgb[0] + 0.587 * rgb[1] + 0.114 * rgb[2]
        return sat(rgb) < 0.14 and 118 <= L <= 158

    im = Image.open(path).convert("RGB")
    w, h = im.size
    bg = [[False] * w for _ in range(h)]
    q: deque[tuple[int, int]] = deque()
    for x in range(w):
        for y in (0, h - 1):
            if is_backdrop_pixel(im.getpixel((x, y))):
                bg[y][x] = True
                q.append((x, y))
    for y in range(h):
        for x in (0, w - 1):
            if is_backdrop_pixel(im.getpixel((x, y))):
                if not bg[y][x]:
                    bg[y][x] = True
                    q.append((x, y))
    for y in range(min(int(h * 0.08), h)):
        for x in range(w):
            if is_backdrop_pixel(im.getpixel((x, y))) and not bg[y][x]:
                bg[y][x] = True
                q.append((x, y))
    while q:
        x, y = q.popleft()
        for nx, ny in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
            if 0 <= nx < w and 0 <= ny < h and not bg[ny][nx]:
                if is_backdrop_pixel(im.getpixel((nx, ny))):
                    bg[ny][nx] = True
                    q.append((nx, ny))
    for y in range(h):
        for x in range(w):
            if bg[y][x]:
                im.putpixel((x, y), CORNER_RGB)
    bx = max(1, int(w * 0.03))
    by = max(1, int(h * 0.03))
    for y in range(h):
        for x in range(w):
            if x < bx or x >= w - bx or y < by or y >= h - by:
                im.putpixel((x, y), CORNER_RGB)
    im.save(path)


def write_cc0_manifest(entry: dict, manifest_name: str = "garage-cc0-manifest.json") -> None:
    manifest_path = ART_ROOT / manifest_name
    payload: dict = {"fidelity": "cc0-blender-baked", "blenderVersion": BLENDER_VERSION, "assets": []}
    if manifest_path.exists():
        payload = json.loads(manifest_path.read_text(encoding="utf-8"))
    assets = [a for a in payload.get("assets", []) if a["id"] != entry["id"]]
    assets.append(entry)
    assets.sort(key=lambda a: a["id"])
    payload["assets"] = assets
    manifest_path.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")