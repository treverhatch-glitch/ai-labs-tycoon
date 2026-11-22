#!/usr/bin/env python3
"""
Generate in-repo low-poly Wavefront OBJ props for AI Labs Tycoon.

License: CC0 / public-domain dedication — original geometry, no third-party meshes.
Run from repo root: python3 tools/art/generate_campus_kit.py
"""
from __future__ import annotations

import json
import math
from dataclasses import dataclass
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[2]
OUT_DIR = REPO_ROOT / "unity" / "Assets" / "Art" / "Generated" / "campus-kit"


@dataclass
class Mesh:
    name: str
    vertices: list[tuple[float, float, float]]
    faces: list[tuple[int, ...]]

    def write_obj(self, path: Path) -> None:
        path.parent.mkdir(parents=True, exist_ok=True)
        lines = [f"# {self.name} — procedural, repo-owned", f"o {self.name}"]
        for x, y, z in self.vertices:
            lines.append(f"v {x:.6f} {y:.6f} {z:.6f}")
        for face in self.faces:
            idx = " ".join(str(i + 1) for i in face)
            lines.append(f"f {idx}")
        path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def box(w: float, h: float, d: float, ox: float = 0, oy: float = 0, oz: float = 0) -> Mesh:
    hw, hh, hd = w * 0.5, h * 0.5, d * 0.5
    v = [
        (ox - hw, oy, oz - hd),
        (ox + hw, oy, oz - hd),
        (ox + hw, oy, oz + hd),
        (ox - hw, oy, oz + hd),
        (ox - hw, oy + h, oz - hd),
        (ox + hw, oy + h, oz - hd),
        (ox + hw, oy + h, oz + hd),
        (ox - hw, oy + h, oz + hd),
    ]
    faces = [
        (0, 1, 5, 4),
        (1, 2, 6, 5),
        (2, 3, 7, 6),
        (3, 0, 4, 7),
        (4, 5, 6, 7),
        (0, 3, 2, 1),
    ]
    return Mesh("box", v, faces)


def merge(*parts: Mesh) -> Mesh:
    verts: list[tuple[float, float, float]] = []
    faces: list[tuple[int, ...]] = []
    for p in parts:
        base = len(verts)
        verts.extend(p.vertices)
        for f in p.faces:
            faces.append(tuple(i + base for i in f))
    return Mesh("merged", verts, faces)


def desk() -> Mesh:
    top = box(1.2, 0.06, 0.7, oy=0.74)
    leg_fl = box(0.08, 0.74, 0.08, -0.5, 0.37, -0.28)
    leg_fr = box(0.08, 0.74, 0.08, 0.5, 0.37, -0.28)
    leg_bl = box(0.08, 0.74, 0.08, -0.5, 0.37, 0.28)
    leg_br = box(0.08, 0.74, 0.08, 0.5, 0.37, 0.28)
    monitor = box(0.42, 0.28, 0.04, 0.15, 0.95, -0.1)
    stand = box(0.08, 0.12, 0.08, 0.15, 0.82, -0.1)
    return merge(top, leg_fl, leg_fr, leg_bl, leg_br, monitor, stand)


def chair() -> Mesh:
    seat = box(0.55, 0.08, 0.55, oy=0.45)
    back = box(0.55, 0.55, 0.06, oy=0.72, oz=-0.24)
    for dx, dz in ((-0.2, -0.2), (0.2, -0.2), (-0.2, 0.2), (0.2, 0.2)):
        leg = box(0.07, 0.45, 0.07, dx, 0.225, dz)
        seat = merge(seat, leg)
    return merge(seat, back)


def server_rack() -> Mesh:
    body = box(0.6, 1.8, 0.75)
    slots = merge()
    for i in range(6):
        y = 0.2 + i * 0.28
        slots = merge(slots, box(0.52, 0.18, 0.02, oy=y, oz=0.38))
    return merge(body, slots)


def potted_plant() -> Mesh:
    pot = box(0.35, 0.32, 0.35, oy=0.16)
    soil = box(0.3, 0.04, 0.3, oy=0.34)
    trunk = box(0.08, 0.35, 0.08, oy=0.53)
    crown = box(0.45, 0.35, 0.45, oy=0.88)
    return merge(pot, soil, trunk, crown)


def whiteboard() -> Mesh:
    board = box(1.4, 0.9, 0.05, oy=1.1)
    frame = merge(
        box(1.48, 0.06, 0.08, oy=1.57),
        box(1.48, 0.06, 0.08, oy=0.63),
        box(0.06, 0.96, 0.08, -0.71, 1.1),
        box(0.06, 0.96, 0.08, 0.71, 1.1),
    )
    return merge(board, frame)


def building_module() -> Mesh:
    """1×1 tile slab; aligns with HqGridLayout.TileSize (0.45m)."""
    t = 0.45
    floor = box(t, 0.12, t, oy=0.06)
    wall_n = box(t, 0.55, 0.08, oy=0.35, oz=-t * 0.5 + 0.04)
    wall_s = box(t, 0.55, 0.08, oy=0.35, oz=t * 0.5 - 0.04)
    wall_w = box(0.08, 0.55, t - 0.16, ox=-t * 0.5 + 0.04, oy=0.35)
    wall_e = box(0.08, 0.55, t - 0.16, ox=t * 0.5 - 0.04, oy=0.35)
    return merge(floor, wall_n, wall_s, wall_w, wall_e)


def quad_tree() -> Mesh:
    trunk = box(0.18, 0.9, 0.18, oy=0.45)
    foliage = box(1.1, 1.2, 1.1, oy=1.4)
    return merge(trunk, foliage)


def sign_pole() -> Mesh:
    pole = box(0.08, 2.2, 0.08, oy=1.1)
    sign = box(0.9, 0.5, 0.06, oy=1.85)
    return merge(pole, sign)


def world_map_pin() -> Mesh:
    stem = box(0.06, 0.35, 0.06, oy=0.175)
    head = box(0.22, 0.22, 0.22, oy=0.46)
    # octahedron-ish via two pyramids (box stack)
    tip = box(0.12, 0.18, 0.12, oy=0.68)
    return merge(stem, head, tip)


def rival_badge() -> Mesh:
    """Diamond plaque for rival lab markers."""
    return box(0.5, 0.5, 0.08, oy=0.25)


def gpu_blade() -> Mesh:
    return box(0.35, 0.02, 0.9, oy=0.01)


def coffee_cart() -> Mesh:
    base = box(0.8, 0.85, 0.5, oy=0.425)
    umbrella = box(1.0, 0.06, 1.0, oy=1.2)
    post = box(0.06, 0.35, 0.06, oy=1.0)
    return merge(base, post, umbrella)


def research_bench() -> Mesh:
    table = box(1.0, 0.75, 0.6, oy=0.375)
    scope = box(0.12, 0.35, 0.12, 0.25, 0.95, 0.0)
    lens = box(0.18, 0.08, 0.18, 0.25, 1.15, 0.0)
    return merge(table, scope, lens)


def data_stack() -> Mesh:
    disks = merge()
    for i in range(4):
        disks = merge(disks, box(0.5, 0.08, 0.5, oy=0.04 + i * 0.1))
    return disks


def gauntlet_lectern() -> Mesh:
    podium = box(0.55, 1.0, 0.45, oy=0.5)
    screen = box(0.7, 0.45, 0.05, oy=1.15, oz=-0.2)
    return merge(podium, screen)


def bloc_tile() -> Mesh:
    return box(1.0, 0.15, 1.0, oy=0.075)


def safety_cone() -> Mesh:
    # stacked boxes approximate a cone for low-poly style
    parts = merge()
    for i in range(4):
        w = 0.5 - i * 0.1
        parts = merge(parts, box(w, 0.12, w, oy=0.06 + i * 0.12))
    return parts


def monitor_wall() -> Mesh:
    return box(2.0, 1.2, 0.08, oy=0.6)


def bike_rack() -> Mesh:
    rail = box(1.4, 0.08, 0.25, oy=0.5)
    post_a = box(0.08, 1.0, 0.08, -0.6, 0.5, 0)
    post_b = box(0.08, 1.0, 0.08, 0.6, 0.5, 0)
    return merge(rail, post_a, post_b)


def tab_rail_plaque() -> Mesh:
    return box(0.56, 0.56, 0.04, oy=0.28)


def lounge_sofa() -> Mesh:
    seat = box(1.6, 0.45, 0.75, oy=0.225)
    back = box(1.6, 0.55, 0.18, oy=0.5, oz=-0.28)
    arm_l = box(0.12, 0.5, 0.75, -0.74, 0.25)
    arm_r = box(0.12, 0.5, 0.75, 0.74, 0.25)
    return merge(seat, back, arm_l, arm_r)


def fence_segment() -> Mesh:
    post_l = box(0.1, 1.0, 0.1, -1.0, 0.5)
    post_r = box(0.1, 1.0, 0.1, 1.0, 0.5)
    rail_t = box(2.1, 0.06, 0.06, oy=0.85)
    rail_m = box(2.1, 0.06, 0.06, oy=0.45)
    return merge(post_l, post_r, rail_t, rail_m)


def solar_shade() -> Mesh:
    canopy = box(1.8, 0.08, 1.2, oy=2.4)
    leg_fl = box(0.08, 2.4, 0.08, -0.75, 1.2, -0.5)
    leg_fr = box(0.08, 2.4, 0.08, 0.75, 1.2, -0.5)
    leg_bl = box(0.08, 2.4, 0.08, -0.75, 1.2, 0.5)
    leg_br = box(0.08, 2.4, 0.08, 0.75, 1.2, 0.5)
    return merge(canopy, leg_fl, leg_fr, leg_bl, leg_br)


def water_cooler() -> Mesh:
    body = box(0.4, 0.9, 0.4, oy=0.45)
    jug = box(0.28, 0.35, 0.28, oy=1.05)
    drip = box(0.08, 0.12, 0.08, oy=0.06, oz=0.22)
    return merge(body, jug, drip)


def elevator_core() -> Mesh:
    shaft = box(1.2, 2.8, 1.2, oy=1.4)
    door = box(0.9, 2.0, 0.06, oy=1.0, oz=0.57)
    return merge(shaft, door)


def cafeteria_table() -> Mesh:
    top = box(1.4, 0.08, 0.8, oy=0.74)
    leg = box(0.1, 0.74, 0.1, 0.0, 0.37, 0.0)
    return merge(top, leg)


def vending_machine() -> Mesh:
    body = box(0.55, 1.6, 0.5, oy=0.8)
    glass = box(0.45, 0.7, 0.04, oy=1.0, oz=0.23)
    return merge(body, glass)


def hallway_bench() -> Mesh:
    seat = box(1.5, 0.1, 0.45, oy=0.45)
    back = box(1.5, 0.5, 0.08, oy=0.7, oz=-0.18)
    leg_l = box(0.08, 0.45, 0.35, -0.65, 0.225)
    leg_r = box(0.08, 0.45, 0.35, 0.65, 0.225)
    return merge(seat, back, leg_l, leg_r)


MESH_BUILDERS: dict[str, callable] = {
    "prop-desk": desk,
    "prop-chair": chair,
    "prop-server-rack": server_rack,
    "prop-potted-plant": potted_plant,
    "prop-whiteboard": whiteboard,
    "building-module-1x1": building_module,
    "exterior-quad-tree": quad_tree,
    "exterior-sign-pole": sign_pole,
    "ui-world-map-pin": world_map_pin,
    "ui-rival-badge": rival_badge,
    "prop-gpu-blade": gpu_blade,
    "prop-coffee-cart": coffee_cart,
    "prop-research-bench": research_bench,
    "prop-data-stack": data_stack,
    "prop-gauntlet-lectern": gauntlet_lectern,
    "ui-bloc-tile": bloc_tile,
    "prop-safety-cone": safety_cone,
    "prop-monitor-wall": monitor_wall,
    "exterior-bike-rack": bike_rack,
    "ui-tab-rail-plaque": tab_rail_plaque,
    "prop-lounge-sofa": lounge_sofa,
    "exterior-fence-segment": fence_segment,
    "exterior-solar-shade": solar_shade,
    "prop-water-cooler": water_cooler,
    "building-elevator-core": elevator_core,
    "prop-cafeteria-table": cafeteria_table,
    "prop-vending-machine": vending_machine,
    "exterior-hallway-bench": hallway_bench,
}


def main() -> None:
    manifest_entries = []
    for asset_id, builder in MESH_BUILDERS.items():
        mesh = builder()
        mesh.name = asset_id
        rel = Path("campus-kit") / f"{asset_id}.obj"
        out = OUT_DIR / f"{asset_id}.obj"
        mesh.write_obj(out)
        manifest_entries.append(
            {
                "id": asset_id,
                "path": f"Assets/Art/Generated/campus-kit/{asset_id}.obj",
                "license": "CC0-1.0 (procedural, in-repo)",
                "vertexCount": len(mesh.vertices),
                "faceCount": len(mesh.faces),
                "pivot": "base-centre unless noted",
                "scaleMeters": 1.0,
            }
        )

    manifest = {
        "generator": "tools/art/generate_campus_kit.py",
        "tileSizeMeters": 0.45,
        "assets": manifest_entries,
    }
    manifest_path = REPO_ROOT / "unity" / "Assets" / "Art" / "Generated" / "manifest.json"
    manifest_path.parent.mkdir(parents=True, exist_ok=True)
    manifest_path.write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {len(manifest_entries)} OBJ files under {OUT_DIR}")
    print(f"Manifest: {manifest_path}")


if __name__ == "__main__":
    main()