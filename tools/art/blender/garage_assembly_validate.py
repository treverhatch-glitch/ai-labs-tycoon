"""Fail the Blender build when multipart garage layouts violate assembly rules (Z-up build)."""
from __future__ import annotations

import bpy
from mathutils import Vector

from garage_assembly import _world_aabb


def _mesh_parts(parts: list[bpy.types.Object]) -> list[bpy.types.Object]:
    return [p for p in parts if p.type == "MESH" and p.name != "ConnectSpine"]


def _centre(ob: bpy.types.Object) -> Vector:
    lo, hi = _world_aabb(ob)
    return (lo + hi) * 0.5


def _by_prefix(parts: list[bpy.types.Object], prefix: str) -> list[bpy.types.Object]:
    return [p for p in parts if p.name.startswith(prefix)]


def _distinct_floor_plan(meshes: list[bpy.types.Object], min_dist: float, label: str) -> None:
    """Y-up build: four corners spread on the floor plane XZ."""
    centres = [_centre(m) for m in meshes]
    for i in range(len(meshes)):
        for j in range(i + 1, len(meshes)):
            d = (Vector((centres[i].x, 0, centres[i].z)) - Vector((centres[j].x, 0, centres[j].z))).length
            if d < min_dist:
                raise RuntimeError(
                    f"{label}: {meshes[i].name} vs {meshes[j].name} floor_dist={d:.4f} < {min_dist}"
                )


def validate_folding_chair(parts: list[bpy.types.Object]) -> None:
    legs = sorted(_by_prefix(parts, "Leg"), key=lambda o: o.name)
    if len(legs) != 4:
        raise RuntimeError(f"folding-chair: need 4 legs, got {len(legs)}")
    _distinct_floor_plan(legs, 0.12, "folding-chair legs")
    seat = next((p for p in parts if p.name == "Seat"), None)
    if seat is None:
        raise RuntimeError("folding-chair: missing Seat")
    seat_lo, _ = _world_aabb(seat)
    leg_tops = max(_world_aabb(leg)[1].y for leg in legs)
    if seat_lo.y < leg_tops - 0.02:
        raise RuntimeError(
            f"folding-chair: seat below leg tops (seat_lo.y={seat_lo.y:.3f}, legs_top={leg_tops:.3f})"
        )


def validate_desk_crt(parts: list[bpy.types.Object]) -> None:
    legs = sorted(_by_prefix(parts, "Leg"), key=lambda o: o.name)
    if len(legs) != 4:
        raise RuntimeError(f"desk-crt: need 4 legs, got {len(legs)}")
    _distinct_floor_plan(legs, 0.14, "desk-crt legs")
    desktop = next((p for p in parts if p.name == "Desktop"), None)
    if desktop is None:
        raise RuntimeError("desk-crt: missing Desktop")
    d_lo, _d_hi = _world_aabb(desktop)
    d_ctr = _centre(desktop)
    for leg in legs:
        c = _centre(leg)
        if abs(c.x - d_ctr.x) > 0.62 or abs(c.z - d_ctr.z) > 0.36:
            raise RuntimeError(f"desk-crt: {leg.name} not under desktop extent")
        _, leg_hi = _world_aabb(leg)
        if leg_hi.y > d_lo.y + 0.02:
            raise RuntimeError(f"desk-crt: {leg.name} penetrates desktop (leg_top={leg_hi.y:.3f}, desktop_lo={d_lo.y:.3f})")
        if d_lo.y - leg_hi.y > 0.12:
            raise RuntimeError(f"desk-crt: {leg.name} gap under desktop {d_lo.y - leg_hi.y:.3f}m")


def _require_core_stack(parts: list[bpy.types.Object], core_name: str, *, min_dist: float = 0.04) -> None:
    core = next((p for p in parts if p.name == core_name), None)
    if core is None:
        raise RuntimeError(f"missing core part {core_name!r}")
    core_c = _centre(core)
    for ob in parts:
        if ob == core or ob.type != "MESH":
            continue
        c = _centre(ob)
        if (c - core_c).length > 2.5:
            raise RuntimeError(f"{ob.name} centre too far from {core_name} ({(c - core_c).length:.2f}m)")


def validate_coffee_drip(parts: list[bpy.types.Object]) -> None:
    base = next((p for p in parts if p.name == "Base"), None)
    body = next((p for p in parts if p.name == "Body"), None)
    if not base or not body:
        raise RuntimeError("coffee-drip: missing Base or Body")
    bc, bb = _centre(base), _centre(body)
    if abs(bc.x - bb.x) > 0.12 or abs(bc.z - bb.z) > 0.12:
        raise RuntimeError("coffee-drip: body not aligned over base in plan")


def validate_crt_tower(parts: list[bpy.types.Object]) -> None:
    chassis = next((p for p in parts if p.name == "Chassis"), None)
    if not chassis:
        raise RuntimeError("crt-tower: missing Chassis")
    cc = _centre(chassis)
    for ob in parts:
        if ob.name in ("Screen",) or ob.name.startswith("Led"):
            continue
        c = _centre(ob)
        if abs(c.x - cc.x) > 0.14:
            raise RuntimeError(f"crt-tower: {ob.name} off chassis centre in x")


def validate_flat_wall(asset_id: str, parts: list[bpy.types.Object], core_name: str) -> None:
    core = next((p for p in parts if p.name == core_name), None)
    if not core:
        raise RuntimeError(f"{asset_id}: missing {core_name}")
    core_c = _centre(core)
    for ob in parts:
        if ob == core:
            continue
        c = _centre(ob)
        if (c - core_c).length > 1.85:
            raise RuntimeError(f"{asset_id}: {ob.name} too far from {core_name}")


def validate_lot_curb(parts: list[bpy.types.Object]) -> None:
    curb = next((p for p in parts if p.name == "Curb"), None)
    if not curb:
        raise RuntimeError("lot-curb: missing Curb")
    cc = _centre(curb)
    posts = _by_prefix(parts, "Post")
    for post in posts:
        c = _centre(post)
        if (c - cc).length > 0.75:
            raise RuntimeError(f"lot-curb: {post.name} too far from curb")


VALIDATORS = {
    "garage-folding-chair": validate_folding_chair,
    "garage-desk-crt": validate_desk_crt,
    "garage-coffee-drip": validate_coffee_drip,
    "garage-crt-tower": validate_crt_tower,
    "garage-exterior-wall": lambda p: validate_flat_wall("exterior-wall", p, "Core"),
    "garage-wall-panel": lambda p: validate_flat_wall("wall-panel", p, "Wall"),
    "garage-window-frame": lambda p: validate_flat_wall("window-frame", p, "Frame"),
    "garage-lot-curb": validate_lot_curb,
}


def validate_garage_assembly(asset_id: str, parts: list[bpy.types.Object]) -> None:
    fn = VALIDATORS.get(asset_id)
    if fn is None:
        return
    fn(_mesh_parts(parts))
    print(f"OK assembly_rules {asset_id}")