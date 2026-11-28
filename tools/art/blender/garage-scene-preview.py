#!/usr/bin/env python3
"""Contact sheet: garage props assembled on studio grey (room-kit-style framing)."""
from __future__ import annotations

import json
import math
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
SPACING = 0.34
PROP_SCALE = 1.0
FLOOR_Y = 0.0


def import_prop(asset_id: str, loc: tuple[float, float, float]) -> bpy.types.Object | None:
    glb = KIT / f"{asset_id}.glb"
    if not glb.exists():
        print(f"WARN missing {glb}")
        return None
    before = set(bpy.context.scene.objects)
    bpy.ops.import_scene.gltf(filepath=str(glb))
    imported = [o for o in bpy.context.scene.objects if o not in before and o.type == "MESH"]
    if not imported:
        return None
    import blender_common as common  # type: ignore

    imported = [o for o in common.flatten_glb_preview_meshes(imported) if len(o.data.vertices) > 0]
    if not imported:
        return None
    joined = common.duplicate_join_meshes(imported, f"{asset_id}_scene_join")
    if joined is None or joined.type != "MESH" or len(joined.data.vertices) < 4:
        for ob in list(imported):
            bpy.data.objects.remove(ob, do_unlink=True)
        return None
    for ob in imported:
        try:
            bpy.data.objects.remove(ob, do_unlink=True)
        except ReferenceError:
            pass
    for ob in [
        o for o in bpy.context.scene.objects if o not in before and o is not joined and o.type == "MESH"
    ]:
        bpy.data.objects.remove(ob, do_unlink=True)
    tex_dir = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures" / asset_id
    common.apply_preview_material(joined, tex_dir, asset_id)
    joined.scale = (PROP_SCALE, PROP_SCALE, PROP_SCALE)
    joined.location = Vector(loc)
    bpy.context.view_layer.update()
    mins_y = min((joined.matrix_world @ v.co).y for v in joined.data.vertices)
    if abs(mins_y - FLOOR_Y) > 1e-4:
        joined.location.y += FLOOR_Y - mins_y
    bpy.context.view_layer.update()
    return joined


def layout_ids() -> list[str]:
    data = json.loads(MANIFEST.read_text(encoding="utf-8"))
    return [a["id"] for a in data["assets"]]


def main() -> None:
    os.environ["GARAGE_SCENE_PREVIEW"] = "1"
    os.environ["GARAGE_HERO_FLAT_MATERIAL"] = "1"
    bpy.ops.wm.read_factory_settings(use_empty=True)
    try:
        joins: list[bpy.types.Object] = []
        ids = layout_ids()
        rows = (len(ids) + COLS - 1) // COLS
        ox = -((COLS - 1) * SPACING) * 0.5
        oz = -((rows - 1) * SPACING) * 0.5
        for i, asset_id in enumerate(ids):
            col = i % COLS
            row = i // COLS
            x = ox + col * SPACING
            z = oz + row * SPACING
            ob = import_prop(asset_id, (x, 0.0, z))
            if ob:
                joins.append(ob)
        if not joins:
            raise RuntimeError("no scene props imported")

        keep = set(joins)
        for ob in list(bpy.context.scene.objects):
            if ob.type == "MESH" and ob not in keep:
                bpy.data.objects.remove(ob, do_unlink=True)
        bpy.context.view_layer.update()

        lo = Vector((1e9, 1e9, 1e9))
        hi = Vector((-1e9, -1e9, -1e9))
        for ob in joins:
            for corner in ob.bound_box:
                w = ob.matrix_world @ Vector(corner)
                lo.x, lo.y, lo.z = min(lo.x, w.x), min(lo.y, w.y), min(lo.z, w.z)
                hi.x, hi.y, hi.z = max(hi.x, w.x), max(hi.y, w.y), max(hi.z, w.z)
        ctr = (lo + hi) * 0.5
        for ob in joins:
            ob.location.x -= ctr.x
            ob.location.z -= ctr.z
        bpy.context.view_layer.update()
        min_y = min((ob.matrix_world @ v.co).y for ob in joins for v in ob.data.vertices)
        for ob in joins:
            ob.location.y += FLOOR_Y - min_y
        bpy.context.view_layer.update()
        cluster_r = 0.0
        for ob in joins:
            cluster_r = max(cluster_r, math.hypot(ob.location.x, ob.location.z))
        cluster_r = max(cluster_r, 0.45)
        pruned: list[bpy.types.Object] = []
        for ob in joins:
            xz = math.hypot(ob.location.x, ob.location.z)
            span_y = max(v.co.y for v in ob.data.vertices) - min(v.co.y for v in ob.data.vertices)
            if xz > cluster_r * 1.35 and span_y < 0.35 and len(ob.data.vertices) < 500:
                bpy.data.objects.remove(ob, do_unlink=True)
            else:
                pruned.append(ob)
        joins = pruned
        lo2 = Vector((1e9, 1e9, 1e9))
        hi2 = Vector((-1e9, -1e9, -1e9))
        for ob in joins:
            for corner in ob.bound_box:
                w = ob.matrix_world @ Vector(corner)
                lo2.x, lo2.y, lo2.z = min(lo2.x, w.x), min(lo2.y, w.y), min(lo2.z, w.z)
                hi2.x, hi2.y, hi2.z = max(hi2.x, w.x), max(hi2.y, w.y), max(hi2.z, w.z)
        frame_span = max(hi2.x - lo2.x, hi2.z - lo2.z, 0.65) * 0.92

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

        for ob in joins:
            boost_ob_saturation(ob)

        import cc0_common as cc0  # type: ignore

        out = PREVIEW / "garage-scene-kit.png"
        mask = PREVIEW / "garage-scene-kit.mask.png"
        cc0.render_kit_preview(
            joins,
            out,
            resolution=768,
            mode="scene",
            fill=0.80,
            margin=0.04,
            frame_objects=joins,
            frame_span=frame_span,
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
                    "framing_fill": 0.80,
                    "framing_margin": 0.04,
                    "frame_span": frame_span,
                    "up_axis": "Y",
                    "floor_plane": "horizontal_y",
                    "props": len(joins),
                },
                indent=2,
            )
            + "\n",
            encoding="utf-8",
        )
        print("OK garage-scene-kit", out, "props", len(joins))
    finally:
        os.environ.pop("GARAGE_SCENE_PREVIEW", None)
        os.environ.pop("GARAGE_HERO_FLAT_MATERIAL", None)


if __name__ == "__main__":
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    main()