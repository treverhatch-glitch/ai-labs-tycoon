#!/usr/bin/env python3
"""Render HUD icons in Blender (3D glyphs). Wisp sheet previews come from generate_wisp_mascot.py."""
from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_hud_icon as hi  # noqa: E402
import bpy  # noqa: E402


def _icon_cash() -> bpy.types.Object:
    bc.reset_scene()
    card = bc.add_cube("CashBody", (0.55, 0.38, 0.06))
    card.data.materials.append(hi.solid_mat("orange", hi._hex((200, 120, 48)), emission=0.15))
    rim = bc.add_cube("CashRim", (0.62, 0.44, 0.04))
    rim.data.materials.append(hi.solid_mat("paper", hi._hex((244, 240, 232)), emission=0.1))
    chip = bc.add_cube("Dollar", (0.18, 0.18, 0.05), (0, 0, 0.06))
    chip.data.materials.append(hi.solid_mat("glyph", hi._hex((244, 240, 232)), emission=0.35))
    root = bc.link_or_join([rim, card, chip])
    bc.bevel_object(root, 0.025, 2)
    return root


def _icon_date() -> bpy.types.Object:
    bc.reset_scene()
    body = bc.add_cube("CalBody", (0.48, 0.58, 0.06))
    body.data.materials.append(hi.solid_mat("tan", hi._hex((200, 184, 160))))
    band = bc.add_cube("CalBand", (0.5, 0.16, 0.07), (0, 0.22, 0.02))
    band.data.materials.append(hi.solid_mat("era", hi._hex((168, 88, 40)), emission=0.2))
    nines = bc.add_cube("Digits", (0.22, 0.14, 0.05), (0, -0.05, 0.05))
    nines.data.materials.append(hi.solid_mat("ink", hi._hex((26, 29, 36)), emission=0.05))
    root = bc.link_or_join([body, band, nines])
    bc.bevel_object(root, 0.02, 2)
    return root


def _icon_era_bar() -> bpy.types.Object:
    bc.reset_scene()
    track = bc.add_cube("Track", (0.82, 0.12, 0.05))
    track.data.materials.append(hi.solid_mat("track", hi._hex((58, 58, 68))))
    fill = bc.add_cube("Fill", (0.28, 0.12, 0.06), (-0.22, 0, 0.01))
    fill.data.materials.append(hi.solid_mat("era", hi._hex((168, 88, 40)), emission=0.25))
    label = bc.add_cube("Label", (0.32, 0.08, 0.04), (0, 0.16, 0.02))
    label.data.materials.append(hi.solid_mat("paper", hi._hex((244, 240, 232)), emission=0.15))
    return bc.link_or_join([track, fill, label])


def _icon_build() -> bpy.types.Object:
    bc.reset_scene()
    roof = bc.add_cube("Roof", (0.62, 0.48, 0.05), (0, 0, 0.2))
    roof.data.materials.append(hi.solid_mat("gold", hi._hex((242, 191, 90)), emission=0.2))
    base = bc.add_cube("Base", (0.42, 0.24, 0.06), (0, 0, 0.02))
    base.data.materials.append(hi.solid_mat("ink", hi._hex((26, 29, 36))))
    return bc.link_or_join([roof, base])


def _icon_hire() -> bpy.types.Object:
    bc.reset_scene()
    head = bc.add_cube("Head", (0.22, 0.22, 0.12), (0, 0.18, 0.04))
    head.data.materials.append(hi.solid_mat("paper", hi._hex((244, 240, 232))))
    torso = bc.add_cube("Torso", (0.42, 0.38, 0.1), (0, -0.05, 0.02))
    torso.data.materials.append(hi.solid_mat("green", hi._hex((102, 191, 115)), emission=0.15))
    badge = bc.add_cube("Badge", (0.16, 0.16, 0.08), (0.28, -0.02, 0.04))
    badge.data.materials.append(hi.solid_mat("era", hi._hex((168, 88, 40)), emission=0.2))
    return bc.link_or_join([head, torso, badge])


def _icon_train() -> bpy.types.Object:
    bc.reset_scene()
    bars = []
    for i, (w, col) in enumerate(
        (
            (0.62, (140, 102, 242)),
            (0.5, (106, 70, 208)),
            (0.38, (74, 38, 176)),
        )
    ):
        bar = bc.add_cube(f"Bar{i}", (w, 0.12, 0.05), (0, -0.12 + i * 0.14, 0.03))
        bar.data.materials.append(hi.solid_mat(f"p{i}", hi._hex(col), emission=0.2))
        bars.append(bar)
    return bc.link_or_join(bars)


ICON_BUILDERS = {
    "icon-cash-era0": _icon_cash,
    "icon-date-era0": _icon_date,
    "icon-era-bar": _icon_era_bar,
    "icon-build-panel": _icon_build,
    "icon-hire-panel": _icon_hire,
    "icon-train-panel": _icon_train,
}


def main() -> None:
    for asset_id, builder in ICON_BUILDERS.items():
        root = builder()
        hi.finish_hud_icon(asset_id, root)
    print("HUD icon batch OK (wisp previews: generate_wisp_mascot.py)")


if __name__ == "__main__":
    main()