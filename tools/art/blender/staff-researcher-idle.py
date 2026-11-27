#!/usr/bin/env python3
"""CC0 Quaternius modular character → researcher hero (lab coat, shirt/tie, glasses)."""
import os
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import kenney_researcher_lib as kr  # noqa: E402


def main():
    os.environ.pop("RESEARCHER_STOCK_RECOLOR", None)
    kr.build_researcher_kenney()
    entry = bc.finish_asset(
        "staff-researcher-idle",
        "staff-kit",
        license_note="CC0-1.0 (Quaternius Ultimate Modular Character + in-repo doctor styling)",
    )
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)