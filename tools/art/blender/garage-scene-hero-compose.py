#!/usr/bin/env python3
"""Cut-away garage hero — joined L walls + ambientCG PBR + Kenney whole models."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
PREVIEW = REPO / "docs/art/previews"
HDRI = REPO / "tools/art/cc0/incoming/brown_photostudio_02_1k.exr"

ROOM_W = 4.0
ROOM_D = 3.0
WALL_H = 2.45
WALL_T = 0.12
FLOOR_Z = 0.0
FLOOR_TOL = 0.02

CEILING_KEYS = {"ceiling_light", "light_cable"}
DESK_TOP_KEYS = {"monitor", "keyboard", "mouse", "computerScreen", "computerKeyboard", "computerMouse"}
WALL_KEYS = {"roll_door", "strip_light", "building_door"}
SHELL_NAMES = ("shell_floor", "shell_walls_L", "shell_skirting", "strip_light")


def _cube(loc: tuple[float, float, float], scale: tuple[float, float, float]) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=loc)
    ob = bpy.context.active_object
    ob.scale = Vector(scale)
    bpy.ops.object.transform_apply(scale=True)
    return ob


def _strip_light() -> bpy.types.Object:
    ob = _cube((ROOM_W * 0.62, 0.16, WALL_H - 0.18), (ROOM_W * 0.28, 0.05, 0.04))
    ob.name = "strip_light"
    mat = bpy.data.materials.new("strip_light_emit")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    emit = nodes.new("ShaderNodeEmission")
    emit.inputs["Color"].default_value = (0.92, 0.9, 0.82, 1.0)
    emit.inputs["Strength"].default_value = 1.1
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Base Color"].default_value = (0.78, 0.76, 0.7, 1.0)
    bsdf.inputs["Roughness"].default_value = 0.55
    mix = nodes.new("ShaderNodeMixShader")
    mix.inputs["Fac"].default_value = 0.55
    links.new(emit.outputs["Emission"], mix.inputs[1])
    links.new(bsdf.outputs["BSDF"], mix.inputs[2])
    links.new(mix.outputs["Shader"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)
    return ob


def build_shell(acg) -> list[bpy.types.Object]:
    floor = _cube((ROOM_W * 0.5, ROOM_D * 0.5, 0.045), (ROOM_W * 0.5, ROOM_D * 0.5, 0.045))
    floor.name = "shell_floor"
    acg.assign(floor, "concrete", uv_scale=2.2)

    back = _cube((ROOM_W * 0.5, WALL_T * 0.5, WALL_H * 0.5), (ROOM_W * 0.5, WALL_T * 0.5, WALL_H * 0.5))
    side = _cube((WALL_T * 0.5, ROOM_D * 0.5, WALL_H * 0.5), (WALL_T * 0.5, ROOM_D * 0.5, WALL_H * 0.5))
    bpy.ops.object.select_all(action="DESELECT")
    back.select_set(True)
    side.select_set(True)
    bpy.context.view_layer.objects.active = back
    bpy.ops.object.join()
    walls = bpy.context.active_object
    walls.name = "shell_walls_L"
    cutter = _cube((ROOM_W * 0.58, 0.22, 1.15), (0.52, 0.18, 1.02))
    cutter.name = "_door_cutter"
    mod = walls.modifiers.new("DoorCut", "BOOLEAN")
    mod.operation = "DIFFERENCE"
    mod.object = cutter
    bpy.context.view_layer.objects.active = walls
    bpy.ops.object.modifier_apply(modifier=mod.name)
    bpy.data.objects.remove(cutter, do_unlink=True)
    acg.assign(walls, "brick", uv_scale=1.35)

    skirt = _cube((ROOM_W * 0.5, 0.11, 0.08), (ROOM_W * 0.5, 0.04, 0.08))
    skirt.name = "shell_skirting"
    acg.assign(skirt, "brick", uv_scale=2.6)

    strip = _strip_light()
    for ob in (floor, walls, skirt, strip):
        low = min((ob.matrix_world @ v.co).z for v in ob.data.vertices)
        if low > 1e-4:
            ob.location.z -= low
    bpy.context.view_layer.update()
    names = ("shell_floor", "shell_walls_L", "shell_skirting", "strip_light")
    return [bpy.data.objects[n] for n in names if n in bpy.data.objects]


def build_ceiling_light(kh) -> list[bpy.types.Object]:
    objs: list[bpy.types.Object] = []
    cable = _cube((ROOM_W * 0.55, ROOM_D * 0.48, 2.22), (0.02, 0.02, 0.55))
    cable.name = "light_cable"
    cable_mat = bpy.data.materials.new("light_cable_mat")
    cable_mat.use_nodes = True
    bsdf = cable_mat.node_tree.nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Base Color"].default_value = (0.2, 0.2, 0.22, 1.0)
    bsdf.inputs["Roughness"].default_value = 0.62
    out = cable_mat.node_tree.nodes.new("ShaderNodeOutputMaterial")
    cable_mat.node_tree.links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    cable.data.materials.append(cable_mat)
    objs.append(cable)
    lamp = kh.import_kenney(
        "furniture",
        "lamp",
        (ROOM_W * 0.55, ROOM_D * 0.48, 2.38),
        scale=(1.2, 1.2, 0.55),
        palette_key="trim",
        up_axis="Z",
        floor_snap=False,
    )
    if lamp:
        lamp.name = "ceiling_light"
        objs.append(lamp)
    return objs


def build_furniture(kh) -> list[bpy.types.Object]:
    props: list[bpy.types.Object] = []
    specs = [
        ("building", "door", (ROOM_W * 0.58, 0.16, FLOOR_Z), 0.0, 1.12, "metal", "roll_door"),
        ("furniture", "workbench", (0.95, ROOM_D - 0.85, FLOOR_Z), 0.0, 1.32, "desk", "workbench"),
        ("furniture", "server_rack", (0.78, 1.05, FLOOR_Z), 90.0, 1.22, "metal", "server_rack"),
        ("furniture", "shelf", (ROOM_W - 0.82, 0.95, FLOOR_Z), -90.0, 1.18, "desk", "shelf_low"),
        ("furniture", "box_closed", (ROOM_W - 0.55, 0.55, FLOOR_Z), 14.0, 1.2, "cardboard", "box_a"),
        ("furniture", "box_open", (ROOM_W - 0.35, 0.85, FLOOR_Z), -8.0, 1.15, "cardboard", "box_b"),
        ("furniture", "desk", (1.85, ROOM_D - 0.95, FLOOR_Z), -8.0, 1.08, "desk", "desk"),
        ("furniture", "chair", (2.35, ROOM_D - 1.15, FLOOR_Z), 22.0, 1.05, "plastic", "chair"),
    ]
    for pack, asset, loc, rot, sc, pal, inst in specs:
        ob = kh.import_kenney(
            pack,
            asset,
            loc,
            rot_z_deg=rot,
            scale=sc,
            palette_key=pal,
            up_axis="Z",
        )
        if ob:
            ob.name = f"kenney_{inst}"
            props.append(ob)
    desk_x, desk_y = 1.85, ROOM_D - 0.95
    desk_z = 0.74
    for asset, off, name in (
        ("screen", (0.0, 0.0, desk_z), "monitor"),
        ("keyboard", (-0.06, 0.18, desk_z - 0.02), "keyboard"),
        ("mouse", (0.38, 0.2, desk_z - 0.02), "mouse"),
    ):
        ob = kh.import_kenney(
            "furniture",
            asset,
            (desk_x + off[0], desk_y + off[1], off[2]),
            rot_z_deg=-8.0,
            scale=1.05,
            palette_key="screen" if asset == "screen" else "plastic",
            up_axis="Z",
            floor_snap=False,
        )
        if ob:
            ob.name = f"kenney_{name}"
            props.append(ob)
    return props


def main() -> None:
    import ambientcg_lib as acg  # type: ignore
    import kenney_hero_lib as kh  # type: ignore

    bpy.ops.wm.read_factory_settings(use_empty=True)
    build_shell(acg)
    for n in ("shell_floor", "shell_walls_L", "shell_skirting"):
        ob = bpy.data.objects.get(n)
        if ob:
            kh.snap_to_floor_z(ob, FLOOR_Z)
    props = build_furniture(kh)
    props += build_ceiling_light(kh)

    all_meshes = [bpy.data.objects[n] for n in SHELL_NAMES if n in bpy.data.objects] + props

    map_errs = acg.audit_surface_maps([n for n in SHELL_NAMES if n.startswith("shell_")])
    rough_errs = acg.audit_roughness_floor(all_meshes)
    gnd_errs = kh.audit_grounding(
        all_meshes,
        up_axis="Z",
        floor=FLOOR_Z,
        wall_mounted=WALL_KEYS,
        ceiling_mounted=CEILING_KEYS,
        exempt=DESK_TOP_KEYS,
        wall_plane="side",
        wall_coord=0.16,
        tol_floor=FLOOR_TOL,
        tol_wall=0.32,
        ceiling_z=2.05,
    )
    if map_errs:
        raise RuntimeError(f"shell PBR: {map_errs}")
    if rough_errs:
        raise RuntimeError(f"roughness: {rough_errs[:6]}")
    if gnd_errs:
        raise RuntimeError(f"grounding (>2cm): {gnd_errs[:8]}")

    import blender_preview_rig as rig  # type: ignore

    out = PREVIEW / "garage-scene-urp-hero.png"
    PREVIEW.mkdir(parents=True, exist_ok=True)
    rig.render_preview_studio(
        all_meshes,
        out,
        resolution=1024,
        up_axis="Z",
        lens_mm=38.0,
        azimuth_deg=42.0,
        elevation_deg=32.0,
        framing_margin=0.66,
        mask_meshes=all_meshes,
        hdri_path=HDRI if HDRI.is_file() else None,
        hdri_strength=0.4,
        hdri_rotation_deg=118.0,
        hdri_lighting_only=True,
        render_engine="CYCLES",
        cycles_samples=128,
    )
    meta_path = out.with_name(out.stem + ".frame.json")
    meta = {}
    if meta_path.is_file():
        meta = json.loads(meta_path.read_text(encoding="utf-8"))
    meta["grounding_violations"] = kh.audit_grounding(
        all_meshes,
        up_axis="Z",
        floor=FLOOR_Z,
        wall_mounted=WALL_KEYS,
        ceiling_mounted=CEILING_KEYS,
        exempt=DESK_TOP_KEYS,
        wall_plane="side",
        wall_coord=0.16,
        tol_floor=FLOOR_TOL,
        tol_wall=0.32,
        ceiling_z=2.05,
    )
    meta["roughness_violations"] = rough_errs
    meta_path.write_text(json.dumps(meta, indent=2) + "\n", encoding="utf-8")
    print("OK garage-scene-urp-hero", out, "meshes", len(all_meshes))


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()