"""Two Point Hospital–style PC tower from CC0 Quaternius + Kenney factory accents."""
from __future__ import annotations

import bpy
from mathutils import Vector

import cc0_common as cc
import mesh_builders as mb

# Target world size (m): narrow width, shallower depth, height dominant.
TARGET_W = 0.20
TARGET_D = 0.34
TARGET_H = 0.52
MIN_TRIS = 800


def _world_bbox(objs: list[bpy.types.Object]) -> tuple[Vector, Vector]:
    return cc._world_bbox(objs)


def _scale_fit(ob: bpy.types.Object) -> None:
    mn, mx = _world_bbox([ob])
    size = mx - mn
    s = min(TARGET_W / max(size.x, 1e-6), TARGET_D / max(size.y, 1e-6), TARGET_H / max(size.z, 1e-6))
    s *= 0.97
    ob.scale = (s, s, s)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(scale=True)
    mn, mx = _world_bbox([ob])
    center = (mn + mx) / 2
    ob.location -= Vector((center.x, center.y, mn.z))
    bpy.context.view_layer.update()


def _import_factory_piece(name: str) -> bpy.types.Object:
    obs = cc.import_glb(cc.factory_glb(name))
    for o in obs:
        cc.apply_transforms(o)
    return cc.join_meshes(obs) if len(obs) > 1 else obs[0]


def _place_accent(
    name: str,
    location: Vector,
    scale: tuple[float, float, float],
    rotation: tuple[float, float, float] = (0.0, 0.0, 0.0),
) -> bpy.types.Object:
    ob = _import_factory_piece(name)
    ob.location = location
    ob.scale = scale
    ob.rotation_euler = rotation
    cc.apply_transforms(ob)
    return ob


def _import_quaternius_tower() -> bpy.types.Object:
    gltf = (
        cc.INCOMING
        / "quaternius-scifi-megakit/Modular SciFi MegaKit[Standard]/glTF/Props/Prop_Computer.gltf"
    )
    if gltf.is_file():
        obs = cc.import_glb(gltf)
    else:
        obs = cc.import_obj(cc.quaternius_obj("Prop_Computer"))
    for o in obs:
        cc.apply_transforms(o)
    root = cc.join_meshes(obs) if len(obs) > 1 else obs[0]
    _scale_fit(root)
    return root


def paint_tp_hospital_tower(ob: bpy.types.Object) -> None:
    """Saturated TP-style vertex bake — fully opaque."""
    mesh = ob.data
    while mesh.color_attributes:
        mesh.color_attributes.remove(mesh.color_attributes[0])
    vcol = mesh.color_attributes.new(name="BakeColor", type="FLOAT_COLOR", domain="CORNER")
    mn, mx = _world_bbox([ob])
    fw = mx.x - mn.x
    fd = mx.y - mn.y
    fh = mx.z - mn.z
    x0, y0, z0 = mn.x, mn.y, mn.z

    chassis = (0.72, 0.74, 0.80)
    chassis_shadow = (0.52, 0.54, 0.60)
    front_trim = (0.95, 0.58, 0.22)
    bay_dark = (0.28, 0.30, 0.36)
    bay_light = (0.42, 0.44, 0.50)
    purple = (0.58, 0.42, 0.96)
    power = (0.98, 0.55, 0.18)
    rgb_cyan = (0.25, 0.92, 0.88)
    rgb_magenta = (0.95, 0.28, 0.72)
    rgb_green = (0.38, 0.95, 0.48)
    fan_glass = (0.35, 0.75, 0.95)

    for poly in mesh.polygons:
        for li in poly.loop_indices:
            co = mesh.vertices[mesh.loops[li].vertex_index].co
            x, y, z = co.x, co.y, co.z
            nx = (x - x0) / max(fw, 1e-6)
            ny = (y - y0) / max(fd, 1e-6)
            nz = (z - z0) / max(fh, 1e-6)
            c = chassis
            if nz < 0.06:
                c = chassis_shadow
            if ny < 0.12:
                c = bay_light if nz > 0.55 else bay_dark
                if 0.62 < nz < 0.78 and 0.35 < nx < 0.65:
                    c = front_trim
                if 0.38 < nz < 0.52 and nx > 0.62:
                    c = power
                if 0.15 < nz < 0.35:
                    c = purple
            if nx > 0.82 and 0.25 < nz < 0.75:
                c = (rgb_cyan, rgb_magenta, rgb_green)[int(nz * 12) % 3]
            if nx > 0.78 and 0.12 < nz < 0.45 and ny > 0.35:
                c = fan_glass
            if nx < 0.10 or ny > 0.92:
                c = chassis_shadow
            vcol.data[li].color = (c[0], c[1], c[2], 1.0)


def _tri_count(ob: bpy.types.Object) -> int:
    return sum(len(p.vertices) for p in ob.data.polygons)


def build_pc_tower_cc0() -> bpy.types.Object:
    pieces: list[bpy.types.Object] = [_import_quaternius_tower()]
    mn, mx = _world_bbox(pieces)
    w, d, h = mx.x - mn.x, mx.y - mn.y, mx.z - mn.z
    front = mn.y

    pieces.append(
        _place_accent(
            "screen-panel-small",
            Vector((0.0, front - 0.008, mn.z + h * 0.74)),
            (w * 2.6, w * 0.45, h * 0.11),
            (1.5708, 0.0, 0.0),
        )
    )
    pieces.append(
        _place_accent(
            "screen-panel-flat",
            Vector((0.0, front - 0.006, mn.z + h * 0.44)),
            (w * 2.2, w * 0.55, h * 0.14),
            (1.5708, 0.0, 0.0),
        )
    )
    pieces.append(
        _place_accent(
            "piston-thin-round",
            Vector((mx.x - w * 0.06, front - 0.004, mn.z + h * 0.26)),
            (0.28, 0.28, 0.28),
        )
    )
    for i, zf in enumerate((0.18, 0.34, 0.50)):
        pieces.append(
            _place_accent(
                "piston-round",
                Vector((mx.x - w * 0.05, mn.y + d * (0.42 + i * 0.12), mn.z + h * zf)),
                (0.32, 0.32, 0.32),
            )
        )

    root = cc.join_meshes(pieces)
    cc.apply_transforms(root)
    _scale_fit(root)
    paint_tp_hospital_tower(root)
    mb.snap_root_to_floor(root)

    tris = _tri_count(root)
    if tris < MIN_TRIS:
        raise RuntimeError(f"tower: {tris} tris (need >={MIN_TRIS})")
    return root