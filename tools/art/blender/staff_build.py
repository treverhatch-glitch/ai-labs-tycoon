"""Distinct low-poly staff humanoids for packet 2 (Y-up, grounded at y=0)."""
from __future__ import annotations

import blender_common as bc


def _feet_and_hands(shoulder_w: float) -> None:
    for sx in (-1, 1):
        bc.add_cube(f"Foot_{sx}", (0.14, 0.06, 0.24), (0.14 * sx, 0.03, 0.02 * sx))
        bc.add_cube(f"Hand_{sx}", (0.09, 0.07, 0.08), (shoulder_w * 0.52 * sx, 0.84, 0.05 * sx))


def build_human_base(shoulder_w: float = 0.38, pants_tone: bool = True) -> None:
    for sx in (-1, 1):
        bc.add_cylinder(f"Leg_{sx}", 0.095, 0.82, (0.14 * sx, 0.41, 0.0), vertices=10)
    bc.add_cube("Hips", (0.36, 0.17, 0.23), (0, 0.80, 0))
    bc.add_cube("Torso", (shoulder_w, 0.48, 0.25), (0, 1.18, 0))
    bc.add_cube("Neck", (0.13, 0.10, 0.13), (0, 1.43, 0))
    bc.add_cube("Head", (0.25, 0.26, 0.25), (0, 1.57, 0))
    for sx in (-1, 1):
        bc.add_cylinder(f"Arm_{sx}", 0.072, 0.38, (shoulder_w * 0.35 * sx, 1.05, 0.0), vertices=8)
        bc.add_cube(f"Shoulder_{sx}", (0.11, 0.09, 0.12), (shoulder_w * 0.40 * sx, 1.22, 0))
    _feet_and_hands(shoulder_w)


def build_founder_idle() -> None:
    build_human_base(0.40)
    bc.add_cube("Hood", (0.44, 0.32, 0.28), (0, 1.24, -0.02))
    bc.add_cube("HoodPocket", (0.18, 0.14, 0.04), (0.12, 1.02, 0.13))
    bc.add_cube("Hair", (0.28, 0.06, 0.22), (0, 1.69, 0.02))
    bc.add_cube("LaptopStrut", (0.08, 0.12, 0.08), (0.16, 1.0, 0.08))
    bc.add_cube("LaptopBase", (0.36, 0.05, 0.26), (0.14, 0.98, 0.14))
    bc.add_cube("LaptopScreen", (0.30, 0.20, 0.025), (0.14, 1.06, 0.24))
    screen = bc.add_cube("LaptopScreenEmit", (0.24, 0.16, 0.012), (0.14, 1.06, 0.26))
    bc.assign_emission(screen, (0.25, 0.85, 0.55, 1.0), strength=2.5)


def build_gpu_tech_idle() -> None:
    build_human_base(0.39)
    bc.add_cylinder("Cap", 0.14, 0.07, (0, 1.69, 0), vertices=12)
    for sx in (-1, 1):
        bc.add_cube(f"Glove_{sx}", (0.10, 0.08, 0.10), (0.22 * sx, 0.84, 0.08))
    bc.add_cube("Toolbelt", (0.42, 0.10, 0.18), (0, 0.88, 0.12))
    bc.add_cube("Pouch", (0.10, 0.12, 0.08), (0.20, 0.90, 0.16))
    bc.add_cylinder("Screwdriver", 0.025, 0.18, (0.24, 0.92, 0.20), vertices=6)


def build_safety_idle() -> None:
    build_human_base(0.39)
    bc.add_cube("HiVisVest", (0.44, 0.46, 0.27), (0, 1.15, 0.01))
    bc.add_cube("HiVisStripe", (0.40, 0.06, 0.28), (0, 1.05, 0.02))
    bc.add_cube("Helmet", (0.30, 0.10, 0.30), (0, 1.69, 0))
    bc.add_cube("BadgeClip", (0.12, 0.15, 0.04), (0.18, 1.08, 0.14))


def build_sales_idle() -> None:
    build_human_base(0.42)
    bc.add_cube("Blazer", (0.44, 0.50, 0.26), (0, 1.17, 0.01))
    bc.add_cube("BlazerLapel", (0.12, 0.22, 0.03), (0.08, 1.18, 0.14))
    bc.add_cube("PhoneStrut", (0.06, 0.10, 0.06), (0.20, 1.00, 0.06))
    bc.add_cube("Phone", (0.06, 0.12, 0.04), (0.28, 1.02, 0.10))
    bc.add_cube("BriefStrut", (0.07, 0.14, 0.07), (-0.14, 0.98, 0.04))
    bc.add_cube("BriefEdge", (0.18, 0.24, 0.05), (-0.22, 0.96, 0.06))


def build_researcher_idle() -> None:
    build_human_base(0.36)
    bc.add_cube("LabCoat", (0.40, 0.62, 0.24), (0, 1.02, 0.02))
    bc.add_cube("Glasses", (0.22, 0.04, 0.08), (0, 1.58, 0.14))
    bc.add_cube("GlassesLensL", (0.08, 0.06, 0.02), (-0.07, 1.58, 0.15))
    bc.add_cube("GlassesLensR", (0.08, 0.06, 0.02), (0.07, 1.58, 0.15))
    bc.add_cube("Ponytail", (0.08, 0.18, 0.10), (-0.16, 1.62, -0.05))
    bc.add_cube("Clipboard", (0.16, 0.22, 0.03), (0.20, 0.98, 0.08))


def build_engineer_work() -> None:
    build_human_base(0.42)
    bc.add_cube("HardHat", (0.36, 0.10, 0.36), (0, 1.69, 0))
    bc.add_cube("LaptopStrut", (0.08, 0.14, 0.08), (0.10, 1.02, 0.08))
    bc.add_cube("LaptopBase", (0.38, 0.06, 0.28), (0.08, 1.00, 0.14))
    bc.add_cube("LaptopScreen", (0.32, 0.22, 0.04), (0.08, 1.08, 0.26))
    screen = bc.add_cube("LaptopScreenEmit", (0.26, 0.18, 0.012), (0.08, 1.08, 0.28))
    bc.assign_emission(screen, (0.18, 0.72, 0.38, 1.0), strength=3.0)


def build_recruiter_idle() -> None:
    build_human_base(0.38)
    bc.add_cube("BlazerRec", (0.40, 0.48, 0.26), (0, 1.16, 0))
    bc.add_cube("Tie", (0.06, 0.24, 0.025), (0, 1.28, 0.14))
    bc.add_cube("FolderStrut", (0.07, 0.14, 0.07), (-0.12, 1.00, 0.04))
    bc.add_cube("Folder", (0.20, 0.26, 0.05), (-0.22, 0.98, 0.06))


def build_pose_typing() -> None:
    build_human_base(0.40)
    bc.add_cube("DeskStrut", (0.10, 0.24, 0.10), (0.14, 0.92, 0.10))
    bc.add_cube("DeskEdge", (0.52, 0.06, 0.34), (0, 0.72, 0.18))
    bc.add_cube("Keyboard", (0.38, 0.03, 0.18), (0, 0.80, 0.22))
    bc.add_cube("KeyFace", (0.34, 0.01, 0.14), (0, 0.825, 0.28))
    bc.add_cube("Monitor", (0.32, 0.24, 0.03), (0, 1.06, 0.24))
    screen = bc.add_cube("MonitorScreen", (0.26, 0.18, 0.02), (0, 1.06, 0.25))
    bc.assign_emission(screen, (0.22, 0.62, 0.95, 1.0), strength=2.8)