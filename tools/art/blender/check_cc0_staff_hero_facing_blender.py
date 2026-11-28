#!/usr/bin/env python3
"""Facing gate for founder + researcher (head/chest/toes/knees/travel vs toe-forward)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
from kenney_hero_walk_strip import (  # noqa: E402
    build_founder_walk_scene,
    build_researcher_walk_scene,
    configure_preview_env,
)
from kenney_face_axis import toe_forward  # noqa: E402
from kenney_staff_facing_metrics import (  # noqa: E402
    gate_facing_idle,
    gate_facing_walk,
    measure_facing,
    walk_travel_forward,
)


def _run_asset(asset_id: str) -> tuple[bool, dict]:
    configure_preview_env(asset_id)
    scene_data = (
        build_founder_walk_scene()
        if asset_id == "staff-founder-idle"
        else build_researcher_walk_scene()
    )
    scene = bpy.context.scene
    frames = scene_data.walk_frames
    scene.frame_set(frames[0])
    bpy.context.view_layer.update()
    ref = toe_forward(scene_data.render_meshes).normalized()
    idle_report = measure_facing(
        scene_data.render_meshes,
        scene_data.arm,
        scene_data.body,
        travel=ref,
        travel_motion=ref,
    )
    scene.frame_set(frames[1])
    bpy.context.view_layer.update()
    motion = walk_travel_forward(
        scene_data.arm, frames[0], frames[2], scene, body=scene_data.body
    )
    walk_report = measure_facing(
        scene_data.render_meshes,
        scene_data.arm,
        scene_data.body,
        travel=ref,
        travel_motion=motion,
    )
    ok_idle, msg_idle = gate_facing_idle(idle_report)
    ok_walk, msg_walk = gate_facing_walk(walk_report)
    ok = ok_idle and ok_walk
    payload = {
        "asset_id": asset_id,
        "idle": idle_report.as_dict(),
        "walk_mid_stride": walk_report.as_dict(),
        "gate_idle": msg_idle,
        "gate_walk": msg_walk,
    }
    return ok, payload


def main() -> int:
    results: dict[str, dict] = {}
    rc = 0
    for asset_id in ("staff-founder-idle", "staff-researcher-idle"):
        ok, payload = _run_asset(asset_id)
        results[asset_id] = payload
        tag = "OK" if ok else "FAIL"
        print(
            f"{tag} {asset_id} facing idle_max={payload['idle']['max_delta_deg']} "
            f"walk_max={payload['walk_mid_stride']['max_delta_deg']} "
            f"{payload['gate_idle']}/{payload['gate_walk']}"
        )
        if not ok:
            rc = 1
    out = Path(__file__).resolve().parents[3] / "docs/art/previews/staff-hero-facing-metrics.json"
    out.write_text(json.dumps(results, indent=2) + "\n")
    print(f"OK facing_metrics wrote {out.relative_to(Path(__file__).resolve().parents[3])}")
    return rc


if __name__ == "__main__":
    raise SystemExit(main())