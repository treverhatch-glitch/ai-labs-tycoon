"""
Shared studio preview rig (garage / staff / HUD lanes).
World bbox over all mesh vertices; camera at radius/tan(fov/2)*1.15; 3/4 ~30°.
Writes RGB preview + `.mask.png` alpha pass for Hatch gates.
"""
from __future__ import annotations

import json
import math
import os
from pathlib import Path

import bpy
from bpy_extras.object_utils import world_to_camera_view
from mathutils import Vector

# Neutral studio (does not inflate saturation gate)
WORLD_TOP = (0.62, 0.61, 0.60, 1.0)
WORLD_HORIZON = (0.48, 0.47, 0.46, 1.0)
WORLD_BOTTOM = (0.28, 0.27, 0.26, 1.0)
FLOOR_COLOR = (0.80, 0.48, 0.14, 1.0)
MID_GREY_RGBA = (0.50, 0.50, 0.52, 1.0)

FRAMING_MARGIN = 1.15  # Hatch: radius/tan(fov/2)*1.15
DEFAULT_AZIMUTH_DEG = 45.0
DEFAULT_ELEVATION_DEG = 30.0
DEFAULT_LENS_MM = 50.0


def mesh_bounds_world(objects: list[bpy.types.Object]) -> tuple[Vector, Vector]:
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    any_pt = False
    for ob in objects:
        if ob.type != "MESH" or not ob.data.vertices:
            continue
        for vert in ob.data.vertices:
            world = ob.matrix_world @ vert.co
            mins.x = min(mins.x, world.x)
            mins.y = min(mins.y, world.y)
            mins.z = min(mins.z, world.z)
            maxs.x = max(maxs.x, world.x)
            maxs.y = max(maxs.y, world.y)
            maxs.z = max(maxs.z, world.z)
            any_pt = True
    if not any_pt:
        for ob in objects:
            if ob.type != "MESH":
                continue
            for corner in ob.bound_box:
                world = ob.matrix_world @ Vector(corner)
                mins.x = min(mins.x, world.x)
                mins.y = min(mins.y, world.y)
                mins.z = min(mins.z, world.z)
                maxs.x = max(maxs.x, world.x)
                maxs.y = max(maxs.y, world.y)
                maxs.z = max(maxs.z, world.z)
                any_pt = True
    if not any_pt or mins.x > maxs.x:
        return Vector((0, 0, 0)), Vector((1, 1, 1))
    return mins, maxs


def bounding_sphere_radius(mins: Vector, maxs: Vector, objects: list[bpy.types.Object] | None = None) -> float:
    center = (mins + maxs) * 0.5
    r = 0.0
    if objects:
        for ob in objects:
            if ob.type != "MESH":
                continue
            for vert in ob.data.vertices:
                d = (ob.matrix_world @ vert.co - center).length
                r = max(r, d)
    if r < 1e-6:
        span = maxs - mins
        r = max(0.2, span.length * 0.5)
    return max(0.2, r)


def camera_half_fov_rad(cam_data: bpy.types.Camera) -> float:
    """Smaller half-FOV (limits framing on square renders)."""
    focal = max(cam_data.lens, 1e-3)
    fov_h = 2.0 * math.atan(cam_data.sensor_width / (2.0 * focal))
    fov_v = 2.0 * math.atan(cam_data.sensor_height / (2.0 * focal))
    return min(fov_h, fov_v) * 0.5


def setup_studio_world() -> None:
    world = bpy.data.worlds.new("EarlyPreviewWorld")
    bpy.context.scene.world = world
    world.use_nodes = True
    nodes = world.node_tree.nodes
    links = world.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputWorld")
    bg = nodes.new("ShaderNodeBackground")
    coord = nodes.new("ShaderNodeTexCoord")
    sep = nodes.new("ShaderNodeSeparateXYZ")
    ramp = nodes.new("ShaderNodeValToRGB")
    ramp.color_ramp.elements[0].position = 0.0
    ramp.color_ramp.elements[0].color = WORLD_BOTTOM
    ramp.color_ramp.elements[1].position = 0.45
    ramp.color_ramp.elements[1].color = WORLD_HORIZON
    elem = ramp.color_ramp.elements.new(0.85)
    elem.color = WORLD_TOP
    links.new(coord.outputs["Generated"], sep.inputs["Vector"])
    links.new(sep.outputs["Z"], ramp.inputs["Fac"])
    links.new(ramp.outputs["Color"], bg.inputs["Color"])
    bg.inputs["Strength"].default_value = 1.0
    links.new(bg.outputs["Background"], out.inputs["Surface"])


def setup_mid_grey_world() -> None:
    """Pass-4: flat mid-grey backdrop (no gradient wall)."""
    world = bpy.data.worlds.new("EarlyPreviewWorld")
    bpy.context.scene.world = world
    world.use_nodes = True
    nodes = world.node_tree.nodes
    links = world.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputWorld")
    bg = nodes.new("ShaderNodeBackground")
    bg.inputs["Color"].default_value = MID_GREY_RGBA
    bg.inputs["Strength"].default_value = 1.0
    links.new(bg.outputs["Background"], out.inputs["Surface"])


def _mid_grey_preview() -> bool:
    return os.environ.get("EARLY_PREVIEW_MID_GREY", "").strip().lower() in ("1", "true", "yes")


def _contact_shadow_only() -> bool:
    return os.environ.get("EARLY_PREVIEW_CONTACT_SHADOW", "").strip().lower() in ("1", "true", "yes")


def add_shadow_floor_y(mins: Vector, maxs: Vector) -> bpy.types.Object:
    center = (mins + maxs) * 0.5
    footprint = max(maxs.x - mins.x, maxs.z - mins.z, 0.5)
    bpy.ops.mesh.primitive_plane_add(
        size=footprint * 1.8,
        location=(center.x, mins.y - 0.002, center.z),
        rotation=(math.pi / 2, 0, 0),
    )
    floor = bpy.context.active_object
    floor.name = "PreviewShadowFloor"
    mat = bpy.data.materials.new("PreviewFloor")
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = FLOOR_COLOR
        bsdf.inputs["Roughness"].default_value = 0.92
    floor.data.materials.append(mat)
    return floor


def add_shadow_floor_z(mins: Vector, maxs: Vector) -> bpy.types.Object:
    center = (mins + maxs) * 0.5
    footprint = max(maxs.x - mins.x, maxs.y - mins.y, 0.5)
    bpy.ops.mesh.primitive_plane_add(size=footprint * 1.8, location=(center.x, center.y, mins.z - 0.002))
    floor = bpy.context.active_object
    floor.name = "PreviewShadowFloor"
    mat = bpy.data.materials.new("PreviewFloor")
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = FLOOR_COLOR
        bsdf.inputs["Roughness"].default_value = 0.92
    floor.data.materials.append(mat)
    return floor


def infer_up_axis(mins: Vector, maxs: Vector) -> str:
    span = maxs - mins
    return "Y" if span.y >= span.z * 1.1 else "Z"


def setup_three_point_lights(center: Vector, radius: float, *, soft: bool = False) -> None:
    key_loc = center + Vector((radius * 1.4, -radius * 1.0, radius * 1.2))
    fill_loc = center + Vector((-radius * 1.1, -radius * 0.5, radius * 0.85))
    rim_loc = center + Vector((radius * 0.15, radius * 1.3, radius * 1.0))
    energy = max(120.0, radius * 150.0)
    scale = (0.62, 0.14, 0.22) if soft else (0.85, 0.30, 0.40)
    for name, loc, e_mul in (("Key", key_loc, scale[0]), ("Fill", fill_loc, scale[1]), ("Rim", rim_loc, scale[2])):
        bpy.ops.object.light_add(type="AREA", location=loc)
        light = bpy.context.active_object
        light.name = name
        light.data.energy = energy * e_mul
        light.data.size = max(0.45, radius * 0.7)
        direction = center - loc
        light.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()


def _place_camera(
    center: Vector,
    cam_dist: float,
    lens_mm: float,
    azimuth_deg: float,
    elevation_deg: float,
    up_axis: str = "Y",
) -> bpy.types.Object:
    azimuth = math.radians(azimuth_deg)
    elev = math.radians(elevation_deg)
    if up_axis == "Y":
        offset = Vector(
            (
                math.cos(elev) * math.cos(azimuth),
                math.sin(elev),
                math.cos(elev) * math.sin(azimuth),
            )
        ) * cam_dist
    else:
        offset = Vector(
            (
                math.cos(elev) * math.cos(azimuth),
                math.cos(elev) * math.sin(azimuth),
                math.sin(elev),
            )
        ) * cam_dist
    bpy.ops.object.camera_add(location=center + offset)
    cam = bpy.context.active_object
    cam.name = "PreviewCamera"
    cam.data.lens = lens_mm
    direction = center - cam.location
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    bpy.context.scene.camera = cam
    return cam


def frame_camera(
    objects: list[bpy.types.Object],
    lens_mm: float = DEFAULT_LENS_MM,
    margin: float = FRAMING_MARGIN,
    azimuth_deg: float = DEFAULT_AZIMUTH_DEG,
    elevation_deg: float = DEFAULT_ELEVATION_DEG,
    up_axis: str = "Y",
) -> bpy.types.Object:
    mins, maxs = mesh_bounds_world(objects)
    center = (mins + maxs) * 0.5
    radius = bounding_sphere_radius(mins, maxs, objects)
    cam = _place_camera(center, 1.0, lens_mm, azimuth_deg, elevation_deg, up_axis=up_axis)
    half_fov = camera_half_fov_rad(cam.data)
    cam_dist = (radius / math.tan(half_fov)) * margin
    bpy.data.objects.remove(cam, do_unlink=True)
    return _place_camera(center, cam_dist, lens_mm, azimuth_deg, elevation_deg, up_axis=up_axis)


def _apply_mask_material(ob: bpy.types.Object, stash: list) -> None:
    stash.append((ob, list(ob.data.materials)))
    mat = bpy.data.materials.new("_PreviewMask")
    mat.use_nodes = True
    nt = mat.node_tree
    nt.nodes.clear()
    emit = nt.nodes.new("ShaderNodeEmission")
    emit.inputs["Color"].default_value = (1.0, 1.0, 1.0, 1.0)
    emit.inputs["Strength"].default_value = 1.0
    out = nt.nodes.new("ShaderNodeOutputMaterial")
    nt.links.new(emit.outputs["Emission"], out.inputs["Surface"])
    mat.blend_method = "OPAQUE"
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def render_hero_mask(
    hero_meshes: list[bpy.types.Object],
    path: Path,
    resolution: int,
) -> None:
    """RGBA alpha pass: heroes opaque, floor/lights/world hidden."""
    scene = bpy.context.scene
    floor = bpy.data.objects.get("PreviewShadowFloor")
    if floor:
        floor.hide_render = True
    stash: list = []
    for ob in list(scene.objects):
        if ob.type == "LIGHT":
            ob.hide_render = True
        elif ob.type == "MESH" and ob.name != "PreviewShadowFloor":
            ob.hide_render = ob not in hero_meshes
    for ob in hero_meshes:
        _apply_mask_material(ob, stash)
    if scene.world and scene.world.use_nodes:
        for n in scene.world.node_tree.nodes:
            if n.type == "BACKGROUND":
                n.inputs["Color"].default_value = (0.0, 0.0, 0.0, 1.0)
                n.inputs["Strength"].default_value = 0.0
    scene.render.film_transparent = True
    scene.render.image_settings.color_mode = "RGBA"
    scene.render.resolution_x = resolution
    scene.render.resolution_y = resolution
    scene.render.filepath = str(path)
    bpy.ops.render.render(write_still=True)
    scene.render.film_transparent = False
    if floor:
        floor.hide_render = False
    for ob, mats in stash:
        ob.data.materials.clear()
        for m in mats:
            ob.data.materials.append(m)
    for ob in scene.objects:
        if ob.type in {"MESH", "LIGHT"}:
            ob.hide_render = False


def render_preview_studio(
    focus: bpy.types.Object | list[bpy.types.Object],
    path: Path,
    resolution: int = 512,
    up_axis: str | None = None,
    lens_mm: float = DEFAULT_LENS_MM,
) -> None:
    hero = focus if isinstance(focus, list) else [focus]
    hero_meshes = [o for o in hero if o.type == "MESH"]
    if not hero_meshes:
        hero_meshes = [
            o for o in bpy.context.scene.objects if o.type == "MESH" and o.name != "PreviewShadowFloor"
        ]

    scene = bpy.context.scene
    scene.render.engine = "BLENDER_EEVEE"
    if hasattr(scene, "eevee"):
        scene.eevee.taa_render_samples = 32
        if hasattr(scene.eevee, "use_gtao"):
            scene.eevee.use_gtao = True
            scene.eevee.gtao_distance = 0.35 if _mid_grey_preview() else 0.4
            scene.eevee.gtao_factor = 2.2 if _mid_grey_preview() else 1.4

    mid_grey = _mid_grey_preview()
    if mid_grey:
        setup_mid_grey_world()
    else:
        setup_studio_world()
    mins, maxs = mesh_bounds_world(hero_meshes)
    axis = (up_axis or infer_up_axis(mins, maxs)).upper()
    if _contact_shadow_only():
        center = (mins + maxs) * 0.5
        span = max(maxs.x - mins.x, maxs.z - mins.z, 0.35)
        if axis == "Y":
            bpy.ops.mesh.primitive_plane_add(
                size=span * 0.55,
                location=(center.x, mins.y - 0.001, center.z),
                rotation=(math.pi / 2, 0, 0),
            )
        else:
            bpy.ops.mesh.primitive_plane_add(
                size=span * 0.55,
                location=(center.x, center.y, mins.z - 0.001),
            )
        floor = bpy.context.active_object
        floor.name = "PreviewShadowFloor"
        mat = bpy.data.materials.new("ContactShadow")
        mat.use_nodes = True
        bsdf = mat.node_tree.nodes.get("Principled BSDF")
        if bsdf:
            bsdf.inputs["Base Color"].default_value = (0.12, 0.12, 0.13, 1.0)
            bsdf.inputs["Roughness"].default_value = 0.95
        floor.data.materials.append(mat)
    elif not mid_grey:
        if axis == "Y":
            add_shadow_floor_y(mins, maxs)
        else:
            add_shadow_floor_z(mins, maxs)
    center = (mins + maxs) * 0.5
    radius = bounding_sphere_radius(mins, maxs, hero_meshes)
    setup_three_point_lights(center, radius, soft=mid_grey)
    margin = float(os.environ.get("PREVIEW_FRAMING_MARGIN", str(FRAMING_MARGIN)))
    azimuth_deg = float(os.environ.get("PREVIEW_AZIMUTH_DEG", str(DEFAULT_AZIMUTH_DEG)))
    cam = frame_camera(
        hero_meshes,
        lens_mm=lens_mm,
        up_axis=axis,
        margin=margin,
        azimuth_deg=azimuth_deg,
    )
    half_fov = camera_half_fov_rad(cam.data)
    cam_dist = (radius / math.tan(half_fov)) * margin

    meta_path = path.with_name(path.stem + ".frame.json")
    meta_path.write_text(
        json.dumps(
            {
                "camera_dist": cam_dist,
                "bbox_radius": radius,
                "framing_margin": margin,
                "lens_mm": lens_mm,
                "azimuth_deg": azimuth_deg,
                "elevation_deg": DEFAULT_ELEVATION_DEG,
                "up_axis": axis,
                "mid_grey": mid_grey,
                "contact_shadow": _contact_shadow_only(),
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )

    scene.render.film_transparent = False
    scene.render.image_settings.color_mode = "RGB"
    scene.render.resolution_x = resolution
    scene.render.resolution_y = resolution
    scene.render.filepath = str(path)
    path.parent.mkdir(parents=True, exist_ok=True)
    bpy.ops.render.render(write_still=True)
    mask_path = path.with_name(path.stem + ".mask.png")
    render_hero_mask(hero_meshes, mask_path, resolution)