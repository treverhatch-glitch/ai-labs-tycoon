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
    "floor": (0.47, 0.48, 0.47, 1.0),
    "wall": (0.92, 0.58, 0.28, 1.0),
    "trim": (0.88, 0.48, 0.18, 1.0),
    "metal": (0.48, 0.62, 0.88, 1.0),
    "glass": (0.55, 0.75, 0.88, 1.0),
    "wood": (0.72, 0.44, 0.20, 1.0),
    "prop": (0.78, 0.58, 0.38, 1.0),
    "board": (0.92, 0.90, 0.86, 1.0),
    "screen": (0.22, 0.48, 0.88, 1.0),
    "lamp_accent": (0.95, 0.58, 0.12, 1.0),
    "box": (0.78, 0.62, 0.38, 1.0),
    "box_open": (0.72, 0.56, 0.34, 1.0),
    "chair": (0.32, 0.52, 0.78, 1.0),
    "shelf": (0.78, 0.62, 0.38, 1.0),
    "plant": (0.22, 0.72, 0.32, 1.0),
    "floor_stain": (0.06, 0.055, 0.05, 1.0),
    "rug_muted": (0.42, 0.28, 0.30, 1.0),
    "speaker_dark": (0.14, 0.15, 0.17, 1.0),
}

GARAGE_KIT = ART_ROOT / "garage-kit"
CORNER_RGB = (150, 150, 150)


def _srgb_byte_to_linear(byte: int) -> float:
    c = byte / 255.0
    if c <= 0.04045:
        return c / 12.92
    return ((c + 0.055) / 1.055) ** 2.4


BACKDROP_GREY = _srgb_byte_to_linear(150)


def garage_glb(name: str) -> Path:
    return GARAGE_KIT / f"{name}.glb"


def reset_scene() -> None:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.context.scene.render.engine = "CYCLES"
    bpy.context.scene.cycles.device = "CPU"
    bpy.context.scene.cycles.samples = 64
    bpy.context.scene.unit_settings.system = "METRIC"
    bpy.context.scene.unit_settings.scale_length = 1.0


def kenney_glb(name: str) -> Path:
    return INCOMING / "kenney_building-kit/Models/GLB format" / name


def kenney_furniture_glb(name: str) -> Path:
    return INCOMING / "kenney_furniture-kit/Models/GLB format" / name


def import_gltf(path: Path, name: str) -> list[bpy.types.Object]:
    before = set(bpy.context.scene.objects)
    bpy.ops.import_scene.gltf(filepath=str(path))
    imported = [o for o in bpy.context.scene.objects if o not in before and o.type == "MESH"]
    for ob in imported:
        ob.name = name if len(imported) == 1 else f"{name}_{ob.name}"
    return imported


def add_metric_box(
    name: str,
    size: tuple[float, float, float],
    location: tuple[float, float, float],
    role: str | None = None,
) -> bpy.types.Object:
    """Axis-aligned box with applied scale (Blender Y-up)."""
    bpy.ops.mesh.primitive_cube_add(size=1, location=location)
    ob = bpy.context.active_object
    ob.name = name
    ob.scale = (size[0] / 2, size[1] / 2, size[2] / 2)
    bpy.ops.object.transform_apply(scale=True)
    if role:
        apply_palette_material(ob, role)
    return ob


def world_bbox(ob: bpy.types.Object) -> tuple[Vector, Vector]:
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for corner in ob.bound_box:
        w = ob.matrix_world @ Vector(corner)
        mins.x = min(mins.x, w.x)
        mins.y = min(mins.y, w.y)
        mins.z = min(mins.z, w.z)
        maxs.x = max(maxs.x, w.x)
        maxs.y = max(maxs.y, w.y)
        maxs.z = max(maxs.z, w.z)
    return mins, maxs


def snap_to_floor(ob: bpy.types.Object, floor_top: float = 0.0) -> float:
    bpy.context.view_layer.update()
    mins, _ = world_bbox(ob)
    delta = floor_top - mins.y
    ob.location.y += delta
    return delta


def weld_mesh(ob: bpy.types.Object, threshold: float = 0.0008) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=threshold)
    bpy.ops.object.mode_set(mode="OBJECT")


def join_welded(
    objects: list[bpy.types.Object],
    name: str,
    role: str | None = None,
) -> bpy.types.Object:
    ob = merge_mesh_objects(objects, name)
    weld_mesh(ob)
    if role:
        apply_palette_material(ob, role)
    return ob


def mesh_boolean_difference(target: bpy.types.Object, cutter: bpy.types.Object) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    target.select_set(True)
    bpy.context.view_layer.objects.active = target
    mod = target.modifiers.new("WallCut", "BOOLEAN")
    mod.operation = "DIFFERENCE"
    mod.object = cutter
    mod.solver = "EXACT"
    bpy.ops.object.modifier_apply(modifier=mod.name)
    bpy.data.objects.remove(cutter, do_unlink=True)


def cardboard_lid_gap_m(ob: bpy.types.Object) -> float:
    """Vertical gap between lowest and next-loose-part top (lid float), in metres."""
    import bmesh

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.verts.ensure_lookup_table()
    bm.faces.ensure_lookup_table()
    parts: list[list[bmesh.types.BMVert]] = []
    seen = set()
    for v in bm.verts:
        if v.index in seen:
            continue
        stack = [v]
        comp: list[bmesh.types.BMVert] = []
        while stack:
            cur = stack.pop()
            if cur.index in seen:
                continue
            seen.add(cur.index)
            comp.append(cur)
            for e in cur.link_edges:
                other = e.other_vert(cur)
                if other.index not in seen:
                    stack.append(other)
        if comp:
            parts.append(comp)
    bm.free()
    if len(parts) < 2:
        return 0.0
    spans: list[tuple[float, float, float]] = []
    for comp in parts:
        zs = [ob.matrix_world @ v.co for v in comp]
        zvals = [p.z for p in zs]
        spans.append((min(zvals), max(zvals), max(zvals) - min(zvals)))
    spans.sort(key=lambda t: t[0])
    base_top = spans[0][1]
    best = 0.0
    for z0, z1, _ in spans[1:]:
        gap = z0 - base_top
        if gap > best:
            best = gap
    return best


def snap_cardboard_lid(ob: bpy.types.Object, max_gap: float = 0.01) -> None:
    gap = cardboard_lid_gap_m(ob)
    if gap <= max_gap:
        return
    import bmesh

    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bm.verts.ensure_lookup_table()
    seen: set[int] = set()
    parts: list[list[bmesh.types.BMVert]] = []
    for v in bm.verts:
        if v.index in seen:
            continue
        stack = [v]
        comp: list[bmesh.types.BMVert] = []
        while stack:
            cur = stack.pop()
            if cur.index in seen:
                continue
            seen.add(cur.index)
            comp.append(cur)
            for e in cur.link_edges:
                other = e.other_vert(cur)
                if other.index not in seen:
                    stack.append(other)
        parts.append(comp)
    spans = []
    for comp in parts:
        zvals = [(ob.matrix_world @ v.co).z for v in comp]
        spans.append((min(zvals), max(zvals), comp))
    spans.sort(key=lambda t: t[0])
    if len(spans) < 2:
        bm.free()
        return
    base_top = spans[0][1]
    lid_mn, _, lid_verts = spans[-1]
    gap = lid_mn - base_top
    if gap <= max_gap:
        bm.free()
        return
    inv = ob.matrix_world.inverted()
    shift = gap - 0.001
    for v in lid_verts:
        w = ob.matrix_world @ v.co
        w.z -= shift
        v.co = inv @ w
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()
    bpy.context.view_layer.update()


def monolith_cardboard_box(ob: bpy.types.Object, max_gap: float = 0.01) -> None:
    """Force closed cardboard to a single contact shell (mesh bounds for gates)."""
    weld_cardboard_closed(ob, max_gap=max_gap)
    for _ in range(4):
        if cardboard_lid_gap_m(ob) <= max_gap:
            break
        weld_cardboard_closed(ob, max_gap=max_gap)
    import bmesh

    gap = cardboard_lid_gap_m(ob)
    if gap <= max_gap:
        return
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    mw = ob.matrix_world
    inv = mw.inverted()
    zvals = sorted((mw @ v.co).z for v in bm.verts)
    if len(zvals) < 4:
        bm.free()
        return
    body_top = zvals[int(len(zvals) * 0.55)]
    shift = gap + 0.002
    for v in bm.verts:
        wz = (mw @ v.co).z
        if wz > body_top:
            w = mw @ v.co
            w.z -= shift
            v.co = inv @ w
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()
    bpy.context.view_layer.update()
    mesh_remove_doubles(ob, 0.12)
    snap_cardboard_lid(ob, max_gap=max_gap)


def weld_cardboard_closed(ob: bpy.types.Object, max_gap: float = 0.01) -> None:
    """Merge lid shell onto closed Kenney cardboard box (render + gate contact)."""
    mesh_remove_doubles(ob, 0.08)
    for _ in range(8):
        snap_cardboard_lid(ob, max_gap=max_gap)
        if cardboard_lid_gap_m(ob) <= max_gap:
            return
    import bmesh

    gap = cardboard_lid_gap_m(ob)
    if gap <= max_gap:
        return
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    mw = ob.matrix_world
    inv = mw.inverted()
    zvals = sorted((mw @ v.co).z for v in bm.verts)
    if not zvals:
        bm.free()
        return
    split = zvals[int(len(zvals) * 0.68)]
    shift = gap - 0.0005
    for v in bm.verts:
        if (mw @ v.co).z >= split - 0.015:
            w = mw @ v.co
            w.z -= shift
            v.co = inv @ w
    bm.to_mesh(ob.data)
    ob.data.update()
    bm.free()
    bpy.context.view_layer.update()
    snap_cardboard_lid(ob, max_gap=max_gap)


def mesh_remove_doubles(ob: bpy.types.Object, threshold: float = 0.015) -> None:
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    bpy.ops.mesh.remove_doubles(threshold=threshold)
    bpy.ops.object.mode_set(mode="OBJECT")


def apply_palette_hierarchy(ob: bpy.types.Object, role: str) -> None:
    if ob.type == "MESH":
        apply_palette_material(ob, role)
    for child in ob.children:
        apply_palette_hierarchy(child, role)


def _simple_tint_material(name: str, role: str) -> bpy.types.Material:
    """Kenney-friendly tint: preserve mesh reads, no object-zone procedural paint."""
    tint = PALETTE.get(role, PALETTE["prop"])
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Base Color"].default_value = tint
    rough = 0.55 if role in ("wood", "chair") else 0.48
    if role in ("metal", "screen"):
        rough = 0.42
        bsdf.inputs["Metallic"].default_value = 0.35
    elif role == "lamp_accent":
        rough = 0.38
    bsdf.inputs["Roughness"].default_value = rough
    if role == "lamp_accent":
        emit = nodes.new("ShaderNodeEmission")
        emit.inputs["Color"].default_value = (1.0, 0.85, 0.45, 1.0)
        emit.inputs["Strength"].default_value = 0.22
        add = nodes.new("ShaderNodeAddShader")
        links.new(bsdf.outputs["BSDF"], add.inputs[0])
        links.new(emit.outputs["Emission"], add.inputs[1])
        links.new(add.outputs["Shader"], out.inputs["Surface"])
    elif role == "screen":
        emit = nodes.new("ShaderNodeEmission")
        emit.inputs["Color"].default_value = (0.55, 0.62, 0.78, 1.0)
        emit.inputs["Strength"].default_value = 0.06
        add = nodes.new("ShaderNodeAddShader")
        links.new(bsdf.outputs["BSDF"], add.inputs[0])
        links.new(emit.outputs["Emission"], add.inputs[1])
        links.new(add.outputs["Shader"], out.inputs["Surface"])
    else:
        links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    return mat


def apply_kenney_restyle(ob: bpy.types.Object, roles: list[str]) -> None:
    """Assign per-slot tints so Kenney furniture keeps silhouette (seat, legs, shade)."""
    if ob.type != "MESH":
        for child in ob.children:
            apply_kenney_restyle(child, roles)
        return
    slot_count = max(1, len(ob.data.materials))
    if slot_count == 1 and len(ob.data.polygons):
        ob.data.materials.clear()
        ob.data.materials.append(_simple_tint_material(f"m_{ob.name}_0", roles[0]))
        return
    while len(ob.data.materials) < len(roles):
        ob.data.materials.append(None)
    for i, role in enumerate(roles):
        mat = _simple_tint_material(f"m_{ob.name}_{i}_{role}", role)
        if i < len(ob.data.materials):
            ob.data.materials[i] = mat
    for i in range(len(roles), len(ob.data.materials)):
        role = roles[-1]
        ob.data.materials[i] = _simple_tint_material(f"m_{ob.name}_{i}_{role}", role)


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


def apply_palette_material(ob: bpy.types.Object, role: str) -> None:
    """Solid role tints (+ optional concrete floor) so exports are readable, not palette noise."""
    tint = PALETTE.get(role, PALETTE["prop"])
    mat = bpy.data.materials.new(name=f"m_{ob.name}_{role}")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    concrete = INCOMING / "ambientcg-Concrete015/Concrete015_2K-JPG_Color.jpg"
    if role == "floor" and concrete.is_file():
        tex = nodes.new("ShaderNodeTexImage")
        tex.image = load_image(concrete)
        mapping = nodes.new("ShaderNodeMapping")
        mapping.inputs["Scale"].default_value = (2.8, 2.8, 2.8)
        coord = nodes.new("ShaderNodeTexCoord")
        links.new(coord.outputs["Object"], mapping.inputs["Vector"])
        links.new(mapping.outputs["Vector"], tex.inputs["Vector"])
        warm = nodes.new("ShaderNodeMixRGB")
        warm.blend_type = "OVERLAY"
        bright = nodes.new("ShaderNodeMixRGB")
        bright.blend_type = "MIX"
        bright.inputs["Fac"].default_value = 0.55
        bright.inputs["Color2"].default_value = (0.88, 0.86, 0.84, 1.0)
        warm = nodes.new("ShaderNodeMixRGB")
        warm.blend_type = "OVERLAY"
        warm.inputs["Fac"].default_value = 0.22
        warm.inputs["Color2"].default_value = (0.72, 0.52, 0.34, 1.0)
        stain = nodes.new("ShaderNodeTexNoise")
        stain.inputs["Scale"].default_value = 4.0
        stain.inputs["Detail"].default_value = 6.0
        stain_map = nodes.new("ShaderNodeMapRange")
        stain_map.inputs["From Min"].default_value = 0.35
        stain_map.inputs["From Max"].default_value = 0.7
        stain_map.inputs["To Min"].default_value = 0.88
        stain_map.inputs["To Max"].default_value = 1.0
        links.new(stain.outputs["Fac"], stain_map.inputs["Value"])
        stain_mix = nodes.new("ShaderNodeMixRGB")
        stain_mix.blend_type = "MULTIPLY"
        stain_mix.inputs["Color2"].default_value = (0.55, 0.52, 0.50, 1.0)
        links.new(tex.outputs["Color"], bright.inputs["Color1"])
        links.new(bright.outputs["Color"], warm.inputs["Color1"])
        links.new(warm.outputs["Color"], stain_mix.inputs["Color1"])
        stain_fac = nodes.new("ShaderNodeMath")
        stain_fac.operation = "MULTIPLY"
        stain_fac.inputs[1].default_value = 0.28
        links.new(stain_map.outputs["Result"], stain_fac.inputs[0])
        links.new(stain_fac.outputs["Value"], stain_mix.inputs["Fac"])
        links.new(stain_mix.outputs["Color"], bsdf.inputs["Base Color"])
        bsdf.inputs["Roughness"].default_value = 0.84
    elif role == "floor_stain":
        mat.blend_method = "BLEND"
        if hasattr(mat, "shadow_method"):
            mat.shadow_method = "HASHED"
        coord = nodes.new("ShaderNodeTexCoord")
        mapping = nodes.new("ShaderNodeMapping")
        mapping.inputs["Scale"].default_value = (0.95, 0.95, 0.95)
        links.new(coord.outputs["Object"], mapping.inputs["Vector"])
        noise = nodes.new("ShaderNodeTexNoise")
        noise.inputs["Scale"].default_value = 6.5
        noise.inputs["Detail"].default_value = 5.0
        sep = nodes.new("ShaderNodeSeparateXYZ")
        links.new(mapping.outputs["Vector"], sep.inputs["Vector"])
        dx = nodes.new("ShaderNodeMath")
        dx.operation = "MULTIPLY"
        dx.inputs[1].default_value = 2.2
        links.new(sep.outputs["X"], dx.inputs[0])
        dy = nodes.new("ShaderNodeMath")
        dy.operation = "MULTIPLY"
        dy.inputs[1].default_value = 2.2
        links.new(sep.outputs["Y"], dy.inputs[0])
        radial = nodes.new("ShaderNodeMath")
        radial.operation = "SQRT"
        add = nodes.new("ShaderNodeMath")
        add.operation = "ADD"
        sqx = nodes.new("ShaderNodeMath")
        sqx.operation = "MULTIPLY"
        sqy = nodes.new("ShaderNodeMath")
        sqy.operation = "MULTIPLY"
        links.new(dx.outputs["Value"], sqx.inputs[0])
        links.new(dx.outputs["Value"], sqx.inputs[1])
        links.new(dy.outputs["Value"], sqy.inputs[0])
        links.new(dy.outputs["Value"], sqy.inputs[1])
        links.new(sqx.outputs["Value"], add.inputs[0])
        links.new(sqy.outputs["Value"], add.inputs[1])
        links.new(add.outputs["Value"], radial.inputs[0])
        ramp = nodes.new("ShaderNodeMapRange")
        ramp.inputs["From Min"].default_value = 0.18
        ramp.inputs["From Max"].default_value = 0.58
        ramp.inputs["To Min"].default_value = 1.0
        ramp.inputs["To Max"].default_value = 0.0
        links.new(radial.outputs["Value"], ramp.inputs["Value"])
        edge = nodes.new("ShaderNodeMath")
        edge.operation = "MULTIPLY"
        links.new(ramp.outputs["Result"], edge.inputs[0])
        links.new(noise.outputs["Fac"], edge.inputs[1])
        base_dark = (0.28, 0.26, 0.24, 1.0)
        oil = nodes.new("ShaderNodeMixRGB")
        oil.blend_type = "MIX"
        oil.inputs["Color1"].default_value = base_dark
        oil.inputs["Color2"].default_value = (0.26, 0.24, 0.22, 1.0)
        oil_fac = nodes.new("ShaderNodeMath")
        oil_fac.operation = "MULTIPLY"
        oil_fac.inputs[1].default_value = 1.0
        links.new(edge.outputs["Value"], oil_fac.inputs[0])
        links.new(noise.outputs["Fac"], oil_fac.inputs[1])
        links.new(oil_fac.outputs["Value"], oil.inputs["Fac"])
        smear = nodes.new("ShaderNodeMixRGB")
        smear.blend_type = "OVERLAY"
        smear.inputs["Fac"].default_value = 0.28
        smear.inputs["Color1"].default_value = base_dark
        smear.inputs["Color2"].default_value = (0.20, 0.19, 0.17, 1.0)
        links.new(noise.outputs["Fac"], smear.inputs["Fac"])
        links.new(oil.outputs["Color"], smear.inputs["Color1"])
        links.new(smear.outputs["Color"], bsdf.inputs["Base Color"])
        bsdf.inputs["Roughness"].default_value = 0.96
        if "Specular IOR Level" in bsdf.inputs:
            bsdf.inputs["Specular IOR Level"].default_value = 0.12
        alpha_rad = nodes.new("ShaderNodeMapRange")
        alpha_rad.inputs["From Min"].default_value = 0.08
        alpha_rad.inputs["From Max"].default_value = 0.52
        alpha_rad.inputs["To Min"].default_value = 0.88
        alpha_rad.inputs["To Max"].default_value = 0.02
        links.new(radial.outputs["Value"], alpha_rad.inputs["Value"])
        alpha_noise = nodes.new("ShaderNodeMath")
        alpha_noise.operation = "MULTIPLY"
        links.new(alpha_rad.outputs["Result"], alpha_noise.inputs[0])
        links.new(noise.outputs["Fac"], alpha_noise.inputs[1])
        alpha_floor = nodes.new("ShaderNodeMath")
        alpha_floor.operation = "MAXIMUM"
        alpha_floor.inputs[1].default_value = 0.52
        links.new(alpha_noise.outputs["Value"], alpha_floor.inputs[0])
        links.new(alpha_floor.outputs["Value"], bsdf.inputs["Alpha"])
        links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    elif role == "roll_door":
        bsdf.inputs["Base Color"].default_value = (0.78, 0.80, 0.84, 1.0)
        bsdf.inputs["Roughness"].default_value = 0.38
        bsdf.inputs["Metallic"].default_value = 0.42
        bands = nodes.new("ShaderNodeTexWave")
        bands.wave_type = "BANDS"
        bands.inputs["Scale"].default_value = 28.0
        bands.inputs["Distortion"].default_value = 4.0
        grooves = nodes.new("ShaderNodeMixRGB")
        grooves.blend_type = "MIX"
        grooves.inputs["Color1"].default_value = (0.78, 0.80, 0.84, 1.0)
        grooves.inputs["Color2"].default_value = (0.42, 0.44, 0.48, 1.0)
        links.new(bands.outputs["Color"], grooves.inputs["Fac"])
        links.new(grooves.outputs["Color"], bsdf.inputs["Base Color"])
        emit = nodes.new("ShaderNodeEmission")
        emit.inputs["Color"].default_value = (0.72, 0.74, 0.78, 1.0)
        emit.inputs["Strength"].default_value = 0.08
        add = nodes.new("ShaderNodeAddShader")
        links.new(bsdf.outputs["BSDF"], add.inputs[0])
        links.new(emit.outputs["Emission"], add.inputs[1])
        links.new(add.outputs["Shader"], out.inputs["Surface"])
    elif role in ("box", "box_open"):
        card = (0.90, 0.70, 0.44, 1.0)
        bsdf.inputs["Base Color"].default_value = card
        bsdf.inputs["Roughness"].default_value = 0.76
        wr = nodes.new("ShaderNodeTexNoise")
        wr.inputs["Scale"].default_value = 14.0
        wr.inputs["Detail"].default_value = 5.0
        mix = nodes.new("ShaderNodeMixRGB")
        mix.blend_type = "OVERLAY"
        mix.inputs["Fac"].default_value = 0.42
        mix.inputs["Color1"].default_value = card
        mix.inputs["Color2"].default_value = (0.82, 0.62, 0.38, 1.0)
        links.new(wr.outputs["Fac"], mix.inputs["Fac"])
        links.new(mix.outputs["Color"], bsdf.inputs["Base Color"])
        emit = nodes.new("ShaderNodeEmission")
        emit.inputs["Color"].default_value = card
        emit.inputs["Strength"].default_value = 0.14
        add = nodes.new("ShaderNodeAddShader")
        links.new(bsdf.outputs["BSDF"], add.inputs[0])
        links.new(emit.outputs["Emission"], add.inputs[1])
        links.new(add.outputs["Shader"], out.inputs["Surface"])
    elif role == "chair":
        coord = nodes.new("ShaderNodeTexCoord")
        sep = nodes.new("ShaderNodeSeparateXYZ")
        links.new(coord.outputs["Object"], sep.inputs["Vector"])
        leg_mask = nodes.new("ShaderNodeMath")
        leg_mask.operation = "LESS_THAN"
        leg_mask.inputs[1].default_value = 0.22
        seat_mask = nodes.new("ShaderNodeMath")
        seat_mask.operation = "LESS_THAN"
        seat_mask.inputs[1].default_value = 0.42
        fabric = nodes.new("ShaderNodeTexNoise")
        fabric.inputs["Scale"].default_value = 18.0
        seat_col = nodes.new("ShaderNodeMixRGB")
        seat_col.inputs["Color1"].default_value = (0.34, 0.56, 0.88, 1.0)
        seat_col.inputs["Color2"].default_value = (0.26, 0.44, 0.72, 1.0)
        links.new(fabric.outputs["Fac"], seat_col.inputs["Fac"])
        back_wave = nodes.new("ShaderNodeTexWave")
        back_wave.wave_type = "BANDS"
        back_wave.inputs["Scale"].default_value = 14.0
        back_col = nodes.new("ShaderNodeMixRGB")
        back_col.inputs["Color1"].default_value = (0.28, 0.48, 0.82, 1.0)
        back_col.inputs["Color2"].default_value = (0.20, 0.34, 0.62, 1.0)
        links.new(back_wave.outputs["Color"], back_col.inputs["Fac"])
        leg_col = nodes.new("ShaderNodeMixRGB")
        leg_col.inputs["Color1"].default_value = (0.48, 0.50, 0.54, 1.0)
        leg_col.inputs["Color2"].default_value = (0.32, 0.34, 0.38, 1.0)
        links.new(fabric.outputs["Fac"], leg_col.inputs["Fac"])
        stage1 = nodes.new("ShaderNodeMixRGB")
        links.new(seat_mask.outputs["Value"], stage1.inputs["Fac"])
        links.new(leg_col.outputs["Color"], stage1.inputs["Color1"])
        links.new(seat_col.outputs["Color"], stage1.inputs["Color2"])
        final = nodes.new("ShaderNodeMixRGB")
        links.new(leg_mask.outputs["Value"], final.inputs["Fac"])
        links.new(stage1.outputs["Color"], final.inputs["Color1"])
        links.new(back_col.outputs["Color"], final.inputs["Color2"])
        links.new(final.outputs["Color"], bsdf.inputs["Base Color"])
        bsdf.inputs["Roughness"].default_value = 0.52
        bump = nodes.new("ShaderNodeBump")
        bump.inputs["Strength"].default_value = 0.18
        links.new(fabric.outputs["Fac"], bump.inputs["Height"])
        links.new(bump.outputs["Normal"], bsdf.inputs["Normal"])
    elif role == "lamp_accent":
        coord = nodes.new("ShaderNodeTexCoord")
        sep = nodes.new("ShaderNodeSeparateXYZ")
        links.new(coord.outputs["Object"], sep.inputs["Vector"])
        shade_mask = nodes.new("ShaderNodeMath")
        shade_mask.operation = "GREATER_THAN"
        shade_mask.inputs[1].default_value = 0.55
        metal = nodes.new("ShaderNodeTexNoise")
        metal.inputs["Scale"].default_value = 22.0
        arm_col = nodes.new("ShaderNodeMixRGB")
        arm_col.inputs["Color1"].default_value = (0.62, 0.64, 0.68, 1.0)
        arm_col.inputs["Color2"].default_value = (0.42, 0.44, 0.48, 1.0)
        links.new(metal.outputs["Fac"], arm_col.inputs["Fac"])
        shade_col = nodes.new("ShaderNodeMixRGB")
        shade_col.inputs["Color1"].default_value = (0.96, 0.86, 0.62, 1.0)
        shade_col.inputs["Color2"].default_value = (0.88, 0.72, 0.38, 1.0)
        links.new(metal.outputs["Fac"], shade_col.inputs["Fac"])
        lamp_mix = nodes.new("ShaderNodeMixRGB")
        links.new(shade_mask.outputs["Value"], lamp_mix.inputs["Fac"])
        links.new(arm_col.outputs["Color"], lamp_mix.inputs["Color1"])
        links.new(shade_col.outputs["Color"], lamp_mix.inputs["Color2"])
        links.new(lamp_mix.outputs["Color"], bsdf.inputs["Base Color"])
        bsdf.inputs["Roughness"].default_value = 0.35
        emit = nodes.new("ShaderNodeEmission")
        emit_st = nodes.new("ShaderNodeMath")
        emit_st.operation = "MULTIPLY"
        emit_st.inputs[1].default_value = 0.35
        links.new(shade_mask.outputs["Value"], emit_st.inputs[0])
        emit.inputs["Color"].default_value = (1.0, 0.82, 0.42, 1.0)
        links.new(emit_st.outputs["Value"], emit.inputs["Strength"])
        add = nodes.new("ShaderNodeAddShader")
        links.new(bsdf.outputs["BSDF"], add.inputs[0])
        links.new(emit.outputs["Emission"], add.inputs[1])
        links.new(add.outputs["Shader"], out.inputs["Surface"])
    else:
        bsdf.inputs["Base Color"].default_value = tint
        bsdf.inputs["Roughness"].default_value = 0.62 if role not in ("metal", "glass", "wood") else 0.48
    if role in ("metal",):
        bsdf.inputs["Metallic"].default_value = 0.55
    if role == "wood":
        bsdf.inputs["Roughness"].default_value = 0.68
    if role == "glass":
        bsdf.inputs["Roughness"].default_value = 0.08
        if "Transmission Weight" in bsdf.inputs:
            bsdf.inputs["Transmission Weight"].default_value = 0.35
        elif "Transmission" in bsdf.inputs:
            bsdf.inputs["Transmission"].default_value = 0.35
    if role == "screen":
        emit = nodes.new("ShaderNodeEmission")
        emit.inputs["Color"].default_value = tint
        emit.inputs["Strength"].default_value = 0.12
        add = nodes.new("ShaderNodeAddShader")
        links.new(bsdf.outputs["BSDF"], add.inputs[0])
        links.new(emit.outputs["Emission"], add.inputs[1])
        links.new(add.outputs["Shader"], out.inputs["Surface"])
    elif role not in ("lamp_accent", "box", "box_open", "roll_door"):
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


def setup_hdri_world(strength: float = 0.95) -> None:
    setup_preview_world_grey_hdri(hdri_strength=strength)


def setup_preview_world_grey_hdri(*, hdri_strength: float = 1.05) -> None:
    """Camera sees flat RGB 150; HDRI lights non-camera rays only."""
    world = bpy.data.worlds.new("cc0_preview_world")
    bpy.context.scene.world = world
    world.use_nodes = True
    nodes = world.node_tree.nodes
    links = world.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputWorld")
    grey_bg = nodes.new("ShaderNodeBackground")
    grey_bg.inputs["Color"].default_value = (BACKDROP_GREY, BACKDROP_GREY, BACKDROP_GREY, 1)
    grey_bg.inputs["Strength"].default_value = 1.0
    light_path = nodes.new("ShaderNodeLightPath")
    mix = nodes.new("ShaderNodeMixShader")
    links.new(light_path.outputs["Is Camera Ray"], mix.inputs["Fac"])
    if HDRI_PATH.is_file():
        env = nodes.new("ShaderNodeTexEnvironment")
        env.image = load_image(HDRI_PATH)
        mapping = nodes.new("ShaderNodeMapping")
        texcoord = nodes.new("ShaderNodeTexCoord")
        mapping.inputs["Rotation"].default_value[2] = math.radians(215)
        links.new(texcoord.outputs["Generated"], mapping.inputs["Vector"])
        links.new(mapping.outputs["Vector"], env.inputs["Vector"])
        hdri_bg = nodes.new("ShaderNodeBackground")
        links.new(env.outputs["Color"], hdri_bg.inputs["Color"])
        hdri_bg.inputs["Strength"].default_value = hdri_strength
        links.new(hdri_bg.outputs["Background"], mix.inputs[1])
    else:
        amb = nodes.new("ShaderNodeBackground")
        amb.inputs["Color"].default_value = (0.72, 0.70, 0.68, 1)
        amb.inputs["Strength"].default_value = 0.6
        links.new(amb.outputs["Background"], mix.inputs[1])
    # Fac=1 (camera rays) → grey; Fac=0 → HDRI lights the scene
    links.new(grey_bg.outputs["Background"], mix.inputs[2])
    links.new(mix.outputs["Shader"], out.inputs["Surface"])


def add_preview_warm_lights_z(room_center: Vector) -> list[bpy.types.Object]:
    lights: list[bpy.types.Object] = []
    bpy.ops.object.light_add(type="POINT", location=(room_center.x, room_center.y, room_center.z + 2.0))
    key = bpy.context.active_object
    key.name = "preview_warm_key"
    key.data.color = (1.0, 0.82, 0.66)
    key.data.energy = 120.0
    key.data.shadow_soft_size = 0.38
    lights.append(key)
    bpy.ops.object.light_add(type="AREA", location=(room_center.x + 1.0, room_center.y - 0.6, room_center.z + 1.8))
    fill = bpy.context.active_object
    fill.name = "preview_warm_fill"
    fill.data.color = (1.0, 0.90, 0.78)
    fill.data.energy = 55.0
    fill.data.size = 1.6
    lights.append(fill)
    return lights


def add_preview_warm_lights(room_center: Vector) -> list[bpy.types.Object]:
    lights: list[bpy.types.Object] = []
    bpy.ops.object.light_add(type="POINT", location=(room_center.x, room_center.y + 1.6, room_center.z))
    key = bpy.context.active_object
    key.name = "preview_warm_key"
    key.data.color = (1.0, 0.82, 0.66)
    key.data.energy = 95.0
    key.data.shadow_soft_size = 0.42
    lights.append(key)
    bpy.ops.object.light_add(type="AREA", location=(room_center.x + 1.2, 1.8, room_center.z + 0.8))
    fill = bpy.context.active_object
    fill.name = "preview_warm_fill"
    fill.data.color = (1.0, 0.90, 0.78)
    fill.data.energy = 40.0
    fill.data.size = 1.4
    lights.append(fill)
    return lights


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
    span_h = max(maxs.x - mins.x, maxs.z - mins.z, 0.1)
    span_v = max(maxs.y - mins.y, 0.1)
    span = max(span_h, span_v * 0.55)
    return center, span


def _camera_distance_for_fill(
    span: float,
    lens_mm: float,
    fill: float,
    margin: float,
    *,
    min_half: float = 0.75,
) -> float:
    """Perspective distance so subject span fills ~fill of frame with ~margin border."""
    sensor = 36.0
    fov = 2.0 * math.atan(sensor / (2.0 * lens_mm))
    effective_fill = max(0.45, min(0.85, fill * (1.0 - 2.0 * margin)))
    half = max(span * 0.5, min_half)
    return half / (math.tan(fov * 0.5) * effective_fill)


def render_kit_preview(
    objects: list[bpy.types.Object],
    path: Path,
    *,
    resolution: int = 768,
    mode: str = "scene",
    fill: float = 0.70,
    margin: float = 0.05,
    frame_span: float | None = None,
    frame_objects: list[bpy.types.Object] | None = None,
    up_axis: str = "Y",
    mask_path: Path | None = None,
    hero_yaw_deg: float | None = None,
    hero_pitch_deg: float | None = None,
) -> None:
    """Warm point + HDRI (hidden from camera); world corners RGB 150; ~70% fill."""
    PREVIEW_ROOT.mkdir(parents=True, exist_ok=True)
    setup_preview_world_grey_hdri(hdri_strength=0.72)
    bpy.context.scene.render.engine = "CYCLES"
    bpy.context.scene.cycles.samples = 80 if mode == "hero" else 96
    bpy.context.scene.cycles.use_denoising = False
    bpy.context.scene.render.film_transparent = False
    bpy.context.scene.view_settings.view_transform = "Standard"
    bpy.context.scene.view_settings.look = "None"
    bpy.context.scene.view_settings.exposure = 0.10 if mode == "scene" else 0.0
    if hasattr(bpy.context.scene, "cycles"):
        bpy.context.scene.cycles.film_exposure = 1.0

    frame_src = frame_objects or objects
    center, span3 = _bounds_center(frame_src)
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in frame_src:
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins.x = min(mins.x, w.x)
            mins.y = min(mins.y, w.y)
            mins.z = min(mins.z, w.z)
            maxs.x = max(maxs.x, w.x)
            maxs.y = max(maxs.y, w.y)
            maxs.z = max(maxs.z, w.z)
    span_z = max(maxs.z - mins.z, 0.1)
    if up_axis == "Z":
        span_xy = max(maxs.x - mins.x, maxs.y - mins.y, 0.1)
        if mode == "hero":
            span = max(span_xy, span_z * 0.95, 0.18)
        else:
            span = max(span_xy, 3.6)
    else:
        span_xz = max(maxs.x - mins.x, maxs.z - mins.z, 0.1)
        span_y = max(maxs.y - mins.y, 0.1)
        if mode == "hero":
            span = max(span_xz, span_y * 0.95, 0.18)
        else:
            span = max(span_xz, 3.6)
    if frame_span is not None:
        span = frame_span

    if up_axis == "Z":
        wall_top = maxs.z
    else:
        wall_top = maxs.y
    room = span if span <= 5.5 else max(span, 4.0)
    floor_h = 0.05
    lens = 48.0
    default_pitch = 35.0 if (mode == "hero" and up_axis == "Y") else (34.0 if mode == "scene" and up_axis == "Y" else 32.0)
    pitch_deg = hero_pitch_deg if hero_pitch_deg is not None else default_pitch
    pitch = math.radians(pitch_deg)
    hero_tight = mode == "hero"
    min_half = 0.06 if hero_tight else 0.75
    if mode == "scene" and up_axis == "Y":
        dist_scale = 0.90
    else:
        dist_scale = 1.0 if hero_tight else (0.88 if mode == "hero" else 0.62)
    subject_span = span if hero_tight else room + 0.55
    dist = _camera_distance_for_fill(
        subject_span, lens, fill, margin, min_half=min_half
    ) * dist_scale
    if not hero_tight:
        dist = max(dist, room * 0.85)
    horiz = dist * math.cos(pitch)
    elev = dist * math.sin(pitch)

    shadow: bpy.types.Object | None = None
    if up_axis == "Z":
        if mode == "hero":
            span_z = max(maxs.z - mins.z, 0.1)
            target = Vector((center.x, center.y, mins.z + span_z * 0.44))
            lights = add_preview_warm_lights_z(target)
            yaw = math.radians(36.0)
            cam_loc = Vector(
                (
                    center.x + math.cos(yaw) * horiz * 0.92,
                    center.y + math.sin(yaw) * horiz * 0.92,
                    mins.z + span_z * 0.32 + elev * 0.78,
                )
            )
        else:
            target = Vector((mins.x + span * 0.48, mins.y + span * 0.48, floor_h + wall_top * 0.42))
            lights = add_preview_warm_lights_z(target)
            cam_loc = Vector(
                (
                    maxs.x + horiz * 0.58,
                    maxs.y + horiz * 0.58,
                    floor_h + wall_top * 0.28 + elev * 0.68,
                )
            )
        bpy.context.scene.cycles.use_shadow_catcher = False
    else:
        span_y = max(maxs.y - mins.y, 0.1)
        yaw_deg = hero_yaw_deg if hero_yaw_deg is not None else (42.0 if mode == "scene" else 45.0)
        yaw_rad = math.radians(yaw_deg)
        if mode == "hero":
            target = Vector((center.x, mins.y + span_y * 0.40, center.z))
            lights = add_preview_warm_lights(target)
            cam_loc = Vector(
                (
                    center.x + math.cos(yaw_rad) * horiz,
                    mins.y + span_y * 0.36 + elev * 0.70,
                    center.z + math.sin(yaw_rad) * horiz,
                )
            )
            bpy.context.scene.cycles.use_shadow_catcher = False
        elif mode == "scene":
            scene_pitch = math.radians(hero_pitch_deg if hero_pitch_deg is not None else 34.0)
            sh = dist * math.cos(scene_pitch)
            se = dist * math.sin(scene_pitch)
            target = Vector((center.x, mins.y + span_y * 0.44, center.z))
            lights = add_preview_warm_lights(target)
            for light in lights:
                light.data.energy *= 1.28
            cam_loc = Vector(
                (
                    center.x + math.cos(yaw_rad) * sh * 0.95,
                    mins.y + span_y * 0.28 + se * 0.72,
                    center.z + math.sin(yaw_rad) * sh * 0.95,
                )
            )
        else:
            target = Vector((center.x, max(center.y, floor_h + 0.85), center.z))
            lights = add_preview_warm_lights(target)
            cam_loc = Vector((room + horiz * 0.58, target.y + elev * 0.55, room + horiz * 0.58))
        if mode == "scene":
            bpy.ops.mesh.primitive_plane_add(
                size=max(span_xz, 1.2) * 1.08,
                location=(center.x, mins.y - 0.002, center.z),
            )
            shadow = bpy.context.active_object
            shadow.name = "preview_shadow_catcher"
            smat = bpy.data.materials.new("shadow_grey")
            smat.use_nodes = True
            sg = smat.node_tree.nodes.get("Principled BSDF")
            sg.inputs["Base Color"].default_value = (BACKDROP_GREY, BACKDROP_GREY, BACKDROP_GREY, 1)
            sg.inputs["Roughness"].default_value = 0.96
            shadow.data.materials.append(smat)
            shadow.is_shadow_catcher = True
            bpy.context.scene.cycles.use_shadow_catcher = True
        elif mode != "hero":
            bpy.ops.mesh.primitive_plane_add(size=room * 1.05, location=(room / 2, -0.002, room / 2))
            shadow = bpy.context.active_object
            shadow.name = "preview_shadow_catcher"
            smat = bpy.data.materials.new("shadow_grey")
            smat.use_nodes = True
            sg = smat.node_tree.nodes.get("Principled BSDF")
            sg.inputs["Base Color"].default_value = (BACKDROP_GREY, BACKDROP_GREY, BACKDROP_GREY, 1)
            sg.inputs["Roughness"].default_value = 0.96
            shadow.data.materials.append(smat)
            shadow.is_shadow_catcher = True
            bpy.context.scene.cycles.use_shadow_catcher = True
        else:
            shadow = None
            bpy.context.scene.cycles.use_shadow_catcher = False
    bpy.ops.object.camera_add(location=cam_loc)
    cam = bpy.context.active_object
    cam.data.lens = lens
    cam.data.clip_end = dist * 6.0
    direction = target - cam.location
    cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    bpy.context.scene.camera = cam

    bpy.context.scene.render.resolution_x = resolution
    bpy.context.scene.render.resolution_y = resolution
    bpy.context.scene.render.image_settings.file_format = "PNG"
    bpy.context.scene.render.filepath = str(path)
    bpy.ops.render.render(write_still=True)

    if mask_path is not None:
        import blender_preview_rig as rig  # type: ignore

        rig.render_hero_mask(objects, mask_path, resolution)

    if shadow and bpy.data.objects.get(shadow.name):
        bpy.data.objects.remove(shadow, do_unlink=True)
    for ob in lights:
        bpy.data.objects.remove(ob, do_unlink=True)
    bpy.data.objects.remove(cam, do_unlink=True)
    _quantize_backdrop_rgb150(path)


def _set_render_double_sided(objects: list[bpy.types.Object]) -> None:
    for ob in objects:
        if ob.type != "MESH" or not ob.data:
            continue
        for slot in ob.data.materials:
            if slot:
                slot.use_backface_culling = False


def render_ortho_wall_check(
    objects: list[bpy.types.Object],
    path: Path,
    *,
    view: str,
    room: float = 4.0,
    wall_h: float = 2.7,
    resolution: int = 512,
    hide_render_names: frozenset[str] | None = None,
    interior_slice: bool = False,
) -> None:
    """Flat grey world; orthographic wall QA (top or front-left room slice)."""
    PREVIEW_ROOT.mkdir(parents=True, exist_ok=True)
    setup_preview_world_grey_hdri(hdri_strength=0.55)
    bpy.context.scene.render.engine = "CYCLES"
    bpy.context.scene.cycles.samples = 72
    bpy.context.scene.cycles.use_denoising = False
    bpy.context.scene.render.film_transparent = False
    hidden: list[tuple[bpy.types.Object, bool]] = []
    hide_set = hide_render_names or frozenset()
    for ob in objects:
        if ob.name in hide_set:
            hidden.append((ob, ob.hide_render))
            ob.hide_render = True
    render_obs = [o for o in objects if not o.hide_render]
    _set_render_double_sided(render_obs)
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in render_obs:
        mn, mx = world_bbox(ob)
        mins.x = min(mins.x, mn.x)
        mins.y = min(mins.y, mn.y)
        mins.z = min(mins.z, mn.z)
        maxs.x = max(maxs.x, mx.x)
        maxs.y = max(maxs.y, mx.y)
        maxs.z = max(maxs.z, mx.z)
    center = (mins + maxs) * 0.5
    lights = add_preview_warm_lights_z(Vector((center.x, center.y, maxs.z * 0.55)))
    span_x = max(maxs.x - mins.x, 0.05)
    span_y = max(maxs.y - mins.y, 0.05)
    span_z = max(maxs.z - mins.z, 0.05)
    bpy.ops.object.camera_add()
    cam = bpy.context.active_object
    cam.data.type = "ORTHO"
    cam.data.clip_start = 0.01
    cam.data.clip_end = room * 4.0
    if view == "top":
        cam.data.ortho_scale = max(span_x, span_y) * 1.12
        cam.location = Vector((center.x, center.y, maxs.z + max(span_x, span_y) * 1.6))
        cam.rotation_euler = (0.0, 0.0, 0.0)
    elif view == "front_left":
        cam.data.ortho_scale = max(span_y, span_z) * (0.58 if interior_slice else 1.08)
        if interior_slice:
            cam.location = Vector((0.42, center.y + 0.12, center.z + 0.05))
        else:
            cam.location = Vector((mins.x - max(span_x, 1.0) * 1.2, center.y, center.z))
        direction = Vector((1.0, 0.0, 0.0))
        cam.rotation_euler = direction.to_track_quat("-Z", "Y").to_euler()
    else:
        raise ValueError(view)
    bpy.context.scene.camera = cam
    bpy.context.scene.render.resolution_x = resolution
    bpy.context.scene.render.resolution_y = resolution
    bpy.context.scene.render.image_settings.file_format = "PNG"
    bpy.context.scene.render.filepath = str(path)
    bpy.ops.render.render(write_still=True)
    hull = _ortho_project_hull(render_obs, cam, resolution, resolution)
    gate_path = Path(f"{path}.gate.json")
    gate_path.write_text(
        json.dumps({"w": resolution, "h": resolution, "hull": hull}, indent=2) + "\n",
        encoding="utf-8",
    )
    bpy.data.objects.remove(cam, do_unlink=True)
    for ob in lights:
        bpy.data.objects.remove(ob, do_unlink=True)
    for ob, prev in hidden:
        ob.hide_render = prev
    _quantize_backdrop_rgb150(path)


def build_stain_decal_material(name: str = "stain_decal") -> bpy.types.Material:
    """Radial alpha stain (soft edge, no hard quad corners in render)."""
    size = 256
    buf: list[float] = []
    for j in range(size):
        for i in range(size):
            u = (i / (size - 1) - 0.5) * 2.0
            v = (j / (size - 1) - 0.5) * 2.0
            r = math.sqrt(u * u + 0.82 * v * v)
            alpha = max(0.0, min(1.0, (0.86 - r) / 0.36))
            alpha *= max(0.0, min(1.0, (0.58 - abs(u)) / 0.32)) * max(0.0, min(1.0, (0.58 - abs(v)) / 0.32))
            a = min(1.0, alpha**0.92 + 0.48)
            buf.extend((0.07, 0.065, 0.06, a))
    img = bpy.data.images.new(f"{name}_tex", size, size, alpha=True)
    img.colorspace_settings.name = "sRGB"
    img.pixels = buf
    mat = bpy.data.materials.new(name)
    mat.blend_method = "BLEND"
    if hasattr(mat, "shadow_method"):
        mat.shadow_method = "HASHED"
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    nodes.clear()
    out = nodes.new("ShaderNodeOutputMaterial")
    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.inputs["Roughness"].default_value = 0.95
    tex = nodes.new("ShaderNodeTexImage")
    tex.image = img
    links.new(tex.outputs["Color"], bsdf.inputs["Base Color"])
    links.new(tex.outputs["Alpha"], bsdf.inputs["Alpha"])
    links.new(bsdf.outputs["BSDF"], out.inputs["Surface"])
    return mat


def _ortho_project_hull(
    objects: list[bpy.types.Object],
    cam: bpy.types.Object,
    w: int,
    h: int,
) -> list[list[int]]:
    import bpy_extras.object_utils as object_utils

    scene = bpy.context.scene
    pts: list[tuple[float, float]] = []
    for ob in objects:
        if ob.type != "MESH" or not ob.data:
            continue
        for corner in ob.bound_box:
            wco = ob.matrix_world @ Vector(corner)
            co = object_utils.world_to_camera_view(scene, cam, wco)
            if -0.08 <= co.x <= 1.08 and -0.08 <= co.y <= 1.08:
                pts.append((co.x * w, (1.0 - co.y) * h))
    if len(pts) < 3:
        return []
    pts.sort(key=lambda p: (p[0], p[1]))

    def cross(o, a, b):
        return (a[0] - o[0]) * (b[1] - o[1]) - (a[1] - o[1]) * (b[0] - o[0])

    lower: list[tuple[float, float]] = []
    for p in pts:
        while len(lower) >= 2 and cross(lower[-2], lower[-1], p) <= 0:
            lower.pop()
        lower.append(p)
    upper: list[tuple[float, float]] = []
    for p in reversed(pts):
        while len(upper) >= 2 and cross(upper[-2], upper[-1], p) <= 0:
            upper.pop()
        upper.append(p)
    hull = lower[:-1] + upper[:-1]
    return [[int(round(x)), int(round(y))] for x, y in hull]


def _quantize_backdrop_rgb150(path: Path) -> None:
    """Force every studio-backdrop pixel to exact RGB 150 (no edge vignette)."""
    try:
        from PIL import Image
    except ImportError:
        return
    from collections import deque

    def sat(rgb):
        r, g, b = (c / 255.0 for c in rgb)
        mx, mn = max(r, g, b), min(r, g, b)
        return 0.0 if mx <= 1e-6 else (mx - mn) / mx

    def is_backdrop_pixel(rgb: tuple[int, int, int]) -> bool:
        if rgb == CORNER_RGB:
            return True
        L = 0.299 * rgb[0] + 0.587 * rgb[1] + 0.114 * rgb[2]
        return sat(rgb) < 0.10 and 125 <= L <= 156

    im = Image.open(path).convert("RGB")
    w, h = im.size
    bg = [[False] * w for _ in range(h)]
    q: deque[tuple[int, int]] = deque()
    for x in range(w):
        for y in (0, h - 1):
            if is_backdrop_pixel(im.getpixel((x, y))):
                bg[y][x] = True
                q.append((x, y))
    for y in range(h):
        for x in (0, w - 1):
            if is_backdrop_pixel(im.getpixel((x, y))):
                if not bg[y][x]:
                    bg[y][x] = True
                    q.append((x, y))
    for y in range(min(int(h * 0.08), h)):
        for x in range(w):
            if is_backdrop_pixel(im.getpixel((x, y))) and not bg[y][x]:
                bg[y][x] = True
                q.append((x, y))
    while q:
        x, y = q.popleft()
        for nx, ny in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)):
            if 0 <= nx < w and 0 <= ny < h and not bg[ny][nx]:
                if is_backdrop_pixel(im.getpixel((nx, ny))):
                    bg[ny][nx] = True
                    q.append((nx, ny))
    for y in range(h):
        for x in range(w):
            if bg[y][x]:
                im.putpixel((x, y), CORNER_RGB)
    bx = max(1, int(w * 0.03))
    by = max(1, int(h * 0.03))
    for y in range(h):
        for x in range(w):
            if x < bx or x >= w - bx or y < by or y >= h - by:
                im.putpixel((x, y), CORNER_RGB)
    im.save(path)


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