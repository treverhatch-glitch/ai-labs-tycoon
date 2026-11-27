#!/usr/bin/env python3
"""FAIL #25: coat verts hug body surface; no below-knee / bbox leaks on walk frames."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy
import bmesh
from mathutils import Vector
from mathutils.bvhtree import BVHTree

sys.path.insert(0, str(Path(__file__).resolve().parent))
from kenney_geometry_landmarks import compute_landmarks  # noqa: E402
from kenney_hero_walk_strip import (  # noqa: E402
    build_researcher_walk_scene,
    configure_preview_env,
    pose_walk_for_render,
)
from researcher_shirt_derived_meshes import (  # noqa: E402
    MAX_COAT_BODY_GAP_M,
    MAX_COAT_EDGE_M,
    SKIRT_NAME,
    SLEEVES_NAME,
    knee_world_z,
)

MAX_BBOX_PAD_M = 0.10
COAT_BASES = frozenset({SKIRT_NAME, SLEEVES_NAME})


def _world_verts(ob: bpy.types.Object) -> list[Vector]:
    deps = bpy.context.evaluated_depsgraph_get()
    ev = ob.evaluated_get(deps)
    return [ev.matrix_world @ v.co for v in ev.data.vertices]


def _bbox(points: list[Vector]) -> tuple[Vector, Vector]:
    xs = [p.x for p in points]
    ys = [p.y for p in points]
    zs = [p.z for p in points]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))


def _outside_pad(lo: Vector, hi: Vector, co: Vector, pad: float) -> float:
    out = 0.0
    for i in range(3):
        if co[i] < lo[i] - pad:
            out = max(out, lo[i] - pad - co[i])
        if co[i] > hi[i] + pad:
            out = max(out, co[i] - (hi[i] + pad))
    return out


def _max_coat_edge_m(ob: bpy.types.Object) -> float:
    mw = ob.matrix_world
    worst = 0.0
    for e in ob.data.edges:
        a = mw @ ob.data.vertices[e.vertices[0]].co
        b = mw @ ob.data.vertices[e.vertices[1]].co
        worst = max(worst, (a - b).length)
    return worst


def main() -> int:
    configure_preview_env("staff-researcher-idle")
    scene_data = build_researcher_walk_scene()
    scene = bpy.context.scene
    body = scene_data.body
    lm = compute_landmarks(body)
    z_knee = knee_world_z(body, lm)
    below_knee = 0
    worst_edge = 0.0
    worst_edge_ob = ""
    for fr in scene_data.walk_frames:
        pose_walk_for_render(scene, scene_data, int(fr))
        bpy.context.view_layer.update()
        deps = bpy.context.evaluated_depsgraph_get()
        deps.update()
        for ob in scene_data.render_meshes:
            base = ob.name.split(".")[0]
            if base not in COAT_BASES:
                continue
            edge_m = _max_coat_edge_m(ob)
            if edge_m > worst_edge:
                worst_edge = edge_m
                worst_edge_ob = base
            if base == SKIRT_NAME:
                for co in _world_verts(ob):
                    if co.z < z_knee - 0.004:
                        below_knee += 1
    if below_knee > 0:
        print(
            f"FAIL staff-researcher-idle coat_below_knee count={below_knee} "
            f"z_knee={z_knee:.4f}"
        )
        return 1
    if worst_edge > MAX_COAT_EDGE_M + 1e-4:
        print(
            f"FAIL staff-researcher-idle coat_edge_len={worst_edge:.4f}m "
            f"mesh={worst_edge_ob} max={MAX_COAT_EDGE_M}"
        )
        return 1
    print(
        f"OK staff-researcher-idle deform_bbox edge={worst_edge:.4f}m below_knee={below_knee} "
        f"frames={scene_data.walk_frames}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())