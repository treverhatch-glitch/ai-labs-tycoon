#!/usr/bin/env python3
"""For-sale sign — two posts + one flat board facing preview camera."""
import math
import os
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


def main():
    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.20"
    os.environ["PREVIEW_AZIMUTH_DEG"] = "40"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    tex_dir = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures/garage-land-for-sale-sign"
    albedo = tex_dir / "garage-land-for-sale-sign_albedo.png"
    write_sign_albedo(albedo)
    post_w, post_h = 0.10, 1.65
    span = 1.05
    post_l = bc.add_cube("PostL", (post_w, post_h, post_w), (-span * 0.5, post_h * 0.5, 0))
    post_r = bc.add_cube("PostR", (post_w, post_h, post_w), (span * 0.5, post_h * 0.5, 0))
    bc.assign_solid(post_l, (0.52, 0.38, 0.26, 1.0), 0.58)
    bc.assign_solid(post_r, (0.52, 0.38, 0.26, 1.0), 0.58)
    board = bc.add_cube("Board", (1.2, 0.8, 0.08), (0, 1.42, 0))
    bc.assign_image_texture(board, albedo, uv_scale=1.0, roughness=0.42)
    entry = bc.finish_asset("garage-land-for-sale-sign", "garage-kit")
    write_sign_albedo(albedo)
    bc.reexport_packet34_glb("garage-land-for-sale-sign", "garage-kit")
    bc.rerender_garage_preview("garage-land-for-sale-sign", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris", entry["subShapeCount"], "parts")
    return entry


if __name__ == "__main__":
    bc.run_script(main)