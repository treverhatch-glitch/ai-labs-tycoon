#!/usr/bin/env python3
"""
Garage HQ environment + 1995 props for Mum's Garage 14×10 lot (UVs, materials, 300+ tris on hero props).

License: CC0 — procedural geometry, no third-party meshes.
"""
from __future__ import annotations

import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from garage_kit_builders import (  # noqa: E402
    MATS,
    build_crt_tower,
    build_desk_crt,
    build_folding_chair,
    build_voodude_rack,
)
from mesh_lib import MaterialDef, RichMesh, bevel_box, cylinder, merge_rich, mesh_stats, write_obj_with_mtl  # noqa: E402

REPO_ROOT = Path(__file__).resolve().parents[2]
OUT_DIR = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "garage-kit"
MATERIAL_DEFS = {k: MaterialDef(k, v) for k, v in MATS.items()}


def garage_floor_tile() -> RichMesh:
    slab = bevel_box(0.45, 0.04, 0.45, mat="carpet_beige")
    slab.name = "garage-floor-tile"
    return slab


def garage_wall_panel() -> RichMesh:
    m = merge_rich(
        bevel_box(0.45, 2.2, 0.08, mat="wall_beige"),
        bevel_box(0.45, 0.12, 0.1, oz=0.02, mat="trim_orange"),
    )
    m.name = "garage-wall-panel"
    return m


def garage_exterior_wall() -> RichMesh:
    m = merge_rich(
        bevel_box(0.45, 2.4, 0.18, mat="concrete_warm"),
        bevel_box(0.48, 0.08, 0.2, oy=2.27, mat="trim_orange"),
    )
    m.name = "garage-exterior-wall"
    return m


def garage_roll_door() -> RichMesh:
    parts = [bevel_box(2.0, 2.2, 0.12, mat="metal_scratched")]
    for i in range(10):
        parts.append(bevel_box(1.88, 0.12, 0.04, oy=0.12 + i * 0.2, oz=0.04, mat="metal_frame"))
    parts.append(bevel_box(0.25, 0.08, 0.08, 0.72, 0.35, 0.08, mat="trim_orange"))
    m = merge_rich(*parts)
    m.name = "garage-roll-door"
    return m


def garage_server_closet() -> RichMesh:
    m = merge_rich(
        bevel_box(1.1, 2.0, 0.9, mat="rack_paint"),
        bevel_box(0.88, 1.72, 0.06, oz=0.42, mat="metal_scratched"),
        bevel_box(0.7, 0.12, 0.04, oy=1.85, oz=0.44, mat="metal_frame"),
    )
    m.name = "garage-server-closet"
    return m


def garage_land_for_sale_sign() -> RichMesh:
    m = merge_rich(
        cylinder(0.05, 1.6, segments=8, mat="wood_dark"),
        bevel_box(1.2, 0.7, 0.08, oy=1.45, mat="sign_board"),
        bevel_box(0.08, 0.5, 0.08, -0.55, oy=0.25, mat="wood_dark"),
    )
    m.name = "garage-land-for-sale-sign"
    return m


def garage_lot_curb() -> RichMesh:
    m = merge_rich(
        bevel_box(0.45, 0.14, 0.22, mat="concrete_warm"),
        bevel_box(0.45, 0.03, 0.28, oy=0.15, oz=0.12, mat="carpet_beige"),
    )
    m.name = "garage-lot-curb"
    return m


def garage_window_frame() -> RichMesh:
    m = merge_rich(
        bevel_box(0.9, 0.7, 0.1, oy=1.5, mat="wood_oak"),
        bevel_box(0.7, 0.5, 0.04, oy=1.5, oz=0.03, mat="glass_pane"),
        bevel_box(0.95, 0.06, 0.14, oy=1.12, oz=0.02, mat="trim_orange"),
    )
    m.name = "garage-window-frame"
    return m


def garage_whiteboard_1995() -> RichMesh:
    m = merge_rich(
        bevel_box(1.4, 0.9, 0.05, oy=1.1, mat="plastic_keys"),
        bevel_box(1.48, 0.06, 0.08, oy=1.57, mat="metal_frame"),
        bevel_box(0.5, 0.04, 0.12, oy=0.62, oz=0.04, mat="metal_scratched"),
    )
    m.name = "garage-whiteboard-1995"
    return m


def garage_coffee_drip() -> RichMesh:
    m = merge_rich(
        bevel_box(0.28, 0.32, 0.22, mat="plastic_char"),
        bevel_box(0.2, 0.06, 0.18, oy=0.35, mat="metal_scratched"),
        cylinder(0.1, 0.22, segments=10, oy=0.39, oz=0.05, mat="glass_pane"),
    )
    m.name = "garage-coffee-drip"
    return m


def garage_cable_spool() -> RichMesh:
    m = merge_rich(
        cylinder(0.1, 0.16, segments=12, mat="wood_oak"),
        cylinder(0.16, 0.24, segments=14, oy=0.08, mat="copper_coil"),
    )
    m.name = "garage-cable-spool"
    return m


MESH_BUILDERS: dict[str, callable] = {
    "garage-floor-tile": garage_floor_tile,
    "garage-wall-panel": garage_wall_panel,
    "garage-exterior-wall": garage_exterior_wall,
    "garage-roll-door": garage_roll_door,
    "garage-server-closet": garage_server_closet,
    "garage-land-for-sale-sign": garage_land_for_sale_sign,
    "garage-lot-curb": garage_lot_curb,
    "garage-window-frame": garage_window_frame,
    "garage-desk-crt": build_desk_crt,
    "garage-crt-tower": build_crt_tower,
    "garage-voodude-rack": build_voodude_rack,
    "garage-whiteboard-1995": garage_whiteboard_1995,
    "garage-coffee-drip": garage_coffee_drip,
    "garage-folding-chair": build_folding_chair,
    "garage-cable-spool": garage_cable_spool,
}


def main() -> None:
    manifest_entries = []
    for asset_id, builder in MESH_BUILDERS.items():
        mesh = builder()
        if not mesh.name:
            mesh.name = asset_id
        out = OUT_DIR / f"{asset_id}.obj"
        write_obj_with_mtl(mesh, out, MATERIAL_DEFS)
        verts, tris = mesh_stats(mesh)
        manifest_entries.append(
            {
                "id": asset_id,
                "path": f"Assets/Art/EarlyGame/garage-kit/{asset_id}.obj",
                "mtl": f"Assets/Art/EarlyGame/garage-kit/{asset_id}.mtl",
                "license": "CC0-1.0 (procedural, in-repo)",
                "vertexCount": verts,
                "triangleCount": tris,
                "era": "dial-up-days",
                "scaleMeters": 1.0,
            }
        )

    manifest = {
        "generator": "tools/art/generate_garage_kit.py",
        "lot": "garage",
        "gridTiles": {"width": 14, "height": 10},
        "tileSizeMeters": 0.45,
        "fidelity": "packet-2",
        "assets": manifest_entries,
    }
    manifest_path = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "garage-manifest.json"
    manifest_path.parent.mkdir(parents=True, exist_ok=True)
    manifest_path.write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {len(manifest_entries)} garage OBJ+MTL under {OUT_DIR}")


if __name__ == "__main__":
    main()