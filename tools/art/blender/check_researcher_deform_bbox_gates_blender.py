#!/usr/bin/env python3
"""FAIL #21: coat/part verts must stay within body rest bbox + 0.15m on walk frames."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
from kenney_hero_walk_strip import (  # noqa: E402
    apply_idle_arm_blend,
    build_researcher_walk_scene,
    configure_preview_env,
)

MAX_OUTSIDE_M = 0.15


def _world_verts(ob: bpy.types.Object) -> list[Vector]:
    deps = bpy.context.evaluated_depsgraph_get()
    ev = ob.evaluated_get(deps)
    return [ev.matrix_world @ v.co for v in ev.data.vertices]


def _bbox(points: list[Vector]) -> tuple[Vector, Vector]:
    xs = [p.x for p in points]
    ys = [p.y for p in points]
    zs = [p.z for p in points]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))


def _outside(lo: Vector, hi: Vector, co: Vector) -> float:
    out = 0.0
    for i in range(3):
        if co[i] < lo[i] - MAX_OUTSIDE_M:
            out = max(out, lo[i] - MAX_OUTSIDE_M - co[i])
        if co[i] > hi[i] + MAX_OUTSIDE_M:
            out = max(out, co[i] - (hi[i] + MAX_OUTSIDE_M))
    return out


def main() -> int:
    configure_preview_env("staff-researcher-idle")
    scene_data = build_researcher_walk_scene()
    scene = bpy.context.scene
    body = scene_data.body
    scene.frame_set(scene_data.walk_frames[0])
    apply_idle_arm_blend(
        scene_data.arm, scene_data.idle_arm_quats, scene_data.bind_arm_quats
    )
    bpy.context.view_layer.update()
    lo, hi = _bbox(_world_verts(body))
    worst = 0.0
    worst_name = ""
    worst_fr = 0
    for fr in scene_data.walk_frames:
        scene.frame_set(fr)
        apply_idle_arm_blend(
            scene_data.arm, scene_data.idle_arm_quats, scene_data.bind_arm_quats
        )
        bpy.context.view_layer.update()
        deps = bpy.context.evaluated_depsgraph_get()
        deps.update()
        for ob in scene_data.render_meshes:
            if ob.type != "MESH":
                continue
            for co in _world_verts(ob):
                d = _outside(lo, hi, co)
                if d > worst:
                    worst = d
                    worst_name = ob.name.split(".")[0]
                    worst_fr = fr
    if worst > 1e-4:
        print(
            f"FAIL staff-researcher-idle deform_bbox outside={worst:.4f}m "
            f"mesh={worst_name} frame={worst_fr} max={MAX_OUTSIDE_M}"
        )
        return 1
    print(
        f"OK staff-researcher-idle deform_bbox worst={worst:.4f}m "
        f"frames={scene_data.walk_frames}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())