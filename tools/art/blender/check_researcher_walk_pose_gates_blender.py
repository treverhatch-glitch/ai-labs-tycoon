#!/usr/bin/env python3
"""FAIL#22 pose walk gate — ankle spread, knee flex, arm swing per gait tile frame."""
from __future__ import annotations

import math
import sys
from pathlib import Path

import bpy
from mathutils import Vector

sys.path.insert(0, str(Path(__file__).resolve().parent))
from kenney_hero_walk_strip import (  # noqa: E402
    _ankle_spread_x,
    _spread_minmax_frames,
    apply_idle_arm_blend,
    build_researcher_walk_scene,
)

ANKLE_CONTACT_PASS_DELTA_M = 0.15
KNEE_PASS_MIN_DEG = 20.0
ARM_SWING_MIN_DEG = 15.0


def _bone_world(arm: bpy.types.Object, name: str) -> Vector | None:
    pb = arm.pose.bones.get(name)
    if not pb:
        return None
    return arm.matrix_world @ pb.head


def _bone_tail_world(arm: bpy.types.Object, name: str) -> Vector | None:
    pb = arm.pose.bones.get(name)
    if not pb:
        return None
    return arm.matrix_world @ (pb.head + pb.tail)


def _angle_deg(a: Vector, b: Vector, c: Vector) -> float:
    u = (a - b).normalized()
    v = (c - b).normalized()
    dot = max(-1.0, min(1.0, u.dot(v)))
    return math.degrees(math.acos(dot))


def _knee_flex_deg(arm: bpy.types.Object, side: str) -> float:
    hip = _bone_world(arm, f"{side}UpLeg")
    knee = _bone_world(arm, f"{side}Leg")
    foot = _bone_tail_world(arm, f"{side}Leg") or _bone_world(arm, f"{side}Foot")
    if not hip or not knee or not foot:
        return 0.0
    straight = 180.0
    return straight - _angle_deg(hip, knee, foot)


def _upper_arm_swing_deg(arm: bpy.types.Object, side: str, bind_quats: dict) -> float:
    ua = arm.pose.bones.get(f"{side}Arm")
    if not ua:
        return 0.0
    bind = bind_quats.get(f"{side}Arm")
    if not bind:
        return 0.0
    dq = ua.rotation_quaternion @ bind.inverted()
    return abs(math.degrees(dq.angle))


def main() -> int:
    data = build_researcher_walk_scene()
    scene = bpy.context.scene
    arm = data.arm
    labels = data.labels

    rows: list[tuple[str, int, float, float, float, float, float]] = []
    for label, fr in zip(labels, data.walk_frames, strict=True):
        scene.frame_set(int(fr))
        apply_idle_arm_blend(arm, data.idle_arm_quats, data.bind_arm_quats)
        spread = _ankle_spread_x(arm)
        knee_l = _knee_flex_deg(arm, "Left")
        knee_r = _knee_flex_deg(arm, "Right")
        knee_max = max(knee_l, knee_r)
        swing_l = _upper_arm_swing_deg(arm, "Left", data.idle_arm_quats)
        swing_r = _upper_arm_swing_deg(arm, "Right", data.idle_arm_quats)
        swing_max = max(swing_l, swing_r)
        rows.append((label, fr, spread, knee_max, swing_max, swing_l, swing_r))
        print(
            f"OK researcher_walk_pose frame={fr} label={label} "
            f"ankle_x={spread:.4f} knee_L={knee_l:.2f} knee_R={knee_r:.2f} "
            f"arm_swing_L={swing_l:.2f} arm_swing_R={swing_r:.2f}"
        )

    by_label = {r[0]: r for r in rows}
    contact_spreads = [
        by_label["contact_L"][2],
        by_label["contact_R"][2],
    ]
    passing_spreads = [by_label["down"][2], by_label["up"][2]]
    ankle_delta = max(
        max(contact_spreads) - min(passing_spreads),
        max(passing_spreads) - min(contact_spreads),
    )
    lo_fr, hi_fr = _spread_minmax_frames(
        arm,
        scene,
        scene.frame_start,
        scene.frame_end,
        data.bind_arm_quats,
        data.idle_arm_quats,
        forbidden=set(),
    )
    scene.frame_set(lo_fr)
    apply_idle_arm_blend(arm, data.idle_arm_quats, data.bind_arm_quats)
    lo_sp = _ankle_spread_x(arm)
    scene.frame_set(hi_fr)
    apply_idle_arm_blend(arm, data.idle_arm_quats, data.bind_arm_quats)
    hi_sp = _ankle_spread_x(arm)
    clip_delta = hi_sp - lo_sp
    ankle_pass = max(ankle_delta, clip_delta)
    if ankle_pass < ANKLE_CONTACT_PASS_DELTA_M:
        print(
            f"FAIL researcher_walk_pose ankle_contact_pass_delta={ankle_delta:.4f} "
            f"clip_delta={clip_delta:.4f} want>={ANKLE_CONTACT_PASS_DELTA_M}"
        )
        return 1

    passing_knee = max(by_label["down"][3], by_label["up"][3])
    if passing_knee < KNEE_PASS_MIN_DEG:
        print(
            f"FAIL researcher_walk_pose passing_knee={passing_knee:.2f} "
            f"want>={KNEE_PASS_MIN_DEG}"
        )
        return 1

    passing_swing = max(by_label["down"][4], by_label["up"][4])
    if passing_swing < ARM_SWING_MIN_DEG:
        print(
            f"FAIL researcher_walk_pose passing_arm_swing={passing_swing:.2f} "
            f"want>={ARM_SWING_MIN_DEG}"
        )
        return 1

    print(
        f"OK researcher_walk_pose ankle_delta={ankle_delta:.4f} "
        f"clip_delta={clip_delta:.4f} clip_frames=({lo_fr},{hi_fr}) "
        f"passing_knee={passing_knee:.2f} passing_arm_swing={passing_swing:.2f} "
        f"frames={list(data.walk_frames)}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())