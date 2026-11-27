"""GLB head above feet in preview camera (screen-space)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy
from bpy_extras.object_utils import world_to_camera_view
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_preview_rig as rig  # noqa: E402

_args = sys.argv[sys.argv.index("--") + 1 :] if "--" in sys.argv else [sys.argv[-1]]
REPO = Path(_args[0])
GLB = Path(_args[1]) if len(_args) > 1 else REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.glb"
FRAME = REPO / "docs/art/previews/staff-founder-idle.frame.json"
MIN_SCREEN_GAP = 0.08


def _centroid(ob: bpy.types.Object) -> Vector:
    pts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    return sum(pts, Vector()) / max(1, len(pts))


def main() -> None:
    if not GLB.is_file():
        print("FAIL staff-founder-idle missing glb")
        raise SystemExit(1)
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)
    bpy.ops.import_scene.gltf(filepath=str(GLB))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    by = {o.name.split(".")[0]: o for o in meshes}
    head = by.get("Head")
    feet = [by[n] for n in ("Foot_-1", "Foot_1", "Leg_-1", "Leg_1") if n in by]
    if not head or len(feet) < 2:
        print("FAIL staff-founder-idle screen_up_parts")
        raise SystemExit(1)
    frame = json.loads(FRAME.read_text()) if FRAME.is_file() else {}
    yaw = float(frame.get("staff_yaw_deg", 30))
    elev = float(frame.get("staff_elev_deg", 11))
    cam, _ = rig.frame_staff_front_quarter(meshes, 50.0, 1.46, yaw, elev)
    scene = bpy.context.scene
    scene.camera = cam
    bpy.context.view_layer.update()

    def view_y(world: Vector) -> float:
        co = world_to_camera_view(scene, cam, world)
        return co.y

    head_y = view_y(_centroid(head))
    foot_y = sum(view_y(_centroid(f)) for f in feet) / len(feet)
    bpy.data.objects.remove(cam, do_unlink=True)
    gap = head_y - foot_y
    if head_y <= foot_y:
        print(f"FAIL staff-founder-idle screen_inverted head_y={head_y:.4f} foot_y={foot_y:.4f}")
        raise SystemExit(1)
    if gap < MIN_SCREEN_GAP:
        print(f"FAIL staff-founder-idle screen_gap={gap:.4f}")
        raise SystemExit(1)
    print(f"OK staff-founder-idle screen_up head_y={head_y:.4f} foot_y={foot_y:.4f}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()