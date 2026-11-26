#!/usr/bin/env python3
"""Reject previews with a bright floor plate (Hatch: mid-grey + contact shadow only)."""
from __future__ import annotations

import argparse
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
DEFAULT_IDS: list[str] = []


def check(path: Path, plate_frac: float = 0.08) -> tuple[bool, str]:
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
    if ratio > 0.35 or (uniform_share > 0.55 and uniform[0] > 220):
        return False, f"plate_bright={ratio:.2f} uniform={uniform_share:.2f}"
    return True, f"plate_ok bright={ratio:.2f}"


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
        ok, msg = check(p)
        if ok:
            print(f"OK {aid} {msg}")
        else:
            print(f"FAIL {aid} {msg}")
            rc = 1
    return rc


if __name__ == "__main__":
    sys.exit(main())