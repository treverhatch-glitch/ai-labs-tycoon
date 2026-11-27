"""Regression: legacy Kenney Y-up bake + old -90° Z flip inverts preview screen-up."""
from __future__ import annotations

import math
import sys
from pathlib import Path

import bpy
from bpy_extras.object_utils import world_to_camera_view
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_preview_rig as rig  # noqa: E402
import kenney_founder_lib as kf  # noqa: E402

REPO = Path(sys.argv[-1])


def _parts_from_baked(ob: bpy.types.Object) -> list[bpy.types.Object]:
    ob.rotation_euler = (math.pi / 2, 0.0, 0.0)
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    bpy.ops.object.transform_apply(rotation=True)
    ob.rotation_euler = (0.0, 0.0, -math.pi / 2)
    bpy.ops.object.transform_apply(rotation=True)
    return kf._split_parts(ob)


def _screen_ok(meshes: list[bpy.types.Object]) -> bool:
    by = {o.name.split(".")[0]: o for o in meshes}
    head, feet = by.get("Head"), [by.get(n) for n in ("Foot_1", "Foot_-1")]
    feet = [f for f in feet if f]
    if not head or len(feet) < 2:
        return False
    cam, _ = rig.frame_staff_front_quarter(meshes, 50.0, 1.46, 30.0, 11.0)
    scene = bpy.context.scene
    scene.camera = cam

    def cen(o: bpy.types.Object) -> Vector:
        pts = [o.matrix_world @ v.co for v in o.data.vertices]
        return sum(pts, Vector()) / len(pts)

    hy = world_to_camera_view(scene, cam, cen(head)).y
    fy = sum(world_to_camera_view(scene, cam, cen(f)).y for f in feet) / len(feet)
    bpy.data.objects.remove(cam, do_unlink=True)
    return hy > fy


def main() -> None:
    arm, mesh = kf._import_rigged()
    kf._apply_idle_rest_pose(arm)
    kf._ground_rig(arm, mesh)
    baked = kf._bake_mesh(mesh, arm)
    legacy_parts = _parts_from_baked(baked)
    if _screen_ok(legacy_parts):
        print("FAIL staff-founder-idle legacy_bake_should_invert_screen")
        raise SystemExit(1)
    print("OK staff-founder-idle legacy_bake_inverts_screen")
    raise SystemExit(0)


if __name__ == "__main__":
    main()