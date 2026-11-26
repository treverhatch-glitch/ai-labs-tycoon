#!/usr/bin/env python3
"""Render HUD icons and wisp mood sheets with studio previews."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_preview_rig as rig  # noqa: E402
import bpy  # noqa: E402

REPO = Path(__file__).resolve().parents[3]
HUD_OUT = REPO / "unity/Assets/Art/EarlyGame/hud-icons"
WISP_OUT = REPO / "unity/Assets/Art/EarlyGame/wisp-moods"
STREAM = REPO / "unity/Assets/StreamingAssets/EarlyGame"


def _wisp_body(mood_id: str) -> bpy.types.Object:
    mood = mood_id.replace("wisp-", "")
    loc_y = 0.72
    body: bpy.types.Object
    if mood == "happy":
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.44, segments=32, ring_count=14, location=(0, loc_y, 0))
        body = bpy.context.active_object
        body.scale = (1.02, 1.05, 0.98)
        bpy.ops.object.transform_apply(scale=True)
    elif mood == "angry":
        bpy.ops.mesh.primitive_ico_sphere_add(radius=0.42, subdivisions=2, location=(0, loc_y, 0))
        body = bpy.context.active_object
        for i, (ox, oy, oz) in enumerate(
            ((0.30, 0.16, 0.0), (-0.30, 0.16, 0.0), (0, 0.28, 0.0), (0, -0.22, 0.26), (0, -0.22, -0.26))
        ):
            bpy.ops.mesh.primitive_cone_add(radius1=0.07, depth=0.16, location=(ox, loc_y + oy, oz))
            sp = bpy.context.active_object
            sp.name = f"Spike_{i}"
            bc.assign_solid(sp, (0.92, 0.22, 0.18, 1.0), 0.35)
    elif mood == "worried":
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.42, segments=28, ring_count=12, location=(0, loc_y, 0))
        body = bpy.context.active_object
        body.scale = (1.08, 0.82, 0.90)
        bpy.ops.object.transform_apply(scale=True)
    elif mood == "glowing":
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.48, segments=36, ring_count=18, location=(0, loc_y, 0))
        body = bpy.context.active_object
        halo = bc.add_cylinder("GlowHalo", 0.54, 0.05, (0, loc_y + 0.20, 0), vertices=24)
        bc.assign_emission(halo, (0.72, 0.48, 1.0, 1.0), strength=6.0)
    else:
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.42, segments=30, ring_count=14, location=(0, loc_y, 0))
        body = bpy.context.active_object

    body.name = "WispBody"
    bc.smooth_shade(body)
    return body


def _wisp_face(mood_id: str, core: tuple[float, float, float, float]) -> None:
    mood = mood_id.replace("wisp-", "")
    base_y = 0.72
    eye_y, eye_sep = 0.08, 0.12
    face_z = 0.40
    if mood == "worried":
        eye_y, eye_sep = 0.06, 0.13
    elif mood == "angry":
        eye_y, eye_sep = 0.05, 0.11
    dark = (0.08, 0.06, 0.10, 1.0)
    white = (0.98, 0.98, 1.0, 1.0)
    for sx in (-1, 1):
        sclera = bc.add_cube(f"Sclera_{sx}", (0.10, 0.10, 0.03), (eye_sep * sx, base_y + eye_y, face_z))
        bc.assign_solid(sclera, white, 0.2)
        pupil = bc.add_cube(f"Eye_{sx}", (0.05, 0.06, 0.02), (eye_sep * sx, base_y + eye_y, face_z + 0.02))
        bc.assign_solid(pupil, dark, 0.15)
    if mood == "happy":
        mouth = bc.add_cube("Smile", (0.16, 0.04, 0.02), (0, base_y - 0.03, face_z))
        bc.assign_solid(mouth, (0.92, 0.22, 0.45, 1.0), 0.3)
    elif mood == "angry":
        mouth = bc.add_cube("Frown", (0.14, 0.035, 0.02), (0, base_y + 0.02, face_z))
        bc.assign_solid(mouth, (0.85, 0.12, 0.10, 1.0), 0.3)
        for sx in (-1, 1):
            brow = bc.add_cube(f"Brow_{sx}", (0.10, 0.03, 0.02), (eye_sep * sx, base_y + 0.14, face_z))
            bc.assign_solid(brow, dark, 0.2)
    elif mood == "worried":
        mouth = bc.add_cube("Omouth", (0.10, 0.10, 0.02), (0, base_y - 0.08, face_z))
        bc.assign_solid(mouth, (0.95, 0.55, 0.12, 1.0), 0.35)
    else:
        mouth = bc.add_cube("NeutralMouth", (0.12, 0.03, 0.02), (0, base_y - 0.06, face_z))
        bc.assign_solid(mouth, core, 0.35)


def _body_color(core: tuple[float, float, float, float]) -> tuple[float, float, float, float]:
    return bc._boost_saturation(core, 2.1)


def render_wisp(mood_id: str, core: tuple[float, float, float, float]) -> None:
    bc.reset_scene()
    bpy.context.scene.render.engine = "BLENDER_EEVEE"
    body = _wisp_body(mood_id)
    body_col = _body_color(core)
    bc.assign_solid(body, body_col, 0.38)
    accent = bc._boost_saturation(core, 2.2)
    if "glowing" in mood_id:
        if body.data.materials and body.data.materials[0]:
            bsdf = body.data.materials[0].node_tree.nodes.get("Principled BSDF")
            if bsdf:
                bsdf.inputs["Emission Color"].default_value = accent
                bsdf.inputs["Emission Strength"].default_value = 3.5
    _wisp_face(mood_id, accent)
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    root = bc.link_or_join(mesh_obs) if len(mesh_obs) > 1 else body
    bc.bevel_object(root, width=0.006, segments=1)
    prev = REPO / "docs/art/previews" / f"{mood_id}.png"
    prev.parent.mkdir(parents=True, exist_ok=True)
    import os

    os.environ["EARLY_ART_PREVIEW_KIND"] = "wisp"
    rig.render_preview_studio(root, prev)
    sheet_path = WISP_OUT / f"{mood_id}-sheet.png"
    WISP_OUT.mkdir(parents=True, exist_ok=True)
    sheet_path.write_bytes(prev.read_bytes())
    (STREAM / "wisp-moods").mkdir(parents=True, exist_ok=True)
    (STREAM / "wisp-moods" / sheet_path.name).write_bytes(sheet_path.read_bytes())
    for i in range(4):
        frame_path = WISP_OUT / f"{mood_id}_f{i}.png"
        frame_path.write_bytes(sheet_path.read_bytes())
        (STREAM / "wisp-moods" / frame_path.name).write_bytes(frame_path.read_bytes())
    print("WISP", mood_id, sheet_path)


def main():
    moods = {
        "wisp-happy": (0.12, 0.95, 0.28, 1),
        "wisp-neutral": (0.35, 0.72, 0.88, 1),
        "wisp-worried": (0.98, 0.62, 0.12, 1),
        "wisp-angry": (0.72, 0.08, 0.62, 1),
        "wisp-glowing": (0.55, 0.35, 1.0, 1),
    }
    for mood_id, col in moods.items():
        render_wisp(mood_id, col)


if __name__ == "__main__":
    main()