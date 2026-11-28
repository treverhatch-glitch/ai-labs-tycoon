#!/usr/bin/env python3
"""Fail garage kit if albedo has >6 hue clusters or GLB has <2 materials."""
from __future__ import annotations

import argparse
import json
import os
import struct
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
TEX_ROOT = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures"
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"
MAX_HUE_CLUSTERS = 6
MIN_MATERIALS = 2
BLENDER = os.environ.get("BLENDER_BIN", "blender")

BLENDER_INVENTORY = """
import json
import sys
from pathlib import Path

import bpy
from mathutils import Vector

repo = Path(sys.argv[-1])
manifest = json.loads((repo / "unity/Assets/Art/EarlyGame/garage-manifest.json").read_text())
for entry in manifest["assets"]:
    asset_id = entry["id"]
    glb = repo / "unity/Assets/Art/EarlyGame/garage-kit" / f"{asset_id}.glb"
    bpy.ops.wm.read_factory_settings(use_empty=True)
    if not glb.exists():
        print(f"MISSING {asset_id}")
        continue
    bpy.ops.import_scene.gltf(filepath=str(glb))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    names = [o.name for o in meshes]
    mins = [1e9, 1e9, 1e9]
    maxs = [-1e9, -1e9, -1e9]
    for ob in meshes:
        for corner in ob.bound_box:
            w = ob.matrix_world @ Vector(corner)
            mins[0] = min(mins[0], w.x)
            mins[1] = min(mins[1], w.y)
            mins[2] = min(mins[2], w.z)
            maxs[0] = max(maxs[0], w.x)
            maxs[1] = max(maxs[1], w.y)
            maxs[2] = max(maxs[2], w.z)
    mat_count = 0
    if meshes:
        mat_count = len([m for m in meshes[0].data.materials if m is not None])
    print(
        f"INV {asset_id} objects={names} materials={mat_count} "
        f"bbox_min={[round(x, 4) for x in mins]} bbox_max={[round(x, 4) for x in maxs]}"
    )
"""


def glb_material_count(glb: Path) -> int:
    raw = glb.read_bytes()
    if len(raw) < 20:
        return 0
    off = 12
    chunk_len, chunk_type = struct.unpack_from("<II", raw, off)
    off += 8
    if chunk_type != 0x4E4F534A:
        return 0
    payload = raw[off : off + chunk_len]
    gltf = json.loads(payload.decode("utf-8"))
    return len(gltf.get("materials", []))


def count_hue_clusters(rgb, *, min_sat: float = 0.14, bin_deg: float = 20.0) -> int:
    import colorsys

    import numpy as np

    flat = np.asarray(rgb, dtype=np.float64).reshape(-1, 3) / 255.0
    total = len(flat)
    counts: dict[int, int] = {}
    for r, g, b in flat:
        h, s, v = colorsys.rgb_to_hsv(float(r), float(g), float(b))
        if s < min_sat or v < 0.07:
            continue
        bin_id = int((h * 360.0) // bin_deg) % int(360 // bin_deg)
        counts[bin_id] = counts.get(bin_id, 0) + 1
    min_pix = max(48, int(total * 0.0035))
    return sum(1 for c in counts.values() if c >= min_pix)


def check_albedo(asset_id: str) -> tuple[int, int]:
    albedo = TEX_ROOT / asset_id / f"{asset_id}_albedo.png"
    if not albedo.exists():
        print(f"FAIL {asset_id} missing albedo {albedo}")
        return 1, 0
    from PIL import Image

    import numpy as np

    img = Image.open(albedo).convert("RGB")
    arr = np.array(img)
    clusters = count_hue_clusters(arr)
    return (1, clusters) if clusters > MAX_HUE_CLUSTERS else (0, clusters)


def check_materials(asset_id: str) -> tuple[int, int]:
    glb = KIT / f"{asset_id}.glb"
    if not glb.exists():
        print(f"FAIL {asset_id} missing glb")
        return 1, 0
    n = glb_material_count(glb)
    return (1, n) if n < MIN_MATERIALS else (0, n)


def print_glb_inventory() -> None:
    subprocess.run([BLENDER, "-b", "--python-expr", BLENDER_INVENTORY, "--", str(REPO)], check=False)


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--inventory", action="store_true", help="Print GLB object names and bboxes")
    parser.add_argument(
        "--expect-fail",
        action="store_true",
        help="Exit 0 when failures present (proof runs on pass-5 head)",
    )
    args = parser.parse_args()

    if args.inventory:
        print_glb_inventory()
        return 0

    try:
        from PIL import Image  # noqa: F401
    except ImportError:
        print("FAIL: Pillow required")
        return 1

    manifest = json.loads(MANIFEST.read_text(encoding="utf-8"))
    kit = REPO / "unity/Assets/Art/EarlyGame/garage-kit"

    def _import_restyle(entry: dict) -> bool:
        if entry.get("source") in ("cc0-polyhaven", "kenney-cc0"):
            return True
        sidecar = kit / f"{entry['id']}.asset.json"
        if sidecar.is_file():
            lic = json.loads(sidecar.read_text(encoding="utf-8")).get("license", "")
            return "Kenney" in lic or "Poly Haven" in lic
        return False

    rc = 0
    for entry in manifest["assets"]:
        asset_id = entry["id"]
        if _import_restyle(entry):
            fail_h, clusters = check_albedo(asset_id)
            if fail_h:
                print(f"FAIL {asset_id} hue_clusters={clusters} (max {MAX_HUE_CLUSTERS})")
                rc = 1
            else:
                print(f"OK {asset_id} cc0 hue_clusters={clusters}")
            continue
        fail_h, clusters = check_albedo(asset_id)
        fail_m, mats = check_materials(asset_id)
        if fail_h:
            print(f"FAIL {asset_id} hue_clusters={clusters} (max {MAX_HUE_CLUSTERS})")
        if fail_m:
            print(f"FAIL {asset_id} glb_materials={mats} (need >={MIN_MATERIALS})")
        if fail_h or fail_m:
            rc = 1
        else:
            print(f"OK {asset_id} hue_clusters={clusters} glb_materials={mats}")

    if args.expect_fail and rc != 0:
        print("OK proof: gate failed as expected on pass-5 head assets")
        return 0
    if args.expect_fail and rc == 0:
        print("FAIL proof: expected gate failure on pass-5 head but all checks passed")
        return 1
    return rc


if __name__ == "__main__":
    raise SystemExit(main())