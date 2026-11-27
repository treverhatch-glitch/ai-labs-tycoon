#!/usr/bin/env python3
"""FAIL#22+#23 pose walk gate — same armature pose as walk PNG renders."""
from __future__ import annotations

import os
import sys

os.environ.setdefault("RESEARCHER_STOCK_RECOLOR", "1")
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
from kenney_hero_walk_strip import (  # noqa: E402
    _ankle_spread_x,
    _spread_minmax_frames,
    build_researcher_walk_scene,
    measure_walk_frame_pose,
    pose_walk_for_render,
)

ANKLE_CONTACT_PASS_DELTA_M = 0.15
KNEE_PASS_MIN_DEG = 20.0
ARM_SWING_MIN_DEG = 15.0


def main() -> int:
    data = build_researcher_walk_scene()
    scene = bpy.context.scene
    arm = data.arm
    labels = data.labels

    rows: list[tuple[str, int, dict[str, float]]] = []
    for label, fr in zip(labels, data.walk_frames, strict=True):
        metrics = measure_walk_frame_pose(data, int(fr))
        rows.append((label, int(fr), metrics))
        act = arm.animation_data.action if arm.animation_data else None
        act_name = act.name if act else "none"
        print(
            f"OK researcher_walk_pose frame={int(fr)} label={label} action={act_name} "
            f"ankle_x={metrics['ankle_x']:.4f} knee_L={metrics['knee_l']:.2f} "
            f"knee_R={metrics['knee_r']:.2f} arm_swing_L={metrics['arm_swing_l']:.2f} "
            f"arm_swing_R={metrics['arm_swing_r']:.2f}"
        )

    by_label = {r[0]: r for r in rows}
    for contact in ("contact_L", "contact_R"):
        m = by_label[contact][2]
        knee = max(m["knee_l"], m["knee_r"])
        swing = max(m["arm_swing_l"], m["arm_swing_r"])
        if knee < KNEE_PASS_MIN_DEG:
            print(
                f"FAIL researcher_walk_pose contact={contact} knee={knee:.2f} "
                f"want>={KNEE_PASS_MIN_DEG}"
            )
            return 1
        if swing < ARM_SWING_MIN_DEG:
            print(
                f"FAIL researcher_walk_pose contact={contact} arm_swing={swing:.2f} "
                f"want>={ARM_SWING_MIN_DEG}"
            )
            return 1
    passing_spreads = [by_label["down"][2]["ankle_x"], by_label["up"][2]["ankle_x"]]
    contact_spreads = [by_label["contact_L"][2]["ankle_x"], by_label["contact_R"][2]["ankle_x"]]
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
    pose_walk_for_render(scene, data, lo_fr)
    lo_sp = _ankle_spread_x(arm)
    pose_walk_for_render(scene, data, hi_fr)
    hi_sp = _ankle_spread_x(arm)
    clip_delta = hi_sp - lo_sp
    ankle_pass = max(ankle_delta, clip_delta)
    if ankle_pass < ANKLE_CONTACT_PASS_DELTA_M:
        print(
            f"FAIL researcher_walk_pose ankle_contact_pass_delta={ankle_delta:.4f} "
            f"clip_delta={clip_delta:.4f} want>={ANKLE_CONTACT_PASS_DELTA_M}"
        )
        return 1

    passing_knee = max(
        by_label["down"][2]["knee_l"],
        by_label["down"][2]["knee_r"],
        by_label["up"][2]["knee_l"],
        by_label["up"][2]["knee_r"],
    )
    if passing_knee < KNEE_PASS_MIN_DEG:
        print(
            f"FAIL researcher_walk_pose passing_knee={passing_knee:.2f} "
            f"want>={KNEE_PASS_MIN_DEG}"
        )
        return 1

    passing_swing = max(
        by_label["down"][2]["arm_swing_l"],
        by_label["down"][2]["arm_swing_r"],
        by_label["up"][2]["arm_swing_l"],
        by_label["up"][2]["arm_swing_r"],
    )
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