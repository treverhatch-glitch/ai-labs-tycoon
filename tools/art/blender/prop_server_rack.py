"""Headless bake: prop-server-rack — procedural dark cabinet + Kenney accents."""
from __future__ import annotations

import json
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
SOURCE_NOTE = (
    "procedural: dark bevelled cabinet (Metal048A tint); "
    "Kenney GLB accents: screen-panel-flat.glb (glass door strip only); "
    "ambientCG Metal048A_1K-JPG"
)
KENNEY_GLBS_USED: tuple[str, ...] = (
    "screen-panel-flat.glb",
)


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
    bevel: float = 0.012,
) -> bpy.types.Object:
    bpy.ops.mesh.primitive_cube_add(size=1, location=location)
    ob = bpy.context.active_object
    ob.name = name
    ob.dimensions = size
    bpy.ops.object.transform_apply(scale=True, location=True, rotation=True)
    mod = ob.modifiers.new("Bevel", type="BEVEL")
    mod.width = bevel
    mod.segments = 2
    mod.limit_method = "ANGLE"
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.modifier_apply(modifier=mod.name)
    return ob


def _scale_parts_uniform(parts: list[bpy.types.Object], factor: float) -> None:
    _common._apply_scale_uniform(parts, factor)


def _gate_assembly(parts: list[bpy.types.Object]) -> None:
    mn, mx = _bbox(parts)
    w, d, h = mx.x - mn.x, mx.y - mn.y, mx.z - mn.z
    if mn.z > 0.02:
        raise RuntimeError(f"FAIL rack: base floats min_z={mn.z:.4f}")
    if h < TARGET_H * 0.92 or h > TARGET_H * 1.06:
        raise RuntimeError(f"FAIL rack: height {h:.3f}m")
    if w > TARGET_W * 1.08 or d > TARGET_D * 1.10:
        raise RuntimeError(f"FAIL rack: footprint {w:.2f}×{d:.2f}m")


def build() -> dict:
    _common._reset_scene()
    parts: list[bpy.types.Object] = []
    hh = TARGET_H / 2.0

    shell = _bevel_box("rack_body", (TARGET_W, TARGET_D, TARGET_H), (0.0, 0.0, hh), 0.014)
    parts.append(shell)

    door = _bevel_box(
        "rack_door",
        (TARGET_W * 0.90, 0.035, TARGET_H * 0.88),
        (0.0, TARGET_D * 0.5 - 0.02, hh),
        0.008,
    )
    parts.append(door)

    glass = _import_part(KENNEY_GLBS_USED[0])
    mn, mx = _bbox(glass)
    _scale_parts_uniform(glass, (TARGET_W * 0.75) / max(mx.x - mn.x, 1e-6))
    _scale_parts_uniform(glass, 0.12 / max(_bbox(glass)[1].z - _bbox(glass)[0].z, 1e-6))
    mn, mx = _bbox(glass)
    cx = (mn.x + mx.x) * 0.5
    _move_parts(glass, Vector((-cx, TARGET_D * 0.5 - 0.01 - mx.y, hh - 0.08 - mn.z)))
    for o in glass:
        o.name = "rack_door_glass"
    parts.extend(glass)

    u_h = TARGET_H * 0.11
    for i in range(6):
        z = 0.22 + i * u_h
        bay = _bevel_box(
            f"rack_bay_{i}",
            (TARGET_W * 0.78, 0.018, u_h * 0.72),
            (0.0, TARGET_D * 0.5 - 0.06, z),
            0.004,
        )
        parts.append(bay)
        if i % 2 == 0:
            led = _bevel_box(
                f"rack_led_green_{i}",
                (0.012, 0.008, 0.012),
                (TARGET_W * 0.32, TARGET_D * 0.5 + 0.005, z + u_h * 0.15),
                0.002,
            )
            parts.append(led)
        else:
            led = _bevel_box(
                f"rack_led_blue_{i}",
                (0.012, 0.008, 0.012),
                (TARGET_W * 0.32, TARGET_D * 0.5 + 0.005, z + u_h * 0.15),
                0.002,
            )
            parts.append(led)

    feet = _bevel_box(
        "rack_feet",
        (TARGET_W * 0.88, 0.04, 0.05),
        (0.0, TARGET_D * 0.42, 0.025),
        0.006,
    )
    parts.append(feet)

    _gate_assembly(parts)
    return _common.finish_prop_server_rack_slot_export(
        parts,
        SOURCE_NOTE,
        extra_manifest={
            "targetSizeM": [TARGET_W, TARGET_D, TARGET_H],
            "kenneyGlbs": list(KENNEY_GLBS_USED),
            "pipelineNote": "procedural-cabinet-v2",
        },
    )


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))