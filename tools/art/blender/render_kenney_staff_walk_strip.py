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
    bake_evaluated_render_meshes,
    build_founder_walk_scene,
    build_researcher_walk_scene,
    configure_preview_env,
    dispose_baked_render_meshes,
    founder_eye_preview_tweak,
    ensure_locomotion_action,
    measure_walk_frame_pose,
    pose_walk_for_render,
    researcher_lab_coat_preview_tweak,
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
    pose_walk_for_render(scene, scene_data, int(scene_data.walk_frames[0]))
    apply_walk_materials_to_hero(scene_data.render_meshes, asset_id)
    if asset_id == "staff-founder-idle":
        founder_eye_preview_tweak(scene_data.render_meshes)
    else:
        researcher_lab_coat_preview_tweak(scene_data.render_meshes)
    mat_ref = material_fingerprint(scene_data.body)
    tiles: list[Image.Image] = []
    tmp_dir = Path(bpy.app.tempdir)
    ensure_locomotion_action(scene_data.arm, scene_data.action_name)
    use_pose_bake = asset_id == "staff-researcher-idle"
    for i, fr in enumerate(scene_data.walk_frames):
        label = scene_data.labels[i] if i < len(scene_data.labels) else f"tile_{i}"
        pose_walk_for_render(scene, scene_data, int(fr))
        metrics = measure_walk_frame_pose(scene_data, int(fr), repose=False)
        if asset_id == "staff-researcher-idle":
            print(
                f"OK researcher_walk_render frame={int(fr)} label={label} "
                f"ankle_x={metrics['ankle_x']:.4f} "
                f"knee_L={metrics['knee_l']:.2f} knee_R={metrics['knee_r']:.2f} "
                f"arm_swing_L={metrics['arm_swing_l']:.2f} arm_swing_R={metrics['arm_swing_r']:.2f} "
                f"shoulder_L={metrics['shoulder_swing_l']:.2f} "
                f"shoulder_R={metrics['shoulder_swing_r']:.2f}"
            )
        apply_walk_materials_to_hero(scene_data.render_meshes, asset_id)
        if asset_id == "staff-researcher-idle":
            researcher_lab_coat_preview_tweak(scene_data.render_meshes)
        elif asset_id == "staff-founder-idle":
            founder_eye_preview_tweak(scene_data.render_meshes)
        bpy.context.view_layer.update()
        bpy.context.evaluated_depsgraph_get().update()
        if use_pose_bake:
            from researcher_scene_mesh_audit import (
                assert_render_island_audit,
                audit_researcher_render_meshes,
            )

            label = scene_data.labels[i] if i < len(scene_data.labels) else f"tile_{i}"
            audit_researcher_render_meshes(
                scene_data.body,
                scene_data.render_meshes,
                arm=scene_data.arm,
            )
            ok_r, msg_r = assert_render_island_audit(
                scene_data.body,
                scene_data.render_meshes,
                frame=int(fr),
                panel=label,
            )
            if not ok_r:
                print(f"FAIL staff-researcher-idle {msg_r}")
                return 1
            print(f"OK researcher_render_islands {msg_r}")
        ok_fp, msg_fp = fingerprints_match(mat_ref, material_fingerprint(scene_data.body))
        if not ok_fp:
            print(f"FAIL {asset_id} walk_strip tile={i} {msg_fp}")
            return 1
        tile = tmp_dir / f"walk_{i}.png"
        render_meshes = scene_data.render_meshes
        cleanup: list = []
        if use_pose_bake:
            try:
                render_meshes, cleanup = bake_evaluated_render_meshes(scene_data.render_meshes)
            except Exception as exc:  # noqa: BLE001
                print(f"WARN staff-researcher-idle walk_bake {exc}")
                render_meshes = scene_data.render_meshes
                cleanup = []
        try:
            bc.render_preview(
                render_meshes,
                tile,
                asset_id=asset_id,
                mask_meshes=render_meshes,
            )
        finally:
            if cleanup:
                dispose_baked_render_meshes(scene_data.render_meshes, cleanup)
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


if __name__ == "__main__":
    raise SystemExit(main())