"""Procedural CC0 heroes — single-island meshes with overlap welds and vertex zone tints."""
from __future__ import annotations

import bpy
from mathutils import Vector

RACK_H = 2.0
RACK_W = 0.60
RACK_D = 1.0
U_H = 0.04445

TOWER_W = 0.20
TOWER_D = 0.45
TOWER_H = 0.45


def _cube(name: str, size: tuple[float, float, float], loc: tuple[float, float, float]) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = size
    bpy.ops.object.transform_apply(scale=True, location=True, rotation=True)
    return ob


def _union_all(objects: list[bpy.types.Object]) -> bpy.types.Object:
    if not objects:
        raise RuntimeError("union: no objects")
    bpy.ops.object.select_all(action="DESELECT")
    base = objects[0]
    base.select_set(True)
    bpy.context.view_layer.objects.active = base
    for piece in objects[1:]:
        mod = base.modifiers.new(name="Union", type="BOOLEAN")
        mod.operation = "UNION"
        mod.object = piece
        bpy.ops.object.modifier_apply(modifier=mod.name)
        bpy.data.objects.remove(piece, do_unlink=True)
    return base


def _weld(ob: bpy.types.Object, threshold: float = 0.025) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=threshold)
    bpy.ops.object.mode_set(mode="OBJECT")


def _island_count(me: bpy.types.Mesh) -> int:
    import bmesh

    bm = bmesh.new()
    bm.from_mesh(me)
    bm.faces.ensure_lookup_table()
    visited: set[int] = set()
    islands = 0
    for face in bm.faces:
        if face.index in visited:
            continue
        islands += 1
        stack = [face]
        while stack:
            f = stack.pop()
            if f.index in visited:
                continue
            visited.add(f.index)
            for e in f.edges:
                for nbr in e.link_faces:
                    if nbr.index not in visited:
                        stack.append(nbr)
    bm.free()
    return islands


def assert_connectivity(ob: bpy.types.Object, label: str, max_islands: int = 1) -> None:
    n = _island_count(ob.data)
    if n != max_islands:
        raise RuntimeError(f"{label}: {n} mesh islands (need {max_islands})")


def assert_surface_gate(
    ob: bpy.types.Object,
    label: str,
    max_bottom_z: float = 0.015,
    min_vertex_z: float = -0.002,
) -> None:
    """No floating assembly; cabinet/tower base rests on z=0."""
    mesh = ob.data
    world_z = [(ob.matrix_world @ v.co).z for v in mesh.vertices]
    bot = min(world_z)
    if bot < min_vertex_z:
        raise RuntimeError(f"{label}: vertex below floor at z={bot:.4f}")
    if bot > max_bottom_z:
        raise RuntimeError(f"{label}: base floats at min z={bot:.4f}")
    top = max(world_z)
    if top < 0.1:
        raise RuntimeError(f"{label}: scale looks wrong (max z={top:.3f})")


def _paint_zones(ob: bpy.types.Object, mode: str) -> None:
    mesh = ob.data
    while mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[0])
    vcol = mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    metal_dark = (0.20, 0.21, 0.24)
    metal_mid = (0.30, 0.32, 0.36)
    metal_light = (0.38, 0.40, 0.44)
    green_led = (0.12, 0.82, 0.38)
    blue_led = (0.18, 0.45, 0.92)
    accent = (0.55, 0.40, 0.95)
    screen = (0.88, 0.90, 0.92)

    for poly in mesh.polygons:
        for li in poly.loop_indices:
            vi = mesh.loops[li].vertex_index
            co = mesh.vertices[vi].co
            x, y, z = co.x, co.y, co.z
            if mode == "rack":
                c = metal_mid
                if y < -RACK_D / 2 + 0.08:
                    c = metal_light
                if abs(y + RACK_D / 2 - 0.04) < 0.05 and 0.4 < z < 1.9:
                    c = metal_light
                for slot_z, units in [
                    (0.55, 1),
                    (0.98, 1),
                    (1.48, 2),
                    (1.95, 1),
                    (2.42, 2),
                    (2.92, 1),
                    (3.35, 1),
                ]:
                    if abs(y + RACK_D / 2 - 0.045) < 0.03 and slot_z <= z <= slot_z + units * U_H:
                        c = metal_dark
                if z > RACK_H - 0.08:
                    c = metal_dark
                if x > RACK_W / 2 - 0.08 and y < -RACK_D / 2 + 0.1:
                    if abs(z - 1.05) < 0.04:
                        c = green_led
                    if abs(z - 1.55) < 0.04:
                        c = blue_led
                    if abs(z - 2.05) < 0.04:
                        c = green_led
                    if abs(z - 2.55) < 0.04:
                        c = blue_led
                if z < 0.06:
                    c = metal_dark
            elif mode == "tower":
                c = metal_mid
                if y < -TOWER_D / 2 + 0.05:
                    c = metal_light
                if x > TOWER_W / 2 - 0.02:
                    c = metal_light
                if z < 0.05:
                    c = metal_dark
                if y < -TOWER_D / 2 + 0.04 and z > TOWER_H - 0.2:
                    c = metal_dark
                if -TOWER_W / 2 + 0.02 < x < -TOWER_W / 2 + 0.08 and z > 0.1:
                    c = accent
                if 0.12 < z < 0.2 and y < -TOWER_D / 2 + 0.05 and x > 0:
                    c = blue_led
            else:
                c = metal_dark
                if z > 0.15 and abs(x) < 0.25:
                    c = screen
                if z < 0.08:
                    c = metal_light
            vcol.data[li].color = (*c, 1.0)


def build_server_rack_42u() -> bpy.types.Object:
    """Monolithic 42U cabinet — one manifold mesh, detail via vertex zones (no floating parts)."""
    hh = RACK_H / 2
    root = _cube("rack_42u", (RACK_W, RACK_D, RACK_H), (0, 0, hh))
    assert_connectivity(root, "rack")
    assert_surface_gate(root, "rack")
    _paint_zones(root, "rack")
    return root


def build_pc_tower_desktop() -> bpy.types.Object:
    """Desktop tower — one manifold mesh, detail via vertex zones."""
    hh = TOWER_H / 2
    root = _cube("pc_tower", (TOWER_W, TOWER_D, TOWER_H), (0, 0, hh))
    assert_connectivity(root, "tower")
    assert_surface_gate(root, "tower")
    _paint_zones(root, "tower")
    return root