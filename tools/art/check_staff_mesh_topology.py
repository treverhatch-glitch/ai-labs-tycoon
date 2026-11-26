#!/usr/bin/env python3
"""Fail prepush when staff manifest reports >1 island or missing grounded export."""
from __future__ import annotations

import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
MANIFEST = REPO / "unity/Assets/Art/EarlyGame/staff-manifest.json"


def main() -> int:
    if not MANIFEST.exists():
        print("FAIL missing staff-manifest.json")
        return 1
    rc = 0
    for entry in json.loads(MANIFEST.read_text())["assets"]:
        asset_id = entry["id"]
        islands = entry.get("meshIslands")
        grounded = entry.get("grounded", True)
        glb = REPO / "unity" / "Assets" / "Art" / "EarlyGame" / "staff-kit" / f"{asset_id}.glb"
        if not glb.exists():
            print(f"FAIL missing {asset_id}.glb")
            rc = 1
            continue
        if islands is None:
            print(f"FAIL {asset_id} missing meshIslands in manifest")
            rc = 1
        elif islands > 1:
            print(f"FAIL {asset_id} islands={islands}")
            rc = 1
        elif not grounded:
            print(f"FAIL {asset_id} not grounded")
            rc = 1
        else:
            print(f"OK {asset_id} islands={islands}")
    return rc


if __name__ == "__main__":
    sys.exit(main())