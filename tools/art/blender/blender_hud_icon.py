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
    return (rgb[0] / 255, rgb[1] / 255, rgb[2] / 255, 1.0)


def solid_mat(name: str, rgba: tuple[float, float, float, float], emission: float = 0.0) -> bpy.types.Material:
    mat = bpy.data.materials.new(name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes["Principled BSDF"]
    bsdf.inputs["Base Color"].default_value = rgba
    if emission > 0:
        bsdf.inputs["Emission Color"].default_value = rgba
        bsdf.inputs["Emission Strength"].default_value = emission
    return mat


def finish_hud_icon(asset_id: str, root: bpy.types.Object) -> None:
    """EEVEE preview → hud-icons PNG, StreamingAssets copy, docs/art/previews."""
    bpy.context.view_layer.objects.active = root
    root.select_set(True)
    preview = REPO / "docs/art/previews" / f"{asset_id}.png"
    preview.parent.mkdir(parents=True, exist_ok=True)
    try:
        import os

        bc.render_preview(root, preview, asset_id=asset_id)
    except Exception as exc:  # noqa: BLE001
        print(f"WARN preview {asset_id}: {exc}")
    HUD_OUT.mkdir(parents=True, exist_ok=True)
    icon_path = HUD_OUT / f"{asset_id}.png"
    if preview.exists():
        icon_path.write_bytes(preview.read_bytes())
    STREAM.mkdir(parents=True, exist_ok=True)
    shutil.copy2(icon_path, STREAM / icon_path.name)
    print("OK HUD", asset_id, icon_path, f"blender {bc.BLENDER_VERSION}")