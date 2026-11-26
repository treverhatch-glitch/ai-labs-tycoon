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
    mb.assert_single_object(root, max_gap_m=0.01)
    dark_metal = [
        (0.18, 0.19, 0.21),
        (0.22, 0.23, 0.26),
        (0.26, 0.27, 0.30),
        (0.14, 0.15, 0.17),
        (0.32, 0.33, 0.36),
        (0.12, 0.78, 0.38),
        (0.18, 0.45, 0.92),
    ]
    root = mb.color_parts_by_index(root, dark_metal)
    sources = [
        "tools/art/cc0/incoming/ambientcg/Metal048A (palette reference)",
        "procedural:bpy 42U cabinet",
    ]
    return cc.finish_cc0_asset("compute-server-rack", root, sources)


if __name__ == "__main__":
    print(json.dumps(build(), indent=2))