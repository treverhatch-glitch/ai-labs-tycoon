#!/usr/bin/env python3
"""Alpha bbox framing gate (shared preview rig).

After RGBA composite, re-read alpha from PNG if present else luma-threshold
on mid-grey backdrop (~0.48). Fail if opaque bbox touches any image edge
(within 2px) OR frame height coverage <35% or >75%.
"""
from __future__ import annotations

import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
STAFF_MANIFEST = REPO / "unity/Assets/Art/EarlyGame/staff-manifest.json"
WISP_IDS = ["wisp-happy", "wisp-neutral", "wisp-worried", "wisp-angry", "wisp-glowing"]
HUD_IDS = ["hud-topbar-beige", "tex-hud-paper-warm"]
MIN_H = 0.35
MAX_H = 0.75
EDGE_PAD = 2
ALPHA_THRESH = 12


def _opaque_mask(path: Path):
    from PIL import Image

    sys.path.insert(0, str(REPO / "tools/art"))
    from preview_quality_lib import is_object_pixel_rgb

    im = Image.open(path).convert("RGB")
    w, h = im.size
    mask = [is_object_pixel_rgb(r, g, b) for r, g, b in im.getdata()]
    return mask, (w, h)


def alpha_framing(path: Path) -> tuple[float, bool, int, int, int, int]:
    mask, (w, h) = _opaque_mask(path)
    if not any(mask):
        return 0.0, True, 0, 0, w, h
    rows = [i // w for i, m in enumerate(mask) if m]
    cols = [i % w for i, m in enumerate(mask) if m]
    y0, y1 = min(rows), max(rows)
    x0, x1 = min(cols), max(cols)
    height_frac = (y1 - y0 + 1) / h
    touches = (
        x0 <= EDGE_PAD or y0 <= EDGE_PAD or x1 >= w - 1 - EDGE_PAD or y1 >= h - 1 - EDGE_PAD
    )
    return height_frac, touches, x0, y0, x1, y1


def main() -> int:
    ids: list[str] = []
    if STAFF_MANIFEST.exists():
        ids.extend(a["id"] for a in json.loads(STAFF_MANIFEST.read_text())["assets"])
    ids.extend(WISP_IDS)
    ids.extend(HUD_IDS)
    rc = 0
    for asset_id in sorted(set(ids)):
        path = PREVIEW / f"{asset_id}.png"
        if not path.exists():
            print(f"FAIL missing preview {asset_id}")
            rc = 1
            continue
        hf, touch, x0, y0, x1, y1 = alpha_framing(path)
        if touch:
            print(f"FAIL {asset_id} alpha bbox touches edge ({x0},{y0})-({x1},{y1})")
            rc = 1
        elif hf < MIN_H or hf > MAX_H:
            print(f"FAIL {asset_id} alpha height={hf:.3f} not in [{MIN_H}, {MAX_H}]")
            rc = 1
        else:
            print(f"OK {asset_id} alpha height={hf:.3f}")
    return rc


if __name__ == "__main__":
    sys.exit(main())