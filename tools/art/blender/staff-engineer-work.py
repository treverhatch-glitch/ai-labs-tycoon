#!/usr/bin/env python3
"""Engineer CC0 hero — Kenney platformer base + role props."""
import os
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import staff_cc0 as scc0  # noqa: E402

LICENSE_NOTE = (
    "CC0: Kenney Starter Kit 3D Platformer (character.glb) + in-repo bake; "
    "see docs/art/LICENSES.md"
)


def main():
    os.environ["EARLY_ART_STAFF_CC0"] = "1"
    scc0.build_engineer_cc0()
    entry = bc.finish_asset("staff-engineer-work", "staff-kit", license_note=LICENSE_NOTE)
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)