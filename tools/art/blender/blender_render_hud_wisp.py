#!/usr/bin/env python3
"""Render HUD icons and wisp mood sheets in Blender (PNG primary). SVG fallbacks stay from generate_early_hud_icons.py."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import bpy  # noqa: E402

REPO = Path(__file__).resolve().parents[3]
HUD_OUT = REPO / "unity/Assets/Art/EarlyGame/hud-icons"
WISP_OUT = REPO / "unity/Assets/Art/EarlyGame/wisp-moods"
STREAM = REPO / "unity/Assets/StreamingAssets/EarlyGame"


def render_icon(name: str, color: tuple[float, float, float, float]) -> None:
    bc.reset_scene()
    bpy.context.scene.render.engine = "BLENDER_EEVEE"
    bpy.ops.mesh.primitive_plane_add(size=1.2)
    plane = bpy.context.active_object
    mat = bpy.data.materials.new(name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes["Principled BSDF"]
    bsdf.inputs["Base Color"].default_value = color
    bsdf.inputs["Emission Color"].default_value = color
    bsdf.inputs["Emission Strength"].default_value = 0.6
    plane.data.materials.append(mat)
    preview = REPO / "docs/art/previews" / f"{name}.png"
    preview.parent.mkdir(parents=True, exist_ok=True)
    try:
        bc.render_preview(plane, preview)
    except Exception:
        pass
    HUD_OUT.mkdir(parents=True, exist_ok=True)
    icon_path = HUD_OUT / f"{name}.png"
    if preview.exists():
        icon_path.write_bytes(preview.read_bytes())
    (STREAM / "hud-icons").mkdir(parents=True, exist_ok=True)
    (STREAM / "hud-icons" / icon_path.name).write_bytes(icon_path.read_bytes())
    print("HUD", name, icon_path)


def render_wisp(mood_id: str, core: tuple[float, float, float, float]) -> None:
    bc.reset_scene()
    bpy.context.scene.render.engine = "BLENDER_EEVEE"
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.35, segments=24, ring_count=12)
    body = bpy.context.active_object
    mat = bpy.data.materials.new(mood_id)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes["Principled BSDF"]
    bsdf.inputs["Base Color"].default_value = core
    bsdf.inputs["Emission Color"].default_value = core
    bsdf.inputs["Emission Strength"].default_value = 2.5
    body.data.materials.append(mat)
    sheet_path = WISP_OUT / f"{mood_id}-sheet.png"
    WISP_OUT.mkdir(parents=True, exist_ok=True)
    bc.render_preview(body, sheet_path)
    prev = REPO / "docs/art/previews" / f"{mood_id}.png"
    prev.write_bytes(sheet_path.read_bytes())
    (STREAM / "wisp-moods").mkdir(parents=True, exist_ok=True)
    (STREAM / "wisp-moods" / sheet_path.name).write_bytes(sheet_path.read_bytes())
    print("WISP", mood_id, sheet_path)


def main():
    icons = {
        "icon-cash-era0": (0.66, 0.35, 0.16, 1),
        "icon-date-era0": (0.78, 0.72, 0.63, 1),
        "icon-era-bar": (0.66, 0.35, 0.16, 1),
        "icon-build-panel": (0.95, 0.75, 0.35, 1),
        "icon-hire-panel": (0.4, 0.75, 0.45, 1),
        "icon-train-panel": (0.55, 0.4, 0.95, 1),
    }
    for name, col in icons.items():
        render_icon(name, col)
    moods = {
        "wisp-happy": (0.55, 0.4, 0.95, 1),
        "wisp-neutral": (0.35, 0.55, 0.95, 1),
        "wisp-worried": (0.95, 0.75, 0.35, 1),
        "wisp-angry": (0.91, 0.36, 0.29, 1),
        "wisp-glowing": (0.69, 0.5, 1, 1),
    }
    for mood_id, col in moods.items():
        render_wisp(mood_id, col)


if __name__ == "__main__":
    main()