#!/usr/bin/env python3
"""Placement gates: grounded props, roll door width, 8+ real-scale props."""
from __future__ import annotations

import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
GATES = REPO / "docs/art/previews/garage-interior-room-kit-scene-gates.json"
FIXTURE_4E = REPO / "docs/art/fixtures/garage-interior-room-kit-scene-gates-owner-fail-4e22d8dd.json"
ROOM = 4.0
THIN_PROP_MAX_M = 0.12


def placement_gate_errs(data: dict, *, label: str = "") -> list[str]:
    prefix = f"{label} " if label else ""
    errs: list[str] = []
    tol = float(data.get("groundToleranceM", 0.01))
    corner_gap = float(data.get("wallCornerGapM", 99.0))
    if corner_gap >= 0.01:
        errs.append(f"FAIL: {prefix}wall corner gap {corner_gap * 100:.1f} cm >= 1 cm")

    wall_h = float(data.get("wallHeightM", 2.7))
    door_name = str(data.get("rollDoorObjectName", ""))
    door_w = float(data.get("rollDoorWidthM", 0))
    if door_name != "kit_roll_door":
        errs.append(f"FAIL: {prefix}roll door name {door_name!r} != kit_roll_door")
    if door_w < 2.5:
        errs.append(f"FAIL: {prefix}roll door width {door_w:.2f} m < 2.5 m")
    door_min_z = float(data.get("rollDoorMinZM", 99.0))
    if door_min_z > 0.02:
        errs.append(f"FAIL: {prefix}roll door minZ {door_min_z:.3f} m > 0.02")
    if data.get("rollDoorSingleMesh") is not True:
        errs.append(f"FAIL: {prefix}roll door must be single merged mesh")
    lid_gaps = data.get("cardboardLidGapM") or {}
    for key, gap in lid_gaps.items():
        g = float(gap)
        if g > tol:
            errs.append(f"FAIL: {prefix}{key} cardboard lid mesh gap {g * 100:.2f} cm > 1 cm")
    kenney = data.get("kenneyFurnitureGlb") or {}
    for req in ("prop_desk", "prop_chair", "prop_lamp", "prop_box_a"):
        glb = kenney.get(req)
        if not glb or not str(glb).endswith(".glb"):
            errs.append(f"FAIL: {prefix}missing kenneyFurnitureGlb[{req!r}]")
    door_max_y = float(data.get("rollDoorMaxYM", 99.0))
    wall_plane = float(data.get("rollDoorWallPlaneYM", 0.05))
    if door_max_y > wall_plane + 0.05:
        errs.append(f"FAIL: {prefix}roll door maxY {door_max_y:.3f} > wall plane + 5 cm")

    props = data.get("propInstances") or []
    if len(props) < int(data.get("minPropCount", 8)):
        errs.append(f"FAIL: {prefix}prop count {len(props)} < 8")

    fb = data.get("floorBounds") or {}
    min_x = float(fb.get("minX", 0))
    max_x = float(fb.get("maxX", ROOM))
    min_y = float(fb.get("minY", fb.get("minZ", 0)))
    max_y = float(fb.get("maxY", fb.get("maxZ", ROOM)))

    for p in props:
        name = p.get("objectName", "?")
        support = str(p.get("support", "floor"))
        if support == "desk":
            gap_m = float(p.get("supportContactGapM", 99.0))
            if gap_m > tol:
                errs.append(f"FAIL: {prefix}{name} desk support gap {gap_m * 100:.1f} cm > 1 cm")
        else:
            gap_m = float(p.get("floorContactGapM", p.get("supportContactGapM", 99.0)))
            if gap_m > tol:
                errs.append(f"FAIL: {prefix}{name} floor gap {gap_m * 100:.1f} cm > 1 cm")
        if not p.get("grounded"):
            errs.append(f"FAIL: {prefix}{name} not grounded")
        if not p.get("insideFloorBounds", True):
            errs.append(f"FAIL: {prefix}{name} outside floor bounds")
        x = float(p.get("x", -1))
        y = float(p.get("y", p.get("z", -1)))
        if x < min_x - tol or x > max_x + tol or y < min_y - tol or y > max_y + tol:
            errs.append(f"FAIL: {prefix}{name} center outside footprint")
        ratio = float(p.get("heightToWallRatio", 0))
        height_m = float(p.get("heightM", 0))
        if name == "prop_rug":
            if height_m > THIN_PROP_MAX_M:
                errs.append(f"FAIL: {prefix}{name} rug thickness {height_m:.2f} m > {THIN_PROP_MAX_M}")
        elif ratio <= 0.02 or ratio >= 1.05:
            errs.append(f"FAIL: {prefix}{name} height/wall ratio {ratio:.2f} out of range")
        if wall_h and float(p.get("heightM", 0)) > wall_h:
            errs.append(f"FAIL: {prefix}{name} taller than wall")

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