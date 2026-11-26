"""
Studio preview rig for early-art quality pass (43 assets).
Auto-framing, three-point lights, warm gradient world, ground shadow catcher.
"""
from __future__ import annotations

import math
from pathlib import Path

import bpy
from mathutils import Vector

# Warm 1995 garage studio (matches Hatch brief)
WORLD_TOP = (0.72, 0.68, 0.62, 1.0)
WORLD_HORIZON = (0.55, 0.50, 0.46, 1.0)
WORLD_BOTTOM = (0.38, 0.34, 0.30, 1.0)
FLOOR_COLOR = (0.42, 0.38, 0.34, 1.0)


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
    """Ground plane for garage scripts that use Y-up."""
    bpy.ops.mesh.primitive_plane_add(size=8, location=(0, y - 0.002, 0), rotation=(math.pi / 2, 0, 0))
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
    for name, loc, e_mul in (("Key", key_loc, 1.0), ("Fill", fill_loc, 0.45), ("Rim", rim_loc, 0.65)):
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
    margin: float = 1.25,
) -> bpy.types.Object:
    mins, maxs = _mesh_bounds(objects)
    center = (mins + maxs) * 0.5
    extent = (maxs - mins).length
    radius = max(0.35, extent * 0.5) * margin
    cam_dist = radius / math.tan(math.radians(lens_mm) / 2.0) * 0.85
    azimuth = math.radians(38)
    elev = math.radians(22)
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
    setup_studio_world()
    mins, maxs = _mesh_bounds(hero_meshes)
    add_shadow_floor_y(mins.y)
    center = (mins + maxs) * 0.5
    radius = max(0.4, (maxs - mins).length * 0.5)
    setup_three_point_lights(center, radius)
    frame_camera(hero_meshes, margin=1.02)
    bpy.context.scene.render.resolution_x = resolution
    bpy.context.scene.render.resolution_y = resolution
    bpy.context.scene.render.filepath = str(path)
    path.parent.mkdir(parents=True, exist_ok=True)
    bpy.ops.render.render(write_still=True)