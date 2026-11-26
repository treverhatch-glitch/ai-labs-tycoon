#!/usr/bin/env python3
"""Fail near-uniform / low-chroma preview PNGs for BlenderBaked gate assets."""
from __future__ import annotations

import hashlib
import json
import struct
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW_DIR = REPO / "docs" / "art" / "previews"
BLENDER_ROOT = REPO / "unity" / "Assets" / "Art" / "BlenderBaked"

GATED_ASSETS = (
    "prop-desk",
    "prop-server-rack",
    "prop-whiteboard",
    "ui-rival-badge",
)

MIN_STDDEV = 8.0
MIN_COVERAGE = 0.20
MIN_MEAN_LUM = 40.0
MIN_MEAN_SAT = 0.10


def _read_png_rgb(path: Path) -> tuple[int, int, list[tuple[int, int, int]]]:
    import zlib

    data = path.read_bytes()
    if data[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError(f"not PNG: {path}")
    pos = 8
    width = height = None
    color_type = None
    raw = b""
    while pos < len(data):
        length = struct.unpack(">I", data[pos : pos + 4])[0]
        pos += 4
        ctype = data[pos : pos + 4]
        pos += 4
        chunk = data[pos : pos + length]
        pos += length + 4
        if ctype == b"IHDR":
            width, height, bitdepth, color_type = struct.unpack(">IIBB", chunk[:10])[:4]
        elif ctype == b"IDAT":
            raw += chunk
        elif ctype == b"IEND":
            break
    if width is None or height is None:
        raise ValueError(f"no IHDR: {path}")
    bpp = {0: 1, 2: 3, 3: 1, 4: 2, 6: 4}.get(color_type or 2, 3)
    inflated = zlib.decompress(raw)
    row_bpp = width * bpp
    stride = row_bpp + 1
    pixels: list[tuple[int, int, int]] = []
    offset = 0
    for y in range(height):
        row = inflated[offset : offset + stride]
        offset += stride
        if not row:
            break
        body = row[1:]
        for x in range(width):
            i = x * bpp
            if bpp >= 3:
                pixels.append((body[i], body[i + 1], body[i + 2]))
            else:
                g = body[i]
                pixels.append((g, g, g))
    return width, height, pixels


def stats(path: Path) -> dict[str, float]:
    width, height, pixels = _read_png_rgb(path)
    if not pixels:
        return {"stddev": 0.0, "coverage": 0.0, "mean_lum": 0.0, "mean_sat": 0.0}
    lums: list[int] = []
    sats: list[float] = []
    for r, g, b in pixels:
        lums.append((r * 299 + g * 587 + b * 114) // 1000)
        mx, mn = max(r, g, b), min(r, g, b)
        sats.append((mx - mn) / 255.0 if mx else 0.0)
    n = len(lums)
    mean = sum(lums) / n
    var = sum((x - mean) ** 2 for x in lums) / n
    std = var**0.5
    coverage = sum(1 for x in lums if x > 24) / n
    # Chroma gate on center crop (excludes grey backdrop; catches black subject mass).
    mx0 = int(width * 0.2)
    mx1 = int(width * 0.8)
    my0 = int(height * 0.15)
    my1 = int(height * 0.85)
    center_lum: list[int] = []
    center_sat: list[float] = []
    for y in range(height):
        for x in range(width):
            if mx0 <= x < mx1 and my0 <= y < my1:
                i = y * width + x
                center_lum.append(lums[i])
                center_sat.append(sats[i])
    cn = len(center_lum) or 1
    return {
        "stddev": std,
        "coverage": coverage,
        "mean_lum": sum(center_lum) / cn,
        "mean_sat": sum(center_sat) / cn,
    }


def gate_fail(s: dict[str, float]) -> list[str]:
    reasons: list[str] = []
    if s["stddev"] < MIN_STDDEV or s["coverage"] < MIN_COVERAGE:
        reasons.append(
            f"uniform stddev={s['stddev']:.2f} coverage={s['coverage']:.1%}"
        )
    if s["mean_lum"] < MIN_MEAN_LUM or s["mean_sat"] < MIN_MEAN_SAT:
        reasons.append(
            f"chroma mean_lum={s['mean_lum']:.1f} mean_sat={s['mean_sat']:.3f}"
        )
    return reasons


def glb_sha256(asset_id: str) -> str | None:
    glb = BLENDER_ROOT / asset_id / f"mesh_{asset_id}.glb"
    if not glb.is_file():
        return None
    return hashlib.sha256(glb.read_bytes()).hexdigest()


def write_frame_json(preview: Path, asset_id: str, s: dict[str, float]) -> None:
    sha = glb_sha256(asset_id)
    frame = PREVIEW_DIR / f"{asset_id}.frame.json"
    frame.write_text(
        json.dumps(
            {
                "assetId": asset_id,
                "preview": str(preview.relative_to(REPO)),
                "glb_sha256": sha,
                "resolution": 1024,
                "quality": s,
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )


def main() -> int:
    failures: list[str] = []
    for aid in GATED_ASSETS:
        png = PREVIEW_DIR / f"{aid}.png"
        if not png.is_file():
            failures.append(f"{aid}: missing preview")
            continue
        s = stats(png)
        write_frame_json(png, aid, s)
        bad = gate_fail(s)
        if bad:
            failures.append(f"{aid}: " + "; ".join(bad))
    if failures:
        for line in failures:
            print(f"FAIL preview quality: {line}", file=sys.stderr)
        return 1
    print(f"OK: {len(GATED_ASSETS)} gated preview(s) pass stddev/coverage + luminance/saturation")
    return 0


if __name__ == "__main__":
    sys.exit(main())