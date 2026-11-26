"""Headless upright gate (invoked by check_staff_upright.py)."""
from __future__ import annotations

import sys
from pathlib import Path

repo = Path(sys.argv[-1])
sys.path.insert(0, str(Path(__file__).resolve().parent))
import check_staff_gltf_blender as gate  # noqa: E402

raise SystemExit(gate.main())