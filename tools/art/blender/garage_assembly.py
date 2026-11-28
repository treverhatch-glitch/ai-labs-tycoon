"""Snap multipart garage kits into one connected cluster (Z-up build)."""
from __future__ import annotations

import bpy
from mathutils import Vector


def _world_aabb(ob: bpy.types.Object) -> tuple[Vector, Vector]:
    pts = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    xs = [p.x for p in pts]
    ys = [p.y for p in pts]
    zs = [p.z for p in pts]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))


def _gap(a: tuple[Vector, Vector], b: tuple[Vector, Vector]) -> float:
    lo_a, hi_a = a
    lo_b, hi_b = b
    dx = max(0.0, max(lo_a.x - hi_b.x, lo_b.x - hi_a.x))
    dy = max(0.0, max(lo_a.y - hi_b.y, lo_b.y - hi_a.y))
    dz = max(0.0, max(lo_a.z - hi_b.z, lo_b.z - hi_a.z))
    return (dx * dx + dy * dy + dz * dz) ** 0.5


def _diag(parts: list[bpy.types.Object]) -> float:
    if not parts:
        return 1.0
    lo = Vector((1e9, 1e9, 1e9))
    hi = Vector((-1e9, -1e9, -1e9))
    for ob in parts:
        blo, bhi = _world_aabb(ob)
        lo.x, lo.y, lo.z = min(lo.x, blo.x), min(lo.y, blo.y), min(lo.z, blo.z)
        hi.x, hi.y, hi.z = max(hi.x, bhi.x), max(hi.y, bhi.y), max(hi.z, bhi.z)
    return max((hi - lo).length, 0.25)


def _volume(ob: bpy.types.Object) -> float:
    lo, hi = _world_aabb(ob)
    d = hi - lo
    return d.x * d.y * d.z


def _union(boxes: list[tuple[Vector, Vector]]) -> tuple[Vector, Vector]:
    lo = Vector((1e9, 1e9, 1e9))
    hi = Vector((-1e9, -1e9, -1e9))
    for blo, bhi in boxes:
        lo.x, lo.y, lo.z = min(lo.x, blo.x), min(lo.y, blo.y), min(lo.z, blo.z)
        hi.x, hi.y, hi.z = max(hi.x, bhi.x), max(hi.y, bhi.y), max(hi.z, bhi.z)
    return lo, hi


def absorb_to_main_cluster(mesh_parts: list[bpy.types.Object], *, max_gap: float = 0.02) -> None:
    """Grow cluster from largest part; pull strays until union gap <= max_gap."""
    if len(mesh_parts) < 2:
        return
    boxes = [_world_aabb(p) for p in mesh_parts]
    vols = [_volume(p) for p in mesh_parts]
    seed = max(range(len(mesh_parts)), key=lambda i: vols[i])
    cluster = {seed}
    changed = True
    while changed:
        changed = False
        ulo, uhi = _union([boxes[i] for i in cluster])
        for j in range(len(mesh_parts)):
            if j in cluster:
                continue
            if _gap(boxes[j], (ulo, uhi)) <= max_gap:
                cluster.add(j)
                changed = True
    ulo, uhi = _union([_world_aabb(mesh_parts[i]) for i in cluster])
    ctr = (ulo + uhi) * 0.5
    for i, ob in enumerate(mesh_parts):
        if i in cluster:
            continue
        for _ in range(80):
            box = _world_aabb(ob)
            gap = _gap(box, (ulo, uhi))
            if gap <= max_gap:
                break
            ob_ctr = (box[0] + box[1]) * 0.5
            delta = ctr - ob_ctr
            if delta.length < 1e-6:
                break
            step = min(max(gap - max_gap * 0.1, 0.005), 0.15)
            ob.location += delta.normalized() * step
            bpy.context.view_layer.update()
    bpy.context.view_layer.update()


def tighten_garage_cluster(parts: list[bpy.types.Object], *, gap_frac: float = 0.02) -> None:
    """Pull detached parts toward the main body until bbox gap <= 2cm."""
    mesh_parts = [p for p in parts if p.type == "MESH" and p.name != "ConnectSpine"]
    if len(mesh_parts) < 2:
        return
    span = _diag(mesh_parts)
    max_gap = min(max(span * gap_frac, 0.008), 0.02)
    main = max(mesh_parts, key=_volume)
    main_box = _world_aabb(main)
    main_ctr = (main_box[0] + main_box[1]) * 0.5
    mains = sorted(mesh_parts, key=_volume, reverse=True)[:3]
    for ob in mesh_parts:
        if ob in mains:
            continue
        target = main if _volume(ob) > _volume(main) * 0.2 else min(
            mains, key=lambda m: _gap(_world_aabb(ob), _world_aabb(m))
        )
        target_box = _world_aabb(target)
        target_ctr = (target_box[0] + target_box[1]) * 0.5
        for _ in range(96):
            gap = _gap(_world_aabb(ob), target_box)
            if gap <= max_gap:
                break
            ob_ctr = (_world_aabb(ob)[0] + _world_aabb(ob)[1]) * 0.5
            delta = target_ctr - ob_ctr
            if delta.length < 1e-6:
                break
            step = min(max(gap - max_gap * 0.15, 0.004), 0.12)
            ob.location += delta.normalized() * step
            bpy.context.view_layer.update()
            target_box = _world_aabb(target)
    bpy.context.view_layer.update()
    absorb_to_main_cluster(mesh_parts, max_gap=0.02)


def parent_under_assembly_root(
    parts: list[bpy.types.Object], asset_id: str, *, root_name: str = "GarageKitRoot"
) -> bpy.types.Object:
    mesh_parts = [p for p in parts if p.type == "MESH" and p.name != "ConnectSpine"]
    root = bpy.data.objects.new(root_name, None)
    bpy.context.collection.objects.link(root)
    root.matrix_world.identity()
    frozen = [(ob, ob.matrix_world.copy()) for ob in mesh_parts]
    for ob in mesh_parts:
        if ob.parent:
            ob.parent = None
    bpy.context.view_layer.update()
    for ob, mw in frozen:
        ob.parent = root
        ob.matrix_world = mw
    bpy.context.view_layer.update()
    return root


def bake_parts_local_transforms(parts: list[bpy.types.Object]) -> None:
    """Bake each part's loc/rot/scale into mesh data (pass10 float fix)."""
    mesh_parts = [p for p in parts if p.type == "MESH" and p.name != "ConnectSpine"]
    for ob in mesh_parts:
        bpy.ops.object.select_all(action="DESELECT")
        ob.select_set(True)
        bpy.context.view_layer.objects.active = ob
        bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    bpy.context.view_layer.update()