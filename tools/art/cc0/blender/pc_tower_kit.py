"""Era 1 PC tower — ONE case body + front-face extrusions (no separate bezel slab)."""
from __future__ import annotations

import math

import bpy

import cc0_common as cc
import mesh_builders as mb

TOWER_W = 0.20
TOWER_D = 0.45
TOWER_H = 0.45
PROTRUDE = 0.008  # 8 mm front extrusion
CASE_BEVEL = 0.006  # 6 mm case edges
DETAIL_BEVEL = 0.002  # 2 mm detail edges
BEZEL_INSET = 0.003  # 3 mm recessed drive panel

MESH_SPEC: list[tuple[str, tuple[float, float, float], tuple[float, float, float]]] = []
MATERIAL_SPEC: list[tuple[str, str]] = []

# Material slot indices (glTF sub-materials)
MAT_BODY = 0
MAT_BEZEL = 1
MAT_LED_GREEN = 2
MAT_LED_AMBER = 3
MAT_POWER = 4

COL_CASE = (0.86, 0.82, 0.72)
COL_BEZEL = (0.62, 0.58, 0.52)
COL_BAY = (0.48, 0.44, 0.40)
COL_SLOT = (0.28, 0.26, 0.24)
COL_POWER = (0.58, 0.54, 0.48)
COL_LED_PWR = (0.15, 0.92, 0.35)
COL_LED_HDD = (0.98, 0.62, 0.12)
COL_BADGE = (0.36, 0.34, 0.32)
COL_SIDE_SEAM = (0.78, 0.74, 0.66)


def _raw_box(name: str, dims: tuple[float, float, float], loc: tuple[float, float, float]) -> bpy.types.Object:
    MESH_SPEC.append((name, dims, loc))
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = dims
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    return ob


def _raw_cylinder(name: str, radius: float, depth: float, loc: tuple[float, float, float]) -> bpy.types.Object:
    MESH_SPEC.append((name, (radius * 2, radius * 2, depth), loc))
    bpy.ops.mesh.primitive_cylinder_add(radius=radius, depth=depth, location=loc)
    ob = bpy.context.active_object
    ob.name = name
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    return ob


def _bevel(ob: bpy.types.Object, width: float, segments: int = 1) -> None:
    if width <= 0:
        return
    mod = ob.modifiers.new("Bevel", type="BEVEL")
    mod.width = width
    mod.segments = segments
    mod.limit_method = "ANGLE"
    mod.angle_limit = math.radians(60)
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.modifier_apply(modifier=mod.name)


def _part_mat_index(name: str) -> int:
    if name == "led_pwr":
        return MAT_LED_GREEN
    if name == "led_hdd":
        return MAT_LED_AMBER
    if name == "power_btn":
        return MAT_POWER
    if name in ("bay_upper", "bay_lower", "floppy", "cd_slot", "eject_btn", "vent_bar", "bezel_band", "badge_plate"):
        return MAT_BEZEL
    return MAT_BODY


def _part_face_color(name: str) -> tuple[float, float, float]:
    if name == "led_pwr":
        return COL_LED_PWR
    if name == "led_hdd":
        return COL_LED_HDD
    if name == "power_btn":
        return COL_POWER
    if name in ("bay_upper", "bay_lower"):
        return COL_BAY
    if name in ("cd_slot", "floppy", "vent_bar", "eject_btn"):
        return COL_SLOT
    if name == "badge_plate":
        return COL_BADGE
    if name in ("bezel_band",):
        return COL_BEZEL
    if name in ("side_seam",):
        return COL_SIDE_SEAM
    if name in ("side_vent",):
        return COL_SLOT
    return COL_CASE


def _paint_part_faces(ob: bpy.types.Object, rgb: tuple[float, float, float]) -> None:
    mesh = ob.data
    if mesh.color_attributes.get("BakeColor"):
        mesh.color_attributes.remove(mesh.color_attributes["BakeColor"])
    vcol = mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    col = (rgb[0], rgb[1], rgb[2], 1.0)
    for poly in mesh.polygons:
        for li in poly.loop_indices:
            vcol.data[li].color = col


def _assign_part_materials(ob: bpy.types.Object, name: str) -> None:
    idx = _part_mat_index(name)
    while len(ob.data.materials) <= idx:
        ob.data.materials.append(None)
    for poly in ob.data.polygons:
        poly.material_index = idx


def mesh_spec_report() -> str:
    lines = ["| Mesh | WxDxH (m) | center (x,y,z) |", "|------|-----------|----------------|"]
    for name, dims, loc in MESH_SPEC:
        lines.append(f"| {name} | {dims[0]:.3f}×{dims[1]:.3f}×{dims[2]:.3f} | ({loc[0]:.3f}, {loc[1]:.3f}, {loc[2]:.3f}) |")
    return "\n".join(lines)


def material_spec_report() -> str:
    lines = ["| Slot | Role |", "|------|------|"]
    for slot, role in MATERIAL_SPEC:
        lines.append(f"| {slot} | {role} |")
    return "\n".join(lines)


def _split_material_seams(ob: bpy.types.Object) -> None:
    import bmesh

    mesh = ob.data
    bm = bmesh.new()
    bm.from_mesh(mesh)
    split = [
        e
        for e in bm.edges
        if len(e.link_faces) == 2 and e.link_faces[0].material_index != e.link_faces[1].material_index
    ]
    if split:
        bmesh.ops.split_edges(bm, edges=split)
    bm.to_mesh(mesh)
    bm.free()
    mesh.update()


def _fix_tower_normals(ob: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.normals_make_consistent(inside=False)
    bpy.ops.mesh.faces_shade_smooth()
    bpy.ops.object.mode_set(mode="OBJECT")
    if hasattr(ob.data, "use_auto_smooth"):
        ob.data.use_auto_smooth = True
        ob.data.auto_smooth_angle = math.radians(89.5)


def _merge_face_colors(root: bpy.types.Object) -> None:
    """Per-face CORNER colors after material seam split (no LED/teal bleed)."""
    mesh = root.data
    while mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[0])
    vcol = mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    part_map = {
        MAT_LED_GREEN: COL_LED_PWR,
        MAT_LED_AMBER: COL_LED_HDD,
        MAT_POWER: COL_POWER,
        MAT_BEZEL: COL_BEZEL,
        MAT_BODY: COL_CASE,
    }
    for poly in mesh.polygons:
        mi = poly.material_index
        c = part_map.get(mi, COL_CASE)
        if mi == MAT_BEZEL and poly.normal.y < -0.7:
            zs = [mesh.vertices[vi].co.z for vi in poly.vertices]
            if sum(zs) / max(len(zs), 1) > 0.32:
                c = COL_BAY
        if mi == MAT_BODY and abs(poly.normal.x) > 0.85:
            ys = [mesh.vertices[vi].co.y for vi in poly.vertices]
            ymid = sum(ys) / max(len(ys), 1)
            if abs(ymid + 0.210) < 0.003 and poly.normal.x > 0:
                c = COL_SIDE_SEAM
        col = (c[0], c[1], c[2], 1.0)
        for li in poly.loop_indices:
            vcol.data[li].color = col


def build_pc_tower_cc0() -> bpy.types.Object:
    """Local axes: width X, depth Y, height Z. **Front = −Y**."""
    MESH_SPEC.clear()
    MATERIAL_SPEC.clear()
    MATERIAL_SPEC.extend(
        [
            (str(MAT_BODY), "beige case body (sides/top)"),
            (str(MAT_BEZEL), "grey-beige bezel / drive panel"),
            (str(MAT_LED_GREEN), "emissive power LED"),
            (str(MAT_LED_AMBER), "emissive HDD LED"),
            (str(MAT_POWER), "dark grey-beige power cylinder"),
        ]
    )

    hh = TOWER_H / 2
    y0 = -TOWER_D / 2
    yc = y0 - PROTRUDE / 2
    y_inset = y0 - BEZEL_INSET - 0.0015

    parts: list[bpy.types.Object] = []
    case = _raw_box("case_body", (TOWER_W, TOWER_D, TOWER_H), (0.0, 0.0, hh))
    _bevel(case, CASE_BEVEL, segments=1)
    parts.append(case)

    parts.append(_raw_box("bezel_band", (0.19, BEZEL_INSET, 0.14), (0.0, y_inset, 0.355)))

    parts.append(_raw_box("bay_upper", (0.18, PROTRUDE, 0.040), (0.0, yc, 0.392)))
    parts.append(_raw_box("bay_lower", (0.18, PROTRUDE, 0.040), (0.0, yc, 0.342)))
    parts.append(_raw_box("floppy", (0.10, PROTRUDE, 0.010), (0.0, yc, 0.302)))
    parts.append(_raw_box("badge_plate", (0.04, PROTRUDE, 0.012), (0.0, yc, 0.282)))
    parts.append(_raw_box("cd_slot", (0.12, PROTRUDE, 0.012), (0.02, yc, 0.385)))
    parts.append(_raw_box("eject_btn", (0.028, PROTRUDE, 0.028), (0.07, yc, 0.385)))
    parts.append(_raw_cylinder("power_btn", 0.020, 0.006, (-0.06, yc, 0.190)))
    parts.append(_raw_box("led_pwr", (0.012, PROTRUDE, 0.012), (-0.02, yc, 0.190)))
    parts.append(_raw_box("led_hdd", (0.012, PROTRUDE, 0.012), (0.01, yc, 0.190)))
    parts.append(_raw_box("vent_bar", (0.12, PROTRUDE, 0.008), (0.0, yc, 0.088)))

    xc = TOWER_W / 2 - 0.0005
    parts.append(_raw_box("side_vent", (0.002, 0.055, 0.042), (xc, TOWER_D / 2 - 0.048, 0.052)))

    for ob in parts:
        if ob.name != "case_body":
            _bevel(ob, DETAIL_BEVEL, segments=1)
        _paint_part_faces(ob, _part_face_color(ob.name))
        _assign_part_materials(ob, ob.name)

    root = cc.join_meshes(parts)
    cc.apply_transforms(root)
    _fix_tower_normals(root)
    _split_material_seams(root)
    _merge_face_colors(root)
    mb.snap_root_to_floor(root)
    return root