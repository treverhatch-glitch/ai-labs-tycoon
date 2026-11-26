#!/usr/bin/env python3
"""Fail near-uniform preview PNGs (Hatch art bar)."""
from __future__ import annotations

import hashlib
import json
import struct
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW_DIR = REPO / "docs" / "art" / "previews"
BLENDER_ROOT = REPO / "unity" / "Assets" / "Art" / "BlenderBaked"
MIN_STDDEV = 8.0
MIN_COVERAGE = 0.20


def _read_png_pixels(path: Path) -> list[int]:
    """Decode RGB8 from PNG without Pillow (stdlib-only)."""
    import zlib

    data = path.read_bytes()
    if data[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError(f"not PNG: {path}")
    pos = 8
    width = height = None
    raw = b""
    while pos < len(data):
        length = struct.unpack(">I", data[pos : pos + 4])[0]
        pos += 4
        ctype = data[pos : pos + 4]
        pos += 4
        chunk = data[pos : pos + length]
        pos += length + 4
        if ctype == b"IHDR":
            width, height = struct.unpack(">II", chunk[:8])
        elif ctype == b"IDAT":
            raw += chunk
        elif ctype == b"IEND":
            break
    if width is None or height is None:
        raise ValueError(f"no IHDR: {path}")
    inflated = zlib.decompress(raw)
    stride = width * 3 + 1
    lum: list[int] = []
    for y in range(height):
        row = inflated[y * stride : (y + 1) * stride]
        filt = row[0]
        if filt != 0:
            continue
        for x in range(width):
            r, g, b = row[1 + x * 3 : 1 + x * 3 + 3]
            lum.append((r * 299 + g * 587 + b * 114) // 1000)
    return lum


def stats(path: Path) -> tuple[float, float]:
    lum = _read_png_pixels(path)
    if not lum:
        return 0.0, 0.0
    n = len(lum)
    mean = sum(lum) / n
    var = sum((x - mean) ** 2 for x in lum) / n
    std = var**0.5
    coverage = sum(1 for x in lum if x > 24) / n
    return std, coverage


def glb_sha256(asset_id: str) -> str | None:
    glb = BLENDER_ROOT / asset_id / f"mesh_{asset_id}.glb"
    if not glb.is_file():
        glb = BLENDER_ROOT / asset_id / f"{asset_id}.glb"
    if not glb.is_file():
        return None
    return hashlib.sha256(glb.read_bytes()).hexdigest()


def write_frame_json(preview: Path, asset_id: str) -> None:
    sha = glb_sha256(asset_id)
    if not sha:
        return
    frame = PREVIEW_DIR / f"{asset_id}.frame.json"
    frame.write_text(
        json.dumps(
            {
                "assetId": asset_id,
                "preview": str(preview.relative_to(REPO)),
                "glb_sha256": sha,
                "resolution": 1024,
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )


def main() -> int:
    failures: list[str] = []
    assets = sorted(BLENDER_ROOT.iterdir()) if BLENDER_ROOT.is_dir() else []
    checked = 0
    for sub in assets:
        if not sub.is_dir():
            continue
        aid = sub.name
        png = PREVIEW_DIR / f"{aid}.png"
        if not png.is_file():
            continue
        checked += 1
        std, cov = stats(png)
        write_frame_json(png, aid)
        if std < MIN_STDDEV or cov < MIN_COVERAGE:
            failures.append(f"{aid}: stddev={std:.2f} coverage={cov:.2%}")
    if checked == 0:
        print("OK: no BlenderBaked previews to check")
        return 0
    for line in failures:
        print(f"FAIL preview quality: {line}", file=sys.stderr)
    if failures:
        return 1
    print(f"OK: {checked} preview(s) pass stddev>={MIN_STDDEV} coverage>={MIN_COVERAGE:.0%}")
    return 0


if __name__ == "__main__":
    sys.exit(main())