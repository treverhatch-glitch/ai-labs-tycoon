#!/usr/bin/env python3
"""Record pinned CC0 incoming paths and SHA-256."""
from __future__ import annotations

import hashlib
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
INCOMING = ROOT / "tools" / "art" / "cc0" / "incoming"

PATHS = [
    INCOMING / "kenney/animated-characters-3.zip",
    INCOMING / "kenney/animated-characters-3/Model/characterMedium.fbx",
    INCOMING / "quaternius/animated_human.glb",
    INCOMING / "quaternius/character.glb",
    INCOMING / "polyhaven/studio_small_08_2k.hdr",
]


def sha256(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as fp:
        for block in iter(lambda: fp.read(1 << 20), b""):
            h.update(block)
    return h.hexdigest()


def main() -> None:
    files = []
    for path in PATHS:
        if not path.is_file():
            raise SystemExit(f"missing {path}")
        rel = path.relative_to(ROOT).as_posix()
        files.append({"path": rel, "bytes": path.stat().st_size, "sha256": sha256(path)})
    manifest = {
        "schema": 1,
        "license": "CC0-1.0",
        "files": files,
    }
    out = INCOMING.parent / "manifest.json"
    out.write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    print(f"OK {out} ({len(files)} files)")


if __name__ == "__main__":
    main()