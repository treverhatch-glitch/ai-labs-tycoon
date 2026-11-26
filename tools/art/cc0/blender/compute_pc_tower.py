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
    sources = ["procedural:bpy desktop tower (single island)"]
    return cc.finish_cc0_asset("compute-pc-tower", root, sources)


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))