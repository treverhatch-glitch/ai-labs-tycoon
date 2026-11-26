#!/usr/bin/env python3
"""Black server rack, orange trim, blinking GPU LEDs."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("PostL", (0.06, 1.90, 0.06), (-0.30, 0.95, 0))
    bc.add_cube("PostR", (0.06, 1.90, 0.06), (0.30, 0.95, 0))
    bc.add_cube("Top", (0.62, 0.05, 0.62), (0, 1.88, 0))
    bc.add_cube("Base", (0.62, 0.05, 0.62), (0, 0.025, 0))
    for slot in range(6):
        sy = 0.18 + slot * 0.27
        bc.add_cube(f"Shelf{slot}", (0.54, 0.03, 0.52), (0, sy, 0))
        bc.add_cube(f"Unit{slot}", (0.48, 0.18, 0.46), (0, sy + 0.11, 0.02))
        led = bc.add_cube(f"Led{slot}", (0.06, 0.06, 0.03), (0.20, sy + 0.08, 0.24))
        if slot % 2 == 0:
            bc.assign_emission(led, (0.95, 0.35, 0.08, 1.0), strength=5.0)
        else:
            bc.assign_emission(led, (0.15, 0.85, 0.35, 1.0), strength=5.0)
    bc.add_cube("TrimBar", (0.62, 0.12, 0.06), (0, 1.38, 0.30))
    badge = bc.add_cube("Badge", (0.36, 0.14, 0.03), (0, 1.75, 0.29))
    bc.assign_emission(badge, (0.95, 0.55, 0.08, 1.0), strength=6.0)
    entry = bc.finish_asset("garage-voodude-rack", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)