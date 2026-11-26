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
    mid = ramp.color_ramp.elements.new(0.55)
    mid.color = (
        *((base_rgb[i] * 0.35 + accent_rgb[i] * 0.65) / 255 for i in range(3)),
        1,
    )
    wave = nodes.new("ShaderNodeTexWave")
    wave.wave_type = "BANDS"
    wave.inputs["Scale"].default_value = 6.5
    mix = nodes.new("ShaderNodeMixRGB")
    mix.blend_type = "OVERLAY"
    mix.inputs["Fac"].default_value = 0.35
    links.new(wave.outputs["Color"], mix.inputs[1])
    links.new(ramp.outputs["Color"], mix.inputs[2])
    links.new(noise.outputs["Fac"], ramp.inputs["Fac"])
    hue = nodes.new("ShaderNodeHueSaturation")
    hue.inputs["Saturation"].default_value = 2.25
    hue.inputs["Value"].default_value = 1.08
    links.new(mix.outputs["Color"], hue.inputs["Color"])
    links.new(hue.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = 0.48
    bsdf.inputs["Emission Color"].default_value = (1.0, 0.72, 0.42, 1.0)
    bsdf.inputs["Emission Strength"].default_value = 0.12
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
    try:
        os.environ["EARLY_ART_PREVIEW_PROFILE"] = "hud"
        os.environ.setdefault("HUD_ORTHO_FILL", "0.68")
        bc.render_preview(plane, preview)
    except Exception as exc:  # noqa: BLE001
        print(f"WARN texture preview {asset_id}: {exc}")
    print("OK texture", asset_id, f"blender {bc.BLENDER_VERSION}")