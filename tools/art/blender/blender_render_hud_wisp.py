#!/usr/bin/env python3
"""Render HUD icons and wisp mood sheets with studio previews."""
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


def _wisp_face(mood_id: str) -> None:
    """Simple eyes + mouth cubes parented to body."""
    mood = mood_id.replace("wisp-", "")
    eye_y, eye_sep = 0.08, 0.09
    if mood == "worried":
        eye_y, eye_sep = 0.06, 0.11
    elif mood == "angry":
        eye_y, eye_sep = 0.05, 0.10
    for sx in (-1, 1):
        eye = bc.add_cube(f"Eye_{sx}", (0.05, 0.05, 0.02), (eye_sep * sx, eye_y, 0.30))
        bc.assign_solid(eye, (0.95, 0.92, 0.88, 1.0), 0.2)
        cheek = bc.add_cube(f"Cheek_{sx}", (0.08, 0.05, 0.02), (eye_sep * sx, eye_y - 0.08, 0.28))
        bc.assign_solid(cheek, (0.95, 0.35, 0.55, 1.0), 0.22)
    if mood == "happy":
        mouth = bc.add_cube("Smile", (0.12, 0.02, 0.02), (0, -0.04, 0.31))
        bc.assign_solid(mouth, (0.92, 0.35, 0.55, 1.0), 0.25)
    elif mood == "angry":
        mouth = bc.add_cube("Frown", (0.10, 0.02, 0.02), (0, -0.02, 0.30))
        bc.assign_solid(mouth, (0.92, 0.22, 0.18, 1.0), 0.25)
        for sx in (-1, 1):
            brow = bc.add_cube(f"Brow_{sx}", (0.06, 0.02, 0.02), (eye_sep * sx, 0.12, 0.31))
            bc.assign_solid(brow, (0.1, 0.1, 0.12, 1.0), 0.2)
    elif mood == "worried":
        mouth = bc.add_cube("Omouth", (0.06, 0.06, 0.02), (0, -0.06, 0.30))
        bc.assign_solid(mouth, (0.95, 0.68, 0.22, 1.0), 0.28)
    elif mood == "glowing":
        halo = bc.add_cylinder("Halo", 0.42, 0.02, (0, 0.18, 0), vertices=20)
        bc.assign_emission(halo, (0.75, 0.55, 1.0, 1.0), strength=4.0)
    else:
        mouth = bc.add_cube("NeutralMouth", (0.08, 0.02, 0.02), (0, -0.05, 0.30))
        bc.assign_solid(mouth, (0.55, 0.65, 0.95, 1.0), 0.25)


def render_wisp(mood_id: str, core: tuple[float, float, float, float]) -> None:
    bc.reset_scene()
    bpy.context.scene.render.engine = "BLENDER_EEVEE"
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.44, segments=32, ring_count=16, location=(0, 0.44, 0))
    body = bpy.context.active_object
    body.name = "WispBody"
    bc.smooth_shade(body)
    subsurf = body.modifiers.new("Subsurf", "SUBSURF")
    subsurf.levels = 1
    bc.assign_emission(body, core, strength=2.8 if "glowing" in mood_id else 2.2)
    _wisp_face(mood_id)
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    roots = bc.link_or_join(mesh_obs)
    bc.bevel_object(roots, width=0.008, segments=1)
    sheet_path = WISP_OUT / f"{mood_id}-sheet.png"
    WISP_OUT.mkdir(parents=True, exist_ok=True)
    bc.render_preview(roots, sheet_path)
    prev = REPO / "docs/art/previews" / f"{mood_id}.png"
    prev.parent.mkdir(parents=True, exist_ok=True)
    prev.write_bytes(sheet_path.read_bytes())
    (STREAM / "wisp-moods").mkdir(parents=True, exist_ok=True)
    (STREAM / "wisp-moods" / sheet_path.name).write_bytes(sheet_path.read_bytes())
    for i in range(4):
        frame_path = WISP_OUT / f"{mood_id}_f{i}.png"
        frame_path.write_bytes(sheet_path.read_bytes())
        (STREAM / "wisp-moods" / frame_path.name).write_bytes(frame_path.read_bytes())
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