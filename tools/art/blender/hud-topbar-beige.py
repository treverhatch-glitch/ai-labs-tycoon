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
    trim = hi._hex((220, 98, 22))
    accent = hi._hex((42, 132, 210))
    bar = bc.add_cube("Bar", (2.4, 0.36, 0.06))
    bar.data.materials.append(hi.solid_mat("bar_blue", (0.12, 0.48, 0.92, 1.0), emission=0.15))
    stripe = bc.add_cube("Trim", (2.4, 0.10, 0.07), (0, -0.11, 0.04))
    stripe.data.materials.append(hi.solid_mat("trim", (0.98, 0.42, 0.08, 1.0), emission=1.6))
    tick = bc.add_cube("AccentTick", (0.22, 0.12, 0.09), (-0.88, 0.02, 0.04))
    tick.data.materials.append(hi.solid_mat("accent", accent, emission=1.1))
    gem = bc.add_cube("Gem", (0.18, 0.16, 0.12), (0.72, 0.02, 0.04))
    gem.data.materials.append(hi.solid_mat("gem", (0.98, 0.12, 0.55, 1.0), emission=1.8))
    groove = bc.add_cube("Groove", (2.2, 0.04, 0.08), (0, 0.02, 0.02))
    groove.data.materials.append(hi.solid_mat("groove", (0.05, 0.06, 0.08, 1.0), emission=0.0))
    root = bc.link_or_join([bar, stripe, tick, gem, groove])
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()