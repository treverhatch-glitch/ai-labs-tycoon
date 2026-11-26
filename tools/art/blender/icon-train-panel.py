#!/usr/bin/env python3
"""Blender HUD: train panel glyph."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_hud_icon as hi  # noqa: E402

ASSET_ID = "icon-train-panel"


def main() -> None:
    bc.reset_scene()
    bars = []
    colors = ((140, 102, 242), (106, 70, 208), (74, 38, 176))
    ys = (0.14, 0, -0.14)
    for i, (col, y) in enumerate(zip(colors, ys)):
        bars.append(hi.add_glyph_bar(f"Bar{i}", (0.58 - i * 0.08, 0.1, 0.06), (0, y, 0), hi._hex(col), emission=0.35))
    root = bc.link_or_join(bars)
    bc.bevel_object(root, 0.015, 2)
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()