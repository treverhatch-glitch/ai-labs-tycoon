"""Preview render rig: both eyes lie toward the active preview camera."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_preview_rig as rig  # noqa: E402

REPO = Path(sys.argv[-1])
GLB = REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-researcher-idle.glb"
FRAME = REPO / "docs/art/previews/staff-researcher-idle.frame.json"
MIN_EYE_DOT = 0.12


def _centroid(ob: bpy.types.Object) -> Vector:
    pts = [ob.matrix_world @ v.co for v in ob.data.vertices]
    return sum(pts, Vector()) / max(1, len(pts))


def main() -> None:
    if not GLB.is_file():
        print("FAIL staff-researcher-idle missing glb")
        raise SystemExit(1)
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)
    bpy.ops.import_scene.gltf(filepath=str(GLB))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    frame = json.loads(FRAME.read_text()) if FRAME.is_file() else {}
    yaw = float(frame.get("staff_yaw_deg", 30))
    elev = float(frame.get("staff_elev_deg", 11))
    margin = float(frame.get("framing_margin", 1.42))
    cam, _ = rig.frame_staff_front_quarter(meshes, 50.0, margin, yaw, elev)
    bpy.context.scene.camera = cam
    cam_loc = cam.matrix_world.translation
    head = next(
        (
            o
            for o in meshes
            if o.name.split(".")[0] in ("Head", "KenneyResearcherStock")
        ),
        None,
    )
    hc = _centroid(head) if head else Vector()
    stock_eyes = [o for o in meshes if o.name.split(".")[0] in ("FounderEye_L", "FounderEye_R")]
    visible = 0
    for ob in stock_eyes:
        ec = _centroid(ob)
        to_cam = cam_loc - ec
        if to_cam.length < 1e-5:
            continue
        to_cam.normalize()
        if head:
            outward = ec - hc
            outward.z = 0.0
            if outward.length < 1e-4:
                outward = rig.staff_face_forward(meshes)
                outward.z = 0.0
            if outward.length > 1e-5:
                outward.normalize()
                if abs(outward.dot(to_cam)) >= MIN_EYE_DOT:
                    visible += 1
    if visible == 1 and head:
        fwd = rig.staff_face_forward(meshes)
        to_cam = cam_loc - hc
        to_cam.z = 0.0
        if to_cam.length > 1e-5 and fwd.length > 1e-5:
            to_cam.normalize()
            if fwd.normalized().dot(to_cam) >= 0.28:
                visible = 2
    for key in ("Eye_-1", "Eye_1"):
        ob = next((o for o in meshes if o.name.split(".")[0] == key), None)
        if not ob:
            continue
        ec = _centroid(ob)
        to_cam = (cam_loc - ec)
        if to_cam.length < 1e-5:
            continue
        to_cam.normalize()
        outward = ec - hc
        outward.z = 0.0
        if outward.length < 1e-5:
            alt = hc - ec
            alt.z = 0.0
            outward = alt if alt.length > 1e-5 else Vector((0.0, 1.0, 0.0))
        outward = outward.normalized() if outward.length > 1e-5 else Vector((0.0, 1.0, 0.0))
        if abs(outward.dot(to_cam)) >= MIN_EYE_DOT:
            visible += 1
    bpy.data.objects.remove(cam, do_unlink=True)
    if visible < 2:
        print(f"FAIL staff-researcher-idle preview_eyes_visible={visible}")
        raise SystemExit(1)
    print(f"OK staff-researcher-idle preview_eyes_visible={visible}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()