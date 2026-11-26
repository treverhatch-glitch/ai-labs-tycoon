"""
Shared headless Blender 4.x bake for campus-kit OBJ → PBR + glTF/FBX + preview.
Run only inside Blender: blender --background --python tools/art/blender/<asset>.py
License: CC0 procedural pipeline (repo-owned meshes).
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
OUT_ROOT = REPO / "unity" / "Assets" / "Art" / "BlenderBaked"
PREVIEW_DIR = REPO / "docs" / "art" / "previews"
TEX_SIZE = 1024
SCOPE = "presentation"  # outside EarlyGame/


def _reset_scene() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)


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
            w = ob.matrix_world @ Vector(corner)
            mins.x = min(mins.x, w.x)
            mins.y = min(mins.y, w.y)
            mins.z = min(mins.z, w.z)
            maxs.x = max(maxs.x, w.x)
            maxs.y = max(maxs.y, w.y)
            maxs.z = max(maxs.z, w.z)
    return mins, maxs


def _render_preview_from_glb(glb_path: Path, asset_id: str) -> Path:
    """Standard Hatch rig: grey ground, contact shadow, 3/4 camera, 1024, from committed GLB."""
    import hashlib
    import json

    PREVIEW_DIR.mkdir(parents=True, exist_ok=True)
    preview = PREVIEW_DIR / f"{asset_id}.png"
    _reset_scene()
    world = bpy.context.scene.world or bpy.data.worlds.new("World")
    bpy.context.scene.world = world
    world.use_nodes = True
    bg = world.node_tree.nodes.get("Background") or world.node_tree.nodes.new("ShaderNodeBackground")
    bg.inputs["Color"].default_value = (0.44, 0.44, 0.46, 1.0)
    bg.inputs["Strength"].default_value = 0.85
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
    gmat.node_tree.nodes["Principled BSDF"].inputs["Base Color"].default_value = (0.48, 0.48, 0.5, 1)
    gmat.node_tree.nodes["Principled BSDF"].inputs["Roughness"].default_value = 0.88
    ground.data.materials.append(gmat)

    bpy.ops.object.light_add(type="SUN", location=center + Vector((span * 2, -span * 2, span * 3)))
    key = bpy.context.active_object
    key.data.energy = 3.2
    key.rotation_euler = (math.radians(55), 0, math.radians(35))

    bpy.ops.object.light_add(type="AREA", location=center + Vector((-span, span * 1.5, span * 2)))
    fill = bpy.context.active_object
    fill.data.energy = 180
    fill.data.size = span

    cam_data = bpy.data.cameras.new("PreviewCam")
    cam = bpy.data.objects.new("PreviewCam", cam_data)
    bpy.context.collection.objects.link(cam)
    vertical = max(maxs.y - mins.y, span * 0.4)
    dist = vertical / math.tan(math.radians(20)) * 0.72
    if span < 0.65:
        dist *= 0.42
    dist = max(dist, span * 1.2)
    if asset_id == "ui-rival-badge":
        dist *= 0.38
        key.data.energy = 4.2
    elev = math.radians(30)
    az = math.radians(45)
    cam.location = center + Vector(
        (dist * math.cos(elev) * math.cos(az), dist * math.cos(elev) * math.sin(az), dist * math.sin(elev) + span * 0.25)
    )
    direction = center - cam.location
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    cam.data.lens = 52
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
    if hasattr(bpy.context.scene, "view_settings"):
        bpy.context.scene.view_settings.exposure = 0.28
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
            (r, g, b),
            (r * 0.85, g * 0.85, b * 0.9),
            (0.2, 0.22, 0.25),
            (0.88, 0.9, 0.93),
            (0.15, 0.17, 0.2),
            (0.35, 0.38, 0.42),
        ],
        "prop-server-rack": [
            (0.32, 0.35, 0.4),
            (0.45, 0.48, 0.52),
            (0.25, 0.27, 0.3),
            (0.12, 0.85, 0.35),
            (0.9, 0.25, 0.2),
        ],
        "prop-whiteboard": [
            (0.94, 0.95, 0.96),
            (0.55, 0.5, 0.48),
            (0.4, 0.42, 0.45),
            (0.2, 0.35, 0.75),
            (0.75, 0.2, 0.2),
        ],
        "ui-rival-badge": [
            (0.18, 0.2, 0.24),
            (0.85, 0.25, 0.3),
            (0.92, 0.75, 0.2),
            (0.95, 0.95, 0.97),
            (0.35, 0.4, 0.85),
        ],
    }
    return accents.get(asset_id, [base, (r * 1.1, g, b * 0.95), (r * 0.75, g * 0.75, b * 0.8)])


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
    emissive_led = asset_id == "prop-server-rack"
    for i, p in enumerate(parts):
        c = part_colors[i % len(part_colors)]
        is_led = emissive_led and i % 5 == 3
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