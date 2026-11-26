#!/usr/bin/env python3
"""Founder: camera faces character + face pixels present in PNG."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
AID = "staff-founder-idle"
PNG = REPO / "docs/art/previews" / f"{AID}.png"
BLENDER = Path(__import__("os").environ.get("BLENDER_BIN", "blender"))
CAM_SCRIPT = REPO / "tools/art/blender/check_cc0_staff_face_camera_blender.py"


def _png_face_pixels() -> tuple[bool, str]:
    from PIL import Image

    mask_path = PNG.with_name(PNG.stem + ".mask.png")
    if not mask_path.is_file():
        return False, "no_mask"
    img = Image.open(PNG).convert("RGBA")
    mask = Image.open(mask_path).convert("L")
    w, h = img.size
    ys = [y for y in range(h) for x in range(w) if mask.getpixel((x, y)) > 48]
    if not ys:
        return False, "empty_mask"
    min_y, max_y = min(ys), max(ys)
    span = max(1, max_y - min_y)
    top_lo = max_y - int(span * 0.22)
    x_lo, x_hi = int(w * 0.34), int(w * 0.66)
    band = 0
    for y in range(top_lo, max_y):
        for x in range(x_lo, x_hi):
            if mask.getpixel((x, y)) <= 48:
                continue
            band += 1
    if band < 120:
        return False, f"face_band_px={band}"
    return True, f"face_band_px={band}"


def main() -> int:
    if not PNG.is_file():
        print(f"FAIL {AID} missing preview")
        return 1
    proc = subprocess.run(
        [str(BLENDER), "-b", "--python", str(CAM_SCRIPT), "--", str(REPO)],
        cwd=REPO,
        text=True,
        capture_output=True,
    )
    printed = False
    for line in (proc.stdout or "").splitlines():
        if line.startswith("OK ") or line.startswith("FAIL "):
            print(line)
            printed = True
    if proc.returncode != 0:
        if not printed and proc.stderr:
            print(proc.stderr, file=sys.stderr)
        return 1
    ok, msg = _png_face_pixels()
    if not ok:
        print(f"FAIL {AID} {msg}")
        return 1
    print(f"OK {AID} {msg}")
    return 0


if __name__ == "__main__":
    sys.exit(main())