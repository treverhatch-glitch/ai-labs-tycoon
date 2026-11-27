#!/usr/bin/env python3
"""CC0 Kenney blocky character → founder hero (recolored hoodie/jeans, baked mesh)."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import kenney_founder_lib as kf  # noqa: E402


def main():
    kf.build_founder_kenney()
    entry = bc.finish_asset(
        "staff-founder-idle",
        "staff-kit",
        license_note="CC0-1.0 (Kenney Animated Characters 3 + in-repo bake)",
    )
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)