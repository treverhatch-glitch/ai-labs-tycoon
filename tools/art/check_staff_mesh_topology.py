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
        part_count = entry.get("partCount")
        tris = entry.get("triangleCount", 0)
        grounded = entry.get("grounded", True)
        glb = REPO / "unity" / "Assets" / "Art" / "EarlyGame" / "staff-kit" / f"{asset_id}.glb"
        if not glb.exists():
            print(f"FAIL missing {asset_id}.glb")
            rc = 1
            continue
        if islands is None:
            print(f"FAIL {asset_id} missing meshIslands in manifest")
            rc = 1
        elif islands is not None and islands > 48:
            print(f"FAIL {asset_id} islands={islands} need<=48")
            rc = 1
        elif part_count is not None and part_count < 6:
            print(f"FAIL {asset_id} partCount={part_count} need>=6")
            rc = 1
        elif tris < 1500:
            print(f"FAIL {asset_id} tris={tris} need>=1500")
            rc = 1
        elif not grounded:
            print(f"FAIL {asset_id} not grounded")
            rc = 1
        else:
            print(f"OK {asset_id} islands={islands} parts={part_count} tris={tris}")
    return rc


if __name__ == "__main__":
    sys.exit(main())