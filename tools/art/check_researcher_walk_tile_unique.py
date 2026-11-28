#!/usr/bin/env python3
"""FAIL if any two walk-strip tiles are pixel-identical (researcher)."""
from __future__ import annotations

import hashlib
import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-researcher-walk-strip.png"


def main() -> int:
    if not PNG.is_file():
        print("FAIL researcher_walk_tile_unique missing_png")
        return 1
    img = Image.open(PNG).convert("RGB")
    w, h = img.size
    if w < 256:
        print(f"FAIL researcher_walk_tile_unique strip_too_narrow {w}")
        return 1
    gap = 8
    tile_w = (w - gap * 3) // 4
    hashes: list[str] = []
    for i in range(4):
        x0 = i * (tile_w + gap)
        tile = img.crop((x0, 0, x0 + tile_w, h))
        hashes.append(hashlib.sha256(tile.tobytes()).hexdigest())
    for i in range(4):
        for j in range(i + 1, 4):
            if hashes[i] == hashes[j]:
                print(
                    f"FAIL researcher_walk_tile_unique tiles {i}=={j} sha={hashes[i][:12]}"
                )
                return 1
    print(f"OK researcher_walk_tile_unique tiles_distinct sha_prefix={[h[:8] for h in hashes]}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())