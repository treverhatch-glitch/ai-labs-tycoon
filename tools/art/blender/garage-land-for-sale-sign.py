#!/usr/bin/env python3
"""FOR SALE hero sign — Kenney board, procedural posts, mesh text (Blender Z-up → glTF Y-up)."""
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
POST_SPAN = 0.90

CC0_SOURCE = {
    "kenneyCityKitSuburban": {
        "name": "City Kit (Suburban) 2.0",
        "author": "Kenney (www.kenney.nl)",
        "license": "CC0 1.0",
        "licenseUrl": "https://creativecommons.org/publicdomain/zero/1.0/",
        "downloadUrl": "https://opengameart.org/sites/default/files/kenney_city-kit-suburban_2.0.zip",
        "downloadedAt": "2025-11-26",
        "piecesUsed": ["fence-1x2.glb", "driveway-short.glb", "building-type-b.glb"],
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
    subprocess.run(
        ["python3", str(REPO / "tools/art/paint_sign_albedo.py"), str(tex_path)],
        check=True,
        cwd=str(REPO),
    )


def _scale_to_dimensions(ob, target: tuple[float, float, float]) -> None:
    bpy = bc.bpy
    dims = ob.dimensions
    ob.scale = (target[0] / max(dims.x, 1e-6), target[1] / max(dims.y, 1e-6), target[2] / max(dims.z, 1e-6))
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.transform_apply(scale=True)


def _procedural_post(name: str, x: float) -> bc.bpy.types.Object:
    ob = bc.add_cube(name, (POST_W, POST_W, POST_H), (x, 0.0, POST_H * 0.5))
    bc.assign_solid(ob, (0.52, 0.38, 0.26, 1.0), 0.58)
    return ob


def _kenney_board() -> bc.bpy.types.Object:
    obj = bc.import_cc0_kenney_glb("driveway-short.glb")
    obj.name = "board"
    _scale_to_dimensions(obj, (BOARD_X, BOARD_Y, BOARD_Z))
    inner = BOARD_X - POST_W * 1.1
    if obj.dimensions.x > inner:
        obj.scale.x *= inner / max(obj.dimensions.x, 1e-6)
        bc.bpy.ops.object.transform_apply(scale=True)
    bc.assign_solid(obj, (0.98, 0.96, 0.94, 1.0), 0.40)
    return obj


def _layout(post_l, post_r, board) -> None:
    half = POST_SPAN * 0.5
    post_l.location = (-half, 0.0, POST_H * 0.5)
    post_r.location = (half, 0.0, POST_H * 0.5)
    board.location = (0.0, 0.0, POST_H - BOARD_Z * 0.5)
    bc.bpy.context.view_layer.update()


def main():
    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.20"
    os.environ["PREVIEW_AZIMUTH_DEG"] = "40"
    os.environ["PREVIEW_ELEVATION_DEG"] = "8"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "0"
    tex_dir = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures/garage-land-for-sale-sign"
    albedo = tex_dir / "garage-land-for-sale-sign_albedo.png"
    write_sign_albedo(albedo)
    post_l = _procedural_post("PostL", 0.0)
    post_r = _procedural_post("PostR", 0.0)
    board = _kenney_board()
    _layout(post_l, post_r, board)
    bc.add_sign_mesh_text("letters", "FOR SALE", board, face_dir=1.0, extrude_m=0.014)
    letters = bc.bpy.context.scene.objects["letters"]
    entry = bc.finish_sign_named_asset(
        "garage-kit",
        "CC0-1.0 (Kenney suburban kit + Blender bake)",
        [post_l, post_r, board, letters],
        ["PostL", "PostR", "board", "letters"],
        cc0_source=CC0_SOURCE,
    )
    write_sign_albedo(albedo)
    bc.rerender_garage_preview("garage-land-for-sale-sign", "garage-kit")
    bc.rerender_garage_preview("garage-roll-door", "garage-kit")
    subprocess.run(["python3", str(REPO / "tools/art/assert_sign_front_ltr.py")], check=True, cwd=str(REPO))
    bc.render_sign_garage_hero("garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris", entry["subShapeCount"], "parts")
    return entry


if __name__ == "__main__":
    bc.run_script(main)