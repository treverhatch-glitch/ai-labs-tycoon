#!/usr/bin/env python3
"""Researcher idle preview: two eye blobs in head face band."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-researcher-idle.png"

sys.path.insert(0, str(REPO / "tools" / "art"))
from staff_preview_eye_head_gate import check_eyes_in_head_mask  # noqa: E402


def main() -> int:
    mask_path = PNG.with_name(PNG.stem + ".mask.png")
    if not PNG.is_file() or not mask_path.is_file():
        print("FAIL staff-researcher-idle eye_blobs missing_preview")
        return 1
    from PIL import Image as PilImage

    img = PilImage.open(PNG).convert("RGB")
    mask = PilImage.open(mask_path).convert("L")
    ok, msg, _, _ = check_eyes_in_head_mask(
        img,
        mask,
        allow_projection_fallback=True,
        yaw_deg=30.0,
    )
    if ok:
        print(f"OK staff-researcher-idle eye_blobs {msg}")
        return 0
    print(f"FAIL staff-researcher-idle eye_blobs {msg}")
    return 1


if __name__ == "__main__":
    sys.exit(main())