#!/usr/bin/env python3
"""Flat 2D HUD previews from SVG glyphs on #969696 (pass8)."""
from __future__ import annotations

import json
import io
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
SVG_DIR = REPO / "unity/Assets/Art/EarlyGame/hud-icons"
PREVIEW = REPO / "docs/art/previews"
HUD_OUT = REPO / "unity/Assets/Art/EarlyGame/hud-icons"
STREAM = REPO / "unity/Assets/StreamingAssets/EarlyGame/hud-icons"
STUDIO_RGB = (150, 150, 150)
SIZE = 512
FILL = 0.76

HUD_ASSETS = [
    "hud-topbar-beige",
    "icon-cash-era0",
    "icon-date-era0",
    "icon-era-bar",
    "icon-build-panel",
    "icon-hire-panel",
    "icon-train-panel",
    "icon-gpu-hire",
    "icon-week-advance",
    "panel-build-bg",
    "panel-hire-bg",
    "panel-train-bg",
]


def rasterize_svg(svg_path: Path, target_px: int) -> "Image.Image":
    import cairosvg
    from PIL import Image

    png_bytes = cairosvg.svg2png(
        url=str(svg_path),
        output_width=target_px,
        output_height=target_px,
    )
    return Image.open(io.BytesIO(png_bytes)).convert("RGBA")


def compose_one(asset_id: str) -> None:
    from PIL import Image

    svg = SVG_DIR / f"{asset_id}.svg"
    if not svg.is_file():
        raise FileNotFoundError(svg)
    if asset_id == "hud-topbar-beige":
        fg = rasterize_svg(svg, SIZE)
        bar_h = max(72, int(SIZE * 72 / 256))
        fg = fg.resize((SIZE, bar_h), Image.Resampling.LANCZOS)
    else:
        target = int(SIZE * FILL)
        fg = rasterize_svg(svg, target)
    ox = (SIZE - fg.width) // 2
    oy = (SIZE - fg.height) // 2
    rgb = Image.new("RGB", (SIZE, SIZE), STUDIO_RGB)
    rgb.paste(fg.convert("RGB"), (ox, oy), fg.split()[3])
    preview = PREVIEW / f"{asset_id}.png"
    mask = PREVIEW / f"{asset_id}.mask.png"
    preview.parent.mkdir(parents=True, exist_ok=True)
    rgb.save(preview)

    mask_im = Image.new("RGBA", (SIZE, SIZE), (0, 0, 0, 0))
    mask_im.paste(fg, (ox, oy), fg)
    mask_im.save(mask)

    meta = preview.with_name(preview.stem + ".frame.json")
    meta.write_text(
        json.dumps(
            {
                "preview_kind": "hud",
                "camera_type": "orthographic",
                "flat_backdrop": True,
                "backdrop_rgb": list(STUDIO_RGB),
                "framing_fill": FILL,
                "source": "svg_compose",
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )
    HUD_OUT.mkdir(parents=True, exist_ok=True)
    STREAM.mkdir(parents=True, exist_ok=True)
    icon_path = HUD_OUT / f"{asset_id}.png"
    icon_path.write_bytes(preview.read_bytes())
    (STREAM / icon_path.name).write_bytes(preview.read_bytes())
    print(f"OK compose {asset_id}")


def main() -> int:
    try:
        import cairosvg  # noqa: F401
        from PIL import Image  # noqa: F401
    except ImportError as exc:
        print(f"FAIL compose_hud needs cairosvg+Pillow: {exc}")
        return 1
    rc = 0
    for aid in HUD_ASSETS:
        try:
            compose_one(aid)
        except Exception as exc:  # noqa: BLE001
            print(f"FAIL {aid} {exc}")
            rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())