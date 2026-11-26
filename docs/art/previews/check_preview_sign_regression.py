#!/usr/bin/env python3
"""Regression: known-bad sign preview commits must fail corner + mirror gates."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "tools" / "art"))

from packet34_preview_gates import (  # noqa: E402
    check_backdrop_mid_grey,
    check_sign_for_sale_contrast,
    check_sign_preview_board_text,
    check_sign_text_template_orientation,
)

FIXTURES = (
    ("2b7b6e40", REPO / "tools/art/fixtures/preview_regression/sign-2b7b6e40"),
    ("a2c3e44c", REPO / "tools/art/fixtures/preview_regression/sign-a2c3e44c"),
)


def _must_fail(sha: str, label: str, errs: list[str]) -> int:
    if not errs:
        print(f"FAIL regression {label} expected_gate_errors on fixture {sha}")
        return 1
    print(f"OK regression {label} fixture {sha} rejected ({errs[0]})")
    return 0


def _check_sign_pair(sha: str, root: Path, stem: str, label: str) -> int:
    rgb = root / f"{stem}.png"
    mask = root / f"{stem}.mask.png"
    if not rgb.is_file() or not mask.is_file():
        print(f"FAIL regression missing fixture {rgb}")
        return 1
    errs: list[str] = []
    errs.extend(check_backdrop_mid_grey(rgb))
    errs.extend(check_sign_text_template_orientation(rgb, mask, label=label))
    if stem == "for-sale-sign":
        errs.extend(check_sign_for_sale_contrast(rgb, mask))
        errs.extend(check_sign_preview_board_text(rgb, mask, label="sign_hero"))
    else:
        errs.extend(check_sign_preview_board_text(rgb, mask, label="sign_front"))
    return _must_fail(sha, f"{stem}", errs)


def main() -> int:
    rc = 0
    for sha, root in FIXTURES:
        if not root.is_dir():
            print(f"FAIL regression missing fixture dir {root}")
            return 1
        rc |= _check_sign_pair(sha, root, "for-sale-sign", "sign_hero")
        rc |= _check_sign_pair(sha, root, "garage-land-for-sale-sign-front", "sign_front")
    return rc


if __name__ == "__main__":
    raise SystemExit(main())