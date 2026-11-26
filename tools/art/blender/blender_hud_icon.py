"""Shared HUD icon renderer for headless Blender (one bpy script per icon)."""
from __future__ import annotations

import shutil
from pathlib import Path

import bpy

import blender_common as bc

REPO = Path(__file__).resolve().parents[3]
HUD_OUT = REPO / "unity/Assets/Art/EarlyGame/hud-icons"
STREAM = REPO / "unity/Assets/StreamingAssets/EarlyGame/hud-icons"


def _hex(rgb: tuple[int, int, int]) -> tuple[float, float, float, float]:
    import colorsys

    r, g, b = (rgb[0] / 255, rgb[1] / 255, rgb[2] / 255)
    h, light, sat = colorsys.rgb_to_hls(r, g, b)
    sat = min(1.0, max(sat, 0.28) * 1.55)
    light = min(0.82, max(light, 0.18))
    r, g, b = colorsys.hls_to_rgb(h, light, sat)
    return (r, g, b, 1.0)


def solid_mat(name: str, rgba: tuple[float, float, float, float], emission: float = 0.0) -> bpy.types.Material:
    mat = bpy.data.materials.new(name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes["Principled BSDF"]
    bsdf.inputs["Base Color"].default_value = rgba
    bsdf.inputs["Roughness"].default_value = 0.42
    emission = min(max(emission, 0.11), 0.22)
    if emission > 0:
        bsdf.inputs["Emission Color"].default_value = rgba
        bsdf.inputs["Emission Strength"].default_value = emission
    return mat


def add_glyph_bar(
    name: str,
    size: tuple[float, float, float],
    location: tuple[float, float, float],
    rgba: tuple[float, float, float, float],
    emission: float = 0.0,
) -> bpy.types.Object:
    ob = bc.add_cube(name, size, location)
    ob.data.materials.append(solid_mat(name, rgba, emission=emission))
    return ob


def _boost_material_saturation(root: bpy.types.Object, gain: float = 2.15) -> None:
    for slot in root.material_slots:
        mat = slot.material
        if not mat or not mat.use_nodes:
            continue
        bsdf = mat.node_tree.nodes.get("Principled BSDF")
        if not bsdf:
            continue
        hue = mat.node_tree.nodes.new("ShaderNodeHueSaturation")
        hue.inputs["Saturation"].default_value = gain
        hue.inputs["Value"].default_value = 1.05
        links = mat.node_tree.links
        src = bsdf.inputs["Base Color"].links[0].from_socket if bsdf.inputs["Base Color"].links else None
        if src:
            links.remove(bsdf.inputs["Base Color"].links[0])
            links.new(src, hue.inputs["Color"])
        else:
            hue.inputs["Color"].default_value = bsdf.inputs["Base Color"].default_value
        links.new(hue.outputs["Color"], bsdf.inputs["Base Color"])


def finish_hud_icon(asset_id: str, root: bpy.types.Object) -> None:
    """Same studio rig as garage (#88): full bbox, neutral gradient backdrop."""
    bc.smooth_shade(root)
    bc.bevel_object(root, 0.02, 2)
    preview = REPO / "docs/art/previews" / f"{asset_id}.png"
    preview.parent.mkdir(parents=True, exist_ok=True)
    _boost_material_saturation(root)
    try:
        import blender_preview_rig as rig  # noqa: WPS433

        rig.render_preview_hud(root, preview)
    except Exception:
        bc.render_preview(root, preview)
    HUD_OUT.mkdir(parents=True, exist_ok=True)
    icon_path = HUD_OUT / f"{asset_id}.png"
    if preview.exists():
        icon_path.write_bytes(preview.read_bytes())
    STREAM.mkdir(parents=True, exist_ok=True)
    shutil.copy2(icon_path, STREAM / icon_path.name)
    print("OK HUD", asset_id, icon_path, f"blender {bc.BLENDER_VERSION}")