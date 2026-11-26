"""Headless bake: prop-desk hero from Kenney Furniture Kit (CC0) + restyle."""
from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import bpy  # noqa: E402
from mathutils import Vector  # noqa: E402

import _common  # noqa: E402

KENNEY = _common.CC0_ROOT / "kenney-furniture-kit" / "Models" / "GLTF format"
TARGET_WIDTH = 1.14
SOURCE_NOTE = "docs/art/cc0/kenney-furniture-kit (desk, computerScreen, computerKeyboard, computerMouse)"

DESK_COLORS = {
    "desk": (0.58, 0.5, 0.42),
    "frame": (0.22, 0.24, 0.28),
    "screen": (0.12, 0.14, 0.18),
    "display": (0.15, 0.42, 0.78),
    "keyboard": (0.18, 0.2, 0.24),
    "mouse": (0.32, 0.34, 0.38),
}


def _import_part(filename: str) -> list[bpy.types.Object]:
    path = KENNEY / filename
    if not path.is_file():
        raise FileNotFoundError(path)
    return _common._import_gltf_mesh(path)


def build_desk() -> bpy.types.Object:
    _common._reset_scene()
    desk_parts = _import_part("desk.glb")
    _mins, maxs = _common._bbox_world(desk_parts)
    width = max(maxs.x - _mins.x, 0.01)
    _common._apply_scale_uniform(desk_parts, TARGET_WIDTH / width)
    _mins, maxs = _common._bbox_world(desk_parts)
    top_y = maxs.y
    desk_scale = TARGET_WIDTH / width

    periph = [
        ("computerScreen.glb", Vector((0.14, top_y, -0.06)), DESK_COLORS["screen"], 1.02),
        ("computerKeyboard.glb", Vector((-0.16, top_y, 0.1)), DESK_COLORS["keyboard"], 1.0),
        ("computerMouse.glb", Vector((0.26, top_y, 0.12)), DESK_COLORS["mouse"], 1.05),
    ]
    all_obs = list(desk_parts)
    for fname, loc, color, sm in periph:
        parts = _import_part(fname)
        _common._apply_scale_uniform(parts, desk_scale * sm)
        for p in parts:
            p.location = loc
        all_obs.extend(parts)

    _common._bevel_meshes(all_obs, width=0.005, segments=2)
    for p in desk_parts:
        _common._paint_mesh_vertex_color(p, DESK_COLORS["desk"])
    for p in all_obs:
        if p in desk_parts:
            continue
        name = p.name.lower()
        if "screen" in name:
            _common._paint_mesh_vertex_color(p, DESK_COLORS["display"])
        else:
            _common._paint_mesh_vertex_color(
                p,
                DESK_COLORS["keyboard"] if "keyboard" in name else DESK_COLORS["mouse"],
            )

    bpy.ops.object.select_all(action="DESELECT")
    for o in all_obs:
        o.select_set(True)
    bpy.context.view_layer.objects.active = all_obs[0]
    if len(all_obs) > 1:
        bpy.ops.object.join()
    joined = bpy.context.view_layer.objects.active
    joined.location = Vector((0.0, 0.0, -_common._bbox_world([joined])[0].y))
    joined.name = "mesh_prop-desk"
    return joined


def main() -> None:
    obj = build_desk()
    manifest = _common.finish_baked_export(
        "prop-desk",
        obj,
        SOURCE_NOTE,
        extra_manifest={"kenneyParts": ["desk.glb", "computerScreen.glb", "computerKeyboard.glb", "computerMouse.glb"]},
    )
    print(__import__("json").dumps(manifest, indent=2))


main()