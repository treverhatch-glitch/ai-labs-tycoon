#!/usr/bin/env python3
"""Run sign front/hero gates on exact git-committed PNG bytes (default HEAD)."""
from __future__ import annotations

import subprocess
import sys
import tempfile
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "tools" / "art"))
from committed_preview_io import git_show_bytes  # noqa: E402
from packet34_preview_gates import (  # noqa: E402
    check_backdrop_mid_grey,
    check_sign_contact_shadow_under_base,
    check_sign_front_board_frame_width,
    check_sign_front_ocr_words,
    check_sign_mask_studio_backdrop,
    check_sign_post_wood_hue,
    check_sign_preview_subject_framing,
    check_sign_subject_fill_extent,
    check_sign_text_board_wcag_contrast,
)

SIGN_STEMS = ("garage-land-for-sale-sign-front", "garage-land-for-sale-sign-hero")


def _git_head(repo: Path = REPO) -> str:
    return subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=repo, text=True).strip()


def _materialize(rev: str, rel: str) -> Path:
    data = git_show_bytes(rev, rel)
    tmp = tempfile.NamedTemporaryFile(suffix=Path(rel).name, delete=False)
    tmp.write(data)
    tmp.close()
    return Path(tmp.name)


def verify(
    *,
    rev: str | None = None,
    verbose: bool = True,
    use_git: bool = True,
) -> tuple[int, list[str]]:
    rev = rev or _git_head()
    lines: list[str] = []
    rc = 0
    for stem in SIGN_STEMS:
        rgb_rel = f"docs/art/previews/{stem}.png"
        mask_rel = f"docs/art/previews/{stem}.mask.png"
        if use_git:
            rgb = _materialize(rev, rgb_rel)
            mask = _materialize(rev, mask_rel)
        else:
            rgb = REPO / rgb_rel
            mask = REPO / mask_rel
        label = "sign_front" if "front" in stem else "sign_hero"
        checks: list[tuple[str, list[str]]] = [
            ("backdrop", check_backdrop_mid_grey(rgb)),
            ("mask_backdrop", check_sign_mask_studio_backdrop(mask)),
            ("fill", check_sign_subject_fill_extent(rgb, mask, label=label)),
            ("wood_hue", check_sign_post_wood_hue(rgb, mask, label=label)),
            ("contact", check_sign_contact_shadow_under_base(rgb, mask, label=label)),
            ("framing", check_sign_preview_subject_framing(rgb, mask, label=label)),
            ("wcag", check_sign_text_board_wcag_contrast(rgb, mask, label=label)),
        ]
        if "front" in stem:
            checks.insert(2, ("board_width", check_sign_front_board_frame_width(rgb, mask)))
            checks.append(("ocr", check_sign_front_ocr_words(rgb)))
        else:
            checks.insert(2, ("board_width", check_sign_front_board_frame_width(rgb, mask)))
        for name, errs in checks:
            status = "OK" if not errs else "FAIL"
            detail = errs[0] if errs else ""
            line = f"{status} {stem} {name} {detail}"
            lines.append(line)
            if errs:
                rc = 1
            if verbose:
                print(line)
    lines.insert(0, f"verify_sign_preview_committed rev={rev}")
    if verbose and rc == 0:
        print("OK sign_preview_committed all gates")
    return rc, lines


def main() -> int:
    use_git = "--working-tree" not in sys.argv
    rc, _ = verify(verbose=True, use_git=use_git)
    return rc


if __name__ == "__main__":
    raise SystemExit(main())