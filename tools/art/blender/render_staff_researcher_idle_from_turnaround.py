#!/usr/bin/env python3
"""Idle hero PNG from turnaround front tile (Q478 — same rig/materials as turnaround)."""
from __future__ import annotations

import os
import sys
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
from kenney_hero_walk_strip import (  # noqa: E402
    apply_idle_arm_blend,
    apply_walk_materials_to_hero,
    build_researcher_walk_scene,
    configure_preview_env,
    ensure_locomotion_action,
    researcher_lab_coat_preview_tweak,
)
from render_kenney_staff_turnaround import (  # noqa: E402
    _effective_cam_yaw,
    _pose_idle,
    _render_tile,
)

PREVIEW = Path(__file__).resolve().parents[3] / "docs/art/previews"
ASSET_ID = "staff-researcher-idle"


def main() -> int:
    configure_preview_env(ASSET_ID)
    os.environ.pop("STAFF_PREVIEW_SKIP_MASK", None)
    os.environ.pop("STAFF_PREVIEW_LOCK_CAMERA", None)
    scene_data = build_researcher_walk_scene()
    ensure_locomotion_action(scene_data.arm, scene_data.action_name)
    apply_walk_materials_to_hero(scene_data.render_meshes, ASSET_ID)
    researcher_lab_coat_preview_tweak(scene_data.render_meshes)
    apply_idle_arm_blend(
        scene_data.arm, scene_data.idle_arm_quats, scene_data.bind_arm_quats
    )
    bpy.context.view_layer.update()
    out = PREVIEW / f"{ASSET_ID}.png"
    yaw = _effective_cam_yaw(ASSET_ID, 0.0)
    _render_tile(
        scene_data.render_meshes,
        ASSET_ID,
        yaw,
        out,
        walk_pose=False,
        scene_data=scene_data,
    )
    print(f"OK idle_hero_from_turnaround {out.relative_to(PREVIEW.parents[2])} yaw={yaw}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())