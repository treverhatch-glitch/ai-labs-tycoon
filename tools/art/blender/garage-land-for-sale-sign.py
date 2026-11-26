#!/usr/bin/env python3
"""For-sale sign — twin wood posts + board with baked FOR SALE decal."""
import math
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import bpy

REPO = Path(__file__).resolve().parents[3]


def _paint_for_sale_albedo(tex_path: Path) -> None:
    from PIL import Image, ImageDraw, ImageFont

    _prev_bomb = Image.MAX_IMAGE_PIXELS
    Image.MAX_IMAGE_PIXELS = max(_prev_bomb or 0, 512 * 1024 * 1024)
    im = Image.open(tex_path).convert("RGB")
    draw = ImageDraw.Draw(im)
    w, h = im.size
    margin = w // 10
    draw.rectangle((margin, h // 7, w - margin, h - h // 7), fill=(252, 228, 56))
    header_h = max(130, h // 5)
    draw.rectangle((margin, h // 7, w - margin, h // 7 + header_h), fill=(214, 72, 28))
    font = None
    candidates = (
        "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
        "/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf",
        "DejaVuSans-Bold.ttf",
        "LiberationSans-Bold.ttf",
    )
    for path in candidates:
        try:
            font = ImageFont.truetype(path, size=96)
            break
        except OSError:
            continue
    if font is None:
        font = ImageFont.load_default()
        draw.text((w // 4, h // 3), "FOR SALE", fill=(32, 16, 8))
        im.save(tex_path)
        Image.MAX_IMAGE_PIXELS = _prev_bomb
        return
    text = "FOR SALE"
    bbox = draw.textbbox((0, 0), text, font=font, stroke_width=3)
    tw, th = bbox[2] - bbox[0], bbox[3] - bbox[1]
    draw.text(
        ((w - tw) // 2, (h - th) // 2 - 12),
        text,
        fill=(198, 42, 28),
        font=font,
        stroke_width=2,
        stroke_fill=(32, 16, 8),
    )
    im.save(tex_path)
    Image.MAX_IMAGE_PIXELS = _prev_bomb


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.12"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    post_w, post_h = 0.11, 1.75
    span = 1.05
    for side, x in (("PostL", -span * 0.5), ("PostR", span * 0.5)):
        post = bc.add_cube(side, (post_w, post_h, post_w), (x, post_h * 0.5, 0))
        bc.assign_solid(post, (0.42, 0.30, 0.16, 1.0), 0.62)
    board = bc.add_cube("Board", (1.28, 0.82, 0.10), (0, 1.48, 0.05))
    bc.assign_solid(board, (0.98, 0.92, 0.22, 1.0), 0.38)
    bc.add_cube("Header", (1.32, 0.14, 0.12), (0, 1.86, 0.06))
    bc.add_cube("Cap", (1.32, 0.08, 0.12), (0, 1.94, 0.05))
    bc.add_cube("Crossbar", (1.34, 0.14, 0.18), (0, 1.05, 0.0))
    bc.add_cube("NailL", (0.12, 0.12, 0.16), (-span * 0.5, 1.05, 0.0))
    bc.add_cube("NailR", (0.12, 0.12, 0.16), (span * 0.5, 1.05, 0.0))
    bc.add_cube("Stud", (0.95, 0.72, 0.14), (0, 1.48, 0.0))
    yaw = math.radians(42.0)
    for ob in bpy.context.scene.objects:
        if ob.type != "MESH":
            continue
        if any(tag in ob.name for tag in ("Board", "Header", "Cap", "Stud")):
            ob.rotation_euler[1] = yaw
            ob.select_set(True)
        else:
            ob.select_set(False)
    if bpy.context.selected_objects:
        bpy.context.view_layer.objects.active = bpy.context.selected_objects[0]
        bpy.ops.object.transform_apply(rotation=True)
    bpy.ops.object.select_all(action="DESELECT")
    entry = bc.finish_asset("garage-land-for-sale-sign", "garage-kit")
    tex_dir = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures/garage-land-for-sale-sign"
    albedo = tex_dir / "garage-land-for-sale-sign_albedo.png"
    if albedo.is_file():
        _paint_for_sale_albedo(albedo)
        bc.reexport_packet34_glb("garage-land-for-sale-sign", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)