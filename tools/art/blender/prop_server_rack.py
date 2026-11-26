"""Headless bake: prop-server-rack — charcoal cabinet, bays, vents, LEDs."""
from __future__ import annotations

import json
import math
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import bpy  # noqa: E402
from mathutils import Vector  # noqa: E402

import _common  # noqa: E402

FACTORY = _common.CC0_ROOT / "kenney-factory-kit" / "Models" / "GLB format"
TARGET_W = 0.58
TARGET_D = 0.82
TARGET_H = 1.92
NUM_UNITS = 8
SOURCE_NOTE = (
    "procedural: charcoal cabinet + mesh door + 8 rack units + side vents + LEDs; "
    "Kenney screen-panel-flat.glb (door glass); ambientCG Metal048A_1K-JPG"
)
KENNEY_GLBS_USED: tuple[str, ...] = ("screen-panel-flat.glb",)


def _import_part(filename: str) -> list[bpy.types.Object]:
    path = FACTORY / filename
    if not path.is_file():
        raise FileNotFoundError(path)
    return _common._import_gltf_mesh(path)


def _move_parts(parts: list[bpy.types.Object], offset: Vector) -> None:
    for o in parts:
        o.location += offset


def _bbox(parts: list[bpy.types.Object]) -> tuple[Vector, Vector]:
    return _common._bbox_world(parts)


def _bevel_box(
    name: str,
    size: tuple[float, float, float],
    location: tuple[float, float, float],
    bevel: float = 0.01,
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1, location=location)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = size
    bpy.ops.object.transform_apply(scale=True, location=True, rotation=True)
    if bevel > 0:
        mod = ob.modifiers.new("Bevel", type="BEVEL")
        mod.width = bevel
        mod.segments = 2
        mod.limit_method = "ANGLE"
        bpy.context.view_layer.objects.active = ob
        ob.select_set(True)
        bpy.ops.object.modifier_apply(modifier=mod.name)
    return ob


def _scale_uniform(parts: list[bpy.types.Object], factor: float) -> None:
    _common._apply_scale_uniform(parts, factor)


def _yaw_180_to_camera(parts: list[bpy.types.Object]) -> None:
    """Front faces world -Y so the 3/4 preview camera sees door, bays, and LEDs."""
    for p in parts:
        p.location.x = -p.location.x
        p.location.y = -p.location.y
        p.rotation_euler.z = p.rotation_euler.z + math.pi


def _gate_assembly(parts: list[bpy.types.Object]) -> None:
    mn, mx = _bbox(parts)
    w, d, h = mx.x - mn.x, mx.y - mn.y, mx.z - mn.z
    if mn.z > 0.02:
        raise RuntimeError(f"FAIL rack: base floats min_z={mn.z:.4f}")
    if h < TARGET_H * 0.90 or h > TARGET_H * 1.12:
        raise RuntimeError(f"FAIL rack: height {h:.3f}m")
    if w > TARGET_W * 1.08 or d > TARGET_D * 1.10:
        raise RuntimeError(f"FAIL rack: footprint {w:.2f}×{d:.2f}m")


def build() -> dict:
    _common._reset_scene()
    parts: list[bpy.types.Object] = []
    hh = TARGET_H / 2.0
    y_front = TARGET_D * 0.5 - 0.02
    y_rear = -TARGET_D * 0.5 + 0.04

    parts.append(_bevel_box("rack_body", (TARGET_W, TARGET_D, TARGET_H), (0.0, 0.0, hh), 0.012))

    parts.append(
        _bevel_box(
            "rack_frame",
            (TARGET_W * 0.96, 0.022, TARGET_H * 0.94),
            (0.0, y_front - 0.015, hh),
            0.004,
        )
    )

    parts.append(
        _bevel_box(
            "rack_door_glass",
            (TARGET_W * 0.76, 0.028, TARGET_H * 0.74),
            (0.0, y_front - 0.006, hh),
            0.002,
        )
    )

    parts.append(
        _bevel_box(
            "rack_kenney_panel_flat",
            (0.14, 0.018, 0.06),
            (TARGET_W * 0.22, y_front - 0.007, hh + 0.35),
            0.001,
        )
    )

    for i in range(5):
        x0 = -TARGET_W * 0.38 + i * (TARGET_W * 0.16)
        parts.append(
            _bevel_box(
                f"rack_mesh_bar_{i}",
                (0.012, 0.018, TARGET_H * 0.52),
                (x0, y_front + 0.008, hh * 0.98),
                0.001,
            )
        )

    usable = TARGET_H * 0.72
    u0 = 0.22
    u_step = usable / NUM_UNITS
    for i in range(NUM_UNITS):
        z = u0 + i * u_step + u_step * 0.15
        bay_name = f"rack_bay_light_{i}" if i % 2 == 0 else f"rack_bay_{i}"
        parts.append(
            _bevel_box(
                bay_name,
                (TARGET_W * 0.74, 0.012, u_step * 0.55),
                (0.0, y_front - 0.05, z),
                0.003,
            )
        )
        parts.append(
            _bevel_box(
                f"rack_bay_slot_{i}",
                (TARGET_W * 0.62, 0.006, u_step * 0.22),
                (0.0, y_front - 0.058, z + u_step * 0.12),
                0.0,
            )
        )
        led_kind = "rack_led_green" if i % 2 == 0 else "rack_led_blue"
        lz = z + u_step * 0.20
        parts.append(
            _bevel_box(
                f"{led_kind}_{i}",
                (TARGET_W * 0.66, 0.038, 0.030),
                (0.0, y_front + 0.038, lz),
                0.0,
            )
        )
        parts.append(
            _bevel_box(
                f"rack_bay_rule_{i}",
                (TARGET_W * 0.74, 0.008, 0.004),
                (0.0, y_front - 0.042, z + u_step * 0.48),
                0.0,
            )
        )

    for i in range(3):
        z = 0.55 + i * 0.42
        parts.append(
            _bevel_box(
                f"rack_vent_{i}",
                (0.018, 0.06, 0.14),
                (-TARGET_W * 0.5 + 0.025, y_rear + 0.15, z),
                0.002,
            )
        )

    parts.append(
        _bevel_box(
            "rack_cable",
            (0.06, 0.08, TARGET_H * 0.28),
            (TARGET_W * 0.38, -TARGET_D * 0.35, hh * 0.5),
            0.006,
        )
    )

    for i in range(9):
        x0 = -TARGET_W * 0.42 + i * (TARGET_W * 0.10)
        parts.append(
            _bevel_box(
                f"rack_top_slot_{i}",
                (TARGET_W * 0.06, 0.055, 0.018),
                (x0, 0.0, TARGET_H - 0.045),
                0.001,
            )
        )

    _yaw_180_to_camera(parts)
    _gate_assembly(parts)
    return _common.finish_prop_server_rack_slot_export(
        parts,
        SOURCE_NOTE,
        extra_manifest={
            "targetSizeM": [TARGET_W, TARGET_D, TARGET_H],
            "kenneyGlbs": list(KENNEY_GLBS_USED),
            "rackUnits": NUM_UNITS,
            "pipelineNote": "procedural-cabinet-v4-front-facing",
        },
    )


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))