"""Kenney CC0 glTF import helpers for garage hero (Blender Z-up, game Y-up parity)."""
from __future__ import annotations

import math
import zipfile
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
KENNEY = REPO / "tools/art/cc0/incoming/kenney"
_CACHE = REPO / "tools/art/cc0/staging/garage-hero-kenney"

# Early-game role palette (1995 garage).
PALETTE = {
    "trim": (0.659, 0.345, 0.157, 1.0),  # #A85828
    "wall": (0.847, 0.784, 0.690, 1.0),  # #D8C8B0
    "floor": (0.784, 0.722, 0.627, 1.0),  # #C8B8A0
    "desk": (0.549, 0.431, 0.282, 1.0),  # #8C6E48
    "metal": (0.416, 0.431, 0.471, 1.0),  # #6A6E78
    "screen": (0.227, 0.549, 0.314, 1.0),  # #3A8C50
    "plastic": (0.290, 0.290, 0.322, 1.0),  # #4A4A52
}

ZIP_PATHS: dict[str, Path] = {
    "furniture": KENNEY / "kenney_furniture-kit.zip",
    "building": KENNEY / "kenney_building-kit.zip",
    "suburban": KENNEY / "kenney_city-kit-suburban_20.zip",
}

MODEL_INNER: dict[tuple[str, str], str] = {
    ("building", "floor"): "Models/GLB format/floor.glb",
    ("building", "wall"): "Models/GLB format/wall.glb",
    ("building", "wall_side"): "Models/GLB format/wall-half.glb",
    ("building", "doorway"): "Models/GLB format/wall-doorway-wide-square.glb",
    ("building", "door"): "Models/GLB format/door-rotate-square-b.glb",
    ("furniture", "desk"): "Models/GLTF format/desk.glb",
    ("furniture", "chair"): "Models/GLTF format/chairDesk.glb",
    ("furniture", "screen"): "Models/GLTF format/computerScreen.glb",
    ("furniture", "keyboard"): "Models/GLTF format/computerKeyboard.glb",
    ("furniture", "mouse"): "Models/GLTF format/computerMouse.glb",
    ("furniture", "shelf"): "Models/GLTF format/bookcaseOpenLow.glb",
}


def _extract_glb(pack: str, asset: str) -> Path:
    inner = MODEL_INNER[(pack, asset)]
    _CACHE.mkdir(parents=True, exist_ok=True)
    dest = _CACHE / inner.replace("/", "__")
    if dest.is_file():
        return dest
    zpath = ZIP_PATHS[pack]
    if not zpath.is_file():
        raise FileNotFoundError(zpath)
    with zipfile.ZipFile(zpath, "r") as zf:
        dest.write_bytes(zf.read(inner))
    return dest


def _tint_materials(ob: bpy.types.Object, rgba: tuple[float, float, float, float]) -> None:
    for slot in ob.material_slots:
        mat = slot.material
        if mat is None:
            continue
        if not mat.use_nodes:
            continue
        bsdf = mat.node_tree.nodes.get("Principled BSDF")
        if bsdf:
            bsdf.inputs["Base Color"].default_value = rgba
            if "Roughness" in bsdf.inputs:
                bsdf.inputs["Roughness"].default_value = 0.62


def mesh_min_z(ob: bpy.types.Object) -> float:
    return min((ob.matrix_world @ v.co).z for v in ob.data.vertices)


def snap_to_floor_z(ob: bpy.types.Object, floor_z: float = 0.0) -> None:
    bpy.context.view_layer.update()
    low = mesh_min_z(ob)
    if abs(low - floor_z) > 1e-4:
        ob.location.z += floor_z - low
    bpy.context.view_layer.update()


def import_kenney(
    pack: str,
    asset: str,
    location: tuple[float, float, float],
    rot_z_deg: float = 0.0,
    scale: float = 1.0,
    palette_key: str = "wall",
) -> bpy.types.Object | None:
    glb = _extract_glb(pack, asset)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.selected_objects if o.type == "MESH"]
    if not meshes:
        return None
    bpy.ops.object.select_all(action="DESELECT")
    for o in meshes:
        o.select_set(True)
    bpy.context.view_layer.objects.active = meshes[0]
    if len(meshes) > 1:
        bpy.ops.object.join()
    root = bpy.context.view_layer.objects.active
    root.name = f"kenney_{pack}_{asset}"
    # Kenney GLTF is Y-up; scene uses Z-up (game Y-up).
    root.rotation_euler[0] = math.radians(-90.0)
    bpy.ops.object.transform_apply(rotation=True)
    root.location = Vector(location)
    root.scale = (scale, scale, scale)
    root.rotation_euler[2] = math.radians(rot_z_deg)
    bpy.ops.object.transform_apply(scale=True, rotation=True)
    _tint_materials(root, PALETTE.get(palette_key, PALETTE["wall"]))
    snap_to_floor_z(root)
    return root