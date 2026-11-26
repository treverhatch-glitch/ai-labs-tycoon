"""Gate: head forward in camera space aligns toward the lens (+Z from subject)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_preview_rig as rig  # noqa: E402

REPO = Path(sys.argv[-1])
GLB = REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.glb"
FRAME = REPO / "docs/art/previews/staff-founder-idle.frame.json"
MIN_FACE_DOT = 0.5


def main() -> None:
    if not GLB.is_file():
        print("FAIL staff-founder-idle missing glb")
        raise SystemExit(1)
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)
    bpy.ops.import_scene.gltf(filepath=str(GLB))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    frame = json.loads(FRAME.read_text()) if FRAME.is_file() else {}
    yaw = float(frame.get("staff_yaw_deg", 30))
    elev = float(frame.get("staff_elev_deg", 11))
    cam, _ = rig.frame_staff_front_quarter(meshes, 50.0, 1.46, yaw, elev)
    bpy.context.scene.camera = cam
    cam_inv = cam.matrix_world.inverted()
    rot = cam_inv.to_3x3()
    fwd_world = rig.staff_face_forward(meshes)
    fwd_cam = (rot @ fwd_world).normalized()
    head = next((o for o in meshes if o.name.split(".")[0] == "Head"), None)
    if not head:
        print("FAIL staff-founder-idle missing_head")
        raise SystemExit(1)
    hc = sum((head.matrix_world @ v.co for v in head.data.vertices), Vector()) / len(
        head.data.vertices
    )
    head_cam = cam_inv @ hc
    toward_cam = (-head_cam)
    if toward_cam.length < 1e-5:
        bpy.data.objects.remove(cam, do_unlink=True)
        print("FAIL staff-founder-idle head_on_camera")
        raise SystemExit(1)
    toward_cam.normalize()
    face_toward = fwd_cam.dot(toward_cam)
    nose_d, back_d = 1e9, -1.0
    cam_loc = cam.matrix_world.translation
    for v in head.data.vertices:
        w = head.matrix_world @ v.co
        rel = w - hc
        up = Vector((0.0, 0.0, 1.0))
        rel = rel - up * rel.dot(up)
        if rel.length < 1e-5:
            continue
        d_cam = (w - cam_loc).length
        ndot = rel.normalized().dot(fwd_world)
        if ndot > 0 and d_cam < nose_d:
            nose_d = d_cam
        if ndot < 0 and d_cam > back_d:
            back_d = d_cam
    bpy.data.objects.remove(cam, do_unlink=True)
    if nose_d >= back_d:
        print(f"FAIL staff-founder-idle back_closer nose={nose_d:.3f} back={back_d:.3f}")
        raise SystemExit(1)
    if face_toward < MIN_FACE_DOT:
        print(f"FAIL staff-founder-idle face_cam_dot={face_toward:.2f} want>={MIN_FACE_DOT}")
        raise SystemExit(1)
    print(f"OK staff-founder-idle face_cam_dot={face_toward:.2f}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()