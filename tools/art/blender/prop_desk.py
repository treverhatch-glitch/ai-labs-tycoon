"""Headless bake: prop-desk."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import _common  # noqa: E402

_common.bake_asset("prop-desk", palette=(0.62, 0.58, 0.52))