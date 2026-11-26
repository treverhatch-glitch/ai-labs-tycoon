"""
Shared headless Blender 4.x bake: CC0 kit imports or campus-kit OBJ → glTF/FBX + preview.
Run only inside Blender: blender --background --python tools/art/blender/<asset>.py
"""
from __future__ import annotations

import json
import math
import shutil
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
OBJ_IN = REPO / "unity" / "Assets" / "Art" / "Generated" / "campus-kit"
CC0_ROOT = REPO / "docs" / "art" / "cc0"
WOOD092 = CC0_ROOT / "ambientcg" / "Wood092_1K-JPG"
WOOD092_COLOR = WOOD092 / "Wood092_1K-JPG_Color.jpg"
WOOD092_NORMAL = WOOD092 / "Wood092_1K-JPG_NormalGL.jpg"
WOOD092_ROUGH = WOOD092 / "Wood092_1K-JPG_Roughness.jpg"
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


def _init_wood_mask(obj: bpy.types.Object, on: bool) -> None:
    mesh = obj.data
    if mesh.color_attributes.get("WoodMask") is None:
        mesh.color_attributes.new(name="WoodMask", type="FLOAT_COLOR", domain="CORNER")
    wm = mesh.color_attributes["WoodMask"]
    val = (1.0, 0.0, 0.0, 1.0) if on else (0.0, 0.0, 0.0, 1.0)
    for i in range(len(mesh.loops)):
        wm.data[i].color = val


def _sample_image_bilinear(
    px: list[float], w: int, h: int, u: float, v: float
) -> tuple[float, float, float]:
    u = u % 1.0
    v = v % 1.0
    fx = max(0.0, min(w - 1.001, u * w))
    fy = max(0.0, min(h - 1.001, (1.0 - v) * h))
    x0, y0 = int(fx), int(fy)
    x1 = min(w - 1, x0 + 1)
    y1 = min(h - 1, y0 + 1)
    tx, ty = fx - x0, fy - y0

    def pix(ix: int, iy: int) -> tuple[float, float, float]:
        o = (iy * w + ix) * 4
        return px[o], px[o + 1], px[o + 2]

    c00 = pix(x0, y0)
    c10 = pix(x1, y0)
    c01 = pix(x0, y1)
    c11 = pix(x1, y1)
    return (
        c00[0] * (1 - tx) * (1 - ty) + c10[0] * tx * (1 - ty) + c01[0] * (1 - tx) * ty + c11[0] * tx * ty,
        c00[1] * (1 - tx) * (1 - ty) + c10[1] * tx * (1 - ty) + c01[1] * (1 - tx) * ty + c11[1] * tx * ty,
        c00[2] * (1 - tx) * (1 - ty) + c10[2] * tx * (1 - ty) + c01[2] * (1 - tx) * ty + c11[2] * tx * ty,
    )


def _lerp3(a: tuple[float, float, float], b: tuple[float, float, float], t: float) -> tuple[float, float, float]:
    t = max(0.0, min(1.0, t))
    return (a[0] * (1 - t) + b[0] * t, a[1] * (1 - t) + b[1] * t, a[2] * (1 - t) + b[2] * t)


def _paint_wood_grain(
    obj: bpy.types.Object,
    light: tuple[float, float, float] = (0.78, 0.54, 0.34),
    dark: tuple[float, float, float] = (0.52, 0.34, 0.22),
    grain_scale: float = 42.0,
) -> None:
    """Procedural streak grain for cc0-restyle-v1 albedo bake (presentation hero)."""
    mesh = obj.data
    if mesh.color_attributes.get("BakeColor") is None:
        mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    vcol = mesh.color_attributes["BakeColor"]
    for poly in mesh.polygons:
        for li in poly.loop_indices:
            vi = mesh.loops[li].vertex_index
            co = obj.matrix_world @ mesh.vertices[vi].co
            streak = math.sin(co.x * grain_scale + co.y * 3.7) * 0.55
            ring = math.sin((co.x + co.y) * 18.0) * 0.25
            bump = math.sin(co.z * 55.0) * 0.08
            t = 0.48 + streak * 0.34 + ring * 0.16 + bump
            rgb = _lerp3(light, dark, t)
            vcol.data[li].color = (rgb[0], rgb[1], rgb[2], 1.0)


def _blend_cc0_wood(
    obj: bpy.types.Object,
    color_jpg: Path,
    blend: float = 1.0,
    uv_tiles: float = 1.0,
    *,
    world_meters_per_repeat: float = 0.5,
) -> None:
    """Blend ambientCG wood into BakeColor only where WoodMask is set (desk carcass)."""
    if not color_jpg.is_file():
        return
    mesh = obj.data
    vcol = mesh.color_attributes.get("BakeColor")
    wmask = mesh.color_attributes.get("WoodMask")
    if vcol is None or wmask is None:
        return
    img = bpy.data.images.load(str(color_jpg), check_existing=True)
    img.colorspace_settings.name = "sRGB"
    w, h = img.size
    px = list(img.pixels)
    rep = max(0.25, 1.0 / world_meters_per_repeat)
    for poly in mesh.polygons:
        for li in poly.loop_indices:
            if wmask.data[li].color[0] < 0.5:
                continue
            vi = mesh.loops[li].vertex_index
            co = obj.matrix_world @ mesh.vertices[vi].co
            tu = (co.x * rep + co.y * 0.2) % 1.0
            tv = (co.z * rep) % 1.0
            tc = _sample_image_bilinear(px, w, h, tu, tv)
            bc = vcol.data[li].color
            c = _lerp3((bc[0], bc[1], bc[2]), tc, blend)
            vcol.data[li].color = (c[0], c[1], c[2], 1.0)


def _paint_plastic_keys(
    obj: bpy.types.Object,
    base: tuple[float, float, float] = (0.16, 0.18, 0.22),
    key_hi: tuple[float, float, float] = (0.72, 0.74, 0.78),
) -> None:
    """Keyboard: lighter caps on upper facets so it reads seated on the desk."""
    mesh = obj.data
    if mesh.color_attributes.get("BakeColor") is None:
        mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    vcol = mesh.color_attributes["BakeColor"]
    mn, mx = _bbox_world([obj])
    top_z = mx.z - (mx.z - mn.z) * 0.08
    for poly in mesh.polygons:
        normal = poly.normal
        up = max(0.0, normal.z)
        for li in poly.loop_indices:
            vi = mesh.loops[li].vertex_index
            co = obj.matrix_world @ mesh.vertices[vi].co
            on_top = 1.0 if co.z >= top_z else 0.0
            mix = min(1.0, up * 0.65 + on_top * 0.85 + math.sin(co.x * 90.0) * 0.08)
            rgb = _lerp3(base, key_hi, mix)
            vcol.data[li].color = (rgb[0], rgb[1], rgb[2], 1.0)


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


def _apply_baked_albedo(
    mat: bpy.types.Material,
    tex_path: Path,
    *,
    emissive_strength: float = 0.0,
) -> None:
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    tex = nodes.new("ShaderNodeTexImage")
    rel = str(tex_path)
    if rel not in bpy.data.images:
        img = bpy.data.images.load(rel, check_existing=True)
    else:
        img = bpy.data.images[rel]
    tex.image = img
    img.colorspace_settings.name = "sRGB"
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = 0.40 if emissive_strength <= 0.0 else 0.42
    if emissive_strength > 0.0:
        em_in = bsdf.inputs.get("Emission Color") or bsdf.inputs.get("Emission")
        if em_in is not None:
            links.new(tex.outputs["Color"], em_in)
        strength = bsdf.inputs.get("Emission Strength")
        if strength is not None:
            strength.default_value = emissive_strength
        links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    else:
        links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])


def _assign_prop_desk_material_slots(obj: bpy.types.Object) -> tuple[bpy.types.Material, bpy.types.Material]:
    """Slot 0 = wood/peripherals; slot 1 = monitor screen (EmitMask)."""
    mesh = obj.data
    mask = mesh.color_attributes.get("EmitMask")
    if mask is None:
        raise RuntimeError("prop-desk missing EmitMask color attribute")
    body = bpy.data.materials.new("M_prop-desk_body")
    body.use_nodes = True
    screen = bpy.data.materials.new("M_prop-desk_screen")
    screen.use_nodes = True
    mesh.materials.clear()
    mesh.materials.append(body)
    mesh.materials.append(screen)
    for poly in mesh.polygons:
        emit_votes = 0.0
        nloops = len(poly.loop_indices) or 1
        for li in poly.loop_indices:
            emit_votes += float(mask.data[li].color[0])
        emit_votes /= nloops
        poly.material_index = 1 if emit_votes > 0.55 else 0
    return body, screen


def _apply_baked_pbr_body(
    mat: bpy.types.Material,
    tex_path: Path,
    normal_path: Path | None,
    rough_path: Path | None,
    *,
    roughness: float = 0.40,
) -> None:
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    tex = nodes.new("ShaderNodeTexImage")
    img = bpy.data.images.load(str(tex_path), check_existing=True)
    img.colorspace_settings.name = "sRGB"
    tex.image = img
    tex.interpolation = "Linear"
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = roughness
    if normal_path and normal_path.is_file():
        ntex = nodes.new("ShaderNodeTexImage")
        nimg = bpy.data.images.load(str(normal_path), check_existing=True)
        nimg.colorspace_settings.name = "Non-Color"
        ntex.image = nimg
        ntex.interpolation = "Linear"
        nm = nodes.new("ShaderNodeNormalMap")
        links.new(ntex.outputs["Color"], nm.inputs["Color"])
        links.new(nm.outputs["Normal"], bsdf.inputs["Normal"])
    if rough_path and rough_path.is_file():
        rtex = nodes.new("ShaderNodeTexImage")
        rimg = bpy.data.images.load(str(rough_path), check_existing=True)
        rimg.colorspace_settings.name = "Non-Color"
        rtex.image = rimg
        rtex.interpolation = "Linear"
        links.new(rtex.outputs["Color"], bsdf.inputs["Roughness"])
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])


def _smart_uv(obj: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.uv.smart_project(angle_limit=math.radians(66), island_margin=0.02)
    bpy.ops.object.mode_set(mode="OBJECT")


def _pack_uv_islands(obj: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.uv.pack_islands(margin=0.015)
    bpy.ops.object.mode_set(mode="OBJECT")


def _uv_cube_tiles(obj: bpy.types.Object, repeats: float) -> None:
    """Box UV with ~1 repeat per 0.5 m (repeats = face span × 2 / meter)."""
    bpy.ops.object.select_all(action="DESELECT")
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.uv.cube_project(cube_size=1.0)
    bpy.ops.object.mode_set(mode="OBJECT")
    mesh = obj.data
    uv = mesh.uv_layers.active
    if uv is None:
        return
    for d in uv.data:
        d.uv[0] *= repeats
        d.uv[1] *= repeats


def _barycentric(
    px: float,
    py: float,
    ax: float,
    ay: float,
    bx: float,
    by: float,
    cx: float,
    cy: float,
) -> tuple[float, float, float] | None:
    v0x, v0y = bx - ax, by - ay
    v1x, v1y = cx - ax, cy - ay
    v2x, v2y = px - ax, py - ay
    det = v0x * v1y - v0y * v1x
    if abs(det) < 1e-12:
        return None
    u = (v2x * v1y - v1x * v2y) / det
    v = (v0x * v2y - v2x * v0y) / det
    if u < -0.001 or v < -0.001 or u + v > 1.001:
        return None
    w = 1.0 - u - v
    return w, u, v


def _rasterize_vertex_colors(
    obj: bpy.types.Object,
    tex_path: Path,
    *,
    default_fill: tuple[float, float, float] | None = None,
) -> None:
    """Fill 1024 albedo by triangle raster in UV space (point sampling leaves ~99% fill grey)."""
    mesh = obj.data
    attr = mesh.color_attributes.get("BakeColor")
    uv = mesh.uv_layers.active
    if attr is None or uv is None:
        raise RuntimeError("missing BakeColor UV")
    tex_name = f"T_raster_{obj.name}"
    if tex_name in bpy.data.images:
        bpy.data.images.remove(bpy.data.images[tex_name])
    img = bpy.data.images.new(tex_name, TEX_SIZE, TEX_SIZE, alpha=False)
    neut = default_fill or (0.78, 0.56, 0.38)
    fill = (neut[0], neut[1], neut[2], 1.0)
    px = list(fill) * (TEX_SIZE * TEX_SIZE)

    def put(x: int, y: int, rgb: tuple[float, float, float]) -> None:
        if 0 <= x < TEX_SIZE and 0 <= y < TEX_SIZE:
            o = (y * TEX_SIZE + x) * 4
            px[o : o + 4] = [rgb[0], rgb[1], rgb[2], 1.0]

    for poly in mesh.polygons:
        loops = list(poly.loop_indices)
        if len(loops) < 3:
            continue
        for t in range(1, len(loops) - 1):
            li0, li1, li2 = loops[0], loops[t], loops[t + 1]
            u0, v0 = uv.data[li0].uv
            u1, v1 = uv.data[li1].uv
            u2, v2 = uv.data[li2].uv
            c0 = attr.data[li0].color[:3]
            c1 = attr.data[li1].color[:3]
            c2 = attr.data[li2].color[:3]
            x0, y0 = u0 * TEX_SIZE, (1.0 - v0) * TEX_SIZE
            x1, y1 = u1 * TEX_SIZE, (1.0 - v1) * TEX_SIZE
            x2, y2 = u2 * TEX_SIZE, (1.0 - v2) * TEX_SIZE
            xmin = max(0, int(min(x0, x1, x2)))
            xmax = min(TEX_SIZE - 1, int(max(x0, x1, x2)) + 1)
            ymin = max(0, int(min(y0, y1, y2)))
            ymax = min(TEX_SIZE - 1, int(max(y0, y1, y2)) + 1)
            for y in range(ymin, ymax + 1):
                for x in range(xmin, xmax + 1):
                    ppx, ppy = x + 0.5, y + 0.5
                    bc = _barycentric(ppx, ppy, x0, y0, x1, y1, x2, y2)
                    if bc is None:
                        continue
                    w0, w1, w2 = bc
                    rgb = (
                        c0[0] * w0 + c1[0] * w1 + c2[0] * w2,
                        c0[1] * w0 + c1[1] * w1 + c2[1] * w2,
                        c0[2] * w0 + c1[2] * w1 + c2[2] * w2,
                    )
                    put(x, y, rgb)
    img.pixels = px
    img.filepath_raw = str(tex_path)
    img.file_format = "PNG"
    img.save()


def _bake_maps(obj: bpy.types.Object, mat: bpy.types.Material, out_dir: Path) -> Path:
    tex_dir = out_dir / "textures"
    tex_dir.mkdir(parents=True, exist_ok=True)
    tex_path = tex_dir / f"{obj.name}_basecolor.png"
    desk_fill = (0.12, 0.13, 0.15) if "prop-desk" in obj.name else None
    _rasterize_vertex_colors(obj, tex_path, default_fill=desk_fill)

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
    gltf_kw = dict(
        filepath=str(glb),
        export_format="GLB",
        use_selection=True,
        export_texcoords=True,
        export_materials="EXPORT",
        export_image_format="AUTO",
        export_keep_originals=False,
    )
    if obj.name == "mesh_prop-desk":
        gltf_kw["export_colors"] = False
    bpy.ops.export_scene.gltf(**gltf_kw)
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


def _setup_preview_hdri(
    world: bpy.types.World,
    hdr_path: Path,
    backdrop: float = 150.0 / 255.0,
    *,
    lit_strength: float = 0.10,
) -> None:
    """Backdrop exactly mid-grey for camera; HDRI lights the mesh only."""
    world.use_nodes = True
    nodes = world.node_tree.nodes
    links = world.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputWorld")
    bg_grey = nodes.new("ShaderNodeBackground")
    g = backdrop
    bg_grey.inputs["Color"].default_value = (g, g, g, 1.0)
    bg_grey.inputs["Strength"].default_value = 1.0
    if hdr_path.is_file():
        env = nodes.new("ShaderNodeTexEnvironment")
        if str(hdr_path) not in bpy.data.images:
            env.image = bpy.data.images.load(str(hdr_path), check_existing=True)
        else:
            env.image = bpy.data.images[str(hdr_path)]
        env.image.colorspace_settings.name = "Linear Rec.709"
        bg_lit = nodes.new("ShaderNodeBackground")
        bg_lit.inputs["Strength"].default_value = lit_strength
        links.new(env.outputs["Color"], bg_lit.inputs["Color"])
        path = nodes.new("ShaderNodeLightPath")
        mix = nodes.new("ShaderNodeMixShader")
        links.new(path.outputs["Is Camera Ray"], mix.inputs["Fac"])
        links.new(bg_lit.outputs["Background"], mix.inputs[1])
        links.new(bg_grey.outputs["Background"], mix.inputs[2])
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
    grey150 = 149.35 / 255.0
    _setup_preview_hdri(
        world,
        PREVIEW_HDRI,
        backdrop=grey150,
        lit_strength=0.11 if asset_id == "prop-desk" else 0.10,
    )
    if asset_id == "prop-desk":
        bpy.context.scene.view_settings.view_transform = "Raw"
        bpy.context.scene.view_settings.exposure = 0.0
        bpy.context.scene.view_settings.gamma = 1.0
    bpy.ops.import_scene.gltf(filepath=str(glb_path))
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not mesh_obs:
        raise RuntimeError(f"no mesh in {glb_path}")

    mins, maxs = _world_bbox(mesh_obs)
    center = (mins + maxs) / 2
    span = max(maxs.x - mins.x, maxs.y - mins.y, maxs.z - mins.z, 0.01)

    ground_size = span * (1.05 if asset_id == "prop-desk" else 4.0)
    bpy.ops.mesh.primitive_plane_add(size=ground_size, location=(center.x, center.y, mins.z - 0.002))
    ground = bpy.context.active_object
    gmat = bpy.data.materials.new("Ground")
    gmat.use_nodes = True
    floor_g = grey150
    gmat.node_tree.nodes["Principled BSDF"].inputs["Base Color"].default_value = (floor_g, floor_g, floor_g, 1)
    gmat.node_tree.nodes["Principled BSDF"].inputs["Roughness"].default_value = 0.92
    ground.data.materials.append(gmat)

    bpy.ops.object.light_add(type="SUN", location=center + Vector((span * 2, -span * 2, span * 3)))
    key = bpy.context.active_object
    key.data.energy = 2.86 if asset_id == "prop-desk" else 1.8
    key.rotation_euler = (math.radians(52), 0, math.radians(35))

    bpy.ops.object.light_add(type="AREA", location=center + Vector((-span, span * 1.2, span * 1.5)))
    fill = bpy.context.active_object
    fill.data.energy = 78 if asset_id == "prop-desk" else 55
    fill.data.size = span * (0.9 if asset_id == "prop-desk" else 1.4)
    obj_h = max(maxs.z - mins.z, maxs.y - mins.y, 0.01)
    obj_w = max(maxs.x - mins.x, maxs.y - mins.y, 0.01)
    target_fill = 0.70 if asset_id == "prop-desk" else 0.50
    pad = 1.04 if asset_id == "prop-desk" else 1.0
    ortho = max(obj_h / target_fill, (obj_w * 1.12) / target_fill) * pad

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
        if asset_id == "prop-desk":
            elev = math.radians(28)
            az = math.radians(-38)
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
        if asset_id == "prop-desk" and hasattr(ee, "use_bloom"):
            ee.use_bloom = False
    bpy.context.scene.render.film_transparent = False
    bpy.context.scene.render.resolution_x = 1024
    bpy.context.scene.render.resolution_y = 1024
    bpy.context.scene.render.filepath = str(preview)
    bpy.context.scene.render.image_settings.file_format = "PNG"
    bpy.context.scene.render.image_settings.color_mode = "RGB"
    if asset_id != "prop-desk" and hasattr(bpy.context.scene, "view_settings"):
        bpy.context.scene.view_settings.exposure = 0.22
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


def _ide_dashboard_image(out_path: Path) -> bpy.types.Image:
    """Dark IDE-style screen albedo for Kenney monitor glass slot."""
    if out_path.is_file() and str(out_path) in bpy.data.images:
        return bpy.data.images[str(out_path)]
    size = 512
    img = bpy.data.images.new("T_prop_desk_ide", size, size, alpha=False)
    px: list[float] = []
    for y in range(size):
        for x in range(size):
            bar = 0.52 if x < size * 0.24 and y > size * 0.12 else 0.0
            line = 0.45 if (y % 32) < 2 and x > size * 0.26 else 0.0
            pane = 0.35 if abs(math.sin(x * 0.08) * math.sin(y * 0.06)) > 0.72 and x > size * 0.3 else 0.0
            acc = min(1.0, bar + line + pane)
            bg = (0.05, 0.06, 0.09)
            hi = (0.16, 0.48, 0.62)
            accent = (0.22, 0.72, 0.48)
            c = _lerp3(bg, hi, acc * 0.75)
            c = _lerp3(c, accent, acc * 0.35)
            px.extend([*c, 1.0])
    img.pixels = px
    img.filepath_raw = str(out_path)
    img.file_format = "PNG"
    img.save()
    return img


_MAT_CACHE: dict[str, bpy.types.Material] = {}


def _prop_desk_shared_material(kind: str, tex_dir: Path) -> bpy.types.Material:
    if kind in _MAT_CACHE:
        return _MAT_CACHE[kind]
    mat = bpy.data.materials.new(f"M_prop_desk_{kind}")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    if kind == "wood":
        bsdf.inputs["Roughness"].default_value = 0.42
        bsdf.inputs["Metallic"].default_value = 0.0
        coord = nodes.new("ShaderNodeTexCoord")
        mapping = nodes.new("ShaderNodeMapping")
        mapping.inputs["Scale"].default_value = (2.0, 2.0, 2.0)
        links.new(coord.outputs["Object"], mapping.inputs["Vector"])
        if WOOD092_COLOR.is_file():
            ctex = nodes.new("ShaderNodeTexImage")
            cimg = bpy.data.images.load(str(WOOD092_COLOR), check_existing=True)
            cimg.colorspace_settings.name = "sRGB"
            ctex.image = cimg
            ctex.interpolation = "Linear"
            links.new(mapping.outputs["Vector"], ctex.inputs["Vector"])
            links.new(ctex.outputs["Color"], bsdf.inputs["Base Color"])
        if WOOD092_NORMAL.is_file():
            nt = nodes.new("ShaderNodeTexImage")
            nimg = bpy.data.images.load(str(WOOD092_NORMAL), check_existing=True)
            nimg.colorspace_settings.name = "Non-Color"
            nt.image = nimg
            nt.interpolation = "Linear"
            links.new(mapping.outputs["Vector"], nt.inputs["Vector"])
            nm = nodes.new("ShaderNodeNormalMap")
            links.new(nt.outputs["Color"], nm.inputs["Color"])
            links.new(nm.outputs["Normal"], bsdf.inputs["Normal"])
        if WOOD092_ROUGH.is_file():
            rt = nodes.new("ShaderNodeTexImage")
            rimg = bpy.data.images.load(str(WOOD092_ROUGH), check_existing=True)
            rimg.colorspace_settings.name = "Non-Color"
            rt.image = rimg
            rt.interpolation = "Linear"
            links.new(mapping.outputs["Vector"], rt.inputs["Vector"])
            links.new(rt.outputs["Color"], bsdf.inputs["Roughness"])
    elif kind == "plastic":
        g = 0.12
        bsdf.inputs["Base Color"].default_value = (g, g, g * 1.02, 1.0)
        bsdf.inputs["Roughness"].default_value = 0.40
    elif kind == "key_cap":
        bsdf.inputs["Base Color"].default_value = (0.62, 0.64, 0.68, 1.0)
        bsdf.inputs["Roughness"].default_value = 0.35
    elif kind == "screen":
        tex_dir.mkdir(parents=True, exist_ok=True)
        ide = tex_dir / "mesh_prop-desk_screen_ide.png"
        img = _ide_dashboard_image(ide)
        tex = nodes.new("ShaderNodeTexImage")
        tex.image = img
        tex.interpolation = "Linear"
        links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
        em_in = bsdf.inputs.get("Emission Color") or bsdf.inputs.get("Emission")
        if em_in is not None:
            links.new(tex.outputs["Color"], em_in)
        strength = bsdf.inputs.get("Emission Strength")
        if strength is not None:
            strength.default_value = 1.15
        bsdf.inputs["Roughness"].default_value = 0.45
    elif kind == "pot":
        bsdf.inputs["Base Color"].default_value = (0.78, 0.45, 0.32, 1.0)
        bsdf.inputs["Roughness"].default_value = 0.55
    elif kind == "leaf":
        bsdf.inputs["Base Color"].default_value = (0.32, 0.86, 0.40, 1.0)
        bsdf.inputs["Roughness"].default_value = 0.48
    _MAT_CACHE[kind] = mat
    return mat


def _restyle_slot_by_name(obj: bpy.types.Object, tex_dir: Path) -> None:
    """Replace Kenney GLTF slots: wood → Wood092, metals → plastic/screen."""
    is_screen = "screen" in obj.name.lower()
    for i, slot in enumerate(obj.data.materials):
        if slot is None:
            continue
        name = (slot.name or "").lower()
        if name == "wood" or name.startswith("wood"):
            obj.data.materials[i] = _prop_desk_shared_material("wood", tex_dir)
        elif is_screen and name == "metal.001":
            obj.data.materials[i] = _prop_desk_shared_material("screen", tex_dir)
        elif name == "metalmedium":
            obj.data.materials[i] = _prop_desk_shared_material("key_cap", tex_dir)
        elif "metal" in name:
            obj.data.materials[i] = _prop_desk_shared_material("plastic", tex_dir)


def _assign_plant_materials(obj: bpy.types.Object, tex_dir: Path) -> None:
    mesh = obj.data
    pot = _prop_desk_shared_material("pot", tex_dir)
    leaf = _prop_desk_shared_material("leaf", tex_dir)
    mesh.materials.clear()
    mesh.materials.append(pot)
    mesh.materials.append(leaf)
    mn, mx = _bbox_world([obj])
    split_z = mn.z + (mx.z - mn.z) * 0.35
    for poly in mesh.polygons:
        zs = []
        for li in poly.loop_indices:
            vi = mesh.loops[li].vertex_index
            zs.append((obj.matrix_world @ mesh.vertices[vi].co).z)
        z = sum(zs) / len(zs) if zs else split_z
        poly.material_index = 0 if z < split_z else 1


def finish_prop_desk_slot_export(
    parts: list[bpy.types.Object],
    source_cc0: str,
    extra_manifest: dict | None = None,
) -> dict:
    """Join multi-mesh Kenney desk hero with per-slot PBR; preview from committed GLB."""
    asset_id = "prop-desk"
    out_dir = OUT_ROOT / asset_id
    tex_dir = out_dir / "textures"
    tex_dir.mkdir(parents=True, exist_ok=True)
    _MAT_CACHE.clear()
    bpy.ops.object.select_all(action="DESELECT")
    for p in parts:
        if "plant" in p.name.lower():
            _assign_plant_materials(p, tex_dir)
        else:
            _restyle_slot_by_name(p, tex_dir)
        p.select_set(True)
    bpy.context.view_layer.objects.active = parts[0]
    if len(parts) > 1:
        bpy.ops.object.join()
    obj = bpy.context.view_layer.objects.active
    obj.name = "mesh_prop-desk"
    obj.location = Vector((0.0, 0.0, -_bbox_world([obj])[0].z))
    if WOOD092_COLOR.is_file():
        shutil.copy(WOOD092_COLOR, tex_dir / "Wood092_Color.jpg")
    if WOOD092_NORMAL.is_file():
        shutil.copy(WOOD092_NORMAL, tex_dir / "Wood092_NormalGL.jpg")
    if WOOD092_ROUGH.is_file():
        shutil.copy(WOOD092_ROUGH, tex_dir / "Wood092_Roughness.jpg")
    glb = _export_gltf(obj, out_dir)
    fbx = _export_fbx(obj, out_dir)
    preview_rel = f"docs/art/previews/{asset_id}.png"
    _render_preview_from_glb(glb, asset_id)
    manifest = {
        "assetId": asset_id,
        "scope": SCOPE,
        "sourceCc0": source_cc0,
        "pipeline": "kenney-slot-v11",
        "texture1024": str((tex_dir / "mesh_prop-desk_screen_ide.png").relative_to(REPO)),
        "urpAlbedoPath": "Assets/Art/cc0/ambientcg/Wood092_1K-JPG/Wood092_1K-JPG_Color.jpg",
        "glb": str(glb.relative_to(REPO)),
        "fbx": str(fbx.relative_to(REPO)),
        "preview": preview_rel,
        "previewHdri": str(PREVIEW_HDRI.relative_to(REPO)),
        "textureNormal1024": str((tex_dir / "Wood092_NormalGL.jpg").relative_to(REPO)),
        "textureRoughness1024": str((tex_dir / "Wood092_Roughness.jpg").relative_to(REPO)),
    }
    if extra_manifest:
        manifest.update(extra_manifest)
    (out_dir / "manifest.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    return manifest


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
    if asset_id == "prop-desk" and WOOD092_COLOR.is_file():
        _blend_cc0_wood(obj, WOOD092_COLOR, world_meters_per_repeat=0.5)
    out_dir = OUT_ROOT / asset_id
    out_dir.mkdir(parents=True, exist_ok=True)
    tex_path = _bake_maps(obj, bake_mat, out_dir)
    tex_dir = tex_path.parent
    normal_rel: str | None = None
    rough_rel: str | None = None
    if asset_id == "prop-desk":
        if WOOD092_NORMAL.is_file():
            dest = tex_dir / f"{obj.name}_normal.png"
            shutil.copy(WOOD092_NORMAL, dest)
            normal_rel = str(dest.relative_to(REPO))
        if WOOD092_ROUGH.is_file():
            dest = tex_dir / f"{obj.name}_roughness.png"
            shutil.copy(WOOD092_ROUGH, dest)
            rough_rel = str(dest.relative_to(REPO))
    if asset_id == "prop-desk":
        body_mat, screen_mat = _assign_prop_desk_material_slots(obj)
        _apply_baked_albedo(body_mat, tex_path, emissive_strength=0.0)
        body_mat.node_tree.nodes["Principled BSDF"].inputs["Roughness"].default_value = 0.40
        _apply_baked_albedo(screen_mat, tex_path, emissive_strength=1.5)
    else:
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
    if normal_rel:
        manifest["textureNormal1024"] = normal_rel
    if rough_rel:
        manifest["textureRoughness1024"] = rough_rel
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