#!/usr/bin/env python3
"""For-sale sign — twin posts + flat board facing preview camera."""
import math
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import bpy

REPO = Path(__file__).resolve().parents[3]


def write_sign_albedo(tex_path: Path) -> None:
    """Bake-friendly board albedo via system Python (Blender PIL text is unreliable)."""
    import subprocess

    painter = REPO / "tools/art/paint_sign_albedo.py"
    subprocess.run(["python3", str(painter), str(tex_path)], check=True, cwd=str(REPO))


def _face_board_to_preview_camera() -> None:
    """Rotate billboard so wide face points at default packet34 preview camera."""
    az = math.radians(float(__import__("os").environ.get("PREVIEW_AZIMUTH_DEG", "40")))
    el = math.radians(30.0)
    # Camera offset (Z-up import); board authored Y-up with face normal +Y.
    view = math.atan2(math.cos(el) * math.sin(az), math.cos(el) * math.cos(az))
    meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
    bpy.ops.object.select_all(action="DESELECT")
    for ob in meshes:
        ob.select_set(True)
    if not meshes:
        return
    bpy.context.view_layer.objects.active = meshes[0]
    bpy.ops.transform.rotate(value=view, orient_axis="Y")
    bpy.ops.object.select_all(action="DESELECT")


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.20"
    os.environ["PREVIEW_AZIMUTH_DEG"] = "40"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    tex_dir = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures/garage-land-for-sale-sign"
    albedo = tex_dir / "garage-land-for-sale-sign_albedo.png"
    write_sign_albedo(albedo)
    post_w, post_h = 0.10, 1.65
    span = 1.10
    bc.add_cube("PostL", (post_w, post_h, post_w), (-span * 0.5, post_h * 0.5, 0))
    bc.add_cube("PostR", (post_w, post_h, post_w), (span * 0.5, post_h * 0.5, 0))
    bc.add_cube("Crossbar", (span + 0.12, 0.10, 0.12), (0, post_h, 0))
    # Thin Y → large XZ faces (billboard normal ±Y).
    board = bc.add_cube("Board", (1.26, 0.08, 0.82), (0, 1.42, 0))
    bc.assign_image_texture(board, albedo, uv_scale=1.0, roughness=0.4)
    bc.add_cube("FrameL", (0.06, 0.82, 0.10), (-0.58, 1.42, 0))
    bc.add_cube("FrameR", (0.06, 0.82, 0.10), (0.58, 1.42, 0))
    bc.add_cube("CapL", (0.14, 0.08, 0.14), (-span * 0.5, post_h + 0.04, 0))
    bc.add_cube("CapR", (0.14, 0.08, 0.14), (span * 0.5, post_h + 0.04, 0))
    bc.add_cube("Brace", (1.22, 0.08, 0.14), (0, 1.02, 0))
    entry = bc.finish_asset("garage-land-for-sale-sign", "garage-kit")
    write_sign_albedo(albedo)
    bc.reexport_packet34_glb("garage-land-for-sale-sign", "garage-kit")
    bc.rerender_garage_preview("garage-land-for-sale-sign", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris", entry["subShapeCount"], "parts")
    return entry


if __name__ == "__main__":
    bc.run_script(main)