#!/usr/bin/env python3
"""For-sale sign — twin posts + flat board facing preview camera."""
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
    draw.rectangle((0, 0, w, h), fill=(252, 228, 56))
    font = None
    for path in (
        "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
        "/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf",
    ):
        try:
            font = ImageFont.truetype(path, size=max(72, w // 9))
            break
        except OSError:
            continue
    if font is None:
        im.save(tex_path)
        Image.MAX_IMAGE_PIXELS = _prev_bomb
        return
    text = "FOR SALE"
    bbox = draw.textbbox((0, 0), text, font=font, stroke_width=max(2, w // 128))
    tw, th = bbox[2] - bbox[0], bbox[3] - bbox[1]
    draw.text(
        ((w - tw) // 2, (h - th) // 2),
        text,
        fill=(32, 18, 8),
        font=font,
        stroke_width=max(2, w // 160),
        stroke_fill=(20, 10, 6),
    )
    im.save(tex_path)
    Image.MAX_IMAGE_PIXELS = _prev_bomb


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.20"
    os.environ["PREVIEW_AZIMUTH_DEG"] = "40"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    post_w, post_h = 0.10, 1.65
    span = 1.10
    bc.add_cube("PostL", (post_w, post_h, post_w), (-span * 0.5, post_h * 0.5, 0))
    bc.add_cube("PostR", (post_w, post_h, post_w), (span * 0.5, post_h * 0.5, 0))
    bc.add_cube("Crossbar", (span + 0.12, 0.10, 0.12), (0, post_h, 0))
    bc.add_cube("Board", (1.26, 0.82, 0.08), (0, 1.42, 0.08))
    bc.add_cube("FrameL", (0.06, 0.82, 0.10), (-0.58, 1.42, 0.05))
    bc.add_cube("FrameR", (0.06, 0.82, 0.10), (0.58, 1.42, 0.05))
    bc.add_cube("CapL", (0.14, 0.08, 0.14), (-span * 0.5, post_h + 0.04, 0))
    bc.add_cube("CapR", (0.14, 0.08, 0.14), (span * 0.5, post_h + 0.04, 0))
    bc.add_cube("Brace", (1.22, 0.08, 0.14), (0, 1.02, 0.0))
    entry = bc.finish_asset("garage-land-for-sale-sign", "garage-kit")
    tex_dir = REPO / "unity/Assets/Art/EarlyGame/garage-kit/textures/garage-land-for-sale-sign"
    albedo = tex_dir / "garage-land-for-sale-sign_albedo.png"
    if albedo.is_file():
        _paint_for_sale_albedo(albedo)
        bc.reexport_packet34_glb("garage-land-for-sale-sign", "garage-kit")
        bc.rerender_garage_preview("garage-land-for-sale-sign", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris", entry["subShapeCount"], "parts")
    return entry


if __name__ == "__main__":
    bc.run_script(main)