"""Staff hero facing: head/chest/toes/knees/travel vs toe-forward within 15°."""
from __future__ import annotations

import math
from dataclasses import dataclass

import bpy
from mathutils import Vector

from founder_face_decals import EYE_OBJECT_NAMES
from kenney_face_axis import eye_landmarks_world, toe_forward


MAX_FACING_DELTA_DEG = 15.0


def _xy_unit(v: Vector) -> Vector:
    out = Vector((v.x, v.y, 0.0))
    if out.length < 1e-6:
        return Vector((0.0, -1.0, 0.0))
    return out.normalized()


def _angle_deg(a: Vector, b: Vector) -> float:
    au = _xy_unit(a)
    bu = _xy_unit(b)
    return math.degrees(math.acos(max(-1.0, min(1.0, au.dot(bu)))))


def _bone_dir_eval(arm_ev, name: str) -> Vector | None:
    pb = arm_ev.pose.bones.get(name)
    if not pb:
        return None
    head = arm_ev.matrix_world @ pb.head
    tail = arm_ev.matrix_world @ pb.tail
    d = tail - head
    if d.length < 1e-6:
        return None
    return d


def _chest_forward(arm: bpy.types.Object) -> Vector:
    dg = bpy.context.evaluated_depsgraph_get()
    arm_ev = arm.evaluated_get(dg)
    for name in ("Chest", "Spine", "UpperChest"):
        pb = arm_ev.pose.bones.get(name)
        if not pb:
            continue
        y = (arm_ev.matrix_world.to_3x3() @ pb.matrix.to_3x3() @ Vector((0.0, 1.0, 0.0))).normalized()
        if y.length > 0.5:
            return _xy_unit(y)
    return _xy_unit(Vector((0.0, -1.0, 0.0)))


def _head_forward(mesh_obs: list[bpy.types.Object], body: bpy.types.Object) -> Vector:
    fwd = toe_forward(mesh_obs).normalized()
    body_meshes = [o for o in mesh_obs if o.type == "MESH" and "Eye" not in o.name.split(".")[0]]
    if not body_meshes:
        body_meshes = [body]
    eyes = [o for o in mesh_obs if o.type == "MESH" and o.name.split(".")[0] in EYE_OBJECT_NAMES]
    if len(eyes) >= 2:
        left = next((e for e in eyes if e.name.split(".")[0].endswith("_L")), eyes[0])
        right = next((e for e in eyes if e.name.split(".")[0].endswith("_R")), eyes[-1])
        cl = sum((left.matrix_world @ v.co for v in left.data.vertices), Vector()) / len(left.data.vertices)
        cr = sum((right.matrix_world @ v.co for v in right.data.vertices), Vector()) / len(right.data.vertices)
        mid = (cl + cr) * 0.5
        inter = cr - cl
        if inter.length > 1e-5:
            right_xy = _xy_unit(Vector((-fwd.y, fwd.x, 0.0)))
            if inter.normalized().dot(right_xy) < 0:
                inter = -inter
            nose = mid + _xy_unit(fwd) * (inter.length * 0.85)
            axis = nose - mid
            if axis.dot(fwd) < 0:
                axis = -axis
            if axis.length > 1e-5:
                return _xy_unit(axis)
    try:
        el, er, _ = eye_landmarks_world(body)
        mid = (el + er) * 0.5
        hc = sum((body.matrix_world @ v.co for v in body.data.vertices), Vector()) / len(body.data.vertices)
        axis = mid - hc
        if axis.dot(fwd) < 0:
            axis = -axis
        if axis.length > 1e-5:
            return _xy_unit(axis)
    except Exception:
        pass
    return _xy_unit(fwd)


def _foot_forward(arm: bpy.types.Object, side: str) -> Vector | None:
    dg = bpy.context.evaluated_depsgraph_get()
    arm_ev = arm.evaluated_get(dg)
    for bone in (f"{side}Toes", f"{side}Foot"):
        d = _bone_dir_eval(arm_ev, bone)
        if d:
            return _xy_unit(d)
    return None


def _shin_backward_dot(arm: bpy.types.Object, side: str, travel: Vector) -> float:
    dg = bpy.context.evaluated_depsgraph_get()
    arm_ev = arm.evaluated_get(dg)
    knee = arm_ev.pose.bones.get(f"{side}Leg")
    foot = arm_ev.pose.bones.get(f"{side}Foot")
    if not knee or not foot:
        return -1.0
    kh = arm_ev.matrix_world @ knee.head
    fh = arm_ev.matrix_world @ foot.head
    shin = fh - kh
    if shin.length < 1e-6:
        return -1.0
    return shin.normalized().dot(travel)


@dataclass
class FacingReport:
    travel_deg: float
    travel_motion_deg: float
    head_deg: float
    chest_deg: float
    foot_l_deg: float
    foot_r_deg: float
    knee_l_deg: float
    knee_r_deg: float
    shin_l_dot: float
    shin_r_dot: float
    ref: tuple[float, float]

    def max_delta(self) -> float:
        return max(
            self.head_deg,
            self.chest_deg,
            self.foot_l_deg,
            self.foot_r_deg,
            self.knee_l_deg,
            self.knee_r_deg,
            self.travel_deg,
            self.travel_motion_deg,
        )

    def as_dict(self) -> dict:
        return {
            "ref_xy": [round(self.ref[0], 4), round(self.ref[1], 4)],
            "head_deg": round(self.head_deg, 2),
            "chest_deg": round(self.chest_deg, 2),
            "foot_l_deg": round(self.foot_l_deg, 2),
            "foot_r_deg": round(self.foot_r_deg, 2),
            "knee_l_deg": round(self.knee_l_deg, 2),
            "knee_r_deg": round(self.knee_r_deg, 2),
            "travel_deg": round(self.travel_deg, 2),
            "travel_motion_deg": round(self.travel_motion_deg, 2),
            "shin_l_dot_travel": round(self.shin_l_dot, 3),
            "shin_r_dot_travel": round(self.shin_r_dot, 3),
            "max_delta_deg": round(self.max_delta(), 2),
        }


def _foot_mid_xy(arm: bpy.types.Object) -> Vector | None:
    dg = bpy.context.evaluated_depsgraph_get()
    arm_ev = arm.evaluated_get(dg)
    pts: list[Vector] = []
    for side in ("Left", "Right"):
        for name in (f"{side}Foot", f"{side}Toes"):
            pb = arm_ev.pose.bones.get(name)
            if pb:
                w = arm_ev.matrix_world @ pb.head
                pts.append(Vector((w.x, w.y, 0.0)))
    if not pts:
        return None
    return sum(pts, Vector()) / len(pts)


def _body_com_xy(body: bpy.types.Object) -> Vector:
    pts = [body.matrix_world @ v.co for v in body.data.vertices]
    c = sum(pts, Vector()) / max(1, len(pts))
    return Vector((c.x, c.y, 0.0))


def walk_travel_forward(
    arm: bpy.types.Object,
    frame_a: int,
    frame_b: int,
    scene,
    *,
    body: bpy.types.Object | None = None,
) -> Vector:
    scene.frame_set(frame_a)
    bpy.context.view_layer.update()
    if body is not None:
        p0 = _body_com_xy(body)
    else:
        p0 = _foot_mid_xy(arm)
    scene.frame_set(frame_b)
    bpy.context.view_layer.update()
    if body is not None:
        p1 = _body_com_xy(body)
    else:
        p1 = _foot_mid_xy(arm)
    if p0 is None or p1 is None:
        return _xy_unit(toe_forward([arm]))
    d = p1 - p0
    if d.length < 0.0025:
        mesh_ref = [body] if body is not None else [arm]
        return _xy_unit(toe_forward(mesh_ref))
    return _xy_unit(d)


def _align_with_ref(axis: Vector, ref: Vector) -> Vector:
    a = _xy_unit(axis)
    if a.dot(ref) < 0:
        a = -a
    return a


def _hip_xy(arm: bpy.types.Object) -> Vector:
    dg = bpy.context.evaluated_depsgraph_get()
    arm_ev = arm.evaluated_get(dg)
    pb = arm_ev.pose.bones.get("Hips") or arm_ev.pose.bones.get("Root")
    if pb:
        w = arm_ev.matrix_world @ pb.head
        return Vector((w.x, w.y, 0.0))
    return Vector((0.0, 0.0, 0.0))


def measure_facing(
    mesh_obs: list[bpy.types.Object],
    arm: bpy.types.Object,
    body: bpy.types.Object,
    *,
    travel: Vector | None = None,
    travel_motion: Vector | None = None,
) -> FacingReport:
    ref = toe_forward(mesh_obs).normalized()
    ref_xy = (ref.x, ref.y)
    head = _align_with_ref(_head_forward(mesh_obs, body), ref)
    chest = _align_with_ref(_chest_forward(arm), ref)
    foot_l = _align_with_ref(_foot_forward(arm, "Left") or ref, ref)
    foot_r = _align_with_ref(_foot_forward(arm, "Right") or ref, ref)
    trav = _align_with_ref(travel if travel is not None else ref, ref)
    motion = _align_with_ref(travel_motion if travel_motion is not None else ref, ref)
    dg = bpy.context.evaluated_depsgraph_get()
    arm_ev = arm.evaluated_get(dg)
    knee_l = _bone_dir_eval(arm_ev, "LeftLeg")
    knee_r = _bone_dir_eval(arm_ev, "RightLeg")
    knee_l_dir = _align_with_ref(-knee_l if knee_l else -trav, ref)
    knee_r_dir = _align_with_ref(-knee_r if knee_r else -trav, ref)
    sl = _shin_backward_dot(arm, "Left", trav)
    sr = _shin_backward_dot(arm, "Right", trav)
    return FacingReport(
        travel_deg=_angle_deg(trav, ref),
        travel_motion_deg=_angle_deg(motion, ref),
        head_deg=_angle_deg(head, ref),
        chest_deg=_angle_deg(chest, ref),
        foot_l_deg=_angle_deg(foot_l, ref),
        foot_r_deg=_angle_deg(foot_r, ref),
        knee_l_deg=_angle_deg(knee_l_dir, ref),
        knee_r_deg=_angle_deg(knee_r_dir, ref),
        shin_l_dot=sl,
        shin_r_dot=sr,
        ref=ref_xy,
    )


def gate_facing_idle(report: FacingReport) -> tuple[bool, str]:
    peak = max(report.head_deg, report.chest_deg, report.foot_l_deg, report.foot_r_deg)
    if peak > MAX_FACING_DELTA_DEG:
        return False, f"idle_max_delta={peak:.1f} want<={MAX_FACING_DELTA_DEG}"
    if report.head_deg > 165:
        return False, f"head_backward head_deg={report.head_deg:.1f}"
    return True, "idle_facing_ok"


def gate_facing_walk(report: FacingReport) -> tuple[bool, str]:
    peak = max(
        report.head_deg,
        report.chest_deg,
        report.foot_l_deg,
        report.foot_r_deg,
        report.knee_l_deg,
        report.knee_r_deg,
    )
    if peak > MAX_FACING_DELTA_DEG:
        return False, f"walk_max_delta={peak:.1f} want<={MAX_FACING_DELTA_DEG}"
    if report.travel_motion_deg > MAX_FACING_DELTA_DEG:
        return (
            False,
            f"walk_travel_motion={report.travel_motion_deg:.1f} want<={MAX_FACING_DELTA_DEG}",
        )
    if report.shin_l_dot > 0.42 and report.shin_r_dot > 0.42:
        return (
            False,
            f"shin_both_forward dot_l={report.shin_l_dot:.2f} dot_r={report.shin_r_dot:.2f}",
        )
    if min(report.shin_l_dot, report.shin_r_dot) > 0.38:
        return (
            False,
            f"shin_forward dot_l={report.shin_l_dot:.2f} dot_r={report.shin_r_dot:.2f}",
        )
    return True, "walk_facing_ok"