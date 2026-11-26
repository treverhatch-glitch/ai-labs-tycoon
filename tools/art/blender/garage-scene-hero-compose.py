#!/usr/bin/env python3
"""Cut-away garage hero — L-shell room, garage-kit props, Y-up (Unity / glTF)."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
PREVIEW = REPO / "docs/art/previews"
HDRI = REPO / "tools/art/cc0/incoming/brown_photostudio_02_1k.exr"

ROOM_W = 4.05
ROOM_D = 3.15
WALL_H = 2.35
WALL_T = 0.12


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
    root.name = asset_id
    if isinstance(scale, (int, float)):
        root.scale = (scale, scale, scale)
    else:
        root.scale = Vector(scale)
    bpy.ops.object.transform_apply(scale=True)
    root.rotation_euler[1] = rot_y_deg * 3.14159265 / 180.0
    bpy.ops.object.transform_apply(rotation=True)
    root.location = Vector(location)
    snap_prop_to_floor(root)
    for o in list(bpy.data.objects):
        if o.name not in before and o.name != root.name and o.type == "MESH":
            bpy.data.objects.remove(o, do_unlink=True)
    return root


def _shell_material(name: str, color: tuple[float, float, float, float]) -> bpy.types.Material:
    mat = bpy.data.materials.new(name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = color
        bsdf.inputs["Roughness"].default_value = 0.62
    return mat


def _add_box(
    name: str,
    loc: tuple[float, float, float],
    scale: tuple[float, float, float],
    mat: bpy.types.Material,
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.scale = Vector(scale)
    bpy.ops.object.transform_apply(scale=True)
    ob.data.materials.append(mat)
    return ob


def build_l_shell() -> list[bpy.types.Object]:
    import kenney_hero_lib as kh  # type: ignore

    shell: list[bpy.types.Object] = []
    wall_mat = _shell_material("shell_wall", kh.PALETTE["wall"])
    floor_mat = _shell_material("shell_floor", kh.PALETTE["floor"])

    floor = _add_box(
        "room_floor",
        (ROOM_W * 0.5, 0.03, ROOM_D * 0.5),
        (ROOM_W * 0.5, 0.03, ROOM_D * 0.5),
        floor_mat,
    )
    shell.append(floor)

    back = _add_box(
        "room_back_wall",
        (ROOM_W * 0.5, WALL_H * 0.5, WALL_T * 0.5),
        (ROOM_W * 0.5, WALL_H * 0.5, WALL_T * 0.5),
        wall_mat,
    )
    shell.append(back)

    side = _add_box(
        "room_side_wall",
        (WALL_T * 0.5, WALL_H * 0.5, ROOM_D * 0.5),
        (WALL_T * 0.5, WALL_H * 0.5, ROOM_D * 0.5),
        wall_mat,
    )
    shell.append(side)

    corner = _add_box(
        "room_corner_post",
        (WALL_T * 0.6, WALL_H * 0.5, WALL_T * 0.6),
        (WALL_T * 0.8, WALL_H * 0.5, WALL_T * 0.8),
        wall_mat,
    )
    shell.append(corner)
    return shell


def build_garage_props() -> list[bpy.types.Object]:
    import kenney_hero_lib as kh  # type: ignore

    props: list[bpy.types.Object] = []

    workbench = kh.import_kenney("furniture", "workbench", (1.25, 0, 0.95), rot_y_deg=8.0, scale=1.15, palette_key="desk")
    shelf = kh.import_kenney("furniture", "shelf", (0.75, 0, 2.05), rot_y_deg=90.0, scale=1.08, palette_key="desk")
    desk_pc = import_garage("garage-desk-crt", (2.55, 0, 1.35), rot_y_deg=-22.0)
    rack = import_garage("garage-server-closet", (3.35, 0, 2.05), rot_y_deg=-90.0)
    voodoo = import_garage("garage-voodude-rack", (1.85, 0, 2.35), rot_y_deg=15.0, scale=0.95)
    roll = import_garage("garage-roll-door", (3.05, 0, 0.22), rot_y_deg=0.0, scale=0.92)
    board = import_garage("garage-whiteboard-1995", (2.65, 0, 0.28), rot_y_deg=0.0)
    chair = kh.import_kenney("furniture", "chair", (2.25, 0, 1.15), rot_y_deg=12.0, scale=1.05, palette_key="plastic")
    lamp = kh.import_kenney("furniture", "lamp", (3.15, 0, 1.55), rot_y_deg=0.0, scale=1.0, palette_key="trim")
    box_c = kh.import_kenney("furniture", "box_closed", (1.55, 0, 0.65), rot_y_deg=18.0, scale=0.95, palette_key="cardboard")

    for ob in (workbench, shelf, desk_pc, rack, voodoo, roll, board, chair, lamp, box_c):
        if ob:
            props.append(ob)

    errs = kh.audit_materials([p for p in props if p.name.startswith("kenney_")])
    if errs:
        raise RuntimeError(f"kenney material audit: {errs[:3]}")
    return props


def main() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    shell = build_l_shell()
    props = build_garage_props()
    all_meshes = shell + props

    import kenney_hero_lib as kh  # type: ignore

    kh.purge_import_orphans(all_meshes)

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
        framing_margin=1.26,
        mask_meshes=all_meshes,
        hdri_path=HDRI if HDRI.is_file() else None,
        hdri_strength=0.52,
        hdri_rotation_deg=118.0,
        hdri_lighting_only=True,
        render_engine="CYCLES",
        cycles_samples=128,
    )
    print("OK garage-scene-urp-hero", out, "meshes", len(all_meshes))


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()