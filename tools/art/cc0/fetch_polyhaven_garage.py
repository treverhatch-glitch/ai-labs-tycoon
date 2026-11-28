#!/usr/bin/env python3
"""Download 1k GLTF packs for garage CC0 hero map (Poly Haven API)."""
from __future__ import annotations

import json
import ssl
import urllib.request
from pathlib import Path

REPO = Path(__file__).resolve().parents[3]
MAP = Path(__file__).resolve().parent / "garage_cc0_map.json"
OUT = REPO / "third_party/cc0/polyhaven"
UA = "ai-labs-tycoon-art/1.0 (CC0 fetch; contact: in-repo LICENSES.md)"


def api_get(url: str) -> dict:
    req = urllib.request.Request(url, headers={"User-Agent": UA})
    with urllib.request.urlopen(req, context=ssl.create_default_context(), timeout=120) as resp:
        return json.loads(resp.read().decode())


def download(url: str, dest: Path) -> None:
    dest.parent.mkdir(parents=True, exist_ok=True)
    req = urllib.request.Request(url, headers={"User-Agent": UA})
    with urllib.request.urlopen(req, context=ssl.create_default_context(), timeout=300) as resp:
        dest.write_bytes(resp.read())


def fetch_pack(polyhaven_id: str) -> Path:
    files = api_get(f"https://api.polyhaven.com/files/{polyhaven_id}")
    pick = None
    for res in ("1k", "2k", "4k"):
        gltf = files.get("gltf", {}).get(res, {}).get("gltf")
        if gltf:
            pick = gltf
            break
    if not pick:
        raise RuntimeError(f"{polyhaven_id}: no gltf pack in API")
    pack_dir = OUT / polyhaven_id
    pack_dir.mkdir(parents=True, exist_ok=True)
    includes = pick.get("include") or {}
    for rel, spec in includes.items():
        download(spec["url"], pack_dir / rel)
    main_url = pick["url"]
    main_name = Path(main_url.split("/")[-1]).name
    download(main_url, pack_dir / main_name)
    (pack_dir / "entry.gltf").write_text(main_name + "\n", encoding="utf-8")
    return pack_dir / main_name


def main() -> int:
    data = json.loads(MAP.read_text(encoding="utf-8"))
    rc = 0
    for row in data["assets"]:
        ph = row["polyhavenId"]
        try:
            path = fetch_pack(ph)
            print(f"OK {row['id']} <- {ph} ({path.relative_to(REPO)})")
        except Exception as exc:  # noqa: BLE001
            print(f"FAIL {row['id']} {ph}: {exc}")
            rc = 1
    return rc


if __name__ == "__main__":
    raise SystemExit(main())