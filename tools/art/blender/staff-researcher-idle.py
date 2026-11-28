#!/usr/bin/env python3
"""CC0 Kenney character → researcher hero (recolour lab coat, shirt, glasses)."""
from __future__ import annotations

import os
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import kenney_researcher_lib as kr  # noqa: E402
from kenney_researcher_from_founder import apply_trouser_grey_leg_faces  # noqa: E402


def main():
    os.environ["RESEARCHER_STOCK_RECOLOR"] = "1"
    kr.build_researcher_kenney()
    n_faces = apply_trouser_grey_leg_faces()
    print(f"OK staff-researcher-idle trouser_grey faces={n_faces}")
    entry = bc.finish_asset(
        "staff-researcher-idle",
        "staff-kit",
        license_note="CC0-1.0 (Kenney Animated Characters 3 + in-repo lab-coat recolour)",
    )
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)