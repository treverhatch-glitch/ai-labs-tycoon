"""Headless: flag detached props sitting on the floor away from the figure."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
from staff_connectivity import PROP_RE  # noqa: E402

FLOOR_Z = 0.09
MAX_FLOOR_H = 0.12
MIN_FOOT_AWAY = 0.22


def _bbox(ob: bpy.types.Object) -> tuple[Vector, Vector]:
    co = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    xs = [v.x for v in co]
    ys = [v.y for v in co]
    zs = [v.z for v in co]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))


def _is_prop(base: str) -> bool:
    if PROP_RE.search(base):
        return True
    if base in ("MugHandle",):
        return True
    return False


def main() -> None:
    glb, aid = sys.argv[-2], sys.argv[-1]
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=glb)
    meshes = [
        o
        for o in bpy.context.scene.objects
        if o.type == "MESH" and not o.name.startswith("Preview")
    ]
    for ob in meshes:
        base = ob.name.split(".")[0]
        if not _is_prop(base):
            continue
        lo, hi = _bbox(ob)
        h = hi.z - lo.z
        if lo.z > FLOOR_Z or h > MAX_FLOOR_H:
            continue
        cx = 0.5 * (lo.x + hi.x)
        cy = 0.5 * (lo.y + hi.y)
        away = (cx**2 + cy**2) ** 0.5
        if away >= MIN_FOOT_AWAY:
            print(f"FAIL {aid} floor_loose {base} z={lo.z:.3f} away={away:.3f}")
            return
    print(f"OK {aid} floor_clean")


if __name__ == "__main__":
    main()