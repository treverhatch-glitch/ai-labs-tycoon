#!/usr/bin/env python3
"""CC0 Kenney humanoid → founder hero (idle + mug)."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import cc0_staff_lib as c0  # noqa: E402


def main():
    c0.build_founder_cc0()
    entry = bc.finish_asset(
        "staff-founder-idle",
        "staff-kit",
        license_note="CC0-1.0 (Kenney Animated Characters 3 + Quaternius-derived bake)",
    )
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)