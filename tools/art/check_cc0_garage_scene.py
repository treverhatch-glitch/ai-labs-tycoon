#!/usr/bin/env python3
"""Scene assembly gates: 90° walls, ≥6 props inside 4×4 m floor."""
from __future__ import annotations

import json
import math
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
GATES = REPO / "docs/art/previews/garage-interior-room-kit-scene-gates.json"
ROOM = 4.0


def _is_multiple_of_90(deg: float) -> bool:
    d = deg % 360.0
    if d > 180:
        d -= 360
    for target in (0.0, 90.0, -90.0, 180.0, -180.0, 270.0, -270.0):
        if abs(d - target) < 0.6:
            return True
    return False


def main() -> int:
    if not GATES.is_file():
        print("FAIL: missing scene gate metadata", GATES)
        return 1
    data = json.loads(GATES.read_text(encoding="utf-8"))
    errs: list[str] = []

    fb = data.get("floorBounds") or {}
    min_x = float(fb.get("minX", 0))
    max_x = float(fb.get("maxX", ROOM))
    min_z = float(fb.get("minZ", fb.get("minY", 0)))
    max_z = float(fb.get("maxZ", fb.get("maxY", ROOM)))

    for name, rot in (data.get("wallRotationsDeg") or {}).items():
        if isinstance(rot, dict):
            for axis, label in (("yDeg", "Y"), ("zDeg", "Z")):
                deg = float(rot.get(axis, 0))
                if not _is_multiple_of_90(deg):
                    errs.append(f"FAIL: wall {name} rotation {label}={deg}° not multiple of 90°")
        else:
            if not _is_multiple_of_90(float(rot)):
                errs.append(f"FAIL: wall {name} rotation {rot}° not multiple of 90°")

    props = data.get("propInstances") or []
    inside = [p for p in props if p.get("insideFloorBounds")]
    if len(inside) < int(data.get("minPropCount", 6)):
        errs.append(f"FAIL: props inside floor {len(inside)} < 6")
    for p in props:
        x = float(p.get("x", -1))
        z = float(p.get("z", p.get("y", -1)))
        if x < min_x or x > max_x or z < min_z or z > max_z:
            if p.get("insideFloorBounds"):
                errs.append(f"FAIL: prop {p.get('objectName')} marked inside but outside bounds")

    if errs:
        for e in errs:
            print(e)
        return 1
    print(f"OK: CC0 garage scene gates ({len(inside)} props in floor)")
    return 0


if __name__ == "__main__":
    sys.exit(main())