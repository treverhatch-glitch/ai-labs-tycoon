#!/usr/bin/env python3
"""Grey stock Kenney idle check frame (unmodified proportions) before colour restyle."""
from __future__ import annotations

import os
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402
import kenney_founder_lib as kf  # noqa: E402
from kenney_founder_stock_recolor import export_stock_glb  # noqa: E402


def main() -> None:
    baked = kf.bake_stock_kenney_mesh()
    export_stock_glb(baked)
    out = bc.REPO_ROOT / "docs/art/previews/staff-founder-kenney-stock-check.png"
    os.environ["STAFF_PREVIEW_ASSET"] = "staff-founder-kenney-stock-check"
    os.environ["EARLY_ART_PREVIEW_RIG"] = "1"
    bc.render_preview([baked], out, asset_id="staff-founder-kenney-stock-check")
    print(f"OK stock_check wrote {out}")
    raise SystemExit(0)


if __name__ == "__main__":
    main()