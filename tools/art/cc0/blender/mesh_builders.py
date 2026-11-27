"""CC0 heroes — chunky TP-style detail, voxel-welded single island + gates."""
from __future__ import annotations

import bpy
from mathutils import Vector

RACK_H = 2.0
RACK_W = 0.4826  # 19" rack
RACK_D = 1.0
U_H = 0.04445

TOWER_W = 0.20
TOWER_D = 0.45
TOWER_H = 0.45

MIN_TRIS_RACK = 3200
MIN_TRIS_TOWER = 1200
MIN_COLOR_BANDS = 7


def _cube(name: str, dims: tuple[float, float, float], loc: tuple[float, float, float]) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = dims
    bpy.ops.object.transform_apply(scale=True, location=True, rotation=True)
    return ob


def _join(objects: list[bpy.types.Object]) -> bpy.types.Object:
    bpy.ops.object.select_all(action="DESELECT")
    for ob in objects:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = objects[0]
    if len(objects) > 1:
        bpy.ops.object.join()
    return bpy.context.view_layer.objects.active


def _union_parts(objects: list[bpy.types.Object]) -> bpy.types.Object:
    if not objects:
        raise ValueError("no parts")
    base = objects[0]
    bpy.context.view_layer.objects.active = base
    base.select_set(True)
    for ob in objects[1:]:
        mod = base.modifiers.new("BoolUnion", "BOOLEAN")
        mod.operation = "UNION"
        mod.object = ob
        mod.solver = "EXACT"
        bpy.ops.object.modifier_apply(modifier=mod.name)
        bpy.data.objects.remove(ob, do_unlink=True)
    return base


def _voxel_weld(ob: bpy.types.Object, voxel_size: float) -> None:
    mod = ob.modifiers.new("VoxelWeld", "REMESH")
    mod.mode = "VOXEL"
    mod.voxel_size = voxel_size
    mod.adaptivity = 0.0
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.modifier_apply(modifier=mod.name)


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


def _tri_count(me: bpy.types.Mesh) -> int:
    return sum(len(p.vertices) for p in me.polygons)


def assert_connectivity(ob: bpy.types.Object, label: str, max_islands: int = 1) -> None:
    n = _island_count(ob.data)
    if n != max_islands:
        raise RuntimeError(f"{label}: {n} mesh islands (need {max_islands})")


def assert_surface_gate(ob: bpy.types.Object, label: str, max_bottom_z: float = 0.02) -> None:
    world_z = [(ob.matrix_world @ v.co).z for v in ob.data.vertices]
    bot = min(world_z)
    if bot < -0.002:
        raise RuntimeError(f"{label}: vertex below floor z={bot:.4f}")
    if bot > max_bottom_z:
        raise RuntimeError(f"{label}: base floats min z={bot:.4f}")


def assert_detail_gate(ob: bpy.types.Object, label: str, min_tris: int) -> None:
    tris = _tri_count(ob.data)
    if tris < min_tris:
        raise RuntimeError(f"{label}: {tris} tris (need >={min_tris})")
    mesh = ob.data
    vcol = mesh.color_attributes.get("BakeColor")
    if vcol is None:
        raise RuntimeError(f"{label}: missing BakeColor for band gate")
    buckets: set[tuple[int, int, int]] = set()
    for d in vcol.data:
        c = d.color
        buckets.add((int(c[0] * 10), int(c[1] * 10), int(c[2] * 10)))
    if len(buckets) < MIN_COLOR_BANDS:
        raise RuntimeError(f"{label}: {len(buckets)} color bands (need >={MIN_COLOR_BANDS})")


def _paint_rack(ob: bpy.types.Object) -> None:
    mesh = ob.data
    while mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[0])
    vcol = mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    metal_dark = (0.18, 0.19, 0.22)
    metal_mid = (0.28, 0.30, 0.34)
    metal_light = (0.40, 0.42, 0.46)
    rail = (0.22, 0.24, 0.28)
    faceplate = (0.32, 0.34, 0.38)
    green_led = (0.12, 0.82, 0.38)
    blue_led = (0.18, 0.45, 0.92)
    cable = (0.15, 0.16, 0.18)
    for poly in mesh.polygons:
        for li in poly.loop_indices:
            x, y, z = mesh.vertices[mesh.loops[li].vertex_index].co
            c = metal_mid
            if z < 0.1:
                c = metal_dark
            if x < -RACK_W / 2 + 0.06 or x > RACK_W / 2 - 0.06:
                c = rail
            if y < -RACK_D / 2 + 0.12:
                c = metal_light
            if y > RACK_D / 2 - 0.15:
                c = cable
            for z0, units in [
                (0.35, 1),
                (0.75, 1),
                (1.15, 2),
                (1.62, 1),
                (2.05, 2),
                (2.55, 1),
                (2.95, 1),
                (3.35, 1),
            ]:
                if (
                    y < -RACK_D / 2 + 0.14
                    and abs(x) < RACK_W / 2 - 0.08
                    and z0 <= z <= z0 + units * U_H + 0.01
                ):
                    c = faceplate
            if x > RACK_W / 2 - 0.06 and y < -RACK_D / 2 + 0.2:
                if 0.9 < z < 1.0 or 1.8 < z < 1.9:
                    c = green_led
                if 1.35 < z < 1.45 or 2.25 < z < 2.35:
                    c = blue_led
            if z > RACK_H - 0.12:
                c = metal_dark
            vcol.data[li].color = (*c, 1.0)


def _paint_tower(ob: bpy.types.Object) -> None:
    mesh = ob.data
    while mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[0])
    vcol = mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    metal_mid = (0.28, 0.30, 0.34)
    metal_dark = (0.20, 0.21, 0.24)
    bezel = (0.36, 0.38, 0.42)
    bay = (0.24, 0.26, 0.30)
    accent = (0.55, 0.40, 0.95)
    led = (0.15, 0.55, 0.85)
    vent = (0.18, 0.19, 0.22)
    for poly in mesh.polygons:
        for li in poly.loop_indices:
            x, y, z = mesh.vertices[mesh.loops[li].vertex_index].co
            c = metal_mid
            if z < 0.035:
                c = metal_dark
            if y < -TOWER_D / 2 + 0.05:
                c = bezel
            if y < -TOWER_D / 2 + 0.06 and z > TOWER_H - 0.22:
                c = bay
            if y < -TOWER_D / 2 + 0.055 and 0.34 < z < 0.38 and x > 0.02:
                c = led
            if -TOWER_W / 2 + 0.01 < x < -TOWER_W / 2 + 0.05 and z > 0.08:
                c = accent
            if x > TOWER_W / 2 - 0.04 and 0.12 < z < 0.42:
                c = vent
            vcol.data[li].color = (*c, 1.0)


def build_server_rack_42u() -> bpy.types.Object:
    """Chunky 42U rack: rails, RU faceplates, LEDs, cable bump, casters — voxel-welded."""
    hh = RACK_H / 2
    parts: list[bpy.types.Object] = []
    base_h = 0.10
    cab_h = RACK_H - base_h * 0.55
    cab_cz = base_h * 0.45 + cab_h / 2
    parts.append(_cube("cabinet", (RACK_W, RACK_D, cab_h), (0, 0, cab_cz)))
    parts.append(_cube("plinth", (RACK_W + 0.10, RACK_D + 0.10, base_h), (0, 0, base_h / 2)))
    for sx, sy in ((-1, -1), (-1, 1), (1, -1), (1, 1)):
        parts.append(
            _cube(
                "caster",
                (0.13, 0.13, 0.04),
                (sx * (RACK_W / 2 - 0.04), sy * (RACK_D / 2 - 0.04), 0.05),
            )
        )
    rail_inset = 0.06
    parts.append(
        _cube("rail_l", (0.04, RACK_D - 0.12, RACK_H - 0.18), (-RACK_W / 2 + rail_inset, 0, hh))
    )
    parts.append(
        _cube("rail_r", (0.04, RACK_D - 0.12, RACK_H - 0.18), (RACK_W / 2 - rail_inset, 0, hh))
    )
    door_depth = RACK_D * 0.42
    face_y = -RACK_D / 2 + door_depth / 2
    parts.append(_cube("front_door", (RACK_W - 0.06, door_depth, RACK_H - 0.14), (0, face_y, hh)))
    ru_rows = [
        (0.48, U_H * 0.95),
        (0.70, U_H * 0.95),
        (0.95, U_H * 1.85),
        (1.28, U_H * 0.95),
        (1.52, U_H * 1.85),
        (1.82, U_H * 0.95),
    ]
    for z0, ru_h in ru_rows:
        parts.append(_cube("ru", (RACK_W - 0.10, door_depth + 0.03, ru_h * 0.92), (0, face_y, z0)))
    parts.append(_cube("door_top", (RACK_W - 0.08, 0.05, 0.06), (0, face_y, cab_cz + cab_h / 2 - 0.12)))
    parts.append(_cube("cable_mgr", (RACK_W * 0.65, 0.10, RACK_H * 0.55), (0, RACK_D / 2 - rail_inset, hh)))
    for z in (0.85, 1.15, 1.45, 1.72):
        parts.append(_cube("led", (0.05, RACK_D * 0.22, 0.08), (RACK_W / 2 - 0.06, 0.0, z)))

    root = _union_parts(parts)
    root.name = "rack_42u"
    _voxel_weld(root, 0.011)
    assert_connectivity(root, "rack")
    assert_surface_gate(root, "rack")
    _paint_rack(root)
    assert_detail_gate(root, "rack", MIN_TRIS_RACK)
    return root


def build_pc_tower_desktop() -> bpy.types.Object:
    """Chunky desktop tower: bezel, bays, button, vents, feet — voxel-welded."""
    hh = TOWER_H / 2
    parts: list[bpy.types.Object] = []
    parts.append(_cube("chassis", (TOWER_W, TOWER_D, TOWER_H - 0.03), (0, 0, hh + 0.01)))
    front_y = -TOWER_D / 2 + 0.016
    parts.append(_cube("bezel", (TOWER_W - 0.02, 0.05, TOWER_H - 0.08), (0, front_y, hh)))
    parts.append(_cube("bay_upper", (TOWER_W - 0.06, 0.045, 0.12), (0, front_y, TOWER_H - 0.17)))
    parts.append(_cube("bay_lower", (TOWER_W - 0.06, 0.045, 0.10), (0, front_y, TOWER_H - 0.32)))
    parts.append(_cube("power_btn", (0.032, 0.04, 0.032), (TOWER_W * 0.2, front_y, TOWER_H - 0.26)))
    parts.append(_cube("led_bar", (0.02, 0.035, TOWER_H * 0.42), (-TOWER_W / 2 + 0.04, front_y, hh)))
    for i in range(5):
        z = 0.12 + i * 0.065
        parts.append(_cube(f"vent", (0.04, 0.12, 0.028), (TOWER_W / 2 - 0.03, 0.02, z)))
    parts.append(_cube("plinth", (TOWER_W + 0.02, TOWER_D + 0.02, 0.04), (0, 0, 0.02)))
    parts.append(_cube("foot_bridge", (TOWER_W - 0.06, TOWER_D - 0.06, 0.035), (0, 0, 0.018)))
    for sx, sy in ((-1, -1), (-1, 1), (1, -1), (1, 1)):
        parts.append(
            _cube("foot", (0.05, 0.05, 0.022), (sx * (TOWER_W / 2 - 0.05), sy * (TOWER_D / 2 - 0.05), 0.011))
        )

    root = _union_parts(parts)
    root.name = "pc_tower"
    _voxel_weld(root, 0.0055)
    assert_connectivity(root, "tower")
    assert_surface_gate(root, "tower")
    _paint_tower(root)
    assert_detail_gate(root, "tower", MIN_TRIS_TOWER)
    return root


def snap_root_to_floor(ob: bpy.types.Object) -> None:
    """Translate so lowest vertex sits on z=0."""
    mesh = ob.data
    min_z = min((ob.matrix_world @ v.co).z for v in mesh.vertices)
    if abs(min_z) < 1e-6:
        return
    ob.location.z -= min_z
    bpy.context.view_layer.update()