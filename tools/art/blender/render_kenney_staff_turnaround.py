#!/usr/bin/env python3
"""Turnaround: front | left | back | right at idle + side mid-stride walk tile."""
from __future__ import annotations

import math
import os
import shutil
import sys
import tempfile
from pathlib import Path

import bpy
from mathutils import Matrix
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
YAWS = (0, 90, 180, 270)
LABELS = ("front", "left", "back", "right", "walk side")
GAP = 8


def _pose_walk(arm, frame, scene_data):
    bpy.context.scene.frame_set(frame)
    if scene_data.idle_arm_quats and scene_data.bind_arm_quats:
        apply_idle_arm_blend(arm, scene_data.idle_arm_quats, scene_data.bind_arm_quats)
    bpy.context.view_layer.update()


def _capture_turnaround_bases(
    arm: bpy.types.Object, meshes: list[bpy.types.Object]
) -> dict[str, Matrix]:
    obs = [arm, *[m for m in meshes if m.type == "MESH"]]
    return {ob.name: ob.matrix_world.copy() for ob in obs}


def _rotate_character_root(
    arm: bpy.types.Object,
    meshes: list[bpy.types.Object],
    bases: dict[str, Matrix],
    char_yaw_deg: float,
) -> None:
    """World-Z spin about armature root (fixed camera sees true yaw panels)."""
    pivot = bases[arm.name].translation.copy()
    rot = Matrix.Rotation(math.radians(char_yaw_deg), 4, "Z")
    for ob in [arm, *[m for m in meshes if m.type == "MESH"]]:
        base = bases[ob.name].copy()
        base.translation -= pivot
        ob.matrix_world = rot @ base
        ob.matrix_world.translation += pivot
    bpy.context.view_layer.update()


def _render_tile(
    arm: bpy.types.Object,
    meshes: list,
    asset_id: str,
    char_yaw_deg: float,
    bases: dict[str, Matrix],
    out: Path,
    *,
    walk_pose: bool,
    scene_data,
) -> None:
    os.environ["STAFF_PREVIEW_YAW_DEG"] = os.environ.get("STAFF_TURNAROUND_CAM_YAW", "30")
    os.environ["STAFF_PREVIEW_ELEV_DEG"] = "10"
    _rotate_character_root(arm, meshes, bases, char_yaw_deg)
    if walk_pose and scene_data:
        _pose_walk(scene_data.arm, scene_data.walk_frames[1], scene_data)
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

    arm = scene_data.arm
    meshes = scene_data.render_meshes
    bases = _capture_turnaround_bases(arm, meshes)
    os.environ.setdefault("STAFF_TURNAROUND_CAM_YAW", "30")

    stem = (
        "staff-founder-turnaround"
        if asset_id == "staff-founder-idle"
        else "staff-researcher-turnaround"
    )
    tmp = Path(tempfile.mkdtemp(prefix="turnaround_"))
    tiles: list[Image.Image] = []
    panel_mask_paths: list[Path] = []
    for idx, char_yaw in enumerate(YAWS):
        tile = tmp / f"idle_{char_yaw}.png"
        if idx == 0:
            os.environ.pop("STAFF_PREVIEW_LOCK_CAMERA", None)
        else:
            os.environ["STAFF_PREVIEW_LOCK_CAMERA"] = "1"
        _render_tile(
            arm,
            meshes,
            asset_id,
            float(char_yaw),
            bases,
            tile,
            walk_pose=False,
            scene_data=None,
        )
        tiles.append(Image.open(tile).convert("RGB"))
        mask_src = tile.with_name(tile.stem + ".mask.png")
        panel_mask_paths.append(mask_src if mask_src.is_file() else None)
    walk_tile = tmp / "walk_side.png"
    _render_tile(
        arm,
        meshes,
        asset_id,
        90.0,
        bases,
        walk_tile,
        walk_pose=True,
        scene_data=scene_data,
    )
    tiles.append(Image.open(walk_tile).convert("RGB"))
    _rotate_character_root(arm, meshes, bases, 0.0)

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