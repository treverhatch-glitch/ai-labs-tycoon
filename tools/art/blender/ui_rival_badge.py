"""Headless bake: ui-rival-badge."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import _common  # noqa: E402

_common.bake_asset("ui-rival-badge", palette=(0.92, 0.45, 0.52))