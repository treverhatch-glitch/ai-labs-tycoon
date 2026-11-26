"""CC0 hero: procedural 42U server cabinet (ambientCG metal palette)."""
import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import cc0_common as cc  # noqa: E402
import mesh_builders as mb  # noqa: E402


def build() -> dict:
    cc.reset_scene()
    root = mb.build_server_rack_42u()
    sources = [
        "tools/art/cc0/incoming/ambientcg/Metal048A (palette reference)",
        "procedural:bpy 42U cabinet (single island)",
    ]
    return cc.finish_cc0_asset("compute-server-rack", root, sources)


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))