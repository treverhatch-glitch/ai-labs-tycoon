"""Chibi staff (art Y-up build lane → export rotates to Z-up ~1.72 m)."""
from __future__ import annotations

import blender_common as bc

# Stylised ~1.0 art units tall before export scale (→ 1.72 m).
H = 1.0
LEG_H = 0.45 * H
TORSO_H = 0.30 * H
HEAD_H = 0.19 * H
NECK_H = 0.06 * H
LEG_R = 0.065
FOOT_Y = 0.02


def _rig(shoulder_w: float = 0.38) -> dict[str, float]:
    leg_cy = FOOT_Y + LEG_H * 0.5
    hip_y = FOOT_Y + LEG_H - 0.12
    torso_y = hip_y + TORSO_H * 0.34
    neck_y = torso_y + TORSO_H * 0.32
    head_y = neck_y + (NECK_H + HEAD_H) * 0.5 - 0.07
    shoulder_y = torso_y + TORSO_H * 0.22
    arm_y = torso_y + TORSO_H * 0.05
    arm_len = TORSO_H * 0.46
    hand_y = shoulder_y - 0.02 - arm_len - 0.02
    return {
        "shoulder_w": shoulder_w,
        "leg_cy": leg_cy,
        "hip_y": hip_y,
        "torso_y": torso_y,
        "neck_y": neck_y,
        "head_y": head_y,
        "shoulder_y": shoulder_y,
        "arm_y": arm_y,
        "hand_y": hand_y,
        "foot_y": FOOT_Y,
    }


def _skeleton(shoulder_w: float = 0.38) -> dict[str, float]:
    r = _rig(shoulder_w)
    sw = r["shoulder_w"]
    hy, ty, ny, head_y = r["hip_y"], r["torso_y"], r["neck_y"], r["head_y"]
    for sx in (-1, 1):
        bc.add_cylinder(
            f"Leg_{sx}",
            LEG_R,
            LEG_H,
            (0.11 * sx, r["leg_cy"], 0.0),
            vertices=12,
            axis="Y",
        )
        bc.add_cube(f"Foot_{sx}", (0.14, 0.05, 0.22), (0.11 * sx, r["foot_y"] + 0.02, 0.0))
    bc.add_cube("Hips", (0.36, 0.14, 0.22), (0, hy, 0))
    bc.add_cube("Torso", (sw, TORSO_H, 0.24), (0, ty, 0))
    bc.add_cube("Neck", (0.13, NECK_H, 0.12), (0, ny, 0))
    head = bc.add_cube("Head", (0.36, HEAD_H, 0.30), (0, head_y, 0))
    bc.assign_solid(head, (0.88, 0.72, 0.58, 1.0), 0.55)
    face_z = 0.08
    for sx in (-1, 1):
        eye = bc.add_cube(f"Eye_{sx}", (0.055, 0.045, 0.04), (0.07 * sx, head_y + 0.01, face_z))
        bc.assign_solid(eye, (0.12, 0.14, 0.22, 1.0), 0.22)
        pupil = bc.add_cube(f"Pupil_{sx}", (0.038, 0.038, 0.022), (0.07 * sx, head_y + 0.01, face_z + 0.012))
        bc.assign_solid(pupil, (0.03, 0.03, 0.05, 1.0), 0.12)
    mouth = bc.add_cube("Mouth", (0.09, 0.04, 0.025), (0, head_y - 0.05, face_z))
    bc.assign_solid(mouth, (0.78, 0.38, 0.42, 1.0), 0.45)
    for sx in (-1, 1):
        arm_top = r["shoulder_y"] - 0.02
        arm_len = TORSO_H * 0.46
        bc.add_cylinder(
            f"Arm_{sx}",
            0.055,
            arm_len,
            (sw * 0.34 * sx, arm_top - arm_len * 0.5, 0.02),
            vertices=10,
            axis="Y",
        )
        bc.add_cube(
            f"Shoulder_{sx}",
            (0.13, 0.10, 0.12),
            (sw * 0.34 * sx, r["shoulder_y"], 0),
        )
        hand_y = arm_top - arm_len - 0.02
        hand = bc.add_cube(f"Hand_{sx}", (0.09, 0.07, 0.08), (sw * 0.48 * sx, hand_y, 0.04))
        bc.assign_solid(hand, (0.88, 0.72, 0.58, 1.0), 0.55)
    core = bc.add_cube(
        "SpineCore",
        (0.12, TORSO_H * 0.85, 0.10),
        (0, (hy + ty) * 0.5, 0),
    )
    bc.assign_solid(core, (0.22, 0.26, 0.42, 1.0), 0.62)
    return r


def build_human_base(shoulder_w: float = 0.38) -> dict[str, float]:
    return _skeleton(shoulder_w)


def _attach_headwear(name: str, size: tuple[float, float, float], r: dict[str, float], y_off: float) -> None:
    """Hair / hats sit on head crown with overlap (no floating gap)."""
    crown = r["head_y"] + HEAD_H * 0.5 - size[1] * 0.52 + y_off
    bc.add_cube(name, size, (0, crown, 0))


def build_founder_idle() -> None:
    r = build_human_base(0.40)
    _attach_headwear("Hood", (0.44, 0.28, 0.28), r, 0.0)
    hair_y = r["head_y"] + HEAD_H * 0.5 - 0.02
    bc.add_cube("Hair", (0.26, 0.06, 0.18), (0, hair_y, 0.02))
    mug = bc.add_cylinder("FounderMug", 0.06, 0.12, (0.20, r["hand_y"] + 0.02, 0.12), vertices=12, axis="Y")
    bc.assign_solid(mug, (0.92, 0.48, 0.12, 1.0), 0.35)
    bc.add_cube("MugHandle", (0.04, 0.08, 0.05), (0.26, r["hand_y"] + 0.02, 0.13))


def build_gpu_tech_idle() -> None:
    r = build_human_base(0.39)
    hy, head_y = r["head_y"], r["head_y"]
    bc.add_cube("HeadsetBand", (0.28, 0.04, 0.04), (0, head_y + HEAD_H * 0.40, -0.01))
    for sx, tag in ((-1, "L"), (1, "R")):
        bc.add_cube(
            f"HeadsetCup_{tag}",
            (0.10, 0.08, 0.06),
            (0.11 * sx, head_y + HEAD_H * 0.12, 0.01),
        )
    hy = r["hand_y"]
    card = bc.add_cube("GpuCard", (0.26, 0.16, 0.04), (0.39 * 0.48, hy, 0.04))
    bc.assign_solid(card, (0.12, 0.48, 0.82, 1.0), 0.28)
    bc.add_cube("GpuLed", (0.05, 0.03, 0.02), (0.22, hy + 0.02, 0.05))
    bc.add_cylinder("Wrench", 0.025, 0.22, (-0.20, hy, 0.04), vertices=6, axis="Y")


def build_safety_idle() -> None:
    r = build_human_base(0.39)
    ty, head_y = r["torso_y"], r["head_y"]
    bc.add_cube("HiVisVest", (0.44, TORSO_H * 1.02, 0.28), (0, ty, 0.01))
    bc.add_cube("HiVisStripe", (0.40, 0.06, 0.29), (0, ty - TORSO_H * 0.2, 0.02))
    hat = bc.add_cube("HardHat", (0.36, 0.10, 0.34), (0, head_y + HEAD_H * 0.42, 0))
    bc.assign_solid(hat, (0.92, 0.78, 0.12, 1.0), 0.42)
    shield = bc.add_cube("BadgeShield", (0.11, 0.12, 0.025), (0.16, ty + 0.04, 0.14))
    bc.assign_solid(shield, (0.92, 0.82, 0.18, 1.0), 0.35)


def build_sales_idle() -> None:
    r = build_human_base(0.41)
    ty, head_y = r["torso_y"], r["head_y"]
    blazer = bc.add_cube("Blazer", (0.44, TORSO_H * 1.05, 0.28), (0, ty, 0.01))
    bc.assign_solid(blazer, (0.12, 0.22, 0.55, 1.0), 0.38)
    bc.add_cube("HeadsetBandSales", (0.28, 0.04, 0.04), (0, head_y + HEAD_H * 0.40, -0.02))
    bc.add_cube("HeadsetMic", (0.04, 0.14, 0.035), (0.08, head_y + 0.01, 0.10))
    for sx, tag in ((-1, "L"), (1, "R")):
        bc.add_cube(f"HeadsetCup{tag}", (0.10, 0.08, 0.06), (0.11 * sx, head_y + HEAD_H * 0.12, 0.01))


def build_researcher_idle() -> None:
    r = build_human_base(0.37)
    ty, head_y = r["torso_y"], r["head_y"]
    bc.add_cube("LabCoat", (0.40, TORSO_H * 1.15, 0.26), (0, ty - 0.01, 0.02))
    bc.add_cube("Glasses", (0.24, 0.04, 0.08), (0, head_y + 0.01, 0.10))
    bc.add_cube("Clipboard", (0.20, 0.28, 0.035), (0.37 * 0.48, r["hand_y"], 0.04))
    bc.add_cube("ClipMetal", (0.07, 0.12, 0.025), (0.28, r["hand_y"] + 0.10, 0.13))


def build_engineer_work() -> None:
    r = build_human_base(0.41)
    head_y = r["head_y"]
    hat = bc.add_cube("HardHatEng", (0.38, 0.10, 0.36), (0, head_y + HEAD_H * 0.40, 0))
    bc.assign_solid(hat, (0.92, 0.78, 0.12, 1.0), 0.42)
    base_y = FOOT_Y + 0.05
    bc.add_cube("LaptopBase", (0.36, 0.06, 0.28), (0.12, base_y, 0.14))
    bc.add_cube("LaptopScreen", (0.30, 0.20, 0.045), (0.12, base_y + 0.14, 0.22))
    screen = bc.add_cube("LaptopScreenEmit", (0.24, 0.16, 0.012), (0.12, base_y + 0.14, 0.24))
    bc.assign_emission(screen, (0.18, 0.72, 0.38, 1.0), strength=3.0)


def build_recruiter_idle() -> None:
    r = build_human_base(0.39)
    ty = r["torso_y"]
    blazer = bc.add_cube("BlazerRec", (0.42, TORSO_H * 1.02, 0.26), (0, ty, 0))
    bc.assign_solid(blazer, (0.42, 0.16, 0.22, 1.0), 0.48)
    bc.add_cube("Tie", (0.07, 0.24, 0.03), (0, ty, 0.10))
    phone = bc.add_cube("Phone", (0.06, 0.12, 0.045), (0.39 * 0.48, r["hand_y"], 0.04))
    bc.assign_solid(phone, (0.08, 0.08, 0.10, 1.0), 0.22)
    case = bc.add_cube("Briefcase", (0.26, 0.20, 0.11), (-0.22, FOOT_Y + 0.04, 0.12))
    bc.assign_solid(case, (0.55, 0.38, 0.18, 1.0), 0.42)
    bc.add_cube("BadgeLanyard", (0.05, 0.12, 0.02), (0.05, ty + 0.14, 0.15))


def build_pose_typing() -> None:
    sw = 0.38
    foot_y = 0.02
    leg_h = LEG_H * 0.65
    hip_y = foot_y + leg_h - 0.06
    torso_y = hip_y + TORSO_H * 0.34
    neck_y = torso_y + TORSO_H * 0.28
    head_y = neck_y + (NECK_H + HEAD_H * 0.95) * 0.5 - 0.07
    for sx in (-1, 1):
        bc.add_cylinder(f"Leg_{sx}", LEG_R * 0.95, leg_h, (0.11 * sx, foot_y + leg_h * 0.5, 0.0), vertices=12, axis="Y")
        bc.add_cube(f"Foot_{sx}", (0.13, 0.05, 0.20), (0.11 * sx, foot_y + 0.02, 0.0))
    bc.add_cube("Hips", (0.36, 0.12, 0.22), (0, hip_y, 0))
    bc.add_cube("Torso", (sw, TORSO_H * 0.85, 0.22), (0, torso_y, 0))
    bc.add_cube("Neck", (0.12, NECK_H * 0.9, 0.11), (0, neck_y, 0))
    head = bc.add_cube("Head", (0.34, HEAD_H * 0.95, 0.28), (0, head_y, 0))
    bc.assign_solid(head, (0.88, 0.72, 0.58, 1.0), 0.55)
    for sx in (-1, 1):
        arm_top = torso_y + TORSO_H * 0.05
        arm_len = TORSO_H * 0.38
        bc.add_cylinder(
            f"Arm_{sx}",
            0.05,
            arm_len,
            (sw * 0.34 * sx, arm_top - arm_len * 0.5, 0.06),
            vertices=10,
            axis="Y",
        )
        hand_y = arm_top - arm_len - 0.01
        hand = bc.add_cube(f"Hand_{sx}", (0.08, 0.06, 0.08), (sw * 0.46 * sx, hand_y, 0.06))
        bc.assign_solid(hand, (0.88, 0.72, 0.58, 1.0), 0.55)
    desk_y = foot_y + 0.02
    bc.add_cube("DeskEdge", (0.72, 0.06, 0.42), (0, desk_y + 0.36, 0.22))
    bc.add_cube("Keyboard", (0.36, 0.035, 0.18), (0, desk_y + 0.40, 0.24))
    mouse = bc.add_cube("Mouse", (0.08, 0.04, 0.12), (0.20, desk_y + 0.38, 0.26))
    bc.assign_solid(mouse, (0.12, 0.12, 0.14, 1.0), 0.32)
    chair = bc.add_cube("ChairBack", (0.32, TORSO_H * 1.1, 0.07), (0, torso_y, -0.05))
    bc.assign_solid(chair, (0.22, 0.24, 0.28, 1.0), 0.48)
    bc.add_cube("Monitor", (0.38, 0.24, 0.045), (0, desk_y + 0.58, 0.26))
    screen = bc.add_cube("MonitorScreen", (0.32, 0.18, 0.018), (0, desk_y + 0.58, 0.27))
    bc.assign_emission(screen, (0.22, 0.62, 0.95, 1.0), strength=2.8)