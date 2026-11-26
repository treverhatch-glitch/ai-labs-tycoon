#!/usr/bin/env python3
"""Flatten RGBA doc previews onto lane backdrops for human viewing / Unity RGB."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs" / "art" / "previews"

STUDIO_BG = (140, 140, 145)
HUD_BG = (128, 128, 133)


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
        "garage-lot-curb",
        "garage-window-frame",
    ]
    for name in env_hud:
        p = preview / f"{name}.png"
        if p.exists():
            flatten(p, STUDIO_BG)
    for p in preview.glob("*.png"):
        if p.stem.startswith(("icon-", "panel-", "hud-", "tex-")):
            flatten(p, HUD_BG)
    return 0


if __name__ == "__main__":
    sys.exit(main())