"""Procedural multi-part garage kit shapes (Y-up)."""
from __future__ import annotations

import blender_common as bc


def build_cable_spool() -> None:
    bc.add_cylinder("Hub", 0.11, 0.14, (0, 0.07, 0), vertices=24)
    bc.add_cylinder("FlangeL", 0.22, 0.04, (0, 0.05, 0), vertices=24)
    bc.add_cylinder("FlangeR", 0.22, 0.04, (0, 0.17, 0), vertices=24)
    bc.add_cylinder("Coil", 0.17, 0.22, (0, 0.16, 0), vertices=28)
    bc.add_cube("CableStrand", (0.12, 0.08, 0.12), (0.14, 0.20, 0.10))


def build_coffee_drip() -> None:
    bc.add_cube("Base", (0.34, 0.06, 0.28), (0, 0.03, 0))
    bc.add_cube("Body", (0.30, 0.42, 0.26), (0, 0.28, 0))
    bc.add_cube("Warmer", (0.22, 0.08, 0.20), (0, 0.12, 0))
    bc.add_cube("Pot", (0.18, 0.16, 0.14), (0, 0.20, 0.02))
    bc.add_cube("Carafe", (0.12, 0.26, 0.12), (0, 0.48, 0.06))
    bc.add_cube("Spout", (0.10, 0.06, 0.08), (0.12, 0.44, 0.10))
    bc.add_cylinder("Cord", 0.018, 0.22, (0.05, 0.18, 0.08), vertices=8, rotation_euler=(0.8, 0, 0.2))


def build_crt_tower() -> None:
    bc.add_cube("Chassis", (0.38, 0.72, 0.34), (0, 0.36, 0))
    bc.add_cube("Bezel", (0.42, 0.08, 0.36), (0, 0.78, 0))
    screen = bc.add_cube("Screen", (0.32, 0.28, 0.04), (0, 0.62, 0.12))
    bc.assign_emission(screen, (0.10, 0.48, 0.32, 1.0), 3.2)
    bc.add_cube("FrontPanel", (0.36, 0.12, 0.06), (0, 0.22, 0.16))
    bc.add_cube("SideLabel", (0.04, 0.40, 0.22), (0.17, 0.42, 0.02))
    bc.add_cube("TopTrim", (0.40, 0.05, 0.36), (0, 0.75, 0))
    for i, x in enumerate([-0.12, 0.0, 0.12]):
        led = bc.add_cube(f"Led{i}", (0.03, 0.03, 0.02), (x, 0.14, 0.15))
        bc.assign_emission(led, (0.95, 0.25, 0.12, 1.0), 2.8)


def build_desk_crt() -> None:
    bc.add_cube("Desktop", (1.22, 0.06, 0.62), (0, 0.74, 0))
    bc.add_cube("Drawer", (0.42, 0.22, 0.48), (-0.32, 0.36, 0))
    for x in (-0.48, 0.48):
        bc.add_cube(f"Leg{x}", (0.06, 0.74, 0.06), (x, 0.37, 0.20))
    bc.add_cube("CRTBezel", (0.48, 0.46, 0.40), (0.18, 0.92, 0.10))
    screen = bc.add_cube("Screen", (0.38, 0.30, 0.06), (0.18, 0.88, 0.26))
    bc.assign_emission(screen, (0.08, 0.42, 0.28, 1.0), 2.6)
    bc.add_cube("Keyboard", (0.36, 0.04, 0.14), (0.02, 0.755, 0.20))


def build_exterior_wall() -> None:
    bc.add_cube("Core", (0.52, 2.35, 0.12), (0, 1.18, 0))
    bc.add_cube("Trim", (0.56, 0.14, 0.08), (0, 2.28, 0.04))
    bc.add_cube("Baseboard", (0.54, 0.12, 0.10), (0, 0.08, 0.04))
    bc.add_cube("Stripe", (0.10, 1.60, 0.04), (0.20, 1.20, 0.04))


def build_floor_tile() -> None:
    bc.add_cube("Slab", (1.0, 0.08, 1.0), (0, 0.04, 0))
    for i, (x, z) in enumerate([(-0.25, -0.25), (0.25, 0.25), (-0.25, 0.25)]):
        bc.add_cube(f"TileAccent{i}", (0.22, 0.02, 0.22), (x, 0.05, z))


def build_folding_chair() -> None:
    bc.add_cube("Seat", (0.44, 0.06, 0.40), (0, 0.52, 0))
    bc.add_cube("Backrest", (0.44, 0.48, 0.05), (0, 0.84, -0.14))
    for i, (x, z) in enumerate([(0.17, 0.15), (-0.17, 0.15), (0.17, -0.15), (-0.17, -0.15)]):
        bc.add_cube(f"Leg{i}", (0.035, 0.50, 0.035), (x, 0.25, z))
    bc.add_cube("BraceL", (0.03, 0.30, 0.26), (0.17, 0.38, 0))
    bc.add_cube("BraceR", (0.03, 0.30, 0.26), (-0.17, 0.38, 0))


def build_land_sign() -> None:
    bc.add_cylinder(
        "Post", 0.055, 1.55, (0, 0.78, 0), vertices=12, rotation_euler=(1.5708, 0, 0)
    )
    bc.add_cube("SignBoard", (0.95, 0.62, 0.05), (0, 1.52, 0.02))
    bc.add_cube("Cap", (0.12, 0.08, 0.12), (0, 1.76, 0))
    for i, (x, y, sx, sy) in enumerate(
        [(0, 1.58, 0.55, 0.06), (-0.22, 1.48, 0.12, 0.06), (0.22, 1.48, 0.12, 0.06), (0, 1.38, 0.40, 0.05)]
    ):
        bc.add_cube(f"Letter{i}", (sx, sy, 0.02), (x, y, 0.035))


def build_lot_curb() -> None:
    bc.add_cube("Curb", (0.90, 0.16, 0.22), (0, 0.08, 0))
    bc.add_cube("Grass", (0.88, 0.10, 0.26), (0, 0.14, 0.08))
    bc.add_cube("Gutter", (0.92, 0.04, 0.18), (0, 0.04, -0.10))


def build_roll_door() -> None:
    bc.add_cube("FrameL", (0.10, 2.25, 0.12), (-1.05, 1.12, 0))
    bc.add_cube("FrameR", (0.10, 2.25, 0.12), (1.05, 1.12, 0))
    bc.add_cube("Header", (2.10, 0.14, 0.12), (0, 2.22, 0))
    bc.add_cube("Curtain", (1.85, 1.78, 0.06), (0, 1.02, 0.02))
    bc.add_cube("Drum", (0.24, 0.16, 0.14), (0, 2.06, 0.04))
    for i in range(8):
        bc.add_cube(f"slat{i}", (1.72, 0.04, 0.02), (0, 0.30 + i * 0.20, 0.035))


def build_server_closet() -> None:
    bc.add_cube("Shell", (1.05, 2.05, 0.85), (0, 1.02, 0))
    bc.add_cube("Door", (0.95, 1.95, 0.06), (0, 1.02, 0.42))
    bc.add_cube("Handle", (0.08, 0.30, 0.06), (0.38, 1.0, 0.46))
    for i, y in enumerate((0.55, 1.05, 1.45)):
        bc.add_cube(f"Vent{i}", (0.70, 0.08, 0.04), (0, y, 0.43))
    for i, (x, y) in enumerate([(-0.25, 0.7), (0.1, 1.2), (-0.15, 1.5)]):
        led = bc.add_cube(f"Led{i}", (0.04, 0.04, 0.02), (x, y, 0.44))
        bc.assign_emission(led, (0.15, 0.85, 0.35, 1.0), 2.5)


def build_voodude_rack() -> None:
    bc.add_cube("RackShell", (0.68, 1.90, 0.62), (0, 0.95, 0))
    bc.add_cube("FacePlate", (0.58, 1.45, 0.08), (0, 1.02, 0.30))
    for i in range(4):
        bc.add_cube(f"Bay{i}", (0.50, 0.28, 0.12), (0, 0.45 + i * 0.35, 0.18))
    for i in range(6):
        led = bc.add_cube(f"Led{i}", (0.03, 0.03, 0.02), (-0.18 + (i % 3) * 0.18, 0.5 + (i // 3) * 0.6, 0.26))
        bc.assign_emission(led, (1.0, 0.3, 0.1, 1.0), 2.2)


def build_wall_panel() -> None:
    bc.add_cube("Wall", (0.50, 2.25, 0.10), (0, 1.12, 0))
    bc.add_cube("Stripe", (0.12, 1.75, 0.04), (0.15, 1.15, 0.03))
    bc.add_cube("Trim", (0.52, 0.10, 0.06), (0, 2.18, 0.02))
    bc.add_cube("Outlet", (0.10, 0.12, 0.04), (-0.12, 0.55, 0.04))


def build_whiteboard() -> None:
    bc.add_cube("Surface", (1.25, 0.95, 0.04), (0, 1.22, 0))
    bc.add_cube("FrameL", (0.05, 1.05, 0.06), (-0.64, 1.20, 0))
    bc.add_cube("FrameR", (0.05, 1.05, 0.06), (0.64, 1.20, 0))
    bc.add_cube("Tray", (1.10, 0.06, 0.10), (0, 0.72, 0.04))
    bc.add_cube("EaselLegL", (0.04, 0.55, 0.04), (-0.35, 0.28, -0.12))
    bc.add_cube("EaselLegR", (0.04, 0.55, 0.04), (0.35, 0.28, -0.12))
    for i, (x, y) in enumerate([(0.1, 1.35), (-0.25, 1.15), (0.3, 1.05), (-0.1, 0.95)]):
        bc.add_cube(f"stroke{i}", (0.14, 0.04, 0.02), (x, y, 0.03))


def build_window_frame() -> None:
    bc.add_cube("Frame", (1.05, 1.25, 0.10), (0, 1.30, 0))
    bc.add_cube("Header", (1.08, 0.10, 0.08), (0, 1.92, 0.02))
    bc.add_cube("Sill", (1.10, 0.08, 0.14), (0, 0.72, 0.04))
    bc.add_cube("MullionV", (0.05, 0.95, 0.04), (0, 1.28, 0.02))
    bc.add_cube("MullionH", (0.95, 0.05, 0.04), (0, 1.28, 0.02))
    bc.add_cube("PaneL", (0.42, 0.52, 0.02), (-0.26, 1.28, 0.04))
    pane = bc.add_cube("PaneR", (0.42, 0.52, 0.02), (0.26, 1.28, 0.04))


BUILDERS = {
    "garage-cable-spool": build_cable_spool,
    "garage-coffee-drip": build_coffee_drip,
    "garage-crt-tower": build_crt_tower,
    "garage-desk-crt": build_desk_crt,
    "garage-exterior-wall": build_exterior_wall,
    "garage-floor-tile": build_floor_tile,
    "garage-folding-chair": build_folding_chair,
    "garage-land-for-sale-sign": build_land_sign,
    "garage-lot-curb": build_lot_curb,
    "garage-roll-door": build_roll_door,
    "garage-server-closet": build_server_closet,
    "garage-voodude-rack": build_voodude_rack,
    "garage-wall-panel": build_wall_panel,
    "garage-whiteboard-1995": build_whiteboard,
    "garage-window-frame": build_window_frame,
}