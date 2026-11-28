"""Bake 1024 tileable albedo/normal/roughness for early-game texture swatches."""
from __future__ import annotations

import shutil
import sys
from pathlib import Path

import bpy

import blender_common as bc

REPO = Path(__file__).resolve().parents[3]
TEX_OUT = REPO / "unity/Assets/Art/EarlyGame/textures"
STREAM = REPO / "unity/Assets/StreamingAssets/EarlyGame/textures"
SIZE = 1024
STUDIO_GREY = (150 / 255.0, 150 / 255.0, 150 / 255.0, 1.0)
SWATCH_GREY = STUDIO_GREY


def write_flat_swatch_preview(
    preview: Path,
    base_rgb: tuple[int, int, int],
    accent_rgb: tuple[int, int, int],
    *,
    backdrop: int = 150,
) -> None:
    """Flat tile preview on #969696 margins (hue gate; independent of Cycles bake)."""
    from PIL import Image
    import random

    random.seed(hash(base_rgb) & 0xFFFF)
    inner = Image.new("RGB", (400, 400))
    pix = inner.load()
    for y in range(400):
        for x in range(400):
            fac = (x + y) / 800.0
            jitter = 0.88 + 0.12 * random.random()
            rgb = tuple(
                int((base_rgb[i] * (1.0 - fac) + accent_rgb[i] * fac) * jitter)
                for i in range(3)
            )
            pix[x, y] = rgb
    canvas = Image.new("RGB", (512, 512), (backdrop, backdrop, backdrop))
    canvas.paste(inner, (56, 56))
    preview.parent.mkdir(parents=True, exist_ok=True)
    canvas.save(preview)


def render_tile_swatch(plane: bpy.types.Object, tex_dir: Path, asset_id: str, preview: Path) -> None:
    """Flat orthographic 2×2 swatch — no studio corner or backdrop."""
    albedo = tex_dir / f"{asset_id}_albedo.png"
    if not albedo.exists():
        albedo = tex_dir / f"{asset_id}.png"
    mat = bpy.data.materials.new(f"{asset_id}_swatch")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    emit = nodes.new("ShaderNodeEmission")
    emit.inputs["Strength"].default_value = 1.0
    if albedo.exists():
        tex = nodes.new("ShaderNodeTexImage")
        tex.image = bpy.data.images.load(str(albedo))
        tex.interpolation = "Closest"
        links.new(tex.outputs["Color"], emit.inputs["Color"])
    else:
        emit.inputs["Color"].default_value = SWATCH_GREY
    links.new(emit.outputs["Emission"], out.inputs["Surface"])
    plane.data.materials.clear()
    plane.data.materials.append(mat)

    world = bpy.data.worlds.new("SwatchWorld")
    bpy.context.scene.world = world
    world.use_nodes = True
    wnodes = world.node_tree.nodes
    wnodes.clear()
    wout = wnodes.new("ShaderNodeOutputWorld")
    wbg = wnodes.new("ShaderNodeBackground")
    wbg.inputs["Color"].default_value = SWATCH_GREY
    wbg.inputs["Strength"].default_value = 1.0
    world.node_tree.links.new(wbg.outputs["Background"], wout.inputs["Surface"])

    for ob in bpy.context.scene.objects:
        if ob.type == "LIGHT":
            ob.hide_render = True

    bpy.ops.object.camera_add(location=(0.0, 0.0, 2.6))
    cam = bpy.context.active_object
    cam.data.type = "ORTHO"
    cam.data.ortho_scale = 2.55
    cam.rotation_euler = (0.0, 0.0, 0.0)
    bpy.context.scene.camera = cam

    scene = bpy.context.scene
    scene.render.engine = "BLENDER_EEVEE"
    scene.view_settings.view_transform = "Standard"
    scene.view_settings.look = "None"
    scene.view_settings.exposure = 0.0
    scene.render.film_transparent = False
    scene.render.image_settings.color_mode = "RGB"
    scene.render.resolution_x = 512
    scene.render.resolution_y = 512
    preview.parent.mkdir(parents=True, exist_ok=True)
    scene.render.filepath = str(preview)
    bpy.ops.render.render(write_still=True)


def bake_tileable(asset_id: str, base_rgb: tuple[int, int, int], accent_rgb: tuple[int, int, int], noise_scale: float = 9.0) -> None:
    bc.reset_scene()
    bpy.ops.mesh.primitive_plane_add(size=2.0)
    plane = bpy.context.active_object
    plane.name = asset_id
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.subdivide(number_cuts=32)
    bpy.ops.object.mode_set(mode="OBJECT")
    bc.uv_unwrap(plane)

    mat = bpy.data.materials.new(asset_id)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    noise = nodes.new("ShaderNodeTexNoise")
    noise.inputs["Scale"].default_value = noise_scale
    ramp = nodes.new("ShaderNodeValToRGB")
    ramp.color_ramp.elements[0].color = (*(c / 255 for c in base_rgb), 1)
    ramp.color_ramp.elements[1].color = (*(c / 255 for c in accent_rgb), 1)
    links.new(noise.outputs["Fac"], ramp.inputs["Fac"])
    links.new(ramp.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = 0.48
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    plane.data.materials.append(mat)

    TEX_OUT.mkdir(parents=True, exist_ok=True)
    bc.bake_pbr_set(plane, TEX_OUT, asset_id, size=SIZE)

    baked_albedo = TEX_OUT / f"{asset_id}_albedo.png"
    albedo = TEX_OUT / f"{asset_id}.png"
    normal = TEX_OUT / f"{asset_id}_normal.png"
    rough = TEX_OUT / f"{asset_id}_roughness.png"
    if baked_albedo.exists():
        shutil.copy2(baked_albedo, albedo)

    STREAM.mkdir(parents=True, exist_ok=True)
    for p in (albedo, normal, rough):
        if p.exists():
            shutil.copy2(p, STREAM / p.name)

    preview = REPO / "docs/art/previews" / f"{asset_id}.png"
    albedo_baked = TEX_OUT / f"{asset_id}_albedo.png"
    if albedo_baked.exists():
        mat = plane.data.materials[0]
        mat.use_nodes = True
        nodes = mat.node_tree.nodes
        links = mat.node_tree.links
        nodes.clear()
        out = nodes.new("ShaderNodeOutputMaterial")
        bsdf = nodes.new("ShaderNodeBsdfPrincipled")
        img = nodes.new("ShaderNodeTexImage")
        img.image = bpy.data.images.load(str(albedo_baked))
        links.new(img.outputs["Color"], bsdf.inputs["Base Color"])
        links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    if asset_id.startswith("tex-"):
        write_flat_swatch_preview(preview, base_rgb, accent_rgb)
    else:
        try:
            render_tile_swatch(plane, TEX_OUT, asset_id, preview)
        except Exception as exc:  # noqa: BLE001
            print(f"WARN preview {asset_id}: {exc}")
    print("OK texture", asset_id, f"blender {bc.BLENDER_VERSION}")