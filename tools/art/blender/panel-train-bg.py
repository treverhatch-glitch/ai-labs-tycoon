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
    plate.data.materials.append(hi.solid_mat("fill", hi._hex((72, 40, 108)), emission=0.12))
    rim = bc.add_cube("Rim", (1.08, 1.08, 0.04))
    rim.data.materials.append(hi.solid_mat("stroke", hi._hex((196, 140, 255)), emission=0.35))
    accent = bc.add_cube("Book", (0.36, 0.22, 0.08), (0.22, 0.24, 0.08))
    stripe = bc.add_cube("Stripe", (0.08, 0.88, 0.07), (0.36, -0.28, 0.07))
    stripe.data.materials.append(hi.solid_mat("stripe", hi._hex((120, 96, 240)), emission=0.2))
    accent.data.materials.append(hi.solid_mat("accent", hi._hex((120, 200, 240)), emission=0.22))
    root = bc.link_or_join([rim, plate, accent, stripe])
    bc.bevel_object(root, 0.04, 3)
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()