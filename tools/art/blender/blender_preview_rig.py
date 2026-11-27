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

# Mid-grey seamless studio — no beige gradient wall (per Hatch pkt1 pass 4)
_GREY_150 = 150.0 / 255.0
STUDIO_GREY = (_GREY_150, _GREY_150, _GREY_150, 1.0)
FLOOR_GREY = STUDIO_GREY

FRAMING_MARGIN = 1.15  # Hatch: radius/tan(fov/2)*1.15
DEFAULT_AZIMUTH_DEG = 45.0
DEFAULT_ELEVATION_DEG = 32.0
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


def setup_studio_world(*, staff_flat: bool = False) -> None:
    world = bpy.data.worlds.new("EarlyPreviewWorld")
    bpy.context.scene.world = world
    world.use_nodes = True
    nodes = world.node_tree.nodes
    links = world.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputWorld")
    bg = nodes.new("ShaderNodeBackground")
    # Mid-grey visible backdrop; HDRI may light objects but stays hidden in the render.
    bg.inputs["Color"].default_value = STUDIO_GREY
    bg.inputs["Strength"].default_value = 1.0
    hdr_path = Path(__file__).resolve().parents[2] / "cc0" / "incoming" / "polyhaven" / "studio_small_08_2k.hdr"
    if staff_flat or not hdr_path.is_file():
        bpy.context.scene.view_settings.view_transform = "Standard"
        bpy.context.scene.view_settings.exposure = -0.958
        bpy.context.scene.view_settings.gamma = 1.0
        links.new(bg.outputs["Background"], out.inputs["Surface"])
        return
    if hdr_path.is_file():
        env = nodes.new("ShaderNodeTexEnvironment")
        env.image = bpy.data.images.load(str(hdr_path))
        light = nodes.new("ShaderNodeBackground")
        light.inputs["Strength"].default_value = 0.22
        links.new(env.outputs["Color"], light.inputs["Color"])
        mix = nodes.new("ShaderNodeMixShader")
        mix.inputs["Fac"].default_value = 0.0
        links.new(bg.outputs["Background"], mix.inputs[1])
        links.new(light.outputs["Background"], mix.inputs[2])
        links.new(mix.outputs["Shader"], out.inputs["Surface"])
        return
    links.new(bg.outputs["Background"], out.inputs["Surface"])


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
    if hasattr(floor, "is_shadow_catcher"):
        floor.is_shadow_catcher = True
    return floor


def add_shadow_floor_z(mins: Vector, maxs: Vector) -> bpy.types.Object:
    center = (mins + maxs) * 0.5
    footprint = max(maxs.x - mins.x, maxs.y - mins.y, 0.35)
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
    if hasattr(floor, "is_shadow_catcher"):
        floor.is_shadow_catcher = True
    return floor


def infer_up_axis(mins: Vector, maxs: Vector) -> str:
    span = maxs - mins
    return "Y" if span.y >= span.z * 1.1 else "Z"


def staff_face_forward(mesh_obs: list[bpy.types.Object]) -> Vector:
    """Face forward: hips/torso → head, projected off the standing axis (Z-up import)."""
    by_name = {o.name.split(".")[0]: o for o in mesh_obs if o.type == "MESH"}
    hips = by_name.get("Hips") or by_name.get("Torso")
    head = by_name.get("Head")
    up = Vector((0.0, 0.0, 1.0))
    mins, maxs = mesh_bounds_world(mesh_obs)
    if (maxs - mins).z < (maxs - mins).y:
        up = Vector((0.0, 1.0, 0.0))
    if head and head.data.vertices:
        pts = [head.matrix_world @ v.co for v in head.data.vertices]
        hc = sum(pts, Vector()) / len(pts)
        eye_l = by_name.get("Eye_-1")
        eye_r = by_name.get("Eye_1")
        if eye_l and eye_r:
            el = sum((eye_l.matrix_world @ v.co for v in eye_l.data.vertices), Vector())
            el /= max(1, len(eye_l.data.vertices))
            er = sum((eye_r.matrix_world @ v.co for v in eye_r.data.vertices), Vector())
            er /= max(1, len(eye_r.data.vertices))
            mid = (el + er) * 0.5
            fwd = mid - hc
            fwd.z = 0.0
            if fwd.length > 1e-4:
                if fwd.dot(Vector((0.0, 1.0, 0.0))) < 0:
                    fwd = -fwd
                return fwd.normalized()
        if up.z > 0.9:
            nose = min(pts, key=lambda v: v.y)
            back = max(pts, key=lambda v: v.y)
            fwd = Vector((nose.x - back.x, nose.y - back.y, 0.0))
            if fwd.length > 1e-4:
                if fwd.dot(Vector((0.0, 1.0, 0.0))) < 0:
                    fwd = -fwd
                return fwd.normalized()
        fwd = hc - (
            sum((hips.matrix_world @ v.co for v in hips.data.vertices), Vector())
            / max(1, len(hips.data.vertices))
            if hips and hips.data.vertices
            else hc - Vector((0.0, 0.0, 0.35))
        )
        fwd = fwd - up * fwd.dot(up)
        if fwd.length > 1e-4:
            return fwd.normalized()
    return Vector((0.0, 1.0, 0.0))


def frame_staff_front_quarter(
    mesh_obs: list[bpy.types.Object],
    lens_mm: float,
    margin: float,
    yaw_deg: float = 30.0,
    elev_deg: float = 11.0,
) -> tuple[bpy.types.Object, float]:
    mins, maxs = mesh_bounds_world(mesh_obs)
    center = (mins + maxs) * 0.5
    radius = bounding_sphere_radius(mins, maxs, mesh_obs)
    half_fov = math.radians(22.0)
    cam_dist = (radius / math.tan(half_fov)) * margin
    fwd = staff_face_forward(mesh_obs).normalized()
    right = Vector((-fwd.y, fwd.x, 0.0))
    if right.length < 1e-5:
        right = Vector((1.0, 0.0, 0.0))
    else:
        right.normalize()
    yaw = math.radians(yaw_deg)
    elev = math.radians(elev_deg)
    offset = fwd * (math.cos(yaw) * math.cos(elev)) + right * (math.sin(yaw) * math.cos(elev))
    offset.z = math.sin(elev)
    offset = offset.normalized() * cam_dist
    cam_loc = center - offset
    bpy.ops.object.camera_add(location=cam_loc)
    cam = bpy.context.active_object
    cam.name = "PreviewCamera"
    cam.data.lens = lens_mm
    direction = center - cam_loc
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    bpy.context.scene.camera = cam
    return cam, cam_dist


def setup_three_point_lights(center: Vector, radius: float) -> None:
    key_loc = center + Vector((radius * 1.4, -radius * 1.0, radius * 1.2))
    fill_loc = center + Vector((-radius * 1.1, -radius * 0.5, radius * 0.85))
    rim_loc = center + Vector((radius * 0.15, radius * 1.3, radius * 1.0))
    energy = max(120.0, radius * 150.0)
    for name, loc, e_mul in (("Key", key_loc, 0.85), ("Fill", fill_loc, 0.30), ("Rim", rim_loc, 0.40)):
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


def frame_camera_staff_front(
    objects: list[bpy.types.Object],
    lens_mm: float,
    margin: float,
) -> bpy.types.Object:
    """Orthographic front view from −Y: Y-up on screen, no depth shear between parts."""
    mins, maxs = mesh_bounds_world(objects)
    center = (mins + maxs) * 0.5
    height = max(0.35, maxs.y - mins.y)
    width = max(0.25, maxs.x - mins.x, (maxs.z - mins.z) * 0.65)
    ortho_scale = max(height, width) * margin
    cam_loc = Vector((center.x, center.y - 4.0, center.z))
    bpy.ops.object.camera_add(location=cam_loc)
    cam = bpy.context.active_object
    cam.name = "PreviewCamera"
    cam.data.type = "ORTHO"
    cam.data.ortho_scale = ortho_scale
    direction = center - cam_loc
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


def frame_camera_ortho(objects: list[bpy.types.Object], fill: float = 0.70) -> bpy.types.Object:
    mins, maxs = mesh_bounds_world(objects)
    center = (mins + maxs) * 0.5
    height = max(0.15, maxs.z - mins.z, maxs.y - mins.y)
    span = max(maxs.x - mins.x, height * 0.3)
    ortho_scale = max(height, span) / fill
    bpy.ops.object.camera_add(location=center + Vector((0.0, -4.0, center.z)))
    cam = bpy.context.active_object
    cam.name = "PreviewCamera"
    cam.data.type = "ORTHO"
    cam.data.ortho_scale = ortho_scale
    direction = center - cam.location
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    bpy.context.scene.camera = cam
    return cam


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
) -> None:
    preview_kind = os.environ.get("EARLY_ART_PREVIEW_KIND", "staff").lower()
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
    scene.render.engine = "BLENDER_EEVEE"
    if hasattr(scene, "eevee"):
        scene.eevee.taa_render_samples = 32
        if hasattr(scene.eevee, "use_gtao"):
            scene.eevee.use_gtao = True
            scene.eevee.gtao_distance = 0.55
            scene.eevee.gtao_factor = 1.85 if preview_kind == "staff" else 1.4

    setup_studio_world(staff_flat=(preview_kind == "staff"))
    mins, maxs = mesh_bounds_world(hero_meshes)
    if preview_kind == "staff":
        axis = "Z"
        framing_margin = float(os.environ.get("STAFF_PREVIEW_MARGIN", "1.46"))
        lens_mm = 50.0
        azimuth_deg = float(os.environ.get("STAFF_PREVIEW_YAW_DEG", "35"))
        elevation_deg = float(os.environ.get("STAFF_PREVIEW_ELEV_DEG", "11"))
    elif preview_kind in ("wisp",):
        axis = "Y"
    else:
        axis = (up_axis or infer_up_axis(mins, maxs)).upper()
    center = (mins + maxs) * 0.5
    radius = bounding_sphere_radius(mins, maxs, hero_meshes)
    cam_dist = 0.0
    if preview_kind == "hud":
        setup_three_point_lights(center, max(radius, 0.35))
        frame_camera_ortho(hero_meshes)
    elif preview_kind == "wisp":
        azimuth_deg, elevation_deg, framing_margin = 0.0, 12.0, 1.05
        setup_three_point_lights(center, radius)
        cam = frame_camera(
            hero_meshes,
            lens_mm=42.0,
            margin=framing_margin,
            azimuth_deg=azimuth_deg,
            elevation_deg=elevation_deg,
            up_axis=axis,
        )
        half_fov = camera_half_fov_rad(cam.data)
        cam_dist = (radius / math.tan(half_fov)) * framing_margin
        if hasattr(scene.eevee, "use_bloom"):
            scene.eevee.use_bloom = True
    else:
        if preview_kind == "wisp":
            add_shadow_floor_y(mins, maxs)
        elif preview_kind == "staff":
            pass  # contact shadow via GTAO on grey-150 only — no lit floor tile
        elif axis == "Y":
            add_shadow_floor_y(mins, maxs)
        else:
            add_shadow_floor_z(mins, maxs)
        light_r = radius * (0.55 if preview_kind == "staff" else 1.0)
        setup_three_point_lights(center, light_r)
        if preview_kind == "staff":
            key = bpy.data.objects.get("Key")
            if key and key.type == "LIGHT":
                key.data.energy *= 0.040
            for soft_name, mul in (("Fill", 0.35), ("Rim", 0.12)):
                lite = bpy.data.objects.get(soft_name)
                if lite and lite.type == "LIGHT":
                    lite.data.energy *= mul
            staff_yaw_deg = float(os.environ.get("STAFF_PREVIEW_YAW_DEG", "35"))
            staff_elev_deg = float(os.environ.get("STAFF_PREVIEW_ELEV_DEG", "11"))
            cam, cam_dist = frame_staff_front_quarter(
                hero_meshes,
                lens_mm=lens_mm,
                margin=framing_margin,
                yaw_deg=staff_yaw_deg,
                elev_deg=staff_elev_deg,
            )
        else:
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

    meta_path = path.with_name(path.stem + ".frame.json")
    meta_path.write_text(
        json.dumps(
            {
                "camera_dist": cam_dist,
                "bbox_radius": radius,
                "framing_margin": framing_margin,
                "lens_mm": lens_mm,
                "azimuth_deg": azimuth_deg,
                "elevation_deg": elevation_deg,
                "up_axis": axis,
                "preview_kind": preview_kind,
                "staff_yaw_deg": float(os.environ.get("STAFF_PREVIEW_YAW_DEG", "35")),
                "staff_elev_deg": float(os.environ.get("STAFF_PREVIEW_ELEV_DEG", "11")),
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
    render_hero_mask(mask_heroes, mask_path, resolution)