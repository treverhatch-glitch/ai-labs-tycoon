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

PALETTE = {
    "trim": (0.659, 0.345, 0.157, 1.0),
    "wall": (0.847, 0.784, 0.690, 1.0),
    "floor": (0.784, 0.722, 0.627, 1.0),
    "desk": (0.549, 0.431, 0.282, 1.0),
    "metal": (0.416, 0.431, 0.471, 1.0),
    "screen": (0.227, 0.549, 0.314, 1.0),
    "plastic": (0.290, 0.290, 0.322, 1.0),
    "cardboard": (0.72, 0.58, 0.42, 1.0),
}

ZIP_PATHS: dict[str, Path] = {
    "furniture": KENNEY / "kenney_furniture-kit.zip",
    "building": KENNEY / "kenney_building-kit.zip",
}

MODEL_INNER: dict[tuple[str, str], str] = {
    ("building", "floor"): "Models/GLB format/floor.glb",
    ("building", "wall"): "Models/GLB format/wall.glb",
    ("building", "corner"): "Models/GLB format/wall-corner.glb",
    ("building", "doorway"): "Models/GLB format/wall-doorway-wide-square.glb",
    ("building", "door"): "Models/GLB format/door-rotate-square-b.glb",
    ("furniture", "workbench"): "Models/GLTF format/table.glb",
    ("furniture", "desk"): "Models/GLTF format/desk.glb",
    ("furniture", "chair"): "Models/GLTF format/chairDesk.glb",
    ("furniture", "screen"): "Models/GLTF format/computerScreen.glb",
    ("furniture", "keyboard"): "Models/GLTF format/computerKeyboard.glb",
    ("furniture", "mouse"): "Models/GLTF format/computerMouse.glb",
    ("furniture", "shelf"): "Models/GLTF format/bookcaseOpenLow.glb",
    ("furniture", "box_closed"): "Models/GLTF format/cardboardBoxClosed.glb",
    ("furniture", "box_open"): "Models/GLTF format/cardboardBoxOpen.glb",
    ("furniture", "lamp"): "Models/GLTF format/lampRoundTable.glb",
    ("furniture", "server_rack"): "Models/GLTF format/bookcaseClosed.glb",
}

TEXTURE_INNER: dict[str, str] = {
    "building": "Models/GLB format/Textures/colormap.png",
}

_COLOMAPS: dict[str, bpy.types.Image] = {}


def _pack_dir(pack: str) -> Path:
    d = _CACHE / pack
    d.mkdir(parents=True, exist_ok=True)
    return d


def _ensure_colormap(pack: str) -> bpy.types.Image | None:
    inner = TEXTURE_INNER.get(pack)
    if not inner:
        return None
    if pack in _COLOMAPS:
        return _COLOMAPS[pack]
    dest_dir = _pack_dir(pack) / "Textures"
    dest_dir.mkdir(parents=True, exist_ok=True)
    dest = dest_dir / "colormap.png"
    if not dest.is_file():
        with zipfile.ZipFile(ZIP_PATHS[pack], "r") as zf:
            dest.write_bytes(zf.read(inner))
    img = bpy.data.images.load(str(dest), check_existing=True)
    img.colorspace_settings.name = "sRGB"
    _COLOMAPS[pack] = img
    return img


def _extract_glb(pack: str, asset: str) -> Path:
    inner = MODEL_INNER[(pack, asset)]
    dest_dir = _pack_dir(pack)
    if pack == "building":
        _ensure_colormap(pack)
    dest = dest_dir / Path(inner).name
    if not dest.is_file():
        with zipfile.ZipFile(ZIP_PATHS[pack], "r") as zf:
            dest.write_bytes(zf.read(inner))
    return dest


def _make_palette_material(name: str, palette_key: str) -> bpy.types.Material:
    tint = PALETTE.get(palette_key, PALETTE["wall"])
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Base Color"].default_value = tint
    bsdf.inputs["Roughness"].default_value = 0.58
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    return mat


def _make_colormap_material(name: str, pack: str) -> bpy.types.Material:
    """Kenney building atlas (UV-mapped); avoids EEVEE missing-texture pink on kit meshes."""
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Roughness"].default_value = 0.58
    img = _ensure_colormap(pack)
    if img and img.size[0] > 0:
        tex = nodes.new("ShaderNodeTexImage")
        tex.image = img
        links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    else:
        bsdf.inputs["Base Color"].default_value = PALETTE["wall"]
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    return mat


def _bind_materials(ob: bpy.types.Object, pack: str, palette_key: str) -> None:
    if pack == "building":
        mat = _make_colormap_material(f"{ob.name}_{palette_key}", pack)
    else:
        mat = _make_palette_material(f"{ob.name}_{palette_key}", palette_key)
    ob.data.materials.clear()
    ob.data.materials.append(mat)
    for poly in ob.data.polygons:
        poly.material_index = 0
    ob.update_tag()


def purge_import_orphans(props: list[bpy.types.Object]) -> None:
    """Drop glTF leftovers and force solid materials on all hero meshes."""
    keep = {p.name for p in props}
    for ob in list(bpy.data.objects):
        if ob.type == "MESH" and ob.name not in keep and ob.name != "PreviewShadowFloor":
            bpy.data.objects.remove(ob, do_unlink=True)
    used_mats: set[bpy.types.Material] = set()
    for ob in props:
        if ob.type != "MESH":
            continue
        pack = "building" if "_building_" in ob.name else "furniture"
        pal = "wall"
        if "_floor" in ob.name:
            pal = "floor"
        elif "_door" in ob.name and "doorway" not in ob.name:
            pal = "metal"
        elif any(k in ob.name for k in ("desk", "workbench", "shelf", "lamp", "mouse", "keyboard")):
            pal = "desk" if "workbench" in ob.name or "shelf" in ob.name or "desk" in ob.name else (
                "trim" if "lamp" in ob.name or "mouse" in ob.name else "plastic"
            )
        elif "chair" in ob.name:
            pal = "plastic"
        elif "screen" in ob.name or "server" in ob.name:
            pal = "screen" if "screen" in ob.name else "metal"
        elif "box" in ob.name:
            pal = "cardboard"
        _bind_materials(ob, pack, pal)
        for slot in ob.material_slots:
            if slot.material:
                used_mats.add(slot.material)
    for mat in list(bpy.data.materials):
        if mat not in used_mats:
            bpy.data.materials.remove(mat, do_unlink=True)
    for img in list(bpy.data.images):
        if img.users == 0:
            bpy.data.images.remove(img)
    bpy.context.view_layer.update()


def audit_materials(objects: list[bpy.types.Object]) -> list[str]:
    """Return errors for broken TEX_IMAGE nodes (magenta risk)."""
    errs: list[str] = []
    for ob in objects:
        if ob.type != "MESH":
            continue
        for slot in ob.material_slots:
            mat = slot.material
            if not mat or not mat.use_nodes:
                errs.append(f"{ob.name}: material missing nodes")
                continue
            for node in mat.node_tree.nodes:
                if node.type != "TEX_IMAGE":
                    continue
                img = node.image
                if img is None or img.size[0] == 0:
                    errs.append(f"{ob.name}:{mat.name} TEX_IMAGE broken")
    return errs


def mesh_min_z(ob: bpy.types.Object) -> float:
    return min((ob.matrix_world @ v.co).z for v in ob.data.vertices)


def snap_to_floor_z(ob: bpy.types.Object, floor_z: float = 0.0) -> None:
    bpy.context.view_layer.update()
    low = mesh_min_z(ob)
    if abs(low - floor_z) > 1e-4:
        ob.location.z += floor_z - low
    bpy.context.view_layer.update()


def mesh_min_y(ob: bpy.types.Object) -> float:
    return min((ob.matrix_world @ v.co).y for v in ob.data.vertices)


def audit_grounding(
    objects: list[bpy.types.Object],
    *,
    up_axis: str = "Z",
    floor: float = 0.0,
    wall_mounted: set[str] | None = None,
    exempt: set[str] | None = None,
    ceiling_mounted: set[str] | None = None,
    ceiling_z: float = 2.15,
    wall_plane: str = "back",
    wall_coord: float = 0.0,
    tol_floor: float = 0.02,
    tol_wall: float = 0.22,
) -> list[str]:
    """Fail gate when meshes float: floor props touch floor; wall props touch a wall plane."""
    wall_mounted = wall_mounted or set()
    exempt = exempt or set()
    ceiling_mounted = ceiling_mounted or set()
    errs: list[str] = []
    bpy.context.view_layer.update()
    low_fn = mesh_min_z if up_axis.upper() == "Z" else mesh_min_y

    def _verts(ob: bpy.types.Object) -> list[Vector]:
        return [ob.matrix_world @ v.co for v in ob.data.vertices]

    for ob in objects:
        if ob.type != "MESH" or ob.name == "PreviewShadowFloor":
            continue
        if ob.name.startswith("shell_"):
            continue
        if any(k in ob.name for k in exempt):
            continue
        if any(k in ob.name for k in ceiling_mounted):
            hi = max(p.z for p in _verts(ob)) if up_axis.upper() == "Z" else max(p.y for p in _verts(ob))
            if hi < ceiling_z - 0.35:
                errs.append(f"{ob.name}: not at ceiling z={hi:.3f}")
            continue
        if any(k in ob.name for k in wall_mounted):
            pts = _verts(ob)
            if wall_plane == "back":
                nearest = min(p.x for p in pts)
                if nearest > wall_coord + tol_wall:
                    errs.append(f"{ob.name}: wall gap {nearest - wall_coord:.3f}")
            elif wall_plane == "side":
                nearest = min(p.y for p in pts)
                if nearest > wall_coord + tol_wall:
                    errs.append(f"{ob.name}: wall gap {nearest - wall_coord:.3f}")
            continue
        low = low_fn(ob)
        if low > floor + tol_floor:
            errs.append(f"{ob.name}: float low={low:.3f} floor={floor:.3f}")
    return errs


def snap_to_floor_y(ob: bpy.types.Object, floor_y: float = 0.0) -> None:
    bpy.context.view_layer.update()
    low = mesh_min_y(ob)
    if abs(low - floor_y) > 1e-4:
        ob.location.y += floor_y - low
    bpy.context.view_layer.update()


def import_kenney(
    pack: str,
    asset: str,
    location: tuple[float, float, float],
    rot_z_deg: float = 0.0,
    rot_y_deg: float = 0.0,
    scale: float | tuple[float, float, float] = 1.0,
    palette_key: str = "wall",
    up_axis: str = "Y",
    floor_snap: bool = True,
) -> bpy.types.Object | None:
    glb = _extract_glb(pack, asset)
    before = {o.name for o in bpy.data.objects}
    bpy.ops.import_scene.gltf(filepath=str(glb))
    imported = [o for o in bpy.context.selected_objects]
    spare_names = [o.name for o in imported]
    meshes = [o for o in imported if o.type == "MESH"]
    if not meshes:
        return None
    bpy.ops.object.select_all(action="DESELECT")
    for o in meshes:
        o.select_set(True)
    bpy.context.view_layer.objects.active = meshes[0]
    if len(meshes) > 1:
        bpy.ops.object.join()
    root = bpy.context.view_layer.objects.active
    base = f"kenney_{pack}_{asset}"
    suffix = sum(1 for o in bpy.data.objects if o.name.startswith(base))
    root.name = base if suffix == 0 else f"{base}_{suffix:02d}"
    if isinstance(scale, (int, float)):
        root.scale = (scale, scale, scale)
    else:
        root.scale = Vector(scale)
    root.rotation_euler[1] = math.radians(rot_y_deg)
    root.rotation_euler[2] = math.radians(rot_z_deg)
    bpy.ops.object.transform_apply(scale=True, rotation=True)
    root.location = Vector(location)
    _bind_materials(root, pack, palette_key)
    if floor_snap:
        if up_axis.upper() == "Y":
            snap_to_floor_y(root)
        else:
            snap_to_floor_z(root)
    for name in spare_names:
        if name == root.name:
            continue
        ob = bpy.data.objects.get(name)
        if ob is not None:
            bpy.data.objects.remove(ob, do_unlink=True)
    return root