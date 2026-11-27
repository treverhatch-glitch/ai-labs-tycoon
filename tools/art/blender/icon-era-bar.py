#!/usr/bin/env python3
"""Blender HUD: era progress bar."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_hud_icon as hi  # noqa: E402

ASSET_ID = "icon-era-bar"


def main() -> None:
    bc.reset_scene()
    track = hi.add_glyph_bar("Track", (0.88, 0.14, 0.06), (0, 0, 0), hi._hex((58, 58, 68)))
    fill = hi.add_glyph_bar("Fill", (0.28, 0.14, 0.07), (-0.28, 0, 0.02), hi._hex((168, 88, 40)), emission=0.45)
    label = hi.add_glyph_bar("Label", (0.22, 0.08, 0.05), (0, 0.22, 0.04), hi._hex((244, 240, 232)), emission=0.1)
    root = bc.link_or_join([track, fill, label])
    bc.bevel_object(root, 0.015, 2)
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()