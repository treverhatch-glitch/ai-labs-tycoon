"""Chibi staff humanoids (Y-up, grounded, head ~38% height, overlapping parts)."""
from __future__ import annotations

import blender_common as bc


def _feet_and_hands(shoulder_w: float, hand_y: float, foot_y: float) -> None:
    for sx in (-1, 1):
        bc.add_cube(f"Foot_{sx}", (0.16, 0.07, 0.26), (0.13 * sx, foot_y + 0.03, 0.02 * sx))
        bc.add_cube(f"Hand_{sx}", (0.10, 0.08, 0.09), (shoulder_w * 0.52 * sx, hand_y, 0.05 * sx))


def build_human_base(shoulder_w: float = 0.40) -> None:
    foot_y = 0.06
    leg_h = 0.40
    leg_cy = foot_y + leg_h * 0.5
    hip_y = foot_y + leg_h - 0.02
    torso_y = hip_y + 0.22
    neck_y = torso_y + 0.24
    head_h = 0.52
    head_y = neck_y + head_h * 0.5
    arm_y = torso_y - 0.02
    shoulder_y = torso_y + 0.16
    hand_y = arm_y - 0.22

    for sx in (-1, 1):
        bc.add_cylinder(f"Leg_{sx}", 0.11, leg_h, (0.13 * sx, leg_cy, 0.0), vertices=12, axis="Y")
    bc.add_cube("Hips", (0.40, 0.18, 0.26), (0, hip_y, 0))
    bc.add_cube("Torso", (shoulder_w, 0.38, 0.28), (0, torso_y, 0))
    bc.add_cube("Neck", (0.14, 0.10, 0.14), (0, neck_y, 0))
    head = bc.add_cube("Head", (0.40, head_h, 0.36), (0, head_y, 0))
    bc.assign_solid(head, (0.88, 0.72, 0.58, 1.0), 0.55)
    for sx in (-1, 1):
        eye = bc.add_cube(f"Eye_{sx}", (0.06, 0.05, 0.03), (0.10 * sx, head_y + 0.08, 0.17))
        bc.assign_solid(eye, (0.12, 0.14, 0.22, 1.0), 0.22)
    mouth = bc.add_cube("Mouth", (0.08, 0.03, 0.02), (0, head_y - 0.12, 0.17))
    bc.assign_solid(mouth, (0.78, 0.38, 0.42, 1.0), 0.45)
    for sx in (-1, 1):
        bc.add_cylinder(f"Arm_{sx}", 0.078, 0.28, (shoulder_w * 0.36 * sx, arm_y, 0.0), vertices=10, axis="Y")
        bc.add_cube(f"Shoulder_{sx}", (0.12, 0.10, 0.13), (shoulder_w * 0.42 * sx, shoulder_y, 0))
    _feet_and_hands(shoulder_w, hand_y, foot_y)
    core = bc.add_cube("SpineCore", (0.14, torso_y - hip_y + 0.12, 0.12), (0, (hip_y + torso_y) * 0.5, 0))
    bc.assign_solid(core, (0.22, 0.26, 0.42, 1.0), 0.62)


def build_founder_idle() -> None:
    build_human_base(0.42)
    hood_y = 0.06 + 0.40 + 0.54
    bc.add_cube("Hood", (0.48, 0.36, 0.32), (0, hood_y, -0.02))
    bc.add_cube("Hair", (0.32, 0.08, 0.26), (0, hood_y + 0.28, 0.02))
    mug = bc.add_cylinder("FounderMug", 0.07, 0.14, (0.22, 0.52, 0.12), vertices=12, axis="Y")
    bc.assign_solid(mug, (0.92, 0.48, 0.12, 1.0), 0.35)
    bc.add_cube("MugHandle", (0.04, 0.10, 0.06), (0.28, 0.52, 0.14))


def build_gpu_tech_idle() -> None:
    build_human_base(0.41)
    head_y = 0.06 + 0.40 + 0.20 + 0.24 + 0.24
    bc.add_cube("HeadsetBand", (0.30, 0.05, 0.05), (0, head_y + 0.22, -0.02))
    bc.add_cube("HeadsetCup_L", (0.11, 0.09, 0.07), (-0.15, head_y + 0.16, 0.02))
    bc.add_cube("HeadsetCup_R", (0.11, 0.09, 0.07), (0.15, head_y + 0.16, 0.02))
    card = bc.add_cube("GpuCard", (0.28, 0.18, 0.04), (0.18, 0.62, 0.18))
    bc.assign_solid(card, (0.12, 0.48, 0.82, 1.0), 0.28)
    bc.add_cube("GpuLed", (0.06, 0.04, 0.02), (0.26, 0.64, 0.20))
    bc.add_cylinder("Wrench", 0.028, 0.28, (-0.22, 0.62, 0.14), vertices=6, axis="Y")


def build_safety_idle() -> None:
    build_human_base(0.41)
    torso_y = 0.06 + 0.40 - 0.02 + 0.22
    bc.add_cube("HiVisVest", (0.48, 0.50, 0.30), (0, torso_y, 0.01))
    bc.add_cube("HiVisStripe", (0.44, 0.07, 0.31), (0, torso_y - 0.12, 0.02))
    bc.add_cube("HardHat", (0.38, 0.12, 0.38), (0, torso_y + 0.54, 0))
    shield = bc.add_cube("BadgeShield", (0.12, 0.14, 0.03), (0.18, torso_y + 0.05, 0.16))
    bc.assign_solid(shield, (0.92, 0.82, 0.18, 1.0), 0.35)


def build_sales_idle() -> None:
    build_human_base(0.44)
    torso_y = 0.06 + 0.40 - 0.02 + 0.22
    head_y = torso_y + 0.24 + 0.10 + 0.24
    blazer = bc.add_cube("Blazer", (0.48, 0.52, 0.30), (0, torso_y, 0.01))
    bc.assign_solid(blazer, (0.12, 0.22, 0.55, 1.0), 0.38)
    bc.add_cube("HeadsetBandSales", (0.32, 0.05, 0.05), (0, head_y + 0.22, -0.02))
    bc.add_cube("HeadsetMic", (0.04, 0.18, 0.04), (0.12, head_y + 0.08, 0.16))
    bc.add_cube("HeadsetCupL", (0.12, 0.10, 0.08), (-0.16, head_y + 0.16, 0.02))
    bc.add_cube("HeadsetCupR", (0.12, 0.10, 0.08), (0.16, head_y + 0.16, 0.02))


def build_researcher_idle() -> None:
    build_human_base(0.38)
    torso_y = 0.06 + 0.40 - 0.02 + 0.22
    head_y = torso_y + 0.24 + 0.10 + 0.24
    bc.add_cube("LabCoat", (0.44, 0.66, 0.28), (0, torso_y - 0.02, 0.02))
    bc.add_cube("Glasses", (0.26, 0.05, 0.10), (0, head_y + 0.04, 0.15))
    bc.add_cube("Clipboard", (0.22, 0.32, 0.04), (0.22, 0.58, 0.12))
    bc.add_cube("ClipMetal", (0.08, 0.14, 0.03), (0.30, 0.62, 0.13))


def build_engineer_work() -> None:
    build_human_base(0.44)
    torso_y = 0.06 + 0.40 - 0.02 + 0.22
    head_y = torso_y + 0.24 + 0.10 + 0.24
    hat = bc.add_cube("HardHatEng", (0.40, 0.12, 0.40), (0, head_y + 0.28, 0))
    bc.assign_solid(hat, (0.92, 0.78, 0.12, 1.0), 0.42)
    bc.add_cube("LaptopBase", (0.40, 0.07, 0.30), (0.14, 0.58, 0.12))
    bc.add_cube("LaptopScreen", (0.34, 0.24, 0.05), (0.14, 0.68, 0.24))
    screen = bc.add_cube("LaptopScreenEmit", (0.28, 0.20, 0.014), (0.14, 0.68, 0.26))
    bc.assign_emission(screen, (0.18, 0.72, 0.38, 1.0), strength=3.0)


def build_recruiter_idle() -> None:
    build_human_base(0.40)
    torso_y = 0.06 + 0.40 - 0.02 + 0.22
    blazer = bc.add_cube("BlazerRec", (0.44, 0.52, 0.28), (0, torso_y, 0))
    bc.assign_solid(blazer, (0.42, 0.16, 0.22, 1.0), 0.48)
    bc.add_cube("Tie", (0.07, 0.26, 0.03), (0, torso_y + 0.10, 0.14))
    phone = bc.add_cube("Phone", (0.07, 0.14, 0.05), (0.26, 0.58, 0.12))
    bc.assign_solid(phone, (0.08, 0.08, 0.10, 1.0), 0.22)
    case = bc.add_cube("Briefcase", (0.28, 0.22, 0.12), (-0.24, 0.50, 0.14))
    bc.assign_solid(case, (0.55, 0.38, 0.18, 1.0), 0.42)
    bc.add_cube("BadgeLanyard", (0.06, 0.14, 0.02), (0.06, torso_y + 0.20, 0.16))


def build_pose_typing() -> None:
    foot_y = 0.10
    leg_h = 0.28
    hip_y = foot_y + leg_h
    torso_y = hip_y + 0.18
    head_h = 0.44
    head_y = torso_y + 0.20 + head_h * 0.5
    shoulder_w = 0.40
    for sx in (-1, 1):
        bc.add_cylinder(f"Leg_{sx}", 0.10, leg_h, (0.16 * sx, foot_y + leg_h * 0.5, 0.0), vertices=12, axis="Y")
    bc.add_cube("Hips", (0.38, 0.14, 0.24), (0, hip_y, 0))
    bc.add_cube("Torso", (shoulder_w, 0.34, 0.24), (0, torso_y, 0))
    head = bc.add_cube("Head", (0.38, head_h, 0.34), (0, head_y, 0))
    bc.assign_solid(head, (0.88, 0.72, 0.58, 1.0), 0.55)
    for sx in (-1, 1):
        bc.add_cylinder(
            f"Arm_{sx}", 0.072, 0.22, (shoulder_w * 0.34 * sx, torso_y - 0.04, 0.08), vertices=10, axis="Y"
        )
        hand = bc.add_cube(f"Hand_{sx}", (0.10, 0.08, 0.09), (shoulder_w * 0.50 * sx, 0.58, 0.22))
        bc.assign_solid(hand, (0.88, 0.72, 0.58, 1.0), 0.55)
    mouse = bc.add_cube("Mouse", (0.10, 0.05, 0.14), (0.22, 0.56, 0.34))
    bc.assign_solid(mouse, (0.12, 0.12, 0.14, 1.0), 0.32)
    chair = bc.add_cube("ChairBack", (0.36, 0.42, 0.08), (0, 0.72, -0.08))
    bc.assign_solid(chair, (0.22, 0.24, 0.28, 1.0), 0.48)
    bc.add_cube("DeskEdge", (0.78, 0.08, 0.48), (0, 0.58, 0.28))
    bc.add_cube("Keyboard", (0.40, 0.04, 0.20), (0, 0.62, 0.32))
    bc.add_cube("Monitor", (0.42, 0.28, 0.05), (0, 0.86, 0.30))
    screen = bc.add_cube("MonitorScreen", (0.34, 0.22, 0.022), (0, 0.86, 0.31))
    bc.assign_emission(screen, (0.22, 0.62, 0.95, 1.0), strength=2.8)