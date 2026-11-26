#!/usr/bin/env python3
"""Concrete curb segment ~1m × 0.15m × 0.3m, beveled deck."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402

L, H, D = 1.0, 0.15, 0.30


def main():
    import os

    os.environ["PREVIEW_FRAMING_MARGIN"] = "1.24"
    os.environ["PREVIEW_AZIMUTH_DEG"] = "48"
    os.environ["EARLY_PREVIEW_MID_GREY"] = "1"
    os.environ["EARLY_PREVIEW_CONTACT_SHADOW"] = "1"
    curb = bc.add_cube("CurbDeck", (L, H, D), (0, H * 0.5, 0))
    sw = Path(__file__).resolve().parents[3] / "unity/Assets/Art/EarlyGame/textures/tex-garage-concrete.png"
    if sw.is_file():
        bc.assign_image_texture(curb, sw, uv_scale=(2.5, 0.35, 1.5), coord="GENERATED", roughness=0.82)
    else:
        bc.assign_solid(curb, (0.52, 0.50, 0.48, 1.0), 0.84)
    bc.bevel_object(curb, 0.012, 2)
    entry = bc.finish_asset("garage-lot-curb", "garage-kit")
    print("OK", entry["id"], entry["triangleCount"], "tris", entry["subShapeCount"], "parts")
    return entry


if __name__ == "__main__":
    bc.run_script(main)