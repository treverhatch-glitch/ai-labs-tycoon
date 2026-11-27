"""CC0 hero: Quaternius Prop_Computer + Kenney factory accents (TP Hospital restyle)."""
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
        "quaternius:Prop_Computer (Modular Sci-Fi MegaKit)",
        "kenney:factory-kit screen-panel-small, screen-panel-flat, piston-*",
    ]
    return cc.finish_cc0_asset("compute-pc-tower", root, sources)


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))