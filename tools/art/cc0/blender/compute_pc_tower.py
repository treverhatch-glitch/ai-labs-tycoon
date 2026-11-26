"""CC0 hero: procedural desktop PC tower."""
import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import cc0_common as cc  # noqa: E402
import mesh_builders as mb  # noqa: E402


def build() -> dict:
    cc.reset_scene()
    root = mb.build_pc_tower_desktop()
    palette = [
        (0.24, 0.25, 0.28),
        (0.20, 0.21, 0.24),
        (0.30, 0.32, 0.35),
        (0.55, 0.40, 0.95),
        (0.15, 0.55, 0.85),
        (0.72, 0.75, 0.78),
    ]
    root = mb.color_parts_by_index(root, palette, led_indices={3, 4})
    sources = ["procedural:bpy desktop tower (Kenney/Quaternius scale reference)"]
    return cc.finish_cc0_asset("compute-pc-tower", root, sources)


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))