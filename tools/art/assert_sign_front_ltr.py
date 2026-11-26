#!/usr/bin/env python3
"""Assert garage sign front preview reads FOR SALE left-to-right (F on the left)."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
FRONT = REPO / "docs/art/previews/garage-land-for-sale-sign-front.png"


def _is_red(r: int, g: int, b: int) -> bool:
    return r > 130 and r > g + 24 and r > b + 24


def _ocr_starts_with_f(path: Path) -> bool | None:
    try:
        import pytesseract  # type: ignore
        from PIL import Image
    except ImportError:
        return None
    text = pytesseract.image_to_string(Image.open(path).convert("RGB"), config="--psm 7").strip().upper()
    if not text:
        return False
    return text[0] == "F" or text.startswith("FOR")


def _red_leftmost_is_f(path: Path) -> bool:
    from PIL import Image

    im = Image.open(path).convert("RGB")
    w, h = im.size
    px = im.load()
    cols: dict[int, int] = {}
    for y in range(h):
        for x in range(w):
            r, g, b = px[x, y]
            if _is_red(r, g, b):
                cols[x] = cols.get(x, 0) + 1
    if len(cols) < 12:
        return False
    xs = sorted(cols)
    rx0, rx1 = min(xs), max(xs)
    span = max(1, rx1 - rx0)
    left_mass = sum(cols[x] for x in xs if x <= rx0 + int(span * 0.22))
    right_mass = sum(cols[x] for x in xs if x >= rx1 - int(span * 0.22))
    # Mirrored: heavy strokes land on the right first (E/L); LTR FOR has F weight on the left.
    return left_mass >= right_mass * 0.72


def main() -> int:
    if not FRONT.is_file():
        print(f"FAIL sign_front_ltr missing {FRONT}")
        return 1
    ocr = _ocr_starts_with_f(FRONT)
    if ocr is True:
        print("OK sign_front_ltr ocr")
        return 0
    if ocr is False:
        print("FAIL sign_front_ltr ocr_first_char")
        return 1
    if _red_leftmost_is_f(FRONT):
        print("OK sign_front_ltr red_bbox")
        return 0
    print("FAIL sign_front_ltr red_bbox")
    return 1


if __name__ == "__main__":
    raise SystemExit(main())