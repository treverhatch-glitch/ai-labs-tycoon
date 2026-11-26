#!/usr/bin/env python3
"""Blender HUD: build panel glyph."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_hud_icon as hi  # noqa: E402

ASSET_ID = "icon-build-panel"


def main() -> None:
    bc.reset_scene()
    roof_l = hi.add_glyph_bar("RoofL", (0.22, 0.32, 0.06), (-0.16, 0.14, 0), hi._hex((242, 191, 90)), emission=0.25)
    roof_r = hi.add_glyph_bar("RoofR", (0.22, 0.32, 0.06), (0.16, 0.14, 0), hi._hex((242, 191, 90)), emission=0.25)
    door = hi.add_glyph_bar("Door", (0.22, 0.18, 0.07), (0, -0.12, 0.02), hi._hex((26, 29, 36)))
    root = bc.link_or_join([roof_l, roof_r, door])
    bc.bevel_object(root, 0.02, 2)
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()