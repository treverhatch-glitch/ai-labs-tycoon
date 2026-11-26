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
        "compute-pc-tower": [
            (0.32, 0.34, 0.38),
            (r * 0.55, g * 0.55, b * 0.55),
            (0.22, 0.24, 0.28),
            (0.15, 0.55, 0.85),
            (0.75, 0.78, 0.82),
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


def rasterize_vertex_colors(ob: bpy.types.Object, tex_path: Path) -> None:
    mesh = ob.data
    attr = mesh.color_attributes.get("BakeColor")
    uv = mesh.uv_layers.active
    if attr is None or uv is None:
        raise RuntimeError("missing BakeColor or UV")
    tex_path.parent.mkdir(parents=True, exist_ok=True)
    img = bpy.data.images.new(f"T_raster_{ob.name}", TEX_SIZE, TEX_SIZE, alpha=False)
    px = [0.45, 0.45, 0.48, 1.0] * (TEX_SIZE * TEX_SIZE)
    for poly in mesh.polygons:
        for li in poly.loop_indices:
            u, v = uv.data[li].uv
            x = min(TEX_SIZE - 1, max(0, int(u * TEX_SIZE)))
            y = min(TEX_SIZE - 1, max(0, int((1.0 - v) * TEX_SIZE)))
            c = attr.data[li].color
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
    bsdf.inputs["Roughness"].default_value = 0.52
    bsdf.inputs["Metallic"].default_value = 0.12
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def export_gltf_y_up(ob: bpy.types.Object, path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
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
        export_apply=True,
        export_texcoords=True,
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


def _setup_hdri_grey_world() -> None:
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
        light.inputs["Strength"].default_value = 0.55
        links.new(env.outputs["Color"], light.inputs["Color"])
        links.new(bg.outputs["Background"], mix.inputs[1])
        links.new(light.outputs["Background"], mix.inputs[2])
        mix.inputs["Fac"].default_value = 0.42
        links.new(mix.outputs["Shader"], out.inputs["Surface"])
    else:
        links.new(bg.outputs["Background"], out.inputs["Surface"])


def render_preview_from_glb(glb_path: Path, asset_id: str) -> Path:
    """3/4 ortho preview on grey ground using committed GLB."""
    import hashlib

    PREVIEW_DIR.mkdir(parents=True, exist_ok=True)
    preview = PREVIEW_DIR / f"{asset_id}.png"
    reset_scene()
    _setup_hdri_grey_world()
    bpy.ops.import_scene.gltf(filepath=str(glb_path))
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not mesh_obs:
        raise RuntimeError(f"no mesh in {glb_path}")
    mins, maxs = _world_bbox(mesh_obs)
    center = (mins + maxs) / 2
    span = max(maxs.x - mins.x, maxs.y - mins.y, maxs.z - mins.z, 0.01)
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
    az = math.radians(45)
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


def finish_cc0_asset(asset_id: str, root: bpy.types.Object, sources: list[str]) -> dict:
    root.name = f"mesh_{asset_id}"
    bevel_w = 0.016 if asset_id.startswith("compute-") and asset_id != "compute-monitor-set" else 0.012
    bevel_object(root, width=bevel_w, segments=2)
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
    bake_mat = bake_material(root, asset_id)
    smart_uv(root)

    out_dir = OUT_ROOT / asset_id
    tex_dir = out_dir / "textures"
    albedo = tex_dir / f"{asset_id}_albedo.png"
    ao = tex_dir / f"{asset_id}_ao.png"
    rasterize_vertex_colors(root, albedo)
    bake_ao(root, bake_mat, ao)
    urp_material(root, asset_id, albedo)

    glb = out_dir / f"{asset_id}.glb"
    export_gltf_y_up(root, glb)
    preview = render_preview_from_glb(glb, asset_id)

    return {
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