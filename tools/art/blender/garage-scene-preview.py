#!/usr/bin/env python3
"""One-car 1995 garage interior: shell + all 15 kit props (Y-up imported GLBs)."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
PREVIEW = REPO / "docs/art/previews"

# (asset_id, location XYZ, euler Z degrees) — props sit on floor (y=0) or mount to walls
LAYOUT: list[tuple[str, tuple[float, float, float], float]] = [
    ("garage-desk-crt", (1.25, 0.0, 0.40), 0.0),
    ("garage-folding-chair", (0.35, 0.0, 0.65), 18.0),
    ("garage-coffee-drip", (-0.55, 0.0, 0.95), 0.0),
    ("garage-crt-tower", (1.55, 0.0, 0.95), -8.0),
    ("garage-cable-spool", (-1.15, 0.0, 1.25), 0.0),
    ("garage-server-closet", (-1.65, 0.0, -0.55), 90.0),
    ("garage-voodude-rack", (0.55, 0.0, -0.85), 0.0),
    ("garage-window-frame", (1.85, 1.45, -1.75), 0.0),
    ("garage-whiteboard-1995", (-2.18, 1.35, 0.15), 90.0),
    ("garage-wall-panel", (-2.18, 1.05, -0.85), 90.0),
    ("garage-land-for-sale-sign", (2.05, 0.0, 0.15), -12.0),
    ("garage-lot-curb", (2.25, 0.0, 1.55), 0.0),
    ("garage-floor-tile", (-1.85, 0.0, 1.65), 0.0),
    ("garage-exterior-wall", (2.18, 0.0, -0.5), -90.0),
    ("garage-roll-door", (0.0, 0.0, -2.05), 0.0),
]

FLOOR_TOP_Y = 0.08


def build_garage_shell() -> list[bpy.types.Object]:
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    import blender_common as bc  # noqa: E402

    floor = bc.add_cube("SceneFloor", (5.4, 0.10, 4.6), (0.0, 0.05, 0.0))
    back = bc.add_cube("SceneBackWall", (5.2, 2.75, 0.16), (0.0, 1.38, -2.22))
    left = bc.add_cube("SceneLeftWall", (0.16, 2.75, 4.4), (-2.52, 1.38, 0.0))
    right = bc.add_cube("SceneRightWall", (0.16, 2.75, 4.4), (2.52, 1.38, 0.0))
    ceiling = bc.add_cube("SceneCeilingTrim", (5.0, 0.12, 4.2), (0.0, 2.72, 0.0))
    for ob in (floor, back, left, right, ceiling):
        bc.assign_solid(ob, (0.62, 0.58, 0.52, 1.0), 0.68)
    bc.assign_solid(floor, (0.48, 0.46, 0.44, 1.0), 0.82)
    return [floor, back, left, right, ceiling]


def mesh_min_y(ob: bpy.types.Object) -> float:
    return min((ob.matrix_world @ v.co).y for v in ob.data.vertices)


def mesh_max_y(ob: bpy.types.Object) -> float:
    return max((ob.matrix_world @ v.co).y for v in ob.data.vertices)


def snap_prop_to_floor(ob: bpy.types.Object, floor_y: float = FLOOR_TOP_Y) -> None:
    bpy.context.view_layer.update()
    low = mesh_min_y(ob)
    if low < floor_y - 1e-4:
        ob.location.y += floor_y - low
    bpy.context.view_layer.update()


def mount_to_wall(ob: bpy.types.Object, wall_x: float, target_y: float) -> None:
    """Shift prop so its bounding box center-height aligns after rotation."""
    bpy.context.view_layer.update()
    ob.location.x = wall_x
    low = mesh_min_y(ob)
    high = mesh_max_y(ob)
    mid = (low + high) * 0.5
    ob.location.y += target_y - mid
    bpy.context.view_layer.update()


def import_prop(
    asset_id: str,
    loc: tuple[float, float, float],
    rot_z_deg: float,
    *,
    wall_mount: bool = False,
    mount_y: float = 1.35,
    wall_x: float = 0.0,
) -> bpy.types.Object | None:
    glb = KIT / f"{asset_id}.glb"
    if not glb.exists():
        print(f"WARN missing {glb}")
        return None
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
    root.name = f"prop_{asset_id}"
    root.location = Vector(loc)
    root.rotation_euler[2] = rot_z_deg * 3.14159265 / 180.0
    bpy.ops.object.transform_apply(rotation=True)
    if wall_mount:
        mount_to_wall(root, wall_x, mount_y)
    else:
        snap_prop_to_floor(root)
    return root


def main() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    shell = build_garage_shell()
    props: list[bpy.types.Object] = []
    wall_ids = {"garage-whiteboard-1995", "garage-wall-panel", "garage-window-frame"}
    for asset_id, loc, rot_z in LAYOUT:
        ob = import_prop(
            asset_id,
            loc,
            rot_z,
            wall_mount=asset_id in wall_ids,
            mount_y=loc[1] if loc[1] > 0.5 else 1.35,
            wall_x=loc[0],
        )
        if ob:
            props.append(ob)

    import blender_preview_rig as rig  # type: ignore

    all_meshes = shell + props
    out = PREVIEW / "garage-scene-kit.png"
    PREVIEW.mkdir(parents=True, exist_ok=True)
    rig.render_preview_studio(
        all_meshes,
        out,
        resolution=768,
        up_axis="Y",
        lens_mm=42.0,
        azimuth_deg=42.0,
        elevation_deg=30.0,
        framing_margin=1.08,
        mask_meshes=props,
    )
    print("OK garage-scene-kit", out, "props", len(props), "meshes", len(all_meshes))


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()