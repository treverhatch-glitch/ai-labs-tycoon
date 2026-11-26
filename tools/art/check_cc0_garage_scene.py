#!/usr/bin/env python3
"""Placement gates: grounded props, wall corner gap, 4×4 footprint."""
from __future__ import annotations

import json
import math
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
GATES = REPO / "docs/art/previews/garage-interior-room-kit-scene-gates.json"
FIXTURE_4E = REPO / "docs/art/fixtures/garage-interior-room-kit-scene-gates-owner-fail-4e22d8dd.json"
ROOM = 4.0


def placement_gate_errs(data: dict, *, label: str = "") -> list[str]:
    prefix = f"{label} " if label else ""
    errs: list[str] = []
    tol = float(data.get("groundToleranceM", 0.01))
    wall_tol = float(data.get("wallMountToleranceM", 0.02))
    corner_gap = float(data.get("wallCornerGapM", 99.0))
    if corner_gap >= 0.01:
        errs.append(f"FAIL: {prefix}wall corner gap {corner_gap * 100:.1f} cm >= 1 cm")

    fb = data.get("floorBounds") or {}
    min_x = float(fb.get("minX", 0))
    max_x = float(fb.get("maxX", ROOM))
    min_y = float(fb.get("minY", fb.get("minZ", 0)))
    max_y = float(fb.get("maxY", fb.get("maxZ", ROOM)))

    props = data.get("propInstances") or []
    grounded_ok = [p for p in props if p.get("grounded")]
    floor_props = [p for p in props if p.get("objectName", "").startswith("prop_")]
    if len(grounded_ok) < int(data.get("minPropCount", 6)):
        errs.append(f"FAIL: {prefix}grounded instances {len(grounded_ok)} < 6")

    for p in props:
        name = p.get("objectName", "?")
        gc = float(p.get("groundClearanceM", 99))
        if name.startswith("prop_") and abs(gc) > tol:
            errs.append(f"FAIL: {prefix}{name} ground clearance {gc * 100:.1f} cm > 1 cm")
        if not p.get("insideFloorBounds", False):
            errs.append(f"FAIL: {prefix}{name} outside 4×4 floor bounds")
        x = float(p.get("x", -1))
        y = float(p.get("y", p.get("z", -1)))
        if x < min_x - tol or x > max_x + tol or y < min_y - tol or y > max_y + tol:
            errs.append(f"FAIL: {prefix}{name} center outside footprint")

        if name == "prop_bookcase" and float(p.get("distToLeftWallM", 9)) > wall_tol * 2.5:
            errs.append(f"FAIL: {prefix}{name} not flush to left wall")
        if name == "kit_roll_door" and float(p.get("distToBackWallM", 9)) > wall_tol * 4.0:
            errs.append(f"FAIL: {prefix}{name} not in back wall opening")
        if name == "kit_roll_door" and float(p.get("groundClearanceM", 9)) > tol:
            errs.append(f"FAIL: {prefix}{name} not on slab")

    return errs


def main() -> int:
    if not GATES.is_file():
        print("FAIL: missing scene gate metadata", GATES)
        return 1
    data = json.loads(GATES.read_text(encoding="utf-8"))
    errs = placement_gate_errs(data)
    if FIXTURE_4E.is_file():
        bad = json.loads(FIXTURE_4E.read_text(encoding="utf-8"))
        if not placement_gate_errs(bad, label="fixture-4e22d8dd"):
            errs.append("FAIL: fixture 4e22d8dd placement gates must fail")
    if errs:
        for e in errs:
            print(e)
        return 1
    print("OK: CC0 garage placement gates")
    return 0


if __name__ == "__main__":
    sys.exit(main())