#!/usr/bin/env python3
"""CC0 Quaternius humanoid → researcher hero (lab coat + clipboard)."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import cc0_staff_lib as c0  # noqa: E402


def main():
    c0.build_researcher_cc0()
    entry = bc.finish_asset(
        "staff-researcher-idle",
        "staff-kit",
        license_note="CC0-1.0 (Quaternius Ultimate Modular Character + in-repo bake)",
    )
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)