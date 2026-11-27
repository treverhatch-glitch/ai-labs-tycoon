#!/usr/bin/env python3
"""Turnaround PNGs exist and backdrop is #969696."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
GREY = (150, 150, 150)


def _check(path: Path, name: str) -> tuple[bool, str]:
    if not path.is_file() or path.stat().st_size < 4000:
        return False, f"missing {name}"
    img = Image.open(path).convert("RGB")
    w, h = img.size
    if w < h * 3:
        return False, f"layout {w}x{h}"
    gap = 8
    panel_w = (w - gap * 4) // 5
    if panel_w < 32:
        return False, f"panel_w={panel_w}"
    samples = [4]
    for i in range(4):
        samples.append(i * (panel_w + gap) + panel_w + gap // 2)
    for x in samples:
        r, g, b = img.getpixel((min(x, w - 1), h // 2))
        if abs(r - GREY[0]) > 12 or abs(g - GREY[1]) > 12 or abs(b - GREY[2]) > 12:
            return False, f"backdrop_grey at_x={x} rgb=({r},{g},{b})"
    return True, f"ok {w}x{h}"


def main() -> int:
    rc = 0
    for stem in ("staff-founder-turnaround", "staff-researcher-turnaround"):
        ok, msg = _check(REPO / "docs/art/previews" / f"{stem}.png", stem)
        tag = "OK" if ok else "FAIL"
        asset = "staff-founder-idle" if "founder" in stem else "staff-researcher-idle"
        print(f"{tag} {asset} turnaround {msg}")
        if not ok:
            rc = 1
    return rc


if __name__ == "__main__":
    raise SystemExit(main())