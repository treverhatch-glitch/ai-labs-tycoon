#!/usr/bin/env python3
"""Reject previews with a bright floor plate (Hatch: mid-grey + contact shadow only)."""
from __future__ import annotations

import argparse
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
DEFAULT_IDS: list[str] = []


def check(path: Path, asset_id: str = "", plate_frac: float = 0.08) -> tuple[bool, str]:
    from PIL import Image

    im = Image.open(path).convert("RGB")
    w, h = im.size
    rows = max(4, int(h * plate_frac))
    plate = im.crop((0, h - rows, w, h))
    px = list(plate.getdata())
    bright = sum(1 for r, g, b in px if r > 230 and g > 228 and b > 226)
    ratio = bright / len(px)
    uniform = max(px, key=lambda t: px.count(t))
    uniform_share = px.count(uniform) / len(px)
    col_w = max(8, w // 5)
    col_x0 = (w - col_w) // 2
    mid = im.crop((col_x0, int(h * 0.15), col_x0 + col_w, int(h * 0.85)))
    mid_px = list(mid.getdata())
    mid_bright = sum(1 for r, g, b in mid_px if r > 225 and g > 223 and b > 220)
    mid_ratio = mid_bright / max(1, len(mid_px))
    if ratio > 0.35 or (uniform_share > 0.55 and uniform[0] > 220):
        return False, f"floor_plate_bright={ratio:.2f} uniform={uniform_share:.2f}"
    if asset_id.startswith("staff-") and mid_ratio > 0.28:
        return False, f"vertical_backdrop={mid_ratio:.2f}"
    return True, f"plate_ok bright={ratio:.2f} backdrop={mid_ratio:.2f}"


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("asset_ids", nargs="*", help="preview stems")
    args = ap.parse_args()
    if not args.asset_ids:
        import json

        staff = json.loads((REPO / "unity/Assets/Art/EarlyGame/staff-manifest.json").read_text())
        args.asset_ids = [a["id"] for a in staff["assets"]]
        args.asset_ids += [
            "wisp-happy",
            "wisp-neutral",
            "wisp-worried",
            "wisp-angry",
            "wisp-glowing",
        ]
    rc = 0
    prev_dir = REPO / "docs/art/previews"
    for aid in args.asset_ids:
        p = prev_dir / f"{aid}.png"
        if not p.is_file():
            print(f"FAIL missing {aid}.png")
            rc = 1
            continue
        ok, msg = check(p, aid)
        if ok:
            print(f"OK {aid} {msg}")
        else:
            print(f"FAIL {aid} {msg}")
            rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())