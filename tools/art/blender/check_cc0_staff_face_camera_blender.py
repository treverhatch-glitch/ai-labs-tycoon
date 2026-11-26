"""Gate: camera looks toward the character face (+Y), not the back of the head."""
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
    cam_loc = cam.matrix_world.translation.copy()
    mins = Vector((1e9, 1e9, 1e9))
    maxs = Vector((-1e9, -1e9, -1e9))
    for ob in meshes:
        for v in ob.data.vertices:
            w = ob.matrix_world @ v.co
            for i in range(3):
                mins[i] = min(mins[i], w[i])
                maxs[i] = max(maxs[i], w[i])
    center = (mins + maxs) * 0.5
    cam_fwd = (center - cam_loc).normalized()
    cam_fwd.z = 0
    fwd = rig.staff_face_forward(meshes)
    fwd.z = 0
    if cam_fwd.length < 1e-4 or fwd.length < 1e-4:
        bpy.data.objects.remove(cam, do_unlink=True)
        print("FAIL staff-founder-idle face_axis_degenerate")
        raise SystemExit(1)
    dot = cam_fwd.normalized().dot(fwd.normalized())  # negative when cam looks toward face
    head = next((o for o in meshes if o.name.split(".")[0] == "Head"), None)
    if head:
        hc = sum((head.matrix_world @ v.co for v in head.data.vertices), Vector()) / len(head.data.vertices)
        nose_d, back_d = 1e9, -1.0
        for v in head.data.vertices:
            w = head.matrix_world @ v.co
            rel = w - hc
            rel.z = 0
            if rel.length < 1e-5:
                continue
            d_cam = (w - cam_loc).length
            ndot = rel.normalized().dot(fwd)
            if ndot > 0 and d_cam < nose_d:
                nose_d = d_cam
            if ndot < 0 and d_cam > back_d:
                back_d = d_cam
        if nose_d >= back_d:
            bpy.data.objects.remove(cam, do_unlink=True)
            print(f"FAIL staff-founder-idle back_closer nose={nose_d:.3f} back={back_d:.3f}")
            raise SystemExit(1)
    bpy.data.objects.remove(cam, do_unlink=True)
    if dot > -0.45:
        print(f"FAIL staff-founder-idle camera_not_facing_face dot={dot:.2f}")
        raise SystemExit(1)
    print(f"OK staff-founder-idle face_camera_dot={dot:.2f}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()