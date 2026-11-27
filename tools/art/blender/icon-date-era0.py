#!/usr/bin/env python3
"""Blender HUD: dial-up calendar."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_hud_icon as hi  # noqa: E402

ASSET_ID = "icon-date-era0"


def main() -> None:
    bc.reset_scene()
    back = hi.add_glyph_bar("Back", (0.62, 0.72, 0.06), (0, 0, 0), hi._hex((218, 168, 108)), emission=0.12)
    header = hi.add_glyph_bar("Header", (0.62, 0.18, 0.07), (0, 0.22, 0.04), hi._hex((168, 88, 40)), emission=0.35)
    digits = hi.add_glyph_bar("Digits", (0.32, 0.22, 0.08), (0, -0.08, 0.05), hi._hex((18, 82, 168)), emission=0.28)
    root = bc.link_or_join([back, header, digits])
    bc.bevel_object(root, 0.02, 2)
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()