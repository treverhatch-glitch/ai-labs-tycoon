"""Chibi staff humanoids (Y-up cylinders, grounded at y=0, per-part materials)."""
from __future__ import annotations

import blender_common as bc


def _y(y: float) -> float:
    return 0.05 + y * 0.68


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
        bc.add_cylinder(f"Leg_{sx}", 0.11, leg_h, (0.13 * sx, leg_h * 0.5, 0.0), vertices=10, axis="Y")
    bc.add_cube("Hips", (0.38, 0.16, 0.24), (0, hip_y, 0))
    bc.add_cube("Torso", (shoulder_w, 0.36, 0.26), (0, torso_y, 0))
    bc.add_cube("Neck", (0.12, 0.08, 0.12), (0, neck_y, 0))
    bc.add_cube("Head", (0.36, 0.42, 0.34), (0, head_y, 0))
    for sx in (-1, 1):
        eye = bc.add_cube(f"Eye_{sx}", (0.05, 0.04, 0.02), (0.085 * sx, head_y + 0.05, 0.15))
        bc.assign_solid(eye, (0.12, 0.14, 0.22, 1.0), 0.22)
    mouth = bc.add_cube("Mouth", (0.07, 0.025, 0.02), (0, head_y - 0.07, 0.15))
    bc.assign_solid(mouth, (0.78, 0.38, 0.42, 1.0), 0.45)
    for sx in (-1, 1):
        bc.add_cylinder(f"Arm_{sx}", 0.078, 0.30, (shoulder_w * 0.36 * sx, arm_y, 0.0), vertices=8, axis="Y")
        bc.add_cube(f"Shoulder_{sx}", (0.12, 0.10, 0.13), (shoulder_w * 0.42 * sx, shoulder_y, 0))
    _feet_and_hands(shoulder_w, hand_y, foot_y)


def build_founder_idle() -> None:
    build_human_base(0.42)
    bc.add_cube("Hood", (0.46, 0.34, 0.30), (0, _y(1.24), -0.02))
    bc.add_cube("Hair", (0.30, 0.07, 0.24), (0, _y(1.69), 0.02))
    mug = bc.add_cylinder("FounderMug", 0.07, 0.14, (0.22, _y(0.88), 0.12), vertices=12, axis="Y")
    bc.assign_solid(mug, (0.92, 0.48, 0.12, 1.0), 0.35)
    bc.add_cube("MugHandle", (0.04, 0.10, 0.06), (0.28, _y(0.88), 0.14))


def build_gpu_tech_idle() -> None:
    build_human_base(0.41)
    bc.add_cube("HeadsetBand", (0.28, 0.05, 0.05), (0, _y(1.62), -0.02))
    bc.add_cube("HeadsetCup_L", (0.11, 0.09, 0.07), (-0.15, _y(1.56), 0.02))
    bc.add_cube("HeadsetCup_R", (0.11, 0.09, 0.07), (0.15, _y(1.56), 0.02))
    card = bc.add_cube("GpuCard", (0.28, 0.18, 0.04), (0.18, _y(0.92), 0.18))
    bc.assign_solid(card, (0.12, 0.48, 0.82, 1.0), 0.28)
    bc.add_cube("GpuLed", (0.06, 0.04, 0.02), (0.26, _y(0.94), 0.20))
    bc.add_cylinder("Wrench", 0.028, 0.28, (-0.22, _y(0.92), 0.14), vertices=6, axis="Y")


def build_safety_idle() -> None:
    build_human_base(0.41)
    bc.add_cube("HiVisVest", (0.46, 0.48, 0.28), (0, _y(1.15), 0.01))
    bc.add_cube("HiVisStripe", (0.42, 0.07, 0.29), (0, _y(1.05), 0.02))
    bc.add_cube("HardHat", (0.36, 0.11, 0.36), (0, _y(1.69), 0))


def build_sales_idle() -> None:
    build_human_base(0.44)
    blazer = bc.add_cube("Blazer", (0.46, 0.52, 0.28), (0, _y(1.17), 0.01))
    bc.assign_solid(blazer, (0.12, 0.22, 0.55, 1.0), 0.38)
    bc.add_cube("HeadsetBandSales", (0.30, 0.05, 0.05), (0, _y(1.63), -0.02))
    bc.add_cube("HeadsetMic", (0.04, 0.18, 0.04), (0.12, _y(1.52), 0.16))
    bc.add_cube("HeadsetCupL", (0.12, 0.10, 0.08), (-0.16, _y(1.56), 0.02))
    bc.add_cube("HeadsetCupR", (0.12, 0.10, 0.08), (0.16, _y(1.56), 0.02))


def build_researcher_idle() -> None:
    build_human_base(0.38)
    bc.add_cube("LabCoat", (0.42, 0.64, 0.26), (0, _y(1.02), 0.02))
    bc.add_cube("Glasses", (0.24, 0.05, 0.09), (0, _y(1.58), 0.14))
    bc.add_cube("Clipboard", (0.22, 0.32, 0.04), (0.22, _y(0.98), 0.12))
    bc.add_cube("ClipMetal", (0.08, 0.14, 0.03), (0.30, _y(1.02), 0.13))


def build_engineer_work() -> None:
    build_human_base(0.44)
    bc.add_cube("HardHatEng", (0.38, 0.11, 0.38), (0, _y(1.69), 0))
    bc.add_cube("LaptopBase", (0.40, 0.07, 0.30), (0.14, _y(0.98), 0.10))
    bc.add_cube("LaptopScreen", (0.34, 0.24, 0.05), (0.14, _y(1.08), 0.22))
    screen = bc.add_cube("LaptopScreenEmit", (0.28, 0.20, 0.014), (0.08, _y(1.08), 0.28))
    bc.assign_emission(screen, (0.18, 0.72, 0.38, 1.0), strength=3.0)


def build_recruiter_idle() -> None:
    build_human_base(0.40)
    blazer = bc.add_cube("BlazerRec", (0.42, 0.50, 0.28), (0, _y(1.16), 0))
    bc.assign_solid(blazer, (0.42, 0.16, 0.22, 1.0), 0.48)
    bc.add_cube("Tie", (0.07, 0.26, 0.03), (0, _y(1.28), 0.14))
    phone = bc.add_cube("Phone", (0.07, 0.14, 0.05), (0.26, _y(0.98), 0.12))
    bc.assign_solid(phone, (0.08, 0.08, 0.10, 1.0), 0.22)
    case = bc.add_cube("Briefcase", (0.28, 0.22, 0.12), (-0.24, _y(0.84), 0.14))
    bc.assign_solid(case, (0.55, 0.38, 0.18, 1.0), 0.42)
    bc.add_cube("BadgeLanyard", (0.06, 0.14, 0.02), (0.06, _y(1.10), 0.16))


def build_pose_typing() -> None:
    leg_h = 0.32
    hip_y = leg_h + 0.10
    torso_y = hip_y + 0.22
    head_y = torso_y + 0.38
    shoulder_w = 0.40
    for sx in (-1, 1):
        bc.add_cylinder(f"Leg_{sx}", 0.10, leg_h, (0.16 * sx, leg_h * 0.5, 0.22), vertices=10, axis="Y")
    bc.add_cube("Hips", (0.38, 0.14, 0.24), (0, hip_y, 0.18))
    bc.add_cube("Torso", (shoulder_w, 0.34, 0.24), (0, torso_y, 0.12))
    bc.add_cube("Head", (0.36, 0.42, 0.34), (0, head_y, 0.10))
    for sx in (-1, 1):
        bc.add_cylinder(
            f"Arm_{sx}", 0.072, 0.22, (shoulder_w * 0.34 * sx, torso_y - 0.04, 0.20), vertices=12, axis="Y"
        )
        hand = bc.add_cube(f"Hand_{sx}", (0.10, 0.08, 0.09), (shoulder_w * 0.50 * sx, _y(0.78), 0.26))
        bc.assign_solid(hand, (0.88, 0.72, 0.58, 1.0), 0.55)
    bc.add_cube("DeskEdge", (0.78, 0.08, 0.48), (0, _y(0.62), 0.28))
    bc.add_cube("Keyboard", (0.40, 0.04, 0.20), (0, _y(0.80), 0.32))
    bc.add_cube("KeyFace", (0.36, 0.012, 0.15), (0, _y(0.825), 0.36))
    mouse = bc.add_cube("Mouse", (0.10, 0.05, 0.14), (0.22, _y(0.78), 0.34))
    bc.assign_solid(mouse, (0.12, 0.12, 0.14, 1.0), 0.32)
    chair = bc.add_cube("ChairBack", (0.36, 0.42, 0.08), (0, _y(0.92), -0.08))
    bc.assign_solid(chair, (0.22, 0.24, 0.28, 1.0), 0.48)
    bc.add_cube("Monitor", (0.42, 0.28, 0.05), (0, _y(1.04), 0.30))
    screen = bc.add_cube("MonitorScreen", (0.34, 0.22, 0.022), (0, _y(1.04), 0.31))
    bc.assign_emission(screen, (0.22, 0.62, 0.95, 1.0), strength=2.8)