#!/usr/bin/env python3
"""Unity stock reference GLB must match pinned Kenney kit sha256."""
from __future__ import annotations

import hashlib
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
KIT = REPO / "tools/art/source/kenney/staff-founder-kenney-stock-kit.glb"
KIT_SHA = REPO / "tools/art/source/kenney/staff-founder-kenney-stock-kit.glb.sha256"
UNITY = REPO / "unity/Assets/Art/EarlyGame/staff-kit/staff-founder-kenney-stock.glb"


def _want() -> str:
    if KIT_SHA.is_file():
        return KIT_SHA.read_text(encoding="utf-8").strip().split()[0]
    return hashlib.sha256(KIT.read_bytes()).hexdigest()


def main() -> int:
    want = _want()
    if not KIT.is_file():
        print("FAIL staff-founder-idle kit_glb missing")
        return 1
    kit = hashlib.sha256(KIT.read_bytes()).hexdigest()
    if kit != want:
        print(f"FAIL staff-founder-idle kit_glb sha kit={kit} want={want}")
        return 1
    if not UNITY.is_file():
        print("FAIL staff-founder-idle unity_stock_glb missing")
        return 1
    unity = hashlib.sha256(UNITY.read_bytes()).hexdigest()
    if unity != want:
        print(f"FAIL staff-founder-idle unity_stock_glb sha={unity} want={want}")
        return 1
    print(f"OK staff-founder-idle kit_glb sha256={want[:12]}…")
    return 0


if __name__ == "__main__":
    sys.exit(main())