#!/usr/bin/env python3
"""Blender HUD: wide top bar chrome (matches hud-topbar-beige.svg)."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_hud_icon as hi  # noqa: E402
import bpy  # noqa: E402

ASSET_ID = "hud-topbar-beige"


def main() -> None:
    bc.reset_scene()
    slate = hi._hex((20, 23, 29))
    trim = hi._hex((168, 88, 40))
    bar = bc.add_cube("Bar", (2.4, 0.36, 0.06))
    bar.data.materials.append(hi.solid_mat("slate", slate, emission=0.15))
    stripe = bc.add_cube("Trim", (2.4, 0.04, 0.07), (0, -0.14, 0.04))
    stripe.data.materials.append(hi.solid_mat("trim", trim, emission=0.8))
    root = bc.link_or_join([bar, stripe])
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()