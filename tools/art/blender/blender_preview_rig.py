"""
Studio preview rig for early-art quality pass (shared garage + staff).

BBox auto-frame (~35–75% frame fill), 3/4 view (~30° azimuth), three-point light,
neutral gradient backdrop, ground shadow catcher.
"""
from __future__ import annotations

import math
from pathlib import Path

import bpy
from mathutils import Vector

# Neutral studio (object-mask saturation gate ignores backdrop band)
WORLD_TOP = (0.58, 0.58, 0.60, 1.0)
WORLD_HORIZON = (0.48, 0.48, 0.50, 1.0)
WORLD_BOTTOM = (0.32, 0.32, 0.34, 1.0)
FLOOR_COLOR = (0.40, 0.40, 0.42, 1.0)
PREVIEW_AZIMUTH_DEG = 30.0
PREVIEW_ELEV_DEG = 22.0
FRAME_MARGIN = 0.72  # tighter = larger subject in frame


def _mesh_bounds(objects: list[bpy.types.Object]) -> tuple[Vector, Vector]:
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
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
    if mins.x > maxs.x:
        return Vector((0, 0, 0)), Vector((1, 1, 1))
    return mins, maxs


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


def add_shadow_floor_y(y: float = 0.0) -> bpy.types.Object:
    """Ground plane for Y-up kit scripts (staff, garage)."""
    bpy.ops.mesh.primitive_plane_add(size=4.5, location=(0, y - 0.002, 0), rotation=(math.pi / 2, 0, 0))
    floor = bpy.context.active_object
    floor.name = "PreviewShadowFloor"
    mat = bpy.data.materials.new("PreviewFloor")
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = FLOOR_COLOR
        bsdf.inputs["Roughness"].default_value = 0.9
    floor.data.materials.append(mat)
    return floor


def add_shadow_floor(z: float = 0.0) -> bpy.types.Object:
    bpy.ops.mesh.primitive_plane_add(size=6, location=(0, 0, z - 0.002))
    floor = bpy.context.active_object
    floor.name = "PreviewShadowFloor"
    mat = bpy.data.materials.new("PreviewFloor")
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = FLOOR_COLOR
        bsdf.inputs["Roughness"].default_value = 0.9
    floor.data.materials.append(mat)
    return floor


def setup_three_point_lights(center: Vector, radius: float) -> None:
    key_loc = center + Vector((radius * 1.3, -radius * 1.1, radius * 1.4))
    fill_loc = center + Vector((-radius * 1.0, -radius * 0.6, radius * 0.9))
    rim_loc = center + Vector((radius * 0.2, radius * 1.4, radius * 1.1))
    energy = max(180.0, radius * 220.0)
    for name, loc, e_mul in (("Key", key_loc, 1.15), ("Fill", fill_loc, 0.22), ("Rim", rim_loc, 0.95)):
        bpy.ops.object.light_add(type="AREA", location=loc)
        light = bpy.context.active_object
        light.name = name
        light.data.energy = energy * e_mul
        light.data.size = max(0.5, radius * 0.8)
        direction = center - loc
        light.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()


def frame_camera(
    objects: list[bpy.types.Object],
    lens_mm: float = 50.0,
    margin: float = FRAME_MARGIN,
) -> bpy.types.Object:
    """Frustum fit on mesh AABB; 3/4 view."""
    mins, maxs = _mesh_bounds(objects)
    center = (mins + maxs) * 0.5
    extent = (maxs - mins).length
    radius = max(0.35, extent * 0.5) * margin
    cam_dist = radius / math.tan(math.radians(lens_mm) / 2.0) * 0.85
    azimuth = math.radians(PREVIEW_AZIMUTH_DEG)
    elev = math.radians(PREVIEW_ELEV_DEG)
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


def render_preview_studio(
    focus: bpy.types.Object | list[bpy.types.Object],
    path: Path,
    resolution: int = 512,
) -> None:
    """Render with studio rig; `focus` is hero object or join root."""
    hero = focus if isinstance(focus, list) else [focus]
    hero_meshes = [o for o in hero if o.type == "MESH"]
    if not hero_meshes:
        hero_meshes = [o for o in bpy.context.scene.objects if o.type == "MESH" and o.name != "PreviewShadowFloor"]

    bpy.context.scene.render.engine = "BLENDER_EEVEE"
    if hasattr(bpy.context.scene, "eevee"):
        bpy.context.scene.eevee.taa_render_samples = 32
        if hasattr(bpy.context.scene.eevee, "use_gtao"):
            bpy.context.scene.eevee.use_gtao = True
            bpy.context.scene.eevee.gtao_distance = 0.35
            bpy.context.scene.eevee.gtao_factor = 2.4
    setup_studio_world()
    mins, maxs = _mesh_bounds(hero_meshes)
    center = (mins + maxs) * 0.5
    footprint = max(maxs.x - mins.x, maxs.z - mins.z, 0.35)
    floor = add_shadow_floor_y(mins.y)
    floor.scale = (footprint * 0.55, footprint * 0.55, 1.0)
    bpy.ops.object.select_all(action="DESELECT")
    floor.select_set(True)
    bpy.context.view_layer.objects.active = floor
    bpy.ops.object.transform_apply(scale=True)
    radius = max(0.4, (maxs - mins).length * 0.5)
    setup_three_point_lights(center, radius)
    frame_camera(hero_meshes, margin=FRAME_MARGIN)
    bpy.context.scene.render.resolution_x = resolution
    bpy.context.scene.render.resolution_y = resolution
    bpy.context.scene.render.filepath = str(path)
    path.parent.mkdir(parents=True, exist_ok=True)
    bpy.ops.render.render(write_still=True)