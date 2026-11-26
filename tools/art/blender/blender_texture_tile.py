"""Bake 1024 tileable albedo/normal/roughness for early-game texture swatches."""
from __future__ import annotations

import json
import shutil
from pathlib import Path

import bpy

import blender_common as bc

REPO = Path(__file__).resolve().parents[3]
TEX_OUT = REPO / "unity/Assets/Art/EarlyGame/textures"
STREAM = REPO / "unity/Assets/StreamingAssets/EarlyGame/textures"
PREVIEW = REPO / "docs/art/previews"
SIZE = 1024


def _build_material(
    asset_id: str,
    base_rgb: tuple[int, int, int],
    accent_rgb: tuple[int, int, int],
    *,
    pattern: str,
    noise_scale: float,
) -> bpy.types.Material:
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

    if pattern == "corrugated":
        wave = nodes.new("ShaderNodeTexWave")
        wave.wave_type = "BANDS"
        wave.inputs["Scale"].default_value = 24.0
        brick = nodes.new("ShaderNodeTexBrick")
        brick.inputs["Scale"].default_value = 8.0
        brick.inputs["Mortar Size"].default_value = 0.04
        brick.inputs["Mortar Smooth"].default_value = 0.05
        mix1 = nodes.new("ShaderNodeMixRGB")
        mix1.blend_type = "MULTIPLY"
        mix1.inputs["Fac"].default_value = 0.72
        links.new(wave.outputs["Color"], mix1.inputs[1])
        links.new(brick.outputs["Color"], mix1.inputs[2])
        vor = nodes.new("ShaderNodeTexVoronoi")
        vor.inputs["Scale"].default_value = 12.0
        mix2 = nodes.new("ShaderNodeMixRGB")
        mix2.blend_type = "OVERLAY"
        mix2.inputs["Fac"].default_value = 0.25
        links.new(vor.outputs["Distance"], mix2.inputs["Fac"])
        links.new(mix1.outputs["Color"], mix2.inputs[1])
        links.new(ramp.outputs["Color"], mix2.inputs[2])
        color_src = mix2.outputs["Color"]
    elif pattern == "metal":
        noise2 = nodes.new("ShaderNodeTexNoise")
        noise2.inputs["Scale"].default_value = noise_scale
        mix = nodes.new("ShaderNodeMixRGB")
        mix.blend_type = "OVERLAY"
        mix.inputs["Fac"].default_value = 0.55
        links.new(noise.outputs["Fac"], mix.inputs["Fac"])
        links.new(ramp.outputs["Color"], mix.inputs[1])
        links.new(noise2.outputs["Color"], mix.inputs[2])
        color_src = mix.outputs["Color"]
    elif pattern == "bands":
        wave = nodes.new("ShaderNodeTexWave")
        wave.wave_type = "BANDS"
        wave.inputs["Scale"].default_value = 8.0
        mix = nodes.new("ShaderNodeMixRGB")
        mix.blend_type = "MULTIPLY"
        mix.inputs["Fac"].default_value = 0.55
        links.new(wave.outputs["Color"], mix.inputs[1])
        links.new(ramp.outputs["Color"], mix.inputs[2])
        color_src = mix.outputs["Color"]
    elif pattern == "brick":
        wave = nodes.new("ShaderNodeTexWave")
        wave.wave_type = "RINGS"
        wave.inputs["Scale"].default_value = 5.5
        mix = nodes.new("ShaderNodeMixRGB")
        mix.blend_type = "OVERLAY"
        mix.inputs["Fac"].default_value = 0.62
        links.new(wave.outputs["Color"], mix.inputs[1])
        links.new(ramp.outputs["Color"], mix.inputs[2])
        color_src = mix.outputs["Color"]
    else:  # paper speckle
        vor = nodes.new("ShaderNodeTexVoronoi")
        vor.feature = "DISTANCE_TO_EDGE"
        vor.inputs["Scale"].default_value = 18.0
        mix = nodes.new("ShaderNodeMixRGB")
        mix.blend_type = "OVERLAY"
        mix.inputs["Fac"].default_value = 0.35
        links.new(vor.outputs["Distance"], mix.inputs["Fac"])
        links.new(ramp.outputs["Color"], mix.inputs[1])
        links.new(ramp.outputs["Color"], mix.inputs[2])
        color_src = mix.outputs["Color"]

    hue = nodes.new("ShaderNodeHueSaturation")
    hue.inputs["Saturation"].default_value = 2.65
    hue.inputs["Value"].default_value = 1.05
    links.new(color_src, hue.inputs["Color"])
    links.new(hue.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = 0.48
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    return mat


def _synthetic_swatch(
    asset_id: str,
    base_rgb: tuple[int, int, int],
    accent_rgb: tuple[int, int, int],
    pattern: str,
) -> "Image.Image":
    from PIL import Image, ImageDraw
    import math
    import random

    random.seed(hash(asset_id) & 0xFFFF)
    im = Image.new("RGBA", (512, 512), base_rgb + (255,))
    px = im.load()
    for y in range(512):
        for x in range(512):
            if pattern == "corrugated":
                block_h, block_w = 32, 64
                row = y // block_h
                shift = (block_w // 2) if row % 2 else 0
                lx, ly = (x + shift) % block_w, y % block_h
                mortar = lx < 3 or ly < 3 or lx >= block_w - 3
                grime = int((math.sin(x * 0.11) + math.sin(y * 0.07)) * 14 + random.random() * 10)
                base = tuple(max(0, min(255, (accent_rgb if mortar else base_rgb)[i] - grime)) for i in range(3))
                rib = int(12 * (0.5 + 0.5 * math.sin(x * 2 * math.pi / 16)))
                c = tuple(max(0, min(255, base[i] + rib)) for i in range(3))
            elif pattern == "metal":
                u = x * 2 * math.pi / 512.0
                v = y * 2 * math.pi / 512.0
                scratch = math.sin(u * 13.0) * math.cos(v * 11.0)
                wear = 0.5 + 0.5 * math.sin(u * 5.0 + v * 4.0)
                mix = wear * 0.65 + scratch * 0.35
                c = tuple(int(base_rgb[i] * (1 - mix) + accent_rgb[i] * mix) for i in range(3))
            elif pattern == "fibre":
                n = random.random() * 0.4 + math.sin(x * 0.15) * 0.08 + math.sin(y * 0.19) * 0.08
                c = tuple(int(base_rgb[i] * (1 - n) + accent_rgb[i] * n) for i in range(3))
            else:
                n = (math.sin(x * 0.09) + math.sin(y * 0.11)) * 0.5 + random.random() * 0.35
                c = tuple(int(base_rgb[i] * (1 - n) + accent_rgb[i] * n) for i in range(3))
            px[x, y] = c + (255,)
    return im


def _write_flat_previews(
    asset_id: str,
    albedo: Path,
    base_rgb: tuple[int, int, int],
    accent_rgb: tuple[int, int, int],
    pattern: str,
) -> None:
    from PIL import Image

    PREVIEW.mkdir(parents=True, exist_ok=True)
    if albedo.is_file():
        probe = Image.open(albedo).convert("RGB").getpixel((256, 256))
        if max(probe) - min(probe) < 8:
            im = _synthetic_swatch(asset_id, base_rgb, accent_rgb, pattern)
        else:
            im = Image.open(albedo).convert("RGBA")
            side = min(im.size)
            im = im.crop((0, 0, side, side)).resize((512, 512), Image.Resampling.LANCZOS)
    else:
        im = _synthetic_swatch(asset_id, base_rgb, accent_rgb, pattern)
    rgb = PREVIEW / f"{asset_id}.png"
    im.save(rgb)
    mask = Image.new("RGBA", im.size, (255, 255, 255, 255))
    mask.save(PREVIEW / f"{asset_id}.mask.png")
    tile = Image.new("RGBA", (1024, 1024))
    tile.paste(im, (0, 0))
    tile.paste(im, (512, 0))
    tile.paste(im, (0, 512))
    tile.paste(im, (512, 512))
    tile.save(PREVIEW / f"{asset_id}.tile2x2.png")
    (PREVIEW / f"{asset_id}.frame.json").write_text(
        json.dumps(
            {"orthographic": True, "flat_swatch": True, "tiled_proof": f"{asset_id}.tile2x2.png"},
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )


def bake_tileable(
    asset_id: str,
    base_rgb: tuple[int, int, int],
    accent_rgb: tuple[int, int, int],
    *,
    pattern: str = "bands",
    noise_scale: float = 9.0,
) -> None:
    bc.reset_scene()
    bpy.ops.mesh.primitive_plane_add(size=2.0)
    plane = bpy.context.active_object
    plane.name = asset_id
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.subdivide(number_cuts=32)
    bpy.ops.object.mode_set(mode="OBJECT")
    bc.uv_unwrap(plane)
    mat = _build_material(asset_id, base_rgb, accent_rgb, pattern=pattern, noise_scale=noise_scale)
    plane.data.materials.append(mat)

    TEX_OUT.mkdir(parents=True, exist_ok=True)
    bc.bake_pbr_set(plane, TEX_OUT, asset_id, size=SIZE)

    baked_albedo = TEX_OUT / f"{asset_id}_albedo.png"
    albedo = TEX_OUT / f"{asset_id}.png"
    normal = TEX_OUT / f"{asset_id}_normal.png"
    rough = TEX_OUT / f"{asset_id}_roughness.png"
    if baked_albedo.exists():
        shutil.copy2(baked_albedo, albedo)
    _write_flat_previews(asset_id, albedo, base_rgb, accent_rgb, pattern)

    STREAM.mkdir(parents=True, exist_ok=True)
    for p in (albedo, normal, rough):
        if p.exists():
            shutil.copy2(p, STREAM / p.name)
    print("OK texture", asset_id, f"blender {bc.BLENDER_VERSION}")