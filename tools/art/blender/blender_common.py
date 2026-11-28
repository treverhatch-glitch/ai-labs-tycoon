"""
Shared helpers for headless Blender early-game asset scripts (import from repo; not run alone).
"""
from __future__ import annotations

import colorsys
import json
import math
import os
from pathlib import Path

import bpy
from mathutils import Matrix, Vector

REPO_ROOT = Path(__file__).resolve().parents[3]
ART_ROOT = REPO_ROOT / "unity/Assets/Art/EarlyGame"
PREVIEW_ROOT = REPO_ROOT / "docs/art/previews"
BLENDER_VERSION = f"{bpy.app.version[0]}.{bpy.app.version[1]}.{bpy.app.version[2]}"


def reset_scene() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.context.scene.render.engine = "CYCLES"
    bpy.context.scene.cycles.device = "CPU"
    bpy.context.scene.cycles.samples = 32
    bpy.context.scene.cycles.use_denoising = False
    if bpy.context.preferences.addons.get("cycles") is None:
        bpy.ops.preferences.addon_enable(module="cycles")


def link_or_join(objects: list[bpy.types.Object]) -> bpy.types.Object:
    if not objects:
        raise RuntimeError("no objects to join")
    bpy.ops.object.select_all(action="DESELECT")
    for ob in objects:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = objects[0]
    if len(objects) > 1:
        bpy.ops.object.join()
    return bpy.context.view_layer.objects.active


def add_cube(name: str, size: tuple[float, float, float], location: tuple[float, float, float] = (0, 0, 0)) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1, location=location)
    ob = bpy.context.active_object
    ob.name = name
    ob.scale = (size[0] / 2, size[1] / 2, size[2] / 2)
    bpy.ops.object.transform_apply(scale=True)
    return ob


def add_cylinder(
    name: str,
    radius: float,
    depth: float,
    location: tuple[float, float, float] = (0, 0, 0),
    vertices: int = 16,
    axis: str = "Y",
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cylinder_add(
        radius=radius, depth=depth, vertices=vertices, location=location
    )
    ob = bpy.context.active_object
    ob.name = name
    if axis.upper() == "Y":
        ob.rotation_euler = (math.pi / 2, 0.0, 0.0)
        bpy.ops.object.transform_apply(rotation=True)
    return ob


def bevel_object(ob: bpy.types.Object, width: float = 0.015, segments: int = 2) -> None:
    mod = ob.modifiers.new("Bevel", type="BEVEL")
    mod.width = width
    mod.segments = segments
    mod.limit_method = "ANGLE"
    mod.angle_limit = math.radians(60)
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.modifier_apply(modifier=mod.name)


def solidify_object(ob: bpy.types.Object, thickness: float = 0.02) -> None:
    mod = ob.modifiers.new("Solidify", type="SOLIDIFY")
    mod.thickness = thickness
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.modifier_apply(modifier=mod.name)


def mesh_world_min_y(ob: bpy.types.Object) -> float:
    return min((ob.matrix_world @ v.co).y for v in ob.data.vertices)


def mesh_world_min_z(ob: bpy.types.Object) -> float:
    return min((ob.matrix_world @ v.co).z for v in ob.data.vertices)


def mesh_world_min_y(ob: bpy.types.Object) -> float:
    """Garage kit scripts use Blender Y as vertical (see ground_scene_meshes)."""
    return min((ob.matrix_world @ v.co).y for v in ob.data.vertices)


def ground_scene_meshes() -> None:
    """Early garage scripts use Blender Y as up; rest on y=0."""
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        return
    min_y = min((o.matrix_world @ Vector(c)).y for o in meshes for c in o.bound_box)
    if min_y >= -1e-5:
        return
    dy = -min_y
    for o in meshes:
        if o.parent is not None:
            continue
        o.location.y += dy


def ensure_staff_upright(ob: bpy.types.Object) -> None:
    """Y is up: tallest bbox axis must be Y; head (+Y) above feet."""
    import bmesh

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    if not bm.verts:
        bm.free()
        return
    co = [ob.matrix_world @ v.co for v in bm.verts]
    bm.free()
    xs = [p.x for p in co]
    ys = [p.y for p in co]
    zs = [p.z for p in co]
    span_x, span_y, span_z = max(xs) - min(xs), max(ys) - min(ys), max(zs) - min(zs)
    if span_y < max(span_x, span_z) * 0.98:
        raise RuntimeError(
            f"FAIL {ob.name}: not Y-up span_y={span_y:.3f} span_x={span_x:.3f} span_z={span_z:.3f}"
        )
    if max(ys) - min(ys) < 0.35:
        raise RuntimeError(f"FAIL {ob.name}: staff too short height={span_y:.3f}")


def snap_root_to_ground(ob: bpy.types.Object) -> None:
    """Rest lowest vertex on y=0 (art/garage Y-up lane)."""
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    min_y = mesh_world_min_y(ob)
    if abs(min_y) > 1e-5:
        ob.location.y -= min_y
    bpy.context.view_layer.update()


def snap_root_to_ground_z(ob: bpy.types.Object) -> None:
    """After art-Y → Blender Z-up reorient, rest feet on z=0."""
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    min_z = mesh_world_min_z(ob)
    if abs(min_z) > 1e-5:
        ob.location.z -= min_z
    bpy.context.view_layer.update()


def reorient_staff_mesh_y_to_z(ob: bpy.types.Object) -> None:
    """Art scripts build Y-up; Blender/glTF/Unity need height on Z before export."""
    import bmesh
    from mathutils import Matrix

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    rot = Matrix.Rotation(math.pi / 2, 4, "X")
    bmesh.ops.rotate(bm, cent=(0, 0, 0), matrix=rot, verts=bm.verts)
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()
    ob.rotation_euler = (0.0, 0.0, 0.0)
    ob.location = (0.0, 0.0, 0.0)


def _mesh_scene_span_z(mesh_obs: list[bpy.types.Object]) -> tuple[float, float, float]:
    min_z, max_z = 1e9, -1e9
    for ob in mesh_obs:
        if ob.type != "MESH":
            continue
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            min_z = min(min_z, w.z)
            max_z = max(max_z, w.z)
    return min_z, max_z, max_z - min_z


def orient_staff_assembly_for_export(
    mesh_obs: list[bpy.types.Object], asset_id: str
) -> bpy.types.Object:
    """Y-up parts → parented assembly, rotate root to Z-up ~1.72 m for multi-mesh glTF."""
    import staff_connectivity as staff_conn

    root = staff_conn.parent_staff_hierarchy(mesh_obs)
    ground_scene_meshes()
    bpy.context.view_layer.update()
    if asset_id != "staff-founder-idle":
        root.rotation_euler = (math.pi / 2, 0.0, 0.0)
        bpy.ops.object.select_all(action="DESELECT")
        root.select_set(True)
        bpy.context.view_layer.objects.active = root
        bpy.ops.object.transform_apply(rotation=True)
        bpy.context.view_layer.update()
    _min, _max, sz = _mesh_scene_span_z(mesh_obs)
    if sz < 0.35:
        raise RuntimeError(f"FAIL {asset_id}: height too small pre-scale z={sz:.3f}")
    target_h = 1.72
    scale = target_h / sz
    if abs(scale - 1.0) > 0.02:
        root.scale = (scale, scale, scale)
        bpy.ops.object.transform_apply(scale=True)
    bpy.context.view_layer.update()
    min_z, _max, sz = _mesh_scene_span_z(mesh_obs)
    if abs(min_z) > 1e-5:
        root.location.z -= min_z
    bpy.context.view_layer.update()
    _min, _max, sz = _mesh_scene_span_z(mesh_obs)
    if sz < 1.55 or sz > 2.05:
        raise RuntimeError(f"FAIL {asset_id}: height sz={sz:.3f} want 1.55-2.05")
    return root


def orient_staff_for_gltf(ob: bpy.types.Object, asset_id: str) -> None:
    """Rotate art Y-up mesh to Blender Z-up, scale to ~1.72 m, ground, then export."""
    ensure_staff_upright(ob)
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    reorient_staff_mesh_y_to_z(ob)
    verts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    zs = [p.z for p in verts]
    height = max(zs) - min(zs)
    target_h = 1.72
    if height < 0.35:
        raise RuntimeError(f"FAIL {asset_id}: height too small pre-scale z={height:.3f}")
    scale = target_h / height
    if abs(scale - 1.0) > 0.02:
        ob.scale = (scale, scale, scale)
        bpy.ops.object.transform_apply(scale=True)
    snap_root_to_ground_z(ob)
    verts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    xs = [p.x for p in verts]
    ys = [p.y for p in verts]
    zs = [p.z for p in verts]
    sx, sy, sz = max(xs) - min(xs), max(ys) - min(ys), max(zs) - min(zs)
    if sz < max(sx, sy) * 0.9:
        raise RuntimeError(
            f"FAIL {asset_id}: gltf-upright sz={sz:.3f} sx={sx:.3f} sy={sy:.3f}"
        )
    if sz < 1.55 or sz > 2.05:
        raise RuntimeError(f"FAIL {asset_id}: height sz={sz:.3f} want 1.55-2.05")


def voxel_shell_merge(ob: bpy.types.Object, voxel_size: float = 0.03) -> None:
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    mod = ob.modifiers.new("VoxelShell", "REMESH")
    mod.mode = "VOXEL"
    mod.voxel_size = voxel_size
    mod.use_smooth_shade = True
    bpy.ops.object.modifier_apply(modifier=mod.name)


def add_prejoin_spine(mesh_obs: list[bpy.types.Object], *, staff: bool = False) -> None:
    """Tall core strut before join so multi-part kits share one island."""
    if not mesh_obs:
        return
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in mesh_obs:
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins = Vector((min(mins.x, w.x), min(mins.y, w.y), min(mins.z, w.z)))
            maxs = Vector((max(maxs.x, w.x), max(maxs.y, w.y), max(maxs.z, w.z)))
    height = max(0.4, maxs.y - mins.y)
    span_z = max(0.14, (maxs.z - mins.z) * 0.72)
    center = (mins + maxs) * 0.5
    bpy.ops.mesh.primitive_cube_add(size=1, location=(center.x, (mins.y + maxs.y) * 0.5, center.z))
    spine = bpy.context.active_object
    spine.name = "PrejoinSpine"
    widen = 0.96 if staff else 0.42
    ht = 0.96 if staff else 0.55
    span_x = max(0.16, (maxs.x - mins.x) * widen)
    depth = max(0.14, (maxs.z - mins.z) * (0.72 if not staff else 0.88))
    spine.scale = (span_x, height * ht, depth)
    bpy.ops.object.transform_apply(scale=True)
    mesh_obs.append(spine)


def add_connecting_spine(ob: bpy.types.Object, *, staff: bool = False) -> None:
    """Hidden internal strut so multi-part kits are one manifold island."""
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for corner in ob.bound_box:
        w = ob.matrix_world @ Vector(corner)
        mins = Vector((min(mins.x, w.x), min(mins.y, w.y), min(mins.z, w.z)))
        maxs = Vector((max(maxs.x, w.x), max(maxs.y, w.y), max(maxs.z, w.z)))
    center = (mins + maxs) * 0.5
    height = max(0.1, maxs.y - mins.y)
    bpy.ops.mesh.primitive_cube_add(size=1, location=(center.x, (mins.y + maxs.y) * 0.5, center.z))
    spine = bpy.context.active_object
    spine.name = "ConnectSpine"
    widen = 0.94 if staff else 0.45
    ht = 0.94 if staff else 0.5
    span_x = max(0.08, (maxs.x - mins.x) * widen)
    span_z = max(0.08, (maxs.z - mins.z) * widen)
    spine.scale = (span_x, height * ht + 0.02, span_z)
    bpy.ops.object.transform_apply(scale=True)
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    spine.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.join()
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=0.02)
    bpy.ops.object.mode_set(mode="OBJECT")


def _island_vertex_sets(ob: bpy.types.Object) -> list[set[int]]:
    import bmesh

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.verts.ensure_lookup_table()
    bm.faces.ensure_lookup_table()
    visited: set[int] = set()
    islands: list[set[int]] = []
    for face in bm.faces:
        if face.index in visited:
            continue
        vert_ids: set[int] = set()
        stack = [face]
        while stack:
            f = stack.pop()
            if f.index in visited:
                continue
            visited.add(f.index)
            for v in f.verts:
                vert_ids.add(v.index)
            for e in f.edges:
                for linked in e.link_faces:
                    if linked.index not in visited:
                        stack.append(linked)
        islands.append(vert_ids)
    bm.free()
    return islands


def _island_centroid(ob: bpy.types.Object, vert_ids: set[int]) -> Vector:
    import bmesh

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.verts.ensure_lookup_table()
    pts = [ob.matrix_world @ bm.verts[i].co for i in vert_ids if i < len(bm.verts)]
    bm.free()
    if not pts:
        return ob.matrix_world @ Vector((0.0, 0.0, 0.0))
    return sum(pts, Vector()) / len(pts)


def _add_bridge_cube_between(ob: bpy.types.Object, a: Vector, b: Vector) -> None:
    delta = b - a
    length = delta.length
    if length < 1e-4:
        return
    mid = (a + b) * 0.5
    thick = max(0.12, min(0.22, length * 0.15))
    bpy.ops.mesh.primitive_cube_add(size=1, location=mid)
    bridge = bpy.context.active_object
    bridge.name = "StaffBridge"
    bridge.scale = (thick, thick, length * 0.55)
    bridge.rotation_euler = delta.to_track_quat("Z", "Y").to_euler()
    bpy.ops.object.transform_apply(scale=True, rotation=True)
    bridge.display_type = "BOUNDS"
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bridge.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.join()
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=0.008)
    bpy.ops.object.mode_set(mode="OBJECT")


def staff_union_mesh_parts(mesh_obs: list[bpy.types.Object], asset_id: str) -> bpy.types.Object:
    """Boolean-union chibi parts into one manifold mesh (preserves part detail)."""
    if not mesh_obs:
        raise RuntimeError(f"FAIL {asset_id}: no mesh parts")
    if len(mesh_obs) == 1:
        return mesh_obs[0]
    base = max(mesh_obs, key=lambda o: len(o.data.polygons))
    bpy.context.view_layer.objects.active = base
    base.select_set(True)
    for ob in mesh_obs:
        if ob == base:
            continue
        bpy.ops.object.select_all(action="DESELECT")
        base.select_set(True)
        ob.select_set(True)
        bpy.context.view_layer.objects.active = base
        mod = base.modifiers.new(name=f"Union_{ob.name}", type="BOOLEAN")
        mod.operation = "UNION"
        mod.object = ob
        mod.solver = "FAST"
        try:
            bpy.ops.object.modifier_apply(modifier=mod.name)
        except RuntimeError as exc:
            raise RuntimeError(f"FAIL {asset_id}: boolean union {ob.name}: {exc}") from exc
        bpy.data.objects.remove(ob, do_unlink=True)
    islands = mesh_island_count(base)
    if islands > 2:
        add_connecting_spine(base, staff=True)
        islands = mesh_island_count(base)
    if islands > 2:
        raise RuntimeError(f"FAIL {asset_id}: union islands={islands}")
    return base


def mesh_island_count(ob: bpy.types.Object) -> int:
    import bmesh

    bm = bmesh.new()
    bm.from_mesh(ob.data)
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
                for linked in e.link_faces:
                    if linked.index not in visited:
                        stack.append(linked)
    bm.free()
    return islands


def uv_unwrap(ob: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.uv.smart_project(angle_limit=66, island_margin=0.02)
    bpy.ops.object.mode_set(mode="OBJECT")


def flatten_uv_for_flat_color_bake(ob: bpy.types.Object) -> None:
    """Replace Kenney atlas UVs with few large islands for solid-color emit bake."""
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.uv.smart_project(angle_limit=50, island_margin=0.004)
    bpy.ops.object.mode_set(mode="OBJECT")


def garage_glb_y_up_preview(asset_id: str) -> bool:
    """Y-up Kenney GLBs use Y-up hero; procedural cable-spool + thin slabs use legacy Z hero camera."""
    return asset_id not in {
        "garage-cable-spool",
        "garage-lot-curb",
        "garage-floor-tile",
    }


GARAGE_FURNITURE = frozenset(
    {
        "garage-crt-monitor",
        "garage-desk-crt",
        "garage-folding-chair",
        "garage-server-closet",
        "garage-voodude-rack",
        "garage-coffee-drip",
        "garage-crt-tower",
        "garage-roll-door",
    }
)
MIN_GARAGE_PARTS_SMALL = 5
MIN_GARAGE_PARTS_FURNITURE = 6


def apply_object_transforms(ob: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)


def flatten_glb_preview_meshes(meshes: list[bpy.types.Object]) -> list[bpy.types.Object]:
    """Bake GLB parent hierarchy to world matrices for hero/ortho preview renders."""
    bpy.context.view_layer.update()
    flat: list[bpy.types.Object] = []
    for ob in meshes:
        mw = ob.matrix_world.copy()
        if ob.parent is not None:
            ob.parent = None
        ob.matrix_world = mw
        flat.append(ob)
    bpy.context.view_layer.update()
    return flat


def _desk_crt_world_bbox(ob: bpy.types.Object) -> tuple[Vector, Vector]:
    lo = Vector((1e9, 1e9, 1e9))
    hi = Vector((-1e9, -1e9, -1e9))
    for corner in ob.bound_box:
        w = ob.matrix_world @ Vector(corner)
        lo.x, lo.y, lo.z = min(lo.x, w.x), min(lo.y, w.y), min(lo.z, w.z)
        hi.x, hi.y, hi.z = max(hi.x, w.x), max(hi.y, w.y), max(hi.z, w.z)
    return lo, hi


def desk_crt_on_desktop_bbox_metrics(meshes: list[bpy.types.Object]) -> dict[str, float]:
    """World + projected bbox checks for hero and side-ortho cameras (PASS25 printout)."""
    from bpy_extras.object_utils import world_to_camera_view

    import cc0_common as cc0  # type: ignore

    desk = next((o for o in meshes if "desktop" in o.name.lower()), None)
    crt = next((o for o in meshes if "crt" in o.name.lower()), None)
    if desk is None or crt is None:
        raise RuntimeError("desk-crt preview gate: missing Desktop or CRT mesh in GLB import")

    d_lo, d_hi = _desk_crt_world_bbox(desk)
    c_lo, c_hi = _desk_crt_world_bbox(crt)
    metrics: dict[str, float] = {
        "world_desk_top_y": float(d_hi.y),
        "world_crt_base_y": float(c_lo.y),
        "world_gap_y": float(c_lo.y - d_hi.y),
        "world_crt_x0": float(c_lo.x),
        "world_crt_x1": float(c_hi.x),
        "world_desk_x0": float(d_lo.x),
        "world_desk_x1": float(d_hi.x),
    }
    if c_lo.y < d_hi.y - 0.012:
        raise RuntimeError(
            f"desk-crt preview gate: CRT base y={c_lo.y:.4f} below desktop top y={d_hi.y:.4f}"
        )
    tol_xz = 0.03
    if c_lo.x < d_lo.x - tol_xz or c_hi.x > d_hi.x + tol_xz:
        raise RuntimeError("desk-crt preview gate: CRT X extent outside desktop top")
    if c_lo.z < d_lo.z - tol_xz or c_hi.z > d_hi.z + tol_xz:
        raise RuntimeError("desk-crt preview gate: CRT Z extent outside desktop top")

    frame_obs = meshes
    center, _ = cc0._bounds_center(frame_obs)
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in frame_obs:
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins.x, mins.y, mins.z = min(mins.x, w.x), min(mins.y, w.y), min(mins.z, w.z)
            maxs.x, maxs.y, maxs.z = max(maxs.x, w.x), max(maxs.y, w.y), max(maxs.z, w.z)
    span = max(max(maxs.x - mins.x, maxs.z - mins.z, 0.1), max(maxs.y - mins.y, 0.1) * 0.95, 0.18)
    span_y = max(maxs.y - mins.y, 0.1)
    lo_all, hi_all = mins, maxs
    ortho_center = (lo_all + hi_all) * 0.5

    def _project_with_cam(cam_ob: bpy.types.Object, scene: bpy.types.Scene, pt: Vector) -> Vector:
        co = world_to_camera_view(scene, cam_ob, pt)
        return Vector((co.x, co.y, co.z))

    # Hero camera (matches render_kit_preview Y-up hero path)
    cam_cfg = GARAGE_HERO_CAMERA["garage-desk-crt"]
    fill = float(cam_cfg["fill"])
    margin = float(cam_cfg["margin"])
    pitch = math.radians(float(cam_cfg["pitch"]))
    yaw_rad = math.radians(float(cam_cfg["yaw"]))
    lens = 48.0
    dist = cc0._camera_distance_for_fill(span, lens, fill, margin, min_half=0.06)
    horiz = dist * math.cos(pitch)
    elev = dist * math.sin(pitch)
    hero_loc = Vector(
        (
            center.x + math.cos(yaw_rad) * horiz,
            mins.y + span_y * 0.36 + elev * 0.70,
            center.z + math.sin(yaw_rad) * horiz,
        )
    )
    hero_target = Vector((center.x, mins.y + span_y * 0.40, center.z))
    hero_cam = bpy.data.objects.new(
        "DeskCrtGateHeroCam", bpy.data.cameras.new("DeskCrtGateHeroCamData")
    )
    bpy.context.collection.objects.link(hero_cam)
    bpy.context.scene.camera = hero_cam
    hero_cam.location = hero_loc
    hero_cam.rotation_euler = (hero_target - hero_loc).to_track_quat("-Z", "Y").to_euler()
    hero_cam.data.lens = lens
    bpy.context.view_layer.update()
    scene = bpy.context.scene
    mid_x = (c_lo.x + c_hi.x) * 0.5
    mid_z = (c_lo.z + c_hi.z) * 0.5
    desk_top_pts = [
        _project_with_cam(hero_cam, scene, Vector((d_lo.x, d_hi.y, d_lo.z))),
        _project_with_cam(hero_cam, scene, Vector((d_hi.x, d_hi.y, d_hi.z))),
    ]
    crt_base = _project_with_cam(hero_cam, scene, Vector((mid_x, c_lo.y, mid_z)))
    crt_top = _project_with_cam(hero_cam, scene, Vector((mid_x, c_hi.y, mid_z)))
    metrics["hero_desk_top_ndc_y"] = float(sum(p.y for p in desk_top_pts) / len(desk_top_pts))
    metrics["hero_crt_base_ndc_y"] = float(crt_base.y)
    metrics["hero_crt_top_ndc_y"] = float(crt_top.y)
    crt_corners = [
        _project_with_cam(hero_cam, scene, c_lo),
        _project_with_cam(hero_cam, scene, c_hi),
    ]
    metrics["hero_crt_ndc_x0"] = float(min(p.x for p in crt_corners))
    metrics["hero_crt_ndc_x1"] = float(max(p.x for p in crt_corners))
    crt_cx = sum(p.x for p in crt_corners) / len(crt_corners)
    desk_min_x = min(p.x for p in desk_top_pts)
    desk_max_x = max(p.x for p in desk_top_pts)
    desk_x = [
        _project_with_cam(hero_cam, scene, Vector((d_lo.x, d_hi.y, mid_z))),
        _project_with_cam(hero_cam, scene, Vector((d_hi.x, d_hi.y, mid_z))),
    ]
    metrics["hero_desk_top_ndc_x0"] = float(min(p.x for p in desk_x))
    metrics["hero_desk_top_ndc_x1"] = float(max(p.x for p in desk_x))
    if crt_cx < desk_min_x - 0.04 or crt_cx > desk_max_x + 0.04:
        raise RuntimeError(
            f"desk-crt preview gate: CRT centre outside desktop top projection (crt_x={crt_cx:.3f})"
        )
    # Ortho camera (+X side, matches render_garage_side_ortho_preview_from_glb)
    ortho_span = max(hi_all.x - lo_all.x, hi_all.y - lo_all.y, hi_all.z - lo_all.z, 0.15)
    ortho_cam = bpy.data.objects.new(
        "DeskCrtGateOrthoCam", bpy.data.cameras.new("DeskCrtGateOrthoCamData")
    )
    bpy.context.collection.objects.link(ortho_cam)
    ortho_loc = Vector((hi_all.x + ortho_span * 1.05, ortho_center.y, ortho_center.z))
    ortho_cam.location = ortho_loc
    ortho_cam.rotation_euler = (ortho_center - ortho_loc).to_track_quat("-Z", "Y").to_euler()
    ortho_cam.data.type = "ORTHO"
    ortho_cam.data.ortho_scale = ortho_span * 1.15
    bpy.context.scene.camera = ortho_cam
    bpy.context.view_layer.update()
    ortho_desk = [
        _project_with_cam(ortho_cam, scene, Vector((d_lo.x, d_hi.y, d_lo.z))),
        _project_with_cam(ortho_cam, scene, Vector((d_hi.x, d_hi.y, d_hi.z))),
    ]
    ortho_crt_base = _project_with_cam(ortho_cam, scene, Vector((mid_x, c_lo.y, mid_z)))
    ortho_crt_top = _project_with_cam(ortho_cam, scene, Vector((mid_x, c_hi.y, mid_z)))
    metrics["ortho_desk_top_ndc_y"] = float(sum(p.y for p in ortho_desk) / len(ortho_desk))
    metrics["ortho_crt_base_ndc_y"] = float(ortho_crt_base.y)
    metrics["ortho_crt_top_ndc_y"] = float(ortho_crt_top.y)
    metrics["hero_crt_below_desk_ndc"] = float(crt_base.y - metrics["hero_desk_top_ndc_y"])
    metrics["ortho_crt_below_desk_ndc"] = float(
        ortho_crt_base.y - metrics["ortho_desk_top_ndc_y"]
    )
    ortho_cx = (ortho_crt_base.x + ortho_crt_top.x) * 0.5
    ortho_desk_x0 = float(min(p.x for p in ortho_desk))
    ortho_desk_x1 = float(max(p.x for p in ortho_desk))
    metrics["ortho_crt_ndc_x"] = float(ortho_cx)
    metrics["ortho_desk_top_ndc_x0"] = ortho_desk_x0
    metrics["ortho_desk_top_ndc_x1"] = ortho_desk_x1
    if ortho_cx < ortho_desk_x0 - 0.08 or ortho_cx > ortho_desk_x1 + 0.08:
        raise RuntimeError(
            f"desk-crt preview gate: ortho CRT centre outside desktop "
            f"(crt_x={ortho_cx:.3f} desk_x=[{ortho_desk_x0:.3f},{ortho_desk_x1:.3f}])"
        )
    bpy.data.objects.remove(hero_cam, do_unlink=True)
    bpy.data.objects.remove(ortho_cam, do_unlink=True)
    return metrics


def assert_desk_crt_on_desktop_for_preview(meshes: list[bpy.types.Object]) -> dict[str, float]:
    """Fail if CRT is not on desktop; return bbox metrics for PR logging."""
    metrics = desk_crt_on_desktop_bbox_metrics(meshes)
    ordered = " ".join(f"{k}={metrics[k]:.4f}" for k in sorted(metrics))
    print(f"PASS25_BBOX garage-desk-crt {ordered}")
    return metrics


def import_garage_glb_evaluated_assembly(
    glb_path: Path,
    asset_id: str,
    tex_dir: Path,
) -> list[bpy.types.Object]:
    """Single GLB import; bake evaluated world geometry (no flatten/unparent)."""
    bpy.ops.import_scene.gltf(filepath=str(glb_path))
    depsgraph = bpy.context.evaluated_depsgraph_get()
    old_objs = list(bpy.context.scene.objects)
    meshes: list[bpy.types.Object] = []
    for ob in old_objs:
        if ob.type != "MESH":
            continue
        mesh = bpy.data.meshes.new_from_object(
            ob.evaluated_get(depsgraph),
            preserve_all_data_layers=True,
            depsgraph=depsgraph,
        )
        dup = bpy.data.objects.new(ob.name.split(".", 1)[0], mesh)
        dup.matrix_world.identity()
        bpy.context.collection.objects.link(dup)
        meshes.append(dup)
    for ob in old_objs:
        bpy.data.objects.remove(ob, do_unlink=True)
    for ob in meshes:
        apply_preview_material(ob, tex_dir, asset_id)
    if asset_id == "garage-desk-crt":
        assert_desk_crt_on_desktop_for_preview(meshes)
        bpy.ops.object.select_all(action="DESELECT")
        for ob in meshes:
            ob.select_set(True)
        bpy.context.view_layer.objects.active = meshes[0]
        bpy.ops.object.join()
        joined = bpy.context.view_layer.objects.active
        joined.name = "garage-desk-crt_glb_evaluated"
        return [joined]
    return meshes


def import_garage_glb_joined_for_preview(
    glb_path: Path,
    asset_id: str,
    tex_dir: Path,
    *,
    join_name: str | None = None,
) -> list[bpy.types.Object]:
    """Import committed GLB, apply preview mats, return one joined mesh (no per-part layout shift)."""
    bpy.ops.import_scene.gltf(filepath=str(glb_path))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        raise RuntimeError(f"{asset_id}: GLB import produced no meshes")
    if asset_id == "garage-desk-crt":
        gate_meshes = flatten_glb_preview_meshes([ob for ob in meshes])
        assert_desk_crt_on_desktop_for_preview(gate_meshes)
    else:
        meshes = flatten_glb_preview_meshes(meshes)
    if len(meshes) == 1:
        apply_preview_material(meshes[0], tex_dir, asset_id)
        return [meshes[0]]
    for ob in meshes:
        apply_preview_material(ob, tex_dir, asset_id)
    bpy.ops.object.select_all(action="DESELECT")
    for ob in meshes:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = meshes[0]
    bpy.ops.object.join()
    joined = bpy.context.view_layer.objects.active
    joined.name = join_name or f"{asset_id}_glb_preview_join"
    return [joined]


def _parent_keep_world_export(child: bpy.types.Object, parent: bpy.types.Object) -> None:
    mw = child.matrix_world.copy()
    child.parent = parent
    child.matrix_world = mw


def flatten_assembly_parts(parts: list[bpy.types.Object]) -> None:
    """Clear parent links before assembly ground snap (pass19 desk CRT floor bug)."""
    mesh_parts = [p for p in parts if p.type == "MESH" and p.name != "ConnectSpine"]
    for ob in mesh_parts:
        if ob.parent is None:
            continue
        mw = ob.matrix_world.copy()
        ob.parent = None
        ob.matrix_world = mw
    bpy.context.view_layer.update()


def snap_parts_to_ground(parts: list[bpy.types.Object]) -> None:
    if not parts:
        return
    min_y = min(mesh_world_min_y(ob) for ob in parts)
    if abs(min_y) < 1e-5:
        return
    for ob in parts:
        ob.location.y -= min_y
    bpy.context.view_layer.update()


def duplicate_join_meshes(objects: list[bpy.types.Object], name: str) -> bpy.types.Object:
    dupes: list[bpy.types.Object] = []
    for ob in objects:
        dup = ob.copy()
        dup.data = ob.data.copy()
        bpy.context.collection.objects.link(dup)
        dup.matrix_world = ob.matrix_world.copy()
        dupes.append(dup)
    bpy.ops.object.select_all(action="DESELECT")
    for dup in dupes:
        dup.select_set(True)
    bpy.context.view_layer.objects.active = dupes[0]
    bpy.ops.object.join()
    joined = bpy.context.view_layer.objects.active
    joined.name = name
    return joined


def prune_to_dominant_loose_shell(ob: bpy.types.Object) -> bpy.types.Object:
    """Drop stray mesh islands after GLB join (scene-kit floater cull)."""
    import cc0_common as cc0  # type: ignore

    if ob.type != "MESH" or len(ob.data.vertices) < 8:
        return ob
    saved_name = ob.name

    def _score(shell: bpy.types.Object) -> float:
        lo, hi = cc0.world_bbox(shell)
        vol = (hi.x - lo.x) * (hi.y - lo.y) * (hi.z - lo.z)
        return len(shell.data.vertices) * max(vol, 1e-6)

    work = ob
    for _ in range(6):
        bpy.ops.object.select_all(action="DESELECT")
        work.select_set(True)
        bpy.context.view_layer.objects.active = work
        bpy.ops.object.mode_set(mode="EDIT")
        bpy.ops.mesh.select_all(action="SELECT")
        bpy.ops.mesh.separate(type="LOOSE")
        bpy.ops.object.mode_set(mode="OBJECT")
        shells = [o for o in bpy.context.selected_objects if o.type == "MESH"]
        if len(shells) <= 1:
            work = shells[0] if shells else work
            break
        work = max(shells, key=_score)
        for shell in shells:
            if shell is not work:
                bpy.data.objects.remove(shell, do_unlink=True)
        bpy.context.view_layer.update()
    work.name = saved_name
    bpy.context.view_layer.update()
    return work


def delete_loose_mesh_elements(ob: bpy.types.Object) -> None:
    """Remove stray verts/edges not part of a face (scene-kit floater cleanup)."""
    if ob.type != "MESH":
        return
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.delete_loose(use_verts=True, use_edges=True, use_faces=False)
    bpy.ops.object.mode_set(mode="OBJECT")


def orient_build_z_to_gltf_y_up(ob: bpy.types.Object) -> None:
    """Blender Z-up kit → glTF +Y-up with base on Y=0."""
    snap_root_to_ground(ob)
    ob.rotation_euler = (math.pi / 2.0, 0.0, 0.0)
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.transform_apply(rotation=True)
    min_y = min((v.co.y for v in ob.data.vertices), default=0.0)
    if abs(min_y) > 1e-6:
        for v in ob.data.vertices:
            v.co.y -= min_y
        ob.data.update()
    ob.location = (0.0, 0.0, 0.0)


def duplicate_garage_preview_hierarchy(parts: list[bpy.types.Object], asset_id: str) -> list[bpy.types.Object]:
    """Duplicate kit parts under a preview root; layout preserved for studio render."""
    export_parts = [ob for ob in parts if ob.name != "ConnectSpine"]
    dupes: list[bpy.types.Object] = []
    for ob in export_parts:
        dup = ob.copy()
        dup.data = ob.data.copy()
        bpy.context.collection.objects.link(dup)
        dup.matrix_world = ob.matrix_world.copy()
        dupes.append(dup)
    if not dupes:
        return dupes
    root = bpy.data.objects.new(f"{asset_id}_PreviewAsm", None)
    bpy.context.collection.objects.link(root)
    for dup in dupes:
        dup.parent = root
        dup.matrix_parent_inverse = root.matrix_world.inverted()
    bpy.context.view_layer.update()
    return dupes


def export_garage_multipart_glbf(
    parts: list[bpy.types.Object], glb_path: Path, *, asset_id: str | None = None
) -> None:
    """Export on isolated duplicates so Z-up build meshes stay intact for preview."""
    import blender_preview_rig as rig  # type: ignore
    from garage_assembly import bake_parts_local_transforms, parent_under_assembly_root  # type: ignore

    source = [ob for ob in parts if ob.type == "MESH" and ob.name != "ConnectSpine"]
    if not source:
        raise RuntimeError("export_garage_multipart_glbf: no mesh parts")
    dupes: list[bpy.types.Object] = []
    for ob in source:
        dup = ob.copy()
        dup.data = ob.data.copy()
        bpy.context.collection.objects.link(dup)
        dup.matrix_world = ob.matrix_world.copy()
        dupes.append(dup)
    for d in dupes:
        if d.parent is not None:
            mw = d.matrix_world.copy()
            d.parent = None
            d.matrix_world = mw
    kenney_y = os.environ.get("GARAGE_KENNEY_IMPORT", "").strip() in ("1", "true", "yes")
    if kenney_y:
        snap_parts_to_ground(dupes)
        if asset_id != "garage-desk-crt":
            for d in dupes:
                snap_root_to_ground(d)
    if kenney_y:
        bake_parts_local_transforms(dupes)
    root = parent_under_assembly_root(dupes, asset_id or "garage-export", root_name="GarageKitRoot")
    if not kenney_y:
        root.rotation_euler = (math.pi / 2.0, 0.0, 0.0)
        bpy.context.view_layer.objects.active = root
        root.select_set(True)
        bpy.ops.object.transform_apply(rotation=True)
    if not kenney_y:
        mins, _maxs = rig.mesh_bounds_world(dupes)
        floor = min(mins.y, mins.z)
        if abs(floor) > 1e-6:
            root.location.y -= floor
            bpy.context.view_layer.update()
    bpy.ops.object.select_all(action="DESELECT")
    root.select_set(True)
    for ob in dupes:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = root
    bpy.ops.export_scene.gltf(
        filepath=str(glb_path),
        use_selection=True,
        export_apply=True,
        export_format="GLB",
        export_image_format="AUTO",
        export_yup=True,
    )
    bpy.ops.object.select_all(action="DESELECT")
    for ob in list(dupes) + [root]:
        bpy.data.objects.remove(ob, do_unlink=True)


GARAGE_HERO_CAMERA: dict[str, dict[str, float]] = {
    "garage-crt-monitor": {"yaw": 38.0, "pitch": 28.0, "fill": 0.50, "margin": 0.10},
    "garage-desk-crt": {"yaw": 45.0, "pitch": 35.0, "fill": 0.48, "margin": 0.11},
    "garage-folding-chair": {"yaw": 45.0, "pitch": 35.0, "fill": 0.54, "margin": 0.09},
    "garage-roll-door": {"yaw": 90.0, "pitch": 22.0, "fill": 0.57, "margin": 0.075},
    "garage-server-closet": {"yaw": 45.0, "pitch": 35.0, "fill": 0.54, "margin": 0.08},
}


def render_garage_hero_preview_from_glb(
    asset_id: str,
    glb_path: Path,
    tex_dir: Path,
    preview_path: Path,
    mask_path: Path,
    *,
    fill: float,
    margin: float,
    hero_yaw_deg: float | None = None,
    hero_pitch_deg: float | None = None,
) -> None:
    """Hero preview from committed GLB: Y-up props use tint slots (no baked UV); Z-up cable-spool legacy."""
    import cc0_common as cc0  # type: ignore

    y_up = garage_glb_y_up_preview(asset_id)
    os.environ["GARAGE_HERO_FLAT_MATERIAL"] = "1"
    bpy.ops.wm.read_factory_settings(use_empty=True)
    try:
        frame_obs: list[bpy.types.Object]
        up_axis = "Y" if y_up else "Z"
        floor_plane = "horizontal_y" if y_up else "horizontal_z"
        if y_up and asset_id in ("garage-desk-crt", "garage-roll-door"):
            frame_obs = import_garage_glb_evaluated_assembly(glb_path, asset_id, tex_dir)
        elif y_up:
            bpy.ops.import_scene.gltf(filepath=str(glb_path))
            meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
            if not meshes:
                raise RuntimeError(f"{asset_id}: GLB import produced no meshes")
            meshes = flatten_glb_preview_meshes(meshes)
            for ob in meshes:
                apply_preview_material(ob, tex_dir, asset_id)
            frame_obs = meshes
        else:
            bpy.ops.import_scene.gltf(filepath=str(glb_path))
            meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
            if not meshes:
                raise RuntimeError(f"{asset_id}: GLB import produced no meshes")
            root = bpy.data.objects.new("PreviewOrientRoot", None)
            bpy.context.collection.objects.link(root)
            for ob in meshes:
                mw = ob.matrix_world.copy()
                if ob.parent:
                    ob.parent = None
                ob.parent = root
                ob.matrix_world = mw
            root.rotation_euler = (-math.pi / 2.0, 0.0, 0.0)
            bpy.context.view_layer.update()
            bpy.ops.object.select_all(action="DESELECT")
            for ob in meshes:
                ob.select_set(True)
            bpy.context.view_layer.objects.active = meshes[0]
            bpy.ops.object.join()
            joined = bpy.context.view_layer.objects.active
            joined.name = f"{asset_id}_glb_preview"
            apply_preview_material(joined, tex_dir, asset_id)
            frame_obs = [joined]
        cc0.render_kit_preview(
            frame_obs,
            preview_path,
            resolution=512,
            mode="hero",
            fill=fill,
            margin=margin,
            up_axis=up_axis,
            mask_path=mask_path,
            hero_yaw_deg=hero_yaw_deg,
            hero_pitch_deg=hero_pitch_deg,
        )
        if not y_up:
            bpy.data.objects.remove(frame_obs[0], do_unlink=True)
        meta_path = preview_path.with_name(preview_path.stem + ".frame.json")
        meta_path.write_text(
            json.dumps(
                {
                    "preview_kind": "garage",
                    "flat_backdrop": True,
                    "backdrop_rgb": [150, 150, 150],
                    "framing_fill": fill,
                    "framing_margin": margin,
                    "up_axis": up_axis,
                    "floor_plane": floor_plane,
                    "renderer": "cycles_glb_import_evaluated"
                    if asset_id == "garage-desk-crt"
                    else "cycles_glb_import",
                },
                indent=2,
            )
            + "\n",
            encoding="utf-8",
        )
    finally:
        os.environ.pop("GARAGE_HERO_FLAT_MATERIAL", None)


def render_garage_side_ortho_preview_from_glb(
    asset_id: str,
    glb_path: Path,
    tex_dir: Path,
    ortho_path: Path,
) -> None:
    """Side orthographic preview from committed GLB (Y-up) to expose assembly gaps."""
    import cc0_common as cc0  # type: ignore

    y_up = garage_glb_y_up_preview(asset_id)
    os.environ["GARAGE_HERO_FLAT_MATERIAL"] = "1"
    bpy.ops.wm.read_factory_settings(use_empty=True)
    try:
        if asset_id == "garage-desk-crt":
            meshes = import_garage_glb_evaluated_assembly(glb_path, asset_id, tex_dir)
        elif asset_id == "garage-roll-door":
            meshes = import_garage_glb_evaluated_assembly(glb_path, asset_id, tex_dir)
        else:
            bpy.ops.import_scene.gltf(filepath=str(glb_path))
            meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
            if not meshes:
                raise RuntimeError(f"{asset_id}: GLB import for ortho produced no meshes")
            meshes = flatten_glb_preview_meshes(meshes)
            for ob in meshes:
                apply_preview_material(ob, tex_dir, asset_id)
        bpy.context.view_layer.update()
        lo = Vector((1e9, 1e9, 1e9))
        hi = Vector((-1e9, -1e9, -1e9))
        for ob in meshes:
            for corner in ob.bound_box:
                w = ob.matrix_world @ Vector(corner)
                lo.x = min(lo.x, w.x)
                lo.y = min(lo.y, w.y)
                lo.z = min(lo.z, w.z)
                hi.x = max(hi.x, w.x)
                hi.y = max(hi.y, w.y)
                hi.z = max(hi.z, w.z)
        center = (lo + hi) * 0.5
        span = max(hi.x - lo.x, hi.y - lo.y, hi.z - lo.z, 0.15)
        cc0.setup_preview_world_grey_hdri(hdri_strength=0.55)
        bpy.context.scene.render.engine = "CYCLES"
        bpy.context.scene.cycles.samples = 48
        bpy.context.scene.cycles.use_denoising = False
        bpy.context.scene.render.resolution_x = 512
        bpy.context.scene.render.resolution_y = 512
        bpy.context.scene.render.film_transparent = False
        ortho_view = "side_positive_x"
        if y_up:
            if asset_id == "garage-roll-door":
                cam_loc = Vector((center.x, center.y, hi.z + span * 1.05))
                ortho_view = "front_positive_z"
            elif asset_id == "garage-desk-crt":
                cam_loc = Vector((hi.x + span * 1.05, center.y, center.z))
                ortho_view = "side_positive_x"
            else:
                cam_loc = Vector((hi.x + span * 1.05, center.y, center.z))
            bpy.ops.object.camera_add(location=cam_loc)
            cam = bpy.context.active_object
            direction = center - cam_loc
            cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
        else:
            bpy.ops.object.camera_add(location=(center.x + span * 1.8, center.y, center.z))
            cam = bpy.context.active_object
            cam.rotation_euler = (math.pi / 2.0, 0.0, math.pi / 2.0)
        cam.data.type = "ORTHO"
        cam.data.ortho_scale = span * 1.15
        bpy.context.scene.camera = cam
        bpy.context.scene.render.filepath = str(ortho_path)
        bpy.ops.render.render(write_still=True)
        up_axis = "Y" if y_up else "Z"
        floor_plane = "horizontal_y" if y_up else "horizontal_z"
        meta = ortho_path.with_name(ortho_path.stem + ".frame.json")
        meta.write_text(
            json.dumps(
                {
                    "preview_kind": "garage_ortho",
                    "flat_backdrop": True,
                    "backdrop_rgb": [150, 150, 150],
                    "up_axis": up_axis,
                    "floor_plane": floor_plane,
                    "camera_type": "orthographic",
                    "view": ortho_view,
                },
                indent=2,
            )
            + "\n",
            encoding="utf-8",
        )
    finally:
        os.environ.pop("GARAGE_HERO_FLAT_MATERIAL", None)


def snapshot_poly_materials(ob: bpy.types.Object) -> tuple[list[int], list[bpy.types.Material]]:
    indices = [p.material_index for p in ob.data.polygons]
    materials = [m for m in ob.data.materials if m is not None]
    return indices, materials


def restore_poly_materials(
    ob: bpy.types.Object, snapshot: tuple[list[int], list[bpy.types.Material]]
) -> None:
    indices, materials = snapshot
    ob.data.materials.clear()
    for mat in materials:
        ob.data.materials.append(mat)
    for poly, idx in zip(ob.data.polygons, indices, strict=False):
        if idx < len(ob.data.materials):
            poly.material_index = idx


def _finish_garage_kit_asset(
    asset_id: str,
    category: str,
    license_note: str,
    mesh_obs: list[bpy.types.Object],
    part_count: int,
) -> dict:
    """Multipart garage Z-up build: per-part GLB, vcol preview join (quality pass #88)."""
    out_dir = ART_ROOT / category
    tex_dir = out_dir / "textures" / asset_id
    kenney_import = os.environ.get("GARAGE_KENNEY_IMPORT", "").strip() in ("1", "true", "yes")
    if kenney_import:
        from kenney_garage_asset import apply_kenney_garage_materials  # type: ignore

        apply_kenney_garage_materials(mesh_obs, asset_id)
    else:
        from garage_canonical_materials import assign_part_materials  # type: ignore

        assign_part_materials(mesh_obs, asset_id)
    parts = list(mesh_obs)
    min_parts = MIN_GARAGE_PARTS_FURNITURE if asset_id in GARAGE_FURNITURE else MIN_GARAGE_PARTS_SMALL
    cc0_import = os.environ.get("GARAGE_CC0_IMPORT", "").strip() in ("1", "true", "yes")
    if part_count < min_parts and not cc0_import and not kenney_import:
        raise RuntimeError(f"{asset_id}: need >={min_parts} sub-objects (got {part_count})")

    for ob in parts:
        if ob.name.startswith("RollSlat"):
            smooth_shade(ob)
            continue
        bevel_object(ob, width=0.022, segments=3)
        smooth_shade(ob)
    flatten_assembly_parts(parts)
    snap_parts_to_ground(parts)
    assembly_min_y = min(mesh_world_min_y(ob) for ob in parts)
    ground_slack = 0.002 if kenney_import or cc0_import else 0.06
    if assembly_min_y > ground_slack:
        raise RuntimeError(f"{asset_id}: assembly not grounded (min_y={assembly_min_y:.4f})")

    bake_root = duplicate_join_meshes(parts, f"{asset_id}_bake")
    if kenney_import or cc0_import:
        flatten_uv_for_flat_color_bake(bake_root)
    else:
        uv_unwrap(bake_root)
    mat_snapshot = snapshot_poly_materials(bake_root)
    maps = bake_pbr_set(bake_root, tex_dir, asset_id, size=1024)
    restore_poly_materials(bake_root, mat_snapshot)
    verts = len(bake_root.data.vertices)
    tris = sum(len(p.vertices) for p in bake_root.data.polygons)
    bpy.data.objects.remove(bake_root, do_unlink=True)

    if not kenney_import:
        from garage_assembly_validate import validate_garage_assembly  # type: ignore

        validate_garage_assembly(asset_id, parts)

    preview_path = PREVIEW_ROOT / f"{asset_id}.png"
    ortho_path = PREVIEW_ROOT / f"{asset_id}.ortho.png"
    mask_path = PREVIEW_ROOT / f"{asset_id}.mask.png"
    hero_framing: dict[str, tuple[float, float]] = {
        "garage-cable-spool": (0.56, 0.085),
        "garage-coffee-drip": (0.78, 0.055),
        "garage-crt-tower": (0.60, 0.075),
        "garage-crt-monitor": (0.50, 0.10),
        "garage-desk-crt": (0.62, 0.075),
        "garage-exterior-wall": (0.60, 0.075),
        "garage-land-for-sale-sign": (0.58, 0.075),
        "garage-lot-curb": (0.73, 0.05),
        "garage-roll-door": (0.62, 0.065),
        "garage-server-closet": (0.62, 0.07),
        "garage-floor-tile": (0.78, 0.06),
        "garage-lot-curb": (0.82, 0.05),
        "garage-land-for-sale-sign": (0.72, 0.055),
        "garage-server-closet": (0.72, 0.06),
        "garage-wall-panel": (0.72, 0.06),
        "garage-whiteboard-1995": (0.68, 0.055),
        "garage-folding-chair": (0.58, 0.08),
        "garage-voodude-rack": (0.62, 0.07),
        "garage-wall-panel": (0.60, 0.075),
        "garage-whiteboard-1995": (0.74, 0.05),
        "garage-window-frame": (0.60, 0.075),
    }
    fill, margin = hero_framing.get(asset_id, (0.58, 0.07))
    cam = GARAGE_HERO_CAMERA.get(asset_id, {})
    if cam:
        fill = float(cam.get("fill", fill))
        margin = float(cam.get("margin", margin))
    yaw = cam.get("yaw")
    pitch = cam.get("pitch")

    glb_path = out_dir / f"{asset_id}.glb"
    export_garage_multipart_glbf(parts, glb_path, asset_id=asset_id)
    render_garage_hero_preview_from_glb(
        asset_id,
        glb_path,
        tex_dir,
        preview_path,
        mask_path,
        fill=fill,
        margin=margin,
        hero_yaw_deg=yaw,
        hero_pitch_deg=pitch,
    )
    render_garage_side_ortho_preview_from_glb(asset_id, glb_path, tex_dir, ortho_path)

    entry = {
        "id": asset_id,
        "path": f"Assets/Art/EarlyGame/{category}/{asset_id}.glb",
        "textures": {
            k: f"Assets/Art/EarlyGame/{category}/textures/{asset_id}/{asset_id}_{k}.png" for k in maps
        },
        "preview": f"docs/art/previews/{asset_id}.png",
        "license": license_note,
        "vertexCount": verts,
        "triangleCount": tris,
        "partCount": part_count,
        "blenderVersion": BLENDER_VERSION,
        "generator": f"tools/art/blender/{asset_id}.py",
    }
    (out_dir / f"{asset_id}.asset.json").write_text(
        json.dumps(entry, indent=2) + "\n", encoding="utf-8"
    )
    print(f"OK {asset_id} verts={verts} tris={tris} parts={part_count}")
    return entry


def finish_asset(asset_id: str, category: str, license_note: str = "CC0-1.0 (Blender-baked, in-repo)") -> dict:
    """UV, bake, export glTF, preview; return manifest entry."""
    out_dir = ART_ROOT / category
    tex_dir = out_dir / "textures" / asset_id
    out_dir.mkdir(parents=True, exist_ok=True)
    tex_dir.mkdir(parents=True, exist_ok=True)
    PREVIEW_ROOT.mkdir(parents=True, exist_ok=True)

    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not mesh_obs:
        raise RuntimeError(f"{asset_id}: no mesh")
    part_count = len(mesh_obs)
    import os as _os

    stock_recolor = (
        asset_id == "staff-founder-idle"
        and _os.environ.get("FOUNDER_STOCK_RECOLOR", "").strip() == "1"
    ) or (
        asset_id == "staff-researcher-idle"
        and _os.environ.get("RESEARCHER_STOCK_RECOLOR", "").strip() == "1"
    )
    stock_body_name = (
        "KenneyResearcherStock"
        if asset_id == "staff-researcher-idle"
        else "KenneyFounderStock"
    )
    ground_scene_meshes()
    if category == "garage-kit":
        return _finish_garage_kit_asset(asset_id, category, license_note, mesh_obs, part_count)
    if not stock_recolor:
        tint_garage_parts(mesh_obs, asset_id)
    if category == "staff-kit" and part_count < 6 and not stock_recolor:
        raise RuntimeError(f"FAIL {asset_id}: need >=6 parts before join (have {part_count})")
    if category == "staff-kit":
        import staff_connectivity as staff_conn
        import cc0_staff_lib as staff_c0

        if stock_recolor:
            if asset_id == "staff-researcher-idle":
                from kenney_founder_stock_recolor import ARM_VG_BONES
                from kenney_researcher_from_founder import collect_researcher_founder_meshes

                mesh_obs = collect_researcher_founder_meshes()
            else:
                from kenney_founder_stock_recolor import ARM_VG_BONES, collect_founder_stock_meshes

                mesh_obs = collect_founder_stock_meshes()
            mesh_only = [o for o in mesh_obs if o.type == "MESH"]
            part_count = len(mesh_only)
            want_parts = 6 if asset_id == "staff-researcher-idle" else 3
            if part_count != want_parts:
                raise RuntimeError(
                    f"FAIL {asset_id}: stock_recolor need {want_parts} parts got {part_count}"
                )
            ob = next(o for o in mesh_only if o.name.split(".")[0] == stock_body_name)
            eyes = [o for o in mesh_only if o.name.split(".")[0].startswith("FounderEye_")]
            if {e.name.split('.')[0] for e in eyes} != {"FounderEye_L", "FounderEye_R"}:
                raise RuntimeError(f"FAIL {asset_id}: eye_objects missing")
            print(f"OK {asset_id} eye_objects FounderEye_L FounderEye_R arm_vg={','.join(ARM_VG_BONES)}")
            if mesh_island_count(ob) > 1:
                raise RuntimeError(f"FAIL {asset_id}: stock_body_fragmented islands={mesh_island_count(ob)}")
            ok_head, msg_head = staff_c0.check_founder_stock_single_mesh(ob)
            if not ok_head:
                raise RuntimeError(f"FAIL {asset_id}: {msg_head}")
            print(f"OK {asset_id} {msg_head}")
            print(f"OK {asset_id} chunky_bake stock_recolor_single_mesh")
        else:
            spine_kw: dict = {}
            if asset_id == "staff-founder-idle":
                spine_kw = {"only_parts": frozenset({"Neck", "Torso"}), "max_radius": 0.95}
            ok_spine, msg_spine = staff_c0.check_spine_clearance(mesh_obs, **spine_kw)
            if not ok_spine:
                raise RuntimeError(f"FAIL {asset_id}: {msg_spine}")

            ok_conn, msg_conn = staff_conn.check_prejoin_connectivity(mesh_obs, asset_id)
            if not ok_conn:
                raise RuntimeError(f"FAIL {asset_id}: connectivity {msg_conn}")
            if asset_id == "staff-founder-idle":
                import founder_chunky_gates as fcg

                ok_head, msg_head = staff_c0.check_founder_head_geometry(mesh_obs)
                if not ok_head:
                    raise RuntimeError(f"FAIL {asset_id}: {msg_head}")
                print(f"OK {asset_id} {msg_head}")
                ok_ck, msg_ck = fcg.check_founder_chunky_bake(mesh_obs)
                if not ok_ck:
                    raise RuntimeError(f"FAIL {asset_id}: chunky_bake {msg_ck}")
                print(f"OK {asset_id} chunky_bake {msg_ck}")
    staff_export_root: bpy.types.Object | None = None
    if category == "staff-kit":
        staff_export_root: bpy.types.Object | None = None
        if not stock_recolor:
            staff_export_root = orient_staff_assembly_for_export(mesh_obs, asset_id)
            ok_exp, msg_exp = staff_conn.check_objects_connectivity(mesh_obs, asset_id)
            if not ok_exp:
                raise RuntimeError(f"FAIL {asset_id}: export connectivity {msg_exp}")
            for ob in mesh_obs:
                smooth_shade(ob)
                bevel_object(ob, width=0.006, segments=1)
        else:
            for ob in mesh_obs:
                if ob.type == "MESH":
                    smooth_shade(ob)
        glb_path = out_dir / f"{asset_id}.glb"
        preview_path = PREVIEW_ROOT / f"{asset_id}.png"
        _sanitize_solid_gltf_materials([o for o in mesh_obs if o.type == "MESH"])
        if asset_id == "staff-founder-idle" and not stock_recolor:
            _strip_uv_layers(mesh_obs)
        if stock_recolor:
            try:
                hero_mask = [
                    o
                    for o in mesh_obs
                    if o.type == "MESH"
                    and not o.name.startswith("FounderEye")
                    and not o.name.startswith("ResearcherGlasses")
                    and not o.name.startswith("ResearcherLabCoat")
                    and not o.name.startswith("ResearcherHairCap")
                    and not o.name.startswith("ResearcherCoatSkirt")
                    and not o.name.startswith("ResearcherCoatSleeves")
                ]
                if asset_id == "staff-founder-idle":
                    for ob in mesh_obs:
                        if ob.type != "MESH" or not ob.name.split(".")[0].startswith(
                            "FounderEye_"
                        ):
                            continue
                        mat = ob.active_material
                        if not mat or not mat.use_nodes:
                            continue
                        bsdf = mat.node_tree.nodes.get("Principled BSDF")
                        if bsdf:
                            bsdf.inputs["Base Color"].default_value = (
                                0.05,
                                0.05,
                                0.05,
                                1.0,
                            )
                            bsdf.inputs["Roughness"].default_value = 1.0
                            spec = bsdf.inputs.get("Specular") or bsdf.inputs.get(
                                "Specular IOR Level"
                            )
                            if spec is not None:
                                spec.default_value = 0.0
                render_preview(mesh_obs, preview_path, asset_id=asset_id, mask_meshes=hero_mask)
            except Exception as exc:  # noqa: BLE001
                raise RuntimeError(f"FAIL {asset_id}: final_preview {exc}") from exc
            from founder_face_decals import freeze_eyes_for_gltf_export

            freeze_eyes_for_gltf_export(mesh_obs)
        export_gltf_selection(
            mesh_obs, glb_path, apply_transform=not stock_recolor
        )
        if not stock_recolor:
            try:
                render_preview_from_glb(glb_path, preview_path, asset_id=asset_id)
            except Exception as exc:  # noqa: BLE001
                print(f"WARN preview {asset_id}: {exc}")
        log_staff_material_slots(mesh_obs, asset_id)
        if stock_recolor:
            body_only = [
                o
                for o in mesh_obs
                if o.type == "MESH" and not o.name.startswith("FounderEye")
            ]
            silhouette_mask = preview_path.with_name(
                preview_path.stem + ".silhouette.mask.png"
            )
            _render_silhouette_mask(body_only, silhouette_mask, asset_id)
            print(f"OK {asset_id} silhouette_mask {silhouette_mask.name}")
        mesh_only = [o for o in mesh_obs if o.type == "MESH"]
        verts = sum(len(o.data.vertices) for o in mesh_only)
        tris = sum(len(p.vertices) for o in mesh_only for p in o.data.polygons)
        min_z, _max_z, _sz = _mesh_scene_span_z(mesh_only)
        mesh_count = len(mesh_only)
        root = link_or_join(mesh_only)
        if stock_recolor:
            if asset_id == "staff-researcher-idle":
                from kenney_researcher_from_founder import apply_researcher_founder_base_materials

                apply_researcher_founder_base_materials(root)
            else:
                from kenney_founder_stock_recolor import apply_founder_materials

                apply_founder_materials(root)
        bpy.context.scene.render.engine = "CYCLES"
        uv_unwrap(root)
        maps = bake_pbr_set(root, tex_dir, asset_id, size=1024)
        grounded = min_z <= 0.03
        if tris < 1500:
            raise RuntimeError(f"FAIL {asset_id}: tris={tris} need>=1500")
        entry = {
            "id": asset_id,
            "path": f"Assets/Art/EarlyGame/{category}/{asset_id}.glb",
            "textures": {
                k: f"Assets/Art/EarlyGame/{category}/textures/{asset_id}/{asset_id}_{k}.png"
                for k in maps
            },
            "preview": f"docs/art/previews/{asset_id}.png",
            "license": license_note,
            "vertexCount": verts,
            "triangleCount": tris,
            "meshIslands": mesh_count,
            "partCount": part_count,
            "grounded": grounded,
            "blenderVersion": BLENDER_VERSION,
            "generator": f"tools/art/blender/{asset_id}.py",
        }
        (out_dir / f"{asset_id}.asset.json").write_text(
            json.dumps(entry, indent=2) + "\n", encoding="utf-8"
        )
        print(f"OK {asset_id} verts={verts} tris={tris} meshes={mesh_count}")
        return entry
    else:
        root = link_or_join(mesh_obs)
    snap_root_to_ground(root)
    if category == "garage-kit" and mesh_island_count(root) > 1:
        add_connecting_spine(root)
    voxel_default = category == "garage-kit"
    if mesh_island_count(root) > 1 and (
        voxel_default or os.environ.get("EARLY_ART_VOXEL_SHELL", "0") not in ("0", "false")
    ):
        if category == "staff-kit":
            pass
        else:
            voxel_size = 0.018
            for _ in range(4):
                if mesh_island_count(root) <= 1:
                    break
                voxel_shell_merge(root, voxel_size=voxel_size)
                voxel_size *= 0.82
    snap_root_to_ground(root)
    if category == "staff-kit":
        snap_root_to_ground(root)
    islands = mesh_island_count(root)
    if islands > 1:
        print(f"WARN {asset_id}: {islands} mesh islands after spine/voxel")
    import bmesh

    bm = bmesh.new()
    bm.from_mesh(root.data)
    min_y = min((root.matrix_world @ v.co).y for v in bm.verts) if bm.verts else 0.0
    bm.free()
    if min_y > 0.03:
        raise RuntimeError(f"FAIL {asset_id}: mesh floats above ground (min_y={min_y:.3f})")
    if category == "staff-kit":
        ensure_staff_upright(root)
        bevel_object(root, width=0.008, segments=1)
    else:
        bevel_object(root, width=0.012, segments=2)
    smooth_shade(root)
    uv_unwrap(root)

    maps = bake_pbr_set(root, tex_dir, asset_id, size=1024)
    apply_preview_material(root, tex_dir, asset_id)
    islands_export = mesh_island_count(root)
    if category == "staff-kit":
        orient_staff_for_gltf(root, asset_id)
        islands_export = mesh_island_count(root)
        if islands_export > 48:
            raise RuntimeError(f"FAIL {asset_id}: export islands={islands_export} too many")
    glb_path = out_dir / f"{asset_id}.glb"
    export_gltf(root, glb_path)
    preview_path = PREVIEW_ROOT / f"{asset_id}.png"
    try:
        render_preview(root, preview_path, asset_id=asset_id)
    except Exception as exc:  # noqa: BLE001 — headless apt build may lack denoiser
        print(f"WARN preview {asset_id}: {exc}")

    verts = len(root.data.vertices)
    tris = sum(len(p.vertices) for p in root.data.polygons)
    entry = {
        "id": asset_id,
        "path": f"Assets/Art/EarlyGame/{category}/{asset_id}.glb",
        "textures": {k: f"Assets/Art/EarlyGame/{category}/textures/{asset_id}/{asset_id}_{k}.png" for k in maps},
        "preview": f"docs/art/previews/{asset_id}.png",
        "license": license_note,
        "vertexCount": verts,
        "triangleCount": tris,
        "meshIslands": islands_export,
        "partCount": part_count if category == "staff-kit" else part_count,
        "grounded": min_y <= 0.03,
        "blenderVersion": BLENDER_VERSION,
        "generator": f"tools/art/blender/{asset_id}.py",
    }
    (out_dir / f"{asset_id}.asset.json").write_text(json.dumps(entry, indent=2) + "\n", encoding="utf-8")
    return entry


def assign_solid(
    ob: bpy.types.Object,
    rgba: tuple[float, float, float, float],
    roughness: float = 0.52,
) -> None:
    mat = bpy.data.materials.new(ob.name + "_solid")
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes["Principled BSDF"]
    bsdf.inputs["Base Color"].default_value = rgba
    bsdf.inputs["Roughness"].default_value = roughness
    if "Specular IOR Level" in bsdf.inputs:
        bsdf.inputs["Specular IOR Level"].default_value = 0.0
    elif "Specular" in bsdf.inputs:
        bsdf.inputs["Specular"].default_value = 0.0
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def log_staff_material_slots(mesh_obs: list[bpy.types.Object], asset_id: str) -> None:
    for ob in mesh_obs:
        if ob.type != "MESH":
            continue
        slots = []
        for i, slot in enumerate(ob.material_slots):
            mat = slot.material
            base = (0.0, 0.0, 0.0, 1.0)
            if mat and mat.use_nodes:
                bsdf = mat.node_tree.nodes.get("Principled BSDF")
                if bsdf:
                    base = tuple(bsdf.inputs["Base Color"].default_value)
            slots.append(f"{i}:{mat.name if mat else '-'} rgba={tuple(round(c, 2) for c in base)}")
        print(f"OK {asset_id} slot {ob.name} " + " ".join(slots))


def tint_staff_founder_parts(mesh_obs: list[bpy.types.Object]) -> None:
    """Quaternius founder: skin, brown hair, navy/red hoodie meshes, denim, white sneakers."""
    skin = (0.84, 0.68, 0.54, 1.0)
    hair = (0.36, 0.22, 0.14, 1.0)
    hoodie_navy = (0.08, 0.10, 0.32, 1.0)
    hoodie_red = (0.62, 0.14, 0.12, 1.0)
    denim = (0.05, 0.10, 0.32, 1.0)
    sneaker = (0.92, 0.94, 0.96, 1.0)
    eye = (0.12, 0.10, 0.14, 1.0)
    for ob in mesh_obs:
        base = ob.name.split(".")[0]
        if base in ("Hair",):
            assign_solid(ob, hair, 0.48)
        elif base == "Head":
            assign_solid(ob, skin, 0.74)
        elif base.startswith("Hand_"):
            assign_solid(ob, skin, 0.82)
        elif base.lower().startswith("eye"):
            assign_solid(ob, eye, 0.35)
        elif base == "Eyebrows":
            assign_solid(ob, (0.22, 0.14, 0.10, 1.0), 0.45)
        elif base in ("Hoodie", "HoodieTorso", "HoodiePocket"):
            assign_solid(ob, hoodie_red, 0.54)
        elif base == "Jeans":
            assign_solid(ob, denim, 0.56)
        elif base.startswith("HoodieSleeve") or base == "Hood":
            assign_solid(ob, hoodie_navy, 0.52)
        elif base.startswith("Jeans") or base.startswith("Leg_") or base == "Hips":
            assign_solid(ob, denim, 0.56)
        elif base == "Neck":
            assign_solid(ob, skin, 0.82)
        elif base == "Torso":
            assign_solid(ob, hoodie_red, 0.54)
        elif base.startswith("Sole_"):
            assign_solid(ob, (0.32, 0.35, 0.40, 1.0), 0.72)
        elif base.startswith("LowerArm_") or base.startswith("Hand_"):
            assign_solid(ob, skin, 0.82)
        elif base.startswith("UpperArm_"):
            assign_solid(ob, hoodie_red, 0.54)
        elif base.startswith("Arm_") or base.startswith("Shoulder_"):
            assign_solid(ob, hoodie_navy, 0.54)
        elif base.startswith("Shoe_"):
            assign_solid(ob, sneaker, 0.38)
        elif base.startswith("Foot_"):
            assign_solid(ob, (0.88, 0.90, 0.92, 1.0), 0.38)
        elif base.startswith("Shoulder_"):
            assign_solid(ob, hoodie_navy, 0.54)
    log_staff_material_slots(mesh_obs, "staff-founder-idle")


def tint_garage_parts(mesh_obs: list[bpy.types.Object], asset_id: str) -> None:
    """Per-part saturated materials before join/bake."""
    if asset_id == "staff-founder-idle":
        tint_staff_founder_parts(mesh_obs)
        return
    stroke_colors = (
        (0.12, 0.42, 0.88, 1.0),
        (0.82, 0.18, 0.22, 1.0),
        (0.18, 0.72, 0.28, 1.0),
        (0.55, 0.22, 0.78, 1.0),
    )
    for ob in mesh_obs:
        if ob.data.materials and ob.data.materials[0]:
            nt = ob.data.materials[0].node_tree
            if nt and nt.nodes.get("Principled BSDF"):
                em = nt.nodes["Principled BSDF"].inputs["Emission Strength"].default_value
                if em and em > 0.1:
                    continue
        name = ob.name.lower()
        base = name.split(".")[0]
        if asset_id == "garage-whiteboard-1995" and (
            name.startswith("stroke") or "chart" in name or "pad" in name
        ):
            idx = 0
            try:
                idx = int("".join(c for c in name if c.isdigit()) or "0")
            except ValueError:
                idx = 0
            wb_markers = (
                (0.82, 0.18, 0.22, 1.0),
                (0.12, 0.42, 0.88, 1.0),
                (0.18, 0.72, 0.28, 1.0),
            )
            assign_solid(ob, wb_markers[idx % 3], 0.38)
        elif name.startswith("stroke"):
            idx = 0
            try:
                idx = int("".join(c for c in name if c.isdigit()) or "0")
            except ValueError:
                idx = 0
            assign_solid(ob, stroke_colors[idx % len(stroke_colors)], 0.38)
        elif "screen" in name or "led" in name:
            continue
        elif "leg" in name or "rail" in name or "brace" in name:
            assign_solid(ob, (0.40, 0.44, 0.50, 1.0), 0.32)
        elif "frame" in name or "tray" in name or "trim" in name or "baseboard" in name or name == "sill":
            assign_solid(ob, (0.85, 0.48, 0.14, 1.0), 0.45)
        elif asset_id == "garage-floor-tile" and "slab" in name:
            assign_solid(ob, (0.42, 0.36, 0.30, 1.0), 0.85)
        elif asset_id == "garage-floor-tile" and "pad" in name:
            assign_solid(ob, (0.68, 0.56, 0.44, 1.0), 0.72)
        elif asset_id == "garage-floor-tile" and "grout" in name:
            assign_solid(ob, (0.30, 0.28, 0.26, 1.0), 0.90)
        elif asset_id == "garage-floor-tile" and "accent" in name:
            assign_solid(ob, (0.90, 0.48, 0.12, 1.0), 0.40)
        elif asset_id == "garage-floor-tile" and "corner" in name:
            assign_solid(ob, (0.78, 0.22, 0.18, 1.0), 0.38)
        elif asset_id == "garage-cable-spool" and "hub" in name:
            assign_solid(ob, (0.52, 0.36, 0.22, 1.0), 0.55)
        elif asset_id == "garage-cable-spool" and "coil" in name:
            assign_solid(ob, (0.72, 0.48, 0.18, 1.0), 0.42)
        elif asset_id == "garage-lot-curb" and "curb" in name:
            assign_solid(ob, (0.58, 0.54, 0.50, 1.0), 0.75)
        elif asset_id == "garage-lot-curb" and "grass" in name:
            assign_solid(ob, (0.14, 0.72, 0.24, 1.0), 0.65)
        elif asset_id == "garage-wall-panel" and "stripe" in name:
            assign_solid(ob, (0.88, 0.45, 0.12, 1.0), 0.40)
        elif asset_id == "garage-server-closet" and "handle" in name:
            assign_solid(ob, (0.90, 0.55, 0.12, 1.0), 0.32)
        elif "slat" in name:
            assign_solid(ob, (0.50, 0.54, 0.58, 1.0), 0.28)
        elif "rib" in name:
            assign_solid(ob, (0.35, 0.38, 0.42, 1.0), 0.38)
        elif asset_id == "garage-coffee-drip" and any(k in name for k in ("body", "warmer", "pot", "spout")):
            assign_solid(ob, (0.86, 0.24, 0.10, 1.0), 0.32)
        elif asset_id == "garage-coffee-drip" and "base" in name:
            assign_solid(ob, (0.22, 0.22, 0.24, 1.0), 0.55)
        elif asset_id == "garage-coffee-drip" and "cord" in name:
            assign_solid(ob, (0.05, 0.05, 0.05, 1.0), 0.75)
        elif asset_id == "garage-land-for-sale-sign":
            assign_solid(ob, (0.92, 0.82, 0.14, 1.0), 0.42)
        elif asset_id == "garage-window-frame" and "pane" in name:
            assign_solid(ob, (0.22, 0.62, 0.88, 1.0), 0.12)
        elif asset_id == "garage-window-frame" and "mullion" in name:
            assign_solid(ob, (0.92, 0.52, 0.10, 1.0), 0.35)
        elif asset_id == "garage-window-frame" and "header" in name:
            assign_solid(ob, (0.95, 0.52, 0.08, 1.0), 0.40)
        elif asset_id == "garage-window-frame":
            assign_solid(ob, (0.95, 0.52, 0.08, 1.0), 0.42)
        elif asset_id.startswith("staff-") and "eye" in name:
            assign_solid(ob, (0.12, 0.14, 0.22, 1.0), 0.22)
        elif asset_id.startswith("staff-") and "mouth" in name:
            assign_solid(ob, (0.78, 0.38, 0.42, 1.0), 0.45)
        elif asset_id.startswith("staff-") and "head" in name:
            assign_solid(ob, (0.88, 0.72, 0.58, 1.0), 0.55)
        elif asset_id == "staff-founder-idle" and base in ("torso", "neck"):
            assign_solid(ob, (0.10, 0.62, 0.55, 1.0), 0.60)
        elif asset_id == "staff-founder-idle" and (
            base == "hips" or base.startswith("leg_") or base.startswith("foot_")
        ):
            assign_solid(ob, (0.12, 0.28, 0.58, 1.0), 0.56)
        elif asset_id.startswith("staff-") and ("leg" in name or "arm" in name):
            trousers = {
                "staff-founder-idle": (0.28, 0.18, 0.42, 1.0),
                "staff-gpu-tech-idle": (0.12, 0.18, 0.28, 1.0),
                "staff-safety-idle": (0.18, 0.22, 0.26, 1.0),
                "staff-sales-idle": (0.14, 0.18, 0.42, 1.0),
                "staff-researcher-idle": (0.22, 0.28, 0.48, 1.0),
                "staff-engineer-work": (0.22, 0.28, 0.32, 1.0),
                "staff-recruiter-idle": (0.18, 0.12, 0.32, 1.0),
                "staff-pose-typing": (0.18, 0.28, 0.48, 1.0),
            }
            if "leg" in name:
                assign_solid(ob, trousers.get(asset_id, (0.18, 0.22, 0.38, 1.0)), 0.58)
            else:
                assign_solid(ob, (0.88, 0.72, 0.58, 1.0), 0.55)
        elif asset_id.startswith("staff-") and ("hips" in name or "pants" in name):
            assign_solid(ob, (0.22, 0.26, 0.42, 1.0), 0.62)
        elif asset_id.startswith("staff-") and ("vest" in name or "hivis" in name.lower()):
            assign_solid(ob, (0.98, 0.82, 0.08, 1.0), 0.35)
        elif asset_id.startswith("staff-") and "hoodie" in name.lower():
            assign_solid(ob, (0.28, 0.22, 0.48, 1.0), 0.58)
        elif asset_id.startswith("staff-") and "hood" in name.lower():
            assign_solid(ob, (0.42, 0.28, 0.52, 1.0), 0.48)
        elif asset_id.startswith("staff-") and "shirt" in name.lower():
            assign_solid(ob, (0.22, 0.38, 0.72, 1.0), 0.48)
        elif asset_id.startswith("staff-") and "trouser" in name.lower():
            assign_solid(ob, (0.18, 0.24, 0.42, 1.0), 0.58)
        elif asset_id.startswith("staff-") and "shoe" in name.lower():
            assign_solid(ob, (0.82, 0.84, 0.86, 1.0), 0.40)
        elif asset_id.startswith("staff-") and "labcoat" in name.lower():
            assign_solid(ob, (0.94, 0.94, 0.90, 1.0), 0.42)
        elif asset_id.startswith("staff-") and "blazer" in name.lower():
            assign_solid(ob, (0.22, 0.28, 0.38, 1.0), 0.48)
        elif asset_id.startswith("staff-") and "glove" in name.lower():
            assign_solid(ob, (0.12, 0.12, 0.14, 1.0), 0.32)
        elif asset_id.startswith("staff-") and ("toolbelt" in name.lower() or "pouch" in name.lower()):
            assign_solid(ob, (0.48, 0.32, 0.18, 1.0), 0.55)
        elif asset_id.startswith("staff-") and "glasses" in name.lower():
            assign_solid(ob, (0.18, 0.18, 0.20, 1.0), 0.22)
        elif asset_id.startswith("staff-") and "foot" in name.lower():
            assign_solid(ob, (0.18, 0.20, 0.24, 1.0), 0.42)
        elif asset_id.startswith("staff-") and "hand" in name.lower():
            assign_solid(ob, (0.88, 0.72, 0.58, 1.0), 0.55)
        elif asset_id.startswith("staff-") and ("hat" in name or "hardhat" in name or "cap" in name):
            assign_solid(ob, (0.95, 0.72, 0.12, 1.0), 0.42)
        elif asset_id.startswith("staff-") and ("screen" in name or "monitorscreen" in name):
            continue
        elif asset_id.startswith("staff-") and "laptop" in name:
            assign_solid(ob, (0.62, 0.62, 0.66, 1.0), 0.35)
        elif asset_id.startswith("staff-") and "tie" in name:
            assign_solid(ob, (0.82, 0.22, 0.18, 1.0), 0.42)
        elif asset_id.startswith("staff-") and ("folder" in name or "clipboard" in name or "brief" in name):
            assign_solid(ob, (0.92, 0.88, 0.72, 1.0), 0.48)
        elif asset_id.startswith("staff-") and ("keyboard" in name or name.startswith("key")):
            assign_solid(ob, (0.78, 0.78, 0.82, 1.0), 0.32)
        elif asset_id.startswith("staff-") and "phone" in name:
            assign_solid(ob, (0.12, 0.12, 0.14, 1.0), 0.25)
        elif asset_id.startswith("staff-") and "mug" in name:
            assign_solid(ob, (0.88, 0.48, 0.12, 1.0), 0.35)
        elif asset_id.startswith("staff-") and "mug" in name:
            assign_solid(ob, (0.92, 0.48, 0.12, 1.0), 0.35)
        elif asset_id.startswith("staff-") and "gpucard" in name.lower():
            assign_solid(ob, (0.12, 0.48, 0.82, 1.0), 0.28)
        elif asset_id.startswith("staff-") and "headset" in name.lower():
            assign_solid(ob, (0.22, 0.24, 0.28, 1.0), 0.32)
        elif asset_id.startswith("staff-") and "torso" in name:
            role_shirt = {
                "staff-researcher-idle": (0.28, 0.52, 0.95, 1.0),
                "staff-engineer-work": (0.35, 0.78, 0.32, 1.0),
                "staff-safety-idle": (0.42, 0.55, 0.62, 1.0),
                "staff-sales-idle": (0.45, 0.22, 0.78, 1.0),
                "staff-recruiter-idle": (0.88, 0.32, 0.28, 1.0),
                "staff-gpu-tech-idle": (0.12, 0.72, 0.38, 1.0),
                "staff-founder-idle": (0.82, 0.42, 0.22, 1.0),
                "staff-pose-typing": (0.38, 0.62, 0.82, 1.0),
            }
            assign_solid(ob, role_shirt.get(asset_id, (0.45, 0.48, 0.52, 1.0)), 0.48)
        elif asset_id.startswith("staff-") and ("hair" in name or "ponytail" in name):
            assign_solid(ob, (0.32, 0.22, 0.16, 1.0), 0.55)
        elif asset_id.startswith("staff-") and ("screwdriver" in name or "wrench" in name):
            assign_solid(ob, (0.72, 0.72, 0.75, 1.0), 0.28)
        elif asset_id.startswith("staff-") and "headset" in name.lower():
            assign_solid(ob, (0.22, 0.24, 0.28, 1.0), 0.32)
        elif asset_id.startswith("staff-") and "badge" in name:
            assign_solid(ob, (0.82, 0.82, 0.86, 1.0), 0.32)
        elif asset_id.startswith("staff-"):
            from blender_material_palette import spec_for

            spec = spec_for(asset_id)
            l = spec["light"]
            assign_solid(ob, (l[0], l[1], l[2], 1.0), float(spec["roughness"]))
        elif asset_id == "garage-folding-chair" and ("seat" in name or "back" in name):
            assign_solid(ob, (0.32, 0.52, 0.82, 1.0), 0.48)
        elif asset_id == "garage-desk-crt" and "desktop" in name:
            assign_solid(ob, (0.62, 0.48, 0.32, 1.0), 0.52)
        elif asset_id == "garage-desk-crt" and "drawer" in name:
            assign_solid(ob, (0.48, 0.36, 0.24, 1.0), 0.55)
        elif asset_id == "garage-desk-crt" and ("keyboard" in name or name.startswith("key")):
            assign_solid(ob, (0.78, 0.78, 0.80, 1.0), 0.35)
        elif asset_id == "garage-desk-crt" and "crt" in name:
            assign_crt_screen_material(ob, (0.10, 0.78, 0.52, 1.0))
        elif asset_id == "garage-crt-tower" and (
            "sidelabel" in name or "frontpanel" in name or "toptrim" in name
        ):
            assign_solid(ob, (0.95, 0.48, 0.08, 1.0), 0.35)
        elif asset_id == "garage-crt-tower" and "screen" not in name and "led" not in name:
            assign_solid(ob, (0.78, 0.74, 0.66, 1.0), 0.40)
        elif asset_id == "garage-voodude-rack":
            if "led" in name or "light" in name:
                assign_solid(ob, (0.95, 0.25, 0.15, 1.0), 0.25)
            else:
                assign_solid(ob, (0.10, 0.10, 0.12, 1.0), 0.38)
        elif asset_id == "garage-server-closet":
            assign_solid(ob, (0.18, 0.20, 0.24, 1.0), 0.35)
        elif "surface" in name:
            assign_solid(ob, (0.94, 0.95, 0.92, 1.0), 0.28)
        elif asset_id.startswith("garage-"):
            from blender_material_palette import spec_for

            spec = spec_for(asset_id)
            l = spec["light"]
            assign_solid(ob, (l[0], l[1], l[2], 1.0), float(spec["roughness"]))


def assign_crt_screen_material(
    ob: bpy.types.Object,
    screen_rgba: tuple[float, float, float, float],
    *,
    bezel: tuple[float, float, float, float] = (0.22, 0.24, 0.28, 1.0),
    emission_strength: float = 3.4,
    screen_mix: float = 0.58,
) -> None:
    """Dark bezel + emissive screen tint for desk/crt-tower heroes (no baked UV preview)."""
    mat = bpy.data.materials.new(ob.name + "_crt_hero")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Base Color"].default_value = bezel
    bsdf.inputs["Roughness"].default_value = 0.38
    emit = nodes.new("ShaderNodeEmission")
    emit.inputs["Color"].default_value = screen_rgba
    emit.inputs["Strength"].default_value = emission_strength
    mix = nodes.new("ShaderNodeMixShader")
    mix.inputs["Fac"].default_value = screen_mix
    links.new(bsdf.outputs["BSDF"], mix.inputs[1])
    links.new(emit.outputs["Emission"], mix.inputs[2])
    links.new(mix.outputs["Shader"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def apply_garage_hero_flat_tint(ob: bpy.types.Object, asset_id: str) -> None:
    """Name-based hero/scene tints; avoids baked-albedo speckle on multipart GLB imports."""
    lname = ob.name.lower()
    if asset_id == "garage-crt-monitor":
        assign_solid(ob, (0.48, 0.50, 0.54, 1.0), 0.42)
        return
    if asset_id == "garage-desk-crt":
        if "desktop" in lname:
            assign_solid(ob, (0.62, 0.48, 0.32, 1.0), 0.52)
        elif "keyboard" in lname or lname.startswith("key"):
            assign_solid(ob, (0.78, 0.78, 0.80, 1.0), 0.35)
        elif "crt" in lname:
            assign_crt_screen_material(
                ob, (0.10, 0.78, 0.52, 1.0), bezel=(0.18, 0.20, 0.24, 1.0), screen_mix=0.42
            )
        else:
            assign_solid(ob, (0.48, 0.36, 0.24, 1.0), 0.55)
        return
    if asset_id == "garage-roll-door":
        if "slat" in lname or "curtain" in lname:
            assign_solid(ob, (0.58, 0.60, 0.64, 1.0), 0.34)
        else:
            assign_solid(ob, (0.25, 0.28, 0.32, 1.0), 0.36)
        return


def assign_emission(ob: bpy.types.Object, rgba: tuple[float, float, float, float], strength: float = 2.0) -> None:
    mat = bpy.data.materials.new(ob.name + "_emit")
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes["Principled BSDF"]
    bsdf.inputs["Base Color"].default_value = rgba
    bsdf.inputs["Emission Color"].default_value = rgba
    bsdf.inputs["Emission Strength"].default_value = strength
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def _boost_saturation(rgba: tuple[float, float, float, float], scale: float = 3.05) -> tuple[float, float, float, float]:
    h, s, v = colorsys.rgb_to_hsv(rgba[0], rgba[1], rgba[2])
    s = min(1.0, s * scale)
    v = min(1.0, v * 1.08)
    r, g, b = colorsys.hsv_to_rgb(h, s, v)
    return (r, g, b, rgba[3])


def mesh_has_corner_color_layer(ob: bpy.types.Object) -> bool:
    if getattr(ob.data, "vertex_colors", None):
        return len(ob.data.vertex_colors) > 0
    attrs = getattr(ob.data, "color_attributes", None)
    return bool(attrs and any(a.domain == "CORNER" for a in attrs))


def transfer_material_colors_to_vertex_layer(ob: bpy.types.Object) -> None:
    mesh = ob.data
    while mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[0])
    vcol = mesh.color_attributes.new(name="BakeCol", type="BYTE_COLOR", domain="CORNER")
    mesh.color_attributes.active_color = vcol
    if hasattr(mesh.color_attributes, "active_render_color"):
        mesh.color_attributes.active_render_color = vcol
    for poly in mesh.polygons:
        rgba = (0.55, 0.55, 0.55, 1.0)
        mat = ob.data.materials[poly.material_index] if poly.material_index < len(ob.data.materials) else None
        if mat and mat.use_nodes:
            bsdf = mat.node_tree.nodes.get("Principled BSDF")
            if bsdf:
                if bsdf.inputs["Emission Strength"].default_value > 0.2:
                    rgba = tuple(bsdf.inputs["Emission Color"].default_value)
                else:
                    rgba = tuple(bsdf.inputs["Base Color"].default_value)
        rgba = _boost_saturation(rgba)
        for loop_idx in poly.loop_indices:
            vcol.data[loop_idx].color = (rgba[0], rgba[1], rgba[2], 1.0)


def unify_vertex_color_material(ob: bpy.types.Object, asset_id: str) -> None:
    from blender_material_palette import spec_for

    spec = spec_for(asset_id)
    mat = bpy.data.materials.new(f"{asset_id}_vcol")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    attr = nodes.new("ShaderNodeAttribute")
    attr.attribute_name = "BakeCol"
    color_out = attr.outputs["Color"]
    if asset_id.startswith("garage-"):
        glow_ids = {
            "garage-land-for-sale-sign",
            "garage-whiteboard-1995",
            "garage-window-frame",
            "garage-floor-tile",
        }
        dull_ids = {"garage-coffee-drip", "garage-roll-door", "garage-whiteboard-1995"}
        hue = nodes.new("ShaderNodeHueSaturation")
        hue.inputs["Value"].default_value = 1.10 if asset_id in dull_ids else 1.08
        hue.inputs["Saturation"].default_value = (
            4.85 if asset_id in dull_ids else (4.0 if asset_id in glow_ids else 3.65)
        )
        links.new(color_out, hue.inputs["Color"])
        color_out = hue.outputs["Color"]
    elif asset_id.startswith("staff-"):
        hsv = nodes.new("ShaderNodeHueSaturation")
        hsv.inputs["Saturation"].default_value = 1.95
        hsv.inputs["Value"].default_value = 1.12
        links.new(color_out, hsv.inputs["Color"])
        color_out = hsv.outputs["Color"]
    links.new(color_out, bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = float(spec["roughness"])
    emit = nodes.new("ShaderNodeEmission")
    if asset_id.startswith("garage-"):
        glow_ids = {
            "garage-land-for-sale-sign",
            "garage-whiteboard-1995",
            "garage-window-frame",
            "garage-floor-tile",
        }
        dull_ids = {"garage-coffee-drip", "garage-roll-door", "garage-whiteboard-1995"}
        if "crt" in asset_id or "rack" in asset_id:
            emit.inputs["Strength"].default_value = 1.4
        elif asset_id == "garage-desk-crt":
            emit.inputs["Strength"].default_value = 0.72
        elif asset_id in glow_ids:
            emit.inputs["Strength"].default_value = 0.85
        elif asset_id in dull_ids:
            emit.inputs["Strength"].default_value = 0.72
        else:
            emit.inputs["Strength"].default_value = 0.0
    else:
        emit.inputs["Strength"].default_value = 1.4 if "crt" in asset_id or "rack" in asset_id else 0.0
    if emit.inputs["Strength"].default_value > 0:
        vcol_emit = nodes.new("ShaderNodeAttribute")
        vcol_emit.attribute_name = "BakeCol"
        links.new(vcol_emit.outputs["Color"], emit.inputs["Color"])
        mix = nodes.new("ShaderNodeMixShader")
        if asset_id.startswith("garage-"):
            glow_ids = {
                "garage-land-for-sale-sign",
                "garage-whiteboard-1995",
                "garage-window-frame",
                "garage-floor-tile",
            }
            dull_ids = {"garage-coffee-drip", "garage-roll-door", "garage-whiteboard-1995"}
            mix.inputs["Fac"].default_value = (
                0.45
                if "crt" in asset_id
                else (
                    0.48
                    if asset_id in glow_ids
                    else (0.44 if asset_id in dull_ids else (0.35 if asset_id == "garage-desk-crt" else 0.30))
                )
            )
        else:
            mix.inputs["Fac"].default_value = 0.45 if "crt" in asset_id else 0.30
        links.new(bsdf.outputs["BSDF"], mix.inputs[1])
        links.new(emit.outputs["Emission"], mix.inputs[2])
        links.new(mix.outputs["Shader"], out.inputs["Surface"])
    else:
        links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def smooth_shade(ob: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.shade_smooth()


def _boost_garage_all_slot_materials(ob: bpy.types.Object) -> None:
    """Boost every material slot on a mesh (Kenney tint GLBs)."""
    if ob.type != "MESH":
        return
    saved = list(ob.data.materials)
    for idx, mat in enumerate(saved):
        if not mat:
            continue
        ob.data.materials.clear()
        ob.data.materials.append(mat)
        _boost_garage_part_preview_material(ob)
        saved[idx] = ob.data.materials[0]
    ob.data.materials.clear()
    for mat in saved:
        if mat:
            ob.data.materials.append(mat)


def _boost_garage_part_preview_material(ob: bpy.types.Object) -> None:
    """Multipart garage GLBs keep per-part Garage_* slots; baked albedo UVs are bake-mesh only."""
    if not ob.data.materials or not ob.data.materials[0]:
        return
    mat = ob.data.materials[0]
    if not mat.use_nodes or mat.name.endswith("_preview"):
        return
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if not bsdf:
        return
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    base = bsdf.inputs["Base Color"]
    if base.is_linked:
        link = base.links[0]
        src_sock = link.from_socket
        links.remove(link)
        hue = nodes.new("ShaderNodeHueSaturation")
        hue.inputs["Saturation"].default_value = 1.08
        hue.inputs["Value"].default_value = 1.10
        links.new(src_sock, hue.inputs["Color"])
        links.new(hue.outputs["Color"], base)
    else:
        base.default_value = _boost_saturation(tuple(base.default_value), scale=1.25)
    lname = ob.name.lower()
    if "screen" in lname or "led" in lname or "glass" in lname:
        em = max(float(bsdf.inputs["Emission Strength"].default_value), 1.15)
        bsdf.inputs["Emission Strength"].default_value = em
        if float(bsdf.inputs["Emission Color"].default_value[0]) < 0.2:
            bsdf.inputs["Emission Color"].default_value = (0.12, 0.72, 0.45, 1.0)


def apply_preview_material(ob: bpy.types.Object, tex_dir: Path, asset_id: str) -> None:
    """EEVEE preview uses baked albedo for staff; garage multipart keeps Garage_* slots."""
    albedo = tex_dir / f"{asset_id}_albedo.png"
    rough = tex_dir / f"{asset_id}_roughness.png"
    flat_slots = os.environ.get("GARAGE_HERO_FLAT_MATERIAL", "").strip() in ("1", "true", "yes")
    if flat_slots and asset_id in ("garage-desk-crt", "garage-roll-door"):
        apply_garage_hero_flat_tint(ob, asset_id)
        return
    if asset_id.startswith("garage-"):
        mat0 = ob.data.materials[0] if ob.data.materials else None
        use_baked = ob.name.endswith("_preview") or "_preview" in ob.name
        if flat_slots and mat0 and (mat0.name.startswith("Garage_") or mat0.name.startswith("m_")):
            _boost_garage_all_slot_materials(ob)
            if asset_id == "garage-cable-spool":
                for mat in ob.data.materials:
                    if not mat or not mat.use_nodes:
                        continue
                    bsdf = mat.node_tree.nodes.get("Principled BSDF")
                    if not bsdf:
                        continue
                    lname = mat.name.lower()
                    nodes = mat.node_tree.nodes
                    links = mat.node_tree.links
                    base = bsdf.inputs["Base Color"]
                    hue = nodes.new("ShaderNodeHueSaturation")
                    hue.inputs["Saturation"].default_value = 2.0
                    hue.inputs["Value"].default_value = 1.18
                    if base.is_linked:
                        src = base.links[0].from_socket
                        links.remove(base.links[0])
                        links.new(src, hue.inputs["Color"])
                    else:
                        hue.inputs["Color"].default_value = base.default_value
                    links.new(hue.outputs["Color"], base)
                    if "coil" in lname or "copper" in lname:
                        base.default_value = (0.78, 0.48, 0.26, 1.0)
                    elif "cable" in lname:
                        base.default_value = (0.12, 0.12, 0.14, 1.0)
            return
        if mat0 and mat0.name.startswith("Garage_") and not use_baked:
            _boost_garage_part_preview_material(ob)
            return
    force_albedo = asset_id.startswith("staff-")
    bake_col = getattr(ob.data, "color_attributes", None)
    has_bake_vcol = bake_col is not None and bake_col.get("BakeCol") is not None
    if not force_albedo and (has_bake_vcol or ob.data.vertex_colors) and ob.data.materials and ob.data.materials[0]:
        mat = ob.data.materials[0]
        node_types = {n.type for n in mat.node_tree.nodes} if mat.use_nodes else set()
        if mat.name.endswith("_vcol") or "ShaderNodeAttribute" in node_types or "VERTEX_COLOR" in node_types:
            if not asset_id.startswith("garage-"):
                return
    if not albedo.exists():
        return
    try:
        from PIL import Image
        import numpy as np

        arr = np.array(Image.open(albedo).convert("RGB"))
        if float(arr.max()) < 80:
            return
    except Exception:
        pass
    mat = bpy.data.materials.new(f"{asset_id}_preview")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    tex = nodes.new("ShaderNodeTexImage")
    tex.image = bpy.data.images.load(str(albedo))
    if tex.image:
        tex.image.colorspace_settings.name = "sRGB"
        tex.image.reload()
    hue = nodes.new("ShaderNodeHueSaturation")
    if asset_id.startswith("garage-"):
        scene_sheet = os.environ.get("GARAGE_SCENE_PREVIEW", "").strip() in ("1", "true", "yes")
        hue.inputs["Saturation"].default_value = 1.48 if scene_sheet else 1.0
    else:
        hue.inputs["Saturation"].default_value = 1.45
    hue.inputs["Value"].default_value = 1.0 if asset_id.startswith("garage-") else 1.05
    links.new(tex.outputs["Color"], hue.inputs["Color"])
    links.new(hue.outputs["Color"], bsdf.inputs["Base Color"])
    surf_out = bsdf.outputs["BSDF"]
    if rough.exists():
        rtex = nodes.new("ShaderNodeTexImage")
        rtex.image = bpy.data.images.load(str(rough))
        links.new(rtex.outputs["Color"], bsdf.inputs["Roughness"])
    emit_strength = 0.0
    emit_color = (0.15, 0.65, 0.38, 1.0)
    if not asset_id.startswith("garage-"):
        if "crt" in asset_id or "screen" in asset_id:
            emit_strength = 1.2
        elif asset_id in {
            "garage-land-for-sale-sign",
            "garage-whiteboard-1995",
            "garage-window-frame",
            "garage-floor-tile",
        }:
            emit_strength = 0.55
            emit_color = (0.92, 0.62, 0.12, 1.0)
    bsdf.inputs["Emission Strength"].default_value = emit_strength
    bsdf.inputs["Emission Color"].default_value = emit_color
    links.new(surf_out, out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)
    for poly in ob.data.polygons:
        poly.material_index = 0


def bake_pbr_set(ob: bpy.types.Object, tex_dir: Path, asset_id: str, size: int = 1024) -> list[str]:
    """Bake albedo, normal, roughness image maps."""
    from blender_material_palette import spec_for

    spec = spec_for(asset_id)
    kinds = ["albedo", "normal", "roughness"]
    paths: dict[str, str] = {}
    multi_solid = len(ob.data.materials) > 1 or (
        ob.data.materials and ob.data.materials[0] and "_solid" in ob.data.materials[0].name
    )
    staff_flat_bake = asset_id == "staff-founder-idle" and len(ob.data.materials) > 1
    kenney_flat_bake = (
        os.environ.get("GARAGE_KENNEY_IMPORT", "").strip() in ("1", "true", "yes")
        and asset_id.startswith("garage-")
    )
    if multi_solid:
        transfer_material_colors_to_vertex_layer(ob)
        if staff_flat_bake or kenney_flat_bake:
            bake_mat = bpy.data.materials.new(f"{asset_id}_flat_bake")
            bake_mat.use_nodes = True
            nodes = bake_mat.node_tree.nodes
            links = bake_mat.node_tree.links
            nodes.clear()
            out = nodes.new("ShaderNodeOutputMaterial")
            emit = nodes.new("ShaderNodeEmission")
            attr = nodes.new("ShaderNodeAttribute")
            attr.attribute_name = "BakeCol"
            links.new(attr.outputs["Color"], emit.inputs["Color"])
            emit.inputs["Strength"].default_value = 1.0
            links.new(emit.outputs["Emission"], out.inputs["Surface"])
            ob.data.materials.clear()
            ob.data.materials.append(bake_mat)
        elif not asset_id.startswith("staff-") and not kenney_flat_bake:
            unify_vertex_color_material(ob, asset_id)
        else:
            unify_vertex_color_material(ob, asset_id)

    has_slots = len(ob.data.materials) > 0 and ob.data.materials[0] is not None
    if not has_slots:
        mat = bpy.data.materials.new(name=f"{asset_id}_bake")
        mat.use_nodes = True
        nodes = mat.node_tree.nodes
        links = mat.node_tree.links
        nodes.clear()
        out = nodes.new("ShaderNodeOutputMaterial")
        bsdf = nodes.new("ShaderNodeBsdfPrincipled")
        noise = nodes.new("ShaderNodeTexNoise")
        noise.inputs["Scale"].default_value = float(spec["noise"])
        color_ramp = nodes.new("ShaderNodeValToRGB")
        d = spec["dark"]
        l = spec["light"]
        color_ramp.color_ramp.elements[0].color = (d[0], d[1], d[2], 1)
        color_ramp.color_ramp.elements[1].color = (l[0], l[1], l[2], 1)
        links.new(noise.outputs["Fac"], color_ramp.inputs["Fac"])
        links.new(color_ramp.outputs["Color"], bsdf.inputs["Base Color"])
        bsdf.inputs["Roughness"].default_value = float(spec["roughness"])
        links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
        ob.data.materials.clear()
        ob.data.materials.append(mat)
        nodes = mat.node_tree.nodes
    else:
        bake_mat = ob.data.materials[0]
        bake_mat.use_nodes = True
        nodes = bake_mat.node_tree.nodes

    for kind in kinds:
        img = bpy.data.images.new(f"{asset_id}_{kind}", width=size, height=size)
        img.filepath = str(tex_dir / f"{asset_id}_{kind}.png")
        img.file_format = "PNG"

        bake_node = nodes.new("ShaderNodeTexImage")
        bake_node.image = img
        nodes.active = bake_node
        bpy.ops.object.select_all(action="DESELECT")
        ob.select_set(True)
        bpy.context.view_layer.objects.active = ob

        bake_type = "DIFFUSE"
        if kind == "normal":
            bake_type = "NORMAL"
        elif kind == "roughness":
            bake_type = "ROUGHNESS"

        bpy.context.scene.render.bake.use_selected_to_active = False
        bpy.context.scene.render.bake.margin = 8
        restore: list[tuple] = []
        try:
            if kind == "albedo" and (staff_flat_bake or kenney_flat_bake):
                bpy.ops.object.bake(type="EMIT")
            elif kind == "albedo" and mesh_has_corner_color_layer(ob):
                bpy.ops.object.bake(type="DIFFUSE", pass_filter={"COLOR"})
            elif kind == "albedo":
                slot = ob.data.materials[0]
                if slot and slot.use_nodes:
                    nt = slot.node_tree
                    bsdf = nt.nodes.get("Principled BSDF")
                    out = nt.nodes.get("Material Output")
                    if bsdf and out:
                        prev = out.inputs["Surface"].links[0].from_socket if out.inputs["Surface"].links else None
                        emit = nt.nodes.new("ShaderNodeEmission")
                        emit.inputs["Color"].default_value = bsdf.inputs["Base Color"].default_value
                        emit.inputs["Strength"].default_value = 1.0
                        nt.links.new(emit.outputs["Emission"], out.inputs["Surface"])
                        restore.append((nt, emit, prev, out))
                bpy.ops.object.bake(type="EMIT")
                for nt, emit, prev, out in restore:
                    if out.inputs["Surface"].links:
                        nt.links.remove(out.inputs["Surface"].links[0])
                    nt.nodes.remove(emit)
                    if prev:
                        nt.links.new(prev, out.inputs["Surface"])
            elif kind == "normal":
                bpy.ops.object.bake(type="NORMAL")
            else:
                bpy.ops.object.bake(type="ROUGHNESS")
        except Exception:
            bpy.ops.object.bake(type="AO")
        img.filepath_raw = str(tex_dir / f"{asset_id}_{kind}.png")
        img.save()
        paths[kind] = img.filepath_raw

    return list(paths.keys())


def export_gltf(ob: bpy.types.Object, path: Path) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.export_scene.gltf(
        filepath=str(path),
        use_selection=True,
        export_apply=True,
        export_format="GLB",
        export_image_format="AUTO",
        export_texcoords=True,
        export_normals=True,
        export_materials="EXPORT",
    )


def _strip_uv_layers(mesh_obs: list[bpy.types.Object]) -> None:
    for ob in mesh_obs:
        while ob.data.uv_layers:
            ob.data.uv_layers.remove(ob.data.uv_layers.active)
        if hasattr(ob.data, "color_attributes"):
            for key in list(ob.data.color_attributes.keys()):
                ob.data.color_attributes.remove(ob.data.color_attributes[key])
        for key in list(getattr(ob.data, "vertex_colors", {}).keys()) if hasattr(ob.data, "vertex_colors") else []:
            del ob.data.vertex_colors[key]


def _sanitize_solid_gltf_materials(mesh_obs: list[bpy.types.Object]) -> None:
    for ob in mesh_obs:
        for slot in ob.material_slots:
            mat = slot.material
            if not mat or not mat.use_nodes:
                continue
            nodes = mat.node_tree.nodes
            bsdf = nodes.get("Principled BSDF")
            if not bsdf:
                continue
            if bsdf.inputs["Base Color"].is_linked:
                for link in list(mat.node_tree.links):
                    if link.to_socket == bsdf.inputs["Base Color"]:
                        mat.node_tree.links.remove(link)
            for node in list(nodes):
                if node.type in ("TEX_IMAGE", "NORMAL_MAP", "BUMP"):
                    nodes.remove(node)


def export_gltf_selection(
    objects: list[bpy.types.Object], path: Path, *, apply_transform: bool = True
) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    mesh_list = [o for o in objects if o.type in ("MESH", "EMPTY")]
    if not mesh_list:
        raise RuntimeError("export_gltf_selection: no objects")
    for ob in mesh_list:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = mesh_list[0]
    bpy.ops.export_scene.gltf(
        filepath=str(path),
        use_selection=True,
        export_apply=apply_transform,
        export_format="GLB",
        export_image_format="AUTO",
        export_texcoords=True,
        export_normals=True,
        export_materials="EXPORT",
    )


def apply_staff_preview_material(ob: bpy.types.Object, asset_id: str) -> None:
    """Saturated per-role colors for studio preview (Hatch readable silhouettes)."""
    from blender_material_palette import spec_for

    role_accents = {
        "staff-founder-idle": (0.42, 0.18, 0.98, 1.0),
        "staff-gpu-tech-idle": (0.12, 0.78, 0.42, 1.0),
        "staff-safety-idle": (0.35, 0.92, 0.22, 1.0),
        "staff-sales-idle": (0.14, 0.22, 0.68, 1.0),
        "staff-researcher-idle": (0.22, 0.55, 0.95, 1.0),
        "staff-engineer-work": (0.92, 0.42, 0.08, 1.0),
        "staff-recruiter-idle": (0.92, 0.22, 0.38, 1.0),
        "staff-pose-typing": (0.92, 0.18, 0.72, 1.0),
    }
    role_darks = {
        "staff-founder-idle": (0.18, 0.08, 0.32, 1.0),
        "staff-gpu-tech-idle": (0.05, 0.22, 0.12, 1.0),
        "staff-safety-idle": (0.35, 0.32, 0.05, 1.0),
        "staff-sales-idle": (0.06, 0.08, 0.22, 1.0),
        "staff-researcher-idle": (0.08, 0.18, 0.38, 1.0),
        "staff-engineer-work": (0.32, 0.16, 0.04, 1.0),
        "staff-recruiter-idle": (0.02, 0.12, 0.28, 1.0),
        "staff-pose-typing": (0.05, 0.22, 0.28, 1.0),
    }
    spec = spec_for(asset_id)
    light = _boost_saturation(role_accents.get(asset_id, (*spec["light"], 1.0)), 2.0)
    dark = _boost_saturation(role_darks.get(asset_id, (*spec["dark"], 1.0)), 1.75)
    mat = bpy.data.materials.new(f"{asset_id}_staff_preview")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    ramp = nodes.new("ShaderNodeValToRGB")
    texco = nodes.new("ShaderNodeTexCoord")
    ramp.color_ramp.elements[0].color = dark
    ramp.color_ramp.elements[1].color = light
    sep = nodes.new("ShaderNodeSeparateXYZ")
    links.new(texco.outputs["Generated"], sep.inputs["Vector"])
    links.new(sep.outputs["Y"], ramp.inputs["Fac"])
    links.new(ramp.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = float(spec["roughness"])
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def _render_silhouette_mask(
    mesh_obs: list[bpy.types.Object], mask_path: Path, asset_id: str
) -> None:
    """Pre-export body mask for IoU vs in-scene stock check (RGB still from post-export GLB)."""
    import blender_preview_rig as rig  # type: ignore

    if asset_id:
        os.environ["STAFF_PREVIEW_ASSET"] = asset_id
    tmp_rgb = mask_path.with_name(mask_path.stem + ".rgb-tmp.png")
    rig.render_preview_studio(mesh_obs, tmp_rgb, mask_meshes=mesh_obs)
    src_mask = tmp_rgb.with_name(tmp_rgb.stem + ".mask.png")
    if not src_mask.is_file():
        raise RuntimeError(f"FAIL {asset_id}: silhouette_mask missing")
    mask_path.parent.mkdir(parents=True, exist_ok=True)
    mask_path.write_bytes(src_mask.read_bytes())
    for extra in (tmp_rgb, src_mask, tmp_rgb.with_name(tmp_rgb.stem + ".frame.json")):
        if extra.is_file():
            extra.unlink()


def render_preview_from_glb(glb_path: Path, path: Path, asset_id: str | None = None) -> None:
    """Render studio preview from committed multi-mesh staff GLB (Z-up export)."""
    glb_path = glb_path.resolve()
    if not glb_path.is_file():
        raise FileNotFoundError(glb_path)
    before = set(bpy.context.scene.objects)
    bpy.ops.import_scene.gltf(filepath=str(glb_path))
    imported = [o for o in bpy.context.scene.objects if o not in before]
    mesh_obs = [o for o in imported if o.type == "MESH" and o.name != "PreviewShadowFloor"]
    if not mesh_obs:
        raise RuntimeError(f"no meshes in {glb_path}")
    stock_hero = asset_id in ("staff-founder-idle", "staff-researcher-idle")
    if asset_id and asset_id.startswith("staff-") and not stock_hero:
        tint_garage_parts(mesh_obs, asset_id)
    if asset_id:
        os.environ["STAFF_PREVIEW_ASSET"] = asset_id
    mask_obs = mesh_obs
    if stock_hero:
        mask_obs = [
            o
            for o in mesh_obs
            if not o.name.startswith("FounderEye") and not o.name.startswith("ResearcherGlasses")
        ]
    try:
        render_preview(mesh_obs, path, asset_id=asset_id, mask_meshes=mask_obs)
    finally:
        for ob in imported:
            try:
                bpy.data.objects.remove(ob, do_unlink=True)
            except ReferenceError:
                pass


def render_preview(
    ob: bpy.types.Object | list[bpy.types.Object],
    path: Path,
    asset_id: str | None = None,
    mask_meshes: bpy.types.Object | list[bpy.types.Object] | None = None,
) -> None:
    use_rig = os.environ.get("EARLY_ART_PREVIEW_RIG", "1").strip().lower()
    if use_rig not in ("0", "false", "no"):
        try:
            import blender_preview_rig as rig  # type: ignore

            prev_kind = "garage"
            if asset_id:
                if asset_id.startswith("staff-"):
                    prev_kind = "staff"
                elif asset_id.startswith("wisp-"):
                    prev_kind = "wisp"
                elif asset_id.startswith(("hud-", "icon-", "panel-")) or asset_id.startswith("tex-hud-"):
                    prev_kind = "hud"
                if asset_id.startswith(("staff-", "icon-", "hud-", "panel-")):
                    os.environ["STAFF_PREVIEW_ASSET"] = asset_id
                else:
                    os.environ.pop("STAFF_PREVIEW_ASSET", None)
            else:
                os.environ.pop("STAFF_PREVIEW_ASSET", None)
            os.environ["EARLY_ART_PREVIEW_KIND"] = prev_kind
            rig_kw: dict = {}
            if asset_id and asset_id.startswith("garage-"):
                rig_kw["up_axis"] = "Z"
            rig.render_preview_studio(ob, path, mask_meshes=mask_meshes, **rig_kw)
            return
        except Exception as exc:  # noqa: BLE001
            print(f"WARN preview rig, fallback legacy: {exc}")
    bpy.context.scene.render.engine = "BLENDER_EEVEE"
    if hasattr(bpy.context.scene, "eevee"):
        bpy.context.scene.eevee.taa_render_samples = 16
    bpy.ops.object.camera_add(location=(2.2, -2.0, 1.6))
    cam = bpy.context.active_object
    direction = Vector((0, 0, 0.8)) - cam.location
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    bpy.context.scene.camera = cam
    bpy.ops.object.light_add(type="AREA", location=(1.5, -1.0, 2.5))
    light = bpy.context.active_object
    light.data.energy = 350
    bpy.context.scene.render.resolution_x = 512
    bpy.context.scene.render.resolution_y = 512
    bpy.context.scene.render.filepath = str(path)
    bpy.ops.render.render(write_still=True)


def write_manifest(entries: list[dict], manifest_name: str, extra: dict | None = None) -> None:
    payload = {"fidelity": "blender-baked", "blenderVersion": BLENDER_VERSION, "assets": entries}
    if extra:
        payload.update(extra)
    path = ART_ROOT / manifest_name
    path.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")


def run_script(main_fn) -> None:
    os.chdir(REPO_ROOT)
    reset_scene()
    entry = main_fn()
    if entry is None:
        return
    category = "garage-kit" if entry["id"].startswith("garage-") else "staff-kit"
    manifest = "garage-manifest.json" if category == "garage-kit" else "staff-manifest.json"
    manifest_path = ART_ROOT / manifest
    existing = {"assets": []}
    if manifest_path.exists():
        existing = json.loads(manifest_path.read_text())
    assets = [a for a in existing.get("assets", []) if a["id"] != entry["id"]]
    assets.append(entry)
    assets.sort(key=lambda a: a["id"])
    write_manifest(assets, manifest, extra={k: v for k, v in existing.items() if k != "assets"})