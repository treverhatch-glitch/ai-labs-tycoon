#!/usr/bin/env python3
"""Hero garage interior: EarlyGame GLBs, Poly Haven HDRI, 3/4 tycoon camera (URP lane parity)."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
PREVIEW = REPO / "docs/art/previews"
HDRI = REPO / "tools/art/cc0/incoming/brown_photostudio_02_1k.exr"

# Match EarlyGameGarageDecorator lot 14×10 @ 0.45 tile (w×h metres).
LOT_W = 14 * 0.45
LOT_H = 10 * 0.45
FLOOR_Y = 0.0


def mesh_min_y(ob: bpy.types.Object) -> float:
    return min((ob.matrix_world @ v.co).y for v in ob.data.vertices)


def snap_prop_to_floor(ob: bpy.types.Object, floor_y: float = FLOOR_Y) -> None:
    bpy.context.view_layer.update()
    low = mesh_min_y(ob)
    if abs(low - floor_y) > 1e-4:
        ob.location.y += floor_y - low
    bpy.context.view_layer.update()


def import_prop(asset_id: str, loc: tuple[float, float, float], rot_y_deg: float = 0.0) -> bpy.types.Object | None:
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
    root.rotation_euler[1] = rot_y_deg * 3.14159265 / 180.0
    bpy.ops.object.transform_apply(rotation=True)
    snap_prop_to_floor(root)
    return root


def garage_layout() -> list[tuple[str, tuple[float, float, float], float]]:
    w, h = LOT_W, LOT_H
    return [
        ("garage-floor-tile", (w * 0.5, FLOOR_Y, h * 0.5), 0.0),
        ("garage-wall-panel", (w * 0.5, FLOOR_Y, 0.35), 0.0),
        ("garage-exterior-wall", (w * 0.5, FLOOR_Y, h - 0.25), 180.0),
        ("garage-roll-door", (w * 0.5, FLOOR_Y, -0.12), 0.0),
        ("garage-server-closet", (w - 0.75, FLOOR_Y, h - 0.55), -8.0),
        ("garage-land-for-sale-sign", (-0.35, FLOOR_Y, h * 0.5), 75.0),
        ("garage-desk-crt", (1.35, FLOOR_Y, 1.15), 12.0),
        ("garage-voodude-rack", (w - 1.1, FLOOR_Y, 1.45), -15.0),
        ("garage-whiteboard-1995", (3.1, FLOOR_Y, 0.32), 0.0),
        ("garage-coffee-drip", (0.75, FLOOR_Y, 0.65), 22.0),
        ("garage-folding-chair", (2.0, FLOOR_Y, 1.85), -20.0),
        ("garage-crt-tower", (w - 1.8, FLOOR_Y, 0.9), 8.0),
        ("garage-window-frame", (0.4, FLOOR_Y, h - 0.4), 90.0),
        ("garage-cable-spool", (4.2, FLOOR_Y, 2.8), 0.0),
        ("garage-lot-curb", (-0.2, FLOOR_Y, -0.35), 0.0),
    ]


def main() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    props: list[bpy.types.Object] = []
    for asset_id, loc, rot in garage_layout():
        ob = import_prop(asset_id, loc, rot)
        if ob:
            props.append(ob)

    import blender_preview_rig as rig  # type: ignore

    out = PREVIEW / "garage-scene-urp-hero.png"
    PREVIEW.mkdir(parents=True, exist_ok=True)
    mask_props = [p for p in props if p.name != "prop_garage-floor-tile"]
    rig.render_preview_studio(
        props,
        out,
        resolution=1024,
        up_axis="Y",
        lens_mm=42.0,
        azimuth_deg=42.0,
        elevation_deg=30.0,
        framing_margin=1.12,
        mask_meshes=mask_props,
        hdri_path=HDRI if HDRI.is_file() else None,
        hdri_strength=0.48,
        hdri_rotation_deg=118.0,
    )
    print("OK garage-scene-urp-hero", out, "props", len(props))


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()