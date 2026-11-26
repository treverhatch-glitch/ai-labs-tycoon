#!/usr/bin/env python3
"""Hatch composites: staff lineup (8) + wisp mood row."""
from __future__ import annotations

import json
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PREVIEW = REPO / "docs/art/previews"
OUT = REPO / "docs/art/previews"
STAFF_MANIFEST = REPO / "unity/Assets/Art/EarlyGame/staff-manifest.json"
WISP_IDS = ["wisp-happy", "wisp-neutral", "wisp-worried", "wisp-angry", "wisp-glowing"]
BG = (122, 122, 128)


def _row(ids: list[str], out_name: str, cell: int = 256) -> None:
    from PIL import Image

    tiles: list[Image.Image] = []
    for aid in ids:
        p = PREVIEW / f"{aid}.png"
        if not p.exists():
            raise SystemExit(f"FAIL missing {p}")
        tiles.append(Image.open(p).convert("RGB").resize((cell, cell)))
    w, h = cell * len(tiles), cell
    canvas = Image.new("RGB", (w, h), BG)
    for i, t in enumerate(tiles):
        canvas.paste(t, (i * cell, 0))
    path = OUT / out_name
    canvas.save(path)
    print("OK composite", out_name, path)


def main() -> None:
    staff = [a["id"] for a in json.loads(STAFF_MANIFEST.read_text())["assets"]]
    _row(staff, "staff-lineup-packet2.png")
    _row(WISP_IDS, "wisp-moods-row.png")


if __name__ == "__main__":
    main()