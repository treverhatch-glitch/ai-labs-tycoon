#!/usr/bin/env python3
"""Packet 3–4 Hatch hard gates (packet34 bar)."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
sys.path.insert(0, str(REPO / "tools/art"))

from check_env_glb_packet34 import check_all_env_glbs  # noqa: E402
from check_env_preview_glb_linkage import main as check_preview_glb_linkage  # noqa: E402
from packet34_preview_gates import (  # noqa: E402
    ALL_HUD_SPRITE_IDS,
    ICON_IDS,
    check_backdrop_mid_grey,
    check_env_preview_backdrop,
    check_icon_ortho,
    check_icon_sat_coverage,
    check_icon_sheet,
    check_near_duplicates,
    check_texture_swatch,
    check_tile2x2_seam,
)
from preview_quality_lib import mask_gate_errors  # noqa: E402

ENV_IDS = (
    "garage-exterior-wall",
    "garage-land-for-sale-sign",
    "garage-lot-curb",
    "garage-window-frame",
)
TEX_IDS = ("tex-hud-paper-warm", "tex-garage-wall-beige", "tex-garage-trim-orange")


def check_env_preview(asset_id: str) -> int:
    rc = 0
    rgb = PREVIEW / f"{asset_id}.png"
    mask = PREVIEW / f"{asset_id}.mask.png"
    meta = PREVIEW / f"{asset_id}.frame.json"
    if not rgb.is_file() or not mask.is_file() or not meta.is_file():
        print(f"FAIL {asset_id} missing rgb/mask/frame")
        return 1
    for err in check_backdrop_mid_grey(rgb):
        print(f"FAIL {asset_id} {err}")
        rc = 1
    for err in check_env_preview_backdrop(asset_id, rgb, mask):
        print(f"FAIL {asset_id} {err}")
        rc = 1
    for err in mask_gate_errors(mask):
        print(f"FAIL {asset_id} {err}")
        rc = 1
    if rc == 0:
        print(f"OK {asset_id} env_preview")
    return rc


def main() -> int:
    try:
        from PIL import Image  # noqa: F401
    except ImportError:
        print("FAIL: Pillow required")
        return 1

    rc = check_all_env_glbs()
    rc |= check_preview_glb_linkage()
    for asset_id in ENV_IDS:
        rc |= check_env_preview(asset_id)
    for asset_id in ALL_HUD_SPRITE_IDS:
        errs = check_icon_ortho(PREVIEW / f"{asset_id}.frame.json")
        errs.extend(check_icon_sat_coverage(asset_id, PREVIEW))
        if errs:
            for err in errs:
                print(f"FAIL {asset_id} {err}")
            rc |= 1
        else:
            print(f"OK {asset_id} hud_sprite")
    for err in check_near_duplicates(PREVIEW, ICON_IDS):
        print(f"FAIL {err}")
        rc |= 1
    for err in check_near_duplicates(PREVIEW, TEX_IDS, min_sim=0.90):
        print(f"FAIL {err}")
        rc |= 1
    for asset_id in TEX_IDS:
        tex_errs = check_texture_swatch(PREVIEW, asset_id)
        if asset_id == "tex-garage-trim-orange":
            tex_errs.extend(check_tile2x2_seam(PREVIEW, asset_id))
        if tex_errs:
            for err in tex_errs:
                print(f"FAIL {asset_id} {err}")
            rc |= 1
        else:
            print(f"OK {asset_id} texture_swatch")
    for err in check_icon_sheet(PREVIEW):
        print(f"FAIL {err}")
        rc |= 1
    else:
        print("OK icon-sheet-64")
    return rc


if __name__ == "__main__":
    sys.exit(main())