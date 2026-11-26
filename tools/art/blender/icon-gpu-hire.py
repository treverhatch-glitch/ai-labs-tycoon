#!/usr/bin/env python3
"""Blender HUD: GPU hire badge."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import blender_hud_icon as hi  # noqa: E402
import bpy  # noqa: E402

ASSET_ID = "icon-gpu-hire"


def main() -> None:
    bc.reset_scene()
    body = bc.add_cube("Body", (0.55, 0.65, 0.12))
    body.data.materials.append(hi.solid_mat("chassis", hi._hex((90, 90, 102))))
    slot1 = bc.add_cube("Gpu", (0.45, 0.12, 0.06), (0, 0.08, 0.1))
    slot1.data.materials.append(hi.solid_mat("gpu", hi._hex((140, 102, 242)), emission=0.5))
    slot2 = bc.add_cube("Status", (0.45, 0.12, 0.06), (0, -0.08, 0.1))
    slot2.data.materials.append(hi.solid_mat("ok", hi._hex((102, 191, 115)), emission=0.35))
    root = bc.link_or_join([body, slot1, slot2])
    bc.bevel_object(root, 0.02, 2)
    hi.finish_hud_icon(ASSET_ID, root)


if __name__ == "__main__":
    main()