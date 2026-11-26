#!/usr/bin/env python3
"""Tiny seamless PNG swatches for garage materials (CC0, stdlib only)."""
from __future__ import annotations

import json
import struct
import zlib
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[2]
OUT = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "textures"


def _hex_rgb(h: str) -> tuple[int, int, int]:
    h = h.lstrip("#")
    return int(h[0:2], 16), int(h[2:4], 16), int(h[4:6], 16)


def write_png(path: Path, width: int, height: int, pixel_fn) -> None:
    """Write an RGB PNG without external dependencies."""
    raw_rows = []
    for y in range(height):
        row = b"\x00"
        for x in range(width):
            row += bytes(pixel_fn(x, y))
        raw_rows.append(row)
    compressed = zlib.compress(b"".join(raw_rows), 9)

    def chunk(tag: bytes, data: bytes) -> bytes:
        return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", zlib.crc32(tag + data) & 0xFFFFFFFF)

    ihdr = struct.pack(">IIBBBBB", width, height, 8, 2, 0, 0, 0)
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", ihdr) + chunk(b"IDAT", compressed) + chunk(b"IEND", b""))


def checker(base: tuple[int, int, int], alt: tuple[int, int, int], period: int = 8):
    def fn(x: int, y: int) -> tuple[int, int, int]:
        if ((x // period) + (y // period)) % 2:
            return alt
        return base

    return fn


TEXTURES: dict[str, dict] = {
    "tex-garage-carpet": {"hex": "#B8A890", "noise": "#A89880", "size": 32},
    "tex-garage-concrete": {"hex": "#C8B8A0", "noise": "#B8A890", "size": 32},
    "tex-garage-trim-orange": {"hex": "#A85828", "noise": "#C87830", "size": 16},
    "tex-garage-wall-beige": {"hex": "#D8C8B0", "noise": "#C8B8A0", "size": 32},
    "tex-garage-door-metal": {"hex": "#8A9098", "noise": "#7A8088", "size": 24},
    "tex-garage-server-paint": {"hex": "#6A6E78", "noise": "#5A5E68", "size": 24},
    "tex-hud-paper-warm": {"hex": "#F4F0E8", "noise": "#E8E0D4", "size": 16},
    "tex-crt-phosphor": {"hex": "#66BF73", "noise": "#4A9A58", "size": 16},
}


def main() -> None:
    entries = []
    for asset_id, spec in TEXTURES.items():
        base = _hex_rgb(spec["hex"])
        alt = _hex_rgb(spec["noise"])
        size = spec["size"]
        fn = checker(base, alt, max(4, size // 8))
        out = OUT / f"{asset_id}.png"
        write_png(out, size, size, fn)
        entries.append(
            {
                "id": asset_id,
                "path": f"Assets/Art/EarlyGame/textures/{asset_id}.png",
                "license": "CC0-1.0 (procedural, in-repo)",
                "sizePx": size,
            }
        )
    manifest = REPO_ROOT / "unity" / "Assets" / "Art" / "EarlyGame" / "textures-manifest.json"
    manifest.write_text(
        json.dumps({"generator": "tools/art/generate_early_textures.py", "assets": entries}, indent=2) + "\n",
        encoding="utf-8",
    )
    print(f"Wrote {len(entries)} early textures PNG")


if __name__ == "__main__":
    main()