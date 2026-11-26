"""Chibi staff humanoids for packet 2 (Y-up, grounded at y=0, per-part materials before bake)."""
from __future__ import annotations

import blender_common as bc

# Legacy tall rig Y → chibi scale (~2.5-head silhouette, full figure in frame).
def _y(y: float) -> float:
    return 0.05 + y * 0.68


def _chest_badge(rgba: tuple[float, float, float, float]) -> None:
    ob = bc.add_cube("RoleBadge", (0.32, 0.28, 0.05), (0.06, _y(1.12), 0.19))
    bc.assign_emission(ob, rgba, strength=4.2)
    sash = bc.add_cube("RoleSash", (0.10, 0.48, 0.06), (-0.15, _y(1.04), 0.13))
    bc.assign_emission(sash, (rgba[2], rgba[0], rgba[1], 1.0), strength=3.2)


def _feet_and_hands(shoulder_w: float, hand_y: float, foot_y: float) -> None:
    for sx in (-1, 1):
        bc.add_cube(f"Foot_{sx}", (0.16, 0.07, 0.26), (0.13 * sx, foot_y, 0.02 * sx))
        bc.add_cube(f"Hand_{sx}", (0.10, 0.08, 0.09), (shoulder_w * 0.52 * sx, hand_y, 0.05 * sx))


def build_human_base(shoulder_w: float = 0.40) -> None:
    leg_h = 0.46
    hip_y = _y(0.80)
    torso_y = _y(1.18)
    neck_y = _y(1.43)
    head_y = _y(1.57)
    arm_y = _y(1.05)
    shoulder_y = _y(1.22)
    hand_y = _y(0.84)
    foot_y = _y(0.03)

    for sx in (-1, 1):
        bc.add_cylinder(f"Leg_{sx}", 0.11, leg_h, (0.13 * sx, leg_h * 0.5, 0.0), vertices=10)
    bc.add_cube("Hips", (0.38, 0.16, 0.24), (0, hip_y, 0))
    bc.add_cube("Torso", (shoulder_w, 0.36, 0.26), (0, torso_y, 0))
    bc.add_cube("Neck", (0.12, 0.08, 0.12), (0, neck_y, 0))
    bc.add_cube("Head", (0.32, 0.34, 0.30), (0, head_y, 0))
    for sx in (-1, 1):
        bc.add_cube(f"Eye_{sx}", (0.055, 0.045, 0.02), (0.085 * sx, head_y + 0.05, 0.15))
    bc.add_cube("Mouth", (0.07, 0.025, 0.02), (0, head_y - 0.07, 0.15))
    for sx in (-1, 1):
        bc.add_cylinder(f"Arm_{sx}", 0.078, 0.30, (shoulder_w * 0.36 * sx, arm_y, 0.0), vertices=8)
        bc.add_cube(f"Shoulder_{sx}", (0.12, 0.10, 0.13), (shoulder_w * 0.42 * sx, shoulder_y, 0))
    _feet_and_hands(shoulder_w, hand_y, foot_y)


def build_founder_idle() -> None:
    build_human_base(0.42)
    _chest_badge((0.58, 0.14, 0.98, 1.0))
    bc.add_cube("Hood", (0.46, 0.34, 0.30), (0, _y(1.24), -0.02))
    bc.add_cube("HoodPocket", (0.20, 0.15, 0.05), (0.12, _y(1.02), 0.13))
    bc.add_cube("Hair", (0.30, 0.07, 0.24), (0, _y(1.69), 0.02))
    bc.add_cube("LaptopStrut", (0.09, 0.13, 0.09), (0.16, _y(1.0), 0.08))
    bc.add_cube("LaptopBase", (0.38, 0.06, 0.28), (0.14, _y(0.98), 0.14))
    bc.add_cube("LaptopScreen", (0.52, 0.36, 0.04), (0.12, _y(1.08), 0.30))
    screen = bc.add_cube("LaptopScreenEmit", (0.26, 0.18, 0.012), (0.14, _y(1.06), 0.26))
    bc.assign_emission(screen, (0.25, 0.85, 0.55, 1.0), strength=2.5)


def build_gpu_tech_idle() -> None:
    build_human_base(0.41)
    _chest_badge((0.08, 0.92, 0.38, 1.0))
    bc.add_cylinder("Cap", 0.15, 0.08, (0, _y(1.69), 0), vertices=12)
    for sx in (-1, 1):
        bc.add_cube(f"Glove_{sx}", (0.11, 0.09, 0.11), (0.22 * sx, _y(0.84), 0.08))
    bc.add_cube("HeadsetBand", (0.28, 0.05, 0.05), (0, _y(1.62), -0.02))
    bc.add_cube("HeadsetCup_L", (0.11, 0.09, 0.07), (-0.15, _y(1.56), 0.02))
    bc.add_cube("HeadsetCup_R", (0.11, 0.09, 0.07), (0.15, _y(1.56), 0.02))
    bc.add_cube("Toolbelt", (0.44, 0.11, 0.19), (0, _y(0.88), 0.12))
    bc.add_cube("Pouch", (0.11, 0.13, 0.09), (0.20, _y(0.90), 0.16))
    bc.add_cylinder("Screwdriver", 0.028, 0.20, (0.24, _y(0.92), 0.20), vertices=6)


def build_safety_idle() -> None:
    build_human_base(0.41)
    _chest_badge((0.98, 0.88, 0.05, 1.0))
    bc.add_cube("HiVisVest", (0.46, 0.48, 0.28), (0, _y(1.15), 0.01))
    bc.add_cube("HiVisStripe", (0.42, 0.07, 0.29), (0, _y(1.05), 0.02))
    bc.add_cube("Helmet", (0.32, 0.11, 0.32), (0, _y(1.69), 0))
    bc.add_cube("BadgeClip", (0.13, 0.16, 0.05), (0.18, _y(1.08), 0.14))


def build_sales_idle() -> None:
    build_human_base(0.44)
    _chest_badge((0.12, 0.22, 0.88, 1.0))
    bc.add_cube("Blazer", (0.46, 0.52, 0.28), (0, _y(1.17), 0.01))
    bc.add_cube("BlazerLapel", (0.13, 0.24, 0.04), (0.08, _y(1.18), 0.14))
    bc.add_cube("PhoneStrut", (0.07, 0.11, 0.07), (0.20, _y(1.00), 0.06))
    bc.add_cube("Phone", (0.07, 0.13, 0.05), (0.28, _y(1.02), 0.10))
    bc.add_cube("BriefStrut", (0.08, 0.15, 0.08), (-0.14, _y(0.98), 0.04))
    bc.add_cube("BriefEdge", (0.38, 0.42, 0.10), (-0.32, _y(0.72), 0.22))


def build_researcher_idle() -> None:
    build_human_base(0.38)
    _chest_badge((0.18, 0.62, 0.95, 1.0))
    bc.add_cube("LabCoat", (0.42, 0.64, 0.26), (0, _y(1.02), 0.02))
    bc.add_cube("Glasses", (0.24, 0.05, 0.09), (0, _y(1.58), 0.14))
    bc.add_cube("GlassesLensL", (0.09, 0.07, 0.02), (-0.07, _y(1.58), 0.15))
    bc.add_cube("GlassesLensR", (0.09, 0.07, 0.02), (0.07, _y(1.58), 0.15))
    bc.add_cube("Ponytail", (0.09, 0.20, 0.11), (-0.16, _y(1.62), -0.05))
    bc.add_cube("Clipboard", (0.18, 0.24, 0.04), (0.20, _y(0.98), 0.08))


def build_engineer_work() -> None:
    build_human_base(0.44)
    _chest_badge((0.95, 0.42, 0.05, 1.0))
    bc.add_cube("HardHat", (0.38, 0.11, 0.38), (0, _y(1.69), 0))
    bc.add_cylinder("Wrench", 0.038, 0.52, (-0.28, _y(1.05), 0.20), vertices=6)
    bc.add_cube("WrenchHead", (0.09, 0.05, 0.04), (-0.28, _y(0.92), 0.14))
    bc.add_cube("LaptopStrut", (0.09, 0.15, 0.09), (0.10, _y(1.02), 0.08))
    bc.add_cube("LaptopBase", (0.40, 0.07, 0.30), (0.08, _y(1.00), 0.14))
    bc.add_cube("LaptopScreen", (0.34, 0.24, 0.05), (0.08, _y(1.08), 0.26))
    screen = bc.add_cube("LaptopScreenEmit", (0.28, 0.20, 0.014), (0.08, _y(1.08), 0.28))
    bc.assign_emission(screen, (0.18, 0.72, 0.38, 1.0), strength=3.0)


def build_recruiter_idle() -> None:
    build_human_base(0.40)
    _chest_badge((0.92, 0.12, 0.28, 1.0))
    bc.add_cube("BlazerRec", (0.42, 0.50, 0.28), (0, _y(1.16), 0))
    bc.add_cube("Tie", (0.07, 0.26, 0.03), (0, _y(1.28), 0.14))
    bc.add_cube("FolderStrut", (0.08, 0.15, 0.08), (-0.12, _y(1.00), 0.04))
    bc.add_cube("Folder", (0.38, 0.46, 0.08), (-0.38, _y(0.92), 0.28))


def build_pose_typing() -> None:
    build_human_base(0.42)
    _chest_badge((0.22, 0.82, 0.92, 1.0))
    bc.add_cube("DeskStrut", (0.11, 0.26, 0.11), (0.14, _y(0.92), 0.10))
    bc.add_cube("DeskEdge", (0.78, 0.08, 0.48), (0, _y(0.62), 0.28))
    bc.add_cube("Keyboard", (0.40, 0.04, 0.20), (0, _y(0.80), 0.22))
    bc.add_cube("KeyFace", (0.36, 0.012, 0.15), (0, _y(0.825), 0.28))
    bc.add_cube("Monitor", (0.56, 0.42, 0.05), (0, _y(1.02), 0.34))
    screen = bc.add_cube("MonitorScreen", (0.28, 0.20, 0.022), (0, _y(1.06), 0.25))
    bc.assign_emission(screen, (0.22, 0.62, 0.95, 1.0), strength=2.8)