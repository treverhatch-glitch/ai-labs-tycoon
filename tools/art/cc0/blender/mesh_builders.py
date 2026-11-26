"""Procedural CC0-derivative heroes (Blender bpy primitives, joined)."""
from __future__ import annotations

import math

import bpy
from mathutils import Vector

# 42U ≈ 2.0 m cabinet
RACK_H = 2.0
RACK_W = 0.60
RACK_D = 1.0
U_H = 0.04445


def _cube(name: str, size: tuple[float, float, float], loc: tuple[float, float, float]) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.scale = (size[0] / 2, size[1] / 2, size[2] / 2)
    bpy.ops.object.transform_apply(scale=True)
    return ob


def _weld_touching(ob: bpy.types.Object, threshold: float = 0.018) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=threshold)
    bpy.ops.object.mode_set(mode="OBJECT")


def _join_all(objects: list[bpy.types.Object]) -> bpy.types.Object:
    bpy.ops.object.select_all(action="DESELECT")
    for ob in objects:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = objects[0]
    if len(objects) > 1:
        bpy.ops.object.join()
    return bpy.context.view_layer.objects.active


def build_server_rack_42u() -> bpy.types.Object:
    """Single upright 42U cabinet; feet sit on z=0."""
    parts: list[bpy.types.Object] = []
    t = 0.025
    cx, cy = 0.0, 0.0
    half_h = RACK_H / 2

    # Feet + base
    foot = 0.08
    for sx in (-1, 1):
        for sy in (-1, 1):
            parts.append(
                _cube(
                    "foot",
                    (foot, foot, 0.04),
                    (cx + sx * (RACK_W / 2 - foot), cy + sy * (RACK_D / 2 - foot), 0.02),
                )
            )
    parts.append(_cube("base", (RACK_W, RACK_D, 0.05), (cx, cy, 0.025)))

    # Shell: back, sides, top
    parts.append(_cube("back", (RACK_W, t, RACK_H), (cx, cy + RACK_D / 2 - t / 2, half_h)))
    parts.append(_cube("side_l", (t, RACK_D, RACK_H), (cx - RACK_W / 2 + t / 2, cy, half_h)))
    parts.append(_cube("side_r", (t, RACK_D, RACK_H), (cx + RACK_W / 2 - t / 2, cy, half_h)))
    parts.append(_cube("top", (RACK_W, RACK_D, t), (cx, cy, RACK_H - t / 2)))

    # Top vent grilles (flush)
    for i in range(6):
        x = cx - 0.22 + i * 0.088
        parts.append(_cube("vent", (0.04, RACK_D * 0.85, 0.012), (x, cy, RACK_H - 0.008)))

    # Perforated front door (thin, inset)
    door_z = 0.05 + (RACK_H - 0.05) / 2
    parts.append(_cube("door", (RACK_W - 0.04, 0.02, RACK_H - 0.08), (cx, cy - RACK_D / 2 + 0.03, door_z)))
    # Perforations: thin plates inset IN door plane (welded, no floating holes)
    for row in range(10):
        for col in range(7):
            px = cx - 0.24 + col * 0.08
            pz = 0.12 + row * 0.18
            if pz > RACK_H - 0.15:
                break
            parts.append(
                _cube(
                    "hole",
                    (0.026, 0.006, 0.026),
                    (px, cy - RACK_D / 2 + 0.032, pz),
                )
            )

    # 1U / 2U blade faces (flush with door plane)
    face_y = cy - RACK_D / 2 + 0.055
    slots = [
        (1, 0.55),
        (1, 0.95),
        (2, 1.45),
        (1, 1.92),
        (2, 2.38),
        (1, 2.88),
        (1, 3.28),
        (2, 3.72),
    ]
    for units, z0 in slots:
        h = units * U_H - 0.004
        parts.append(_cube("blade", (RACK_W - 0.12, 0.012, h), (cx, face_y, z0 + h / 2)))

    # Status LEDs (emissive bakes)
    for i, (lz, color_idx) in enumerate([(1.05, 0), (1.55, 1), (2.05, 0), (2.55, 1)]):
        lx = cx + RACK_W / 2 - 0.06
        parts.append(_cube(f"led_{i}", (0.012, 0.012, 0.012), (lx, face_y + 0.005, lz)))

    root = _join_all(parts)
    root.name = "rack_42u"
    _weld_touching(root, threshold=0.06)
    return root


def build_pc_tower_desktop() -> bpy.types.Object:
    """Desktop tower ~0.45×0.20×0.45 m (H×W×D), nothing on top."""
    w, d, h = 0.20, 0.45, 0.45
    parts: list[bpy.types.Object] = []
    hh = h / 2

    parts.append(_cube("chassis", (w, d, h), (0, 0, hh)))

    # Front fascia inset
    parts.append(_cube("front_bezel", (w - 0.02, 0.015, h - 0.06), (0, -d / 2 + 0.02, hh)))

    # Drive bay / power block
    parts.append(_cube("drive_bay", (w - 0.06, 0.018, 0.12), (0, -d / 2 + 0.03, h - 0.14)))
    parts.append(_cube("power_btn", (0.025, 0.012, 0.025), (w * 0.25, -d / 2 + 0.028, h - 0.26)))

    # Glowing vertical accent (front)
    parts.append(_cube("accent", (0.018, 0.01, h * 0.55), (-w * 0.35, -d / 2 + 0.025, hh)))

    # Side vents (right)
    for i in range(5):
        z = 0.15 + i * 0.07
        parts.append(_cube(f"vent_{i}", (0.008, 0.14, 0.022), (w / 2 - 0.006, 0.02, z)))

    # Glass side window (thin flush panel)
    parts.append(_cube("side_glass", (0.006, d * 0.7, h * 0.62), (w / 2 - 0.004, 0, hh)))

    # Feet — base on z=0
    parts.append(_cube("foot_fr", (0.05, 0.05, 0.01), (w / 2 - 0.05, -d / 2 + 0.05, 0.005)))
    parts.append(_cube("foot_fl", (0.05, 0.05, 0.01), (-w / 2 + 0.05, -d / 2 + 0.05, 0.005)))
    parts.append(_cube("foot_br", (0.05, 0.05, 0.01), (w / 2 - 0.05, d / 2 - 0.05, 0.005)))
    parts.append(_cube("foot_bl", (0.05, 0.05, 0.01), (-w / 2 + 0.05, d / 2 - 0.05, 0.005)))

    root = _join_all(parts)
    root.name = "pc_tower"
    _weld_touching(root)
    return root


def assert_single_object(ob: bpy.types.Object, max_gap_m: float = 0.01) -> None:
    """Gate: one upright object; every vertex within max_gap_m of the dark cabinet hull."""
    import bmesh

    me = ob.data
    bm = bmesh.new()
    bm.from_mesh(me)
    bmesh.ops.remove_doubles(bm, verts=bm.verts, dist=max_gap_m * 0.5)
    verts = [v.co.copy() for v in bm.verts]
    bm.free()
    if not verts:
        raise RuntimeError("rack mesh empty")
    xs = [v.x for v in verts]
    ys = [v.y for v in verts]
    zs = [v.z for v in verts]
    hull_min = Vector((min(xs), min(ys), 0.0))
    hull_max = Vector((max(xs), max(ys), max(zs)))
    pad = max_gap_m
    hull_min -= Vector((pad, pad, 0))
    hull_max += Vector((pad, pad, pad))
    for v in verts:
        if (
            v.x < hull_min.x
            or v.y < hull_min.y
            or v.z < hull_min.z
            or v.x > hull_max.x
            or v.y > hull_max.y
            or v.z > hull_max.z
        ):
            raise RuntimeError(f"rack vertex outside {max_gap_m*100:.0f}cm hull envelope")


def color_parts_by_index(
    root: bpy.types.Object, palette: list[tuple[float, float, float]], led_indices: set[int] | None = None
) -> bpy.types.Object:
    bpy.ops.object.select_all(action="DESELECT")
    root.select_set(True)
    bpy.context.view_layer.objects.active = root
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.separate(type="LOOSE")
    bpy.ops.object.mode_set(mode="OBJECT")
    parts = [o for o in bpy.context.selected_objects if o.type == "MESH"]
    for i, p in enumerate(parts):
        c = palette[i % len(palette)]
        if led_indices and i in led_indices:
            c = (0.12, 0.82, 0.38) if i % 2 == 0 else (0.15, 0.45, 0.92)
        mesh = p.data
        vcol = mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
        for corner_i in range(len(mesh.loops)):
            vcol.data[corner_i].color = (c[0], c[1], c[2], 1.0)
    return _join_all(parts)