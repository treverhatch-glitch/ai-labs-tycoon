#!/usr/bin/env python3
"""Staff + wisp preview quality (shared rig metrics).

Uses tools/art/preview_quality_lib.py:
- Staff: object-mask mean saturation >= 0.15; floor 0.06 blocks gray blobs.
- Wisp: pastel body (object sat 0.06–0.13); mood accent sat >= 0.15.
- Distinctness: center-crop 32×32 mean RGB distance >= MIN_RGB_DISTANCE (14.0).
"""
from __future__ import annotations

import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
STAFF_MANIFEST = REPO / "unity/Assets/Art/EarlyGame/staff-manifest.json"
MIN_SAT_STAFF = 0.15
MIN_SAT_FLOOR = 0.06
MIN_LUMA_VAR_STAFF = 0.0054
MIN_LUMA_VAR_WISP = 0.0017
WISP_ACCENT_SAT = 0.15
WISP_BODY_SAT_MAX = 0.13
WISP_IDS = ["wisp-happy", "wisp-neutral", "wisp-worried", "wisp-angry", "wisp-glowing"]

sys.path.insert(0, str(REPO / "tools/art"))
from preview_quality_lib import (  # noqa: E402
    MIN_RGB_DISTANCE,
    downscale_rgb_crop,
    mean_saturation_object_mask,
    rgb_distance,
    rgb_distance_full,
    wisp_mood_accent_sat,
)


def main() -> int:
    ids: list[str] = []
    if STAFF_MANIFEST.exists():
        ids.extend(a["id"] for a in json.loads(STAFF_MANIFEST.read_text())["assets"])
    ids.extend(WISP_IDS)
    rc = 0
    signatures: dict[str, list[tuple[int, int, int]]] = {}
    for asset_id in ids:
        path = PREVIEW / f"{asset_id}.png"
        if not path.exists():
            print("FAIL missing preview", asset_id)
            rc = 1
            continue
        sat, lvar, _accent = mean_saturation_object_mask(path)
        if asset_id.startswith("staff-"):
            if sat < MIN_SAT_FLOOR:
                print(f"FAIL {asset_id} object_sat={sat:.3f} < floor {MIN_SAT_FLOOR}")
                rc = 1
            elif sat < MIN_SAT_STAFF:
                print(f"FAIL {asset_id} object_sat={sat:.3f} < {MIN_SAT_STAFF}")
                rc = 1
            elif lvar < MIN_LUMA_VAR_STAFF:
                print(f"FAIL {asset_id} flat frame luma_var={lvar:.4f}")
                rc = 1
            else:
                print(f"OK {asset_id} object_sat={sat:.3f} lvar={lvar:.4f}")
        elif asset_id.startswith("wisp-"):
            accent = wisp_mood_accent_sat(path)
            if sat < 0.02:
                print(f"FAIL {asset_id} wisp unreadable object_sat={sat:.3f}")
                rc = 1
            elif accent < WISP_ACCENT_SAT:
                print(f"FAIL {asset_id} mood accent_sat={accent:.3f} < {WISP_ACCENT_SAT}")
                rc = 1
            elif sat > WISP_BODY_SAT_MAX:
                print(f"FAIL {asset_id} body too saturated object_sat={sat:.3f} > {WISP_BODY_SAT_MAX}")
                rc = 1
            elif lvar < MIN_LUMA_VAR_WISP:
                print(f"FAIL {asset_id} flat frame luma_var={lvar:.4f}")
                rc = 1
            else:
                print(f"OK {asset_id} body_sat={sat:.3f} accent={accent:.3f} lvar={lvar:.4f}")
        signatures[asset_id] = downscale_rgb_crop(path)

    staff_ids = [i for i in signatures if i.startswith("staff-")]
    for i, a in enumerate(staff_ids):
        for b in staff_ids[i + 1 :]:
            dist = rgb_distance(signatures[a], signatures[b])
            if dist < MIN_RGB_DISTANCE:
                print(f"FAIL previews near-identical {a} vs {b} dist={dist:.2f} < {MIN_RGB_DISTANCE}")
                rc = 1
            else:
                print(f"OK distinct {a} vs {b} dist={dist:.2f}")

    wisp_only = [i for i in signatures if i.startswith("wisp-")]
    for i, a in enumerate(wisp_only):
        for b in wisp_only[i + 1 :]:
            dist = rgb_distance(signatures[a], signatures[b])
            if dist < MIN_RGB_DISTANCE:
                print(f"FAIL wisp near-identical {a} vs {b} dist={dist:.2f}")
                rc = 1
            else:
                print(f"OK wisp distinct {a} vs {b} dist={dist:.2f}")
    return rc


if __name__ == "__main__":
    sys.exit(main())