"""Chibi staff humanoid (Y-up build → Z-up multi-mesh glTF ~1.72 m)."""
from __future__ import annotations

import blender_common as bc

H = 1.0
LEG_H = 0.45 * H
TORSO_H = 0.30 * H
HEAD_H = 0.19 * H
NECK_H = 0.06 * H
HEAD_R = HEAD_H * 0.52
LEG_R = 0.052
FOOT_Y = 0.02


def _add_head(name: str, head_y: float):
    import bpy

    bpy.ops.mesh.primitive_uv_sphere_add(
        radius=HEAD_R, segments=20, ring_count=12, location=(0, head_y, 0)
    )
    ob = bpy.context.active_object
    ob.name = name
    bc.smooth_shade(ob)
    bc.assign_solid(ob, (0.88, 0.72, 0.58, 1.0), 0.55)
    return ob


def _rig(shoulder_w: float = 0.38) -> dict[str, float]:
    leg_cy = FOOT_Y + LEG_H * 0.5
    hip_y = FOOT_Y + LEG_H - 0.10
    torso_y = hip_y + TORSO_H * 0.22
    neck_y = torso_y + TORSO_H * 0.28
    head_y = neck_y + NECK_H * 0.45 + HEAD_R * 0.80
    shoulder_y = torso_y + TORSO_H * 0.18
    upper_len = TORSO_H * 0.26
    lower_len = TORSO_H * 0.22
    hand_y = shoulder_y - upper_len - lower_len - 0.02
    return {
        "shoulder_w": shoulder_w,
        "leg_cy": leg_cy,
        "hip_y": hip_y,
        "torso_y": torso_y,
        "neck_y": neck_y,
        "head_y": head_y,
        "shoulder_y": shoulder_y,
        "upper_len": upper_len,
        "lower_len": lower_len,
        "hand_y": hand_y,
        "foot_y": FOOT_Y,
    }


def _limbs_and_torso(r: dict[str, float], sw: float) -> None:
    hy, ty, ny, head_y = r["hip_y"], r["torso_y"], r["neck_y"], r["head_y"]
    for sx in (-1, 1):
        bc.add_cylinder(
            f"Leg_{sx}",
            LEG_R,
            LEG_H,
            (0.105 * sx, r["leg_cy"], 0.0),
            vertices=14,
            axis="Y",
        )
        bc.add_cube(f"Foot_{sx}", (0.13, 0.05, 0.21), (0.105 * sx, r["foot_y"] + 0.025, 0.0))
    bc.add_cube("Hips", (0.34, 0.13, 0.22), (0, hy, 0))
    bc.add_cube("Torso", (sw, TORSO_H, 0.24), (0, ty, 0))
    bc.add_cube("Neck", (0.12, NECK_H * 1.05, 0.12), (0, ny, 0))
    _add_head("Head", head_y)
    face_z = 0.075
    for sx in (-1, 1):
        eye = bc.add_cube(
            f"Eye_{sx}", (0.052, 0.042, 0.035), (0.068 * sx, head_y + 0.02, face_z)
        )
        bc.assign_solid(eye, (0.12, 0.14, 0.22, 1.0), 0.22)
        pupil = bc.add_cube(
            f"Pupil_{sx}", (0.036, 0.036, 0.02), (0.068 * sx, head_y + 0.02, face_z + 0.012)
        )
        bc.assign_solid(pupil, (0.03, 0.03, 0.05, 1.0), 0.12)
    mouth = bc.add_cube("Mouth", (0.08, 0.035, 0.022), (0, head_y - 0.055, face_z))
    bc.assign_solid(mouth, (0.78, 0.38, 0.42, 1.0), 0.45)
    for sx in (-1, 1):
        sh_x = sw * 0.36 * sx
        bc.add_cube(
            f"Shoulder_{sx}",
            (0.12, 0.12, 0.14),
            (sh_x * 0.92, r["shoulder_y"] + 0.02, 0),
        )
        upper_cy = r["shoulder_y"] - r["upper_len"] * 0.5 + 0.02
        bc.add_cylinder(
            f"UpperArm_{sx}",
            0.048,
            r["upper_len"] * 1.04,
            (sh_x * 0.98, upper_cy, 0.02),
            vertices=10,
            axis="Y",
        )
        elbow_y = r["shoulder_y"] - r["upper_len"] + 0.01
        bc.add_cube(f"Elbow_{sx}", (0.08, 0.08, 0.08), (sh_x * 0.98, elbow_y, 0.02))
        lower_cy = elbow_y - r["lower_len"] * 0.5 + 0.01
        bc.add_cylinder(
            f"LowerArm_{sx}",
            0.044,
            r["lower_len"] * 1.03,
            (sh_x * 1.02, lower_cy, 0.03),
            vertices=10,
            axis="Y",
        )
        hand = bc.add_cube(
            f"Hand_{sx}",
            (0.085, 0.075, 0.075),
            (sh_x * 1.04, r["hand_y"] + 0.03, 0.045),
        )
        bc.assign_solid(hand, (0.88, 0.72, 0.58, 1.0), 0.55)


def build_human_base(shoulder_w: float = 0.38) -> dict[str, float]:
    r = _rig(shoulder_w)
    _limbs_and_torso(r, r["shoulder_w"])
    return r


def _headwear(name: str, size: tuple[float, float, float], head_y: float, y_embed: float = 0.04) -> None:
    crown = head_y + HEAD_R - size[1] * 0.55 - y_embed
    bc.add_cube(name, size, (0, crown, 0))


def _prop_in_hand(
    name: str,
    size: tuple[float, float, float],
    hand_pos: tuple[float, float, float],
    offset: tuple[float, float, float] = (0.04, 0.0, 0.06),
) -> bpy.types.Object:
    loc = (hand_pos[0] + offset[0], hand_pos[1] + offset[1], hand_pos[2] + offset[2])
    return bc.add_cube(name, size, loc)


def build_founder_idle() -> None:
    r = build_human_base(0.40)
    _headwear("Hood", (0.42, 0.22, 0.30), r["head_y"], 0.06)
    hair_y = r["head_y"] + HEAD_R * 0.55 - 0.04
    bc.add_cube("Hair", (0.28, 0.07, 0.20), (0, hair_y, 0.02))
    hx = 0.40 * 0.52
    mug = bc.add_cylinder(
        "FounderMug", 0.055, 0.11, (hx, r["hand_y"] + 0.02, 0.10), vertices=12, axis="Y"
    )
    bc.assign_solid(mug, (0.92, 0.48, 0.12, 1.0), 0.35)
    bc.add_cube("MugHandle", (0.035, 0.07, 0.045), (hx + 0.055, r["hand_y"] + 0.02, 0.105))


def build_gpu_tech_idle() -> None:
    r = build_human_base(0.39)
    head_y = r["head_y"]
    bc.add_cube("HeadsetBand", (0.26, 0.04, 0.04), (0, head_y + HEAD_R * 0.55, -0.01))
    for sx, tag in ((-1, "L"), (1, "R")):
        bc.add_cube(
            f"HeadsetCup_{tag}",
            (0.10, 0.08, 0.06),
            (0.11 * sx, head_y + HEAD_R * 0.2, 0.01),
        )
    hy = r["hand_y"]
    card = bc.add_cube("GpuCard", (0.24, 0.14, 0.035), (0.39 * 0.52, hy + 0.01, 0.05))
    bc.assign_solid(card, (0.12, 0.48, 0.82, 1.0), 0.28)
    bc.add_cube("GpuLed", (0.04, 0.025, 0.018), (0.22, hy + 0.03, 0.06))
    hand_x = -0.39 * 0.36 * 1.04
    bc.add_cylinder(
        "Wrench",
        0.028,
        0.10,
        (hand_x, r["hand_y"] + 0.03, 0.045),
        vertices=6,
        axis="Y",
    )


def build_safety_idle() -> None:
    r = build_human_base(0.39)
    ty, head_y = r["torso_y"], r["head_y"]
    bc.add_cube("HiVisVest", (0.44, TORSO_H * 1.02, 0.28), (0, ty, 0.01))
    bc.add_cube("HiVisStripe", (0.40, 0.06, 0.29), (0, ty - TORSO_H * 0.2, 0.02))
    hat_y = head_y + HEAD_R - 0.06
    hat = bc.add_cube("HardHat", (0.36, 0.10, 0.34), (0, hat_y, 0))
    bc.assign_solid(hat, (0.92, 0.78, 0.12, 1.0), 0.42)
    shield = bc.add_cube("BadgeShield", (0.06, 0.07, 0.016), (0, ty, 0.10))
    bc.assign_solid(shield, (0.92, 0.82, 0.18, 1.0), 0.35)


def build_sales_idle() -> None:
    r = build_human_base(0.41)
    ty, head_y = r["torso_y"], r["head_y"]
    blazer = bc.add_cube("Blazer", (0.44, TORSO_H * 1.05, 0.28), (0, ty, 0.01))
    bc.assign_solid(blazer, (0.12, 0.22, 0.55, 1.0), 0.38)
    bc.add_cube("HeadsetBandSales", (0.26, 0.04, 0.04), (0, head_y + HEAD_R * 0.55, -0.02))
    bc.add_cube("HeadsetMic", (0.04, 0.14, 0.035), (0.08, head_y + 0.01, 0.10))
    for sx, tag in ((-1, "L"), (1, "R")):
        bc.add_cube(
            f"HeadsetCup{tag}", (0.10, 0.08, 0.06), (0.11 * sx, head_y + HEAD_R * 0.18, 0.01)
        )


def build_researcher_idle() -> None:
    r = build_human_base(0.37)
    ty, head_y = r["torso_y"], r["head_y"]
    bc.add_cube("LabCoat", (0.40, TORSO_H * 1.12, 0.26), (0, ty - 0.01, 0.02))
    bc.add_cube("Glasses", (0.24, 0.04, 0.08), (0, head_y + 0.01, 0.095))
    hand_x = 0.37 * 0.52
    tab = bc.add_cube("Tablet", (0.18, 0.24, 0.03), (hand_x, r["hand_y"] + 0.02, 0.05))
    bc.assign_emission(tab, (0.22, 0.55, 0.92, 1.0), strength=1.5)


def build_engineer_work() -> None:
    r = build_human_base(0.41)
    head_y = r["head_y"]
    hat_y = head_y + HEAD_R - 0.06
    hat = bc.add_cube("HardHatEng", (0.38, 0.10, 0.36), (0, hat_y, 0))
    bc.assign_solid(hat, (0.92, 0.78, 0.12, 1.0), 0.42)
    desk_y = r["leg_cy"]
    bc.add_cube("DeskTop", (0.36, 0.10, 0.32), (0.105, desk_y, 0.14))
    base_y = desk_y + 0.02
    bc.add_cube("LaptopBase", (0.32, 0.05, 0.24), (0.14, base_y, 0.20))
    bc.add_cube("LaptopScreen", (0.28, 0.18, 0.04), (0.14, base_y + 0.06, 0.22))
    screen = bc.add_cube("LaptopScreenEmit", (0.22, 0.14, 0.012), (0.14, base_y + 0.11, 0.235))
    bc.assign_emission(screen, (0.18, 0.72, 0.38, 1.0), strength=3.0)
    lap = bc.add_cube("Laptop", (0.30, 0.22, 0.05), (0.39 * 0.50, r["hand_y"] + 0.03, 0.08))
    bc.assign_solid(lap, (0.35, 0.38, 0.42, 1.0), 0.32)


def build_recruiter_idle() -> None:
    r = build_human_base(0.39)
    ty = r["torso_y"]
    blazer = bc.add_cube("BlazerRec", (0.42, TORSO_H * 1.02, 0.26), (0, ty, 0))
    bc.assign_solid(blazer, (0.42, 0.16, 0.22, 1.0), 0.48)
    bc.add_cube("Tie", (0.07, 0.14, 0.026), (0, ty, 0.10))
    hand_x = 0.39 * 0.36 * 1.04
    phone = bc.add_cube("Phone", (0.07, 0.12, 0.045), (hand_x, r["hand_y"] + 0.03, 0.045))
    bc.assign_solid(phone, (0.08, 0.08, 0.10, 1.0), 0.22)
    clip = bc.add_cube("Clipboard", (0.18, 0.26, 0.03), (hand_x - 0.02, r["hand_y"] + 0.03, 0.06))
    bc.add_cube("ClipMetal", (0.06, 0.10, 0.022), (hand_x - 0.02, r["hand_y"] + 0.14, 0.07))
    case = bc.add_cube("Briefcase", (0.26, 0.18, 0.10), (-0.22, FOOT_Y + 0.04, 0.12))
    bc.assign_solid(case, (0.55, 0.38, 0.18, 1.0), 0.42)


def build_pose_typing() -> None:
    sw = 0.38
    foot_y = 0.02
    leg_h = LEG_H * 0.64
    hip_y = foot_y + leg_h - 0.08
    torso_y = hip_y + TORSO_H * 0.22
    neck_y = torso_y + TORSO_H * 0.26
    head_y = neck_y + NECK_H * 0.5 + HEAD_R * 0.88
    shoulder_y = torso_y + TORSO_H * 0.22
    upper_len = TORSO_H * 0.24
    lower_len = TORSO_H * 0.20
    hand_y = shoulder_y - upper_len - lower_len - 0.01
    for sx in (-1, 1):
        bc.add_cylinder(
            f"Leg_{sx}", LEG_R * 0.95, leg_h, (0.10 * sx, foot_y + leg_h * 0.5, 0.0), vertices=12, axis="Y"
        )
        bc.add_cube(f"Foot_{sx}", (0.13, 0.05, 0.20), (0.10 * sx, foot_y + 0.02, 0.0))
    bc.add_cube("Hips", (0.36, 0.12, 0.22), (0, hip_y, 0))
    bc.add_cube("Torso", (sw, TORSO_H * 0.84, 0.22), (0, torso_y, 0))
    bc.add_cube("Neck", (0.11, NECK_H * 0.9, 0.11), (0, neck_y, 0))
    _add_head("Head", head_y)
    sh_w = sw * 0.36
    for sx in (-1, 1):
        bc.add_cube(f"Shoulder_{sx}", (0.11, 0.12, 0.13), (sh_w * sx * 0.92, shoulder_y + 0.02, 0))
        upper_cy = shoulder_y - upper_len * 0.5 + 0.02
        bc.add_cylinder(
            f"UpperArm_{sx}",
            0.046,
            upper_len * 1.04,
            (sh_w * sx * 0.98, upper_cy, 0.05),
            vertices=10,
            axis="Y",
        )
        elbow_y = shoulder_y - upper_len + 0.01
        bc.add_cube(f"Elbow_{sx}", (0.08, 0.08, 0.08), (sh_w * sx * 0.98, elbow_y, 0.06))
        lower_cy = elbow_y - lower_len * 0.5 + 0.01
        bc.add_cylinder(
            f"LowerArm_{sx}",
            0.042,
            lower_len * 1.03,
            (sh_w * sx * 1.02, lower_cy, 0.07),
            vertices=10,
            axis="Y",
        )
        hand = bc.add_cube(f"Hand_{sx}", (0.085, 0.075, 0.075), (sh_w * sx * 1.04, hand_y + 0.04, 0.08))
        bc.assign_solid(hand, (0.88, 0.72, 0.58, 1.0), 0.55)
    desk_y = foot_y + leg_h * 0.5
    bc.add_cube("DeskTop", (0.36, 0.12, 0.32), (0.11, desk_y, 0.0))
    bc.add_cube("ChairBack", (0.30, TORSO_H, 0.06), (0, torso_y, -0.06))
    bc.add_cube("Keyboard", (0.34, 0.02, 0.16), (0, desk_y + 0.04, 0.04))
    mouse = bc.add_cube("Mouse", (0.07, 0.03, 0.10), (0.18, desk_y + 0.04, 0.05))
    bc.assign_solid(mouse, (0.12, 0.12, 0.14, 1.0), 0.32)
    stand_y = desk_y + 0.04
    bc.add_cube("MonitorStand", (0.10, 0.04, 0.08), (0, stand_y, 0.10))
    bc.add_cube("Monitor", (0.36, 0.22, 0.04), (0, stand_y + 0.06, 0.10))
    screen = bc.add_cube("MonitorScreen", (0.30, 0.17, 0.015), (0, stand_y + 0.06, 0.11))
    bc.assign_emission(screen, (0.22, 0.62, 0.95, 1.0), strength=2.8)