#!/usr/bin/env python3
"""Glowing wisp mascot sprite sheets with 4-frame bob animation (CC0 PNG)."""
from __future__ import annotations

import json
import math
import shutil
import struct
import zlib
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[2]
OUT = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "wisp-moods"
STREAM = REPO_ROOT / "unity/Assets/StreamingAssets/EarlyGame/wisp-moods"
FRAME = 128
FRAMES = 4
SHEET_W = FRAME * FRAMES


MOODS = {
    "wisp-happy": ((140, 102, 242), (102, 191, 115), 0.85),
    "wisp-neutral": ((90, 140, 242), (120, 140, 200), 0.65),
    "wisp-worried": ((242, 191, 90), (180, 120, 80), 0.7),
    "wisp-angry": ((232, 93, 74), (180, 40, 30), 0.9),
    "wisp-glowing": ((176, 128, 255), (140, 220, 255), 1.0),
}


def write_rgba(path: Path, width: int, height: int, pixel_fn) -> None:
    raw_rows = []
    for y in range(height):
        row = b"\x00"
        for x in range(width):
            r, g, b, a = pixel_fn(x, y)
            row += bytes((r, g, b, a))
        raw_rows.append(row)
    compressed = zlib.compress(b"".join(raw_rows), 9)

    def chunk(tag: bytes, data: bytes) -> bytes:
        return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", zlib.crc32(tag + data) & 0xFFFFFFFF)

    ihdr = struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0)
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", ihdr) + chunk(b"IDAT", compressed) + chunk(b"IEND", b""))


def render_sheet(mood_id: str, core: tuple[int, int, int], glow: tuple[int, int, int], intensity: float) -> None:
    def pixel_fn(gx: int, gy: int) -> tuple[int, int, int, int]:
        return 0, 0, 0, 0

    def frame_pixel(fi: int, lx: int, ly: int) -> tuple[int, int, int, int]:
        bob = 4 * math.sin(fi * math.pi / 2)
        cx, cy = FRAME / 2, FRAME / 2 + bob
        dx, dy = lx - cx, ly - cy
        dist = math.sqrt(dx * dx + dy * dy)
        body_r = 22 + 2 * math.sin(fi * 0.8)
        glow_r = 38 + 6 * intensity
        if dist > glow_r:
            return 0, 0, 0, 0
        if dist <= body_r:
            t = dist / body_r
            r = int(core[0] * (1 - t * 0.2))
            g = int(core[1] * (1 - t * 0.2))
            b = int(core[2] * (1 - t * 0.2))
            return r, g, b, 255
        fall = (dist - body_r) / (glow_r - body_r)
        alpha = int((1 - fall) * 180 * intensity)
        r = int(glow[0] * (1 - fall * 0.5))
        g = int(glow[1] * (1 - fall * 0.5))
        b = int(glow[2] * (1 - fall * 0.5))
        return r, g, b, max(0, min(255, alpha))

    def sheet_pixel(x: int, y: int) -> tuple[int, int, int, int]:
        fi = x // FRAME
        lx, ly = x % FRAME, y
        return frame_pixel(fi, lx, ly)

    path = OUT / f"{mood_id}-sheet.png"
    write_rgba(path, SHEET_W, FRAME, sheet_pixel)
    STREAM.mkdir(parents=True, exist_ok=True)
    shutil.copy2(path, STREAM / path.name)

    for fi in range(FRAMES):
        single = OUT / f"{mood_id}_f{fi}.png"

        def pix(sx: int, sy: int, frame_i: int = fi) -> tuple[int, int, int, int]:
            return frame_pixel(frame_i, sx, sy)

        write_rgba(single, FRAME, FRAME, pix)
        shutil.copy2(single, STREAM / single.name)
        preview_dir = REPO_ROOT / "docs/art/previews"
        preview_dir.mkdir(parents=True, exist_ok=True)
        shutil.copy2(single, preview_dir / f"{mood_id}.png")


def main() -> None:
    # remove legacy SVG moods
    for old in OUT.glob("*.svg"):
        old.unlink()
    entries = []
    for mood_id, (core, glow, intensity) in MOODS.items():
        render_sheet(mood_id, core, glow, intensity)
        entries.append(
            {
                "id": mood_id,
                "path": f"Assets/Art/EarlyGame/wisp-moods/{mood_id}-sheet.png",
                "frames": FRAMES,
                "frameSizePx": FRAME,
                "license": "CC0-1.0 (procedural, in-repo)",
                "animation": "bob-glow",
            }
        )
    manifest = REPO_ROOT / "unity/Assets/Art/EarlyGame" / "wisp-manifest.json"
    manifest.write_text(
        json.dumps({"generator": "tools/art/generate_wisp_mascot.py", "fidelity": "packet-2", "assets": entries}, indent=2) + "\n",
        encoding="utf-8",
    )
    print(f"Wrote {len(entries)} wisp sprite sheets ({FRAMES} frames each)")


if __name__ == "__main__":
    main()