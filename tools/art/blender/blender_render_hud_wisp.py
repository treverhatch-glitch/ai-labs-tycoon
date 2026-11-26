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
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.42, segments=28, ring_count=12, location=(0, loc_y, 0))
        body = bpy.context.active_object
        body.scale = (1.04, 0.95, 1.0)
        bpy.ops.object.transform_apply(scale=True)
    elif mood == "worried":
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.42, segments=28, ring_count=12, location=(0, loc_y, 0))
        body = bpy.context.active_object
        body.scale = (1.08, 0.82, 0.90)
        bpy.ops.object.transform_apply(scale=True)
    elif mood == "glowing":
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.48, segments=36, ring_count=18, location=(0, loc_y, 0))
        body = bpy.context.active_object
        body.scale = (1.0, 1.08, 1.0)
        bpy.ops.object.transform_apply(scale=True)
    else:
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.42, segments=30, ring_count=14, location=(0, loc_y, 0))
        body = bpy.context.active_object

    body.name = "WispBody"
    bc.smooth_shade(body)
    return body


def _wisp_face(mood_id: str, core: tuple[float, float, float, float]) -> None:
    mood = mood_id.replace("wisp-", "")
    base_y = 0.72
    face_z = 0.43
    eye_sep = 0.12
    eye_w, eye_h = 0.20, 0.20
    dark = (0.02, 0.02, 0.04, 1.0)
    white = (0.99, 0.99, 0.97, 1.0)
    pupil_w, pupil_h = 0.13, 0.14

    if mood == "glowing":
        for sx in (-1, 1):
            sclera = bc.add_cube(f"EyeWhite_{sx}", (eye_w, eye_h, 0.04), (eye_sep * sx, base_y + 0.08, face_z))
            bc.assign_solid(sclera, white, 0.12)
            pupil = bc.add_cube(f"Pupil_{sx}", (pupil_w, pupil_h, 0.028), (eye_sep * sx, base_y + 0.08, face_z + 0.02))
            bc.assign_solid(pupil, dark, 0.1)
            lid = bc.add_cube(f"EyeLid_{sx}", (eye_w + 0.02, 0.05, 0.04), (eye_sep * sx, base_y + 0.10, face_z - 0.01))
            bc.assign_emission(lid, (0.92, 0.88, 1.0, 1.0), strength=2.2)
        mouth_outer = bc.add_cube("GlowSmile", (0.24, 0.10, 0.04), (0, base_y - 0.10, face_z + 0.01))
        bc.assign_emission(mouth_outer, (0.88, 0.78, 1.0, 1.0), strength=2.8)
        inner = bc.add_cube("GlowSmileInner", (0.18, 0.06, 0.025), (0, base_y - 0.10, face_z + 0.02))
        bc.assign_solid(inner, (0.45, 0.28, 0.72, 1.0), 0.25)
        return

    eye_y = 0.08 if mood != "worried" else 0.11
    for sx in (-1, 1):
        sclera = bc.add_cube(f"EyeWhite_{sx}", (eye_w, eye_h, 0.035), (eye_sep * sx, base_y + eye_y, face_z))
        bc.assign_solid(sclera, white, 0.15)
        pupil = bc.add_cube(f"Pupil_{sx}", (pupil_w, pupil_h, 0.022), (eye_sep * sx, base_y + eye_y, face_z + 0.018))
        bc.assign_solid(pupil, dark, 0.12)

    if mood == "angry":
        for sx in (-1, 1):
            brow = bc.add_cube(
                f"Brow_{sx}",
                (0.14, 0.035, 0.025),
                (eye_sep * sx + sx * 0.02, base_y + 0.20, face_z + 0.01),
            )
            bc.assign_solid(brow, dark, 0.2)
        mouth = bc.add_cube("Frown", (0.20, 0.08, 0.035), (0, base_y - 0.06, face_z + 0.01))
        bc.assign_solid(mouth, (0.82, 0.10, 0.08, 1.0), 0.32)
    elif mood == "worried":
        for sx in (-1, 1):
            brow = bc.add_cube(f"BrowRaise_{sx}", (0.14, 0.04, 0.03), (eye_sep * sx, base_y + 0.22, face_z))
            bc.assign_solid(brow, dark, 0.2)
        mouth = bc.add_cube("WorriedMouth", (0.20, 0.10, 0.035), (0, base_y - 0.12, face_z + 0.01))
        bc.assign_solid(mouth, (0.95, 0.55, 0.12, 1.0), 0.35)
    elif mood == "happy":
        mouth = bc.add_cube("Smile", (0.24, 0.09, 0.035), (0, base_y - 0.12, face_z + 0.01))
        bc.assign_solid(mouth, (0.92, 0.22, 0.45, 1.0), 0.3)
        cheek = bc.add_cube("CheekSparkle", (0.06, 0.05, 0.02), (0.20, base_y - 0.04, face_z))
        bc.assign_emission(cheek, (0.95, 0.88, 0.2, 1.0), strength=1.5)
    else:
        mouth = bc.add_cube("NeutralMouth", (0.20, 0.05, 0.035), (0, base_y - 0.10, face_z + 0.01))
        bc.assign_solid(mouth, (0.35, 0.32, 0.36, 1.0), 0.35)


def _body_color(core: tuple[float, float, float, float]) -> tuple[float, float, float, float]:
    return bc._boost_saturation(core, 2.5)


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