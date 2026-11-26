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


def _world_bbox(objs: list[bpy.types.Object]) -> tuple[Vector, Vector]:
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in objs:
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins.x = min(mins.x, w.x)
            mins.y = min(mins.y, w.y)
            mins.z = min(mins.z, w.z)
            maxs.x = max(maxs.x, w.x)
            maxs.y = max(maxs.y, w.y)
            maxs.z = max(maxs.z, w.z)
    return mins, maxs


def _render_preview_from_glb(glb_path: Path, asset_id: str) -> Path:
    """Standard Hatch rig: grey ground, contact shadow, 3/4 camera, 1024, from committed GLB."""
    import hashlib
    import json

    PREVIEW_DIR.mkdir(parents=True, exist_ok=True)
    preview = PREVIEW_DIR / f"{asset_id}.png"
    _reset_scene()
    bpy.ops.import_scene.gltf(filepath=str(glb_path))
    mesh_obs = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not mesh_obs:
        raise RuntimeError(f"no mesh in {glb_path}")

    mins, maxs = _world_bbox(mesh_obs)
    center = (mins + maxs) / 2
    span = max(maxs.x - mins.x, maxs.y - mins.y, maxs.z - mins.z, 0.01)

    bpy.ops.mesh.primitive_plane_add(size=span * 4, location=(center.x, center.y, mins.z - 0.002))
    ground = bpy.context.active_object
    gmat = bpy.data.materials.new("Ground")
    gmat.use_nodes = True
    gmat.node_tree.nodes["Principled BSDF"].inputs["Base Color"].default_value = (0.55, 0.55, 0.55, 1)
    gmat.node_tree.nodes["Principled BSDF"].inputs["Roughness"].default_value = 0.92
    ground.data.materials.append(gmat)

    bpy.ops.object.light_add(type="SUN", location=center + Vector((span * 2, -span * 2, span * 3)))
    key = bpy.context.active_object
    key.data.energy = 2.8
    key.rotation_euler = (math.radians(55), 0, math.radians(35))

    bpy.ops.object.light_add(type="AREA", location=center + Vector((-span, span * 1.5, span * 2)))
    fill = bpy.context.active_object
    fill.data.energy = 180
    fill.data.size = span

    cam_data = bpy.data.cameras.new("PreviewCam")
    cam = bpy.data.objects.new("PreviewCam", cam_data)
    bpy.context.collection.objects.link(cam)
    dist = span * 2.4
    elev = math.radians(30)
    az = math.radians(45)
    cam.location = center + Vector(
        (dist * math.cos(elev) * math.cos(az), dist * math.cos(elev) * math.sin(az), dist * math.sin(elev) + span * 0.25)
    )
    direction = center - cam.location
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    cam.data.lens = 52
    bpy.context.scene.camera = cam

    bpy.context.scene.render.engine = "BLENDER_EEVEE"
    bpy.context.scene.render.resolution_x = 1024
    bpy.context.scene.render.resolution_y = 1024
    bpy.context.scene.render.filepath = str(preview)
    bpy.context.scene.render.image_settings.file_format = "PNG"
    if hasattr(bpy.context.scene, "view_settings"):
        bpy.context.scene.view_settings.exposure = 0.35
    bpy.ops.render.render(write_still=True)

    sha = hashlib.sha256(glb_path.read_bytes()).hexdigest()
    frame = PREVIEW_DIR / f"{asset_id}.frame.json"
    frame.write_text(
        json.dumps(
            {
                "assetId": asset_id,
                "preview": str(preview.relative_to(REPO)),
                "glb": str(glb_path.relative_to(REPO)),
                "glb_sha256": sha,
                "resolution": 1024,
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )
    return preview


def bake_asset(asset_id: str, palette: tuple[float, float, float] | None = None) -> dict:
    """Import campus-kit OBJ, UV, bake 1024 base color, export glTF/FBX, preview."""
    obj_path = OBJ_IN / f"{asset_id}.obj"
    if not obj_path.is_file():
        raise FileNotFoundError(obj_path)
    _reset_scene()
    obj = _import_obj(obj_path)
    obj.name = f"mesh_{asset_id}"
    mod = obj.modifiers.new("Bevel", type="BEVEL")
    mod.width = 0.012
    mod.segments = 2
    mod.limit_method = "ANGLE"
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)
    bpy.ops.object.modifier_apply(modifier=mod.name)
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
    preview = _render_preview_from_glb(glb, asset_id)
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