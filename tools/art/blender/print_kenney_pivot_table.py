#!/usr/bin/env python3
"""Print Kenney stock rig pivot gaps (head vs parent tail, metres)."""
from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import kenney_founder_lib as kf  # noqa: E402
from kenney_rig_pivot_gate import pivot_gap_table  # noqa: E402


def main() -> int:
    arm, _mesh = kf._import_rigged()
    for row in pivot_gap_table(arm):
        print(row)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())