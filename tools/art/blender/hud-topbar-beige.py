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
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.30"
    bc.reset_scene()
    slate = hi._hex((28, 32, 42))
    trim = hi._hex((212, 96, 32))
    bar = bc.add_cube("Bar", (2.4, 0.36, 0.06))
    bar.data.materials.append(hi.solid_mat("slate", slate))
    stripe = bc.add_cube("Trim", (2.4, 0.04, 0.07), (0, -0.14, 0.04))
    stripe.data.materials.append(hi.solid_mat("trim", trim, emission=0.14))
    root = bc.link_or_join([bar, stripe])
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()