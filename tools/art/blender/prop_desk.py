"""Headless bake: prop-desk hero from Kenney Furniture Kit (CC0) + restyle."""
from __future__ import annotations

import math
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import bpy  # noqa: E402
from mathutils import Vector  # noqa: E402

import _common  # noqa: E402

KENNEY = _common.CC0_ROOT / "kenney-furniture-kit" / "Models" / "GLTF format"
TARGET_WIDTH = 1.14
SURFACE_TOL = 0.01  # 1 cm Hatch bar
SOURCE_NOTE = "docs/art/cc0/kenney-furniture-kit (desk, computerScreen, computerKeyboard, computerMouse)"

# cc0-restyle-v1 role palette
PALETTE = {
    "wood_top": (0.62, 0.52, 0.4),
    "legs_pedestal": (0.26, 0.24, 0.22),
    "monitor_bezel": (0.14, 0.15, 0.17),
    "monitor_glass": (0.12, 0.38, 0.72),
    "keyboard": (0.2, 0.22, 0.26),
    "mouse": (0.34, 0.36, 0.4),
}


def _import_part(filename: str) -> list[bpy.types.Object]:
    path = KENNEY / filename
    if not path.is_file():
        raise FileNotFoundError(path)
    return _common._import_gltf_mesh(path)


def _snap_min_y(objs: list[bpy.types.Object], surface_y: float, tol: float = SURFACE_TOL) -> None:
    for o in objs:
        bpy.context.view_layer.update()
        mins, _ = _common._bbox_world([o])
        o.location.y += surface_y - mins.y
    bpy.context.view_layer.update()
    failures: list[str] = []
    for o in objs:
        mins, _ = _common._bbox_world([o])
        gap = abs(mins.y - surface_y)
        if gap > tol:
            failures.append(f"{o.name} min_y gap {gap:.4f}m (tol {tol})")
    if failures:
        raise RuntimeError("FAIL prop-desk assembly gate: " + "; ".join(failures))


def _desktop_surface_y(desk_parts: list[bpy.types.Object]) -> float:
    tops = [p for p in desk_parts if "drawer" not in p.name.lower()]
    if not tops:
        tops = desk_parts
    _mins, maxs = _common._bbox_world(tops)
    return float(maxs.y)


def _face_chair_and_camera(objs: list[bpy.types.Object]) -> None:
    """Chair side = +Z; 3/4 preview camera sits at +X / +Y quarter."""
    yaw = math.radians(205)
    for o in objs:
        o.rotation_euler.z += yaw


def build_desk() -> bpy.types.Object:
    _common._reset_scene()
    desk_parts = _import_part("desk.glb")
    _mins, maxs = _common._bbox_world(desk_parts)
    width = max(maxs.x - _mins.x, 0.01)
    scale = TARGET_WIDTH / width
    _common._apply_scale_uniform(desk_parts, scale)
    surface_y = _desktop_surface_y(desk_parts)

    peripherals: list[bpy.types.Object] = []
    def _center(objs: list[bpy.types.Object]) -> Vector:
        mn, mx = _common._bbox_world(objs)
        return (mn + mx) / 2

    screen = _import_part("computerScreen.glb")
    _common._apply_scale_uniform(screen, scale * 1.02)
    _snap_min_y(screen, surface_y)
    sc = _center(screen)
    for o in screen:
        o.location += Vector((0.02, 0.0, 0.02)) - Vector((sc.x, 0.0, sc.z))
    _face_chair_and_camera(screen)
    _snap_min_y(screen, surface_y)

    keyboard = _import_part("computerKeyboard.glb")
    _common._apply_scale_uniform(keyboard, scale)
    _snap_min_y(keyboard, surface_y)
    kc = _center(keyboard)
    for o in keyboard:
        o.location += Vector((-0.08, 0.0, 0.12)) - Vector((kc.x, 0.0, kc.z))
    _snap_min_y(keyboard, surface_y)

    mouse = _import_part("computerMouse.glb")
    _common._apply_scale_uniform(mouse, scale * 1.05)
    _snap_min_y(mouse, surface_y)
    mx, _, mz = _center(mouse)
    for o in mouse:
        o.location += Vector((0.26 - mx, 0.0, 0.12 - mz))
    _snap_min_y(mouse, surface_y)

    peripherals.extend(screen)
    peripherals.extend(keyboard)
    peripherals.extend(mouse)
    _snap_min_y(peripherals, surface_y)

    all_obs = list(desk_parts) + peripherals
    _common._bevel_meshes(all_obs, width=0.006, segments=2)
    for p in desk_parts:
        if "drawer" in p.name.lower():
            _common._paint_mesh_vertex_color(p, PALETTE["legs_pedestal"])
        else:
            _common._paint_mesh_vertex_color(p, PALETTE["wood_top"])
    for p in peripherals:
        name = p.name.lower()
        if "screen" in name:
            _common._paint_mesh_vertex_color(p, PALETTE["monitor_glass"])
        elif "keyboard" in name:
            _common._paint_mesh_vertex_color(p, PALETTE["keyboard"])
        else:
            _common._paint_mesh_vertex_color(p, PALETTE["mouse"])

    bpy.ops.object.select_all(action="DESELECT")
    for o in all_obs:
        o.select_set(True)
    bpy.context.view_layer.objects.active = all_obs[0]
    if len(all_obs) > 1:
        bpy.ops.object.join()
    joined = bpy.context.view_layer.objects.active
    joined.location = Vector((0.0, 0.0, -_common._bbox_world([joined])[0].y))
    joined.name = "mesh_prop-desk"
    return joined


def main() -> None:
    obj = build_desk()
    manifest = _common.finish_baked_export(
        "prop-desk",
        obj,
        SOURCE_NOTE,
        extra_manifest={"kenneyParts": ["desk.glb", "computerScreen.glb", "computerKeyboard.glb", "computerMouse.glb"]},
    )
    print(__import__("json").dumps(manifest, indent=2))


main()