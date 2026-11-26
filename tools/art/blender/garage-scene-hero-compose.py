#!/usr/bin/env python3
"""Furnished cut-away garage — continuous L-shell PBR + EarlyGame props (Z-up)."""
from __future__ import annotations

import json
import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
PREVIEW = REPO / "docs/art/previews"
HDRI = REPO / "tools/art/cc0/incoming/brown_photostudio_02_1k.exr"

ROOM_W = 5.4
ROOM_D = 3.6
WALL_H = 2.45
FLOOR_Z = 0.0
SKIRT_H = 0.14

WALL_MOUNT_KEYS = {"roll_door", "whiteboard", "strip_light"}
EXEMPT_KEYS = {"desk_pc", "chair", "coffee", "pc_tower", "cable_spool", "whiteboard"}
CEILING_MOUNT_KEYS = {"strip_light"}


def _cube(name: str, loc: tuple[float, float, float], scale: tuple[float, float, float]) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.scale = Vector(scale)
    bpy.ops.object.transform_apply(scale=True)
    return ob


def _emissive_strip(name: str, loc: tuple[float, float, float], scale: tuple[float, float, float]) -> bpy.types.Object:
    ob = _cube(name, loc, scale)
    mat = bpy.data.materials.new(name=f"{name}_emit")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    emit = nodes.new("ShaderNodeEmission")
    emit.inputs["Color"].default_value = (0.95, 0.92, 0.82, 1.0)
    emit.inputs["Strength"].default_value = 3.8
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Base Color"].default_value = (0.82, 0.8, 0.74, 1.0)
    bsdf.inputs["Roughness"].default_value = 0.35
    mix = nodes.new("ShaderNodeMixShader")
    mix.inputs["Fac"].default_value = 0.82
    links.new(emit.outputs["Emission"], mix.inputs[1])
    links.new(bsdf.outputs["BSDF"], mix.inputs[2])
    links.new(mix.outputs["Shader"], out.inputs["Surface"])
    ob.data.materials.append(mat)
    return ob


def build_l_shell(gk) -> list[bpy.types.Object]:
    shell: list[bpy.types.Object] = []
    floor = _cube("shell_floor", (ROOM_W * 0.5, ROOM_D * 0.5, 0.05), (ROOM_W * 0.5, ROOM_D * 0.5, 0.05))
    gk.assign_pbr(floor, "garage-floor-tile", uv_scale=2.4)
    shell.append(floor)
    back = _cube("shell_wall_back", (ROOM_W * 0.5, 0.07, WALL_H * 0.5), (ROOM_W * 0.5, 0.07, WALL_H * 0.5))
    gk.assign_pbr(back, "garage-wall-panel", uv_scale=1.8)
    side = _cube("shell_wall_side", (0.07, ROOM_D * 0.5, WALL_H * 0.5), (0.07, ROOM_D * 0.5, WALL_H * 0.5))
    gk.assign_pbr(side, "garage-exterior-wall", uv_scale=1.6)
    shell.extend([back, side])
    skirt_back = _cube(
        "shell_skirt_back",
        (ROOM_W * 0.5, 0.12, SKIRT_H * 0.5),
        (ROOM_W * 0.5, 0.05, SKIRT_H * 0.5),
    )
    gk.assign_pbr(skirt_back, "garage-lot-curb", uv_scale=3.0)
    skirt_side = _cube(
        "shell_skirt_side",
        (0.12, ROOM_D * 0.5, SKIRT_H * 0.5),
        (0.05, ROOM_D * 0.5, SKIRT_H * 0.5),
    )
    gk.assign_pbr(skirt_side, "garage-lot-curb", uv_scale=3.0)
    shell.extend([skirt_back, skirt_side])
    strip = _emissive_strip(
        "hero_strip_light",
        (ROOM_W * 0.55, ROOM_D - 0.18, WALL_H - 0.12),
        (ROOM_W * 0.32, 0.06, 0.05),
    )
    shell.append(strip)
    return shell


def import_kit(
    gk,
    kh,
    asset_id: str,
    location: tuple[float, float, float],
    rot_z_deg: float = 0.0,
    scale: float | tuple[float, float, float] = 1.0,
    instance: str | None = None,
    floor_snap: bool = True,
) -> bpy.types.Object | None:
    glb = KIT / f"{asset_id}.glb"
    if not glb.is_file():
        print(f"WARN missing {glb}")
        return None
    before = {o.name for o in bpy.data.objects}
    bpy.ops.import_scene.gltf(filepath=str(glb))
    imported = [o for o in bpy.context.selected_objects if o.type == "MESH"]
    if not imported:
        return None
    bpy.ops.object.select_all(action="DESELECT")
    for o in imported:
        o.select_set(True)
    bpy.context.view_layer.objects.active = imported[0]
    if len(imported) > 1:
        bpy.ops.object.join()
    root = bpy.context.view_layer.objects.active
    root.name = instance or asset_id.replace("garage-", "").replace("-", "_")
    root.rotation_euler[0] = math.radians(90.0)
    bpy.ops.object.transform_apply(rotation=True)
    if isinstance(scale, (int, float)):
        root.scale = (scale, scale, scale)
    else:
        root.scale = Vector(scale)
    bpy.ops.object.transform_apply(scale=True)
    root.rotation_euler[2] = math.radians(rot_z_deg)
    bpy.ops.object.transform_apply(rotation=True)
    root.location = Vector(location)
    gk.assign_pbr(root, asset_id)
    if floor_snap:
        kh.snap_to_floor_z(root, floor_z=FLOOR_Z)
    else:
        bpy.context.view_layer.update()
    for name in before:
        ob = bpy.data.objects.get(name)
        if ob and ob.name not in {root.name} and ob.type == "MESH" and ob.name not in before:
            pass
    for o in list(bpy.data.objects):
        if o.name not in before and o.name != root.name and o.type == "MESH":
            bpy.data.objects.remove(o, do_unlink=True)
    return root


def build_props(gk, kh) -> list[bpy.types.Object]:
    props: list[bpy.types.Object] = []
    roll = import_kit(
        gk,
        kh,
        "garage-roll-door",
        (ROOM_W * 0.52, 0.2, 0.0),
        rot_z_deg=0.0,
        scale=1.05,
        instance="roll_door",
        floor_snap=True,
    )
    if roll:
        props.append(roll)
    wb = import_kit(
        gk,
        kh,
        "garage-whiteboard-1995",
        (ROOM_W * 0.38, 0.19, 1.22),
        instance="whiteboard",
        floor_snap=False,
    )
    if wb:
        props.append(wb)
    spool = import_kit(
        gk,
        kh,
        "garage-cable-spool",
        (0.9, ROOM_D - 0.85, FLOOR_Z),
        rot_z_deg=8.0,
        scale=1.15,
        instance="cable_spool",
    )
    if spool:
        props.append(spool)
    closet = import_kit(
        gk,
        kh,
        "garage-server-closet",
        (0.95, 1.05, FLOOR_Z),
        rot_z_deg=0.0,
        scale=1.08,
        instance="server_closet",
    )
    if closet:
        props.append(closet)
    rack = import_kit(
        gk,
        kh,
        "garage-voodude-rack",
        (ROOM_W - 0.75, 1.55, FLOOR_Z),
        rot_z_deg=-90.0,
        scale=1.05,
        instance="gpu_rack",
    )
    if rack:
        props.append(rack)
    desk = import_kit(
        gk,
        kh,
        "garage-desk-crt",
        (ROOM_W * 0.45, ROOM_D - 1.05, FLOOR_Z),
        rot_z_deg=-12.0,
        scale=1.12,
        instance="desk_pc",
    )
    if desk:
        props.append(desk)
    tower = import_kit(
        gk,
        kh,
        "garage-crt-tower",
        (ROOM_W * 0.72, ROOM_D - 0.95, FLOOR_Z),
        rot_z_deg=-8.0,
        scale=1.1,
        instance="pc_tower",
    )
    if tower:
        props.append(tower)
    chair = import_kit(
        gk,
        kh,
        "garage-folding-chair",
        (ROOM_W * 0.58, ROOM_D - 1.25, FLOOR_Z),
        rot_z_deg=24.0,
        scale=1.08,
        instance="chair",
    )
    if chair:
        props.append(chair)
    coffee = import_kit(
        gk,
        kh,
        "garage-coffee-drip",
        (ROOM_W * 0.38, ROOM_D - 0.72, FLOOR_Z),
        scale=1.05,
        instance="coffee",
    )
    if coffee:
        props.append(coffee)
    for i, (asset, loc) in enumerate(
        (
            ("box_closed", (ROOM_W - 0.95, 0.85, FLOOR_Z)),
            ("box_open", (ROOM_W - 0.55, 0.55, FLOOR_Z)),
        )
    ):
        ob = kh.import_kenney(
            "furniture",
            asset,
            loc,
            scale=1.25,
            up_axis="Z",
            palette_key="cardboard",
        )
        if ob:
            ob.name = f"clutter_{asset}_{i}"
            props.append(ob)
    return props


def main() -> None:
    import garage_kit_lib as gk  # type: ignore
    import kenney_hero_lib as kh  # type: ignore

    bpy.ops.wm.read_factory_settings(use_empty=True)
    shell = build_l_shell(gk)
    props = build_props(gk, kh)
    all_meshes = shell + props
    gnd_errs = kh.audit_grounding(
        props,
        up_axis="Z",
        floor=FLOOR_Z,
        wall_mounted=WALL_MOUNT_KEYS,
        ceiling_mounted=CEILING_MOUNT_KEYS,
        exempt=EXEMPT_KEYS | {"computer", "crt", "clutter"},
        wall_plane="side",
        wall_coord=0.18,
        tol_wall=0.35,
    )
    pbr_audit = [
        o
        for o in all_meshes
        if o.name.startswith(("shell_", "roll_", "whiteboard", "cable_", "server_", "gpu_", "desk_", "pc_", "chair", "coffee"))
    ]
    mat_errs = gk.audit_pbr(pbr_audit)
    if mat_errs:
        raise RuntimeError(f"PBR audit: {mat_errs[:5]}")
    if gnd_errs:
        raise RuntimeError(f"grounding failed ({len(gnd_errs)}): {gnd_errs[:6]}")

    import blender_preview_rig as rig  # type: ignore

    out = PREVIEW / "garage-scene-urp-hero.png"
    PREVIEW.mkdir(parents=True, exist_ok=True)
    rig.render_preview_studio(
        all_meshes,
        out,
        resolution=1024,
        up_axis="Z",
        lens_mm=40.0,
        azimuth_deg=42.0,
        elevation_deg=28.0,
        framing_margin=0.82,
        mask_meshes=all_meshes,
        hdri_path=HDRI if HDRI.is_file() else None,
        hdri_strength=0.42,
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
        props,
        up_axis="Z",
        floor=FLOOR_Z,
        wall_mounted=WALL_MOUNT_KEYS,
        ceiling_mounted=CEILING_MOUNT_KEYS,
        exempt=EXEMPT_KEYS | {"computer", "crt", "clutter"},
        wall_plane="side",
        wall_coord=0.18,
        tol_wall=0.35,
    )
    meta_path.write_text(json.dumps(meta, indent=2) + "\n", encoding="utf-8")
    print("OK garage-scene-urp-hero", out, "meshes", len(all_meshes))


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()