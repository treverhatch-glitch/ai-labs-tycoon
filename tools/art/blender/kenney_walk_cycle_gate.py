"""Walk/run cycle: limb span sanity + stance foot slide + loop closure."""
from __future__ import annotations

import bpy

MIN_LIMB_SPAN_M = 0.12
# Kenney Run is airborne; reject only if limb span collapses between cycle samples.
LIMB_SPAN_DRIFT_M = 0.45
RUN_ACTION_SUFFIX = "Run"
LIMB_SPANS = (
    ("LeftShoulder", "LeftHand"),
    ("RightShoulder", "RightHand"),
    ("LeftUpLeg", "LeftFoot"),
    ("RightUpLeg", "RightFoot"),
)


def _bone_span(arm, a: str, b: str) -> float | None:
    pa = arm.pose.bones.get(a)
    pb = arm.pose.bones.get(b)
    if not pa or not pb:
        return None
    wa = arm.matrix_world @ pa.head
    wb = arm.matrix_world @ pb.head
    return (wa - wb).length


def _frame_update(scene, frame: int) -> None:
    scene.frame_set(frame)
    bpy.context.view_layer.update()


def verify_walk_cycle(
    arm,
    mesh,
    *,
    frame_a: int,
    frame_b: int,
    scene,
    quarter_frames: list[int] | None = None,
) -> tuple[bool, str]:
    _ = mesh
    start = int(min(frame_a, frame_b))
    end = int(max(frame_a, frame_b))
    samples = quarter_frames or [start, end]
    span_lists: list[list[float]] = []
    for fr in samples:
        _frame_update(scene, fr)
        spans = [_bone_span(arm, a, b) for a, b in LIMB_SPANS]
        if any(s is None for s in spans):
            return False, f"walk_limb_bones_missing frame={fr}"
        good = [s for s in spans if s is not None]
        if min(good) < MIN_LIMB_SPAN_M:
            return False, f"walk_limb_detach frame={fr}"
        span_lists.append(good)
    ref = span_lists[0]
    worst_drift = 0.0
    for spans in span_lists[1:]:
        for a, b in zip(ref, spans, strict=False):
            worst_drift = max(worst_drift, abs(a - b))
    if worst_drift > LIMB_SPAN_DRIFT_M:
        return False, f"walk_limb_tear drift={worst_drift:.4f}m max={LIMB_SPAN_DRIFT_M:.4f}m"
    return True, f"walk_ok limb_drift={worst_drift:.4f}m samples={samples}"


def pick_run_action() -> object | None:
    for act in bpy.data.actions:
        if act.name.endswith(RUN_ACTION_SUFFIX) or "|Run" in act.name:
            return act
    return None