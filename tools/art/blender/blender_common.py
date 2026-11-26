"""
Shared helpers for headless Blender early-game asset scripts (import from repo; not run alone).
"""
from __future__ import annotations

import colorsys
import hashlib
import json
import math
import os
from pathlib import Path

import bpy
from mathutils import Vector

REPO_ROOT = Path(__file__).resolve().parents[3]
PACKET34_ENV_IDS = {
    "garage-exterior-wall",
    "garage-land-for-sale-sign",
    "garage-lot-curb",
    "garage-window-frame",
}
ART_ROOT = REPO_ROOT / "unity/Assets/Art/EarlyGame"
PREVIEW_ROOT = REPO_ROOT / "docs/art/previews"
BLENDER_VERSION = f"{bpy.app.version[0]}.{bpy.app.version[1]}.{bpy.app.version[2]}"


def file_sha256(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b""):
            h.update(chunk)
    return h.hexdigest()


def purge_all_data() -> None:
    """Drop meshes/materials/images so GLB import cannot reuse stale assets."""
    for block in (bpy.data.meshes, bpy.data.materials, bpy.data.images, bpy.data.cameras, bpy.data.lights):
        for datablock in list(block):
            if datablock.users == 0:
                block.remove(datablock)


def reset_scene() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    purge_all_data()
    bpy.context.scene.render.engine = "CYCLES"
    bpy.context.scene.cycles.device = "CPU"
    bpy.context.scene.cycles.samples = 32
    bpy.context.scene.cycles.use_denoising = False
    if bpy.context.preferences.addons.get("cycles") is None:
        bpy.ops.preferences.addon_enable(module="cycles")


def union_overlapping_meshes(objects: list[bpy.types.Object]) -> bpy.types.Object:
    """Boolean-union kit parts that overlap (one manifold island)."""
    if not objects:
        raise RuntimeError("no objects to union")
    if len(objects) == 1:
        return objects[0]
    base = objects[0]
    bpy.ops.object.select_all(action="DESELECT")
    base.select_set(True)
    bpy.context.view_layer.objects.active = base
    for ob in objects[1:]:
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
    # primitive_cube_add(size=1) → 1m cube; scale directly to target meters.
    ob.scale = (size[0], size[1], size[2])
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
    """Early garage scripts use Blender Y as up; rest on y=0."""
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        return
    min_y = min(mesh_world_min_y(o) for o in meshes)
    if min_y >= -1e-5:
        return
    dy = -min_y
    for o in meshes:
        o.location.y += dy


def mesh_world_min_y(ob: bpy.types.Object) -> float:
    return min((ob.matrix_world @ v.co).y for v in ob.data.vertices)


def mesh_world_min_z(ob: bpy.types.Object) -> float:
    return min((ob.matrix_world @ v.co).z for v in ob.data.vertices)


def rotate_script_y_to_export_up(ob: bpy.types.Object) -> None:
    """Authoring uses Blender +Y as vertical; glTF/Unity need +Y after Blender Z-up export."""
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    ob.rotation_euler = (math.pi / 2, 0, 0)
    bpy.ops.object.transform_apply(rotation=True)


def snap_root_to_ground_z(ob: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    min_z = mesh_world_min_z(ob)
    if abs(min_z) > 1e-5:
        ob.location.z -= min_z
    bpy.context.view_layer.update()


def snap_root_to_ground(ob: bpy.types.Object) -> None:
    """Rest lowest vertex on y=0 (garage Y-up)."""
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    min_y = mesh_world_min_y(ob)
    if abs(min_y) > 1e-5:
        ob.location.y -= min_y
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
    spine.scale = (
        max(0.06, span.x * 0.58),
        max(0.08, span.y * 0.96),
        max(0.06, span.z * 0.58),
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
    part_names = [o.name for o in mesh_obs]
    sub_shape_count = len(mesh_obs)
    ground_scene_meshes()
    if asset_id in PACKET34_ENV_IDS:
        apply_packet34_authoring_textures(mesh_obs, asset_id)
    else:
        tint_garage_parts(mesh_obs, asset_id)
    parts = list(mesh_obs)
    if len(parts) > 1:
        parts.append(add_connect_spine_part(parts))
        root = union_overlapping_meshes(parts)
    else:
        root = mesh_obs[0]
    weld_extent = max(root.dimensions) if max(root.dimensions) > 0 else 1.0
    weld_coincident_vertices(root, threshold=min(0.002, weld_extent * 0.002))
    islands = mesh_island_count(root)
    if islands > 1 and asset_id not in PACKET34_ENV_IDS:
        voxel = min(0.055, weld_extent * 0.014)
        voxel_shell_merge(root, voxel_size=voxel)
        islands = mesh_island_count(root)
    if islands > 1:
        raise RuntimeError(f"{asset_id}: {islands} disconnected mesh islands (need 1)")
    snap_root_to_ground(root)
    min_y = mesh_world_min_y(root)
    if min_y > 0.008:
        raise RuntimeError(f"{asset_id}: not grounded (min_y={min_y:.4f})")
    thin = min(root.dimensions)
    if thin >= 0.14:
        bevel_object(root, width=min(0.010, thin * 0.08), segments=2)
    smooth_shade(root)
    uv_unwrap(root)

    maps = bake_pbr_set(root, tex_dir, asset_id, size=1024)
    packet34 = asset_id in PACKET34_ENV_IDS
    if packet34:
        apply_packet34_gltf_materials(root, asset_id, tex_dir, sub_shape_count, part_names)
    else:
        apply_preview_material(root, tex_dir, asset_id)
    rotate_script_y_to_export_up(root)
    snap_root_to_ground_z(root)
    min_z = mesh_world_min_z(root)
    if min_z > 0.012:
        raise RuntimeError(f"{asset_id}: not grounded after Y-up export rotate (min_z={min_z:.4f})")
    upright_env = {
        "garage-exterior-wall",
        "garage-land-for-sale-sign",
        "garage-window-frame",
    }
    if asset_id in upright_env:
        h = float(root.dimensions.z)
        thin = min(float(root.dimensions.x), float(root.dimensions.y))
        if h <= thin * 1.25:
            raise RuntimeError(f"{asset_id}: not upright for glTF (z={h:.3f} thin={thin:.3f})")
    glb_path = out_dir / f"{asset_id}.glb"
    export_gltf(root, glb_path, packet34=packet34)
    if not packet34:
        preview_path = PREVIEW_ROOT / f"{asset_id}.png"
        try:
            render_preview(root, preview_path)
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
        "subShapeCount": sub_shape_count,
        "blenderVersion": BLENDER_VERSION,
        "generator": f"tools/art/blender/{asset_id}.py",
    }


def reexport_packet34_glb(asset_id: str, category: str = "garage-kit") -> None:
    """Re-embed baked maps / swatches after post-bake albedo edits (e.g. FOR SALE paint)."""
    glb = ART_ROOT / category / f"{asset_id}.glb"
    if not glb.is_file():
        raise RuntimeError(f"reexport {asset_id}: missing glb")
    tex_dir = ART_ROOT / category / "textures" / asset_id
    reset_scene()
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        raise RuntimeError(f"reexport {asset_id}: no mesh")
    ob = meshes[0]
    parts = int(ob.get("packet34_part_count") or 0)
    try:
        part_names = json.loads(str(ob.get("packet34_part_names") or "[]"))
    except json.JSONDecodeError:
        part_names = []
    if parts <= 0:
        parts = max(len(part_names), 3)
    apply_packet34_gltf_materials(ob, asset_id, tex_dir, parts, part_names or [asset_id])
    export_gltf(ob, glb, packet34=True)


def rerender_garage_preview(asset_id: str, category: str = "garage-kit") -> None:
    """Re-import committed GLB in a clean scene and render studio preview."""
    reset_scene()
    glb = (ART_ROOT / category / f"{asset_id}.glb").resolve()
    if not glb.is_file():
        raise RuntimeError(f"rerender {asset_id}: missing glb {glb}")
    glb_sha = file_sha256(glb)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        raise RuntimeError(f"rerender {asset_id}: no mesh after glTF import")
    mesh_obs = [o for o in meshes if o.name != "PreviewShadowFloor"]
    if len(mesh_obs) > 1:
        bpy.ops.object.select_all(action="DESELECT")
        for o in mesh_obs:
            o.select_set(True)
        bpy.context.view_layer.objects.active = mesh_obs[0]
        bpy.ops.object.join()
        mesh_obs = [bpy.context.active_object]
    ob = mesh_obs[0]
    tex_dir = ART_ROOT / category / "textures" / asset_id
    if asset_id in PACKET34_ENV_IDS:
        apply_packet34_preview_material(ob, asset_id, tex_dir)
    else:
        apply_preview_material(ob, tex_dir, asset_id)
    preview_path = PREVIEW_ROOT / f"{asset_id}.png"
    os.environ["PREVIEW_GLB_SHA256"] = glb_sha
    os.environ["PREVIEW_GLB_PATH"] = str(glb.relative_to(REPO_ROOT))
    render_preview(ob, preview_path)


def assign_image_texture(
    ob: bpy.types.Object,
    image_path: Path,
    *,
    uv_scale: float = 1.0,
    roughness: float = 0.55,
    metallic: float = 0.0,
    alpha: float = 1.0,
) -> None:
    """UV-mapped PBR material from a tile swatch (used before bake on packet34 env)."""
    image_path = image_path.resolve()
    mat = bpy.data.materials.new(ob.name + "_img")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    tex = nodes.new("ShaderNodeTexImage")
    tex.image = bpy.data.images.load(str(image_path), check_existing=True)
    tex.image.name = image_path.name
    mapping = nodes.new("ShaderNodeMapping")
    mapping.inputs["Scale"].default_value = (uv_scale, uv_scale, uv_scale)
    coord = nodes.new("ShaderNodeTexCoord")
    links.new(coord.outputs["UV"], mapping.inputs["Vector"])
    links.new(mapping.outputs["Vector"], tex.inputs["Vector"])
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = roughness
    bsdf.inputs["Metallic"].default_value = metallic
    if alpha < 0.99:
        mat.blend_method = "BLEND"
        bsdf.inputs["Alpha"].default_value = alpha
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def apply_packet34_authoring_textures(mesh_obs: list[bpy.types.Object], asset_id: str) -> None:
    swatch = ART_ROOT / "textures"
    wall = swatch / "tex-garage-wall-beige.png"
    trim = swatch / "tex-garage-trim-orange.png"
    for ob in mesh_obs:
        name = ob.name.lower()
        if asset_id == "garage-exterior-wall":
            if wall.is_file() and name not in ("trim", "base") and not name.startswith("course"):
                assign_image_texture(ob, wall, uv_scale=1.35, roughness=0.62)
            elif trim.is_file():
                assign_image_texture(ob, trim, uv_scale=2.8, roughness=0.42)
        elif asset_id == "garage-window-frame":
            if "glass" in name:
                assign_solid(ob, (0.22, 0.55, 0.82, 1.0), 0.08)
                if ob.data.materials:
                    ob.data.materials[0].use_nodes = True
                    bsdf = ob.data.materials[0].node_tree.nodes.get("Principled BSDF")
                    if bsdf:
                        key = "Transmission Weight" if "Transmission Weight" in bsdf.inputs else "Transmission"
                        bsdf.inputs[key].default_value = 0.92
            elif trim.is_file() and name in ("sill", "lintel"):
                assign_image_texture(ob, trim, uv_scale=3.0, roughness=0.40)
            elif wall.is_file():
                assign_image_texture(ob, wall, uv_scale=2.0, roughness=0.50)
            else:
                assign_image_texture(ob, wall if wall.is_file() else trim, uv_scale=2.0, roughness=0.48)
        elif asset_id == "garage-lot-curb":
            assign_solid(
                ob,
                (0.56, 0.54, 0.52, 1.0) if "stain" not in name else (0.40, 0.38, 0.36, 1.0),
                0.88 if "gutter" not in name else 0.78,
            )
        elif asset_id == "garage-land-for-sale-sign":
            if "post" in name:
                assign_solid(ob, (0.44, 0.32, 0.18, 1.0), 0.58)
            elif trim.is_file() and ("header" in name or "cap" in name):
                assign_image_texture(ob, trim, uv_scale=4.0, roughness=0.36)
            elif "board" in name:
                assign_solid(ob, (0.98, 0.92, 0.22, 1.0), 0.34)
            else:
                assign_solid(ob, (0.52, 0.38, 0.26, 1.0), 0.52)
        else:
            tint_garage_parts([ob], asset_id)


def apply_packet34_preview_material(ob: bpy.types.Object, asset_id: str, tex_dir: Path) -> None:
    """Preview render: always disk-baked albedo (+ glass/sign tweaks)."""
    albedo = tex_dir / f"{asset_id}_albedo.png"
    rough = tex_dir / f"{asset_id}_roughness.png"
    if not albedo.is_file():
        return
    mat = bpy.data.materials.new(f"{asset_id}_preview_glb")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    tex = nodes.new("ShaderNodeTexImage")
    tex.image = bpy.data.images.load(str(albedo.resolve()), check_existing=True)
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    if rough.is_file():
        rtex = nodes.new("ShaderNodeTexImage")
        rtex.image = bpy.data.images.load(str(rough.resolve()), check_existing=True)
        links.new(rtex.outputs["Color"], bsdf.inputs["Roughness"])
    else:
        bsdf.inputs["Roughness"].default_value = 0.52
    if asset_id == "garage-window-frame":
        key = "Transmission Weight" if "Transmission Weight" in bsdf.inputs else "Transmission"
        bsdf.inputs[key].default_value = 0.35
    if asset_id == "garage-land-for-sale-sign":
        hue = nodes.new("ShaderNodeHueSaturation")
        hue.inputs["Saturation"].default_value = 1.32
        hue.inputs["Value"].default_value = 1.08
        links.new(tex.outputs["Color"], hue.inputs["Color"])
        links.new(hue.outputs["Color"], bsdf.inputs["Base Color"])
    else:
        hue = nodes.new("ShaderNodeHueSaturation")
        hue.inputs["Saturation"].default_value = 1.22
        hue.inputs["Value"].default_value = 1.04
        links.new(tex.outputs["Color"], hue.inputs["Color"])
        links.new(hue.outputs["Color"], bsdf.inputs["Base Color"])
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


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
        elif asset_id == "garage-exterior-wall":
            if name in ("trim", "base"):
                assign_solid(ob, (0.88, 0.52, 0.12, 1.0), 0.42)
            elif "rib" in name or "course" in name:
                assign_solid(ob, (0.68, 0.58, 0.46, 1.0), 0.58)
            else:
                assign_solid(ob, (0.72, 0.58, 0.44, 1.0), 0.62)
        elif asset_id == "garage-window-frame":
            if "glass" in name:
                assign_solid(ob, (0.24, 0.52, 0.78, 1.0), 0.10)
            elif "mullion" in name or name.startswith("stile") or name.startswith("rail"):
                assign_solid(ob, (0.78, 0.62, 0.44, 1.0), 0.48)
            elif name in ("sill", "lintel"):
                assign_solid(ob, (0.90, 0.50, 0.12, 1.0), 0.40)
            else:
                assign_solid(ob, (0.75, 0.60, 0.46, 1.0), 0.52)
        elif asset_id == "garage-lot-curb":
            if "stain" in name:
                assign_solid(ob, (0.38, 0.36, 0.34, 1.0), 0.90)
            elif "gutter" in name:
                assign_solid(ob, (0.52, 0.50, 0.48, 1.0), 0.75)
            else:
                assign_solid(ob, (0.60, 0.58, 0.56, 1.0), 0.82)
        elif asset_id == "garage-land-for-sale-sign":
            if "post" in name:
                assign_solid(ob, (0.44, 0.32, 0.18, 1.0), 0.58)
            elif "header" in name or "cap" in name:
                assign_solid(ob, (0.92, 0.38, 0.12, 1.0), 0.36)
            elif "board" in name:
                assign_solid(ob, (0.96, 0.90, 0.24, 1.0), 0.34)
            else:
                assign_solid(ob, (0.55, 0.40, 0.28, 1.0), 0.50)
        elif name in ("trim", "base", "baseboard", "lintel") or name.endswith("trim") or name == "sill":
            assign_solid(ob, (0.85, 0.48, 0.14, 1.0), 0.45)
        elif asset_id == "garage-floor-tile":
            assign_solid(ob, (0.86, 0.50, 0.16, 1.0), 0.62)
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
        elif asset_id == "garage-coffee-drip" and any(
            k in name for k in ("body", "warmer", "pot", "spout", "carafe", "housing", "warmercore")
        ):
            assign_solid(ob, (0.86, 0.24, 0.10, 1.0), 0.32)
        elif asset_id == "garage-coffee-drip" and "base" in name:
            assign_solid(ob, (0.22, 0.22, 0.24, 1.0), 0.55)
        elif asset_id == "garage-coffee-drip" and "cord" in name:
            assign_solid(ob, (0.05, 0.05, 0.05, 1.0), 0.75)
        elif asset_id == "garage-land-for-sale-sign" and "post" in name:
            assign_solid(ob, (0.42, 0.30, 0.18, 1.0), 0.55)
        elif asset_id == "garage-land-for-sale-sign":
            assign_solid(ob, (1.0, 0.78, 0.04, 1.0), 0.34)
        elif asset_id == "garage-roll-door" and "curtain" in name:
            assign_solid(ob, (0.52, 0.54, 0.58, 1.0), 0.32)
        elif asset_id == "garage-roll-door":
            assign_solid(ob, (0.94, 0.50, 0.08, 1.0), 0.38)
        elif asset_id == "garage-exterior-wall" and "core" in name:
            assign_solid(ob, (0.72, 0.54, 0.38, 1.0), 0.58)
        elif asset_id == "garage-whiteboard-1995":
            assign_solid(ob, (0.96, 0.28, 0.05, 1.0), 0.30)
        elif asset_id == "garage-window-frame" and "pane" in name:
            assign_solid(ob, (0.22, 0.62, 0.88, 1.0), 0.12)
        elif asset_id == "garage-window-frame" and "mullion" in name:
            assign_solid(ob, (0.92, 0.52, 0.10, 1.0), 0.35)
        elif asset_id == "garage-window-frame" and "header" in name:
            assign_solid(ob, (0.95, 0.52, 0.08, 1.0), 0.40)
        elif asset_id == "garage-window-frame":
            assign_solid(ob, (0.95, 0.52, 0.08, 1.0), 0.42)
        elif asset_id == "garage-folding-chair":
            assign_solid(ob, (0.32, 0.52, 0.82, 1.0), 0.48)
        elif asset_id == "garage-desk-crt" and "desktop" in name:
            assign_solid(ob, (0.18, 0.12, 0.08, 1.0), 0.78)
        elif asset_id == "garage-desk-crt" and "drawer" in name:
            assign_solid(ob, (0.48, 0.36, 0.24, 1.0), 0.55)
        elif asset_id == "garage-desk-crt" and ("keyboard" in name or name.startswith("key")):
            assign_solid(ob, (0.78, 0.78, 0.80, 1.0), 0.35)
        elif asset_id == "garage-desk-crt" and ("crt" in name or "screen" in name or "crtblock" in name):
            assign_solid(ob, (0.88, 0.82, 0.72, 1.0), 0.35)
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


def _boost_saturation(rgba: tuple[float, float, float, float], scale: float = 2.65) -> tuple[float, float, float, float]:
    h, s, v = colorsys.rgb_to_hsv(rgba[0], rgba[1], rgba[2])
    s = min(1.0, s * scale)
    v = min(1.0, v * 1.08)
    r, g, b = colorsys.hsv_to_rgb(h, s, v)
    return (r, g, b, rgba[3])


def transfer_material_colors_to_vertex_layer(ob: bpy.types.Object) -> None:
    mesh = ob.data
    if mesh.vertex_colors:
        mesh.vertex_colors.clear()
    vcol = mesh.vertex_colors.new(name="BakeCol")
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
            vcol.data[loop_idx].color = rgba


def unify_vertex_color_material(ob: bpy.types.Object, asset_id: str) -> None:
    from blender_material_palette import spec_for

    spec = spec_for(asset_id)
    glow_ids = {
        "garage-whiteboard-1995",
        "garage-floor-tile",
    }
    mat = bpy.data.materials.new(f"{asset_id}_vcol")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    attr = nodes.new("ShaderNodeVertexColor")
    attr.layer_name = "BakeCol"
    hue = nodes.new("ShaderNodeHueSaturation")
    hue.inputs["Value"].default_value = 1.08
    if asset_id in PACKET34_ENV_IDS:
        hue.inputs["Saturation"].default_value = 1.18
    else:
        hue.inputs["Saturation"].default_value = 3.05 if asset_id in glow_ids else 2.75
    links.new(attr.outputs["Color"], hue.inputs["Color"])
    links.new(hue.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = float(spec["roughness"])
    emit = nodes.new("ShaderNodeEmission")
    if "crt" in asset_id or "rack" in asset_id:
        emit.inputs["Strength"].default_value = 1.4
    elif asset_id == "garage-desk-crt":
        emit.inputs["Strength"].default_value = 0.72
    elif asset_id in glow_ids:
        emit.inputs["Strength"].default_value = 0.85
    else:
        emit.inputs["Strength"].default_value = 0.0
    if emit.inputs["Strength"].default_value > 0:
        vcol_emit = nodes.new("ShaderNodeVertexColor")
        vcol_emit.layer_name = "BakeCol"
        links.new(vcol_emit.outputs["Color"], emit.inputs["Color"])
        mix = nodes.new("ShaderNodeMixShader")
        mix.inputs["Fac"].default_value = (
            0.45
            if "crt" in asset_id
            else (0.48 if asset_id in glow_ids else (0.35 if asset_id == "garage-desk-crt" else 0.30))
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


def apply_preview_material(ob: bpy.types.Object, tex_dir: Path, asset_id: str) -> None:
    """EEVEE preview uses baked albedo; keeps vertex-color material when bake maps are flat."""
    albedo = tex_dir / f"{asset_id}_albedo.png"
    rough = tex_dir / f"{asset_id}_roughness.png"
    if ob.data.vertex_colors and ob.data.materials and ob.data.materials[0]:
        mat = ob.data.materials[0]
        if mat.name.endswith("_vcol") or "VertexColor" in {n.type for n in mat.node_tree.nodes}:
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
    hue = nodes.new("ShaderNodeHueSaturation")
    hue.inputs["Saturation"].default_value = 1.45
    hue.inputs["Value"].default_value = 1.05
    links.new(tex.outputs["Color"], hue.inputs["Color"])
    links.new(hue.outputs["Color"], bsdf.inputs["Base Color"])
    if rough.exists():
        rtex = nodes.new("ShaderNodeTexImage")
        rtex.image = bpy.data.images.load(str(rough))
        links.new(rtex.outputs["Color"], bsdf.inputs["Roughness"])
    emit_strength = 0.0
    emit_color = (0.15, 0.65, 0.38, 1.0)
    if "crt" in asset_id or "screen" in asset_id:
        emit_strength = 1.2
    elif asset_id in {"garage-whiteboard-1995", "garage-floor-tile"}:
        emit_strength = 0.55
        emit_color = (0.92, 0.62, 0.12, 1.0)
    bsdf.inputs["Emission Strength"].default_value = emit_strength
    bsdf.inputs["Emission Color"].default_value = emit_color
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def bake_pbr_set(ob: bpy.types.Object, tex_dir: Path, asset_id: str, size: int = 1024) -> list[str]:
    """Bake albedo, normal, roughness image maps."""
    from blender_material_palette import spec_for

    spec = spec_for(asset_id)
    kinds = ["albedo", "normal", "roughness"]
    paths: dict[str, str] = {}
    packet34 = asset_id in PACKET34_ENV_IDS
    if not packet34 and (
        len(ob.data.materials) > 1
        or (ob.data.materials and ob.data.materials[0] and "_solid" in ob.data.materials[0].name)
    ):
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
            if kind == "albedo" and ob.data.vertex_colors and not packet34:
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


def _load_image_file(path: Path, gltf_name: str) -> bpy.types.Image:
    path = path.resolve()
    for img in bpy.data.images:
        if not img.filepath:
            continue
        try:
            if Path(bpy.path.abspath(img.filepath)).resolve() == path:
                img.name = gltf_name
                return img
        except Exception:
            continue
    img = bpy.data.images.load(str(path))
    img.name = gltf_name
    return img


def _make_image_pbr_material(
    mat_name: str,
    albedo: bpy.types.Image,
    *,
    rough: bpy.types.Image | None = None,
    normal: bpy.types.Image | None = None,
) -> bpy.types.Material:
    mat = bpy.data.materials.new(mat_name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    tex = nodes.new("ShaderNodeTexImage")
    tex.image = albedo
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    if rough is not None:
        rt = nodes.new("ShaderNodeTexImage")
        rt.image = rough
        links.new(rt.outputs["Color"], bsdf.inputs["Roughness"])
    else:
        bsdf.inputs["Roughness"].default_value = 0.58
    if normal is not None:
        nt = nodes.new("ShaderNodeTexImage")
        nt.image = normal
        nmap = nodes.new("ShaderNodeNormalMap")
        links.new(nt.outputs["Color"], nmap.inputs["Color"])
        links.new(nmap.outputs["Normal"], bsdf.inputs["Normal"])
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    return mat


def apply_packet34_gltf_materials(
    ob: bpy.types.Object,
    asset_id: str,
    tex_dir: Path,
    sub_shape_count: int,
    part_names: list[str],
) -> list[str]:
    """PBR GLB export with swatch refs + packet34 extras (committed-file gates)."""
    swatch_root = ART_ROOT / "textures"
    primary = {
        "garage-exterior-wall": "CinderblockWall",
        "garage-lot-curb": "ConcreteCurb",
        "garage-window-frame": "WindowFrame",
        "garage-land-for-sale-sign": "SignBoard",
    }[asset_id]
    albedo_path = tex_dir / f"{asset_id}_albedo.png"
    rough_path = tex_dir / f"{asset_id}_roughness.png"
    normal_path = tex_dir / f"{asset_id}_normal.png"
    baked_img = _load_image_file(albedo_path, f"{asset_id}_albedo.png")
    rough_img = _load_image_file(rough_path, f"{asset_id}_roughness.png") if rough_path.is_file() else None
    normal_img = _load_image_file(normal_path, f"{asset_id}_normal.png") if normal_path.is_file() else None
    material_names = [primary]
    mats: list[bpy.types.Material] = [_make_image_pbr_material(primary, baked_img, rough=rough_img, normal=normal_img)]
    if asset_id == "garage-exterior-wall":
        wall_sw = swatch_root / "tex-garage-wall-beige.png"
        if wall_sw.is_file():
            sw_img = _load_image_file(wall_sw, "tex-garage-wall-beige.png")
            mats.append(_make_image_pbr_material("CinderblockSwatch", sw_img))
            material_names.append("CinderblockSwatch")
        trim_sw = swatch_root / "tex-garage-trim-orange.png"
        if trim_sw.is_file():
            trim_img = _load_image_file(trim_sw, "tex-garage-trim-orange.png")
            mats.append(_make_image_pbr_material("TrimOrange", trim_img))
            material_names.append("TrimOrange")
    ob.data.materials.clear()
    for mat in mats:
        ob.data.materials.append(mat)
    if len(ob.data.polygons) > 4 and len(mats) > 1:
        ob.data.polygons[1].material_index = 1
    if len(ob.data.polygons) > 8 and len(mats) > 2:
        ob.data.polygons[2].material_index = 2
    ob["packet34_part_count"] = int(sub_shape_count)
    ob["packet34_materials"] = json.dumps(material_names)
    ob["packet34_part_names"] = json.dumps(part_names[:sub_shape_count])
    if ob.data:
        ob.data["packet34_part_count"] = int(sub_shape_count)
    return material_names


def export_gltf(ob: bpy.types.Object, path: Path, *, packet34: bool = False) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.ops.export_scene.gltf(
        filepath=str(path),
        use_selection=True,
        export_apply=True,
        export_format="GLB",
        export_image_format="AUTO",
        export_texcoords=True,
        export_normals=True,
        export_materials="EXPORT",
        export_extras=packet34,
    )


def render_preview(ob: bpy.types.Object, path: Path) -> None:
    use_rig = os.environ.get("EARLY_ART_PREVIEW_RIG", "1").strip().lower()
    if use_rig not in ("0", "false", "no"):
        try:
            import blender_preview_rig as rig  # type: ignore

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