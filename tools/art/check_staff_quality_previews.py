#!/usr/bin/env python3
"""Packet 2 distinctness gate (sat/alpha via check_early_quality_previews)."""
from __future__ import annotations

import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
STAFF_MANIFEST = REPO / "unity/Assets/Art/EarlyGame/staff-manifest.json"
WISP_IDS = ["wisp-happy", "wisp-neutral", "wisp-worried", "wisp-angry", "wisp-glowing"]

sys.path.insert(0, str(REPO / "tools/art"))
from preview_quality_lib import (  # noqa: E402
    MIN_RGB_DISTANCE,
    downscale_rgb_crop,
    masked_mean_rgb,
    mean_rgb_distance,
    rgb_distance,
)


def main() -> int:
    ids: list[str] = []
    if STAFF_MANIFEST.exists():
        ids.extend(a["id"] for a in json.loads(STAFF_MANIFEST.read_text())["assets"])
    ids.extend(WISP_IDS)
    signatures: dict[str, list[tuple[int, int, int]]] = {}
    for asset_id in ids:
        path = PREVIEW / f"{asset_id}.png"
        if not path.exists():
            print("FAIL missing preview", asset_id)
            return 1
        signatures[asset_id] = (masked_mean_rgb(path), downscale_rgb_crop(path, size=32))

    rc = 0
    staff_ids = [i for i in signatures if i.startswith("staff-")]
    for i, a in enumerate(staff_ids):
        for b in staff_ids[i + 1 :]:
            ma, sa = signatures[a]
            mb, sb = signatures[b]
            dist = max(mean_rgb_distance(ma, mb), rgb_distance(sa, sb) * 18.0)
            if dist < MIN_RGB_DISTANCE:
                print(f"FAIL previews near-identical {a} vs {b} dist={dist:.2f} < {MIN_RGB_DISTANCE}")
                rc = 1
            else:
                print(f"OK distinct {a} vs {b} dist={dist:.2f}")

    wisp_only = [i for i in signatures if i.startswith("wisp-")]
    for i, a in enumerate(wisp_only):
        for b in wisp_only[i + 1 :]:
            ma, sa = signatures[a]
            mb, sb = signatures[b]
            dist = max(mean_rgb_distance(ma, mb), rgb_distance(sa, sb) * 18.0)
            if dist < MIN_RGB_DISTANCE:
                print(f"FAIL wisp near-identical {a} vs {b} dist={dist:.2f}")
                rc = 1
            else:
                print(f"OK wisp distinct {a} vs {b} dist={dist:.2f}")
    return rc


if __name__ == "__main__":
    sys.exit(main())