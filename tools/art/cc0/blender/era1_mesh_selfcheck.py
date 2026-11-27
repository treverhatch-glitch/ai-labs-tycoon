"""Automated connectivity checks for era1 compute hero kits (Blender)."""
from __future__ import annotations

from mathutils import Vector

import bpy

GAP_TOL = 0.002  # 2 mm


def _world_aabb(ob: bpy.types.Object) -> tuple[Vector, Vector]:
    corners = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    xs = [c.x for c in corners]
    ys = [c.y for c in corners]
    zs = [c.z for c in corners]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))


def _union_aabb(boxes: list[tuple[Vector, Vector]]) -> tuple[Vector, Vector]:
    mins = Vector((min(b[0].x for b in boxes), min(b[0].y for b in boxes), min(b[0].z for b in boxes)))
    maxs = Vector((max(b[1].x for b in boxes), max(b[1].y for b in boxes), max(b[1].z for b in boxes)))
    return mins, maxs


def _aabb_gap(a: tuple[Vector, Vector], b: tuple[Vector, Vector]) -> float:
    gap = 0.0
    for i in range(3):
        if a[1][i] < b[0][i]:
            gap = max(gap, float(b[0][i] - a[1][i]))
        elif b[1][i] < a[0][i]:
            gap = max(gap, float(a[0][i] - b[1][i]))
    return gap


def _footprint_contains(
    part: tuple[Vector, Vector], shell: tuple[Vector, Vector], slack_xy: float = 0.015, slack_z_below: float = 0.12
) -> bool:
    """XZ/Y footprint: part must not stick out beyond shell + slack (foot may extend below)."""
    smin, smax = shell
    pmin, pmax = part
    if pmin.x < smin.x - slack_xy or pmax.x > smax.x + slack_xy:
        return False
    if pmin.y < smin.y - slack_xy or pmax.y > smax.y + slack_xy:
        return False
    if pmin.z < smin.z - slack_z_below:
        return False
    if pmax.z > smax.z + slack_xy:
        return False
    return True


def assert_monitor_parts(parts: list[bpy.types.Object], root_name: str = "body_tapered") -> None:
    shell_names = {root_name, "bezel_frame", "neck"}
    pedestal = {"base_foot", "neck"}
    shell = _union_aabb([_world_aabb(p) for p in parts if p.name in shell_names])
    errors: list[str] = []
    skip_gap = pedestal | {
        "power_btn",
        "led_pwr",
        "screen",
        "rear_housing",
    }
    for ob in parts:
        if ob.name in shell_names:
            continue
        box = _world_aabb(ob)
        gap = _aabb_gap(box, shell)
        if ob.name not in skip_gap and gap > GAP_TOL + 1e-6:
            errors.append(f"{ob.name}: gap {gap*1000:.1f}mm from body shell")
        slack_z = 0.14 if ob.name in pedestal else 0.12
        if not _footprint_contains(box, shell, slack_z_below=slack_z):
            errors.append(f"{ob.name}: outside body footprint")
    if errors:
        raise RuntimeError("monitor self-check FAIL:\n  " + "\n  ".join(errors))


def assert_rack_parts(parts: list[bpy.types.Object], post_names: tuple[str, ...], rail_z: tuple[float, float]) -> None:
    posts = [p for p in parts if p.name in post_names]
    if len(posts) != 4:
        raise RuntimeError(f"rack self-check FAIL: expected 4 posts, got {len(posts)}")
    rails = [p for p in parts if p.name.startswith("rail_")]
    z_bot, z_top = rail_z
    errors: list[str] = []
    for post in posts:
        pmin, pmax = _world_aabb(post)
        touches_bot = pmin.z <= z_bot + GAP_TOL and pmax.z >= z_bot - GAP_TOL
        touches_top = pmax.z >= z_top - GAP_TOL and pmin.z <= z_top + GAP_TOL
        if not (touches_bot and touches_top):
            errors.append(f"{post.name}: post must span rails z={z_bot:.3f}..{z_top:.3f}")
    shell = _union_aabb([_world_aabb(p) for p in posts + rails])
    for ob in parts:
        if ob.name in post_names or ob.name.startswith("rail_"):
            continue
        box = _world_aabb(ob)
        gap = _aabb_gap(box, shell)
        if gap > GAP_TOL + 1e-6:
            errors.append(f"{ob.name}: gap {gap*1000:.1f}mm from frame")
    vert_mount = [p for p in parts if p.name.startswith("rail_mount_v")]
    if vert_mount:
        errors.append(f"rack has {len(vert_mount)} forbidden vertical mount bar(s)")
    if errors:
        raise RuntimeError("rack self-check FAIL:\n  " + "\n  ".join(errors))