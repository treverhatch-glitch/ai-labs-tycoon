"""
Shared headless Blender 4.x bake for campus-kit OBJ → PBR + glTF/FBX + preview.
Run only inside Blender: blender --background --python tools/art/blender/<asset>.py
License: CC0 procedural pipeline (repo-owned meshes).
"""
from __future__ import annotations

import json
import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
OBJ_IN = REPO / "unity" / "Assets" / "Art" / "Generated" / "campus-kit"
OUT_ROOT = REPO / "unity" / "Assets" / "Art" / "BlenderBaked"
PREVIEW_DIR = REPO / "docs" / "art" / "previews"
TEX_SIZE = 1024
SCOPE = "presentation"  # outside EarlyGame/


def _reset_scene() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)


def _import_obj(obj_path: Path) -> bpy.types.Object:
    bpy.ops.wm.obj_import(filepath=str(obj_path))
    objs = [o for o in bpy.context.selected_objects if o.type == "MESH"]
    if not objs:
        raise RuntimeError(f"No mesh after OBJ import: {obj_path}")
    root = objs[0]
    if len(objs) > 1:
        bpy.context.view_layer.objects.active = root
        for o in objs[1:]:
            o.select_set(True)
        bpy.ops.object.join()
    return root


def _ensure_material(obj: bpy.types.Object, asset_id: str) -> bpy.types.Material:
    mat_name = f"M_{asset_id}"
    mat = bpy.data.materials.get(mat_name) or bpy.data.materials.new(mat_name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Base Color"].default_value = (0.72, 0.74, 0.78, 1.0)
    bsdf.inputs["Roughness"].default_value = 0.55
    bsdf.inputs["Metallic"].default_value = 0.08
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    if obj.data.materials:
        obj.data.materials[0] = mat
    else:
        obj.data.materials.append(mat)
    return mat


def _add_bake_nodes(mat: bpy.types.Material) -> tuple[bpy.types.ShaderNodeTexImage, bpy.types.Image]:
    img_name = f"T_{mat.name}"
    if img_name in bpy.data.images:
        bpy.data.images.remove(bpy.data.images[img_name])
    image = bpy.data.images.new(img_name, TEX_SIZE, TEX_SIZE, alpha=False)
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    tex = nodes.new("ShaderNodeTexImage")
    tex.image = image
    tex.select = True
    nodes.active = tex
    bsdf = next(n for n in nodes if n.type == "BSDF_PRINCIPLED")
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    return tex, image


def _smart_uv(obj: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.uv.smart_project(angle_limit=math.radians(66), island_margin=0.02)
    bpy.ops.object.mode_set(mode="OBJECT")


def _bake_diffuse(obj: bpy.types.Object, mat: bpy.types.Material) -> Path:
    _add_bake_nodes(mat)
    bpy.ops.object.select_all(action="DESELECT")
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    bpy.context.scene.render.engine = "CYCLES"
    bpy.context.scene.cycles.device = "CPU"
    bpy.context.scene.cycles.samples = 16
    bpy.ops.object.bake(type="DIFFUSE", pass_filter={"COLOR"}, margin=4, use_clear=True)
    out_dir = OUT_ROOT / obj.name.replace("mesh_", "")
    tex_dir = out_dir / "textures"
    tex_dir.mkdir(parents=True, exist_ok=True)
    tex_path = tex_dir / f"{obj.name}_basecolor.png"
    for node in mat.node_tree.nodes:
        if node.type == "TEX_IMAGE" and node.image:
            node.image.filepath_raw = str(tex_path)
            node.image.file_format = "PNG"
            node.image.save()
            return tex_path
    raise RuntimeError("Bake did not produce image")


def _export_gltf(obj: bpy.types.Object, out_dir: Path) -> Path:
    glb = out_dir / f"{obj.name}.glb"
    bpy.ops.object.select_all(action="DESELECT")
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    bpy.ops.export_scene.gltf(
        filepath=str(glb),
        export_format="GLB",
        use_selection=True,
        export_texcoords=True,
        export_materials="EXPORT",
        export_image_format="AUTO",
    )
    return glb


def _export_fbx(obj: bpy.types.Object, out_dir: Path) -> Path:
    fbx = out_dir / f"{obj.name}.fbx"
    bpy.ops.object.select_all(action="DESELECT")
    obj.select_set(True)
    bpy.context.view_layer.objects.active = obj
    bpy.ops.export_scene.fbx(
        filepath=str(fbx),
        use_selection=True,
        apply_scale_options="FBX_SCALE_ALL",
        mesh_smooth_type="FACE",
    )
    return fbx


def _render_preview(obj: bpy.types.Object, asset_id: str, tex_path: Path | None = None) -> Path:
    PREVIEW_DIR.mkdir(parents=True, exist_ok=True)
    preview = PREVIEW_DIR / f"{asset_id}.png"
    if tex_path and tex_path.is_file():
        import shutil

        shutil.copy(tex_path, preview)
        return preview
    cam_data = bpy.data.cameras.new("PreviewCam")
    cam = bpy.data.objects.new("PreviewCam", cam_data)
    bpy.context.collection.objects.link(cam)
    bound = [Vector(corner) for corner in obj.bound_box]
    center = sum(bound, Vector()) / 8
    cam.location = center + Vector((2.2, -2.2, 1.6))
    cam.data.lens = 50
    direction = center - cam.location
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    light_data = bpy.data.lights.new("Key", type="AREA")
    light_data.energy = 400
    light = bpy.data.objects.new("Key", light_data)
    light.location = center + Vector((1.5, 1.0, 2.5))
    bpy.context.collection.objects.link(light)
    bpy.context.scene.camera = cam
    bpy.context.scene.render.engine = "BLENDER_EEVEE"
    bpy.context.scene.render.resolution_x = 512
    bpy.context.scene.render.resolution_y = 512
    bpy.context.scene.render.filepath = str(preview)
    bpy.context.scene.render.image_settings.file_format = "PNG"
    try:
        bpy.ops.render.render(write_still=True)
    except RuntimeError:
        if tex_path and tex_path.is_file():
            import shutil

            shutil.copy(tex_path, preview)
        else:
            raise
    return preview


def bake_asset(asset_id: str, palette: tuple[float, float, float] | None = None) -> dict:
    """Import campus-kit OBJ, UV, bake 1024 base color, export glTF/FBX, preview."""
    obj_path = OBJ_IN / f"{asset_id}.obj"
    if not obj_path.is_file():
        raise FileNotFoundError(obj_path)
    _reset_scene()
    obj = _import_obj(obj_path)
    obj.name = f"mesh_{asset_id}"
    mat = _ensure_material(obj, asset_id)
    if palette:
        bsdf = next(n for n in mat.node_tree.nodes if n.type == "BSDF_PRINCIPLED")
        bsdf.inputs["Base Color"].default_value = (*palette, 1.0)
    _smart_uv(obj)
    out_dir = OUT_ROOT / asset_id
    out_dir.mkdir(parents=True, exist_ok=True)
    tex_path = _bake_diffuse(obj, mat)
    glb = _export_gltf(obj, out_dir)
    fbx = _export_fbx(obj, out_dir)
    preview = _render_preview(obj, asset_id, tex_path)
    manifest = {
        "assetId": asset_id,
        "scope": SCOPE,
        "sourceObj": str(obj_path.relative_to(REPO)),
        "texture1024": str(tex_path.relative_to(REPO)),
        "glb": str(glb.relative_to(REPO)),
        "fbx": str(fbx.relative_to(REPO)),
        "preview": str(preview.relative_to(REPO)),
    }
    (out_dir / "manifest.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    return manifest


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: blender --background --python _common.py -- <asset_id>", file=sys.stderr)
        raise SystemExit(2)
    aid = sys.argv[-1]
    print(json.dumps(bake_asset(aid), indent=2))