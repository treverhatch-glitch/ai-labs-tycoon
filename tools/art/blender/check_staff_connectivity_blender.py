"""Headless connectivity gate on imported staff GLBs."""
from __future__ import annotations

import json
import sys
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
import staff_connectivity as sc  # noqa: E402

repo = Path(sys.argv[-1])
manifest = json.loads((repo / "unity/Assets/Art/EarlyGame/staff-manifest.json").read_text())
rc = 0
for entry in manifest["assets"]:
    aid = entry["id"]
    glb = repo / "unity/Assets/Art/EarlyGame/staff-kit" / f"{aid}.glb"
    if not glb.is_file():
        print(f"FAIL {aid} missing glb")
        rc = 1
        continue
    ok, msg = sc.check_gltf_connectivity(str(glb), aid)
    if ok:
        print(f"OK {aid} {msg}")
    else:
        print(f"FAIL {aid} {msg}")
        rc = 1
raise SystemExit(rc)