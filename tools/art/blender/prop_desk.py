"""Headless bake: prop-desk — Kenney GLTF per-slot materials (no vertex masks)."""
from __future__ import annotations

import math
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import bpy  # noqa: E402

import _common  # noqa: E402

KENNEY = _common.CC0_ROOT / "kenney-furniture-kit" / "Models" / "GLTF format"
CAMPUS_PLANT = _common.REPO / "unity" / "Assets" / "Art" / "Generated" / "campus-kit" / "prop-potted-plant.obj"
TARGET_WIDTH = 1.14
SURFACE_TOL = 0.012
SOURCE_NOTE = (
    "docs/art/cc0/kenney-furniture-kit "
    "(desk, computerScreen, computerKeyboard, computerMouse, pottedPlant); "
    "ambientCG Wood092"
)

KENNEY_OFFSETS = {
    "computerScreen.glb": (0.56, 0.46),
    "computerKeyboard.glb": (0.30, 0.70),
    "computerMouse.glb": (0.62, 0.64),
    "pottedPlant.glb": (0.84, 0.76),
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
    if not (x0 - 0.02 <= mn.x and mx.x <= x1 + 0.02 and y0 - 0.02 <= mn.y and mx.y <= y1 + 0.02):
        raise RuntimeError(f"FAIL {label}: outside desktop XY [{x0:.2f},{x1:.2f}]×[{y0:.2f},{y1:.2f}]")


def _ensure_potted_plant_glb() -> Path:
    glb = KENNEY / "pottedPlant.glb"
    if glb.is_file():
        return glb
    if not CAMPUS_PLANT.is_file():
        raise FileNotFoundError(CAMPUS_PLANT)
    _common._reset_scene()
    bpy.ops.wm.obj_import(filepath=str(CAMPUS_PLANT))
    objs = [o for o in bpy.context.selected_objects if o.type == "MESH"]
    if not objs:
        raise RuntimeError("campus plant OBJ import failed")
    if len(objs) > 1:
        bpy.ops.object.select_all(action="DESELECT")
        for o in objs:
            o.select_set(True)
        bpy.context.view_layer.objects.active = objs[0]
        bpy.ops.object.join()
    plant = bpy.context.view_layer.objects.active
    plant.name = "pottedPlant"
    bpy.ops.object.select_all(action="DESELECT")
    plant.select_set(True)
    bpy.context.view_layer.objects.active = plant
    bpy.ops.export_scene.gltf(filepath=str(glb), export_format="GLB", use_selection=True)
    return glb


def _yaw_face_camera(parts: list[bpy.types.Object], yaw_deg: float = 31.0) -> None:
    from mathutils import Vector

    for o in parts:
        o.rotation_euler = (0.0, 0.0, math.radians(yaw_deg))
    bpy.context.view_layer.update()


def build_desk_parts() -> list[bpy.types.Object]:
    _common._reset_scene()
    desk_parts = _import_part("desk.glb")
    width = _common._bbox_world(desk_parts)[1].x - _common._bbox_world(desk_parts)[0].x
    scale = TARGET_WIDTH / max(width, 0.01)
    _common._apply_scale_uniform(desk_parts, scale)
    _common._bevel_meshes(desk_parts, width=0.015, segments=3)
    surface_z = _desktop_surface_z(desk_parts)
    xy = _desktop_xy_bounds(desk_parts)

    all_obs: list[bpy.types.Object] = list(desk_parts)

    periph = (
        ("computerScreen.glb", scale * 1.02),
        ("computerKeyboard.glb", scale * 1.10),
        ("computerMouse.glb", scale * 1.05),
    )
    for fname, sc in periph:
        parts = _import_part(fname)
        _common._apply_scale_uniform(parts, sc)
        if "Screen" in fname:
            _yaw_face_camera(parts)
        u, v = KENNEY_OFFSETS[fname]
        _place_on_desktop(parts, surface_z, xy, u, v)
        if "Keyboard" in fname or "Mouse" in fname:
            for o in parts:
                o.location.z -= 0.002
        if "Screen" in fname:
            for o in parts:
                o.location.z -= 0.002
        _gate_peripheral_on_desktop(parts, surface_z, xy, fname)
        all_obs.extend(parts)

    _ensure_potted_plant_glb()
    plant_parts = _import_part("pottedPlant.glb")
    plant_sc = scale * 0.82
    _common._apply_scale_uniform(plant_parts, plant_sc)
    u, v = KENNEY_OFFSETS["pottedPlant.glb"]
    _place_on_desktop(plant_parts, surface_z, xy, u, v)
    all_obs.extend(plant_parts)

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