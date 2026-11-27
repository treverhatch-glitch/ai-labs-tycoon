"""Project eye-decal landmarks to preview camera; both inside head bbox, in front of lens."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy
from bpy_extras.object_utils import world_to_camera_view
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_preview_rig as rig  # noqa: E402
from kenney_face_axis import eye_landmarks_world  # noqa: E402

REPO = Path(sys.argv[-1])
FRAME = REPO / "docs/art/previews/staff-founder-idle.frame.json"
GLB = REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.glb"


def _head_ndc_bounds(ob: bpy.types.Object, scene: bpy.types.Scene, cam: bpy.types.Object) -> tuple[float, float, float, float]:
    zs = [(ob.matrix_world @ v.co).z for v in ob.data.vertices]
    lo, hi = min(zs), max(zs)
    span = max(1e-4, hi - lo)
    head_vs = [ob.matrix_world @ v.co for v in ob.data.vertices if (v.co.z - lo) / span >= 0.52]
    xs: list[float] = []
    ys: list[float] = []
    for w in head_vs:
        co = world_to_camera_view(scene, cam, w)
        if co.z <= 0:
            continue
        xs.append(co.x)
        ys.append(co.y)
    if not xs:
        return 0.0, 1.0, 0.0, 1.0
    return min(xs), max(xs), min(ys), max(ys)


def main() -> None:
    if not GLB.is_file():
        print("FAIL staff-founder-idle eye_project missing_glb")
        raise SystemExit(1)
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)
    before = set(bpy.context.scene.objects)
    bpy.ops.import_scene.gltf(filepath=str(GLB))
    imported = [o for o in bpy.context.scene.objects if o not in before]
    ob = next((o for o in imported if o.type == "MESH"), None)
    if not ob:
        print("FAIL staff-founder-idle eye_project no_mesh")
        raise SystemExit(1)
    from kenney_founder_stock_recolor import apply_founder_materials

    apply_founder_materials(ob)
    frame = json.loads(FRAME.read_text()) if FRAME.is_file() else {}
    yaw = float(frame.get("staff_yaw_deg", 30))
    elev = float(frame.get("staff_elev_deg", 10))
    margin = float(frame.get("framing_margin", 1.45))
    mesh_obs = [ob]
    cam, _ = rig.frame_staff_front_quarter(mesh_obs, 50.0, margin, yaw, elev)
    scene = bpy.context.scene
    scene.camera = cam
    bpy.context.view_layer.update()
    el, er, _brow = eye_landmarks_world(ob)
    vis = 0
    x0, x1, y0, y1 = _head_ndc_bounds(ob, scene, cam)
    pad = 0.08
    for label, w in (("L", el), ("R", er)):
        co = world_to_camera_view(scene, cam, w)
        if co.z <= 0.05:
            print(f"FAIL staff-founder-idle eye_project {label}_behind_cam z={co.z:.3f}")
            raise SystemExit(1)
        if not (x0 - pad <= co.x <= x1 + pad and y0 - pad <= co.y <= y1 + pad):
            print(
                f"FAIL staff-founder-idle eye_project {label}_outside_head ndc=({co.x:.3f},{co.y:.3f})"
            )
            raise SystemExit(1)
        if not (-0.05 < co.x < 1.05 and 0.01 < co.y < 0.99):
            print(f"FAIL staff-founder-idle eye_project {label}_off_screen ndc=({co.x:.3f},{co.y:.3f})")
            raise SystemExit(1)
        vis += 1
    bpy.data.objects.remove(cam, do_unlink=True)
    print(f"OK staff-founder-idle eye_project visible={vis}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()