"""Gate: staff hero camera sees face closer than back of head (front 3/4)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_preview_rig as rig  # noqa: E402

REPO = Path(sys.argv[-1])
HERO = "staff-founder-idle"
GLB = REPO / "unity/Assets/Art/EarlyGame/staff-kit" / f"{HERO}.glb"
FRAME = REPO / "docs/art/previews" / f"{HERO}.frame.json"


def _mesh_by_prefix(name: str, obs: list[bpy.types.Object]) -> bpy.types.Object | None:
    for ob in obs:
        if ob.type != "MESH":
            continue
        if ob.name.split(".")[0] == name:
            return ob
    return None


def _gate_for_imported(obs: list[bpy.types.Object]) -> tuple[bool, str]:
    head = _mesh_by_prefix("Head", obs) or _mesh_by_prefix("KenneyFounderStock", obs)
    if not head or not head.data.vertices:
        return False, "missing_head"
    frame = json.loads(FRAME.read_text()) if FRAME.is_file() else {}
    yaw = float(frame.get("staff_yaw_deg", 42.0))
    elev = float(frame.get("staff_elev_deg", 11.0))
    mesh_obs = [o for o in obs if o.type == "MESH"]
    cam, _ = rig.frame_staff_front_quarter(
        mesh_obs, lens_mm=50.0, margin=1.46, yaw_deg=yaw, elev_deg=elev
    )
    cam_loc = cam.matrix_world.translation.copy()
    mins, maxs = rig.mesh_bounds_world(mesh_obs)
    center = (mins + maxs) * 0.5
    head_c = sum((head.matrix_world @ v.co for v in head.data.vertices), Vector()) / len(head.data.vertices)
    to_cam = (cam_loc - head_c).normalized()
    face_min = 1e9
    back_min = 1e9
    for v in head.data.vertices:
        w = head.matrix_world @ v.co
        rel = w - head_c
        if rel.length < 1e-6:
            continue
        d = (w - cam_loc).length
        side = rel.normalized().dot(to_cam)
        if side > 0.02:
            face_min = min(face_min, d)
        elif side < -0.02:
            back_min = min(back_min, d)
    bpy.data.objects.remove(cam, do_unlink=True)
    if face_min >= back_min or face_min > 1e8:
        return False, f"back_closer face_d={face_min:.3f} back_d={back_min:.3f}"
    return True, f"front_ok face_d={face_min:.3f} back_d={back_min:.3f} yaw={yaw:.0f}"


def main() -> None:
    if not GLB.is_file():
        print(f"FAIL {HERO} missing glb")
        raise SystemExit(1)
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)
    before = set(bpy.context.scene.objects)
    bpy.ops.import_scene.gltf(filepath=str(GLB))
    imported = [o for o in bpy.context.scene.objects if o not in before]
    mesh_obs = [o for o in imported if o.type == "MESH"]
    ok, msg = _gate_for_imported(mesh_obs)
    for ob in imported:
        bpy.data.objects.remove(ob, do_unlink=True)
    if ok:
        print(f"OK {HERO} {msg}")
        raise SystemExit(0)
    print(f"FAIL {HERO} {msg}")
    raise SystemExit(1)


if __name__ == "__main__":
    main()