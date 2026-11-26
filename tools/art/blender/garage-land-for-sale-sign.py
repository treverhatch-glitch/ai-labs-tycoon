#!/usr/bin/env python3
"""For-sale sign — posts behind board, FOR SALE mesh text (no albedo paint)."""
import os
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402

REPO = Path(__file__).resolve().parents[3]

BOARD_X = 0.90
BOARD_Y = 0.06
BOARD_Z = 0.60
POST_W = 0.10
POST_H = 1.65
POST_Z = -0.12
POST_SPAN = 1.05


def write_sign_albedo(tex_path: Path) -> None:
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

    board_y = POST_H - BOARD_Z * 0.5
    post_l = bc.add_cube(
        "PostL",
        (POST_W, POST_H, POST_W),
        (-POST_SPAN * 0.5, POST_H * 0.5, POST_Z),
    )
    post_r = bc.add_cube(
        "PostR",
        (POST_W, POST_H, POST_W),
        (POST_SPAN * 0.5, POST_H * 0.5, POST_Z),
    )
    bc.assign_solid(post_l, (0.52, 0.38, 0.26, 1.0), 0.58)
    bc.assign_solid(post_r, (0.52, 0.38, 0.26, 1.0), 0.58)

    back_rail = bc.add_cube(
        "SignBackRail",
        (POST_SPAN * 0.92, 0.10, 0.10),
        (0, board_y - 0.25, POST_Z + 0.04),
    )
    bc.assign_solid(back_rail, (0.50, 0.36, 0.24, 1.0), 0.58)

    board = bc.add_cube("Board", (BOARD_X, BOARD_Y, BOARD_Z), (0, board_y, 0))
    bc.assign_solid(board, (0.98, 0.96, 0.94, 1.0), 0.40)

    bc.add_sign_mesh_text("SignText", "FOR SALE", board, face_dir=-1.0, extrude_m=0.003)

    entry = bc.finish_asset("garage-land-for-sale-sign", "garage-kit")
    write_sign_albedo(albedo)
    bc.rerender_garage_preview("garage-land-for-sale-sign", "garage-kit")
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