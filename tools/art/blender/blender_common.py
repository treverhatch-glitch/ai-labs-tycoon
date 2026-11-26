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
from mathutils import Vector

REPO_ROOT = Path(__file__).resolve().parents[3]
ART_ROOT = REPO_ROOT / "unity/Assets/Art/EarlyGame"
PREVIEW_ROOT = REPO_ROOT / "docs/art/previews"
BLENDER_VERSION = f"{bpy.app.version[0]}.{bpy.app.version[1]}.{bpy.app.version[2]}"


def reset_scene() -> None:
    try:
        from garage_canonical_materials import clear_cache  # type: ignore

        clear_cache()
    except Exception:
        pass
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.context.scene.render.engine = "CYCLES"
    bpy.context.scene.cycles.device = "CPU"
    bpy.context.scene.cycles.samples = 32
    bpy.context.scene.cycles.use_denoising = False
    if bpy.context.preferences.addons.get("cycles") is None:
        bpy.ops.preferences.addon_enable(module="cycles")


def union_overlapping_meshes(objects: list[bpy.types.Object]) -> bpy.types.Object:
    """Boolean-union kit parts; prefer ConnectSpine as merge hub."""
    if not objects:
        raise RuntimeError("no objects to union")
    if len(objects) == 1:
        return objects[0]
    ordered = list(objects)
    spine_idx = next((i for i, o in enumerate(ordered) if o.name == "ConnectSpine"), None)
    if spine_idx is not None:
        ordered = [ordered[spine_idx]] + [o for i, o in enumerate(ordered) if i != spine_idx]
    base = ordered[0]
    bpy.ops.object.select_all(action="DESELECT")
    base.select_set(True)
    bpy.context.view_layer.objects.active = base
    for ob in ordered[1:]:
        mod = base.modifiers.new(f"Union_{ob.name}", "BOOLEAN")
        mod.operation = "UNION"
        mod.object = ob
        mod.solver = "EXACT"
        try:
            bpy.ops.object.modifier_apply(modifier=mod.name)
        except RuntimeError:
            mod.solver = "FLOAT"
            bpy.ops.object.modifier_apply(modifier=mod.name)
        bpy.data.objects.remove(ob, do_unlink=True)
    bpy.context.view_layer.objects.active = base
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=0.0001)
    bpy.ops.object.mode_set(mode="OBJECT")
    return base


def join_bridge_mesh(root: bpy.types.Object, bridge: bpy.types.Object) -> bpy.types.Object:
    """Join bridge spine without boolean (preserves material slots)."""
    bpy.ops.object.select_all(action="DESELECT")
    root.select_set(True)
    bridge.select_set(True)
    bpy.context.view_layer.objects.active = root
    bpy.ops.object.join()
    merged = bpy.context.view_layer.objects.active
    weld_extent = max(merged.dimensions) if max(merged.dimensions) > 0 else 1.0
    weld_coincident_vertices(merged, threshold=min(0.04, weld_extent * 0.035))
    return merged


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
    rotation_euler: tuple[float, float, float] | None = None,
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cylinder_add(
        radius=radius, depth=depth, vertices=vertices, location=location
    )
    ob = bpy.context.active_object
    ob.name = name
    if rotation_euler:
        ob.rotation_euler = rotation_euler
        bpy.ops.object.transform_apply(rotation=True)
    return ob


def weld_coincident_vertices(ob: bpy.types.Object, threshold: float = 0.06) -> int:
    """Merge overlapping verts after join (returns removed count)."""
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    result = bpy.ops.mesh.remove_doubles(threshold=threshold)
    bpy.ops.object.mode_set(mode="OBJECT")
    if isinstance(result, set):
        for r in result:
            if "Removed" in str(r):
                try:
                    return int(str(r).split()[-2])
                except (ValueError, IndexError):
                    pass
    return 0


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


def ground_scene_meshes() -> None:
    """Garage kit builds in Blender Z-up; rest lowest point on z=0."""
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        return
    min_z = min((o.matrix_world @ Vector(c)).z for o in meshes for c in o.bound_box)
    if min_z >= -1e-5:
        return
    dz = -min_z
    for o in meshes:
        o.location.z += dz


def mesh_world_min_z(ob: bpy.types.Object) -> float:
    if not ob.data.vertices:
        return 0.0
    return min((ob.matrix_world @ v.co).z for v in ob.data.vertices)


def apply_object_transforms(ob: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)


def snap_root_to_ground(ob: bpy.types.Object) -> None:
    """Rest lowest vertex on z=0 (Blender Z-up build)."""
    apply_object_transforms(ob)
    min_z = min((v.co.z for v in ob.data.vertices), default=0.0)
    if abs(min_z) > 1e-6:
        for v in ob.data.vertices:
            v.co.z -= min_z
        ob.data.update()
    ob.location = (0.0, 0.0, 0.0)
    bpy.context.view_layer.update()


def voxel_shell_merge(ob: bpy.types.Object, voxel_size: float = 0.03) -> None:
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    mod = ob.modifiers.new("VoxelShell", "REMESH")
    mod.mode = "VOXEL"
    mod.voxel_size = voxel_size
    mod.use_smooth_shade = True
    bpy.ops.object.modifier_apply(modifier=mod.name)


def _bounds_world(objects: list[bpy.types.Object]) -> tuple[Vector, Vector]:
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in objects:
        if ob.type != "MESH":
            continue
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins = Vector((min(mins.x, w.x), min(mins.y, w.y), min(mins.z, w.z)))
            maxs = Vector((max(maxs.x, w.x), max(maxs.y, w.y), max(maxs.z, w.z)))
    return mins, maxs


def add_connect_spine_part(objects: list[bpy.types.Object]) -> bpy.types.Object:
    """Invisible overlap core so non-touching kit parts boolean-union to one island."""
    mins, maxs = _bounds_world(objects)
    center = (mins + maxs) * 0.5
    span = maxs - mins
    bpy.ops.mesh.primitive_cube_add(size=1, location=center)
    spine = bpy.context.active_object
    spine.name = "ConnectSpine"
    # Tall assets: thin spine in X/Y; wide assets: span on all axes modestly.
    if span.z >= span.x * 1.05 and span.z >= span.y * 1.05:
        spine.scale = (
            max(0.15, span.x * 0.85),
            max(0.15, span.y * 0.85),
            max(0.15, span.z * 1.25),
        )
    else:
        spine.scale = (
            max(0.15, span.x * 1.25),
            max(0.15, span.y * 1.25),
            max(0.15, span.z * 1.25),
        )
    bpy.ops.object.transform_apply(scale=True)
    assign_solid(spine, (0.5, 0.5, 0.5, 1.0), 0.5)
    return spine


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


def island_centroids(ob: bpy.types.Object) -> list[Vector]:
    import bmesh

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    visited: set[int] = set()
    centers: list[Vector] = []
    for face in bm.faces:
        if face.index in visited:
            continue
        stack = [face]
        acc = Vector((0.0, 0.0, 0.0))
        count = 0
        while stack:
            f = stack.pop()
            if f.index in visited:
                continue
            visited.add(f.index)
            for v in f.verts:
                acc += v.co
                count += 1
            for e in f.edges:
                for linked in e.link_faces:
                    if linked.index not in visited:
                        stack.append(linked)
        if count > 0:
            centers.append(ob.matrix_world @ (acc / count))
    bm.free()
    return centers


def bridge_mesh_islands(ob: bpy.types.Object, iterations: int = 10) -> bpy.types.Object:
    from garage_canonical_materials import assign_slot  # type: ignore

    active = ob
    for _ in range(iterations):
        if mesh_island_count(active) <= 1:
            return active
        centers = island_centroids(active)
        if len(centers) < 2:
            return active
        a, b = centers[0], centers[1]
        mid = (a + b) * 0.5
        dist = max((a - b).length, 0.06)
        inv = active.matrix_world.inverted()
        local_mid = inv @ mid
        local_dir = (inv @ b - inv @ a).normalized()
        bpy.ops.mesh.primitive_cube_add(size=1, location=local_mid)
        link = bpy.context.active_object
        link.name = "IslandBridge"
        thick = max(0.045, min(0.09, dist * 0.08))
        link.scale = (thick, thick, dist * 0.55)
        link.rotation_euler = local_dir.to_track_quat("Z", "Y").to_euler()
        bpy.ops.object.transform_apply(scale=True, rotation=True)
        assign_slot(link, "dark_metal")
        active = join_bridge_mesh(active, link)
    return active


def uv_unwrap(ob: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="OBJECT")
    if bpy.ops.object.mode_set.poll():
        bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    try:
        bpy.ops.uv.smart_project(angle_limit=66, island_margin=0.02)
    except RuntimeError as exc:
        print(f"WARN uv.smart_project: {exc}")
        try:
            bpy.ops.uv.unwrap(method="ANGLE_BASED", margin=0.02)
        except RuntimeError:
            pass
    bpy.ops.object.mode_set(mode="OBJECT")


GARAGE_FURNITURE = frozenset(
    {
        "garage-desk-crt",
        "garage-folding-chair",
        "garage-server-closet",
        "garage-voodude-rack",
        "garage-coffee-drip",
        "garage-crt-tower",
        "garage-roll-door",
    }
)
GARAGE_TEXTURE_PROPS = frozenset({"garage-floor-tile"})
GARAGE_UPRIGHT_PROPS = frozenset(
    {
        "garage-roll-door",
        "garage-whiteboard-1995",
        "garage-land-for-sale-sign",
        "garage-window-frame",
    }
)
MIN_GARAGE_PARTS_SMALL = 5
MIN_GARAGE_PARTS_FURNITURE = 6


def snap_parts_to_ground(parts: list[bpy.types.Object]) -> None:
    if not parts:
        return
    min_z = min(mesh_world_min_z(ob) for ob in parts)
    if abs(min_z) < 1e-5:
        return
    for ob in parts:
        ob.location.z -= min_z
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


def export_garage_multipart_glbf(parts: list[bpy.types.Object], glb_path: Path) -> None:
    stash: list[tuple[bpy.types.Object, object]] = []
    export_parts = [ob for ob in parts if ob.name != "ConnectSpine"]
    for ob in export_parts:
        stash.append((ob, ob.matrix_world.copy()))
        orient_build_z_to_gltf_y_up(ob)
    bpy.ops.object.select_all(action="DESELECT")
    for ob in export_parts:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = export_parts[0]
    bpy.ops.export_scene.gltf(
        filepath=str(glb_path),
        use_selection=True,
        export_apply=False,
        export_format="GLB",
        export_image_format="AUTO",
        export_yup=True,
    )
    for ob, matrix in stash:
        ob.matrix_world = matrix


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
    ground_scene_meshes()
    tint_garage_parts(mesh_obs, asset_id)
    parts = list(mesh_obs)
    min_parts = (
        MIN_GARAGE_PARTS_FURNITURE if asset_id in GARAGE_FURNITURE else MIN_GARAGE_PARTS_SMALL
    )
    garage_multipart = asset_id.startswith("garage-")
    cc0_import = os.environ.get("GARAGE_CC0_IMPORT", "").strip() in ("1", "true", "yes")
    if garage_multipart and part_count < min_parts and not cc0_import:
        raise RuntimeError(f"{asset_id}: need >={min_parts} sub-objects (got {part_count})")

    bevel_w = 0.022 if garage_multipart else 0.012
    for ob in parts:
        bevel_object(ob, width=bevel_w, segments=3)
        smooth_shade(ob)
    snap_parts_to_ground(parts)
    for ob in parts:
        z0 = mesh_world_min_z(ob)
        if z0 > 0.025:
            ob.location.z -= z0
            bpy.context.view_layer.update()
        z0 = mesh_world_min_z(ob)
        if z0 > 0.06:
            raise RuntimeError(f"{asset_id}:{ob.name} not grounded (min_z={z0:.4f})")

    if garage_multipart:
        bake_root = duplicate_join_meshes(parts, f"{asset_id}_bake")
        uv_unwrap(bake_root)
        mat_snapshot = snapshot_poly_materials(bake_root)
        maps = bake_pbr_set(bake_root, tex_dir, asset_id, size=1024)
        restore_poly_materials(bake_root, mat_snapshot)
        verts = len(bake_root.data.vertices)
        tris = sum(len(p.vertices) for p in bake_root.data.polygons)
        bpy.data.objects.remove(bake_root, do_unlink=True)
        glb_path = out_dir / f"{asset_id}.glb"
        export_garage_multipart_glbf(parts, glb_path)
        preview_join = duplicate_join_meshes(parts, f"{asset_id}_preview")
        transfer_material_colors_to_vertex_layer(preview_join)
        unify_vertex_color_material(preview_join, asset_id)
        for ob in parts:
            ob.hide_render = True
        preview_targets = [preview_join]
    else:
        root = parts[0]
        uv_unwrap(root)
        maps = bake_pbr_set(root, tex_dir, asset_id, size=1024)
        verts = len(root.data.vertices)
        tris = sum(len(p.vertices) for p in root.data.polygons)
        glb_path = out_dir / f"{asset_id}.glb"
        snap_root_to_ground(root)
        apply_object_transforms(root)
        export_gltf(root, glb_path, y_up=True)
        preview_targets = [root]

    garage_preview_join = None
    if garage_multipart and preview_targets and preview_targets[0].name.endswith("_preview"):
        garage_preview_join = preview_targets[0]
    else:
        for ob in preview_targets:
            apply_preview_material(ob, tex_dir, asset_id)
    preview_path = PREVIEW_ROOT / f"{asset_id}.png"
    try:
        render_preview(
            preview_targets if len(preview_targets) > 1 else preview_targets[0],
            preview_path,
        )
    except Exception as exc:  # noqa: BLE001 — headless apt build may lack denoiser
        print(f"WARN preview {asset_id}: {exc}")
    if garage_preview_join is not None:
        for ob in parts:
            ob.hide_render = False
        bpy.data.objects.remove(garage_preview_join, do_unlink=True)

    return {
        "id": asset_id,
        "path": f"Assets/Art/EarlyGame/{category}/{asset_id}.glb",
        "textures": {k: f"Assets/Art/EarlyGame/{category}/textures/{asset_id}/{asset_id}_{k}.png" for k in maps},
        "preview": f"docs/art/previews/{asset_id}.png",
        "license": license_note,
        "vertexCount": verts,
        "triangleCount": tris,
        "partCount": part_count,
        "blenderVersion": BLENDER_VERSION,
        "generator": f"tools/art/blender/{asset_id}.py",
    }


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
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def tint_garage_parts(mesh_obs: list[bpy.types.Object], asset_id: str) -> None:
    """Per-part canonical materials (≤4 slots per prop) before join/bake."""
    from garage_canonical_materials import assign_slot  # type: ignore

    cc0_import = os.environ.get("GARAGE_CC0_IMPORT", "").strip().lower() in ("1", "true", "yes")
    stroke_slots = ("marker_red", "marker_blue", "marker_green", "plastic_orange")
    for ob in mesh_obs:
        if ob.data.materials and ob.data.materials[0]:
            nt = ob.data.materials[0].node_tree
            if nt and nt.nodes.get("Principled BSDF"):
                em = nt.nodes["Principled BSDF"].inputs["Emission Strength"].default_value
                if em and em > 0.1:
                    continue
        name = ob.name.lower()
        if name.startswith("stroke"):
            idx = int("".join(c for c in name if c.isdigit()) or "0")
            assign_slot(ob, stroke_slots[idx % len(stroke_slots)])
        elif "screen" in name or "led" in name:
            continue
        elif "leg" in name or "rail" in name or "brace" in name:
            assign_slot(ob, "steel")
        elif "stripe" in name:
            assign_slot(ob, "plastic_orange")
        elif "frame" in name or "tray" in name or "trim" in name or "baseboard" in name or name == "sill":
            assign_slot(ob, "plastic_orange")
        elif asset_id == "garage-floor-tile":
            assign_slot(ob, "concrete" if "slab" in name else "plastic_orange")
        elif asset_id == "garage-cable-spool" and "hub" in name:
            assign_slot(ob, "wood")
        elif asset_id == "garage-cable-spool" and ("coil" in name or "flange" in name.lower()):
            assign_slot(ob, "coil_copper")
        elif asset_id == "garage-cable-spool" and "cable" in name:
            assign_slot(ob, "cable_black")
        elif asset_id == "garage-lot-curb" and "curb" in name:
            assign_slot(ob, "curb_grey")
        elif asset_id == "garage-lot-curb" and "grass" in name:
            assign_slot(ob, "grass")
        elif asset_id == "garage-whiteboard-1995" and "chart" in name:
            assign_slot(ob, "marker_green")
        elif asset_id == "garage-whiteboard-1995" and "padred" in name:
            assign_slot(ob, "marker_red")
        elif asset_id == "garage-whiteboard-1995" and "padblue" in name:
            assign_slot(ob, "marker_blue")
        elif asset_id == "garage-lot-curb":
            assign_slot(ob, "concrete")
        elif asset_id == "garage-wall-panel" and "stripe" in name:
            assign_slot(ob, "plastic_orange")
        elif asset_id == "garage-server-closet" and "handle" in name:
            assign_slot(ob, "plastic_orange")
        elif "slat" in name:
            assign_slot(ob, "corrugated")
        elif asset_id == "garage-coffee-drip" and "base" in name:
            assign_slot(ob, "plastic_dark")
        elif asset_id == "garage-coffee-drip" and "cord" in name:
            assign_slot(ob, "cable_black")
        elif asset_id == "garage-coffee-drip":
            assign_slot(ob, "plastic_orange")
        elif asset_id == "garage-land-for-sale-sign" and "post" in name:
            assign_slot(ob, "steel")
        elif asset_id == "garage-land-for-sale-sign":
            assign_slot(ob, "sign_yellow")
        elif asset_id == "garage-roll-door" and ("curtain" in name or "slat" in name):
            assign_slot(ob, "corrugated")
        elif asset_id == "garage-roll-door":
            assign_slot(ob, "plastic_orange")
        elif asset_id == "garage-exterior-wall" and "core" in name:
            assign_slot(ob, "plastic_beige")
        elif asset_id == "garage-whiteboard-1995" and "surface" in name:
            assign_slot(ob, "whiteboard")
        elif asset_id == "garage-whiteboard-1995" and name.startswith("stroke"):
            continue
        elif asset_id == "garage-whiteboard-1995":
            assign_slot(ob, "frame_wood")
        elif asset_id == "garage-window-frame" and "pane" in name:
            assign_slot(ob, "glass_dark")
        elif asset_id == "garage-window-frame":
            assign_slot(ob, "plastic_orange")
        elif asset_id == "garage-folding-chair" and "seat" in name:
            assign_slot(ob, "wood")
        elif asset_id == "garage-folding-chair":
            assign_slot(ob, "steel")
        elif cc0_import and asset_id == "garage-desk-crt" and name.startswith(("crt_", "keyboard")):
            assign_slot(ob, "crt_bezel" if "bezel" in name else "keyboard_grey")
        elif cc0_import and asset_id == "garage-desk-crt" and ("metal" in name or "frame" in name or "leg" in name):
            assign_slot(ob, "steel")
        elif cc0_import and asset_id == "garage-desk-crt":
            assign_slot(ob, "desk_wood")
        elif cc0_import and asset_id == "garage-roll-door" and name.startswith("slat"):
            assign_slot(ob, "corrugated")
        elif asset_id == "garage-desk-crt" and "desktop" in name:
            assign_slot(ob, "desk_wood")
        elif asset_id == "garage-desk-crt" and "drawer" in name:
            assign_slot(ob, "wood")
        elif asset_id == "garage-desk-crt" and ("keyboard" in name or name.startswith("key")):
            assign_slot(ob, "keyboard_grey")
        elif asset_id == "garage-desk-crt" and ("crt" in name or "bezel" in name):
            assign_slot(ob, "crt_bezel")
        elif asset_id == "garage-desk-crt" and "leg" in name:
            assign_slot(ob, "steel")
        elif asset_id == "garage-crt-tower" and "screen" in name:
            continue
        elif asset_id == "garage-crt-tower" and "led" in name:
            continue
        elif asset_id == "garage-crt-tower":
            assign_slot(ob, "crt_bezel")
        elif asset_id == "garage-voodude-rack" and "led" in name:
            continue
        elif asset_id == "garage-voodude-rack":
            assign_slot(ob, "dark_metal")
        elif asset_id == "garage-server-closet":
            assign_slot(ob, "dark_metal")
        elif "surface" in name:
            assign_slot(ob, "whiteboard")
        elif asset_id.startswith("garage-"):
            assign_slot(ob, "plastic_beige")


def assign_emission(ob: bpy.types.Object, rgba: tuple[float, float, float, float], strength: float = 2.0) -> None:
    mat = bpy.data.materials.new(ob.name + "_emit")
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes["Principled BSDF"]
    bsdf.inputs["Base Color"].default_value = rgba
    bsdf.inputs["Emission Color"].default_value = rgba
    bsdf.inputs["Emission Strength"].default_value = strength
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def _boost_saturation(rgba: tuple[float, float, float, float], scale: float = 1.05) -> tuple[float, float, float, float]:
    h, s, v = colorsys.rgb_to_hsv(rgba[0], rgba[1], rgba[2])
    s = min(1.0, s * scale)
    v = min(1.0, v * 1.08)
    r, g, b = colorsys.hsv_to_rgb(h, s, v)
    return (r, g, b, rgba[3])


def ensure_garage_export_materials(ob: bpy.types.Object, asset_id: str = "") -> None:
    """Guarantee ≥2 named URP slots when boolean union collapses material indices."""
    from garage_canonical_materials import get_canonical_material  # type: ignore

    slot_names = {
        "garage-folding-chair": ("steel", "wood"),
        "garage-desk-crt": ("desk_wood", "crt_bezel", "steel", "keyboard_grey"),
        "garage-crt-tower": ("crt_bezel", "glass_dark", "plastic_beige"),
        "garage-cable-spool": ("wood", "cable_black", "coil_copper"),
        "garage-roll-door": ("corrugated", "plastic_orange", "dark_metal"),
        "garage-server-closet": ("dark_metal", "plastic_orange"),
        "garage-lot-curb": ("curb_grey", "grass", "concrete"),
        "garage-coffee-drip": ("plastic_orange", "plastic_dark"),
        "garage-land-for-sale-sign": ("sign_yellow", "steel"),
        "garage-whiteboard-1995": ("whiteboard", "frame_wood", "marker_red"),
        "garage-window-frame": ("plastic_orange", "glass_dark"),
        "garage-wall-panel": ("plastic_beige", "plastic_orange"),
        "garage-exterior-wall": ("plastic_beige", "concrete"),
        "garage-floor-tile": ("concrete", "plastic_orange"),
        "garage-voodude-rack": ("dark_metal", "steel"),
    }
    names = list(slot_names.get(asset_id, ("plastic_beige", "steel", "dark_metal", "wood")))
    n_mats = len([m for m in ob.data.materials if m is not None])
    if n_mats >= len(names):
        return
    ob.data.materials.clear()
    for name in names[:4]:
        ob.data.materials.append(get_canonical_material(name))
    z_vals: list[float] = []
    for poly in ob.data.polygons:
        acc = 0.0
        for vi in poly.vertices:
            acc += ob.data.vertices[vi].co.z
        z_vals.append(acc / len(poly.vertices))
    if not z_vals:
        return
    sorted_z = sorted(z_vals)
    buckets = max(2, min(len(names), 4))
    thresholds = [sorted_z[int(len(sorted_z) * (i + 1) / buckets)] for i in range(buckets - 1)]
    for poly, z in zip(ob.data.polygons, z_vals, strict=False):
        bucket = 0
        for t in thresholds:
            if z > t:
                bucket += 1
        poly.material_index = min(bucket, len(ob.data.materials) - 1)
    if asset_id in {"garage-roll-door", "garage-whiteboard-1995"}:
        mats_n = len(ob.data.materials)
        for i, poly in enumerate(ob.data.polygons):
            poly.material_index = i % mats_n


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


def transfer_material_colors_to_vertex_layer(ob: bpy.types.Object) -> None:
    mesh = ob.data
    attr_name = "BakeCol"
    if attr_name in mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[attr_name])
    vcol = mesh.color_attributes.new(name=attr_name, type="FLOAT_COLOR", domain="CORNER")
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
        if mat and mat.name == "Garage_whiteboard":
            rgba = _boost_saturation((0.78, 0.84, 0.92, 1.0), scale=1.4)
        else:
            rgba = _boost_saturation(rgba)
        for loop_idx in poly.loop_indices:
            vcol.data[loop_idx].color = (*rgba[:3], 1.0)


def apply_garage_bake_shader(ob: bpy.types.Object, asset_id: str) -> None:
    """Temporary single shader for albedo bake from part material colours (not random faces)."""
    from blender_material_palette import spec_for

    spec = spec_for(asset_id)
    mat = bpy.data.materials.new(f"{asset_id}_bake_vcol")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    if bpy.app.version >= (3, 4, 0):
        attr = nodes.new("ShaderNodeAttribute")
        attr.attribute_name = "BakeCol"
        attr_col = attr.outputs.get("Color", attr.outputs[0])
    else:
        attr = nodes.new("ShaderNodeVertexColor")
        attr.layer_name = "BakeCol"
        attr_col = attr.outputs["Color"]
    hue = nodes.new("ShaderNodeHueSaturation")
    hue.inputs["Value"].default_value = 1.02
    hue.inputs["Saturation"].default_value = 1.28
    links.new(attr_col, hue.inputs["Color"])
    links.new(hue.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = float(spec["roughness"])
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def unify_vertex_color_material(ob: bpy.types.Object, asset_id: str) -> None:
    from blender_material_palette import spec_for

    spec = spec_for(asset_id)
    glow_ids = {
        "garage-land-for-sale-sign",
        "garage-whiteboard-1995",
        "garage-window-frame",
        "garage-floor-tile",
    }
    dull_ids = {
        "garage-coffee-drip",
        "garage-roll-door",
        "garage-whiteboard-1995",
    }
    mat = bpy.data.materials.new(f"{asset_id}_vcol")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    if bpy.app.version >= (3, 4, 0):
        attr = nodes.new("ShaderNodeAttribute")
        attr.attribute_name = "BakeCol"
        attr_col = attr.outputs.get("Color", attr.outputs[0])
    else:
        attr = nodes.new("ShaderNodeVertexColor")
        attr.layer_name = "BakeCol"
        attr_col = attr.outputs["Color"]
    hue = nodes.new("ShaderNodeHueSaturation")
    hue.inputs["Value"].default_value = 1.10 if asset_id in dull_ids else 1.08
    if asset_id in dull_ids:
        hue.inputs["Saturation"].default_value = 4.85
    else:
        hue.inputs["Saturation"].default_value = 4.0 if asset_id in glow_ids else 3.65
    links.new(attr_col, hue.inputs["Color"])
    links.new(hue.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = float(spec["roughness"])
    emit = nodes.new("ShaderNodeEmission")
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
    if emit.inputs["Strength"].default_value > 0:
        if bpy.app.version >= (3, 4, 0):
            vcol_emit = nodes.new("ShaderNodeAttribute")
            vcol_emit.attribute_name = "BakeCol"
            e_out = vcol_emit.outputs.get("Color", vcol_emit.outputs[0])
        else:
            vcol_emit = nodes.new("ShaderNodeVertexColor")
            vcol_emit.layer_name = "BakeCol"
            e_out = vcol_emit.outputs["Color"]
        links.new(e_out, emit.inputs["Color"])
        mix = nodes.new("ShaderNodeMixShader")
        mix.inputs["Fac"].default_value = (
            0.45
            if "crt" in asset_id
            else (
                0.48
                if asset_id in glow_ids
                else (
                    0.44
                    if asset_id in dull_ids
                    else (0.35 if asset_id == "garage-desk-crt" else 0.30)
                )
            )
        )
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
        hue.inputs["Saturation"].default_value = 1.32
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
    """EEVEE preview uses baked albedo; keeps vertex-color material when bake maps are flat."""
    albedo = tex_dir / f"{asset_id}_albedo.png"
    rough = tex_dir / f"{asset_id}_roughness.png"
    if asset_id.startswith("garage-"):
        mat0 = ob.data.materials[0] if ob.data.materials else None
        if mat0 and mat0.name.startswith("Garage_"):
            _boost_garage_part_preview_material(ob)
            return
    if ob.data.materials and ob.data.materials[0]:
        mat = ob.data.materials[0]
        skip_vcol = (
            mat.name.endswith("_vcol")
            or mat.name.endswith("_bake_vcol")
            or "VertexColor" in {n.type for n in mat.node_tree.nodes}
        )
        if skip_vcol and not asset_id.startswith("garage-"):
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
        hue.inputs["Saturation"].default_value = 1.48
    else:
        hue.inputs["Saturation"].default_value = 1.45
    hue.inputs["Value"].default_value = 1.06 if asset_id.startswith("garage-") else 1.05
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
    garage_bake = asset_id.startswith("garage-") and asset_id not in GARAGE_TEXTURE_PROPS
    use_vcol = garage_bake
    use_vcol = use_vcol or (
        len(ob.data.materials) > 1
        and not garage_bake
    )
    use_vcol = use_vcol or (
        ob.data.materials and ob.data.materials[0] and "_solid" in ob.data.materials[0].name
    )
    if garage_bake:
        if "BakeCol" not in ob.data.color_attributes:
            transfer_material_colors_to_vertex_layer(ob)
        apply_garage_bake_shader(ob, asset_id)
    elif use_vcol:
        if "BakeCol" not in ob.data.color_attributes:
            transfer_material_colors_to_vertex_layer(ob)
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
            if kind == "albedo" and use_vcol:
                slot = ob.data.materials[0]
                nt = slot.node_tree
                outn = nt.nodes.get("Material Output")
                hue_n = next((n for n in nt.nodes if n.type == "HUE_SAT"), None)
                bake_emit = nt.nodes.new("ShaderNodeEmission")
                if hue_n:
                    nt.links.new(hue_n.outputs["Color"], bake_emit.inputs["Color"])
                bake_emit.inputs["Strength"].default_value = 1.0
                prev = (
                    outn.inputs["Surface"].links[0].from_socket if outn.inputs["Surface"].links else None
                )
                nt.links.new(bake_emit.outputs["Emission"], outn.inputs["Surface"])
                bpy.ops.object.bake(type="EMIT")
                if outn.inputs["Surface"].links:
                    nt.links.remove(outn.inputs["Surface"].links[0])
                nt.nodes.remove(bake_emit)
                if prev:
                    nt.links.new(prev, outn.inputs["Surface"])
            elif kind == "albedo" and ob.data.vertex_colors:
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

    if "albedo" in paths:
        _multiply_ao_into_albedo(ob, tex_dir, asset_id, size, paths["albedo"])

    return list(paths.keys())


def _multiply_ao_into_albedo(
    ob: bpy.types.Object, tex_dir: Path, asset_id: str, size: int, albedo_path: str
) -> None:
    """Bake ambient occlusion into albedo for contact detail."""
    ao_img = bpy.data.images.new(f"{asset_id}_ao_tmp", width=size, height=size)
    bake_node = None
    bake_mat = ob.data.materials[0] if ob.data.materials else None
    if bake_mat and bake_mat.use_nodes:
        bake_node = bake_mat.node_tree.nodes.new("ShaderNodeTexImage")
        bake_node.image = ao_img
        bake_mat.node_tree.nodes.active = bake_node
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    try:
        bpy.ops.object.bake(type="AO")
    except Exception as exc:
        print(f"WARN ao bake {asset_id}: {exc}")
        bpy.data.images.remove(ao_img)
        return
    albedo = bpy.data.images.load(albedo_path)
    w, h = albedo.size
    al_px = list(albedo.pixels)
    ao_px = list(ao_img.pixels)
    for i in range(0, len(al_px), 4):
        j = i
        ao = 0.35 + 0.65 * ao_px[j]
        al_px[i] *= ao
        al_px[i + 1] *= ao
        al_px[i + 2] *= ao
    albedo.pixels = al_px
    albedo.save()
    if bake_mat and bake_mat.use_nodes and bake_node:
        bake_mat.node_tree.nodes.remove(bake_node)
    bpy.data.images.remove(ao_img)


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
    min_y = min((v.co.y for v in ob.data.vertices), default=0.0)
    if min_y > 0.02:
        for v in ob.data.vertices:
            v.co.y -= min_y
        ob.data.update()
    ob.location = (0.0, 0.0, 0.0)


def export_gltf(
    ob: bpy.types.Object, path: Path, *, y_up: bool = True, apply: bool = True
) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.ops.export_scene.gltf(
        filepath=str(path),
        use_selection=True,
        export_apply=apply,
        export_format="GLB",
        export_image_format="AUTO",
        export_yup=y_up,
    )


def asset_id_from_path(path: Path) -> str:
    return path.stem


def render_preview(ob: bpy.types.Object | list[bpy.types.Object], path: Path) -> None:
    use_rig = os.environ.get("EARLY_ART_PREVIEW_RIG", "1").strip().lower()
    if use_rig not in ("0", "false", "no"):
        try:
            import blender_preview_rig as rig  # type: ignore

            focus = ob if isinstance(ob, list) else [ob]
            axis = "Z" if asset_id_from_path(path).startswith("garage-") else "Z"
            rig.render_preview_studio(focus, path, up_axis=axis)
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