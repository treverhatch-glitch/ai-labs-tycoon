"""Value noise and FBM for tileable textures (stdlib, CC0)."""
from __future__ import annotations

import hashlib
import math
import struct
import zlib
from pathlib import Path


def _hash2(x: int, y: int, seed: int) -> float:
    h = hashlib.sha256(f"{seed}:{x}:{y}".encode()).digest()
    return int.from_bytes(h[:2], "big") / 65535.0


def value_noise(x: float, y: float, seed: int = 0) -> float:
    x0, y0 = int(math.floor(x)), int(math.floor(y))
    tx, ty = x - x0, y - y0
    v00, v10 = _hash2(x0, y0, seed), _hash2(x0 + 1, y0, seed)
    v01, v11 = _hash2(x0, y0 + 1, seed), _hash2(x0 + 1, y0 + 1, seed)
    sx = tx * tx * (3 - 2 * tx)
    sy = ty * ty * (3 - 2 * ty)
    a = v00 + (v10 - v00) * sx
    b = v01 + (v11 - v01) * sx
    return a + (b - a) * sy


def fbm(x: float, y: float, octaves: int = 5, seed: int = 0) -> float:
    amp, freq, acc = 1.0, 1.0, 0.0
    norm = 0.0
    for i in range(octaves):
        acc += amp * value_noise(x * freq, y * freq, seed + i * 17)
        norm += amp
        amp *= 0.5
        freq *= 2.1
    return acc / norm


def height_carpet(x: float, y: float) -> float:
    return 0.55 + 0.25 * fbm(x * 8, y * 8, 4, 1) + 0.1 * math.sin(x * 40) * math.sin(y * 3)


def height_concrete(x: float, y: float) -> float:
    stains = max(0, fbm(x * 3, y * 3, 3, 2) - 0.55) * 0.4
    return 0.6 + 0.3 * fbm(x * 6, y * 6, 5, 3) - stains


def height_metal(x: float, y: float) -> float:
    scratch = abs(math.sin(x * 120 + y * 4)) * 0.15
    return 0.5 + 0.2 * fbm(x * 12, y * 12, 4, 4) - scratch


def height_wood(x: float, y: float) -> float:
    grain = 0.15 * math.sin(y * 35 + fbm(x, y, 2, 5) * 2)
    return 0.5 + grain + 0.08 * fbm(x * 4, y * 18, 3, 6)


def write_png_rgb(path: Path, width: int, height: int, rgb_fn) -> None:
    raw_rows = []
    for y in range(height):
        row = b"\x00"
        for x in range(width):
            r, g, b = rgb_fn(x / width, y / height, x, y)
            row += bytes((max(0, min(255, int(r))), max(0, min(255, int(g))), max(0, min(255, int(b)))))
        raw_rows.append(row)
    compressed = zlib.compress(b"".join(raw_rows), 9)

    def chunk(tag: bytes, data: bytes) -> bytes:
        return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", zlib.crc32(tag + data) & 0xFFFFFFFF)

    ihdr = struct.pack(">IIBBBBB", width, height, 8, 2, 0, 0, 0)
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(b"\x89PNG\r\n\x1a\n" + chunk(b"IHDR", ihdr) + chunk(b"IDAT", compressed) + chunk(b"IEND", b""))


def write_normal_map(path: Path, width: int, height: int, height_fn, strength: float = 2.5) -> None:
    def rgb_fn(_nx, _ny, x, y):
        xu = (x + 1) % width
        yu = (y + 1) % height
        h = height_fn(x / width, y / height)
        hx = height_fn(xu / width, y / height) - h
        hy = height_fn(x / width, yu / height) - h
        nx, ny, nz = -hx * strength, -hy * strength, 1.0
        ln = math.sqrt(nx * nx + ny * ny + nz * nz) or 1.0
        return (
            (nx / ln * 0.5 + 0.5) * 255,
            (ny / ln * 0.5 + 0.5) * 255,
            (nz / ln * 0.5 + 0.5) * 255,
        )

    write_png_rgb(path, width, height, rgb_fn)