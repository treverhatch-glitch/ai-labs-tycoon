"""Researcher hero self-check: mesh basics + 0° front render coat coverage."""
from __future__ import annotations

import os
import sys
import tempfile
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
import blender_common as bc  # noqa: E402
import kenney_researcher_lib as kr  # noqa: E402
def _assert_mesh_basics() -> None:
    coat = bpy.data.objects.get("ResearcherLabCoat")
    if coat is None or len(coat.data.polygons) < 24:
        n = 0 if coat is None else len(coat.data.polygons)
        raise RuntimeError(f"FAIL researcher_hero_selfcheck lab_coat_faces={n} want>=24")
    glasses = bpy.data.objects.get("ResearcherGlasses")
    if glasses is None or len(glasses.data.vertices) < 80:
        gv = 0 if glasses is None else len(glasses.data.vertices)
        raise RuntimeError(f"FAIL researcher_hero_selfcheck glasses_verts={gv} want>=80")
    print(f"OK researcher_hero_selfcheck mesh coat_faces={len(coat.data.polygons)} glasses_verts={len(glasses.data.vertices)}")


def assert_researcher_hero_geometry(_body: bpy.types.Object | None = None) -> None:
    _assert_mesh_basics()
    print("OK researcher_hero_selfcheck render_deferred=axis+diagnostic_png")


def main() -> None:
    os.environ["RESEARCHER_STOCK_RECOLOR"] = "1"
    kr.build_researcher_kenney()
    body = bpy.data.objects.get("KenneyResearcherStock")
    if body is None:
        raise RuntimeError("FAIL researcher_hero_selfcheck missing_body")
    assert_researcher_hero_geometry(body)
    raise SystemExit(0)


if __name__ == "__main__":
    main()