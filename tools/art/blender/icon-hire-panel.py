#!/usr/bin/env python3
"""Blender HUD: hire panel glyph."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_hud_icon as hi  # noqa: E402

ASSET_ID = "icon-hire-panel"


def main() -> None:
    bc.reset_scene()
    head = bc.add_cylinder("Head", 0.14, 0.16, (0, 0.2, 0), vertices=14)
    head.data.materials.append(hi.solid_mat("skin", hi._hex((244, 240, 232))))
    body = hi.add_glyph_bar("Body", (0.38, 0.22, 0.07), (0, -0.08, 0), hi._hex((102, 191, 115)), emission=0.2)
    badge = hi.add_glyph_bar("Badge", (0.14, 0.14, 0.08), (0.28, -0.02, 0.03), hi._hex((168, 88, 40)), emission=0.35)
    root = bc.link_or_join([head, body, badge])
    bc.bevel_object(root, 0.02, 2)
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()