#!/usr/bin/env python3
"""Regression: known-bad sign previews (e.g. 2b7b6e40) must fail current gates."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "tools" / "art"))

from packet34_preview_gates import (  # noqa: E402
    check_backdrop_mid_grey,
    check_sign_for_sale_contrast,
    check_sign_preview_board_text,
)

FIXTURE_ROOT = REPO / "tools/art/fixtures/preview_regression/sign-2b7b6e40"
BAD_SHA = "2b7b6e40"


def _must_fail(label: str, errs: list[str]) -> int:
    if not errs:
        print(f"FAIL regression {label} expected_gate_errors on fixture {BAD_SHA}")
        return 1
    print(f"OK regression {label} fixture {BAD_SHA} rejected ({errs[0]})")
    return 0


def main() -> int:
    rc = 0
    hero = FIXTURE_ROOT / "for-sale-sign.png"
    hero_mask = FIXTURE_ROOT / "for-sale-sign.mask.png"
    front = FIXTURE_ROOT / "garage-land-for-sale-sign-front.png"
    front_mask = FIXTURE_ROOT / "garage-land-for-sale-sign-front.mask.png"
    for path in (hero, hero_mask, front, front_mask):
        if not path.is_file():
            print(f"FAIL regression missing fixture {path}")
            return 1
    errs = []
    errs.extend(check_backdrop_mid_grey(hero))
    errs.extend(check_sign_for_sale_contrast(hero, hero_mask))
    errs.extend(check_sign_preview_board_text(hero, hero_mask, label="sign_hero"))
    rc |= _must_fail("for-sale-sign", errs)

    errs = []
    errs.extend(check_backdrop_mid_grey(front))
    errs.extend(check_sign_preview_board_text(front, front_mask, label="sign_front"))
    rc |= _must_fail("garage-land-for-sale-sign-front", errs)
    return rc


if __name__ == "__main__":
    sys.exit(main())