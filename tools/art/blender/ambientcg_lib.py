"""ambientCG (CC0) PBR bind — concrete floor + brick wall for hero shell."""
from __future__ import annotations

from pathlib import Path

import bpy

REPO = Path(__file__).resolve().parents[3]
ACG = REPO / "tools/art/cc0/incoming/ambientcg"

SET_DIR = {
    "concrete": ACG / "Concrete047A_1K-JPG",
    "brick": ACG / "Bricks085_1K-JPG",
}

SET_FILES = {
    "concrete": {
        "color": "Concrete047A_1K-JPG_Color.jpg",
        "normal": "Concrete047A_1K-JPG_NormalGL.jpg",
        "roughness": "Concrete047A_1K-JPG_Roughness.jpg",
    },
    "brick": {
        "color": "Bricks085_1K-JPG_Color.jpg",
        "normal": "Bricks085_1K-JPG_NormalGL.jpg",
        "roughness": "Bricks085_1K-JPG_Roughness.jpg",
    },
}

_CACHE: dict[str, bpy.types.Material] = {}


def _img(path: Path, colorspace: str) -> bpy.types.Image | None:
    if not path.is_file():
        return None
    im = bpy.data.images.load(str(path), check_existing=True)
    im.colorspace_settings.name = colorspace
    return im


def material(set_id: str, uv_scale: float = 1.0) -> bpy.types.Material:
    key = f"{set_id}@{uv_scale:.2f}"
    if key in _CACHE:
        return _CACHE[key]
    base = SET_DIR.get(set_id)
    names = SET_FILES.get(set_id)
    if not base or not names:
        raise FileNotFoundError(f"unknown ambientcg set {set_id}")
    alb_p = base / names["color"]
    if not alb_p.is_file():
        raise FileNotFoundError(f"missing ambientcg {alb_p} — run fetch_ambientcg_shell.sh")
    mat = bpy.data.materials.new(f"acg_{set_id}")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Roughness"].default_value = 0.72
    tc = nodes.new("ShaderNodeTexCoord")
    mapping = nodes.new("ShaderNodeMapping")
    links.new(tc.outputs["UV"], mapping.inputs["Vector"])
    if abs(uv_scale - 1.0) > 1e-3:
        mapping.inputs["Scale"].default_value = (uv_scale, uv_scale, uv_scale)
    uv = mapping.outputs["Vector"]
    alb = _img(alb_p, "sRGB")
    tex = nodes.new("ShaderNodeTexImage")
    tex.image = alb
    links.new(uv, tex.inputs["Vector"])
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    rough_p = base / names["roughness"]
    rp = _img(rough_p, "Non-Color")
    if rp:
        rt = nodes.new("ShaderNodeTexImage")
        rt.image = rp
        links.new(uv, rt.inputs["Vector"])
        links.new(rt.outputs["Color"], bsdf.inputs["Roughness"])
    norm_p = base / names["normal"]
    np = _img(norm_p, "Non-Color")
    if np:
        nt = nodes.new("ShaderNodeTexImage")
        nt.image = np
        links.new(uv, nt.inputs["Vector"])
        nmap = nodes.new("ShaderNodeNormalMap")
        links.new(nt.outputs["Color"], nmap.inputs["Color"])
        links.new(nmap.outputs["Normal"], bsdf.inputs["Normal"])
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    _CACHE[key] = mat
    return mat


def assign(ob: bpy.types.Object, set_id: str, uv_scale: float = 1.0) -> None:
    if ob.type != "MESH":
        return
    mat = material(set_id, uv_scale=uv_scale)
    ob.data.materials.clear()
    ob.data.materials.append(mat)
    for poly in ob.data.polygons:
        poly.material_index = 0
    ob.update_tag()


def audit_surface_maps(object_names: list[str]) -> list[str]:
    errs: list[str] = []
    for name in object_names:
        ob = bpy.data.objects.get(name)
        if ob is None or ob.type != "MESH" or not name.startswith("shell_"):
            continue
        for slot in ob.material_slots:
            mat = slot.material
            if not mat or not mat.use_nodes:
                errs.append(f"{ob.name}: no material nodes")
                continue
            has_alb = has_rough = False
            for node in mat.node_tree.nodes:
                if node.type != "TEX_IMAGE" or not node.image:
                    continue
                nm = node.image.name.lower()
                if "color" in nm:
                    has_alb = True
                if "rough" in nm:
                    has_rough = True
            if not has_alb or not has_rough:
                errs.append(f"{ob.name}:{mat.name} missing color/rough maps")
    return errs


def audit_roughness_floor(
    objects: list[bpy.types.Object],
    *,
    glass_keys: tuple[str, ...] = ("screen", "emit", "strip"),
    min_rough: float = 0.2,
) -> list[str]:
    errs: list[str] = []
    for ob in objects:
        if ob.type != "MESH":
            continue
        if any(k in ob.name for k in glass_keys):
            continue
        for slot in ob.material_slots:
            mat = slot.material
            if not mat or not mat.use_nodes:
                continue
            has_rough_tex = any(
                n.type == "TEX_IMAGE" and n.image and "rough" in n.image.name.lower()
                for n in mat.node_tree.nodes
            )
            if has_rough_tex:
                continue
            bsdf = next((n for n in mat.node_tree.nodes if n.type == "BSDF_PRINCIPLED"), None)
            if bsdf and float(bsdf.inputs["Roughness"].default_value) < min_rough:
                errs.append(f"{ob.name}:{mat.name} roughness<{min_rough}")
    return errs