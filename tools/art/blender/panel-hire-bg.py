#!/usr/bin/env python3
"""Blender HUD: hire panel plate."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_hud_icon as hi  # noqa: E402

ASSET_ID = "panel-hire-bg"


def main() -> None:
    bc.reset_scene()
    plate = bc.add_cube("Plate", (1.0, 1.0, 0.06))
    plate.data.materials.append(hi.solid_mat("fill", hi._hex((28, 86, 52)), emission=0.12))
    rim = bc.add_cube("Rim", (1.08, 1.08, 0.04))
    rim.data.materials.append(hi.solid_mat("stroke", hi._hex((140, 220, 120)), emission=0.35))
    accent = bc.add_cube("Badge", (0.14, 0.42, 0.08), (-0.32, 0.18, 0.08))
    stripe = bc.add_cube("Stripe", (1.0, 0.08, 0.07), (0.0, 0.38, 0.07))
    stripe.data.materials.append(hi.solid_mat("stripe", hi._hex((48, 196, 96)), emission=0.2))
    accent.data.materials.append(hi.solid_mat("accent", hi._hex((220, 64, 96)), emission=0.22))
    root = bc.link_or_join([rim, plate, accent, stripe])
    bc.bevel_object(root, 0.04, 3)
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()