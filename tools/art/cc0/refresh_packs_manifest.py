#!/usr/bin/env python3
"""Record sha256 of CC0 archives under tools/art/cc0/incoming/archives/."""
from __future__ import annotations

import hashlib
import json
from datetime import datetime, timezone
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
ARCH = ROOT / "tools/art/cc0/incoming/archives"
MANIFEST = ROOT / "tools/art/cc0/packs.manifest.json"

PACKS = [
    {
        "id": "kenney-furniture-kit",
        "author": "Kenney",
        "license": "CC0-1.0",
        "pageUrl": "https://kenney.nl/assets/furniture-kit",
        "archive": "kenney_furniturePack.zip",
        "mirrorUrl": "https://opengameart.org/content/furniture-kit",
    },
    {
        "id": "kenney-factory-kit",
        "author": "Kenney",
        "license": "CC0-1.0",
        "pageUrl": "https://kenney.nl/assets/factory-kit",
        "archive": "kenney_factory-kit_3.0.zip",
        "mirrorUrl": "https://opengameart.org/content/factory-kit",
    },
    {
        "id": "quaternius-scifi-megakit",
        "author": "Quaternius",
        "license": "CC0-1.0",
        "pageUrl": "https://quaternius.com/packs/modularscifimegakit.html",
        "archive": "modular_scifi_megakitstandard.zip",
        "mirrorUrl": "https://opengameart.org/content/modular-sci-fi-megakit",
    },
    {
        "id": "polyhaven-studio-small-09",
        "author": "Poly Haven",
        "license": "CC0-1.0",
        "pageUrl": "https://polyhaven.com/a/studio_small_09",
        "archive": "studio_small_09_1k.hdr",
        "mirrorUrl": "https://dl.polyhaven.org/file/ph-assets/HDRIs/hdr/1k/studio_small_09_1k.hdr",
    },
    {
        "id": "ambientcg-metal048a",
        "author": "ambientCG",
        "license": "CC0-1.0",
        "pageUrl": "https://ambientcg.com/view?id=Metal048A",
        "archive": "Metal048A_1K-JPG.zip",
        "mirrorUrl": "https://ambientcg.com/get?file=Metal048A_1K-JPG.zip",
    },
]


def sha256(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as f:
        for chunk in iter(lambda: f.read(1 << 20), b""):
            h.update(chunk)
    return h.hexdigest()


def main() -> None:
    entries = []
    for pack in PACKS:
        path = ARCH / pack["archive"]
        if not path.is_file():
            raise SystemExit(f"missing archive: {path}")
        entries.append({**pack, "sha256": sha256(path), "bytes": path.stat().st_size})
    payload = {
        "updatedAt": datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
        "incomingRoot": "tools/art/cc0/incoming",
        "packs": entries,
    }
    MANIFEST.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")
    print(f"wrote {MANIFEST.relative_to(ROOT)} ({len(entries)} packs)")


if __name__ == "__main__":
    main()