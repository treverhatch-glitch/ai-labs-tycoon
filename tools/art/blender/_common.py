"""
Shared headless Blender 4.x bake: CC0 kit imports or campus-kit OBJ → glTF/FBX + preview.
Run only inside Blender: blender --background --python tools/art/blender/<asset>.py
"""
from __future__ import annotations

import json
import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
OBJ_IN = REPO / "unity" / "Assets" / "Art" / "Generated" / "campus-kit"
CC0_ROOT = REPO / "docs" / "art" / "cc0"
PREVIEW_HDRI = CC0_ROOT / "polyhaven" / "hdr" / "studio_small_09_1k.hdr"
OUT_ROOT = REPO / "unity" / "Assets" / "Art" / "BlenderBaked"
PREVIEW_DIR = REPO / "docs" / "art" / "previews"
TEX_SIZE = 1024
SCOPE = "presentation"  # outside EarlyGame/


def _reset_scene() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)


def _import_gltf_mesh(glb_path: Path) -> list[bpy.types.Object]:
    before = {o.name for o in bpy.data.objects}
    bpy.ops.import_scene.gltf(filepath=str(glb_path))
    added = [o for o in bpy.data.objects if o.name not in before and o.type == "MESH"]
    return added or [o for o in bpy.context.selected_objects if o.type == "MESH"]


def _bbox_world(objs: list[bpy.types.Object]) -> tuple[Vector, Vector]:
    return _world_bbox(objs)


def _apply_scale_uniform(objs: list[bpy.types.Object], factor: float) -> None:
    for o in objs:
        o.scale = (o.scale.x * factor, o.scale.y * factor, o.scale.z * factor)
    bpy.ops.object.select_all(action="DESELECT")
    for o in objs:
        o.select_set(True)
    if objs:
        bpy.context.view_layer.objects.active = objs[0]
        bpy.ops.object.transform_apply(scale=True)


def _bevel_meshes(objs: list[bpy.types.Object], width: float = 0.006, segments: int = 2) -> None:
    for o in objs:
        bpy.ops.object.select_all(action="DESELECT")
        o.select_set(True)
        bpy.context.view_layer.objects.active = o
        mod = o.modifiers.new("Bevel", type="BEVEL")
        mod.width = width
        mod.segments = segments
        mod.limit_method = "ANGLE"
        bpy.ops.object.modifier_apply(modifier=mod.name)


def _paint_mesh_vertex_color(obj: bpy.types.Object, rgb: tuple[float, float, float]) -> None:
    mesh = obj.data
    if mesh.color_attributes.get("BakeColor") is None:
        mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    vcol = mesh.color_attributes["BakeColor"]
    col = (rgb[0], rgb[1], rgb[2], 1.0)
    for corner_i in range(len(mesh.loops)):
        vcol.data[corner_i].color = col


def _import_obj(obj_path: Path) -> bpy.types.Object:
    bpy.ops.wm.obj_import(filepath=str(obj_path))
    objs = [o for o in bpy.context.selected_objects if o.type == "MESH"]
    if not objs:
        raise RuntimeError(f"No mesh after OBJ import: {obj_path}")
    root = objs[0]
    if len(objs) > 1:
        bpy.context.view_layer.objects.active = root
        for o in objs[1:]:
            o.select_set(True)
        bpy.ops.object.join()
    return root


def _ensure_material(obj: bpy.types.Object, asset_id: str) -> bpy.types.Material:
    mat_name = f"M_{asset_id}"
    mat = bpy.data.materials.get(mat_name) or bpy.data.materials.new(mat_name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Base Color"].default_value = (0.72, 0.74, 0.78, 1.0)
    bsdf.inputs["Roughness"].default_value = 0.55
    bsdf.inputs["Metallic"].default_value = 0.08
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    if obj.data.materials:
        obj.data.materials[0] = mat
    else:
        obj.data.materials.append(mat)
    return mat


def _add_bake_target(mat: bpy.types.Material) -> tuple[bpy.types.ShaderNodeTexImage, bpy.types.Image]:
    """Bake target image only — do not wire into BSDF before bake (avoids black circular dep)."""
    img_name = f"T_{mat.name}"
    if img_name in bpy.data.images:
        bpy.data.images.remove(bpy.data.images[img_name])
    image = bpy.data.images.new(img_name, TEX_SIZE, TEX_SIZE, alpha=False)
    nodes = mat.node_tree.nodes
    tex = nodes.new("ShaderNodeTexImage")
    tex.image = image
    tex.select = True
    nodes.active = tex
    return tex, image


def _apply_baked_albedo(mat: bpy.types.Material, tex_path: Path) -> None:
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    bsdf = next(n for n in nodes if n.type == "BSDF_PRINCIPLED")
    for n in list(nodes):
        if n.type == "TEX_IMAGE":
            nodes.remove(n)
    tex = nodes.new("ShaderNodeTexImage")
    rel = str(tex_path)
    if rel not in bpy.data.images:
        img = bpy.data.images.load(rel, check_existing=True)
    else:
        img = bpy.data.images[rel]
    tex.image = img
    img.colorspace_settings.name = "sRGB"
    if bsdf.inputs["Base Color"].links:
        for link in bsdf.inputs["Base Color"].links:
            links.remove(link)
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])


def _smart_uv(obj: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.uv.smart_project(angle_limit=math.radians(66), island_margin=0.02)
    bpy.ops.object.mode_set(mode="OBJECT")


def _rasterize_vertex_colors(obj: bpy.types.Object, tex_path: Path) -> None:
    """Paint 1024 basecolor from UV + BakeColor (reliable vs Cycles emit on headless)."""
    mesh = obj.data
    attr = mesh.color_attributes.get("BakeColor")
    uv = mesh.uv_layers.active
    if attr is None or uv is None:
        raise RuntimeError("missing BakeColor UV")
    tex_name = f"T_raster_{obj.name}"
    if tex_name in bpy.data.images:
        bpy.data.images.remove(bpy.data.images[tex_name])
    img = bpy.data.images.new(tex_name, TEX_SIZE, TEX_SIZE, alpha=False)
    px = [0.55, 0.55, 0.56, 1.0] * (TEX_SIZE * TEX_SIZE)
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


def _bake_maps(obj: bpy.types.Object, mat: bpy.types.Material, out_dir: Path) -> Path:
    tex_dir = out_dir / "textures"
    tex_dir.mkdir(parents=True, exist_ok=True)
    tex_path = tex_dir / f"{obj.name}_basecolor.png"
    _rasterize_vertex_colors(obj, tex_path)

    _add_bake_target(mat)
    bpy.ops.object.select_all(action="DESELECT")
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    bpy.context.scene.render.engine = "CYCLES"
    bpy.context.scene.cycles.device = "CPU"
    bpy.context.scene.cycles.samples = 24

    ao_path = tex_dir / f"{obj.name}_ao.png"
    ao_img = bpy.data.images.new(f"T_ao_{mat.name}", TEX_SIZE, TEX_SIZE, alpha=False)
    ao_node = mat.node_tree.nodes.new("ShaderNodeTexImage")
    ao_node.image = ao_img
    ao_node.select = True
    mat.node_tree.nodes.active = ao_node
    bpy.ops.object.bake(type="AO", margin=8, use_clear=True)
    ao_img.filepath_raw = str(ao_path)
    ao_img.file_format = "PNG"
    ao_img.save()
    return tex_path


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


def _material_for_part(name: str, rgba: tuple[float, float, float], emissive: bool = False) -> bpy.types.Material:
    mat = bpy.data.materials.get(name) or bpy.data.materials.new(name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Base Color"].default_value = (*rgba, 1.0)
    bsdf.inputs["Roughness"].default_value = 0.52
    bsdf.inputs["Metallic"].default_value = 0.1 if not emissive else 0.0
    if emissive:
        em = bsdf.inputs.get("Emission Color") or bsdf.inputs.get("Emission")
        if em is not None:
            em.default_value = (*rgba, 1.0)
        strength = bsdf.inputs.get("Emission Strength")
        if strength is not None:
            strength.default_value = 3.5
    mat.node_tree.links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    return mat


def _vertex_color_from_slots(obj: bpy.types.Object, part_colors: list[tuple[float, float, float]]) -> None:
    mesh = obj.data
    if mesh.color_attributes:
        mesh.color_attributes.active_color = None
        while mesh.color_attributes:
            mesh.color_attributes.remove(mesh.color_attributes[0])
    vcol = mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    for poly in mesh.polygons:
        idx = poly.material_index % len(part_colors)
        r, g, b = part_colors[idx]
        col = (r, g, b, 1.0)
        for loop_i in poly.loop_indices:
            vcol.data[loop_i].color = col


def _material_vertex_color(name: str) -> bpy.types.Material:
    mat = bpy.data.materials.get(name) or bpy.data.materials.new(name)
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
    return mat


def _join_colored_parts(parts: list[bpy.types.Object]) -> bpy.types.Object:
    bpy.ops.object.select_all(action="DESELECT")
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = parts[0]
    if len(parts) > 1:
        bpy.ops.object.join()
    return bpy.context.view_layer.objects.active


def _export_gltf(obj: bpy.types.Object, out_dir: Path) -> Path:
    glb = out_dir / f"{obj.name}.glb"
    bpy.ops.object.select_all(action="DESELECT")
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    for img in bpy.data.images:
        if img.filepath and not img.packed_file:
            img.pack()
    bpy.ops.export_scene.gltf(
        filepath=str(glb),
        export_format="GLB",
        use_selection=True,
        export_texcoords=True,
        export_materials="EXPORT",
        export_image_format="AUTO",
        export_keep_originals=False,
    )
    return glb


def _export_fbx(obj: bpy.types.Object, out_dir: Path) -> Path:
    fbx = out_dir / f"{obj.name}.fbx"
    bpy.ops.object.select_all(action="DESELECT")
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    bpy.ops.export_scene.fbx(
        filepath=str(fbx),
        use_selection=True,
        apply_scale_options="FBX_SCALE_ALL",
        mesh_smooth_type="FACE",
    )
    return fbx


def _world_bbox(objs: list[bpy.types.Object]) -> tuple[Vector, Vector]:
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in objs:
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector((corner[0], corner[1], corner[2]))
            mins.x = min(mins.x, w.x)
            mins.y = min(mins.y, w.y)
            mins.z = min(mins.z, w.z)
            maxs.x = max(maxs.x, w.x)
            maxs.y = max(maxs.y, w.y)
            maxs.z = max(maxs.z, w.z)
    return mins, maxs


def _setup_preview_hdri(world: bpy.types.World, hdr_path: Path, backdrop: float = 150.0 / 255.0) -> None:
    """~150 sRGB grey for camera; HDRI at low strength for lighting only."""
    world.use_nodes = True
    nodes = world.node_tree.nodes
    links = world.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputWorld")
    bg_grey = nodes.new("ShaderNodeBackground")
    bg_grey.inputs["Color"].default_value = (backdrop, backdrop, backdrop + 0.01, 1.0)
    bg_grey.inputs["Strength"].default_value = 1.0
    if hdr_path.is_file():
        env = nodes.new("ShaderNodeTexEnvironment")
        if str(hdr_path) not in bpy.data.images:
            env.image = bpy.data.images.load(str(hdr_path), check_existing=True)
        else:
            env.image = bpy.data.images[str(hdr_path)]
        env.image.colorspace_settings.name = "Linear Rec.709"
        bg_lit = nodes.new("ShaderNodeBackground")
        bg_lit.inputs["Strength"].default_value = 0.35
        links.new(env.outputs["Color"], bg_lit.inputs["Color"])
        path = nodes.new("ShaderNodeLightPath")
        mix = nodes.new("ShaderNodeMixShader")
        links.new(path.outputs["Is Camera Ray"], mix.inputs["Fac"])
        links.new(bg_grey.outputs["Background"], mix.inputs[1])
        links.new(bg_lit.outputs["Background"], mix.inputs[2])
        links.new(mix.outputs["Shader"], out.inputs["Surface"])
    else:
        links.new(bg_grey.outputs["Background"], out.inputs["Surface"])


def _render_preview_from_glb(glb_path: Path, asset_id: str) -> Path:
    """Poly Haven HDRI + mid-grey ground, 3/4 ortho camera, 1024, from committed GLB."""
    import hashlib
    import json

    PREVIEW_DIR.mkdir(parents=True, exist_ok=True)
    preview = PREVIEW_DIR / f"{asset_id}.png"
    _reset_scene()
    world = bpy.context.scene.world or bpy.data.worlds.new("World")
    bpy.context.scene.world = world
    _setup_preview_hdri(world, PREVIEW_HDRI)
    bpy.ops.import_scene.gltf(filepath=str(glb_path))
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not mesh_obs:
        raise RuntimeError(f"no mesh in {glb_path}")

    mins, maxs = _world_bbox(mesh_obs)
    center = (mins + maxs) / 2
    span = max(maxs.x - mins.x, maxs.y - mins.y, maxs.z - mins.z, 0.01)

    bpy.ops.mesh.primitive_plane_add(size=span * 4, location=(center.x, center.y, mins.z - 0.002))
    ground = bpy.context.active_object
    gmat = bpy.data.materials.new("Ground")
    gmat.use_nodes = True
    floor_g = 0.46
    gmat.node_tree.nodes["Principled BSDF"].inputs["Base Color"].default_value = (floor_g, floor_g, floor_g + 0.02, 1)
    gmat.node_tree.nodes["Principled BSDF"].inputs["Roughness"].default_value = 0.92
    ground.data.materials.append(gmat)

    bpy.ops.mesh.primitive_plane_add(size=span * 6, location=(center.x, center.y + span * 1.5, center.z + span * 0.2), rotation=(math.radians(90), 0, 0))
    backdrop = bpy.context.active_object
    bmat = bpy.data.materials.new("Backdrop")
    bmat.use_nodes = True
    bmat.node_tree.nodes["Principled BSDF"].inputs["Base Color"].default_value = (150 / 255, 150 / 255, 152 / 255, 1)
    backdrop.data.materials.append(bmat)

    bpy.ops.object.light_add(type="SUN", location=center + Vector((span * 2, -span * 2, span * 3)))
    key = bpy.context.active_object
    key.data.energy = 2.4 if asset_id == "prop-desk" else 1.8
    key.rotation_euler = (math.radians(52), 0, math.radians(35))

    bpy.ops.object.light_add(type="AREA", location=center + Vector((-span, span * 1.2, span * 1.5)))
    fill = bpy.context.active_object
    fill.data.energy = 55
    fill.data.size = span * 1.4

    obj_h = max(maxs.y - mins.y, 0.01)
    obj_w = max(maxs.x - mins.x, maxs.z - mins.z, 0.01)
    target_fill = 0.335 if asset_id == "prop-desk" else 0.50
    pad = 0.88 if asset_id == "prop-desk" else 1.0
    ortho = max(obj_h / target_fill, (obj_w * 1.18) / target_fill) * pad

    cam_data = bpy.data.cameras.new("PreviewCam")
    cam = bpy.data.objects.new("PreviewCam", cam_data)
    bpy.context.collection.objects.link(cam)
    cam.data.type = "ORTHO"
    cam.data.ortho_scale = ortho

    dist = span * 4.0
    if asset_id == "ui-rival-badge":
        elev = math.radians(6)
        az = math.radians(0)
        cam.location = center + Vector((0.0, -dist, obj_h * 0.05))
    else:
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
        if hasattr(ee, "use_soft_shadows"):
            ee.use_soft_shadows = True
        if hasattr(ee, "use_shadows"):
            ee.use_shadows = True
    bpy.context.scene.render.film_transparent = False
    bpy.context.scene.render.resolution_x = 1024
    bpy.context.scene.render.resolution_y = 1024
    bpy.context.scene.render.filepath = str(preview)
    bpy.context.scene.render.image_settings.file_format = "PNG"
    bpy.context.scene.render.image_settings.color_mode = "RGB"
    if hasattr(bpy.context.scene, "view_settings"):
        bpy.context.scene.view_settings.exposure = 0.2 if asset_id == "prop-desk" else 0.22
        bpy.context.scene.view_settings.view_transform = "Standard"
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
                "resolution": 1024,
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )
    return preview


def _part_palette(asset_id: str, base: tuple[float, float, float]) -> list[tuple[float, float, float]]:
    """Tint variants for loose parts (visible in bake + preview)."""
    r, g, b = base
    accents: dict[str, list[tuple[float, float, float]]] = {
        "prop-desk": [
            (0.62, 0.55, 0.48),
            (0.52, 0.46, 0.4),
            (0.28, 0.3, 0.34),
            (0.12, 0.13, 0.15),
            (0.85, 0.87, 0.9),
            (0.22, 0.24, 0.26),
            (0.15, 0.35, 0.55),
        ],
        "prop-server-rack": [
            (0.22, 0.24, 0.28),
            (0.34, 0.36, 0.4),
            (0.42, 0.44, 0.48),
            (0.18, 0.2, 0.22),
            (0.12, 0.78, 0.38),
            (0.18, 0.42, 0.88),
        ],
        "prop-whiteboard": [
            (0.97, 0.98, 0.99),
            (0.72, 0.74, 0.76),
            (0.58, 0.6, 0.62),
            (0.2, 0.45, 0.85),
            (0.85, 0.2, 0.22),
            (0.25, 0.75, 0.35),
        ],
        "ui-rival-badge": [
            (0.9, 0.18, 0.28),
            (0.14, 0.42, 0.88),
            (0.95, 0.78, 0.18),
            (0.82, 0.12, 0.22),
            (0.12, 0.55, 0.92),
            (0.88, 0.22, 0.35),
        ],
    }
    return accents.get(asset_id, [base, (r * 1.1, g, b * 0.95), (r * 0.75, g * 0.75, b * 0.8)])


def finish_baked_export(
    asset_id: str,
    obj: bpy.types.Object,
    source_cc0: str,
    extra_manifest: dict | None = None,
) -> dict:
    """UV, 1024 albedo + AO, glTF/FBX, HDRI preview for an already-assembled mesh."""
    obj.name = f"mesh_{asset_id}"
    bake_mat = _material_vertex_color(f"M_bake_{asset_id}")
    obj.data.materials.clear()
    obj.data.materials.append(bake_mat)
    _smart_uv(obj)
    out_dir = OUT_ROOT / asset_id
    out_dir.mkdir(parents=True, exist_ok=True)
    tex_path = _bake_maps(obj, bake_mat, out_dir)
    urp_mat = _ensure_material(obj, asset_id)
    _apply_baked_albedo(urp_mat, tex_path)
    obj.data.materials.clear()
    obj.data.materials.append(urp_mat)
    ao_path = tex_path.parent / f"{obj.name}_ao.png"
    glb = _export_gltf(obj, out_dir)
    fbx = _export_fbx(obj, out_dir)
    preview_rel = f"docs/art/previews/{asset_id}.png"
    manifest = {
        "assetId": asset_id,
        "scope": SCOPE,
        "sourceCc0": source_cc0,
        "pipeline": "cc0-restyle-v1",
        "texture1024": str(tex_path.relative_to(REPO)),
        "textureAo1024": str(ao_path.relative_to(REPO)),
        "urpAlbedoPath": str(tex_path.relative_to(REPO)).replace("unity/", "Assets/"),
        "glb": str(glb.relative_to(REPO)),
        "fbx": str(fbx.relative_to(REPO)),
        "preview": preview_rel,
        "previewHdri": str(PREVIEW_HDRI.relative_to(REPO)),
    }
    if extra_manifest:
        manifest.update(extra_manifest)
    (out_dir / "manifest.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    _render_preview_from_glb(glb, asset_id)
    return manifest


def bake_asset(asset_id: str, palette: tuple[float, float, float] | None = None) -> dict:
    """Import campus-kit OBJ, UV, bake 1024 base color, export glTF/FBX, preview."""
    obj_path = OBJ_IN / f"{asset_id}.obj"
    if not obj_path.is_file():
        raise FileNotFoundError(obj_path)
    base = palette or (0.72, 0.74, 0.78)
    part_colors = _part_palette(asset_id, base)
    _reset_scene()
    obj = _import_obj(obj_path)
    obj.name = f"mesh_{asset_id}"
    mod = obj.modifiers.new("Bevel", type="BEVEL")
    mod.width = 0.01
    mod.segments = 2
    mod.limit_method = "ANGLE"
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    bpy.ops.object.modifier_apply(modifier=mod.name)
    parts = _split_loose_parts(obj)
    for i, p in enumerate(parts):
        c = part_colors[i % len(part_colors)]
        is_led = asset_id == "prop-server-rack" and (i % 9) in (7, 8)
        if is_led:
            c = (0.12, 0.82, 0.38) if (i % 2) == 0 else (0.15, 0.45, 0.92)
        mat = _material_for_part(f"M_{asset_id}_{i}", c, emissive=is_led)
        if p.data.materials:
            p.data.materials[0] = mat
        else:
            p.data.materials.append(mat)
        mesh = p.data
        if mesh.color_attributes.get("BakeColor") is None:
            mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
        vcol = mesh.color_attributes["BakeColor"]
        col = (c[0], c[1], c[2], 1.0)
        for corner_i in range(len(mesh.loops)):
            vcol.data[corner_i].color = col
    obj = _join_colored_parts(parts)
    obj.name = f"mesh_{asset_id}"
    bake_mat = _material_vertex_color(f"M_bake_{asset_id}")
    obj.data.materials.clear()
    obj.data.materials.append(bake_mat)
    _smart_uv(obj)
    out_dir = OUT_ROOT / asset_id
    out_dir.mkdir(parents=True, exist_ok=True)
    tex_path = _bake_maps(obj, bake_mat, out_dir)
    urp_mat = _ensure_material(obj, asset_id)
    _apply_baked_albedo(urp_mat, tex_path)
    obj.data.materials.clear()
    obj.data.materials.append(urp_mat)
    glb = _export_gltf(obj, out_dir)
    fbx = _export_fbx(obj, out_dir)
    preview = _render_preview_from_glb(glb, asset_id)
    manifest = {
        "assetId": asset_id,
        "scope": SCOPE,
        "sourceObj": str(obj_path.relative_to(REPO)),
        "texture1024": str(tex_path.relative_to(REPO)),
        "urpAlbedoPath": str(tex_path.relative_to(REPO)).replace("unity/", "Assets/"),
        "glb": str(glb.relative_to(REPO)),
        "fbx": str(fbx.relative_to(REPO)),
        "preview": str(preview.relative_to(REPO)),
    }
    (out_dir / "manifest.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    return manifest


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: blender --background --python _common.py -- <asset_id>", file=sys.stderr)
        raise SystemExit(2)
    aid = sys.argv[-1]
    print(json.dumps(bake_asset(aid), indent=2))