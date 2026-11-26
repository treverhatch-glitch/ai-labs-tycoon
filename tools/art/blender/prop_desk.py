"""Headless bake: prop-desk hero from Kenney Furniture Kit (CC0) + restyle."""
from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import bpy  # noqa: E402
from mathutils import Vector  # noqa: E402

import _common  # noqa: E402

KENNEY = _common.CC0_ROOT / "kenney-furniture-kit" / "Models" / "GLTF format"
TARGET_WIDTH = 1.14
SURFACE_TOL = 0.012  # 1 cm Hatch bar (+ numeric slack)
SOURCE_NOTE = "docs/art/cc0/kenney-furniture-kit (desk, computerScreen, computerKeyboard, computerMouse)"

# Native snap: (u, v) fractions on desk top in X and Y (Blender Z-up horizontal plane).
KENNEY_OFFSETS = {
    "computerScreen.glb": (0.50, 0.54),
    "computerKeyboard.glb": (0.36, 0.52),
    "computerMouse.glb": (0.62, 0.54),
}

PALETTE = {
    "wood_top": (0.62, 0.52, 0.4),
    "legs_pedestal": (0.26, 0.24, 0.22),
    "monitor_glass": (0.12, 0.38, 0.72),
    "keyboard": (0.2, 0.22, 0.26),
    "mouse": (0.34, 0.36, 0.4),
}


def _import_part(filename: str) -> list[bpy.types.Object]:
    path = KENNEY / filename
    if not path.is_file():
        raise FileNotFoundError(path)
    return _common._import_gltf_mesh(path)


def _desk_top_parts(desk_parts: list[bpy.types.Object]) -> list[bpy.types.Object]:
    return [p for p in desk_parts if "drawer" not in p.name.lower()] or desk_parts


def _desktop_surface_z(top_parts: list[bpy.types.Object]) -> float:
    return float(_common._bbox_world(top_parts)[1].z)


def _desktop_xy_bounds(top_parts: list[bpy.types.Object]) -> tuple[float, float, float, float]:
    mn, mx = _common._bbox_world(top_parts)
    return float(mn.x), float(mx.x), float(mn.y), float(mx.y)


def _snap_drawer_into_carcass(desk_parts: list[bpy.types.Object]) -> None:
    drawers = [p for p in desk_parts if "drawer" in p.name.lower()]
    tops = _desk_top_parts(desk_parts)
    if not drawers or not tops:
        return
    tmn, tmx = _common._bbox_world(tops)
    for dr in drawers:
        dmn, dmx = _common._bbox_world([dr])
        runner_z = tmn.z + (tmx.z - tmn.z) * 0.26
        dr.location.z += runner_z - dmn.z
        cavity_y = tmn.y + (tmx.y - tmn.y) * 0.42
        dr.location.y += cavity_y - (dmn.y + dmx.y) * 0.5


def _place_on_desktop(
    parts: list[bpy.types.Object],
    surface_z: float,
    xy_bounds: tuple[float, float, float, float],
    u: float,
    v: float,
) -> None:
    x0, x1, y0, y1 = xy_bounds
    tx = x0 + (x1 - x0) * u
    ty = y0 + (y1 - y0) * v
    mn, mx = _common._bbox_world(parts)
    cx, cy = (mn.x + mx.x) * 0.5, (mn.y + mx.y) * 0.5
    for o in parts:
        o.location += Vector((tx - cx, ty - cy, surface_z - mn.z))
    bpy.context.view_layer.update()


def _gate_peripheral_on_desktop(
    parts: list[bpy.types.Object],
    surface_z: float,
    xy_bounds: tuple[float, float, float, float],
    label: str,
) -> None:
    x0, x1, y0, y1 = xy_bounds
    mn, mx = _common._bbox_world(parts)
    gap = abs(mn.z - surface_z)
    if gap > SURFACE_TOL:
        raise RuntimeError(f"FAIL {label}: base Z gap {gap:.4f}m (surface {surface_z:.3f})")
    if not (x0 - 0.02 <= mn.x and mx.x <= x1 + 0.02 and y0 - 0.02 <= mn.y and mx.y <= y1 + 0.02):
        raise RuntimeError(
            f"FAIL {label}: outside desktop XY "
            f"[{x0:.2f},{x1:.2f}]×[{y0:.2f},{y1:.2f}] bbox=({mn.x:.2f},{mn.y:.2f})-({mx.x:.2f},{mx.y:.2f})"
        )


def build_desk() -> bpy.types.Object:
    _common._reset_scene()
    desk_parts = _import_part("desk.glb")
    width = _common._bbox_world(desk_parts)[1].x - _common._bbox_world(desk_parts)[0].x
    scale = TARGET_WIDTH / max(width, 0.01)
    _common._apply_scale_uniform(desk_parts, scale)
    top_parts = _desk_top_parts(desk_parts)
    _common._bevel_meshes(desk_parts, width=0.006, segments=2)
    _snap_drawer_into_carcass(desk_parts)
    surface_z = _desktop_surface_z(top_parts)
    xy = _desktop_xy_bounds(top_parts)

    all_obs: list[bpy.types.Object] = list(desk_parts)
    periph_files = (
        ("computerScreen.glb", scale * 1.02),
        ("computerKeyboard.glb", scale),
        ("computerMouse.glb", scale * 1.05),
    )
    for fname, sc in periph_files:
        parts = _import_part(fname)
        _common._apply_scale_uniform(parts, sc)
        u, v = KENNEY_OFFSETS[fname]
        _place_on_desktop(parts, surface_z, xy, u, v)
        if "Keyboard" in fname:
            for o in parts:
                o.location.z -= 0.0015
            bpy.context.view_layer.update()
        if "Screen" in fname:
            for o in parts:
                o.location.z -= 0.001
            bpy.context.view_layer.update()
        if "Mouse" in fname:
            for o in parts:
                o.location.z -= 0.006
            bpy.context.view_layer.update()
        _gate_peripheral_on_desktop(parts, surface_z, xy, fname)
        all_obs.extend(parts)
    for p in desk_parts:
        col = PALETTE["legs_pedestal"] if "drawer" in p.name.lower() else PALETTE["wood_top"]
        _common._paint_mesh_vertex_color(p, col)
    for p in all_obs:
        if p in desk_parts:
            continue
        nm = p.name.lower()
        if "screen" in nm:
            _common._paint_mesh_vertex_color(p, PALETTE["monitor_glass"])
        elif "keyboard" in nm:
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
    joined.location = Vector((0.0, 0.0, -_common._bbox_world([joined])[0].z))
    joined.name = "mesh_prop-desk"
    return joined


def main() -> None:
    obj = build_desk()
    manifest = _common.finish_baked_export(
        "prop-desk",
        obj,
        SOURCE_NOTE,
        extra_manifest={
            "kenneyParts": ["desk.glb", "computerScreen.glb", "computerKeyboard.glb", "computerMouse.glb"],
            "kenneyOffsets": KENNEY_OFFSETS,
        },
    )
    print(__import__("json").dumps(manifest, indent=2))


main()