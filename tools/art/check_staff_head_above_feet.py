#!/usr/bin/env python3
"""Head zone must sit above feet in staff preview (catches upside-down frames)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/staff-manifest.json"

SKIN_MIN = (150, 115, 95)
LEG_MAX = (90, 100, 140)


def _is_skin(r: int, g: int, b: int) -> bool:
    if r >= SKIN_MIN[0] and g >= SKIN_MIN[1] and b >= SKIN_MIN[2] and r > g > b * 0.85:
        return True
    return r > 175 and g > 105 and b < 110 and (r - b) > 40


def _is_head_tone(r: int, g: int, b: int) -> bool:
    if _is_skin(r, g, b):
        return True
    if r > 140 and g > 85 and b < 120 and (r - b) > 25:
        return True
    return r > 165 and g > 120 and b < 95


def _is_leg_blue(r: int, g: int, b: int) -> bool:
    if b < 95 or r > 95:
        return False
    return b > r + 18 and b > g + 5 and g >= 70


def analyze(asset_id: str) -> tuple[bool, str]:
    from PIL import Image

    rgb_path = PREVIEW / f"{asset_id}.png"
    mask_path = PREVIEW / f"{asset_id}.mask.png"
    if not rgb_path.is_file() or not mask_path.is_file():
        return False, "missing rgb/mask"
    rgb = Image.open(rgb_path).convert("RGB")
    mask = Image.open(mask_path).convert("RGBA")
    w, h = mask.size
    rp = list(rgb.getdata())
    mp = list(mask.getdata())
    ys: list[int] = []
    for y in range(h):
        for x in range(w):
            i = y * w + x
            if mp[i][3] >= 20:
                ys.append(y)
    if not ys:
        return False, "empty mask"
    y0, y1 = min(ys), max(ys)
    span = y1 - y0 + 1
    top_third = y0 + span // 3
    bot_third = y1 - span // 3
    top_skin = bot_skin = 0
    head_ys: list[int] = []
    leg_ys: list[int] = []
    for y in range(h):
        for x in range(w):
            i = y * w + x
            if mp[i][3] < 20:
                continue
            r, g, b = rp[i]
            if _is_head_tone(r, g, b):
                head_ys.append(y)
                if y <= top_third:
                    top_skin += 1
                if y >= bot_third:
                    bot_skin += 1
            if _is_leg_blue(r, g, b):
                leg_ys.append(y)
    if not head_ys:
        return False, "no_head_pixels"
    if not leg_ys:
        return False, "no_leg_pixels"
    mean_head = sum(head_ys) / len(head_ys)
    mean_leg = sum(leg_ys) / len(leg_ys)
    if mean_head >= mean_leg - 4:
        return False, f"head_below_feet mean_head={mean_head:.1f} mean_leg={mean_leg:.1f}"
    if top_skin < 45:
        return False, f"head_top={top_skin} need>=45"
    return True, f"top_skin={top_skin} mean_head={mean_head:.0f} mean_leg={mean_leg:.0f}"


def main() -> int:
    ids = [a["id"] for a in json.loads(MANIFEST.read_text())["assets"]]
    rc = 0
    for aid in ids:
        ok, msg = analyze(aid)
        if ok:
            print(f"OK {aid} {msg}")
        else:
            print(f"FAIL {aid} {msg}")
            rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())