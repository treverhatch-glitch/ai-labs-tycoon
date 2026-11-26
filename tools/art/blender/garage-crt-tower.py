#!/usr/bin/env python3
"""Beige CRT tower + floppy bay + green power LED."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402


def main():
    bc.add_cube("Plinth", (0.30, 0.05, 0.26), (0, 0.025, 0))
    bc.add_cube("Lower", (0.26, 0.42, 0.22), (0, 0.26, 0))
    bc.add_cube("Upper", (0.32, 0.38, 0.28), (0, 0.69, -0.02))
    bc.add_cube("CRTBezel", (0.30, 0.32, 0.14), (0, 0.78, 0.08))
    screen = bc.add_cube("Screen", (0.28, 0.26, 0.04), (0, 0.78, 0.17))
    bc.assign_emission(screen, (0.08, 0.98, 0.48, 1.0), strength=12.0)
    bc.add_cube("FloppyBay", (0.22, 0.06, 0.26), (0, 0.44, 0.14))
    bc.add_cube("SideLabel", (0.10, 0.52, 0.26), (0.17, 0.72, 0.0))
    bc.add_cube("FrontPanel", (0.34, 0.50, 0.03), (0.0, 0.72, 0.16))
    bc.add_cube("TopTrim", (0.36, 0.07, 0.32), (0, 1.02, 0.0))
    led = bc.add_cube("PowerLed", (0.025, 0.025, 0.02), (0.10, 0.50, 0.20))
    bc.assign_emission(led, (0.2, 0.95, 0.25, 1.0), strength=6.0)
    entry = bc.finish_asset("garage-crt-tower", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris")
    return entry


if __name__ == "__main__":
    bc.run_script(main)