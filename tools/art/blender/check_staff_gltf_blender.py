"""Import staff GLBs; require Blender Z-up standing pose (Unity +Y after glTF)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

repo = Path(sys.argv[-1])
manifest = json.loads((repo / "unity/Assets/Art/EarlyGame/staff-manifest.json").read_text())

MIN_HEIGHT = 1.55
MAX_HEIGHT = 2.05
HEAD_FOOT_MIN_GAP = 0.45
MAX_SPINE_RADIUS_M = 0.6
HERO_IDS = frozenset({"staff-founder-idle", "staff-researcher-idle"})


def _centroid(ob: bpy.types.Object) -> Vector:
    pts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    return sum(pts, Vector()) / max(1, len(pts))


def _spine_radius(meshes: list[bpy.types.Object]) -> tuple[bool, str]:
    by_name = {o.name.split(".")[0]: o for o in meshes}
    hips = by_name.get("Hips")
    head = by_name.get("Head")
    if not hips or not head:
        return False, "spine_parts_missing"
    p0 = _centroid(hips)
    p1 = _centroid(head)
    axis = p1 - p0
    if axis.length < 0.2:
        return False, "spine_axis_short"
    axis_n = axis.normalized()
    worst = ("", 0.0)
    for ob in meshes:
        base = ob.name.split(".")[0]
        for v in ob.data.vertices:
            p = ob.matrix_world @ v.co
            rel = p - p0
            perp = rel - axis_n * rel.dot(axis_n)
            r = perp.length
            if r > worst[1]:
                worst = (base, r)
    if worst[1] > MAX_SPINE_RADIUS_M:
        return False, f"spine_r={worst[1]:.3f}m on {worst[0]} max={MAX_SPINE_RADIUS_M}m"
    return True, f"spine_r={worst[1]:.3f}m"


def analyze_gltf(aid: str, glb: Path) -> tuple[bool, str]:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    if not meshes:
        return False, "no_meshes"
    verts: list[Vector] = []
    for ob in meshes:
        verts.extend(ob.matrix_world @ v.co for v in ob.data.vertices)
    xs = [p.x for p in verts]
    ys = [p.y for p in verts]
    zs = [p.z for p in verts]
    sx, sy, sz = max(xs) - min(xs), max(ys) - min(ys), max(zs) - min(zs)
    min_z, max_z = min(zs), max(zs)
    if sz < max(sx, sy) * 0.92:
        return False, f"not_z_up sx={sx:.3f} sy={sy:.3f} sz={sz:.3f}"
    if sz < MIN_HEIGHT or sz > MAX_HEIGHT:
        return False, f"height_sz={sz:.3f} want {MIN_HEIGHT}-{MAX_HEIGHT}"
    if min_z > 0.03:
        return False, f"float min_z={min_z:.4f}"
    band = max(0.08, sz * 0.12)
    foot_z = sum(z for z in zs if z <= min_z + band) / max(1, sum(1 for z in zs if z <= min_z + band))
    head_z = sum(z for z in zs if z >= max_z - band) / max(1, sum(1 for z in zs if z >= max_z - band))
    if head_z <= foot_z + HEAD_FOOT_MIN_GAP:
        return False, f"head_below_feet head_z={head_z:.3f} foot_z={foot_z:.3f} gap={head_z-foot_z:.3f}"
    if aid in HERO_IDS:
        if any(n == "Cube" for n in (o.name.split(".")[0] for o in meshes)):
            return False, "stray_cube_mesh"
        ok_spine, spine_msg = _spine_radius(meshes)
        if not ok_spine:
            return False, spine_msg
        return True, f"z_up sz={sz:.3f} {spine_msg} meshes={len(meshes)}"
    return True, f"z_up sz={sz:.3f} head_z={head_z:.3f} foot_z={foot_z:.3f} min_z={min_z:.3f} meshes={len(meshes)}"


def main() -> int:
    rc = 0
    for entry in manifest["assets"]:
        aid = entry["id"]
        glb = repo / "unity/Assets/Art/EarlyGame/staff-kit" / f"{aid}.glb"
        if not glb.is_file():
            print(f"FAIL {aid} missing glb")
            rc = 1
            continue
        ok, msg = analyze_gltf(aid, glb)
        if ok:
            print(f"OK {aid} {msg}")
        else:
            print(f"FAIL {aid} {msg}")
            rc = 1
    return rc


if __name__ == "__main__":
    raise SystemExit(main())