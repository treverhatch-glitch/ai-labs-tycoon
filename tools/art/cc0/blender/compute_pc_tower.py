"""CC0 hero: compute-pc-tower-era1 — beige 90s rect desktop tower."""
import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import cc0_common as cc  # noqa: E402
import pc_tower_kit as kit  # noqa: E402


def build() -> dict:
    cc.reset_scene()
    root = kit.build_pc_tower_cc0()
    sources = [
        "procedural:era1 beige 90s desktop tower (flat per-part vertex colors)",
        "style: TPH chunky rect; saturated power/hdd accents only",
    ]
    out = cc.finish_cc0_asset("compute-pc-tower-era1", root, sources)
    out["generator"] = "tools/art/cc0/blender/compute_pc_tower.py"
    return out


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))