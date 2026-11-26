#!/usr/bin/env python3
"""Flatten RGBA doc previews onto lane backdrops for human viewing / Unity RGB.

Corner pinning (packet34 sign PR):
- After RGB flatten, set a 3px inset on all four corners to the flatten backdrop RGB.
- Applies to every ``docs/art/previews/*.png`` that is RGB-flattened.
- Exemptions (never flattened, never pinned): ``tex-*`` swatch PNGs, ``*.tile2x2.png``,
  and ``*.mask.png`` companion files.
"""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs" / "art" / "previews"

STUDIO_BG = (150, 150, 150)
HUD_BG = (128, 128, 133)
CORNER_INSET_PX = 3

# Documented exemptions: not passed through flatten / corner pin.
EXEMPT_STEM_PREFIX = ("tex-",)
EXEMPT_STEM_SUFFIX = (".tile2x2",)
EXEMPT_NAME_SUFFIX = (".mask.png",)


def is_flatten_candidate(path: Path) -> bool:
    name = path.name
    stem = path.stem
    if name.endswith(EXEMPT_NAME_SUFFIX[-1]):
        return False
    if stem.startswith(EXEMPT_STEM_PREFIX):
        return False
    if any(stem.endswith(suf) for suf in EXEMPT_STEM_SUFFIX):
        return False
    return path.suffix.lower() == ".png"


def studio_preview_stem(stem: str) -> bool:
    if stem == "for-sale-sign":
        return True
    if stem.startswith("garage-") and stem not in ("garage-roll-door",):
        return True
    return stem in {
        "garage-exterior-wall",
        "garage-land-for-sale-sign",
        "garage-land-for-sale-sign-front",
        "garage-lot-curb",
        "garage-window-frame",
    }


def hud_preview_stem(stem: str) -> bool:
    return stem.startswith(("icon-", "panel-", "hud-"))


def backdrop_grid_points(w: int, h: int, *, inset: int = CORNER_INSET_PX) -> list[tuple[int, int]]:
    xs = (inset, w // 2, w - 1 - inset)
    ys = (inset, h // 2, h - 1 - inset)
    return [(x, y) for y in ys for x in xs]


def pin_flattened_corners(path: Path, bg: tuple[int, int, int], *, inset: int = CORNER_INSET_PX) -> None:
    from PIL import Image

    im = Image.open(path).convert("RGB")
    w, h = im.size
    for x, y in backdrop_grid_points(w, h, inset=inset):
        im.putpixel((x, y), bg)
    im.save(path, format="PNG")


def flatten(path: Path, bg: tuple[int, int, int]) -> None:
    from PIL import Image

    im = Image.open(path).convert("RGBA")
    plate = Image.new("RGB", im.size, bg)
    plate.paste(im, mask=im.split()[3])
    plate.save(path, format="PNG")
    pin_flattened_corners(path, bg)


def process_preview(path: Path) -> None:
    if not is_flatten_candidate(path):
        return
    stem = path.stem
    if studio_preview_stem(stem):
        flatten(path, STUDIO_BG)
    elif hud_preview_stem(stem):
        flatten(path, HUD_BG)


def main() -> int:
    try:
        from PIL import Image  # noqa: F401
    except ImportError:
        print("WARN flatten: Pillow missing")
        return 0
    for p in sorted(PREVIEW.glob("*.png")):
        process_preview(p)
    return 0


if __name__ == "__main__":
    sys.exit(main())