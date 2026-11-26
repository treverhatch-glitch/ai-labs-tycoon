#!/usr/bin/env python3
"""Render HUD icons and wisp mood sheets with studio previews."""
import math
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_preview_rig as rig  # noqa: E402
import wisp_face_geom as wfg  # noqa: E402
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


def _add_eye(cx: float, cy: float, cz: float, r: float, phi: float, tag: str) -> None:
    eye_sz, _mw, thick = wfg.face_dims(r)
    theta = math.radians(92)
    x, y, z = wfg.sphere_point(cx, cy, cz, r, theta, phi)
    xn, yn, zn = x - cx, y - cy, z - cz
    ln = math.sqrt(xn * xn + yn * yn + zn * zn) or 1.0
    xn, yn, zn = xn / ln, yn / ln, zn / ln
    ox, oy, oz = x + xn * thick * 0.5, y + yn * thick * 0.5, z + zn * thick * 0.5
    sclera = bc.add_cube(f"EyeWhite_{tag}", (thick, eye_sz, eye_sz), (ox, oy, oz))
    bc.assign_emission(sclera, (1.0, 1.0, 0.98, 1.0), strength=1.8)
    pupil = bc.add_cube(
        f"Pupil_{tag}",
        (thick * 0.65, eye_sz * 0.55, eye_sz * 0.55),
        (ox + xn * thick * 0.25, oy + yn * thick * 0.25, oz + zn * thick * 0.25),
    )
    bc.assign_solid(pupil, (0.02, 0.02, 0.04, 1.0), 0.85)


def _add_mouth(cx: float, cy: float, cz: float, r: float, mouth_h: float, rgba, emit: float = 0.0) -> bpy.types.Object:
    _es, mouth_w, thick = wfg.face_dims(r)
    theta = math.radians(104)
    phi = math.pi
    x, y, z = wfg.sphere_point(cx, cy, cz, r, theta, phi)
    xn, yn, zn = x - cx, y - cy, z - cz
    ln = math.sqrt(xn * xn + yn * yn + zn * zn) or 1.0
    xn, yn, zn = xn / ln, yn / ln, zn / ln
    ox, oy, oz = x + xn * thick * 0.5, y + yn * thick * 0.5, z + zn * thick * 0.5
    mouth = bc.add_cube("WispMouth", (thick, mouth_w, mouth_h), (ox, oy, oz))
    if emit > 0.1:
        bc.assign_emission(mouth, rgba, strength=emit)
    else:
        bc.assign_solid(mouth, rgba, 0.35)
    return mouth


def _wisp_face(mood_id: str, body: bpy.types.Object) -> None:
    mood = mood_id.replace("wisp-", "")
    r = wfg.body_radius(body)
    cx, cy, cz = body.matrix_world.translation.x, body.matrix_world.translation.y, body.matrix_world.translation.z
    # Preview camera: azimuth 0° → view from +X; face sits on −X hemisphere (φ ≈ π).
    phi_l = math.pi - math.radians(14)
    phi_r = math.pi + math.radians(14)
    _add_eye(cx, cy, cz, r, phi_l, "L")
    _add_eye(cx, cy, cz, r, phi_r, "R")

    if mood == "glowing":
        for sx, tag in ((-1, "L"), (1, "R")):
            phi = phi_l if sx < 0 else phi_r
            theta = math.radians(92)
            x, y, z = wfg.sphere_point(cx, cy, cz, r, theta, phi)
            xn, yn, zn = x - cx, y - cy, z - cz
            ln = math.sqrt(xn * xn + yn * yn + zn * zn) or 1.0
            xn, yn, zn = xn / ln, yn / ln, zn / ln
            ox, oy, oz = x + xn * 0.008, y + yn * 0.008, z + zn * 0.008
            lid = bc.add_cube(f"EyeLid_{tag}", (0.01, r * 0.2, r * 0.08), (ox, oy, oz))
            bc.assign_emission(lid, (0.92, 0.88, 1.0, 1.0), strength=3.0)
        _add_mouth(cx, cy, cz, r, r * 0.14, (0.88, 0.78, 1.0, 1.0), emit=4.0)
        return

    if mood == "angry":
        for sx, tag in ((-1, "L"), (1, "R")):
            phi = phi_l if sx < 0 else phi_r
            x, y, z = wfg.sphere_point(cx, cy, cz, r, math.radians(88), phi + sx * 0.06)
            brow = bc.add_cube(f"Brow_{tag}", (r * 0.18, r * 0.05, 0.004), (x, y, z))
            bc.assign_solid(brow, (0.05, 0.05, 0.08, 1.0), 0.2)
        _add_mouth(cx, cy, cz, r, r * 0.10, (0.82, 0.10, 0.08, 1.0))
    elif mood == "worried":
        for sx, tag in ((-1, "L"), (1, "R")):
            phi = phi_l if sx < 0 else phi_r
            x, y, z = wfg.sphere_point(cx, cy, cz, r, math.radians(86), phi)
            brow = bc.add_cube(f"BrowRaise_{tag}", (r * 0.16, r * 0.05, 0.004), (x, y, z))
            bc.assign_solid(brow, (0.05, 0.05, 0.08, 1.0), 0.2)
        _add_mouth(cx, cy, cz, r, r * 0.12, (0.95, 0.55, 0.12, 1.0))
    elif mood == "happy":
        _add_mouth(cx, cy, cz, r, r * 0.11, (0.98, 0.12, 0.48, 1.0), emit=2.2)
        x, y, z = wfg.sphere_point(cx, cy, cz, r, math.radians(72), math.pi - math.radians(18))
        cheek = bc.add_cube("CheekSparkle", (r * 0.08, r * 0.06, 0.004), (x, y, z))
        bc.assign_emission(cheek, (0.95, 0.88, 0.2, 1.0), strength=1.2)
    else:
        _add_mouth(cx, cy, cz, r, r * 0.06, (0.35, 0.32, 0.36, 1.0))


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
    _wisp_face(mood_id, body)
    bc.bevel_object(body, width=0.004, segments=1)
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    root = mesh_obs
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