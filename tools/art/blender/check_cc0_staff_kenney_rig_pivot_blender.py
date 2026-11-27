#!/usr/bin/env python3
"""Headless Kenney rig pivot gate (founder + researcher stock lane)."""
from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import kenney_founder_lib as kf  # noqa: E402
from kenney_rig_pivot_gate import verify_joint_pivots  # noqa: E402


def main() -> int:
    if "--" not in sys.argv:
        print("FAIL kenney_rig_pivot usage", file=sys.stderr)
        return 1
    repo = Path(sys.argv[sys.argv.index("--") + 1])
    _ = repo
    arm, _mesh = kf._import_rigged()
    ok, msg = verify_joint_pivots(arm)
    asset = "staff-kenney-stock"
    if ok:
        print(f"OK {asset} rig_pivot {msg}")
        return 0
    print(f"FAIL {asset} rig_pivot {msg}")
    return 1


if __name__ == "__main__":
    raise SystemExit(main())