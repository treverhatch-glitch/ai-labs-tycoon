"""GLB: researcher eyes on head (world Z band + surface distance)."""
from __future__ import annotations

import sys
from pathlib import Path

import bpy
from mathutils import Vector
from mathutils.bvhtree import BVHTree

_args = sys.argv[sys.argv.index("--") + 1 :] if "--" in sys.argv else sys.argv[-1:]
REPO = Path(_args[0])
GLB = REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-researcher-idle.glb"
ASSET = "staff-researcher-idle"


def _world_bbox(ob: bpy.types.Object) -> tuple[Vector, Vector]:
    pts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    xs = [p.x for p in pts]
    ys = [p.y for p in pts]
    zs = [p.z for p in pts]
    return Vector((min(xs), min(ys), min(zs))), Vector((max(xs), max(ys), max(zs)))


def _body_bvh(body: bpy.types.Object) -> BVHTree:
    import bmesh

    deps = bpy.context.evaluated_depsgraph_get()
    eval_ob = body.evaluated_get(deps)
    mesh = eval_ob.to_mesh()
    bm = bmesh.new()
    bm.from_mesh(mesh)
    bm.transform(eval_ob.matrix_world)
    eval_ob.to_mesh_clear()
    tree = BVHTree.FromBMesh(bm)
    bm.free()
    return tree


def _head_z_band(body: bpy.types.Object) -> tuple[float, float]:
    pts = [body.matrix_world @ v.co for v in body.data.vertices]
    zs = [p.z for p in pts]
    lo, hi = min(zs), max(zs)
    span = max(1e-6, hi - lo)
    head_z = [p.z for p in pts if (p.z - lo) / span > 0.72]
    return min(head_z), max(head_z)


def main() -> None:
    if not GLB.is_file():
        print(f"FAIL {ASSET} eye_glb_world missing_glb")
        raise SystemExit(1)
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=str(GLB))
    by = {o.name.split(".")[0]: o for o in bpy.context.scene.objects if o.type == "MESH"}
    body = by.get("KenneyResearcherStock") or by.get("Head")
    eyes = [by[n] for n in ("FounderEye_L", "FounderEye_R") if n in by]
    if not body or len(eyes) != 2:
        print(f"FAIL {ASSET} eye_glb_world parts body={bool(body)} eyes={len(eyes)}")
        raise SystemExit(1)

    z0, z1 = _head_z_band(body)
    bvh = _body_bvh(body)
    pad_z = 0.10 if body.name.split(".")[0] == "Head" else 0.05
    for eye in eyes:
        e0, e1 = _world_bbox(eye)
        center = (e0 + e1) * 0.5
        if e0.z < 0.35:
            print(f"FAIL {ASSET} eye_glb_world {eye.name} on_floor z={e0.z:.3f}")
            raise SystemExit(1)
        if e0.z < z0 - pad_z or e1.z > z1 + pad_z:
            print(f"FAIL {ASSET} eye_glb_world {eye.name} z=({e0.z:.3f},{e1.z:.3f}) head_z=({z0:.3f},{z1:.3f})")
            raise SystemExit(1)
        best_d = 1e9
        for v in eye.data.vertices:
            p = eye.matrix_world @ v.co
            nearest, _normal, _idx, dist = bvh.find_nearest(p)
            if nearest is not None:
                best_d = min(best_d, dist)
        if best_d > 0.12:
            print(
                f"FAIL {ASSET} eye_glb_world {eye.name} surface_dist={best_d:.4f} "
                f"center=({center.x:.3f},{center.y:.3f},{center.z:.3f})"
            )
            raise SystemExit(1)
        if abs(center.x) > 0.22:
            print(f"FAIL {ASSET} eye_glb_world {eye.name} too_lateral x={center.x:.3f}")
            raise SystemExit(1)
    print(f"OK {ASSET} eye_glb_world head_contains_eyes")
    raise SystemExit(0)


if __name__ == "__main__":
    main()