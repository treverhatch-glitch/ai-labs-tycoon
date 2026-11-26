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


def _wisp_body(mood_id: str) -> bpy.types.Object:
    mood = mood_id.replace("wisp-", "")
    loc_y = 0.82
    body: bpy.types.Object
    if mood == "happy":
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.50, segments=32, ring_count=14, location=(0, loc_y, 0))
        body = bpy.context.active_object
        body.scale = (1.05, 1.12, 1.0)
        bpy.ops.object.transform_apply(scale=True)
    elif mood == "angry":
        bpy.ops.mesh.primitive_ico_sphere_add(radius=0.48, subdivisions=2, location=(0, loc_y, 0))
        body = bpy.context.active_object
        for i, (ox, oy, oz) in enumerate(
            ((0.32, 0.18, 0.0), (-0.32, 0.18, 0.0), (0, 0.32, 0.0), (0, -0.25, 0.28), (0, -0.25, -0.28))
        ):
            bpy.ops.mesh.primitive_cone_add(radius1=0.06, depth=0.14, location=(ox, loc_y + oy, oz))
            sp = bpy.context.active_object
            sp.name = f"Spike_{i}"
            bc.assign_solid(sp, (0.92, 0.22, 0.18, 1.0), 0.35)
    elif mood == "worried":
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.44, segments=28, ring_count=12, location=(0, loc_y, 0))
        body = bpy.context.active_object
        body.scale = (1.05, 0.88, 0.92)
        bpy.ops.object.transform_apply(scale=True)
    elif mood == "glowing":
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.54, segments=36, ring_count=18, location=(0, loc_y, 0))
        body = bpy.context.active_object
        halo = bc.add_cylinder("GlowHalo", 0.58, 0.04, (0, loc_y + 0.22, 0), vertices=24)
        bc.assign_emission(halo, (0.78, 0.55, 1.0, 1.0), strength=5.0)
        aura = bc.add_cylinder("AuraRing", 0.82, 0.025, (0, loc_y, 0), vertices=32)
        bc.assign_emission(aura, (0.62, 0.38, 1.0, 1.0), strength=6.0)
    else:
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.40, segments=30, ring_count=14, location=(0, loc_y, 0))
        body = bpy.context.active_object

    body.name = "WispBody"
    bc.smooth_shade(body)
    return body


def _wisp_face(mood_id: str, core: tuple[float, float, float, float]) -> None:
    mood = mood_id.replace("wisp-", "")
    base_y = 0.82
    eye_y, eye_sep = 0.10, 0.11
    if mood == "worried":
        eye_y, eye_sep = 0.07, 0.12
    elif mood == "angry":
        eye_y, eye_sep = 0.06, 0.10
    dark = (0.12, 0.10, 0.14, 1.0)
    for sx in (-1, 1):
        eye = bc.add_cube(f"Eye_{sx}", (0.06, 0.07, 0.03), (eye_sep * sx, base_y + eye_y, 0.34))
        bc.assign_emission(eye, dark, strength=0.4)
        sclera = bc.add_cube(f"Sclera_{sx}", (0.07, 0.07, 0.02), (eye_sep * sx, base_y + eye_y, 0.36))
        bc.assign_emission(sclera, (0.98, 0.98, 1.0, 1.0), strength=1.2)
    if mood == "happy":
        mouth = bc.add_cube("Smile", (0.14, 0.03, 0.02), (0, base_y - 0.02, 0.36))
        bc.assign_emission(mouth, (0.95, 0.18, 0.55, 1.0), strength=2.5)
        for sx in (-1, 1):
            cheek = bc.add_cube(f"Cheek_{sx}", (0.08, 0.06, 0.02), (0.17 * sx, base_y - 0.02, 0.37))
            bc.assign_emission(cheek, (0.98, 0.35, 0.62, 1.0), strength=3.0)
    elif mood == "angry":
        mouth = bc.add_cube("Frown", (0.12, 0.03, 0.02), (0, base_y + 0.02, 0.35))
        bc.assign_emission(mouth, (0.95, 0.12, 0.08, 1.0), strength=2.2)
        for sx in (-1, 1):
            brow = bc.add_cube(f"Brow_{sx}", (0.08, 0.025, 0.02), (eye_sep * sx, base_y + 0.14, 0.36))
            bc.assign_solid(brow, dark, 0.2)
    elif mood == "worried":
        mouth = bc.add_cube("Omouth", (0.10, 0.10, 0.02), (0, base_y - 0.08, 0.35))
        bc.assign_emission(mouth, (0.98, 0.32, 0.05, 1.0), strength=4.8)
        for sx in (-1, 1):
            brow = bc.add_cube(f"WorriedBrow_{sx}", (0.11, 0.035, 0.02), (eye_sep * sx, base_y + 0.16, 0.37))
            bc.assign_emission(brow, (0.98, 0.42, 0.05, 1.0), strength=6.5)
    else:
        mouth = bc.add_cube("NeutralMouth", (0.10, 0.025, 0.02), (0, base_y - 0.06, 0.35))
        bc.assign_emission(mouth, core, strength=2.0)
        badge = bc.add_cube("NeutralBadge", (0.06, 0.14, 0.02), (0.20, base_y + 0.02, 0.37))
        bc.assign_emission(badge, (0.25, 0.65, 0.95, 1.0), strength=4.0)


def _pastel_body(core: tuple[float, float, float, float], mood: str) -> tuple[float, float, float, float]:
    rf, gf, bf, _ = core
    lum = 0.2126 * rf + 0.7152 * gf + 0.0722 * bf
    gray = (lum, lum, lum)
    mix_by_mood = {"happy": 0.42, "neutral": 0.65, "worried": 0.48, "angry": 0.38, "glowing": 0.52}
    mix = mix_by_mood.get(mood, 0.58)
    body = (
        gray[0] * mix + rf * (1.0 - mix),
        gray[1] * mix + gf * (1.0 - mix),
        gray[2] * mix + bf * (1.0 - mix),
        1.0,
    )
    return tuple(min(1.0, max(0.55, c)) for c in body[:3]) + (1.0,)


def _mood_accent(core: tuple[float, float, float, float]) -> tuple[float, float, float, float]:
    return bc._boost_saturation(core, 2.4)


def render_wisp(mood_id: str, core: tuple[float, float, float, float]) -> None:
    bc.reset_scene()
    bpy.context.scene.render.engine = "BLENDER_EEVEE"
    body = _wisp_body(mood_id)
    mood = mood_id.replace("wisp-", "")
    body_col = _pastel_body(core, mood)
    accent = _mood_accent(core)
    bc.assign_solid(body, body_col, 0.32)
    emit = 4.2 if "glowing" in mood_id else 0.35
    if body.data.materials and body.data.materials[0]:
        bsdf = body.data.materials[0].node_tree.nodes.get("Principled BSDF")
        if bsdf:
            bsdf.inputs["Emission Color"].default_value = accent
            bsdf.inputs["Emission Strength"].default_value = emit
    _wisp_face(mood_id, accent)
    stripe_y = 0.82
    stripe_offsets = {
        "happy": 0.18,
        "neutral": 0.02,
        "worried": -0.14,
        "angry": 0.08,
        "glowing": 0.22,
    }
    stripe_sizes = {
        "happy": (0.26, 0.11, 0.05),
        "neutral": (0.18, 0.08, 0.04),
        "worried": (0.20, 0.07, 0.04),
        "angry": (0.30, 0.12, 0.06),
        "glowing": (0.34, 0.09, 0.05),
    }
    sz = stripe_sizes.get(mood, (0.22, 0.10, 0.04))
    off = stripe_offsets.get(mood, 0.05)
    stripe = bc.add_cube(f"MoodStripe_{mood}", sz, (0, stripe_y + off, 0.43))
    bc.assign_emission(stripe, accent, strength=5.0)
    markers = {
        "happy": ((0.42, 0.34, 0.08), (-0.46, stripe_y + 0.18, 0.56), (0.98, 0.25, 0.08, 1.0)),
        "neutral": ((0.36, 0.46, 0.08), (0.46, stripe_y + 0.04, 0.56), (0.08, 0.35, 0.98, 1.0)),
        "worried": ((0.42, 0.34, 0.08), (-0.50, stripe_y - 0.20, 0.56), (0.55, 0.12, 0.92, 1.0)),
        "angry": ((0.36, 0.30, 0.08), (0.45, stripe_y + 0.18, 0.56), (0.98, 0.12, 0.08, 1.0)),
        "glowing": ((0.46, 0.52, 0.08), (0.52, stripe_y + 0.30, 0.56), (0.12, 0.02, 0.98, 1.0)),
    }
    if mood in markers:
        msz, mloc, mcol = markers[mood]
        mark = bc.add_cube(f"MoodMark_{mood}", msz, mloc)
        bc.assign_emission(mark, mcol, strength=6.0)
    if mood == "angry":
        horn = bc.add_cube("AngryHorn", (0.16, 0.08, 0.05), (0, stripe_y + 0.28, 0.40))
        bc.assign_emission(horn, (0.95, 0.12, 0.08, 1.0), strength=5.0)
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if len(mesh_obs) > 1:
        root = bc.link_or_join(mesh_obs)
    else:
        root = body
    bc.bevel_object(root, width=0.006, segments=1)
    sheet_path = WISP_OUT / f"{mood_id}-sheet.png"
    WISP_OUT.mkdir(parents=True, exist_ok=True)
    prev = REPO / "docs/art/previews" / f"{mood_id}.png"
    prev.parent.mkdir(parents=True, exist_ok=True)
    rig.render_preview_studio(root, prev)
    sheet_path.write_bytes(prev.read_bytes())
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
        "wisp-happy": (0.88, 0.42, 0.95, 1),
        "wisp-neutral": (0.35, 0.72, 0.88, 1),
        "wisp-worried": (0.98, 0.55, 0.08, 1),
        "wisp-angry": (0.95, 0.18, 0.15, 1),
        "wisp-glowing": (0.55, 0.35, 1.0, 1),
    }
    for mood_id, col in moods.items():
        render_wisp(mood_id, col)


if __name__ == "__main__":
    main()