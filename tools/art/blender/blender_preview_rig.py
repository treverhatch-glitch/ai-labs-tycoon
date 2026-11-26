"""
Studio preview rig for early-art quality pass (43 assets).
Auto-framing, three-point lights, warm gradient world, ground shadow catcher.
"""
from __future__ import annotations

import math
from pathlib import Path

import bpy
from mathutils import Vector

# Neutral studio (Hatch #88: low-sat backdrop for object-mask QA)
WORLD_TOP = (0.72, 0.72, 0.74, 1.0)
WORLD_HORIZON = (0.58, 0.58, 0.60, 1.0)
WORLD_BOTTOM = (0.38, 0.38, 0.40, 1.0)
FLOOR_COLOR = (0.42, 0.42, 0.44, 1.0)


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


def setup_flat_world() -> None:
    world = bpy.data.worlds.new("EarlyPreviewFlat")
    bpy.context.scene.world = world
    world.use_nodes = True
    nodes = world.node_tree.nodes
    links = world.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputWorld")
    bg = nodes.new("ShaderNodeBackground")
    bg.inputs["Color"].default_value = (0.55, 0.55, 0.58, 1.0)
    bg.inputs["Strength"].default_value = 1.0
    links.new(bg.outputs["Background"], out.inputs["Surface"])


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
    energy = max(95.0, radius * 135.0)
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
        if hasattr(bpy.context.scene.eevee, "use_gtao"):
            bpy.context.scene.eevee.use_gtao = True
            bpy.context.scene.eevee.gtao_distance = 0.35
            bpy.context.scene.eevee.gtao_factor = 2.4
    setup_studio_world()
    mins, maxs = _mesh_bounds(hero_meshes)
    center = (mins + maxs) * 0.5
    span_y = maxs.y - mins.y
    footprint = max(maxs.x - mins.x, maxs.z - mins.z, 0.35)
    if span_y >= 0.18:
        floor = add_shadow_floor_y(mins.y)
        floor.scale = (footprint * 0.42, footprint * 0.42, 1.0)
        bpy.ops.object.select_all(action="DESELECT")
        floor.select_set(True)
        bpy.context.view_layer.objects.active = floor
        bpy.ops.object.transform_apply(scale=True)
    extent = (maxs - mins).length
    radius = max(0.4, extent * 0.5)
    light_scale = 0.55 if extent < 1.25 else 1.0
    setup_three_point_lights(center, radius * light_scale)
    span = maxs - mins
    footprint_xz = max(span.x, span.z, 0.01)
    aspect = footprint_xz / max(min(span.x, span.z), 0.01)
    margin = 1.12 if extent > 2.2 else 0.76 if extent < 1.25 else 0.94
    if aspect > 2.0:
        margin *= 1.18
    if span.y > span.x * 2.5:
        margin *= 1.10
    if max(span.x, span.z) > 0.7:
        margin *= 1.26
    if span.y < 0.08:
        margin *= 1.24
    if span.y > span.x * 3 and span.x < 0.65:
        margin *= 0.962
    if extent > 1.8:
        margin *= 1.14
    frame_camera(hero_meshes, margin=margin)
    bpy.context.scene.render.resolution_x = resolution
    bpy.context.scene.render.resolution_y = resolution
    bpy.context.scene.render.filepath = str(path)
    path.parent.mkdir(parents=True, exist_ok=True)
    bpy.ops.render.render(write_still=True)


def render_preview_hud(
    focus: bpy.types.Object | list[bpy.types.Object],
    path: Path,
    resolution: int = 512,
) -> None:
    """Flat neutral backdrop for HUD/icons (saturated object-mask QA)."""
    hero = focus if isinstance(focus, list) else [focus]
    hero_meshes = [o for o in hero if o.type == "MESH"]
    bpy.context.scene.render.engine = "BLENDER_EEVEE"
    if hasattr(bpy.context.scene, "eevee"):
        bpy.context.scene.eevee.taa_render_samples = 32
    setup_flat_world()
    mins, maxs = _mesh_bounds(hero_meshes)
    center = (mins + maxs) * 0.5
    extent = (maxs - mins).length
    radius = max(0.35, extent * 0.5)
    setup_three_point_lights(center, radius * 0.42)
    frame_camera(hero_meshes, margin=0.82)
    bpy.context.scene.render.resolution_x = resolution
    bpy.context.scene.render.resolution_y = resolution
    bpy.context.scene.render.filepath = str(path)
    path.parent.mkdir(parents=True, exist_ok=True)
    bpy.ops.render.render(write_still=True)