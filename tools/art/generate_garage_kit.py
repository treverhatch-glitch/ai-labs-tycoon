#!/usr/bin/env python3
"""
Garage HQ environment + 1995 props for Mum's Garage 14×10 lot.

License: CC0 — procedural geometry, no third-party meshes.
"""
from __future__ import annotations

import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from generate_campus_kit import Mesh, box, merge, server_rack, whiteboard  # noqa: E402

REPO_ROOT = Path(__file__).resolve().parents[2]
OUT_DIR = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "garage-kit"


def garage_floor_tile() -> Mesh:
    """0.45 m carpet tile (HqGridLayout.TileSize)."""
    slab = box(0.45, 0.04, 0.45, oy=0.02)
    pad = box(0.38, 0.01, 0.38, oy=0.045)
    return merge(slab, pad)


def garage_wall_panel() -> Mesh:
    panel = box(0.45, 2.2, 0.08, oy=1.1)
    baseboard = box(0.45, 0.12, 0.1, oy=0.06, oz=0.02)
    return merge(panel, baseboard)


def garage_exterior_wall() -> Mesh:
    core = box(0.45, 2.4, 0.18, oy=1.2)
    trim = box(0.48, 0.08, 0.2, oy=2.35, oz=0.01)
    return merge(core, trim)


def garage_roll_door() -> Mesh:
    frame = box(2.0, 2.2, 0.12, oy=1.1)
    slat = merge()
    for i in range(8):
        slat = merge(slat, box(1.85, 0.14, 0.04, oy=0.2 + i * 0.24, oz=0.04))
    handle = box(0.25, 0.08, 0.08, 0.7, 0.35, 0.08)
    return merge(frame, slat, handle)


def garage_server_closet() -> Mesh:
    """Server closet beside the garage; fits one rack."""
    shell = box(1.1, 2.0, 0.9, oy=1.0)
    door = box(0.9, 1.75, 0.06, oy=0.95, oz=0.42)
    vent = box(0.7, 0.12, 0.04, oy=1.85, oz=0.44)
    return merge(shell, door, vent)


def garage_land_for_sale_sign() -> Mesh:
    post = box(0.1, 1.6, 0.1, oy=0.8)
    board = box(1.2, 0.7, 0.08, oy=1.45)
    stake = box(0.08, 0.5, 0.08, -0.55, 0.25, 0)
    return merge(post, board, stake)


def garage_lot_curb() -> Mesh:
    """Border segment for the land-for-sale lot edge."""
    curb = box(0.45, 0.14, 0.22, oy=0.07)
    grass = box(0.45, 0.03, 0.28, oy=0.165, oz=0.12)
    return merge(curb, grass)


def garage_window_frame() -> Mesh:
    frame = box(0.9, 0.7, 0.1, oy=1.5)
    pane = box(0.7, 0.5, 0.04, oy=1.5, oz=0.03)
    sill = box(0.95, 0.06, 0.14, oy=1.12, oz=0.02)
    return merge(frame, pane, sill)


def garage_desk_crt() -> Mesh:
    top = box(1.1, 0.06, 0.65, oy=0.74)
    leg_fl = box(0.07, 0.74, 0.07, -0.48, 0.37, -0.26)
    leg_fr = box(0.07, 0.74, 0.07, 0.48, 0.37, -0.26)
    leg_bl = box(0.07, 0.74, 0.07, -0.48, 0.37, 0.26)
    leg_br = box(0.07, 0.74, 0.07, 0.48, 0.37, 0.26)
    crt = box(0.48, 0.42, 0.45, 0.12, 0.88, -0.05)
    crt_face = box(0.4, 0.32, 0.04, 0.12, 0.9, 0.18)
    keyboard = box(0.42, 0.03, 0.18, 0.0, 0.77, 0.15)
    return merge(top, leg_fl, leg_fr, leg_bl, leg_br, crt, crt_face, keyboard)


def garage_crt_tower() -> Mesh:
    body = box(0.22, 0.55, 0.42, oy=0.275)
    floppy = box(0.18, 0.06, 0.32, oy=0.42, oz=0.2)
    led = box(0.04, 0.04, 0.02, 0.05, 0.52, 0.22)
    return merge(body, floppy, led)


def garage_voodude_rack() -> Mesh:
    """VooDude 1-era GPU rack (1995)."""
    rack = server_rack()
    badge = box(0.35, 0.12, 0.02, oy=1.72, oz=0.38)
    card = box(0.45, 0.08, 0.5, oy=0.85, oz=0.2)
    fan = box(0.2, 0.2, 0.04, oy=0.85, oz=0.46)
    return merge(rack, badge, card, fan)


def garage_whiteboard_1995() -> Mesh:
    board = whiteboard()
    marker_tray = box(0.5, 0.04, 0.12, oy=0.62, oz=0.04)
    return merge(board, marker_tray)


def garage_coffee_drip() -> Mesh:
    base = box(0.28, 0.32, 0.22, oy=0.16)
    warmer = box(0.2, 0.06, 0.18, oy=0.35)
    pot = box(0.18, 0.22, 0.14, oy=0.5, oz=0.05)
    return merge(base, warmer, pot)


def garage_folding_chair() -> Mesh:
    seat = box(0.5, 0.04, 0.48, oy=0.48)
    back = box(0.5, 0.45, 0.04, oy=0.72, oz=-0.22)
    leg_a = box(0.05, 0.48, 0.05, -0.2, 0.24, 0.15)
    leg_b = box(0.05, 0.48, 0.05, 0.2, 0.24, -0.15)
    return merge(seat, back, leg_a, leg_b)


def garage_cable_spool() -> Mesh:
    hub = box(0.2, 0.16, 0.2, oy=0.08)
    coil = box(0.32, 0.24, 0.32, oy=0.2)
    return merge(hub, coil)


MESH_BUILDERS: dict[str, callable] = {
    "garage-floor-tile": garage_floor_tile,
    "garage-wall-panel": garage_wall_panel,
    "garage-exterior-wall": garage_exterior_wall,
    "garage-roll-door": garage_roll_door,
    "garage-server-closet": garage_server_closet,
    "garage-land-for-sale-sign": garage_land_for_sale_sign,
    "garage-lot-curb": garage_lot_curb,
    "garage-window-frame": garage_window_frame,
    "garage-desk-crt": garage_desk_crt,
    "garage-crt-tower": garage_crt_tower,
    "garage-voodude-rack": garage_voodude_rack,
    "garage-whiteboard-1995": garage_whiteboard_1995,
    "garage-coffee-drip": garage_coffee_drip,
    "garage-folding-chair": garage_folding_chair,
    "garage-cable-spool": garage_cable_spool,
}


def main() -> None:
    manifest_entries = []
    for asset_id, builder in MESH_BUILDERS.items():
        mesh = builder()
        mesh.name = asset_id
        out = OUT_DIR / f"{asset_id}.obj"
        mesh.write_obj(out)
        manifest_entries.append(
            {
                "id": asset_id,
                "path": f"Assets/Art/EarlyGame/garage-kit/{asset_id}.obj",
                "license": "CC0-1.0 (procedural, in-repo)",
                "vertexCount": len(mesh.vertices),
                "faceCount": len(mesh.faces),
                "era": "dial-up-days",
                "scaleMeters": 1.0,
            }
        )

    manifest = {
        "generator": "tools/art/generate_garage_kit.py",
        "lot": "garage",
        "gridTiles": {"width": 14, "height": 10},
        "tileSizeMeters": 0.45,
        "assets": manifest_entries,
    }
    manifest_path = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "garage-manifest.json"
    manifest_path.parent.mkdir(parents=True, exist_ok=True)
    manifest_path.write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {len(manifest_entries)} garage OBJ under {OUT_DIR}")


if __name__ == "__main__":
    main()