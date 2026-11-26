#!/usr/bin/env python3
"""FOR SALE hero sign — restyled posts/board (CC0 lane; see docs/art/LICENSES.md)."""
import math
import os
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402

REPO = Path(__file__).resolve().parents[3]

BOARD_X = 0.90
BOARD_Y = 0.02
BOARD_Z = 0.60
POST_W = 0.10
POST_H = 1.20
POST_DEPTH = 0.10


def write_sign_albedo(tex_path: Path) -> None:
    import subprocess

    painter = REPO / "tools/art/paint_sign_albedo.py"
    subprocess.run(["python3", str(painter), str(tex_path)], check=True, cwd=str(REPO))


def main():
    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.20"
    os.environ["PREVIEW_AZIMUTH_DEG"] = "32"
    os.environ["PREVIEW_ELEVATION_DEG"] = "6"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    tex_dir = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures/garage-land-for-sale-sign"
    albedo = tex_dir / "garage-land-for-sale-sign_albedo.png"
    write_sign_albedo(albedo)

    board_y = POST_H - BOARD_Z * 0.5
    post_x = BOARD_X * 0.5 - POST_W * 0.5
    post_z = -POST_DEPTH * 0.55
    cyl_rot = (math.pi / 2, 0.0, 0.0)
    post_l = bc.add_cylinder(
        "PostL",
        POST_W * 0.48,
        POST_H,
        (-post_x, POST_H * 0.5, post_z),
        vertices=14,
        rotation_euler=cyl_rot,
    )
    post_r = bc.add_cylinder(
        "PostR",
        POST_W * 0.48,
        POST_H,
        (post_x, POST_H * 0.5, post_z),
        vertices=14,
        rotation_euler=cyl_rot,
    )
    bc.assign_solid(post_l, (0.52, 0.38, 0.26, 1.0), 0.58)
    bc.assign_solid(post_r, (0.52, 0.38, 0.26, 1.0), 0.58)

    # Back rail + gussets tie posts to board (connected sign silhouette in preview).
    back_z = post_z - POST_W * 0.08
    rail = bc.add_cube(
        "SignBackRail",
        (BOARD_X - POST_W * 0.35, 0.07, 0.07),
        (0, board_y - BOARD_Z * 0.42, back_z),
    )
    bc.assign_solid(rail, (0.48, 0.34, 0.22, 1.0), 0.58)
    for side, x_off in (("SignGussetL", -post_x), ("SignGussetR", post_x)):
        gus = bc.add_cube(
            side,
            (0.07, 0.18, 0.07),
            (x_off, board_y - BOARD_Z * 0.55, back_z + 0.02),
        )
        bc.assign_solid(gus, (0.48, 0.34, 0.22, 1.0), 0.58)

    board = bc.add_cube("board", (BOARD_X, BOARD_Y, BOARD_Z), (0, board_y, 0))
    bc.assign_solid(board, (0.98, 0.96, 0.94, 1.0), 0.40)
    bc.bevel_object(board, min(0.012, BOARD_Y * 0.45), 3)
    bc.add_sign_mesh_text("letters", "FOR SALE", board, face_dir=1.0, extrude_m=0.006)

    entry = bc.finish_asset("garage-land-for-sale-sign", "garage-kit")
    write_sign_albedo(albedo)
    bc.rerender_garage_preview("garage-land-for-sale-sign", "garage-kit")
    bc.render_sign_garage_hero("garage-kit")
    print(
        "OK",
        entry["id"],
        entry["triangleCount"],
        "tris",
        entry["subShapeCount"],
        "parts",
    )
    return entry


if __name__ == "__main__":
    bc.run_script(main)