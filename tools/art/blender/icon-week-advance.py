#!/usr/bin/env python3
"""Blender HUD: week advance chevron."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_hud_icon as hi  # noqa: E402
import bpy  # noqa: E402

ASSET_ID = "icon-week-advance"


def main() -> None:
    bc.reset_scene()
    arrow = bc.add_cube("Arrow", (0.5, 0.35, 0.08))
    arrow.rotation_euler = (0, 0, 0.15)
    arrow.data.materials.append(hi.solid_mat("blue", hi._hex((64, 136, 217)), emission=0.4))
    tab = bc.add_cube("Tab", (0.14, 0.35, 0.08), (-0.42, 0, 0))
    tab.data.materials.append(hi.solid_mat("paper", hi._hex((228, 148, 88)), emission=0.18))
    root = bc.link_or_join([arrow, tab])
    bc.bevel_object(root, 0.015, 2)
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()