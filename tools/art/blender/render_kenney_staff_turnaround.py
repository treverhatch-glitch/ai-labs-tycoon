#!/usr/bin/env python3
"""Turnaround: front | left | back | right at idle + side mid-stride walk tile."""
from __future__ import annotations

import os
import shutil
import sys
import tempfile
from pathlib import Path

import bpy
from PIL import Image, ImageDraw

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
from kenney_hero_walk_strip import (  # noqa: E402
    apply_walk_materials_to_hero,
    build_founder_walk_scene,
    build_researcher_walk_scene,
    configure_preview_env,
    founder_eye_preview_tweak,
    researcher_lab_coat_preview_tweak,
    apply_idle_arm_blend,
)

GREY = (150, 150, 150)
CAM_YAWS = (0, 90, 180, 270)
LABELS = ("front", "left", "back", "right", "walk side")
GAP = 8


def _pose_walk(arm, frame, scene_data):
    bpy.context.scene.frame_set(frame)
    if scene_data.idle_arm_quats and scene_data.bind_arm_quats:
        apply_idle_arm_blend(arm, scene_data.idle_arm_quats, scene_data.bind_arm_quats)
    bpy.context.view_layer.update()


def _pose_idle(scene_data) -> None:
    bpy.context.scene.frame_set(bpy.context.scene.frame_start)
    apply_idle_arm_blend(
        scene_data.arm, scene_data.idle_arm_quats, scene_data.bind_arm_quats
    )
    bpy.context.view_layer.update()


def _effective_cam_yaw(asset_id: str, cam_yaw_deg: float) -> float:
    """True Z orbit; founder straight-on hides eye meshes — tiny yaw for panel 0."""
    if asset_id == "staff-founder-idle" and cam_yaw_deg == 0.0:
        return 10.0
    return cam_yaw_deg


def _render_tile(
    meshes: list,
    asset_id: str,
    cam_yaw_deg: float,
    out: Path,
    *,
    walk_pose: bool,
    scene_data,
) -> None:
    os.environ["STAFF_PREVIEW_YAW_DEG"] = str(_effective_cam_yaw(asset_id, cam_yaw_deg))
    os.environ["STAFF_PREVIEW_ELEV_DEG"] = "10"
    os.environ.pop("STAFF_PREVIEW_LOCK_CAMERA", None)
    apply_walk_materials_to_hero(meshes, asset_id)
    if asset_id == "staff-founder-idle":
        founder_eye_preview_tweak(meshes)
    else:
        researcher_lab_coat_preview_tweak(meshes)
    if scene_data and scene_data.arm:
        if walk_pose:
            _pose_walk(scene_data.arm, scene_data.walk_frames[0], scene_data)
        else:
            _pose_idle(scene_data)
    bpy.context.view_layer.update()
    bc.render_preview(meshes, out, asset_id=asset_id, mask_meshes=meshes)


def main() -> int:
    if "--" not in sys.argv:
        print("FAIL turnaround usage: -- <staff-founder-idle|staff-researcher-idle>", file=sys.stderr)
        return 1
    asset_id = sys.argv[sys.argv.index("--") + 1]
    if asset_id not in ("staff-founder-idle", "staff-researcher-idle"):
        print(f"FAIL turnaround bad_asset={asset_id}")
        return 1
    configure_preview_env(asset_id)
    os.environ.pop("STAFF_PREVIEW_SKIP_MASK", None)
    os.environ.pop("STAFF_PREVIEW_LOCK_CAMERA", None)
    scene_data = (
        build_founder_walk_scene()
        if asset_id == "staff-founder-idle"
        else build_researcher_walk_scene()
    )
    apply_walk_materials_to_hero(scene_data.render_meshes, asset_id)
    if asset_id == "staff-founder-idle":
        founder_eye_preview_tweak(scene_data.render_meshes)
    else:
        researcher_lab_coat_preview_tweak(scene_data.render_meshes)

    scene = bpy.context.scene
    scene.frame_set(scene.frame_start)
    apply_idle_arm_blend(
        scene_data.arm, scene_data.idle_arm_quats, scene_data.bind_arm_quats
    )
    bpy.context.view_layer.update()
    meshes = scene_data.render_meshes
    stem = (
        "staff-founder-turnaround"
        if asset_id == "staff-founder-idle"
        else "staff-researcher-turnaround"
    )
    tmp = Path(tempfile.mkdtemp(prefix="turnaround_"))
    tiles: list[Image.Image] = []
    panel_mask_paths: list[Path] = []
    for cam_yaw in CAM_YAWS:
        tile = tmp / f"idle_{cam_yaw}.png"
        _render_tile(
            meshes,
            asset_id,
            float(cam_yaw),
            tile,
            walk_pose=False,
            scene_data=scene_data,
        )
        tiles.append(Image.open(tile).convert("RGB"))
        mask_src = tile.with_name(tile.stem + ".mask.png")
        panel_mask_paths.append(mask_src if mask_src.is_file() else None)
    walk_tile = tmp / "walk_side.png"
    _render_tile(
        meshes,
        asset_id,
        90.0,
        walk_tile,
        walk_pose=True,
        scene_data=scene_data,
    )
    tiles.append(Image.open(walk_tile).convert("RGB"))
    walk_mask = walk_tile.with_name(walk_tile.stem + ".mask.png")
    if walk_mask.is_file():
        panel_mask_paths.append(walk_mask)

    w, h = tiles[0].size
    sheet_w = w * len(tiles) + GAP * (len(tiles) - 1)
    sheet = Image.new("RGB", (sheet_w, h), GREY)
    for i, im in enumerate(tiles):
        sheet.paste(im, (i * (w + GAP), 0))
    draw = ImageDraw.Draw(sheet)
    for i, label in enumerate(LABELS):
        x = i * (w + GAP) + 8
        draw.text((x, 8), label, fill=(40, 40, 40))
    out = bc.PREVIEW_ROOT / f"{stem}.png"
    out.parent.mkdir(parents=True, exist_ok=True)
    for i, msrc in enumerate(panel_mask_paths):
        if msrc is not None and msrc.is_file():
            shutil.copy2(msrc, out.parent / f"{stem}.panel{i}.mask.png")
    sheet.save(out)
    print(f"OK turnaround wrote {out.relative_to(bc.REPO_ROOT)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())