#!/usr/bin/env python3
"""Run cycle strip: 0/25/50/75% frames, 3/4 view on #969696 studio grey."""
from __future__ import annotations

import os
import sys
from pathlib import Path

import bpy
from PIL import Image

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import kenney_founder_lib as kf  # noqa: E402
from kenney_walk_cycle_gate import pick_run_action, verify_walk_cycle  # noqa: E402

RUN_FBX = (
    kf.INCOMING / "kenney/animated-characters-3/Animations/run.fbx"
)
LABELS = ("0%", "25%", "50%", "75%")


def _apply_hero_materials(asset_id: str, mesh: bpy.types.Object) -> None:
    if asset_id == "staff-researcher-idle":
        from kenney_researcher_stock_recolor import apply_researcher_materials

        apply_researcher_materials(mesh)
    else:
        from kenney_founder_stock_recolor import apply_founder_materials

        apply_founder_materials(mesh)


def main() -> int:
    if "--" not in sys.argv:
        print("FAIL walk_strip usage: -- <asset_id>", file=sys.stderr)
        return 1
    asset_id = sys.argv[sys.argv.index("--") + 1]
    if asset_id not in ("staff-founder-idle", "staff-researcher-idle"):
        print(f"FAIL walk_strip bad_asset={asset_id}")
        return 1
    if not RUN_FBX.is_file():
        print(f"FAIL walk_strip missing {RUN_FBX}")
        return 1
    arm, mesh = kf._import_rigged()
    kf._ground_rig(arm, mesh)
    if asset_id == "staff-researcher-idle":
        from kenney_researcher_stock_recolor import _pose_researcher_idle_arms

        _pose_researcher_idle_arms(arm)
    else:
        kf._pose_kenney_idle_arms(arm)
    _apply_hero_materials(asset_id, mesh)
    kf._import_fbx(str(RUN_FBX))
    action = pick_run_action()
    if not action:
        print("FAIL walk_strip no_run_action")
        return 1
    arm.animation_data_create()
    arm.animation_data.action = action
    start, end = (int(action.frame_range[0]), int(action.frame_range[1]))
    span = max(1, end - start)
    frame_ids = [start, start + span // 4, start + span // 2, start + (3 * span) // 4]
    scene = bpy.context.scene
    scene.frame_start = start
    scene.frame_end = end
    ok_w, msg_w = verify_walk_cycle(
        arm,
        mesh,
        frame_a=start,
        frame_b=end,
        scene=scene,
        quarter_frames=frame_ids,
    )
    if not ok_w:
        print(f"FAIL {asset_id} walk_cycle {msg_w}")
        return 1
    print(f"OK {asset_id} walk_cycle {msg_w}")
    os.environ["EARLY_ART_PREVIEW_RIG"] = "1"
    os.environ["STAFF_PREVIEW_YAW_DEG"] = "30"
    os.environ["STAFF_PREVIEW_ELEV_DEG"] = "10"
    os.environ["STAFF_PREVIEW_MARGIN"] = "1.44"
    if asset_id == "staff-founder-idle":
        os.environ["STAFF_PREVIEW_FOUNDER_LIGHT"] = os.environ.get(
            "STAFF_PREVIEW_FOUNDER_LIGHT", "0.305"
        )
    tiles: list[Image.Image] = []
    tmp_dir = Path(bpy.app.tempdir)
    for i, fr in enumerate(frame_ids):
        scene.frame_set(fr)
        bpy.context.view_layer.update()
        tile = tmp_dir / f"walk_{i}.png"
        bc.render_preview([mesh, arm], tile, asset_id=asset_id, mask_meshes=[mesh])
        tiles.append(Image.open(tile).convert("RGB"))
    w, h = tiles[0].size
    gap = 8
    sheet = Image.new("RGB", (w * 4 + gap * 3, h), (150, 150, 150))
    for i, im in enumerate(tiles):
        sheet.paste(im, (i * (w + gap), 0))
    out = bc.PREVIEW_ROOT / f"{asset_id.replace('-idle', '')}-walk-strip.png"
    if asset_id == "staff-founder-idle":
        out = bc.PREVIEW_ROOT / "staff-founder-walk-strip.png"
    else:
        out = bc.PREVIEW_ROOT / "staff-researcher-walk-strip.png"
    out.parent.mkdir(parents=True, exist_ok=True)
    sheet.save(out)
    print(f"OK walk_strip wrote {out.relative_to(bc.REPO_ROOT)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())