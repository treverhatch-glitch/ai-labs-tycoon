#!/usr/bin/env python3
"""Grey stock Kenney idle check frame (unmodified proportions) before colour restyle."""
from __future__ import annotations

import os
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import kenney_founder_lib as kf  # noqa: E402
from kenney_founder_standing_gate import rig_head_foot_world_z  # noqa: E402


def main() -> None:
    arm, mesh = kf._import_rigged()
    kf._pose_kenney_idle_arms(arm)
    kf._ground_rig(arm, mesh)
    refs = rig_head_foot_world_z(arm)
    if not refs:
        raise RuntimeError("FAIL stock_check rig_head_foot_missing")
    head_ref, foot_ref = refs
    baked = kf._bake_mesh(mesh, arm, head_ref, foot_ref)
    kf._ensure_upright_z(baked, head_ref, foot_ref)
    kf._scale_character(baked)
    bc.assign_solid(baked, (0.58, 0.58, 0.58, 1.0), 0.62)
    out = bc.REPO_ROOT / "docs/art/previews/staff-founder-kenney-stock-check.png"
    os.environ["STAFF_PREVIEW_ASSET"] = "staff-founder-kenney-stock-check"
    os.environ["EARLY_ART_PREVIEW_RIG"] = "1"
    bc.render_preview([baked], out, asset_id="staff-founder-kenney-stock-check")
    print(f"OK stock_check wrote {out}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()