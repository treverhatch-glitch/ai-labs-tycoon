"""Bake 1024 tileable albedo/normal/roughness for early-game texture swatches."""
from __future__ import annotations

import os
import shutil
from pathlib import Path

import bpy

import blender_common as bc

REPO = Path(__file__).resolve().parents[3]
TEX_OUT = REPO / "unity/Assets/Art/EarlyGame/textures"
STREAM = REPO / "unity/Assets/StreamingAssets/EarlyGame/textures"
SIZE = 1024


def bake_tileable(
    asset_id: str,
    base_rgb: tuple[int, int, int],
    accent_rgb: tuple[int, int, int],
    noise_scale: float = 9.0,
    fiber_spots: bool = False,
) -> None:
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
    base_color = ramp.outputs["Color"]
    if fiber_spots:
        vor = nodes.new("ShaderNodeTexVoronoi")
        vor.feature = "DISTANCE_TO_EDGE"
        vor.inputs["Scale"].default_value = 14.0
        mix = nodes.new("ShaderNodeMixRGB")
        mix.blend_type = "MULTIPLY"
        mix.inputs["Fac"].default_value = 0.62
        spot_ramp = nodes.new("ShaderNodeValToRGB")
        spot_ramp.color_ramp.elements[0].color = (0.82, 0.32, 0.12, 1)
        spot_ramp.color_ramp.elements[1].color = (0.12, 0.48, 0.88, 1)
        links.new(vor.outputs["Distance"], spot_ramp.inputs["Fac"])
        links.new(base_color, mix.inputs["Color1"])
        links.new(spot_ramp.outputs["Color"], mix.inputs["Color2"])
        base_color = mix.outputs["Color"]
        luma_noise = nodes.new("ShaderNodeTexNoise")
        luma_noise.inputs["Scale"].default_value = 24.0
        luma_ramp = nodes.new("ShaderNodeValToRGB")
        luma_ramp.color_ramp.elements[0].color = (0.28, 0.24, 0.18, 1)
        luma_ramp.color_ramp.elements[1].color = (1.05, 1.02, 0.92, 1)
        links.new(luma_noise.outputs["Fac"], luma_ramp.inputs["Fac"])
        luma_mix = nodes.new("ShaderNodeMixRGB")
        luma_mix.blend_type = "MULTIPLY"
        luma_mix.inputs["Fac"].default_value = 0.48
        links.new(base_color, luma_mix.inputs["Color1"])
        links.new(luma_ramp.outputs["Color"], luma_mix.inputs["Color2"])
        base_color = luma_mix.outputs["Color"]
        bands = nodes.new("ShaderNodeTexWave")
        bands.wave_type = "BANDS"
        bands.inputs["Scale"].default_value = 2.8
        bands.inputs["Distortion"].default_value = 3.5
        band_ramp = nodes.new("ShaderNodeValToRGB")
        band_ramp.color_ramp.elements[0].color = (0.12, 0.08, 0.05, 1)
        band_ramp.color_ramp.elements[1].color = (1.08, 0.98, 0.88, 1)
        links.new(bands.outputs["Color"], band_ramp.inputs["Fac"])
        band_mix = nodes.new("ShaderNodeMixRGB")
        band_mix.blend_type = "MULTIPLY"
        band_mix.inputs["Fac"].default_value = 0.72
        links.new(base_color, band_mix.inputs["Color1"])
        links.new(band_ramp.outputs["Color"], band_mix.inputs["Color2"])
        base_color = band_mix.outputs["Color"]
    links.new(base_color, bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = 0.48
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    plane.data.materials.append(mat)

    preview = REPO / "docs/art/previews" / f"{asset_id}.png"
    preview.parent.mkdir(parents=True, exist_ok=True)
    try:
        os.environ["EARLY_ART_PREVIEW_KIND"] = "hud"
        bc.render_preview(plane, preview, asset_id=asset_id)
    except Exception as exc:  # noqa: BLE001
        print(f"WARN preview {asset_id}: {exc}")
    bpy.context.scene.render.engine = "CYCLES"

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

    print("OK texture", asset_id, f"blender {bc.BLENDER_VERSION}")