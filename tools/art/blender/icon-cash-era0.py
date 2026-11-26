#!/usr/bin/env python3
"""Blender HUD: dial-up cash badge."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_hud_icon as hi  # noqa: E402

ASSET_ID = "icon-cash-era0"


def main() -> None:
    bc.reset_scene()
    body = hi.add_glyph_bar("Bill", (0.72, 0.46, 0.08), (0, 0, 0), hi._hex((232, 118, 28)), emission=0.22)
    rim = hi.add_glyph_bar("Rim", (0.76, 0.5, 0.05), (0, 0, 0.04), hi._hex((42, 28, 18)), emission=0.0)
    mark = hi.add_glyph_bar("Dollar", (0.12, 0.28, 0.09), (0, 0.02, 0.05), hi._hex((255, 232, 148)), emission=0.45)
    shade = hi.add_glyph_bar("Shade", (0.74, 0.12, 0.06), (0, -0.18, 0.02), hi._hex((24, 16, 10)), emission=0.0)
    root = bc.link_or_join([rim, body, mark, shade])
    bc.bevel_object(root, 0.02, 2)
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()