#!/usr/bin/env python3
"""Plain white sign board albedo (FOR SALE is mesh text in Blender, not painted here)."""
from __future__ import annotations

import sys
from pathlib import Path


def paint(path: Path, *, min_unique: int = 1, min_bytes: int = 2_500) -> None:
    from PIL import Image

    path.parent.mkdir(parents=True, exist_ok=True)
    im = Image.new("RGB", (1024, 1024), (252, 252, 250))
    uniq = len({px for px in im.getdata()})
    if uniq < min_unique:
        raise SystemExit(f"sign albedo flat unique={uniq}")
    im.save(path)
    if path.stat().st_size < min_bytes:
        raise SystemExit(f"sign albedo too small bytes={path.stat().st_size}")


def main() -> int:
    if len(sys.argv) < 2:
        print("usage: paint_sign_albedo.py <albedo.png>", file=sys.stderr)
        return 1
    paint(Path(sys.argv[1]))
    print("OK", sys.argv[1])
    return 0


if __name__ == "__main__":
    raise SystemExit(main())