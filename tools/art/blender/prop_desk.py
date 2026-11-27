"""Headless bake: prop-desk — Kenney GLTF per-slot materials (no vertex masks)."""
from __future__ import annotations

import math
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import bpy  # noqa: E402

import _common  # noqa: E402

KENNEY = _common.CC0_ROOT / "kenney-furniture-kit" / "Models" / "GLTF format"
TARGET_WIDTH = 1.14
SURFACE_TOL = 0.012
PROP_GAP_M = 0.018
PLANT_NATIVE_HEIGHT_MAX = 0.55
PLANT_MONITOR_HEIGHT_RATIO_MAX = 1.2
SOURCE_NOTE = (
    "docs/art/cc0/kenney-furniture-kit "
    "(desk, computerScreen, computerKeyboard, computerMouse, pottedPlant); "
    "ambientCG Wood092"
)

KENNEY_OFFSETS = {
    "computerScreen.glb": (0.56, 0.46),
    "computerKeyboard.glb": (0.30, 0.70),
    "computerMouse.glb": (0.62, 0.64),
    "pottedPlant.glb": (0.88, 0.68),
}


def _import_part(filename: str) -> list[bpy.types.Object]:
    path = KENNEY / filename
    if not path.is_file():
        raise FileNotFoundError(path)
    return _common._import_gltf_mesh(path)


def _desktop_surface_z(parts: list[bpy.types.Object]) -> float:
    return float(_common._bbox_world(parts)[1].z)


def _desktop_xy_bounds(parts: list[bpy.types.Object]) -> tuple[float, float, float, float]:
    mn, mx = _common._bbox_world(parts)
    return float(mn.x), float(mx.x), float(mn.y), float(mx.y)


def _place_on_desktop(
    parts: list[bpy.types.Object],
    surface_z: float,
    xy_bounds: tuple[float, float, float, float],
    u: float,
    v: float,
) -> None:
    x0, x1, y0, y1 = xy_bounds
    tx = x0 + (x1 - x0) * u
    ty = y0 + (y1 - y0) * v
    mn, mx = _common._bbox_world(parts)
    cx, cy = (mn.x + mx.x) * 0.5, (mn.y + mx.y) * 0.5
    for o in parts:
        from mathutils import Vector

        o.location += Vector((tx - cx, ty - cy, surface_z - mn.z))
    bpy.context.view_layer.update()


def _gate_peripheral_on_desktop(
    parts: list[bpy.types.Object],
    surface_z: float,
    xy_bounds: tuple[float, float, float, float],
    label: str,
) -> None:
    x0, x1, y0, y1 = xy_bounds
    mn, mx = _common._bbox_world(parts)
    gap = abs(mn.z - surface_z)
    if gap > SURFACE_TOL:
        raise RuntimeError(f"FAIL {label}: base Z gap {gap:.4f}m (surface {surface_z:.3f})")
    edge = 0.055 if "plant" in label.lower() else 0.02
    if not (x0 - edge <= mn.x and mx.x <= x1 + edge and y0 - edge <= mn.y and mx.y <= y1 + edge):
        raise RuntimeError(f"FAIL {label}: outside desktop XY [{x0:.2f},{x1:.2f}]×[{y0:.2f},{y1:.2f}]")


def _plant_glb_needs_replace(glb: Path) -> bool:
    if not glb.is_file():
        return True
    _common._reset_scene()
    parts = _common._import_gltf_mesh(glb)
    mn, mx = _common._bbox_world(parts)
    h = mx.z - mn.z
    _common._reset_scene()
    return h > PLANT_NATIVE_HEIGHT_MAX or h < 0.22


def _build_kenney_potted_plant_mesh() -> bpy.types.Object:
    """Low-poly Kenney Furniture Kit scale plant (~0.36 m tall, CC0 procedural)."""
    _common._reset_scene()
    bpy.ops.mesh.primitive_cylinder_add(vertices=10, radius=0.085, depth=0.13, location=(0.0, 0.0, 0.065))
    pot = bpy.context.active_object
    pot.name = "pottedPlant_pot"
    bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=1, radius=0.11, location=(0.0, 0.0, 0.24))
    leaf = bpy.context.active_object
    leaf.name = "pottedPlant_leaf"
    leaf.scale = (1.05, 1.05, 0.75)
    bpy.ops.object.transform_apply(scale=True)
    for dx, dy, dz in ((0.07, 0.05, 0.31), (-0.06, 0.06, 0.33), (0.02, -0.07, 0.30)):
        bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=1, radius=0.065, location=(dx, dy, dz))
        bpy.context.active_object.scale = (1.1, 1.1, 0.65)
        bpy.ops.object.transform_apply(scale=True)
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    bpy.ops.object.select_all(action="DESELECT")
    for o in meshes:
        o.select_set(True)
    bpy.context.view_layer.objects.active = pot
    bpy.ops.object.join()
    plant = bpy.context.view_layer.objects.active
    plant.name = "pottedPlant"
    mn, mx = _common._bbox_world([plant])
    height = mx.z - mn.z
    target_h = 0.36
    if height > 0.01:
        fac = target_h / height
        plant.scale = (fac, fac, fac)
        bpy.ops.object.transform_apply(scale=True)
        mn, mx = _common._bbox_world([plant])
        plant.location.z -= mn.z
        bpy.ops.object.transform_apply(location=True)
    return plant


def _ensure_potted_plant_glb() -> Path:
    glb = KENNEY / "pottedPlant.glb"
    if not _plant_glb_needs_replace(glb):
        return glb
    glb.parent.mkdir(parents=True, exist_ok=True)
    plant = _build_kenney_potted_plant_mesh()
    bpy.ops.object.select_all(action="DESELECT")
    plant.select_set(True)
    bpy.context.view_layer.objects.active = plant
    bpy.ops.export_scene.gltf(filepath=str(glb), export_format="GLB", use_selection=True)
    _common._reset_scene()
    return glb


def _yaw_face_camera(parts: list[bpy.types.Object], yaw_deg: float = 31.0) -> None:
    for o in parts:
        o.rotation_euler = (0.0, 0.0, math.radians(yaw_deg))
    bpy.context.view_layer.update()


def _footprint_disjoint(mn_a, mx_a, mn_b, mx_b, gap: float = PROP_GAP_M) -> bool:
    """Props must not overlap on the desktop footprint (Blender X/Y); Z may stack."""
    return (
        mx_a.x + gap < mn_b.x
        or mx_b.x + gap < mn_a.x
        or mx_a.y + gap < mn_b.y
        or mx_b.y + gap < mn_a.y
    )


def _place_plant_clear(
    plant_parts: list[bpy.types.Object],
    prop_map: dict[str, list[bpy.types.Object]],
    surface_z: float,
    xy: tuple[float, float, float, float],
    plant_scale: float,
) -> tuple[float, float]:
    """Find back-corner UV with footprint clearance from monitor/keyboard/mouse."""
    preferred = [(0.12, 0.56), (0.10, 0.50), (0.88, 0.62), (0.14, 0.62), (0.86, 0.54)]
    candidates = preferred + [(u / 100.0, v / 100.0) for v in range(40, 76, 3) for u in range(6, 30, 2)]
    boxes = {k: _common._bbox_world(v) for k, v in prop_map.items() if k == "monitor"}
    x0, x1, y0, y1 = xy
    for o in list(plant_parts):
        bpy.data.objects.remove(o, do_unlink=True)
    plant_parts.clear()
    for u, v in (c for c in candidates if c[1] >= 0.42):
        trial = _import_part("pottedPlant.glb")
        _common._apply_scale_uniform(trial, plant_scale)
        _place_on_desktop(trial, surface_z, xy, u, v)
        mn_p, mx_p = _common._bbox_world(trial)
        if not (x0 - 0.06 <= mn_p.x and mx_p.x <= x1 + 0.06 and y0 - 0.06 <= mn_p.y and mx_p.y <= y1 + 0.06):
            for o in trial:
                bpy.data.objects.remove(o, do_unlink=True)
            continue
        clear = all(
            _footprint_disjoint(mn_p, mx_p, mn_b, mx_b, gap=0.012)
            for mn_b, mx_b in boxes.values()
        )
        if clear:
            plant_parts.extend(trial)
            return u, v
        for o in trial:
            bpy.data.objects.remove(o, do_unlink=True)
    raise RuntimeError("FAIL plant: no clear back-corner placement on desktop")


def _gate_disjoint_props(props: dict[str, list[bpy.types.Object]]) -> None:
    monitor = props["monitor"]
    plant = props["plant"]
    mn_m, mx_m = _common._bbox_world(monitor)
    mn_p, mx_p = _common._bbox_world(plant)
    plant_h = mx_p.z - mn_p.z
    monitor_h = mx_m.z - mn_m.z
    ratio = plant_h / max(monitor_h, 1e-6)
    if ratio > PLANT_MONITOR_HEIGHT_RATIO_MAX:
        raise RuntimeError(
            f"FAIL plant height {plant_h:.3f}m > {PLANT_MONITOR_HEIGHT_RATIO_MAX}× monitor {monitor_h:.3f}m"
        )
    boxes: dict[str, tuple] = {}
    for label, parts in props.items():
        boxes[label] = _common._bbox_world(parts)
    mn_p, mx_p = boxes["plant"]
    mn_m, mx_m = boxes["monitor"]
    if not _footprint_disjoint(mn_p, mx_p, mn_m, mx_m):
        raise RuntimeError("FAIL prop footprint overlap: plant ∩ monitor")
    gap3 = PROP_GAP_M
    vol_clear = (
        mx_p.x + gap3 < mn_m.x
        or mx_m.x + gap3 < mn_p.x
        or mx_p.y + gap3 < mn_m.y
        or mx_m.y + gap3 < mn_p.y
        or mx_p.z + gap3 < mn_m.z
        or mx_m.z + gap3 < mn_p.z
    )
    if not vol_clear:
        raise RuntimeError("FAIL plant ∩ monitor 3D bbox overlap")


def build_desk_parts() -> list[bpy.types.Object]:
    _ensure_potted_plant_glb()
    _common._reset_scene()
    desk_parts = _import_part("desk.glb")
    width = _common._bbox_world(desk_parts)[1].x - _common._bbox_world(desk_parts)[0].x
    scale = TARGET_WIDTH / max(width, 0.01)
    _common._apply_scale_uniform(desk_parts, scale)
    _common._bevel_meshes(desk_parts, width=0.015, segments=3)
    surface_z = _desktop_surface_z(desk_parts)
    xy = _desktop_xy_bounds(desk_parts)

    all_obs: list[bpy.types.Object] = list(desk_parts)
    prop_map: dict[str, list[bpy.types.Object]] = {}

    periph = (
        ("computerScreen.glb", scale * 1.02),
        ("computerKeyboard.glb", scale * 1.10),
        ("computerMouse.glb", scale * 1.05),
    )
    for fname, sc in periph:
        parts = _import_part(fname)
        _common._apply_scale_uniform(parts, sc)
        key = fname.replace(".glb", "").replace("computer", "").lower() or fname
        if "screen" in fname.lower():
            key = "monitor"
            _yaw_face_camera(parts)
        elif "keyboard" in fname.lower():
            key = "keyboard"
        elif "mouse" in fname.lower():
            key = "mouse"
        u, v = KENNEY_OFFSETS[fname]
        _place_on_desktop(parts, surface_z, xy, u, v)
        if key in ("keyboard", "mouse", "monitor"):
            for o in parts:
                o.location.z -= 0.002
        _gate_peripheral_on_desktop(parts, surface_z, xy, fname)
        prop_map[key] = parts
        all_obs.extend(parts)

    plant_parts: list[bpy.types.Object] = []
    plant_scale = scale * 0.61
    u, v = _place_plant_clear(plant_parts, prop_map, surface_z, xy, plant_scale)
    KENNEY_OFFSETS["pottedPlant.glb"] = (u, v)
    prop_map["plant"] = plant_parts
    all_obs.extend(plant_parts)
    _gate_peripheral_on_desktop(plant_parts, surface_z, xy, "pottedPlant.glb")

    _gate_disjoint_props(prop_map)

    return all_obs


def main() -> None:
    parts = build_desk_parts()
    manifest = _common.finish_prop_desk_slot_export(
        parts,
        SOURCE_NOTE,
        extra_manifest={
            "kenneyParts": [
                "desk.glb",
                "computerScreen.glb",
                "computerKeyboard.glb",
                "computerMouse.glb",
                "pottedPlant.glb",
            ],
            "kenneyOffsets": KENNEY_OFFSETS,
            "cc0Wood": "docs/art/cc0/ambientcg/Wood092_1K-JPG",
            "restyle": "kenney-slot-v11",
        },
    )
    print(__import__("json").dumps(manifest, indent=2))


main()