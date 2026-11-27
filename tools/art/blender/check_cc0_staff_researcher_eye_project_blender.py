"""FounderEye_L/R in exported GLB: raycast + front-facing normals."""
from __future__ import annotations

import json
import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_preview_rig as rig  # noqa: E402
from founder_face_decals import EYE_OBJECT_NAMES  # noqa: E402
from kenney_face_axis import toe_forward  # noqa: E402

REPO = Path(sys.argv[-1])
FRAME = REPO / "docs/art/previews/staff-researcher-idle.frame.json"
GLB = REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-researcher-idle.glb"
DIAG_FRAME = REPO / "docs/art/previews/staff-founder-face-diagnostic-0deg.frame.json"
COS_25 = math.cos(math.radians(25.0))


def _eye_centroid(eye: bpy.types.Object) -> Vector:
    pts = [eye.matrix_world @ v.co for v in eye.data.vertices]
    return sum(pts, Vector()) / max(1, len(pts))


def _eye_forward_polys(eye: bpy.types.Object, front: Vector) -> int:
    n = 0
    for poly in eye.data.polygons:
        normal = (eye.matrix_world.to_3x3() @ poly.normal).normalized()
        if normal.dot(front) >= COS_25:
            n += 1
    return n


def _ray_hits_eye(depsgraph, cam: bpy.types.Object, eye: bpy.types.Object) -> bool:
    target = _eye_centroid(eye)
    origin = cam.matrix_world.translation
    delta = target - origin
    if delta.length < 1e-5:
        return False
    direction = delta.normalized()
    for t in (0.95, 0.88, 1.0, 0.82, 1.05):
        from_v = origin if t <= 1.0 else origin
        to_v = origin + direction * (delta.length * min(t, 1.0))
        if t > 1.0:
            to_v = target + direction * (eye.dimensions.length * 0.1)
        cast_dir = (to_v - from_v).normalized()
        hit, _loc, _norm, _fi, hit_obj, _mat = bpy.context.scene.ray_cast(
            depsgraph, from_v, cast_dir if cast_dir.length else direction
        )
        if hit and hit_obj and hit_obj.name.split(".")[0] in EYE_OBJECT_NAMES:
            return True
    cast_dir = direction
    hit, _loc, _norm, _fi, hit_obj, _mat = bpy.context.scene.ray_cast(
        depsgraph, origin, cast_dir
    )
    return bool(hit and hit_obj and hit_obj.name.split(".")[0] in EYE_OBJECT_NAMES)


def _gate(mesh_obs: list, yaw: float, elev: float, margin: float) -> bpy.types.Object:
    cam, _ = rig.frame_staff_front_quarter(mesh_obs, 50.0, margin, yaw, elev)
    bpy.context.scene.camera = cam
    bpy.context.view_layer.update()
    return cam


def _load_scene() -> list[bpy.types.Object]:
    if GLB.is_file():
        bpy.ops.object.select_all(action="SELECT")
        bpy.ops.object.delete(use_global=False)
        bpy.ops.import_scene.gltf(filepath=str(GLB))
    else:
        import cc0_staff_lib as c0

        c0.build_researcher_cc0()
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    eyes = [o for o in meshes if o.name.split(".")[0] in EYE_OBJECT_NAMES]
    if len(eyes) != 2:
        return []
    return meshes


def main() -> None:
    mesh_obs = _load_scene()
    eyes = [o for o in mesh_obs if o.name.split(".")[0] in EYE_OBJECT_NAMES]
    if len(mesh_obs) < 3 or len(eyes) != 2:
        print(
            f"FAIL staff-researcher-idle eye_project components={len(mesh_obs)} "
            f"eyes={len(eyes)} want>=3 meshes, 2 eyes"
        )
        raise SystemExit(1)
    front = toe_forward(mesh_obs).normalized()
    if sum(_eye_forward_polys(e, front) for e in eyes) < 2:
        print("FAIL staff-researcher-idle eye_project front_normals")
        raise SystemExit(1)

    depsgraph = bpy.context.evaluated_depsgraph_get()
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
        hits = sum(1 for eye in eyes if _ray_hits_eye(depsgraph, cam, eye))
        if hits < 2:
            print(f"FAIL staff-researcher-idle eye_project {tag} ray={hits}")
            raise SystemExit(1)
        bpy.data.objects.remove(cam, do_unlink=True)

    print("OK staff-researcher-idle eye_project raycast=2")
    raise SystemExit(0)


if __name__ == "__main__":
    main()