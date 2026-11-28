#!/usr/bin/env python3
"""Re-render hero previews for garage manifest props (no rebake)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy

REPO = Path(__file__).resolve().parents[3]
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
TEX = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures"
PREVIEW = REPO / "docs/art/previews"
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"


_FRAMING: dict[str, tuple[float, float]] = {
    "garage-cable-spool": (0.54, 0.08),
    "garage-exterior-wall": (0.54, 0.08),
    "garage-land-for-sale-sign": (0.56, 0.075),
    "garage-server-closet": (0.54, 0.08),
    "garage-folding-chair": (0.52, 0.09),
    "garage-window-frame": (0.56, 0.08),
}


def rerender(asset_id: str) -> None:
    glb = KIT / f"{asset_id}.glb"
    if not glb.is_file():
        raise FileNotFoundError(glb)
    bpy.ops.wm.read_factory_settings(use_empty=True)
    before = set(bpy.context.scene.objects)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    imported = [o for o in bpy.context.scene.objects if o not in before and o.type == "MESH"]
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    import blender_common as common  # type: ignore
    import cc0_common as cc0  # type: ignore

    joined = common.duplicate_join_meshes(imported, f"{asset_id}_preview")
    for ob in imported:
        bpy.data.objects.remove(ob, do_unlink=True)
    tex_dir = TEX / asset_id
    common.apply_preview_material(joined, tex_dir, asset_id)
    preview_path = PREVIEW / f"{asset_id}.png"
    mask_path = PREVIEW / f"{asset_id}.mask.png"
    fill, margin = _FRAMING.get(asset_id, (0.62, 0.06))
    cc0.render_kit_preview(
        [joined],
        preview_path,
        resolution=512,
        mode="hero",
        fill=fill,
        margin=margin,
        up_axis="Z",
        mask_path=mask_path,
    )
    meta_path = preview_path.with_name(preview_path.stem + ".frame.json")
    meta_path.write_text(
        json.dumps(
            {
                "preview_kind": "garage",
                "flat_backdrop": True,
                "backdrop_rgb": [150, 150, 150],
                "framing_fill": fill,
                "framing_margin": margin,
                "up_axis": "Z",
                "floor_plane": "horizontal_z",
                "renderer": "cycles_tpH",
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )
    print("OK rerender", asset_id)


def main() -> None:
    assets = [a["id"] for a in json.loads(MANIFEST.read_text())["assets"]]
    for aid in assets:
        rerender(aid)


if __name__ == "__main__":
    main()