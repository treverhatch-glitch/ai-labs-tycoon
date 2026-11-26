"""High-fidelity garage prop builders (CC0)."""
from __future__ import annotations

import math

from mesh_lib import RichMesh, bevel_box, cylinder, merge_rich, _add_quad  # noqa: F401

MATS = {
    "wood_oak": (0.55, 0.42, 0.30),
    "wood_dark": (0.38, 0.28, 0.20),
    "metal_frame": (0.32, 0.34, 0.36),
    "metal_scratched": (0.48, 0.50, 0.52),
    "crt_bezel": (0.62, 0.64, 0.66),
    "crt_screen": (0.22, 0.55, 0.28),
    "crt_glass": (0.12, 0.35, 0.20),
    "plastic_char": (0.18, 0.18, 0.20),
    "plastic_keys": (0.78, 0.76, 0.72),
    "rack_paint": (0.42, 0.44, 0.48),
    "gpu_green": (0.25, 0.45, 0.32),
    "led_red": (0.85, 0.15, 0.10),
    "fabric_seat": (0.35, 0.38, 0.55),
    "fabric_back": (0.32, 0.35, 0.50),
    "trim_orange": (0.66, 0.35, 0.16),
    "carpet_beige": (0.72, 0.66, 0.58),
    "concrete_warm": (0.75, 0.70, 0.64),
    "wall_beige": (0.82, 0.76, 0.66),
    "glass_pane": (0.70, 0.82, 0.90),
    "sign_board": (0.92, 0.88, 0.55),
    "copper_coil": (0.72, 0.48, 0.28),
}


def _mat(name: str) -> str:
    return name


def subdiv_top(w: float, d: float, y: float, ox: float, oz: float, subdiv: int, mat: str) -> RichMesh:
    m = RichMesh(material=_mat(mat))
    hw, hd = w * 0.5, d * 0.5
    for iz in range(subdiv):
        for ix in range(subdiv):
            x0 = ox - hw + w * ix / subdiv
            x1 = ox - hw + w * (ix + 1) / subdiv
            z0 = oz - hd + d * iz / subdiv
            z1 = oz - hd + d * (iz + 1) / subdiv
            _add_quad(m, (x0, y, z0), (x1, y, z0), (x1, y, z1), (x0, y, z1), (0, 1, 0))
    return m


def build_desk_crt() -> RichMesh:
    top = subdiv_top(1.15, 0.68, 0.74, 0.0, 0.0, 4, "wood_oak")
    drawers = merge_rich(
        bevel_box(0.32, 0.22, 0.48, -0.36, 0.0, 0.05, mat=_mat("wood_dark")),
        bevel_box(0.32, 0.22, 0.48, 0.0, 0.0, 0.05, mat=_mat("wood_dark")),
        bevel_box(0.32, 0.22, 0.48, 0.36, 0.0, 0.05, mat=_mat("wood_dark")),
    )
    for dx, dz in ((-0.46, -0.28), (0.46, -0.28), (-0.46, 0.28), (0.46, 0.28)):
        leg = cylinder(0.035, 0.72, segments=8, ox=dx, oy=0.0, oz=dz, mat=_mat("metal_frame"))
        drawers = merge_rich(drawers, leg)

    # CRT with curved screen segments
    screen = RichMesh(material=_mat("crt_glass"))
    segs = 14
    rx, ry, rz = 0.14, 0.88, -0.02
    rw, rh, rd = 0.46, 0.38, 0.42
    for i in range(segs):
        t0 = -0.35 + 0.7 * i / segs
        t1 = -0.35 + 0.7 * (i + 1) / segs
        x0, x1 = rx + rw * t0, rx + rw * t1
        z0, z1 = rz + rd * 0.15 * math.cos(t0 * 2), rz + rd * 0.15 * math.cos(t1 * 2)
        _add_quad(
            screen,
            (x0, ry - rh * 0.5, z0 - 0.18),
            (x1, ry - rh * 0.5, z1 - 0.18),
            (x1, ry + rh * 0.5, z1 + 0.05),
            (x0, ry + rh * 0.5, z0 + 0.05),
            (0, 0, 1),
        )
    bezel = bevel_box(0.5, 0.44, 0.46, rx, ry, rz - 0.08, bevel=0.03, mat=_mat("crt_bezel"))
    stand = bevel_box(0.12, 0.14, 0.1, rx, 0.62, rz - 0.05, mat=_mat("crt_bezel"))

    kb = bevel_box(0.44, 0.04, 0.2, 0.0, 0.76, 0.14, mat=_mat("plastic_char"))
    keys = RichMesh(material=_mat("plastic_keys"))
    for row in range(3):
        for col in range(10):
            kx = -0.17 + col * 0.038
            kz = 0.08 + row * 0.045
            keys = merge_rich(keys, bevel_box(0.032, 0.012, 0.032, kx, 0.79, kz, bevel=0.002))

    m = merge_rich(top, drawers, screen, bezel, stand, kb, keys)
    m.name = "garage-desk-crt"
    return m


def build_voodude_rack() -> RichMesh:
    frame = bevel_box(0.62, 1.85, 0.78, 0, 0.925, 0, bevel=0.015, mat=_mat("rack_paint"))
    parts: list[RichMesh] = [frame]
    for slot in range(6):
        sy = 0.22 + slot * 0.27
        u = bevel_box(0.54, 0.2, 0.58, 0, sy, 0.02, bevel=0.008, mat=_mat("metal_scratched"))
        card = bevel_box(0.42, 0.06, 0.48, 0, sy + 0.05, 0.18, mat=_mat("gpu_green"))
        fan = cylinder(0.08, 0.02, segments=12, ox=0.12, oy=sy + 0.08, oz=0.32, mat=_mat("metal_frame"))
        led = bevel_box(0.03, 0.03, 0.02, 0.24, sy + 0.12, 0.36, mat=_mat("led_red"))
        parts.extend([u, card, fan, led])
    badge = bevel_box(0.38, 0.1, 0.02, 0, 1.72, 0.36, mat=_mat("trim_orange"))
    parts.append(badge)
    m = merge_rich(*parts)
    m.name = "garage-voodude-rack"
    return m


def build_folding_chair() -> RichMesh:
    seat = subdiv_top(0.52, 0.5, 0.48, 0, 0, 3, "fabric_seat")
    back = bevel_box(0.5, 0.48, 0.05, 0, 0.72, -0.22, bevel=0.02, mat=_mat("fabric_back"))
    parts = [seat, back]
    for dx, dz, lean in ((-0.2, 0.18, -0.08), (0.2, 0.18, -0.08), (-0.2, -0.18, 0.08), (0.2, -0.18, 0.08)):
        leg = cylinder(0.018, 0.46, segments=8, ox=dx, oy=0.0, oz=dz, mat=_mat("metal_frame"))
        parts.append(leg)
    m = merge_rich(*parts)
    m.name = "garage-folding-chair"
    return m


def build_crt_tower() -> RichMesh:
    body = bevel_box(0.24, 0.58, 0.44, 0, 0.29, 0, bevel=0.015, mat=_mat("plastic_char"))
    drive = bevel_box(0.2, 0.08, 0.34, 0, 0.44, 0.2, mat=_mat("metal_scratched"))
    led = bevel_box(0.04, 0.04, 0.02, 0.06, 0.54, 0.22, mat=_mat("led_red"))
    slot = bevel_box(0.16, 0.03, 0.02, -0.05, 0.36, 0.21, mat=_mat("metal_frame"))
    m = merge_rich(body, drive, led, slot)
    m.name = "garage-crt-tower"
    return m


def build_simple(id_suffix: str, w: float, h: float, d: float, mat: str) -> RichMesh:
    m = bevel_box(w, h, d, 0, h * 0.5 if id_suffix.startswith("garage-wall") else 0, 0, mat=_mat(mat))
    m.name = id_suffix
    return m