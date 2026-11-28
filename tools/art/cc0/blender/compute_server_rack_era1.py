"""CC0 hero: compute-server-rack-era1 — open wire/frame rack."""
import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import cc0_common as cc  # noqa: E402
import rack_era1_frame_kit as kit  # noqa: E402


def build() -> dict:
    cc.reset_scene()
    export_objs = kit.build_rack_era1()
    sources = [
        "procedural:era1 open wire frame rack",
        "style: corner posts, horizontal rails, no enclosed cabinet",
    ]
    out = cc.finish_cc0_asset("compute-server-rack-era1", export_objs[0], sources, export_objects=export_objs)
    out["generator"] = "tools/art/cc0/blender/compute_server_rack_era1.py"
    return out


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))