#!/usr/bin/env python3
"""Concrete curb ~1m with sloped street face and visible aggregate texture."""
import math
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402

L, H, D = 1.0, 0.15, 0.30


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.24"
    os.environ["PREVIEW_AZIMUTH_DEG"] = "48"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "0"
    deck = bc.add_cube("CurbDeck", (L, H * 0.72, D * 0.82), (0, H * 0.36, D * 0.06))
    slope = bc.add_cube("CurbSlope", (L, H * 0.32, D * 0.38), (0, H * 0.11, -D * 0.28))
    slope.rotation_euler = (math.radians(-22.0), 0.0, 0.0)
    import bpy

    bpy.ops.object.transform_apply(rotation=True, scale=False)
    sw = Path(__file__).resolve().parents[3] / "unity/Assets/Art/EarlyGame/textures/tex-garage-concrete.png"
    for ob in (deck, slope):
        if sw.is_file():
            bc.assign_image_texture(ob, sw, uv_scale=(2.5, 0.45, 1.6), coord="UV", roughness=0.90)
        else:
            bc.assign_solid(ob, (0.54, 0.52, 0.50, 1.0), 0.86)
        bc.bevel_object(ob, 0.008, 2)
    import bpy

    bpy.ops.object.select_all(action="DESELECT")
    for ob in (deck, slope):
        ob.select_set(True)
    bpy.context.view_layer.objects.active = deck
    bpy.ops.object.join()
    bpy.context.active_object.name = "CurbDeck"
    entry = bc.finish_asset("garage-lot-curb", "garage-kit")
    bc.rerender_garage_preview("garage-lot-curb", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris", entry["subShapeCount"], "parts")
    return entry


if __name__ == "__main__":
    bc.run_script(main)