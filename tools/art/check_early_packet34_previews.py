#!/usr/bin/env python3
"""Packet 3–4: alpha framing, object-mask sat, near-duplicate guard."""
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
    from early_art_preview_gates import (  # noqa: WPS433
        check_near_duplicates,
        gate_preview_env,
        gate_preview_hud,
    )

    rc = 0
    for asset_id in ENV_IDS:
        path = PREVIEW / f"{asset_id}.png"
        if not path.exists():
            print(f"FAIL missing preview {asset_id}")
            rc = 1
            continue
        ok, detail = gate_preview_env(path)
        print(f"{'OK' if ok else 'FAIL'} {asset_id} {detail}")
        rc |= 0 if ok else 1

    for asset_id in (*HUD_IDS, *TEX_IDS):
        path = PREVIEW / f"{asset_id}.png"
        if not path.exists():
            print(f"FAIL missing preview {asset_id}")
            rc = 1
            continue
        ok, detail = gate_preview_hud(path)
        print(f"{'OK' if ok else 'FAIL'} {asset_id} {detail}")
        rc |= 0 if ok else 1

    dup_rc, dup_msgs = check_near_duplicates(PREVIEW, ENV_IDS, hud=False)
    rc |= dup_rc
    for line in dup_msgs:
        print(line)
    dup_rc, dup_msgs = check_near_duplicates(PREVIEW, HUD_IDS, hud=True)
    rc |= dup_rc
    for line in dup_msgs:
        print(line)
    return rc


if __name__ == "__main__":
    sys.exit(main())