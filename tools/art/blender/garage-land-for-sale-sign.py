#!/usr/bin/env python3
"""For-sale sign — post, board, FOR SALE label."""
import sys
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def _label_mesh(name: str, text: str, location: tuple[float, float, float]) -> bpy.types.Object:
    bpy.ops.object.text_add(location=location)
    ob = bpy.context.active_object
    ob.name = name
    ob.data.body = text
    ob.data.size = 0.22
    ob.data.extrude = 0.04
    ob.data.align_x = "CENTER"
    ob.data.align_y = "CENTER"
    ob.rotation_euler = (1.5708, 0, 0)
    bpy.ops.object.convert(target="MESH")
    ob = bpy.context.active_object
    bc.assign_solid(ob, (0.12, 0.08, 0.06, 1.0), 0.55)
    return ob


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.24"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    bc.add_cube("Post", (0.12, 1.75, 0.12), (0, 0.875, 0))
    board = bc.add_cube("Board", (1.1, 0.85, 0.10), (0, 1.45, 0.04))
    bc.assign_solid(board, (0.92, 0.82, 0.12, 1.0), 0.48)
    bc.add_cube("Neck", (0.14, 0.22, 0.14), (0, 1.28, 0))
    _label_mesh("Label", "FOR SALE", (0, 1.45, 0.08))
    bc.add_cube("Stud", (0.12, 0.12, 0.12), (0, 1.35, 0.02))
    entry = bc.finish_asset("garage-land-for-sale-sign", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)