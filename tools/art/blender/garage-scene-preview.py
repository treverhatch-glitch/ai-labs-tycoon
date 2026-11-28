#!/usr/bin/env python3
"""Contact sheet: garage props assembled on studio grey (TPH-style framing)."""
from __future__ import annotations

import json
import os
import sys
from pathlib import Path

import bpy
from mathutils import Vector

REPO = Path(__file__).resolve().parents[3]
KIT = REPO / "unity/Assets/Art/EarlyGame/garage-kit"
PREVIEW = REPO / "docs/art/previews"
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"

COLS = 4
SPACING = 0.44
PROP_SCALE = 1.0
FLOOR_Y = 0.0


def import_prop_meshes(asset_id: str, loc: tuple[float, float, float]) -> list[bpy.types.Object]:
    glb = KIT / f"{asset_id}.glb"
    if not glb.exists():
        print(f"WARN missing {glb}")
        return []
    before = set(bpy.context.scene.objects)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    imported = [o for o in bpy.context.scene.objects if o not in before and o.type == "MESH"]
    if not imported:
        return []
    import blender_common as common  # type: ignore

    tex_dir = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures" / asset_id
    for ob in imported:
        common.apply_preview_material(ob, tex_dir, asset_id)
        ob.scale = (PROP_SCALE, PROP_SCALE, PROP_SCALE)
    bpy.context.view_layer.update()
    min_y = min((ob.matrix_world @ v.co).y for ob in imported for v in ob.data.vertices)
    lift = FLOOR_Y - min_y
    lx, _, lz = loc
    for ob in imported:
        ob.location.x += lx
        ob.location.y += lift
        ob.location.z += lz
    bpy.context.view_layer.update()
    return imported


def layout_ids() -> list[str]:
    data = json.loads(MANIFEST.read_text(encoding="utf-8"))
    return [a["id"] for a in data["assets"]]


def main() -> None:
    os.environ["GARAGE_SCENE_PREVIEW"] = "1"
    os.environ["GARAGE_HERO_FLAT_MATERIAL"] = "1"
    bpy.ops.wm.read_factory_settings(use_empty=True)
    try:
        scene_meshes: list[bpy.types.Object] = []
        ids = layout_ids()
        rows = (len(ids) + COLS - 1) // COLS
        ox = -((COLS - 1) * SPACING) * 0.5
        oz = -((rows - 1) * SPACING) * 0.5
        for i, asset_id in enumerate(ids):
            col = i % COLS
            row = i // COLS
            x = ox + col * SPACING
            z = oz + row * SPACING
            scene_meshes.extend(import_prop_meshes(asset_id, (x, 0.0, z)))
        if not scene_meshes:
            raise RuntimeError("no scene props imported")

        def boost_ob_saturation(ob: bpy.types.Object) -> None:
            mat = ob.data.materials[0] if ob.data.materials else None
            if not mat or not mat.use_nodes:
                return
            nodes = mat.node_tree.nodes
            links = mat.node_tree.links
            bsdf = nodes.get("Principled BSDF")
            if not bsdf:
                return
            base = bsdf.inputs["Base Color"]
            if base.is_linked:
                src = base.links[0].from_socket
                links.remove(base.links[0])
            else:
                src = None
            hue = nodes.new("ShaderNodeHueSaturation")
            hue.inputs["Saturation"].default_value = 1.92
            hue.inputs["Value"].default_value = 1.14
            if src:
                links.new(src, hue.inputs["Color"])
            else:
                hue.inputs["Color"].default_value = base.default_value
            links.new(hue.outputs["Color"], base)

        for ob in scene_meshes:
            boost_ob_saturation(ob)

        import cc0_common as cc0  # type: ignore

        out = PREVIEW / "garage-scene-kit.png"
        mask = PREVIEW / "garage-scene-kit.mask.png"
        cc0.render_kit_preview(
            scene_meshes,
            out,
            resolution=768,
            mode="scene",
            fill=0.68,
            margin=0.09,
            frame_objects=scene_meshes,
            up_axis="Y",
            mask_path=mask,
            hero_yaw_deg=42.0,
            hero_pitch_deg=34.0,
        )
        meta = PREVIEW / "garage-scene-kit.frame.json"
        meta.write_text(
            json.dumps(
                {
                    "preview_kind": "garage",
                    "flat_backdrop": True,
                    "backdrop_rgb": [150, 150, 150],
                    "framing_fill": 0.68,
                    "framing_margin": 0.09,
                    "up_axis": "Y",
                    "floor_plane": "horizontal_y",
                    "props": len(ids),
                    "meshes": len(scene_meshes),
                },
                indent=2,
            )
            + "\n",
            encoding="utf-8",
        )
        print("OK garage-scene-kit", out, "meshes", len(scene_meshes))
    finally:
        os.environ.pop("GARAGE_SCENE_PREVIEW", None)
        os.environ.pop("GARAGE_HERO_FLAT_MATERIAL", None)


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()