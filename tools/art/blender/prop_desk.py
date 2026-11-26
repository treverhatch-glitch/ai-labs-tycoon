"""Headless bake: prop-desk hero from Kenney Furniture Kit (CC0) + restyle."""
from __future__ import annotations

import math
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import bpy  # noqa: E402
from mathutils import Matrix, Vector  # noqa: E402

import _common  # noqa: E402

KENNEY = _common.CC0_ROOT / "kenney-furniture-kit" / "Models" / "GLTF format"
TARGET_WIDTH = 1.14
SURFACE_TOL = 0.012
SOURCE_NOTE = (
    "docs/art/cc0/kenney-furniture-kit (desk, computerScreen, computerKeyboard, computerMouse); "
    "ambientCG Wood092"
)

KENNEY_OFFSETS = {
    "computerScreen.glb": (0.56, 0.46),
    "computerKeyboard.glb": (0.30, 0.70),
    "computerMouse.glb": (0.62, 0.64),
}

PALETTE = {
    "legs_pedestal": (0.48, 0.47, 0.46),
    "mouse": (0.40, 0.42, 0.46),
    "plant_pot": (0.74, 0.42, 0.28),
    "plant_leaf": (0.28, 0.74, 0.38),
}


def _import_part(filename: str) -> list[bpy.types.Object]:
    path = KENNEY / filename
    if not path.is_file():
        raise FileNotFoundError(path)
    return _common._import_gltf_mesh(path)


def _desk_shell_parts(desk_parts: list[bpy.types.Object]) -> list[bpy.types.Object]:
    return [p for p in desk_parts if "drawer" not in p.name.lower()]


def _desktop_surface_z(top_parts: list[bpy.types.Object]) -> float:
    return float(_common._bbox_world(top_parts)[1].z)


def _desktop_xy_bounds(top_parts: list[bpy.types.Object]) -> tuple[float, float, float, float]:
    mn, mx = _common._bbox_world(top_parts)
    return float(mn.x), float(mx.x), float(mn.y), float(mx.y)


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
        raise RuntimeError(f"FAIL {label}: outside desktop XY [{x0:.2f},{x1:.2f}]×[{y0:.2f},{y1:.2f}]")


def _init_emit_mask(obj: bpy.types.Object) -> None:
    mesh = obj.data
    if mesh.color_attributes.get("EmitMask") is None:
        mesh.color_attributes.new(name="EmitMask", type="FLOAT_COLOR", domain="CORNER")
    em = mesh.color_attributes["EmitMask"]
    for i in range(len(mesh.loops)):
        em.data[i].color = (0.0, 0.0, 0.0, 1.0)


def _paint_monitor(p: bpy.types.Object) -> None:
    mesh = p.data
    if mesh.color_attributes.get("BakeColor") is None:
        mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    vcol = mesh.color_attributes["BakeColor"]
    _init_emit_mask(p)
    emask = mesh.color_attributes["EmitMask"]
    mn, mx = _common._bbox_world([p])
    h = max(mx.z - mn.z, 1e-6)
    cam_dir = Vector((0.22, -0.68, 0.48)).normalized()
    for poly in mesh.polygons:
        n_world = (p.matrix_world.to_3x3() @ poly.normal).normalized()
        face_screen = n_world.dot(cam_dir) > 0.42
        for li in poly.loop_indices:
            vi = mesh.loops[li].vertex_index
            co = p.matrix_world @ mesh.vertices[vi].co
            t = (co.z - mn.z) / h
            if face_screen or t > 0.52:
                ui_bg = (0.14, 0.18, 0.24)
                chart = 0.4 + 0.6 * abs(math.sin(co.x * 18.0) * math.sin(co.z * 15.0))
                line = 0.55 if abs(co.y * 12.0) % 1.0 < 0.1 or abs(co.x * 10.0) % 1.0 < 0.08 else 0.0
                accent = min(1.0, chart * 0.55 + line)
                col = _common._lerp3(ui_bg, (0.52, 0.90, 0.98), accent)
                col = _common._lerp3(col, (0.72, 0.88, 0.94), min(0.28, accent))
                emask.data[li].color = (1.0, 0.0, 0.0, 1.0)
            elif t > 0.22:
                col = (0.32, 0.34, 0.38)
                emask.data[li].color = (0.0, 0.0, 0.0, 1.0)
            else:
                col = (0.36, 0.38, 0.42)
                emask.data[li].color = (0.0, 0.0, 0.0, 1.0)
            vcol.data[li].color = (*col, 1.0)


def _paint_keyboard(p: bpy.types.Object) -> None:
    mesh = p.data
    if mesh.color_attributes.get("BakeColor") is None:
        mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    vcol = mesh.color_attributes["BakeColor"]
    _init_emit_mask(p)
    mn, mx = _common._bbox_world([p])
    top_z = mx.z - (mx.z - mn.z) * 0.06
    for poly in mesh.polygons:
        for li in poly.loop_indices:
            vi = mesh.loops[li].vertex_index
            co = p.matrix_world @ mesh.vertices[vi].co
            gx = math.sin(co.x * 110.0) * math.sin(co.y * 95.0)
            cap = 0.78 if co.z >= top_z and gx > -0.2 else 0.22
            base = (0.52, 0.54, 0.58)
            hi = (0.74, 0.76, 0.80)
            rgb = _common._lerp3(base, hi, cap)
            vcol.data[li].color = (*rgb, 1.0)


def _add_potted_plant(
    surface_z: float,
    xy_bounds: tuple[float, float, float, float],
) -> bpy.types.Object:
    """Terracotta pot + leaf cluster (CC0-procedural accent)."""
    x0, x1, y0, y1 = xy_bounds
    tx = x0 + (x1 - x0) * 0.84
    ty = y0 + (y1 - y0) * 0.76
    bpy.ops.mesh.primitive_cylinder_add(radius=0.05, depth=0.055, location=(tx, ty, surface_z + 0.03))
    pot = bpy.context.active_object
    _common._paint_mesh_vertex_color(pot, PALETTE["plant_pot"])
    _init_emit_mask(pot)
    leaves: list[bpy.types.Object] = []
    for ox, oy, r in ((0, 0, 0.05), (-0.02, 0.015, 0.04), (0.02, -0.01, 0.038)):
        bpy.ops.mesh.primitive_uv_sphere_add(radius=r, location=(tx + ox, ty + oy, surface_z + 0.09))
        leaf = bpy.context.active_object
        _common._paint_mesh_vertex_color(leaf, PALETTE["plant_leaf"])
        _init_emit_mask(leaf)
        leaves.append(leaf)
    bpy.ops.object.select_all(action="DESELECT")
    pot.select_set(True)
    for leaf in leaves:
        leaf.select_set(True)
    bpy.context.view_layer.objects.active = pot
    bpy.ops.object.join()
    joined = bpy.context.active_object
    joined.name = "desk_plant"
    return joined


def build_desk() -> bpy.types.Object:
    _common._reset_scene()
    desk_parts = _import_part("desk.glb")
    width = _common._bbox_world(desk_parts)[1].x - _common._bbox_world(desk_parts)[0].x
    scale = TARGET_WIDTH / max(width, 0.01)
    desk_parts = _desk_shell_parts(desk_parts)
    _common._apply_scale_uniform(desk_parts, scale)
    top_parts = desk_parts
    _common._bevel_meshes(desk_parts, width=0.015, segments=3)
    surface_z = _desktop_surface_z(top_parts)
    xy = _desktop_xy_bounds(top_parts)

    all_obs: list[bpy.types.Object] = list(desk_parts)
    for p in desk_parts:
        _init_emit_mask(p)

    periph_files = (
        ("computerScreen.glb", scale * 1.02),
        ("computerKeyboard.glb", scale * 1.10),
        ("computerMouse.glb", scale * 1.05),
    )
    for fname, sc in periph_files:
        parts = _import_part(fname)
        _common._apply_scale_uniform(parts, sc)
        u, v = KENNEY_OFFSETS[fname]
        _place_on_desktop(parts, surface_z, xy, u, v)
        if "Screen" in fname:
            for o in parts:
                o.matrix_world = o.matrix_world @ Matrix.Rotation(math.radians(17.0), 4, "X")
                o.location.z -= 0.002
        if "Keyboard" in fname:
            for o in parts:
                o.location.z -= 0.002
        if "Mouse" in fname:
            for o in parts:
                o.location.z -= 0.006
        bpy.context.view_layer.update()
        _gate_peripheral_on_desktop(parts, surface_z, xy, fname)
        all_obs.extend(parts)

    plant = _add_potted_plant(surface_z, xy)
    all_obs.append(plant)

    oak_light = (1.0, 0.78, 0.52)
    oak_dark = (0.78, 0.54, 0.36)
    for p in desk_parts:
        zspan = _common._bbox_world([p])[1].z - _common._bbox_world([p])[0].z
        if zspan < 0.12:
            _common._paint_mesh_vertex_color(p, PALETTE["legs_pedestal"])
        else:
            _common._paint_wood_grain(p, light=oak_light, dark=oak_dark, grain_scale=32.0)

    for p in all_obs:
        if p in desk_parts or p is plant:
            continue
        nm = p.name.lower()
        if "screen" in nm:
            _paint_monitor(p)
        elif "keyboard" in nm:
            _paint_keyboard(p)
        else:
            _init_emit_mask(p)
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
            "cc0Wood": "docs/art/cc0/ambientcg/Wood092_1K-JPG",
            "restyle": "wood-grain-v8",
        },
    )
    print(__import__("json").dumps(manifest, indent=2))


main()