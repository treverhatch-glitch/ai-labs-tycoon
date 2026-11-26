#!/usr/bin/env python3
"""Furnished cut-away garage hero — textured L-room shell + garage-kit props, Y-up."""
from __future__ import annotations

import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
PREVIEW = REPO / "docs/art/previews"
HDRI = REPO / "tools/art/cc0/incoming/brown_photostudio_02_1k.exr"

TILE = 0.45
COLS = 9
ROWS = 7
ROOM_W = COLS * TILE
ROOM_D = ROWS * TILE
WALL_H = 2.2


def mesh_min_y(ob: bpy.types.Object) -> float:
    return min((ob.matrix_world @ v.co).y for v in ob.data.vertices)


def snap_prop_to_floor(ob: bpy.types.Object, floor_y: float = 0.0) -> None:
    bpy.context.view_layer.update()
    low = mesh_min_y(ob)
    if abs(low - floor_y) > 1e-4:
        ob.location.y += floor_y - low
    bpy.context.view_layer.update()


def import_garage(
    asset_id: str,
    location: tuple[float, float, float],
    rot_y_deg: float = 0.0,
    scale: float | tuple[float, float, float] = 1.0,
    instance: str | None = None,
    floor_snap: bool = True,
) -> bpy.types.Object | None:
    import garage_kit_lib as gk  # type: ignore

    glb = KIT / f"{asset_id}.glb"
    if not glb.is_file():
        print(f"WARN missing {glb}")
        return None
    keep_before = {o.name for o in bpy.data.objects}
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
    root.name = instance or asset_id
    if isinstance(scale, (int, float)):
        root.scale = (scale, scale, scale)
    else:
        root.scale = Vector(scale)
    bpy.ops.object.transform_apply(scale=True)
    root.rotation_euler[1] = rot_y_deg * math.pi / 180.0
    bpy.ops.object.transform_apply(rotation=True)
    root.location = Vector(location)
    gk.assign_pbr(root, asset_id)
    if floor_snap:
        snap_prop_to_floor(root)
    else:
        root.location = Vector(location)
        bpy.context.view_layer.update()
    for o in list(bpy.data.objects):
        if o.name not in keep_before and o.name != root.name and o.type == "MESH":
            bpy.data.objects.remove(o, do_unlink=True)
    return root


def _shell_box(
    name: str,
    location: tuple[float, float, float],
    scale: tuple[float, float, float],
    texture_id: str,
) -> bpy.types.Object:
    import garage_kit_lib as gk  # type: ignore

    bpy.ops.mesh.primitive_cube_add(size=1.0, location=location)
    ob = bpy.context.active_object
    ob.name = name
    ob.scale = Vector(scale)
    bpy.ops.object.transform_apply(scale=True)
    gk.assign_pbr(ob, texture_id)
    return ob


def build_corner_shell() -> list[bpy.types.Object]:
    floor = _shell_box(
        "room_floor",
        (ROOM_W * 0.5, 0.02, ROOM_D * 0.5),
        (ROOM_W * 0.5, 0.02, ROOM_D * 0.5),
        "garage-floor-tile",
    )
    back = _shell_box(
        "room_back_wall",
        (ROOM_W * 0.5, WALL_H * 0.5, 0.1),
        (ROOM_W * 0.5, WALL_H * 0.5, 0.1),
        "garage-exterior-wall",
    )
    side = _shell_box(
        "room_side_wall",
        (0.1, WALL_H * 0.5, ROOM_D * 0.5),
        (0.1, WALL_H * 0.5, ROOM_D * 0.5),
        "garage-wall-panel",
    )
    return [floor, back, side]


def build_furnished_props() -> list[bpy.types.Object]:
    props: list[bpy.types.Object] = []
    specs = [
        ("garage-roll-door", (2.35, 0.0, 0.22), 0.0, "roll_door", True),
        ("garage-whiteboard-1995", (3.05, 1.15, 0.22), 0.0, "whiteboard", False),
        ("garage-window-frame", (0.22, 1.05, 1.6), 90.0, "side_window", False),
        ("garage-cable-spool", (1.05, 0, 1.15), 12.0, "workbench_spool", True),
        ("garage-server-closet", (0.95, 0, 2.25), -4.0, "shelves_closet", True),
        ("garage-voodude-rack", (3.05, 0, 1.85), -82.0, "server_rack", True),
        ("garage-desk-crt", (2.2, 0, 1.5), -20.0, "desk_pc", True),
        ("garage-crt-tower", (2.7, 0, 1.05), 8.0, "pc_tower", True),
        ("garage-folding-chair", (1.72, 0, 1.28), 22.0, "chair", True),
        ("garage-coffee-drip", (2.48, 0, 1.58), 0.0, "coffee", True),
    ]
    for asset_id, loc, rot, inst, snap in specs:
        ob = import_garage(asset_id, loc, rot_y_deg=rot, instance=inst, floor_snap=snap)
        if ob:
            props.append(ob)
    return props


def main() -> None:
    import garage_kit_lib as gk  # type: ignore

    bpy.ops.wm.read_factory_settings(use_empty=True)
    shell = build_corner_shell()
    props = build_furnished_props()
    all_meshes = shell + props
    errs = gk.audit_pbr(all_meshes)
    if errs:
        raise RuntimeError(f"PBR audit failed ({len(errs)}): {errs[:5]}")

    import blender_preview_rig as rig  # type: ignore

    out = PREVIEW / "garage-scene-urp-hero.png"
    PREVIEW.mkdir(parents=True, exist_ok=True)
    rig.render_preview_studio(
        all_meshes,
        out,
        resolution=1024,
        up_axis="Y",
        lens_mm=42.0,
        azimuth_deg=42.0,
        elevation_deg=30.0,
        framing_margin=1.375,
        mask_meshes=all_meshes,
        hdri_path=HDRI if HDRI.is_file() else None,
        hdri_strength=0.58,
        hdri_rotation_deg=118.0,
        hdri_lighting_only=True,
        render_engine="CYCLES",
        cycles_samples=128,
    )
    print("OK garage-scene-urp-hero", out, "meshes", len(all_meshes))


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()