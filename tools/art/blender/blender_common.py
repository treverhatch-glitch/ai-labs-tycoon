"""
Shared helpers for headless Blender early-game asset scripts (import from repo; not run alone).
"""
from __future__ import annotations

import colorsys
import hashlib
import json
import math
import os
import subprocess
from pathlib import Path

import bpy
from mathutils import Matrix, Vector

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
CC0_ROOT = REPO_ROOT / "tools/art/cc0"
KENNEY_SUBURBAN_GLB = CC0_ROOT / "incoming/kenney-city-kit-suburban/Models/GLB format"

# tex-garage-wall-beige swatch: 512px tile, 128×64px blocks → 4×8 blocks per repeat.
WALL_BEIGE_TILE_BLOCKS_X = 4
WALL_BEIGE_TILE_BLOCKS_Y = 8
WALL_WORLD_W_M = 4.0
WALL_WORLD_H_M = 3.0
WALL_BLOCK_U_M = 0.4
WALL_BLOCK_V_M = 0.2


def wall_swatch_mapping_scale(
    *,
    wall_w: float = WALL_WORLD_W_M,
    wall_h: float = WALL_WORLD_H_M,
    block_u: float = WALL_BLOCK_U_M,
    block_v: float = WALL_BLOCK_V_M,
) -> tuple[float, float, float]:
    """Texture repeats so 4×3 m wall shows ~10×15 blocks (not 4×8 per repeat)."""
    sx = (wall_w / block_u) / WALL_BEIGE_TILE_BLOCKS_X
    sy = (wall_h / block_v) / WALL_BEIGE_TILE_BLOCKS_Y
    return (sx, sy, 1.0)


def import_cc0_kenney_glb(filename: str) -> bpy.types.Object:
    """Import one Kenney City Kit (Suburban) GLB from tools/art/cc0/incoming."""
    path = KENNEY_SUBURBAN_GLB / filename
    if not path.is_file():
        raise RuntimeError(f"CC0 Kenney missing: {path} (run tools/art/cc0/fetch_cc0_assets.sh)")
    mesh_before = {o.name for o in bpy.context.scene.objects if o.type == "MESH"}
    bpy.ops.import_scene.gltf(filepath=str(path))
    imported = [o for o in bpy.context.scene.objects if o.type == "MESH" and o.name not in mesh_before]
    if not imported:
        imported = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if len(imported) > 1:
        bpy.ops.object.select_all(action="DESELECT")
        for o in imported:
            o.select_set(True)
        bpy.context.view_layer.objects.active = imported[0]
        bpy.ops.object.join()
        return bpy.context.active_object
    return imported[0]


def wall_blocks_per_metre_h(
    *,
    mapping_scale_x: float,
    wall_w: float = WALL_WORLD_W_M,
) -> float:
    return mapping_scale_x * WALL_BEIGE_TILE_BLOCKS_X / wall_w


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


def _load_sign_bold_font() -> bpy.types.VectorFont | None:
    for path in (
        "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
        "/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf",
    ):
        try:
            return bpy.data.fonts.load(path)
        except RuntimeError:
            continue
    return None


SIGN_TEXT_RED_RGBA = (200 / 255.0, 16 / 255.0, 46 / 255.0, 1.0)  # #C8102E


def make_sign_text_red_material(*, preview: bool = False) -> bpy.types.Material:
    mat = bpy.data.materials.new("SignTextRed")
    mat.use_nodes = True
    mat.use_backface_culling = False
    if preview:
        nt = mat.node_tree
        nodes = nt.nodes
        links = nt.links
        nodes.clear()
        out = nodes.new("ShaderNodeOutputMaterial")
        emit = nodes.new("ShaderNodeEmission")
        emit.inputs["Color"].default_value = SIGN_TEXT_RED_RGBA
        emit.inputs["Strength"].default_value = 1.0
        links.new(emit.outputs["Emission"], out.inputs["Surface"])
        return mat
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = SIGN_TEXT_RED_RGBA
        bsdf.inputs["Roughness"].default_value = 0.45
        for em_key in ("Emission Color", "Emission"):
            if em_key in bsdf.inputs:
                bsdf.inputs[em_key].default_value = (0.0, 0.0, 0.0, 1.0)
        for str_key in ("Emission Strength", "Emission Energy"):
            if str_key in bsdf.inputs:
                bsdf.inputs[str_key].default_value = 0.0
    return mat


def assign_sign_text_red(ob: bpy.types.Object) -> None:
    mat = make_sign_text_red_material()
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def _sign_board_face_axes(board: bpy.types.Object, *, face_dir: float) -> tuple[Vector, Vector, Vector]:
    """Return (view_dir, u_left_to_right, v_up) for the public board face in world space."""
    thin_i = min(range(3), key=lambda i: board.dimensions[i])
    normal = Vector((0.0, 0.0, 0.0))
    normal[thin_i] = float(face_dir)
    normal = (board.matrix_world.to_3x3() @ normal).normalized()
    up = Vector((0.0, 1.0, 0.0))
    u = up.cross(normal)
    if u.length < 1e-5:
        u = Vector((1.0, 0.0, 0.0))
    u.normalize()
    v = normal.cross(u).normalized()
    return normal, u, v


def assert_sign_text_positive_det(letters: bpy.types.Object) -> None:
    det = letters.matrix_world.to_3x3().determinant()
    if det <= 1e-8:
        raise RuntimeError(f"sign letters world det={det:.6f} must be > 0 (no negative scale)")


def _center_sign_letters_on_board(board: bpy.types.Object, letters: bpy.types.Object) -> None:
    """Snap glyph bounds to board face bounds on the thin board plane."""
    bpy.context.view_layer.update()
    thin_i = min(range(3), key=lambda i: board.dimensions[i])
    corners_b = [board.matrix_world @ Vector(c) for c in board.bound_box]
    corners_l = [letters.matrix_world @ Vector(c) for c in letters.bound_box]
    if thin_i == 1:
        b0, b1 = min(v.z for v in corners_b), max(v.z for v in corners_b)
        l0, l1 = min(v.z for v in corners_l), max(v.z for v in corners_l)
        letters.location.z += (b0 + b1 - l0 - l1) * 0.5
        b0, b1 = min(v.x for v in corners_b), max(v.x for v in corners_b)
        l0, l1 = min(v.x for v in corners_l), max(v.x for v in corners_l)
        letters.location.x += (b0 + b1 - l0 - l1) * 0.5
    elif thin_i == 2:
        b0, b1 = min(v.y for v in corners_b), max(v.y for v in corners_b)
        l0, l1 = min(v.y for v in corners_l), max(v.y for v in corners_l)
        letters.location.y += (b0 + b1 - l0 - l1) * 0.5
    else:
        b0, b1 = min(v.z for v in corners_b), max(v.z for v in corners_b)
        l0, l1 = min(v.z for v in corners_l), max(v.z for v in corners_l)
        letters.location.z += (b0 + b1 - l0 - l1) * 0.5
    bpy.ops.object.select_all(action="DESELECT")
    letters.select_set(True)
    bpy.context.view_layer.objects.active = letters
    bpy.ops.object.transform_apply(location=True)


def ensure_sign_text_ltr(board: bpy.types.Object, letters: bpy.types.Object, *, face_dir: float = -1.0) -> None:
    """Rotate mesh text 180° on board normal when mirrored; keep det > 0."""
    bpy.context.view_layer.update()
    assert_sign_text_positive_det(letters)
    _view, u, _v = _sign_board_face_axes(board, face_dir=face_dir)
    verts = [letters.matrix_world @ Vector(v.co) for v in letters.data.vertices]
    if len(verts) < 12:
        return
    us = sorted(v.dot(u) for v in verts)
    u_min, u_max = us[0], us[-1]
    span = max(1e-6, u_max - u_min)
    left_mass = sum(1 for v in verts if v.dot(u) <= u_min + span * 0.2)
    right_mass = sum(1 for v in verts if v.dot(u) >= u_max - span * 0.2)
    if left_mass >= right_mass * 0.65:
        return
    pivot = board.matrix_world.translation.copy()
    view, _u, _v = _sign_board_face_axes(board, face_dir=face_dir)
    bpy.ops.object.select_all(action="DESELECT")
    letters.select_set(True)
    bpy.context.view_layer.objects.active = letters
    rot = Matrix.Rotation(math.pi, 4, view.normalized())
    letters.matrix_world = Matrix.Translation(pivot) @ rot @ Matrix.Translation(-pivot) @ letters.matrix_world
    bpy.ops.object.transform_apply(rotation=True, location=True)
    bpy.context.view_layer.update()
    assert_sign_text_positive_det(letters)


def add_sign_mesh_text(
    name: str,
    text: str,
    board: bpy.types.Object,
    *,
    face_dir: float = -1.0,
    extrude_m: float = 0.003,
) -> bpy.types.Object:
    """FOR SALE mesh on board face; positive scale only; LTR corrected before export."""
    bpy.ops.object.text_add(location=board.location)
    ob = bpy.context.active_object
    ob.name = name
    ob.data.body = text
    font = _load_sign_bold_font()
    if font:
        ob.data.font = font
    ob.data.extrude = extrude_m
    ob.data.resolution_u = 6
    ob.data.size = 0.28
    ob.data.space_character = 0.95
    ob.data.align_x = "CENTER"
    ob.data.align_y = "CENTER"
    thin_i = min(range(3), key=lambda i: board.dimensions[i])
    if thin_i == 1:
        ob.rotation_euler = (math.pi / 2, 0.0, 0.0)
        board_w = float(board.dimensions.x)
        board_h = float(board.dimensions.z)
    elif thin_i == 2:
        ob.rotation_euler = (0.0, 0.0, 0.0)
        board_w = float(board.dimensions.x)
        board_h = float(board.dimensions.y)
    else:
        ob.rotation_euler = (0.0, math.pi / 2, 0.0)
        board_w = float(board.dimensions.z)
        board_h = float(board.dimensions.y)
    bpy.ops.object.convert(target="MESH")
    bpy.context.view_layer.update()
    dx, dy = float(ob.dimensions.x), float(ob.dimensions.y)
    if dx > 1e-4 and dy > 1e-4:
        sx = abs((board_w * 0.492) / dx)
        sy = abs((board_h * 0.32) / dy)
        ob.scale = (sx, sy, 1.0)
        bpy.ops.object.transform_apply(scale=True)
    bpy.ops.object.transform_apply(rotation=True)
    bpy.context.view_layer.update()
    board_c = board.matrix_world @ (sum((Vector(c) for c in board.bound_box), Vector()) / 8.0)
    thin_h = float(min(board.dimensions)) * 0.5
    thin_i = min(range(3), key=lambda i: board.dimensions[i])
    if thin_i == 2:
        ob.location = board_c + Vector((0, 0, face_dir * (thin_h + extrude_m * 0.55)))
    elif thin_i == 1:
        ob.location = board_c + Vector((0, face_dir * (thin_h + extrude_m * 0.55), 0))
    else:
        ob.location = board_c + Vector((face_dir * (thin_h + extrude_m * 0.55), 0, 0))
    bpy.ops.object.transform_apply(location=True)
    assign_sign_text_red(ob)
    for _ in range(6):
        if mesh_island_count(ob) <= 1:
            break
        stitch_sign_mesh_islands(ob, max_bridge=0.06)
        weld_coincident_vertices(ob, threshold=0.004)
    _center_sign_letters_on_board(board, ob)
    ensure_sign_text_ltr(board, ob, face_dir=face_dir)
    _center_sign_letters_on_board(board, ob)
    assert_sign_text_positive_det(ob)
    return ob


def merge_sign_text_with_backing(text_ob: bpy.types.Object, board: bpy.types.Object) -> bpy.types.Object:
    """Weld glyphs to a shared hidden slab so the text mesh is one island."""
    import bmesh

    bpy.context.view_layer.update()
    corners = [board.matrix_world @ Vector(c) for c in board.bound_box]
    mins = Vector((min(v.x for v in corners), min(v.y for v in corners), min(v.z for v in corners)))
    maxs = Vector((max(v.x for v in corners), max(v.y for v in corners), max(v.z for v in corners)))
    t_corners = [text_ob.matrix_world @ Vector(c) for c in text_ob.bound_box]
    t_mins = Vector((min(v.x for v in t_corners), min(v.y for v in t_corners), min(v.z for v in t_corners)))
    t_maxs = Vector((max(v.x for v in t_corners), max(v.y for v in t_corners), max(v.z for v in t_corners)))
    center = (t_mins + t_maxs) * 0.5
    span = t_maxs - t_mins
    y_face = maxs.y - float(board.dimensions.y) * 0.52
    bpy.ops.mesh.primitive_cube_add(
        size=1,
        location=(center.x, (y_face + t_mins.y) * 0.5, center.z),
    )
    backing = bpy.context.active_object
    backing.name = "SignTextBacking"
    backing.scale = (
        max(0.12, span.x * 1.04),
        max(0.004, (t_mins.y - y_face) * 0.65),
        max(0.08, span.z * 1.06),
    )
    bpy.ops.object.transform_apply(scale=True)
    assign_sign_text_red(backing)
    bpy.ops.object.select_all(action="DESELECT")
    text_ob.select_set(True)
    backing.select_set(True)
    bpy.context.view_layer.objects.active = text_ob
    bpy.ops.object.join()
    root = bpy.context.view_layer.objects.active
    bpy.ops.object.transform_apply(location=True)
    weld_coincident_vertices(root, threshold=0.0035)
    bpy.context.view_layer.update()
    islands = mesh_island_count(root)
    if islands > 1:
        for _ in range(6):
            if mesh_island_count(root) <= 1:
                break
            stitch_sign_mesh_islands(root, max_bridge=0.08)
        weld_coincident_vertices(root, threshold=0.006)
    return root


def _write_sign_white_albedo(tex_dir: Path, asset_id: str) -> None:
    import subprocess

    albedo = tex_dir / f"{asset_id}_albedo.png"
    painter = REPO_ROOT / "tools/art/paint_sign_albedo.py"
    if painter.is_file():
        subprocess.run(["python3", str(painter), str(albedo)], check=True, cwd=str(REPO_ROOT))


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


def mesh_world_min_on_axis(ob: bpy.types.Object, axis: str = "y") -> float:
    idx = {"x": 0, "y": 1, "z": 2}[axis.lower()]
    return min((ob.matrix_world @ v.co)[idx] for v in ob.data.vertices)


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


def weld_mesh_merge_distance(ob: bpy.types.Object, threshold: float = 0.08) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=threshold)
    bpy.ops.object.mode_set(mode="OBJECT")


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


def add_sign_solid_core(objects: list[bpy.types.Object]) -> bpy.types.Object:
    """Hidden AABB core so boolean chains every glyph/post into one island (no voxel)."""
    mins, maxs = _bounds_world(objects)
    center = (mins + maxs) * 0.5
    span = maxs - mins
    bpy.ops.mesh.primitive_cube_add(size=1, location=center)
    core = bpy.context.active_object
    core.name = "SignSolidCore"
    core.scale = (
        max(0.04, span.x * 0.11),
        max(0.9, span.y * 0.92),
        max(0.06, span.z * 0.42),
    )
    bpy.ops.object.transform_apply(scale=True)
    assign_solid(core, (0.55, 0.40, 0.28, 1.0), 0.55)
    return core


def add_sign_weld_plate(objects: list[bpy.types.Object]) -> bpy.types.Object:
    """Wide hidden plate so posts/board/glyphs boolean to one island."""
    mins, maxs = _bounds_world(objects)
    center = (mins + maxs) * 0.5
    span = maxs - mins
    bpy.ops.mesh.primitive_cube_add(size=1, location=center)
    plate = bpy.context.active_object
    plate.name = "SignWeldPlate"
    plate.scale = (
        max(0.35, span.x * 0.98),
        max(0.06, span.y * 0.16),
        max(0.22, span.z * 0.98),
    )
    bpy.ops.object.transform_apply(scale=True)
    assign_solid(plate, (0.96, 0.94, 0.92, 1.0), 0.5)
    return plate


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


def _mesh_face_islands(bm: "bmesh.types.BMesh") -> list[list]:
    visited: set[int] = set()
    islands: list[list] = []
    bm.faces.ensure_lookup_table()
    for face in bm.faces:
        if face.index in visited:
            continue
        stack = [face]
        island = []
        while stack:
            f = stack.pop()
            if f.index in visited:
                continue
            visited.add(f.index)
            island.append(f)
            for e in f.edges:
                for linked in e.link_faces:
                    if linked.index not in visited:
                        stack.append(linked)
        islands.append(island)
    return islands


def build_sign_text_glyphs_mask(
    text_ob: bpy.types.Object,
    board_ob: bpy.types.Object,
    *,
    res: int = 384,
    dilate: int = 3,
) -> dict:
    """Rasterize text verts on the board XZ plane (authoring) for post-voxel red tagging."""
    bw = float(board_ob.dimensions.x)
    bh = float(board_ob.dimensions.z)
    cx = float(board_ob.location.x)
    cz = float(board_ob.location.z)
    cells: set[tuple[int, int]] = set()
    mw = text_ob.matrix_world
    corners = [board_ob.matrix_world @ Vector(c) for c in board_ob.bound_box]
    bx0 = min(v.x for v in corners)
    bz0 = min(v.z for v in corners)

    def _stamp(world_co: Vector) -> None:
        u = (world_co.x - bx0) / bw
        vv = (world_co.z - bz0) / bh
        if not (0.0 <= u <= 1.0 and 0.0 <= vv <= 1.0):
            return
        ix = int(max(0, min(res - 1, u * (res - 1))))
        iy = int(max(0, min(res - 1, vv * (res - 1))))
        cells.add((ix, iy))

    for v in text_ob.data.vertices:
        _stamp(mw @ v.co)
    if dilate > 0 and cells:
        grown: set[tuple[int, int]] = set(cells)
        for ix, iy in list(cells):
            for dx in range(-dilate, dilate + 1):
                for dy in range(-dilate, dilate + 1):
                    nx, ny = ix + dx, iy + dy
                    if 0 <= nx < res and 0 <= ny < res:
                        grown.add((nx, ny))
        cells = grown
    return {"res": res, "cells": cells}


def assign_sign_text_faces_red_from_mask(ob: bpy.types.Object, spec: dict) -> None:
    """Tag export-rotated faces whose board-plane projection hits the text raster."""
    import bmesh

    red = make_sign_text_red_material()
    red.name = "SignTextRed"
    board = None
    for slot in ob.data.materials:
        if slot and "SignTextRed" not in slot.name:
            board = slot
            break
    if board is None:
        board = bpy.data.materials.new("SignBoard")
    ob.data.materials.clear()
    ob.data.materials.append(board)
    ob.data.materials.append(red)
    cells: set[tuple[int, int]] = set(spec.get("cells") or ())
    if not cells:
        assign_sign_text_faces_red(ob, view_dir=(0.0, 0.0, 1.0))
        return
    res = int(spec.get("res") or 384)
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    bm.normal_update()
    xs = [v.co.x for v in bm.verts]
    ys = [v.co.y for v in bm.verts]
    x_lo, x_hi = min(xs), max(xs)
    y_lo, y_hi = min(ys), max(ys)
    x_pad = (x_hi - x_lo) * 0.11
    board_x0, board_x1 = x_lo + x_pad, x_hi - x_pad
    board_y0 = y_lo + (y_hi - y_lo) * 0.52
    board_y1 = y_hi - max(0.01, (y_hi - y_lo) * 0.02)
    span_x = max(1e-4, board_x1 - board_x0)
    span_y = max(1e-4, board_y1 - board_y0)
    cell_list = list(cells)
    tagged = 0
    z_lo = min(v.co.z for v in bm.verts)
    z_hi = max(v.co.z for v in bm.verts)
    z_thin = z_hi - z_lo
    z_front = z_hi - max(0.03, z_thin * 0.42)
    max_uv_dist = 0.048
    for face in bm.faces:
        c = face.calc_center_median()
        if not (board_y0 <= c.y <= board_y1 and board_x0 <= c.x <= board_x1):
            face.material_index = 0
            continue
        if c.z < z_front and face.normal.z < 0.12:
            face.material_index = 0
            continue
        u = (c.x - board_x0) / span_x
        v = (c.y - board_y0) / span_y
        if not (0.0 <= u <= 1.0 and 0.0 <= v <= 1.0):
            face.material_index = 0
            continue
        best = 1.0
        for cx_i, cy_i in cell_list:
            du = u - (cx_i / (res - 1))
            dv = v - (cy_i / (res - 1))
            best = min(best, math.hypot(du, dv))
            if best <= max_uv_dist:
                break
        hit = best <= max_uv_dist
        face.material_index = 1 if hit else 0
        if hit:
            tagged += 1
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()
    if tagged < 80:
        assign_sign_text_faces_red(ob, view_dir=(0.0, 0.0, 1.0))


def snapshot_sign_text_face_indices(ob: bpy.types.Object) -> set[int]:
    """Faces that carried SignTextRed before GLB material reset."""
    text_faces: set[int] = set()
    for i, poly in enumerate(ob.data.polygons):
        idx = int(poly.material_index)
        if idx <= 0 or idx >= len(ob.data.materials):
            continue
        mat = ob.data.materials[idx]
        name = (mat.name if mat else "") or ""
        if "SignTextRed" in name or name.startswith("SignText"):
            text_faces.add(i)
    return text_faces


def restore_sign_text_face_materials(
    ob: bpy.types.Object, text_faces: set[int], *, board_mat=None, red_mat=None
) -> None:
    """Re-apply SignBoard / SignTextRed slots after apply_packet34_gltf_materials reset."""
    if red_mat is None:
        red_mat = make_sign_text_red_material()
        red_mat.name = "SignTextRed"
    if board_mat is None:
        board_mat = ob.data.materials[0] if ob.data.materials else bpy.data.materials.new("SignBoard")
    ob.data.materials.clear()
    ob.data.materials.append(board_mat)
    ob.data.materials.append(red_mat)
    if text_faces:
        for i, poly in enumerate(ob.data.polygons):
            poly.material_index = 1 if i in text_faces else 0
        return
    assign_sign_text_faces_red(ob, view_dir=(0.0, 0.0, 1.0))


def assign_sign_text_faces_red(
    ob: bpy.types.Object,
    *,
    view_dir: tuple[float, float, float] = (0.0, 0.0, 1.0),
) -> None:
    """Tag FOR SALE faces on the public board face (glTF +Z front after export rotate)."""
    import bmesh

    view = Vector(view_dir).normalized()
    alt_view = -view
    red = make_sign_text_red_material()
    red.name = "SignTextRed"
    board = None
    for slot in ob.data.materials:
        if slot and "SignTextRed" not in slot.name:
            board = slot
            break
    if board is None:
        board = bpy.data.materials.new("SignBoard")
    ob.data.materials.clear()
    ob.data.materials.append(board)
    ob.data.materials.append(red)
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.faces.ensure_lookup_table()
    bm.normal_update()
    xs = [v.co.x for v in bm.verts]
    ys = [v.co.y for v in bm.verts]
    zs = [v.co.z for v in bm.verts]
    x_lo, x_hi = min(xs), max(xs)
    y_lo, y_hi = min(ys), max(ys)
    z_lo, z_hi = min(zs), max(zs)
    thin = min(x_hi - x_lo, y_hi - y_lo, z_hi - z_lo)
    thick_axis = 0
    if (y_hi - y_lo) == thin:
        thick_axis = 1
    elif (z_hi - z_lo) == thin:
        thick_axis = 2
    board_center = (
        (x_lo + x_hi) * 0.5,
        y_lo + (y_hi - y_lo) * 0.72,
        (z_lo + z_hi) * 0.5,
    )
    half_w = (x_hi - x_lo) * 0.44
    board_y0 = y_lo + (y_hi - y_lo) * 0.42
    z_front = z_hi - max(0.05, thin * 4.0)
    for face in bm.faces:
        c = face.calc_center_median()
        if thick_axis == 2:
            glyph = (
                face.normal.z > 0.28
                and c.z >= z_front
                and board_y0 <= c.y <= y_hi + 0.02
                and abs(c.x - board_center[0]) <= half_w
            )
        else:
            glyph = (
                face.normal.y < -0.35
                and c.y <= y_lo + max(0.08, thin * 6.0)
                and board_y0 <= c.y <= y_hi + 0.02
                and abs(c.x - board_center[0]) <= half_w
            )
        face.material_index = 1 if glyph else 0
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()


def prune_small_mesh_islands(ob: bpy.types.Object, *, min_faces: int = 18) -> int:
    """Drop stray boolean shards (keeps main sign body + glyphs)."""
    import bmesh

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    islands = _mesh_face_islands(bm)
    removed = 0
    if len(islands) <= 1:
        bm.free()
        return 0
    islands.sort(key=len, reverse=True)
    for island in islands[1:]:
        if len(island) >= min_faces:
            continue
        bmesh.ops.delete(bm, geom=island, context="FACES")
        removed += 1
    if removed:
        bm.to_mesh(ob.data)
        ob.data.update()
    bm.free()
    return removed


def stitch_sign_mesh_islands(ob: bpy.types.Object, *, max_bridge: float = 0.12) -> None:
    """Bridge stray glyph islands to the sign body with thin hidden faces (post-join bake)."""
    import bmesh

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.verts.ensure_lookup_table()
    islands = _mesh_face_islands(bm)
    if len(islands) <= 1:
        bm.free()
        return
    islands.sort(key=len, reverse=True)
    body_verts = {v for f in islands[0] for v in f.verts}
    for island in islands[1:]:
        verts_i = list({v for f in island for v in f.verts})
        best_dist = max_bridge * 2.0
        pair: tuple | None = None
        for v1 in verts_i:
            for v2 in body_verts:
                d = (v1.co - v2.co).length
                if d < best_dist:
                    best_dist = d
                    pair = (v1, v2)
        if pair is None or best_dist > max_bridge:
            continue
        v1, v2 = pair
        mid = (v1.co + v2.co) * 0.5
        a = bm.verts.new(mid + Vector((0.0015, 0, 0)))
        b = bm.verts.new(mid + Vector((-0.0015, 0, 0)))
        for face_verts in ((v1, a, b), (a, v2, b), (v1, v2, a)):
            try:
                bm.faces.new(face_verts)
            except ValueError:
                pass
        body_verts.update({v1, v2, a, b})
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()


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


def _snap_sign_letters_to_board(board: bpy.types.Object, letters: bpy.types.Object, *, face_dir: float = 1.0) -> None:
    bpy.context.view_layer.update()
    thin_i = min(range(3), key=lambda i: board.dimensions[i])
    bc = board.matrix_world.translation
    half_board = board.dimensions[thin_i] * 0.5
    half_letters = letters.dimensions[thin_i] * 0.5
    target = Vector(bc)
    target[thin_i] += face_dir * (half_board + half_letters + 0.0015)
    letters.matrix_world.translation = target


def _assemble_sign_named_parts(mesh_obs: list[bpy.types.Object]) -> tuple[bpy.types.Object, bpy.types.Object, bpy.types.Object]:
    board = next(o for o in mesh_obs if o.name == "board")
    letters = next(o for o in mesh_obs if o.name == "letters")
    post_bits = [o for o in mesh_obs if o not in (board, letters)]
    bpy.ops.object.select_all(action="DESELECT")
    for o in post_bits:
        o.select_set(True)
    bpy.context.view_layer.objects.active = post_bits[0]
    if len(post_bits) > 1:
        bpy.ops.object.join()
    posts = bpy.context.view_layer.objects.active
    posts.name = "posts"
    assign_solid(posts, (0.52, 0.38, 0.26, 1.0), 0.58)
    board.name = "board"
    letters.name = "letters"
    assign_sign_text_red(letters)
    weld_coincident_vertices(board, threshold=0.002)
    return posts, board, letters


def apply_sign_part_gltf_materials(
    posts: bpy.types.Object,
    board: bpy.types.Object,
    letters: bpy.types.Object,
    tex_dir: Path,
    part_names: list[str],
) -> None:
    albedo_path = tex_dir / "garage-land-for-sale-sign_albedo.png"
    rough_path = tex_dir / "garage-land-for-sale-sign_roughness.png"
    normal_path = tex_dir / "garage-land-for-sale-sign_normal.png"
    baked_img = _load_image_file(albedo_path, "garage-land-for-sale-sign_albedo.png")
    rough_img = _load_image_file(rough_path, "garage-land-for-sale-sign_roughness.png") if rough_path.is_file() else None
    normal_img = _load_image_file(normal_path, "garage-land-for-sale-sign_normal.png") if normal_path.is_file() else None
    board_mat = _make_image_pbr_material("SignBoard", baked_img, rough=rough_img, normal=normal_img)
    red_mat = make_sign_text_red_material()
    red_mat.name = "SignTextRed"
    post_mat = bpy.data.materials.new("SignPostWood")
    post_mat.use_nodes = True
    bsdf = post_mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = (0.52, 0.38, 0.26, 1.0)
        bsdf.inputs["Roughness"].default_value = 0.58
    for ob, mats in (
        (posts, [post_mat]),
        (board, [board_mat]),
        (letters, [red_mat]),
    ):
        ob.data.materials.clear()
        for m in mats:
            ob.data.materials.append(m)
        for poly in ob.data.polygons:
            poly.material_index = 0
    names = json.dumps(["posts", "board", "letters"])
    mats_json = json.dumps(["SignBoard", "SignPostWood", "SignTextRed"])
    for ob in (posts, board, letters):
        ob["packet34_part_count"] = 3
        ob["packet34_materials"] = mats_json
        ob["packet34_part_names"] = names
        if ob.data:
            ob.data["packet34_part_count"] = 3


def export_gltf_selection(objects: list[bpy.types.Object], path: Path, *, packet34: bool = False) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    for ob in objects:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = objects[0]
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


def finish_sign_named_asset(
    category: str,
    license_note: str,
    mesh_obs: list[bpy.types.Object],
    part_names: list[str],
    *,
    cc0_source: dict | None = None,
) -> dict:
    """Y-up sign GLB: named nodes posts / board / letters (no voxel merge)."""
    asset_id = "garage-land-for-sale-sign"
    out_dir = ART_ROOT / category
    tex_dir = out_dir / "textures" / asset_id
    out_dir.mkdir(parents=True, exist_ok=True)
    tex_dir.mkdir(parents=True, exist_ok=True)
    PREVIEW_ROOT.mkdir(parents=True, exist_ok=True)

    posts, board, letters = _assemble_sign_named_parts(list(mesh_obs))
    smooth_shade(board)
    uv_unwrap(board)
    maps = bake_pbr_set(board, tex_dir, asset_id, size=1024)
    _write_sign_white_albedo(tex_dir, asset_id)
    apply_sign_part_gltf_materials(posts, board, letters, tex_dir, part_names)
    bpy.context.view_layer.update()
    letters.parent = board
    min_z = mesh_world_min_on_axis(posts, "z")
    if abs(min_z) > 1e-5:
        for ob in (posts, board, letters):
            ob.location.z -= min_z
    bpy.context.view_layer.update()
    if letters.parent:
        bpy.ops.object.select_all(action="DESELECT")
        letters.select_set(True)
        bpy.context.view_layer.objects.active = letters
        bpy.ops.object.parent_clear(type="CLEAR_KEEP_TRANSFORM")
    _snap_sign_letters_to_board(board, letters, face_dir=1.0)
    ensure_sign_text_ltr(board, letters, face_dir=1.0)
    min_up = min(mesh_world_min_on_axis(o, "z") for o in (posts, board, letters))
    if abs(min_up) > 1e-5:
        for ob in (posts, board, letters):
            ob.location.z -= min_up
    bpy.context.view_layer.update()
    for ob in (posts, board, letters):
        bpy.ops.object.select_all(action="DESELECT")
        ob.select_set(True)
        bpy.context.view_layer.objects.active = ob
        bpy.ops.object.transform_apply(location=True, rotation=True, scale=True)
    bpy.ops.object.select_all(action="DESELECT")
    letters.select_set(True)
    bpy.context.view_layer.objects.active = letters
    bpy.ops.object.origin_set(type="ORIGIN_GEOMETRY", center="MEDIAN")
    _snap_sign_letters_to_board(board, letters, face_dir=1.0)
    _center_sign_letters_on_board(board, letters)
    ensure_sign_text_ltr(board, letters, face_dir=1.0)
    _center_sign_letters_on_board(board, letters)
    assert_sign_text_positive_det(letters)
    bpy.context.view_layer.update()
    glb_path = out_dir / f"{asset_id}.glb"
    export_gltf_selection([posts, board, letters], glb_path, packet34=True)
    preview_path = PREVIEW_ROOT / f"{asset_id}.png"
    try:
        render_preview([posts, board, letters], preview_path)
    except Exception as exc:  # noqa: BLE001
        print(f"WARN preview {asset_id}: {exc}")
    tris = sum(len(o.data.polygons) for o in (posts, board, letters))
    verts = sum(len(o.data.vertices) for o in (posts, board, letters))
    out = {
        "id": asset_id,
        "path": f"Assets/Art/EarlyGame/{category}/{asset_id}.glb",
        "textures": {
            k: f"Assets/Art/EarlyGame/{category}/textures/{asset_id}/{asset_id}_{k}.png" for k in maps
        },
        "preview": f"docs/art/previews/{asset_id}.png",
        "license": license_note,
        "vertexCount": verts,
        "triangleCount": tris,
        "subShapeCount": 3,
        "blenderVersion": BLENDER_VERSION,
        "generator": f"tools/art/blender/{asset_id}.py",
    }
    if cc0_source:
        out["cc0Source"] = cc0_source
    return out


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
    if asset_id == "garage-land-for-sale-sign":
        return finish_sign_named_asset(category, license_note, mesh_obs, part_names)
    sub_shape_count = len(mesh_obs)
    ground_scene_meshes()
    if asset_id == "garage-exterior-wall":
        tint_garage_parts(mesh_obs, asset_id)
    elif asset_id in PACKET34_ENV_IDS and asset_id != "garage-land-for-sale-sign":
        apply_packet34_authoring_textures(mesh_obs, asset_id)
    else:
        tint_garage_parts(mesh_obs, asset_id)
    parts = list(mesh_obs)
    if len(parts) > 1 and asset_id not in ("garage-land-for-sale-sign",):
        parts.append(add_connect_spine_part(parts))
    if len(parts) > 1:
        if asset_id == "garage-lot-curb":
            root = link_or_join(parts)
        elif asset_id == "garage-land-for-sale-sign":
            text_obs = [o for o in parts if "SignText" in o.name]
            board_obs = [o for o in parts if o.name == "Board"]
            sign_text_mask = (
                build_sign_text_glyphs_mask(text_obs[0], board_obs[0])
                if text_obs and board_obs
                else None
            )
            root = link_or_join(parts)
            sign_extent = max(root.dimensions) if max(root.dimensions) > 0 else 1.0
            weld_coincident_vertices(root, threshold=min(0.08, sign_extent * 0.03))
            for _ in range(4):
                if mesh_island_count(root) <= 1:
                    break
                stitch_sign_mesh_islands(root, max_bridge=0.14)
                weld_coincident_vertices(root, threshold=0.02)
            if mesh_island_count(root) > 1:
                plate = add_sign_weld_plate([root])
                root = union_overlapping_meshes([root, plate])
                weld_coincident_vertices(root, threshold=0.02)
            if mesh_island_count(root) > 1:
                voxel_shell_merge(root, voxel_size=min(0.009, sign_extent * 0.0038))
                weld_coincident_vertices(root, threshold=0.01)
            for _ in range(3):
                if mesh_island_count(root) <= 1:
                    break
                spine = add_connect_spine_part([root])
                root = union_overlapping_meshes([root, spine])
                weld_coincident_vertices(root, threshold=0.02)
            if mesh_island_count(root) > 1:
                plate = add_sign_weld_plate([root])
                root = union_overlapping_meshes([root, plate])
                weld_coincident_vertices(root, threshold=0.015)
            if mesh_island_count(root) > 1:
                voxel_shell_merge(root, voxel_size=min(0.011, sign_extent * 0.0045))
                weld_coincident_vertices(root, threshold=0.012)
            if sign_text_mask is not None:
                root["packet34_sign_text_mask"] = json.dumps(
                    {**sign_text_mask, "cells": list(sign_text_mask["cells"])}
                )
        else:
            root = union_overlapping_meshes(parts)
    else:
        root = mesh_obs[0]
    weld_extent = max(root.dimensions) if max(root.dimensions) > 0 else 1.0
    islands = mesh_island_count(root)
    if asset_id == "garage-lot-curb" and islands > 1:
        voxel_shell_merge(root, voxel_size=min(0.018, weld_extent * 0.008))
        islands = mesh_island_count(root)
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
    if asset_id == "garage-exterior-wall":
        apply_wall_post_union_textures(root, wall_w=4.0, wall_h=3.0, block_u=0.4, block_v=0.2)

    maps = bake_pbr_set(root, tex_dir, asset_id, size=1024)
    if asset_id == "garage-land-for-sale-sign":
        _write_sign_white_albedo(tex_dir, asset_id)
    if asset_id == "garage-exterior-wall":
        _boost_wall_albedo_mortar(tex_dir, asset_id)
    packet34 = asset_id in PACKET34_ENV_IDS
    if packet34:
        apply_packet34_gltf_materials(root, asset_id, tex_dir, sub_shape_count, part_names)
    else:
        apply_preview_material(root, tex_dir, asset_id)
    rotate_script_y_to_export_up(root)
    if asset_id == "garage-land-for-sale-sign":
        mask_raw = root.get("packet34_sign_text_mask")
        if mask_raw:
            try:
                spec = json.loads(str(mask_raw))
                spec["cells"] = {tuple(int(x) for x in pair) for pair in spec.get("cells", [])}
                assign_sign_text_faces_red_from_mask(root, spec)
            except (json.JSONDecodeError, TypeError, ValueError):
                assign_sign_text_faces_red(root, view_dir=(0.0, 0.0, 1.0))
        else:
            assign_sign_text_faces_red(root, view_dir=(0.0, 0.0, 1.0))
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
    if asset_id == "garage-land-for-sale-sign":
        mask_raw = ob.get("packet34_sign_text_mask")
        if mask_raw:
            try:
                spec = json.loads(str(mask_raw))
                spec["cells"] = {tuple(int(x) for x in pair) for pair in spec.get("cells", [])}
                assign_sign_text_faces_red_from_mask(ob, spec)
            except (json.JSONDecodeError, TypeError, ValueError):
                assign_sign_text_faces_red(ob, view_dir=(0.0, 0.0, 1.0))
        else:
            assign_sign_text_faces_red(ob, view_dir=(0.0, 0.0, 1.0))
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
    tex_dir = ART_ROOT / category / "textures" / asset_id
    if asset_id == "garage-land-for-sale-sign" and len(mesh_obs) >= 2:
        import blender_preview_rig as rig  # type: ignore

        apply_sign_preview_materials(mesh_obs, tex_dir)
        preview_path = PREVIEW_ROOT / f"{asset_id}.png"
        os.environ["PREVIEW_GLB_SHA256"] = glb_sha
        os.environ["PREVIEW_GLB_PATH"] = str(glb.relative_to(REPO_ROOT))
        os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
        post_obs = [o for o in mesh_obs if o.name == "posts"]

        os.environ["SIGN_PREVIEW_UP_AXIS"] = "Z"
        os.environ["PREVIEW_AZIMUTH_DEG"] = "40"
        os.environ["PREVIEW_ELEVATION_DEG"] = "18"
        os.environ["PREVIEW_FRAMING_MARGIN"] = "0.90"
        os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
        os.environ["EARLY_PREVIEW_VISIBLE_GROUND"] = "1"
        render_preview(mesh_obs, preview_path)

        assembled = PREVIEW_ROOT / "for-sale-sign.png"
        os.environ["SIGN_PREVIEW_UP_AXIS"] = "Z"
        os.environ["PREVIEW_AZIMUTH_DEG"] = "38"
        os.environ["PREVIEW_ELEVATION_DEG"] = "20"
        os.environ["PREVIEW_FRAMING_MARGIN"] = "0.78"
        os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
        os.environ["EARLY_PREVIEW_VISIBLE_GROUND"] = "1"
        render_preview(mesh_obs, assembled)

        front = PREVIEW_ROOT / "garage-land-for-sale-sign-front.png"
        reset_scene()
        bpy.ops.import_scene.gltf(filepath=str(glb))
        front_meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
        apply_sign_preview_materials(front_meshes, tex_dir)
        os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
        os.environ["SIGN_PREVIEW_UP_AXIS"] = "Z"
        os.environ["PREVIEW_AZIMUTH_DEG"] = "34"
        os.environ["PREVIEW_ELEVATION_DEG"] = "17"
        os.environ["PREVIEW_FRAMING_MARGIN"] = "0.84"
        os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
        os.environ["EARLY_PREVIEW_VISIBLE_GROUND"] = "1"
        render_preview(front_meshes, front)
        ob = mesh_obs[0]
    else:
        if len(mesh_obs) > 1:
            bpy.ops.object.select_all(action="DESELECT")
            for o in mesh_obs:
                o.select_set(True)
            bpy.context.view_layer.objects.active = mesh_obs[0]
            bpy.ops.object.join()
            mesh_obs = [bpy.context.active_object]
        ob = mesh_obs[0]
        if asset_id in PACKET34_ENV_IDS:
            apply_packet34_preview_material(ob, asset_id, tex_dir)
        else:
            apply_preview_material(ob, tex_dir, asset_id)
        preview_path = PREVIEW_ROOT / f"{asset_id}.png"
        os.environ["PREVIEW_GLB_SHA256"] = glb_sha
        os.environ["PREVIEW_GLB_PATH"] = str(glb.relative_to(REPO_ROOT))
        render_preview(ob, preview_path)
    meta_path = preview_path.with_name(preview_path.stem + ".frame.json")
    if meta_path.is_file():
        meta = json.loads(meta_path.read_text(encoding="utf-8"))
        if asset_id == "garage-exterior-wall":
            sx, sy, _ = wall_swatch_mapping_scale()
            meta["wall_uv_mapping_scale_x"] = sx
            meta["wall_uv_mapping_scale_y"] = sy
            meta["wall_swatch_blocks_per_tile_x"] = WALL_BEIGE_TILE_BLOCKS_X
            meta["wall_world_w_m"] = WALL_WORLD_W_M
            meta["wall_blocks_per_m_h"] = wall_blocks_per_metre_h(mapping_scale_x=sx)
            meta_path.write_text(json.dumps(meta, indent=2) + "\n", encoding="utf-8")


def _join_mesh_objects(meshes: list[bpy.types.Object]) -> bpy.types.Object:
    if len(meshes) == 1:
        return meshes[0]
    bpy.ops.object.select_all(action="DESELECT")
    for o in meshes:
        o.select_set(True)
    bpy.context.view_layer.objects.active = meshes[0]
    bpy.ops.object.join()
    return bpy.context.active_object


def _import_glb_meshes(path: Path) -> list[bpy.types.Object]:
    n_before = len([o for o in bpy.context.scene.objects if o.type == "MESH"])
    bpy.ops.import_scene.gltf(filepath=str(path))
    return [o for o in bpy.context.scene.objects if o.type == "MESH"][n_before:]


def render_sign_garage_hero(category: str = "garage-kit") -> None:
    """Hero: Kenney suburban facade + roll door + sign (facade ~50–70% of frame)."""
    import blender_preview_rig as rig  # type: ignore

    reset_scene()
    door_glb = (ART_ROOT / category / "garage-roll-door.glb").resolve()
    sign_glb = (ART_ROOT / category / "garage-land-for-sale-sign.glb").resolve()
    if not door_glb.is_file() or not sign_glb.is_file():
        raise RuntimeError("hero: missing roll door or sign GLB")
    facade = import_cc0_kenney_glb("building-type-b.glb")
    facade.name = "KenneyFacade"
    bpy.context.view_layer.update()
    dx, dy, dz = (float(facade.dimensions.x), float(facade.dimensions.y), float(facade.dimensions.z))
    facade.scale = (4.8 / max(dx, 1e-6), 2.85 / max(dy, 1e-6), 2.55 / max(dz, 1e-6))
    bpy.ops.object.select_all(action="DESELECT")
    facade.select_set(True)
    bpy.context.view_layer.objects.active = facade
    bpy.ops.object.transform_apply(scale=True)
    min_y = mesh_world_min_on_axis(facade, "y")
    facade.location.y -= min_y
    door_meshes = _import_glb_meshes(door_glb)
    door_ob = _join_mesh_objects(door_meshes)
    door_ob.name = "GarageRollDoor"
    sign_imported = _import_glb_meshes(sign_glb)
    sign_meshes = [o for o in sign_imported if o.name in {"posts", "board", "letters"}]
    if len(sign_meshes) < 3:
        raise RuntimeError("hero: sign parts missing after import")
    bpy.context.view_layer.update()
    fmin, fmax = rig.mesh_bounds_world([facade])
    fcenter = (fmin + fmax) * 0.5
    spans = {
        "x": fmax.x - fmin.x,
        "y": fmax.y - fmin.y,
        "z": fmax.z - fmin.z,
    }
    thin_axis = min(spans, key=spans.get)
    bpy.context.view_layer.update()
    ddx, ddy, ddz = (float(door_ob.dimensions.x), float(door_ob.dimensions.y), float(door_ob.dimensions.z))
    door_ob.scale = (
        2.55 / max(ddx, 1e-6),
        max(1.0, 2.35 / max(ddy, 1e-6)),
        2.35 / max(ddz, 1e-6),
    )
    bpy.ops.object.select_all(action="DESELECT")
    door_ob.select_set(True)
    bpy.context.view_layer.objects.active = door_ob
    bpy.ops.object.transform_apply(scale=True)
    bpy.context.view_layer.update()
    door_ob.location.x = fcenter.x
    door_ob.location.z = fcenter.z - 0.32
    if thin_axis == "y":
        inset = float(door_ob.dimensions.y) * 0.52
        door_ob.location.y = fmax.y - inset
    elif thin_axis == "z":
        inset = float(door_ob.dimensions.z) * 0.52
        door_ob.location.z = fmax.z - inset
        door_ob.location.y = fcenter.y - 0.32
    else:
        inset = float(door_ob.dimensions.x) * 0.52
        door_ob.location.x = fmax.x - inset
        door_ob.location.y = fcenter.y - 0.32
    bpy.context.view_layer.update()
    smin, smax = rig.mesh_bounds_world(sign_meshes)
    scenter = (smin + smax) * 0.5
    gap = 0.62
    for o in sign_meshes:
        o.location.x = fcenter.x + (o.location.x - scenter.x)
        o.location.z = fcenter.z - 0.22 + (o.location.z - scenter.z)
        if thin_axis == "y":
            o.location.y = fmax.y + gap
        elif thin_axis == "z":
            o.location.z = fmax.z + gap
        else:
            o.location.x = fmax.x + gap
    bpy.context.view_layer.update()
    assign_solid(facade, (0.72, 0.70, 0.66, 1.0), 0.52)
    apply_packet34_preview_material(door_ob, "garage-roll-door", ART_ROOT / category / "textures/garage-roll-door")
    apply_sign_preview_materials(sign_meshes, ART_ROOT / category / "textures/garage-land-for-sale-sign")
    hero = PREVIEW_ROOT / "garage-land-for-sale-sign-hero.png"
    os.environ.setdefault("EARLY_PREVIEW_MID_GREY", "1")
    os.environ.setdefault("EARLY_PREVIEW_CONTACT_SHADOW", "1")
    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.02"
    os.environ["PREVIEW_AZIMUTH_DEG"] = "36"
    os.environ["PREVIEW_ELEVATION_DEG"] = "10"
    os.environ["PREVIEW_GLB_SHA256"] = file_sha256(sign_glb)
    os.environ["PREVIEW_GLB_PATH"] = str(sign_glb.relative_to(REPO_ROOT))
    hero_meshes = sign_meshes + [facade, door_ob]
    render_preview(hero_meshes, hero)
    meta_path = hero.with_name(hero.stem + ".frame.json")
    if meta_path.is_file():
        meta = json.loads(meta_path.read_text(encoding="utf-8"))
        meta["hero_assets"] = [
            str(sign_glb.relative_to(REPO_ROOT)),
            str(door_glb.relative_to(REPO_ROOT)),
            "tools/art/cc0/incoming/kenney-city-kit-suburban/Models/GLB format/building-type-b.glb",
        ]
        meta["roll_door_preview"] = "docs/art/previews/garage-roll-door.png"
        meta["roll_door_glb_sha256"] = file_sha256(door_glb)
        meta_path.write_text(json.dumps(meta, indent=2) + "\n", encoding="utf-8")


def _billboard_face_axis(ob: bpy.types.Object) -> str:
    dx, dy, dz = (float(ob.dimensions.x), float(ob.dimensions.y), float(ob.dimensions.z))
    dims = (("X", dx), ("Y", dy), ("Z", dz))
    return min(dims, key=lambda t: t[1])[0]


def project_billboard_face_uvs(ob: bpy.types.Object, face_axis: str) -> None:
    """Cube-project UVs onto ±face_axis so one albedo covers the sign board face only."""
    import bmesh

    if not ob.data.uv_layers:
        ob.data.uv_layers.new(name="UVMap")
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    me = ob.data
    bm = bmesh.from_edit_mesh(me)
    bm.faces.ensure_lookup_table()
    axis_idx = {"X": 0, "Y": 1, "Z": 2}[face_axis.upper()]
    for face in bm.faces:
        face.select = abs(face.normal[axis_idx]) > 0.82
    bmesh.update_edit_mesh(me)
    bpy.ops.uv.cube_project(cube_size=2.0, scale_to_bounds=True)
    bpy.ops.object.mode_set(mode="OBJECT")


def assign_billboard_face_texture(
    ob: bpy.types.Object,
    image_path: Path,
    *,
    face_axis: str = "Y",
    white: tuple[float, float, float, float] = (0.98, 0.98, 0.96, 1.0),
    uv_scale: float | tuple[float, float, float] = 1.0,
    roughness: float = 0.42,
    flat_lit: bool = False,
    face_dir: float = 1.0,
    both_faces: bool = False,
) -> None:
    """Map albedo only on ±face_axis faces; solid white on edge faces (avoids UV bleed on cube)."""
    image_path = image_path.resolve()
    mat = bpy.data.materials.new(ob.name + "_billboard")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    geo = nodes.new("ShaderNodeNewGeometry")
    axis_idx = {"X": 0, "Y": 1, "Z": 2}[face_axis.upper()]
    sep = nodes.new("ShaderNodeSeparateXYZ")
    links.new(geo.outputs["Normal"], sep.inputs["Vector"])
    if both_faces:
        abs_ax = nodes.new("ShaderNodeMath")
        abs_ax.operation = "ABSOLUTE"
        links.new(sep.outputs[face_axis], abs_ax.inputs[0])
        is_face = nodes.new("ShaderNodeMath")
        is_face.operation = "GREATER_THAN"
        is_face.inputs[1].default_value = 0.82
        abs_out = "Value" if "Value" in abs_ax.outputs else 0
        links.new(abs_ax.outputs[abs_out], is_face.inputs[0])
    else:
        axis_vec = [0.0, 0.0, 0.0]
        axis_vec[axis_idx] = float(face_dir)
        facing = nodes.new("ShaderNodeVectorMath")
        facing.operation = "DOT_PRODUCT"
        facing.inputs[1].default_value = tuple(axis_vec)
        links.new(geo.outputs["Normal"], facing.inputs[0])
        is_face = nodes.new("ShaderNodeMath")
        is_face.operation = "GREATER_THAN"
        is_face.inputs[1].default_value = 0.82
        dot_out = "Value" if "Value" in facing.outputs else "Scalar"
        links.new(facing.outputs[dot_out], is_face.inputs[0])
    tex = nodes.new("ShaderNodeTexImage")
    tex.image = bpy.data.images.load(str(image_path), check_existing=True)
    tex.image.colorspace_settings.name = "sRGB"
    if isinstance(uv_scale, (int, float)):
        scale = (float(uv_scale), float(uv_scale), float(uv_scale))
    else:
        scale = uv_scale
    mapping = nodes.new("ShaderNodeMapping")
    mapping.inputs["Scale"].default_value = scale
    tco = nodes.new("ShaderNodeTexCoord")
    links.new(tco.outputs["UV"], mapping.inputs["Vector"])
    links.new(mapping.outputs["Vector"], tex.inputs["Vector"])
    mix = nodes.new("ShaderNodeMixRGB")
    mix.inputs["Color1"].default_value = white
    links.new(is_face.outputs[0], mix.inputs["Fac"])
    links.new(tex.outputs["Color"], mix.inputs["Color2"])
    links.new(mix.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = roughness
    if flat_lit:
        emit = nodes.new("ShaderNodeEmission")
        links.new(mix.outputs["Color"], emit.inputs["Color"])
        emit.inputs["Strength"].default_value = 1.0
        links.new(emit.outputs["Emission"], out.inputs["Surface"])
    else:
        links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def assign_image_texture(
    ob: bpy.types.Object,
    image_path: Path,
    *,
    uv_scale: float | tuple[float, float, float] = 1.0,
    coord: str = "UV",
    roughness: float = 0.55,
    metallic: float = 0.0,
    alpha: float = 1.0,
) -> None:
    """PBR material from a tile swatch (used before bake on packet34 env)."""
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
    tex.image.colorspace_settings.name = "sRGB"
    mapping = nodes.new("ShaderNodeMapping")
    if isinstance(uv_scale, (int, float)):
        scale = (float(uv_scale), float(uv_scale), float(uv_scale))
    else:
        scale = uv_scale
    mapping.inputs["Scale"].default_value = scale
    tco = nodes.new("ShaderNodeTexCoord")
    socket = tco.outputs["Generated"] if coord.upper() == "GENERATED" else tco.outputs["UV"]
    links.new(socket, mapping.inputs["Vector"])
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


def _make_generated_image_material(
    name: str,
    image_path: Path,
    scale: tuple[float, float, float],
    *,
    roughness: float = 0.55,
) -> bpy.types.Material:
    image_path = image_path.resolve()
    mat = bpy.data.materials.new(name)
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
    mapping.inputs["Scale"].default_value = scale
    tco = nodes.new("ShaderNodeTexCoord")
    links.new(tco.outputs["Generated"], mapping.inputs["Vector"])
    links.new(mapping.outputs["Vector"], tex.inputs["Vector"])
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = roughness
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    return mat


def _boost_curb_albedo(tex_dir: Path, asset_id: str) -> None:
    from PIL import Image

    path = tex_dir / f"{asset_id}_albedo.png"
    if not path.is_file():
        return
    im = Image.open(path).convert("RGB")
    px = [tuple(int(c * 0.68) for c in p) for p in im.getdata()]
    im.putdata(px)
    im.save(path)


def _boost_wall_albedo_mortar(tex_dir: Path, asset_id: str) -> None:
    """Push mortar lines in baked wall albedo for Hatch readability."""
    from PIL import Image, ImageEnhance

    path = tex_dir / f"{asset_id}_albedo.png"
    if not path.is_file():
        return
    im = Image.open(path).convert("RGB")
    im = ImageEnhance.Contrast(im).enhance(1.38)
    im = ImageEnhance.Color(im).enhance(1.22)
    im.save(path)


def apply_wall_post_union_textures(
    ob: bpy.types.Object,
    *,
    wall_w: float = 4.0,
    wall_h: float = 3.0,
    block_u: float = 0.4,
    block_v: float = 0.2,
    trim_band: float = 0.18,
    vertical_axis: str = "Y",
) -> None:
    """One bake-friendly cinderblock+trim shader using generated coords (post-union)."""
    swatch = ART_ROOT / "textures"
    wall = swatch / "tex-garage-wall-beige.png"
    trim = swatch / "tex-garage-trim-orange.png"
    if not wall.is_file() or not trim.is_file():
        return
    mat = bpy.data.materials.new("WallBakeCombined")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    coord = nodes.new("ShaderNodeTexCoord")
    sep = nodes.new("ShaderNodeSeparateXYZ")
    links.new(coord.outputs["Generated"], sep.inputs["Vector"])
    top = nodes.new("ShaderNodeMath")
    top.operation = "GREATER_THAN"
    top.inputs[1].default_value = 1.0 - (trim_band / wall_h)
    bot = nodes.new("ShaderNodeMath")
    bot.operation = "LESS_THAN"
    bot.inputs[1].default_value = trim_band / wall_h
    or_trim = nodes.new("ShaderNodeMath")
    or_trim.operation = "MAXIMUM"
    v_axis = sep.outputs[vertical_axis]
    links.new(v_axis, top.inputs[0])
    links.new(v_axis, bot.inputs[0])
    links.new(top.outputs[0], or_trim.inputs[0])
    links.new(bot.outputs[0], or_trim.inputs[1])

    flat_ax = "X"
    vert_ax = vertical_axis
    planar = nodes.new("ShaderNodeCombineXYZ")
    links.new(sep.outputs[flat_ax], planar.inputs["X"])
    links.new(sep.outputs[vert_ax], planar.inputs["Y"])

    def _tex(img_path: Path, scale: tuple[float, float, float]) -> bpy.types.Node:
        mapping = nodes.new("ShaderNodeMapping")
        mapping.inputs["Scale"].default_value = scale
        links.new(planar.outputs["Vector"], mapping.inputs["Vector"])
        tex = nodes.new("ShaderNodeTexImage")
        tex.image = bpy.data.images.load(str(img_path), check_existing=True)
        tex.extension = "REPEAT"
        links.new(mapping.outputs["Vector"], tex.inputs["Vector"])
        return tex

    tex_wall = _tex(wall, wall_swatch_mapping_scale(wall_w=wall_w, wall_h=wall_h, block_u=block_u, block_v=block_v))
    tex_trim = _tex(trim, (wall_w * 1.25, 3.5, 1.0))
    mix = nodes.new("ShaderNodeMixRGB")
    links.new(or_trim.outputs[0], mix.inputs["Fac"])
    links.new(tex_wall.outputs["Color"], mix.inputs[1])
    links.new(tex_trim.outputs["Color"], mix.inputs[2])
    bright = nodes.new("ShaderNodeBrightContrast")
    bright.inputs["Bright"].default_value = 1.02
    bright.inputs["Contrast"].default_value = 0.52
    hue = nodes.new("ShaderNodeHueSaturation")
    hue.inputs["Saturation"].default_value = 1.08
    hue.inputs["Value"].default_value = 1.02
    links.new(mix.outputs["Color"], bright.inputs["Color"])
    links.new(bright.outputs["Color"], hue.inputs["Color"])
    links.new(hue.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = 0.58
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
            block_u, block_v = 0.4, 0.2
            wall_w, wall_h = 4.0, 3.0
            if wall.is_file() and (
                name == "core"
                or (
                    name not in ("trim", "base")
                    and not name.startswith("course")
                    and "rib" not in name
                    and "connect" not in name
                    and name != "weld"
                )
            ):
                assign_image_texture(
                    ob,
                    wall,
                    uv_scale=wall_swatch_mapping_scale(
                        wall_w=wall_w, wall_h=wall_h, block_u=block_u, block_v=block_v
                    ),
                    coord="GENERATED",
                    roughness=0.62,
                )
            elif trim.is_file():
                assign_image_texture(
                    ob,
                    trim,
                    uv_scale=(wall_w * 1.2, 4.0, 1.0),
                    coord="GENERATED",
                    roughness=0.42,
                )
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


def _wall_vertical_axis(ob: bpy.types.Object) -> str:
    dx, dy, dz = (float(ob.dimensions.x), float(ob.dimensions.y), float(ob.dimensions.z))
    if dz >= dy and dz >= dx:
        return "Z"
    if dy >= dx:
        return "Y"
    return "Z"


def apply_wall_preview_swatch_material(ob: bpy.types.Object) -> None:
    """Matte cinderblock preview: same swatch + world Generated mapping as bake (~2.5 blocks/m)."""
    wall = ART_ROOT / "textures" / "tex-garage-wall-beige.png"
    if not wall.is_file():
        apply_wall_post_union_textures(ob, vertical_axis=_wall_vertical_axis(ob))
        return
    vertical_axis = _wall_vertical_axis(ob)
    flat_ax = "X" if vertical_axis != "X" else "Y"
    mat = bpy.data.materials.new("WallPreviewSwatch")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    coord = nodes.new("ShaderNodeTexCoord")
    sep = nodes.new("ShaderNodeSeparateXYZ")
    links.new(coord.outputs["Generated"], sep.inputs["Vector"])
    planar = nodes.new("ShaderNodeCombineXYZ")
    links.new(sep.outputs[flat_ax], planar.inputs["X"])
    links.new(sep.outputs[vertical_axis], planar.inputs["Y"])
    mapping = nodes.new("ShaderNodeMapping")
    mapping.inputs["Scale"].default_value = wall_swatch_mapping_scale()
    tex = nodes.new("ShaderNodeTexImage")
    tex.image = bpy.data.images.load(str(wall.resolve()), check_existing=True)
    tex.image.colorspace_settings.name = "sRGB"
    tex.extension = "REPEAT"
    links.new(planar.outputs["Vector"], mapping.inputs["Vector"])
    links.new(mapping.outputs["Vector"], tex.inputs["Vector"])
    bright = nodes.new("ShaderNodeBrightContrast")
    bright.inputs["Bright"].default_value = 0.0
    bright.inputs["Contrast"].default_value = 0.28
    links.new(tex.outputs["Color"], bright.inputs["Color"])
    links.new(bright.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = 0.62
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def apply_packet34_preview_material(ob: bpy.types.Object, asset_id: str, tex_dir: Path) -> None:
    """Preview render: baked albedo; wall uses committed swatch on UVs (matte)."""
    if asset_id == "garage-exterior-wall":
        apply_wall_preview_swatch_material(ob)
        return
    albedo = tex_dir / f"{asset_id}_albedo.png"
    if asset_id == "garage-land-for-sale-sign" and albedo.is_file():
        rough = tex_dir / f"{asset_id}_roughness.png"
        board_mat = _make_image_pbr_material(
            "SignBoard",
            bpy.data.images.load(str(albedo.resolve()), check_existing=True),
            rough=bpy.data.images.load(str(rough.resolve()), check_existing=True)
            if rough.is_file()
            else None,
        )
        red = make_sign_text_red_material(preview=True)
        saved_idx = [int(p.material_index) for p in ob.data.polygons]
        ob.data.materials.clear()
        ob.data.materials.append(board_mat)
        ob.data.materials.append(red)
        for i, poly in enumerate(ob.data.polygons):
            poly.material_index = min(saved_idx[i], 1)
        if not any(p.material_index == 1 for p in ob.data.polygons):
            mask_raw = ob.get("packet34_sign_text_mask")
            if mask_raw:
                try:
                    spec = json.loads(str(mask_raw))
                    spec["cells"] = {tuple(int(x) for x in pair) for pair in spec.get("cells", [])}
                    assign_sign_text_faces_red_from_mask(ob, spec)
                except (json.JSONDecodeError, TypeError, ValueError):
                    assign_sign_text_faces_red(ob, view_dir=(0.0, 0.0, 1.0))
            else:
                assign_sign_text_faces_red(ob, view_dir=(0.0, 0.0, 1.0))
        return
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
    tco = nodes.new("ShaderNodeTexCoord")
    tex = nodes.new("ShaderNodeTexImage")
    tex.image = bpy.data.images.load(str(albedo.resolve()), check_existing=True)
    tex.image.colorspace_settings.name = "sRGB"
    links.new(tco.outputs["UV"], tex.inputs["Vector"])
    curb_matte = asset_id == "garage-lot-curb"
    if rough.is_file() and not curb_matte:
        rtex = nodes.new("ShaderNodeTexImage")
        rtex.image = bpy.data.images.load(str(rough.resolve()), check_existing=True)
        links.new(tco.outputs["UV"], rtex.inputs["Vector"])
        links.new(rtex.outputs["Color"], bsdf.inputs["Roughness"])
    else:
        bsdf.inputs["Roughness"].default_value = 0.90 if curb_matte else 0.52
    if asset_id == "garage-window-frame":
        key = "Transmission Weight" if "Transmission Weight" in bsdf.inputs else "Transmission"
        bsdf.inputs[key].default_value = 0.55
        ior_key = "IOR" if "IOR" in bsdf.inputs else None
        if ior_key:
            bsdf.inputs[ior_key].default_value = 1.42
        if "Roughness" in bsdf.inputs:
            bsdf.inputs["Roughness"].default_value = 0.18
    if asset_id == "garage-lot-curb":
        curb_tone = nodes.new("ShaderNodeBrightContrast")
        curb_tone.inputs["Bright"].default_value = -0.38
        curb_tone.inputs["Contrast"].default_value = 0.22
        links.new(tex.outputs["Color"], curb_tone.inputs["Color"])
        links.new(curb_tone.outputs["Color"], bsdf.inputs["Base Color"])
    else:
        links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
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
            if "board" in name:
                continue
            if "post" in name:
                assign_solid(ob, (0.44, 0.32, 0.18, 1.0), 0.58)
            elif "header" in name or "cap" in name:
                assign_solid(ob, (0.92, 0.38, 0.12, 1.0), 0.36)
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

    bpy.context.scene.render.engine = "CYCLES"
    if bpy.context.preferences.addons.get("cycles") is None:
        bpy.ops.preferences.addon_enable(module="cycles")
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
            elif kind == "albedo" and packet34:
                bpy.context.scene.render.bake.use_pass_direct = False
                bpy.context.scene.render.bake.use_pass_indirect = False
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
                        base_in = bsdf.inputs["Base Color"]
                        if base_in.links:
                            nt.links.new(base_in.links[0].from_socket, emit.inputs["Color"])
                        else:
                            emit.inputs["Color"].default_value = base_in.default_value
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
    if asset_id == "garage-land-for-sale-sign":
        red = make_sign_text_red_material()
        red.name = "SignTextRed"
        mats.append(red)
        material_names.append("SignTextRed")
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
    if asset_id == "garage-window-frame":
        for poly in ob.data.polygons:
            poly.material_index = 0
    elif asset_id != "garage-land-for-sale-sign":
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


def apply_sign_preview_materials(mesh_obs: list[bpy.types.Object], tex_dir: Path) -> None:
    albedo = tex_dir / "garage-land-for-sale-sign_albedo.png"
    rough = tex_dir / "garage-land-for-sale-sign_roughness.png"
    board_mat = None
    if albedo.is_file():
        board_mat = _make_image_pbr_material(
            "SignBoard",
            bpy.data.images.load(str(albedo.resolve()), check_existing=True),
            rough=bpy.data.images.load(str(rough.resolve()), check_existing=True)
            if rough.is_file()
            else None,
        )
    red = make_sign_text_red_material(preview=False)
    red.name = "SignTextRedPreview"
    post_mat = bpy.data.materials.new("SignPostWoodPreview")
    post_mat.use_nodes = True
    bsdf = post_mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = (0.52, 0.38, 0.26, 1.0)
        bsdf.inputs["Roughness"].default_value = 0.58
    for ob in mesh_obs:
        if ob.name == "letters":
            ob.data.materials.clear()
            ob.data.materials.append(red)
        elif ob.name == "board":
            ob.data.materials.clear()
            flat = bpy.data.materials.new("SignBoardFlatPreview")
            flat.use_nodes = True
            nt = flat.node_tree
            nodes = nt.nodes
            links = nt.links
            nodes.clear()
            out = nodes.new("ShaderNodeOutputMaterial")
            emit = nodes.new("ShaderNodeEmission")
            flat_white = (0.98, 0.96, 0.94, 1.0)
            emit.inputs["Color"].default_value = flat_white
            emit.inputs["Strength"].default_value = 0.95
            links.new(emit.outputs["Emission"], out.inputs["Surface"])
            ob.data.materials.append(flat)
        elif ob.name == "posts":
            ob.data.materials.clear()
            ob.data.materials.append(post_mat)


def render_preview(ob: bpy.types.Object | list[bpy.types.Object], path: Path) -> None:
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