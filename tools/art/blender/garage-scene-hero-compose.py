#!/usr/bin/env python3
"""Furnished cut-away garage hero — Kenney CC0 shell on grid, EarlyGame PBR, Z-up."""
from __future__ import annotations

import json
import math
import sys
from pathlib import Path

import bpy

REPO = Path(__file__).resolve().parents[3]
PREVIEW = REPO / "docs/art/previews"
HDRI = REPO / "tools/art/cc0/incoming/brown_photostudio_02_1k.exr"

TILE = 2.0
COLS = 3
ROWS = 2
ROOM_W = COLS * TILE
ROOM_D = ROWS * TILE
WALL_X = 0.0
WALL_Y = 0.0
FLOOR_Z = 0.0

WALL_MOUNT_KEYS = {"building_door"}
CEILING_MOUNT_KEYS = {"lamp_ceiling"}
DESK_MOUNT_KEYS = {"computerScreen", "computerKeyboard", "computerMouse", "_screen", "_keyboard", "_mouse"}


def _import(kh, pack: str, asset: str, loc: tuple[float, float, float], **kw):
    ob = kh.import_kenney(pack, asset, loc, up_axis="Z", **kw)
    return ob


def _pbr_shell(gk, meshes: list[bpy.types.Object]) -> None:
    for ob in meshes:
        if "floor" in ob.name:
            gk.assign_pbr(ob, "garage-floor-tile")
        elif "corner" in ob.name:
            gk.assign_pbr(ob, "garage-exterior-wall")
        elif "doorway" in ob.name or "wall" in ob.name:
            gk.assign_pbr(ob, "garage-wall-panel" if "doorway" in ob.name else "garage-exterior-wall")


def build_room(kh) -> list[bpy.types.Object]:
    meshes: list[bpy.types.Object] = []
    for ix in range(COLS):
        for iy in range(ROWS):
            ob = _import(
                kh,
                "building",
                "floor",
                (TILE * 0.5 + ix * TILE, TILE * 0.5 + iy * TILE, FLOOR_Z),
                palette_key="floor",
            )
            if ob:
                meshes.append(ob)
    for iy in range(ROWS):
        ob = _import(
            kh,
            "building",
            "wall",
            (WALL_X, TILE * 0.5 + iy * TILE, 0.0),
            rot_z_deg=90.0,
            palette_key="wall",
        )
        if ob:
            meshes.append(ob)
    for ix in range(COLS):
        ob = _import(
            kh,
            "building",
            "wall",
            (TILE * 0.5 + ix * TILE, WALL_Y, 0.0),
            rot_z_deg=0.0,
            palette_key="wall",
        )
        if ob:
            meshes.append(ob)
    corner = _import(kh, "building", "corner", (WALL_X, WALL_Y, 0.0), palette_key="wall")
    if corner:
        meshes.append(corner)
    doorway = _import(
        kh,
        "building",
        "doorway",
        (TILE * 1.5, WALL_Y, 0.0),
        palette_key="wall",
    )
    if doorway:
        meshes.append(doorway)
    door = _import(
        kh,
        "building",
        "door",
        (ROOM_W * 0.5 - TILE * 0.35, WALL_Y + 0.08, 0.0),
        palette_key="metal",
    )
    if door:
        meshes.append(door)
    return meshes


def build_furniture(kh) -> list[bpy.types.Object]:
    props: list[bpy.types.Object] = []
    bench = _import(
        kh, "furniture", "workbench", (WALL_X + 0.65, ROOM_D - TILE * 0.55, FLOOR_Z), palette_key="desk"
    )
    if bench:
        props.append(bench)
    shelf = _import(
        kh, "furniture", "server_rack", (WALL_X + 0.55, TILE * 0.45, FLOOR_Z), palette_key="metal"
    )
    if shelf:
        props.append(shelf)
    low_shelf = _import(
        kh, "furniture", "shelf", (ROOM_W - TILE * 0.85, WALL_Y + 0.55, FLOOR_Z), palette_key="desk"
    )
    if low_shelf:
        props.append(low_shelf)
    box_a = _import(
        kh, "furniture", "box_closed", (ROOM_W - TILE * 0.45, TILE * 0.35, FLOOR_Z), palette_key="cardboard"
    )
    box_b = _import(
        kh, "furniture", "box_open", (ROOM_W - TILE * 0.35, TILE * 0.85, FLOOR_Z), palette_key="cardboard"
    )
    for b in (box_a, box_b):
        if b:
            props.append(b)
    desk = _import(kh, "furniture", "desk", (TILE * 1.05, ROOM_D - TILE * 0.75, FLOOR_Z), palette_key="desk")
    if desk:
        props.append(desk)
    chair = _import(
        kh, "furniture", "chair", (TILE * 1.55, ROOM_D - TILE * 0.95, FLOOR_Z), rot_z_deg=-18.0, palette_key="plastic"
    )
    if chair:
        props.append(chair)
    screen = _import(
        kh,
        "furniture",
        "screen",
        (TILE * 1.05, ROOM_D - TILE * 0.75, FLOOR_Z + 0.72),
        palette_key="screen",
    )
    if screen:
        props.append(screen)
    kb = _import(
        kh,
        "furniture",
        "keyboard",
        (TILE * 1.02, ROOM_D - TILE * 0.62, FLOOR_Z + 0.74),
        palette_key="plastic",
    )
    if kb:
        props.append(kb)
    mouse = _import(
        kh,
        "furniture",
        "mouse",
        (TILE * 1.42, ROOM_D - TILE * 0.62, FLOOR_Z + 0.74),
        palette_key="plastic",
    )
    if mouse:
        props.append(mouse)
    lamp = kh.import_kenney(
        "furniture",
        "lamp",
        (ROOM_W * 0.5, ROOM_D * 0.52, 2.28),
        scale=(3.2, 3.2, 0.35),
        palette_key="trim",
        up_axis="Z",
        floor_snap=False,
    )
    if lamp:
        lamp.name = "kenney_furniture_lamp_ceiling"
        lamp.rotation_euler[0] = math.pi
        bpy.context.view_layer.update()
        props.append(lamp)
    return props


def main() -> None:
    import garage_kit_lib as gk  # type: ignore
    import kenney_hero_lib as kh  # type: ignore

    bpy.ops.wm.read_factory_settings(use_empty=True)
    shell = build_room(kh)
    props = build_furniture(kh)
    all_meshes = shell + props
    kh.purge_import_orphans(all_meshes)
    _pbr_shell(gk, shell)
    mat_errs = kh.audit_materials(all_meshes)
    pbr_errs = gk.audit_pbr([o for o in shell if o.name.startswith("kenney_building")])
    gnd_errs = kh.audit_grounding(
        all_meshes,
        up_axis="Z",
        floor=FLOOR_Z,
        wall_mounted=WALL_MOUNT_KEYS,
        ceiling_mounted=CEILING_MOUNT_KEYS,
        exempt=DESK_MOUNT_KEYS,
        wall_plane="side",
        wall_coord=WALL_Y,
        tol_wall=0.28,
    )
    if mat_errs or pbr_errs:
        raise RuntimeError(f"material audit: {mat_errs[:3]} pbr: {pbr_errs[:3]}")
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
        lens_mm=42.0,
        azimuth_deg=42.0,
        elevation_deg=30.0,
        framing_margin=0.90,
        mask_meshes=all_meshes,
        hdri_path=HDRI if HDRI.is_file() else None,
        hdri_strength=0.58,
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
        wall_mounted=WALL_MOUNT_KEYS,
        ceiling_mounted=CEILING_MOUNT_KEYS,
        exempt=DESK_MOUNT_KEYS,
        wall_plane="side",
        wall_coord=WALL_Y,
        tol_wall=0.28,
    )
    meta_path.write_text(json.dumps(meta, indent=2) + "\n", encoding="utf-8")
    print("OK garage-scene-urp-hero", out, "meshes", len(all_meshes))


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()