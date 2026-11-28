#!/usr/bin/env python3
"""Garage preview assembly: grounded multipart GLBs + flat #969696 studio meta."""
from __future__ import annotations

import json
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"

FLOOR_SLACK = 0.04
BACKDROP_RGB = (150, 150, 150)
FLOOR_PLANES = frozenset({"horizontal_y", "horizontal_z"})


def check_frame_meta(asset_id: str) -> int:
    meta = PREVIEW / f"{asset_id}.frame.json"
    if not meta.is_file():
        print(f"FAIL {asset_id} missing frame meta")
        return 1
    data = json.loads(meta.read_text(encoding="utf-8"))
    rc = 0
    if data.get("preview_kind") != "garage":
        print(f"FAIL {asset_id} preview_kind={data.get('preview_kind')!r} need garage")
        rc = 1
    if not data.get("flat_backdrop"):
        print(f"FAIL {asset_id} flat_backdrop missing (need #969696 studio)")
        rc = 1
    fp = data.get("floor_plane")
    if fp not in FLOOR_PLANES:
        print(f"FAIL {asset_id} floor_plane={fp!r}")
        rc = 1
    bg = data.get("backdrop_rgb")
    if bg != list(BACKDROP_RGB):
        print(f"FAIL {asset_id} backdrop_rgb={bg} need {list(BACKDROP_RGB)}")
        rc = 1
    if rc == 0:
        print(f"OK {asset_id} studio_flat floor={fp} up={data.get('up_axis')}")
    return rc


def _glb_part_layout(glb: Path) -> list[dict]:
    script = f"""
import bpy
from pathlib import Path
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath={str(glb)!r})
rows = []
for ob in bpy.data.objects:
    if ob.type != 'MESH':
        continue
    ws = [ob.matrix_world @ v.co for v in ob.data.vertices]
    if not ws:
        continue
    ys = [p.y for p in ws]
    xs = [p.x for p in ws]
    zs = [p.z for p in ws]
    rows.append({{
        "name": ob.name,
        "min_y": float(min(ys)),
        "max_y": float(max(ys)),
        "cx": float(sum(xs) / len(xs)),
        "cy": float(sum(ys) / len(ys)),
        "cz": float(sum(zs) / len(zs)),
    }})
print("PARTLAYOUT", rows)
"""
    proc = subprocess.run(
        ["blender", "-b", "--python-expr", script],
        cwd=REPO,
        capture_output=True,
        text=True,
        check=False,
    )
    if proc.returncode != 0:
        raise RuntimeError(proc.stderr or proc.stdout)
    for line in proc.stdout.splitlines():
        if line.startswith("PARTLAYOUT"):
            return eval(line.split("PARTLAYOUT", 1)[1].strip())
    return []


def check_glb_grounded(asset_id: str) -> int:
    glb = KIT / f"{asset_id}.glb"
    if not glb.is_file():
        print(f"FAIL missing {glb}")
        return 1
    try:
        parts = _glb_part_layout(glb)
    except RuntimeError as exc:
        print(f"FAIL {asset_id} glb_import {exc}")
        return 1
    if len(parts) < 5 and asset_id != "garage-floor-tile":
        print(f"FAIL {asset_id} mesh_parts={len(parts)} (need >=5)")
        return 1
    assembly_min_y = min(p["min_y"] for p in parts)
    if assembly_min_y > FLOOR_SLACK:
        print(f"FAIL {asset_id} assembly_min_y={assembly_min_y:.4f} > {FLOOR_SLACK}")
        return 1
    below = [p for p in parts if p["min_y"] < -0.02]
    if below:
        print(f"FAIL {asset_id} below_floor {below[0]['name']} min_y={below[0]['min_y']:.4f}")
        return 1
    xs = [p["cx"] for p in parts]
    zs = [p["cz"] for p in parts]
    span_x = max(xs) - min(xs)
    span_z = max(zs) - min(zs)
    margin = max(0.35, span_x, span_z) * 0.15
    x0, x1 = min(xs) - margin, max(xs) + margin
    z0, z1 = min(zs) - margin, max(zs) + margin
    exploded = [
        p
        for p in parts
        if p["cx"] < x0 or p["cx"] > x1 or p["cz"] < z0 or p["cz"] > z1
    ]
    if exploded:
        bad = exploded[0]
        print(f"FAIL {asset_id} exploded_part {bad['name']} off_layout xz=({bad['cx']:.2f},{bad['cz']:.2f})")
        return 1
    print(f"OK {asset_id} grounded assembly_min_y={assembly_min_y:.3f} parts={len(parts)}")
    rc = check_glb_assembly_rules(asset_id, parts)
    return rc


def check_glb_assembly_rules(asset_id: str, parts: list[dict]) -> int:
    """Real assembly: distinct leg corners, not bogus cluster graph."""
    legs = sorted([p for p in parts if "Leg" in p["name"] or p["name"].startswith("Leg")], key=lambda p: p["name"])
    if asset_id == "garage-folding-chair" and len(legs) >= 4:
        pts = [(l["cx"], l["cy"]) for l in legs[:4]]
        for i in range(4):
            for j in range(i + 1, 4):
                d = ((pts[i][0] - pts[j][0]) ** 2 + (pts[i][1] - pts[j][1]) ** 2) ** 0.5
                if d < 0.10:
                    print(f"FAIL {asset_id} legs_overlap {legs[i]['name']} vs {legs[j]['name']} d={d:.3f}")
                    return 1
        print(f"OK {asset_id} four_distinct_leg_corners")
    if asset_id == "garage-desk-crt" and len(legs) >= 4:
        pts = [(l["cx"], l["cy"]) for l in legs[:4]]
        for i in range(4):
            for j in range(i + 1, 4):
                d = ((pts[i][0] - pts[j][0]) ** 2 + (pts[i][1] - pts[j][1]) ** 2) ** 0.5
                if d < 0.12:
                    print(f"FAIL {asset_id} legs_overlap {legs[i]['name']} vs {legs[j]['name']} d={d:.3f}")
                    return 1
        print(f"OK {asset_id} desk_legs_under_top")
    return 0


def main() -> int:
    assets = json.loads(MANIFEST.read_text(encoding="utf-8"))["assets"]
    rc = 0
    for entry in assets:
        aid = entry["id"]
        rc |= check_frame_meta(aid)
        rc |= check_glb_grounded(aid)
    rc |= check_frame_meta("garage-scene-kit")
    return rc


if __name__ == "__main__":
    sys.exit(main())