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
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cylinder_add(
        radius=radius, depth=depth, vertices=vertices, location=location
    )
    ob = bpy.context.active_object
    ob.name = name
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
        o.location.y += dy


def snap_root_to_ground(ob: bpy.types.Object) -> None:
    """Rest bottom vertices on y=0 (garage Y-up); edit-mesh snap survives remesh/voxel."""
    import bmesh

    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    mat_loc, mat_rot, _ = ob.matrix_world.decompose()
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    if not bm.verts:
        bm.free()
        return
    world_min = min((mat_loc + v.co).y for v in bm.verts)
    if abs(world_min) < 1e-4:
        bm.free()
        return
    for v in bm.verts:
        v.co.y -= world_min
    bm.to_mesh(ob.data)
    bm.free()
    ob.data.update()
    ob.location.y = 0.0


def voxel_shell_merge(ob: bpy.types.Object, voxel_size: float = 0.03) -> None:
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    mod = ob.modifiers.new("VoxelShell", "REMESH")
    mod.mode = "VOXEL"
    mod.voxel_size = voxel_size
    mod.use_smooth_shade = True
    bpy.ops.object.modifier_apply(modifier=mod.name)


def add_prejoin_spine(mesh_obs: list[bpy.types.Object]) -> None:
    """Tall core strut before join so multi-part staff props become one island."""
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
    span_x = max(0.16, (maxs.x - mins.x) * 0.42)
    spine.scale = (span_x, height * 0.55, span_z)
    bpy.ops.object.transform_apply(scale=True)
    mesh_obs.append(spine)


def add_connecting_spine(ob: bpy.types.Object) -> None:
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
    span_x = max(0.08, (maxs.x - mins.x) * 0.45)
    span_z = max(0.08, (maxs.z - mins.z) * 0.45)
    spine.scale = (span_x, height * 0.5 + 0.02, span_z)
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
    ground_scene_meshes()
    tint_garage_parts(mesh_obs, asset_id)
    if category == "staff-kit":
        add_prejoin_spine(mesh_obs)
    root = link_or_join(mesh_obs)
    snap_root_to_ground(root)
    if mesh_island_count(root) > 1:
        add_connecting_spine(root)
    voxel_default = category == "staff-kit"
    if mesh_island_count(root) > 1 and (
        voxel_default or os.environ.get("EARLY_ART_VOXEL_SHELL", "0") not in ("0", "false")
    ):
        if category == "staff-kit":
            if mesh_island_count(root) > 1:
                voxel_shell_merge(root, voxel_size=0.045)
            if mesh_island_count(root) > 1:
                voxel_shell_merge(root, voxel_size=0.058)
            if mesh_island_count(root) > 1:
                voxel_shell_merge(root, voxel_size=0.072)
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
    bevel_object(root, width=0.012, segments=2)
    smooth_shade(root)
    uv_unwrap(root)

    maps = bake_pbr_set(root, tex_dir, asset_id, size=1024)
    apply_preview_material(root, tex_dir, asset_id)
    islands_export = mesh_island_count(root)
    glb_path = out_dir / f"{asset_id}.glb"
    export_gltf(root, glb_path)
    preview_path = PREVIEW_ROOT / f"{asset_id}.png"
    try:
        render_preview(root, preview_path, asset_id=asset_id)
    except Exception as exc:  # noqa: BLE001 — headless apt build may lack denoiser
        print(f"WARN preview {asset_id}: {exc}")

    verts = len(root.data.vertices)
    tris = sum(len(p.vertices) for p in root.data.polygons)
    return {
        "id": asset_id,
        "path": f"Assets/Art/EarlyGame/{category}/{asset_id}.glb",
        "textures": {k: f"Assets/Art/EarlyGame/{category}/textures/{asset_id}/{asset_id}_{k}.png" for k in maps},
        "preview": f"docs/art/previews/{asset_id}.png",
        "license": license_note,
        "vertexCount": verts,
        "triangleCount": tris,
        "meshIslands": islands_export,
        "grounded": min_y <= 0.03,
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
    """Per-part saturated materials before join/bake."""
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
        if name.startswith("stroke"):
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
        elif asset_id.startswith("staff-") and ("leg" in name or "arm" in name):
            assign_solid(ob, (0.18, 0.22, 0.38, 1.0), 0.58)
        elif asset_id.startswith("staff-") and ("hips" in name or "pants" in name):
            assign_solid(ob, (0.22, 0.26, 0.42, 1.0), 0.62)
        elif asset_id.startswith("staff-") and ("vest" in name or "hivis" in name.lower()):
            assign_solid(ob, (0.98, 0.82, 0.08, 1.0), 0.35)
        elif asset_id.startswith("staff-") and "hood" in name.lower():
            assign_solid(ob, (0.42, 0.28, 0.52, 1.0), 0.48)
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
        elif asset_id.startswith("staff-") and ("foot" in name.lower() or "hand" in name.lower()):
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
        elif asset_id.startswith("staff-") and ("rolebadge" in name or "rolesash" in name):
            continue
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
            assign_solid(ob, (0.72, 0.70, 0.62, 1.0), 0.42)
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


def transfer_material_colors_to_vertex_layer(ob: bpy.types.Object) -> None:
    mesh = ob.data
    while mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[0])
    vcol = mesh.color_attributes.new(name="BakeCol", type="BYTE_COLOR", domain="CORNER")
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
    if asset_id.startswith("staff-"):
        hsv = nodes.new("ShaderNodeHueSaturation")
        hsv.inputs["Saturation"].default_value = 1.95
        hsv.inputs["Value"].default_value = 1.12
        links.new(color_out, hsv.inputs["Color"])
        color_out = hsv.outputs["Color"]
    links.new(color_out, bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = float(spec["roughness"])
    emit = nodes.new("ShaderNodeEmission")
    emit.inputs["Strength"].default_value = 1.4 if "crt" in asset_id or "rack" in asset_id else 0.0
    if emit.inputs["Strength"].default_value > 0:
        vcol_emit = nodes.new("ShaderNodeAttribute")
        vcol_emit.attribute_name = "BakeCol"
        links.new(vcol_emit.outputs["Color"], emit.inputs["Color"])
        mix = nodes.new("ShaderNodeMixShader")
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


def apply_preview_material(ob: bpy.types.Object, tex_dir: Path, asset_id: str) -> None:
    """EEVEE preview uses baked albedo; keeps vertex-color material when bake maps are flat."""
    albedo = tex_dir / f"{asset_id}_albedo.png"
    rough = tex_dir / f"{asset_id}_roughness.png"
    bake_col = getattr(ob.data, "color_attributes", None)
    has_bake_vcol = bake_col is not None and bake_col.get("BakeCol") is not None
    if (has_bake_vcol or ob.data.vertex_colors) and ob.data.materials and ob.data.materials[0]:
        mat = ob.data.materials[0]
        node_types = {n.type for n in mat.node_tree.nodes} if mat.use_nodes else set()
        if mat.name.endswith("_vcol") or "ShaderNodeAttribute" in node_types or "VERTEX_COLOR" in node_types:
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
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    if rough.exists():
        rtex = nodes.new("ShaderNodeTexImage")
        rtex.image = bpy.data.images.load(str(rough))
        links.new(rtex.outputs["Color"], bsdf.inputs["Roughness"])
    emit_strength = 0.0
    if "crt" in asset_id or "screen" in asset_id:
        emit_strength = 1.2
    bsdf.inputs["Emission Strength"].default_value = emit_strength
    bsdf.inputs["Emission Color"].default_value = (0.15, 0.65, 0.38, 1.0)
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def bake_pbr_set(ob: bpy.types.Object, tex_dir: Path, asset_id: str, size: int = 1024) -> list[str]:
    """Bake albedo, normal, roughness image maps."""
    from blender_material_palette import spec_for

    spec = spec_for(asset_id)
    kinds = ["albedo", "normal", "roughness"]
    paths: dict[str, str] = {}
    if len(ob.data.materials) > 1 or (ob.data.materials and ob.data.materials[0] and "_solid" in ob.data.materials[0].name):
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
            if kind == "albedo" and ob.data.vertex_colors:
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
        export_apply=False,
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


def render_preview(ob: bpy.types.Object, path: Path, asset_id: str | None = None) -> None:
    use_rig = os.environ.get("EARLY_ART_PREVIEW_RIG", "1").strip().lower()
    if use_rig not in ("0", "false", "no"):
        try:
            import blender_preview_rig as rig  # type: ignore

            prev_kind = "staff"
            if asset_id:
                if asset_id.startswith("wisp-"):
                    prev_kind = "wisp"
                elif asset_id.startswith("hud-") or asset_id.startswith("tex-hud-"):
                    prev_kind = "hud"
            os.environ["EARLY_ART_PREVIEW_KIND"] = prev_kind
            rig.render_preview_studio(ob, path)
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