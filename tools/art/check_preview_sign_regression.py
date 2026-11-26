#!/usr/bin/env python3
"""Regression: known-bad sign preview commits must fail corner + mirror gates."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "tools" / "art"))

from packet34_preview_gates import (  # noqa: E402
    SIGN_PREVIEW_ALLOWED_PNG,
    check_backdrop_mid_grey,
    check_sign_assembly_metadata,
    check_sign_for_sale_contrast,
    check_sign_front_board_frame_width,
    check_sign_front_ocr_words,
    check_sign_preview_board_text,
    check_sign_preview_subject_framing,
    check_sign_preview_yaw,
    check_sign_text_board_wcag_contrast,
    check_sign_text_inside_board_mask,
    check_sign_text_template_orientation,
    check_sign_front_posts_to_ground,
)

FIXTURES = (
    ("2b7b6e40", REPO / "tools/art/fixtures/preview_regression/sign-2b7b6e40"),
    ("a2c3e44c", REPO / "tools/art/fixtures/preview_regression/sign-a2c3e44c"),
    ("419f4309", REPO / "tools/art/fixtures/preview_regression/sign-419f4309"),
    ("e7d53735", REPO / "tools/art/fixtures/preview_regression/sign-e7d53735"),
    ("dd158f9e", REPO / "tools/art/fixtures/preview_regression/sign-dd158f9e"),
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
    errs.extend(check_sign_preview_subject_framing(rgb, mask, label=label))
    errs.extend(check_sign_text_inside_board_mask(rgb, mask, label=label))
    errs.extend(check_sign_text_board_wcag_contrast(rgb, mask, label=label))
    if "hero" in stem or stem == "for-sale-sign":
        errs.extend(check_sign_for_sale_contrast(rgb, mask))
        errs.extend(check_sign_preview_board_text(rgb, mask, label="sign_hero"))
    else:
        errs.extend(check_sign_preview_board_text(rgb, mask, label="sign_front"))
        errs.extend(check_sign_front_ocr_words(rgb))
        errs.extend(check_sign_front_board_frame_width(rgb, mask))
        errs.extend(check_sign_front_posts_to_ground(rgb, mask))
        frame = root / f"{stem}.frame.json"
        errs.extend(check_sign_preview_yaw(frame, expected_deg=0.0, label="sign_front"))
        errs.extend(check_sign_assembly_metadata(frame, label="sign_front"))
    if "hero" in stem:
        frame = root / f"{stem}.frame.json"
        errs.extend(check_sign_preview_yaw(frame, expected_deg=35.0, label="sign_hero"))
        errs.extend(check_sign_assembly_metadata(frame, label="sign_hero"))
    return _must_fail(sha, f"{stem}", errs)


def _check_scope_manifest(sha: str, root: Path) -> int:
    manifest = root / "preview-png-diff-manifest.txt"
    if not manifest.is_file():
        print(f"FAIL regression missing {manifest}")
        return 1
    names = [ln.strip() for ln in manifest.read_text(encoding="utf-8").splitlines() if ln.strip()]
    errs: list[str] = []
    if len(names) > len(SIGN_PREVIEW_ALLOWED_PNG):
        errs.append(f"sign_preview_png_diff_count={len(names)} max={len(SIGN_PREVIEW_ALLOWED_PNG)}")
    extra = [n for n in names if n not in SIGN_PREVIEW_ALLOWED_PNG]
    if extra:
        errs.append(f"sign_preview_png_out_of_scope extra={extra[:6]}")
    return _must_fail(sha, "preview_scope", errs)


def main() -> int:
    rc = 0
    for sha, root in FIXTURES:
        if not root.is_dir():
            print(f"FAIL regression missing fixture dir {root}")
            return 1
        if sha in ("419f4309", "e7d53735", "dd158f9e"):
            rc |= _check_sign_pair(sha, root, "garage-land-for-sale-sign-front", "sign_front")
            rc |= _check_sign_pair(sha, root, "garage-land-for-sale-sign-hero", "sign_hero")
            if sha == "419f4309":
                rc |= _check_scope_manifest(sha, root)
            continue
        rc |= _check_sign_pair(sha, root, "for-sale-sign", "sign_hero")
        rc |= _check_sign_pair(sha, root, "garage-land-for-sale-sign-front", "sign_front")
    return rc


if __name__ == "__main__":
    raise SystemExit(main())