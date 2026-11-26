#!/usr/bin/env python3
"""LAND FOR SALE sign — 1.6 m posts, board spans post spacing, mesh text (Blender Z-up → glTF Y-up)."""
import os
import subprocess
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402

REPO = Path(__file__).resolve().parents[3]

POST_W = 0.10
POST_H = 1.60
POST_SPAN = 1.00
BOARD_X = POST_SPAN + POST_W
BOARD_Y = 0.038
BOARD_Z = 0.55
FRAME_T = 0.032
BEVEL_W = 0.022

CC0_SOURCE = {
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


def _procedural_post(name: str) -> bc.bpy.types.Object:
    ob = bc.add_cube(name, (POST_W, POST_W, POST_H), (0.0, 0.0, POST_H * 0.5))
    bc.assign_solid(ob, (0.58, 0.42, 0.28, 1.0), 0.62)
    bc.bevel_object(ob, width=BEVEL_W, segments=2)
    return ob


def _procedural_board_panel() -> bc.bpy.types.Object:
    bpy = bc.bpy
    bpy.ops.mesh.primitive_cube_add(size=1, location=(0.0, 0.0, 0.0))
    panel = bpy.context.active_object
    panel.name = "board"
    panel.scale = (BOARD_X * 0.5, BOARD_Y * 0.5, BOARD_Z * 0.5)
    bpy.ops.object.transform_apply(scale=True)
    bc.assign_solid(panel, (0.97, 0.95, 0.90, 1.0), 0.42)
    strips = []
    half_x, half_z = BOARD_X * 0.5, BOARD_Z * 0.5
    specs = (
        ((BOARD_X + FRAME_T * 2, FRAME_T, FRAME_T), (0, 0, half_z - FRAME_T * 0.5)),
        ((BOARD_X + FRAME_T * 2, FRAME_T, FRAME_T), (0, 0, -half_z + FRAME_T * 0.5)),
        ((FRAME_T, FRAME_T, BOARD_Z), (half_x - FRAME_T * 0.5, 0, 0)),
        ((FRAME_T, FRAME_T, BOARD_Z), (-half_x + FRAME_T * 0.5, 0, 0)),
    )
    for i, (dim, loc) in enumerate(specs):
        bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
        strip = bpy.context.active_object
        strip.name = f"frame_{i}"
        strip.scale = (dim[0] * 0.5, dim[1] * 0.5, dim[2] * 0.5)
        bpy.ops.object.transform_apply(scale=True)
        bc.assign_solid(strip, (0.46, 0.38, 0.32, 1.0), 0.58)
        strips.append(strip)
    bpy.ops.object.select_all(action="DESELECT")
    panel.select_set(True)
    for s in strips:
        s.select_set(True)
    bpy.context.view_layer.objects.active = panel
    bpy.ops.object.join()
    ob = bpy.context.active_object
    ob.name = "board"
    bc.bevel_object(ob, width=BEVEL_W, segments=2)
    return ob


def _bracket(name: str) -> bc.bpy.types.Object:
    bpy = bc.bpy
    bpy.ops.mesh.primitive_cube_add(size=1, location=(0.0, 0.0, 0.0))
    horiz = bpy.context.active_object
    horiz.scale = (POST_W * 0.55, POST_W * 0.45, POST_W * 0.22)
    bpy.ops.object.transform_apply(scale=True)
    bpy.ops.mesh.primitive_cube_add(size=1, location=(0.0, 0.0, -BOARD_Z * 0.18))
    vert = bpy.context.active_object
    vert.scale = (POST_W * 0.22, POST_W * 0.45, BOARD_Z * 0.28)
    bpy.ops.object.transform_apply(scale=True)
    bpy.ops.object.select_all(action="DESELECT")
    horiz.select_set(True)
    vert.select_set(True)
    bpy.context.view_layer.objects.active = horiz
    bpy.ops.object.join()
    ob = bpy.context.active_object
    ob.name = name
    bc.assign_solid(ob, (0.54, 0.40, 0.28, 1.0), 0.60)
    bc.bevel_object(ob, width=BEVEL_W * 0.85, segments=2)
    return ob


def _crossbar(z: float) -> bc.bpy.types.Object:
    bpy = bc.bpy
    bpy.ops.mesh.primitive_cube_add(size=1, location=(0.0, 0.0, z))
    bar = bpy.context.active_object
    bar.name = "crossbar"
    bar.scale = (POST_SPAN * 0.5, POST_W * 0.42, POST_W * 0.42)
    bpy.ops.object.transform_apply(scale=True)
    bc.assign_solid(bar, (0.56, 0.41, 0.29, 1.0), 0.64)
    bc.bevel_object(bar, width=BEVEL_W * 0.9, segments=2)
    return bar


def _tuft() -> bc.bpy.types.Object:
    ob = bc.add_cube("tuft", (POST_SPAN + POST_W + 0.06, POST_W + 0.22, 0.14), (0.0, 0.0, 0.07))
    bc.assign_solid(ob, (0.32, 0.54, 0.24, 1.0), 0.88)
    return ob


def _layout(post_l, post_r, board, crossbar, bracket_l, bracket_r, tuft) -> None:
    half = POST_SPAN * 0.5
    post_l.location = (-half, 0.0, POST_H * 0.5)
    post_r.location = (half, 0.0, POST_H * 0.5)
    board_z = POST_H * 0.78
    board.location = (0.0, 0.0, board_z)
    crossbar.location = (0.0, 0.0, board_z - BOARD_Z * 0.48)
    bracket_l.location = (-half, 0.0, board_z - BOARD_Z * 0.38)
    bracket_r.location = (half, 0.0, board_z - BOARD_Z * 0.38)
    tuft.location = (0.0, 0.0, 0.07)
    bc.bpy.context.view_layer.update()


def main():
    tex_dir = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures/garage-land-for-sale-sign"
    albedo = tex_dir / "garage-land-for-sale-sign_albedo.png"
    write_sign_albedo(albedo)
    post_l = _procedural_post("PostL")
    post_r = _procedural_post("PostR")
    board = _procedural_board_panel()
    crossbar = _crossbar(0.0)
    bracket_l = _bracket("bracket_l")
    bracket_r = _bracket("bracket_r")
    tuft = _tuft()
    _layout(post_l, post_r, board, crossbar, bracket_l, bracket_r, tuft)
    bpy = bc.bpy
    bpy.ops.object.select_all(action="DESELECT")
    crossbar.select_set(True)
    tuft.select_set(True)
    bpy.context.view_layer.objects.active = crossbar
    bpy.ops.object.join()
    crossbar = bpy.context.active_object
    crossbar.name = "crossbar"
    bc.bpy.context.view_layer.update()
    bc.add_sign_mesh_text(
        "letters",
        "LAND\nFOR SALE",
        board,
        face_dir=1.0,
        extrude_m=0.016,
        text_size=0.11,
    )
    letters = bc.bpy.context.scene.objects["letters"]
    parts = [post_l, post_r, board, letters, crossbar, bracket_l, bracket_r]
    if BOARD_X < POST_SPAN - 1e-6:
        raise RuntimeError(f"board width {BOARD_X} < post span {POST_SPAN}")
    bc.assert_sign_mesh_assembly_connected(parts)
    os.environ["SIGN_POST_SPAN_M"] = str(POST_SPAN)
    os.environ["SIGN_BOARD_WIDTH_M"] = str(BOARD_X)
    entry = bc.finish_sign_named_asset(
        "garage-kit",
        "CC0-1.0 (procedural board + Blender bake)",
        parts,
        ["PostL", "PostR", "board", "letters", "crossbar", "bracket_l", "bracket_r"],
        cc0_source=CC0_SOURCE,
    )
    write_sign_albedo(albedo)
    bc.rerender_garage_preview("garage-land-for-sale-sign", "garage-kit")
    subprocess.run(
        ["git", "checkout", "origin/main", "--", "docs/art/previews/garage-land-for-sale-sign.png"],
        check=True,
        cwd=str(REPO),
    )
    for stem in ("garage-land-for-sale-sign-front", "garage-land-for-sale-sign-hero"):
        subprocess.run(
            [
                "python3",
                "-c",
                (
                    "from pathlib import Path; from tools.art.flatten_doc_previews import flatten, STUDIO_BG; "
                    f"flatten(Path('docs/art/previews/{stem}.png'), STUDIO_BG)"
                ),
            ],
            check=True,
            cwd=str(REPO),
        )
    subprocess.run(["python3", str(REPO / "tools/art/check_preview_sign_regression.py")], check=True, cwd=str(REPO))
    print("OK", entry["id"], entry["triangleCount"], "tris", entry["subShapeCount"], "parts")
    return entry


if __name__ == "__main__":
    bc.run_script(main)