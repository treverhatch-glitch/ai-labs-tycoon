"""Headless bake: prop-server-rack."""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import _common  # noqa: E402

_common.bake_asset("prop-server-rack", palette=(0.35, 0.38, 0.42))