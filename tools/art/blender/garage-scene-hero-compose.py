#!/usr/bin/env python3
"""Cut-away garage — floor-edge L walls, ambientCG PBR, Kenney whole models."""
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
FLOOR_TOP = 0.09
MARGIN_XY = 0.06

CEILING_KEYS = {"light_cable"}
DESK_TOP_KEYS = {"monitor", "keyboard", "mouse", "desk_lamp"}
WALL_KEYS = {"roll_door", "strip_light"}
SHELL_NAMES = (
    "shell_floor",
    "shell_wall_back",
    "shell_wall_side",
    "shell_skirting",
    "shell_skirting_side",
    "strip_light",
)

BRICK_UV = 4.2
CONCRETE_UV = 2.2


def _snap_bottom_z(ob: bpy.types.Object, floor: float = 0.0) -> None:
    bpy.context.view_layer.update()
    low = min((ob.matrix_world @ v.co).z for v in ob.data.vertices)
    if abs(low - floor) > 1e-5:
        ob.location.z += floor - low
    bpy.context.view_layer.update()


def _cube(loc: tuple[float, float, float], scale: tuple[float, float, float]) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=loc)
    ob = bpy.context.active_object
    ob.scale = Vector(scale)
    bpy.ops.object.transform_apply(scale=True)
    return ob


def _strip_light() -> bpy.types.Object:
    ob = _cube((ROOM_W * 0.55, WALL_T + 0.04, WALL_H - 0.16), (ROOM_W * 0.26, 0.045, 0.035))
    ob.name = "strip_light"
    mat = bpy.data.materials.new("strip_light_emit")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    emit = nodes.new("ShaderNodeEmission")
    emit.inputs["Color"].default_value = (0.92, 0.9, 0.82, 1.0)
    emit.inputs["Strength"].default_value = 1.0
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Roughness"].default_value = 0.55
    mix = nodes.new("ShaderNodeMixShader")
    mix.inputs["Fac"].default_value = 0.5
    links.new(emit.outputs["Emission"], mix.inputs[1])
    links.new(bsdf.outputs["BSDF"], mix.inputs[2])
    links.new(mix.outputs["Shader"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)
    return ob


def build_shell(acg) -> None:
    floor = _cube((ROOM_W * 0.5, ROOM_D * 0.5, FLOOR_TOP * 0.5), (ROOM_W * 0.5, ROOM_D * 0.5, FLOOR_TOP * 0.5))
    floor.name = "shell_floor"
    acg.assign(floor, "concrete", uv_scale=CONCRETE_UV)

    back = _cube(
        (ROOM_W * 0.5, WALL_T * 0.5, WALL_H * 0.5),
        (ROOM_W * 0.5, WALL_T * 0.5, WALL_H * 0.5),
    )
    back.name = "shell_wall_back"
    side = _cube(
        (WALL_T * 0.5, ROOM_D * 0.5, WALL_H * 0.5),
        (WALL_T * 0.5, ROOM_D * 0.5, WALL_H * 0.5),
    )
    side.name = "shell_wall_side"
    for ob in (floor, back, side):
        _snap_bottom_z(ob, FLOOR_Z)

    cutter = _cube((1.55, WALL_T + 0.12, 1.12), (0.48, 0.22, 0.95))
    cutter.name = "_door_cutter"
    mod = back.modifiers.new("DoorCut", "BOOLEAN")
    mod.operation = "DIFFERENCE"
    mod.object = cutter
    bpy.context.view_layer.objects.active = back
    bpy.ops.object.modifier_apply(modifier=mod.name)
    bpy.data.objects.remove(cutter, do_unlink=True)

    acg.assign(back, "brick", uv_scale=BRICK_UV)
    acg.assign(side, "brick", uv_scale=BRICK_UV)

    skirt_back = _cube((ROOM_W * 0.5, WALL_T + 0.02, 0.06), (ROOM_W * 0.48, 0.03, 0.06))
    skirt_back.name = "shell_skirting"
    skirt_side = _cube((WALL_T + 0.02, ROOM_D * 0.5, 0.06), (0.03, ROOM_D * 0.48, 0.06))
    skirt_side.name = "shell_skirting_side"
    _snap_bottom_z(skirt_back, FLOOR_TOP)
    _snap_bottom_z(skirt_side, FLOOR_TOP)
    acg.assign(skirt_back, "brick", uv_scale=BRICK_UV * 1.4)
    acg.assign(skirt_side, "brick", uv_scale=BRICK_UV * 1.4)

    _strip_light()


def audit_prop_bounds(
    objects: list[bpy.types.Object],
    *,
    x0: float,
    y0: float,
    x1: float,
    y1: float,
    margin: float = MARGIN_XY,
    skip_prefix: tuple[str, ...] = ("shell_",),
    skip_keys: tuple[str, ...] = CEILING_KEYS,
    wall_keys: tuple[str, ...] = (),
) -> list[str]:
    errs: list[str] = []
    bpy.context.view_layer.update()
    for ob in objects:
        if ob.type != "MESH":
            continue
        if ob.name.startswith(skip_prefix):
            continue
        if any(k in ob.name for k in skip_keys):
            continue
        if any(k in ob.name for k in wall_keys):
            continue
        for v in ob.data.vertices:
            p = ob.matrix_world @ v.co
            if p.x < x0 - margin or p.x > x1 + margin or p.y < y0 - margin or p.y > y1 + margin:
                errs.append(f"{ob.name}: xy out of floor ({p.x:.2f},{p.y:.2f})")
                break
    return errs


def build_furniture(kh) -> list[bpy.types.Object]:
    props: list[bpy.types.Object] = []
    door_x, door_y = 1.55, WALL_T + 0.14
    specs = [
        ("building", "door", (door_x, door_y, FLOOR_TOP), 0.0, 1.02, "metal", "roll_door"),
        ("furniture", "workbench", (1.08, ROOM_D - 0.78, FLOOR_TOP), 6.0, 1.22, "desk", "workbench"),
        ("furniture", "server_rack", (0.82, 0.95, FLOOR_TOP), 90.0, 1.18, "metal", "server_rack"),
        ("furniture", "shelf", (ROOM_W - 0.78, 0.92, FLOOR_TOP), -88.0, 1.12, "desk", "shelf_low"),
        ("furniture", "box_closed", (ROOM_W - 0.58, 0.52, FLOOR_TOP), 10.0, 1.1, "cardboard", "box_a"),
        ("furniture", "box_open", (ROOM_W - 0.38, 0.78, FLOOR_TOP), -6.0, 1.08, "cardboard", "box_b"),
        ("furniture", "desk", (2.05, ROOM_D - 1.02, FLOOR_TOP), -10.0, 1.05, "desk", "desk"),
        ("furniture", "chair", (2.62, ROOM_D - 1.28, FLOOR_TOP), 18.0, 1.02, "plastic", "chair"),
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
            _snap_bottom_z(ob, FLOOR_TOP)
            props.append(ob)

    desk = bpy.data.objects.get("kenney_desk")
    if desk:
        top_z = max((desk.matrix_world @ v.co).z for v in desk.data.vertices)
        desk_xy = (desk.matrix_world.translation.x, desk.matrix_world.translation.y)
        for asset, dx, dy, name, pal in (
            ("screen", 0.0, 0.02, "monitor", "screen"),
            ("keyboard", -0.05, 0.14, "keyboard", "plastic"),
            ("mouse", 0.34, 0.16, "mouse", "plastic"),
        ):
            ob = kh.import_kenney(
                "furniture",
                asset,
                (desk_xy[0] + dx, desk_xy[1] + dy, top_z + 0.5),
                rot_z_deg=-10.0,
                scale=1.0,
                palette_key=pal,
                up_axis="Z",
                floor_snap=False,
            )
            if ob:
                ob.name = f"kenney_{name}"
                _snap_bottom_z(ob, top_z + 0.01)
                props.append(ob)

        lamp = kh.import_kenney(
            "furniture",
            "lamp",
            (desk_xy[0] + 0.38, desk_xy[1] - 0.1, top_z + 0.5),
            scale=0.85,
            palette_key="trim",
            up_axis="Z",
            floor_snap=False,
        )
        if lamp:
            lamp.name = "kenney_desk_lamp"
            _snap_bottom_z(lamp, top_z + 0.01)
            props.append(lamp)
    return props


def main() -> None:
    import ambientcg_lib as acg  # type: ignore
    import kenney_hero_lib as kh  # type: ignore

    bpy.ops.wm.read_factory_settings(use_empty=True)
    build_shell(acg)
    props = build_furniture(kh)

    all_meshes = [bpy.data.objects[n] for n in SHELL_NAMES if n in bpy.data.objects] + props

    map_errs = acg.audit_surface_maps([n for n in SHELL_NAMES if n.startswith("shell_")])
    rough_errs = acg.audit_roughness_floor(all_meshes)
    gnd_errs = kh.audit_grounding(
        all_meshes,
        up_axis="Z",
        floor=FLOOR_TOP,
        wall_mounted=WALL_KEYS,
        ceiling_mounted=CEILING_KEYS,
        exempt=DESK_TOP_KEYS,
        wall_plane="side",
        wall_coord=WALL_T,
        tol_floor=FLOOR_TOL,
        tol_wall=0.28,
    )
    xy_errs = audit_prop_bounds(
        props,
        x0=0.0,
        y0=0.0,
        x1=ROOM_W,
        y1=ROOM_D,
        wall_keys=tuple(WALL_KEYS),
    )
    if map_errs:
        raise RuntimeError(f"shell PBR: {map_errs}")
    if rough_errs:
        raise RuntimeError(f"roughness: {rough_errs[:6]}")
    if gnd_errs:
        raise RuntimeError(f"grounding (>2cm): {gnd_errs[:8]}")
    if xy_errs:
        raise RuntimeError(f"prop bounds: {xy_errs[:8]}")

    import blender_preview_rig as rig  # type: ignore

    out = PREVIEW / "garage-scene-urp-hero.png"
    PREVIEW.mkdir(parents=True, exist_ok=True)
    rig.render_preview_studio(
        all_meshes,
        out,
        resolution=1024,
        up_axis="Z",
        lens_mm=36.0,
        azimuth_deg=42.0,
        elevation_deg=32.0,
        framing_margin=0.58,
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
        floor=FLOOR_TOP,
        wall_mounted=WALL_KEYS,
        ceiling_mounted=CEILING_KEYS,
        exempt=DESK_TOP_KEYS,
        wall_plane="side",
        wall_coord=WALL_T,
        tol_floor=FLOOR_TOL,
        tol_wall=0.28,
    )
    meta["bounds_violations"] = xy_errs
    meta["roughness_violations"] = rough_errs
    meta["floor_top_z"] = FLOOR_TOP
    meta_path.write_text(json.dumps(meta, indent=2) + "\n", encoding="utf-8")
    print("OK garage-scene-urp-hero", out, "meshes", len(all_meshes))


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()