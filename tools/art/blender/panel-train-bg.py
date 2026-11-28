#!/usr/bin/env python3
"""Blender HUD: train panel plate."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_hud_icon as hi  # noqa: E402

ASSET_ID = "panel-train-bg"


def main() -> None:
    bc.reset_scene()
    plate = bc.add_cube("Plate", (1.0, 1.0, 0.06))
    plate.data.materials.append(hi.solid_mat("fill", hi._hex((26, 24, 36))))
    rim = bc.add_cube("Rim", (1.08, 1.08, 0.04))
    rim.data.materials.append(hi.solid_mat("stroke", hi._hex((140, 102, 242)), emission=0.25))
    root = bc.link_or_join([rim, plate])
    bc.bevel_object(root, 0.04, 3)
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()