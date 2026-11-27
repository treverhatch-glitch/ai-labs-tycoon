#!/usr/bin/env python3
"""Assert garage sign front preview OCR reads LAND, FOR, SALE."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "tools/art"))

from packet34_preview_gates import check_sign_front_ocr_words  # noqa: E402

FRONT = REPO / "docs/art/previews/garage-land-for-sale-sign-front.png"


def main() -> int:
    if not FRONT.is_file():
        print(f"FAIL sign_front_ltr missing {FRONT}")
        return 1
    errs = check_sign_front_ocr_words(FRONT)
    if errs:
        print(f"FAIL sign_front_ltr {errs[0]}")
        return 1
    print("OK sign_front_ltr ocr")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())