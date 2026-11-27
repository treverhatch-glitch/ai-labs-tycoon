"""Era 1 CRT monitor — tapered shell, hollow bezel frame, bulged visible screen."""
from __future__ import annotations

import math

import bmesh
import bpy

import cc0_common as cc
import era1_mesh_selfcheck as sc
import mesh_builders as mb
import pc_tower_kit as tower

FACE_W = 0.40
FACE_H = 0.38
MON_D = 0.40
REAR_W = 0.26
REAR_H = 0.24
SCREEN_W = 0.30
SCREEN_H = 0.23
BEZEL_T = 0.035
RECESS = 0.01
SCREEN_BULGE = 0.01
OVERLAP = 0.004

MESH_SPEC: list[tuple[str, tuple[float, float, float], tuple[float, float, float]]] = []

MAT_BODY = 0
MAT_SCREEN = 1
MAT_LED_GREEN = 2
MAT_POWER = 3

COL_CASE = tower.COL_CASE
COL_BEZEL = tower.COL_BEZEL
COL_SCREEN = (0.10, 0.15, 0.13)
COL_POWER = tower.COL_POWER
COL_LED_PWR = tower.COL_LED_PWR


def _spec(name: str, dims: tuple[float, float, float], loc: tuple[float, float, float]) -> None:
    MESH_SPEC.append((name, dims, loc))


def _tapered_body(name: str, z_center: float) -> bpy.types.Object:
    y_front = -MON_D / 2
    y_rear = MON_D / 2 - 0.010
    mesh = bpy.data.meshes.new(name)
    ob = bpy.data.objects.new(name, mesh)
    bpy.context.collection.objects.link(ob)
    bm = bmesh.new()
    z0 = z_center - FACE_H / 2
    z1 = z_center + FACE_H / 2
    zr0 = z_center - REAR_H / 2 + 0.006
    zr1 = z_center + REAR_H / 2 - 0.006
    wf = FACE_W / 2 - 0.010
    wr = REAR_W / 2 - 0.006
    coords = [
        (-wf, y_front, z0),
        (wf, y_front, z0),
        (wf, y_front, z1),
        (-wf, y_front, z1),
        (-wr, y_rear, zr0),
        (wr, y_rear, zr0),
        (wr, y_rear, zr1),
        (-wr, y_rear, zr1),
    ]
    verts = [bm.verts.new(c) for c in coords]
    bm.faces.new(verts[0:4])
    bm.faces.new(list(reversed(verts[4:8])))
    bm.faces.new([verts[0], verts[1], verts[5], verts[4]])
    bm.faces.new([verts[1], verts[2], verts[6], verts[5]])
    bm.faces.new([verts[2], verts[3], verts[7], verts[6]])
    bm.faces.new([verts[3], verts[0], verts[4], verts[7]])
    bm.normal_update()
    bm.to_mesh(mesh)
    bm.free()
    _spec(name, (FACE_W, MON_D, FACE_H), (0, 0, z_center))
    return ob


def _bezel_frame(z_center: float, y_front: float) -> bpy.types.Object:
    """Outer ~0.40×0.38, opening ~0.30×0.23, 3.5cm thick frame (not a solid slab)."""
    y_outer = y_front - BEZEL_T / 2
    bpy.ops.mesh.primitive_cube_add(size=1, location=(0, y_outer, z_center))
    outer = bpy.context.active_object
    outer.name = "bezel_frame"
    outer.dimensions = (FACE_W, BEZEL_T, FACE_H)
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    _spec("bezel_frame", (FACE_W, BEZEL_T, FACE_H), (0, y_outer, z_center))

    y_cut = y_front - BEZEL_T - 0.004
    bpy.ops.mesh.primitive_cube_add(size=1, location=(0, y_cut, z_center))
    cutter = bpy.context.active_object
    cutter.name = "_bezel_cutter"
    cutter.dimensions = (SCREEN_W, BEZEL_T + 0.012, SCREEN_H)
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)

    mod = outer.modifiers.new("BezelHole", "BOOLEAN")
    mod.operation = "DIFFERENCE"
    mod.object = cutter
    mod.solver = "EXACT"
    bpy.context.view_layer.objects.active = outer
    outer.select_set(True)
    bpy.ops.object.modifier_apply(modifier=mod.name)
    bpy.data.objects.remove(cutter, do_unlink=True)
    return outer


def _crt_screen_panel(z_center: float) -> bpy.types.Object:
    """Single watertight screen slab — glTF x±0.15, y 0.105–0.335, z 0.213–0.218."""
    y_front = -0.218
    y_back = -0.213
    y_mid = (y_front + y_back) / 2.0
    depth = abs(y_back - y_front)
    _spec("screen", (SCREEN_W, depth, SCREEN_H), (0, y_mid, z_center))
    bpy.ops.mesh.primitive_cube_add(size=1, location=(0, y_mid, z_center))
    ob = bpy.context.active_object
    ob.name = "screen"
    ob.dimensions = (SCREEN_W, depth, SCREEN_H)
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    return ob


def _part_mat_index(name: str) -> int:
    if name == "screen" or name.startswith("screen_tab"):
        return MAT_SCREEN
    if name in ("led_pwr", "power_led"):
        return MAT_LED_GREEN
    if name in ("power_btn", "power_button"):
        return MAT_POWER
    return MAT_BODY


def _part_color(name: str) -> tuple[float, float, float]:
    if name == "screen" or name.startswith("screen_tab"):
        return COL_SCREEN
    if name in ("led_pwr", "power_led"):
        return COL_LED_PWR
    if name in ("power_btn", "power_button"):
        return COL_POWER
    if name == "bezel_frame":
        return COL_BEZEL
    return COL_CASE


def mesh_spec_report() -> str:
    lines = ["| Object | Wx×D×H (m) | center (x,y,z) |", "|--------|-----------|----------------|"]
    for name, dims, loc in MESH_SPEC:
        lines.append(f"| {name} | {dims[0]:.3f}×{dims[1]:.3f}×{dims[2]:.3f} | ({loc[0]:.3f}, {loc[1]:.3f}, {loc[2]:.3f}) |")
    lines.append("\n*Parent: `body_tapered` → join, floor z=0.*")
    return "\n".join(lines)


def build_monitor_era1() -> list[bpy.types.Object]:
    """Return separate export meshes: shell, screen, power_button, power_led."""
    MESH_SPEC.clear()
    z_body = 0.22
    y_front = -MON_D / 2

    shell_parts: list[bpy.types.Object] = []
    shell_parts.append(_tapered_body("body_tapered", z_body))
    shell_parts.append(_bezel_frame(z_body, y_front))
    shell_parts.append(_overlap_cylinder("base_foot", 0.11, 0.024, (0, 0, 0.012)))
    shell_parts.append(_overlap_box("neck", (0.082, 0.082, 0.090), (0, 0, 0.067)))

    screen = _crt_screen_panel(z_body)
    screen.name = "screen"

    # glTF Y-up: vertical ≈ Blender Z; depth (front) ≈ -Blender Y.
    y_depth = -0.2375
    bz = 0.065
    power_led = _overlap_box("power_led", (0.006, 0.005, 0.006), (0.13, y_depth, bz))
    power_button = _overlap_box("power_button", (0.014, 0.005, 0.008), (0.16, y_depth, bz))

    sc.assert_monitor_parts(shell_parts + [screen, power_button, power_led], root_name="body_tapered")

    slot_mats = [bpy.data.materials.new(f"_mon_slot_{i}") for i in range(4)]
    for ob in shell_parts + [screen, power_button, power_led]:
        if ob.name not in ("screen", "bezel_frame"):
            tower._bevel(ob, 0.003, segments=1)
        if ob.name != "screen":
            tower._paint_part_faces(ob, _part_color(ob.name))
    tower._apply_global_material_slots(shell_parts + [screen, power_button, power_led], slot_mats, _part_mat_index)

    shell = mb._union_parts(shell_parts)
    cc.apply_transforms(shell)
    _recess_screen_opening_verts(shell, z_body)
    _clamp_rear_overhang(shell)
    tower._fix_tower_normals(shell)
    shell.name = "body_tapered"
    _seal_bezel_mesh(shell)

    _weld_screen_export(screen)
    tower._fix_tower_normals(screen)
    return [shell, screen, power_button, power_led]


def _recess_screen_opening_verts(ob: bpy.types.Object, z_center: float) -> None:
    """Push interior shell verts behind glTF z=0.205 (no face delete — keeps manifold)."""
    x_half = SCREEN_W / 2 - 0.003
    z0 = z_center - SCREEN_H / 2
    z1 = z_center + SCREEN_H / 2
    y_cut = -0.205
    for v in ob.data.vertices:
        if -v.co.y <= 0.205:
            continue
        if abs(v.co.x) <= x_half and z0 <= v.co.z <= z1:
            if v.co.y < y_cut:
                v.co.y = y_cut
    ob.data.update()


def _weld_screen_export(ob: bpy.types.Object) -> None:
    """8-vert watertight slab: weld then single UV/normal corner per position for glTF."""
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    mod = ob.modifiers.new("_weld", "WELD")
    mod.merge_threshold = _WELD_DIST
    bpy.ops.object.modifier_apply(modifier=mod.name)
    bpy.ops.object.shade_smooth()
    if ob.data.uv_layers:
        uv_layer = ob.data.uv_layers.active.data
        for loop in uv_layer:
            loop.uv = (0.5, 0.5)
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=_WELD_DIST)
    bpy.ops.mesh.normals_make_consistent(inside=False)
    bpy.ops.object.mode_set(mode="OBJECT")
    ob.data.use_auto_smooth = True
    ob.data.auto_smooth_angle = math.radians(180)


def _clamp_rear_overhang(ob: bpy.types.Object) -> None:
    """GLB rear band z<0.215: keep within bezel outline (|x|≤0.185, y≤0.395)."""
    mesh = ob.data
    for v in mesh.vertices:
        z_g = -v.co.y
        y_g = v.co.z
        if z_g < 0.215:
            if abs(v.co.x) > 0.185:
                v.co.x = 0.185 if v.co.x > 0 else -0.185
            if y_g > 0.395:
                v.co.z = 0.395
    mesh.update()


def _rear_housing(z_center: float, y_front: float) -> bpy.types.Object:
    """Closed tapered rear solid from just behind bezel — no sheet overhang."""
    y0 = y_front + BEZEL_T * 0.35
    y1 = MON_D / 2 - 0.006
    ym = (y0 + y1) / 2
    depth = y1 - y0
    mesh = bpy.data.meshes.new("rear_housing")
    ob = bpy.data.objects.new("rear_housing", mesh)
    bpy.context.collection.objects.link(ob)
    bm = bmesh.new()
    z0 = z_center - FACE_H / 2 + 0.008
    z1 = z_center + FACE_H / 2 - 0.008
    zr0 = z_center - REAR_H / 2 + 0.006
    zr1 = z_center + REAR_H / 2 - 0.006
    wf = FACE_W / 2 - 0.014
    wr = REAR_W / 2 - 0.008
    coords = [
        (-wf, y0, z0),
        (wf, y0, z0),
        (wf, y0, z1),
        (-wf, y0, z1),
        (-wr, y1, zr0),
        (wr, y1, zr0),
        (wr, y1, zr1),
        (-wr, y1, zr1),
    ]
    verts = [bm.verts.new(c) for c in coords]
    bm.faces.new(verts[0:4])
    bm.faces.new(list(reversed(verts[4:8])))
    for quad in ((0, 1, 5, 4), (1, 2, 6, 5), (2, 3, 7, 6), (3, 0, 4, 7)):
        bm.faces.new([verts[i] for i in quad])
    bm.normal_update()
    bm.to_mesh(mesh)
    bm.free()
    _spec("rear_housing", (FACE_W - 0.028, depth, FACE_H - 0.016), (0, ym, z_center))
    return ob


def _groove_box(name: str, dims: tuple[float, float, float], loc: tuple[float, float, float]) -> bpy.types.Object:
    _spec(name, dims, loc)
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = dims
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    return ob


def _overlap_box(name: str, dims: tuple[float, float, float], loc: tuple[float, float, float]) -> bpy.types.Object:
    _spec(name, dims, loc)
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = dims
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    return ob


def _overlap_cylinder(name: str, radius: float, depth: float, loc: tuple[float, float, float]) -> bpy.types.Object:
    _spec(name, (radius * 2, radius * 2, depth), loc)
    bpy.ops.mesh.primitive_cylinder_add(radius=radius, depth=depth, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    return ob


def _parent_apply(parts: list[bpy.types.Object], root_name: str) -> None:
    root = next(p for p in parts if p.name == root_name)
    for p in parts:
        if p != root:
            p.parent = root
    bpy.ops.object.select_all(action="DESELECT")
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = root
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)


_BEZEL_PINHOLES_GLTF = [(-0.057, 0.354), (-0.066, 0.07), (0.09, 0.067)]
_WELD_DIST = 0.0005


def _seal_bezel_mesh(root: bpy.types.Object) -> None:
    """Merge-by-distance and close T-junctions on the front bezel band (glTF z > 0.225)."""
    bm = bmesh.new()
    bm.from_mesh(root.data)
    front = [v for v in bm.verts if -v.co.y > 0.225]
    if front:
        bmesh.ops.remove_doubles(bm, verts=front, dist=0.003)
    bmesh.ops.remove_doubles(bm, verts=bm.verts, dist=_WELD_DIST)
    for x_g, y_g in _BEZEL_PINHOLES_GLTF:
        near = [
            v
            for v in bm.verts
            if -v.co.y > 0.225 and (v.co.x - x_g) ** 2 + (v.co.z - y_g) ** 2 < (0.014 * 0.014)
        ]
        if len(near) > 1:
            bmesh.ops.remove_doubles(bm, verts=near, dist=0.006)
    bmesh.ops.remove_doubles(bm, verts=bm.verts, dist=_WELD_DIST)
    bm.normal_update()
    bm.to_mesh(root.data)
    bm.free()
    root.data.update()


def _final_weld(root: bpy.types.Object) -> None:
    bpy.context.view_layer.objects.active = root
    root.select_set(True)
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=_WELD_DIST)
    bpy.ops.object.mode_set(mode="OBJECT")