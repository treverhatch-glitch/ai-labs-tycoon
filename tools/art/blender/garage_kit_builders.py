"""Procedural multi-part garage kit shapes (Blender Z-up build)."""
from __future__ import annotations

import blender_common as bc


def cb(name, w, hy, dz, x, loc_y, loc_z):
    """Y-up authoring → Z-up mesh: size (X, Y-depth, Z-height)."""
    return bc.add_cube(name, (w, dz, hy), (x, loc_z, loc_y))


def cyl_z(name, radius, height, x, loc_y, loc_z, **kw):
    return bc.add_cylinder(name, radius, height, (x, loc_z, loc_y), **kw)


def build_cable_spool() -> None:
    cyl_z("Hub", 0.11, 0.14, 0, 0, 0.07)
    cyl_z("FlangeL", 0.22, 0.04, 0, 0, 0.05)
    cyl_z("FlangeR", 0.22, 0.04, 0, 0, 0.17)
    cyl_z("Coil", 0.17, 0.22, 0, 0, 0.16)
    cb("CableStrand", 0.12, 0.08, 0.12, 0.14, 0.10, 0.20)


def build_coffee_drip() -> None:
    cb("Base", 0.34, 0.06, 0.28, 0, 0, 0.03)
    cb("Body", 0.30, 0.42, 0.26, 0, 0, 0.28)
    cb("Warmer", 0.22, 0.08, 0.20, 0, 0, 0.12)
    cb("Pot", 0.18, 0.16, 0.14, 0, 0.02, 0.20)
    cb("Carafe", 0.12, 0.26, 0.12, 0, 0.06, 0.48)
    cb("Spout", 0.10, 0.06, 0.08, 0.12, 0.10, 0.44)
    cb("Cord", 0.035, 0.04, 0.20, 0.14, 0.08, 0.12)


def build_crt_tower() -> None:
    cb("Chassis", 0.38, 0.72, 0.34, 0, 0, 0.36)
    cb("Bezel", 0.42, 0.08, 0.36, 0, 0, 0.78)
    screen = cb("Screen", 0.32, 0.28, 0.04, 0, 0.12, 0.62)
    bc.assign_emission(screen, (0.10, 0.48, 0.32, 1.0), 3.2)
    cb("FrontPanel", 0.36, 0.12, 0.06, 0, 0.16, 0.22)
    cb("SideLabel", 0.04, 0.40, 0.22, 0.17, 0.02, 0.42)
    cb("TopTrim", 0.40, 0.05, 0.36, 0, 0, 0.75)
    for i, x in enumerate([-0.12, 0.0, 0.12]):
        led = cb(f"Led{i}", 0.03, 0.03, 0.02, x, 0.15, 0.14)
        bc.assign_emission(led, (0.95, 0.25, 0.12, 1.0), 2.8)


def build_desk_crt() -> None:
    cb("Desktop", 1.22, 0.06, 0.62, 0, 0, 0.74)
    cb("Drawer", 0.42, 0.22, 0.48, -0.32, 0, 0.36)
    for x in (-0.48, 0.48):
        cb(f"Leg{x}", 0.06, 0.74, 0.06, x, 0.20, 0.37)
    cb("CRTBezel", 0.48, 0.46, 0.40, 0.18, 0.10, 0.92)
    screen = cb("Screen", 0.38, 0.30, 0.06, 0.18, 0.26, 0.88)
    bc.assign_emission(screen, (0.08, 0.42, 0.28, 1.0), 2.6)
    cb("Keyboard", 0.36, 0.04, 0.14, 0.02, 0.20, 0.755)


def build_exterior_wall() -> None:
    cb("Core", 0.52, 2.35, 0.12, 0, 0, 1.18)
    cb("Trim", 0.56, 0.14, 0.08, 0, 0.02, 2.28)
    cb("Baseboard", 0.54, 0.12, 0.10, 0, 0.02, 0.08)
    cb("Stripe", 0.10, 1.60, 0.14, 0.20, 0, 1.18)


def build_floor_tile() -> None:
    cb("Slab", 1.0, 0.08, 1.0, 0, 0, 0.04)
    for i, (x, z) in enumerate([(-0.25, -0.25), (0.25, 0.25), (-0.25, 0.25)]):
        cb(f"TileAccent{i}", 0.22, 0.02, 0.22, x, z, 0.05)


def build_folding_chair() -> None:
    cb("Seat", 0.44, 0.06, 0.40, 0, 0, 0.52)
    cb("Backrest", 0.44, 0.48, 0.05, 0, -0.14, 0.84)
    for i, (x, z) in enumerate([(0.17, 0.15), (-0.17, 0.15), (0.17, -0.15), (-0.17, -0.15)]):
        cb(f"Leg{i}", 0.045, 0.54, 0.045, x, z, 0.28)
    cb("BraceL", 0.03, 0.30, 0.26, 0.17, 0, 0.38)
    cb("BraceR", 0.03, 0.30, 0.26, -0.17, 0, 0.38)


def build_land_sign() -> None:
    cb("Post", 0.11, 1.65, 0.11, 0, 0, 0.825)
    cb("PostCollar", 0.18, 0.14, 0.18, 0, 0, 1.36)
    cb("SignBoard", 0.95, 0.62, 0.08, 0, 0.02, 1.46)
    cb("Cap", 0.12, 0.08, 0.12, 0, 0, 1.78)
    for i, (x, z, sx, hy) in enumerate(
        [(0, 1.48, 0.55, 0.06), (-0.22, 1.42, 0.12, 0.06), (0.22, 1.42, 0.12, 0.06), (0, 1.36, 0.40, 0.05)]
    ):
        cb(f"Letter{i}", sx, hy, 0.06, x, 0.02, z)


def build_lot_curb() -> None:
    cb("Curb", 0.90, 0.16, 0.22, 0, 0, 0.08)
    cb("Grass", 0.88, 0.10, 0.26, 0, 0.08, 0.14)
    cb("Gutter", 0.92, 0.04, 0.18, 0, -0.10, 0.04)


def build_roll_door() -> None:
    cb("FrameL", 0.10, 2.25, 0.12, -1.05, 0, 1.12)
    cb("FrameR", 0.10, 2.25, 0.12, 1.05, 0, 1.12)
    cb("Header", 2.10, 0.14, 0.12, 0, 0, 2.22)
    cb("Curtain", 1.85, 1.78, 0.06, 0, 0.02, 1.02)
    cb("Drum", 0.24, 0.16, 0.14, 0, 0.04, 2.06)
    for i in range(8):
        cb(f"slat{i}", 1.72, 0.04, 0.02, 0, 0.035, 0.30 + i * 0.20)


def build_server_closet() -> None:
    cb("Shell", 1.05, 2.05, 0.85, 0, 0, 1.02)
    cb("Door", 0.95, 1.95, 0.08, 0, 0.38, 1.02)
    cb("Handle", 0.08, 0.30, 0.08, 0.38, 0.42, 1.0)
    for i, z in enumerate((0.55, 1.05, 1.45)):
        cb(f"Vent{i}", 0.70, 0.10, 0.06, 0, 0.40, z)
    for i, (x, z) in enumerate([(-0.25, 0.7), (0.1, 1.2), (-0.15, 1.5)]):
        led = cb(f"Led{i}", 0.05, 0.05, 0.05, x, 0.40, z)
        bc.assign_emission(led, (0.15, 0.85, 0.35, 1.0), 2.5)


def build_voodude_rack() -> None:
    cb("RackShell", 0.68, 1.90, 0.62, 0, 0, 0.95)
    cb("FacePlate", 0.58, 1.45, 0.08, 0, 0.30, 1.02)
    for i in range(4):
        cb(f"Bay{i}", 0.50, 0.28, 0.12, 0, 0.18, 0.45 + i * 0.35)
    for i in range(6):
        led = cb(f"Led{i}", 0.03, 0.03, 0.02, -0.18 + (i % 3) * 0.18, 0.26, 0.5 + (i // 3) * 0.6)
        bc.assign_emission(led, (1.0, 0.3, 0.1, 1.0), 2.2)


def build_wall_panel() -> None:
    cb("Wall", 0.50, 2.25, 0.10, 0, 0, 1.12)
    cb("Stripe", 0.12, 1.75, 0.04, 0.15, 0.03, 1.15)
    cb("Trim", 0.52, 0.10, 0.06, 0, 0.02, 2.18)
    cb("Outlet", 0.10, 0.12, 0.04, -0.12, 0.04, 0.55)


def build_whiteboard() -> None:
    cb("Surface", 1.25, 0.95, 0.04, 0, 0, 1.22)
    cb("BackPlate", 1.20, 0.92, 0.03, 0, -0.02, 1.22)
    cb("FrameL", 0.05, 1.05, 0.06, -0.64, 0, 1.20)
    cb("FrameR", 0.05, 1.05, 0.06, 0.64, 0, 1.20)
    cb("Tray", 1.10, 0.06, 0.10, 0, 0.02, 0.72)
    cb("MountTabL", 0.06, 0.08, 0.10, -0.52, -0.02, 1.65)
    cb("MountTabR", 0.06, 0.08, 0.10, 0.52, -0.02, 1.65)
    for i, (x, z) in enumerate([(0.1, 1.35), (-0.25, 1.15), (0.3, 1.05), (-0.1, 0.95)]):
        cb(f"stroke{i}", 0.14, 0.05, 0.03, x, 0.01, z)


def build_window_frame() -> None:
    cb("Frame", 1.05, 1.25, 0.10, 0, 0, 1.30)
    cb("Header", 1.08, 0.10, 0.08, 0, 0.02, 1.92)
    cb("Sill", 1.10, 0.08, 0.14, 0, 0.04, 0.72)
    cb("MullionV", 0.05, 0.95, 0.04, 0, 0.02, 1.28)
    cb("MullionH", 0.95, 0.05, 0.04, 0, 0.02, 1.28)
    cb("PaneL", 0.42, 0.52, 0.02, -0.26, 0.04, 1.28)
    pane = cb("PaneR", 0.42, 0.52, 0.02, 0.26, 0.04, 1.28)
    bc.assign_solid(pane, (0.72, 0.82, 0.92, 0.35), 0.08)


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