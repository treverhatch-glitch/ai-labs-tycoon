#!/usr/bin/env python3
"""Print torso gate boxes + coat% for researcher turnaround PNGs (CI / PR #95)."""
from __future__ import annotations

import argparse
import subprocess
import sys
from io import BytesIO
from pathlib import Path

from PIL import Image

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "tools" / "art"))

from researcher_render_pixel_gates import torso_gate_box, torso_gate_boxes_report


def _load_turnaround(sha: str | None, path: Path | None) -> Image.Image:
    if path and path.is_file():
        return Image.open(path).convert("RGB")
    if not sha:
        raise SystemExit("need --sha or --png")
    proc = subprocess.run(
        ["git", "show", f"{sha}:docs/art/previews/staff-researcher-turnaround.png"],
        cwd=REPO,
        capture_output=True,
    )
    if proc.returncode != 0:
        raise SystemExit(f"missing turnaround for {sha}")
    return Image.open(BytesIO(proc.stdout)).convert("RGB")


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--sha", action="append", default=[])
    ap.add_argument("--png", type=Path)
    args = ap.parse_args()
    shas = args.sha or []
    if args.png:
        img = _load_turnaround(None, args.png)
        panels = []
        pw = img.size[0] // 5 if img.size[0] > img.size[1] * 4 else img.size[0] // 4
        gap = 8
        pw = (img.size[0] - gap * 4) // 5 if img.size[0] > img.size[1] * 4 else pw
        for i in range(5 if img.size[0] > img.size[1] * 4 else 4):
            x0 = i * (pw + gap) if img.size[0] > img.size[1] * 4 else i * pw
            panels.append(img.crop((x0, 0, x0 + pw, img.size[1])))
        for row in torso_gate_boxes_report(panels):
            print(row)
        return 0
    for sha in shas:
        img = _load_turnaround(sha, None)
        gap = 8
        pw = (img.size[0] - gap * 4) // 5
        panels = [img.crop((i * (pw + gap), 0, i * (pw + gap) + pw, img.size[1])) for i in range(5)]
        print(f"--- {sha}")
        for row in torso_gate_boxes_report(panels):
            print(row)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())