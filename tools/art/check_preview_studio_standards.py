#!/usr/bin/env python3
"""Studio gates: #969696 backdrop, >=40% frame fill, swatch hue means."""
from __future__ import annotations

import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
STUDIO_RGB = (150, 150, 150)
BACKDROP_TOL = 14
MIN_FRAME_FILL = 0.40
MAX_FRAME_FILL = 0.78

SWATCH_MEANS: dict[str, tuple[int, int, int]] = {
    "tex-garage-trim-orange": (215, 100, 42),
    "tex-garage-wall-beige": (210, 188, 158),
    "tex-hud-paper-warm": (244, 240, 232),
}
SWATCH_TOL = 38

GARAGE_MANIFEST = REPO / "unity/Assets/Art/EarlyGame/garage-manifest.json"
HUD_IDS = [
    "hud-topbar-beige",
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
]


def check_backdrop(path: Path) -> tuple[bool, str]:
    from PIL import Image

    im = Image.open(path).convert("RGB")
    w, h = im.size
    inset = max(4, w // 64)
    corners = [
        im.getpixel((inset, inset)),
        im.getpixel((w - 1 - inset, inset)),
        im.getpixel((inset, h - 1 - inset)),
        im.getpixel((w - 1 - inset, h - 1 - inset)),
    ]
    for i, px in enumerate(corners):
        if any(abs(px[c] - STUDIO_RGB[c]) > BACKDROP_TOL for c in range(3)):
            return False, f"corner{i}={px} need~{STUDIO_RGB}"
    return True, "backdrop_969696"


def check_fill(asset_id: str) -> tuple[bool, str]:
    mask = PREVIEW / f"{asset_id}.mask.png"
    if not mask.is_file():
        return False, "missing_mask"
    sys.path.insert(0, str(REPO / "tools/art"))
    from preview_quality_lib import analyze_alpha_mask
    alpha = analyze_alpha_mask(mask)
    extent = float(alpha.get("extent_coverage", 0.0))
    if extent < MIN_FRAME_FILL:
        return False, f"frame_fill={extent:.3f}<{MIN_FRAME_FILL}"
    if not asset_id.startswith(("icon-", "panel-", "hud-")) and extent > MAX_FRAME_FILL:
        return False, f"frame_fill={extent:.3f}>{MAX_FRAME_FILL}"
    return True, f"frame_fill={extent:.3f}"


def check_swatch(asset_id: str) -> tuple[bool, str]:
    target = SWATCH_MEANS.get(asset_id)
    if not target:
        return True, "skip"
    path = PREVIEW / f"{asset_id}.png"
    from PIL import Image

    im = Image.open(path).convert("RGB")
    w, h = im.size
    crop = im.crop((w // 4, h // 4, 3 * w // 4, 3 * h // 4))
    px = list(crop.getdata())
    if not px:
        return False, "empty"
    mr = sum(p[0] for p in px) / len(px)
    mg = sum(p[1] for p in px) / len(px)
    mb = sum(p[2] for p in px) / len(px)
    mean = (int(round(mr)), int(round(mg)), int(round(mb)))
    if (
        abs(mean[0] - target[0]) > SWATCH_TOL
        or abs(mean[1] - target[1]) > SWATCH_TOL
        or abs(mean[2] - target[2]) > SWATCH_TOL
    ):
        return False, f"mean={mean} want~{target}"
    return True, f"mean={mean}"


def main() -> int:
    assets = [a["id"] for a in json.loads(GARAGE_MANIFEST.read_text())["assets"]]
    assets += ["garage-scene-kit"]
    assets += HUD_IDS
    assets += list(SWATCH_MEANS.keys())
    rc = 0
    for asset_id in assets:
        path = PREVIEW / f"{asset_id}.png"
        if not path.is_file():
            print(f"FAIL {asset_id} missing_preview")
            rc = 1
            continue
        if asset_id.startswith("tex-"):
            msg = "garage_swatch_only"
            if asset_id.startswith("tex-hud-"):
                ok, msg = check_backdrop(path)
                if not ok:
                    print(f"FAIL {asset_id} backdrop {msg}")
                    rc = 1
                    continue
            if asset_id in SWATCH_MEANS:
                ok2, msg2 = check_swatch(asset_id)
                if not ok2:
                    print(f"FAIL {asset_id} swatch {msg2}")
                    rc = 1
                    continue
                print(f"OK {asset_id} {msg} {msg2}")
            else:
                print(f"OK {asset_id} {msg}")
            continue
        ok, msg = check_backdrop(path)
        if not ok:
            print(f"FAIL {asset_id} backdrop {msg}")
            rc = 1
            continue
        if not asset_id.startswith("wisp-"):
            ok3, msg3 = check_fill(asset_id)
            if not ok3:
                print(f"FAIL {asset_id} {msg3}")
                rc = 1
                continue
            print(f"OK {asset_id} {msg} {msg3}")
        else:
            print(f"OK {asset_id} {msg} wisp_exempt_fill")
    return rc


if __name__ == "__main__":
    sys.exit(main())