#!/usr/bin/env python3
"""Validate committed compute-kit CC0 hero outputs."""
from __future__ import annotations

import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
MANIFEST = ROOT / "unity/Assets/Art/Cc0/compute-kit/compute-manifest.json"
EXPECTED = {"compute-pc-tower-era1", "compute-monitor-era1", "compute-server-rack-era1"}


def main() -> int:
    if not MANIFEST.is_file():
        print(f"FAIL: missing {MANIFEST}", file=sys.stderr)
        return 1
    data = json.loads(MANIFEST.read_text(encoding="utf-8"))
    ids = {a["id"] for a in data.get("assets", [])}
    if ids != EXPECTED:
        print(f"FAIL: asset ids {sorted(ids)} != {sorted(EXPECTED)}", file=sys.stderr)
        return 1
    for asset in data["assets"]:
        aid = asset["id"]
        glb = ROOT / "unity/Assets/Art/Cc0/compute-kit" / aid / f"{aid}.glb"
        alb = ROOT / "unity/Assets/Art/Cc0/compute-kit" / aid / "textures" / f"{aid}_albedo.png"
        ao = ROOT / "unity/Assets/Art/Cc0/compute-kit" / aid / "textures" / f"{aid}_ao.png"
        prev = ROOT / asset["preview"]
        paths = [glb, alb, ao, prev]
        pf = asset.get("previewFront")
        if pf:
            paths.append(ROOT / pf)
        for path in paths:
            if not path.is_file():
                print(f"FAIL: missing {path.relative_to(ROOT)}", file=sys.stderr)
                return 1
    print("OK: compute-kit CC0 manifest + artifacts")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())