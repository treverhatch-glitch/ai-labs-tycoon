#!/usr/bin/env python3
"""Render packet34 env previews from committed GLBs (not in-process bake mesh)."""
from __future__ import annotations

import os
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402

os.environ.setdefault("EARLY_ART_PREVIEW_RIG", "1")
os.environ.setdefault("EARLY_PREVIEW_MID_GREY", "1")
os.environ.setdefault("EARLY_PREVIEW_CONTACT_SHADOW", "1")

ENV_IDS = (
    "garage-exterior-wall",
    "garage-land-for-sale-sign",
    "garage-lot-curb",
    "garage-window-frame",
)

PREVIEW_ENV: dict[str, dict[str, str]] = {
    "garage-exterior-wall": {"PREVIEW_FRAMING_MARGIN": "1.02", "PREVIEW_AZIMUTH_DEG": "52"},
    "garage-land-for-sale-sign": {"PREVIEW_FRAMING_MARGIN": "1.12", "PREVIEW_AZIMUTH_DEG": "42"},
    "garage-lot-curb": {"PREVIEW_FRAMING_MARGIN": "1.14"},
    "garage-window-frame": {"PREVIEW_FRAMING_MARGIN": "1.12"},
}


def main() -> int:
    for asset_id in ENV_IDS:
        print(f"=== preview from GLB {asset_id} ===")
        for key in ("PREVIEW_FRAMING_MARGIN", "PREVIEW_AZIMUTH_DEG"):
            os.environ.pop(key, None)
        for key, val in PREVIEW_ENV.get(asset_id, {}).items():
            os.environ[key] = val
        bc.rerender_garage_preview(asset_id, "garage-kit")
    print("OK packet34 env previews from GLB")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())