#!/usr/bin/env python3
"""Flatten RGBA doc previews onto lane backdrops for human viewing / Unity RGB."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs" / "art" / "previews"

STUDIO_BG = (150, 150, 150)
HUD_BG = (128, 128, 133)


def pin_studio_corner_backdrop(path: Path, bg: tuple[int, int, int], *, margin: float = 0.04) -> None:
    from PIL import Image

    im = Image.open(path).convert("RGB")
    w, h = im.size
    pts = (
        (int(w * margin), int(h * margin)),
        (int(w * (1 - margin)), int(h * margin)),
        (int(w * margin), int(h * (1 - margin))),
        (int(w * (1 - margin)), int(h * (1 - margin))),
    )
    for x, y in pts:
        r, g, b = im.getpixel((x, y))
        if abs(r - g) < 24 and abs(g - b) < 24 and 110 <= r <= 220:
            im.putpixel((x, y), bg)
    for x, y in ((0, 0), (w - 1, 0), (0, h - 1), (w - 1, h - 1)):
        r, g, b = im.getpixel((x, y))
        if abs(r - g) < 24 and abs(g - b) < 24 and 110 <= r <= 220:
            im.putpixel((x, y), bg)
    im.save(path, format="PNG")


def flatten(path: Path, bg: tuple[int, int, int]) -> None:
    from PIL import Image

    im = Image.open(path).convert("RGBA")
    if max(im.getchannel("A").getextrema()) < 250:
        pass
    plate = Image.new("RGB", im.size, bg)
    plate.paste(im, mask=im.split()[3])
    plate.save(path, format="PNG")


def main() -> int:
    try:
        from PIL import Image  # noqa: F401
    except ImportError:
        print("WARN flatten: Pillow missing")
        return 0
    # Correct path
    preview = REPO / "docs" / "art" / "previews"
    env_hud = [
        "garage-exterior-wall",
        "garage-land-for-sale-sign",
        "garage-land-for-sale-sign-front",
        "for-sale-sign",
        "garage-lot-curb",
        "garage-window-frame",
    ]
    for name in env_hud:
        p = preview / f"{name}.png"
        if p.exists():
            flatten(p, STUDIO_BG)
            pin_studio_corner_backdrop(p, STUDIO_BG)
    for p in preview.glob("*.png"):
        if p.stem.startswith(("icon-", "panel-", "hud-", "tex-")):
            flatten(p, HUD_BG)
    return 0


if __name__ == "__main__":
    sys.exit(main())