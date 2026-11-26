"""Shared low-poly staff silhouettes for packet 2 (Y-up, grounded at y=0)."""
from __future__ import annotations

import blender_common as bc


def build_human_base(shoulder_w: float = 0.38) -> None:
    """Legs, hips, torso, arms, head — parts overlap at joints."""
    for sx in (-1, 1):
        bc.add_cylinder(f"Leg_{sx}", 0.10, 0.86, (0.13 * sx, 0.43, 0.01 * sx), vertices=10)
    bc.add_cube("Hips", (0.34, 0.16, 0.22), (0, 0.80, 0))
    bc.add_cube("Torso", (shoulder_w, 0.46, 0.24), (0, 1.17, 0))
    bc.add_cube("Neck", (0.14, 0.10, 0.14), (0, 1.44, 0))
    bc.add_cube("Head", (0.26, 0.26, 0.26), (0, 1.56, 0))
    for sx in (-1, 1):
        bc.add_cylinder(
            f"Arm_{sx}",
            0.075,
            0.40,
            (shoulder_w * 0.36 * sx, 1.06, 0.0),
            vertices=8,
        )
        bc.add_cube(f"Shoulder_{sx}", (0.10, 0.10, 0.12), (shoulder_w * 0.38 * sx, 1.22, 0))


def build_researcher_idle() -> None:
    build_human_base(0.36)
    bc.add_cube("Ponytail", (0.08, 0.20, 0.10), (-0.17, 1.62, -0.04))
    bc.add_cube("ClipStrut", (0.08, 0.14, 0.08), (0.14, 1.02, 0.06))
    bc.add_cube("Clipboard", (0.16, 0.22, 0.03), (0.20, 0.98, 0.08))


def build_engineer_work() -> None:
    build_human_base(0.42)
    bc.add_cube("HardHat", (0.36, 0.10, 0.36), (0, 1.68, 0))
    bc.add_cube("LaptopStrut", (0.10, 0.22, 0.10), (0.08, 1.06, 0.06))
    bc.add_cube("LaptopBase", (0.40, 0.06, 0.28), (0, 1.00, 0.10))
    bc.add_cube("LaptopScreen", (0.34, 0.22, 0.04), (0, 1.10, 0.26))
    screen = bc.add_cube("LaptopScreenEmit", (0.28, 0.18, 0.01), (0, 1.10, 0.27))
    bc.assign_emission(screen, (0.18, 0.72, 0.38, 1.0), strength=3.2)


def build_safety_idle() -> None:
    build_human_base(0.38)
    bc.add_cube("Vest", (0.40, 0.42, 0.26), (0, 1.14, 0.01))
    bc.add_cube("BadgeClip", (0.12, 0.16, 0.04), (0.18, 1.06, 0.10))


def build_sales_idle() -> None:
    build_human_base(0.40)
    bc.add_cube("PhoneStrut", (0.06, 0.12, 0.06), (0.20, 1.02, 0.06))
    bc.add_cube("Phone", (0.06, 0.12, 0.04), (0.26, 1.02, 0.08))
    bc.add_cube("BriefStrut", (0.08, 0.16, 0.08), (-0.14, 0.98, 0.04))
    bc.add_cube("BriefEdge", (0.18, 0.26, 0.05), (-0.22, 0.96, 0.06))


def build_recruiter_idle() -> None:
    build_human_base(0.37)
    bc.add_cube("Tie", (0.06, 0.22, 0.025), (0, 1.28, 0.13))
    bc.add_cube("FolderStrut", (0.08, 0.14, 0.08), (-0.12, 1.00, 0.04))
    bc.add_cube("Folder", (0.20, 0.26, 0.05), (-0.22, 0.98, 0.06))


def build_gpu_tech_idle() -> None:
    build_human_base(0.38)
    bc.add_cylinder("Cap", 0.14, 0.06, (0, 1.68, 0), vertices=12)
    bc.add_cube("ToolStrut", (0.08, 0.14, 0.08), (0.18, 1.02, 0.04))
    bc.add_cylinder("Screwdriver", 0.02, 0.22, (0.28, 1.00, 0.06), vertices=6)


def build_founder_idle() -> None:
    build_human_base(0.38)
    bc.add_cube("Hair", (0.30, 0.06, 0.24), (0, 1.70, 0))
    bc.add_cube("MugStrut", (0.08, 0.10, 0.08), (0.16, 1.02, 0.04))
    mug = bc.add_cylinder("Mug", 0.05, 0.12, (0.24, 1.00, 0.06), vertices=10)
    bc.assign_solid(mug, (0.88, 0.48, 0.12, 1.0), 0.35)


def build_pose_typing() -> None:
    build_human_base(0.40)
    bc.add_cube("DeskStrut", (0.12, 0.28, 0.12), (0.14, 0.94, 0.10))
    bc.add_cube("DeskEdge", (0.50, 0.06, 0.32), (0, 0.72, 0.18))
    bc.add_cube("Keyboard", (0.38, 0.03, 0.18), (0, 0.80, 0.22))
    bc.add_cube("KeyFace", (0.34, 0.01, 0.14), (0, 0.825, 0.28))
    bc.add_cube("Monitor", (0.32, 0.24, 0.03), (0, 1.06, 0.24))
    screen = bc.add_cube("MonitorScreen", (0.26, 0.18, 0.02), (0, 1.06, 0.25))
    bc.assign_emission(screen, (0.22, 0.62, 0.95, 1.0), strength=2.8)