#!/usr/bin/env python3
"""Cut-away garage hero — Kenney CC0 shell + furniture, Z-up floor, HDRI lights only."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy

REPO = Path(__file__).resolve().parents[3]
PREVIEW = REPO / "docs/art/previews"
HDRI = REPO / "tools/art/cc0/incoming/brown_photostudio_02_1k.exr"
TILE = 1.0
ROOM_W = 4.2
ROOM_D = 3.6


def build_cutaway_garage() -> list[bpy.types.Object]:
    import kenney_hero_lib as kh  # type: ignore

    props: list[bpy.types.Object] = []
    for ix in range(4):
        for iz in range(3):
            loc = (ix * TILE - 1.5, iz * TILE - 1.2, 0.0)
            ob = kh.import_kenney("building", "floor", loc, scale=1.0, palette_key="floor")
            if ob:
                props.append(ob)

    for i in range(5):
        ob = kh.import_kenney(
            "building",
            "wall",
            (i * TILE - 2.0, ROOM_D * 0.5, 0.0),
            rot_z_deg=0.0,
            palette_key="wall",
        )
        if ob:
            props.append(ob)

    for i in range(4):
        ob = kh.import_kenney(
            "building",
            "wall_side",
            (-ROOM_W * 0.5, i * TILE - 1.0, 0.0),
            rot_z_deg=90.0,
            palette_key="wall",
        )
        if ob:
            props.append(ob)

    door_wall = kh.import_kenney(
        "building",
        "doorway",
        (ROOM_W * 0.5, 0.25, 0.0),
        rot_z_deg=-90.0,
        palette_key="wall",
    )
    door = kh.import_kenney(
        "building",
        "door",
        (ROOM_W * 0.5, 0.35, 0.0),
        rot_z_deg=-90.0,
        palette_key="metal",
    )
    if door_wall:
        props.extend([door_wall, door] if door else [door_wall])

    desk = kh.import_kenney("furniture", "desk", (-0.35, -0.25, 0.0), rot_z_deg=8.0, palette_key="desk")
    chair = kh.import_kenney("furniture", "chair", (-0.1, -0.55, 0.0), rot_z_deg=-15.0, palette_key="plastic")
    screen = kh.import_kenney("furniture", "screen", (-0.32, -0.05, 0.0), rot_z_deg=8.0, palette_key="metal")
    kb = kh.import_kenney("furniture", "keyboard", (-0.28, -0.18, 0.0), rot_z_deg=8.0, palette_key="plastic")
    mouse = kh.import_kenney("furniture", "mouse", (-0.05, -0.2, 0.0), rot_z_deg=8.0, palette_key="trim")
    shelf = kh.import_kenney("furniture", "shelf", (1.05, -0.45, 0.0), rot_z_deg=-6.0, palette_key="desk")
    for ob in (desk, chair, screen, kb, mouse, shelf):
        if ob:
            props.append(ob)

    return props


def main() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    props = build_cutaway_garage()
    shell = [p for p in props if p.name.startswith("kenney_building")]
    interior = [p for p in props if not p.name.startswith("kenney_building") or "floor" in p.name]

    import blender_preview_rig as rig  # type: ignore

    out = PREVIEW / "garage-scene-urp-hero.png"
    PREVIEW.mkdir(parents=True, exist_ok=True)
    rig.render_preview_studio(
        props,
        out,
        resolution=1024,
        up_axis="Z",
        lens_mm=42.0,
        azimuth_deg=42.0,
        elevation_deg=30.0,
        framing_margin=1.02,
        mask_meshes=interior,
        hdri_path=HDRI if HDRI.is_file() else None,
        hdri_strength=1.0,
        hdri_rotation_deg=118.0,
        hdri_lighting_only=True,
    )
    print("OK garage-scene-urp-hero", out, "props", len(props))


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()