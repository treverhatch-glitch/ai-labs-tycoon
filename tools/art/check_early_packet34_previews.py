#!/usr/bin/env python3
"""Packet 3–4: Hatch alpha-mask gates (shared with #88 340fa3cd)."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"

ENV_IDS = (
    "garage-exterior-wall",
    "garage-land-for-sale-sign",
    "garage-lot-curb",
    "garage-window-frame",
)
HUD_IDS = (
    "icon-cash-era0",
    "icon-date-era0",
    "icon-era-bar",
    "icon-build-panel",
    "icon-hire-panel",
    "icon-train-panel",
    "icon-gpu-hire",
    "icon-week-advance",
    "panel-build-bg",
    "panel-hire-bg",
    "panel-train-bg",
    "hud-topbar-beige",
)
TEX_IDS = ("tex-hud-paper-warm", "tex-garage-wall-beige", "tex-garage-trim-orange")


def main() -> int:
    try:
        from PIL import Image  # noqa: F401
    except ImportError:
        print("FAIL: Pillow required for preview gates")
        return 1

    sys.path.insert(0, str(REPO / "tools" / "art"))
    from check_early_quality_previews import check_id  # noqa: E402

    rc = 0
    for asset_id in (*ENV_IDS, *HUD_IDS, *TEX_IDS):
        rc |= check_id(PREVIEW, asset_id)
    return rc


if __name__ == "__main__":
    sys.exit(main())