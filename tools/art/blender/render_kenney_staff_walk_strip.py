#!/usr/bin/env python3
"""Walk strip: textured hero, Kenney Run CC0, contact L/R gait frames."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy
from PIL import Image

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
from kenney_hero_walk_strip import (  # noqa: E402
    KENNEY_RUN_LICENSE,
    apply_walk_materials_to_hero,
    build_founder_walk_scene,
    build_researcher_walk_scene,
    configure_preview_env,
    founder_eye_preview_tweak,
    apply_idle_arm_blend,
)
from kenney_walk_cycle_gate import (  # noqa: E402
    fingerprints_match,
    material_fingerprint,
    verify_walk_cycle,
)


def main() -> int:
    if "--" not in sys.argv:
        print("FAIL walk_strip usage: -- <asset_id>", file=sys.stderr)
        return 1
    asset_id = sys.argv[sys.argv.index("--") + 1]
    if asset_id not in ("staff-founder-idle", "staff-researcher-idle"):
        print(f"FAIL walk_strip bad_asset={asset_id}")
        return 1
    configure_preview_env(asset_id)
    scene_data = (
        build_founder_walk_scene()
        if asset_id == "staff-founder-idle"
        else build_researcher_walk_scene()
    )
    scene = bpy.context.scene
    ok_w, msg_w = verify_walk_cycle(
        scene_data.arm,
        scene_data.body,
        frame_a=int(scene_data.walk_frames[0]),
        frame_b=int(scene_data.walk_frames[-1]),
        scene=scene,
        quarter_frames=scene_data.walk_frames,
        asset_id=asset_id,
        render_meshes=scene_data.render_meshes,
        idle_arm_quats=scene_data.idle_arm_quats,
        bind_arm_quats=scene_data.bind_arm_quats,
    )
    if not ok_w:
        print(f"FAIL {asset_id} walk_cycle {msg_w}")
        return 1
    print(
        f"OK {asset_id} walk_cycle {msg_w} "
        f"anim=Kenney/Run license={KENNEY_RUN_LICENSE} "
        f"frames={scene_data.walk_frames} labels={scene_data.labels}"
    )
    if asset_id == "staff-founder-idle":
        founder_eye_preview_tweak(scene_data.render_meshes)
    _pose_walk_frame(
        scene,
        scene_data.arm,
        scene_data.walk_frames[0],
        scene_data.idle_arm_quats,
        scene_data.bind_arm_quats,
    )
    apply_walk_materials_to_hero(scene_data.render_meshes, asset_id)
    mat_ref = material_fingerprint(scene_data.body)
    tiles: list[Image.Image] = []
    tmp_dir = Path(bpy.app.tempdir)
    for i, fr in enumerate(scene_data.walk_frames):
        _pose_walk_frame(
            scene,
            scene_data.arm,
            fr,
            scene_data.idle_arm_quats,
            scene_data.bind_arm_quats,
        )
        apply_walk_materials_to_hero(scene_data.render_meshes, asset_id)
        ok_fp, msg_fp = fingerprints_match(mat_ref, material_fingerprint(scene_data.body))
        if not ok_fp:
            print(f"FAIL {asset_id} walk_strip tile={i} {msg_fp}")
            return 1
        tile = tmp_dir / f"walk_{i}.png"
        bc.render_preview(
            scene_data.render_meshes,
            tile,
            asset_id=asset_id,
            mask_meshes=scene_data.render_meshes,
        )
        tiles.append(Image.open(tile).convert("RGB"))
    w, h = tiles[0].size
    gap = 8
    sheet = Image.new("RGB", (w * 4 + gap * 3, h), (150, 150, 150))
    for i, im in enumerate(tiles):
        sheet.paste(im, (i * (w + gap), 0))
    stem = (
        "staff-founder-walk-strip"
        if asset_id == "staff-founder-idle"
        else "staff-researcher-walk-strip"
    )
    out = bc.PREVIEW_ROOT / f"{stem}.png"
    out.parent.mkdir(parents=True, exist_ok=True)
    sheet.save(out)
    print(f"OK walk_strip wrote {out.relative_to(bc.REPO_ROOT)}")
    return 0


def _pose_walk_frame(scene, arm, frame: int, idle_arm_quats, bind_arm_quats) -> None:
    scene.frame_set(frame)
    apply_idle_arm_blend(arm, idle_arm_quats, bind_arm_quats)
    bpy.context.view_layer.update()
    dg = bpy.context.evaluated_depsgraph_get()
    dg.update()


if __name__ == "__main__":
    raise SystemExit(main())