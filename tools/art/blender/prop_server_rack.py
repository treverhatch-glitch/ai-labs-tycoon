"""Headless bake: prop-server-rack — Kenney Factory Kit slot assembly (Metal048A)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import bpy  # noqa: E402
from mathutils import Vector  # noqa: E402

import _common  # noqa: E402

FACTORY = _common.CC0_ROOT / "kenney-factory-kit" / "Models" / "GLB format"
TARGET_W = 0.60
TARGET_D = 0.85
TARGET_H = 2.00
SOURCE_NOTE = (
    "docs/art/cc0/kenney-factory-kit "
    "(structure-tall, structure-high, machine-fortified, machine-window, "
    "screen-panel-small, indicator-special-lines); ambientCG Metal048A"
)


def _import_part(filename: str) -> list[bpy.types.Object]:
    path = FACTORY / filename
    if not path.is_file():
        raise FileNotFoundError(path)
    return _common._import_gltf_mesh(path)


def _duplicate_collection(parts: list[bpy.types.Object]) -> list[bpy.types.Object]:
    out: list[bpy.types.Object] = []
    for ob in parts:
        copy = ob.copy()
        copy.data = ob.data.copy()
        bpy.context.collection.objects.link(copy)
        out.append(copy)
    return out


def _move_parts(parts: list[bpy.types.Object], offset: Vector) -> None:
    for o in parts:
        o.location += offset


def _bbox(parts: list[bpy.types.Object]) -> tuple[Vector, Vector]:
    return _common._bbox_world(parts)


def _uniform_scale(parts: list[bpy.types.Object], factor: float) -> None:
    _common._apply_scale_uniform(parts, factor)


def _scale_xyz(parts: list[bpy.types.Object], sx: float, sy: float, sz: float) -> None:
    for o in parts:
        o.scale = (o.scale.x * sx, o.scale.y * sy, o.scale.z * sz)
    bpy.ops.object.select_all(action="DESELECT")
    for o in parts:
        o.select_set(True)
    if parts:
        bpy.context.view_layer.objects.active = parts[0]
        bpy.ops.object.transform_apply(scale=True)


def _center_on_floor(parts: list[bpy.types.Object]) -> None:
    mn, mx = _bbox(parts)
    cx, cy = (mn.x + mx.x) * 0.5, (mn.y + mx.y) * 0.5
    _move_parts(parts, Vector((-cx, -cy, -mn.z)))


def _gate_assembly(parts: list[bpy.types.Object]) -> None:
    mn, mx = _bbox(parts)
    w, d, h = mx.x - mn.x, mx.y - mn.y, mx.z - mn.z
    if mn.z > 0.02:
        raise RuntimeError(f"FAIL rack: base floats min_z={mn.z:.4f}")
    if h < TARGET_H * 0.88 or h > TARGET_H * 1.12:
        raise RuntimeError(f"FAIL rack: height {h:.3f}m (want ~{TARGET_H}m)")
    if w > TARGET_W * 1.10 or d > TARGET_D * 1.12:
        raise RuntimeError(f"FAIL rack: footprint {w:.2f}×{d:.2f}m (max {TARGET_W}×{TARGET_D})")


def build() -> dict:
    _common._reset_scene()
    parts: list[bpy.types.Object] = []

    post = _import_part("structure-tall.glb")
    mn, mx = _bbox(post)
    pw = mx.x - mn.x
    _scale_xyz(
        post,
        1.0,
        (TARGET_D * 0.78) / (mx.y - mn.y),
        TARGET_H / (mx.z - mn.z),
    )
    mn, mx = _bbox(post)
    post_l = post
    post_r = _duplicate_collection(post)
    _move_parts(post_l, Vector((-TARGET_W * 0.5 + pw * 0.5, 0.0, 0.0)))
    _move_parts(post_r, Vector((TARGET_W * 0.5 - pw * 0.5, 0.0, 0.0)))
    parts.extend(post_l)
    parts.extend(post_r)

    spine = _import_part("machine-fortified.glb")
    mn, mx = _bbox(spine)
    _scale_xyz(
        spine,
        (TARGET_W * 0.82) / (mx.x - mn.x),
        (TARGET_D * 0.58) / (mx.y - mn.y),
        (TARGET_H * 0.90) / (mx.z - mn.z),
    )
    mn, mx = _bbox(spine)
    _move_parts(spine, Vector((0.0, -TARGET_D * 0.18, 0.0)))
    parts.extend(spine)

    for z in (0.42, 0.92, 1.42, 1.92):
        slot = _import_part("machine-window.glb")
        mn, mx = _bbox(slot)
        _scale_xyz(
            slot,
            (TARGET_W * 0.70) / (mx.x - mn.x),
            (TARGET_D * 0.42) / (mx.y - mn.y),
            0.28 / (mx.z - mn.z),
        )
        mn, mx = _bbox(slot)
        _move_parts(slot, Vector((0.0, -TARGET_D * 0.05 - (mn.y + mx.y) * 0.5, z)))
        parts.extend(slot)

    for i in range(2):
        panel = _import_part("screen-panel-small.glb")
        mn, mx = _bbox(panel)
        _uniform_scale(panel, 0.24 / (mx.z - mn.z))
        pmn, pmx = _bbox(panel)
        _move_parts(panel, Vector((TARGET_W * 0.34, -TARGET_D * 0.38 - pmx.y, 0.6 + i * 0.55)))
        parts.extend(panel)

    led_rows = (0.55, 1.05, 1.55)
    for i, z in enumerate(led_rows):
        led = _import_part("indicator-special-lines.glb")
        mn, mx = _bbox(led)
        _uniform_scale(led, 0.16 / (mx.x - mn.x))
        lmn, lmx = _bbox(led)
        _move_parts(led, Vector((TARGET_W * 0.38, -TARGET_D * 0.40 - lmx.y, z)))
        parts.extend(led)

    back = _import_part("structure-high.glb")
    mn, mx = _bbox(back)
    _scale_xyz(back, 1.0, 0.06 / (mx.y - mn.y), TARGET_H / (mx.z - mn.z))
    bmn, bmx = _bbox(back)
    _move_parts(back, Vector((0.0, -TARGET_D * 0.5 - bmx.y, 0.0)))
    parts.extend(back)

    _center_on_floor(parts)
    _gate_assembly(parts)
    return _common.finish_prop_server_rack_slot_export(
        parts,
        SOURCE_NOTE,
        extra_manifest={
            "targetSizeM": [TARGET_W, TARGET_D, TARGET_H],
            "pipelineNote": "ALT-484 local prep kenney-factory-v1",
        },
    )


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))