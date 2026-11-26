"""
CC0 import → restyle → bake → export helpers (headless Blender).
"""
from __future__ import annotations

import json
import math
from pathlib import Path

import bpy
from mathutils import Vector

REPO_ROOT = Path(__file__).resolve().parents[3]
INCOMING = REPO_ROOT / "tools/art/cc0/incoming"
ART_ROOT = REPO_ROOT / "unity/Assets/Art/EarlyGame"
PREVIEW_ROOT = REPO_ROOT / "docs/art/previews"
HDRI_PATH = INCOMING / "polyhaven" / "studio_small_08_1k.hdr"
COLORMAP = INCOMING / "kenney_building-kit/Models/GLB format/Textures/colormap.png"
BLENDER_VERSION = f"{bpy.app.version[0]}.{bpy.app.version[1]}.{bpy.app.version[2]}"

PALETTE = {
    "floor": (0.50, 0.48, 0.45, 1.0),
    "wall": (0.78, 0.72, 0.62, 1.0),
    "trim": (0.88, 0.48, 0.18, 1.0),
    "metal": (0.55, 0.58, 0.62, 1.0),
    "glass": (0.55, 0.75, 0.88, 1.0),
    "prop": (0.65, 0.62, 0.58, 1.0),
    "board": (0.92, 0.90, 0.86, 1.0),
}

BACKDROP_GREY = 150.0 / 255.0


def reset_scene() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.context.scene.render.engine = "CYCLES"
    bpy.context.scene.cycles.device = "CPU"
    bpy.context.scene.cycles.samples = 64
    bpy.context.scene.unit_settings.system = "METRIC"
    bpy.context.scene.unit_settings.scale_length = 1.0


def kenney_glb(name: str) -> Path:
    return INCOMING / "kenney_building-kit/Models/GLB format" / name


def import_gltf(path: Path, name: str) -> list[bpy.types.Object]:
    before = set(bpy.context.scene.objects)
    bpy.ops.import_scene.gltf(filepath=str(path))
    imported = [o for o in bpy.context.scene.objects if o not in before and o.type == "MESH"]
    for ob in imported:
        ob.name = name if len(imported) == 1 else f"{name}_{ob.name}"
    return imported


def merge_mesh_objects(objects: list[bpy.types.Object], name: str) -> bpy.types.Object:
    if not objects:
        raise RuntimeError(f"no meshes for {name}")
    bpy.ops.object.select_all(action="DESELECT")
    for ob in objects:
        ob.select_set(True)
    bpy.context.view_layer.objects.active = objects[0]
    if len(objects) > 1:
        bpy.ops.object.join()
    root = bpy.context.view_layer.objects.active
    root.name = name
    return root


def bevel_object(ob: bpy.types.Object, width: float = 0.018, segments: int = 2) -> None:
    mod = ob.modifiers.new("Bevel", type="BEVEL")
    mod.width = width
    mod.segments = segments
    mod.limit_method = "ANGLE"
    mod.angle_limit = math.radians(50)
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.modifier_apply(modifier=mod.name)


def load_image(path: Path) -> bpy.types.Image | None:
    if not path.is_file():
        return None
    return bpy.data.images.load(str(path), check_existing=True)


def apply_kenney_material(ob: bpy.types.Object, role: str) -> None:
    """Kenney colormap + role tint so GLB exports with bound albedo (no magenta)."""
    tint = PALETTE.get(role, PALETTE["prop"])
    mat = bpy.data.materials.new(name=f"m_{ob.name}_{role}")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    tex = nodes.new("ShaderNodeTexImage")
    tex.image = load_image(COLORMAP)
    mix = nodes.new("ShaderNodeMixRGB")
    mix.blend_type = "MULTIPLY"
    mix.inputs["Fac"].default_value = 0.65
    mix.inputs["Color2"].default_value = tint
    links.new(tex.outputs["Color"], mix.inputs["Color1"])
    links.new(mix.outputs["Color"], bsdf.inputs["Base Color"])
    bsdf.inputs["Roughness"].default_value = 0.58 if role != "glass" else 0.12
    if role == "glass":
        if "Transmission Weight" in bsdf.inputs:
            bsdf.inputs["Transmission Weight"].default_value = 0.25
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    ob.data.materials.clear()
    ob.data.materials.append(mat)


def bake_object_maps(ob: bpy.types.Object, tex_dir: Path, asset_id: str, size: int = 1024) -> dict[str, str]:
    tex_dir.mkdir(parents=True, exist_ok=True)
    kinds = ["albedo", "ao", "normal"]
    out_paths: dict[str, str] = {}
    for kind in kinds:
        img = bpy.data.images.new(f"{asset_id}_{kind}", width=size, height=size)
        img.file_format = "PNG"
        path = tex_dir / f"{asset_id}_{kind}.png"
        img.filepath_raw = str(path)
        mat = ob.data.materials[0] if ob.data.materials else None
        if mat and mat.use_nodes:
            bake_node = mat.node_tree.nodes.new("ShaderNodeTexImage")
            bake_node.image = img
            mat.node_tree.nodes.active = bake_node
        bpy.ops.object.select_all(action="DESELECT")
        ob.select_set(True)
        bpy.context.view_layer.objects.active = ob
        bpy.context.scene.render.bake.margin = 12
        if kind == "albedo":
            bpy.ops.object.bake(type="DIFFUSE", pass_filter={"COLOR"})
        elif kind == "ao":
            bpy.ops.object.bake(type="AO")
        else:
            bpy.ops.object.bake(type="NORMAL")
        img.save()
        out_paths[kind] = str(path)
    return out_paths


def export_gltf_collection(objects: list[bpy.types.Object], path: Path) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    for ob in objects:
        ob.select_set(True)
    bpy.ops.export_scene.gltf(
        filepath=str(path),
        use_selection=True,
        export_apply=True,
        export_format="GLB",
        export_yup=True,
        export_image_format="AUTO",
    )


def setup_hdri_world(strength: float = 1.35) -> None:
    world = bpy.data.worlds.new("cc0_preview_world")
    bpy.context.scene.world = world
    world.use_nodes = True
    nodes = world.node_tree.nodes
    links = world.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputWorld")
    bg = nodes.new("ShaderNodeBackground")
    if HDRI_PATH.is_file():
        env = nodes.new("ShaderNodeTexEnvironment")
        env.image = load_image(HDRI_PATH)
        mapping = nodes.new("ShaderNodeMapping")
        texcoord = nodes.new("ShaderNodeTexCoord")
        mapping.inputs["Rotation"].default_value[2] = math.radians(215)
        links.new(texcoord.outputs["Generated"], mapping.inputs["Vector"])
        links.new(mapping.outputs["Vector"], env.inputs["Vector"])
        links.new(env.outputs["Color"], bg.inputs["Color"])
        bg.inputs["Strength"].default_value = strength
    else:
        bg.inputs["Color"].default_value = (0.8, 0.8, 0.82, 1)
        bg.inputs["Strength"].default_value = 1.0
    links.new(bg.outputs["Background"], out.inputs["Surface"])


def _bounds_center(objects: list[bpy.types.Object]) -> tuple[Vector, float]:
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in objects:
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins.x = min(mins.x, w.x)
            mins.y = min(mins.y, w.y)
            mins.z = min(mins.z, w.z)
            maxs.x = max(maxs.x, w.x)
            maxs.y = max(maxs.y, w.y)
            maxs.z = max(maxs.z, w.z)
    center = (mins + maxs) * 0.5
    span = max(maxs.x - mins.x, maxs.y - mins.y, maxs.z - mins.z, 0.1)
    return center, span


def render_kit_preview(objects: list[bpy.types.Object], path: Path, resolution: int = 768) -> None:
    """HDRI-only light, grey ~150 backdrop, cut-away corner framing ~70% fill."""
    PREVIEW_ROOT.mkdir(parents=True, exist_ok=True)
    setup_hdri_world(strength=1.35)
    bpy.context.scene.render.engine = "CYCLES"
    bpy.context.scene.cycles.samples = min(96, 128)
    bpy.context.scene.cycles.use_denoising = False
    if hasattr(bpy.context.scene, "cycles"):
        bpy.context.scene.cycles.film_exposure = 1.05
    if hasattr(bpy.context.scene.view_settings, "exposure"):
        bpy.context.scene.view_settings.exposure = 0.0

    center, span = _bounds_center(objects)

    # Mid-grey ground + backdrop (not emissive; HDRI provides light)
    bpy.ops.mesh.primitive_plane_add(size=span * 2.8, location=(center.x, center.y, 0.0))
    ground = bpy.context.active_object
    ground.name = "preview_ground"
    gmat = bpy.data.materials.new("ground_grey150")
    gmat.use_nodes = True
    gbsdf = gmat.node_tree.nodes.get("Principled BSDF")
    gbsdf.inputs["Base Color"].default_value = (BACKDROP_GREY, BACKDROP_GREY, BACKDROP_GREY, 1)
    gbsdf.inputs["Roughness"].default_value = 0.88
    ground.data.materials.append(gmat)

    bpy.ops.mesh.primitive_plane_add(
        size=span * 3.2,
        location=(center.x + span, center.y + span, center.z),
        rotation=(math.radians(90), 0, math.radians(45)),
    )
    backdrop = bpy.context.active_object
    backdrop.name = "preview_backdrop"
    bmat = bpy.data.materials.new("backdrop_grey150")
    bmat.use_nodes = True
    bbsdf = bmat.node_tree.nodes.get("Principled BSDF")
    bbsdf.inputs["Base Color"].default_value = (BACKDROP_GREY, BACKDROP_GREY, BACKDROP_GREY, 1)
    bbsdf.inputs["Roughness"].default_value = 0.95
    backdrop.data.materials.append(bmat)

    # Camera outside open corner (+X, +Y, +Z) — not inside closed mesh
    dist = span * 2.15
    cam_loc = Vector((center.x + dist * 0.72, center.y + dist * 0.72, center.z + dist * 0.42))
    target = Vector((center.x, center.y, center.z + span * 0.25))
    bpy.ops.object.camera_add(location=cam_loc)
    cam = bpy.context.active_object
    direction = target - cam.location
    cam.data.lens = 40
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    bpy.context.scene.camera = cam

    bpy.context.scene.render.resolution_x = resolution
    bpy.context.scene.render.resolution_y = resolution
    bpy.context.scene.render.image_settings.file_format = "PNG"
    bpy.context.scene.render.filepath = str(path)
    bpy.ops.render.render(write_still=True)

    bpy.data.objects.remove(ground, do_unlink=True)
    bpy.data.objects.remove(backdrop, do_unlink=True)
    bpy.data.objects.remove(cam, do_unlink=True)


def write_cc0_manifest(entry: dict, manifest_name: str = "garage-cc0-manifest.json") -> None:
    manifest_path = ART_ROOT / manifest_name
    payload: dict = {"fidelity": "cc0-blender-baked", "blenderVersion": BLENDER_VERSION, "assets": []}
    if manifest_path.exists():
        payload = json.loads(manifest_path.read_text(encoding="utf-8"))
    assets = [a for a in payload.get("assets", []) if a["id"] != entry["id"]]
    assets.append(entry)
    assets.sort(key=lambda a: a["id"])
    payload["assets"] = assets
    manifest_path.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")