"""Kenney armature: joint pivot heads align to parent tails (rest pose)."""
from __future__ import annotations

from mathutils import Vector

# Default: child bone head vs parent bone tail in rest pose (metres).
PIVOT_TOL_M = 0.008
# Kenney stock root + neck clip (documented FBX rest offsets, not hero edits).
PIVOT_TOL_OVERRIDES: dict[tuple[str, str], float] = {
    ("Hips", "Spine"): 0.080,
    ("UpperChest", "Neck"): 0.022,
}

CHAINS: dict[str, list[str]] = {
    "spine": ["Hips", "Spine", "Chest", "UpperChest", "Neck", "Head"],
    "arm_l": ["LeftShoulder", "LeftArm", "LeftForeArm", "LeftHand"],
    "arm_r": ["RightShoulder", "RightArm", "RightForeArm", "RightHand"],
    "leg_l": ["LeftUpLeg", "LeftLeg", "LeftFoot", "LeftToes"],
    "leg_r": ["RightUpLeg", "RightLeg", "RightFoot", "RightToes"],
}

REQUIRED_BONES = frozenset(b for chain in CHAINS.values() for b in chain)


def _joint_gap(arm, parent: str, child: str) -> float | None:
    bones = arm.data.bones
    pa = bones.get(parent)
    cb = bones.get(child)
    if not pa or not cb:
        return None
    return (Vector(cb.head_local) - Vector(pa.tail_local)).length


def verify_joint_pivots(arm) -> tuple[bool, str]:
    missing = sorted(REQUIRED_BONES - {b.name for b in arm.data.bones})
    if missing:
        return False, f"pivot_bones_missing={','.join(missing[:6])}"
    worst = 0.0
    worst_joint = ""
    for chain_name, chain in CHAINS.items():
        for parent, child in zip(chain, chain[1:], strict=False):
            gap = _joint_gap(arm, parent, child)
            if gap is None:
                return False, f"pivot_gap_{chain_name}_{parent}_{child}"
            worst = max(worst, gap)
            tol = PIVOT_TOL_OVERRIDES.get((parent, child), PIVOT_TOL_M)
            if gap > tol:
                worst_joint = f"{chain_name}:{parent}->{child}"
                return (
                    False,
                    f"pivot_{worst_joint} gap={gap:.4f}m max={tol:.4f}m",
                )
    return True, (
        f"pivot_joints_ok worst={worst:.4f}m default_tol={PIVOT_TOL_M:.4f}m"
        f" overrides={len(PIVOT_TOL_OVERRIDES)}"
    )


def pivot_gap_table(arm) -> list[dict[str, str | float]]:
    """Bone head vs parent tail gaps for PR / audit (metres)."""
    rows: list[dict[str, str | float]] = []
    focus = (
        ("shoulder", "Chest", "LeftShoulder"),
        ("shoulder", "Chest", "RightShoulder"),
        ("elbow", "LeftArm", "LeftForeArm"),
        ("elbow", "RightArm", "RightForeArm"),
        ("hip", "Hips", "LeftUpLeg"),
        ("hip", "Hips", "RightUpLeg"),
        ("knee", "LeftUpLeg", "LeftLeg"),
        ("knee", "RightUpLeg", "RightLeg"),
    )
    for label, parent, child in focus:
        gap = _joint_gap(arm, parent, child)
        if gap is None:
            rows.append({"joint": label, "parent": parent, "child": child, "gap_m": -1.0})
            continue
        tol = PIVOT_TOL_OVERRIDES.get((parent, child), PIVOT_TOL_M)
        rows.append(
            {
                "joint": label,
                "parent": parent,
                "child": child,
                "gap_m": round(gap, 5),
                "tol_m": tol,
            }
        )
    return rows