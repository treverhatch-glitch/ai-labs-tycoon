#!/usr/bin/env python3
"""Cut-away garage hero — Kenney CC0 shell + startup props, Z-up floor."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy

REPO = Path(__file__).resolve().parents[3]
PREVIEW = REPO / "docs/art/previews"
HDRI = REPO / "tools/art/cc0/incoming/brown_photostudio_02_1k.exr"

ROOM_W = 4.0
ROOM_D = 3.0


def build_cutaway_garage() -> list[bpy.types.Object]:
    import kenney_hero_lib as kh  # type: ignore

    props: list[bpy.types.Object] = []

    floor = kh.import_kenney(
        "building",
        "floor",
        (ROOM_W * 0.5, ROOM_D * 0.5, 0.0),
        scale=(ROOM_W * 0.5, ROOM_D * 0.5, 1.0),
        palette_key="floor",
    )
    if floor:
        props.append(floor)

    corner = kh.import_kenney("building", "corner", (0.0, ROOM_D, 0.0), palette_key="wall")
    if corner:
        props.append(corner)

    for i, x in enumerate((1.0, 2.4, 3.6)):
        ob = kh.import_kenney("building", "wall", (x, ROOM_D, 0.0), rot_z_deg=90.0, palette_key="wall")
        if ob:
            props.append(ob)

    for y in (2.0, 1.0):
        ob = kh.import_kenney("building", "wall", (0.0, y, 0.0), rot_z_deg=0.0, palette_key="wall")
        if ob:
            props.append(ob)

    door_frame = kh.import_kenney(
        "building",
        "doorway",
        (ROOM_W, ROOM_D * 0.55, 0.0),
        rot_z_deg=0.0,
        palette_key="wall",
    )
    roll_door = kh.import_kenney(
        "building",
        "door",
        (ROOM_W - 0.05, ROOM_D * 0.55, 0.0),
        rot_z_deg=90.0,
        palette_key="metal",
    )
    for ob in (door_frame, roll_door):
        if ob:
            props.append(ob)

    prop_specs = [
        ("furniture", "workbench", (1.0, 0.9), 0.0, 1.15, "desk"),
        ("furniture", "desk", (2.0, 1.6), 8.0, 1.0, "desk"),
        ("furniture", "chair", (1.75, 1.35), -12.0, 1.0, "plastic"),
        ("furniture", "screen", (2.25, 1.62), 8.0, 1.0, "metal"),
        ("furniture", "keyboard", (2.12, 1.48), 8.0, 1.0, "plastic"),
        ("furniture", "mouse", (1.95, 1.42), 8.0, 1.0, "trim"),
        ("furniture", "shelf", (0.65, 1.2), -8.0, 1.0, "desk"),
        ("furniture", "server_rack", (3.35, 2.35), 6.0, 1.0, "metal"),
        ("furniture", "box_closed", (3.1, 0.75), 15.0, 0.9, "cardboard"),
        ("furniture", "box_open", (2.75, 0.55), -10.0, 0.85, "cardboard"),
        ("furniture", "lamp", (2.35, 2.15), 0.0, 1.0, "trim"),
    ]
    for pack, asset, (x, y), rot, sc, pal in prop_specs:
        ob = kh.import_kenney(pack, asset, (x, y, 0.0), rot_z_deg=rot, scale=sc, palette_key=pal)
        if ob:
            props.append(ob)

    errs = kh.audit_materials(props)
    if errs:
        for e in errs:
            print("WARN material", e)
        raise RuntimeError(f"material audit failed: {len(errs)} issues")
    return props


def main() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    props = build_cutaway_garage()
    keep = {p.name for p in props}
    for ob in list(bpy.data.objects):
        if ob.name not in keep and ob.type != "CAMERA":
            bpy.data.objects.remove(ob, do_unlink=True)
    room_shell = [p for p in props if p.name.startswith("kenney_building")]
    import kenney_hero_lib as kh  # type: ignore

    kh.purge_import_orphans(props)

    import blender_preview_rig as rig  # type: ignore

    out = PREVIEW / "garage-scene-urp-hero.png"
    PREVIEW.mkdir(parents=True, exist_ok=True)
    rig.render_preview_studio(
        room_shell + props,
        out,
        resolution=1024,
        up_axis="Z",
        lens_mm=42.0,
        azimuth_deg=42.0,
        elevation_deg=30.0,
        framing_margin=1.15,
        mask_meshes=room_shell + props,
        hdri_path=HDRI if HDRI.is_file() else None,
        hdri_strength=1.0,
        hdri_rotation_deg=118.0,
        hdri_lighting_only=True,
    )
    print("OK garage-scene-urp-hero", out, "props", len(props))


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()