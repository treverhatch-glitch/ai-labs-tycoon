"""Headless bake: prop-whiteboard."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import _common  # noqa: E402

_common.bake_asset("prop-whiteboard", palette=(0.95, 0.96, 0.98))