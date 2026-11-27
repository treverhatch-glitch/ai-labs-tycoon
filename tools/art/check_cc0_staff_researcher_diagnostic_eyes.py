#!/usr/bin/env python3
"""0° diagnostic must show two dark eye blobs in head face band."""
from __future__ import annotations

import sys
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
PNG = REPO / "docs/art/previews/staff-researcher-face-diagnostic-0deg.png"

sys.path.insert(0, str(REPO / "tools" / "art"))
from staff_preview_eye_head_gate import check_eyes_in_head_mask  # noqa: E402


def main() -> int:
    mask_path = PNG.with_name(PNG.stem + ".mask.png")
    if not PNG.is_file() or not mask_path.is_file():
        print("FAIL staff-researcher-idle diagnostic_eyes missing_preview")
        return 1
    img = Image.open(PNG).convert("RGB")
    mask = Image.open(mask_path).convert("L")
    ok, msg, blobs, _ = check_eyes_in_head_mask(img, mask, allow_projection_fallback=False)
    if ok and len(blobs) < 2:
        print(f"FAIL staff-researcher-idle diagnostic_eyes face_blobs={len(blobs)} need_2")
        return 1
    if ok:
        print(f"OK staff-researcher-idle diagnostic_eyes {msg}")
        return 0
    print(f"FAIL staff-researcher-idle diagnostic_eyes {msg}")
    return 1


if __name__ == "__main__":
    sys.exit(main())