"""
Shared studio preview rig (garage / staff / HUD lanes).
World bbox over all mesh vertices; camera at radius/tan(fov/2)*1.15; 3/4 ~30°.
Writes RGB preview + `.mask.png` alpha pass for Hatch gates.
"""
from __future__ import annotations

import json
import math
from pathlib import Path

import bpy
from bpy_extras.object_utils import world_to_camera_view
from mathutils import Vector

# Hatch garage hero: flat RGB ~150 backdrop, contact-shadow floor slightly darker
STUDIO_GREY = (150 / 255, 150 / 255, 150 / 255, 1.0)
FLOOR_GREY = (0.48, 0.48, 0.50, 1.0)

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
    bg.inputs["Color"].default_value = STUDIO_GREY
    bg.inputs["Strength"].default_value = 1.0
    links.new(bg.outputs["Background"], out.inputs["Surface"])


def setup_hdri_world(hdri_path: Path, strength: float = 0.55, rotation_z_deg: float = 118.0) -> None:
    """Poly Haven (CC0) environment visible in camera (legacy). Prefer lighting_only."""
    setup_hdri_lighting_only(hdri_path, strength=strength, rotation_z_deg=rotation_z_deg, visible_hdri=True)


def setup_hdri_lighting_only(
    hdri_path: Path,
    strength: float = 1.0,
    rotation_z_deg: float = 118.0,
    visible_hdri: bool = False,
) -> None:
    """Grey 150 camera backdrop; HDRI lights scene only (Cycles-safe Light Path mix)."""
    grey = STUDIO_GREY
    if visible_hdri and hdri_path.is_file():
        world = bpy.data.worlds.new("HdriVisibleWorld")
        bpy.context.scene.world = world
        world.use_nodes = True
        nodes = world.node_tree.nodes
        links = world.node_tree.links
        nodes.clear()
        out = nodes.new("ShaderNodeOutputWorld")
        tex = nodes.new("ShaderNodeTexEnvironment")
        tex.image = bpy.data.images.load(str(hdri_path))
        tex.image.colorspace_settings.name = "Linear Rec.709"
        bg = nodes.new("ShaderNodeBackground")
        bg.inputs["Strength"].default_value = 1.0
        links.new(tex.outputs["Color"], bg.inputs["Color"])
        links.new(bg.outputs["Background"], out.inputs["Surface"])
        return

    world = bpy.data.worlds.new("HdriLightingWorld")
    bpy.context.scene.world = world
    world.use_nodes = True
    nodes = world.node_tree.nodes
    links = world.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputWorld")
    bg_cam = nodes.new("ShaderNodeBackground")
    bg_cam.inputs["Color"].default_value = grey
    bg_cam.inputs["Strength"].default_value = 1.0

    use_light_path = bpy.context.scene.render.engine == "CYCLES"
    if hdri_path.is_file() and use_light_path:
        tex = nodes.new("ShaderNodeTexEnvironment")
        tex.image = bpy.data.images.load(str(hdri_path))
        tex.image.colorspace_settings.name = "Linear Rec.709"
        mapping = nodes.new("ShaderNodeMapping")
        mapping.inputs["Rotation"].default_value[2] = math.radians(rotation_z_deg)
        coord = nodes.new("ShaderNodeTexCoord")
        bg_light = nodes.new("ShaderNodeBackground")
        bg_light.inputs["Strength"].default_value = strength
        links.new(coord.outputs["Generated"], mapping.inputs["Vector"])
        links.new(mapping.outputs["Vector"], tex.inputs["Vector"])
        links.new(tex.outputs["Color"], bg_light.inputs["Color"])
        light_path = nodes.new("ShaderNodeLightPath")
        mix = nodes.new("ShaderNodeMixShader")
        links.new(light_path.outputs["Is Camera Ray"], mix.inputs["Fac"])
        links.new(bg_light.outputs["Background"], mix.inputs[1])
        links.new(bg_cam.outputs["Background"], mix.inputs[2])
        links.new(mix.outputs["Shader"], out.inputs["Surface"])
    else:
        # EEVEE batch: flat grey 150 visible; HDRI path carried for Unity, area lights in rig.
        links.new(bg_cam.outputs["Background"], out.inputs["Surface"])


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
        bsdf.inputs["Base Color"].default_value = FLOOR_GREY
        bsdf.inputs["Roughness"].default_value = 0.92
    floor.data.materials.append(mat)
    return floor


def add_shadow_floor_z(mins: Vector, maxs: Vector) -> bpy.types.Object:
    center = (mins + maxs) * 0.5
    footprint = max(maxs.x - mins.x, maxs.y - mins.y, 0.5)
    bpy.ops.mesh.primitive_plane_add(size=footprint * 1.12, location=(center.x, center.y, mins.z - 0.002))
    floor = bpy.context.active_object
    floor.name = "PreviewShadowFloor"
    mat = bpy.data.materials.new("PreviewFloor")
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = FLOOR_GREY
        bsdf.inputs["Roughness"].default_value = 0.92
    floor.data.materials.append(mat)
    return floor


def infer_up_axis(mins: Vector, maxs: Vector) -> str:
    span = maxs - mins
    return "Y" if span.y >= span.z * 1.1 else "Z"


def setup_three_point_lights(center: Vector, radius: float, energy_scale: float = 1.0) -> None:
    key_loc = center + Vector((radius * 1.4, -radius * 1.0, radius * 1.2))
    fill_loc = center + Vector((-radius * 1.1, -radius * 0.5, radius * 0.85))
    rim_loc = center + Vector((radius * 0.15, radius * 1.3, radius * 1.0))
    energy = max(120.0, radius * 150.0) * energy_scale
    warm = (1.0, 0.91, 0.76)
    for name, loc, e_mul in (("Key", key_loc, 1.0), ("Fill", fill_loc, 0.28), ("Rim", rim_loc, 0.35)):
        bpy.ops.object.light_add(type="AREA", location=loc)
        light = bpy.context.active_object
        light.name = name
        light.data.energy = energy * e_mul
        light.data.size = max(0.55, radius * 0.85)
        light.data.color = warm
        if hasattr(light.data, "shadow_soft_size"):
            light.data.shadow_soft_size = max(0.25, radius * 0.18)
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
    """RGBA mask: white hero silhouettes on opaque black (filled coverage for extent gate)."""
    scene = bpy.context.scene
    floor = bpy.data.objects.get("PreviewShadowFloor")
    if floor:
        floor.hide_render = True
    stash: list = []
    world_stash = scene.world
    mask_world = bpy.data.worlds.new("_PreviewMaskWorld")
    scene.world = mask_world
    mask_world.use_nodes = True
    wn = mask_world.node_tree.nodes
    wl = mask_world.node_tree.links
    wn.clear()
    wout = wn.new("ShaderNodeOutputWorld")
    wbg = wn.new("ShaderNodeBackground")
    wbg.inputs["Color"].default_value = (0.0, 0.0, 0.0, 1.0)
    wl.new(wbg.outputs["Background"], wout.inputs["Surface"])
    for ob in list(scene.objects):
        if ob.type == "LIGHT":
            ob.hide_render = True
        elif ob.type == "MESH" and ob.name != "PreviewShadowFloor":
            ob.hide_render = ob not in hero_meshes
    for ob in hero_meshes:
        _apply_mask_material(ob, stash)
    scene.render.film_transparent = False
    scene.render.image_settings.color_mode = "RGBA"
    scene.render.resolution_x = resolution
    scene.render.resolution_y = resolution
    scene.render.filepath = str(path)
    bpy.ops.render.render(write_still=True)
    scene.world = world_stash
    bpy.data.worlds.remove(mask_world, do_unlink=True)
    if floor:
        floor.hide_render = False
    for ob, mats in stash:
        ob.data.materials.clear()
        for m in mats:
            ob.data.materials.append(m)
    for ob in scene.objects:
        if ob.type in {"MESH", "LIGHT"}:
            ob.hide_render = False


def repair_eevee_magenta_speckles(path: Path) -> int:
    """Replace batch EEVEE (255,0,255) speckles with local median (not missing-texture signal)."""
    if not path.is_file():
        return 0
    import struct
    import zlib

    raw = path.read_bytes()
    if raw[:8] != b"\x89PNG\r\n\x1a\n":
        return 0
    pos = 8
    chunks: list[tuple[bytes, bytes]] = []
    idat = b""
    w = h = 0
    color_type = 2
    while pos < len(raw):
        length = struct.unpack(">I", raw[pos : pos + 4])[0]
        pos += 4
        ctype = raw[pos : pos + 4]
        pos += 4
        chunk = raw[pos : pos + length]
        pos += length + 4
        if ctype == b"IHDR":
            w, h = struct.unpack(">II", chunk[:8])
            color_type = chunk[9]
            chunks.append((ctype, chunk))
        elif ctype == b"IDAT":
            idat += chunk
        elif ctype == b"IEND":
            chunks.append((ctype, chunk))
            break
        else:
            chunks.append((ctype, chunk))
    if color_type != 2 or w <= 0:
        return 0

  # decode to flat rgb
    data = zlib.decompress(idat)
    px = [0] * (w * h * 3)
    off = 0
    for y in range(h):
        off += 1
        row = data[off : off + w * 3]
        off += w * 3
        px[y * w * 3 : (y + 1) * w * 3] = row

    def is_mag(r: int, g: int, b: int) -> bool:
        return r >= 235 and g <= 45 and b >= 235

    fixed = 0
    for y in range(h):
        for x in range(w):
            i = (y * w + x) * 3
            r, g, b = px[i], px[i + 1], px[i + 2]
            if not is_mag(r, g, b):
                continue
            rs: list[int] = []
            for dy, dx in ((-1, 0), (1, 0), (0, -1), (0, 1)):
                ny, nx = y + dy, x + dx
                if 0 <= ny < h and 0 <= nx < w:
                    j = (ny * w + nx) * 3
                    if not is_mag(px[j], px[j + 1], px[j + 2]):
                        rs.extend(px[j : j + 3])
            if rs:
                px[i] = sum(rs[0::3]) // max(1, len(rs) // 3)
                px[i + 1] = sum(rs[1::3]) // max(1, len(rs) // 3)
                px[i + 2] = sum(rs[2::3]) // max(1, len(rs) // 3)
            else:
                px[i : i + 3] = (128, 128, 128)
            fixed += 1

    if fixed == 0:
        return 0

    def _chunk(ctype: bytes, data: bytes) -> bytes:
        return struct.pack(">I", len(data)) + ctype + data + struct.pack(">I", zlib.crc32(ctype + data) & 0xFFFFFFFF)

    rows = b"".join(b"\x00" + bytes(px[y * w * 3 : (y + 1) * w * 3]) for y in range(h))
    idat = zlib.compress(rows, 9)
    ihdr = struct.pack(">IIBBBBB", w, h, 8, 2, 0, 0, 0)
    out = b"\x89PNG\r\n\x1a\n" + _chunk(b"IHDR", ihdr) + _chunk(b"IDAT", idat) + _chunk(b"IEND", b"")
    path.write_bytes(out)
    return fixed


def render_preview_studio(
    focus: bpy.types.Object | list[bpy.types.Object],
    path: Path,
    resolution: int = 512,
    up_axis: str | None = None,
    lens_mm: float = DEFAULT_LENS_MM,
    azimuth_deg: float = DEFAULT_AZIMUTH_DEG,
    elevation_deg: float = DEFAULT_ELEVATION_DEG,
    framing_margin: float = FRAMING_MARGIN,
    mask_meshes: bpy.types.Object | list[bpy.types.Object] | None = None,
    hdri_path: Path | None = None,
    hdri_strength: float = 0.55,
    hdri_rotation_deg: float = 118.0,
    hdri_lighting_only: bool = True,
    render_engine: str = "BLENDER_EEVEE",
    cycles_samples: int = 128,
) -> None:
    hero = focus if isinstance(focus, list) else [focus]
    hero_meshes = [o for o in hero if o.type == "MESH"]
    if mask_meshes is not None:
        mask_focus = mask_meshes if isinstance(mask_meshes, list) else [mask_meshes]
        mask_heroes = [o for o in mask_focus if o.type == "MESH"]
    else:
        mask_heroes = hero_meshes
    if not hero_meshes:
        hero_meshes = [
            o for o in bpy.context.scene.objects if o.type == "MESH" and o.name != "PreviewShadowFloor"
        ]

    scene = bpy.context.scene
    engine = render_engine.upper()
    scene.render.engine = "CYCLES" if engine == "CYCLES" else "BLENDER_EEVEE"
    if scene.render.engine == "CYCLES":
        scene.cycles.device = "CPU"
        scene.cycles.samples = max(128, int(cycles_samples))
        scene.cycles.use_denoising = True
        if hasattr(scene.cycles, "denoiser"):
            scene.cycles.denoiser = "OPENIMAGEDENOISE"
    elif hasattr(scene, "eevee"):
        scene.eevee.taa_render_samples = max(128, int(cycles_samples))
        if hasattr(scene.eevee, "use_gtao"):
            scene.eevee.use_gtao = True
            scene.eevee.gtao_distance = 0.55
            scene.eevee.gtao_factor = 1.25
        if hasattr(scene.eevee, "use_soft_shadows"):
            scene.eevee.use_soft_shadows = True
        if hasattr(scene.eevee, "use_shadows"):
            scene.eevee.use_shadows = True
        if hasattr(scene.eevee, "shadow_cube_size"):
            scene.eevee.shadow_cube_size = "2048"
        if hasattr(scene.eevee, "shadow_cascade_size"):
            scene.eevee.shadow_cascade_size = "2048"

    if hdri_path and hdri_path.is_file():
        if hdri_lighting_only:
            setup_hdri_lighting_only(hdri_path, strength=hdri_strength, rotation_z_deg=hdri_rotation_deg)
        else:
            setup_hdri_world(hdri_path, strength=hdri_strength, rotation_z_deg=hdri_rotation_deg)
    else:
        setup_studio_world()
    mins, maxs = mesh_bounds_world(hero_meshes)
    axis = (up_axis or infer_up_axis(mins, maxs)).upper()
    add_contact_floor = path.stem != "garage-scene-urp-hero"
    if add_contact_floor:
        if axis == "Y":
            add_shadow_floor_y(mins, maxs)
        else:
            add_shadow_floor_z(mins, maxs)
    center = (mins + maxs) * 0.5
    radius = bounding_sphere_radius(mins, maxs, hero_meshes)
    if path.stem == "garage-scene-urp-hero" and hdri_path and hdri_lighting_only:
        light_mul = 2.05
    elif hdri_path and hdri_lighting_only:
        light_mul = 3.4
    else:
        light_mul = 1.0
    setup_three_point_lights(center, radius, energy_scale=light_mul)
    cam = frame_camera(
        hero_meshes,
        lens_mm=lens_mm,
        margin=framing_margin,
        azimuth_deg=azimuth_deg,
        elevation_deg=elevation_deg,
        up_axis=axis,
    )
    half_fov = camera_half_fov_rad(cam.data)
    cam_dist = (radius / math.tan(half_fov)) * framing_margin

    bpy.context.view_layer.update()
    ndc_x: list[float] = []
    ndc_y: list[float] = []
    for ob in hero_meshes:
        if ob.type != "MESH":
            continue
        for vert in ob.data.vertices:
            co = world_to_camera_view(scene, cam, ob.matrix_world @ vert.co)
            ndc_x.append(co.x)
            ndc_y.append(co.y)
    if ndc_x:
        screen_extent = max(max(ndc_x) - min(ndc_x), max(ndc_y) - min(ndc_y))
    else:
        screen_extent = 0.0

    meta_path = path.with_name(path.stem + ".frame.json")
    meta_path.write_text(
        json.dumps(
            {
                "camera_dist": cam_dist,
                "bbox_radius": radius,
                "framing_margin": framing_margin,
                "screen_extent": screen_extent,
                "lens_mm": lens_mm,
                "azimuth_deg": azimuth_deg,
                "elevation_deg": elevation_deg,
                "up_axis": axis,
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )

    scene.view_settings.view_transform = "Standard"
    scene.view_settings.look = "None"
    scene.view_settings.exposure = -0.72 if path.stem == "garage-scene-urp-hero" else 0.0
    scene.render.film_transparent = False
    scene.render.image_settings.color_mode = "RGB"
    scene.render.resolution_x = resolution
    scene.render.resolution_y = resolution
    scene.render.filepath = str(path)
    path.parent.mkdir(parents=True, exist_ok=True)
    bpy.ops.render.render(write_still=True)
    mask_path = path.with_name(path.stem + ".mask.png")
    render_hero_mask(mask_heroes, mask_path, resolution)