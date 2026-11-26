#!/usr/bin/env python3
"""FOR SALE hero sign — Kenney CC0 posts/board + mesh text; garage facade hero."""
import math
import os
import subprocess
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

CC0_SOURCE = {
    "kenneyCityKitSuburban": {
        "name": "City Kit (Suburban) 2.0",
        "author": "Kenney (www.kenney.nl)",
        "license": "CC0 1.0",
        "licenseUrl": "https://creativecommons.org/publicdomain/zero/1.0/",
        "downloadUrl": "https://opengameart.org/sites/default/files/kenney_city-kit-suburban_2.0.zip",
        "downloadedAt": "2025-11-26",
        "piecesUsed": [
            "fence-1x2.glb",
            "driveway-short.glb",
            "building-type-b.glb",
        ],
        "sha256Zip": "5869c35cf30b1c87bdb2d197b6d325eebadd2ef08ea27f04797e8e08d77a9a39",
    },
    "polyHavenHdri": {
        "name": "studio_small_09",
        "license": "CC0 1.0",
        "licenseUrl": "https://polyhaven.com/license",
        "downloadUrl": "https://dl.polyhaven.org/file/ph-assets/HDRIs/hdr/2k/studio_small_09_2k.hdr",
        "downloadedAt": "2025-11-26",
        "sha256": "36724313c0fc66dab126ff90f081b85a0b1ef47be65eda1417bd20b033f0573f",
        "fetchScript": "tools/art/cc0/fetch_cc0_assets.sh",
    },
}


def write_sign_albedo(tex_path: Path) -> None:
    painter = REPO / "tools/art/paint_sign_albedo.py"
    subprocess.run(["python3", str(painter), str(tex_path)], check=True, cwd=str(REPO))


def _scale_to_dimensions(ob, target: tuple[float, float, float]) -> None:
    bpy = bc.bpy
    bpy.context.view_layer.update()
    dims = ob.dimensions
    ob.scale = (
        target[0] / max(dims.x, 1e-6),
        target[1] / max(dims.y, 1e-6),
        target[2] / max(dims.z, 1e-6),
    )
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(scale=True)


def _kenney_post(name: str, x: float) -> bc.bpy.types.Object:
    """Kenney fence-1x2 scaled to a vertical Y-up post (feet on Y=0)."""
    obj = bc.import_cc0_kenney_glb("fence-1x2.glb")
    obj.name = name
    _scale_to_dimensions(obj, (POST_W, POST_H, POST_W))
    obj.location = (x, POST_H * 0.5, 0.0)
    obj.rotation_euler = (0.0, 0.0, 0.0)
    bc.assign_solid(obj, (0.52, 0.38, 0.26, 1.0), 0.58)
    return obj


def _kenney_board() -> bc.bpy.types.Object:
    """Y-up billboard: wide X, tall Y, thin Z (public face ±Z)."""
    obj = bc.import_cc0_kenney_glb("driveway-short.glb")
    obj.name = "board"
    _scale_to_dimensions(obj, (BOARD_X, BOARD_Z, BOARD_Y))
    inner_span = BOARD_X - POST_W * 1.1
    if obj.dimensions.x > inner_span:
        obj.scale.x *= inner_span / max(obj.dimensions.x, 1e-6)
        bc.bpy.ops.object.transform_apply(scale=True)
    bc.assign_solid(obj, (0.98, 0.96, 0.94, 1.0), 0.40)
    return obj


def _rigid_sign_layout(post_l, post_r, board) -> None:
    """Board spans between posts; posts vertical on ground (no tilt)."""
    post_x = BOARD_X * 0.5 - POST_W * 0.45
    post_l.location = (-post_x, POST_H * 0.5, 0.0)
    post_r.location = (post_x, POST_H * 0.5, 0.0)
    post_l.rotation_euler = (0.0, 0.0, 0.0)
    post_r.rotation_euler = (0.0, 0.0, 0.0)
    board.location = (0.0, POST_H - BOARD_Z * 0.5, 0.0)
    board.rotation_euler = (0.0, 0.0, 0.0)
    bc.bpy.context.view_layer.update()


def main():
    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.18"
    os.environ["PREVIEW_AZIMUTH_DEG"] = "34"
    os.environ["PREVIEW_ELEVATION_DEG"] = "6"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    tex_dir = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures/garage-land-for-sale-sign"
    albedo = tex_dir / "garage-land-for-sale-sign_albedo.png"
    write_sign_albedo(albedo)

    post_l = _kenney_post("PostL", 0.0)
    post_r = _kenney_post("PostR", 0.0)
    board = _kenney_board()
    _rigid_sign_layout(post_l, post_r, board)
    bc.add_sign_mesh_text("letters", "FOR SALE", board, face_dir=1.0, extrude_m=0.010)
    letters = bc.bpy.context.scene.objects["letters"]

    mesh_obs = [post_l, post_r, board, letters]
    part_names = [o.name for o in mesh_obs]
    license_note = "CC0-1.0 (Kenney suburban kit + Blender bake)"
    entry = bc.finish_sign_named_asset(
        "garage-kit",
        license_note,
        mesh_obs,
        part_names,
        cc0_source=CC0_SOURCE,
    )
    write_sign_albedo(albedo)
    bc.rerender_garage_preview("garage-land-for-sale-sign", "garage-kit")
    bc.rerender_garage_preview("garage-roll-door", "garage-kit")
    subprocess.run(
        ["python3", str(REPO / "tools/art/assert_sign_front_ltr.py")],
        check=True,
        cwd=str(REPO),
    )
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