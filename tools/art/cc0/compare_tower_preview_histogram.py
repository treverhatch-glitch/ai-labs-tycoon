#!/usr/bin/env python3
"""Compare tower 3/4 preview histogram to PASS reference (9ea57244)."""
from __future__ import annotations

import struct
import subprocess
import sys
import zlib
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
REF = ROOT / "docs/art/previews/.tower-ref-9ea57244.png"
CUR = ROOT / "docs/art/previews/compute-pc-tower-era1.png"
TOL_MEAN = 12.0
TOL_CORNER = 18.0


def _png_rgba(path: Path) -> list[tuple[int, int, int]]:
    raw = path.read_bytes()
    if raw[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError(f"not png: {path}")
    pos = 8
    width = height = 0
    data = b""
    while pos < len(raw):
        length = struct.unpack(">I", raw[pos : pos + 4])[0]
        pos += 4
        ctype = raw[pos : pos + 4]
        pos += 4
        chunk = raw[pos : pos + length]
        pos += length + 4
        if ctype == b"IHDR":
            width, height = struct.unpack(">II", chunk[:8])
        elif ctype == b"IDAT":
            data += chunk
        elif ctype == b"IEND":
            break
    inflated = zlib.decompress(data)
    stride = width * 4 + 1
    rows = []
    i = 0
    for _ in range(height):
        i += 1
        row = inflated[i : i + width * 4]
        i += width * 4
        rows.append(row)
    px = []
    for y in range(height):
        row = rows[y]
        for x in range(width):
            o = x * 4
            px.append((row[o], row[o + 1], row[o + 2]))
    return px


def _stats(path: Path) -> dict:
    px = _png_rgba(path)
    import math

    w = int(math.sqrt(len(px)))
    corners = [px[2], px[w - 3], px[-w + 2], px[-3]]
    means = [sum(c) / 3 for c in px]
    corner_mean = sum(sum(c) / 3 for c in corners) / 4
    return {
        "mean_luma": sum(means) / len(means),
        "corner_luma": corner_mean,
        "p95": sorted(means)[int(len(means) * 0.95)],
    }


def main() -> int:
    if not REF.is_file():
        subprocess.check_call(
            ["git", "show", "9ea57244:docs/art/previews/compute-pc-tower-era1.png"],
            cwd=ROOT,
            stdout=open(REF, "wb"),
        )
    if not CUR.is_file():
        print(f"FAIL: missing {CUR}", file=sys.stderr)
        return 1
    ref = _stats(REF)
    cur = _stats(CUR)
    dm = abs(ref["mean_luma"] - cur["mean_luma"])
    dc = abs(ref["corner_luma"] - cur["corner_luma"])
    print(f"ref mean={ref['mean_luma']:.1f} corner={ref['corner_luma']:.1f} p95={ref['p95']:.1f}")
    print(f"cur mean={cur['mean_luma']:.1f} corner={cur['corner_luma']:.1f} p95={cur['p95']:.1f}")
    if dm > TOL_MEAN or dc > TOL_CORNER:
        print(f"FAIL: Δmean={dm:.1f} Δcorner={dc:.1f}", file=sys.stderr)
        return 1
    print("OK: tower preview histogram within tolerance of 9ea57244")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())