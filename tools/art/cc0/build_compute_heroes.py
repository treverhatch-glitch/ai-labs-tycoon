#!/usr/bin/env python3
"""Invoke Blender bake scripts and write compute-manifest.json."""
from __future__ import annotations

import json
import os
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
BLENDER = os.environ.get("BLENDER_BIN", "blender")
SCRIPTS = [
    ROOT / "tools/art/cc0/blender/compute_pc_tower.py",
    ROOT / "tools/art/cc0/blender/compute_monitor_era1.py",
    ROOT / "tools/art/cc0/blender/compute_server_rack_era1.py",
]


def parse_json_tail(stdout: str) -> dict:
    marker = '"id": "compute-'
    pos = stdout.rfind(marker)
    if pos < 0:
        raise ValueError("no JSON in blender stdout")
    start = stdout.rfind("{", 0, pos)
    if start < 0:
        raise ValueError("no JSON object start in blender stdout")
    obj, _end = json.JSONDecoder().raw_decode(stdout[start:])
    return obj


def main() -> None:
    entries: list[dict] = []
    skip_tower = os.environ.get("CC0_SKIP_TOWER", "").strip() in ("1", "true", "yes")
    manifest_path = ROOT / "unity/Assets/Art/Cc0/compute-kit/compute-manifest.json"
    prior: dict[str, dict] = {}
    if skip_tower and manifest_path.is_file():
        data = json.loads(manifest_path.read_text(encoding="utf-8"))
        prior = {a["id"]: a for a in data.get("assets", [])}

    for script in SCRIPTS:
        if skip_tower and "compute_pc_tower" in script.name:
            entry = prior.get("compute-pc-tower-era1")
            if entry:
                entries.append(entry)
                print(f"==> skip tower (frozen); keep manifest {entry['id']}", flush=True)
                continue
        print(f"==> {script.relative_to(ROOT)}", flush=True)
        proc = subprocess.run(
            [BLENDER, "--background", "--python", str(script)],
            cwd=ROOT,
            capture_output=True,
            text=True,
        )
        if proc.returncode != 0:
            sys.stdout.write(proc.stdout)
            sys.stderr.write(proc.stderr)
            raise SystemExit(proc.returncode)
        entry = parse_json_tail(proc.stdout)
        entries.append(entry)
        print(f"    baked {entry['id']}", flush=True)

    manifest = {
        "lane": "compute-hardware-cc0",
        "fidelity": "cc0-restyle-bake",
        "assets": sorted(entries, key=lambda e: e["id"]),
    }
    out = ROOT / "unity/Assets/Art/Cc0/compute-kit/compute-manifest.json"
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    print(f"wrote {out.relative_to(ROOT)}")


if __name__ == "__main__":
    main()