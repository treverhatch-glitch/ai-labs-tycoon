#!/usr/bin/env python3
"""Founder preview only: upright + 70–75% fill."""
from __future__ import annotations

import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "tools/art"))
from check_cc0_staff_preview_upright import analyze  # noqa: E402

PNG = REPO / "docs/art/previews/staff-founder-idle.png"


def main() -> int:
    if not PNG.is_file():
        print("FAIL staff-founder-idle missing preview")
        return 1
    ok, msg = analyze(PNG)
    if not ok:
        print(f"FAIL staff-founder-idle {msg}")
        return 1
    fill = float(msg.split("fill=")[-1])
    if fill < 0.70 or fill > 0.75:
        print(f"FAIL staff-founder-idle fill={fill:.2f} want 0.70-0.75")
        return 1
    print(f"OK staff-founder-idle {msg}")
    return 0


if __name__ == "__main__":
    sys.exit(main())