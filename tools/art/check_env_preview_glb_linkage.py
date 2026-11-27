#!/usr/bin/env python3
"""Fail when packet34 env preview frame.json glb_sha256 != committed GLB file."""
from __future__ import annotations

import hashlib
import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs" / "art" / "previews"
GLB_DIR = REPO / "unity/Assets/Art/EarlyGame/garage-kit"

ENV_IDS = (
    "garage-exterior-wall",
    "garage-land-for-sale-sign",
    "garage-lot-curb",
    "garage-window-frame",
)


def sha256_file(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b""):
            h.update(chunk)
    return h.hexdigest()


def check_asset(asset_id: str) -> list[str]:
    frame = PREVIEW / f"{asset_id}.frame.json"
    glb = GLB_DIR / f"{asset_id}.glb"
    errs: list[str] = []
    if not frame.is_file():
        return [f"missing_frame_json"]
    if not glb.is_file():
        return [f"missing_glb"]
    meta = json.loads(frame.read_text(encoding="utf-8"))
    recorded = meta.get("glb_sha256")
    if not recorded:
        return ["missing_glb_sha256_in_frame"]
    actual = sha256_file(glb)
    if recorded != actual:
        errs.append(f"glb_sha256_mismatch frame={recorded[:12]} glb={actual[:12]}")
    recorded_path = meta.get("glb_path", "")
    if recorded_path and recorded_path != f"unity/Assets/Art/EarlyGame/garage-kit/{asset_id}.glb":
        errs.append(f"glb_path_unexpected {recorded_path}")
    return errs


def main() -> int:
    rc = 0
    for asset_id in ENV_IDS:
        errs = check_asset(asset_id)
        if errs:
            for err in errs:
                print(f"FAIL {asset_id} preview_link {err}")
            rc = 1
        else:
            print(f"OK {asset_id} preview_glb_linkage")
    return rc


if __name__ == "__main__":
    sys.exit(main())