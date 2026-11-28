"""Extract garage hero meshes from garage-interior-room-kit source (Z-up), then Y-up for garage-kit."""
from __future__ import annotations

import importlib.util
import math
from pathlib import Path
from typing import Iterable

import bpy
from mathutils import Vector


def _load_room_kit_module():
    path = Path(__file__).with_name("garage-interior-room-kit.py")
    spec = importlib.util.spec_from_file_location("garage_interior_room_kit", path)
    if spec is None or spec.loader is None:
        raise RuntimeError("cannot load garage-interior-room-kit.py")
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


import cc0_common as cc0  # noqa: E402

room_kit = _load_room_kit_module()


def _rotate_z_up_to_y_up(objects: Iterable[bpy.types.Object]) -> None:
    """Room kit is Z-vertical; garage-kit heroes are Y-up (floor at y=0)."""
    objs = list(objects)
    bpy.context.view_layer.update()
    pivot = bpy.data.objects.new("RoomKitPivot", None)
    bpy.context.collection.objects.link(pivot)
    for ob in objs:
        mw = ob.matrix_world.copy()
        ob.parent = pivot
        ob.matrix_world = mw
    pivot.rotation_euler = (-math.pi / 2.0, 0.0, 0.0)
    bpy.context.view_layer.update()
    for ob in objs:
        mw = ob.matrix_world.copy()
        ob.parent = None
        ob.matrix_world = mw
    bpy.data.objects.remove(pivot, do_unlink=True)
    bpy.context.view_layer.update()


def _uniform_scale_parts(objects: list[bpy.types.Object], factor: float) -> None:
    if abs(factor - 1.0) < 1e-6:
        return
    for ob in objects:
        ob.scale = (ob.scale.x * factor, ob.scale.y * factor, ob.scale.z * factor)
    bpy.context.view_layer.update()
    for ob in objects:
        bpy.ops.object.select_all(action="DESELECT")
        ob.select_set(True)
        bpy.context.view_layer.objects.active = ob
        bpy.ops.object.transform_apply(scale=True)
    bpy.context.view_layer.update()


def _uniform_scale_to_desk_height(desk: bpy.types.Object, objects: list[bpy.types.Object], target_h: float) -> float:
    bpy.context.view_layer.update()
    lo, hi = cc0.world_bbox(desk)
    h = max(hi.y - lo.y, 1e-6)
    factor = target_h / h
    _uniform_scale_parts(objects, factor)
    return factor


def _center_xz_and_ground(objects: list[bpy.types.Object]) -> None:
    bpy.context.view_layer.update()
    lo = Vector((1e9, 1e9, 1e9))
    hi = Vector((-1e9, -1e9, -1e9))
    for ob in objects:
        mn, mx = cc0.world_bbox(ob)
        lo.x, lo.y, lo.z = min(lo.x, mn.x), min(lo.y, mn.y), min(lo.z, mn.z)
        hi.x, hi.y, hi.z = max(hi.x, mx.x), max(hi.y, mx.y), max(hi.z, mx.z)
    cx, cz = (lo.x + hi.x) * 0.5, (lo.z + hi.z) * 0.5
    for ob in objects:
        ob.location.x -= cx
        ob.location.z -= cz
    bpy.context.view_layer.update()
    min_y = min(cc0.world_bbox(ob)[0].y for ob in objects)
    for ob in objects:
        ob.location.y -= min_y
    bpy.context.view_layer.update()


def _build_room_kit_desk_and_screen() -> tuple[bpy.types.Object, bpy.types.Object]:
    """Same desk+monitor layout as place_founder_cluster (fit_screen_to_desk + stack)."""
    cx, cy = 0.0, 0.0
    desk = room_kit.furniture_prop(
        "prop_desk",
        room_kit.KENNEY_GLB_SOURCES["prop_desk"],
        (cx, cy),
        rot_z=math.radians(200),
        scale=room_kit.FURN_SCALE,
    )
    screen = room_kit.furniture_prop(
        "prop_screen",
        room_kit.KENNEY_GLB_SOURCES["prop_screen"],
        (cx + 0.06, cy + 0.08),
        rot_z=math.radians(200),
        scale=room_kit.FURN_SCALE * 0.72,
    )
    room_kit.fit_screen_to_desk(screen, desk, width_ratio=0.40)
    room_kit.snap_prop_on_support(screen, desk)
    return desk, screen


def fit_screen_to_desk_y_up(
    screen: bpy.types.Object, desk: bpy.types.Object, *, width_ratio: float = 0.44
) -> None:
    """Same width_ratio as room-kit fit_screen_to_desk, for Y-up pass16 desk."""
    bpy.context.view_layer.update()
    d_lo, d_hi = cc0.world_bbox(desk)
    desk_w = max(d_hi.x - d_lo.x, d_hi.z - d_lo.z, 0.01)
    s_lo, s_hi = cc0.world_bbox(screen)
    sw = max(s_hi.x - s_lo.x, s_hi.z - s_lo.z, 0.01)
    factor = (desk_w * width_ratio) / sw
    _uniform_scale_parts([screen], factor)


def _close_roll_door_bottom_gap(ob: bpy.types.Object) -> bpy.types.Object:
    """Seal vertical gap between bottom bar and slat slab (pass24 hero tweak)."""
    bpy.context.view_layer.update()
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.separate(type="LOOSE")
    bpy.ops.object.mode_set(mode="OBJECT")
    shells = [o for o in bpy.context.selected_objects if o.type == "MESH"]
    if len(shells) < 2:
        if len(shells) == 1:
            return shells[0]
        return ob
    lower = min(shells, key=lambda s: cc0.world_bbox(s)[0].y)
    rest = [s for s in shells if s is not lower]
    upper = min(rest, key=lambda s: cc0.world_bbox(s)[0].y)
    _, l_hi = cc0.world_bbox(lower)
    u_lo, _ = cc0.world_bbox(upper)
    gap = u_lo.y - l_hi.y
    if gap > 0.0005:
        lower.location.y += gap + 0.024
        bpy.context.view_layer.update()
    bpy.ops.object.select_all(action="DESELECT")
    for shell in shells:
        shell.select_set(True)
    bpy.context.view_layer.objects.active = shells[0]
    bpy.ops.object.join()
    return bpy.context.view_layer.objects.active


def extract_roll_door_y_up() -> list[bpy.types.Object]:
    """Merged Kenney slat roll door from room-kit build_roll_door (same mesh)."""
    door = room_kit.build_roll_door()
    door.name = "kit_roll_door"
    cc0.apply_palette_material(door, "roll_door")
    _rotate_z_up_to_y_up([door])
    target_h = 2.35
    lo, hi = cc0.world_bbox(door)
    cur_h = max(hi.y - lo.y, 0.01)
    factor = target_h / cur_h
    _uniform_scale_parts([door], factor)
    door = _close_roll_door_bottom_gap(door)
    door.name = "kit_roll_door"
    _center_xz_and_ground([door])
    return [door]


def spawn_crt_monitor_for_desk(desk: bpy.types.Object, *, name: str = "CrtMonitor") -> bpy.types.Object:
    """Kenney computerScreen with room-kit width_ratio on an existing Y-up desk."""
    import kenney_garage_asset as kga  # type: ignore

    screen = kga._import_kenney_furniture("computerScreen.glb", name)
    kga._orient_kenney_y_up(screen)
    screen.location = desk.location.copy()
    kga._center_on_part(screen, desk, dx=0.02, dz=-0.02)
    fit_screen_to_desk_y_up(screen, desk, width_ratio=0.44)
    kga._center_on_part(screen, desk, dx=0.0, dz=0.0)
    lo, hi = cc0.world_bbox(screen)
    span = max(hi.x - lo.x, hi.y - lo.y, hi.z - lo.z, 1e-6)
    hero_max = 0.42
    if span < hero_max * 0.98:
        _uniform_scale_parts([screen], hero_max / span)
        kga._center_on_part(screen, desk, dx=0.0, dz=0.0)
    return screen


def extract_crt_monitor_y_up(*, desk_target_h: float = 0.76) -> list[bpy.types.Object]:
    """Standalone monitor at origin (room-kit scale vs 0.76 m desk)."""
    import kenney_garage_asset as kga  # type: ignore

    desk = kga._import_kenney_furniture("desk.glb", "DesktopRef")
    kga._orient_kenney_y_up(desk)
    kga._scale_height(desk, desk_target_h)
    kga._center_xz(desk)
    kga._snap_floor(desk)
    screen = spawn_crt_monitor_for_desk(desk, name="CrtMonitor")
    kga._place_on_top(screen, desk, overlap=0.002)
    screen.name = "CrtMonitor"
    _center_xz_and_ground([screen])
    bpy.data.objects.remove(desk, do_unlink=True)
    bpy.context.view_layer.update()
    return [screen]


def measure_desk_crt_layout(parts: list[bpy.types.Object]) -> dict[str, float]:
    desk = next(p for p in parts if p.name.lower().startswith("desktop"))
    screen = next(p for p in parts if "crt" in p.name.lower())
    d_lo, d_hi = cc0.world_bbox(desk)
    m_lo, m_hi = cc0.world_bbox(screen)
    return {
        "desk_top_y": float(d_hi.y),
        "crt_base_y": float(m_lo.y),
        "gap_y": float(abs(m_lo.y - d_hi.y)),
        "desk_x0": float(d_lo.x),
        "desk_x1": float(d_hi.x),
        "crt_x0": float(m_lo.x),
        "crt_x1": float(m_hi.x),
        "desk_z0": float(d_lo.z),
        "desk_z1": float(d_hi.z),
        "crt_z0": float(m_lo.z),
        "crt_z1": float(m_hi.z),
        "desk_top_x": float((d_hi.x - d_lo.x)),
        "crt_x_span": float((m_hi.x - m_lo.x)),
    }