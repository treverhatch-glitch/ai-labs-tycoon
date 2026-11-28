#!/usr/bin/env python3
"""Staff RGB/mask previews must be taller than wide (upright silhouette, not lying on side)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/staff-manifest.json"

sys.path.insert(0, str(REPO / "tools" / "art"))
from preview_quality_lib import analyze_alpha_mask  # noqa: E402


def main() -> int:
    ids = [a["id"] for a in json.loads(MANIFEST.read_text())["assets"]]
    rc = 0
    for aid in ids:
        mask = PREVIEW / f"{aid}.mask.png"
        if not mask.is_file():
            print(f"FAIL {aid} missing mask")
            rc = 1
            continue
        alpha = analyze_alpha_mask(mask)
        h = float(alpha["height_coverage"])
        w = float(alpha["width_coverage"])
        ext = float(alpha["extent_coverage"])
        if h < 0.18 or ext < 0.30:
            print(f"FAIL {aid} preview_too_short h={h:.3f} extent={ext:.3f}")
            rc = 1
        else:
            print(f"OK {aid} preview_upright h={h:.3f} w={w:.3f}")
    return rc


if __name__ == "__main__":
    sys.exit(main())