"""Eye decals on front face: raycast + normals within 25° of shoe-forward."""
from __future__ import annotations

import json
import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_preview_rig as rig  # noqa: E402
from kenney_face_axis import eye_landmarks_world, toe_forward  # noqa: E402

REPO = Path(sys.argv[-1])
FRAME = REPO / "docs/art/previews/staff-founder-idle.frame.json"
STOCK_BLEND = REPO / "tools/art/source/kenney/staff-founder-kenney-stock.blend"
DIAG_FRAME = REPO / "docs/art/previews/staff-founder-face-diagnostic-0deg.frame.json"
COS_25 = math.cos(math.radians(25.0))


def _eye_slot(ob: bpy.types.Object) -> int:
    return next(
        (i for i, m in enumerate(ob.data.materials) if m and "FounderEye" in m.name),
        7,
    )


def _poly_centroid_normal(ob: bpy.types.Object, poly_index: int) -> tuple[Vector, Vector]:
    mesh = ob.data
    poly = mesh.polygons[poly_index]
    center = Vector()
    for li in poly.loop_indices:
        center += ob.matrix_world @ mesh.vertices[mesh.loops[li].vertex_index].co
    center /= len(poly.loop_indices)
    normal = (ob.matrix_world.to_3x3() @ poly.normal).normalized()
    return center, normal


def _ray_hit_eye(depsgraph, cam: bpy.types.Object, target: Vector, ob: bpy.types.Object, eye_idx: int) -> bool:
    origin = cam.matrix_world.translation
    for nudge in (0.0, -0.03, -0.06, 0.02):
        aim = target + (target - origin).normalized() * nudge if (target - origin).length > 1e-4 else target
        direction = (aim - origin).normalized()
        hit, _loc, _norm, face_i, hit_obj, _mat = bpy.context.scene.ray_cast(
            depsgraph, origin, direction
        )
        if hit and hit_obj == ob and face_i >= 0:
            if ob.data.polygons[face_i].material_index == eye_idx:
                return True
    return False


def _front_eye_polys(
    cam: bpy.types.Object, ob: bpy.types.Object, eye_idx: int, front: Vector
) -> int:
    cam_loc = cam.matrix_world.translation
    count = 0
    for poly in ob.data.polygons:
        if poly.material_index != eye_idx:
            continue
        center, normal = _poly_centroid_normal(ob, poly.index)
        if normal.dot(front) < COS_25:
            continue
        to_cam = (cam_loc - center).normalized()
        if normal.dot(to_cam) > 0.12:
            count += 1
    return count


def _eye_mesh_components(ob: bpy.types.Object, eye_idx: int) -> int:
    mesh = ob.data
    eye_faces = {i for i, p in enumerate(mesh.polygons) if p.material_index == eye_idx}
    if not eye_faces:
        return 0
    edge_to_faces: dict[tuple[int, int], list[int]] = {}
    for fi in eye_faces:
        poly = mesh.polygons[fi]
        verts = [mesh.loops[li].vertex_index for li in poly.loop_indices]
        for a, b in zip(verts, verts[1:] + verts[:1]):
            key = (min(a, b), max(a, b))
            edge_to_faces.setdefault(key, []).append(fi)
    adj: dict[int, set[int]] = {fi: set() for fi in eye_faces}
    for faces in edge_to_faces.values():
        eye_only = [f for f in faces if f in eye_faces]
        for i in eye_only:
            for j in eye_only:
                if i != j:
                    adj[i].add(j)
    seen: set[int] = set()
    comps = 0
    for fi in eye_faces:
        if fi in seen:
            continue
        comps += 1
        stack = [fi]
        while stack:
            cur = stack.pop()
            if cur in seen:
                continue
            seen.add(cur)
            stack.extend(adj[cur] - seen)
    return comps


def _aligned_eye_count(ob: bpy.types.Object, eye_idx: int, front: Vector) -> int:
    n = 0
    for poly in ob.data.polygons:
        if poly.material_index != eye_idx:
            continue
        _, normal = _poly_centroid_normal(ob, poly.index)
        if normal.dot(front) >= COS_25:
            n += 1
    return n


def _gate(mesh_obs: list, yaw: float, elev: float, margin: float) -> bpy.types.Object:
    cam, _ = rig.frame_staff_front_quarter(mesh_obs, 50.0, margin, yaw, elev)
    bpy.context.scene.camera = cam
    bpy.context.view_layer.update()
    return cam


def main() -> None:
    if not STOCK_BLEND.is_file():
        print("FAIL staff-founder-idle eye_project missing_blend")
        raise SystemExit(1)
    bpy.ops.wm.open_mainfile(filepath=str(STOCK_BLEND))
    ob = next((o for o in bpy.context.scene.objects if o.type == "MESH"), None)
    if not ob:
        print("FAIL staff-founder-idle eye_project no_mesh")
        raise SystemExit(1)
    from kenney_founder_stock_recolor import apply_founder_materials

    apply_founder_materials(ob)
    eye_idx = _eye_slot(ob)
    front = toe_forward([ob]).normalized()
    components = _eye_mesh_components(ob, eye_idx)
    if components != 2:
        print(f"FAIL staff-founder-idle eye_project components={components} want=2")
        raise SystemExit(1)
    aligned = _aligned_eye_count(ob, eye_idx, front)
    if aligned < 2:
        print(f"FAIL staff-founder-idle eye_project front_normals={aligned}")
        raise SystemExit(1)

    mesh_obs = [ob]
    depsgraph = bpy.context.evaluated_depsgraph_get()
    el, er, _ = eye_landmarks_world(ob)

    frame = json.loads(FRAME.read_text()) if FRAME.is_file() else {}
    for tag, yaw, elev, margin in (
        ("hero", float(frame.get("staff_yaw_deg", 30)), float(frame.get("staff_elev_deg", 10)), float(frame.get("framing_margin", 1.45))),
        ("diag0", 0.0, 8.0, 1.35),
    ):
        if tag == "diag0" and DIAG_FRAME.is_file():
            df = json.loads(DIAG_FRAME.read_text())
            yaw = float(df.get("staff_yaw_deg", 0))
            elev = float(df.get("staff_elev_deg", 8))
            margin = float(df.get("framing_margin", 1.35))
        cam = _gate(mesh_obs, yaw, elev, margin)
        hits = sum(
            1
            for target in (el, er)
            if _ray_hit_eye(depsgraph, cam, target, ob, eye_idx)
        )
        if hits < 2:
            vis = _front_eye_polys(cam, ob, eye_idx, front)
            if vis < 2:
                print(f"FAIL staff-founder-idle eye_project {tag} ray={hits} poly={vis}")
                raise SystemExit(1)
        bpy.data.objects.remove(cam, do_unlink=True)

    print("OK staff-founder-idle eye_project raycast=2")
    raise SystemExit(0)


if __name__ == "__main__":
    main()