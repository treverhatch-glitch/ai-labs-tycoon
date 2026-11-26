"""Garage-kit glTF import with in-repo PBR texture bind (Unity EarlyGame parity)."""
from __future__ import annotations

from pathlib import Path

import bpy

REPO = Path(__file__).resolve().parents[3]
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"

_PBR_CACHE: dict[str, bpy.types.Material] = {}


def _load_image(path: Path, colorspace: str) -> bpy.types.Image | None:
    if not path.is_file():
        return None
    img = bpy.data.images.load(str(path), check_existing=True)
    img.colorspace_settings.name = colorspace
    return img


def pbr_material(asset_id: str) -> bpy.types.Material:
    if asset_id in _PBR_CACHE:
        return _PBR_CACHE[asset_id]
    tex_dir = KIT / "textures" / asset_id
    mat = bpy.data.materials.new(name=f"pbr_{asset_id}")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Roughness"].default_value = 0.55
    alb = _load_image(tex_dir / f"{asset_id}_albedo.png", "sRGB")
    if alb:
        tex = nodes.new("ShaderNodeTexImage")
        tex.image = alb
        links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    rough = _load_image(tex_dir / f"{asset_id}_roughness.png", "Non-Color")
    if rough:
        rtex = nodes.new("ShaderNodeTexImage")
        rtex.image = rough
        links.new(rtex.outputs["Color"], bsdf.inputs["Roughness"])
    norm = _load_image(tex_dir / f"{asset_id}_normal.png", "Non-Color")
    if norm:
        nt = nodes.new("ShaderNodeTexImage")
        nt.image = norm
        nmap = nodes.new("ShaderNodeNormalMap")
        links.new(nt.outputs["Color"], nmap.inputs["Color"])
        links.new(nmap.outputs["Normal"], bsdf.inputs["Normal"])
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    _PBR_CACHE[asset_id] = mat
    return mat


def assign_pbr(ob: bpy.types.Object, asset_id: str) -> None:
    if ob.type != "MESH":
        return
    mat = pbr_material(asset_id)
    ob.data.materials.clear()
    ob.data.materials.append(mat)
    for poly in ob.data.polygons:
        poly.material_index = 0
    ob.update_tag()


def audit_pbr(objects: list[bpy.types.Object]) -> list[str]:
    errs: list[str] = []
    names = [o.name for o in objects]
    for name in names:
        ob = bpy.data.objects.get(name)
        if ob is None or ob.type != "MESH":
            continue
        for slot in ob.material_slots:
            mat = slot.material
            if not mat or not mat.use_nodes:
                errs.append(f"{ob.name}: missing material")
                continue
            has_tex = any(n.type == "TEX_IMAGE" and n.image for n in mat.node_tree.nodes)
            if not has_tex:
                errs.append(f"{ob.name}:{mat.name} no bound albedo")
    return errs